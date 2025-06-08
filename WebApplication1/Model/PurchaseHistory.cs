using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Model;

public class PurchaseHistory
{
    [Required]
    public int AvailableProgramId { get; set; }
    [Required]
    public int CustomerId { get; set; }
    [Required]
    [Column(TypeName = "datetime")]
    public DateTime PurchaseDate { get; set; }
    public int? Rating { get; set; }
    
    public virtual AvailableProgram AvailableProgram { get; set; }
    public virtual Customer Customer { get; set; }
}