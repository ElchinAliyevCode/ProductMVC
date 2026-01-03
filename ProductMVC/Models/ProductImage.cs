using System.ComponentModel.DataAnnotations;

namespace ProductMVC.Models;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; }

    [Required]
    [MaxLength(512)]
    public string ImageUrl { get; set; }
}
