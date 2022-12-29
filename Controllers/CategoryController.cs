using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers;

public class CategoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public CategoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Category
    public async Task<IActionResult> Index()
    {
        return _context.Categories != null ?
                    View(await _context.Categories.ToListAsync()) :
                    Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
    }

    //we created Create and Update methods in the same method and called it "AddOrEdit" method
    [HttpGet]
    public IActionResult AddOrEdit(int id = 0)
    {
        if (id == 0) //Create
            return View(new Category());
        else //Edit
            return View(_context.Categories.Find(id));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddOrEdit(Category category)
    {
        if (ModelState.IsValid)
        {
            if (category.Id == 0) //Create
                _context.Add(category);
            else //Edit
                _context.Update(category);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(category);
    }

    // POST: Category/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        if (_context.Categories == null)
        {
            return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
        }
        var category = await _context.Categories.FindAsync(id);
        if (category != null)
        {
            _context.Categories.Remove(category);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}

