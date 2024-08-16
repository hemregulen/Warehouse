using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Net;
using Warehouse.Contact.Model;

namespace Warehouse.Contact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppSettings _appSettings;

        protected string folderName = "";
        public HomeController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
             folderName = _appSettings.FolderName;
        }
        [HttpPost("Post")]
        public HttpStatusCode Post(Request request)
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            DeleteFile();
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            string fileName = $"{request.Name}.txt";
            string fullPath = Path.Combine(folderName, fileName);
            System.IO.File.WriteAllText(fullPath, $"{request.IP}");
            return HttpStatusCode.OK;
        }
        [HttpPost("Get")]
        public string Get(string Name)
        {
            string fullPath = Path.Combine(folderName, Name);
            if (Directory.Exists(fullPath))
                return System.IO.File.ReadAllText(fullPath+".txt");

            return "Dosya bulanamadı";
        }
        private async Task DeleteFile()
        {
            string[] files = Directory.GetFiles(folderName);
            foreach (string file in files)
            {
                DateTime lastModified = System.IO.File.GetLastWriteTime(file);
                if (lastModified < DateTime.Now.AddDays(-1))
                {
                    try
                    {
                        System.IO.File.Delete(file);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                    }
                }
            }
        }
    }
}
