using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using SimpleRenamer.API.Models;
using System.Diagnostics;

namespace SimpleRenamer.API.Controllers
{
    public class HomeController : Controller
    {
        private readonly IFileProvider _fileProvider;
        public HomeController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }
        public IActionResult Index()
        {
            var contents = _fileProvider.GetDirectoryContents("");
            var fileInfo = _fileProvider.GetFileInfo(@"C:\Dummy\Castle.S01E01");
            fileInfo.
            return View(contents);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
