using System;
using System.Linq;
using System.Threading.Tasks;
using ExpenseTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace ExpenseTracker.Controllers
{
    public class ExpenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Expense
        public async Task<IActionResult> Index()
        {
            return View(await _context.Expenses.OrderByDescending(e => e.Date).ToListAsync());
        }
       


        // GET: Expense/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            if (id == 0)
                return View(new Expense());
            else
                return View(_context.Expenses.Find(id));
        }

        // POST: Expense/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,Title,Amount,Category,Date,Description")] Expense expense)
        {
            if (ModelState.IsValid)
            {
                if (expense.Id == 0)
                    _context.Add(expense);
                else
                    _context.Update(expense);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expense);
        }

        // POST: Expense/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
            {
                return NotFound();
            }

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Summary()
        {
            var categoryTotals = await _context.Expenses
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
                .ToDictionaryAsync(x => x.Category, x => x.Total);

            var monthlyTotals = await _context.Expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Total = g.Sum(e => e.Amount)
                })
                .ToDictionaryAsync(x => x.Month, x => x.Total);

            var viewModel = new ExpenseSummaryViewModel
            {
                CategoryTotals = categoryTotals,
                MonthlyTotals = monthlyTotals
            };

            return View(viewModel);
        }

        public IActionResult Homepage()
        {
            return View();
        }
    }
}