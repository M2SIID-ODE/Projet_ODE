using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AnalysisServices.AdomdClient;



namespace Cuboïdes
{
    class Program
    {
        static void Main(string[] args)
        {


            // Chaine de connexion SSAS
            AdomdConnection conn = new AdomdConnection("Data Source=localhost;Catalog= Cube_consolide");  // CATALOG : Nom du cube

            // Membres
            DataSet ds;
            DataTable dt;

            // Ouverture de la connexion SSAS
            conn.Open();

            ds = conn.GetSchemaDataSet(AdomdSchemaGuid.MeasureGroupDimensions, null);
            dt = ds.Tables[0];

            String[] test_dimensions = new String[10];
            int cpt = 0;

            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn col in dt.Columns)
                    if (col.ColumnName == "DIMENSION_GRANULARITY")
                    {
                        test_dimensions[cpt] = row[col].ToString();
                        cpt = cpt + 1;
                        Console.WriteLine(col.ColumnName + " = " + row[col].ToString());
                    }

                Console.WriteLine();
            }


            //String[] dimensions = { "[DIM LIEUX].[LIEU PK]", "[DIM PRODUITS].[PRODUIT PK]", "[DIM CLIENTS].[CLIENT PK]", "[DIM TEMPS].[TEMPS PK]"};
            String[] cuboides = { "01", "123", "23", "3", "013" };
            int[] poids_cuboides = { 0, 0, 0, 0, 0 };
            int poids;
            String commandText="";

            //String commandText = "SELECT ({ CROSSJOIN ({([DIM LIEUX].[LIEU PK].Members)},{([DIM PRODUITS].[PRODUIT PK].Members)})}) ON COLUMNS,{([DIM CLIENTS].[CLIENT PK].Members)} on ROWS  FROM [Data Warehouse ODE]";

            //String commandText = "SELECT ({ NONEMPTYCROSSJOIN ({([DIM CLIENTS].[CLIENT PK].Members)},{([DIM LIEUX].[LIEU PK].Members)},{([DIM PRODUITS].[PRODUIT PK].Members)})}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";

            for (int i = 0; i < cuboides.Length; i++)

            {

                poids = 0;

                Console.WriteLine("NB Cuboides : " + cuboides.Length);

                Console.WriteLine("Cuboides : " + cuboides[i] + " taille = " + cuboides[i].Length);

                if (cuboides[i].Length == 1)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i])] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                }
                if (cuboides[i].Length == 2)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                }
                if (cuboides[i].Length == 3)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                }
                if (cuboides[i].Length == 4)
                {
                    commandText = "SELECT ({ NONEMPTYCROSSJOIN ((NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(0, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(1, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(2, 1))] + ".Members)}),NONEMPTY({(" + test_dimensions[int.Parse(cuboides[i].Substring(3, 1))] + ".Members)})))}) ON ROWS, ([Measures].[UNITES VENDUES]) on COLUMNS  FROM [Data Warehouse ODE]";
                }


                try
                {
                    AdomdCommand cmd = new AdomdCommand(commandText, conn);
                    // Si requete MDX fausse : Crashe ici. Cliquer sur "VIEW DETAILS" de la pop-up de debuggage pour voir le retour de SSAS... A ameliorer avec un TRY..CATCH !
                    CellSet cs = cmd.ExecuteCellSet();

                    // Iterations sur toutes les lignes et les colonnes
                    foreach (Position pRow in cs.Axes[1].Positions)
                    {
                        foreach (Position pCol in cs.Axes[0].Positions)
                        {

                            // Recuperation des valeurs formatées pour cette cellule
                            //Console.Write(cs[pCol.Ordinal, pRow.Ordinal].FormattedValue + ", ");
                            poids = poids + 1;
                        }
                        //Console.WriteLine();
                    }


                    Console.WriteLine("Poids cuboide " + cuboides[i] + " = " + poids);
                    poids_cuboides[i] = poids;
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }

            }
            Console.ReadKey();
        }
    }
}
