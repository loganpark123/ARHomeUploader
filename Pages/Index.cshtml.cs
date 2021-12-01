using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using SampleApp.Data;
using SampleApp.Models;

namespace SampleApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly AppDbContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly string _targetFilePath;
        public IndexModel(AppDbContext context, IFileProvider fileProvider, IConfiguration config)
        {
            _context = context;
            _fileProvider = fileProvider;
            _targetFilePath = config.GetValue<string>("StoredFilesPath");
        }

        //public IList<AppFile> DatabaseFiles { get; private set; }
        public IDirectoryContents PhysicalFiles { get; private set; }

        public async Task OnGet()
        {
            //DatabaseFiles = await _context.File.AsNoTracking().ToListAsync();
            PhysicalFiles = _fileProvider.GetDirectoryContents(string.Empty);
        }

        public async Task<IActionResult> OnGetDownloadDbAsync(int? id)
        {
            if (id == null)
            {
                return Page();
            }

            var requestFile = await _context.File.SingleOrDefaultAsync(m => m.Id == id);

            if (requestFile == null)
            {
                return Page();
            }

            // Don't display the untrusted file name in the UI. HTML-encode the value.
            return File(requestFile.Content, MediaTypeNames.Application.Octet, WebUtility.HtmlEncode(requestFile.UntrustedName));
        }

        public IActionResult OnGetDownloadPhysical(string fileName)
        {
            var downloadFile = _fileProvider.GetFileInfo(fileName);

            return PhysicalFile(downloadFile.PhysicalPath, MediaTypeNames.Application.Octet, fileName);
        }

        //public IActionResult OnGetDownloadModel(string fileName)
        //{
        //    var downloadFile = _fileProvider.GetFileInfo("extracted/"+ fileName + ".glb");

        //    return PhysicalFile(downloadFile.PhysicalPath, MediaTypeNames.Application.Octet, fileName + ".glb");
        //}
        //[HttpGet("{id}")]
        public IActionResult OnGetDownloadModel(int file)
        {
            var downloadFile = _fileProvider.GetFileInfo("extracted/" + file + "/model.glb");

            return PhysicalFile(downloadFile.PhysicalPath, MediaTypeNames.Application.Octet, "model" + file + ".glb");
        }
        public IActionResult OnGetNumModels()
        {
            int num = Directory.GetDirectories(_targetFilePath + "\\extracted").Length;
            return new JsonResult(num);
        }
    }
}
