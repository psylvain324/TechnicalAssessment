using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicalAssessment.Data;

namespace TechnicalAssessment.Controllers
{
    public class UploaderController : Controller
    {
        private UploadService uploadService = new UploadService();

        [HttpPost]
        public async Task<IActionResult> Index(IFormFile file)
        {
            string filename = file.FileName;

            using (FileStream output = System.IO.File.Create((filename)))
                await file.CopyToAsync(output);

            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(IFormFile file)
        {
            if (file != null)
            {
                uploadService.uploadTransaction(file.FileName);
            }

            return View();
        }
    }
}
