using GOPH.Entites;
using Microsoft.AspNetCore.Authorization;

namespace GOPH.Security.Requirements
{
    public class CanOptionWholesaleHandler : AuthorizationHandler<CanOptionWholesaleRequirements>
    {
        public CanOptionWholesaleHandler()
        {

        }
        //public CanOptionCategoryUserHandler(UserManager<AppUser> userManager)
        //{
        //    _userManager = userManager;
        //}
      
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanOptionWholesaleRequirements requirement)
        {
            //var user = _userManager.GetUserAsync(context.User).Result;

            if (context.User.IsInRole("Admin") || context.User.IsInRole("Administrator"))
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            if (context.User.Claims.Any(x => x.Value.Equals("wholesale")))
            {
                context.Succeed(requirement);

                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
