using RobinOperasyonWebService.Models;
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
    public class SignatureController : ApiController
    {
        SignatureAction Service = new SignatureAction();


        [Route("api/Signature/AddSignature")]
        [HttpPost]
        public SignatureModels GetTicket(SignatureModel model)
        {
            return Service.AddSignature(model);
        }


        [Route("api/Signature/GetSignature")]
        [HttpPost]
        public GetSignatureActive GetSignatureActive(GetSignatureModel model)
        {
            return Service.GetSignature(model);
        }



    }
}
