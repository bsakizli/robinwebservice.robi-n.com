using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{

    public class Dashboard
    {
        public bool IsCode { get; set; }
        public string Message { get; set; }

        public DashboardModel Items { get; set; }
    }



    public class DashboardModel
    {
        public string ActiveCount { get; set; }
        public string WatingCount { get; set; }

        public string ClosedCount { get; set; }

        public string RobinCount { get; set; }
    }
}
