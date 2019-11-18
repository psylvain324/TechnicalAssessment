using Microsoft.AspNetCore.Http;

namespace TechnicalAssessment.Services.Interfaces
{
    public interface IServiceUpload
    {
        public void UploadCsv(IFormFile file);
        public void UploadXml(IFormFile file);
    }
}
