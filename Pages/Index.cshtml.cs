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


namespace SampleApp.Pages
{
    public class IndexModel : PageModel
    {
 
        private readonly IFileProvider _fileProvider;
        private readonly string _targetFilePath;
        public IndexModel(IFileProvider fileProvider, IConfiguration config)
        {

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

        
        //fetches the file that is described by the filename and then returns it. This allows users to download the original gltf using the UI or the api 
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

        //This is the HTTP Get that returns the .glb file. This was better suited for the headset because it only needs to request one file rather than all files in a directory. 
        public IActionResult OnGetDownloadModel(int file)
        {
            var downloadFile = _fileProvider.GetFileInfo("extracted/" + file + "/model.glb");

            return PhysicalFile(downloadFile.PhysicalPath, MediaTypeNames.Application.Octet, "model" + file + ".glb");
        }

        //This method returns the number of models in the extracted directory. This way the headset knows how many new files have been uploaded and how many models to request.
        public IActionResult OnGetNumModels()
        {
            int num = Directory.GetDirectories(_targetFilePath + "\\extracted").Length;
            return new JsonResult(num);
        }
    }
}
