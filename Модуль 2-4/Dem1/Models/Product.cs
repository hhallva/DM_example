using System.IO;

namespace Dem1.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public decimal Price { get; set; }

    public int ProviderId { get; set; }

    public int ManufacturerId { get; set; }

    public int CategoryId { get; set; }

    public int Discount { get; set; }

    public int Amount { get; set; }

    public string Desctription { get; set; } = null!;

    public string? Photo { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual Manufacturer Manufacturer { get; set; } = null!;

    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    public virtual Provider Provider { get; set; } = null!;


    public string PhotoPath
    {
        get
        {
            string root = Environment.CurrentDirectory;
            string path = string.IsNullOrWhiteSpace(Photo) 
                ? Path.Combine(root, "Images", "picture.png")
                : Path.Combine(root, "Images", Photo);

            return path;
        }
    }

    public bool IsBigDiscount => Discount > 15;

    public bool HasDiscount => Discount > 0;

    public decimal DicscountedPrice => Price * (1 - Discount / 100M);



}
