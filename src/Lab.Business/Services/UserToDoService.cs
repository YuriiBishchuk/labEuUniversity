using Lab.API.Helpers;
using Lab.Business.Models;
using Lab.DataAccess.Entities;
using Lab.DataAccess.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab.Business.Services
{
    public interface IUserToDoService
    {
        Task<IEnumerable<ToDo>> GetToDosByUserId(int userId);
        Task<ToDo> GetToDoByIdForUser(int userId, int id);
        Task CreateToDoForUser(int userId, ToDo toDo);
        Task UpdateToDoForUser(int userId, int id, ToDo toDo);
        Task DeleteToDoForUser(int userId, int id);
    }

    public class UserToDoService : IUserToDoService
    {
        private readonly IToDoRepository _toDoRepository;

        public UserToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        public async Task<IEnumerable<ToDo>> GetToDosByUserId(int userId)
        {
            return await _toDoRepository.GetIQueryable().Where(t => t.UserId == userId).ToListAsync();
        }

        public async Task<ToDo> GetToDoByIdForUser(int userId, int id)
        {
            var toDo = await _toDoRepository.GetIQueryable().FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);
            if (toDo == null)
            {
                throw new Exception("Завдання не знайдено або не належить вам.");
            }
            return toDo;
        }

        public async Task CreateToDoForUser(int userId, ToDo toDo)
        {
            if (toDo == null)
            {
                throw new ArgumentNullException(nameof(toDo));
            }

            if (toDo.UserId != userId)
            {
                throw new Exception("Ви можете створювати тільки завдання для себе.");
            }

            await _toDoRepository.AddAsync(toDo);
            await _toDoRepository.SaveChangesAsync();
        }

        public async Task UpdateToDoForUser(int userId, int id, ToDo toDo)
        {
            var existingToDo = await _toDoRepository.GetIQueryable().FirstOrDefaultAsync(t => t.UserId == userId && t.Id == id);
            if (existingToDo == null)
            {
                throw new Exception("Завдання не знайдено або не належить вам.");
            }

            existingToDo.Title = toDo.Title;
            existingToDo.Description = toDo.Description;
            existingToDo.State = toDo.State;

            await _toDoRepository.SaveChangesAsync();
        }

        public async Task DeleteToDoForUser(int userId, int id)
        {
            var toDo = await _toDoRepository.GetIQueryable().AnyAsync(t => t.UserId == userId && t.Id == id);
            if (!toDo)
            {
                throw new Exception("Завдання не знайдено або не належить вам.");
            }

            await _toDoRepository.DeleteAsync(id);
            await _toDoRepository.SaveChangesAsync();
        }
    }
}
