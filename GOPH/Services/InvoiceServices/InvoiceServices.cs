using GOPH.DbContextLayer;
using System.Collections.Generic;

namespace GOPH.Services.InvoiceServices
{
    public class InvoiceServices : IinvoceSevices
    {
        private readonly AppDbContext _context;
        public InvoiceServices( AppDbContext appDbContext)
        {

           
            _context = appDbContext;

        }

        public string GetId()
        {
            throw new NotImplementedException();
        }
    }
}
