using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Warehouse.Contact.Model;

namespace Warehouse.Contact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        public IConfiguration Configuration { get; }
        protected string folderName = "";
        public HomeController(IConfiguration configuration)
        {
            Configuration = configuration;
            string? folderName = Configuration["folderName"]?.ToString();
        }
        [HttpPost("Get")]
        public string Get(Request request)
        {
            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            DeleteFile();
            string fileName = $"{request.Name}-{request.IP}.txt";
            string fullPath = Path.Combine(folderName, fileName);

            if (!Directory.Exists(folderName))
                Directory.CreateDirectory(folderName);
            System.IO.File.WriteAllText(fullPath, $"Name: {request.Name}\nIP: {request.IP}");

            return "";
        }
        private async Task DeleteFile()
        {
            string[] files = Directory.GetFiles(folderName);
            foreach (string file in files)
            {
                DateTime lastModified = System.IO.File.GetLastWriteTime(file);
                if (lastModified < DateTime.Now.AddMinutes(-1))
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
