using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Schecle
{
    public class Finansbank :BaseDAL
    {
        public class OBMAlarm
        {
            public string ID { get; set; }
            public string Title { get; set; }
        }

        public int JsonPostRequest(string jsonRequest, string postUrl)
        {
            try
            {
                WebClient client = new WebClient();
                client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                string result = client.UploadString(postUrl, "POST", jsonRequest);
                return 1;
            }
            catch (Exception e)
            {
                string result = e.Message;
                return 0;
            }
        }




        public void CreateOBMAlarm(string Title, string Description, string Severity, string Priority, string State, string Category, string Application)
        {
            try
            {
                WebClient client = new WebClient();
                string jsonstring;
                jsonstring = client.DownloadString("http://10.176.0.149:1907/v1/ABBAS/GetOBMAlarms");
                OBMAlarm[] obmAlarms = Newtonsoft.Json.JsonConvert.DeserializeObject<OBMAlarm[]>(jsonstring);
                OBMAlarm obmAlarm = new OBMAlarm
                {
                    Title = Title,

                };
                string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(obmAlarm);
                int result = JsonPostRequest(jsonRequest, "http://10.176.0.149:1907/v1/ABBAS/AddOBMAlarms");
            }
            catch (Exception e)
            {
                string error = e.Message;
            }
        }
    }
}
