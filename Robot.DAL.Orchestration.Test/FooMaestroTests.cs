using AutoMapper;
using Robot.DAL.Core;
using Robot.DAL.Orchestration.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Robot.DAL.Orchestration.Test
{
    [TestClass]
    public class FooMaestroTests
    {
        private const int INITIAL_COUNT = 10;
        private GenericMaestro<Foo, FooDTO, Guid> Maestro;

        #region Initialization

        private static IMapper _mapper;

        [ClassInitialize]
        public static void InitClass(TestContext ctx)
        {
            _mapper = FooMapper.Singleton;
        }

        [TestInitialize]
        public void InitTest()
        {
            Maestro = new GenericMaestro<Foo, FooDTO, Guid>(new FooService(), _mapper);

            for (int i = 0; i < INITIAL_COUNT; i++)
                Maestro.Create(new FooDTO(i+1));
        }

        #endregion

        [TestMethod]
        public void TestUnpagedGetAll()
        {
            var expFoos = Maestro.GetAll();
            Assert.IsFalse(expFoos.HasError);

            var foos = expFoos.Value;

            Assert.AreEqual(INITIAL_COUNT, foos.Count());
        }

        [TestMethod]
        public void TestPagedGetAll_FullPage()
        {
            var expFoos = Maestro.GetAll(0, INITIAL_COUNT * 10);
            Assert.IsFalse(expFoos.HasError);

            var foos = expFoos.Value;

            Assert.AreEqual(INITIAL_COUNT, foos.Count());
        }

        [TestMethod]
        public void TestPagedGetAll_EmptyPage()
        {
            var expFoos = Maestro.GetAll(1, INITIAL_COUNT);
            Assert.IsFalse(expFoos.HasError);

            var foos = expFoos.Value;

            Assert.AreEqual(0, foos.Count());
        }

        [TestMethod]
        public void TestPagedGetAll_SmallPage()
        {
            const int PAGE = 2;
            const int PAGE_SIZE = 3;

            var expFoos = Maestro.GetAll(PAGE, PAGE_SIZE);
            Assert.IsFalse(expFoos.HasError);

            var foos = expFoos.Value.ToList();

            Assert.AreEqual(PAGE_SIZE, foos.Count);
        }

        [TestMethod]
        public void TestCreateAndFindById()
        {
            const int VALUE = 7357;
            var fooDto = new FooDTO(VALUE);

            var newDto = Maestro.Create(fooDto);
            Assert.IsFalse(newDto.HasError);

            var foundDto = Maestro.GetById(newDto.Value.Id);
            Assert.IsFalse(foundDto.HasError);

            Assert.AreEqual(VALUE, foundDto.Value.Value);
        }

        [TestMethod]
        public async Task TestAsyncStuff()
        {
            const int VALUE = 7357;
            var fooDto = new FooDTO(VALUE);

            var newDto = await Maestro.CreateAsync(fooDto);
            Assert.IsFalse(newDto.HasError);

            var foundDto = Maestro.GetById(newDto.Value.Id);
            Assert.IsFalse(foundDto.HasError);

            Assert.AreEqual(VALUE, foundDto.Value.Value);
        }

        [TestMethod]
        public void TestDuplicateIdCreate()
        {
            var firstFoo = Maestro.GetAll().Value.First();
            
            var result = Maestro.Create(firstFoo);
            Assert.IsTrue(result.HasError);
            Assert.AreEqual(DALErrorCode.DatabaseError, result.ErrorCode);
        }

        [TestMethod]
        public void TestUpdate()
        {
            const int NEW_VALUE = 50;

            var firstFoo = Maestro.GetAll().Value.First();
            var oldValue = firstFoo.Value;
            var fooId = firstFoo.Id;

            firstFoo.Value = NEW_VALUE;
            Maestro.Update(firstFoo);

            var dto = Maestro.GetById(fooId);
            Assert.IsFalse(dto.HasError);

            Assert.AreEqual(NEW_VALUE, dto.Value.Value);
        }

        [TestMethod]
        public void TestDelete()
        {
            var firstFoo = Maestro.GetAll().Value.First();

            var result = Maestro.Delete(firstFoo);
            Assert.IsFalse(result.HasError);
            Assert.IsTrue(result.Value);

            var dto = Maestro.GetById(firstFoo.Id);
            Assert.IsTrue(dto.HasError);
            Assert.AreEqual(DALErrorCode.EntityNotFound, dto.ErrorCode);
        }

        [TestMethod]
        public void TestCreateOrUpdate()
        {
            var fooDto = new FooDTO(int.MaxValue);

            var initialCount = Maestro.GetAll().Value.Count();

            var newBar = Maestro.CreateOrUpdate(fooDto);

            Assert.AreEqual(initialCount + 1, Maestro.GetAll().Value.Count());
            Assert.AreEqual(int.MaxValue, newBar.Value.Value);

            newBar.Value.Value = int.MinValue;

            newBar = Maestro.CreateOrUpdate(newBar.Value);

            Assert.AreEqual(initialCount + 1, Maestro.GetAll().Value.Count());
            Assert.AreEqual(int.MinValue, newBar.Value.Value);
        }
    }
}
