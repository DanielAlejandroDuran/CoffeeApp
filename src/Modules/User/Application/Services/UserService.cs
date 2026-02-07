using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeApp.src.Modules.User.Application.Interfaces;
using CoffeeApp.src.Modules.User.Domain.Entities;

namespace CoffeeApp.src.Modules.User.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _repo;

    public UserService(IUserRepository repo)
    {
        _repo = repo;
    }

    public Task<IEnumerable<Usuario>> ConsultarUsuariosAsync()
    {
        return _repo.GetAllAsync()!;
    }

    public async Task RegistrarUsuarioConTareaAsync(string name, string email, string password)
    {
        var existentes = await _repo.GetAllAsync();
        if (existentes.Any(u => u?.email == email))
            throw new Exception("el usuario ya exite.");
        var user = new Usuario
        {
            nombre = name,
            email = email,
            password_hash = PasswordHelper.HashPassword(password),
            fecha_registro = DateTime.Now,
            verificado = false,
            rol = "User"
        };
        _repo.Add(user);
        await _repo.SaveAsync();
    }
    public async Task ActualizarUsuario(int id, string newname, string newemail)
    {
        var user = await _repo.GetByIdAsync(id);

        if (user == null)
            throw new Exception($"Usuario con ID {id}no encontrado.");
        user.nombre = newname;
        user.email = newemail;

        _repo.Update(user);
        await _repo.SaveAsync();
    }

    public async Task EliminarUsuario(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null)
            throw new Exception($"Usuario con ID {id} no encontrado.");
        _repo.Remove(user);
        await _repo.SaveAsync();
    }

    public async Task<Usuario?> ObtenerUsuarioPorIdAsync(int id)
    {
        return await _repo.GetByIdAsync(id);
    }
}
