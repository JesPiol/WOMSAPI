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
        private const string APIKey = "eyJhbGciOiJIUzI1NiJ9.eyJ0aWQiOjI4NTkzOTIwMywiYWFpIjoxMSwidWlkIjo0NTMzMDE3NiwiaWFkIjoiMjAyMy0xMC0wM1QwNjozODoxMS4wMDBaIiwicGVyIjoibWU6d3JpdGUiLCJhY3RpZCI6MTY4MzU1MzEsInJnbiI6InVzZTEifQ.o8D20zMcXEmjcKZfGpkquB596SEBXhWIiVLaiFjtp_s"; // from sir andy
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
                List<MondayDataViewModel> dt = new List<MondayDataViewModel>();

                var bIds = GetBoardIds();
                var items = bIds.Select(e => e.BoardId).Distinct();

                foreach (var b in items)
                {
                        var query = "query { boards (ids: "+b+") { " +
                        "items_page (limit: 10) { " +
                        "items { id name column_values { id text } " +
                        "subitems { id name column_values { id text } } } } } }";
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
                                  Subitems = item.Subitems
                                      .Select(subitem => new Subitem
                                      {
                                          Id = subitem.Id,
                                          Name = subitem.Name,
                                          Column_values = subitem.Column_values
                                      })
                                      .ToList()
                              })
                          )
                          .ToList();

                        foreach (var i in itemList)
                        {
                            TimeSpan planned_ts = Convert.ToDateTime(i.Column_values[7].Text) - Convert.ToDateTime(i.Column_values[6].Text);
                            var plannedDaysDuration = planned_ts.Days;

                            TimeSpan actual_ts = i.Column_values[11].Text == "" || i.Column_values[10].Text == "" ? 
                                TimeSpan.FromSeconds(2) : (Convert.ToDateTime(i.Column_values[11].Text) - Convert.ToDateTime(i.Column_values[10].Text));
                            var actualDaysDuration = actual_ts.Days;

                            foreach (var sub in i.Subitems)
                            {
                                if (i.Subitems.Count() > 0)
                                {
                                    dt.Add(new MondayDataViewModel
                                    {
                                        GroupId = i.GroupId,
                                        GroupName = i.GroupTitle,
                                        ItemId = i.Id,
                                        ItemName = i.Name,
                                        WorkGroup = i.Column_values[1].Text,
                                        PIC = i.Column_values[2].Text,
                                        SectionHead = i.Column_values[3].Text,
                                        Contractor = i.Column_values[4].Text,
                                        ContractorPIC = i.Column_values[5].Text,
                                        PlannedStartDate = i.Column_values[6].Text,
                                        PlannedEndDate = i.Column_values[7].Text,
                                        PlannedDaysDuration = plannedDaysDuration.ToString(),
                                        PlannedPercentageAccomplishment = i.Column_values[9].Text,
                                        ActualStartDate = i.Column_values[10].Text,
                                        ActualEndDate = i.Column_values[11].Text,
                                        ActualDaysDuration = actualDaysDuration.ToString(),
                                        ActualPercentageAccomplishment = i.Column_values[13].Text,
                                        Variance = i.Column_values[14].Text,
                                        ObservationRemarksStatus = i.Column_values[15].Text,
                                        LastUpdated = i.Column_values[16].Text,
                                        ProgressPicture = i.Column_values[17].Text,
                                        ITPDocs = i.Column_values[18].Text,
                                        JobCardNo = i.Column_values[19].Text,
                                        Sub_ItemId = sub.Id,
                                        Sub_ItemName = sub.Name,
                                        Sub_PONo = sub.Column_values[0].Text,
                                        Sub_DateSubmit = sub.Column_values[1].Text,
                                        Sub_Id = sub.Column_values[2].Text,
                                        Sub_IdName = sub.Column_values[3].Text,
                                        Sub_MilestoneGroup = sub.Column_values[4].Text,
                                        Sub_MilestoneName = sub.Column_values[5].Text,
                                        Sub_TaskNo = sub.Column_values[6].Text,
                                        Sub_PlannedDaysDuration = sub.Column_values[7].Text,
                                        Sub_WeightedPercentage = sub.Column_values[8].Text,
                                        Sub_PlannedStartDate = sub.Column_values[9].Text,
                                        Sub_PlannedEndDate = sub.Column_values[10].Text,
                                        Sub_TargetCompletionPercentage = sub.Column_values[11].Text,
                                        Sub_TargetRelativeWeight = sub.Column_values[12].Text,
                                        Sub_ActualStartDate = sub.Column_values[13].Text == "" ? "-" : sub.Column_values[13].Text,
                                        Sub_ActualEndDate = sub.Column_values[14].Text == "" ? "-" : sub.Column_values[14].Text,
                                        Sub_ActualCompletionPercentage = sub.Column_values[15].Text,
                                        Sub_ActualRelativeWeight = sub.Column_values[16].Text,
                                        Sub_PlannedStartValidation = sub.Column_values[17].Text,
                                        Sub_ActualEndValidation = sub.Column_values[18].Text,
                                        Sub_DPRDate = sub.Column_values[19].Text,
                                        Sub_MainStatus = sub.Column_values[20].Text == "" ? "-" : sub.Column_values[20].Text,
                                        Sub_InspectionRequirement = sub.Column_values[21].Text,
                                        Sub_ITPStatus = sub.Column_values[22].Text,
                                        Sub_ITPRemarks = sub.Column_values[23].Text,
                                        Sub_ITPReference = sub.Column_values[24].Text,
                                        Sub_ITPUploadSigned = sub.Column_values[25].Text,
                                        Sub_MilestoneItem = sub.Column_values[26].Text,
                                        Sub_MilestoneDetails = sub.Column_values[27].Text,
                                        Sub_PaymentTermsCode = sub.Column_values[28].Text,
                                        Sub_LastUpdated = sub.Column_values[29].Text,

                                    });
                                }
                                
                            }
                        }
                    
                    }
                    else
                    {
                        throw new Exception($"Error from Monday.com API: {response.StatusCode}");
                    }
                }
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
                                PRDate = Convert.ToDateTime(item.PRDate).ToString("MM/dd/yyyy"),
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
                                PRReleasedDate = Convert.ToDateTime(item.PRReleasedDate).ToString("MM/dd/yyyy"),
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
            //string sc1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/GetPODetailsCOCA";
            //string sl1 = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/GetPODetailsCOCA";
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
                    //poLine = dt
                    //.Select(item => new Models.POLineViewModel
                    //{
                    //    PRNo = item.PRNo_Line,
                    //    No = item.No_Line
                    //}).ToList();
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
                    //var _poDetail = prNo.Join(items, pr => pr, item => item.PRNo, (pr, item) => new PODetail
                    //{
                    //    PRNo = item.PRNo,
                    //    PONo = item.PONo,
                    //    DateArchived = Convert.ToDateTime(item.DateArchived).ToString("MM/dd/yyyy"),
                    //    BuyFromVendorNo = item.BuyFromVendorNo,
                    //    BuyFromVendorName = item.BuyFromVendorName,
                    //    POStatus = item.POStatus,
                    //    POTotalLineAmount = item.POTotalLineAmount,
                    //    POBillingTerms = item.POBillingTerms,
                    //    NoOfProgressBilling = item.NoOfProgressBilling,
                    //    PBMilestone = item.PBMilestone,
                    //    VersionNo = item.VersionNo,
                    //    PaymentType = item.PaymentType,
                    //    Company = c,
                    //    PlantNo = item.PlantNo,
                    //    OrderDate = item.OrderDate,
                    //    POPaymentTerms = item.POPaymentTermCode,
                    //    POContractAmount = "",
                    //    POAmendedAmount = "",
                    //    RetentionPercent = "",
                    //    RetentionAmount = "",
                    //    RetentionReleaseDate = "",
                    //    ProgressBillingPercent = ""
                    //}).ToList();

                    //poDetail.AddRange(_poDetail);
                    foreach (var i in prNo)
                    {
                        var item = items.LastOrDefault(e => e.PRNo == i);
                        if (item != null)
                        {
                            poDetail.Add(new Models.PODetail()
                            {
                                PRNo = item.PRNo,
                                PONo = item.PONo,
                                DateArchived = Convert.ToDateTime(item.DateArchived).ToString("MM/dd/yyyy"),
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
                                OrderDate = item.OrderDate,
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
                    var vlEntry = exportItemData.VendorLedgerEntry;

                    // For CV Entry
                    var cvEntry = exportItemData.CheckLedgerEntry;

                    foreach (var item in data)
                    {
                        var _vlEntry = vlEntry.FirstOrDefault(e => e.DocumentNo == item.PPINNo);
                        var cvRefNo = _vlEntry == null ? "" : (_vlEntry.ClosedByEntryNo == 0 ? _vlEntry.AppliesToId : _vlEntry.DocumentNo);
                        var _clEntry = cvEntry.FirstOrDefault(e => e.CheckNo == cvRefNo);

                        if (item.PONo != "")
                        {
                            piDetail.Add(new Models.PIDetail()
                            {
                                PPINNo = item.PPINNo,
                                InvoiceDate = Convert.ToDateTime(item.InvoiceDate).ToString("MM/dd/yyyy"),
                                InvoiceAmount = item.InvoiceAmount,
                                PONo = item.PONo,
                                Company = c,
                                CVReferenceNo = cvRefNo,
                                PIPaymentTerms = item.PIPaymentTermCode,
                                InvoiceNo = item.VendorInvoiceNo,
                                InvoiceAmountIncVAT = item.AmountIncludingVAT,
                                WorkPenalties = "",
                                AmountPayable = "",
                                PPINDate = item.PostingDate,
                                CVDate = _clEntry == null ? "" : _clEntry.CheckDate,
                                CVStatus = _clEntry == null ? "" : _clEntry.EntryStatus,
                                CVAmount = _clEntry == null ? "" : _clEntry.Amount
                            });
                        }
                    }
                }
            }
            return Ok(piDetail.ToList());
        }

        [HttpGet]
        [Route("api/GetPOLineDetailsFromNAV")]
        public IHttpActionResult GetPOLineDetailsFromNAV()
        {
            string[] comp = { "SCPC", "SLPGC" };

            /* Decrypt sensitive parameters */
            //string _sc = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Sem%20Calaca%20Power%20Corporation/Codeunit/GetPODetailsCOCA";
            //string _sl = "http://thulium.smcdacon.com:7077/BC130_POWER_TEST/WS/Southwest%20Luzon%20Power%20Gen%20Corp/Codeunit/GetPODetailsCOCA";
            var username = Properties.Resources.NavSemiraraUname;
            var password = Properties.Resources.NavSemiraraPass;
            List<Models.POLineViewModel> poLineDetail = new List<Models.POLineViewModel>();
            string url_link = "";

            foreach (var c in comp)
            {
                if (c == "SCPC")
                    url_link = sc;
                else
                    url_link = sl;

                /* Get Code Unit Properties */
                CocaHandshake.CocaHandshake codeunit_service = new CocaHandshake.CocaHandshake();
                //GetPODetailsFromNAV.GetPODetailsCOCA codeunit_service = new GetPODetailsFromNAV.GetPODetailsCOCA();
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

                if (exportItemData.POLine != null)
                {
                    var conString = "Database=COCAWEBDB;Server=192.168.70.231;user=ict;password=ict@ictdept";
                    string query = "select PRNo from PODetails";
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
                            PRNo = dt.Rows[i]["PRNo"].ToString()
                        });
                    }
                    var prNo = arr.Select(e => e.PRNo).ToList();


                    var items = exportItemData.POLine;
                    var data = items.Where(e => prNo.Contains(e.PRNo_Line)).ToList();

                    foreach (var item in data)
                    {
                        if (item.PRNo_Line != "")
                        {
                            poLineDetail.Add(new Models.POLineViewModel()
                            {
                                DocumentNo = "",
                                No = item.No_Line,
                                PRNo = item.PRNo_Line,
                                Description = item.Description,
                                Description2 = item.Description2,
                                Description3 = "",
                                WarrantyDurationEndDate = "",
                                WarrantyDurationStartDate = ""
                            });
                        }
                    }
                }
            }
            return Ok(poLineDetail.ToList());
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


        //[HttpPost]
        //[Route("api/ImportPOData")]
        //public string ImportPOData(PODetail po)
        //{
        //    var conString = "Database=CDSDb;Server=192.168.70.231;user=ict;password=ict@ictdept";
        //    string query = @"INSERT INTO PODetail (";
        //    SqlConnection conn = new SqlConnection(conString);
        //    conn.Open();
        //    SqlCommand cmd = new SqlCommand(query, conn);
        //    SqlDataAdapter sqlDa = new SqlDataAdapter(cmd);
        //    DataTable dt = new DataTable();
        //    sqlDa.Fill(dt);

        //    try
        //    {
        //        if (dt.Rows.Count <= 0)
        //        {
        //            throw new Exception("No results found");
        //        }
        //        var arr = new List<ViolationViewModel>();

        //        for (var i = 0; i < dt.Rows.Count; i++)
        //        {
        //            int offense = Convert.ToInt32(dt.Rows[i]["ViolationOffense"]);
        //            arr.Add(new ViolationViewModel
        //            {
        //                IdNo = dt.Rows[i]["IdNo"].ToString(),
        //                PONo = dt.Rows[i]["PONo"].ToString(),
        //                ViolationType = dt.Rows[i]["ViolationTypeName"].ToString(),
        //                ViolationDescription = dt.Rows[i]["Description"].ToString(),
        //                ViolationDate = dt.Rows[i]["ViolationDate"].ToString(),
        //                NoOfOffense = (offense == 1 ? "First Offense" : (offense == 2 ? "Second Offense" : (offense == 3 ? "Third Offense" : "Fourth Offense"))),
        //                MonetaryPenalty = (offense == 1 ? dt.Rows[i]["FirstPenalty"].ToString() : (offense == 2 ? dt.Rows[i]["SecondPenalty"].ToString() : (offense == 3 ? dt.Rows[i]["ThirdPenalty"].ToString() : dt.Rows[i]["SucceedingPenalty"].ToString()))),
        //                ProcessDate = dt.Rows[i]["ViolationEffectivityDate"].ToString()
        //            });
        //        }

        //        conn.Close();
        //        return Ok(arr);
        //    }
        //    catch (Exception e)
        //    {
        //        var result = e.Message;
        //        return Ok(result);
        //    }
        //}

    }
}
