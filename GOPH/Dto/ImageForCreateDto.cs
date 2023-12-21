using GOPH.Entites;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using GOPH.ModelValidation;
using Microsoft.AspNetCore.Mvc;

namespace GOPH.Dto
{
    public class ImageForCreateDto
    {
      

        
        public string Url { get; set; }

        public string productId { get; set; }

        [BindProperty]
        [FileImgValidations(new string[] { ".jpg", ".jpeg", ".png", ".jfif", ".webp" })]
        [Display(Name = "Ảnh")]
        public IFormFile FormFile { get; set; }

    }
}
