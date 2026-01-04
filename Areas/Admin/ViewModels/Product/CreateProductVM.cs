using System.ComponentModel.DataAnnotations;

namespace Fruitables_MVC.Areas.Admin.ViewModels.Product
{
    public class CreateProductVM
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public IFormFile MainImage { get; set; }

        [Required]
        public IEnumerable<IFormFile> AdditionalImages { get; set; }
    }
}
