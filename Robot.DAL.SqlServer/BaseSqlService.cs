using Robot.DAL.Core;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.DAL.SqlServer
{
    public class BaseSqlService<T> : IService<T>
        where T : class
    {
        public DbContext DbContext { get; set; }

        private IDbSet<T> _dataSet;
        protected IDbSet<T> DataSet {
            get {
                if (_dataSet == null)
                    _dataSet = DbContext.Set<T>();

                return _dataSet;
            }
        }

        public BaseSqlService(DbContext dbContext)
        {
            DbContext = dbContext;
        }

        #region IService Members

        public virtual T Create(T obj) => DataSet.Add(obj);

        public virtual IQueryable<T> GetAll() => DataSet.AsNoTracking();

        public virtual T Update(T obj)
        {
            DataSet.Attach(obj);
            DbContext.Entry(obj).State = EntityState.Modified;

            return obj;
        }

        public virtual void Delete(T obj)
        {
            DataSet.Attach(obj);
            DbContext.Entry(obj).State = EntityState.Deleted;
        }

        #endregion

        #region Protected Utils

        private string CreateSqlQuery(string spName, string spReturnName, IDictionary<string, object> spParams)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("EXEC @{0} = {1} ", spReturnName, spName);

            var paramNames = spParams.Keys.Select(key => "@" + key);
            sb.Append(string.Join(", ", paramNames));

            return sb.ToString();
        }

        protected int ExecuteStoredProcedure(string spName, string spReturnName, IDictionary<string, object> spParams)
        {
            var query = CreateSqlQuery(spName, spReturnName, spParams);

            var returnCode = new SqlParameter();
            returnCode.ParameterName = spReturnName;
            returnCode.SqlDbType = System.Data.SqlDbType.Int;
            returnCode.Direction = System.Data.ParameterDirection.Output;

            var sqlParams = new LinkedList<SqlParameter>();
            sqlParams.AddLast(returnCode);

            spParams.Keys.ForEach(key => sqlParams.AddLast(new SqlParameter(key, spParams[key])));

            var data = DbContext.Database.SqlQuery<object>(query, sqlParams.ToArray());

            var item = data.FirstOrDefault();

            return (int)returnCode.Value;
        }

        #endregion
    }
}
