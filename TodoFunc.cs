using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Chunliu.Functions.Models;
using Microsoft.EntityFrameworkCore;

namespace Chunliu.Functions
{
    public class TodoFunc
    {
        private readonly TodoContext _todoContext;
        private const string _route = "todo";

        public TodoFunc(TodoContext todoContext)
        {
            _todoContext = todoContext;
            _todoContext.Database.EnsureCreated();
        }

        [FunctionName("TodoFunc_GetTodos")]
        public async Task<IActionResult> GetTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = _route)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Getting todo list items");
            var todos = await _todoContext.TodoItems.ToListAsync();

            return new OkObjectResult(todos);
        }

        [FunctionName("TodoFunc_GetTodoById")]
        public async Task<IActionResult> GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = _route + "/{id}")] HttpRequest req,
            ILogger log, long id)
        {
            log.LogInformation("Getting todo item by Id");
            var todo = await _todoContext.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                log.LogInformation($"Item {id} not found");
                return new NotFoundResult();
            }

            return new OkObjectResult(todo);
        }

        [FunctionName("TodoFunc_Create")]
        public async Task<IActionResult> Create(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = _route)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating a new todo item");
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<TodoItem>(body);
                if (string.IsNullOrEmpty(input.Name))
                {
                    return new BadRequestResult();
                }
                var todo = new TodoItem { Name = input.Name };
                await _todoContext.TodoItems.AddAsync(todo);
                await _todoContext.SaveChangesAsync();
                return new CreatedObjectResult(_route, todo.Id.ToString(), todo);
            }
            catch(Exception ex)
            {
                log.LogCritical(ex.Message);
                return new BadRequestResult();
            } 
        }

        [FunctionName("TodoFunc_Update")]
        public async Task<IActionResult> Update(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = _route + "/{id}")] HttpRequest req,
            ILogger log, long id)
        {
            log.LogInformation("Updating an existing todo item");
            try
            {
                var body = await new StreamReader(req.Body).ReadToEndAsync();
                var input = JsonConvert.DeserializeObject<TodoItem>(body);
                if (id != input.Id)
                {
                    return new BadRequestResult();
                }

                var todo = await _todoContext.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
                if (todo == null)
                {
                    log.LogInformation($"Item {id} not found");
                    return new NotFoundResult();
                }

                todo.Name = input.Name;
                todo.IsCompleted = input.IsCompleted;
                _todoContext.TodoItems.Update(todo);
                await _todoContext.SaveChangesAsync();
                return new NoContentResult();
            }
            catch
            {
                return new BadRequestResult();
            } 
        }

        [FunctionName("TodoFunc_Delete")]
        public async Task<IActionResult> Delete(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = _route + "/{id}")] HttpRequest req,
            ILogger log, long id)
        {
            log.LogInformation("Getting todo item by Id");
            var todo = await _todoContext.TodoItems.FirstOrDefaultAsync(t => t.Id == id);
            if (todo == null)
            {
                log.LogInformation($"Item {id} not found");
                return new NotFoundResult();
            }

            _todoContext.TodoItems.Remove(todo);
            await _todoContext.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
