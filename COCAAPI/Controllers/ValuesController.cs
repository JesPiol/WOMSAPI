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
        //private const string APIKey = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjI5Nzg5NzM0OCwiYWFpIjoxMSwidWlkIjo0MzA0NTkxMCwiaWFkIjoiMjAyMy0xMS0yMlQwMjo1MDoxOS4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6MTY4MzU1MzEsInJnbiI6InVzZTEifQ._rkd67FZYN5UA2SYaNUU232R5us4gR3QhpWrESAhpg0"; // from sir louie
        //private const string APIKey = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjMxNjQ5MjYzOCwiYWFpIjoxMSwidWlkIjo0NDA5OTk0MCwiaWFkIjoiMjAyNC0wMi0wMVQwNjoxNDoxNS4yMDdaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6MTY4MzU1MzEsInJnbiI6InVzZTEifQ.fnlrVv7skPAizfDLuvqW3Q9ncjrxJvBb1HMDntMv1xY"; 
        //private const string APIKey = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjI4NTkzOTIwMywiYWFpIjoxMSwidWlkIjo0NTMzMDE3NiwiaWFkIjoiMjAyMy0xMC0wM1QwNjozODoxMS4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6MTY4MzU1MzEsInJnbiI6InVzZTEifQ.o8D20zMcXEmjcKZfGpkquB596SEBXhWIiVLaiFjtp_s"; // from sir andy
        private const string APIKey = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjMyNzQ4MDY4MSwiYWFpIjoxMSwidWlkIjo1NjQ4OTgyMywiaWFkIjoiMjAyNC0wMy0wMVQwOTozMjoxNC4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6MjE1NTI0NjUsInJnbiI6InVzZTEifQ.eO4ZiaQtUfqfdq_9KBmkk1-vrFjLmEoNISbIrl9LADw"; // from sir andy semcalaca account
        private const string BaseUrl = "https://api.monday.com/v2";

        private const string sc = "http://VELA.semcalaca.com:7077/BC2019_CPC/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/CocaHandshake";
        private const string sl = "http://VELA.semcalaca.com:7077/BC2019_CPC/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/CocaHandshake";

        public List<MondayData> GetBoardIds()
        {
            var conString = "Database=COCAWEBDB;Server=192.168.70.231;user=ict;password=ict@ictdept";
            string query = @"select po.PONo, po.BoardId from PODetails po where po.BoardId != ''";
            SqlConnection conn = new SqlConnection(conString);
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDa.Fill(dt);
            
            var arr = new List<MondayData>();

            for (var i = 0; i < dt.Rows.Count; i++)
            {
                arr.Add(new MondayData
                {
                    PONo = dt.Rows[i]["PONo"].ToString(),
                    BoardId = dt.Rows[i]["BoardId"].ToString()
                });
            }

            conn.Close();
            return arr.ToList();
            
        }

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
        
        [HttpPost]
        private async Task<IEnumerable> GetBoardItems()
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", APIKey);
                httpClient.DefaultRequestHeaders.Add("API-Version", "2023-10");
                List<MondayDataViewModel2> dt = new List<MondayDataViewModel2>();

                //var bIds = GetBoardIds();
                //var items = bIds.Select(e => e.BoardId).Distinct();

                //foreach (var b in items)
                //{

                var query = "query { boards (ids: [6160972389, 6160972749, 6160973083, 6180267129, 6180267169, 6180267185, " +
                    "6180267195, 6180267212, 6182803567, 6180267158, 6182803520, 6192899046, 6197077439, " +
                    "6211122876, 6218807001, 6226501254, 6226523192, 6228653892, 6231193068, 6239496443 " +
                    "6219606761, 6239807475]) " +
                    "{ items_page { items { id name group { id title } " +
                                "column_values { id text } } } } }";

                //var query = "query { boards (ids: [6160972389]) " +
                //   "{ items_page { items { id name group { id title } " +
                //               "column_values { id text } } } } }";
                var content = new StringContent(JsonConvert.SerializeObject(new { query }), Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(BaseUrl, content).ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var mondayApiResponse = JsonConvert.DeserializeObject<MondayApiResponse>(responseData);

                    List<Item> itemList = mondayApiResponse.Data.Boards
                        .SelectMany(board => board.Items_page.Items
                        .Select(item => new Item
                        {
                            Id = item.Id,
                            Name = item.Name,
                            Column_values = item.Column_values,
                            Group = item.Group
                        })).ToList();

                    //int TargetCompletionPercentage = 0;
                    //int ActualCompletionPercentage = 0;
                    //int TotalPlannedDaysDuration = 0;
                    //int TotalActualDaysDuration = 0;

                    //decimal WeightedPercentage = 0;

                    foreach (var i in itemList)
                    {
                        var weightPercentage = Convert.ToDecimal(i.Column_values[8].Text) / 78;
                        try
                        {
                            dt.Add(new MondayDataViewModel2
                            {
                                GroupName = i.Group.Title,
                                ItemName = i.Name,
                                PONo = i.Column_values[1].Text,
                                DateSubmitted = i.Column_values[2].Text,
                                ID = i.Column_values[3].Text,
                                IDName = i.Column_values[4].Text,
                                MilestoneGroup = i.Column_values[5].Text,
                                MilestoneName = i.Column_values[6].Text,
                                TaskNo = i.Column_values[7].Text,
                                PlannedDaysDuration = i.Column_values[8].Text,
                                WeightedPercentage = i.Column_values[9].Text == "" ? weightPercentage.ToString() : i.Column_values[9].Text,
                                PlannedStartDate = i.Column_values[10].Text,
                                PlannedEndDate = i.Column_values[11].Text,
                                TargetPercentCompletion = i.Column_values[12].Text,
                                TargetRelativeWeight = i.Column_values[13].Text,
                                ActualStartDate = i.Column_values[14].Text,
                                ActualEndDate = i.Column_values[15].Text,
                                ActualPercentCompletion = i.Column_values[16].Text,
                                ActualRelativeWeight = i.Column_values[17].Text,
                                PlannedStartValidation = i.Column_values[18].Text,
                                ActualEndValidation = i.Column_values[19].Text,
                                DPRDate = i.Column_values[20].Text,
                                MainStatus = i.Column_values[21].Text,
                                InspectionRequirement = i.Column_values[22].Text,
                                ITPStatus = i.Column_values[23].Text,
                                RemarksForITP = i.Column_values[24].Text,
                                ReferenceITP = i.Column_values[25].Text,
                                UploadSignedITP = i.Column_values[26].Text,
                                ProgressPhotos = i.Column_values[27].Text,
                                MilestoneItem = i.Column_values[28].Text,
                                MilestoneDetails = i.Column_values[29].Text,
                                PaymentTermsCode = i.Column_values[30].Text,
                                LastUpdated = i.Column_values[31].Text
                            });
                        }
                        catch(Exception e)
                        {
                            var test = i;
                        }
                        

                        //TimeSpan planned_ts = i.Column_values[7].Text == "" || i.Column_values[6].Text == "" ?
                        //    TimeSpan.FromSeconds(2) : Convert.ToDateTime(i.Column_values[7].Text) - Convert.ToDateTime(i.Column_values[6].Text);
                        //var plannedDaysDuration = planned_ts.Days;

                        //TotalPlannedDaysDuration += plannedDaysDuration;

                        //TimeSpan actual_ts = i.Column_values[10].Text == "" || i.Column_values[9].Text == "" ? 
                        //    TimeSpan.FromSeconds(2) : (Convert.ToDateTime(i.Column_values[10].Text) - Convert.ToDateTime(i.Column_values[9].Text));
                        //var actualDaysDuration = actual_ts.Days;

                        //TotalActualDaysDuration += actualDaysDuration;
                        
                        //foreach (var sub in i.Groups)
                        //{
                        //    if (i.Groups.Count() > 0)
                        //    {
                                //    // Calculate the value of weighted percentage
                                //    // Assuming Sub_WeightedPercentage contains comma-separated numeric values
                                //    var weightedPercentageText = i.Subitems.Select(e => e.Column_values[7]).ToList();
                                //    //string[] weightedPercentageValues = weightedPercentageText.Split(',');

                                //    foreach (var item in weightedPercentageText)
                                //    {
                                //        // Add the parsed weighted percentage to the total
                                //        WeightedPercentage += Convert.ToDecimal(item.Text);
                                //    }

                                //dt.Add(new MondayDataViewModel
                                //{
                                //    GroupId = i.GroupId,
                                //    GroupName = i.GroupTitle,
                                //    ItemId = i.Id,
                                //    ItemName = i.Name,
                                //    WorkGroup = i.Column_values[1].Text,
                                //    PIC = i.Column_values[2].Text,
                                //    SectionHead = i.Column_values[3].Text,
                                //    Contractor = i.Column_values[4].Text,
                                //    ContractorPIC = i.Column_values[5].Text,
                                //    PlannedStartDate = i.Column_values[6].Text,
                                //    PlannedEndDate = i.Column_values[7].Text,
                                //    PlannedDaysDuration = plannedDaysDuration.ToString(),
                                //    PlannedPercentageAccomplishment = i.Column_values[12].Text,
                                //    ActualStartDate = i.Column_values[9].Text,
                                //    ActualEndDate = i.Column_values[10].Text,
                                //    ActualDaysDuration = actualDaysDuration.ToString(),
                                //    ActualPercentageAccomplishment = i.Column_values[13].Text,
                                //    Variance = i.Column_values[14].Text,
                                //    ObservationRemarksStatus = i.Column_values[15].Text,
                                //    LastUpdated = i.Column_values[16].Text,
                                //    ProgressPicture = i.Column_values[17].Text,
                                //    ITPDocs = i.Column_values[18].Text,
                                //    JobCardNo = i.Column_values[19].Text,
                                //    Sub_ItemId = sub.Id,
                                //    Sub_ItemName = sub.Name,
                                //    Sub_PONo = sub.Column_values[0].Text,
                                //    Sub_DateSubmit = sub.Column_values[1].Text,
                                //    Sub_Id = sub.Column_values[2].Text,
                                //    Sub_IdName = sub.Column_values[3].Text,
                                //    Sub_MilestoneGroup = sub.Column_values[4].Text,
                                //    Sub_MilestoneName = sub.Column_values[5].Text,
                                //    Sub_TaskNo = sub.Column_values[6].Text,
                                //    Sub_PlannedDaysDuration = sub.Column_values[7].Text,
                                //    //Sub_WeightedPercentage = (Convert.ToInt32(sub.Column_values[7].Text) / WeightedPercentage).ToString("#0.000"),
                                //    Sub_WeightedPercentage = sub.Column_values[8].Text,
                                //    Sub_PlannedStartDate = sub.Column_values[9].Text,
                                //    Sub_PlannedEndDate = sub.Column_values[10].Text,
                                //    Sub_TargetCompletionPercentage = sub.Column_values[11].Text,
                                //    Sub_TargetRelativeWeight = sub.Column_values[12].Text,
                                //    Sub_ActualStartDate = sub.Column_values[13].Text == "" ? "-" : sub.Column_values[13].Text,
                                //    Sub_ActualEndDate = sub.Column_values[14].Text == "" ? "-" : sub.Column_values[14].Text,
                                //    Sub_ActualCompletionPercentage = sub.Column_values[15].Text,
                                //    Sub_ActualRelativeWeight = sub.Column_values[16].Text,
                                //    Sub_PlannedStartValidation = sub.Column_values[17].Text,
                                //    Sub_ActualEndValidation = sub.Column_values[18].Text,
                                //    Sub_DPRDate = sub.Column_values[19].Text,
                                //    Sub_MainStatus = sub.Column_values[20].Text == "" ? "-" : sub.Column_values[20].Text,
                                //    Sub_InspectionRequirement = sub.Column_values[21].Text,
                                //    Sub_ITPStatus = sub.Column_values[22].Text,
                                //    Sub_ITPRemarks = sub.Column_values[23].Text,
                                //    Sub_ITPReference = sub.Column_values[24].Text,
                                //    Sub_ITPUploadSigned = sub.Column_values[25].Text,
                                //    Sub_MilestoneItem = sub.Column_values[26].Text,
                                //    Sub_MilestoneDetails = sub.Column_values[27].Text,
                                //    Sub_PaymentTermsCode = sub.Column_values[28].Text,
                                //    Sub_LastUpdated = sub.Column_values[29].Text,

                                //});
                            //}
                                
                        //}
                    }
                    
                }
                else
                {
                    throw new Exception($"Error from Monday.com API: {response.StatusCode}");
                }
                //}
                return dt;
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

            //string sc1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/CocaHandshake";
            //string sl1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/CocaHandshake";
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
                CocaHandshake.CocaHandshake codeunit_service = new CocaHandshake.CocaHandshake();
                //CocaHandshake_.CocaHandshake codeunit_service = new CocaHandshake_.CocaHandshake();
                NetworkCredential netCred; Uri uri; ICredentials credentials;
                codeunit_service.Url = url_link;
                codeunit_service.Timeout = 1800000;
                netCred = new NetworkCredential(@"" + username, password);
                uri = new Uri(codeunit_service.Url);
                credentials = netCred.GetCredential(uri, "Basic");
                codeunit_service.Credentials = credentials;
                codeunit_service.PreAuthenticate = true;

                //CocaHandshake_.XMLPortForCOCAPRHeader exportItemData = new CocaHandshake_.XMLPortForCOCAPRHeader();
                CocaHandshake.XMLPortForCOCAPRHeader exportItemData = new CocaHandshake.XMLPortForCOCAPRHeader();
                codeunit_service.GetPRForCOCA(ref exportItemData);

                // Get PR No. with WK item no
                var items = exportItemData.PRHeader.ToList();

                if (items != null)
                {
                    var prLineData = items.Where(e => e.PRLine[0].No_Line.Contains("WK"));
                    foreach (var item in prLineData)
                    {
                        prLine.Add(new PRLineViewModel()
                        {
                            PRNo = item.PRLine[0].PRNo_Line,
                            No = item.PRLine[0].No_Line
                        });
                    }
                }

                var prNo = prLine.Select(e => e.PRNo).Distinct().ToList();
                if (exportItemData.PRHeader != null)
                {
                    foreach (var i in prNo)
                    {
                        var item = items.LastOrDefault(e => e.PRNo == i);
                        if (item != null)
                        {
                            prDetail.Add(new PRDetail()
                            {
                                PRNo = item.PRNo,
                                PRDate = item.PRDate == "" || item.PRDate == null ? "" : Convert.ToDateTime(item.PRDate).ToString("MM/dd/yyyy"),
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
                                PRReleasedDate = item.PRReleasedDate == "" || item.PRReleasedDate == null ? "" : Convert.ToDateTime(item.PRReleasedDate).ToString("MM/dd/yyyy"),
                            });
                        }
                    }
                }
            }
            return Ok(prDetail.ToList());
        }

        [HttpGet]
        [Route("api/GetPODetailsFromNAV")]
        public IHttpActionResult GetPODetailsFromNAV()
        {
            string[] comp = { "SCPC", "SLPGC" };


            /* Decrypt sensitive parameters */
            string sc1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/CocaHandshake";
            string sl1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/CocaHandshake";
            var username = Properties.Resources.NavSemiraraUname;
            var password = Properties.Resources.NavSemiraraPass;
            List<Models.PODetail> poDetail = new List<Models.PODetail>();
            List<Models.POLineViewModel> poLine = new List<Models.POLineViewModel>();
            string url_link = "";

            foreach (var c in comp)
            {
                if (c == "SCPC")
                    url_link = sc1;
                else
                    url_link = sl1;

                // Instantiate CocaHandshake service
                //CocaHandshake.CocaHandshake codeunit_service = new CocaHandshake.CocaHandshake();
                CocaHandshake_.CocaHandshake codeunit_service = new CocaHandshake_.CocaHandshake(); // test server

                // Set URL and Timeout
                codeunit_service.Url = url_link;
                codeunit_service.Timeout = 1800000;

                // Set credentials
                codeunit_service.Credentials = new NetworkCredential(username, password);
                codeunit_service.PreAuthenticate = true;

                // Call the method directly
                //CocaHandshake.XMLPortForCOCAPOHeader exportItemData = new CocaHandshake.XMLPortForCOCAPOHeader();
                CocaHandshake_.XMLPortForCOCAPOHeader exportItemData = new CocaHandshake_.XMLPortForCOCAPOHeader(); // test server
                codeunit_service.GetPOForCOCA(ref exportItemData);

                var items = exportItemData.POHeader.ToList();
                
                if (items != null)
                {
                    var poLineData = items.Where(e => e.POLine[0].No_Line.Contains("WK"));
                    foreach (var item in poLineData)
                    {
                        poLine.Add(new Models.POLineViewModel()
                        {
                            PRNo = item.POLine[0].PRNo_Line,
                            No = item.POLine[0].No_Line
                        });
                    }
                }

                var prNo = poLine.Select(e => e.PRNo).Distinct().ToList();

                if (exportItemData.POHeader != null)
                {
                    var dt = exportItemData.POHeader;
                    foreach (var i in prNo)
                    {
                        var item = items.FirstOrDefault(e => e.PRNo == i);
                        if (item != null)
                        {
                            poDetail.Add(new Models.PODetail()
                            {
                                PRNo = item.PRNo,
                                PONo = item.PONo,
                                DateArchived = item.DateArchived == "" || item.DateArchived == null ? "" : Convert.ToDateTime(item.DateArchived).ToString("MM/dd/yyyy"),
                                BuyFromVendorNo = item.BuyFromVendorNo,
                                BuyFromVendorName = item.BuyFromVendorName,
                                POStatus = item.POStatus,
                                POTotalLineAmount = item.POTotalLineAmount,
                                POBillingTerms = item.POBillingTerms,
                                NoOfProgressBilling = item.NoOfProgressBilling,
                                PBMilestone = item.PBMilestone,
                                VersionNo = item.VersionNo,
                                PaymentType = item.PaymentType,
                                Company = c,
                                PlantNo = item.PlantNo,
                                OrderDate = item.OrderDate == "" || item.OrderDate == null ? "" : Convert.ToDateTime(item.OrderDate).ToString("MM/dd/yyyy"),
                                POPaymentTerms = item.POPaymentTermCode,
                                POContractAmount = "",
                                POAmendedAmount = "",
                                RetentionPercent = "",
                                RetentionAmount = "",
                                RetentionReleaseDate = "",
                                ProgressBillingPercent = ""
                            });
                        }
                    }
                }
            }
            return Ok(poDetail.ToList());
        }

        [HttpGet]
        [Route("api/GetPIDetailsFromNAV")]
        public IHttpActionResult GetPIDetailsFromNAV()
        {
            string[] comp = { "SCPC", "SLPGC" };

            /* Decrypt sensitive parameters */
            //string sc1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/CocaHandshake";
            //string sl1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/CocaHandshake";
            var username = Properties.Resources.NavSemiraraUname;
            var password = Properties.Resources.NavSemiraraPass;
            List<Models.PIDetail> piDetail = new List<Models.PIDetail>();
            string url_link = "";

            foreach (var c in comp)
            {
                if (c == "SCPC")
                    url_link = sc;
                else
                    url_link = sl;

                /* Get Code Unit Properties */
                //CocaHandshake_.CocaHandshake codeunit_service = new CocaHandshake_.CocaHandshake();
                CocaHandshake.CocaHandshake codeunit_service = new CocaHandshake.CocaHandshake();
                NetworkCredential netCred; Uri uri; ICredentials credentials;
                codeunit_service.Url = url_link;
                codeunit_service.Timeout = 1800000;
                netCred = new NetworkCredential(@"" + username, password);
                uri = new Uri(codeunit_service.Url);
                credentials = netCred.GetCredential(uri, "Basic");
                codeunit_service.Credentials = credentials;
                codeunit_service.PreAuthenticate = true;

                //CocaHandshake_.XMLPortForCOCAPIHeader exportItemData = new CocaHandshake_.XMLPortForCOCAPIHeader();
                CocaHandshake.XMLPortForCOCAPIHeader exportItemData = new CocaHandshake.XMLPortForCOCAPIHeader();
                codeunit_service.GetPIForCOCA(ref exportItemData);
                
                if (exportItemData.PIHeader != null)
                {
                    var conString = "Database=COCAWEBDB;Server=192.168.70.231;user=ict;password=ict@ictdept";
                    string query = "select PONo from PODetails";
                    SqlConnection conn = new SqlConnection(conString);
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    var arr = new List<PODetail>();
                    sqlDa.Fill(dt);

                    for (var i = 0; i < dt.Rows.Count; i++)
                    {
                        arr.Add(new PODetail
                        {
                            PONo = dt.Rows[i]["PONo"].ToString()
                        });
                    }
                    var poNo = arr.Select(e => e.PONo).ToList();


                    var items = exportItemData.PIHeader;
                    var data = items.Where(e => poNo.Contains(e.PONo)).ToList();

                    // For Vendor Ledger Entry
                    var vlEntry = exportItemData.PIHeader[0].VendorLedgerEntry.ToList();

                    // For CV Entry
                    var cvEntry = exportItemData.CheckLedgerEntry;

                    foreach (var item in data)
                    {
                        string cvDate = "";
                        string cvStatus = "";
                        string cvAmount = "0";
                        var _vlEntry = vlEntry.FirstOrDefault(e => e.DocumentNo == item.PPINNo);
                        var cvRefNo = _vlEntry == null ? "" : (_vlEntry.ClosedByEntryNo == 0 ? _vlEntry.AppliesToId : _vlEntry.ClosedByEntryNo.ToString());
                        var _vlEntry2 = vlEntry.FirstOrDefault(e => e.EntryNo.ToString() == cvRefNo);
                        if (_vlEntry2 != null)
                        {
                            var _clEntry = cvEntry.FirstOrDefault(e => e.CheckNo == _vlEntry2.DocumentNo);
                            cvRefNo = _clEntry == null ? "" : _clEntry.CheckNo;
                            cvDate = _clEntry == null ? "" : _clEntry.CheckDate;
                            cvStatus = _clEntry == null ? "" : _clEntry.EntryStatus;
                            cvAmount = _clEntry == null ? "0" : _clEntry.Amount;
                        }

                        if (item.PONo != "")
                        {
                            piDetail.Add(new Models.PIDetail()
                            {
                                PPINNo = item.PPINNo,
                                InvoiceDate = item.InvoiceDate == "" ? "" : Convert.ToDateTime(item.InvoiceDate).ToString("MM/dd/yyyy"),
                                InvoiceAmount = item.InvoiceAmount,
                                PONo = item.PONo,
                                Company = c,
                                CVReferenceNo = cvRefNo,
                                PIPaymentTerms = item.PIPaymentTermCode,
                                InvoiceNo = item.VendorInvoiceNo,
                                InvoiceAmountIncVAT = item.AmountIncludingVAT,
                                WorkPenalties = "0",
                                AmountPayable = "0",
                                PPINDate = item.PostingDate == "" ? "" : Convert.ToDateTime(item.PostingDate).ToString("MM/dd/yyyy"),
                                CVDate = cvDate == "" ? "" : Convert.ToDateTime(cvDate).ToString("MM/dd/yyyy"),
                                CVStatus = cvStatus,
                                CVAmount = cvAmount
                            });
                        }
                    }
                }
            }
            return Ok(piDetail.ToList());
        }

        //[HttpGet]
        //[Route("api/GetPOLineDetailsFromNAV")]
        //public IHttpActionResult GetPOLineDetailsFromNAV()
        //{
        //    string[] comp = { "SCPC", "SLPGC" };

        //    /* Decrypt sensitive parameters */
        //    //string _sc = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/CocaHandshake";
        //    //string _sl = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/CocaHandshake";
        //    var username = Properties.Resources.NavSemiraraUname;
        //    var password = Properties.Resources.NavSemiraraPass;
        //    List<Models.POLineViewModel> poLineDetail = new List<Models.POLineViewModel>();
        //    string url_link = "";

        //    foreach (var c in comp)
        //    {
        //        if (c == "SCPC")
        //            url_link = sc;
        //        else
        //            url_link = sl;

        //        /* Get Code Unit Properties */
        //        CocaHandshake.CocaHandshake codeunit_service = new CocaHandshake.CocaHandshake();
        //        //CocaHandshake_.CocaHandshake codeunit_service = new CocaHandshake_.CocaHandshake();
        //        NetworkCredential netCred; Uri uri; ICredentials credentials;
        //        codeunit_service.Url = url_link;
        //        codeunit_service.Timeout = 1800000;
        //        netCred = new NetworkCredential(@"" + username, password);
        //        uri = new Uri(codeunit_service.Url);
        //        credentials = netCred.GetCredential(uri, "Basic");
        //        codeunit_service.Credentials = credentials;
        //        codeunit_service.PreAuthenticate = true;

        //        //CocaHandshake_.XMLPortForCOCAPOHeader exportItemData = new CocaHandshake_.XMLPortForCOCAPOHeader();
        //        CocaHandshake.XMLPortForCOCAPOHeader exportItemData = new CocaHandshake.XMLPortForCOCAPOHeader();
        //        codeunit_service.GetPOForCOCA(ref exportItemData);

        //        if (exportItemData.POHeader != null)
        //        {
        //            var conString = "Database=COCAWEBDB;Server=192.168.70.231;user=ict;password=ict@ictdept";
        //            string query = "select PRNo from PODetails";
        //            SqlConnection conn = new SqlConnection(conString);
        //            conn.Open();
        //            SqlCommand cmd = new SqlCommand(query, conn);
        //            SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
        //            DataTable dt = new DataTable();
        //            var arr = new List<PODetail>();
        //            sqlDa.Fill(dt);

        //            for (var i = 0; i < dt.Rows.Count; i++)
        //            {
        //                arr.Add(new PODetail
        //                {
        //                    PRNo = dt.Rows[i]["PRNo"].ToString()
        //                });
        //            }
        //            var prNo = arr.Select(e => e.PRNo).ToList();


        //            var items = exportItemData.POLine;
        //            var data = items.Where(e => prNo.Contains(e.PRNo_Line)).ToList();

        //            foreach (var item in data)
        //            {
        //                if (item.PRNo_Line != "")
        //                {
        //                    poLineDetail.Add(new Models.POLineViewModel()
        //                    {
        //                        DocumentNo = "",
        //                        No = item.No_Line,
        //                        PRNo = item.PRNo_Line,
        //                        Description = item.Description,
        //                        Description2 = item.Description2,
        //                        Description3 = "",
        //                        WarrantyDurationEndDate = "",
        //                        WarrantyDurationStartDate = ""
        //                    });
        //                }
        //            }
        //        }
        //    }
        //    return Ok(poLineDetail.ToList());
        //}

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
                        "where vt.ViolationTypeName != '' and n.PONo != ''";
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
                    arr.Add(new ViolationViewModel
                    {
                        IdNo = dt.Rows[i]["IdNo"].ToString(),
                        PONo = dt.Rows[i]["PONo"].ToString(),
                        ViolationType = dt.Rows[i]["ViolationTypeName"].ToString(),
                        ViolationDescription = dt.Rows[i]["Description"].ToString(),
                        ViolationDate = dt.Rows[i]["ViolationDate"].ToString(),
                        NoOfOffense = (offense == 1 ? "First Offense" : (offense == 2 ? "Second Offense" : (offense == 3 ? "Third Offense" : "Fourth Offense"))),
                        MonetaryPenalty = (offense == 1 ? dt.Rows[i]["FirstPenalty"].ToString() : (offense == 2 ? dt.Rows[i]["SecondPenalty"].ToString() : (offense == 3 ? dt.Rows[i]["ThirdPenalty"].ToString() : dt.Rows[i]["SucceedingPenalty"].ToString()))),
                        ProcessDate = dt.Rows[i]["ViolationEffectivityDate"].ToString()
                    });
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


        [HttpGet]
        [Route("api/GetBorrowedTools")]
        public IHttpActionResult GetBorrowedTools()
        {
            var conString = "Database=TMS;Server=192.168.70.231;user=ict;password=ict@ictdept";
            string query = @"select c.ItemCode, b.ToolCode, a.WorkOrder, b.Quantity, b.DateIssued, b.UnitCost
                    , CASE 
                            WHEN a.DocStatus = 0 THEN 'Open'
                            WHEN a.DocStatus = 1 THEN 'Released'
                            WHEN a.DocStatus = 2 THEN 'Closed'
                            ELSE 'Cancelled'
                        END AS DocStatus
	                    , b.QuantityTransferred
	                    ,e.Quantity as Quantity2
	                    , e.Status, c.Description
	                    , c.Description2
                    from AFBorrower a
                    left join AFBorrowerIssue b on b.AFBorrowerId = a.Id
                    left join Item c on c.Id = b.ItemID
                    left join ItemDetail d on d.ItemId = c.Id
                    left join AFBorrowerReturn e on e.AFBorrowerIssueID = b.id 
                    group by c.ItemCode, b.ToolCode, a.WorkOrder, b.Quantity, b.DateIssued, b.UnitCost, a.DocStatus, b.QuantityTransferred
	                ,e.Quantity, e.Status, c.Description
	, c.Description2";
            SqlConnection conn = new SqlConnection(conString);
            conn.Open();
            SqlCommand cmd = new SqlCommand(query, conn);
            SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sqlDa.Fill(dt);

            try
            {
                if (dt.Rows.Count <= 0)
                {
                    throw new Exception("No results found");
                }
                var arr = new List<ToolsViewModel>();

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var aaa = dt.Rows[i]["QuantityTransferred"].ToString();
                    var QuantityTransferred = dt.Rows[i]["QuantityTransferred"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[i]["QuantityTransferred"].ToString());
                    var Quantity2 = dt.Rows[i]["Quantity2"].ToString() == "" ? 0 : Convert.ToInt32(dt.Rows[i]["Quantity2"].ToString());
                    var QuantityRemaining = Quantity2 - QuantityTransferred;

                    arr.Add(new ToolsViewModel
                    {
                        ItemCode = dt.Rows[i]["ItemCode"].ToString(),
                        Description1 = dt.Rows[i]["Description"].ToString(),
                        Description2 = dt.Rows[i]["Description2"].ToString(),
                        ToolCode = dt.Rows[i]["ToolCode"].ToString(),
                        PONo = dt.Rows[i]["WorkOrder"].ToString(),
                        QuantityIssued = dt.Rows[i]["Quantity"].ToString(),
                        DateIssued = dt.Rows[i]["DateIssued"].ToString(),
                        UnitCost = dt.Rows[i]["UnitCost"].ToString(),
                        DocStatus = dt.Rows[i]["DocStatus"].ToString(),
                        QuantityRemaining = QuantityRemaining.ToString()
                    });
                }

                conn.Close();
                return Ok(arr);
            }
            catch (Exception e)
            {
                var result = e.Message;
                return Ok(result);
            }
        }

        [HttpGet]
        [Route("api/GetDepartmentListFromNAV")]
        public IHttpActionResult GetDepartmentListFromNAV()
        {
            string[] comp = { "SCPC", "SLPGC" };


            /* Decrypt sensitive parameters */
            string sc1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/CocaHandshake";
            string sl1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/CocaHandshake";
            var username = Properties.Resources.NavSemiraraUname;
            var password = Properties.Resources.NavSemiraraPass;
            List<Models.Department> department = new List<Models.Department>();
            string url_link = "";

            foreach (var c in comp)
            {
                if (c == "SCPC")
                    url_link = sc1;
                else
                    url_link = sl1;

                /* Get Code Unit Properties */
                CocaHandshake_.CocaHandshake codeunit_service = new CocaHandshake_.CocaHandshake();
                NetworkCredential netCred; Uri uri; ICredentials credentials;
                codeunit_service.Url = url_link;
                codeunit_service.Timeout = 1800000;
                netCred = new NetworkCredential(@"" + username, password);
                uri = new Uri(codeunit_service.Url);
                credentials = netCred.GetCredential(uri, "Basic");
                codeunit_service.Credentials = credentials;
                codeunit_service.PreAuthenticate = true;

                CocaHandshake_.XmlPortForCocaDept exportItemData = new CocaHandshake_.XmlPortForCocaDept();
                codeunit_service.GetDeptForCOCA(ref exportItemData);

                if (exportItemData.DepartmentList != null)
                {
                    var items = exportItemData.DepartmentList;
                    foreach (var i in items)
                    {
                        if (i != null)
                        {
                            department.Add(new Models.Department()
                            {
                                DepartmentCode = i.DepartmentCode,
                                DepartmentName = i.DepartmentName,
                                Company = c
                            });
                        }
                    }
                }//End if null
            }
            return Ok(department.ToList());
        }

        //[HttpGet]
        //[Route("api/ImportPODetails")]
        //public IHttpActionResult ImportPODetails(string PRNo,
        //    )
        //{

        //}

    }
}
