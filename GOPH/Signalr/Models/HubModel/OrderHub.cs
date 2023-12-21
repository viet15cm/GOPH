
using GOPH.DbContextLayer;
using GOPH.Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace ProjectWebNotes.Models
{

   
    public class OrderHub : Hub
    {

        private readonly IHttpContextAccessor _contextAccessor;

        private readonly AppDbContext _context;

        private readonly UserManager<AppUser> _userManager;
        public OrderHub(AppDbContext context, UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
        }

        public class ReponseDataOrder
        {
           public int Count { get; set; }
        }

        public async Task SendMessage(string message)
        {
            var countMessage =  _context.Orders.Count();

            var users = await _context.Users.ToListAsync();

            var reponse = new ReponseDataOrder { Count = countMessage };

            foreach (var user in users)
            {
                await Clients.User(user.Id).SendAsync("InternalMessage", reponse.Count.ToString()).ConfigureAwait(true);
            }

        }

    }
}
