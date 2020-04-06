using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz;
using Quartz.Listener;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace RobinOperasyonWebService.Scheduling
{

    public class XScheduler : IJob
    {
     

        public class EmptorResult
        {
            public int EMPTOR_ID { get; set; }
        }


        public class DynamicAPIRequest
        {
            public string ProcessCode { get; set; }
            public int TicketId { get; set; }
        }


        public Task Execute(IJobExecutionContext context)
        {

            var RobinDB = new rpa_robin01Entities();
            var EmptorDB = new Emptor_ProbilServis_ProdEntities();

            var Record = RobinDB.RBN_EMPTOR_AUTOTICKETCLOSEDScheduler.Where(X => X.Active == true).Where(X => X.Process == 1).ToList();


            foreach (var item in Record)
            {
                DateTime Tarih = DateTime.Now;

                JobDataMap data = context.JobDetail.JobDataMap;

                if (item.LastStartDate <= Tarih || item.LastStartDate == null)
                {
                    var Prosess_Update = RobinDB.RBN_EMPTOR_AUTOTICKETCLOSEDScheduler.Where(X => X.Id == item.Id).First();
                    Prosess_Update.Process = 2;
                    RobinDB.SaveChanges();


                    var Deneme = item.SqlCode;


                    string myString = item.SqlCode.Replace(System.Environment.NewLine, " "); //add a line terminating ;

                    var TicketList = EmptorDB.Database.SqlQuery<EmptorResult>(myString).Take(Convert.ToInt32(item.OneClosedTicketCount)).ToList();

                    foreach (var Ticket in TicketList)
                    {
                        if (EmptorTicketClosedService(Ticket.EMPTOR_ID))
                        {
                            //Başarılı Güncelleme


                            var TicketInsert = RobinDB.Set<RBN_EMPTOR_AUTOCLOSEDTICKET>();
                            TicketInsert.Add(new RBN_EMPTOR_AUTOCLOSEDTICKET
                            {
                                TicketId = Ticket.EMPTOR_ID,
                                AutoClosedId = item.Id,
                                ClosedDate = DateTime.Now,
                                Active = true
                            });
                            if (RobinDB.SaveChanges() == 1)
                            {
                                //Kayıt Kapatıldı ve DB'ye eklendi.
                            }
                            else
                            {
                                //Otomatik Kayıt Kapatma sırasında hata meydana geldi.
                            }

                        }
                        else
                        {
                            //Hatalı Güncelleme - Mail ile bilgilendirme
                        }
                    }

                    var Update = RobinDB.RBN_EMPTOR_AUTOTICKETCLOSEDScheduler.Where(X => X.Id == item.Id).First();
                    Update.LastStartDate = DateTime.Now.AddSeconds(Convert.ToInt32(item.RepeatSchedulerSeconds));
                    Update.Process = 1;
                    RobinDB.SaveChanges();

                }



                //var Update = db.RB.Where(s => s.TOKEN == Tokencode).First();
                //Update.RVERSION = Versioncode;
                //db.SaveChanges();


            }

            return Task.CompletedTask;


        }

        public class RootObject
        {
            public string ResultCode { get; set; }
            public string ResultMessage { get; set; }
        }


        public bool EmptorTicketClosedService(int _TicketId)
        {

            try
            {
                DynamicAPIRequest Request = new DynamicAPIRequest
                {
                    ProcessCode = "PR_EMP_EL_ML_CLOSE-TICKET",
                    TicketId = _TicketId
                };

                string JSON = Newtonsoft.Json.JsonConvert.SerializeObject(Request);


                WebClient client = new WebClient();
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                client.Headers.Add(HttpRequestHeader.Authorization, "Basic cm9iaW4tZWwtbWw6ckBiaW5NbEVs");
                var result = client.UploadString("http://192.168.110.52/DynamicService/ProcessBasicAuth", "POST", JSON);

                var obmAlarms = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(result);

                if (obmAlarms.ResultCode == "0")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception e)
            {
                string result = e.Message;
                return false;
            }

        }


    }


}