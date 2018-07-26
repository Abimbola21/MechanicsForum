using MechanicsForum.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MechanicsForum.Startup))]
namespace MechanicsForum
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesandUsers();
        }
        // In this method we will create default User roles and Admin user for login   
        private void CreateRolesandUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            // In Startup iam creating first Admin Role and creating a default Admin User    
            if (!roleManager.RoleExists("SuperAdmin"))
            {
                // first we create Super Admin role   
                var role = new IdentityRole
                {
                    Name = "SuperAdmin"
                };
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website                  

                ApplicationUser appUser = new ApplicationUser();
                var user = appUser;
                user.UserName = "superAdmin";
                user.Email = "victoria_lasode@yahoo.co.uk";
                string userPWD = "Admin@01";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin   
                if (chkUser.Succeeded)
                {
                    var result = UserManager.AddToRole(user.Id, "SuperAdmin");

                }
            }

        }
    }
}
