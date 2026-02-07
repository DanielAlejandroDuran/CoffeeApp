using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeApp.src.Modules.User.Domain.Entities;

namespace CoffeeApp.src.Modules.User.Application.Interfaces;

public interface IUserRepository
{
    Task<Usuario?> GetByIdAsync(int Id);
    Task<IEnumerable<Usuario?>> GetAllAsync();
    void Add(Usuario entity);
    void Remove(Usuario entity);
    void Update(Usuario entity);
    Task SaveAsync();
}
