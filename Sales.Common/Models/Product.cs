
namespace Sales.Common.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        [Required]
        public string Description { get; set; }
        public decimal Price { get; set; } // Siempre conbiene utilizar "decimal" para manejar valores monetarios en C#
        public bool IsAvailable { get; set; }
        public DateTime PublishOn { get; set; }

        public override string ToString()
        {
            return this.Description;
        }

    }
}
