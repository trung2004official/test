using Microsoft.EntityFrameworkCore;

namespace ApiProjectLearning
{
    public class TodoDb : DbContext
    {
        public TodoDb(DbContextOptions<TodoDb> options) : base(options) { }
        public DbSet<Todo> Todos { get; set; }
    }
}
