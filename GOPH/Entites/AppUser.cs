using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection.Metadata.Ecma335;

namespace GOPH.Entites
{
    public class AppUser : IdentityUser
    {

        [StringLength(100)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [Column(TypeName = "Char")]
        [StringLength(100)]
        public string UrlImage { get; set; }

        [Display(Name = "Họ")]
        [StringLength(20, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string LastName { get; set; }

        [Display(Name = "Tên")]
        [StringLength(50, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
        public string FirstName { get; set; }

        [Display(Name = "Ngày sinh")]
        public DateTime? BirthDate { set; get; }

        [Display(Name = "Công ty")]
        [StringLength(50, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 5)]
        public string Company { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Mô tả bản thân")]
        public string Describe { get; set; }

        [Display(Name = "Quê quán")]
        [StringLength(100, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 5)]
        public string NativePlace { get; set; }

        [Display(Name ="Điểm hiện tại")]
        public int CurrentPoint { get; set; }

        [Display(Name = "Tổng điểm")]
        public int TotalPoints { get; set; }

        [Display(Name = "Ngày giao dịch cuối")]
        public DateTime? LastTrading { get; set; }

        public virtual ICollection<Invoice> IssueAnInvoices { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Voucher> Vouchers { get; set; }


    }
}
