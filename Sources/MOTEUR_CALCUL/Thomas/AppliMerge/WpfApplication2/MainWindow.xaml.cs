/* 
A VOIR ................
pour les variables, voir on ne peut pas utiliser un fichier de configuration http://www.techheadbrothers.com/Articles.aspx/adomd-net-bases-requeteur-olap-multidimensionnel-page-3
http://www.developpez.net/forums/d989898/logiciels/solutions-d-entreprise/business-intelligence/microsoft-bi/ssas/2k8-extraire-storagemode-cube/



aggreagtion :
https://msdn.microsoft.com/en-us/library/ms345091.aspx

http://www.ssas-info.com/analysis-services-scripts/1622-script-to-automate-ssas-partition-management-sql-ssis

https://technet.microsoft.com/en-us/library/ms345091%28v=sql.105%29.aspx
http://www.techheadbrothers.com/Articles.aspx/optimisation-cubes-design-agregations-page-4
https://msdn.microsoft.com/en-us/library/bb934053.aspx

http://www.techheadbrothers.com/Articles.aspx/optimisation-cubes-design-agregations-page-4





*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AnalysisServices;
//using Microsoft.SqlServer.Management.Common;
//using Microsoft.SqlServer.Management.Sdk;
//using Microsoft.SqlServer.Management.Smo;
//using System.Data.SqlClient;
using System.Data;
//using System.Data.OleDb;

namespace WpfApplication2
{

    // -- Debut OLIVIER

    /// <summary>
    /// Classe des dimensions 1D : DIM_TEMPS, DIM_CLIENTS, DIM_LIEUX et DIM_PRODUITS
    /// </summary>
    /// 
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
    // -- Fin OLIVIER



    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public static string Status_Traitement = "OK";

        public MainWindow()
        {
            InitializeComponent();
            InitializeBis();
        }

        // Initialisation de l'interface
        public void InitializeBis()
        {
            Bouton_Connexion.IsEnabled = false;
            Barre_Espace.IsEnabled = false;
            Barre_Selection.IsEnabled = false;
            Bouton_Algo_1.IsEnabled = false;
            Bouton_Algo_2.IsEnabled = false;
            Liste_Cube.Items.Clear();
        }

        // Demande de connexion
        private void Bouton_Connexion_Click(object sender, RoutedEventArgs e)
        {
            string StrConnexion = "Datasource=" + Nom_Server.Text + ";Catalog=" + Nom_Database.Text + ";";
            AdomdConnection conn = Connexion_Base(StrConnexion);

            // -- Debut OLIVIER
            string cubeId = "[Data Warehouse ODE]"; // Nom du cube 

            // Instance de classe de dimensions 1D
            List<Dimension> listDim1D = new List<Dimension>();

            // Liste de toutes les dimensions 1D: 
            GetDimension1DProperties(conn, cubeId, listDim1D);

            // -- Fin OLIVIER

            Liste_Cube.Items.Clear();

            if (Status_Traitement == "OK")
            {
                Barre_Espace.IsEnabled = true;
                Barre_Selection.IsEnabled = true;
                Bouton_Algo_1.IsEnabled = true;
                Bouton_Algo_2.IsEnabled = true;

                int nbOfCubes = conn.Cubes.Count;
                for (int i = 0; i < nbOfCubes; i++)
                {
                    CubeDef cube = conn.Cubes[i];
                    if (cube.Type == CubeType.Cube)
                    {
                        int dd = 0;
                        //Liste_Cube.Items.Add(conn.Cubes[i].Name);
                    }
                }

                // Détermination nombre d'agrégation        -----------------------------------------------------
                Server srv = new Server();
                srv.Connect(Nom_Server.Text);
                Database db = srv.Databases.FindByName(Nom_Database.Text);

                int j = 1;
                foreach (Cube Cube3 in db.Cubes)
                {
                    Liste_Cube.Items.Add(Cube3.Name);
                    string gg = Cube3.StorageLocation;
                    foreach (MeasureGroup Meg in Cube3.MeasureGroups)
                    {

                        foreach (AggregationDesign agr in Meg.AggregationDesigns)
                        {
                            string dd = "dd";
                        }
                        string rr = "kk";


                        // Creation aggregation
                        if (rr == "hi")
                        {
                            // Creation aggregat
                            //Cube cube = server.Databases["Adventure Works DW Standard Edition"].Cubes["InternetSales"];
                            //AggregationDesign ad = cube.MeasureGroups["Fact Internet Sales"].AggregationDesigns["AggregationDesign linked"];
                            //Aggregation aggr = ad.Aggregations.Add("ECM A1", "ECM A1");
                            //AggregationDesign ad = Meg.AggregationDesigns["AggregationDesign linked"];
                            //Ajout des dimensions
                            //foreach (CubeDimension dim in Cube3.Dimensions)
                            //{   aggr.Dimensions.Add(dim.ID);  }
                            //AggregationDimension orderDateAggregationDimension = aggr.Dimensions["Order Date"];
                            //AggregationDimension promotionAggregationDimension = aggr.Dimensions["Promotion"];
                            //AggregationAttribute year =  new AggregationAttribute (cube.Dimensions["Order Date"].Attributes["CalendarYear"].AttributeID);
                            //AggregationAttribute month = new AggregationAttribute(cube.Dimensions["Order Date"].Attributes["EnglishMonthName"].AttributeID);
                            //AggregationAttribute promotion = new AggregationAttribute(cube.Dimensions["Promotion"].Attributes["Promotion Category"].AttributeID);

                            //orderDateAggregationDimension.Attributes.Add(year);
                            //orderDateAggregationDimension.Attributes.Add(month);
                            //promotionAggregationDimension.Attributes.Add(promotion);
                            //ad.Update();




                            double optimization = 0;
                            double storage = 0;
                            long aggCount = 0;
                            bool finished = false;

                            AggregationDesign ad = null;

                            String aggDesignName;
                            String AggregationsDesigned = "";

                            //aggDesignName = Meg.AggregationPrefix + "_" + Meg.Name;
                            aggDesignName = "test" + "_" + Meg.Name;
                            ad = Meg.AggregationDesigns.Add();

                            ad.InitializeDesign();
                            //optimization = 0;
                            //storage = 0;
                            //aggCount = 0;

                            foreach (CubeDimension dim in Cube3.Dimensions)
                            { ad.Dimensions.Add(dim.ID); }

                            //finished = False;
                            //  while (!finished)
                            //While Not finished And optimization < optimizationWanted
                            //And storage < maxStorageBytes
                            //{ ad.DesignAggregations(out optimization, out storage, out aggCount, out finished);
                            //}
                            ad.FinalizeDesign();


                            foreach (Partition part in Meg.Partitions)
                            {
                                part.AggregationDesignID = ad.ID;
                                //    AggregationsDesigned += aggDesignName + " = " + aggCount.ToString() + " aggregations designed\r\n\tOptimization: " + optimization.ToString() + "/" + optimizationWanted.ToString() + "\n\r\tStorage: " + storage.ToString() + "/" + maxStorageBytes.ToString() + " ]\n\r";
                            }

                            // ad.Update();

                            //foreach (Partition part in Meg.Partitions)
                            //{
                            //    part.AggregationDesignID = ad.ID;
                            //    AggregationsDesigned += aggDesignName + " = " + aggCount.ToString() + " aggregations designed\r\n\tOptimization: " + optimization.ToString() + "/" + optimizationWanted.ToString() + "\n\r\tStorage: " + storage.ToString() + "/" + maxStorageBytes.ToString() + " ]\n\r";
                            //}

                            Cube3.Update(UpdateOptions.ExpandFull);
                            //Meg.Update();
                            Cube3.Process(ProcessType.ProcessFull);




                            //ad.Name = aggDesignName;
                            //ad.InitializeDesign();
                            //while ((!finished) && (optimization < optimizationWanted) && (storage < maxStorageBytes))
                            //while (!finished)
                            //{
                            //    ad.DesignAggregations(out optimization, out storage, out aggCount, out finished);
                            //}
                            //ad.FinalizeDesign();
                            //ad.Update()
                            //foreach (Partition part in mg.Partitions)
                            //{
                            //    part.AggregationDesignID = ad.ID;
                            //    AggregationsDesigned += aggDesignName + " = " + aggCount.ToString() + " aggregations designed\r\n\tOptimization: " + optimization.ToString() + "/" + optimizationWanted.ToString() + "\n\r\tStorage: " + storage.ToString() + "/" + maxStorageBytes.ToString() + " ]\n\r";
                            //}

                            //fin dreation aggregtation


                        }


                        //  foreach (Partition part in Meg.Partitions)
                        //  {
                        //      int dd = Meg.AggregationDesigns.Count;
                        //  }
                    }
                    j++;
                }
                srv.Disconnect();
            }
            else
            {
                Barre_Espace.IsEnabled = false;
                Barre_Selection.IsEnabled = false;
                Bouton_Algo_1.IsEnabled = false;
                Bouton_Algo_2.IsEnabled = false;
            }
        }

        // Modification nom de la database
        private void Nom_Database_TextChanged(object sender, TextChangedEventArgs e)
        {
            // On rend actif le bouton si les noms de database et de server sont sélectionnés
            if ((Nom_Database.Text != "") & (Nom_Server.Text != ""))
            {
                Bouton_Connexion.IsEnabled = true;
            }
            else
            {
                Bouton_Connexion.IsEnabled = false;
            }
            Barre_Espace.IsEnabled = false;
            Barre_Selection.IsEnabled = false;
            Bouton_Algo_1.IsEnabled = false;
            Bouton_Algo_2.IsEnabled = false;
            Liste_Cube.Items.Clear();
        }

        // Modification nom du cube
        private void Nom_Cube_TextChanged(object sender, TextChangedEventArgs e)
        {
            // On rend actif le bouton si le nom de database et de server sont sélectionnés
            if ((Nom_Database.Text != "") & (Nom_Server.Text != ""))
            {
                Bouton_Connexion.IsEnabled = true;
            }
            else
            {
                Bouton_Connexion.IsEnabled = false;
            }
            Barre_Espace.IsEnabled = false;
            Barre_Selection.IsEnabled = false;
            Bouton_Algo_1.IsEnabled = false;
            Bouton_Algo_2.IsEnabled = false;
            Liste_Cube.Items.Clear();
        }

        private void Bouton_Algo_1_Click(object sender, RoutedEventArgs e)
        {

            // AFAIRE -------------------------------
            string StrConnexion = "Datasource=" + Nom_Server.Text + ";Catalog=" + Nom_Database.Text + ";";
            AdomdConnection conn = Connexion_Base(StrConnexion);

            if (Status_Traitement == "OK")
            {
                // Partie 1 - Commune  

                // -- Debut OLIVIER : récupération des 1D-dimensions et des propriétés
                string cubeId = "[Data Warehouse ODE]"; // Nom du cube 

                // Instance de classe de dimensions 1D
                List<Dimension> listDim1D = new List<Dimension>();

                // Liste de toutes les dimensions 1D: 
                GetDimension1DProperties(conn, cubeId, listDim1D);
                // -- Fin OLIVIER

                // -- Début THOMAS : "récupération" du treillis des cuboïdes
                List<Dimension> listCuboides = new List<Dimension>(); // liste des cuboïdes
                List<String> index_cuboides = new List<String>(); // liste des index des cuboïdes : plus facile à utiliser par la suite
                String prefix_index = "";
                Console.WriteLine("LISTE DES CUBOÏDES :");
                GetCombinaisons(listDim1D, 0, "", 0, listCuboides, index_cuboides, prefix_index); // appel de la fonction qui crée les combinaisons
                Console.WriteLine();
                // -- Fin THOMAS

                // -- Début THOMAS : "récupération" du nombre de lignes des cuboïdes
                GetPoidsCuboides(index_cuboides, listCuboides, listDim1D, listDim1D.Count, Nom_Server.Text, Nom_Database.Text); // appel de la fonction qui récupère le nombre de lignes des cuboides
                Console.WriteLine();
                // -- Fin Thomas

                // Partie 2 - Algo Spécifique Thomas
                int seuil_poids = 1000000; // le seuil de poids à ne pas dépasser : à récupérer via l'interface plus tard
                int[] solution = new int[listCuboides.Count]; // la solution que va retrouner Metropolis sous forme de 0 et de 1
                int nb_boucle = 100; // le nombre de boucle que l'on veut faire à l'algo de Metropolis : éventuellement à faire saisir par l'utilisateur
                Metropolis(listCuboides, seuil_poids, nb_boucle, solution); // appel de l'algo de Metropolis

                Console.WriteLine();
                Console.WriteLine("CUBOÏDES A MATERIALISER :");
                for (int i = 0; i < solution.Length; i++)
                {
                    if(solution[i] == 1)
                    {
                        Console.Write(listCuboides[i].GetDimensionName()+" | "); // affichage de la solution finale à matérialiser
                    }
                }
                Console.WriteLine();
                // Partie 3 - Commune

                String Status_Deconnexion = DeconnectToCube(StrConnexion);
                // Actualisation infos interface si tout est ok
                //Barre_Espace.IsEnabled = true;
                //Barre_Selection.IsEnabled = true;
                Bouton_Algo_1.IsEnabled = false;
                Bouton_Algo_2.IsEnabled = false;
            }
        }

        private void Bouton_Algo_2_Click(object sender, RoutedEventArgs e)
        {
            // AFAIRE
            string StrConnexion = "Datasource=" + Nom_Server.Text + ";Catalog=" + Nom_Database.Text + ";";
            AdomdConnection conn = Connexion_Base(StrConnexion);

            if (Status_Traitement == "OK")
            {
                // Partie 1 - Commune : récupération des 1D-dimensions et des propriétés
                
                // -- Debut OLIVIER
                string cubeId = "[Data Warehouse ODE]"; // Nom du cube 

                // Instance de classe de dimensions 1D
                List<Dimension> listDim1D = new List<Dimension>();

                // Liste de toutes les dimensions 1D: 
                GetDimension1DProperties(conn, cubeId, listDim1D);
                // -- Fin OLIVIER

                // Partie 2 - Algo Spécifique Olivier


                // Partie 3 - Commune

                String Status_Deconnexion = DeconnectToCube(StrConnexion);
                // Actualisation infos interface si tout est ok
                //Barre_Espace.IsEnabled = true;
                //Barre_Selection.IsEnabled = true;
                Bouton_Algo_1.IsEnabled = false;
                Bouton_Algo_2.IsEnabled = false;
            }

        }

        private AdomdConnection Connexion_Base(string StrConnect)
        {
            Status_Traitement = "KO";
            AdomdConnection conn = new AdomdConnection(StrConnect);
            try
            {
                conn.Open();
                //Utilisation de la donnée pour déclencher l'exception
                int nbOfCubes = conn.Cubes.Count;
                Status_Traitement = "OK";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to connect to data source");
            }

            //ASUPP
            //string strStringConnection = "Provider=MSOLAP.4;Data Source=localhost;Integrated Security=SSPI;Initial Catalog=MyDataBase;Impersonation Level=Impersonate";
            //Server svr = new Server();
            //svr.Connect(strStringConnection);
            //
            //String databaseName = "MyDataBase";
            //Database db = svr.Databases.FindByName(databaseName);
            //Cube cube = db.Cubes.FindByName("MyCube");

            // Liste des databases disponibles
            Server svr = new Server();
            svr.Connect("Localhost");
            foreach (Database db in svr.Databases)
            {
                string dd = db.Name;
            }

















            return conn;
        }

        // Fonction de déconnexion à la base
        public static string DeconnectToCube(string StrConnect)

        {
            String StatusDeconnect = "KO";
            AdomdConnection conn = new AdomdConnection(StrConnect);
            conn.Close();
            return StatusDeconnect;
        }


        // -- Debut OLIVIER

        /// <summary>
        /// Exploration DMV de SSAS (Vues prédefinies sur tables systeme)
        /// </summary>
        /// 
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

        // -- Début THOMAS : Algorithme des combinaisons (fonction récursive qui retroune l'ensemble des combinaisons possibles de notre liste listDim1D
        static void GetCombinaisons(List<Dimension> listDim1D, int profCourante, String prefix, int rang, List<Dimension> listCuboides, List<String> index_cuboides, String prefix_index)
        {
            for (int i = rang; i < listDim1D.Count; i++)
                {
                listCuboides.Add(new Dimension(prefix + listDim1D[i].GetDimensionName(), 0, profCourante + 1));
                Console.WriteLine("Nom : " + prefix + listDim1D[i].GetDimensionName() + " | Nb lignes : ND | Dimension : " + (profCourante + 1).ToString());
                index_cuboides.Add(prefix_index + i.ToString());
            }

                for (int i = rang; i < listDim1D.Count; i++)
                {
                    GetCombinaisons(listDim1D, profCourante + 1, prefix
                          + listDim1D[i].GetDimensionName() + " * ", i + 1, listCuboides, index_cuboides, prefix_index + i.ToString());
                }
        }
        // -- Fin THOMAS

        // -- Début THOMAS : Algorithme qui récupère le nombre de lignes des cuboides
        static void GetPoidsCuboides(List<String> cuboides, List<Dimension> listCuboides, List<Dimension> listDim1D, int nb_dim_1d,String datasource, String catalog)
        {
            // Chaine de connexion SSAS
            AdomdConnection conn = new AdomdConnection("Data Source="+datasource+";Catalog="+catalog);  // CATALOG : Nom du cube

            // Membres
            DataSet ds;
            DataTable dt;

            // Ouverture de la connexion SSAS
            conn.Open();

            ds = conn.GetSchemaDataSet(AdomdSchemaGuid.MeasureGroupDimensions, null);
            dt = ds.Tables[0];

            String[] test_dimensions = new String[nb_dim_1d*2];
            int cpt = 0;
            int poids_une_ligne = 0;

            Console.WriteLine("RÉCUPÉRATION DES CLÉS PRIMAIRES :");
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                    if (col.ColumnName == "DIMENSION_GRANULARITY")
                    {
                        test_dimensions[cpt] = row[col].ToString(); // récupération des clés primaire pour faire les requêtes MDX ensuite
                        cpt = cpt + 1;
                        Console.WriteLine(row[col].ToString());
                    }
            }
            Console.WriteLine();

            int poids;
            String commandText = "";

            Console.WriteLine("CALCULS DU NOMBRE DE LIGNES DES CUBOÏDES :");
            for (int i = 0; i < cuboides.Count; i++)

            {

                poids = 0;

                Console.Write("Cuboide : " + listCuboides[i].GetDimensionName() + " | Dimension = " + listCuboides[i].GetDimensionOrder());

                // on lance une requête MDX selon le nombre de dimensions à croiser /!\ : 4 maximum pour le moment /!\
                if (cuboides[i].Length == 1)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i])] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i])].GetDimensionMemory();
                }
                if (cuboides[i].Length == 2)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory()+ listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory();
                }
                if (cuboides[i].Length == 3)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(2, 1))].GetDimensionMemory();
                }
                if (cuboides[i].Length == 4)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(3, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(2, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(3, 1))].GetDimensionMemory();
                }


                try
                {
                    AdomdCommand cmd = new AdomdCommand(commandText, conn);
                    CellSet cs = cmd.ExecuteCellSet();

                    // On compte le nombre de lignes retournées par la requête (le nombre de colonne est égale à 1)
                    foreach (Position pRow in cs.Axes[1].Positions)
                    {
                        foreach (Position pCol in cs.Axes[0].Positions)
                        {
                            poids = poids + 1;
                        }
                    }


                    Console.WriteLine(" | Nb ligne  = " + poids);
                    listCuboides[i].SetDimensionCount(poids); // on affece le nombre de lignes au cuboide
                    listCuboides[i].SetDimensionMemory(poids_une_ligne);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
        // -- Fin THOMAS

        // -- Début THOMAS
        static void Metropolis(List<Dimension> listCuboides, int seuil_poids, int nb_boucle, int[] sol_act)
        {
            int poids_act = 0;
            int poids_next = 0;
            int i = 0;
            int choix_cube = 0;
            Random rand = new Random();
            double ratio = 0;

            Console.WriteLine("ALGORITHME DE METROPOLIS : ");
            while (i < nb_boucle)
            {
                Console.Write("Solution actuelle : ");
                foreach (int element in sol_act)
                {
                    System.Console.Write(element + " ");
                }

                Console.WriteLine(" - Itération : " + i);

                choix_cube = rand.Next(0, listCuboides.Count); // on choisit un cube à permuter
                Console.Write("Cube choisi pour permutation : n°" + choix_cube);
                if (sol_act[choix_cube] == 0) // s'il est à 0, on ajoute son poids
                    poids_next = poids_act + (listCuboides[choix_cube].GetDimensionCount() * listCuboides[choix_cube].GetDimensionMemory());
                else // sinon on on l'enlève
                    poids_next = poids_act - (listCuboides[choix_cube].GetDimensionCount() * listCuboides[choix_cube].GetDimensionMemory());

                if (poids_next <= seuil_poids) // on continue uniquement si le poids de la future solution ne dépasse pas le seuil fixé
                {
                    if (poids_next >= poids_act) // si c'est une meilleure solution, on effecture la permutation
                    {
                        Console.Write(" | Poids solution "+i+" ("+poids_act+") < Poids solution "+(i+1)+" ("+poids_next+") ET inférieur au seuil ("+seuil_poids+") => Permutation effectué");
                        sol_act[choix_cube] = 1;
                        poids_act = poids_next;
                    }
                    else // sinon, on effectueun tirage de Bernouilli
                    {

                        ratio = Convert.ToDouble(poids_next) / Convert.ToDouble(poids_act);

                        Console.Write(" | Ratio : " + poids_next + "/" + poids_act + " = " + ratio);

                        if (rand.NextDouble() < ratio) // si le tirage est inférieur au ratio (poids de la solution suivante / poids de la solution actuelle), on permute
                        {
                            Console.Write(" | Tirage Bernoulli = Succes => Permutation effectué");
                            sol_act[choix_cube] = 0;
                            poids_act = poids_next;
                        }
                    }
                }
                i = i + 1;
                Console.WriteLine();
            }
            Console.Write("Solution finale : ");
            foreach (int element in sol_act)
            {
                System.Console.Write(element + " ");
            }
            Console.WriteLine();
        }
        // -- Fin THOMAS


    }
}













// Cédric -            pour tests -----------------------------------

//Server MyServer = new Server(".");
//MessageBox.Show(MyServer.Name);
//DatabaseCollection db = MyServer.Databases;
//for (int i = 0; i < db.Count; i++)
//{
//    comboBox.Items.Add(db[i].Name);
//}

/*Server objServer = new Server();
string strconnection = "Data Source=Localhost;Provider=cube;";

objServer.Connect(strconnection);
*/

// Chaine de connexion SSAS
//AdomdConnection conn = new AdomdConnection("Data Source=localhost;Catalog=cube");  // CATALOG : Nom du cube

// Membres
//DataSet ds;
//DataTable dt;


// Ouverture de la connexion SSAS
//conn.Open();





//ds = conn.GetSchemaDataSet(AdomdSchemaGuid.Measures, null);
//dt = ds.Tables[0];

//foreach (DataRow row in dt.Rows)
//{
//    foreach (DataColumn col in dt.Columns)
//        comboBox.Items.Add(col.ColumnName + " = " + row[col].ToString());

//Console.WriteLine(col.ColumnName + " = " + row[col].ToString());

//Console.WriteLine();
//}


/* Liste des mesures
ds = conn.GetSchemaDataSet(AdomdSchemaGuid.Measures, null);
dt = ds.Tables[0];

foreach (DataRow row in dt.Rows)
{
    foreach (DataColumn col in dt.Columns)
        comboBox.Items.Add(col.ColumnName + " = " + row[col].ToString());

    //Console.WriteLine(col.ColumnName + " = " + row[col].ToString());

    //Console.WriteLine();
}

*/


/* using (var conn = new AdomdConnection(@"Data Source=Localhost;Initial Catalog=cube"))
 {
     conn.Open();
     foreach (var cube in conn.Cubes)
     {
         //     Console.WriteLine(cube.Type.ToString() + " " + cube.Name + " " + cube.LastProcessed);
         comboBox.Items.Add(cube.Name);

     }

     conn.Close();


 }
 */



/*  public void PutTranslation(Server root, InputLine l)
{
//D'abord on descend le path jusqu'a l'objet a traduire 
MajorObject closerMajorObject = root;
ModelComponent current = root;
while (l.Path.Length > 0) { string currentPath = l.Path.Split('.')[0]; object childrenCollection = current.GetType() .GetProperty(currentPath.Split('[')[0]) .GetValue(current, null); //On positionne le courant sur la collection fille, a l'index specifie PropertyInfo accessor = childrenCollection.GetType() .GetProperty("Item", new Type[] { typeof(string) }); current = accessor .GetValue(childrenCollection , new object[] { currentPath.Split('[')[1].TrimEnd(']') }) as ModelComponent; //Si on est sur le dernier noeud on vide la chaine pour sortir if (l.Path.Contains('.')) l.Path = l.Path.Substring(currentPath.Length + 1); else l.Path = string.Empty; } //On recupere la collection a modifier object translationCollection = current.GetType() .GetProperty(l.CustomCollection) .GetValue(current, null); //On recherche le type de traduction attendue //(les DimensionAttribute utilisent un type particulier) string type = (current.GetType().Name == "DimensionAttribute") ? "AttributeTranslation" : "Translation"; //On recherche la methode add sur la collection MethodInfo translationAdder = translationCollection.GetType() .GetMethods() .Where(m => m.Name == "Add" && m.GetParameters()[0].ParameterType.Name == type) .First(); //On recherche un objet de traduction pour ce langage si il existe object translation = translationCollection.GetType() .GetMethod("FindByLanguage") .Invoke(translationCollection, new object[] { l.Language }); //Sinon on cree un nouvel objet en invoquant le constructeur (int32) if (translation == null) { translation = translationAdder.GetParameters()[0].ParameterType .GetConstructor(new Type[] { typeof(Int32) }) .Invoke(new object[] { l.Language }); translationAdder.Invoke(translationCollection, new object[] { translation }); } //On affecte la valeur translation.GetType() .GetProperty(l.Property) .SetValue(translation, l.Value, null);










}
*/
// Console.ReadKey();




/*string strDBServerName = "LocalHost";
//string strProviderName = "msolap";

try
{
    Console.WriteLine("Connecting to the Analysis Services ...");

    Server objServer = new Server();
    string strConnection = "Data Source=" + strDBServerName + ";Provider=" + strProviderName + ";";
    //Disconnect from current connection if it's currently connected.
    if (objServer.Connected)
        objServer.Disconnect();
    else
        objServer.Connect(strConnection);


}
catch (Exception ex)
{
    Console.WriteLine("Error in Connecting to the Analysis Services. Error Message -> " + ex.Message);

}
*/
//C:/ Program Files / Cube.cub
//125Go Libres sur 685 Go
//Actuel : 125Mo
//Souhaité : 500 Mo



//récupération info sur disque
//System.IO.DriveInfo di = new System.IO.DriveInfo(@"C:\");
//long i = di.TotalSize;
//long j = di.AvailableFreeSpace;
//long h = (i / 1024) / 1024;
//long k = (j / 1024) / 1024;

//test.Maximum = h;
//test.Minimum = 0;
//test.Value = h - k;








// voir drive info
//hr />foreach (
//DriveInfo di
//in
//DriveInfo.GetDrives())
//          {
//
//   Console.WriteLine(di.Name);
//
//   if (di.IsReady)
//   {
//
//                    Console.WriteLine(di.VolumeLabel);
//
//                  Console.WriteLine(di.TotalSize);
//
//              Console.WriteLine(di.AvailableFreeSpace);
//            }


//Alimentation de la liste des cubes disponibles
//AdomdConnection myconnect = new AdomdConnection("Data Source=localhost");
//myconnect.Open();


//string tblDatabases = myconnect.GetSchemaDataSet();




//foreach (CubeDef cube in myconnect.Cubes)
//{
//    string dde = myconnect.Database; 
//if  (cube.Name.StartsWith("$")) { string ddee =  " "; }
//else
//{
//    string dd = cube.Name.ToString();
//    label9.Content = dd;
//}
//}
//public void ConnectToSql()
//{
// System.Data.SqlClient.SqlConnection conn =
//     new System.Data.SqlClient.SqlConnection();
//  TODO: Modify the connection string and include any
//  additional required properties for your database.
// DSN=baseTest
// conn.ConnectionString = "data source=" + Nom_Server.Text + "; " + ";initial catalog=" + Nom_Database.Text ;
// conn.ConnectionString = "DSN=" + Nom_Database.Text;
// OdbcConnection connection = new OdbcConnection();

//try
//{
//    conn.Open();
// Insert code to process data.

//Bouton_Connexion.IsEnabled = false;
//                Barre_Espace.IsEnabled = true;
//                Barre_Selection.IsEnabled = true;
//                Bouton_Algo_1.IsEnabled = true;
//                Bouton_Algo_2.IsEnabled = true;



            //}
            //catch (Exception ex)
            //{
            //    Libelle_Algo_1.Content = "Aie";
            //    //MessageBox.Show("Failed to connect to data source");
            //}
            //finally
            //{
            //    conn.Close();
            //}
//        }







