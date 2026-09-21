using DailyTaskManager.DTO;
using DailyTaskManager.DTOs;

namespace DailyTaskManager.Services.Interfaces
{
    public interface ITaskService
    {
        //اول چیزی که میده و جایگاه دوم چیزی که میگیره
        Task<CreateTaskResultDto> CreateAsync(CreateTaskDto dto);
        Task<GetTaskResultDto?> GetAsync(int id);
        Task<GetPageTaskResultDto> GetAllAsync(bool? isCompleted,string? search,string? sort,int page); 
        Task<UpdateTaskResultDto?> UpdateAsync(int id, UpdateTaskDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
