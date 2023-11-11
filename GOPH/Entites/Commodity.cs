
using System.ComponentModel.DataAnnotations;

namespace GOPH.Entites
{
    public class Commodity
    {

        public Commodity()
        {
            Id = Guid.NewGuid().ToString().Substring(24);
        }

        [Key]
        public string Id { get; set; }

        [Display(Name = "Tên")]
        [Required(ErrorMessage = "{0} không được bỏ trống.")]
        public string Name { get; set; } 


        public virtual ICollection<Product> Products { get; set; }
    }
}
