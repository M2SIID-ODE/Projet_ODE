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

https://social.msdn.microsoft.com/Forums/en-US/dd318163-810e-48e4-b272-791ee300a658/extracting-cube-dimensions-with-c?forum=sqlanalysisservices


https://msdn.microsoft.com/fr-fr/library/ms174758%28v=SQL.120%29.aspx
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
using System.Data.Sql;
using System.Data.SqlClient;
using System.Windows.Forms;
//using System.Windows.Forms.View;
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
        public static string Status_Traitement = "OK";       // A voir
        public static string Glb_Nom_Server = "";
        public static string Glb_Nom_Database = "";
        public static string Glb_Nom_Cube = "";
        public static List<string> Tab_Agr = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeBis();
        }

        // Initialisation de l'interface lors du premier affichage
        public void InitializeBis()
        {
            Bouton_Connexion.IsEnabled = false;
            Bouton_Aggr.IsEnabled = false;
            Bouton_Algo_1.IsEnabled = false;
            Bouton_Algo_2.IsEnabled = false;
            Nom_Server.Text = "";
            Pres_Aggregat.Content = "";
            Liste_Cube.Items.Clear();
        }

        // Gestion de la modification du champs "Nom du server"
        private void Nom_Server_TextChanged(object sender, TextChangedEventArgs e)
        {
            Liste_Cube.Items.Clear();
            Pres_Aggregat.Content = "";
            Bouton_Aggr.IsEnabled = false;
            Bouton_Algo_1.IsEnabled = false;
            Bouton_Algo_2.IsEnabled = false;
            Tab_Agr.Clear();

            // On rend actif le bouton de connexion si le nom du server est resneigné
            if (Nom_Server.Text != "")
             {
                 Bouton_Connexion.IsEnabled = true;
             }
             else
             {
                 Bouton_Connexion.IsEnabled = false;
             }
         }

         // Demande de connexion au server
         private void Bouton_Connexion_Click(object sender, RoutedEventArgs e)
         {
            // Valeur par défaut des champs
            Liste_Cube.Items.Clear();
            Pres_Aggregat.Content = "";
            Tab_Agr.Clear();

            // Tentative de connexion au server
            Server svr = new Server();

            try
            {
                svr.Connect(Nom_Server.Text);

                // Récupération de la liste des databases
                foreach (Database db in svr.Databases)
                {
                    //Récupération de la liste des databases et cubes
                    for (int i = 0; i < db.Cubes.Count; i++)
                    {
                        Cube cube = db.Cubes[i];
                        Liste_Cube.Items.Add(db.Name + "/" + cube.Name);

                        // Récupération présence d'aggrégats
                        String Stat_Agr = "Aucun"; 
                        foreach (MeasureGroup Meg in cube.MeasureGroups)
                        {
                            if (Meg.AggregationDesigns.Count > 0)
                            {
                                Stat_Agr = "Présent";
                            }
                        }
                        Tab_Agr.Add(Stat_Agr);
                    }
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Server introuvable, merci de vérifier son identifiant.");
            }
            finally
            {
                svr.Disconnect();
            }
        }


        private void Bouton_Algo_1_Click(object sender, RoutedEventArgs e)
        {
            //Connexion à la base
            string StrConnexion = "Datasource=" + Glb_Nom_Server + ";Catalog=" + Glb_Nom_Database + ";";
            AdomdConnection conn = Connexion_Base(StrConnexion);

            if (Status_Traitement == "OK")
            {
                // -- Debut OLIVIER : récupération des 1D-dimensions et des propriétés
                //CVA : string cubeId = "[Data Warehouse ODE]"; // Nom du cube 

                // Instance de classe de dimensions 1D
                List<Dimension> listDim1D = new List<Dimension>();

                // Liste de toutes les dimensions 1D: 
                //CVA : GetDimension1DProperties(conn, cubeId, listDim1D);
                GetDimension1DProperties(conn, Glb_Nom_Cube, listDim1D);
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
                //CVA : GetPoidsCuboides(index_cuboides, listCuboides, listDim1D, listDim1D.Count, Nom_Server.Text, Nom_Database.Text); // appel de la fonction qui récupère le nombre de lignes des cuboides
                GetPoidsCuboides(index_cuboides, listCuboides, listDim1D, listDim1D.Count, conn); // appel de la fonction qui récupère le nombre de lignes des cuboides

                Console.WriteLine();
                // -- Fin Thomas

                // Partie 2 - Algo Spécifique Thomas
                int seuil_poids = 1000000; // le seuil de poids à ne pas dépasser : à récupérer via l'interface plus tard
                int[] solution = new int[listCuboides.Count]; // la solution que va retrouner Metropolis sous forme de 0 et de 1
                int nb_boucle = 100; // le nombre de boucle que l'on veut faire à l'algo de Metropolis : éventuellement à faire saisir par l'utilisateur
                Metropolis(listCuboides, seuil_poids, nb_boucle, solution); // appel de l'algo de Metropolis

                Console.WriteLine();
                Console.WriteLine("CUBOÏDES A MATERIALISER :");

                Deconnexion_Base(StrConnexion);

                // Gestion des agrégats en fonction de la solution retournée
                Server srv = new Server();
                srv.Connect(Glb_Nom_Server);
                Database db = srv.Databases.FindByName(Glb_Nom_Database);
                Cube Cube_maj = db.Cubes.FindByName(Glb_Nom_Cube);

                foreach (MeasureGroup Meg in Cube_maj.MeasureGroups)
                {
                    //Ajout de l'aggrégation Design
                    AggregationDesign ad = null;
                    ad = Meg.AggregationDesigns.Add();
                    ad.Name = "Metro" + Meg.AggregationDesigns.Count;
                    ad.InitializeDesign();
                    int indice = 0;
                    ad.FinalizeDesign();
                    ad.Aggregations.Clear();

                    // Balayage des solutions pour créer de nouvelles aggrégations
                    for (int i = 0; i < solution.Length; i++)
                    {
                        if (solution[i] == 1)
                        {
                            Console.Write(listCuboides[i].GetDimensionName() + " | "); // affichage de la solution finale à matérialiser
                            
                            // Création d'une nouvelle aggrégation
                            Aggregation agg = new Aggregation();
                            indice++;
                            agg.Name = ad.Name + "-" + indice;

                            // Balayage des dimensions
                            foreach (CubeDimension dim in Cube_maj.Dimensions)
                            {
                                // Report de la dimension sur l'aggrégat
                                agg.Dimensions.Add(dim.ID);

                                //Recherche si dimension présente dans la solution séléctionnée
                                int Ind_Rech = listCuboides[i].GetDimensionName().IndexOf(dim.ID);
                                
                                if (Ind_Rech != -1)
                                {
                                // Si présente, on ajoute l'ensemble des champs à l'aggregation
                                    AggregationDimension aggDim = agg.Dimensions[dim.ID];
                                    foreach (DimensionAttribute DimAtt in dim.Dimension.Attributes)
                                    {
                                        if (DimAtt.Usage.ToString() == "Key")
                                        {
                                            AggregationAttribute att = new AggregationAttribute(Cube_maj.Dimensions[dim.ID].Attributes[DimAtt.ID].AttributeID);
                                            aggDim.Attributes.Add(att);
                                        }
                                    }
                                }
                            }
                            ad.Aggregations.Add(agg);
                        }
                    }
                    ad.Update();
                    
                    //Mise à jour du lien sur les partitions
                    foreach (Partition part in Meg.Partitions)
                    {
                        part.AggregationDesignID = ad.ID;
                    }
                    Console.WriteLine();
                }

                //Mise à jour des dimensions en fonction de l'aggrégation
                /*foreach (CubeDimension Dim in Cube_maj.Dimensions)
                {
                    foreach (CubeAttribute Att in Dim.Attributes)
                    {

                        // A faire comparaison avec envoyé Olivier / Thomas
                        if (Dim.Name == "DIM CLIENTS")
                        {
                            Att.AggregationUsage = Microsoft.AnalysisServices.AggregationUsage.Unrestricted;
                        }
                        else
                        {
                            Att.AggregationUsage = Microsoft.AnalysisServices.AggregationUsage.Default;
                        }
                    }
                }*/
                Cube_maj.Update(UpdateOptions.ExpandFull);
                Cube_maj.Process(ProcessType.ProcessFull);
                srv.Disconnect();
                Tab_Agr[Liste_Cube.SelectedIndex] = "Ajouté";
                Pres_Aggregat.Content = Tab_Agr[Liste_Cube.SelectedIndex];
                Bouton_Aggr.IsEnabled = true;
            }
        }


        private void Bouton_Algo_2_Click(object sender, RoutedEventArgs e)
        {
            string StrConnexion = "Datasource=" + Glb_Nom_Server + ";Catalog=" + Glb_Nom_Database + ";";
            AdomdConnection conn = Connexion_Base(StrConnexion);

            if (Status_Traitement == "OK")
            {
                // Partie 1 - Commune : récupération des 1D-dimensions et des propriétés

                // -- Debut OLIVIER
                //CVA : string cubeId = "[Data Warehouse ODE]"; // Nom du cube 

                // Instance de classe de dimensions 1D
                List<Dimension> listDim1D = new List<Dimension>();

                // Liste de toutes les dimensions 1D: 
                //CVA : GetDimension1DProperties(conn, cubeId, listDim1D);
                GetDimension1DProperties(conn, Glb_Nom_Cube, listDim1D);
                // -- Fin OLIVIER

                // Partie 2 - Algo Spécifique Olivier


                // Partie 3 - Commune

                Deconnexion_Base(StrConnexion);

                // Gestion des agrégats
                Server srv = new Server();
                srv.Connect(Glb_Nom_Server);
                Database db = srv.Databases.FindByName(Glb_Nom_Database);
                Cube Cube_maj = db.Cubes.FindByName(Glb_Nom_Cube);

                foreach (MeasureGroup Meg in Cube_maj.MeasureGroups)
                {
                    //Ajout de l'aggrégat
                    AggregationDesign ad = null;
                    ad = Meg.AggregationDesigns.Add();
                    int ordre = Meg.AggregationDesigns.Count + 1;
                    ad.Name = "Mater" + ordre;
                    ad.InitializeDesign();
                    ad.FinalizeDesign();

                    //Mise à jour du lien sur les partitions
                    foreach (Partition part in Meg.Partitions)
                    {
                        part.AggregationDesignID = ad.ID;
                    }
                }

                //Mise à jour des dimensions en fonction de l'aggrégation
                /*foreach (CubeDimension Dim in Cube_maj.Dimensions)
                {
                    foreach (CubeAttribute Att in Dim.Attributes)
                    {

                        // A faire comparaison avec envoyé Olivier / Thomas
                        if (Dim.Name == "DIM CLIENTS")
                        {
                            Att.AggregationUsage = Microsoft.AnalysisServices.AggregationUsage.Unrestricted;
                        }
                        else
                        {
                            Att.AggregationUsage = Microsoft.AnalysisServices.AggregationUsage.Default;
                        }
                    }
                }*/
                Cube_maj.Update(UpdateOptions.ExpandFull);
                Cube_maj.Process(ProcessType.ProcessFull);
                srv.Disconnect();
                Tab_Agr[Liste_Cube.SelectedIndex] = "Ajouté";
                Pres_Aggregat.Content = Tab_Agr[Liste_Cube.SelectedIndex];
                Bouton_Aggr.IsEnabled = true;
            }

        }

        private void Liste_Cube_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Liste_Cube.SelectedIndex != -1)
            {
                string Selection = Liste_Cube.SelectedItem.ToString();
                string searchForThis = "/";
                int FirstOccurs = Selection.IndexOf(searchForThis);

                // Alimentation des globales
                Glb_Nom_Server = Nom_Server.Text.ToString();
                Glb_Nom_Database = Selection.Substring(0, FirstOccurs);
                Glb_Nom_Cube = Selection.Substring(FirstOccurs + 1);

                // Alimenation des informations sur le cube
                Pres_Aggregat.Content = Tab_Agr[Liste_Cube.SelectedIndex];
                if (Tab_Agr[Liste_Cube.SelectedIndex] == "Présent")
                {
                    Bouton_Aggr.IsEnabled = true;
                }
                else
                {
                    Bouton_Aggr.IsEnabled = false;
                }
                Bouton_Algo_1.IsEnabled = true;
                Bouton_Algo_2.IsEnabled = true;
            }
        }

        //Gestion clic sur bouton de suppression des aggrégations
        private void Bouton_Aggr_Click(object sender, RoutedEventArgs e)
        {
            // Gestion d'un message de confirmation
            var result = System.Windows.Forms.MessageBox.Show("Etes-vous sur de vouloir supprimer les aggrégats?", "Confirmation Suppression", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);

            // Suppression des aggrégats présents sur le cube si confirmation
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                Server srv = new Server();
                srv.Connect(Glb_Nom_Server);
                Database db = srv.Databases.FindByName(Glb_Nom_Database);
                Cube Cube_maj = db.Cubes.FindByName(Glb_Nom_Cube);

                foreach (MeasureGroup Meg in Cube_maj.MeasureGroups)
                {
                    Meg.AggregationDesigns.Clear();
                }
                Cube_maj.Update(UpdateOptions.ExpandFull);
                Cube_maj.Process(ProcessType.ProcessFull);
                srv.Disconnect();
                Tab_Agr[Liste_Cube.SelectedIndex] = "Supprimé";
                Pres_Aggregat.Content = Tab_Agr[Liste_Cube.SelectedIndex];
                Bouton_Aggr.IsEnabled = false;
            }
        }

        //Fonction de connexion à la base
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
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Impossible de se connecter à la base");
            }
            return conn;
        }

        // Fonction de déconnexion à la base
        public static string Deconnexion_Base(string StrConnect)
        {
            String StatusDeconnect = "OK";
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
            //CVA                   "     CUBE_NAME = 'Data Warehouse ODE' AND " +
                               "     CUBE_NAME = '" + cubeId + "' AND " +
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
            
            //CVA : gestion liste des dimensions
            String Liste_dim = "";
            for (int i = 0; i < listDim1D.Count(); i++)
            {
                if (i == 0)
                {
                    Liste_dim = "(OBJECT_ID = '" + listDim1D[i].GetDimensionName() + "' ";
                }
                else
                {
                    if (i == listDim1D.Count() - 1)
                    {
                        Liste_dim = Liste_dim + "OR OBJECT_ID = '" + listDim1D[i].GetDimensionName() + "')";
                    }
                    else
                    {
                        Liste_dim = Liste_dim + "OR OBJECT_ID = '" + listDim1D[i].GetDimensionName() + "' ";
                    }
                }
            }
            // CVA : fin
            //    (OBJECT_ID = 'DIM TEMPS' OR OBJECT_ID = 'DIM LIEUX' OR OBJECT_ID = 'DIM PRODUITS' OR OBJECT_ID = 'DIM CLIENTS')


            cmdPart2.CommandText = "SELECT " +
                                    "   (OBJECT_MEMORY_SHRINKABLE + OBJECT_MEMORY_NONSHRINKABLE + OBJECT_MEMORY_CHILD_SHRINKABLE + OBJECT_MEMORY_CHILD_NONSHRINKABLE) " +
                                    "FROM " +
                                    "    $SYSTEM.DISCOVER_OBJECT_MEMORY_USAGE " +
                                    "WHERE " +
                                    "    OBJECT_TYPE_ID = 100006 AND " +
            //CVA                        "    (OBJECT_ID = 'DIM TEMPS' OR OBJECT_ID = 'DIM LIEUX' OR OBJECT_ID = 'DIM PRODUITS' OR OBJECT_ID = 'DIM CLIENTS')" +
                                    Liste_dim +
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
        //CVA static void GetPoidsCuboides(List<String> cuboides, List<Dimension> listCuboides, List<Dimension> listDim1D, int nb_dim_1d, String datasource, String catalog)
        static void GetPoidsCuboides(List<String> cuboides, List<Dimension> listCuboides, List<Dimension> listDim1D, int nb_dim_1d, AdomdConnection conn)
        {
            // Chaine de connexion SSAS
            //CVA AdomdConnection conn = new AdomdConnection("Data Source=" + datasource + ";Catalog=" + catalog);  // CATALOG : Nom du cube

            // Membres
            DataSet ds;
            DataTable dt;

            // CVA : debut
            // Ouverture de la connexion SSAS
            //conn.Open();
            AdomdRestrictionCollection restrictions = new AdomdRestrictionCollection();
            restrictions.Add("CUBE_NAME", Glb_Nom_Cube);
            //restrictions.Add("COORDINATE", null);
            ds = conn.GetSchemaDataSet("MDSCHEMA_MEASUREGROUP_DIMENSIONS", restrictions);
            //ds = conn.GetSchemaDataSet(AdomdSchemaGuid.MeasureGroupDimensions, null);
            //CVA : fin



            dt = ds.Tables[0];
            String[] test_dimensions = new String[nb_dim_1d * 2];
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
                    //CVA commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i])] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i])] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [" + Glb_Nom_Cube + "]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i])].GetDimensionMemory();
                }
                if (cuboides[i].Length == 2)
                {
                    //CVA commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [" + Glb_Nom_Cube + "]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory();
                }
                if (cuboides[i].Length == 3)
                {
                    //CVA commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [" + Glb_Nom_Cube + "]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(2, 1))].GetDimensionMemory();
                }
                if (cuboides[i].Length == 4)
                {
                    //CVA commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(3, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(3, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [" + Glb_Nom_Cube + "]";
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
                        Console.Write(" | Poids solution " + i + " (" + poids_act + ") < Poids solution " + (i + 1) + " (" + poids_next + ") ET inférieur au seuil (" + seuil_poids + ") => Permutation effectué");
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







