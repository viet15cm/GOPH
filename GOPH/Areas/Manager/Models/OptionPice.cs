using System.ComponentModel.DataAnnotations;

namespace GOPH.Areas.Manager.Models
{
    public enum OptionPice
    {
        [Display(Name ="Có")]
        True = 1,

        [Display(Name ="Không")]
        False = 0,
    }

    public class UnverifiedOptionPicePayload
    {
        public OptionPice Status { get; set; } = OptionPice.False;
    }
}
