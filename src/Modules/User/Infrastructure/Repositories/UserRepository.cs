using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeApp.src.Modules.User.Application.Interfaces;
using CoffeeApp.src.Modules.User.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CoffeeApp.src.Shared.Context;

namespace CoffeeApp.src.Modules.User.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.id == id);
    }

    public async Task<IEnumerable<Usuario?>> GetAllAsync() =>
        await _context.Usuarios.ToListAsync();

    public void Add(Usuario entity) =>
        _context.Usuarios.Add(entity);

    public void Remove(Usuario entity) =>
        _context.Usuarios.Remove(entity);

    public void Update(Usuario entity) =>
         _context.Usuarios.Update(entity);

    public async Task SaveAsync() =>
    await _context.SaveChangesAsync();
}
