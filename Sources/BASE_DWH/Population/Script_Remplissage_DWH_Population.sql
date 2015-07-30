/*======================================================================================
  
  Fichier:     Script_Remplissage_DWH_Population.sql

  Résumé:  Rempli la colonne [ODE_DATAWAREHOUSE].[DIM_VILLES].[POPULATION] du projet ODE
  Date:     30/07/2015
  Updated:  

  SQL Server Version: 2014
  
=======================================================================================*/


/*
-- Table de Dimension "Villes" 
[ODE_DATAWAREHOUSE].[DIM_VILLES]
 	[VILLE_PK]
 	[CODE_POSTAL]
 	[CODE_COMMUNE]
 	[CODE_REGION]
 	[CODE_DEPARTEMENT] 
 	[CODE_ARRONDISEMENT]
 	[CODE_CANTON]
 	[NOM_VILLE_MAJ]
 	[NOM_VILLE_MIN]
 	[POPULATION]

*/

-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit


-- Connexion à la base
USE [DataWarehouseODE];
GO


-- Mise en variable du PATH vers le répertoire contenant les CSV à charger
:setvar OdeCsvPath "Z:\GitHub\Projet_ODE\Sources\BASE_DWH\Population\"


-- ----------------
-- MAJ TABLE VILLES
-- ----------------

-- Création de tables temporaires pour charger les fichiers csv
CREATE TABLE [ODE_DATAWAREHOUSE].[VILLES_POPULATION](
 	[CODE_COMMUNE]			[INT]					NOT NULL, 
 	[CODE_REGION]			[INT]					NOT NULL, 
 	[CODE_DEPARTEMENT]		[NVARCHAR](3)			NOT NULL, -- Olivier # 30/07/2015 : Merci la Corse !
 	[CODE_ARRONDISEMENT]	[INT]					NOT NULL, 
 	[CODE_CANTON]			[INT]					NOT NULL, 
 	[POPULATION]			[INT]					NOT NULL
) ON [PRIMARY]; 


BULK INSERT [ODE_DATAWAREHOUSE].[VILLES_POPULATION] FROM N'$(OdeCsvPath)PopulationINSEE.csv' -- chargement du fichier csv dans la table
WITH (
    CODEPAGE='1252', -- CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- Fin de lignes des CSV en LF seul (/n <=> CR + LF)
);

--Remplissage de la table catégories à partir des tables temporaires créées précedemment

UPDATE [ODE_DATAWAREHOUSE].[DIM_VILLES]
SET [POPULATION] = (
	SELECT MAX(V.[POPULATION]) -- Olivier # 30/07/2015 : Sécurité en cas de correspondance multiple
	FROM		
		[ODE_DATAWAREHOUSE].[VILLES_POPULATION] AS V
	WHERE	
		V.[CODE_COMMUNE] 		= [ODE_DATAWAREHOUSE].[DIM_VILLES].[CODE_COMMUNE] AND
		V.[CODE_REGION] 		= [ODE_DATAWAREHOUSE].[DIM_VILLES].[CODE_REGION] AND
		V.[CODE_DEPARTEMENT] 	= [ODE_DATAWAREHOUSE].[DIM_VILLES].[CODE_DEPARTEMENT] AND
		V.[CODE_ARRONDISEMENT] 	= [ODE_DATAWAREHOUSE].[DIM_VILLES].[CODE_ARRONDISEMENT]
		-- V.[CODE_CANTON] 		= [ODE_DATAWAREHOUSE].[DIM_VILLES].[CODE_CANTON]
);


-- Villes torp petites : Aléatoirement entre 1 et 50 pelerins...
UPDATE [ODE_DATAWAREHOUSE].[DIM_VILLES]
SET [POPULATION] = cast( floor(50 * rand() + 1) as INT)
WHERE [POPULATION] IS NULL;

-- Drop de la table temporaire
DROP TABLE [ODE_DATAWAREHOUSE].[VILLES_POPULATION]
GO