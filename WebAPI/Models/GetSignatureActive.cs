using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class GetSignatureActive
    {
        public bool IsCode { get; set; }
        public string Message { get; set; }

        public Signature Items { get; set; }

    }

    public class Signature
    {
        public bool SignatureActive { get; set; }
        public int  SignatureId { get; set; }
    }
}
