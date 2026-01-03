using System.ComponentModel.DataAnnotations;

namespace Fruitables_MVC.Areas.Admin.ViewModels.Category
{
    public class CreateCategoryVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Ad mütləq daxil edilməlidir")]
        public string Name { get; set; }
    }
}
