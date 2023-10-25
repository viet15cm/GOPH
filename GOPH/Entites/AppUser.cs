using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;

namespace GOPH.Entites
{
    public class AppUser : IdentityUser
    {
        public string LastName { get; set; }

        public string FirstName { get; set; }

        public string Address { get; set; }


    }
}
