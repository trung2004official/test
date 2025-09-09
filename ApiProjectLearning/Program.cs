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

            app.MapGet("/", () => "Hello World!");

            app.MapGet("/todoitems", async (TodoDb db) =>
                await db.Todos.ToListAsync());

            app.MapGet("/todoitems/complete", async (TodoDb db) =>
                await db.Todos.Where(t => t.IsComplete).ToListAsync());

            app.MapPost("/todoitems", async (Todo todo, TodoDb db) =>
            {
                db.Todos.Add(todo);
                await db.SaveChangesAsync();

                return Results.Created($"/todoitems/{todo.Id}", todo);
            });

            app.MapPut("/todoitems/{id}", async (int id, Todo InputTodo, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo is null) return Results.NotFound();

                todo.Name = InputTodo.Name;
                todo.IsComplete = InputTodo.IsComplete;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
            {
                if(await db.Todos.FindAsync(id) is Todo todo)
                {
                    db.Todos.Remove(todo);
                    await db.SaveChangesAsync();
                    return Results.NoContent(); 
                };

                return Results.NotFound();
            });

            app.Run();
        }
    }
}
