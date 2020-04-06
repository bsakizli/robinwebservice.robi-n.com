using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class RemoteClosedModel
    {
        public bool IsCode { get; set; }
        public string Message { get; set; }
     
    }

    public class Labels
    {
        public int ActivityId { get; set; }
    }
}
