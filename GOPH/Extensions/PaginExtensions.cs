using GOPH.Paging;
using System.Linq;

namespace GOPH.Extensions
{
    public static class PaginExtensions<T>
    {
        public static int Count(IQueryable<T> source, ProductParameters productParameters)
        {
           
            var items = source.Skip((productParameters.PageNumber - 1) * productParameters.PageSize).Take(productParameters.PageSize);
            return items.Count();


        }
    }
}
