using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI
{
    public class TokenControl
    {

         public TokenC ExpiredToken()
        {
            TokenC tokenC = new TokenC()
            {
                IsCode = true,
                Message = "Token geçerli"
            };

            return tokenC;
        }

    }
}
