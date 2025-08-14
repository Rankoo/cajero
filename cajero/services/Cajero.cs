using System;
using System.IO;


public class Cajero
{
    private Usuario? usuario;
    private readonly string _RUTA = "../../../../database/cuentas.txt";
    private StreamReader sr;
    List<string[]> usuarios = new List<string[]> ();
    public Cajero()
    {
        string basePath = AppDomain.CurrentDomain.BaseDirectory;

        sr = new StreamReader(_RUTA);
        string linea;
        string[] user;
        while ((linea = sr.ReadLine()) != null)
        {
            user = linea.Split('\t');
            usuarios.Add(user);

        }
    }

    // Menú del cajero, muestra 2 diferentes menús dependiendo si el usuario esta logueado o no
    public void Menu()
    {
        bool menu = true;
        int opcion;

        try
        {
            while (menu)
            {
                Console.Clear();
                if (usuario == null)
                {

                    Console.Clear();
                    this.PintarMenu("Bienvenido", "Escoja una Opción:", ["1. Iniciar Sesión", "2. Salir"]);
                    opcion = int.Parse(Console.ReadLine());

                    switch (opcion)
                    {
                        case 1:
                            menu = false;
                            this.IniciarSesion();
                            break;
                        case 2:
                            menu = false;
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    this.PintarMenu(
                        $"Bienvenido {usuario.nombre}",
                        "Escoja una Opción:",
                        [
                            "1. Depositar dinero",
                            "2. Retirar dinero",
                            "3. Consultar saldo",
                            "4. Ver últimos 5 movimientos",
                            "5. Cambiar clave",
                            "6. Salir"
                        ]);

                    opcion = int.Parse(Console.ReadLine());

                    switch (opcion)
                    {
                        case 1:
                            menu = false;
                            this.IniciarSesion();
                            break;
                        case 2:
                            menu = false;
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public void IniciarSesion()
    {
        string user;
        string password;
        bool continuar = true;

        while (continuar)
        {
            Console.Clear();
            if (usuario == null)
            {
                Console.WriteLine("Ingresa el Usuario: ");
                user = Console.ReadLine();
                Console.WriteLine("Ingresa el Contraseña: ");
                password = Console.ReadLine();

                var existe = usuarios.FirstOrDefault(u =>
                    u[0] == user && 
                    System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(u[3])) == password );

                if (existe != null)
                {
                    this.usuario = new Usuario();
                    usuario.numeroCuenta = existe[0];
                    usuario.nombre = existe[1];
                    usuario.apellido = existe[2];
                    usuario.contraseña = existe[3];
                    usuario.saldo = decimal.Parse(existe[4]);

                    Console.WriteLine("");
                    Console.WriteLine($"Bienvenido {existe[1]}");
                    Console.WriteLine("Oprima una tecla para continuar... ");
                    Console.ReadKey(true);
                    continuar = false;                
                    this.Menu();
                }
                else
                {
                    char option;
                    Console.WriteLine("Usuario y/o contraseña equivocados");
                    Console.WriteLine("Oprima 1 para intentar nuevamente");
                    option = char.Parse(Console.ReadLine());
                    switch (option)
                    {
                        case '1':
                            break;
                        default:
                            continuar = false;
                            this.Menu();
                            break;

                    }
                }
            }
            else
            {
                Console.WriteLine($"El susuario {usuario.nombre} esta logueado");
                continuar = false;
                this.Menu();
            }
        }
    }

    // Metodo para dibujar menus
    private void PintarMenu(string titulo, string subtitulo, string[] opciones)
    {
        int ancho = Math.Max(titulo.Length, subtitulo.Length) + 8;
        ancho = Math.Max(ancho, opciones.Max(o => o.Length + 8));

        string borde = new string('-', ancho - 1);
        string vacio = new string(' ', ancho - 1);

        Console.WriteLine($" {borde}");
        Console.WriteLine($"|{titulo.PadLeft((ancho + titulo.Length) / 2).PadRight(ancho - 1)}|");
        Console.WriteLine($" {borde}");
        Console.WriteLine($"|{subtitulo.PadLeft((ancho + subtitulo.Length) / 2).PadRight(ancho - 1)}|");
        Console.WriteLine($" {borde}");


        Console.WriteLine($"|{vacio}|");
        foreach (var opcion in opciones)
        {
            Console.WriteLine($"|{opcion.PadLeft((1 + opcion.Length)).PadRight(ancho - 1)}|");
            Console.WriteLine($"|{vacio}|");
        }

        Console.WriteLine($" {borde}");
    }
}
