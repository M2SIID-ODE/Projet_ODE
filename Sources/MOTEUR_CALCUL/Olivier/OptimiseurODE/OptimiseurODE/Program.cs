/* ============================================================================
  
  Fichier:     OptimiseurODE.cs

  Résumé:  Code source de l'optimiseur de calcul d'agregats
  Date:     26/07/2015
  Updated:  -

  C# sous VS2015 Community Edition - Framework .NET 4.5.2
  
------------------------------------------------------------------------------
  
  Doc MSDN : https://technet.microsoft.com/fr-fr/library/ms123477(v=sql.120).aspx
  Exemple : http://www.yaldex.com/sql_server/progsqlsvr-CHP-20-SECT-6.html

  !! Réferencer dans le projet la lib AdomdClient dans "C:\Program Files\Microsoft.NET\ADOMD.NET\120"
  cf. https://msdn.microsoft.com/fr-fr/library/7314433t(v=VS.90).aspx

  !!  .NET Framework doesn’t install the ADOM.NET library by default. 
  =>   If you reference ADOMD.NET you need to also include the redistributable package for it with your application.
  
============================================================================ */


using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.AdomdClient; // Lib de client XMLA pour SSAS

namespace OptimiseurODE
{







    class Program
    {
        static void Main(string[] args)
        {
            // Chaine de connexion SSAS
            AdomdConnection conn = new AdomdConnection("Data Source=localhost;Catalog= cubeODE ");  // CATALOG : Nom du cube

            // Ouverture de la connexion SSAS
            conn.Open();


            /*
                // -----------
                // Requete MDX
                // -----------

            // Olivier # 26/07/2015 - Attention : Requete MDX fausse pour l'instant !

                string commandText = "SELECT {[FACT_VENTES].[MONTANT_HT_VENTE], " +
                    "[FACT_VENTES].[MARGE_BRUTE]} ON COLUMNS " +
                    "[FACT_VENTES].[MARGE_BRUTE]} ON COLUMNS, " +
                    "{[DIM_PRODUITS].[Univers - Rayon]} ON ROWS " +
                    "FROM [Data Warehouse ODE]" +  // Object ID du cube
                    "WHERE ([DIM_TEMPS].[ANNEE].[20150101])";

                // Envoi à SSAS
                AdomdCommand cmd = new AdomdCommand(commandText, conn);

                // Si requete MDX fausse : Crashe ici. Cliquer sur "VIEW DETAILS" de la pop-up de debuggage pour voir le retour de SSAS... A ameliorer avec un TRY..CATCH !
                CellSet cs = cmd.ExecuteCellSet();

      
                // Iterations sur toutes les lignes et les colonnes
                foreach (Position pRow in cs.Axes[1].Positions)
                {
                    foreach (Position pCol in cs.Axes[0].Positions)
                    {
                        // Recuperation des valeurs formatées pour cette cellule
                        Console.Write(cs[pCol.Ordinal, pRow.Ordinal].FormattedValue + ", ");
                    }
                    Console.WriteLine();
                }
            */

            // ------------------------------------
            // Recuperation de la structure du cube
            // ------------------------------------

            DataSet ds = conn.GetSchemaDataSet(AdomdSchemaGuid.Dimensions, null);

            DataTable dt = ds.Tables[0];
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                    Console.WriteLine(col.ColumnName + " = " + row[col].ToString());

                Console.WriteLine();
            }

            // Cloture de la connexion SSAS
            conn.Close();


            Console.WriteLine(Environment.NewLine + "Press any key to continue.");
            Console.ReadKey()
        }
    }
}
