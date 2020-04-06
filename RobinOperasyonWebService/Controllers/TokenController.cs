using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebAPI;
using WebAPI.Models;

namespace RobinOperasyonWebService.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TokenController : ApiController
    {

        TokenControl TokenControl = new TokenControl();

        public TokenC TokenExpired()
        {
            return TokenControl.ExpiredToken();
        }

    }
}
