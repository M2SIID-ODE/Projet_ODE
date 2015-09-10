/*============================================================================
  
  Fichier:     Script_Remplissage_DWH.sql

  Résumé:  Rempli le DWH (OLTP) du projet ODE
  Date:     26/07/2015
  Updated:  23/08/2015

  SQL Server Version: 2014
  
============================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit


-- Connexion à la base
USE [DataWarehouseODE];
GO

 
-- OLIVIER # 13/07/2015 : Mise en variable du PATH vers le répertoire contenant les CSV à charger
:setvar OdeCsvPath "C:\CONSOLIDATION\Donnees\"



-- OLIVIER # 13/07/2015
-- ----------------------------------
-- ETAPE 0 : RAZ DE TOUTES LES TABLES
-- ----------------------------------
DELETE FROM [ODE_DATAWAREHOUSE].[FACT_VENTES];
DELETE FROM [ODE_DATAWAREHOUSE].[FACT_STOCKS];
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_LIEUX];
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_CLIENTS];
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_VILLES];
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_PRODUITS];
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_TEMPS];
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_CATEGORIES];  -- Olivier # 25/07/2015 : Ajout pour DIM_CATEGORIES

GO


DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_LIEUX]',		RESEED, 1)	WITH NO_INFOMSGS
DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_CLIENTS]',	RESEED, 1)	WITH NO_INFOMSGS
DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_VILLES]',		RESEED, 1)	WITH NO_INFOMSGS
DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_PRODUITS]',	RESEED, 1)	WITH NO_INFOMSGS
DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_CATEGORIES]',	RESEED, 1)	WITH NO_INFOMSGS  -- Olivier # 25/07/2015 : Ajout pour DIM_CATEGORIES

DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_LIEUX]',		RESEED)	WITH NO_INFOMSGS -- Olivier # 26/07/2015 : http://blog.sqlauthority.com/2007/03/15/sql-server-dbcc-reseed-table-identity-value-reset-table-identity/#comment-2406
DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_CLIENTS]',	RESEED)	WITH NO_INFOMSGS
DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_VILLES]',		RESEED)	WITH NO_INFOMSGS
DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_PRODUITS]',	RESEED)	WITH NO_INFOMSGS
DBCC CHECKIDENT('[ODE_DATAWAREHOUSE].[DIM_CATEGORIES]',	RESEED)	WITH NO_INFOMSGS  

GO


-- --------------------------------------
-- ETAPE 1 : REMPLISSAGE TABLE CATEGORIES
-- --------------------------------------

-- BRICE # 19/07/2015

-- Création de tables temporaires pour charger les fichiers csv
create table [ODE_DATAWAREHOUSE].[UNIVERS](  -- on crée une table pour charger ensuite les fichiers CSV
	LIB_UNIVERS varchar(50),
	UNIVERS_PK int,
)

BULK INSERT [ODE_DATAWAREHOUSE].[UNIVERS] FROM N'$(OdeCsvPath)Univers.csv' -- chargement du fichier csv dans la table
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    CODEPAGE='1252', -- OLIVIER # 20/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 20/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
);

create table [ODE_DATAWAREHOUSE].[RAYONS](
	UNIVERS_RAY int,
	LIB_RAYONS varchar(70),
	RAYONS_PK int,
)

BULK INSERT [ODE_DATAWAREHOUSE].[RAYONS] FROM N'$(OdeCsvPath)Rayons.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    CODEPAGE='1252', -- OLIVIER # 20/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 20/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
);

create table [ODE_DATAWAREHOUSE].[FAMILLES](
	RAYONS_FAM int,
	LIB_FAMILLES varchar(70),
	FAMILLES_PK int,
)

BULK INSERT [ODE_DATAWAREHOUSE].[FAMILLES] FROM N'$(OdeCsvPath)Familles.csv'
WITH (
    --CHECK_CONSTRAINTS,
    CODEPAGE='1252', -- OLIVIER # 20/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 20/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
);

create table [ODE_DATAWAREHOUSE].[SOUS_FAMILLES](
	FAMILLES_SFAM int,
	LIB_SOUS_FAMILLES varchar(70),
	SOUS_FAMILLES_PK int,
)

BULK INSERT [ODE_DATAWAREHOUSE].[SOUS_FAMILLES] FROM N'$(OdeCsvPath)Sous_familles.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    --DATAFILETYPE='char',
    CODEPAGE='1252', -- OLIVIER # 20/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 20/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
);

--Remplissage de la table catégories à partir des tables temporaires créées précedemment

	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_CATEGORIES](
		[LIBEL_UNIVERS],
		[ID_UNIVERS],
		[LIBEL_RAYON],
		[ID_RAYON],
		[LIBEL_FAMILLE],
		[ID_FAMILLE],
		[LIBEL_SSFAMILLE],
		[ID_SSFAMILLE])
	SELECT
		LIB_UNIVERS,		-- [LIBEL_UNIVERS]
		UNIVERS_PK,			-- [ID_UNIVERS]
		LIB_RAYONS,			-- [LIBEL_RAYON]
		RAYONS_PK,			-- [ID_RAYON]
		LIB_FAMILLES,		-- [LIBEL_FAMILLE]
		FAMILLES_PK,		-- [ID_FAMILLE]
		LIB_SOUS_FAMILLES,	-- [LIBEL_SSFAMILLE]
		SOUS_FAMILLES_PK	-- [ID_SSFAMILLE]
	FROM		
					[ODE_DATAWAREHOUSE].[UNIVERS] U
		LEFT JOIN	[ODE_DATAWAREHOUSE].[RAYONS] R			ON U.[UNIVERS_PK] = R.[UNIVERS_RAY]
		LEFT JOIN	[ODE_DATAWAREHOUSE].[FAMILLES] F		ON R.[RAYONS_PK] = F.[RAYONS_FAM]
		LEFT JOIN	[ODE_DATAWAREHOUSE].[SOUS_FAMILLES] S	ON F.[FAMILLES_PK] = S.[FAMILLES_SFAM];



-- ------------------------------------
-- ETAPE 2 : REMPLISSAGE TABLE PRODUITS
-- ------------------------------------

-- THOMAS # 09/07/2015


-- REMPLISSAGE TABLE PRODUITS

-- CREATION BIBLIOTHEQUE

create table [ODE_DATAWAREHOUSE].[PRODUITS](
	RAYONS_PDT int,
	LIB_PRODUIT nvarchar(70),
)

BULK INSERT [ODE_DATAWAREHOUSE].[PRODUITS] FROM N'$(OdeCsvPath)Produits.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    CODEPAGE='1252', -- OLIVIER # 16/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 16/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
);

create table [ODE_DATAWAREHOUSE].[FOURNISSEURS](
	UNIVERS_FOUR int,
	LIB_FOURNISSEUR nvarchar(70),
)

BULK INSERT [ODE_DATAWAREHOUSE].[FOURNISSEURS] FROM N'$(OdeCsvPath)Fournisseurs.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    CODEPAGE='1252', -- OLIVIER # 16/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 16/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
);

create table [ODE_DATAWAREHOUSE].[MARQUES](
	RAYONS_MAR int,
	LIB_MARQUE nvarchar(70),
)

BULK INSERT [ODE_DATAWAREHOUSE].[MARQUES] FROM N'$(OdeCsvPath)Marques.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    CODEPAGE='1252', -- OLIVIER # 16/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 16/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
);

-- Transformation pour créer 1 million de produits (24 240 * 40)
-- Olivier # 07/09/2015

DECLARE @i INT = 0

-- on crée une table temporaire grâce aux jointures (left join) pour obtenir un "catalogue" de 24 240 références différentes
	SELECT ROW_NUMBER() OVER(ORDER BY NEWID()) as id,
		SOUS_FAMILLES.SOUS_FAMILLES_PK as categorie,
		SOUS_FAMILLES.LIB_SOUS_FAMILLES + ' ' + PRODUITS.LIB_PRODUIT + '_' + cast(@i as varchar) as Lib_prod , -- on crée le champ du libellé du produit (tant qu on y est) en concaténant le libellé de la sous-famille avec la spécification du produit -- Bernard 10/09/2015
		MARQUES.LIB_MARQUE as marque,
		FOURNISSEURS.LIB_FOURNISSEUR as fournisseur
	INTO [ODE_DATAWAREHOUSE].[TABLE_TEMP]
	FROM [ODE_DATAWAREHOUSE].[UNIVERS]
		LEFT JOIN [ODE_DATAWAREHOUSE].[RAYONS] ON UNIVERS.UNIVERS_PK = RAYONS.UNIVERS_RAY
		LEFT JOIN [ODE_DATAWAREHOUSE].[FOURNISSEURS] ON UNIVERS.UNIVERS_PK = FOURNISSEURS.UNIVERS_FOUR
		LEFT JOIN [ODE_DATAWAREHOUSE].[FAMILLES] ON RAYONS.RAYONS_PK = FAMILLES.RAYONS_FAM
		LEFT JOIN [ODE_DATAWAREHOUSE].[MARQUES] ON RAYONS.RAYONS_PK = MARQUES.RAYONS_MAR
		LEFT JOIN [ODE_DATAWAREHOUSE].[PRODUITS] ON RAYONS.RAYONS_PK = PRODUITS.RAYONS_PDT
		LEFT JOIN [ODE_DATAWAREHOUSE].[SOUS_FAMILLES] ON FAMILLES.FAMILLES_PK = SOUS_FAMILLES.FAMILLES_SFAM
	ORDER BY NEWID()
	
	SET @i = 1 + @i;

WHILE @i < 40
BEGIN
	-- on insert les enrigistrements dans la  table temporaire grâce aux jointures (left join) pour obtenir un "catalogue" de 24 240 références différentes -- Bernard - 10/09/2015
	INSERT INTO [ODE_DATAWAREHOUSE].[TABLE_TEMP]
           ([id]
           ,[categorie]
           ,[Lib_prod]
           ,[marque]
           ,[fournisseur])
	(
	SELECT ROW_NUMBER() OVER(ORDER BY NEWID()) as id,
		SOUS_FAMILLES.SOUS_FAMILLES_PK as categorie,
		SOUS_FAMILLES.LIB_SOUS_FAMILLES + ' ' + PRODUITS.LIB_PRODUIT + '_' + cast(@i as varchar) as Lib_prod , -- on crée le champ du libellé du produit (tant qu on y est) en concaténant le libellé de la sous-famille avec la spécification du produit
		MARQUES.LIB_MARQUE as marque,
		FOURNISSEURS.LIB_FOURNISSEUR as fournisseur
		FROM [ODE_DATAWAREHOUSE].[UNIVERS]
		LEFT JOIN [ODE_DATAWAREHOUSE].[RAYONS] ON UNIVERS.UNIVERS_PK = RAYONS.UNIVERS_RAY
		LEFT JOIN [ODE_DATAWAREHOUSE].[FOURNISSEURS] ON UNIVERS.UNIVERS_PK = FOURNISSEURS.UNIVERS_FOUR
		LEFT JOIN [ODE_DATAWAREHOUSE].[FAMILLES] ON RAYONS.RAYONS_PK = FAMILLES.RAYONS_FAM
		LEFT JOIN [ODE_DATAWAREHOUSE].[MARQUES] ON RAYONS.RAYONS_PK = MARQUES.RAYONS_MAR
		LEFT JOIN [ODE_DATAWAREHOUSE].[PRODUITS] ON RAYONS.RAYONS_PK = PRODUITS.RAYONS_PDT
		LEFT JOIN [ODE_DATAWAREHOUSE].[SOUS_FAMILLES] ON FAMILLES.FAMILLES_PK = SOUS_FAMILLES.FAMILLES_SFAM
	
	)
	SET @i = 1 + @i;
END



-- REMPLISSAGE TABLES
-- on crée les variables qui vont bien
declare @nb_lignes_2 int = 10
declare @compteur_2 int = 0

declare @categorie_fk int
declare @lib_pdt nvarchar(256)
declare @prix_achat money
declare @type_tva int
declare @taux_tva decimal(4,1)
declare @marque nvarchar(256)
declare @grossiste nvarchar(256)
declare @id int


WHILE @compteur_2 < @nb_lignes_2 -- on lance une boucle while pour remplir notre table TABLE_TEMP ?
BEGIN
	SET @categorie_fk = (SELECT TOP 1 categorie FROM [ODE_DATAWAREHOUSE].[TABLE_TEMP]) --on récupère les champs qui nous intéresse
	SET @lib_pdt = (SELECT TOP 1 Lib_prod FROM [ODE_DATAWAREHOUSE].[TABLE_TEMP])
	SET @id = (SELECT TOP 1 id FROM [ODE_DATAWAREHOUSE].[TABLE_TEMP])
	
	-- OLIVIER # 13/07/2015 : Est ce que le TOP 1 séparé en 3 va lire la meme ligne de DIM_CATEGORIE ?
	
	SET @prix_achat = (select cast((1000-1)* rand() + 1 as money)) --on crée un prix d achat aléatoire en 1€ et 1000€
	SET @type_tva = (select cast(round((4-1)* rand() + 1,0) as integer)) --on choisit aléatoirement un taux de TVA parmi les 4 valeurs en application en France (pas crédible en réalité car quasiment tous les produits doivent être à 20%)
	IF @type_tva = 1
		SET @taux_tva = 20
	IF @type_tva = 2
		SET @taux_tva = 10
	IF @type_tva = 3
		SET @taux_tva = 5.5
	IF @type_tva = 4
		SET @taux_tva = 2.1
	SET @marque = (SELECT TOP 1 marque FROM [ODE_DATAWAREHOUSE].[TABLE_TEMP])
	SET @grossiste = (SELECT TOP 1 fournisseur FROM [ODE_DATAWAREHOUSE].[TABLE_TEMP])



	-- OLIVIER # 15/07/2015 : ERREUR due à l absence de lien avec la table des CATEGORIES
	--		Msg 547, Niveau 16, État 0, Ligne 213
	--		L'instruction INSERT est en conflit avec la contrainte FOREIGN KEY "FK_DimProduits_DimCategories_FK". Le conflit s'est produit dans la base de données "DataWarehouseODE", table "ODE_DATAWAREHOUSE.DIM_CATEGORIES", column 'CATEGORIE_PK'.
	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_PRODUITS](CATEGORIE_FK,LIBEL_PRODUIT,PRIX_ACHAT,TAUX_TVA,MARQUE_PRODUIT,GROSSISTE_PRODUIT) --on insère les valeurs dans la table
	VALUES (@categorie_fk,
		@lib_pdt,
		@prix_achat,
		@taux_tva,
		@marque,
		@grossiste)
	
	DELETE FROM [ODE_DATAWAREHOUSE].[TABLE_TEMP] WHERE id = @id -- on drop la ligne pour pas réinsérer exactement la mêm référence

	SET @compteur_2 = @compteur_2 + 1
END

DROP TABLE [ODE_DATAWAREHOUSE].[TABLE_TEMP] --on supprime la table temporaire

-- OLIVIER # 13/07/2015 : Drop des autres tables intermediares
DROP TABLE [ODE_DATAWAREHOUSE].[UNIVERS]
DROP TABLE [ODE_DATAWAREHOUSE].[RAYONS]
DROP TABLE [ODE_DATAWAREHOUSE].[FOURNISSEURS]
DROP TABLE [ODE_DATAWAREHOUSE].[FAMILLES]
DROP TABLE [ODE_DATAWAREHOUSE].[MARQUES]
DROP TABLE [ODE_DATAWAREHOUSE].[PRODUITS]
DROP TABLE [ODE_DATAWAREHOUSE].[SOUS_FAMILLES];
GO


-- ----------------------------------
-- ETAPE 3 : REMPLISSAGE TABLE VILLES
-- ----------------------------------

-- BERNARD # 09/07/2015

BULK INSERT [ODE_DATAWAREHOUSE].[DIM_VILLES] FROM N'$(OdeCsvPath)Script_villes_france.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    CODEPAGE='1252', -- OLIVIER # 16/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 16/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
)
GO


-- ------------------
-- Olivier 04/08/2015
-- MAJ TABLE VILLES
-- ------------------

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
);


-- Villes trop petites : Aléatoirement entre 1 et 50 pelerins...
UPDATE [ODE_DATAWAREHOUSE].[DIM_VILLES]
SET [POPULATION] = cast( floor(50 * rand() + 1) as INT)
WHERE [POPULATION] IS NULL;

-- Drop de la table temporaire
DROP TABLE [ODE_DATAWAREHOUSE].[VILLES_POPULATION]
GO


-- ------------------
-- Olivier 23/08/2015
-- MAJ TABLE VILLES
-- ------------------

-- Création de tables temporaires pour charger les fichiers csv
CREATE TABLE [ODE_DATAWAREHOUSE].[VILLES_REGIONS](
 	[CODE_REGION]			[INT]					NOT NULL, 
 	[NOM_REGION_MAJ]		[NVARCHAR](50)			NOT NULL,
 	[NOM_REGION_MIN]		[NVARCHAR](50)			NOT NULL,
) ON [PRIMARY]; 


BULK INSERT [ODE_DATAWAREHOUSE].[VILLES_REGIONS] FROM N'$(OdeCsvPath)RegionsINSEE.csv' -- chargement du fichier csv dans la table
WITH (
    CODEPAGE='1252', -- CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- Fin de lignes des CSV en LF seul (/n <=> CR + LF)
);

--Remplissage de la table catégories à partir des tables temporaires créées précedemment

UPDATE [ODE_DATAWAREHOUSE].[DIM_VILLES]
SET 
	[NOM_REGION_MAJ] = V.[NOM_REGION_MAJ],
	[NOM_REGION_MIN] = V.[NOM_REGION_MIN]
FROM		
	[ODE_DATAWAREHOUSE].[VILLES_REGIONS] AS V
WHERE	
	V.[CODE_REGION] = [DIM_VILLES].[CODE_REGION]
;

-- Drop de la table temporaire
DROP TABLE [ODE_DATAWAREHOUSE].[VILLES_REGIONS]
GO


-- Création de tables temporaires pour charger les fichiers csv
CREATE TABLE [ODE_DATAWAREHOUSE].[VILLES_DEPARTEMENTS](
	[CODE_REGION]				[INT]					NOT NULL, 
 	[CODE_DEPARTEMENT]			[NVARCHAR](3)			NOT NULL, 
 	[NOM_DEPARTEMENT_MAJ]		[NVARCHAR](50)			NOT NULL,
 	[NOM_DEPARTEMENT_MIN]		[NVARCHAR](50)			NOT NULL,
) ON [PRIMARY]; 


BULK INSERT [ODE_DATAWAREHOUSE].[VILLES_DEPARTEMENTS] FROM N'$(OdeCsvPath)DepartementsINSEE.csv' -- chargement du fichier csv dans la table
WITH (
    CODEPAGE='1252', -- CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- Fin de lignes des CSV en LF seul (/n <=> CR + LF)
);

--Remplissage de la table catégories à partir des tables temporaires créées précedemment

UPDATE [ODE_DATAWAREHOUSE].[DIM_VILLES]
SET 
	[NOM_DEPARTEMENT_MAJ] = V.[NOM_DEPARTEMENT_MAJ],
	[NOM_DEPARTEMENT_MIN] = V.[NOM_DEPARTEMENT_MIN]
FROM		
	[ODE_DATAWAREHOUSE].[VILLES_DEPARTEMENTS] AS V
WHERE	
	V.[CODE_REGION] = [DIM_VILLES].[CODE_REGION] AND
	V.[CODE_DEPARTEMENT] = [DIM_VILLES].[CODE_DEPARTEMENT]
;

-- Drop de la table temporaire
DROP TABLE [ODE_DATAWAREHOUSE].[VILLES_DEPARTEMENTS]
GO




-- ----------------------------------
-- ETAPE 4 : REMPLISSAGE TABLE LIEUX
-- ----------------------------------

-- THOMAS # 09/07/2015

-- OLIVIER # 15/07/2015 : 6 entrepots crées au lieu de 2

-- REMPLISSAGE TABLES
declare @compteur int = 0

declare @type int
declare @type_lieu [CHAR](1)
declare @type_lieu_bis [CHAR](1)
declare @lib_ville nvarchar(50)
declare @ville_fk int
declare @libel_lieu [NVARCHAR](256)
declare @libel_lieu_bis [NVARCHAR](256)
declare @dept nvarchar(20)
declare @date_ouv date, @date_ferm date
declare @est_ferme int
declare @surface [NUMERIC](6,1)
declare @surface_bis [NUMERIC](6,1)
declare @nb_logistique int = 2
declare @nb_entrepot int = 10
declare @nb_magasins int = 121
declare @nb_sit_internet int = 1



WHILE @compteur < @nb_logistique
BEGIN
	SET @ville_fk =  (SELECT TOP 1 VILLE_PK FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] ORDER BY NEWID()) --on choisit une ville au hasard dans la table des villes
	SET @lib_ville = (SELECT NOM_VILLE_MAJ FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] WHERE VILLE_PK = @ville_fk) -- on récupère le nom de la ville associée
	SET @dept = (SELECT CODE_DEPARTEMENT FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] WHERE VILLE_PK = @ville_fk) -- et le département

	SET @date_ouv = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 3650) * -1, getdate())) -- on génère une date d'ouverture aléatoire sur les 10 dernières années
	SET @date_ferm = '01/01/1900' -- on initialise la date de fermeture

	SET @est_ferme = (select cast(round((4-1)* rand() + 1,0) as integer)) --on effectue un tirage pour décider si le magasin est fermé où non
	IF @est_ferme = 1 -- la magasin a 1 chance sur 4 d'être fermé
	BEGIN
		WHILE @date_ferm <= @date_ouv -- dans ce cas on lui attribue une date de fermeture postérieur à la date d'ouverture
			SET @date_ferm = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 3650) * -1, getdate()))
	END

	SET @surface = (select cast((1000-100)* rand() + 1 as numeric(6,1))) -- on tire au sort une surface entre 100 et 1000 m²

	SET @type_lieu = 'P'
	SET @libel_lieu = 'Plateforme logistique de '+@lib_ville+' ('+@dept+')'

	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_LIEUX] ([VILLE_FK],[TYPE_LIEU],[LIBEL_LIEU],[DATE_OUVERTURE],[DATE_FERMETURE],[SURFACE_M2]) -- on insert nos données dans la table LIEUX
	VALUES (@ville_fk,
		@type_lieu,
		@libel_lieu,
		@date_ouv,
		@date_ferm,
		@surface)

	
	SET @compteur = @compteur + 1
END

	SET @compteur = 0

WHILE @compteur < @nb_entrepot
BEGIN
	SET @ville_fk =  (SELECT TOP 1 VILLE_PK FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] ORDER BY NEWID()) --on choisit une ville au hasard dans la table des villes
	SET @lib_ville = (SELECT NOM_VILLE_MIN FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] WHERE VILLE_PK = @ville_fk) -- on récupère le nom de la ville associée
	SET @dept = (SELECT CODE_DEPARTEMENT FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] WHERE VILLE_PK = @ville_fk) -- et le département

	SET @date_ouv = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 3650) * -1, getdate())) -- on génère une date d'ouverture aléatoire sur les 10 dernières années
	SET @date_ferm = '01/01/1900' -- on initialise la date de fermeture

	SET @est_ferme = (select cast(round((4-1)* rand() + 1,0) as integer)) --on effectue un tirage pour décider si le magasin est fermé où non
	IF @est_ferme = 1 -- la magasin a 1 chance sur 4 d'être fermé
	BEGIN
		WHILE @date_ferm <= @date_ouv -- dans ce cas on lui attribue une date de fermeture postérieur à la date d'ouverture
			SET @date_ferm = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 3650) * -1, getdate()))
	END

	SET @surface = (select cast((1000-100)* rand() + 1 as numeric(6,1))) -- on tire au sort une surface entre 100 et 1000 m²

	SET @type_lieu = 'E'
	SET @libel_lieu = 'Entrepôt de '+@lib_ville+' ('+@dept+')'

	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_LIEUX] ([VILLE_FK],[TYPE_LIEU],[LIBEL_LIEU],[DATE_OUVERTURE],[DATE_FERMETURE],[SURFACE_M2]) -- on insert nos données dans la table LIEUX
	VALUES (@ville_fk,
		@type_lieu,
		@libel_lieu,
		@date_ouv,
		@date_ferm,
		@surface)
	
	SET @compteur = @compteur + 1
END

SET @compteur = 0

WHILE @compteur < @nb_magasins
BEGIN
	SET @ville_fk =  (SELECT TOP 1 VILLE_PK FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] ORDER BY NEWID()) --on choisit une ville au hasard dans la table des villes
	SET @lib_ville = (SELECT NOM_VILLE_MIN FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] WHERE VILLE_PK = @ville_fk) -- on récupère le nom de la ville associée
	SET @dept = (SELECT CODE_DEPARTEMENT FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] WHERE VILLE_PK = @ville_fk) -- et le département

	SET @date_ouv = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 3650) * -1, getdate())) -- on génère une date d'ouverture aléatoire sur les 10 dernières années
	SET @date_ferm = '01/01/1900' -- on initialise la date de fermeture

	SET @est_ferme = (select cast(round((4-1)* rand() + 1,0) as integer)) --on effectue un tirage pour décider si le magasin est fermé où non
	IF @est_ferme = 1 -- la magasin a 1 chance sur 4 d'être fermé
	BEGIN
		WHILE @date_ferm <= @date_ouv -- dans ce cas on lui attribue une date de fermeture postérieur à la date d'ouverture
			SET @date_ferm = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 3650) * -1, getdate()))
	END

	SET @surface = (select cast((1000-100)* rand() + 1 as numeric(6,1))) -- on tire au sort une surface entre 100 et 1000 m²
	SET @surface_bis = (select cast((1000-100)* rand() + 1 as numeric(6,1))) -- on tire au sort une surface entre 100 et 1000 m²

	SET @type_lieu = 'R'
	SET @libel_lieu = 'Magasin de '+@lib_ville+' ('+@dept+')'

	SET @type_lieu_bis = 'M'
	SET @libel_lieu_bis = 'Stock du magasin de '+@lib_ville+' ('+@dept+')'

	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_LIEUX] ([VILLE_FK],[TYPE_LIEU],[LIBEL_LIEU],[DATE_OUVERTURE],[DATE_FERMETURE],[SURFACE_M2]) -- on insert nos données dans la table LIEUX
	VALUES (@ville_fk,
		@type_lieu,
		@libel_lieu,
		@date_ouv,
		@date_ferm,
		@surface)

	
	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_LIEUX] ([VILLE_FK],[TYPE_LIEU],[LIBEL_LIEU],[DATE_OUVERTURE],[DATE_FERMETURE],[SURFACE_M2]) -- on insert nos données dans la table LIEUX
	VALUES (@ville_fk,
		@type_lieu_bis,
		@libel_lieu_bis,
		@date_ouv,
		@date_ferm,
		@surface_bis)


	SET @compteur = @compteur + 1
END

SET @compteur = 0

WHILE @compteur < @nb_sit_internet
BEGIN
	SET @ville_fk =  (SELECT TOP 1 VILLE_PK FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] ORDER BY NEWID()) --on choisit une ville au hasard dans la table des villes
	SET @lib_ville = (SELECT NOM_VILLE_MIN FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] WHERE VILLE_PK = @ville_fk) -- on récupère le nom de la ville associée
	SET @dept = (SELECT CODE_DEPARTEMENT FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] WHERE VILLE_PK = @ville_fk) -- et le département

	SET @date_ouv = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 3650) * -1, getdate())) -- on génère une date d'ouverture aléatoire sur les 10 dernières années
	SET @date_ferm = '01/01/1900' -- on initialise la date de fermeture

	SET @est_ferme = (select cast(round((4-1)* rand() + 1,0) as integer)) --on effectue un tirage pour décider si le magasin est fermé où non
	IF @est_ferme = 1 -- la magasin a 1 chance sur 4 d'être fermé
	BEGIN
		WHILE @date_ferm <= @date_ouv -- dans ce cas on lui attribue une date de fermeture postérieur à la date d'ouverture
			SET @date_ferm = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 3650) * -1, getdate()))
	END

	SET @surface = 0 -- on tire au sort une surface entre 100 et 1000 m²
	SET @surface_bis = (select cast((1000-100)* rand() + 1 as numeric(6,1))) -- on tire au sort une surface entre 100 et 1000 m²

	SET @type_lieu = 'I'
	SET @libel_lieu = 'Site Internet administré depuis '+@lib_ville+' ('+@dept+')'

	SET @type_lieu_bis = 'S'
	SET @libel_lieu_bis = 'Stock pour site Internet à '+@lib_ville+' ('+@dept+')'

	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_LIEUX] ([VILLE_FK],[TYPE_LIEU],[LIBEL_LIEU],[DATE_OUVERTURE],[DATE_FERMETURE],[SURFACE_M2]) -- on insert nos données dans la table LIEUX
	VALUES (@ville_fk,
		@type_lieu,
		@libel_lieu,
		@date_ouv,
		@date_ferm,
		@surface)

	
	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_LIEUX] ([VILLE_FK],[TYPE_LIEU],[LIBEL_LIEU],[DATE_OUVERTURE],[DATE_FERMETURE],[SURFACE_M2]) -- on insert nos données dans la table LIEUX
	VALUES (@ville_fk,
		@type_lieu_bis,
		@libel_lieu_bis,
		@date_ouv,
		@date_ferm,
		@surface_bis)


	SET @compteur = @compteur + 1
END


-- -----------------------------------
-- ETAPE 5 : REMPLISSAGE TABLE CLIENTS
-- -----------------------------------

-- 08/07/2015 # CEDRIC

DECLARE @nb_poste_total AS int 
DECLARE @nb_poste_deb AS int
DECLARE @nb_insert AS int
DECLARE @taux_remise AS [DECIMAL](6, 2)
DECLARE @typeChar AS [CHAR](1)
DECLARE @i AS int , @j AS int
DECLARE @nb_anonyme AS int
DECLARE @nb_internet AS int
DECLARE @nb_nominatif AS int
DECLARE @nb_pro AS int
DECLARE @nb_societe AS int
DECLARE @nom AS [NVARCHAR](256)
DECLARE @date_naissance AS date
DECLARE @date_souscription AS date
DECLARE @code_fidelite AS [NVARCHAR](32)
DECLARE @consonne AS nvarchar(20) = 'BCDFGHJKLMNPQRSTVWXZ'
DECLARE @voyelle AS nvarchar(6) = 'AEIOUY'
DECLARE @length AS int

-- OLIVIER # 24/07/2015 - Globalisation de l activation de la trace sur tous les print
DECLARE @traceOn BIT;
SET @traceOn = 0;						-- Activation de la trace : 1

--La table ville devra etre chargée en amont
--!!!!!! modifier ici le nombre poste total souhaité à la fin du traitement
SET @nb_poste_total = 100000
--!!!!!!!!!!!!!!!!!!!!!!!


-- Création de tables temporaires pour charger les fichiers csv
-- Olivier # 07/09/2015
CREATE TABLE [ODE_DATAWAREHOUSE].[NOMS](
	[NOM]				[NVARCHAR](50)			NOT NULL
) ON [PRIMARY]; 

-- Olivier # 07/09/2015
BULK INSERT [ODE_DATAWAREHOUSE].[NOMS] FROM N'$(OdeCsvPath)NomsFamilles.csv' -- chargement du fichier csv dans la table
WITH (
    CODEPAGE='1252', -- CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- Fin de lignes des CSV en LF seul (/n <=> CR + LF)
);



-- OLIVIER # 24/07/2015 - Globalisation de l activation de la trace sur tous les print
IF @traceOn = 1
BEGIN
	PRINT 'Remplissage automatique de la table Clients'
	PRINT '--------------------------------------------------'
	PRINT 'Nombre de postes souhaités   : ' + CAST(@nb_poste_total as nvarchar)
END;
	
-- Recupération nombre de postes en table
(SELECT @nb_poste_deb = count(*) FROM [ODE_DATAWAREHOUSE].[DIM_CLIENTS])

-- OLIVIER # 24/07/2015 - Globalisation de l activation de la trace sur tous les print
IF @traceOn = 1
PRINT 'Nombre de postes avant ajout : ' + CAST(@nb_poste_deb as nvarchar)

-- Calcul nombre de postes à ajouter
SET @nb_insert = @nb_poste_total - @nb_poste_deb

-- OLIVIER # 24/07/2015 - Globalisation de l activation de la trace sur tous les print
IF @traceOn = 1
	PRINT 'Nombre de postes à ajouter   : ' + CAST(@nb_insert as nvarchar)

SET @nb_anonyme = @nb_insert* 40 / 100 
SET @nb_internet = @nb_insert * 20 / 100 
SET @nb_nominatif = @nb_insert * 20 / 100 
SET @nb_pro = @nb_insert * 10 / 100 
SET @nb_societe = @nb_insert - @nb_anonyme - @nb_internet - @nb_nominatif - @nb_pro

-- OLIVIER # 24/07/2015 - Globalisation de l activation de la trace sur tous les print
IF @traceOn = 1
BEGIN
	PRINT '--------------------------------------------------'
	PRINT 'Nombre d''anonyme             : ' + CAST(@nb_anonyme as nvarchar)
	PRINT 'Nombre internet              : ' + CAST(@nb_internet as nvarchar)
	PRINT 'Nombre nominatif             : ' + CAST(@nb_nominatif as nvarchar)
	PRINT 'Nombre professionnel         : ' + CAST(@nb_pro as nvarchar)
	PRINT 'Nombre société               : ' + CAST(@nb_societe as nvarchar)
END;

-- Boucle de traitement
SET @i = 1
WHILE(@i <= @nb_insert)
	BEGIN

-- Selection ville aléatoire dans la table Ville
		SET @ville_fk =  (SELECT TOP 1 VILLE_PK FROM [ODE_DATAWAREHOUSE].[DIM_VILLES] ORDER BY NEWID())

-- Type de client aléatoire (champs de valeur)
-- A : Anonyme
-- I : Internet
-- N : Nominatif
-- P : Professionnel artisan
-- S : Société
		SET @typeChar =
			CASE 
			WHEN @i> @nb_anonyme + @nb_internet + @nb_nominatif + @nb_pro THEN 'S'
			WHEN @i> @nb_anonyme + @nb_internet + @nb_nominatif           THEN 'P'
			WHEN @i> @nb_anonyme + @nb_internet                           THEN 'N'
			WHEN @i> @nb_anonyme                                          THEN 'I'
			WHEN @i> 0                                                    THEN 'A'
	END

		-- Nom du client aléatoire selon le type de client
		IF @typeChar = 'A' 
		BEGIN
			SET @nom = 'Client anonyme'
			SET @taux_remise = 0
		END

		IF @typeChar = 'I' OR @typeChar = 'N' OR @typeChar = 'S' OR @typeChar = 'P'
		BEGIN
			-- Olivier # 07/09/2015
			-- Tirage aléatoire d un nom (Simple ou composé de 2 une fois sur 3)
			SET @taux_remise = (select cast(round((30)* rand() + 1,0) as integer))
			SET @nom = (SELECT TOP 1 UPPER(NOM) FROM [ODE_DATAWAREHOUSE].[NOMS] ORDER BY NEWID()) --on choisit un nom au hasard dans la table des noms
			IF 3 * rand() <= 1
			BEGIN
				SET @nom = @nom + '-' + (SELECT TOP 1 UPPER(NOM) FROM [ODE_DATAWAREHOUSE].[NOMS] ORDER BY NEWID()) --on choisit un nom au hasard dans la table des noms
			END
			/*
			-- recup nom aléatoire avec longueur aléatoire entre 2 et 102
			SET @length = (select cast(round((50 -1)* rand() + 1,0) as integer)) + 2
			SET @j = 1
			SET @nom =''
			
			WHILE @j < @length
			BEGIN
				SET @nom = @nom + substring(@consonne, convert(int, rand()*20), 1)
				SET @nom = @nom + substring(@voyelle, convert(int, rand()*6), 1)
				SET @j += 1
			END
			*/
			; -- Olivier # 07/09/2015
		END

		IF @typeChar = 'I'
		BEGIN
			SET @nom = 'INT ' + @nom -- Olivier # 07/09/2015
			SET @taux_remise = 0
		END

		IF @typeChar = 'N'
		BEGIN
			-- SET @nom = @nom + ' BOB ' + CAST(@i as nvarchar) -- Olivier # 07/09/2015
			-- Ajout du prénom
			SET @nom = @nom + ', ' + (SELECT TOP 1 NOM FROM [ODE_DATAWAREHOUSE].[NOMS] ORDER BY NEWID()) --on choisit un prénom au hasard dans la table des noms
			SET @taux_remise = (select cast(round((30)* rand() + 1,0) as integer))
		END

		IF @typeChar = 'S'
		BEGIN
			SET @nom = 'SARL ' + @nom -- Olivier # 07/09/2015
			SET @taux_remise = cast(30* rand() + 1 as integer)
		END
		
		IF @typeChar = 'P'
		BEGIN
			SET @nom = 'PRO ' + @nom -- Olivier # 07/09/2015
			SET @taux_remise = cast(30* rand() + 1 as integer)
		END

		
		-- Date de naissance aléatoire sur les 80 dernières années
		IF @typeChar = 'I' OR @typeChar = 'N' OR @typeChar = 'P' 
		BEGIN
			-- SET @date_naissance = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 31025) * -1, getdate()))
			SET @date_naissance = DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 29200), '01/01/1920'); -- Olivier # 07/09/2015
		END
		ELSE
		BEGIN
			SET @date_naissance = '01/01/0001' 
		END
		
		-- Date de souscription aléatoire sur les 20 dernières années
		-- SET @date_souscription = (SELECT dateadd(day, (abs(CHECKSUM(newid())) % 7300) * -1, getdate()))
		SET @date_souscription = DATEADD(DAY, ABS(CHECKSUM(NEWID()) % 7300), '01/01/1995') -- Olivier # 07/09/2015

		
		-- Code fidélité aléatoire : Série de 12 chiffres 
        IF @taux_remise > 0
		BEGIN
		SET	@code_fidelite = FORMAT ((1000000000000-1) * rand() + 1, '000000000000') -- Olivier # 07/09/2015  
		END
		/*
		SET @code_fidelite = ''
	    IF @taux_remise > 0
		BEGIN
		   	SET @length = (select cast(round((16 -1)* rand() + 1,0) as integer)) + 1
			SET @j = 1
			WHILE @j < @length
			BEGIN
				SET @code_fidelite = @code_fidelite + substring(@consonne, convert(int, rand()*20), 1)
				SET @code_fidelite = @code_fidelite + substring(@voyelle, convert(int, rand()*6), 1)
				SET @j += 1
			END
		END
		*/

-- Insertion en table Clients
	INSERT INTO [ODE_DATAWAREHOUSE].[DIM_CLIENTS]
		([VILLE_FK],
		 [TAUX_REMISE],
		 [TYPE_CLIENT],
		 [NOM_CLIENT],
		 [DATE_NAISSANCE],
		 [DATE_SOUSCRIPTION],
		 [CODE_FIDELITE])
	VALUES
		(@ville_fk,
		 @taux_remise,
		 @typeChar,
		 @nom,
		 @date_naissance,
		 @date_souscription,
		 @code_fidelite)

    SET @i += 1

END

-- Drop de la table temporaire
-- Olivier # 07/09/2015
DROP TABLE [ODE_DATAWAREHOUSE].[NOMS]
GO




-- ---------------------------------
-- ETAPE 6 : REMPLISSAGE TABLE TEMPS
-- ---------------------------------

-- 13/07/2015 # OLIVIER

--  Source du CSV : Annexes du livre 
--	> Business Intelligence avec SQL Server 2014
--	> Par Sébastien FANTINI et Franck GAVAND
--	> Edition ENI


------------------------------
-- Chargement CSV de DIM_TEMPS
------------------------------

-- Vidange de la table
-- Pas de TRUNCATE cause FK
DELETE FROM [ODE_DATAWAREHOUSE].[DIM_TEMPS];
GO

SET DATEFORMAT DMY
GO

BULK INSERT [ODE_DATAWAREHOUSE].[DIM_TEMPS] FROM N'$(OdeCsvPath)DimTemps.csv'
WITH (
    --CHECK_CONSTRAINTS,
    --CODEPAGE='ACP',
    CODEPAGE='1252', -- OLIVIER # 16/07/2015 : CodePage du LATIN-1 pour gestion des accents
    FIELDTERMINATOR=';',
    ROWTERMINATOR='0x0a' -- OLIVIER # 16/07/2015 : Fin de lignes des CSV en LF seul (/n <=> CR + LF)
    --KEEPIDENTITY,
    --TABLOCK
);
GO


-- ----------------------------------
-- ETAPE 7 : REMPLISSAGE TABLE VENTES
-- ----------------------------------

-- 13/07/2015 # OLIVIER

-- Declarations de variables
DECLARE @typeLieuVente CHAR;
DECLARE @nbrMoyenArticle INT;
DECLARE @nbrArticlesIndex INT;
DECLARE @nbrArticlesVente INT;
DECLARE @tauxMargeMoyen NUMERIC(4,1);
DECLARE @tauxMargeProduit NUMERIC(4,1);
DECLARE @nbrJourMois INT;
DECLARE @moisIndex INT;
DECLARE @jourIndex INT;
DECLARE @typeLieuSelection CHAR;
DECLARE @dateVenteFk DATETIME;
DECLARE @produitFk INT;
DECLARE @clientFk INT;
DECLARE @lieuFk INT;
DECLARE @montantHtVente MONEY;
DECLARE @montantTvaVente MONEY;
DECLARE @margeBrute MONEY;
DECLARE @unitesVendues INT;
DECLARE @nbrAnnuelVenteInternet INT;
DECLARE @nbrAnnuelVenteMagasin INT;
DECLARE @anneeIndex INT;
DECLARE @anneeDebut INT;
DECLARE @anneeFin INT;
DECLARE @numTickets INT;
DECLARE @nbrMoyenArticleInternet INT;
DECLARE @nbrMoyenArticleMagasin INT;
DECLARE @tauxMargeArticle NUMERIC(4,1);
DECLARE @tauxMargeMoyenInternet NUMERIC(4,1);
DECLARE @tauxMargeMoyenMagasin NUMERIC(4,1);
DECLARE @ecartMoyenMarge NUMERIC(4,1);


-- Constantes du code
SET @nbrAnnuelVenteInternet = 200000;	 -- Nombre de ventes anuelles sur Internet. Cible : 200 000
SET @nbrAnnuelVenteMagasin  = 800000;	-- Nombre de ventes anuelles en Magasin. Cible : 800 000
SET @anneeDebut = 2010;					-- Annee de debut, incluse
SET @anneeFin = 2015;					-- Annee de fin, incluse
SET @numTickets = 1;					-- Numero du 1er ticket de caisse (Arbitraire)
SET @nbrMoyenArticleInternet = 5;		-- Sur Internet : 5 articles par ticket en moyenne
SET @nbrMoyenArticleMagasin = 18;		-- En magasin : 18 articles par ticket en moyenne
SET @tauxMargeMoyenInternet = 44.3;		-- Sur Internet - Moyenne 44.3 pourcents de marge
SET @tauxMargeMoyenMagasin = 34.8;		-- En Magasin - Moyenne 34.8 pourcents de marge
SET @ecartMoyenMarge = 20.0;			-- Ecart maximale de variation autour de la marge moyenne


-- OLIVIER # 24/07/2015 - Globalisation de l activation de la trace sur tous les print
DECLARE @traceOn BIT;
SET @traceOn = 0;						-- Activation de la trace : 1

---------------------------------------------------------------------------------
-- Le SI Decisionnel est alimente depuis environ 5 ans (01/01/2010 -> 01/01/2015)
-- Pour toutes les annees depuis 2010 
---------------------------------------------------------------------------------
SET @anneeIndex = @anneeDebut

WHILE (@anneeIndex <= @anneeFin)
BEGIN

	-------------------------------------
	-- Pour toutes les ventes d une année
	------------------------------------- 
	WHILE (@numTickets < (@anneeIndex - @anneeDebut + 1) * (@nbrAnnuelVenteInternet + @nbrAnnuelVenteMagasin)) -- Les numeros de tickets ne sont pas RAZ chaque annee
	BEGIN
		---------------------------------------------
		-- Increment de l index des tickets de caisse
		---------------------------------------------
		SET @numTickets = @numTickets + 1;
	
		------------------------------
		-- Vente Magasin ou Internet ?
		------------------------------
		IF(rand() * (@nbrAnnuelVenteInternet + @nbrAnnuelVenteMagasin) < @nbrAnnuelVenteInternet)
		BEGIN
			SET @typeLieuVente = 'I';
		END;
		ELSE
		BEGIN
			SET @typeLieuVente = 'R';
		END;

		--------------------------------------------------------------
		-- Selection aleatoire de la date de vente au cours de l annee
		--------------------------------------------------------------
		SET @dateVenteFk = (SELECT TOP 1 TEMPS_PK FROM [ODE_DATAWAREHOUSE].[DIM_TEMPS] WHERE [ANNEE_NOM] = 'Calendrier ' + cast(@anneeIndex as nvarchar) ORDER BY NEWID()) -- Olivier # 04/08/2015 : Tirage aleatoire de la date appartenant a cette annee
		print @dateVenteFk;
		
		---------------------------------------------------------------------------
		-- Selection aleatoire du lieu de type R : Rayon de magasin ou I : Internet
		---------------------------------------------------------------------------
		SET @typeLieuSelection = 'X';

		WHILE(@typeLieuSelection != @typeLieuVente)
		BEGIN
			SET @lieuFk = (SELECT TOP 1 LIEU_PK FROM [ODE_DATAWAREHOUSE].[DIM_LIEUX] ORDER BY NEWID()); -- Olivier # 26/07/2015 : Tirage aleatoire du lieu
			SET @typeLieuSelection = (SELECT [TYPE_LIEU] FROM [ODE_DATAWAREHOUSE].[DIM_LIEUX] WHERE [LIEU_PK] = @lieuFk);

			IF(@traceOn = 1)
			BEGIN
				PRINT '@lieuFk = '+ cast(@lieuFk as nvarchar);
				PRINT '@typeLieuSelection = '+ cast(@typeLieuSelection as nvarchar);
			END;

		END;
		
	
		--------------------------------	
		-- Selection aleatoire du client
		--------------------------------	
		SET @clientFk = (SELECT TOP 1 CLIENT_PK FROM [ODE_DATAWAREHOUSE].[DIM_CLIENTS] ORDER BY NEWID()) -- Olivier # 26/07/2015 : Tirage aleatoire du client


		--------------------------------------	
		-- Nombre moyen d'articles de la vente
		--------------------------------------	
		IF(@typeLieuVente = 'I')
		BEGIN
			SET @nbrMoyenArticle = @nbrMoyenArticleInternet; -- Entre 1 et 9 articles sur Internet - Moyenne 5
		END;
		ELSE
		BEGIN
			SET @nbrMoyenArticle = @nbrMoyenArticleMagasin; -- Entre 1 et 35 articles en Magasin - Moyenne 18
		END;

		SET @nbrArticlesVente = cast( floor(1 + (2 * @nbrMoyenArticle - 1) * rand()) as INT); 
		
		IF(@traceOn = 1)
			PRINT '@nbrArticlesVente = '+ cast(@nbrArticlesVente as nvarchar);

			
		----------------------------------------------------	
		-- Pour tous les produits composant une vente donnee
		----------------------------------------------------
		SET @nbrArticlesIndex = 1; -- RAZ

		WHILE(@nbrArticlesIndex <= @nbrArticlesVente)
		BEGIN
			
			--------------------------------------------------------------------------
			-- Taux de marge aleatoire sur le produit. 
			-- Si Magasin : 34.8 +/- 20 pourcents, si Internet : 44.3 +/- 20 pourcents
			--------------------------------------------------------------------------
			IF(@typeLieuVente = 'I')
			BEGIN
				SET @tauxMargeMoyen = @tauxMargeMoyenInternet; -- Entre 24.3 et 64.3 pourcents de marge sur Internet - Moyenne 44.3
			END;
			ELSE
			BEGIN
				SET @tauxMargeMoyen = @tauxMargeMoyenMagasin; -- Entre 14.8 et 54.8 pourcents de marge en Magasin - Moyenne 34.8
			END;
			SET @tauxMargeArticle = cast(@tauxMargeMoyen + @ecartMoyenMarge * (2* rand() - 1) as NUMERIC(4,1));

			IF(@traceOn = 1)
				PRINT '@tauxMargeArticle = '+ cast(@tauxMargeArticle as nvarchar);
			
			------------------------------
			-- Tirage au sort de l article
			------------------------------
			SET @produitFk = (SELECT TOP 1 PRODUIT_PK FROM [ODE_DATAWAREHOUSE].[DIM_PRODUITS] ORDER BY NEWID()) -- Olivier # 26/07/2015 : Tirage aleatoire du produit

			------------------------------------------------------------------------------
			-- Nbr d articles identiques de la vente
			-- On tire un rand() entre 1 et le nombre d'articles restants pour cette vente
			------------------------------------------------------------------------------
			SET @unitesVendues = cast( floor(1 + (@nbrArticlesVente - @nbrArticlesIndex) * rand()) as INT); 
			
			IF(@traceOn = 1)
				PRINT '@unitesVendues = '+ cast(@unitesVendues as nvarchar);

			-------------------------------------
			-- Nbr d articles cumules de la vente
			-------------------------------------
			SET @nbrArticlesIndex = @nbrArticlesIndex + @unitesVendues;

			-----------------------------------------------------------------------
			-- Egale à [UNITES_VENDUES] * [DIM_PRODUITS.PRIX_ACHAT] * [1 + MARGE_BRUTE en pourcents]
			-----------------------------------------------------------------------
			SELECT @montantHtVente = @unitesVendues * [PRIX_ACHAT] * (1 + @tauxMargeArticle / 100) FROM [ODE_DATAWAREHOUSE].[DIM_PRODUITS] WHERE [PRODUIT_PK] = @produitFk;

			-------------------------------------------------------------
			-- Egale à [MONTANT_HT_VENTE] * [DIM_PRODUITS.TAUX_TVA] / 100
			-------------------------------------------------------------
			SELECT @montantTvaVente = @montantHtVente * (1 + [TAUX_TVA] / 100) FROM [ODE_DATAWAREHOUSE].[DIM_PRODUITS] WHERE [PRODUIT_PK] = @produitFk;

			-------------------------------------------------------
			-- Egale au taux de marge de l article * son montant HT
			-------------------------------------------------------
			SET @margeBrute = @montantHtVente * (1 + @tauxMargeArticle / 100);

			IF(@traceOn = 1)
			BEGIN
				PRINT '@dateVenteFk = ' + cast(@dateVenteFk as nvarchar);
				PRINT '@produitFk = ' + cast(@produitFk as nvarchar);
				PRINT '@clientFk = ' + cast(@clientFk as nvarchar);
				PRINT '@lieuFk = ' + cast(@lieuFk as nvarchar);
				PRINT '@montantHtVente = ' + cast(@montantHtVente as nvarchar);
				PRINT '@montantTvaVente = ' + cast(@montantTvaVente as nvarchar);
				PRINT '@margeBrute = ' + cast(@margeBrute as nvarchar);
				PRINT '@unitesVendues = ' + cast(@unitesVendues as nvarchar);
				PRINT '@numTickets = ' + cast(@numTickets as nvarchar);
			END

			---------------------------
			-- INSERT en table de faits
			---------------------------
			INSERT INTO [ODE_DATAWAREHOUSE].[FACT_VENTES](
				[DATE_VENTE_FK],
				[PRODUIT_FK],
				[CLIENT_FK],
				[LIEU_FK],
				[MONTANT_HT_VENTE],
				[MONTANT_TVA_VENTE],
				[MARGE_BRUTE],
				[UNITES_VENDUES],
				[NUM_TICKET]
			) VALUES(
				@dateVenteFk,
				@produitFk,
				@clientFk,
				@lieuFk,
				@montantHtVente,
				@montantTvaVente,
				@margeBrute,
				@unitesVendues,
				@numTickets
			);

		END;
		

		------------------------------------------------------------------
		-- Affichage pour suivi du script tous les 10000 tickets de caisse
		------------------------------------------------------------------
		IF(@numTickets % 10000 = 0)
		BEGIN
			PRINT 'Ticket de caisse no. ' + cast(@numTickets as nvarchar);
		END;

	END;

	----------------------------------
	-- Increment de l index des annees
	----------------------------------
	SET @anneeIndex = @anneeIndex + 1;
	IF(@traceOn = 1)
		PRINT '@anneeIndex = '+ cast(@anneeIndex as nvarchar);

END;
