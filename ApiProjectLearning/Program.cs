using Microsoft.EntityFrameworkCore;

namespace ApiProjectLearning
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //builder.Services.AddDatabaseDeveloperPageExceptionFilter();
            builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
            var app = builder.Build();

            var todoItems = app.MapGroup("/todoitems");

            todoItems.MapGet("/", GetAllTodos);
            todoItems.MapGet("/complete", GetCompletedTodos);
            todoItems.MapGet("/{id}", GetTodo);
            todoItems.MapPost("/", CreateTodo);
            todoItems.MapPut("/{id}", UpdateTodo);
            todoItems.MapDelete("/{id}", DeleteTodo);

            app.Run();

            static async Task<IResult> GetAllTodos (TodoDb db)
            {
                return TypedResults.Ok(await db.Todos.Select(x => new TodoDTO(x)).ToArrayAsync());
            }

            static async Task<IResult> GetCompletedTodos (TodoDb db)
            {
                return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoDTO(x)).ToListAsync());
            }

            static async Task<IResult> GetTodo (int id, TodoDb db)
            {
                return await db.Todos.FindAsync(id) is Todo todo ? TypedResults.Ok(new TodoDTO(todo)) : TypedResults.NotFound();
            }

            static async Task<IResult> CreateTodo(TodoDTO todoDTO, TodoDb db)
            {
                var todoItem = new Todo
                {
                    Name = todoDTO.Name,
                    IsComplete = todoDTO.IsComplete
                };

                db.Todos.Add(todoItem);
                await db.SaveChangesAsync();
                var todoItemDTO = new TodoDTO(todoItem);

                return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDTO);
            }

            static async Task<IResult> UpdateTodo(int id, TodoDTO todoDTO, TodoDb db)
            {
                var todo = await db.Todos.FindAsync(id);
                
                if(todo is null) return TypedResults.NotFound();

                todo.Name = todoDTO.Name;
                todo.IsComplete = todoDTO.IsComplete;

                await db.SaveChangesAsync();

                return TypedResults.NoContent();
            }

            static async Task<IResult> DeleteTodo(int id, TodoDb db)
            {
                var todo = await db.Todos.FindAsync(id);

                if(todo is Todo existedTodo)
                {
                    db.Todos.Remove(existedTodo);
                    await db.SaveChangesAsync();
                    return TypedResults.NoContent();
                } 

                return TypedResults.NotFound();
            }
        }
    }
}
