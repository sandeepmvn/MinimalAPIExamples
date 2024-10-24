using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todo.Models;

namespace Todo.EFCore
{
    public class ToDoDBContext : DbContext
    {
        public ToDoDBContext()
        {
        }
        public ToDoDBContext(DbContextOptions<ToDoDBContext> options) : base(options)
        {
        }

        public DbSet<TodoEntity> Todos => Set<TodoEntity>();


        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer();
        //    }
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
