using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI_Ng.Models;

namespace TodoAPI_Ng.Data
{
    public class ToDoNgDbContext : DbContext
    {
        public ToDoNgDbContext(DbContextOptions<ToDoNgDbContext> options) : base(options)
        {
        }

        public DbSet<ToDo> ToDo { get; set; }
        public DbSet<ToDoList> ToDoList { get; set; }
    }
}
