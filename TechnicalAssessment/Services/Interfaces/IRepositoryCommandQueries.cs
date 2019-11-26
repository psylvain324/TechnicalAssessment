using System.Collections.Generic;
using System.Threading.Tasks;
using TechnicalAssessment.Models;

namespace TechnicalAssessment.Services.Interfaces
{
    public interface ITransactionRepositoryWithCommandsQueries
    {
        IEnumerable<Transaction> Get<T>(T query) where T : IQueryHandler<IEnumerable<Transaction>>;
        Task<IEnumerable<Transaction>> GetAsync<T>(T query) where T : IQueryHandlerAsync<IEnumerable<Transaction>>;
        Transaction GetSingle<T>(T query) where T : IQueryHandler<Transaction>;
        Task<Transaction> GetSingleAsync<T>(T query) where T : IQueryHandlerAsync<Transaction>;
        int Execute<T>(T command) where T : ICommandHandler<int>;
        Task<int> ExecuteAsync<T>(T command) where T : ICommandHandlerAsync<int>;
    }
}
