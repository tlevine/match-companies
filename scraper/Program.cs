using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using OfficeOpenXml;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Globalization;

namespace TZCompanyScraper
{
    class Program
    {
        static string connString = System.Configuration.ConfigurationManager.ConnectionStrings["dbConn"].ConnectionString;

        static void Main(string[] args)
        {
            WorldBankSearchLoadProjectMetadata(true, true);
            WorldBankSearchData();
            
            
            ImportToXslx();
        }

        private static void ImportToXslx()
        { 
            var list = new List<Tuple<short, string, string, string, string>>();
            using (var conn = new SqlConnection(connString))
            {

                conn.Open();

                


                var fo = new FileInfo("pagedata.xlsx");
                if (fo.Exists)
                {
                    fo.Delete();
                }
                using (ExcelPackage excelPackage = new ExcelPackage(fo))
                {
                    {
                        var ws = excelPackage.Workbook.Worksheets.Add("Page Content");
                        ws.Cells[1, 1].Value = "PageId";
                        ws.Cells[1, 2].Value = "PageAddress";
                        ws.Cells[1, 3].Value = "PageContent";
                        ws.Cells[1, 4].Value = "PageTitle";
                        ws.Cells[1, 5].Value = "ProjectCountry";

                        using (var c = conn.CreateCommand())
                        {
                            c.CommandText =
                                "SELECT PageAddress, PageContent, c.PageId, PageTitle, ProjectCountry  FROM [PageContent] c inner join PageMetadata m on c.PageId = m.PageId ";
                            using (var reader = c.ExecuteReader())
                            {
                                var j = 2;
                                while (reader.Read())
                                {
                                    ws.Cells[j, 1].Value = reader["PageId"];
                                    ws.Cells[j, 2].Value = reader["PageAddress"];
                                    ws.Cells[j, 3].Value = reader["PageContent"];
                                    ws.Cells[j, 4].Value = reader["PageTitle"];
                                    ws.Cells[j, 5].Value = reader["ProjectCountry"];
                                    j++;

                                }
                            }
                        }
                    }

                    {
                        var ws = excelPackage.Workbook.Worksheets.Add("Metadata");
                        ws.Cells[1, 1].Value = "PageId";
                        ws.Cells[1, 2].Value = "Published";
                        ws.Cells[1, 3].Value = "ProjectName";
                        ws.Cells[1, 4].Value = "ProjectCountry";
                        ws.Cells[1, 5].Value = "NoticeLanguage";
                        ws.Cells[1, 6].Value = "ContactName";
                        ws.Cells[1, 7].Value = "SubmissionDate";
                        ws.Cells[1, 8].Value = "ProcurementType";
                        ws.Cells[1, 9].Value = "MajorSector";
                        ws.Cells[1, 10].Value = "BidReference";
                        ws.Cells[1, 11].Value = "ProjectId";
                        ws.Cells[1, 12].Value = "NoticeType";
                        ws.Cells[1, 13].Value = "ContactOrganization";
                        ws.Cells[1, 14].Value = "ContractSignatureDate";
                        ws.Cells[1, 15].Value = "ProcurementGroup";

                        using (var c = conn.CreateCommand())
                        {
                            c.CommandText =
                                @"SELECT [PageId]
                                  ,[Published]
                                  ,[ProjectName]
                                  ,[ProjectCountry]
                                  ,[NoticeLanguage]
                                  ,[ContactName]
                                  ,[SubmissionDate]
                                  ,[ProcurementType]
                                  ,[MajorSector]
                                  ,[BidReference]
                                  ,[ProjectId]
                                  ,[NoticeType]
                                  ,[NoticeStatus]
                                  ,[ContactOrganization]
                                  ,[ContractSignatureDate]
                                  ,[ProcurementGroup]
                              FROM [ContractAwards].[dbo].[PageMetadata]";
                            using (var reader = c.ExecuteReader())
                            {
                                var j = 2;
                                while (reader.Read())
                                {
                                    ws.Cells[j, 1].Value = reader[0];
                                    ws.Cells[j, 2].Value = reader[1];
                                    ws.Cells[j, 3].Value = reader[2];
                                    ws.Cells[j, 4].Value = reader[3];
                                    ws.Cells[j, 5].Value = reader[4];
                                    ws.Cells[j, 6].Value = reader[5];
                                    ws.Cells[j, 7].Value = reader[6];
                                    ws.Cells[j, 8].Value = reader[7];
                                    ws.Cells[j, 9].Value = reader[8];
                                    ws.Cells[j, 10].Value = reader[9];
                                    ws.Cells[j, 11].Value = reader[10];
                                    ws.Cells[j, 12].Value = reader[11];
                                    ws.Cells[j, 13].Value = reader[12];
                                    ws.Cells[j, 14].Value = reader[13];
                                    ws.Cells[j, 15].Value = reader[14];
                                    j++;

                                }
                            }
                        }
                    }

                    {
                        var ws = excelPackage.Workbook.Worksheets.Add("Company");
                        ws.Cells[1, 1].Value = "CompanyId";
                        ws.Cells[1, 2].Value = "ProjectName";
                        ws.Cells[1, 3].Value = "ContractNo";
                        ws.Cells[1, 4].Value = "Name";
                        ws.Cells[1, 5].Value = "Status";
                        ws.Cells[1, 6].Value = "Link";
                        ws.Cells[1, 7].Value = "Country";
                        

                        using (var c = conn.CreateCommand())
                        {
                            c.CommandText =
                                @"SELECT 
                                    [Id]
                                   ,[ProjectName]
                                  ,[ContractNo]
                                  ,[Name]
                                  ,[Status]
                                  ,[Link]
                                  
                                  ,[Country]
                              FROM [ContractAwards].[dbo].[Company]";
                            using (var reader = c.ExecuteReader())
                            {
                                var j = 2;
                                while (reader.Read())
                                {
                                    ws.Cells[j, 1].Value = reader[0];
                                    ws.Cells[j, 2].Value = reader[1];
                                    ws.Cells[j, 3].Value = reader[2];
                                    ws.Cells[j, 4].Value = reader[3];
                                    ws.Cells[j, 5].Value = reader[4];
                                    ws.Cells[j, 6].Value = reader[5];
                                    ws.Cells[j, 7].Value = reader[6];
                                    j++;

                                }
                            }
                        }
                    }

                    {
                        var ws = excelPackage.Workbook.Worksheets.Add("Company Profile");
                        ws.Cells[1, 1].Value = "CompanyId";
                        ws.Cells[1, 2].Value = "ProfileKey";
                        ws.Cells[1, 3].Value = "ProfileValue";
                        


                        using (var c = conn.CreateCommand())
                        {
                            c.CommandText =
                                @"SELECT [CompanyId]
                                  ,[ProfileKey]
                                  ,[ProfileValue]
                              FROM [ContractAwards].[dbo].[CompanyProfile]
                              where ProfileKey != ''";
                            using (var reader = c.ExecuteReader())
                            {
                                var j = 2;
                                while (reader.Read())
                                {
                                    ws.Cells[j, 1].Value = reader[0];
                                    ws.Cells[j, 2].Value = reader[1];
                                    ws.Cells[j, 3].Value = reader[2];
                                    j++;

                                }
                            }
                        }
                    }

                    {
                        var ws = excelPackage.Workbook.Worksheets.Add("Issues");
                        ws.Cells[1, 1].Value = "IssueType";
                        ws.Cells[1, 2].Value = "PageId";
                        ws.Cells[1, 3].Value = "PageAddress";



                        using (var c = conn.CreateCommand())
                        {
                            c.CommandText =
                                @"SELECT [IssueType]
                                  ,[PageId]
                                  ,[PageAddress]
                              FROM [ContractAwards].[dbo].[Issue]";
                            using (var reader = c.ExecuteReader())
                            {
                                var j = 2;
                                while (reader.Read())
                                {
                                    ws.Cells[j, 1].Value = reader[0];
                                    ws.Cells[j, 2].Value = reader[1];
                                    ws.Cells[j, 3].Value = reader[2];
                                    j++;

                                }
                            }
                        }
                    }

                    excelPackage.Save();
                }
            }
        }

        private static void OpenCompaniesData()
        {
            List<List<string>> values = new List<List<string>>();
            values.Add(new List<string> { "name.", "company-number", "jurisdiction-code", "incorporation-date", "dissolution-date", "company-type", "registry-url", 
                "branch-status", "inactive", "current-status", "created-at", "updated-at", "retrieved-at", "opencorporates-url", "previous-names", "source" });

            using (WebClient wc = new WebClient())
            {
                for (var page = 1; page <= 100; page++)
                {
                    var content = wc.DownloadString("http://api.opencorporates.com/v0.2/companies/search?q=&jurisdiction_code=tz&format=xml&api_token=wLNZ9efqR37Zdsxvuph4&page=" + page);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(content);

                    var nodes = doc.SelectNodes("//company/company");
                    foreach (XmlNode company in nodes)
                    {
                        var line = new List<string>();
                        line.Add(company.SelectSingleNode("./name").InnerText);
                        line.Add(company.SelectSingleNode("./company-number").InnerText);
                        line.Add(company.SelectSingleNode("./jurisdiction-code").InnerText);
                        line.Add(company.SelectSingleNode("./incorporation-date").InnerText);
                        line.Add(company.SelectSingleNode("./dissolution-date").InnerText);
                        line.Add(company.SelectSingleNode("./company-type").InnerText);
                        line.Add(company.SelectSingleNode("./registry-url").InnerText);
                        line.Add(company.SelectSingleNode("./branch-status").InnerText);
                        line.Add(company.SelectSingleNode("./inactive").InnerText);
                        line.Add(company.SelectSingleNode("./current-status").InnerText);
                        line.Add(company.SelectSingleNode("./created-at").InnerText);
                        line.Add(company.SelectSingleNode("./updated-at").InnerText);
                        line.Add(company.SelectSingleNode("./retrieved-at").InnerText);
                        line.Add(company.SelectSingleNode("./opencorporates-url").InnerText);
                        line.Add(company.SelectSingleNode("./previous-names").InnerText);
                        line.Add(company.SelectSingleNode("./source").InnerText);

                        values.Add(line);
                    }

                    Console.WriteLine("Page: {0}", page);
                    Thread.Sleep(1000);

                    if (page % 10 == 0)
                    {
                        SaveResults(values, page == 10, page == 10 ? 1 : (page - 10) * 30 + 2);
                        values.Clear();
                    }
                }

                

            }
        }

        private static void SaveResults(List<List<string>> values, bool createNew, int startRow)
        {
            var fi = new FileInfo(@"open_companies_data.xlsx");
            if (createNew) fi.Delete();
            using (ExcelPackage p = new ExcelPackage(fi))
            {
                ExcelWorksheet ws;
                if (createNew)
                {
                    ws = p.Workbook.Worksheets.Add("Sheet1");
                }
                else
                {
                    ws = p.Workbook.Worksheets["Sheet1"];
                }
                
                var rn = startRow;
                foreach (var l in values)
                {
                    var vn = 1;
                    foreach (var v in l)
                    {
                        ws.Cells[rn, vn].Value = v;
                        vn++;
                    }
                    rn++;
                }

                p.Save();
            }
        }

        private static void ProcurementData()
        {
            List<List<string>> values = new List<List<string>>();

            var spans = new Tuple<int, string>[7];
            for (int i = 0; i < 7; i++) spans[i] = new Tuple<int, string>(1, "");

            values.Add(new List<string> { "Tender No.", "Entity Name", "Lot No.", "Description", "Provider", "Award Date", "Amount" });
            using (WebClient wc = new WebClient())
            {
                for (var page = 1; page <= 142; page++)
                {

                    var content = wc.DownloadString("http://tender.ppra.go.tz/index.php?page=" + page + "&home=home&view=award");

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(content);
                    var table = doc.DocumentNode.SelectSingleNode("//table[@id='eventspanel']");

                    if (table != null)
                    {
                        var rows = table.SelectNodes("./tr");
                        if (rows != null)
                        {
                            for (var i = 3; i < rows.Count - 1; i++)
                            {
                                var row = rows[i];
                                var line = new List<string>();
                                values.Add(line);
                                var cells = row.SelectNodes("./td");
                                var delta = 0;
                                for (int j = 0; j < 7; j++)
                                {

                                    if (spans[j].Item1 == 1)
                                    {
                                        HtmlNode cell = cells[j - delta];
                                        if (cell.Attributes["rowspan"] != null)
                                        {
                                            int rowspan = 0;
                                            if (Int32.TryParse(cell.Attributes["rowspan"].Value, out rowspan) && rowspan > 1)
                                            {
                                                spans[j] = new Tuple<int, string>(rowspan, cell.InnerText.Trim(' ', '\n'));
                                            }
                                        }

                                        line.Add(cell.InnerText.Trim(' ', '\n'));
                                    }
                                    else
                                    {
                                        delta++;
                                        line.Add(spans[j].Item2);
                                        spans[j] = new Tuple<int, string>(spans[j].Item1 - 1, spans[j].Item2);
                                    }

                                }
                            }
                        }
                    }
                    Console.WriteLine("Page: {0}", page);
                    //Thread.Sleep(1000);
                }
            }

            using (var conn = new SqlConnection(@"Server=localhost;Database=Tanzania;Trusted_Connection=True;"))
            {

                conn.Open();
                using (var c = conn.CreateCommand())
                {
                    c.CommandText = "truncate table PublicProcurrement";
                    c.ExecuteNonQuery();
                }
                for (int i = 1; i < values.Count; i++)
                {
                    var c = values[i];
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "insert into PublicProcurrement(TenderNo,EntityName,LotNo,Description,Provider,AwardDate,Amount) values(@p1,@p2,@p3,@p4,@p5,@p6,@p7)";
                        for (int j = 0; j < c.Count; j++)
                        {
                            cmd.Parameters.Add("p" + (j + 1), c[j]);
                        }

                        cmd.ExecuteNonQuery();

                        
                    }
                }
            }

            //var fi = new FileInfo(@"public_procurement.xlsx");
            //fi.Delete();
            //using (ExcelPackage p = new ExcelPackage(fi))
            //{
            //    var ws = p.Workbook.Worksheets.Add("Sheet1");
            //    var rn = 1;
            //    foreach (var l in values)
            //    {
            //        var vn = 1;
            //        foreach (var v in l)
            //        {
            //            ws.Cells[rn, vn].Value = v;
            //            vn++;
            //        }
            //        rn++;
            //    }

            //    p.Save();
            //}
        }

        private static string TryDownload(string address)
        {
            
            while (true)
            {
                try
                {
                    using (var client = new WebClient())
                    {
                        return client.DownloadString(address);
                    }
                }
                catch (Exception)
                {
                    Thread.Sleep(5000);        
                    Console.WriteLine("Trying to download {0}", address);
                }
            }
                
            
        }

        static string RemoveUnwantedTags(string data)
        {
            if (!String.IsNullOrEmpty(data))
            {
                var document = new HtmlDocument();
                document.LoadHtml(data);

                var nodes = new Queue<HtmlNode>(document.DocumentNode.SelectNodes("./*|./text()"));
                while (nodes.Count > 0)
                {
                    var node = nodes.Dequeue();
                    var parentNode = node.ParentNode;

                    if (node.Name != "#text" && node.Name != "br")
                    {

                        var childNodes = node.SelectNodes("./*|./text()");

                        if (childNodes != null)
                        {
                            foreach (var child in childNodes)
                            {
                                nodes.Enqueue(child);
                                parentNode.InsertBefore(child, node);
                            }
                        }
                        var n = document.CreateTextNode(" ");
                        parentNode.InsertBefore(n, node);
                        parentNode.RemoveChild(node);

                    }
                }

                return document.DocumentNode.InnerHtml;
            }

            return "";
        }

        static List<KeyValuePair<string, string>> ParseParagraph(string input)
        {
            var result = new List<KeyValuePair<string, string>>();
            var arr = input.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in arr)
            {
                var cleaned = line.Clean(new KeyValuePair<string, string>[] {
                            new KeyValuePair<string, string>("\t", ""),
                            new KeyValuePair<string, string>("\n", ""),
                            new KeyValuePair<string, string>("\r", ""),
                            new KeyValuePair<string, string>("&nbsp;", " "),
                            new KeyValuePair<string, string>("&amp;#160;", ""),
                            new KeyValuePair<string, string>("&amp;nbsp;", "")
                        });
                if (cleaned != "")
                {
                    var values = cleaned.Split(":".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length > 1)
                    {
                        result.Add(new KeyValuePair<string, string>(values[0].Trim(), String.Join(":", values.ToList().GetRange(1, values.Length-1)).Trim()));
                    }
                    else
                    {
                        result.Add(new KeyValuePair<string, string>(values[0].Trim(), ""));
                    }
                }
            }

            return result;
        }

        static int GetRecordCount()
        {
            using (var client = new WebClient())
            {
                var content = TryDownload("http://search.worldbank.org/wcontractawards");
                var document = new HtmlDocument();
                document.LoadHtml(content);
                var nodes = document.DocumentNode.SelectNodes("//div[@id='options']//div[@class='facet']");
                if (nodes != null)
                {
                    foreach (var node in nodes)
                    {
                        if (node.InnerText.Contains("Contract Award"))
                        {
                            var match = Regex.Match(node.InnerText, @"Contract Award \(((\d|,)+)\)");
                            if (match != null)
                            {
                                var str = match.Groups[1].Value;
                                return Int32.Parse(str,NumberStyles.AllowThousands );
                            }
                        }
                    }
                }

                return 0;
            }
        }

        private static void WorldBankSearchLoadProjectMetadata(bool loadLinks, bool loadEmpty)
        {
            //using (WebClient client = new WebClient())
            {
                using (var conn = new SqlConnection(connString))
                {
                    conn.Open();
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"
merge dbo.PageContent as target 
using (select @p1) as source (PageAddress)
on (target.PageAddress = source.PageAddress)
when matched then
    update set PageTitle = @p2
when not matched then
	insert (PageAddress, PageContent, PageTitle, UrlId) values (@p1, NULL, @p2, @p3);";
                    HtmlDocument doc = new HtmlDocument();
                    string content;



                    List<string> links = new List<string>();
                    //cmd.CommandText = "select PageAddress from dbo.PageContent";
                    //using (var reader = cmd.ExecuteReader())
                    //{
                    //    while(reader.Read())
                    //    {
                    //        links.Add((string)reader["PageAddress"]);
                    //    }
                    //}
                    //conn.Close();
                    //conn.Dispose();
                    if (loadLinks)
                    {
                        var recordCount = GetRecordCount();
                        var pages = recordCount/10 + 1;
                        Console.WriteLine("Total pages: " + pages);
                        for (int i = 0; i <  pages; i++)
                        {
                            content = TryDownload("http://search.worldbank.org/wcontractawards?&os=" + (i * 10));
                            doc.LoadHtml(content);

                            var nodes = doc.DocumentNode.SelectNodes("//ol[@id='search-results']/li/h3/a");
                            if (nodes != null)
                            {
                                foreach (var n in nodes)
                                {
                                    var link = "http://search.worldbank.org" + n.Attributes["href"].Value;
                                    var title = n.InnerText.Replace("\n", " ").Replace("\r", "");
                                    title = Regex.Replace(title, @"\s+", " ");
                                    links.Add(link);
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddRange(new SqlParameter[] {
                            new SqlParameter("p1", link),
                            new SqlParameter("p2", title),
                            new SqlParameter("p3", Int32.Parse(link.Replace("http://search.worldbank.org/wcontractawards/procdetails/OP", "")))

                        });
                                    cmd.ExecuteNonQuery();
                                }
                                Console.WriteLine("Loaded page number: " + i);
                                Thread.Sleep(200);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    else
                    {
                        cmd.CommandText = "select PageAddress from dbo.PageContent " + (loadEmpty ? "where PageMetadata is null" : "");
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                links.Add((string)reader["PageAddress"]);
                            }
                        }
                    }

                    //List<string> links = new List<string> { "http://search.worldbank.org/wcontractawards/procdetails/OP00019613" };
                    //var list = new List<Content>();
                    //Parallel.ForEach(links, link =>
                    var linkNumber = 0;
                    foreach(var link in links)
                    {
                        Console.WriteLine("Link number: " + (++linkNumber) + " out of " + links.Count);
                        using (var cn = new SqlConnection(connString))
                        {
                            cn.Open();
                            var command = cn.CreateCommand();
                            command.CommandText = "select count(1) from dbo.PageContent where PageContent is not null and PageAddress=@p1";
                            command.Parameters.Add(new SqlParameter("p1", link));
                            var cnt = (int)command.ExecuteScalar();
                            if (cnt != 0) continue;
                        }
                        Console.WriteLine("Download:" + link);
                        try
                        {
                            //WebClient c = new WebClient();
                            var ct = TryDownload(link);
                            ct = ct.Replace("fontface", "font face");
                            var document = new HtmlDocument();
                            document.LoadHtml(ct);
                            var div = document.DocumentNode.SelectSingleNode("//div[@id='container']");
                            var html = div.OuterHtml;

                            ct = TryDownload("http://search.worldbank.org/wcontractawards/moreview/" + link.Substring(link.LastIndexOf('/') + 1));
                            document.LoadHtml(ct);
                            div = document.DocumentNode.SelectSingleNode("//table");
                            var metadata = div.OuterHtml;
                            var l = new Content { Address = link, Metadata = metadata, Html = html };
                            //list.Add();

                            using (var cn = new SqlConnection(connString))
                            {
                                cn.Open();
                                var command = cn.CreateCommand();
                                command.CommandText = @"
merge dbo.PageContent as target 
using (select @p1, @p2) as source (PageAddress, PageContent)
on (target.PageAddress = source.PageAddress) 
when matched then 
	update set PageContent = @p2, PageMetadata = @p3 
when not matched then
	insert (PageAddress, PageContent, PageMetadata) values (@p1, @p2, @p3);";
                                command.Parameters.Clear();
                                command.Parameters.AddRange(new SqlParameter[] {
                            new SqlParameter("p1", l.Address),
                            new SqlParameter("p2", l.Html),
                            new SqlParameter("p3", l.Metadata)
                        });
                                command.ExecuteNonQuery();
                            }
                            //                        
                            Thread.Sleep(300);
                        }
                        catch
                        {
                            using (var cn = new SqlConnection(connString))
                            {
                                cn.Open();
                                var command = cn.CreateCommand();
                                command.CommandText = @"insert into dbo.Backlog values(@p1)";
                                command.Parameters.Clear();
                                command.Parameters.AddRange(new SqlParameter[] {
                                    new SqlParameter("p1", link)
                                });
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                   


                }
            }
        }


//        private static void WorldBankSearchLoadAll(bool loadLinks, bool loadEmpty)
//        {
//            using (WebClient client = new WebClient())
//            {
//                using (var conn = new SqlConnection(connString))
//                {
//                    conn.Open();
//                    var cmd = conn.CreateCommand();
//                    cmd.CommandText = @"
//merge dbo.PageContent as target 
//using (select @p1) as source (PageAddress)
//on (target.PageAddress = source.PageAddress) 
//when not matched then
//	insert (PageAddress, PageContent) values (@p1, NULL);";
//                    HtmlDocument doc = new HtmlDocument();
//                    string content;



//                    List<string> links = new List<string>();
//                    if (loadLinks)
//                    {
//                        for (int i = 0; i < 763; i++)
//                        {
//                            content = client.DownloadString("http://search.worldbank.org/wcontractawards?&os=" + (i * 10));
//                            doc.LoadHtml(content);

//                            var nodes = doc.DocumentNode.SelectNodes("//ol[@id='search-results']/li/h3/a");
//                            foreach (var n in nodes)
//                            {
//                                var link = "http://search.worldbank.org" + n.Attributes["href"].Value;
//                                links.Add(link);
//                                cmd.Parameters.Clear();
//                                cmd.Parameters.AddRange(new [] {
//                            new SqlParameter("p1", link)
                            
//                        });
//                                cmd.ExecuteNonQuery();
//                            }
//                            Thread.Sleep(300);
//                        }
//                    }
//                    else
//                    {
//                        cmd.CommandText = "select PageAddress from dbo.PageContent " + (loadEmpty ? "where PageContent is null" : "");
//                        using (var reader = cmd.ExecuteReader())
//                        {
//                            while (reader.Read())
//                            {
//                                links.Add((string)reader[0]);
//                            }
//                        }

//                    }

//                    //List<string> links = new List<string> { "http://search.worldbank.org/wcontractawards/procdetails/OP00019613" };
//                    cmd.CommandText = @"
//merge dbo.PageContent as target 
//using (select @p1, @p2) as source (PageAddress, PageContent)
//on (target.PageAddress = source.PageAddress) 
//when matched then 
//	update set PageContent = @p2 
//when not matched then
//	insert (PageAddress, PageContent) values (@p1, @p2);";
//                    foreach (var link in links)
//                    {
//                        content = client.DownloadString(link);
//                        content = content.Replace("fontface", "font face");
//                        doc.LoadHtml(content);
//                        var document = doc.DocumentNode.SelectSingleNode("//div[@id='container']");
//                        cmd.Parameters.Clear();
//                        cmd.Parameters.AddRange(new SqlParameter[] {
//                            new SqlParameter("p1", link),
//                            new SqlParameter("p2", document.OuterHtml)
//                        });
//                        cmd.ExecuteNonQuery();
//                        Thread.Sleep(500);
//                    }


//                }
//            }
//        }

        private static void WBMetadataParse()
        {
            var list = new List<Content>();
            using (var conn = new SqlConnection(connString))
            {

                conn.Open();

                using (var c = conn.CreateCommand())
                {
                    c.CommandText = "select [PageId], [PageAddress],[PageTitle], [PageMetadata] FROM [ContractAwards].[dbo].[PageContent] where PageMetadata is not null";
                    using (var reader = c.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Content { Id = (short)reader["PageId"], Address = (string)reader["PageAddress"], Metadata = (string)reader["PageMetadata"], Title = (string)reader["PageTitle"] });
                        }
                    }
                }

                var parsedMetadata = new List<Dictionary<string, string>>();
                HtmlDocument doc = new HtmlDocument();
                foreach (var r in list)
                {
                    var dict = new Dictionary<string, string>();
                    dict.Add("Id", r.Id.ToString());
                    
                    doc.LoadHtml(r.Metadata);
                    var rows = doc.DocumentNode.SelectNodes("//td[not(@colspan)]");
                    for (var i = 0; i < rows.Count-1; i=i+2)
                    {
                        var td = rows[i];
                        var tdValue = rows[i + 1];
                        dict.Add(td.FirstChild.InnerText.Replace(":", ""), tdValue.InnerText.Replace("&amp;", "&"));
                    }
                    parsedMetadata.Add(dict);
                }

                using (var c = conn.CreateCommand())
                {
                    c.CommandText = @"
merge dbo.PageMetadata as target 
using (select @p1) as source (PageId)
on (target.PageId = source.PageId)
when matched then
    update set [Published]             = @p2
           ,[ProjectName]              = @p3
           ,[ProjectCountry]           = @p4
           ,[NoticeLanguage]           = @p5
           ,[ContactName]              = @p6
           ,[SubmissionDate]           = @p7
           ,[ProcurementType]          = @p8
           ,[MajorSector]              = @p9
           ,[BidReference]             = @p10
           ,[ProjectId]                = @p11
           ,[NoticeType]               = @p12
           ,[NoticeStatus]             = @p13
           ,[ContactOrganization]      = @p14
           ,[ContractSignatureDate]    = @p15
           ,[ProcurementGroup]         = @p16
when not matched then
INSERT 
           ([PageId]
           ,[Published]
           ,[ProjectName]
           ,[ProjectCountry]
           ,[NoticeLanguage]
           ,[ContactName]
           ,[SubmissionDate]
           ,[ProcurementType]
           ,[MajorSector]
           ,[BidReference]
           ,[ProjectId]
           ,[NoticeType]
           ,[NoticeStatus]
           ,[ContactOrganization]
           ,[ContractSignatureDate]
           ,[ProcurementGroup])
     VALUES
           (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16);";

                    foreach (var m in parsedMetadata)
                    {
                        DateTime sDate, pDate, csDate;
                        DateTime.TryParseExact(m["Published"], "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out pDate);
                        DateTime.TryParseExact(m["Submission Date"], "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out sDate);
                        DateTime.TryParseExact(m["Contract Signature date"], "dd-MMM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out csDate);
                        c.Parameters.Clear();
                        var p15 = new SqlParameter { ParameterName = "p15", DbType = System.Data.DbType.DateTime };
                        if(csDate != DateTime.MinValue) 
                        {
                            p15.Value = (DateTime?)csDate;
                        }
                        else
                        {
                            p15.Value = DBNull.Value;
                        }
                        //p15.DbType = System.Data.DbType.Date;
                        
                        c.Parameters.AddRange(new SqlParameter[] 
                        {
                            new SqlParameter("p1", Int16.Parse(m["Id"])),
                            new SqlParameter("p2", pDate!=DateTime.MinValue ? (DateTime?)pDate : null),
                            new SqlParameter("p3",m["Project Name"]),
                            new SqlParameter("p4",m["Project Country"]),
                            new SqlParameter("p5",m["Notice Language"]),
                            new SqlParameter("p6",m["Contact Name"]),
                            new SqlParameter("p7",sDate!=DateTime.MinValue ? (DateTime?)sDate : null),
                            new SqlParameter("p8",m["Procurement Type"]),
                            new SqlParameter("p9",m["Major Sector"]),
                            new SqlParameter("p10",m["Bid Reference #"]),
                            new SqlParameter("p11",m["Project Id"]),
                            new SqlParameter("p12",m["Notice Type"]),
                            new SqlParameter("p13",m["Notice Status"]),
                            new SqlParameter("p14",m["Contact Organization"]),
                            p15,
                            new SqlParameter("p16",m["Procurement Group"])
                        });
                        c.ExecuteNonQuery();
                    }
                }
                //var fo = new FileInfo("output.xlsx");
                //if (fo.Exists)
                //{
                //    fo.Delete();
                //}
                //using (ExcelPackage excelPackage = new ExcelPackage(fo))
                
                //foreach (var m in parsedMetadata)
                //{ 
                    
                //}
            }
        }

        private static void WorldBankSearchData()
        {
            //using (WebClient client = new WebClient())
            {
                HtmlDocument doc = new HtmlDocument();
                string content;

                ////Tanzania
                //content = client.DownloadString("http://search.worldbank.org/wcontractawards?qterm=tanzania&image1=%3E%3E&os=");
                ////Kenia
                //content = client.DownloadString("http://search.worldbank.org/wcontractawards?qterm=&_project_ctry_name_exact%5B%5D=Kenya");
                //doc.LoadHtml(content);
                //var node = doc.DocumentNode.SelectSingleNode("//ul[@class='pagination']/li[@class='next']");
                //while (true)
                //{
                //    node = node.PreviousSibling;
                //    if (node.Name == "li")
                //    {
                //        break;
                //    }
                //}
                //var page = node.InnerText;
                //var totalPages = Int32.Parse(page);
                var list = new List<Content>();
                using (var conn = new SqlConnection(connString))
                {

                    conn.Open();
                    
                    using (var c = conn.CreateCommand())
                    {
                        c.CommandText = "SELECT PageAddress, PageContent, c.PageId, PageTitle, ProjectCountry, UrlId  FROM [PageContent] c inner join PageMetadata m on c.PageId = m.PageId";
                        var reader = c.ExecuteReader();
                        while(reader.Read())
                        {
                            list.Add(new Content { Id = (short)reader["PageId"], Address = (string)reader["PageAddress"], Html = (string)reader["PageContent"], Title = (string)reader["PageTitle"], Country = (string)reader["ProjectCountry"], UrlId = (int)reader["UrlId"] });
                        }
                    }
                }

                //List<string> links = new List<string>();
                //for (int i = 0; i < 751; i++)
                //{
                //    content = client.DownloadString("http://search.worldbank.org/wcontractawards?&os=" + (i * 10));
                //    doc.LoadHtml(content);

                //    var nodes = doc.DocumentNode.SelectNodes("//ol[@id='search-results']/li/h3/a");
                //    foreach (var n in nodes)
                //    {
                //        links.Add("http://search.worldbank.org" + n.Attributes["href"].Value);
                //    }
                //}

                //List<string> links = new List<string> { "http://search.worldbank.org/wcontractawards/procdetails/OP00011542" };
                //List<string> links = new List<string> { "http://search.worldbank.org/wcontractawards/procdetails/OP00017104" };
                //bad link
                //List<string> links = new List<string> { "http://search.worldbank.org/wcontractawards/procdetails/OP00017541" };

                var allCompanies = new List<Company>();
                var badLinks = new List<BadContent>();
                //var skippedLinks = new List<>
                //var emptyContractNumberLinks = new List<Content>();

                foreach (var context in list)
                {
                    string project = "", contractNumber = "", country = "";
                    //content = client.DownloadString(link);
                    content = context.Html;
                    //if (content.Contains("Spanish"))
                    //{
                    //    badLinks.Add(new BadContent { Content = context, Reason = "SpanishLanguage" });
                    //    continue;
                    //}
                    //if (content.Contains("French"))
                    //{
                    //    badLinks.Add(new BadContent { Content = context, Reason = "FrenchLanguage" });
                    //    continue;
                    //}
                    var link = context.Address;
                    content = content.Replace("fontface", "font face");
                    doc.LoadHtml(content);
                    var header = doc.DocumentNode.SelectSingleNode("//div[@class='headtxt']");
                    string[] headerArray = header.InnerText.Split("|".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in headerArray)
                    {
                        var cleaned = line.Clean(new KeyValuePair<string, string>[] {
                            new KeyValuePair<string, string>("\t", ""),
                            new KeyValuePair<string, string>("\n", ""),
                            new KeyValuePair<string, string>("\r", ""),
                            new KeyValuePair<string, string>("&nbsp;", "")
                        });
                        var keyVal = cleaned.Split(':');
                        if (keyVal[0].Trim() == "Project")
                        {
                            project = keyVal[1].Trim();
                            //break;
                        }
                        if (keyVal[0].Trim() == "Country")
                        {
                            country = keyVal[1].Trim();
                            //break;
                        }
                    }

                    

                    List<string> records = new List<string>();
                    var rows = doc.DocumentNode.SelectNodes("//div[@class='prc_notice_data']//tr");
                    if (rows != null)
                    {

                        var paragraphs = doc.DocumentNode.SelectNodes("//div[@class='prc_notice_data']//p");
                        if (paragraphs != null)
                        {
                            foreach (var p in paragraphs)
                            {
                                var innerHtml = p.InnerHtml;

                                var html = RemoveUnwantedTags(innerHtml);
                                records.Add(html);
                            }
                        }

                        List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
                        foreach (var record in records)
                        {
                            pairs.AddRange(ParseParagraph(record));
                        }

                        foreach (var pair in pairs)
                        {
                            if (pair.Key.ToLower() == "project id")
                            {
                                contractNumber = pair.Value;
                                break;
                            }
                        }
                        if (rows.Count < 2)
                        {
                            badLinks.Add(new BadContent { Content = context, Reason = "UnknownTableFormat" });
                            continue;
                        }
                        var colCount = rows[1].ChildNodes.Count;
                        var spans = new Tuple<int, string>[colCount];
                        for (int i = 0; i < colCount; i++) spans[i] = new Tuple<int, string>(1, "");

                        var values = new List<List<string>>();
                        for (int i = 1; i < rows.Count; i++)
                        {
                            var row = rows[i];
                            var line = new List<string>();
                            var delta = 0;
                            var cells = row.SelectNodes("./td");
                            if (cells != null)
                            {
                                for (var j = 0; j < colCount; j++)
                                {
                                    if (spans[j].Item1 == 1)
                                    {
                                        if (j - delta < cells.Count)
                                        {
                                            var cell = cells[j - delta];
                                            if (cell.Attributes["rowspan"] != null)
                                            {
                                                int rowspan = 0;
                                                if (Int32.TryParse(cell.Attributes["rowspan"].Value[0].ToString(), out rowspan) && rowspan > 1)
                                                {
                                                    spans[j] = new Tuple<int, string>(rowspan, RemoveUnwantedTags(cell.InnerHtml).Clean(new KeyValuePair<string, string>[] {
                                                                    new KeyValuePair<string, string>("\t", ""),
                                                                    new KeyValuePair<string, string>("\n", " "),
                                                                    new KeyValuePair<string, string>("\r", ""),
                                                                    new KeyValuePair<string, string>("&nbsp;", "")
                                                                }));

                                                }
                                            }

                                            line.Add(RemoveUnwantedTags(cell.InnerHtml).Clean(new KeyValuePair<string, string>[] {
                                                                    new KeyValuePair<string, string>("\t", ""),
                                                                    new KeyValuePair<string, string>("\n", " "),
                                                                    new KeyValuePair<string, string>("\r", ""),
                                                                    new KeyValuePair<string, string>("&nbsp;", "")
                                                                }));
                                        }
                                        else
                                        {
                                            line.Add("");
                                        }
                                    }
                                    else
                                    {
                                        delta++;
                                        line.Add(spans[j].Item2);
                                        spans[j] = new Tuple<int, string>(spans[j].Item1 - 1, spans[j].Item2);
                                    }

                                }
                                values.Add(line);
                            }
                        }

                        var names = values[0];

                        var companies = new List<Company>();
                        for (int i = 1; i < values.Count; i++)
                        {
                            Company c = new Company { ProjectName = project, Profile = new List<KeyValuePair<string, string>>(), Link = link, Country = country };
                            for (int j = 0; j < names.Count; j++)
                            {
                                if (names[j].ToLower().Contains("name") && names[j].ToLower().Contains("consultant"))
                                {
                                    c.Name = values[i][j];
                                    continue;
                                }

                                if (names[j].ToLower().Contains("project id"))
                                {
                                    c.ContractNo = values[i][j];
                                    continue;
                                }

                                c.Profile.Add(new KeyValuePair<string, string>(names[j], values[i][j]));
                            }

                            companies.Add(c);
                        }

                        if (companies.Count > 0)
                        {
                            if (!companies.Exists(c => c.Status == null))
                            {
                                allCompanies.AddRange(companies);
                            }
                            else
                            {
                                badLinks.Add(new BadContent { Content = context, Reason = "CompanyStatusNotFound" });
                            }
                        }
                    }
                    else
                    {
                        var paragraphs = doc.DocumentNode.SelectNodes("//div[@class='prc_notice_data']//p");
                        if (paragraphs == null)
                        {
                            badLinks.Add(new BadContent { Content = context, Reason = "EmptyContent" });
                            continue;
                        }
                        foreach (var p in paragraphs)
                        {
                            var innerHtml = p.InnerHtml;

                            var html = RemoveUnwantedTags(innerHtml);
                            records.Add(html);
                        }

                        List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>>();
                        foreach (var record in records)
                        {
                            pairs.AddRange(ParseParagraph(record));
                        }

                        List<Company> companies = new List<Company>();
                        Company currentCompany = null;

                        foreach (var pair in pairs)
                        {
                            if (pair.Key.Replace(" ", "").ToLower().Contains("contractreference") || pair.Key.Replace(" ", "").ToLower().Contains("projectid") || pair.Key.Replace(" ", "").ToLower().Contains("referenceno"))
                            {
                                contractNumber = pair.Value;
                                context.ContractNumber = contractNumber;
                            }

                            if ((pair.Key.ToLower().Contains("awarded") || pair.Key.ToLower().Contains("evaluated") || pair.Key.ToLower().Contains("rejected")) && (pair.Key.ToLower().Contains("bidder") || pair.Key.ToLower().Contains("consultant")  || pair.Key.ToLower().Contains("firm")))
                            {
                                var companyName = !String.IsNullOrEmpty(pair.Value) ? pair.Value : "";
                                string status = pair.Key.ToLower().Contains("awarded") ? "awarded" : pair.Key.ToLower().Contains("evaluated") ? "evaluated" : "rejected";
                                var c = new Company { ContractNo = contractNumber, ProjectName = project, Status = status, Link = link, Name = companyName, Country = country };
                                companies.Add(c);
                                currentCompany = c;
                                continue;
                            }

                            if (currentCompany != null)
                            {
                                if (pair.Key.ToLower() == "name")
                                {
                                    if (currentCompany.Profile == null)
                                    {
                                        currentCompany.Name = pair.Value;
                                    }
                                    else
                                    {
                                        var c = new Company { ContractNo = contractNumber, ProjectName = project, Status = currentCompany.Status, Name = pair.Value, Link = link, Country = country };
                                        companies.Add(c);
                                        currentCompany = c;
                                    }
                                }
                                else
                                {
                                    if (currentCompany.Profile == null)
                                    {
                                        currentCompany.Profile = new List<KeyValuePair<string, string>>();
                                    }
                                    currentCompany.Profile.Add(pair);
                                }
                            }
                        }

                        if (companies.Count > 0)
                        {
                            if (!companies.Exists(c => c.Status == null))
                            {
                                allCompanies.AddRange(companies);
                            }
                            else
                            {
                                badLinks.Add(new BadContent { Content = context, Reason = "CompanyStatusNotFound" });
                            }
                        }
                        else
                        {
                            badLinks.Add(new BadContent { Content = context, Reason = "CompanyNotFound", Pairs = pairs });
                        }

                        if (contractNumber == "")
                        {
                            badLinks.Add(new BadContent { Content = context, Reason = "ContractNumberNotFound", Pairs = pairs });
                        }
                    }
                }

                badLinks = ExtraParse(allCompanies, badLinks);
                
                //File.Delete(@"..\..\..\db\tz.sdf");
                using (var conn = new SqlConnection(connString))
                {
                    
                    conn.Open();
                    using (var c = conn.CreateCommand())
                    {
                        c.CommandText = "insert into dbo.Issue(IssueType, PageId, PageAddress) values(@p1,@p2,@p3)";
                        foreach (var cnt in badLinks)
                        {
                            c.Parameters.Clear();
                            c.Parameters.AddRange(new SqlParameter [] {
                                new SqlParameter("p1", cnt.Reason),
                                new SqlParameter("p2", cnt.Content.Id),
                                new SqlParameter("p3", cnt.Content.Address)
                            });
                            c.ExecuteNonQuery();
                        }
                        
                    }

                    
                    int max = -1;
                    //using (var c = conn.CreateCommand())
                    //{
                    //    c.CommandText = "select max(id) from company";
                    //    //var tst = c.ExecuteScalar();
                    //    max = (short)c.ExecuteScalar();
                    //}
                    for (int i = 0; i < allCompanies.Count; i++)
                    {
                        var c = allCompanies[i];
                        using (var cmd = conn.CreateCommand())
                        {
                            var id = max+i+1;
                            cmd.CommandText = "insert into Company(ProjectName, ContractNo, Name, Status, Link, Id, Country) values(@p1,@p2,@p3,@p4,@p5,@p6,@p7)";
                            cmd.Parameters.AddRange(
                                    new SqlParameter[] {
                                        new SqlParameter("p1", c.ProjectName.EmptyIfNull()),
                                        new SqlParameter("p2", c.ContractNo.EmptyIfNull()),
                                        new SqlParameter("p3", c.Name.EmptyIfNull()),
                                        new SqlParameter("p4", c.Status.EmptyIfNull()),
                                        new SqlParameter("p5", c.Link.EmptyIfNull()),
                                        new SqlParameter("p6", id),
                                        new SqlParameter("p7", c.Country)
                                    }
                                );
                            cmd.ExecuteNonQuery();

                            cmd.CommandText = "insert into CompanyProfile(CompanyId, ProfileKey, ProfileValue) values(@p1, @p2, @p3)";
                            if (c.Profile != null)
                            {
                                foreach (var p in c.Profile)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.AddRange(
                                        new SqlParameter[] {
                                        new SqlParameter("p1", id),
                                        new SqlParameter("p2", p.Key.EmptyIfNull()),
                                        new SqlParameter("p3", p.Value.EmptyIfNull()),
                                    }
                                    );
                                    cmd.ExecuteNonQuery();
                                }
                            }
                        } 
                    }
                }

                //using (StreamWriter sw = new StreamWriter("badLinks.txt"))
                //{
                //    foreach (var l in badLinks)
                //    {
                //        sw.WriteLine(l);
                //    }
                //}
                //using (StreamWriter sw = new StreamWriter("emptyCNLinks.txt"))
                //{
                //    foreach (var l in emptyContractNumberLinks)
                //    {
                //        sw.WriteLine(l);
                //    }
                //}
            }

           
        }

        private static List<BadContent> ExtraParse(List<Company> allCompanies, List<BadContent> badLinks)
        {
            List<BadContent> result = new List<BadContent>();

            foreach (var b in badLinks)
            {
                var shouldBeAdded = true;
                var content = b.Content;
                if ((b.Reason == "CompanyNotFound" || b.Reason == "ContractNumberNotFound") && b.Pairs != null)
                {
                    Company company = null;
                    foreach (var pair in b.Pairs)
                    {
                        var key = pair.Key.ToLower().Replace(" ", "");
                        if (key.Contains("name") || key.Contains("firm/indv"))
                        {
                            var status = "awarded?";
                            if (key.Contains("awarded"))
                            {
                                status = "awarded";
                            }
                            company = new Company { ContractNo = content.ContractNumber, Status = status, Name = pair.Value, Link = content.Address, Profile = new List<KeyValuePair<string,string>>(), Country = content.Country };
                            break;
                        }
                    }

                    if (company != null)
                    {
                        foreach (var pair in b.Pairs)
                        {
                            company.Profile.Add(new KeyValuePair<string, string>(pair.Key, pair.Value));
                        }

                        allCompanies.Add(company);
                        shouldBeAdded = false;
                    }
                }

                if (shouldBeAdded)
                {
                    result.Add(b);
                }
            }

            return result;
        }

        class Content
        {
            public int Id { get; set; }
            public string Address { get; set; }
            public string Html { get; set; }
            public string Title { get; set; }
            public string Metadata { get; set; }
            public string Country { get; set; }
            public int UrlId { get; set; }

            public string ContractNumber { get; set; }
        }

        class BadContent
        {
            public Content Content { get; set; }
            public string Reason { get; set; }

            public List<KeyValuePair<string, string>> Pairs { get; set; }
        }

        class Company
        {
            public String ProjectName { get; set; }
            public String ContractNo { get; set; }
            public String Name { get; set; }
            public String Status { get; set; }
            public String Link { get; set; }
            public String Country { get; set; }
            public List<KeyValuePair<string, string>> Profile { get; set; }

            public override string ToString()
            {
                return "ProjectName: " + ProjectName + "; ContractNo: " + ContractNo + "; Name :" + Name + "; Status: " + Status + "; " + ( Profile != null ? String.Join("; ", Profile) : "" );
                
            }
        }
    }
}
