using System.ComponentModel.DataAnnotations;

namespace ProductsValidation.Models
{
    public class PriceEdit
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Required]
        [Range(typeof(decimal), "0", "100000")]
        public decimal? Price { get; set; }
    }
}