using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Model;

public class Customer
{
    public int CustomerId { get; set; }
    [Required, MaxLength(50)]
    [Column(TypeName = "nvarchar(50)")]
    public string FirstName { get; set; }
    [Required, MaxLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string LastName { get; set; }
    [MaxLength(100)]
    [Column(TypeName = "nvarchar(100)")]
    public string PhoneNumber { get; set; }
    
    public virtual ICollection<PurchaseHistory> PurchaseHistory { get; set; }
}