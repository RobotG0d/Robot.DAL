using System.Linq;

namespace Robot.DAL.Core
{
    public interface IService<T>
    {
        T Create(T obj);
        IQueryable<T> GetAll();
        T Update(T obj);
        void Delete(T obj);
    }

    public interface IServiceWithId<T, K> : IService<T> where T : BaseEntity<K>
    {
        T GetById(K id);
        T AddOrUpdate(T obj);
    }
}
