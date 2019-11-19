using System;
using System.Threading.Tasks;

namespace TechnicalAssessment.Services.Interfaces
{
    public interface ICommandHandlerAsync<TReturn>
    {
        Task<TReturn> HandleAsync();
    }
}
