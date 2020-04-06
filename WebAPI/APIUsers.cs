using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI
{
    public class APIUsers : BaseDAL
    {

        public ApiUsers AuthUsers(string EmptorUsername)
        {
            ApiUsers apiUsers = new ApiUsers();

            var Record = RobinDB.RBN_EMPTOR_API_USERS.Where(X => X.Active == true).Where(X => X.EmptorUsername == EmptorUsername).FirstOrDefault();

            if (Record != null)
            {
                apiUsers = new ApiUsers()
                {
                    Success = true,
                    Message = "Kullanıcı sisteme tanımlı, token verilebilir."
                };

                

            } else
            {
                apiUsers = new ApiUsers()
                {
                    Success = false,
                    Message = "Kullanıcı sisteme tanımlı değil, sisteme dahil olması gerekmektedir."
                };

            }

            return apiUsers;
            

        }

    }
}
