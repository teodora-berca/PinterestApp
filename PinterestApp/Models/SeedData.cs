using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PinterestApp.Data;

namespace PinterestApp.Models
{
    public static class SeedData
    {
        public static void Initialize (IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                //Verificam daca in baza de date exista cel putin un rol, adica a fost rulat deja codul
                //Daca a fost deja rulat codul, facem return pentru a nu introduce de mai multe ori rolurile
                if(context.Roles.Any())
                {
                    return;
                }
                //Crearea rolurilor in baza de date
                context.Roles.AddRange(
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7210", Name="User", NormalizedName="User".ToUpper() },
                    new IdentityRole { Id = "2c5e174e-3b0e-446f-86af-483d56fd7211", Name = "Admin", NormalizedName = "Admin".ToUpper() }
                    );
                //O noua instanta folosita pentru a crea parolele utilizatorilor
                //Parolele sunt de tip hash
                var hasher = new PasswordHasher<ApplicationUser>();
                //Cream cate un user pentru fiecare rol pe care il avem
                context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                        // primary key
                        UserName = "admin@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@TEST.COM",
                        Email = "admin@test.com",
                        NormalizedUserName = "ADMIN@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")
                    },
                    new ApplicationUser
                    {
                        Id = "8e445865-a24d-4543-a6c6-9443d048cdb2",
                        // primary key
                        UserName = "user@test.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "USER@TEST.COM",
                        Email = "user@test.com",
                        NormalizedUserName = "USER@TEST.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    }
                    ) ; 
                //Facem asocierea dintre useri si roluri
                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7211",
                        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                        UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
                    }
                    );
                context.SaveChanges();
            }
        }
    }
}
