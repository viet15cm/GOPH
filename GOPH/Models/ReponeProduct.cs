namespace GOPH.Models
{
    public class ReponeProduct : StatusMessage
    {
        
        public int PageNumber { get; set; }
        public  string ReturnHtml { get; set; }

        public bool IsPrice { get; set; }
    }
}
