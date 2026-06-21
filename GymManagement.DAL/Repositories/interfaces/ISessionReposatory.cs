using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.interfaces
{
    public interface ISessionReposatory:IGenaricReposatory<Session>
    {
        Task<IEnumerable<Session>> GetALLSessionsWithTrainerAndCategory(CancellationToken c=default);
        Task<int>GetCountOfBookedSlotsAsync(int sessionId, CancellationToken c=default);
    }
}
