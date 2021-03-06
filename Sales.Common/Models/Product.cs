﻿
namespace Sales.Common.Models
{
    using Newtonsoft.Json;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        [Display(Name = "Image")]
        public string ImagePath { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; } // Siempre conbiene utilizar "decimal" para manejar valores monetarios en C#

        [Display(Name = "Is Available")]
        public bool IsAvailable { get; set; }

        [Display(Name = "Publish On")]
        [DataType(DataType.Date)]
        public DateTime PublishOn { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [JsonIgnore] //Json no sabe deserealizar un Icollection, por ese debemos colocarle JsonIgnore
        public virtual Category Category { get; set; } // Como no se mapea en la base de datos se le coloca el atributo "Virtual"

        [NotMapped] // Cuando tengo atributos que forman parte del modelo, PERO que no formen parte de la base de datos, se coloca [NotMapped]
        public byte[] ImageArray { get; set; }

        public string ImageFullPath
        {
            get
            {
                if (string.IsNullOrEmpty(this.ImagePath))
                {
                    return "noproduct";
                }
                //return $"http://200.55.241.235/SalesBackend{this.ImagePath.Substring(1)}"; // el substring es para quitarle el ñuflo
                return $"http://200.55.241.235/SalesAPI{this.ImagePath.Substring(1)}"; // el substring es para quitarle el ñuflo
            }
        }

        public override string ToString()
        {
            return this.Description;
        }

    }
}
