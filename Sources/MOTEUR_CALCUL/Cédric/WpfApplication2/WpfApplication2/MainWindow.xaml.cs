/* 
A VOIR ................
pour les variables, voir on ne peut pas utiliser un fichier de configuration http://www.techheadbrothers.com/Articles.aspx/adomd-net-bases-requeteur-olap-multidimensionnel-page-3
http://www.developpez.net/forums/d989898/logiciels/solutions-d-entreprise/business-intelligence/microsoft-bi/ssas/2k8-extraire-storagemode-cube/


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
//using Microsoft.SqlServer.Management.Common;
//using Microsoft.SqlServer.Management.Sdk;
//using Microsoft.SqlServer.Management.Smo;
//using System.Data.SqlClient;
//using System.Data;
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
                        Liste_Cube.Items.Add(conn.Cubes[i].Name);
                    }
                }
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







