using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using static SampleApp.Pages.BufferedSingleFileUploadPhysical;

namespace SampleApp.Pages
{
    public class DeletePhysicalFileModel : PageModel
    {
        private readonly IFileProvider _fileProvider;

        public DeletePhysicalFileModel(IFileProvider fileProvider)
        {
            _fileProvider = fileProvider;
        }

        public IFileInfo RemoveFile { get; private set; }

        public IActionResult OnGet(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToPage("/Index");
            }

            RemoveFile = _fileProvider.GetFileInfo(fileName);

            if (!RemoveFile.Exists)
            {
                return RedirectToPage("/Index");
            }

            return Page();
        }

        public IActionResult OnPost(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return RedirectToPage("/Index");
            }

            RemoveFile = _fileProvider.GetFileInfo(fileName);
            
            //var RemoveFileExtracted = _fileProvider.GetFileInfo();
            //Dictionary<int, string> hey = BufferedSingleFileUploadPhysical.directories;
            if (RemoveFile.Exists)
            {
                System.IO.File.Delete(RemoveFile.PhysicalPath);
            }
            //if (RemoveFileExtracted.Exists)
            //{
            // delete the extracted files by finding the correct path in the dictionary defined in bufferedSingleFileUploadPhysical.
                System.IO.Directory.Delete(BufferedSingleFileUploadPhysical.directories[fileName],true);
            BufferedSingleFileUploadPhysical.directories.Remove(fileName);
            //}

            return RedirectToPage("./Index");
        }
    }
}
