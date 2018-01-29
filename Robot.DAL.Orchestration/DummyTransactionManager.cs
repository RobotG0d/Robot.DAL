using System;
using System.Threading.Tasks;
using Robot.DAL.Core;
using Robot.Expected;

namespace Robot.DAL.Orchestration
{
    internal class DummyTransactionManager : ITransactionManager
    {
        public ITransaction CreateTransaction() => new DummyTransaction();

        public Task<Expected<bool>> SaveChancesAsync() => Task.FromResult(new Expected<bool>(true));

        public Expected<bool> SaveChanges() => true;
    }

    internal class DummyTransaction : ITransaction
    {
        public Expected<bool> Commit() => true;

        public Task<Expected<bool>> CommitAsync() => Task.FromResult(new Expected<bool>(true));

        public void Dispose() { }
    }
}
