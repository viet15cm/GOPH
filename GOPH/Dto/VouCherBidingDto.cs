using System.ComponentModel.DataAnnotations;

namespace GOPH.Dto
{
    public class VouCherBidingDto
    {
        public string Code { get; set; }

        public int? Price { get; set; }

        public string CustomerId { get; set; }

        public string OrderId { get; set; }

    }
}
