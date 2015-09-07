
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.ComponentModel;




namespace TestingAlgorithm
{

    /// <summary>
    /// Classe des dimensions 1D : DIM_TEMPS, DIM_CLIENTS, DIM_LIEUX et DIM_PRODUITS
    /// </summary>
    /// 
    class Dimension
    {
        // Membres
        private string dimensionName; // Nom "officiel" de la dimension - potentiellement plusieurs si "dimensionOrder" >= 2
        private long dimensionCount; // Nombre de lignes de la dimension
        private int dimensionMemory; // Taille d'une ligne, en octets
        private int dimensionOrder; // Indique la dimension du cuboide. Ex : 1 - 1D / 2 : 2D...

        // Constructeur avec dimensionOrder comme argument
        public Dimension(int inDimensionOrder)
        {
            SetDimensionName("");
            SetDimensionCount(1);
            SetDimensionMemory(0);
            dimensionOrder = inDimensionOrder;
        }

        // Constructeur avec (name, count, dimensionOrder) comme arguments
        public Dimension(string inDimensionName, long inDimensionCount, int inDimensionOrder)
        {
            SetDimensionName(inDimensionName);
            SetDimensionCount(inDimensionCount);
            SetDimensionMemory(0);
            dimensionOrder = inDimensionOrder;
        }

        /// Olivier # 07/09/2015
        // Constructeur avec (name, count, dimensionOrder, dimensionMemory) comme arguments
        public Dimension(string inDimensionName, long inDimensionCount, int inDimensionOrder, int inDimensionMemory)
        {
            SetDimensionName(inDimensionName);
            SetDimensionCount(inDimensionCount);
            SetDimensionMemory(inDimensionMemory);
            dimensionOrder = inDimensionOrder;
        }
        /// Fin Olivier # 07/09/2015

        // Methodes
        public void SetDimensionName(string inDimensionName) { dimensionName = String.Copy(inDimensionName); }
        public void SetDimensionCount(long inDimensionCount) { dimensionCount = inDimensionCount; }
        public void SetDimensionMemory(int inDimensionMemory) { dimensionMemory = inDimensionMemory; }
        // REM : Pas d'accesseur Set pour "dimensionOrder" car parametré à l'instanciation de classe

        public string GetDimensionName() { return (dimensionName); }
        public long GetDimensionCount() { return (dimensionCount); }
        public int GetDimensionMemory() { return (dimensionMemory); }
        public int GetDimensionOrder() { return (dimensionOrder); }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    class Program
    {
        static void Main(string[] args)
        {
            // Instance de classe de dimensions 1D
            List<Dimension> listDim1D = new List<Dimension>();
            // string stmt;
            long Glb_Size;


            // Manual setting
            /*
            Console.WriteLine("Poids Max (Mo) ?");
            stmt = Console.ReadLine();
            Glb_Size = 1024 * 1024 * Int32.Parse(stmt);
            */
            Glb_Size = 100L * 1024L * 1024L * 1024L * 1024L;  // TEMP : 100 To


            //Etape : Liste de toutes les dimensions 1D
            GetDimension1DProperties(listDim1D);

            //Etape : Récupération du treillis des cuboïdes
            List<Dimension> listCuboides = new List<Dimension>(); // liste des cuboïdes
            List<String> index_cuboides = new List<String>(); // liste des index des cuboïdes : plus facile à utiliser par la suite
            String prefix_index = "";
            GetCombinaisons(listDim1D, 0, "", 0, listCuboides, index_cuboides, prefix_index); // appel de la fonction qui crée les combinaisons

            //Etape : Récupération du nombre de lignes des cuboïdes
            GetPoidsCuboides(index_cuboides, listCuboides, listDim1D, listDim1D.Count); // appel de la fonction qui récupère le nombre de lignes des cuboides

            //Etape : Algorithme de Metropolis ou de Matérialisation
            int[] solution = new int[listCuboides.Count]; // la solution que va retrouner Metropolis sous forme de 0 et de 1
            MaterialisationPartielle(listCuboides, Glb_Size, solution); // appel de l'algo de Matérialisation

            // Stop utilisateur
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();

        }


        // Exploration DMV de SSAS (Vues prédefinies sur tables systeme)
        static void GetDimension1DProperties(List<Dimension> listDim1D)
        {
            // string stmt;
            string dimName;
            int nbrDim, dimMemory;
            long dimSize;


            // Manual setting
            /*
            Console.WriteLine("Nbr dimensions ?");
            stmt = Console.ReadLine();
            nbrDim = Int32.Parse(stmt);
            */
            nbrDim = 4;  // TEMP

            for (int i = 0; i < nbrDim; i++)
            {
                // Manual setting
                /*
                Console.WriteLine("Nom du cuboide 1D <{0}> ?", i);
                dimName = Console.ReadLine();
                */
                switch (i) {
                    case (0): { dimName = "DIM_TEMPS"; break; } // TEMP
                    case (1): { dimName = "DIM_LIEUX"; break; } // TEMP
                    case (2): { dimName = "DIM_CLIENTS"; break; } // TEMP
                    case (3): { dimName = "DIM_PRODUITS"; break; } // TEMP
                    default:  { dimName = ""; break; } // TEMP
                };

                // Manual setting
                /*
                Console.WriteLine("Taille du cuboide 1D <{0}> ?", dimName);
                stmt = Console.ReadLine();
                dimSize = Int32.Parse(stmt);
                */
                switch (i)
                {
                    case (0): { dimSize = 3000L; break; } // TEMP
                    case (1): { dimSize = 30000L; break; } // TEMP
                    case (2): { dimSize = 100000L; break; } // TEMP
                    case (3): { dimSize = 1000000L; break; } // TEMP
                    default:  { dimSize = 0L; break; } // TEMP
                };


                // Manual setting
                /*
                Console.WriteLine("Mémoire d une ligne du cuboide 1D <{0}> (Octets) ?", dimMemory);
                stmt = Console.ReadLine();
                dimMemory = Int32.Parse(stmt);
                */
                switch (i)
                {
                    case (0): { dimMemory = 1024; break; } // TEMP
                    case (1): { dimMemory = 1024; break; } // TEMP
                    case (2): { dimMemory = 1024; break; } // TEMP
                    case (3): { dimMemory = 1024; break; } // TEMP
                    default:  { dimMemory = 0; break; } // TEMP
                };

                // Enregistrement dans la listDim1D
                listDim1D.Add(new Dimension(dimName, dimSize, 1, dimMemory));
            }
        }


        /* ************************** */
        /* TO DO : passage en WS JAVA */
        /* ************************** */

        // Algorithme des combinaisons (fonction récursive qui retroune l'ensemble des combinaisons possibles de notre liste listDim1D
        static void GetCombinaisons(List<Dimension> listDim1D, int profCourante, String prefix, int rang, List<Dimension> listCuboides, List<String> index_cuboides, String prefix_index)
        {
            for (int i = rang; i < listDim1D.Count; i++)
            {
                listCuboides.Add(new Dimension(prefix + listDim1D[i].GetDimensionName(), 0, profCourante + 1));
                index_cuboides.Add(prefix_index + i.ToString());
            }

            for (int i = rang; i < listDim1D.Count; i++)
            {
                GetCombinaisons(listDim1D, profCourante + 1, prefix
                      + listDim1D[i].GetDimensionName() + " * ", i + 1, listCuboides, index_cuboides, prefix_index + i.ToString());
            }
        }



        // Algorithme qui récupère le nombre de lignes des cuboides
        static void GetPoidsCuboides(List<String> cuboides, List<Dimension> listCuboides, List<Dimension> listDim1D, int nb_dim_1d)
        {
            // Membres
            // string stmt;
            String[] test_dimensions = new String[nb_dim_1d * 2];
            int poids_une_ligne = 0;
            long poids;


            for (int i = 0; i < cuboides.Count; i++)
            {
                poids = 0;
                // on lance une requête MDX selon le nombre de dimensions à croiser /!\ : 4 maximum pour le moment /!\
                if (cuboides[i].Length == 1)
                {
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i])].GetDimensionMemory();
                }
                if (cuboides[i].Length == 2)
                {
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory();
                }
                if (cuboides[i].Length == 3)
                {
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(2, 1))].GetDimensionMemory();
                }
                if (cuboides[i].Length == 4)
                {
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(2, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(3, 1))].GetDimensionMemory();
                }

                Console.WriteLine("Nbr lignes du cuboide {0}", listCuboides[i].GetDimensionName());
                // Manual setting
                /*
                Console.WriteLine("Nbr lignes du cuboide {0} ?", listCuboides[i].GetDimensionName());
                stmt = Console.ReadLine();
                poids = Int32.Parse(stmt);
                */
                switch (i)
                {
                    // /*
                    case (0):  { poids = 3000L; break; }     // DIM_TEMPS = 3 000
                    case (1):  { poids = 30000L; break; }    // DIM_LIEUX = 30 000
                    case (2):  { poids = 100000L; break; }   // DIM_CLIENTS = 100 000
                    case (3):  { poids = 1000000L; break; }  // DIM_PRODUITS = 1 000 000
                    case (4):  { poids = 1500L * 5000L; break; }  // DIM_TEMPS * DIM_LIEUX = 1 500 * 5 000
                    case (5):  { poids = 1500L * 5000L; break; }  // DIM_TEMPS * DIM_CLIENTS =  1 500 * 5 000
                    case (6):  { poids = 1500L * 5000L; break; }  // DIM_TEMPS * DIM_PRODUITS = 1 500 * 5 000
                    case (7):  { poids = 1500L * 5000L * 5000L; break; }  // DIM_TEMPS * DIM_LIEUX * DIM_CLIENTS = 1 500 * 5 000 * 5 000
                    case (8):  { poids = 1500L * 1500L * 5000L; break; }  // DIM_TEMPS * DIM_LIEUX * DIM_PRODUITS = 1 500 * 1 500 * 5 000
                    case (9):  { poids = 5000000L; break; }  // DIM_TEMPS * DIM_LIEUX * DIM_CLIENTS * DIM_PRODUITS = 5 000 000 (Taile de la table de faits VENTES)
                    case (10): { poids = 1500L * 5000L * 5000L; break; }  // DIM_TEMPS * DIM_CLIENTS * DIM_PRODUITS = 1 500 * 5 000 * 5 000
                    case (11): { poids = 1500L * 5000L; break; }  // DIM_LIEUX * DIM_CLIENTS = 1 500 * 5 000
                    case (12): { poids = 5000L * 5000L; break; }  // DIM_LIEUX * DIM_PRODUITS = 5 000 * 5 000 
                    case (13): { poids = 5000L * 500L * 5000L; break; }  // DIM_LIEUX * DIM_CLIENTS * DIM_PRODUITS = 5 000 * 500 * 5 000
                    case (14): { poids = 5000L * 5000L; break; }  // DIM_CLIENTS * DIM_PRODUITS = 5 000 * 5 000
                    default:   { poids = 0; break; } // TEMP
                    // */
                    /*
                    case (0):  { poids = 300L; break; }     // DIM_TEMPS = 3 000
                    case (1):  { poids = 3000L; break; }    // DIM_LIEUX = 30 000
                    case (2):  { poids = 10000L; break; }   // DIM_CLIENTS = 100 000
                    case (3):  { poids = 100000L; break; }  // DIM_PRODUITS = 1 000 000
                    case (4):  { poids = 150L * 500L; break; }  // DIM_TEMPS * DIM_LIEUX = 1 500 * 5 000
                    case (5):  { poids = 150L * 500L; break; }  // DIM_TEMPS * DIM_CLIENTS =  1 500 * 5 000
                    case (6):  { poids = 150L * 500L; break; }  // DIM_TEMPS * DIM_PRODUITS = 1 500 * 5 000
                    case (7):  { poids = 150L * 500L * 500L; break; }  // DIM_TEMPS * DIM_LIEUX * DIM_CLIENTS = 1 500 * 5 000 * 5 000
                    case (8):  { poids = 150L * 150L * 500L; break; }  // DIM_TEMPS * DIM_LIEUX * DIM_PRODUITS = 1 500 * 1 500 * 5 000
                    case (9):  { poids = 500000L; break; }  // DIM_TEMPS * DIM_LIEUX * DIM_CLIENTS * DIM_PRODUITS = 5 000 000 (Taile de la table de faits VENTES)
                    case (10): { poids = 150L * 500L * 500L; break; }  // DIM_TEMPS * DIM_CLIENTS * DIM_PRODUITS = 1 500 * 5 000 * 5 000
                    case (11): { poids = 150L * 500L; break; }  // DIM_LIEUX * DIM_CLIENTS = 1 500 * 5 000
                    case (12): { poids = 500L * 500L; break; }  // DIM_LIEUX * DIM_PRODUITS = 5 000 * 5 000 
                    case (13): { poids = 500L * 500L * 500L; break; }  // DIM_LIEUX * DIM_CLIENTS * DIM_PRODUITS = 5 000 * 500 * 5 000
                    case (14): { poids = 500L * 500L; break; }  // DIM_CLIENTS * DIM_PRODUITS = 5 000 * 5 000
                    default:   { poids = 0; break; } // TEMP
                    */
                };



                listCuboides[i].SetDimensionCount(poids); // on affecte le nombre de lignes au cuboide
                listCuboides[i].SetDimensionMemory(poids_une_ligne);
            }
        }



        // -- Olivier # 07/09/2015

        /// <summary>
        /// Algorithme de matérailisation partielle - cf cours D111 de Sofian MAABOUT
        /// Solution retournée : Tab [INT] de 0 / 1
        /// </summary>
        /// <param name="listCuboides"></param>
        /// <param name="seuil_poids"></param>
        /// <param name="nb_boucle"></param>
        /// <param name="sol_act"></param>


        /* ************************** */
        /* TO DO : passage en WS JAVA */
        /* ************************** */

        static void MaterialisationPartielle(List<Dimension> listCuboides, double seuil_poids, int[] selectedViews)
        {
            long totalSize;
            long viewSize;
            long benefit;
            long maxBenefit;
            int childrenView;
            int bestView;
            string stmt;
            int maxDim;
            int selection;


            // RAZ de la liste des vues selectionnees
            foreach (int i in selectedViews)
            {
                selectedViews[i] = 0;
            }


            Console.WriteLine("ALGORITHME DE MATERIALISATION PARTIELLE : ");

            // On commence par selectioner la vue de dimension la plus élevée (le "non-sommé")
            maxDim = 0;
            selection = 0;
            for(int i=0; i<listCuboides.Count; i++)
            {
                if (listCuboides[i].GetDimensionOrder() > maxDim) { maxDim = listCuboides[i].GetDimensionOrder(); selection = i; }
            }
            selectedViews[selection] = 1;
            // Console.WriteLine("Top {0} = {1}", selection, maxDim);  // TEMP


            // Tant qu'il reste de la mémoire à occuper
            totalSize = listCuboides[selection].GetDimensionCount() * listCuboides[selection].GetDimensionMemory();
            Console.WriteLine("Initial size : {0} Go", totalSize/(1024 * 1024 * 1024)); // TEMP
            Console.WriteLine("Seuil size : {0} Go", seuil_poids / (1024 * 1024 * 1024)); // TEMP

            while (totalSize < seuil_poids)
            {
                // Console.WriteLine("Memoire {0} sur {1}", totalSize, seuil_poids);  // TEMP

                // RAZ du max du bénéfice et de l'index de la meilleure vue
                maxBenefit = 0;
                bestView = -1;

                // Sur toutes les vues i encore disponibles
                for (int i=0; i<selectedViews.Count(); i++)
                {
                    // Stop utilisateur                              
                    // Console.WriteLine("Press any key to continue"); Console.ReadKey(); // TEMP

                    // Vue déjà traitée : On passe
                    if (selectedViews[i] == 1) { continue; }

                    // RAZ du bénéfice en cours
                    benefit = 0L;

                    // On crée la liste des composantes 1D de la vue j à partir de ses noms
                    stmt = string.Copy(listCuboides[i].GetDimensionName());
                    stmt.Trim();  // Supression de tous les espaces
                    string[] listDim1Di = (stmt.Split(new Char[] { '*' }));  // Découpage par le séparateur '*'
                    for (int k=0; k<listDim1Di.Count(); k++) { listDim1Di[k] = listDim1Di[k].Trim(); }

                    // foreach (string s in listDim1Di) { Console.WriteLine("{0} >" + s + "< {1}",i, listCuboides[i].GetDimensionOrder()); }  // TEMP

                    // Sur toutes les vues j encore disponibles et uniquements les enfants de i : On va sommer leurs bénéfices
                    for (int j=0; j < selectedViews.Count(); j++)
                    {
                        // Vue déjà traitée ou celle en cours : On passe
                        if (selectedViews[j] == 1 || i == j) { continue; }

                        // La vue j est-elle enfant de la vue i ?

                        // Test 1 : Les enfants ont TOUJOURS un rang plus élévé que leurs parents
                        if (listCuboides[j].GetDimensionOrder() < listCuboides[i].GetDimensionOrder()) { continue; }   // Non car hierarchiquement superieure ou de même niveau

                        // Console.WriteLine("i : {0} // dimOrder : {1} # j : {2} // dimOrder : {3}", i, listCuboides[i].GetDimensionOrder(), j, listCuboides[j].GetDimensionOrder()); // TEMP

                        
                        // Test 2 : Les parents contienent toujours TOUTES les dimensions 1D de leurs enfants
                        foreach (string item in listDim1Di)
                        {
                            if (item == null) { continue; } // Securité
                            if (listCuboides[i].GetDimensionName().IndexOf(item) == -1) { continue; } // Si au moins un élement est introuvable => Pas parent => On passe
                        }

                        // Si les tests 1 & 2 sont OK : j est bien enfant de i
                        childrenView = j;

                        // Calcul du gain : Count de la vue-parent (i) - Count de la vue-enfant (j)
                        benefit += (listCuboides[i].GetDimensionCount() - listCuboides[childrenView].GetDimensionCount());
                    } // FIN du for int j

                    // Actualisation de la meilleure vue et du meilleur bénéfice SSI taille acceptable !
                    viewSize = listCuboides[i].GetDimensionMemory() * listCuboides[i].GetDimensionCount();
                    Console.WriteLine("View[{0}] : {1}", i, viewSize / (1024 * 1024 * 1024)); // TEMP
                    if (benefit > maxBenefit && viewSize + totalSize < seuil_poids)
                    {
                        bestView = i;  // Désigne la vue i ayant le meilleur bénéfice "maxBenefit"
                        maxBenefit = benefit;
                    }
                } // FIN du for int i


                // Enregistrement de la best view comme selectionnée
                if (bestView == -1){ break; } // Rien de convaincant... WHILE fini
                else {
                    // Validation de la vue comme selectionnée
                    selectedViews[bestView] = 1;

                    // Ajout du poids de la nouvelle vue
                    Console.WriteLine("bestView:{0} / size:{1} / line:{2}", bestView, listCuboides[bestView].GetDimensionMemory(), listCuboides[bestView].GetDimensionCount());  // TEMP
                    totalSize += listCuboides[bestView].GetDimensionMemory() * listCuboides[bestView].GetDimensionCount();
                    Console.WriteLine("Total size apres selection de {0} : {1} Go", bestView, totalSize / (1024 * 1024 * 1024)); // TEMP
                }
                

            } // FIN du while totalSize < seuil_poids


            // Affichage de la solution retournée
            Console.Write("Solution trouvée : ");
            foreach (int item in selectedViews)
            {
                System.Console.Write(item + " ");
            }
            Console.WriteLine();
        }

        // -- Fin Olivier # 07/09/2015
    }
}


            