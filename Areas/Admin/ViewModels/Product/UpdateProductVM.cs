using Fruitables_MVC.Models;
using System.ComponentModel.DataAnnotations;

namespace Fruitables_MVC.Areas.Admin.ViewModels.Product
{
    public class UpdateProductVM
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public ICollection<ProductImages> ExistingImages { get; set; }

        public IFormFile? MainImage { get; set; }
        public IEnumerable<IFormFile>? AdditionalImages { get; set; }
    }
}