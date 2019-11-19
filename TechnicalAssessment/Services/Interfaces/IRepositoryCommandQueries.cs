using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TechnicalAssessment.Services.Interfaces
{
    public interface IRepositoryWithCommandsQueries
    {
        IEnumerable<T> Get<T>(T query) where T : IQueryHandler<IEnumerable<T>>;
        Task<IEnumerable<T>> GetAsync<T>(T query) where T : IQueryHandlerAsync<IEnumerable<T>>;
        //T GetSingle<T>(T query) where T : IQueryHandler<T>;
        //Task<T> GetSingleAsync<T>(T query) where T : IQueryHandlerAsync<Host>;
        int Execute<T>(T command) where T : ICommandHandler<int>;
        Task<int> ExecuteAsync<T>(T command) where T : ICommandHandlerAsync<int>;
    }
}
