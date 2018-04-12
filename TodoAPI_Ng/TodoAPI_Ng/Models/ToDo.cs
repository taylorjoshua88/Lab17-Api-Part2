using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI_Ng.Models
{
    public class ToDo
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public bool IsDone { get; set; }

        public int? ListId { get; set; }
        public ToDoList List { get; set; }
    }
}
