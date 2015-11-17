/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package org.projetode;

import java.sql.DriverManager;
import java.sql.*;
import java.lang.Math;
import java.util.*;
import java.security.MessageDigest;


/**
 *
 * @author olivier.essner
 */
    public class DimensionUtils {
        
        
    private enum Algorithm {
        METROPOLIS,
        MATERIALISATION_PARTIELLE
    }
    
    // //////////////////
    // Connexion à SQLite
    // //////////////////
    static {
        try{
            Class.forName("org.sqlite.JDBC");
        } catch(ClassNotFoundException e){
            System.out.println("Erreur de chargement du driver SQLite");
        }
    }
    
    // ///////////////////////////////////////////////
    // Calcul du hash SHA256 d'une liste de dimensions
    // ///////////////////////////////////////////////
    public static String generateSHA256(List<Dimension> listCuboides)
    {
        String stmtToHash;
        
        // Formation de la chaine en concatenant tous les items de la liste en entrée
        stmtToHash = "";
        for(Dimension d : listCuboides)
        {
            stmtToHash = stmtToHash + d.toString();
        }
        
        try{
            // Hash depuis la chaine formée
            MessageDigest messageDigest = MessageDigest.getInstance("SHA-256");
            messageDigest.update(stmtToHash.getBytes());
            String encryptedString = new String(messageDigest.digest());
            
            return encryptedString;
            
        } catch(Exception ex){
            throw new RuntimeException(ex);
        }
    }
        
    
    // ///////////////////////////////////
    // Enregistrement en cache des calculs
    // ///////////////////////////////////
    private void CacheWrite(Algorithm typeAlgorithm, List<Dimension> listCuboides, double seuil_poids, int nb_boucle, List<Integer> dimensionToMaterialize)
    {
        String hashCodeList;
        
 
        // Calcul du hash de la listCuboides
        hashCodeList = generateSHA256(listCuboides);
        
        // Enregistrement en base SQLite
        try{
            String path = this.getClass().getResource("/CacheWebServiceOde.db").getPath();
            Connection connection = DriverManager.getConnection("jdbc:sqlite:" + path);
            
            Statement stmtOut = connection.createStatement();
			
            ResultSet executeQuery = stmtOut.executeQuery("INSERT INTO CACHE_WEBSERVICE_ODE(methode, seuil_poids, nb_boucle, hashCode, solution) "
                                                        + "VALUES ('"+ typeAlgorithm +"',"+ (int)(seuil_poids) +","+ nb_boucle +",'"+ hashCodeList +"','"+ dimensionToMaterialize +"');");
        }
        catch(SQLException e)
        {
            System.err.println("TRACE : " + e.getMessage());
        }
        
        return;
    }
    
    
    // ////////////////////////////
    // Lecture en cache des calculs
    // ////////////////////////////
    private boolean CacheRead(Algorithm typeAlgorithm, List<Dimension> listCuboides, double seuil_poids, int nb_boucle, List<Integer> dimensionToMaterialize)
    {
        String hashCodeList;
        boolean flagResult = false;
        
        
        // Calcul du hash de la listCuboides
        hashCodeList = generateSHA256(listCuboides);
        
        // Lecture en base SQLite
        try{
            String path = this.getClass().getResource("/CacheWebServiceOde.db").getPath();
            Connection connection = DriverManager.getConnection("jdbc:sqlite:" + path);
            
            Statement stmtOut = connection.createStatement();
			
            ResultSet executeQuery = stmtOut.executeQuery("SELECT solution FROM CACHE_WEBSERVICE_ODE "
			+ "WHERE CACHE_WEBSERVICE_ODE.methode = '"+ typeAlgorithm +"' "
			+ "AND CACHE_WEBSERVICE_ODE.seuil_poids = "+ (int)(seuil_poids) +" "
			+ "AND CACHE_WEBSERVICE_ODE.nb_boucle = "+ nb_boucle +" "
			+ "AND CACHE_WEBSERVICE_ODE.hashCode = '"+ hashCodeList +"';");
			
            List resultat = new ArrayList();
            
            while(executeQuery.next()){
		flagResult = true;
				
		/*		
                String titre = executeQuery.getString("Titre");
                String description = executeQuery.getString("Description");
                String nom = executeQuery.getString("Nom");
                Recette r = new Recette(titre, description);
                resultat.add(r.GetTitre());
                 */
            }
        }
        catch(SQLException e)
        {
            System.err.println("TRACE : " + e.getMessage());
        }   
        
        return flagResult;
    }
    
    
    // ///////////////////////////////////////
    // Initialisation du tableau des resultats
    // ///////////////////////////////////////
    private void initDimensionToMaterialize(List<Dimension> listCuboides, List<Integer> dimensionToMaterialize)
    {
        int count;
        
        // Compter le nombre d'items
        count = listCuboides.size();
        
        // Créer autant d'items dans la liste
        for(int i=0; i<count; i++)
        {
            dimensionToMaterialize.add(0);
        }
        
        return;
    }
    
    
    // ////////////////////////////////////////////////////////////////////////////////////////////
    // WEB METHODE 1 : Point d'entrée de l'algorithme qui récupère le nombre de lignes des cuboides
    // ////////////////////////////////////////////////////////////////////////////////////////////
    public List<Dimension> GetCombinaisons(List<Dimension> listDim1D)
    {
        List<Dimension> DimensionResultList = new ArrayList<Dimension>();

        List<String> index_cuboides = new ArrayList<String>(); // liste des index des cuboïdes : plus facile à utiliser par la suite
        String prefix_index = "";
        int profCourante = 0;
        String prefix = "";
        int rang = 0;
        
        // Appel initial de la fonction recursive
        FunctionGetCombinaisons(listDim1D, profCourante, prefix, rang, DimensionResultList, index_cuboides, prefix_index);
  
        return DimensionResultList;
    }
    
    
    // ////////////////////////////////////////////////////////////////////////////////////////////
    // WEB METHODE 2 : Point d'entrée de l'algorithme qui récupère le nombre de lignes des cuboides
    // ////////////////////////////////////////////////////////////////////////////////////////////
    public List<String> GetCombinaisonsIndex(List<Dimension> listDim1D)
    {
        List<Dimension> DimensionResultList = new ArrayList<Dimension>();

        List<String> index_cuboides = new ArrayList<String>(); // liste des index des cuboïdes : plus facile à utiliser par la suite
        String prefix_index = "";
        int profCourante = 0;
        String prefix = "";
        int rang = 0;
        
        // Appel initial de la fonction recursive
        FunctionGetCombinaisons(listDim1D, profCourante, prefix, rang, DimensionResultList, index_cuboides, prefix_index);
  
        return index_cuboides;
    }

    
    // //////////////////////////////////////////////////////////////////////////
    // Algorithme qui récupère le nombre de lignes des cuboides lié aux WM 1 et 2
    // //////////////////////////////////////////////////////////////////////////
    static void FunctionGetCombinaisons(List<Dimension> listDim1D, int profCourante, String prefix, int rang, List<Dimension> listCuboides, List<String> index_cuboides, String prefix_index)
    {
        for (int i=rang; i<listDim1D.size(); i++)
        {
            listCuboides.add(new Dimension(prefix + listDim1D.get(i).GetDimensionName(), 0, profCourante+1)); 
            index_cuboides.add(prefix_index + Integer.toString(i));
        }

        for (int i=rang; i<listDim1D.size(); i++)
        {
            FunctionGetCombinaisons(listDim1D, profCourante+1, prefix + listDim1D.get(i).GetDimensionName() + " * ", i+1, listCuboides, index_cuboides, prefix_index + Integer.toString(i));
        }
    }
    

    // ////////////////////////////////////////////////////////////
    // WEB METHODE 3 : Point d'entrée de l'algorithme de Metropolis
    // ////////////////////////////////////////////////////////////
    public List<Integer> Metropolis(List<Dimension> listCuboides, double seuil_poids, int nb_boucle)
    {
        List<Integer> dimensionToMaterialize = new ArrayList<Integer>();
        boolean isCachedValue;
        
        // Gestion du cache
        isCachedValue = CacheRead(Algorithm.METROPOLIS, listCuboides, seuil_poids, nb_boucle, dimensionToMaterialize);
        
        // Si le cache n'existe pas : Traitement puis enregistrement en cache
        if(isCachedValue == true){
            FunctionMetropolis(listCuboides, seuil_poids, nb_boucle, dimensionToMaterialize);
            CacheWrite(Algorithm.METROPOLIS, listCuboides, seuil_poids, nb_boucle, dimensionToMaterialize);
        }
      
        // Renvoi du resultat
        return dimensionToMaterialize;
    }
    

    // ////////////////////////
    // Algorithme de Metropolis
    // ////////////////////////
    static void FunctionMetropolis(List<Dimension> listCuboides, double seuil_poids, int nb_boucle, List<Integer> sol_act)
    {
        double poids_act = 0;
        double poids_next = 0;
        int i = 0;
        int choix_cube = 0;
        Random rnd = new Random();
        double ratio = 0;

        
        while(i<nb_boucle)
        {
            // on choisit un cube à permuter
            choix_cube = rnd.nextInt(listCuboides.size()); 
            
            // s'il est à 0, on ajoute son poids
            if(sol_act.get(choix_cube) == 0) 
                poids_next = poids_act + (listCuboides.get(choix_cube).GetDimensionCount() * listCuboides.get(choix_cube).GetDimensionMemory());
            // sinon on on l'enlève
            else 
                poids_next = poids_act - (listCuboides.get(choix_cube).GetDimensionCount() * listCuboides.get(choix_cube).GetDimensionMemory());

            // on continue uniquement si le poids de la future solution ne dépasse pas le seuil fixé
            if(poids_next <= seuil_poids) 
            {
                // si c'est une meilleure solution, on effecture la permutation
                if(poids_next >= poids_act) 
                {
                   sol_act.set(choix_cube, 1);
                   poids_act = poids_next;
                }
                // sinon, on effectueun tirage de Bernouilli
                else 
                {
                    ratio = poids_next / poids_act;
                    // si le tirage est inférieur au ratio (poids de la solution suivante / poids de la solution actuelle), on permute
                    if(rnd.nextDouble()< ratio) 
                    {
                        sol_act.set(choix_cube, 0);
                        poids_act = poids_next;
                    }
                }
            }
            i = i + 1;
        }
    }  

    // //////////////////////////////////////////////////////////////////////////
    // WEB METHODE 4 :Point d'entrée de l'algorithme de matérialisation partielle
    // //////////////////////////////////////////////////////////////////////////
    public List<Integer> MaterialisationPartielle(List<Dimension> listCuboides, double seuil_poids)
    {
        List<Integer> dimensionToMaterialize = new ArrayList<Integer>();
        boolean isCachedValue;
        
        // Initialisation du tableau des resultats
        initDimensionToMaterialize(listCuboides, dimensionToMaterialize);
        
        // Gestion du cache
        isCachedValue = CacheRead(Algorithm.MATERIALISATION_PARTIELLE, listCuboides, seuil_poids, 0, dimensionToMaterialize);
        
        // Si le cache n'existe pas : Traitement puis enregistrement en cache
        if(isCachedValue == true){
            FunctionMetropolis(listCuboides, seuil_poids, 0, dimensionToMaterialize);
            CacheWrite(Algorithm.MATERIALISATION_PARTIELLE, listCuboides, seuil_poids, 0, dimensionToMaterialize);
        }
  
        // Renvoi du resultat
        return dimensionToMaterialize;
    }

    // /////////////////////////////////////////////////////////////////////////
    // Algorithme de matérialisation partielle - cf cours D111 de Sofian MAABOUT
    // /////////////////////////////////////////////////////////////////////////
    static void FunctionMaterialisationPartielle(List<Dimension> listCuboides, double seuil_poids, List<Integer> selectedViews)
    {
        long totalSize;
        long viewSize;
        long benefit;
        long maxBenefit;
        int childrenView;
        int bestView;
        String stmt;
        int maxDim;
        int selection;


        // On commence par selectioner la vue de dimension la plus élevée (le "non-sommé")
        maxDim = 0;
        selection = 0;
        for(int i=0; i<listCuboides.size(); i++)
        {
            if (listCuboides.get(i).GetDimensionOrder() > maxDim) { 
                maxDim = listCuboides.get(i).GetDimensionOrder(); 
                selection = i; 
            }
        }
        selectedViews.set(selection, 1);

        // Tant qu'il reste de la mémoire à occuper
        totalSize = listCuboides.get(selection).GetDimensionCount() * listCuboides.get(selection).GetDimensionMemory();

        while (totalSize < seuil_poids)
        {
            // RAZ du max du bénéfice et de l'index de la meilleure vue
            maxBenefit = 0;
            bestView = -1;

            // Sur toutes les vues i encore disponibles
            for (int i=0; i<selectedViews.size(); i++)
            {
                // Stop utilisateur                              
                // Console.WriteLine("Press any key to continue"); Console.ReadKey(); // TEMP

                // Vue déjà traitée : On passe
                if (selectedViews.get(i)==1) { continue; }

                // RAZ du bénéfice en cours
                benefit = 0L;

                // On crée la liste des composantes 1D de la vue j à partir de ses noms
                stmt = listCuboides.get(i).GetDimensionName();
                stmt.trim();  // Supression de tous les espaces
                String[] listDim1Di = stmt.split("*");  // Découpage par le séparateur '*'
                
                for (int k=0; k<listDim1Di.length; k++) { listDim1Di[k] = listDim1Di[k].trim(); }

                // Sur toutes les vues j encore disponibles et uniquements les enfants de i : On va sommer leurs bénéfices
                for (int j=0; j < selectedViews.size(); j++)
                {
                    // Vue déjà traitée ou celle en cours : On passe
                    if (selectedViews.get(j)==1 || i==j) { continue; }

                    // La vue j est-elle enfant de la vue i ?

                    // Test 1 : Les enfants ont TOUJOURS un rang plus élévé que leurs parents
                    if (listCuboides.get(j).GetDimensionOrder() < listCuboides.get(i).GetDimensionOrder()) { continue; }   // Non car hierarchiquement superieure ou de même niveau

                    // Console.WriteLine("i : {0} // dimOrder : {1} # j : {2} // dimOrder : {3}", i, listCuboides[i].GetDimensionOrder(), j, listCuboides[j].GetDimensionOrder()); // TEMP


                    // Test 2 : Les parents contienent toujours TOUTES les dimensions 1D de leurs enfants
                    for (String item : listDim1Di)
                    {
                        if (item == null) { continue; } // Securité
                        if (listCuboides.get(i).GetDimensionName().indexOf(item) == -1) { continue; } // Si au moins un élement est introuvable => Pas parent => On passe
                    }

                    // Si les tests 1 & 2 sont OK : j est bien enfant de i
                    childrenView = j;

                    // Calcul du gain : Count de la vue-parent (i) - Count de la vue-enfant (j)
                    benefit += (listCuboides.get(i).GetDimensionCount() - listCuboides.get(childrenView).GetDimensionCount());
                } // FIN du for int j

                // Actualisation de la meilleure vue et du meilleur bénéfice SSI taille acceptable !
                viewSize = listCuboides.get(i).GetDimensionMemory() * listCuboides.get(i).GetDimensionCount();
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
                selectedViews.set(bestView, 1);

                // Ajout du poids de la nouvelle vue
                totalSize += listCuboides.get(bestView).GetDimensionMemory() * listCuboides.get(bestView).GetDimensionCount();
            }
        } // FIN du while totalSize < seuil_poids
    }
}