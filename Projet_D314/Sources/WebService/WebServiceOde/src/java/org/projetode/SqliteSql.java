/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package org.projetode;

import java.security.MessageDigest;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.sql.Statement;
import java.util.ArrayList;
import java.util.List;
import org.apache.log4j.Logger;
import static org.projetode.DimensionUtils.logger;

/**
 *
 * @author olivier.essner
 */
public class SqliteSql {

    // ////////////
    // Logger LOG4J
    // ////////////
    final static Logger logger = Logger.getLogger(SqliteSql.class);
    
    
    // //////////////////
    // Connexion à SQLite
    // //////////////////
    static {
        try{
            Class.forName("org.sqlite.JDBC");
            logger.debug("SqliteSql.SQLiteDriverLoad.success");
        } catch(ClassNotFoundException ex){
            logger.error("SqliteSql.SQLiteDriverLoad.exception", ex); 
        }
    }

    
    // ///////////////////////////////////////////////
    // Calcul du hash SHA256 d'une liste de dimensions
    // ///////////////////////////////////////////////
    private String generateSHA256(List<Dimension> listCuboides)
    {
        String stmtToHash;
        
        // Formation de la chaine en concatenant tous les items de la liste en entrée
        stmtToHash = "";
        for(Dimension d : listCuboides)
        {
            // Format : (dimensionName+ dimensionCount + dimensionMemory + dimensionOrder) pour chaque item
            stmtToHash = stmtToHash + d.GetDimensionName() + d.GetDimensionCount() + d.GetDimensionMemory() + d.GetDimensionOrder();
        }
        logger.debug("SqliteSql.generateSHA256.stmtToHash : " + stmtToHash);
        
        try{
            // Hash depuis la chaine formée
            MessageDigest messageDigest = MessageDigest.getInstance("SHA-256");
            messageDigest.update(stmtToHash.getBytes());
            String encryptedString = new String(messageDigest.digest());
            logger.debug("SqliteSql.generateSHA256.encryptedString : " + encryptedString);
            return encryptedString;
            
        } catch(Exception ex){
            logger.error("SqliteSql.generateSHA256.exception", ex);  
            throw new RuntimeException(ex);
        }
    }
   
        
    
    // ///////////////////////////////////
    // Enregistrement en cache des calculs
    // ///////////////////////////////////
    public void CacheWrite(DimensionUtils.Algorithm typeAlgorithm, List<Dimension> listCuboides, double seuil_poids, int nb_boucle, List<Integer> dimensionToMaterialize)
    {
        String hashCodeList;
        String stmt;
        
 
        // Calcul du hash de la listCuboides
        hashCodeList = generateSHA256(listCuboides);
        
        // Enregistrement en base SQLite
        try{
            String path = this.getClass().getResource("/CacheWebServiceOde.db").getPath();
            logger.debug("SqliteSql.CacheWrite.path : " + path);
            Connection connection = DriverManager.getConnection("jdbc:sqlite:" + path);
            Statement stmtOut = connection.createStatement();
            stmt = "INSERT INTO CACHE_WEBSERVICE_ODE(methode, seuil_poids, nb_boucle, hashCode, solution) "
                                                        + "VALUES ('"+ typeAlgorithm +"',"+ (int)(seuil_poids) +","+ nb_boucle +",'"+ hashCodeList +"','"+ dimensionToMaterialize +"');";
            logger.debug("SqliteSql.CacheWrite.stmt : " + stmt);
            ResultSet executeQuery = stmtOut.executeQuery(stmt);
            connection.close();
            logger.debug("SqliteSql.CacheWrite.success");
        }
        catch(SQLException ex)
        {
            logger.error("SqliteSql.CacheWrite.exception", ex);  
        }
        
        return;
    }
    
    // ////////////////////////////
    // Lecture en cache des calculs
    // ////////////////////////////
    public boolean CacheRead(DimensionUtils.Algorithm typeAlgorithm, List<Dimension> listCuboides, double seuil_poids, int nb_boucle, List<Integer> dimensionToMaterialize)
    {
        String hashCodeList;
        String stmt;
        boolean flagResult = false;
        
        /*
        
        // http://www.tutorialspoint.com/sqlite/sqlite_java.htm
        
        // Calcul du hash de la listCuboides
        hashCodeList = generateSHA256(listCuboides);
        
        // Lecture en base SQLite
        try{
            String path = this.getClass().getResource("/CacheWebServiceOde.db").getPath();
            Connection connection = DriverManager.getConnection("jdbc:sqlite:" + path);
            
            Statement stmtOut = connection.createStatement();
	    stmt = "SELECT solution FROM CACHE_WEBSERVICE_ODE "
			+ "WHERE CACHE_WEBSERVICE_ODE.methode = '"+ typeAlgorithm +"' "
			+ "AND CACHE_WEBSERVICE_ODE.seuil_poids = "+ (int)(seuil_poids) +" "
			+ "AND CACHE_WEBSERVICE_ODE.nb_boucle = "+ nb_boucle +" "
			+ "AND CACHE_WEBSERVICE_ODE.hashCode = '"+ hashCodeList +"';";
            logger.debug("CacheRead.stmt : " + stmt);
            ResultSet executeQuery = stmtOut.executeQuery(stmt);
            List resultat = new ArrayList();
            while(executeQuery.next()){
		flagResult = true;
				
				
                String titre = executeQuery.getString("Titre");
                String description = executeQuery.getString("Description");
                String nom = executeQuery.getString("Nom");
                Recette r = new Recette(titre, description);
                resultat.add(r.GetTitre());
                 
            }
            connection.close();
            logger.debug("SqliteSql.CacheRead.success");
        }
        catch(SQLException ex)
        {
            logger.error("SqliteSql.CacheRead.exception", ex);  
        }   
        */
        return flagResult;
    }
    
}
