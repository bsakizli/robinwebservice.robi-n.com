using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.Owin.Security.OAuth;
using WebAPI;


namespace RobinOperasyonWebService.Authentication
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class Saglayici : OAuthAuthorizationServerProvider
    {

        APIUsers APIUsers = new APIUsers();
        
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            //Burada client validation kullanmadık. İstersek custom client tipleri ile client tipine görede validation sağlayabiliriz.
            context.Validated();
        }

       
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {

            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" }); // Farklı domainlerden istek sorunu yaşamamak için

            //Burada kendi authentication yöntemimizi belirleyebiliriz.Veritabanı bağlantısı vs...




            var R = APIUsers.AuthUsers(context.UserName);

            if (R.Success == true)
            {
                var identity = new ClaimsIdentity(context.Options.AuthenticationType);

                identity.AddClaim(new Claim("name", context.UserName));
                identity.AddClaim(new Claim("yetki", "Admin"));

                context.Validated(identity);
            }
            else
            {
                context.SetError("Geçersiz istek", "Kullanıcı sisteme tanımlı değil, sistem tanımlaması gerekmektedir.");
            }



            



        }
    }
}