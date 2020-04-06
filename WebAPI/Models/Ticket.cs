using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class Ticket
    {

        public bool IsCode { get; set; }
        public string Message { get; set; }

        public List<TicketItemList> Items { get; set; }

    }

    public class TicketItemList
    {
        public string EMPTOR_IDDESC { get; set; }
        public int EMPTOR_ID { get; set; }
        public string ILGILI_FIRMA_ADI { get; set; }
        public string SERVIS_TIPI { get; set; }
        public string SERVIS_TANIMI { get; set; }

        public string COZUM_ACIKLAMASI { get; set; }
        public string OZET_BILGI { get; set; }
        public string KAYIT_DURUMU { get; set; }
        public string ANA_SORUMLU { get; set; }
        public string SAHA_ALT_STATUSU { get; set; }
        public string ONE_SEVIYE_SAHA_ALT_STATUSU { get; set; }
        public string SERVIS_ALANI { get; set; }
        public string SERVIS_ALT_ALANI { get; set; }
        public string KOSUL { get; set; }
        public bool SLA_MUDEHALE { get; set; }
        public bool SLA_COZUM { get; set; }
        public DateTime SLA_MUDEHALE_TARIHI { get; set; }
        public DateTime SLA_COZUM_TARIHI { get; set; }
        public bool AKTITE_VARMI { get; set; }
        public bool ENVANTER_HARKETI_VARMI { get; set; }
        public int AKTIVITE_SAYISI { get; set; }
        public int ENVANTER_SAYISI { get; set; }

        public List<Aktivite> Activites { get; set; }

    }

    public class Aktivite
    {
        public string FULLNAME { get; set; }
        public string EMAIL { get; set; }
        public int AKTIVITE_TYPE_ID { get; set; }
        public string AKTIVITE_TYPE { get; set; }
        public DateTime STARTDATE { get; set; }
        public DateTime ENDDATE { get; set; }
        public int RESPONSIBLEUSERID { get; set; }
        public string DESCRIPTION { get; set; }
        public int CREATE_USER_ID { get; set; }
        public DateTime CREATE_USER_TIME { get; set; }

        public bool TERMINAL { get; set; }
    }
}
