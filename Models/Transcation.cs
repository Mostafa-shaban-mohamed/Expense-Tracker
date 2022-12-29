using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Expense_Tracker.Models;

public class Transcation
{

    [Key]
    public int TransId { get; set; }

    //Category Id
    //Navigation Property
    public Category? Category { get; set; }
    public int CategoryId { get; set; }
    public int Amount { get; set; }

    [Column(TypeName = "nvarchar(75)")]
    public string? Note { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;

    [NotMapped]
    public string? CategoryTitleWithIcon
    {
        get
        {
            return Category == null ? "" : Category.Icon + " " + Category.Title;
        }
    }

    [NotMapped]
    public string? FormattedAmount
    {
        get
        {
            return ((Category == null || Category.Type == "Expense") ? "- " : "+ ") + Amount.ToString("C0");
        }
    }
}