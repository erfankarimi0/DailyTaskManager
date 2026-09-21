using DailyTaskManager.Services;
using DailyTaskManagerTests.Helpers;
using DailyTaskManager.DTOs;
using DailyTaskManager.Enums;


namespace DailyTaskManagerTests
{
    public class TaskServiceTests
    {
        //دریافت تسک ناموجود
        [Fact]
        public async Task GetNonExistentTask()
        {
            // Arrange
            var context = TestDbContext.Create();
            var service = new TaskService(context);

            // Act
            var result = await service.GetAsync(9999);

            // Assert
            Assert.Null(result);
        }


        //ایجاد تسک موفق
        [Fact]
        public async Task CreateValidTask()
        {
            // Arrange
            var context = TestDbContext.Create();
            var service = new TaskService(context);

            var dto = new CreateTaskDto
            {
                Title = "Test Task",
                Description = "Testing",
                IsCompleted = false,
                Priority = TaskPriority.Medium,
                DueDate = null
            };

            // Act
            var result = await service.CreateAsync(dto);

            // Assert
            Assert.True(result.Id > 0);
        }


        //دریافت تسک موجود
        [Fact]
        public async Task GetExistingTask()
        {
            // Arrange
            var context = TestDbContext.Create();

            var task = new DailyTaskManager.Models.Task
            {
                Title = "Existing Task",
                Description = "Test",
                IsCompleted = 0,
                Priority = "Medium",
                DueDate = null,
                CreateDate = DateTime.Now
            };

            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            // Act
            var result = await service.GetAsync(task.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id, result.Id);
            Assert.Equal(task.Title, result.Title);
        }


        //دریافت لیست تسک ها
        [Fact]
        public async Task GetAllTasks()
        {
            // Arrange
            var context = TestDbContext.Create();

            var task1 = new DailyTaskManager.Models.Task
            {
                Title = "Task 1",
                Description = "Test 1",
                IsCompleted = 0,
                Priority = "Low",
                CreateDate = DateTime.Now
            };

            var task2 = new DailyTaskManager.Models.Task
            {
                Title = "Task 2",
                Description = "Test 2",
                IsCompleted = 1,
                Priority = "High",
                CreateDate = DateTime.Now
            };

            context.Tasks.AddRange(task1, task2);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            // Act
            var result = await service.GetAllAsync(null, null, null, 1);

            // Assert
            Assert.Equal(2, result.Tasks.Count);
        }


        //نمایش تسک های انجام شده
        [Fact]
        public async Task GetCompletedTasks()
        {
            // Arrange
            var context = TestDbContext.Create();

            var task1 = new DailyTaskManager.Models.Task
            {
                Title = "Task 1",
                IsCompleted = 0,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            var task2 = new DailyTaskManager.Models.Task
            {
                Title = "Task 2",
                IsCompleted = 1,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            var task3 = new DailyTaskManager.Models.Task
            {
                Title = "Task 3",
                IsCompleted = 1,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            context.Tasks.AddRange(task1, task2, task3);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            // Act
            var result = await service.GetAllAsync(true, null, null, 1);

            // Assert
            Assert.Equal(2, result.Tasks.Count);
            Assert.All(result.Tasks, task => Assert.True(task.IsCompleted));
        }


        //نمایش تسک های انجام نشده
        [Fact]
        public async Task GetPendingTasks()
        {
            // Arrange
            var context = TestDbContext.Create();

            var task1 = new DailyTaskManager.Models.Task
            {
                Title = "Task 1",
                IsCompleted = 0,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            var task2 = new DailyTaskManager.Models.Task
            {
                Title = "Task 2",
                IsCompleted = 1,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            var task3 = new DailyTaskManager.Models.Task
            {
                Title = "Task 3",
                IsCompleted = 0,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            context.Tasks.AddRange(task1, task2, task3);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            // Act
            var result = await service.GetAllAsync(false, null, null, 1);

            // Assert
            Assert.Equal(2, result.Tasks.Count);
            Assert.All(result.Tasks, task => Assert.False(task.IsCompleted));
        }

        //ویرایش یک تسک موجود
        [Fact]
        public async Task UpdateTask()
        {
            // Arrange
            var context = TestDbContext.Create();

            var task = new DailyTaskManager.Models.Task
            {
                Title = "Old Title",
                Description = "Old Description",
                IsCompleted = 0,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var dto = new UpdateTaskDto
            {
                Title = "New Title"
            };

            // Act
            var result = await service.UpdateAsync(task.Id, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(task.Id, result.Id);
            Assert.Equal("New Title", result.Title);
            Assert.NotNull(result.UpdateDate);
        }


        //ویرایش یک تسک ناموجود
        [Fact]
        public async Task UpdateNonExistentTask()
        {
            // Arrange
            var context = TestDbContext.Create();
            var service = new TaskService(context);

            var dto = new UpdateTaskDto
            {
                Title = "New Title"
            };

            // Act
            var result = await service.UpdateAsync(999999, dto);

            // Assert
            Assert.Null(result);
        }


        //وقتی هیچ تغییری برای ویرایش نیست
        [Fact]
        public async Task UpdateWithoutChanges()
        {
            // Arrange
            var context = TestDbContext.Create();

            var task = new DailyTaskManager.Models.Task
            {
                Title = "Test Task",
                Description = "Test Description",
                IsCompleted = 0,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var dto = new UpdateTaskDto();

            // Act
            var result = await service.UpdateAsync(task.Id, dto);

            // Assert
            Assert.Null(result);
        }


        //تغییر وضعیت انجام تسک
        [Fact]
        public async Task UpdateTaskStatus()
        {
            // Arrange
            var context = TestDbContext.Create();

            var task = new DailyTaskManager.Models.Task
            {
                Title = "Test Task",
                Description = "Test Description",
                IsCompleted = 0,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var dto = new UpdateTaskDto
            {
                IsCompleted = true
            };

            // Act
            var result = await service.UpdateAsync(task.Id, dto);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsCompleted);
        }


        //حذف تسک موجود
        [Fact]
        public async Task DeleteTask()
        {
            // Arrange
            var context = TestDbContext.Create();

            var task = new DailyTaskManager.Models.Task
            {
                Title = "Task To Delete",
                Description = "Test",
                IsCompleted = 0,
                Priority = "Medium",
                CreateDate = DateTime.Now
            };

            context.Tasks.Add(task);
            await context.SaveChangesAsync();

            var service = new TaskService(context);

            // Act
            var result = await service.DeleteAsync(task.Id);

            // Assert
            Assert.True(result);

            var deletedTask = await context.Tasks.FindAsync(task.Id);
            Assert.Null(deletedTask);
        }


        //حذف تسک ناموجود
        [Fact]
        public async Task DeleteNonExistentTask()
        {
            // Arrange
            var context = TestDbContext.Create();
            var service = new TaskService(context);

            // Act
            var result = await service.DeleteAsync(999999);

            // Assert
            Assert.False(result);
        }
    }
}