using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.Win32;
using UploadDownload.Models;

namespace UploadDownload.Controllers
{
    public class HomeController : Controller
    {
        private const string uploadPath = "UploadFolder";

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UploadFile(IFormFile dieDatei)
        {
            if (dieDatei == null || dieDatei.Length == 0)
            {
                return Content("Keine Datei ausgewählt oder die Datei ist leer");
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), uploadPath, Path.GetFileName(dieDatei.FileName));

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                dieDatei.CopyTo(stream);
            }

            DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), uploadPath));
            FileInfo[] files = di.GetFiles();

            return RedirectToAction("ShowFiles");
        }

        public IActionResult ShowFiles()
        {
            DirectoryInfo di = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), uploadPath));

            FileInfo[] files = di.GetFiles();

            return View(files);
        }

        
        // http://localhost:56577/Home/Download?filename=
        public IActionResult Download(string filename)
        {
            if (filename == null)
            {
                return Content("Kein Dateiname angegeben.");
            }

            string path = Path.Combine(Directory.GetCurrentDirectory(), uploadPath, filename);

            FileStream stream = new FileStream(path, FileMode.Open);

            // MimeType der Registry und Dateiendung ermitteln
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(Path.GetExtension(path));
            object contentType = regKey.GetValue("Content Type");

            return File(stream, contentType.ToString(), Path.GetFileName(path));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}