using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab.DataAccess.Entities
{
    public enum ToDoState
    {
        Planned,
        Started,
        Finished
    }
    public class ToDo
    {
        public long Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ToDoState State { get; set; }

        public int Index { get; set; }
        public User User { get; set; }
    }
}
