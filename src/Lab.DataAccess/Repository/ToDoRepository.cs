using Lab.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab.DataAccess.Repository
{
    public interface IToDoRepository: IBaseRepository<ToDo>
    {
    }
    public class ToDoRepository : BaseRepository<ToDo>, IToDoRepository
    {
        public ToDoRepository(AppDbContext dbContext) : base(dbContext) { }


    }
}
