using Robot.Expected;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Robot.DAL.Core
{
    public interface IMaestro<EntDto, Key> 
        where EntDto : BaseDTO<Key>
    {
        Expected<EntDto> Create(EntDto dtoObject);
        Expected<EntDto> Update(EntDto dtoObject);
        Expected<bool> Delete(EntDto dtoObject);
        Expected<IEnumerable<EntDto>> GetAll();
        Expected<IEnumerable<EntDto>> GetAll(int page, int pageSize);
        Expected<EntDto> GetById(Key id);
        Expected<EntDto> CreateOrUpdate(EntDto dtoObject);

        Expected<CustomDto> Create<CustomDto>(EntDto dtoObject);
        Expected<CustomDto> Update<CustomDto>(EntDto dtoObject);
        Expected<IEnumerable<CustomDto>> GetAll<CustomDto>();
        Expected<IEnumerable<CustomDto>> GetAll<CustomDto>(int page, int pageSize);
        Expected<CustomDto> GetById<CustomDto>(Key id);
        Expected<CustomDto> CreateOrUpdate<CustomDto>(EntDto dtoObject);

        Task<Expected<EntDto>> CreateAsync(EntDto dtoObject);
        Task<Expected<EntDto>> UpdateAsync(EntDto dtoObject);
        Task<Expected<EntDto>> CreateOrUpdateAsync(EntDto dtoObject);
        Task<Expected<bool>> DeleteAsync(EntDto dtoObject);

        Task<Expected<CustomDto>> CreateAsync<CustomDto>(EntDto dtoObject);
        Task<Expected<CustomDto>> UpdateAsync<CustomDto>(EntDto dtoObject);
        Task<Expected<CustomDto>> CreateOrUpdateAsync<CustomDto>(EntDto dtoObject);
    }
}
