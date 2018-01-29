using Robot.Expected;
using System;
using System.Threading.Tasks;

namespace Robot.DAL.Core
{
    public interface ITransactionManager
    {
        Expected<bool> SaveChanges();
        Task<Expected<bool>> SaveChancesAsync();
        ITransaction CreateTransaction();
    }

    public interface ITransaction : IDisposable
    {
        Expected<bool> Commit();
        Task<Expected<bool>> CommitAsync();
    }
}
