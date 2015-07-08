
/*============================================================================
  
  Fichier:     Script_Remplissage_FactVentes.sql
  Résumé:  Rempli la table Fact_Ventes du DWH du projet ODE
  Date:     08/07/2015
  Updated:  
  SQL Server Version: 2014
  
------------------------------------------------------------------------------

Fact_Ventes :

	[DATE_VENTE_FK] 	[INT]			NOT NULL
	[PRODUIT_FK] 		[INT]			NOT NULL
	[CLIENT_FK] 		[INT]			NOT NULL
	[LIEU_FK] 			[INT]			NOT NULL
	[MONTANT_HT_VENTE]	[MONEY]			NOT NULL
	[MONTANT_TVA_VENTE]	[MONEY]			NOT NULL
	[MARGE_BRUTE]		[MONEY]			NOT NULL
	[UNITES_VENDUES]	[INT]			NOT NULL
	[NUM_TICKET]		[NVARCHAR](256)	NOT NULL
	

  
============================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit

-- Connexion à la base
USE [DataWarehouseODE];
GO


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
DECLARE @nbrClient INT;
DECLARE @nbrLieux INT;
DECLARE @typeLieuSelection CHAR;
DECLARE @dateVenteFk INT;
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
DECLARE @nbrProduits INT;
DECLARE @nbrLieu INT;
DECLARE @traceOn BIT;


-- Constantes du code
SET @nbrAnnuelVenteInternet = 2; -- 2200000;	 -- Nombre de ventes anuelles sur Internet. Cible : 2.2 M
SET @nbrAnnuelVenteMagasin = 9; -- 9600000;	-- Nombre de ventes anuelles en Magasin. Cible : 9.6 M
SET @anneeDebut = 2010;					-- Annee de debut, incluse
SET @anneeFin = 2015;					-- Annee de fin, incluse
SET @numTickets = 1;					-- Numero du 1er ticket de caisse (Arbitraire)
SET @nbrMoyenArticleInternet = 5;		-- Sur Internet : 5 articles par ticket en moyenne
SET @nbrMoyenArticleMagasin = 18;		-- En magasin : 18 articles par ticket en moyenne
SET @tauxMargeMoyenInternet = 44.3;		-- Sur Internet - Moyenne 44.3 pourcents de marge
SET @tauxMargeMoyenMagasin = 34.8;		-- En Magasin - Moyenne 34.8 pourcents de marge
SET @ecartMoyenMarge = 20.0;			-- Ecart maximale de variation autour de la marge moyenne
SET @traceOn = 1;						-- Activation de la trace : 1




TRUNCATE TABLE [ODE_DATAWAREHOUSE].[FACT_VENTES];

--------------------------------------
-- Insertion du jeu de test "a minima"
--------------------------------------

INSERT INTO [ODE_DATAWAREHOUSE].[DIM_VILLES](CODE_POSTAL, CODE_COMMUNE, CODE_REGION, CODE_DEPARTEMENT, CODE_ARRONDISEMENT, CODE_CANTON, NOM_VILLE_MAJ, NOM_VILLE_MIN)
VALUES
	('58800', -- CODE_POSTAL	nvarchar(6)
	1, -- CODE_COMMUNE	int
	5, -- CODE_REGION	int
	58, -- CODE_DEPARTEMENT	int
	1, -- CODE_ARRONDISEMENT	int
	800, -- CODE_CANTON	int
	'CORBIGNY', -- NOM_VILLE_MAJ	nvarchar(256)
	'Corbigny' -- NOM_VILLE_MIN	nvarchar(256)
);


INSERT INTO [ODE_DATAWAREHOUSE].[DIM_LIEUX](VILLE_FK, TYPE_LIEU, LIBEL_LIEU, DATE_OUVERTURE, SURFACE_M2)
VALUES
	((select max([VILLE_PK]) from [ODE_DATAWAREHOUSE].[DIM_VILLES]), -- VILLE_FK	int
	'I', -- TYPE_LIEU	char(1)
	'Site Internet', -- LIBEL_LIEU	nvarchar(256)
	getdate(), -- DATE_OUVERTURE	date
	1000), -- SURFACE_M2	int

	((select max([VILLE_PK]) from [ODE_DATAWAREHOUSE].[DIM_VILLES]), -- VILLE_FK	int
	'R', -- TYPE_LIEU	char(1)
	'Site Internet', -- LIBEL_LIEU	nvarchar(256)
	getdate(), -- DATE_OUVERTURE	date
	1000), -- SURFACE_M2	int

	((select max([VILLE_PK]) from [ODE_DATAWAREHOUSE].[DIM_VILLES]), -- VILLE_FK	int
	'E', -- TYPE_LIEU	char(1)
	'Site Internet', -- LIBEL_LIEU	nvarchar(256)
	getdate(), -- DATE_OUVERTURE	date
	1000) -- SURFACE_M2	int
;


INSERT INTO [ODE_DATAWAREHOUSE].[DIM_CLIENTS](VILLE_FK, TAUX_REMISE, TYPE_CLIENT, NOM_CLIENT, DATE_NAISSANCE, DATE_SOUSCRIPTION)
VALUES
(	(select max([VILLE_PK]) from [ODE_DATAWAREHOUSE].[DIM_VILLES]), -- VILLE_FK	int
	0.0, -- TAUX_REMISE	decimal(6, 2)
	'N', -- TYPE_CLIENT	char(1)
	'Bernard', -- NOM_CLIENT	nvarchar(256)
	getdate(), -- DATE_NAISSANCE	date
	getdate()), -- DATE_SOUSCRIPTION	date

(	(select max([VILLE_PK]) from [ODE_DATAWAREHOUSE].[DIM_VILLES]), -- VILLE_FK	int
	0.0, -- TAUX_REMISE	decimal(6, 2)
	'N', -- TYPE_CLIENT	char(1)
	'Brice', -- NOM_CLIENT	nvarchar(256)
	getdate(), -- DATE_NAISSANCE	date
	getdate()), -- DATE_SOUSCRIPTION	date

(	(select max([VILLE_PK]) from [ODE_DATAWAREHOUSE].[DIM_VILLES]), -- VILLE_FK	int
	0.0, -- TAUX_REMISE	decimal(6, 2)
	'N', -- TYPE_CLIENT	char(1)
	'Cedric', -- NOM_CLIENT	nvarchar(256)
	getdate(), -- DATE_NAISSANCE	date
	getdate()), -- DATE_SOUSCRIPTION	date

(	(select max([VILLE_PK]) from [ODE_DATAWAREHOUSE].[DIM_VILLES]), -- VILLE_FK	int
	0.0, -- TAUX_REMISE	decimal(6, 2)
	'N', -- TYPE_CLIENT	char(1)
	'Olivier', -- NOM_CLIENT	nvarchar(256)
	getdate(), -- DATE_NAISSANCE	date
	getdate()), -- DATE_SOUSCRIPTION	date

(	(select max([VILLE_PK]) from [ODE_DATAWAREHOUSE].[DIM_VILLES]), -- VILLE_FK	int
	0.0, -- TAUX_REMISE	decimal(6, 2)
	'N', -- TYPE_CLIENT	char(1)
	'Thomas', -- NOM_CLIENT	nvarchar(256)
	getdate(), -- DATE_NAISSANCE	date
	getdate()) -- DATE_SOUSCRIPTION	date
;



INSERT INTO [ODE_DATAWAREHOUSE].[DIM_CATEGORIES](LIBEL_UNIVERS, ID_UNIVERS, LIBEL_RAYON, ID_RAYON, LIBEL_FAMILLE, ID_FAMILLE, LIBEL_SSFAMILLE, ID_SSFAMILLE)
VALUES
(	'Univers 1', -- LIBEL_UNIVERS
	1, -- ID_UNIVERS
	'Rayon 1', -- LIBEL_RAYON
	1, -- ID_RAYON
	'Famille 1', -- LIBEL_FAMILLE
	1, -- ID_FAMILLE
	'Sous famille 1', -- LIBEL_SSFAMILLE
	1) -- ID_SSFAMILLE
;



INSERT INTO [ODE_DATAWAREHOUSE].[DIM_PRODUITS](CATEGORIE_FK, LIBEL_PRODUIT, PRIX_ACHAT, TAUX_TVA, MARQUE_PRODUIT)
VALUES
(	(select max([CATEGORIE_PK]) from [ODE_DATAWAREHOUSE].[DIM_CATEGORIES]), -- CATEGORIE_FK
	'Tondeuse a gazon CASTO-MERLIN 1CV', -- LIBEL_PRODUIT
	100.0, -- PRIX_ACHAT
	20.0, -- TAUX_TVA
	'CASTO-MERLIN'), -- MARQUE_PRODUIT

(	(select max([CATEGORIE_PK]) from [ODE_DATAWAREHOUSE].[DIM_CATEGORIES]), -- CATEGORIE_FK
	'Tondeuse a gazon CASTO-MERLIN 2CV', -- LIBEL_PRODUIT
	200.0, -- PRIX_ACHAT
	20.0, -- TAUX_TVA
	'CASTO-MERLIN') -- MARQUE_PRODUIT
;



---------------------------------------------------------------------------------
-- Le SI Decisionnel est alimente depuis environ 5 ans (01/01/2010 -> 01/01/2015)
-- Pour toutes les annees depuis 2010 
---------------------------------------------------------------------------------
SET @anneeIndex = @anneeDebut

WHILE (@anneeIndex <= @anneeFin)
BEGIN

	---------------------------------------
	-- Ventes Magasin : 9.6 millions par an
	-- Ventes Internet : 2.2 millions par an
	---------------------------------------
	
	IF(@traceOn = 1)
		PRINT 'num Ticket Max = '+ cast((@anneeIndex - @anneeDebut + 1) * (@nbrAnnuelVenteInternet + @nbrAnnuelVenteMagasin) as varchar);

	-------------------------------------
	-- Pour toutes les ventes d une année
	------------------------------------- 
	WHILE (@numTickets < (@anneeIndex - @anneeDebut + 1) * (@nbrAnnuelVenteInternet + @nbrAnnuelVenteMagasin)) -- Les numeros de tickets ne sont pas RAZ chaque annee
	BEGIN
		---------------------------------------------
		-- Increment de l index des tickets de caisse
		---------------------------------------------
		SET @numTickets = @numTickets + 1;
		
		-------------------------------------------------------
		-- Vente Magasin ou Internet ?
		-- 9.6 millions en magasin et 2.2 millions sur Internet
		-------------------------------------------------------
		IF(rand() * (@nbrAnnuelVenteInternet + @nbrAnnuelVenteMagasin) < @nbrAnnuelVenteInternet)
		BEGIN
			SET @typeLieuVente = 'I';
		END;
		ELSE
		BEGIN
			SET @typeLieuVente = 'R';
		END;
		END;

		--------------------------------------------------------------
		-- Selection aleatoire de la date de vente au cours de l annee
		--------------------------------------------------------------
		SET @moisIndex = cast( floor(12 * rand() + 1) as INT); -- Tirage du mois, entre 1 et 12
		SELECT @nbrJourMois = COUNT(*) FROM [ODE_DATAWAREHOUSE].[DIM_TEMPS] WHERE [MOIS_CODE] = (10000 * @anneeIndex + 100 * @moisIndex + 1); -- Combien de jour dans le MOIS/ANNEE  
		SET @jourIndex = cast( floor(@nbrJourMois * rand() + 1) as INT); -- Tirage du jour, entre 1 et @nbrJourMois
		SET @dateVenteFk = 10000 * @anneeIndex + 100 * @moisIndex + @jourIndex; -- Generation du FK de la forme YYYYMMDD, exemple : "20150825"


		IF(@traceOn = 1)
			PRINT '@dateVenteFk = '+ cast(@dateVenteFk as varchar);
		
		---------------------------------------------------------------------------
		-- Selection aleatoire du lieu de type R : Rayon de magasin ou I : Internet
		---------------------------------------------------------------------------
		SELECT @nbrLieu = COUNT(*) FROM [ODE_DATAWAREHOUSE].[DIM_LIEUX]; -- La PK de DimLieux etant en auto-increment à 1, les PK sont des INT consecutifs

		SET @typeLieuSelection = 'X';
		
		WHILE(@typeLieuSelection != @typeLieuVente)
		BEGIN

			SET @lieuFk = cast( floor(1 + @NbrLieu * rand()) as INT); 
			SELECT @typeLieuSelection = [TYPE_LIEU] FROM [ODE_DATAWAREHOUSE].[DIM_LIEUX] WHERE [LIEU_PK] = @lieuFk;

		END;

		--------------------------------	
		-- Selection aleatoire du client
		--------------------------------	
		SELECT @nbrClient = COUNT(*) FROM [ODE_DATAWAREHOUSE].[DIM_CLIENTS];
		SET @clientFk = cast( floor(1 + @nbrClient * rand()) as INT); 


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

		----------------------------------------------------	
		-- Pour tous les produits composant une vente donnee
		----------------------------------------------------
		SET @nbrArticlesIndex = 0; -- RAZ

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
			
			------------------------------
			-- Tirage au sort de l article
			------------------------------
			SELECT @nbrProduits = COUNT(*) FROM [ODE_DATAWAREHOUSE].[DIM_PRODUITS];
			SET @produitFk = cast( floor(1 + @nbrProduits * rand()) as INT); 

			------------------------------------------------------------------------------
			-- Nbr d articles identiques de la vente
			-- On tire un rand() entre 1 et le nombre d'articles restants pour cette vente
			------------------------------------------------------------------------------
			SET @unitesVendues = cast( floor(1 + (@nbrProduits - @nbrArticlesIndex) * rand()) as INT); 

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

		-----------------------------------------------------------------
		-- Affichage pour suivi du script tous les 1000 tickets de caisse
		-----------------------------------------------------------------
		IF(@numTickets % 1000 = 0)
		BEGIN
			PRINT 'Ticket de caisse no. ' + @numTickets;
		END;


		----------------------------------
		-- Increment de l index des annees
		----------------------------------
		SET @anneeIndex = @anneeIndex + 1;
		IF(@traceOn = 1)
			PRINT '@anneeIndex = '+ cast(@anneeIndex as varchar);

	END;

