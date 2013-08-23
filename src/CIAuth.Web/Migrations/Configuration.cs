using System.Web.Security;
using CIAuth.Web.Models;
using WebMatrix.WebData;

namespace CIAuth.Web.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<CIAuth.Web.Models.UsersContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(UsersContext context)
        {
            WebSecurity.InitializeDatabaseConnection(
                "DefaultConnection",
                "UserProfile",
                "UserId",
                "UserName", autoCreateTables: true);

            if (!Roles.RoleExists("Administrator"))
                Roles.CreateRole("Administrator");
            if (!Roles.RoleExists("Client"))
                Roles.CreateRole("Client");
            if (!Roles.RoleExists("User"))
                Roles.CreateRole("User");

            if (!WebSecurity.UserExists("administrator"))
                WebSecurity.CreateUserAndAccount(
                    "administrator",
                    "password",
                    new {  });

            if (!Roles.GetRolesForUser("administrator").Contains("Administrator"))
                Roles.AddUsersToRoles(new[] { "administrator" }, new[] { "Administrator" });


            if (!WebSecurity.UserExists("XYZCorp"))
                WebSecurity.CreateUserAndAccount(
                    "XYZCorp",
                    "password",
                    new { });

            if (!Roles.GetRolesForUser("XYZCorp").Contains("Client"))
                Roles.AddUsersToRoles(new[] { "XYZCorp" }, new[] { "Client" });

            var userId = WebSecurity.GetUserId("XYZCorp");

            if (context.Applications.FirstOrDefault(a=>a.ApplicationName=="TestApp") == null)
            {
                context.Applications.Add(new Application
                    {
                        UserId = userId,
                        ApplicationName = "TestApp",
                        ClientSecret = "client_secret",
                        Description = "A test application",
                        GrantType = "AuthorizationCode",
                        Hosts = "http://localhost",
                        LogoUrl = "http://logo.com/logo.png",
                        
                        Offline = true,
                        EncryptionKey = @"<RSAKeyValue>
<Modulus>m117cJ0tRcPxjRj54okNrbiCrfFStid1s6NyWwHdDijzKEfRA29mxpQ9BACEr8JO88SbR4EKQ/5TNWPryDwqGmVrQ6BVZqU37cXuKkwKTPyUMU9k8gzl/4V1P/SNgPvX4ly22Do8muo0OyhBlrT7vYgnzz0XTvVzXxneR1a1SL8=</Modulus>
<Exponent>AQAB</Exponent>
<P>zbdw3+GR/t2fwfYipEUZ7DLCUt/n/hfHaBEUG1HKUwS36c3cBDL5hvWEiC5d+eQhLAFz/vqIpSajWC1ws1/YDQ==</P>
<Q>wVdXHMfeJ97fTcYmPhPi/PQDtKnqm/5BmQ7zIg5UdvHPFlHrE2UYJJrzEopCVgr3PcNCy3OpvJmrzvKmlH1E+w==</Q>
<DP>MmISOBL0AdrfzM5ur5LpBWttIoUKObYzNW6xYPuINQr7zDyJ/VFKwd4R5pSMma5g3XlBoppTqzcqXGdMqfH4ZQ==</DP>
<DQ>SyXojR4+gh7FitRGzwApzQoHWrRkpSdJVfWSmR0axXStek5y4YH8xVWYvw8QQ6NVgCMiFjQpuE2+ktyL1NFv+w==</DQ>
<InverseQ>FcVtHw8QPK9M27/TqfB8YqYxiHk1vhmqB9tJHRN+bIoBGULtB8axVENpRktLToPdJRAawrnc2kX6Wo4QVK8X8Q==</InverseQ>
<D>KzF2G/zWlUrAYOXMsvo/X0iorAsnX5Tg2CxcooiGGGWyFETDCx1xiUu1GuO5/9MZeBP0x5BMdlLCd1lsN6LiYT1XQNhj+od3cXQdsflcXlLIXMcBWGDnCDYKK1Zhu6b6bBf1t8HV1IIhrUAwD7BQL6efDTDzawp1Ab4OufWmi6E=</D>
</RSAKeyValue>",
                    });
                context.SaveChanges();
            }
            
        }
    }
}
