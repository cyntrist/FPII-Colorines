using System;

namespace FPII_Colorines
{
    internal class Program
    {
        const int ANCHO = 15,
                  ALTO = 15;
        private static readonly Random rnd = new Random();
        enum Color { Azul, Cian, Verde, Morado, Rojo, Amarillo, Ninguno };
        struct Posicion { public int x, y; }
        struct Visitados
        {
            public Posicion[] posVis;
            public int pend, fin; //pend es apartir de las que empiezan las pendientes, fin es el primer espacio vacio
        }

        static void Main(string[] args)
        {
            Color[,] tab = new Color[ANCHO, ALTO];
            int cont = 0;
            Console.CursorVisible = false;

            Genera(tab);
            Dibuja(tab, cont);
            while (!FinalPartida(tab))
            {
                Color c = LeeColor();
                cont++;
                Expande(tab, c);
                Dibuja(tab, cont);
            }
        }

        static void Genera(Color[,] tab)
        {
            for (int j = 0; j < ALTO; j++)
                for (int i = 0; i < ANCHO; i++)
                    tab[j, i] = (Color)rnd.Next(0, 6);
        }

        static void Dibuja(Color[,] tab, int cont)
        {
            Console.Clear();
            for (int j = 0; j < ALTO; j++)
            {
                for (int i = 0; i < ANCHO; i++)
                {
                    ConsoleColor color = ConsoleColor.Black;
                    switch (tab[i, j])
                    {
                        case Color.Azul: color = ConsoleColor.DarkBlue; break;
                        case Color.Cian: color = ConsoleColor.DarkCyan; break;
                        case Color.Morado: color = ConsoleColor.DarkMagenta; break;
                        case Color.Verde: color = ConsoleColor.DarkGreen; break;
                        case Color.Rojo: color = ConsoleColor.DarkRed; break;
                        case Color.Amarillo: color = ConsoleColor.DarkYellow; break;
                    }
                    Console.BackgroundColor = color;
                    Console.Write("  ");
                }
                Console.Write('\n');
            }
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Write("\n\n");
            Panel();
            Console.ResetColor();
            Console.WriteLine("\tJugada: " + cont);
        }

        private static void Panel() // esto es una guarrada y seguramente optimizable, pero tengo prisa y lo dejo así for now
        {
            Console.BackgroundColor = ConsoleColor.DarkMagenta;
            Console.Write(" 4 ");
            Console.BackgroundColor = ConsoleColor.DarkRed;
            Console.Write(" 5 ");
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.Write(" 6 ");
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write(" 1 ");
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.Write(" 2 ");
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.Write(" 3 ");
        }

        private static bool EstaVisitado(Visitados vis, Posicion pos)
        {
            int i = 0;
            while (i < vis.fin && !vis.posVis[i].Equals(pos)) i++;
            return i != vis.fin;
        }
        /*
        private static void AñadeVecinos2(ref Visitados vis, Posicion pos)
        { // esto tiene que estar FATAL
            Posicion ady = pos;

            ady.x--; // izq
            AñadeVecino(ady.x, ref vis, ady); // izq
            ady.x += 2; // der
            AñadeVecino(ady.x, ref vis, ady); // der
            ady.x--; // vuelta

            ady.y--; // arriba
            AñadeVecino(ady.y, ref vis, ady); //arriba
            ady.y += 2; // abajo
            AñadeVecino(ady.y, ref vis, ady); // abajo
            ady.y--; // vuelta, redundante
        }

        private static void AñadeVecino2(int coord, ref Visitados vis, Posicion pos)
        {
            if (!EstaVisitado(vis, pos) // no está visitado
                && vis.fin < vis.posVis.Length // cabe
                && coord > -1 // no se sale por arriba (caso 0,0)
                && coord < vis.posVis.Length) // y no se sale por abajo  
            {
                vis.posVis[vis.fin] = pos;
                vis.fin++;
            }
        }
        */
        private static void AñadeVecinos(ref Visitados vis, Posicion pos)
        {
            if (!EstaVisitado(vis, pos) // no está visitado
                && (vis.fin < vis.posVis.Length)) ; // cabe 
            {
                vis.posVis[vis.fin] = pos;
                vis.fin++;
            }
        }

        private static void Expande(Color[,] tab, Color newColor)
        {
            Visitados vis = new Visitados
            {
                posVis = new Posicion[ANCHO * ALTO],
                pend = 0,
                fin = 1
            };
            Posicion pos = new Posicion { x = 0, y = 0 };
            Posicion aux;
            Color oldColor = tab[pos.x, pos.y];

            AñadeVecinos(ref vis, pos);

            if (oldColor != newColor)
            {
                vis.posVis[vis.pend] = pos;
                while (vis.pend < vis.fin)
                {
                    aux = vis.posVis[vis.pend];

                    aux.x--;
                    CompruebaPosCol(tab, aux, oldColor, newColor, ref vis);
                    aux.x += 2;
                    CompruebaPosCol(tab, aux, oldColor, newColor, ref vis);
                    aux.x--;

                    aux.y--;
                    CompruebaPosCol(tab, aux, oldColor, newColor, ref vis);
                    aux.y += 2;
                    CompruebaPosCol(tab, aux, oldColor, newColor, ref vis);
                    aux.y--;

                    tab[aux.x, aux.y] = newColor;
                    vis.pend++;
                }
            }
        }

        private static void CompruebaPosCol(Color[,] tab, Posicion aux, Color oldColor, Color newColor, ref Visitados vis)
        {
            if (aux.x > - 1 
                && aux.x < ANCHO
                && aux.y > - 1
                && aux.y < ALTO 
                && tab[aux.x, aux.y] == oldColor)
            {
                tab[aux.x, aux.y] = newColor;
                AñadeVecinos(ref vis, aux);
            }
        }

        private static bool FinalPartida(Color[,] tab) // TRUE si NO ha encontrado
        {
            int i, j = 0;
            bool encontrado = false; // si ha encontrado uno que no sea igual
            while (j < ALTO && !encontrado) // mientras esté en rango y no haya encontrado uno igual
            {
                i = 0;
                while (i + 1 < ANCHO && !encontrado) // mientras esté en rango y no ha encontrado uno igual
                {
                    if (!tab[j, i].Equals(tab[j, i + 1]))
                        encontrado = true;
                    else
                        i++;
                }
                j++;
            }
            return !encontrado;
        }

        private static Color LeeColor()
        {
            int c = Console.ReadKey(true).KeyChar - '0' - 1;
            Color color;
            switch (c)
            {
                case 0: color = Color.Azul; break;
                case 1: color = Color.Cian; break;
                case 2: color = Color.Verde; break;
                case 3: color = Color.Morado; break;
                case 4: color = Color.Rojo; break;
                case 5: color = Color.Amarillo; break;
                default: color = Color.Ninguno; break;
            }
            return color;
        }
    }
}
