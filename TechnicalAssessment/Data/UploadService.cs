using System;
using System.IO;

namespace TechnicalAssessment.Data
{
    public class UploadService
    {
        private CsvUpload csvUpload;
        private XmlUpload xmlUpload;
        private const string success = "File uploaded successfully";

        private UploadService(CsvUpload csvUpload, XmlUpload xmlUpload)
        {
            this.csvUpload = csvUpload;
            this.xmlUpload = xmlUpload;
        }

        public UploadService()
        {
        }

        public string UploadTransaction(string path)
        {
            string extensionType = Path.GetExtension(path);
            try
            {
                if (extensionType == "csv")
                {
                    csvUpload.UploadTransaction(path);
                }
                else if (extensionType == "xml")
                {
                    xmlUpload.ParseTransactionXML(path);
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }

            return success;
        }
    }
}
