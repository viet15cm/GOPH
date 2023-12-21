using GOPH.Entites;

namespace GOPH.Dto
{
    public class OrderDetailDto : Order
    {
        public string DatetimeDetail { get; set; }
        public string ConvertDatetime { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public decimal TotalPrice { get; set; }

    }
}
