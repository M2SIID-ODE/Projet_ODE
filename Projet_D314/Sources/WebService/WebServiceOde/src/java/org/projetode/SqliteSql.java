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
import java.util.regex.Pattern;
import org.apache.log4j.Logger;
import static org.projetode.DimensionUtils.logger;

/**
 *
 * @author olivier.essner
 * > Tests de la DB avec DB BROWSER : Ne pas se connecter avec celle du projet mais celle déployer sur le serveur GlassFish : \build\web\WEB-INF\classes
 * > Même en cas de re-deploy, la base existante n'est pas écrasée
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
    // Conversion d'un string en list<int>
    // ///////////////////////////////////
    public List<Integer> ConvertResultsStringToList(String dimensionToMaterializeStr)
    {
        String str;
        int tmpInteger;
        
        // Input : // [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0]
        str = dimensionToMaterializeStr.substring(1, dimensionToMaterializeStr.length()-1);
        dimensionToMaterializeStr = str.replaceAll("\\s","");
        // Maintenant : 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0
        
        String[] strArray = dimensionToMaterializeStr.split(Pattern.quote(","));
        List<Integer> dimensionToMaterializeList = new ArrayList<Integer>();
        for(String number : strArray) {
            tmpInteger = Integer.parseInt(number);
            dimensionToMaterializeList.add(tmpInteger); 
        }
        
        return  dimensionToMaterializeList;
    }
    
    
    // ///////////////////////////
    // Vidange du cache de calculs
    // ///////////////////////////
    public boolean CleanCache()
    {
        String hashCodeList;
        String stmt;
        String dimensionToMaterializeStr;
        int tmp;
        

        logger.debug("CacheClean.begin");
        
        // Enregistrement en base SQLite
        try{
            String path = this.getClass().getResource("/CacheWebServiceOde.db").getPath();
            Connection connection = DriverManager.getConnection("jdbc:sqlite:" + path);
            Statement stmtOut = connection.createStatement();
            stmt = "DELETE FROM CACHE_WEBSERVICE_ODE";
            logger.debug("SqliteSql.CacheClean.stmt : " + stmt);
            stmtOut.executeUpdate(stmt);
            stmtOut.close();
            connection.close();
            logger.debug("SqliteSql.CacheClean.success");
        }
        catch(SQLException ex)
        {
            logger.error("SqliteSql.CacheClean.exception", ex);  
            return false;
        }
        return true;
    }
    
    
    
    // ///////////////////////////////////
    // Enregistrement en cache des calculs
    // ///////////////////////////////////
    public void CacheWrite(DimensionUtils.Algorithm typeAlgorithm, List<Dimension> listCuboides, double seuil_poids, int nb_boucle, List<Integer> dimensionToMaterialize)
    {
        String hashCodeList;
        String stmt;
        String dimensionToMaterializeStr;
        int tmp;
        
 
        // Calcul du hash de la listCuboides -> SHA256 => Proba collision faible
        hashCodeList = generateSHA256(listCuboides);
        
        logger.debug("CacheWrite.begin");
        
        // Conversion de la List<Integer> en String 
        dimensionToMaterializeStr = dimensionToMaterialize.toString();
        
        // Enregistrement en base SQLite
        try{
            String path = this.getClass().getResource("/CacheWebServiceOde.db").getPath();
            Connection connection = DriverManager.getConnection("jdbc:sqlite:" + path);
            Statement stmtOut = connection.createStatement();
            stmt = "INSERT INTO CACHE_WEBSERVICE_ODE(methode, seuil_poids, nb_boucle, hashCode, solution) "
                                                        + "VALUES ('"+ typeAlgorithm +"',"+ (int)(seuil_poids) +","+ nb_boucle +",'"+ hashCodeList +"','"+ dimensionToMaterializeStr +"');";
            logger.debug("SqliteSql.CacheWrite.stmt : " + stmt);
            stmtOut.executeUpdate(stmt);
            stmtOut.close();
            connection.close();
            logger.debug("SqliteSql.CacheWrite.success");
        }
        catch(SQLException ex)
        {
            logger.error("SqliteSql.CacheWrite.exception", ex);  
            return;
        }
        return;
    }
    
    // ////////////////////////////
    // Lecture en cache des calculs
    // ////////////////////////////
    public List<Integer> CacheRead(DimensionUtils.Algorithm typeAlgorithm, List<Dimension> listCuboides, double seuil_poids, int nb_boucle)
    {
        String hashCode;
        String stmt;
        List<Integer> dimensionToMaterialize = new ArrayList<Integer>();
        String dimensionToMaterializeStr;
        
                
        // Calcul du hash de la listCuboides
        hashCode = generateSHA256(listCuboides);
        
        logger.debug("CacheRead.begin");
        
        // Lecture en base SQLite
        try{
            dimensionToMaterializeStr = "";
            String path = this.getClass().getResource("/CacheWebServiceOde.db").getPath();
            Connection connection = DriverManager.getConnection("jdbc:sqlite:" + path);
            
            Statement stmtOut = connection.createStatement();
	    stmt = "SELECT solution FROM CACHE_WEBSERVICE_ODE "
			+ "WHERE CACHE_WEBSERVICE_ODE.methode = '"+ typeAlgorithm +"' "
			+ "AND CACHE_WEBSERVICE_ODE.seuil_poids = "+ (int)(seuil_poids) +" "
			+ "AND CACHE_WEBSERVICE_ODE.nb_boucle = "+ nb_boucle +" "
			+ "AND CACHE_WEBSERVICE_ODE.hashCode = '"+ hashCode +"';";
            logger.debug("CacheRead.stmt : " + stmt);
            ResultSet executeQuery = stmtOut.executeQuery(stmt);
            
            while(executeQuery.next()){
                dimensionToMaterializeStr = executeQuery.getString("solution");
                
                logger.debug("SqliteSql.CacheRead.dimensionToMaterializeStr : "+ dimensionToMaterializeStr);

                // Conversion du String en List<Integer>
                if(dimensionToMaterializeStr != "" && dimensionToMaterializeStr != null){
                    dimensionToMaterialize = ConvertResultsStringToList(dimensionToMaterializeStr);
                }
                else{
                    dimensionToMaterialize = null;
                }
            }
            stmtOut.close();
            connection.close();
            logger.debug("SqliteSql.CacheRead.dimensionToMaterialize : "+ dimensionToMaterialize.toString());
        }
        catch(SQLException ex)
        {
            logger.error("SqliteSql.CacheRead.exception", ex);  
            return null;
        }    
        logger.debug("SqliteSql.CacheRead.success");
        return dimensionToMaterialize;
    }
}
