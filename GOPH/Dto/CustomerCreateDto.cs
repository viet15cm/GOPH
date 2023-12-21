using GOPH.Entites;
using GOPH.ModelValidation;
using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class CustomerCreateDto
    {
        public CustomerCreateDto()
        {
            Id = Guid.NewGuid().ToString();
        }

        [Key]
        public string Id { get; set; }

        [Display(Name = "Họ và tên")]
        [StringLength(30, ErrorMessage = "{0} dài từ {2} đến {1} ký tự.", MinimumLength = 3)]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string Name { get; set; }

        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [DataType(DataType.PhoneNumber)]
        [PhoneValidation]
        public string MobilePhone { get; set; }

        [EmailAddress(ErrorMessage = "{0} không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Họ và tên người nhận")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string NameReceiver { get; set; }

        [Display(Name = "Địa chỉ cụ thể")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        public string AddressReceiver { get; set; }

        [Display(Name = "Số điện thoại người nhận")]
        [Required(ErrorMessage = "{0} không được bỏ trống")]
        [DataType(DataType.PhoneNumber)]
        [Phone(ErrorMessage = "{0} không hợp lệ")]
        public string MobilePhoneReceiver { get; set; }

        [Display(Name = "Thêm ghi chú")]
        [DataType(DataType.Text)]
        [StringLength(500, ErrorMessage = "{0} tối đa {1} ký tự ")]
        public string Description { set; get; }
        
        public string OrderId { get; set; }

    }
}
