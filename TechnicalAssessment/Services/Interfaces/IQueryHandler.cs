using System.Collections.Generic;
using System.Threading.Tasks;

namespace TechnicalAssessment.Services.Interfaces
{
    public interface IQueryHandler<T> where T : class
    {
        IEnumerable<T> Handle(T query);
        Task<IEnumerable<T>> HandleAsync(T query);
    }
}
