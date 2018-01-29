using Robot.DAL.Core;
using System;

namespace Robot.DAL.Orchestration.Test.Model
{
    public class Foo : BaseEntity<Guid>
    {
        public int Value { get; set; }

        public Foo() { }

        public Foo(int value)
        {
            Id = Guid.NewGuid();
            Value = value;
        }
    }

    public class FooDTO : BaseDTO<Guid>
    {
        public int Value { get; set; }

        public FooDTO() { }

        public FooDTO(int value)
        {
            Value = value;
        }

        public FooDTO(Guid id, int value) : this(value)
        {
            Id = id;
        }
    }
}
