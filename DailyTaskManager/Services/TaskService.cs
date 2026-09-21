using DailyTaskManager.Data;
using DailyTaskManager.DTO;
using DailyTaskManager.DTOs;
using DailyTaskManager.Enums;
using DailyTaskManager.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DailyTaskManager.Services
{
    public class TaskService : ITaskService
    {
        private readonly DailyTaskManagerContext _context;

        public TaskService(DailyTaskManagerContext context)
        {
            _context = context;
        }

        public async Task<CreateTaskResultDto> CreateAsync(CreateTaskDto dto)
        {
            var task = new DailyTaskManager.Models.Task
            {
                Title = dto.Title,
                Description = dto.Description,
                IsCompleted = dto.IsCompleted ? (sbyte)1 : (sbyte)0,
                Priority = dto.Priority.ToString(),
                DueDate = dto.DueDate,
            };

            _context.Tasks.Add(task);

            await _context.SaveChangesAsync();

            return new CreateTaskResultDto
            {
                Id = task.Id,
                Message = "Task با موفقیت ایجاد شد."
            };
        }




        public async Task<GetTaskResultDto?> GetAsync(int id)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return null;

            return new GetTaskResultDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted == 1,
                Priority = Enum.Parse<TaskPriority>(task.Priority),
                DueDate = task.DueDate,
                CreateDate = task.CreateDate,
                UpdateDate = task.UpdateDate
            };
        }



        public async Task<GetPageTaskResultDto> GetAllAsync(bool? isCompleted,string? search,string? sort,int page)
        {
            const int pageSize = 10;

            if (page < 1)
                page = 1;

            var query = _context.Tasks.AsQueryable();

            if (isCompleted.HasValue)
            {
                query = query.Where(t => t.IsCompleted == (isCompleted.Value ? 1 : 0));
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.Title.Contains(search));
            }

            if (sort == "newest")
            {
                query = query.OrderByDescending(t => t.CreateDate);
            }
            else if (sort == "oldest")
            {
                query = query.OrderBy(t => t.CreateDate);
            }
            else if (sort == "priority")
            {
                query = query.OrderBy(t =>
                    t.Priority == "High" ? 1 :
                    t.Priority == "Medium" ? 2 : 3);
            }
            else if (sort == "priorityAsc")
            {
                query = query.OrderBy(t =>
                    t.Priority == "Low" ? 1 :
                    t.Priority == "Medium" ? 2 : 3);
            }
            else if (sort == "dueDate")
            {
                query = query
                    .OrderBy(t => t.DueDate == null)
                    .ThenBy(t => t.DueDate);
            }

            var totalItems = await query.CountAsync();

            var totalPages = (int)Math.Ceiling(
                totalItems / (double)pageSize
            );

            var tasks = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var taskResults = tasks.Select(task => new GetAllTaskResultDto
            {
                Id = task.Id,
                Title = task.Title,
                IsCompleted = task.IsCompleted == 1,
                Priority = Enum.Parse<TaskPriority>(task.Priority)
            }).ToList();

            return new GetPageTaskResultDto
            {
                Tasks = taskResults,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };
        }





        public async Task<UpdateTaskResultDto?> UpdateAsync(int id, UpdateTaskDto dto)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return null;

            if (string.IsNullOrWhiteSpace(dto.Title) &&
                string.IsNullOrWhiteSpace(dto.Description) &&
                dto.IsCompleted == null &&
                dto.Priority == null &&
                dto.DueDate == null)
            {
                return null;
            }

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(dto.Title) &&
                dto.Title != task.Title)
            {
                task.Title = dto.Title;
                hasChanges = true;
            }

            if (dto.Description != null &&
                dto.Description != task.Description)
            {
                task.Description = dto.Description;
                hasChanges = true;
            }

            if (dto.IsCompleted.HasValue &&
                task.IsCompleted != (dto.IsCompleted.Value ? (sbyte)1 : (sbyte)0))
            {
                task.IsCompleted = dto.IsCompleted.Value ? (sbyte)1 : (sbyte)0;
                hasChanges = true;
            }

            if (dto.Priority.HasValue &&
                task.Priority != dto.Priority.Value.ToString())
            {
                task.Priority = dto.Priority.Value.ToString();
                hasChanges = true;
            }

            if (dto.DueDate.HasValue &&
                task.DueDate != dto.DueDate.Value)
            {
                task.DueDate = dto.DueDate.Value;
                hasChanges = true;
            }

            if (!hasChanges)
                return null;

            var now = DateTime.Now;

            task.UpdateDate = new DateTime(
                now.Year,
                now.Month,
                now.Day,
                now.Hour,
                now.Minute,
                now.Second
            );
            await _context.SaveChangesAsync();

            return new UpdateTaskResultDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                IsCompleted = task.IsCompleted == 1,
                Priority = Enum.Parse<TaskPriority>(task.Priority),
                DueDate = task.DueDate,
                CreateDate = task.CreateDate,
                UpdateDate = task.UpdateDate
            };
        }



        public async Task<bool> DeleteAsync(int id)
        {
            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return false;

            _context.Tasks.Remove(task);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}