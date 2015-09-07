/****************************************************************************************************************************************************
Nom ......... : OptimiseurODE
Role ........ : Programme de gestion de l'interface de saisie et de traitement de l'optimisation des aggrégations dans le cadre du projet Optimiseur ODE
                Le traitement principal aiguillera sur deux traitements annexes :
                       - Algorithme de Metropolis
                       - Algorithme de Matérialisation
Auteurs ..... : Thomas CHOURREAU  
                Olivier ESSNER    
                Cédric VANDEVORDE 
Version ..... : V1.0 du 04/08/2015
*****************************************************************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Microsoft.AnalysisServices.AdomdClient;
using Microsoft.AnalysisServices;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading;

namespace WpfApplication2
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
    public partial class MainWindow : Window
    {
        /// <summary>
        /// DECLARATION DES GLOBALES ----------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// 
        // Définition des globales utilisées par le programme
        public static string Status_Traitement = "OK";
        public static string Glb_Nom_Server = "";
        public static string Glb_Nom_Database = "";
        public static string Glb_Nom_Cube = "";
        public static double Glb_Size = 0;
        public static bool Thread_en_cours = false;
        public static int Glb_Num_Trait; // 1 : Algo Metropolis / 2 : Algo Materialisation
        public static int Glb_Pct;
        public static List<int> Tab_Agr = new List<int>();
        Thread myThread;
        private delegate void ProgressBarDelegateHandler(int step, string etape);
        private ProgressBarDelegateHandler ProgressBarDelegate;

        public MainWindow()
        {
            InitializeComponent();
            ProgressBarDelegate = new ProgressBarDelegateHandler(UpdateProgressBar);
            InitializeBis();
        }

        /// <summary>
        /// GESTION DES INITIALISATIONS -------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// 
        public void InitializeBis()
        {
            Nom_Server.Text = "";
            Bouton_Connexion.IsEnabled = false;
            Init_Liste_Database();
            Init_Liste_Size();
            Init_Info_Cube();
        }

        public void Init_Liste_Database()
        {
            Liste_Cube.Items.Clear();
        }

        public void Init_Info_Cube()
        {
            Bouton_Aggr.IsEnabled = false;
            Bouton_Algo_1.IsEnabled = false;
            Bouton_Algo_2.IsEnabled = false;
            Cancel.IsEnabled = false;
            Size_MB.IsEnabled = false;
            Size_GB.IsEnabled = false;

            Tab_Agr.Clear();
            Pres_Aggregat.Content = "";
            Size_Util.Content = "";

            Size_MB.SelectedIndex = 0;
            Size_GB.SelectedIndex = 0;

            Barre_Progress.Minimum = 0;
            Barre_Progress.Maximum = 100;
            Barre_Progress.Value = 0;
            Lib_Progress.Content = "";
        }

        public void Init_Liste_Size()
        {
            int i;
            Size_MB.Items.Clear();
            Size_GB.Items.Clear();

            for (i = 0; i < 1024; i++)
            {
                Size_MB.Items.Add(i);
            }

            for (i = 0; i < 200; i++)
            {
                Size_GB.Items.Add(i);
            }
        }

        /// <summary>
        /// GESTION DES EVENEMENTS ------------------------------------------------------------------------------------------------------------------
        /// </summary>
        /// 
        // Gestion de la modification du champs "Nom du server"
        private void Nom_Server_TextChanged(object sender, TextChangedEventArgs e)
        {
            Init_Liste_Database();
            Init_Info_Cube();

            // On rend actif le bouton de connexion si le nom du server est renseigné
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
            Init_Liste_Database();
            Init_Info_Cube();

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
                        int Nb_aggregat = 0;
                        //AVOIR
                        foreach (MeasureGroup Meg in cube.MeasureGroups)
                        {
                            Nb_aggregat = Meg.AggregationDesigns.Count;
                        }
                        Tab_Agr.Add(Nb_aggregat);
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

        //Modification du cube selectionné
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

                Alim_Lib_Agg();

                //Activation des boutons infos cube
                Bouton_Algo_1.IsEnabled = false;
                Bouton_Algo_2.IsEnabled = false;
                Size_MB.IsEnabled = true;
                Size_GB.IsEnabled = true;

                Size_Util.Content = "0 Mo";
                Size_MB.SelectedIndex = 0;
                Size_GB.SelectedIndex = 0;
                Barre_Progress.Value = 0;
                Lib_Progress.Content = "";
            }
        }
        // Alimentation des informations sur les aggrégats du cube
        private void Alim_Lib_Agg()
        {
            // Alimentation des informations sur les aggrégats du cube
            int Nb_aggregat = Tab_Agr[Liste_Cube.SelectedIndex];

            if (Nb_aggregat == 0)
            {
                Pres_Aggregat.Content = "Aucun aggrégat Design présent";
                Bouton_Aggr.IsEnabled = false;
            }
            if (Nb_aggregat == 1)
            {
                Pres_Aggregat.Content = "1 seul aggrégat Design présent";
                Bouton_Aggr.IsEnabled = true;
            }
            if (Nb_aggregat > 1)
            {
                Pres_Aggregat.Content = Nb_aggregat + " aggrégats Design présents";
                Bouton_Aggr.IsEnabled = true;
            }
        }

        // Modification du choix nombre GB
        private void Size_GB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Gestion_Modif_Size();
        }

        // Modification du choix nombre MB
        private void Size_MB_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Gestion_Modif_Size();
        }

        // Traitement commun modification size GB/MB
        public void Gestion_Modif_Size()
        {
            //Alimentation par défaut
            Glb_Size = 0;
            Size_Util.Content = "";

            if ((Size_GB.SelectedIndex != -1) && (Size_MB.SelectedIndex != -1))
            {
                if (Size_GB.SelectedIndex != 0)
                {
                    Size_Util.Content = Size_GB.SelectedValue.ToString() + " Go";
                    double temp = 1024 * 1024;
                    temp = temp * 1024;
                    temp = temp * Size_GB.SelectedIndex;
                    Glb_Size = Glb_Size + temp;
                }

                if (Size_GB.SelectedIndex != 0 & Size_MB.SelectedIndex != 0)
                {
                    Size_Util.Content = Size_Util.Content + " et ";
                }

                if (Size_MB.SelectedIndex != 0)
                {
                    Size_Util.Content = Size_Util.Content + Size_MB.SelectedValue.ToString() + " Mo";
                    double temp = 1024 * 1024;
                    temp = temp * Size_MB.SelectedIndex;
                    Glb_Size = Glb_Size + temp;
                }
            }

            if (Glb_Size > 0)
            {
                Bouton_Algo_1.IsEnabled = true;
                Bouton_Algo_2.IsEnabled = true;
            }
            else
            {
                Size_Util.Content = "0 Mo";
                Bouton_Algo_1.IsEnabled = false;
                Bouton_Algo_2.IsEnabled = false;
            }
        }

        //Gestion bouton de suppression des aggrégations
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
                Tab_Agr[Liste_Cube.SelectedIndex] = 0;
                Pres_Aggregat.Content = "Aggrégat Design supprimé";
                Bouton_Aggr.IsEnabled = false;
            }
        }

        //Gestion bouton algorithme de Metropolis
        private void Bouton_Algo_1_Click(object sender, RoutedEventArgs e)
        {
            Thread_en_cours = !Thread_en_cours;
            Glb_Num_Trait = 1;
            myThread = new Thread(new ThreadStart(ThreadPrinc));
            myThread.Start();
        }

        //Gestion bouton algorithme de Matérialisation
        private void Bouton_Algo_2_Click(object sender, RoutedEventArgs e)
        {
            Thread_en_cours = !Thread_en_cours;
            Glb_Num_Trait = 2;
            myThread = new Thread(new ThreadStart(ThreadPrinc));
            myThread.Start();
        }

        //Gestion bouton Cancel
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Arrêt du Thread en cours (algo 1 ou algo 2 en cours)
            myThread.Abort();
            Barre_Progress.Value = 100;
            Lib_Progress.Content = "Traitement annulé par l'utilisateur";
            Thread_en_cours = !Thread_en_cours;
            Deblocage_Zones();
        }

        //Blocage de l'ensemble des zones saisissables et activation bouton cancel
        public void Blocage_Zones()
        {
            Nom_Server.IsEnabled = false;
            Bouton_Connexion.IsEnabled = false;
            Liste_Cube.IsEnabled = false;
            Bouton_Aggr.IsEnabled = false;
            Bouton_Algo_1.IsEnabled = false;
            Bouton_Algo_2.IsEnabled = false;
            Size_MB.IsEnabled = false;
            Size_GB.IsEnabled = false;

            Cancel.IsEnabled = true;
        }

        //Deblocage de l'ensemble des zones saisissables et désactivation bouton cancel
        public void Deblocage_Zones()
        {
            Nom_Server.IsEnabled = true;
            Bouton_Connexion.IsEnabled = true;
            Liste_Cube.IsEnabled = true;
            Alim_Lib_Agg();

            Bouton_Algo_1.IsEnabled = true;
            Bouton_Algo_2.IsEnabled = true;
            Size_MB.IsEnabled = true;
            Size_GB.IsEnabled = true;

            Cancel.IsEnabled = false;
        }

        // Gestion fermeture de l'application
        private void Principal_Closing(object sender, CancelEventArgs e)
        {
            //On doit arreter le Thread en cours si necessaire
            if (Thread_en_cours)
            {
                myThread.Abort();
            }
        }

        /// <summary>
        /// THREAD PRINCIPAL EN CAS DE LANCEMENT ALGO 1 OU 2 ----------------------------------------------------------------------------------------
        ///  </summary>
        /// 
        private void ThreadPrinc()
        {
            DateTime exeStart = DateTime.Now;

            //Etape : Initialisation
            Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 0, "Initialisation" });
            Glb_Pct = 0;

            //Etape : Connexion à la base
            Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 5, "Connexion base" });
            string StrConnexion = "Datasource=" + Glb_Nom_Server + ";Catalog=" + Glb_Nom_Database + ";";
            AdomdConnection conn = Connexion_Base(StrConnexion);

            if (Status_Traitement != "OK")
            {
                return;
            }

            // Instance de classe de dimensions 1D
            List<Dimension> listDim1D = new List<Dimension>();

            //Etape : Liste de toutes les dimensions 1D
            Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 5, "Liste dimensions 1D" });
            GetDimension1DProperties(conn, Glb_Nom_Cube, listDim1D);

            //Etape : Récupération du treillis des cuboïdes
            Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 5, "Récupération du treillis des cuboïdes" });
            List<Dimension> listCuboides = new List<Dimension>(); // liste des cuboïdes
            List<String> index_cuboides = new List<String>(); // liste des index des cuboïdes : plus facile à utiliser par la suite
            String prefix_index = "";
            GetCombinaisons(listDim1D, 0, "", 0, listCuboides, index_cuboides, prefix_index); // appel de la fonction qui crée les combinaisons

            //Etape : Récupération du nombre de lignes des cuboïdes
            Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 5, "Récupération du nombre de lignes des cuboïdes" });
            GetPoidsCuboides(index_cuboides, listCuboides, listDim1D, listDim1D.Count, conn); // appel de la fonction qui récupère le nombre de lignes des cuboides

            //Etape : Algorithme de Metropolis ou de Matérialisation
            double seuil_poids = Glb_Size;
            int[] solution = new int[listCuboides.Count]; // la solution que va retrouner Metropolis sous forme de 0 et de 1
            switch (Glb_Num_Trait)
            {
                case 1:
                    Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 10, "Algorithme de Metropolis" });
                    int nb_boucle = 100; // le nombre de boucle que l'on veut faire faire à l'algo de Metropolis
                    Metropolis(listCuboides, seuil_poids, nb_boucle, solution); // appel de l'algo de Metropolis
                    break;
                case 2:
                    Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 10, "Algorithme de Matérialisation" });
                    MaterialisationPartielle(listCuboides, seuil_poids, solution); // appel de l'algo de Matérialisation
                    break;
                default:
                    break;
            }

            //Etape : Deconnexion à la base
            Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 30, "Deconnexion base" });
            Deconnexion_Base(StrConnexion);

            //Etape : Création aggrégat Design en fonction de la solution
            Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 10, "Création Aggrégat" });
            int nb_aggregats = CreateAgg(solution, listCuboides);

            TimeSpan exeDuration = DateTime.Now.Subtract(exeStart);
            string exeTime = string.Format(" : {0}h, {1}m, {2}s et {3}ms",exeDuration.Hours,exeDuration.Minutes,exeDuration.Seconds,exeDuration.Milliseconds);
            
            //Etape : Fin du traitement
            Dispatcher.Invoke(this.ProgressBarDelegate, new object[] { 30, "Traitement terminé avec succès en " + exeTime + " - Création d'un aggrégat Design et " + nb_aggregats + " aggrégats" });
            
            Thread_en_cours = !Thread_en_cours;
        }

        //Fonction de mise à jour de la barre de progression hors du thread
        private void UpdateProgressBar(int step, string etape)
        {
            // Mise à jour de la barre de progression en fonction de l'étape
            Glb_Pct = Glb_Pct + step;
            Barre_Progress.Value = Glb_Pct;

            switch (Glb_Pct)
            {
                case 0:
                    //Blocage des zones saisissables - Debut du traitement
                    Blocage_Zones();
                    break;
                case 100:
                    //Deblocage des zones saisissables - Fin du traitement
                    Tab_Agr[Liste_Cube.SelectedIndex] = Tab_Agr[Liste_Cube.SelectedIndex] + 1;
                    Deblocage_Zones();
                    break;
                default:
                    break;
            }
            Lib_Progress.Content = Glb_Pct + "%" + " - " + etape;
        }

        /// <summary>
        /// FONCTIONS UTILISEES ---------------------------------------------------------------------------------------------------------------------
        ///  </summary>
        ///
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

        // Exploration DMV de SSAS (Vues prédefinies sur tables systeme)
        static void GetDimension1DProperties(AdomdConnection adoConnect, string cubeId, List<Dimension> listDim1D)
        {
            AdomdCommand cmdPart1 = adoConnect.CreateCommand();
            AdomdCommand cmdPart2 = adoConnect.CreateCommand();

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

            cmdPart2.CommandText = "SELECT " +
                                    "   (OBJECT_MEMORY_SHRINKABLE + OBJECT_MEMORY_NONSHRINKABLE + OBJECT_MEMORY_CHILD_SHRINKABLE + OBJECT_MEMORY_CHILD_NONSHRINKABLE) " +
                                    "FROM " +
                                    "    $SYSTEM.DISCOVER_OBJECT_MEMORY_USAGE " +
                                    "WHERE " +
                                    "    OBJECT_TYPE_ID = 100006 AND " +
                                    Liste_dim +
                                    "ORDER BY OBJECT_ID";

            // Ouverture du reader XML
            AdomdDataReader readerPart2 = cmdPart2.ExecuteReader();

            // Enregistrement des données dans des instances de Dimension
            for (int i = 0; i < listDim1D.Count(); i++)
            {
                readerPart2.Read();

                // 3eme retour : [TOTAL_MEM_SIZE]
                if (listDim1D[i].GetDimensionCount() != 0)
                {
                    listDim1D[i].SetDimensionMemory((int)(readerPart2.GetInt32(0) / listDim1D[i].GetDimensionCount()));
                }
                else
                {
                    listDim1D[i].SetDimensionMemory(0);
                }
            }

            // Fermeture du reader XML
            readerPart2.Close();
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
        static void GetPoidsCuboides(List<String> cuboides, List<Dimension> listCuboides, List<Dimension> listDim1D, int nb_dim_1d, AdomdConnection conn)
        {
            // Membres
            DataSet ds;
            DataTable dt;

            AdomdRestrictionCollection restrictions = new AdomdRestrictionCollection();
            restrictions.Add("CUBE_NAME", Glb_Nom_Cube);
            ds = conn.GetSchemaDataSet("MDSCHEMA_MEASUREGROUP_DIMENSIONS", restrictions);
            dt = ds.Tables[0];
            String[] test_dimensions = new String[nb_dim_1d * 2];
            int cpt = 0;
            int poids_une_ligne = 0;

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                    if (col.ColumnName == "DIMENSION_GRANULARITY")
                    {
                        test_dimensions[cpt] = row[col].ToString(); // récupération des clés primaire pour faire les requêtes MDX ensuite
                        cpt = cpt + 1;
                    }
            }

            int poids;
            String commandText = "";
            for (int i = 0; i < cuboides.Count; i++)
            {
                poids = 0;
                // on lance une requête MDX selon le nombre de dimensions à croiser /!\ : 4 maximum pour le moment /!\
                if (cuboides[i].Length == 1)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i])] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [" + Glb_Nom_Cube + "]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i])].GetDimensionMemory();
                }
                if (cuboides[i].Length == 2)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [" + Glb_Nom_Cube + "]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory();
                }
                if (cuboides[i].Length == 3)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [" + Glb_Nom_Cube + "]";
                    poids_une_ligne = listDim1D[int.Parse(cuboides[i].Substring(0, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(1, 1))].GetDimensionMemory() + listDim1D[int.Parse(cuboides[i].Substring(2, 1))].GetDimensionMemory();
                }
                if (cuboides[i].Length == 4)
                {
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

                    listCuboides[i].SetDimensionCount(poids); // on affecte le nombre de lignes au cuboide
                    listCuboides[i].SetDimensionMemory(poids_une_ligne);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

		/* ************************** */
        /* TO DO : passage en WS JAVA */
        /* ************************** */
		
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

		// -- Olivier # 07/09/2015

		/* ************************** */
        /* TO DO : passage en WS JAVA */
        /* ************************** */
		
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
        // -- Fin Olivier # 07/09/2015
   
   
   
   		/* ************************** */
        /* TO DO : passage en WS JAVA */
        /* ************************** */
		
		// Fonction de création des aggrgéats
		static int CreateAgg(int[] solution, List<Dimension> listCuboides)
        {
            int indice = 0;
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
                ad.FinalizeDesign();
                ad.Aggregations.Clear();

                for (int i = 0; i < solution.Length; i++)
                {
                    if (solution[i] == 1)
                    {
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

                //Mise à jour des liens des partitions
                foreach (Partition part in Meg.Partitions)
                {
                    part.AggregationDesignID = ad.ID;
                }
                Cube_maj.Update(UpdateOptions.ExpandFull);
                Cube_maj.Process(ProcessType.ProcessFull);
                srv.Disconnect();
            }
            return indice;
        }
    }
}

