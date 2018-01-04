using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Sarjee.SimpleRenamer.Common.Interface;
using Sarjee.SimpleRenamer.Common.Model;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleRenamer.Web.Controllers
{
    public class RenameController : Controller
    {
        IScanFiles _scan;
        public RenameController(IScanFiles scan)
        {
            _scan = scan;
        }

        // 
        // GET: /Rename/

        public IActionResult Index()
        {
            return View();
        }

        // 
        // GET: /Rename/Scan?path=C:\Location

        private IFileProvider _fileProvider;

        [HttpGet]
        public async Task<IActionResult> Scan([FromQuery]string filePath)
        {
            List<MatchedFile> matchedFiles = await _scan.ScanAsync(CancellationToken.None);

            return View(matchedFiles);
        }
    }
}
