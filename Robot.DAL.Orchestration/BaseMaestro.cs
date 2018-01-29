using AutoMapper;
using Robot.DAL.Core;
using Robot.Expected;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Robot.DAL.Orchestration
{
    public class BaseMaestro
    {
        public ITransactionManager TransactionManager { get; set; }

        public IMapper AutoMapper { get; set; }

        public BaseMaestro(ITransactionManager transactionManager, IMapper mapper)
        {
            TransactionManager = transactionManager;
            AutoMapper = mapper;
        }

        protected Expected<bool> SaveChanges() => TransactionManager.SaveChanges();

        protected async Task<Expected<bool>> SaveChangesAsync() => await TransactionManager.SaveChancesAsync();

        protected IEnumerable<TDest> Project<TSrc, TDest>(IQueryable<TSrc> source)
        {
            return source.ToList().Select(srcItem => AutoMapper.Map<TSrc, TDest>(srcItem));
        }

        protected IEnumerable<TDest> Project<TSrc, TDest>(ICollection<TSrc> source)
        {
            return source.ToList().Select(srcItem => AutoMapper.Map<TSrc, TDest>(srcItem));
        }
    }
}
