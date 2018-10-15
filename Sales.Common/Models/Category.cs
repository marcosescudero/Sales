
namespace Sales.Common.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        [JsonIgnore] //Json no sabe deserealizar un Icollection, por ese debemos colocarle JsonIgnore
        public virtual ICollection<Product> Products { get; set; } // Virtual: Para que no se mapee en la base de datos.

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.ImagePath))
                {
                    return "noproduct";
                }

                //return $"https://salesbackend.azurewebsites.net{this.ImagePath.Substring(1)}";
                return $"http://200.55.241.235/SalesAPI{this.ImagePath.Substring(1)}";
            }
        }
    }

}
