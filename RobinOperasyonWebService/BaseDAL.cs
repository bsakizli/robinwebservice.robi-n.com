using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RobinOperasyonWebService
{
    public class BaseDAL
    {

        protected rpa_robin01Entities RobinDB;
        protected Emptor_ProbilServis_ProdEntities EmptorDB;

        public BaseDAL()
        {
            RobinDB = new rpa_robin01Entities();
            EmptorDB = new Emptor_ProbilServis_ProdEntities();
        }


    }
}