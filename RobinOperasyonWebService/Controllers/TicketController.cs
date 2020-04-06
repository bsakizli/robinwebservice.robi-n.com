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
    public class TicketController : ApiController
    {
        TicketAction T = new TicketAction();

        
        [Route("api/Ticket/GetTicket")]
        [HttpPost]
        public Ticket GetTicket(R_Ticket R)
        {
            return T.TicketMethod(R.EmptorUserId,R.ParrentId);
        }

        
        [Route("api/Ticket/GetClosedTicketCount")]
        [HttpPost]
        public OpenTicketItem GetClosedTicketCount(R_Ticket R)
        {
            return T.ActiveClosedCountTicket(R.EmptorUserId);
        }


        [Route("api/Ticket/GetDashboardSummary")]
        [HttpPost]
        public Dashboard GetDashboardSummary(R_UserId R)
        {
            return T.DashboardSummary(R.EmptorUserId);
        }


        [Route("api/Ticket/GetRemoteTicketClosedControl")]
        [HttpPost]
        public RemoteClosedModel RemoteTicketClosedControl(R_Tickets R)
        {  
            return T.RemoteTicketClosedControl(R.TicketId);
        }

    }
}
