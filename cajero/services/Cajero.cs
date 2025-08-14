using System;
using System.IO;


public class Cajero
{
    private Usuario? usuario;
    private string _RUTA_USUARIOS;
    private string _RUTA_HISTORIAL;
    private enum acciones
    {
        Deposito,
        Retiro,
        Consignacion
    }

    List<string[]> usuarios = new List<string[]>();
    public Cajero()
    {
        // Construir ruta absoluta robusta
        string basePath = AppDomain.CurrentDomain.BaseDirectory;
        _RUTA_USUARIOS = Path.Combine(basePath, "../../../../database/cuentas.txt");
        _RUTA_HISTORIAL = Path.Combine(basePath, "../../../../database/historial.txt");

        // 2. Crear archivo vacío si no existe
        if (!File.Exists(_RUTA_USUARIOS))
        {
            File.Create(_RUTA_USUARIOS).Close();
        }
        if (!File.Exists(_RUTA_HISTORIAL))
        {
            File.Create(_RUTA_HISTORIAL).Close();
        }

        // 3. Leer datos si hay algo
        using (StreamReader srUsuarios = new StreamReader(_RUTA_USUARIOS))
        {
            string? linea;
            while ((linea = srUsuarios.ReadLine()) != null)
            {
                string[] user = linea.Split('\t');
                usuarios.Add(user);
            }
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
                            this.ConsigarODespositar();
                            break;
                        case 2:
                            menu = false;
                            this.Retirar();
                            break;
                        default:
                            menu = false;
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

    // loggin y autenticación
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
                    System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(u[3])) == password);

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

    private void ConsigarODespositar()
    {
        bool repetir = false;
        char opcion;
        do
        {
            Console.Clear();
            PintarMenu("¿Que operacion quiere realizar?", ["1. Deposito", "2. Consignación", "(Otra tecla). Salir"]);
            opcion = char.Parse(Console.ReadLine());

            switch (opcion)
            {

                case '1':
                    depositar();
                    break;
                case '2':
                    consignar();
                    break;
                default:
                    Menu();
                    break;
            }
        }
        while (repetir);




    }

    void depositar()
    {
        int opcion;
        decimal cantidad;
        bool repetir = false;
        do
        {
            repetir = false;
            Console.Clear();
            Console.WriteLine($"ingresa la cantidad a depositar: ");
            cantidad = decimal.Parse(Console.ReadLine());
            decimal total = usuario.saldo + cantidad;
            PintarMenu($"Deposito", $"Cantidad: {cantidad}, totlal: {total}", ["1. Si", "2. No", "(Otra tecla). Volver al menú"]);
            opcion = int.Parse(Console.ReadLine());

            switch (opcion)
            {
                case 1:
                    int index = usuarios.FindIndex(x => x[0] == usuario.numeroCuenta);
                    usuario.saldo = total;
                    usuarios[index][4] = total.ToString();
                    GuardarUsuarios();
                    RegistrarMovimiento(acciones.Deposito, cantidad, $"Se depositó {cantidad}, la cuenta queda con {total}");
                    break;
                case 2:
                    repetir = true;
                    break;
                default:
                    Menu();
                    repetir = false;
                    break;
            }
        }
        while (repetir);
    }

    void consignar()
    {
        int opcion;
        decimal cantidad;
        string cuentaAConsignar;
        bool repetir = false;
        do
        {
            repetir = false;
            Console.Clear();
            Console.WriteLine($"ingresa la el #de cuenta a consignar: ");
            cuentaAConsignar = Console.ReadLine();
            Console.WriteLine($"ingresa la cantidad a depositar: ");
            cantidad = decimal.Parse(Console.ReadLine());

            PintarMenu($"Consignación a cuenta #{cuentaAConsignar}", $"Cantidad: {cantidad}", ["1. Si", "2. No", "(Otra tecla). Volver al menú"]);
            opcion = int.Parse(Console.ReadLine());
            switch (opcion)
            {
                case 1:
                    int index = usuarios.FindIndex(x => x[0] == cuentaAConsignar);
                    decimal total = decimal.Parse(usuarios[index][4]) + cantidad;
                    usuarios[index][4] = total.ToString();
                    GuardarUsuarios();
                    RegistrarMovimiento(acciones.Consignacion, cantidad, $"Se envio {cantidad} a la cuenta {cuentaAConsignar}, la cuenta queda con {total}");
                    break;
                case 2:
                    repetir = true;
                    break;
                default:
                    Menu();
                    repetir = false;
                    break;
            }
        } while (repetir);
    }

    void Retirar()
    {
        int opcion;
        decimal cantidad;
        bool repetir = false;
        do
        {
            repetir = false;
            Console.Clear();
            Console.WriteLine($"ingresa la cantidad a retirar: ");
            cantidad = decimal.Parse(Console.ReadLine());
            decimal total = usuario.saldo - cantidad;
            PintarMenu($"Retiro", $"Cantidad: {cantidad}, totlal: {total}", ["1. Si", "2. No", "(Otra tecla). Volver al menú"]);
            opcion = int.Parse(Console.ReadLine());

            switch (opcion)
            {
                case 1:
                    int index = usuarios.FindIndex(x => x[0] == usuario.numeroCuenta);
                    usuario.saldo = total;
                    usuarios[index][4] = total.ToString();
                    GuardarUsuarios();
                    RegistrarMovimiento(acciones.Retiro, cantidad, $"Se retiro {cantidad}, la cuenta queda con {total}");
                    break;
                case 2:
                    repetir = true;
                    break;
                default:
                    Menu();
                    repetir = false;
                    break;
            }
        }
        while (repetir);
    }

    // metodos de persistencia
    void GuardarUsuarios()
    {
        try
        {
            using (StreamWriter sw = new StreamWriter(_RUTA_USUARIOS, append: false))
            {
                foreach (var user  in usuarios) {
                    Console.WriteLine(string.Join("\t", user));
                    sw.WriteLine(string.Join("\t", user));
                }
            }

        } catch(Exception e)
        {
            Console.WriteLine("Error" + e.Message);
            Console.ReadKey();
        }
    }

    void RegistrarMovimiento(acciones tipoMovimiento, decimal cantidad, string descripcion)
    {
        try
        { 
            string[] movimiento =
            {
                this.usuario.numeroCuenta,
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                tipoMovimiento.ToString(),
                cantidad.ToString(),
                descripcion
            };

            using (StreamWriter sw = new StreamWriter(_RUTA_HISTORIAL, append: true))
            {
                sw.WriteLine(string.Join("\t", movimiento));
            }

            Console.WriteLine("¡Movimento realizado!, orpima calquier tecla para ir al menú");
            Console.ReadKey(true);
            Menu();
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            Console.ReadKey();
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

    private void PintarMenu(string titulo, string[] opciones)
    {
        int ancho = Math.Max(titulo.Length, opciones.Max(o => o.Length + 8));

        string borde = new string('-', ancho - 1);
        string vacio = new string(' ', ancho - 1);

        Console.WriteLine($" {borde}");
        Console.WriteLine($"|{titulo.PadLeft((ancho + titulo.Length) / 2).PadRight(ancho - 1)}|");
        Console.WriteLine($" {borde}");

        Console.WriteLine($"|{vacio}|");
        foreach (var opcion in opciones)
        {
            Console.WriteLine($"|{opcion.PadLeft((1 + opcion.Length)).PadRight(ancho - 1)}|");
            Console.WriteLine($"|{vacio}|");
        }

        Console.WriteLine($" {borde}");
    }

    private void PintarMenu(string[] opciones)
    {
        int ancho = opciones.Max(o => o.Length + 8);

        string borde = new string('-', ancho - 1);
        string vacio = new string(' ', ancho - 1);

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
