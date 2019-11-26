using System.Collections.Generic;

namespace TechnicalAssessment.Services.Interfaces
{
    public interface IServiceExport<T> where T : class
    {
        public string CsvExport(List<T> items, string file);
        public string XmlExport(List<T> items, string file);
    }
}
