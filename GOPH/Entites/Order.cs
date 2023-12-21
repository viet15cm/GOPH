using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace GOPH.Entites
{
    public class Order
    {

        [Key]
        public string Id { get; set; }

        [Display(Name ="Ngày đặt")]
        public DateTime DateCreate { get; set; }


        [Display(Name = "Tài khoản đăng nhập")]
        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public virtual AppUser AppUser { get; set; }

        public virtual Customer Customer { get; set; }

        [Display(Name = "Chốt đơn")]
        public bool IsCloseTheOrder { get; set; }

        [Display(Name = "Thùng rác")]
        public bool RecycleBin { get; set; }
        public virtual Voucher Voucher { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public virtual Invoice Invoice { get; set; }

    }
}
