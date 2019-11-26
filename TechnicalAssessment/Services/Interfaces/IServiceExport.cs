using System.Collections.Generic;
using System.IO;

namespace TechnicalAssessment.Services.Interfaces
{
    public interface IServiceExport<T> where T : class
    {
        public string CsvExport(List<T> items, string csvFile, string csvPat);
        public void XmlExport(List<T> items);
    }
}
