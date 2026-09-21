using DailyTaskManager.DTO;
using DailyTaskManager.DTOs;
using DailyTaskManager.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DailyTaskManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;

        public TasksController(ITaskService taskService)
        {
            _taskService = taskService;
        }



        [HttpPost]
        public async Task<ActionResult<CreateTaskResultDto>> Create(CreateTaskDto dto)
        {
            //برای تست همان کار ApiController را میکند اما برای تست دستی ...
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _taskService.CreateAsync(dto);

            return StatusCode(201, result);
        }



            [HttpGet("{id}")]
        public async Task<ActionResult<GetTaskResultDto>> Get(int id)
        {
            var result = await _taskService.GetAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }



        [HttpGet]
        public async Task<ActionResult<GetPageTaskResultDto>> GetAll(bool? isCompleted,string? search,string? sort,int page = 1)
        {
            var result = await _taskService.GetAllAsync(isCompleted,search,sort,page);

            return Ok(result);
        }



        [HttpPut("{id}")]
        public async Task<ActionResult<UpdateTaskResultDto>> Update(int id,UpdateTaskDto dto)
        {
            var result = await _taskService.UpdateAsync(id, dto);

            if (result == null)
                return NotFound();

            return Ok(result);
        }



        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _taskService.DeleteAsync(id);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
