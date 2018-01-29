using Robot.DAL.Core;
using Robot.Expected;
using System;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Robot.DAL.SqlServer
{
    public class SqlTransactionManager : ITransactionManager
    {
        private DbContext DbContext { get; set; }

        public SqlTransactionManager(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        public Expected<bool> SaveChanges()
        {
            try
            {
                DbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                return Expected<bool>.Error(e, DALErrorCode.DatabaseError);
            }
        }

        public ITransaction CreateTransaction()
        {
            return new SqlTransaction(DbContext);
        }

        public async Task<Expected<bool>> SaveChancesAsync()
        {
            try
            {
                await DbContext.SaveChangesAsync();

                return true;
            }
            catch (Exception e)
            {
                return Expected<bool>.Error(e, DALErrorCode.DatabaseError);
            }
        }
    }
}
