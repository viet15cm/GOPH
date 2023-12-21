using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GOPH.Entites
{
    public class Invoice
    {
   
        [Key]
        public string Id { get; set; }

        [Display(Name ="Ngày xuất")]
      
        public DateTime DateCreate { get; set; }

        [Display(Name = "Nhân viên")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public virtual AppUser Employee { get; set; }
        public string OrderId { get; set; }

        [Display(Name ="Tổng tiền")]
        public decimal TotalPrice { get; set; }


        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }



    }
}
