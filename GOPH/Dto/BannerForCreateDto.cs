using GOPH.ModelValidation;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class BannerForCreateDto
    {
        public string Url { get; set; }

        public string EventId { get; set; }

        [BindProperty]
        [FileImgValidations(new string[] { ".jpg", ".jpeg", ".png", ".jfif", ".webp" })]
        [Display(Name = "Ảnh")]
        public IFormFile FormFile { get; set; }
    }
}
