using COCAAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace COCAAPI.Controllers
{

    public class ValuesController : ApiController
    {
        //private const string APIKey = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjI5Nzg5NzM0OCwiYWFpIjoxMSwidWlkIjo0MzA0NTkxMCwiaWFkIjoiMjAyMy0xMS0yMlQwMjo1MDoxOS4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6MTY4MzU1MzEsInJnbiI6InVzZTEifQ._rkd67FZYN5UA2SYaNUU232R5us4gR3QhpWrESAhpg0";
        private const string APIKey = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjMxNjQ5MjYzOCwiYWFpIjoxMSwidWlkIjo0NDA5OTk0MCwiaWFkIjoiMjAyNC0wMi0wMVQwNjoxNDoxNS4yMDdaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6MTY4MzU1MzEsInJnbiI6InVzZTEifQ.fnlrVv7skPAizfDLuvqW3Q9ncjrxJvBb1HMDntMv1xY";
        private const string BaseUrl = "https://api.monday.com/v2";

        private const string sc = "http://VELA.semcalaca.com:7077/BC2019_CPC/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/CocaHandshake";
        private const string sl = "http://VELA.semcalaca.com:7077/BC2019_CPC/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/CocaHandshake";

        [HttpGet]
        [Route("api/GetMondayDetails")]
        public async Task<IHttpActionResult> Post()
        {
            try
            {
                var items = await GetBoardItems();
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error processing webhook data: {ex.Message}");
            }
        }
        
        private async Task<IEnumerable> GetBoardItems()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", APIKey);
                httpClient.DefaultRequestHeaders.Add("API-Version", "2023-10");

                var query = "query { boards (ids: 5910428555) { groups { id title items_page { items { id name column_values { id text } } } } } }";
                //var query = "query { boards (ids: 5919506787) { id groups { id title items_page { cursor items { id name column_values { id text } } } } } }";
                var content = new StringContent(JsonConvert.SerializeObject(new { query }), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(BaseUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var mondayApiResponse = JsonConvert.DeserializeObject<MondayApiResponse>(responseData);


                    List<MondayData> dt = new List<MondayData>();
                    List<Item> itemList = mondayApiResponse.Data.Boards
                    .SelectMany(x => x.Groups
                        .Select(e => new
                        {
                            GroupId = e.Id,
                            GroupTitle = e.Title,
                            Items = e.Items_page.Items
                        }))
                    .SelectMany(c => c.Items
                        .Select(a => new Item
                        {
                            Id = a.Id,
                            Name = a.Name,
                            Column_values = a.Column_values,
                            GroupId = c.GroupId,
                            GroupTitle = c.GroupTitle
                        }))
                    .ToList();

                    foreach (var i in itemList)
                    {
                        dt.Add(new MondayData
                        {
                            GroupId = i.GroupId,
                            GroupName = i.GroupTitle,
                            ItemId = i.Id,
                            ItemName = i.Name,
                            PIC = i.Column_values[1].text,
                            Contractors = i.Column_values[2].text,
                            PlannedStartDate = i.Column_values[3].text,
                            PlannedEndDate = i.Column_values[4].text,
                            PlannedDaysDuration = i.Column_values[5].text,
                            TargetCompletionPercentage = i.Column_values[6].text,
                            ActualStartSate = i.Column_values[7].text,
                            ActualEndDate = i.Column_values[8].text,
                            ActualDaysDuration = i.Column_values[9].text,
                            ActualCompletionPercentage = i.Column_values[10].text,
                            Variance = i.Column_values[11].text,
                            ObservationRemarksStatus = i.Column_values[12].text,
                            LastUpdated = i.Column_values[13].text
                        });
                    }
                    return dt;
                }
                else
                {
                    throw new Exception($"Error from Monday.com API: {response.StatusCode}");
                }
            }
        }







        //// GET api/values
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/values/5
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/values
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/values/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/values/5
        //public void Delete(int id)
        //{
        //}


        [HttpGet]
        [Route("api/GetPRDetailsFromNAV")]
        public IHttpActionResult GetPRDetailsFromNAV()
        {
            string[] comp = { "SCPC", "SLPGC" };

            /* Decrypt sensitive parameters */

            //string sc = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/GetPRDetailsCOCA";
            //string sl = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/GetPRDetailsCOCA";
            var username = Properties.Resources.NavSemiraraUname;
            var password = Properties.Resources.NavSemiraraPass;
            List<PRDetail> prDetail = new List<PRDetail>();
            List<PRLineViewModel> prLine = new List<PRLineViewModel>();
            string url_link = "";

            foreach(var c in comp)
            {
                if (c == "SCPC")
                    url_link = sc;
                else
                    url_link = sl;


                /* Get Code Unit Properties */
                //GetPRDetailsFromNAV.GetPRDetailsCOCA codeunit_service = new GetPRDetailsFromNAV.GetPRDetailsCOCA();
                CocaHandshake.CocaHandshake codeunit_service = new CocaHandshake.CocaHandshake();
                NetworkCredential netCred; Uri uri; ICredentials credentials;
                codeunit_service.Url = url_link;
                codeunit_service.Timeout = 1800000;
                netCred = new NetworkCredential(@"" + username, password);
                uri = new Uri(codeunit_service.Url);
                credentials = netCred.GetCredential(uri, "Basic");
                codeunit_service.Credentials = credentials;
                codeunit_service.PreAuthenticate = true;

                //GetPRDetailsFromNAV.XMLPortForCOCAPRHeader exportItemData = new GetPRDetailsFromNAV.XMLPortForCOCAPRHeader();
                CocaHandshake.XMLPortForCOCAPRHeader exportItemData = new CocaHandshake.XMLPortForCOCAPRHeader();
                codeunit_service.GetPRForCOCA(ref exportItemData);

                // Get PR No. with WK item no
                if (exportItemData.PRLine != null)
                {
                    var items = exportItemData.PRLine;
                    var dt = items.Where(e => e.No_Line.Contains("WK") && e.PRNo_Line != "").ToList();
                    foreach (var item in dt)
                    {
                        prLine.Add(new PRLineViewModel()
                        {
                            PRNo = item.PRNo_Line,
                            No = item.No_Line
                        });
                    }
                }

                var prNo = prLine.Select(e => e.PRNo).Distinct().ToList();
                if (exportItemData.PRHeader != null)
                {
                    var items = exportItemData.PRHeader;
                    foreach (var i in prNo)
                    {
                        var item = items.LastOrDefault(e => e.PRNo == i);
                        if (item != null)
                        {
                            prDetail.Add(new PRDetail()
                            {
                                PRNo = item.PRNo,
                                PRDate = item.PRDate,
                                Department = item.Department,
                                IntendedFor = item.IntendedFor,
                                PlantNo = item.PlantNo,
                                WorkDescription = item.WorkDescription,
                                PurchaserCode = item.PurchaserCode,
                                PurchaserName = item.PurchaserName,
                                PRMonitoringStatus = item.PRMonitoringStatus,
                                PRApprovingStatus = item.PRApprovingStatus,
                                PRStatus = item.PRStatus,
                                PRType = item.PRType,
                                OutageCode = item.OutageCode,
                                OutageCodeDescription = item.OutageCodeDescription,
                                Priority = item.Priority,
                                PlantRelated = item.PlantRelated,
                                TechnicalReportRequired = item.TechnicalReportRequired,
                                ProjectLocation = item.ProjectLocation,
                                ProjectInCharge = item.ProjectInCharge,
                                BudgetTotalAmount = item.BudgetTotalAmount,
                                Company = c,
                                PRReleasedDate = item.PRReleasedDate
                            });
                        }
                    }
                }//End if null
            }
            return Ok(prDetail.ToList());
        }

        [HttpGet]
        [Route("api/GetPODetailsFromNAV")]
        public IHttpActionResult GetPODetailsFromNAV()
        {
            string[] comp = { "SCPC", "SLPGC" };


            /* Decrypt sensitive parameters */
            //string sc = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/GetPODetailsCOCA";
            //string sl = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/GetPODetailsCOCA";
            var username = Properties.Resources.NavSemiraraUname;
            var password = Properties.Resources.NavSemiraraPass;
            List<Models.PODetail> poDetail = new List<Models.PODetail>();
            List<Models.POLineViewModel> poLine = new List<Models.POLineViewModel>();
            string url_link = "";

            foreach (var c in comp)
            {
                if (c == "SCPC")
                    url_link = sc;
                else
                    url_link = sl;

                /* Get Code Unit Properties */
                //GetPODetailsFromNAV.GetPODetailsCOCA codeunit_service = new GetPODetailsFromNAV.GetPODetailsCOCA();
                CocaHandshake.CocaHandshake codeunit_service = new CocaHandshake.CocaHandshake();
                NetworkCredential netCred; Uri uri; ICredentials credentials;
                codeunit_service.Url = url_link;
                codeunit_service.Timeout = 1800000;
                netCred = new NetworkCredential(@"" + username, password);
                uri = new Uri(codeunit_service.Url);
                credentials = netCred.GetCredential(uri, "Basic");
                codeunit_service.Credentials = credentials;
                codeunit_service.PreAuthenticate = true;

                //GetPODetailsFromNAV.XMLPortForCOCAPOHeader exportItemData = new GetPODetailsFromNAV.XMLPortForCOCAPOHeader();
                CocaHandshake.XMLPortForCOCAPOHeader exportItemData = new CocaHandshake.XMLPortForCOCAPOHeader();
                codeunit_service.GetPOForCOCA(ref exportItemData);

                // Get PR No. with WK item no
                if (exportItemData.POLine != null)
                {
                    var items = exportItemData.POLine;
                    var dt = items.Where(e => e.No_Line.Contains("WK") && e.PRNo_Line != "").ToList();
                    foreach (var item in dt)
                    {
                        poLine.Add(new Models.POLineViewModel()
                        {
                            PRNo = item.PRNo_Line,
                            No = item.No_Line
                        });
                    }
                }

                var prNo = poLine.Select(e => e.PRNo).Distinct().ToList();
                //var prNo = poLine.Select(e => e.PRNo).ToList();

                if (exportItemData.POHeader != null)
                {
                    var items = exportItemData.POHeader;
                    foreach (var i in prNo)
                    {
                        var item = items.LastOrDefault(e => e.PRNo == i);
                        if (item != null)
                        {
                            poDetail.Add(new Models.PODetail()
                            {
                                PRNo = item.PRNo,
                                PONo = item.PONo,
                                DateArchived = item.DateArchived,
                                BuyFromVendorNo = item.BuyFromVendorNo,
                                BuyFromVendorName = item.BuyFromVendorName,
                                POStatus = item.POStatus,
                                POTotalLineAmount = item.POTotalLineAmount,
                                POBillingTerms = item.POBillingTerms,
                                NoOfProgressBilling = item.NoOfProgressBilling,
                                PBMilestone = item.PBMilestone,
                                VersionNo = item.VersionNo,
                                PaymentType = item.PaymentType,
                                Company = c
                            });
                        }
                    }
                }//End if null
            }
            return Ok(poDetail.ToList());
        }

        [HttpGet]
        [Route("api/GetPIDetailsFromNAV")]
        public IHttpActionResult GetPIDetailsFromNAV()
        {
            string[] comp = { "SCPC", "SLPGC" };

            /* Decrypt sensitive parameters */
            //string sc = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/GetPIDetailsCOCA";
            //string sl = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/GetPIDetailsCOCA";
            var username = Properties.Resources.NavSemiraraUname;
            var password = Properties.Resources.NavSemiraraPass;
            List<Models.PIDetail> piDetail = new List<Models.PIDetail>();
            string url_link = "";
            var poDetail = GetPODetailsFromNAV();

            foreach (var c in comp)
            {
                if (c == "SCPC")
                    url_link = sc;
                else
                    url_link = sl;

                /* Get Code Unit Properties */
                //GetPIDetailsFromNAV.GetPIDetailsCOCA codeunit_service = new GetPIDetailsFromNAV.GetPIDetailsCOCA();
                CocaHandshake.CocaHandshake codeunit_service = new CocaHandshake.CocaHandshake();
                NetworkCredential netCred; Uri uri; ICredentials credentials;
                codeunit_service.Url = url_link;
                codeunit_service.Timeout = 1800000;
                netCred = new NetworkCredential(@"" + username, password);
                uri = new Uri(codeunit_service.Url);
                credentials = netCred.GetCredential(uri, "Basic");
                codeunit_service.Credentials = credentials;
                codeunit_service.PreAuthenticate = true;

                //GetPIDetailsFromNAV.XMLPortForCOCAPIHeader exportItemData = new GetPIDetailsFromNAV.XMLPortForCOCAPIHeader();
                CocaHandshake.XMLPortForCOCAPIHeader exportItemData = new CocaHandshake.XMLPortForCOCAPIHeader();
                codeunit_service.GetPIForCOCA(ref exportItemData);
                
                if (exportItemData.PIHeader != null)
                {
                    var items = exportItemData.PIHeader;
                    foreach (var item in items)
                    {
                        if (item.PONo != "")
                        {
                            piDetail.Add(new Models.PIDetail()
                            {
                                PPINNo = item.PPINNo,
                                InvoiceDate = item.InvoiceDate,
                                InvoiceAmount = item.InvoiceAmount,
                                PONo = item.PONo,
                                Company = c
                            });
                        }
                    }
                }
            }
            return Ok(piDetail.ToList());
        }

        [HttpGet]
        [Route("api/GetViolationsFromCDS")]
        public IHttpActionResult GetViolationsFromCDS()
        {
            var conString = "Database=CDSDb;Server=192.168.70.231;user=ict;password=ict@ictdept";
            string query = "select n.PONo" +
                        ", i.CardNo as IdNo " +
                        ", CONCAT(i.FirstName, ' ', i.LastName) as ContractorName" +
                        ", vt.ViolationTypeName " +
                        ", v.Description " +
                        ", e.ViolationDate " +
                        ", e.ViolationOffense " +
                        ", e.ViolationEffectivityDate " +
                        ", e.ViolationOffense " +
                        ", v.FirstPenalty " +
                        ", v.SecondPenalty " +
                        ", v.ThirdPenalty " +
                        ", v.SucceedingPenalty " +
                        "from InfoGenerals i " +
                        "left join ViolationEmployees e on e.InfoGeneralId = i.Id " +
                        "left join NavPoReleasedTable n on n.Id = i.WorkOrderNo " +
                        "left join Violations v on e.ViolationsId = v.Id " +
                        "left join ViolationTypes vt on vt.Id = v.ViolationTypeId " +
                        "where vt.ViolationTypeName != ''";
            SqlConnection conn = new SqlConnection(conString);
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDa.Fill(dt);

            try
            {
                if(dt.Rows.Count <= 0) {
                    throw new Exception("No results found");
                }
                var arr = new List<ViolationViewModel>();

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    int offense = Convert.ToInt32(dt.Rows[i]["ViolationOffense"]);
                    if (offense >= 1)
                    {
                        for(var j = 1; j <= offense; j++)
                        {
                            arr.Add(new ViolationViewModel
                            {
                                IdNo = dt.Rows[i]["IdNo"].ToString(),
                                PONo = dt.Rows[i]["PONo"].ToString(),
                                ViolationType = dt.Rows[i]["ViolationTypeName"].ToString(),
                                ViolationDescription = dt.Rows[i]["Description"].ToString(),
                                ViolationDate = dt.Rows[i]["ViolationDate"].ToString(),
                                NoOfOffense = (j == 1 ? "First Offense" : (j == 2 ? "Second Offense" : (j == 3 ? "Third Offense" : "Fourth Offense"))),
                                MonetaryPenalty = (j == 1 ? dt.Rows[i]["FirstPenalty"].ToString() : (j == 2 ? dt.Rows[i]["SecondPenalty"].ToString() : (j == 3 ? dt.Rows[i]["ThirdPenalty"].ToString() : dt.Rows[i]["SucceedingPenalty"].ToString()))),
                                ProcessDate = dt.Rows[i]["ViolationEffectivityDate"].ToString()
                            });
                        }
                    }
                }

                conn.Close();
                return Ok(arr);
            }
            catch(Exception e)
            {
                var result = e.Message;
                return Ok(result);
            }
        }

    }
}
