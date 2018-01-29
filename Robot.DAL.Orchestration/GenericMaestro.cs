using AutoMapper;
using Robot.DAL.Core;
using Robot.DAL.Orchestration.Extensions;
using Robot.Expected;
using Robot.Expected.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace Robot.DAL.Orchestration
{
    public class GenericMaestro<T, TDto, K> : BaseMaestro, IMaestro<TDto, K>
        where T : BaseEntity<K>
        where TDto : BaseDTO<K>
    {

        #region Private Fields

        private IServiceWithId<T, K> _service;

        #endregion

        public GenericMaestro(IServiceWithId<T, K> entityService, ITransactionManager transactionManager, IMapper mapper) : base(transactionManager, mapper)
        {
            _service = entityService;
        }

        public GenericMaestro(IServiceWithId<T, K> entityService, IMapper mapper) : this(entityService, new DummyTransactionManager(), mapper) { }

        #region IMaestro Methods

        public virtual Expected<IEnumerable<TDto>> GetAll(int page, int pageSize) => GetAll<TDto>(page, pageSize);

        public virtual Expected<IEnumerable<TDto>> GetAll() => GetAll<TDto>();

        public virtual Expected<TDto> Create(TDto dtoObject) => Create<TDto>(dtoObject);

        public virtual Expected<TDto> Update(TDto dtoObject) => Update<TDto>(dtoObject);

        public virtual Expected<TDto> GetById(K id) => GetById<TDto>(id);

        public virtual Expected<TDto> CreateOrUpdate(TDto dtoObject) => CreateOrUpdate<TDto>(dtoObject);

        public virtual Expected<bool> Delete(TDto dtoObject)
        {
            var entityService = _service;

            var entity = AutoMapper.Map<T>(dtoObject);

            entityService.Delete(entity);

            var saveResult = SaveChanges();
            if (saveResult.HasError)
                return Expected<bool>.ConvertErrorFrom(saveResult);

            return true;
        }

        public virtual Expected<IEnumerable<CustomDto>> GetAll<CustomDto>(int page, int pageSize)
        {
            var entities = GetAllEntities(page, pageSize);

            if (entities.HasError)
                return Expected<IEnumerable<CustomDto>>.ConvertErrorFrom(entities);

            return Project<T, CustomDto>(entities.Value).AsExpected();
        }

        public virtual Expected<IEnumerable<CustomDto>> GetAll<CustomDto>()
        {
            var objectList = GetAllEntities();

            if (objectList.HasError)
                return Expected<IEnumerable<CustomDto>>.ConvertErrorFrom(objectList);

            return Project<T, CustomDto>(objectList.Value).AsExpected();
        }

        public virtual Expected<CustomDto> Create<CustomDto>(TDto dtoObject)
        {
            var entity = AutoMapper.Map<T>(dtoObject);

            var newEntity = CreateEntity(entity);
            if (newEntity.HasError)
                return Expected<CustomDto>.ConvertErrorFrom(newEntity);

            return AutoMapper.Map<CustomDto>(newEntity.Value);
        }

        public virtual Expected<CustomDto> Update<CustomDto>(TDto dtoObject)
        {
            var entity = AutoMapper.Map<T>(dtoObject);

            var newEntity = UpdateEntity(entity);
            if (newEntity.HasError)
                return Expected<CustomDto>.ConvertErrorFrom(newEntity);

            return AutoMapper.Map<CustomDto>(newEntity.Value);
        }

        public virtual Expected<CustomDto> GetById<CustomDto>(K id)
        {
            var entity = GetEntityById(id);
            if (entity.HasError)
                return Expected<CustomDto>.ConvertErrorFrom(entity);

            return AutoMapper.Map<CustomDto>(entity.Value);
        }

        public virtual Expected<CustomDto> CreateOrUpdate<CustomDto>(TDto dtoObject)
        {
            var entity = AutoMapper.Map<T>(dtoObject);

            var newEntity = CreateOrUpdateEntity(entity);
            if (newEntity.HasError)
                return Expected<CustomDto>.ConvertErrorFrom(newEntity);

            return AutoMapper.Map<CustomDto>(newEntity.Value);
        }

        public Task<Expected<TDto>> CreateAsync(TDto dtoObject) => CreateAsync<TDto>(dtoObject);

        public Task<Expected<TDto>> UpdateAsync(TDto dtoObject) => UpdateAsync<TDto>(dtoObject);

        public Task<Expected<TDto>> CreateOrUpdateAsync(TDto dtoObject) => CreateOrUpdateAsync<TDto>(dtoObject);

        public async Task<Expected<bool>> DeleteAsync(TDto dtoObject)
        {
            var entityService = _service;

            var entity = AutoMapper.Map<T>(dtoObject);

            entityService.Delete(entity);

            var saveResult = await SaveChangesAsync();
            if (saveResult.HasError)
                return Expected<bool>.ConvertErrorFrom(saveResult);

            return true;
        }

        public async Task<Expected<CustomDto>> CreateAsync<CustomDto>(TDto dtoObject)
        {
            var entity = AutoMapper.Map<T>(dtoObject);

            var newEntity = await CreateEntityAsync(entity);
            if (newEntity.HasError)
                return Expected<CustomDto>.ConvertErrorFrom(newEntity);

            return AutoMapper.Map<CustomDto>(newEntity.Value);
        }

        public async Task<Expected<CustomDto>> UpdateAsync<CustomDto>(TDto dtoObject)
        {
            var entity = AutoMapper.Map<T>(dtoObject);

            var newEntity = await UpdateEntityAsync(entity);
            if (newEntity.HasError)
                return Expected<CustomDto>.ConvertErrorFrom(newEntity);

            return AutoMapper.Map<CustomDto>(newEntity.Value);
        }

        public async Task<Expected<CustomDto>> CreateOrUpdateAsync<CustomDto>(TDto dtoObject)
        {
            var entity = AutoMapper.Map<T>(dtoObject);

            var newEntity = await CreateOrUpdateEntityAsync(entity);
            if (newEntity.HasError)
                return Expected<CustomDto>.ConvertErrorFrom(newEntity);

            return AutoMapper.Map<CustomDto>(newEntity.Value);
        }

        #endregion

        #region Protected Utils

        protected virtual Expected<T> GetEntityById(K id)
        {
            var entityService = _service;

            var entity = entityService.GetById(id);

            if (entity == default(T))
                return Expected<T>.ErrorFormat(DALErrorCode.EntityNotFound, "{0} with id '{1}' was not found.", typeof(T).Name, id);

            return entity;
        }

        protected virtual Expected<IQueryable<T>> GetAllEntities(int page, int pageSize)
        {
            var entities = GetAllEntities();
            if (entities.HasError)
                return entities;

            var entityPage = entities.Value
                                        .OrderBy(e => e.Id)
                                        .Skip(page * pageSize)
                                        .Take(pageSize);

            return entityPage.AsExpected();
        }

        protected virtual Expected<IQueryable<T>> GetAllEntities()
        {
            var entityService = _service;

            var objectList = entityService.GetAll();

            if (objectList == null)
            {
                var errorMessage = "There was an error while fetching all '{0}' objects.".FormatWith(nameof(TDto));
                return Expected<IQueryable<T>>.Error(DALErrorCode.DatabaseError, errorMessage);
            }

            return objectList.AsExpected();
        }

        protected virtual Expected<T> UpdateEntity(T entity)
        {
            var updatedEntity = UpdateEntityInContext(entity);
            if (updatedEntity.HasError)
                return updatedEntity;

            var saveResult = SaveChanges();
            if (saveResult.HasError)
                return Expected<T>.ConvertErrorFrom(saveResult);

            return GetEntityById(updatedEntity.Value.Id);
        }

        protected virtual Expected<T> CreateEntity(T entity)
        {
            var createdEntity = CreateEntityInContext(entity);
            if (createdEntity.HasError)
                return Expected<T>.ConvertErrorFrom(createdEntity);

            var saveResult = SaveChanges();
            if (saveResult.HasError)
                return Expected<T>.ConvertErrorFrom(saveResult);

            return GetEntityById(createdEntity.Value.Id);
        }

        protected virtual Expected<T> CreateOrUpdateEntity(T entity)
        {
            var upsertResult = UpsertEntityInContext(entity);
            if (upsertResult.HasError)
                return upsertResult;

            var saveResult = SaveChanges();
            if (saveResult.HasError)
                return Expected<T>.ConvertErrorFrom(saveResult);

            return GetEntityById(upsertResult.Value.Id);
        }
        
        protected virtual async Task<Expected<T>> CreateEntityAsync(T entity)
        {
            var createdEntity = CreateEntityInContext(entity);
            if (createdEntity.HasError)
                return Expected<T>.ConvertErrorFrom(createdEntity);

            var saveResult = await SaveChangesAsync();
            if (saveResult.HasError)
                return Expected<T>.ConvertErrorFrom(saveResult);

            return GetEntityById(createdEntity.Value.Id);
        }

        protected virtual async Task<Expected<T>> UpdateEntityAsync(T entity)
        {
            var updatedEntity = UpdateEntityInContext(entity);
            if (updatedEntity.HasError)
                return updatedEntity;

            var saveResult = await SaveChangesAsync();
            if (saveResult.HasError)
                return Expected<T>.ConvertErrorFrom(saveResult);

            return GetEntityById(updatedEntity.Value.Id);
        }

        protected virtual async Task<Expected<T>> CreateOrUpdateEntityAsync(T entity)
        {
            var upsertResult = UpsertEntityInContext(entity);
            if (upsertResult.HasError)
                return upsertResult;

            var saveResult = await SaveChangesAsync();
            if (saveResult.HasError)
                return Expected<T>.ConvertErrorFrom(saveResult);

            return GetEntityById(upsertResult.Value.Id);
        }

        #endregion

        #region Private Utils

        private Expected<T> CreateEntityInContext(T entity)
        {
            var newEntity = _service.Create(entity);
            if (newEntity == default(T))
            {
                var errorMessage = "There was an error while creating a new {0}.".FormatWith(typeof(T).Name);
                return Expected<T>.Error(DALErrorCode.DatabaseError, errorMessage);
            }

            return newEntity;
        }

        private Expected<T> UpdateEntityInContext(T entity)
        {
            var newEntity = _service.Update(entity);
            if (newEntity == default(T))
            {
                var errorMessage = "There was an error while updating {0} with id '{1}'.".FormatWith(typeof(T).Name, entity.Id);
                return Expected<T>.Error(DALErrorCode.DatabaseError, errorMessage);
            }

            return newEntity;
        }

        private Expected<T> UpsertEntityInContext(T entity)
        {
            var newEntity = _service.AddOrUpdate(entity);
            if (newEntity == default(T))
            {
                var errorMessage = "There was an error while upserting the {0} with Id '{1}'.".FormatWith(typeof(T).Name, entity.Id);
                return Expected<T>.Error(DALErrorCode.DatabaseError, errorMessage);
            }

            return newEntity;
        }
        
        #endregion
    }
}
