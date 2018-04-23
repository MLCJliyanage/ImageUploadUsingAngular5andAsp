using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace UploadProject.Controllers
{
    [RoutePrefix("api/Upload")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UploadController : ApiController 
    {
        TestDBEntities testDB = new TestDBEntities();
        public string Get(int id)
        {

            // string base64 = Convert.ToBase64String(bytes);
            IEnumerable<byte[]> abc = testDB.spDownload(id);
            byte[] cde = abc.SelectMany(i => i).ToArray();
            string swe = Convert.ToBase64String(cde);
            return swe;

        }
        [HttpPost]
        public HttpResponseMessage UploadJsonFile()
        {
            HttpResponseMessage response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string fileExtention = Path.GetExtension(fileName);
                    int fileSize = postedFile.ContentLength;                   

                    if(fileExtention.ToLower() == ".jpg" || fileExtention.ToLower() == ".bmp" || fileExtention.ToLower() == ".gif" || fileExtention.ToLower() == ".png")
                    {
                        Stream stream = postedFile.InputStream;
                        BinaryReader binaryReader = new BinaryReader(stream);
                        byte[] bytes = binaryReader.ReadBytes((int)stream.Length);

                        
                        testDB.spUpload(fileName, fileSize, bytes);
                    }
                    var filePath = HttpContext.Current.Server.MapPath("~/UploadFile/" + postedFile.FileName);
                    postedFile.SaveAs(filePath);
                }
            }
            return response;
        }
    }     
}