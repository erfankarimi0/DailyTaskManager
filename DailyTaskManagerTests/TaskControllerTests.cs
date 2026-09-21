using DailyTaskManager.Controllers;
using DailyTaskManager.DTOs;
using DailyTaskManager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DailyTaskManagerTests
{
    public class TaskControllerTests
    {
        //ایجاد تسک با اطلاعات نامعتبر
        [Fact]
        public async Task CreateInvalidTask()
        {
            // Arrange
            var serviceMock = new Mock<ITaskService>();
            var controller = new TasksController(serviceMock.Object);

            controller.ModelState.AddModelError("Title", "Title is required");

            var dto = new CreateTaskDto
            {
                Title = "",
                Description = "Test"
            };

            // Act
            var result = await controller.Create(dto);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
    }
}