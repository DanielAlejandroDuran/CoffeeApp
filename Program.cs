using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CoffeeApp.src.Shared.Context;
using CoffeeApp.src.Modules.User.Infrastructure.Repositories;
using CoffeeApp.src.Modules.User.UI; // Para LoginMenu y SignupMenu
using CoffeeApp.src.Modules.User.Application.Services;

namespace CoffeeApp
{
    class Program
    {
        static void Main(string[] args)
        {
            // 1. Cargar configuración desde appsettings.json
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 2. Configurar DbContext con la cadena de conexión
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseMySql(
                config.GetConnectionString("MySqlDB"),
                new MySqlServerVersion(new Version(8, 0, 36)) // Cambia a tu versión real de MySQL
            );

            // 3. Crear instancia del DbContext
            using var context = new AppDbContext(optionsBuilder.Options);

            // 4. Crear repositorio
            var userRepository = new UserRepository(context);

            // 5. Llamar al menú principal pasando el repositorio
            MostrarMenuPrincipal(userRepository);
        }

        static void MostrarMenuPrincipal(UserRepository userRepository)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("====================================");
                Console.WriteLine("       BIENVENIDO A APPCOFFEE       ");
                Console.WriteLine("====================================");
                Console.WriteLine("\nMenú Principal:\n");
                Console.WriteLine("1. Iniciar sesión");
                Console.WriteLine("2. Registrarse");
                Console.WriteLine("3. Salir");
                Console.WriteLine("\n====================================");
                Console.Write("Seleccione una opción: ");

                var opcion = Console.ReadLine();

                switch (opcion)
                {
                    case "1":
                        LoginMenu.ShowLogin(userRepository); // Aquí ya recibe repo
                        break;
                    case "2":
                        var userService = new UserService(userRepository);
                        var signupMenu = new SignupMenu(userService);
                        signupMenu.Mostrar();
                        break;
                    case "3":
                        Console.WriteLine("\n¡Gracias por usar AppCoffee!");
                        return;
                    default:
                        Console.WriteLine("\nOpción no válida. Intente nuevamente.");
                        Console.ReadKey();
                        break;
                }
            }
        }
    }
}
