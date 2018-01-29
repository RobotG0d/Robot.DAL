using Robot.DAL.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Robot.DAL.Orchestration.Test.Model
{
    public class FooService : IServiceWithId<Foo, Guid>
    {
        public Dictionary<Guid, Foo> Dictionary { get; set; }

        public FooService()
        {
            Dictionary = new Dictionary<Guid, Foo>();
        }

        public Foo AddOrUpdate(Foo obj)
        {
            if (obj.Id == default(Guid))
                obj.Id = Guid.NewGuid();

            Dictionary[obj.Id] = obj;

            return obj;
        }

        public Foo Create(Foo obj)
        {
            if(obj.Id == default(Guid))
                obj.Id = Guid.NewGuid();

            if (obj.Id != default(Guid) && Dictionary.ContainsKey(obj.Id))
                return null;

            Dictionary[obj.Id] = obj;

            return obj;
        }

        public void Delete(Foo obj)
        {
            Dictionary.Remove(obj.Id);
        }

        public IQueryable<Foo> GetAll()
        {
            return Dictionary.Values.AsQueryable();
        }

        public Foo GetById(Guid id)
        {
            Foo foo;
            if (Dictionary.TryGetValue(id, out foo))
                return foo;

            return null;
        }

        public Foo Update(Foo obj)
        {
            if (Dictionary.ContainsKey(obj.Id))
            {
                Dictionary[obj.Id] = obj;
                return Dictionary[obj.Id];
            }

            return null;
        }
    }
}
