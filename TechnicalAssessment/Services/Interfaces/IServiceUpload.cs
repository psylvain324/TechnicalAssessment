namespace TechnicalAssessment.Services.Interfaces
{
    public interface IServiceUpload
    {
        public void UploadCsv(string filePath);
        public void UploadXml(string filePath);
    }
}
