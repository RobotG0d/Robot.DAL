using Robot.DAL.Core;
using System.Data.Entity;
using System.Linq;

namespace Robot.DAL.SqlServer
{
    public class BaseSqlServiceWithId<T, K> : BaseSqlService<T>, IServiceWithId<T, K>
        where T : BaseEntity<K>
    {
        public BaseSqlServiceWithId(DbContext dbContext) : base(dbContext){ }

        public T AddOrUpdate(T obj)
        {
            if (obj.Id.Equals(default(K)))
                return Create(obj);

            var currentEntity = GetById(obj.Id);
            if (currentEntity != null)
            {
                DbContext.Entry(currentEntity).State = EntityState.Detached;
                return Update(obj);
            }

            return Create(obj);
        }

        public T GetById(K id)
        {
            return GetAll().FirstOrDefault(entity => (object)entity.Id == (object)id);
        }
    }
}
