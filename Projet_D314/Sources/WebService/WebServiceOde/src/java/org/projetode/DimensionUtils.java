/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package org.projetode;


import java.util.*;

/**
 *
 * @author olivier.essner
 */
    public class DimensionUtils {

    
    // METHODE 1 : Algorithme qui récupère le nombre de lignes des cuboides
    // Appel dans le C# : GetCombinaisons(listDim1D, 0, "", 0, listCuboides, index_cuboides, prefix_index)
    public List<Dimension> GetCombinaisons(List<Dimension> listDim1D, List<Dimension> listCuboides)
    {
        List<Dimension> DimensionResultList = new ArrayList<Dimension>(1);
        return DimensionResultList;
    }
    
/* C# source : 
        // Algorithme qui récupère le nombre de lignes des cuboides
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
 */
     
    
    // METHODE 2 : Algorithme de Metropolis
    public List<Integer> Metropolis(List<Dimension> listCuboides, double seuil_poids, int nb_boucle)
    {
        List<Integer> DimensionToMaterialize = new ArrayList<Integer>();
        return DimensionToMaterialize;
    }
    

    
/* C# source :
        // Algorithme de Metropolis
        static void Metropolis(List<Dimension> listCuboides, double seuil_poids, int nb_boucle, int[] sol_act)
        {
            double poids_act = 0;
            double poids_next = 0;
            int i = 0;
            int choix_cube = 0;
            Random rand = new Random();
            double ratio = 0;

            while (i < nb_boucle)
            {
                choix_cube = rand.Next(0, listCuboides.Count); // on choisit un cube à permuter
                if (sol_act[choix_cube] == 0) // s'il est à 0, on ajoute son poids
                    poids_next = poids_act + (listCuboides[choix_cube].GetDimensionCount() * listCuboides[choix_cube].GetDimensionMemory());
                else // sinon on on l'enlève
                    poids_next = poids_act - (listCuboides[choix_cube].GetDimensionCount() * listCuboides[choix_cube].GetDimensionMemory());

                if (poids_next <= seuil_poids) // on continue uniquement si le poids de la future solution ne dépasse pas le seuil fixé
                {
                    if (poids_next >= poids_act) // si c'est une meilleure solution, on effecture la permutation
                    {
                       sol_act[choix_cube] = 1;
                        poids_act = poids_next;
                    }
                    else // sinon, on effectueun tirage de Bernouilli
                    {

                        ratio = Convert.ToDouble(poids_next) / Convert.ToDouble(poids_act);
                        if (rand.NextDouble() < ratio) // si le tirage est inférieur au ratio (poids de la solution suivante / poids de la solution actuelle), on permute
                        {
                            sol_act[choix_cube] = 0;
                            poids_act = poids_next;
                        }
                    }
                }
                i = i + 1;
            }
        }  
*/

    
    // METHODE 3 : Algorithme de matérailisation partielle - cf cours D111 de Sofian MAABOUT
    public List<Integer> MaterialisationPartielle(List<Dimension> listCuboides, double seuil_poids)
    {
        List<Integer> DimensionToMaterialize = new ArrayList<Integer>();
        return DimensionToMaterialize;
    }
}  
    
/* C# source :     
    // Algorithme de matérailisation partielle - cf cours D111 de Sofian MAABOUT
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
    */