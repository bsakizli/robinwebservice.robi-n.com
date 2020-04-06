using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RobinOperasyonWebService.Models
{
    public class R_Signature
    {
        public int TicketId { get; set; }
        public int AttachmentId { get; set; }

        public bool IsSignature { get; set; }
    }



    public class R_GetSignature
    {
        public int FormId { get; set; }
    }
}