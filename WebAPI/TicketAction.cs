using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;



namespace WebAPI
{
    public class TicketAction : BaseDAL
    {

        public Ticket TicketMethod(int UserId, int ParrentId)
        {
            var Layer = EmptorDB.Database.SqlQuery<TicketItemList>(@"
            SELECT DISTINCT
            CT.ID AS EMPTOR_ID,
            CT.IDDESC AS 'EMPTOR_IDDESC',
            C.FULLNAME AS ILGILI_FIRMA_ADI,
            TY.DESCRIPTION_TR AS SERVIS_TIPI,
            CT.NAME AS SERVIS_TANIMI,
            CT.NOTES AS OZET_BILGI,
            CT.SOLUTIONEXPLANATION AS 'COZUM_ACIKLAMASI',
            TS.DESCRIPTION_TR AS KAYIT_DURUMU,
            USR.FULLNAME AS ANA_SORUMLU,
            TSB.DESCRIPTION_TR AS SAHA_ALT_STATUSU,
            OND.DESCRIPTION_TR AS ONE_SEVIYE_SAHA_ALT_STATUSU,
            TA.DESCRIPTION_TR AS SERVIS_ALANI,
            TAA.DESCRIPTION_TR AS SERVIS_ALT_ALANI,
            CTCC.NAME AS KOSUL,
            CT.C_MAXINTERVENTIONDATE AS SLA_MUDEHALE_TARIHI,
            CT.C_MAXSOLUTIONDATE AS SLA_COZUM_TARIHI,
            CASE  WHEN CT.C_MAXINTERVENTIONDATE > CT.C_INTERVENTIONDATE   THEN CONVERT(bit,1) ELSE CONVERT(bit,0) END AS SLA_MUDEHALE,
            CASE  WHEN CT.C_MAXSOLUTIONDATE > CT.C_SOLUTIONDATE   THEN CONVERT(bit,1) ELSE CONVERT(bit,0) END AS SLA_COZUM,
            CASE  WHEN (SELECT CONVERT(int,COUNT(*)) FROM CRMTBL_ACTIVITY AS TA WHERE 1 = 1 AND TA.TICKETID = CT.ID) > 0   THEN CONVERT(bit,1) ELSE CONVERT(bit,0) END AS AKTITE_VARMI,
            CASE  WHEN (SELECT CONVERT(int,COUNT(*)) FROM PROTBL_INVENTORYTRANSACTIONDETAIL AS EN WITH(NOLOCK) WHERE 1 = 1 AND EN.TICKETID=CT.ID) > 0   THEN CONVERT(bit,1) ELSE CONVERT(bit,0) END AS ENVANTER_HARKETI_VARMI,
            (SELECT CONVERT(int,COUNT(*)) FROM CRMTBL_ACTIVITY AS TA WHERE 1 = 1 AND TA.TICKETID = CT.ID) AS AKTIVITE_SAYISI,
            (SELECT CONVERT(int,COUNT(*)) FROM PROTBL_INVENTORYTRANSACTIONDETAIL AS EN WITH(NOLOCK) WHERE 1 = 1 AND EN.TICKETID=CT.ID) AS ENVANTER_SAYISI
            FROM CRMTBL_TICKET AS CT 
            INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = CT.C_CONCERNEDACCOUNTID
            INNER JOIN CRMTBL_TICKETTYPE AS TY WITH(NOLOCK) ON TY.ID = CT.TICKETTYPEID
            INNER JOIN CRMTBL_TICKETSTATUS AS TS WITH(NOLOCK) ON TS.ID = CT.TICKETSTATUSID
            INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = CT.RESPONSIBLEUSERID
            INNER JOIN PROTBL_TICKETSTATUSSUB AS TSB WITH(NOLOCK) ON TSB.ID = CT.C_TICKETSTATUSSUBID
            LEFT OUTER JOIN PROTBL_TICKETSTATUSSUB_LEVELONE AS OND WITH(NOLOCK) ON OND.ID = CT.C_TICKETSTATUSSUBID_LEVELONE
            LEFT OUTER JOIN PROTBL_TICKETAREA AS TA WITH(NOLOCK) ON TA.ID = CT.C_TICKETAREAID
            LEFT OUTER JOIN PROTBL_TICKETAREASUB AS TAA WITH(NOLOCK) ON TAA.ID = CT.C_TICKETSUBAREAID
            INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = CT.CONTRACTCONDITIONID
            LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = CT.ID
            LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = CT.ID
            LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = CT.ID
            WHERE 1 = 1
            AND CT.ACTIVE = 1
            AND CT.RESPONSIBLEUSERID=" + UserId + @"
            AND CT.TICKETSTATUSID=1
            AND CT.C_TICKETSTATUSSUBID IN (4,6,93)
            AND (CT.C_TICKETAREAID IS NOT NULL AND CT.C_TICKETSUBAREAID IS NOT NULL)
            AND (CTCC.ID IS NOT NULL)
            AND (CTA.ID IS NOT NULL)
            AND (CT.SOLUTIONEXPLANATION IS NOT NULL)
            --AND (TAT.ID IS NOT NULL)
            AND (CTCC.NAME IS NOT NULL)
            AND (INV.TICKETID IS NULL)
            AND (C.DEFPARENTCUSTOMERID = "+ ParrentId + @")
            ORDER BY CT.ID DESC
            ").ToArray();
            List<TicketItemList> ticketItemLists = new List<TicketItemList>();
            foreach (var item in Layer)
            {

                var EmptorAktivite = EmptorDB.Database.SqlQuery<Aktivite>(@"
                SELECT DISTINCT
                U.FULLNAME,
                U.EMAIL,
                --P.DESCRIPTION_TR,
                CTA.STARTDATE,
                CTA.ENDDATE,
                CTA.RESPONSIBLEUSERID,
                AKTYPE.ID AS AKTIVITE_TYPE_ID,
				AKTYPE.DESCRIPTION_TR AS AKTIVITE_TYPE,
                CTA.DESCRIPTION,
                CTA.CREATE_USER_ID,
                CTA.CREATE_USER_TIME,
                CONVERT(bit,CTA.IsTerminal) AS TERMINAL
                FROM CRMTBL_ACTIVITY AS CTA
                INNER JOIN BIZTBL_USER AS U WITH(NOLOCK) ON U.ID = CTA.CREATE_USER_ID
                INNER JOIN CRMTBL_ACTIVITYTYPE AS AKTYPE WITH(NOLOCK) ON AKTYPE.ID = CTA.ACTIVITYTYPEID
                WHERE 1 = 1
                AND CTA.ACTIVE=1 AND CTA.TICKETID=" + item.EMPTOR_ID+"").ToArray();


                List<Aktivite> aktivites = new List<Aktivite>();

                foreach (var EItem in EmptorAktivite)
                {

                    DateTime AktiviteGirisTarihi = EItem.CREATE_USER_TIME;
                    DateTime AktiviteBaslangicTarihi = EItem.STARTDATE;
                    DateTime AktiviteBitisTarihi = EItem.ENDDATE;

                

                   

                    Aktivite aktivite = new Aktivite()
                    {
                        FULLNAME = EItem.FULLNAME,
                        EMAIL = EItem.EMAIL,
                        DESCRIPTION = EItem.DESCRIPTION,
                        STARTDATE = EItem.STARTDATE,
                        ENDDATE = EItem.ENDDATE,
                        RESPONSIBLEUSERID = EItem.RESPONSIBLEUSERID,
                        AKTIVITE_TYPE_ID = EItem.AKTIVITE_TYPE_ID,
                        AKTIVITE_TYPE = EItem.AKTIVITE_TYPE,
                        CREATE_USER_ID  = EItem.CREATE_USER_ID,
                        CREATE_USER_TIME = EItem.CREATE_USER_TIME,
                        TERMINAL = EItem.TERMINAL
                    };

                    aktivites.Add(aktivite);
                }

               

                TicketItemList ticketItemList = new TicketItemList()
                {
                    EMPTOR_ID = item.EMPTOR_ID,
                    EMPTOR_IDDESC = item.EMPTOR_IDDESC,
                    ILGILI_FIRMA_ADI = item.ILGILI_FIRMA_ADI,
                    SERVIS_TIPI = item.SERVIS_TIPI,
                    SERVIS_TANIMI = item.SERVIS_TANIMI,
                    OZET_BILGI = item.OZET_BILGI,
                    COZUM_ACIKLAMASI = item.COZUM_ACIKLAMASI,
                    KAYIT_DURUMU = item.KAYIT_DURUMU,
                    ANA_SORUMLU = item.ANA_SORUMLU,
                    SAHA_ALT_STATUSU = item.SAHA_ALT_STATUSU,
                    ONE_SEVIYE_SAHA_ALT_STATUSU = item.ONE_SEVIYE_SAHA_ALT_STATUSU,
                    SERVIS_ALANI = item.SERVIS_ALANI,
                    SERVIS_ALT_ALANI = item.SERVIS_ALT_ALANI,
                    KOSUL = item.KOSUL,
                    SLA_MUDEHALE_TARIHI = item.SLA_MUDEHALE_TARIHI,
                    SLA_COZUM_TARIHI=item.SLA_COZUM_TARIHI,
                    SLA_MUDEHALE = item.SLA_MUDEHALE,
                    SLA_COZUM = item.SLA_COZUM,
                    AKTITE_VARMI = item.AKTITE_VARMI,
                    ENVANTER_HARKETI_VARMI = item.ENVANTER_HARKETI_VARMI,
                    AKTIVITE_SAYISI = item.AKTIVITE_SAYISI,
                    ENVANTER_SAYISI = item.ENVANTER_SAYISI,
                    Activites = aktivites


                };
                ticketItemLists.Add(ticketItemList);
            }

            Ticket Baris = new Ticket() {
                IsCode = true,
                Message = "Kayıtlar başarıyla listelenmiştir.",
                Items = ticketItemLists
            };

            return Baris;

        }


        public OpenTicketItem ActiveClosedCountTicket(int UserId)
        {

            
            try
            {

                var KayitSayisi = EmptorDB.Database.SqlQuery<TicketOpenCounts>(@"
                DECLARE @USERID INT;
                SET @USERID = "+ UserId + @";
SELECT *
FROM(VALUES 
    --PROJE PARENT
('2677399', 
 'ZENIA GENEL MERKEZ', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 2677399
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'zenia.jpg'
),
--PROJE PARENT
--PROJE PARENT
('3629723', 
 'TURKCELL BAYİ PARENT', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 3629723
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'turkcell.jpg'
),
--PROJE PARENT
--PROJE PARENT
('71497', 
 'PARENT ALTERNATİF BANK A.Ş.-İSTANBUL-MERKEZ', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 71497
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'alternatif.jpg'
),
--PROJE PARENT
--PROJE PARENT
('270855', 
 'FİNANSBANK A.Ş.', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 270855
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'finans.jpg'
),
--PROJE PARENT
--PROJE PARENT
('5052785', 
 'KUVEYT TÜRK KATILIM SAHA BAKIM SÖZLEŞMESİ-S0310701', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
        LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 5052785
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'kuveyt.jpg'
),
('118296', 
 'ŞEKERBANK T.A.Ş.-OPERASYON MERKEZİ/İSTANBUL-KARTAL', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 118296
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'seker_bank.jpg'
),
('4074100', 
 'IPEKYOL_PARENT', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 4074100
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
         --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'ipekyol.jpg'
),
('399224', 
 'ODEABANK OPS PARENT', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 399224
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'odea.jpg'
),
('1415759', 
 'SHELL İSTASYONLARI_PARENT', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 1415759
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'shell.jpg'
),
('37417', 
 'MİLLİ SAVUNMA BAKANLIĞI/ANKARA-BAKANLIKLAR', 
 CONVERT(NVARCHAR,
(
    SELECT COUNT(DISTINCT T.ID)
    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
    WHERE 1 = 1
          AND (T.ACTIVE = 1)
          AND T.TICKETSTATUSID = 1
          AND (C.DEFPARENTCUSTOMERID = 37417
               AND C.ACTIVE = 1)
          AND (INV.TICKETID IS NULL)
          AND (CTCC.ID IS NOT NULL)
          AND (T.SOLUTIONEXPLANATION IS NOT NULL)
          AND (CTA.ID IS NOT NULL)
          AND (T.C_TICKETAREAID IS NOT NULL
               AND T.C_TICKETSUBAREAID IS NOT NULL)
          --AND (TAT.ID IS NOT NULL)
          AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
         AND (CTCC.NAME IS NOT NULL)
         AND (ATH.TICKETID IS NULL)
         AND (USR.ID = @USERID)
)), 
 'msb.jpg'
)
--PROJE PARENT
) t1(ParentId, EmptorCustomerParrent, ActiveTicketCount, CustomerLogo);").ToArray();





                List<TicketOpenCounts> TicketCounts = new List<TicketOpenCounts>();


                foreach (var item in KayitSayisi)
                {

                    TicketOpenCounts ticketOpenCounts = new TicketOpenCounts()
                    {
                        ActiveTicketCount = item.ActiveTicketCount,
                        EmptorCustomerParrent = item.EmptorCustomerParrent,
                        CustomerLogo = item.CustomerLogo,
                        ParentId = item.ParentId
                        
                     
                    };

                    

                    TicketCounts.Add(ticketOpenCounts);


                }

                OpenTicketItem openTicketItem = new OpenTicketItem()
                {
                    IsCode = true,
                    Message = "Servis başarıyla açık kayıt bilgisine ulaşmıştır",
                    TicketCounts = TicketCounts
                };


                return openTicketItem;

            } catch
            {

                OpenTicketItem openTicketItem = new OpenTicketItem
                {
                    IsCode = false,
                    Message = "Service sorunu meydana geldi.",
                    TicketCounts = null
                };

                return openTicketItem;

            }


        }


        public Dashboard DashboardSummary(int UserId)
        {
            try
            {

                var Record = EmptorDB.Database.SqlQuery<DashboardModel>(@"

                 DECLARE @USERID int;

                SET @USERID ="+UserId+ @"


                SELECT *
                FROM(VALUES 
                    --AÇIK KAYIT SAYISI
                (CONVERT(NVARCHAR,
                (
                    SELECT COUNT(DISTINCT T.ID)
                    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
                         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
                         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
                         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
                         INNER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
                         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
                         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
                         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
                    WHERE 1 = 1
                          AND (T.ACTIVE = 1)
                          --AND T.TICKETSTATUSID = 1
                          --AND (C.DEFPARENTCUSTOMERID IN (3629723,2677399,71497,270855,5052785,118296, 4074100, 399224, 1415759, 37417))
                          AND C.ACTIVE = 1
                          AND (INV.TICKETID IS NULL)
                          --AND (CTCC.ID IS NOT NULL)
                          --AND (T.SOLUTIONEXPLANATION IS NOT NULL)
                          --AND (CTA.ID IS NOT NULL)
                          --AND (T.C_TICKETAREAID IS NOT NULL AND T.C_TICKETSUBAREAID IS NOT NULL)
                          --AND (TAT.ID IS NOT NULL)
                          --AND T.C_TICKETSTATUSSUBID IN (4,6,93)
                          AND T.C_TICKETSTATUSSUBID IN(1, 8)
                         --AND (CTCC.NAME IS NOT NULL)
                         --AND (ATH.TICKETID IS NULL)
                         AND (USR.ID = @USERID)
                )),

                    --BEKLEME KAYIT SAYISI
                 CONVERT(NVARCHAR,
                (
                    SELECT COUNT(DISTINCT T.ID)
                    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
                         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
                         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
                         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
                         INNER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
                         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
                         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
                         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
                    WHERE 1 = 1
                          AND (T.ACTIVE = 1)
                          --AND T.TICKETSTATUSID = 1
                          AND (C.DEFPARENTCUSTOMERID IN(3629723, 2677399, 71497, 270855, 5052785,118296, 4074100, 399224, 1415759, 37417))
                         --AND C.ACTIVE = 1
                         -- AND (INV.TICKETID IS NULL)
                         --AND (CTCC.ID IS NOT NULL)
                         --AND (T.SOLUTIONEXPLANATION IS NOT NULL)
                         --AND (CTA.ID IS NOT NULL)
                         --AND (T.C_TICKETAREAID IS NOT NULL AND T.C_TICKETSUBAREAID IS NOT NULL)
                         --AND (TAT.ID IS NOT NULL)
                         AND T.C_TICKETSTATUSSUBID IN(7)
                    AND (CTCC.NAME IS NOT NULL)
                    --AND (ATH.TICKETID IS NULL)
                    AND (USR.ID = @USERID)
                )),

                --KAPATILMASI GEREKEN KAYIT SAYISI
                 CONVERT(NVARCHAR,
                (
                    SELECT COUNT(DISTINCT T.ID)
                    FROM CRMTBL_TICKET AS T WITH(NOLOCK)
                         INNER JOIN CRMTBL_CUSTOMER AS C WITH(NOLOCK) ON C.ID = T.C_CONCERNEDACCOUNTID
                         LEFT OUTER JOIN PROTBL_INVENTORYTRANSACTIONDETAIL AS INV WITH(NOLOCK) ON INV.TICKETID = T.ID
                         LEFT OUTER JOIN CRMTBL_ACTIVITY AS CTA WITH(NOLOCK) ON CTA.TICKETID = T.ID
                         INNER JOIN PROTBL_TICKETATTACHMENT AS TAT WITH(NOLOCK) ON TAT.TICKETID = T.ID
                         INNER JOIN BIZTBL_USER AS USR WITH(NOLOCK) ON USR.ID = T.RESPONSIBLEUSERID
                         INNER JOIN CRMTBL_CONTRACTCONDITION AS CTCC WITH(NOLOCK) ON CTCC.ID = T.CONTRACTCONDITIONID
                         LEFT OUTER JOIN PROTBL_TICKETATTACHMENT AS ATH WITH(NOLOCK) ON ATH.ID = T.ID
                    WHERE 1 = 1
                          AND (T.ACTIVE = 1)
                          AND T.TICKETSTATUSID = 1
                          AND (C.DEFPARENTCUSTOMERID IN(3629723, 2677399, 71497, 270855, 5052785,118296, 4074100, 399224, 1415759, 37417)
                    AND C.ACTIVE = 1)
                         AND (INV.TICKETID IS NULL)
                         AND (CTCC.ID IS NOT NULL)
                         AND (T.SOLUTIONEXPLANATION IS NOT NULL)
                         AND (CTA.ID IS NOT NULL)
                         AND (T.C_TICKETAREAID IS NOT NULL
                              AND T.C_TICKETSUBAREAID IS NOT NULL)
                         AND (TAT.ID IS NOT NULL)
                         AND T.C_TICKETSTATUSSUBID IN(4, 6, 93)
                    AND (CTCC.NAME IS NOT NULL)
                    AND (ATH.TICKETID IS NULL)
                    AND (USR.ID = @USERID)
                )),

                --ROBİN KAPATILAN KAYIT SAYISI
                 CONVERT(NVARCHAR,
                (
                    SELECT COUNT(*)
                    FROM CRMTBL_TICKET AS CT
                    WHERE 1 = 1
                          AND CT.RESPONSIBLEUSERID = @USERID
                          AND CT.C_TICKETSTATUSSUBID = 9
                          AND CT.UPDATE_USER_ID = 6830
                ))
                )) t1(ActiveCount, WatingCount, ClosedCount, RobinCount)
                ").FirstOrDefault();

                


                DashboardModel dashboardModel = new DashboardModel()
                {
                    ActiveCount = Record.ActiveCount,
                    WatingCount = Record.WatingCount,
                    ClosedCount = Record.ClosedCount,
                    RobinCount = Record.RobinCount
                };

                Dashboard dashboard = new Dashboard()
                {
                    IsCode = true,
                    Message = "Listeleme başarıyla yapılmıştır",
                    Items = dashboardModel
                };

                return dashboard;

            }
            catch
            {
                Dashboard dashboard = new Dashboard()
                {
                    IsCode = false,
                    Message = "Listeleme sırasında bir sorun meydana geldi"
                };

                return dashboard;
            }
        }

        public RemoteClosedModel RemoteTicketClosedControl(int TicketId)
        {

            RemoteClosedModel remoteClosedModel = new RemoteClosedModel();

            try
            {
                var Record = EmptorDB.Database.SqlQuery<Labels>(@"
                DECLARE @KAYIT_ID INT;
                SET @KAYIT_ID =  "+TicketId+@";
                SELECT 
                      CA.ID
	   
                FROM CRMTBL_TICKET AS CT WITH(NOLOCK)
                     INNER JOIN CRMTBL_ACTIVITY AS CA WITH(NOLOCK) ON CA.TICKETID = CT.ID
                     INNER JOIN CRMTBL_ACTIVITYTYPE CAT WITH(NOLOCK) ON CAT.ID = CA.ACTIVITYTYPEID
                WHERE 1 = 1
                      AND (CT.ACTIVE = 1 AND CA.ACTIVE=1)
                      AND CT.ID = @KAYIT_ID
	                  AND CAT.ID = 51
            ").ToList();


                if (Record.Count > 0)
                {

                    remoteClosedModel = new RemoteClosedModel
                    {
                        IsCode = false,
                        Message = "Kayıt formsuz kapatılamaz"
                    };

                    return remoteClosedModel;

                } else
                {
                    remoteClosedModel = new RemoteClosedModel
                    {
                        IsCode = true,
                        Message = "Kayıt formsuz bir şekilde kapatılabilir."
                    };

                    return remoteClosedModel;

                }

               
            } catch
            {
                remoteClosedModel = new RemoteClosedModel()
                {
                    IsCode = false,
                    Message = "Servis hatası"
                };
                return remoteClosedModel;
            }
        }
    }
}
