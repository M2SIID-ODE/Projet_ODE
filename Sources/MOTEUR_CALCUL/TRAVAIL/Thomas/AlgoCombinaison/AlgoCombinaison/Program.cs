using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoCombinaison
{
    class Program
    {
        static void Main(string[] args)
        {
            String[] ensemble = { "0", "1", "2", "3", "4" };
            String[] cuboides = { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
            int profondeur = 40;
            displayEnsemble(ensemble, profondeur, 0, "", 0,cuboides);
    
            Console.Read();

        }

      public static void displayEnsemble(String[] ensemble, int profMax,
      int profCourante, String prefix, int rang, String[] cuboides)
        {
            if (profCourante < profMax)
            {
                for (int i = rang; i < ensemble.Length; i++)
                {
                    Console.WriteLine(prefix +ensemble[i]);
                    cuboides[profCourante] = prefix + ensemble[i];
                }

                for (int i = rang; i < ensemble.Length; i++)
                {
                    displayEnsemble(ensemble, profMax, profCourante + 1, prefix
                          +ensemble[i], i + 1,cuboides);
                }
            }
        }


    }
}
