using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using SampleApp.Utilities;
using SharpGLTF.Schema2;

namespace SampleApp.Pages
{
    public class BufferedSingleFileUploadPhysicalModel : PageModel
    {
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { ".txt", ".gltf", ".zip" };
        private readonly string _targetFilePath;
        //public static Dictionary<int, string> directories { get; private set; }

        
        public BufferedSingleFileUploadPhysicalModel(IConfiguration config)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");

            // To save physical files to a path provided by configuration:
            _targetFilePath = config.GetValue<string>("StoredFilesPath");

            // To save physical files to the temporary files folder, use:
            //_targetFilePath = Path.GetTempPath();
        }

        [BindProperty]
        public BufferedSingleFileUploadPhysical FileUpload { get; set; }

        public string Result { get; private set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostUploadAsync()
        {
            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";

                return Page();
            }

            var formFileContent = 
                await FileHelpers.ProcessFormFile<BufferedSingleFileUploadPhysical>(
                    FileUpload.FormFile, ModelState, _permittedExtensions, 
                    _fileSizeLimit);

            if (!ModelState.IsValid)
            {
                Result = "Please correct the form.";

                return Page();
            }

            // For the file name of the uploaded file stored
            // server-side, use Path.GetRandomFileName to generate a safe
            // random file name.
            var trustedFileNameForFileStorage = FileUpload.FormFile.FileName;
            var filePath = Path.Combine(
                _targetFilePath, trustedFileNameForFileStorage);

            // Upload file to the default file path from the config
            using (var fileStream = System.IO.File.Create(filePath))
            {
                await fileStream.WriteAsync(formFileContent);
               
            }
            // This is the count of files that have been uploaded. It is 0 unless there are already files.
            int fCount = 0;
            try
            {
                //fCount = Directory.GetFiles(_targetFilePath + "\\extracted", "*", SearchOption.TopDirectoryOnly).Length;
                fCount = Directory.GetDirectories(_targetFilePath + "\\extracted").Length;
            }
            catch (Exception e)
            {
                
            }
            //This is where the zip file will be extracted
            var extractedPath = Path.Combine(_targetFilePath, "extracted", fCount.ToString());
            ZipFile.ExtractToDirectory(filePath, Path.Combine(_targetFilePath, "extracted", extractedPath));
            //add the extracted file path to a dictionary that corrosponds to its file name
            BufferedSingleFileUploadPhysical.directories.Add(trustedFileNameForFileStorage, extractedPath);
            //load in the gltf file and then save it as a glb to be transmitted
            var model = ModelRoot.Load(extractedPath + "/scene.gltf");
            model.SaveGLB(extractedPath + "/model.glb");
            return RedirectToPage("./Index");
        }
    }

    public class BufferedSingleFileUploadPhysical
    {
        public static Dictionary<string, string> directories = new Dictionary<string, string>();

        [Required]
        [Display(Name="File")]
        public IFormFile FormFile { get; set; }

        [Display(Name="Note")]
        [StringLength(50, MinimumLength = 0)]
        public string Note { get; set; }
    }
}
