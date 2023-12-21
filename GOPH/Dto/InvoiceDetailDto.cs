using GOPH.Entites;

namespace GOPH.Dto
{
    public class InvoiceDetailDto : Invoice
    {
        public string DatetimeDetail { get; set; }
        public string ConvertDatetime { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }

        
    }
}
