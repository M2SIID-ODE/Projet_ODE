
/* ============================================================================
  
  Fichier:     Program.cs

  Résumé:  Code source de banc d essai des fonctions SSAS / C# / XMLA / MDX
  Date:     10/08/2015
  Updated:  -

  C# sous VS2015 Community Edition - Framework .NET 4.5.2
  
------------------------------------------------------------------------------
  
  Doc MSDN : https://technet.microsoft.com/fr-fr/library/ms123477(v=sql.120).aspx
  Exemple : http://www.yaldex.com/sql_server/progsqlsvr-CHP-20-SECT-6.html
  
  Exemple : https://bennyaustin.wordpress.com/2011/03/01/ssas-dmv-queries-cube-metadata/
  Dynamic Management Views (DMV) : https://msdn.microsoft.com/en-us/library/hh230820(v=sql.120).aspx

  !! Réferencer dans l'assembly du projet la lib "AdomdClient" dans "C:\Program Files\Microsoft.NET\ADOMD.NET\120"
  cf. https://msdn.microsoft.com/fr-fr/library/7314433t(v=VS.90).aspx

  !!  .NET Framework doesn’t install the ADOM.NET library by default. 
  =>   If you reference ADOMD.NET you need to also include the redistributable package for it with your application.

  !! Réferencer dans l'assembly du projet la lib "Xmla" dans "C:\Program Files\Microsoft.NET\ADOMD.NET\120"
  cf. https://msdn.microsoft.com/fr-fr/library/7314433t(v=VS.90).aspx
  
============================================================================ */


using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.AdomdClient; // Lib de client ADOMD pour SSAS
using Microsoft.AnalysisServices.Xmla; // Lib de client XMLA pour SSAS
using System.IO; 


namespace OptimiseurOde2
{



    // ----------------------------------------------------------------------------
    // Classe des dimensions 1D : DIM_TEMPS, DIM_CLIENTS, DIM_LIEUX et DIM_PRODUITS
    // ----------------------------------------------------------------------------
    class Dimension
    {
        // Membres
        private string dimensionName; // Nom "officiel" de la dimension - potentiellement plusieurs si "dimensionOrder" >= 2
        private int dimensionCount; // Nombre de lignes de la dimension
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
        public Dimension(string inDimensionName, int inDimensionCount, int inDimensionOrder)
        {
            SetDimensionName(inDimensionName);
            SetDimensionCount(inDimensionCount);
            SetDimensionMemory(0);
            dimensionOrder = inDimensionOrder;
        }

        // Methodes
        public void SetDimensionName(string inDimensionName) { dimensionName = String.Copy(inDimensionName); }
        public void SetDimensionCount(int inDimensionCount) { dimensionCount = inDimensionCount; }
        public void SetDimensionMemory(int inDimensionMemory) { dimensionMemory = inDimensionMemory; }
        // REM : Pas d'accesseur Set pour "dimensionOrder" car parametré à l'instanciation de classe

        public string GetDimensionName() { return (dimensionName); }
        public int GetDimensionCount() { return (dimensionCount); }
        public int GetDimensionMemory() { return (dimensionMemory); }
        public int GetDimensionOrder() { return (dimensionOrder); }
    }




    // -----------------
    // MAIN du programme
    // -----------------
    class Program
    {
        static void Main(string[] args)
        {
            // Chaine de connexion SSAS
            string ssasConnectionString = "Data Source=localhost;Catalog=cubeODE";   // CATALOG : Nom du cube
            string cubeId = "[Data Warehouse ODE]"; // Nom du cube 

            // Instance de classe pour utilisation de fonctions ADOMD
            AdomdConnection adoConnect = new AdomdConnection(ssasConnectionString);

            // Ouverture de la connexion SSAS
            adoConnect.Open();

            // Instance de classe de dimensions 1D
            List<Dimension> listDim1D = new List<Dimension>();

            // Liste de toutes les dimensions 1D: 
            GetDimension1DProperties(adoConnect, cubeId, listDim1D);

            // DISPLAY TEST
            for (int i = 0; i < listDim1D.Count(); i++)
            {
                Console.WriteLine(listDim1D[i].GetDimensionName() + " \t " + listDim1D[i].GetDimensionCount() + " lignes \t " + listDim1D[i].GetDimensionMemory() + " octets");
            }

            // Instance de classe de dimensions 2D, 3D, etc...
            List <Dimension> listDimCombined = new List<Dimension>();

            // Appel de l'algorithme de combinaisons
            ConstructCombinedDimensions(listDim1D, listDimCombined);



            // Reformulation en MDX pour chiffrage de volumetrie de chaque combinaisons


            // Envoi à SSAS



            // Cloture de la connexion SSAS
            adoConnect.Close();

            // Fin de programme : Attente de l'utilisateur
            Console.WriteLine(Environment.NewLine + "Press any key to continue.");
            Console.ReadKey();
        }



        // --------------------------------------
        // THOMAS : Combinaisons de dimensions 1D
        // --------------------------------------

        // Creation d'une liste vide de combinaisons de dimensions : listCombinaisonsDim[N] | N = 
        //    [APEX : 1] + [1D : A(1, nbrDim1D)] + [2D : A(2, nbrDim1D)] + [3D : A(3, nbrDim1D)] + [4D : A(4, nbrDim1D)] + ... WHILE(A < nbrDim1D)
        //    tq. A(i, j) = j! / (i! * (j - i)!) && i <= j
        // Sinon : Laisser faire à la fonction en transmettant simplement un VECTOR de dimensions

        static void ConstructCombinedDimensions(List<Dimension> listDim1D, List<Dimension> listDimCombined)
        {
            ;

            /* TO DO - THOMAS 
                Peut contenir : 
                {
                    ...
                    Dimension tempXxxxx = new Dimension(n); // Tq. n : [INT] Dimension de la combinaison (1D, 2D, etc...)
                    ...
                    Dimension tempXxxxx = new Dimension(2) // Pour les 2D
                    Dimension tempXxxxx = new Dimension(3) // Pour les 3D
                    Dimension tempXxxxx = new Dimension(4) // Pour les 4D
                    ...
                    listDimCombined.Add(tempXxxx);  // Enregistrement dans la liste de dimensions passée en argument, quelque soit sa dimension
                }            
            */
        }



        // -------------------------------------------------------------
        // Exploration DMV de SSAS (Vues prédefinies sur tables systeme)
        // -------------------------------------------------------------
        static void GetDimension1DProperties(AdomdConnection adoConnect, string cubeId, List<Dimension> listDim1D)
        {
            AdomdCommand cmdPart1 = adoConnect.CreateCommand();
            AdomdCommand cmdPart2 = adoConnect.CreateCommand();

            // Dimension tempDim1D = new Dimension(1);


            // Toutes les dimensions d'un cube, leur type et leur cardinalité
            // https://msdn.microsoft.com/en-us/library/ms126309.aspx
            // https://msdn.microsoft.com/en-us/library/ms126180.aspx

            // Etape 1 : Récuperation des noms et des count
            cmdPart1.CommandText = "SELECT " +
                               "     DIMENSION_NAME, " +
                               "     DIMENSION_CARDINALITY " +
                               "FROM " +
                               "     $SYSTEM.MDSCHEMA_DIMENSIONS " +
                               "WHERE " +
                               "     CUBE_NAME = 'Data Warehouse ODE' AND " +
                               "    DIMENSION_CAPTION <> 'Measures' " +
                               "ORDER BY DIMENSION_NAME";

            // Ouverture du reader XML
            AdomdDataReader readerPart1 = cmdPart1.ExecuteReader();

            // Enregistrement des données dans des instances de Dimension
            while (readerPart1.Read())
            {
                // Enregistrement dans la listDim1D
                listDim1D.Add(new Dimension(readerPart1.GetString(0), readerPart1.GetInt32(1), 1));
            }

            // Fermeture du reader XML
            readerPart1.Close();



            // Etape 2 : Récuperation des tailles (Octets)
            // Utilisation mémoire : https://msdn.microsoft.com/en-us/library/bb934098(v=sql.120).aspx
            cmdPart2.CommandText = "SELECT " +
                                    "   (OBJECT_MEMORY_SHRINKABLE + OBJECT_MEMORY_NONSHRINKABLE + OBJECT_MEMORY_CHILD_SHRINKABLE + OBJECT_MEMORY_CHILD_NONSHRINKABLE) " +
                                    "FROM " +
                                    "    $SYSTEM.DISCOVER_OBJECT_MEMORY_USAGE " +
                                    "WHERE " +
                                    "    OBJECT_TYPE_ID = 100006 AND " +
                                    "    (OBJECT_ID = 'DIM TEMPS' OR OBJECT_ID = 'DIM LIEUX' OR OBJECT_ID = 'DIM PRODUITS' OR OBJECT_ID = 'DIM CLIENTS')" +
                                    "ORDER BY OBJECT_ID";

            // Ouverture du reader XML
            AdomdDataReader readerPart2 = cmdPart2.ExecuteReader();

            // Enregistrement des données dans des instances de Dimension

            // while (readerPart2.Read())
            for (int i = 0; i < listDim1D.Count(); i++)
            {
                readerPart2.Read();

                // 3eme retour : [TOTAL_MEM_SIZE]
                if (listDim1D[i].GetDimensionCount() != 0)
                {
                    listDim1D[i].SetDimensionMemory(readerPart2.GetInt32(0) / listDim1D[i].GetDimensionCount());
                }
                else
                {
                    listDim1D[i].SetDimensionMemory(0);
                }
            }

            // Fermeture du reader XML
            readerPart2.Close();
 
        }


        // -------------------------------------------
        // Methode d'envoi du schéma d'agregats (XMLA)
        // -------------------------------------------
        static void SendAggregation(string cubeId)
        {
            XmlaClient clnt = new XmlaClient();  // Attention : C'est une instance de "XmlaClient", pas de "AdomdConnection"
            clnt.Connect(@"localhost\sql05");
            string xmla = "<Process xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" +
                                                " " +
                                                " " +
                                                " " +
                                                " " + cubeId + " " +
                                                " " +
                                                " ";

            // Envoi de la commande XMLA
            clnt.Send(xmla, null);

            // Deconnexion de SSAS
            clnt.Disconnect();
        }




        // ------------------------------------	
        // Methode de processing du cube (XMLA)
        // ------------------------------------	
        static void ProcessCube(string cubeId)
        {
            XmlaClient clnt = new XmlaClient();  // Attention : C'est une instance de "XmlaClient", pas de "AdomdConnection"
            clnt.Connect(@"localhost\sql05");
            string xmla = "<Process xmlns=\"http://schemas.microsoft.com/analysisservices/2003/engine\">" +
                                                "  <Object>" +
                                                "    <DatabaseID>" + cubeId + "</DatabaseID>" +
                                                "  </Object>" +
                                                "  <Type>ProcessFull</Type>" +
                                                "  <WriteBackTableCreation>UseExisting</WriteBackTableCreation>" +
                                                "</Process>";

            // Envoi de la commande XMLA
            clnt.Send(xmla, null);

            // Deconnexion de SSAS
            clnt.Disconnect();
        }
    }
}




