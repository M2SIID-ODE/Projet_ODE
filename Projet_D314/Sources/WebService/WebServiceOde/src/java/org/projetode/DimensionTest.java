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
import org.apache.log4j.Logger;



/**
 *
 * @author olivier.essner
 * > Tests de la DB avec DB BROWSER : Ne pas se connecter avec celle du projet mais celle déployer sur le serveur GlassFish : \build\web\WEB-INF\classes
 * > Même en cas de re-deploy, la base existante n'est pas écrasée
 */
public class DimensionTest {
        
        
    private enum Algorithm {
        METROPOLIS,
        MATERIALISATION_PARTIELLE
    }
    
    DimensionUtils dimTest = new DimensionUtils();
    
    // ////////////
    // Logger LOG4J
    // ////////////
    final static Logger logger = Logger.getLogger(DimensionTest.class);
    


    // ///////////////////////////////
    // Test : Peuplement du tableau 1D
    // ///////////////////////////////
    public List<Dimension> TestPopulateDim1D()
    {
        List<Dimension> listDim1D = new ArrayList<Dimension>();
        
        Dimension d;
        for(int i=0; i<5; i++){
            d = new Dimension(1);
            d.SetDimensionName("DIM_" + i);
            d.SetDimensionCount(1000);
            d.SetDimensionMemory(1024);
            listDim1D.add(d);
            d = null; // Reset
        }
        
        logger.debug("DimensionTest.TestPopulateDim1D.success");
        for(int i=0; i<listDim1D.size(); i++){
            logger.debug("DimensionTest.TestPopulateDim1D.listDim1D["+ i +"] : "
                    + listDim1D.get(i).GetDimensionName() +" # "
                    + listDim1D.get(i).GetDimensionOrder());
        }
        return listDim1D;
    }
    
    
    
    // //////////////////
    // WEB METHODE 1 TEST
    // //////////////////
    public boolean TestGetCombinaisons()
    {
        List<Dimension> listDim1D = new ArrayList<Dimension>();
        List<Dimension> DimensionResultList = new ArrayList<Dimension>();

        
        // Peuplement de listDim1D
        listDim1D = TestPopulateDim1D();

        // Appel de la fonction
        DimensionResultList = dimTest.GetCombinaisons(listDim1D);
        logger.debug("DimensionTest.TestGetCombinaisons.success");
        for(int i=0; i<DimensionResultList.size(); i++){
            logger.debug("DimensionTest.TestGetCombinaisons.DimensionResultList["+ i +"] : "
                    + DimensionResultList.get(i).GetDimensionName() +" # "
                    + DimensionResultList.get(i).GetDimensionOrder());
        }
        
        return true;
    }
    
    
    // //////////////////
    // WEB METHODE 2 TEST
    // //////////////////
    public boolean TestGetCombinaisonsIndex()
    {
        List<Dimension> listDim1D = new ArrayList<Dimension>();
        List<String> index_cuboides = new ArrayList<String>();
        
        // Peuplement de listDim1D
        listDim1D = TestPopulateDim1D();
        
        // Appel de la fonction
        index_cuboides = dimTest.GetCombinaisonsIndex(listDim1D);
        logger.debug("DimensionTest.TestGetCombinaisonsIndex.success");
        for(int i=0; i<index_cuboides.size(); i++){
            logger.debug("DimensionTest.TestGetCombinaisonsIndex.index_cuboides["+ i +"] : "+ index_cuboides.get(i));
        }
        
        return true;
    }
    
    
    // //////////////////
    // WEB METHODE 3 TEST
    // //////////////////
    public boolean TestMetropolis()
    {
        List<Integer> dimensionToMaterialize = new ArrayList<Integer>();
        List<Dimension> listCuboides = new ArrayList<Dimension>();
        List<Dimension> listDim1D = new ArrayList<Dimension>();
        double seuil_poids = 10000;
        int nb_boucle = 1000;
        
        // Peuplement de listCuboides :
        // > Etape 1 : Peuplement de listDim1D
        listDim1D = TestPopulateDim1D();
        // > Etape 2 : Fonction de combinaison
        listCuboides = dimTest.GetCombinaisons(listDim1D);
        logger.debug("DimensionTest.TestMetropolis.listCuboides success");
        for(int i=0; i<listCuboides.size(); i++){
            
            // MAJ des autres champs de Dimensions (Fait par le client C# normallement)
            listCuboides.get(i).SetDimensionCount(10);
            listCuboides.get(i).SetDimensionMemory(10);
            
            logger.debug("DimensionTest.TestMetropolis.listCuboides["+ i +"] : "
                    + listCuboides.get(i).GetDimensionName() +" # "
                    + listCuboides.get(i).GetDimensionOrder());
        }
        
        // Appel de la fonction
        dimensionToMaterialize = dimTest.Metropolis(listCuboides, seuil_poids, nb_boucle);
        logger.debug("DimensionTest.TestMetropolis.success");
        logger.debug("DimensionTest.TestMetropolis.dimensionToMaterialize : "+ dimensionToMaterialize.toString());
        
        return true;
    }
    
    
    // //////////////////
    // WEB METHODE 4 TEST
    // //////////////////
    public boolean TestMaterialisationPartielle()
    {
        List<Integer> dimensionToMaterialize = new ArrayList<Integer>();
        List<Dimension> listCuboides = new ArrayList<Dimension>();
        List<Dimension> listDim1D = new ArrayList<Dimension>();
        double seuil_poids = 10000;

        
        // Peuplement de listCuboides :
        // > Etape 1 : Peuplement de listDim1D
        listDim1D = TestPopulateDim1D();
        // > Etape 2 : Fonction de combinaison
        listCuboides = dimTest.GetCombinaisons(listDim1D);
        logger.debug("DimensionTest.TestMaterialisationPartielle.listCuboides success");
        for(int i=0; i<listCuboides.size(); i++){
            
            // MAJ des autres champs de Dimensions (Fait par le client C# normallement)
            listCuboides.get(i).SetDimensionCount(10);
            listCuboides.get(i).SetDimensionMemory(10);
            
            logger.debug("DimensionTest.TestMaterialisationPartielle.listCuboides["+ i +"] : "
                    + listCuboides.get(i).GetDimensionName() +" # "
                    + listCuboides.get(i).GetDimensionOrder());
        }
        
        // Appel de la fonction
        dimensionToMaterialize = dimTest.MaterialisationPartielle(listCuboides, seuil_poids);
        logger.debug("DimensionTest.TestMaterialisationPartielle.success");
        logger.debug("DimensionTest.TestMaterialisationPartielle.dimensionToMaterialize : "+ dimensionToMaterialize.toString());
        return true;
    }
}