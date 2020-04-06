using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{

    public class OpenTicketItem
    {
        public bool IsCode { get; set; }
        public string Message { get; set; }
        public List<TicketOpenCounts> TicketCounts { get; set; }
    }
    public class TicketOpenCounts
    {
        public string ParentId { get; set; }
        public string EmptorCustomerParrent { get; set; }
        public string ActiveTicketCount { get; set; }
        public string CustomerLogo { get; set; }
    }
}
