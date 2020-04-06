using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class SignatureModels
    {
        public bool IsCode { get; set; }
        public string Message { get; set; }
        public Result Results { get; set; }
    }

    public class Result
    {
        public int SignatureId { get; set; }
        public int FormId { get; set; }
    }


}
