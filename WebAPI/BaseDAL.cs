using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI
{
    public class BaseDAL
    {
        protected Emptor_ProbilServis_ProdEntities EmptorDB;
        protected rpa_robin01Entities RobinDB;

        public BaseDAL()
        {
            EmptorDB = new Emptor_ProbilServis_ProdEntities();
            RobinDB = new rpa_robin01Entities();
        }
    }
}
