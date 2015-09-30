using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoMetropolis
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] sol_act = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int[] poids_cubes = { 10, 25, 5, 35, 63, 29, 2, 77, 41, 53, 86, 20, 54, 32, 8 };
            int seuil_poids = 300;
            int poids_act = 0;
            int poids_next = 0;
            int i = 0;
            int choix_cube = 0;
            Random rand = new Random();
            double ratio = 0;

            // Listage cuboïdes
            String[] treillis = { "A", "B", "C", "D", "", "", "", "", "", "", "", "", "", "", "" };
            for (int d = 0; d <= 3; d++)
            {
                for (int e = 1; e <= 3; e++)
                {
                    treillis[3 + d + e] = treillis[d] + treillis[e+d];
                }
            }

            foreach (String element in treillis)
            {
                System.Console.Write(element + " ");
            }
            // fin liste


            while (i < 1)
            {
                Console.Write("Sol act : ");
                foreach (int element in sol_act)
                {
                    System.Console.Write(element+" ");
                }
                
                Console.WriteLine(" - Compteur : " + i);

                choix_cube = rand.Next(0,15);
                Console.WriteLine("Cube choisi : "+ choix_cube);
                if (sol_act[choix_cube] == 0)
                    poids_next = poids_act + poids_cubes[choix_cube];
                else
                    poids_next = poids_act - poids_cubes[choix_cube];

                if (poids_next <= seuil_poids)
                {
                    if (poids_next >= poids_act)
                    {
                        sol_act[choix_cube] = 1;
                        poids_act = poids_next;
                    }
                    else
                    {

                        ratio = Convert.ToDouble(poids_next) / Convert.ToDouble(poids_act) ;

                        Console.WriteLine("Ratio : " + poids_next + " " + poids_act + " " + ratio);

                        if (rand.NextDouble() < ratio)
                        {
                            Console.WriteLine("Tirage Bernoulli = Succes : " + ratio);
                            sol_act[choix_cube] = 0;
                            poids_act = poids_next;
                        }
                    }
                }
                i = i + 1;
            }
            Console.Read();
        }
    }
}
