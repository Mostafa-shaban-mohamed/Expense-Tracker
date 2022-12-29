using System.Globalization;
using Expense_Tracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Expense_Tracker.Controllers;

public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index(){
        //last 10 days transactions
        DateTime StartDate = DateTime.Today.AddDays(-10);
        DateTime EndDate = DateTime.Today;

        List<Transcation> SelectedTransactions = await _context.Transcations
            .Include(x => x.Category)
            .Where(y => y.Date >= StartDate && y.Date <= EndDate).ToListAsync();

        //Total Income
        int TotalIncome = SelectedTransactions.Where(y => y.Category.Type == "Income").Sum(j => j.Amount);
        ViewBag.TotalIncome = TotalIncome.ToString("C0");

        //Total Expense
        int TotalExpense = SelectedTransactions.Where(y => y.Category.Type == "Expense").Sum(j => j.Amount);
        ViewBag.TotalExpense = TotalExpense.ToString("C0");

        //Balance
        int Balance = TotalIncome - TotalExpense;
        CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
        culture.NumberFormat.CurrencyNegativePattern = 1;
        ViewBag.Balance = String.Format(culture, "{0:C0}", Balance);

        //Doughnut Chart - Expense by Category
        ViewBag.DoughnutChartData = SelectedTransactions.Where(x => x.Category.Type == "Expense")
            .GroupBy(j => j.CategoryId)
            .Select(m => new{
                CategoryTitleWithIcon = m.First().Category.Icon + " " + m.First().Category.Title,
                amount = m.Sum(j => j.Amount),
                formattedAmount = m.Sum(j => j.Amount).ToString("C0")
            }).OrderByDescending(l => l.amount).ToList();
        

        //spline chart: Income vs Expense
        //Income
        List<SplineChartData> incomeSummary = SelectedTransactions.Where(i => i.Category.Type == "Income")
            .GroupBy(i => i.Date)
            .Select(i => new SplineChartData(){
                weeks = i.First().Date.ToString("dd-MMM"),
                income = i.Sum(l => l.Amount)
            }).ToList();

            //Expense
        List<SplineChartData> expenseSummary = SelectedTransactions.Where(i => i.Category.Type == "Expense")
            .GroupBy(i => i.Date)
            .Select(i => new SplineChartData(){
                weeks = i.First().Date.ToString("dd-MMM"),
                expense = i.Sum(l => l.Amount)
            }).ToList();
        //combine Income & Expense
        string[] last10Days = Enumerable.Range(0,10).Select(i => StartDate.AddDays(i).ToString("dd-MMM")).ToArray();

        ViewBag.SplineChartData = from week in last10Days
                                    join income in incomeSummary on week equals income.weeks into dayIncomejoined
                                    from income in dayIncomejoined.DefaultIfEmpty()
                                    join expense in expenseSummary on week equals expense.weeks into expensejoined
                                    from expense in expensejoined.DefaultIfEmpty()
                                    select new {
                                        week = week,
                                        income = income == null ? 0 : income.income,
                                        expense = expense == null ? 0 : expense.expense
                                    };

        //recent transactions
        ViewBag.RecentTrans = await _context.Transcations.Include(i => i.Category)
            .OrderByDescending(i => i.Date).Take(3).ToListAsync();
        return View();
    }
}

public class SplineChartData {
    public string weeks;
    public int income;
    public int expense;
}