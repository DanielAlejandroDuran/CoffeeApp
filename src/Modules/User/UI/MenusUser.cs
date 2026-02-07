using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoffeeApp.src.Modules.User.Infrastructure.Repositories;
using CoffeeApp.src.Modules.User.Application.Services;
using CoffeeApp.src.Modules.User.Domain.Entities;
using CoffeeApp.src.Shared.Context;
using CoffeeApp.src.Modules.CoffeVarieties.UI;

namespace CoffeeApp.src.Modules.User.UI;

public class LoginMenu
{
    public static async Task ShowLogin(UserRepository userRepository)
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("          INICIAR SESIÓN           ");
        Console.WriteLine("====================================");

        Console.Write("\nEmail: ");
        var email = Console.ReadLine();

        Console.Write("Contraseña: ");
        var password = OcultarContrasena();

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("\nEmail y contraseña son obligatorios.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }

        var usuario = (await userRepository.GetAllAsync())
                    .FirstOrDefault(u => u != null && u.email == email && u.password_hash == password);

        if (usuario == null)
        {
            Console.WriteLine("\nCredenciales inválidas.");
            Console.WriteLine("Presione cualquier tecla para continuar...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine($"\n¡Bienvenido {usuario.nombre}!");
        Console.WriteLine("Presione cualquier tecla para continuar...");
        Console.ReadKey();
        Console.Clear();
        
    }

    private static string OcultarContrasena()
    {
        var password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Remove(password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }
}

public class SignupMenu
{
    public static void ShowSignup(UserRepository userRepository)
    {
        Console.Clear();
        Console.WriteLine("====================================");
        Console.WriteLine("          REGISTRARSE              ");
        Console.WriteLine("====================================");

        Console.Write("\nNombre: ");
        var nombre = Console.ReadLine();

        Console.Write("Email: ");
        var email = Console.ReadLine();

        Console.Write("Contraseña: ");
        var password = OcultarContrasena();

        Console.Write("\nConfirmar contraseña: ");
        var confirmPassword = OcultarContrasena();

        ValidarContrasenas(password, confirmPassword);

        // Aquí puedes llamar al servicio de registro de usuario
    }

    private readonly UserService _userService;

    // Inyectamos el servicio en el constructor
    public SignupMenu(UserService userService)
    {
        _userService = userService;
    }

    public void Mostrar()
    {
        Console.Clear();
        MostrarEncabezado();

        var (nombre, email, password) = ObtenerDatosRegistro();

        try
        {
            // Usamos el servicio para registrar el usuario
            _userService.RegistrarUsuarioConTareaAsync(nombre, email, password).Wait();

            Console.WriteLine("\n¡Registro exitoso! Presione cualquier tecla para continuar...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
            Console.WriteLine("Presione cualquier tecla para volver a intentar...");
        }

        Console.ReadKey();
    }

    private void MostrarEncabezado()
    {
        Console.WriteLine("===============================");
        Console.WriteLine("      REGISTRO DE USUARIO      ");
        Console.WriteLine("===============================");
        Console.WriteLine();
    }

    private (string nombre, string email, string password) ObtenerDatosRegistro()
    {
        Console.Write("Nombre completo: ");
        var nombre = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(nombre))
            throw new Exception("El nombre no puede estar vacío");

        Console.Write("Email: ");
        var email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
            throw new Exception("El email no puede estar vacío");

        Console.Write("Contraseña: ");
        var password = OcultarContrasena();

        Console.Write("\nConfirmar contraseña: ");
        var confirmPassword = OcultarContrasena();

        ValidarContrasenas(password, confirmPassword);

        return (nombre, email, password);
    }

    private static void ValidarContrasenas(string password, string confirmPassword)
    {
        if (password != confirmPassword)
        {
            throw new Exception("Las contraseñas no coinciden");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new Exception("La contraseña debe tener al menos 6 caracteres");
        }
    }

    public static string OcultarContrasena()
    {
        var password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password.Remove(password.Length - 1);
                Console.Write("\b \b");
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }


}
