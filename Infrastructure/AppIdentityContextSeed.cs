using Core.IdentityEntities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure
{
    public class AppIdentityContextSeed
    {
        public static async Task SeedUserAsync(UserManager<AppUser> userManager)
        {
            if(!userManager.Users.Any())
            {
                var user = new AppUser
                {
                    DisplayName = "Mohamed",
                    Email = "mohamed@gmail.com",
                    UserName = "mohamedAmin",
                    Address = new Address
                    {
                        FirstName = "Mohamed",
                        LastName = "Amin",
                        Street = "77",
                        State = "Cairo",
                        City = "15_Mayo",
                        ZipCode = "90120"
                    }
                };
                await userManager.CreateAsync(user, "Password123!");
            }
        }
    }
}
