namespace Fruitables_MVC.Models
{
    public class ProductImages : BaseEntity
    {
        public string ImagePath { get; set; }
        public bool isMain { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
