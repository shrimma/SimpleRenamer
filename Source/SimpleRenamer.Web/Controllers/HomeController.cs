using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Sarjee.SimpleRenamer.Web.Models;

namespace Sarjee.SimpleRenamer.Web.Controllers
{
    public class HomeController : Controller
    {
        private IFileProvider _fileProvider;

        public HomeController(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Scan()
        {
            var contents = _fileProvider.GetDirectoryContents("");
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
