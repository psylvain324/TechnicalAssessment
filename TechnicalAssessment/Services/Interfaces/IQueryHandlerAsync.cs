using System;
using System.Threading.Tasks;

namespace TechnicalAssessment.Services.Interfaces
{
    public interface IQueryHandlerAsync<TReturn> : IQueryRoot
    {
        Task<TReturn> HandleAsync();
    }
}
