using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ProductsValidation.Models
{
    public class Product
    {
        public enum Category { Toy, Technique, Clothes, Transport}

        public int Id { get; set; }
        [BindRequired]
        public Category Type { get; set; }
        [Required]
        public string Name { get; set; }
        [MinLength(3)]
        public string Description { get; set; }
        [BindRequired]
        [Range(typeof(decimal), "0", "100000")]
        public decimal Price { get; set; }
    }
}
