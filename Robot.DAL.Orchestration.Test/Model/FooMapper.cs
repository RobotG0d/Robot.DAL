using AutoMapper;
using Robot.DAL.Core;
using System;

namespace Robot.DAL.Orchestration.Test.Model
{
    public class FooMapper
    {
        public static IMapper Singleton { get; }

        static FooMapper()
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<Foo, FooDTO>()
                                                            .IncludeBase<BaseEntity<Guid>, BaseDTO<Guid>>()
                                                            .ReverseMap());
            Singleton = config.CreateMapper();
        }
    }
}
