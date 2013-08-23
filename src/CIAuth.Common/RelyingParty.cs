using System;
using System.Linq;
using System.Security.Permissions;
using System.Threading;

namespace Infrastructure
{
    public class RelyingParty
    {
        private string secretKey = String.Empty;

        public void ShareKeyOutofBand(string key)
        {
            this.secretKey = key;
        }

 
        public void AuthenticateWithEncryptedToken(string token)
        {
            JsonWebEncryptedToken jwt = null;

            try
            {
                jwt = JsonWebEncryptedToken.Parse(token, this.secretKey);

                // Now, swt.Claims will have the list of claims
                //jwt.Claims.ToList().ForEach(c => Console.WriteLine("{0} ==> {1}", c.Type, c.Value));

                //Thread.CurrentPrincipal = new ClaimsPrincipal(new ClaimsIdentity(jwt.Claims, "JWT"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        [PrincipalPermission(SecurityAction.Demand, Role = "Developer")]
        public void TheMethodRequiringAuthZ()
        {
            Console.WriteLine("With great power comes great responsibility - Uncle Ben");
        }
    }
}