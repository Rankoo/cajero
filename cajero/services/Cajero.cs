using System;

public class Cajero
{
    private Usuario? usuario;
    public Cajero()
    {

    }

    public void Menu()
    {
        bool menu = true;
        int opcion;

        if (usuario == null) {
            
            while(menu)
            {
                try
                {
                    Console.Clear();
                    this.PintarMenu("Bienvenido", "Escoja una Opción:", ["1. Iniciar Sesión", "2. Salir"]);
                    opcion = int.Parse(Console.ReadLine());

                    switch (opcion)
                    {
                        case 1:
                            menu = true;
                            break;
                        case 2:
                            menu = false;
                            break;
                        default:
                            break;
                    }
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                }
            }


        } else
        {
            this.PintarMenu("Bienvenido", "Escoja una Opción:", ["1. Iniciar Sesión", "2. Salir"]);
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
