using Robot.DAL.Core;
using Robot.Expected;
using System;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Robot.DAL.SqlServer
{
    public class SqlTransaction : ITransaction
    {
        private DbContextTransaction _transaction { get; }

        private DbContext DbContext { get; }

        public SqlTransaction(DbContext dbContext)
        {
            DbContext = dbContext;
            _transaction = dbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public Expected<bool> Commit()
        {
            try
            {
                DbContext.SaveChanges();

                _transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                return Expected<bool>.Error(e, DALErrorCode.DatabaseError);
            }
        }

        public async Task<Expected<bool>> CommitAsync()
        {
            try
            {
                await DbContext.SaveChangesAsync();

                _transaction.Commit();
                return true;
            }
            catch (Exception e)
            {
                return Expected<bool>.Error(e, DALErrorCode.DatabaseError);
            }
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }
    }
}
