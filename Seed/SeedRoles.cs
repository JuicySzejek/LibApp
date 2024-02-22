using LibApp.Data;
using LibApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;


namespace LibApp.Seed
{
    public class SeedRoles
    {
        public static async Task SeedRolesAndBaseUsers(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync(UserRoles.User))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.User));


                if (!await roleManager.RoleExistsAsync(UserRoles.StoreManager))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.StoreManager));


                if (!await roleManager.RoleExistsAsync(UserRoles.Owner))
                    await roleManager.CreateAsync(new IdentityRole(UserRoles.Owner));


                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();


                var ownerMail = "owner@gmail.com";
                var ownerUser = await userManager.FindByEmailAsync(ownerMail);

                if (ownerUser == null)
                {
                    var owner = new User()
                    {
                        UserName = "Owner",
                        Email = ownerMail,
                        EmailConfirmed = true,
                        Name = "Adam",
                        LastName = "Smasher",
                        Birthdate = new DateTime(1988, 8, 20),
                        MembershipTypeId = MembershipType.Annually,
                        HasNewsletterSubscribed = false
                    };
                    await userManager.CreateAsync(owner, "Owner1234$");
                    await userManager.AddToRoleAsync(owner, UserRoles.Owner);
                }

                var storeManagerMail = "storemanager@gmail.com";
                var storeManagerUser = await userManager.FindByEmailAsync(storeManagerMail);

                if (storeManagerUser == null)
                {
                    var storeManager = new User()
                    {
                        UserName = "StoreManager",
                        Email = storeManagerMail,
                        EmailConfirmed = true,
                        Name = "John",
                        LastName = "Whick",
                        Birthdate = new DateTime(1995, 8, 20),
                        MembershipTypeId = MembershipType.Annually,
                        HasNewsletterSubscribed = false
                    };
                    await userManager.CreateAsync(storeManager, "StoreManager1234$");
                    await userManager.AddToRoleAsync(storeManager, UserRoles.StoreManager);
                }

                var userMail = "user@gmail.com";
                var userAcc = await userManager.FindByEmailAsync(userMail);

                if (userAcc == null)
                {
                    var user = new User()
                    {
                        UserName = "user",
                        Email = userMail,
                        EmailConfirmed = true,
                        Name = "Anna",
                        LastName = "Kowalska",
                        Birthdate = new DateTime(1998, 8, 20),
                        MembershipTypeId = MembershipType.Annually,
                        HasNewsletterSubscribed = true

                    };
                    await userManager.CreateAsync(user, "User1234$");
                    await userManager.AddToRoleAsync(user, UserRoles.User);
                }
            }
        }
    }
}