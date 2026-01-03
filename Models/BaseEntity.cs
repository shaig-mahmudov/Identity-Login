namespace Fruitables_MVC.Models
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public bool isDeleted { get; set; } = false;
    }
}
