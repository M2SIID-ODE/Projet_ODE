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
                            {   ad.Dimensions.Add(dim.ID);  }

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
                

                // Partie 2 - Algo Spécifique Thomas
                

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
                // Partie 1 - Commune


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







