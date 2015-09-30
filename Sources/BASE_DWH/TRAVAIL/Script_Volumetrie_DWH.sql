/*=====================================================================================
  
  Fichier:     Script_Volumetrie_DWH.sql

  Résumé:  Réalise le comptage du cube sur les dimensions {CLIENT, LIEU, DATE, PRODUIT}
  Date:     02/08/2015
  Updated:  

  SQL Server Version: 2014
  
======================================================================================*/


-->> NOTE: THIS SCRIPT MUST BE RUN IN SQLCMD MODE INSIDE SQL SERVER MANAGEMENT STUDIO. <<--
-- Menu "QUERY" > "SQLCMD MODE"
:on error exit


-- Connexion à la base
USE [DataWarehouseODE];
SET nocount ON;
GO

select V.CLIENT_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.CLIENT_FK
print 'Nombre de tuples {Client} : ' + cast(@@ROWCOUNT as varchar);

select V.LIEU_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.LIEU_FK
print 'Nombre de tuples {Lieu} : ' + cast(@@ROWCOUNT as varchar);

select V.DATE_VENTE_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.DATE_VENTE_FK
print 'Nombre de tuples {Date} : ' + cast(@@ROWCOUNT as varchar);

select V.PRODUIT_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.PRODUIT_FK
print 'Nombre de tuples {Produit} : ' + cast(@@ROWCOUNT as varchar);

select V.CLIENT_FK, V.LIEU_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by  V.CLIENT_FK, V.LIEU_FK
print 'Nombre de tuples {Client – Lieu} : ' + cast(@@ROWCOUNT as varchar);

select V.CLIENT_FK, V.DATE_VENTE_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.CLIENT_FK, V.DATE_VENTE_FK
print 'Nombre de tuples {Client – Date} : ' + cast(@@ROWCOUNT as varchar);

select V.CLIENT_FK, V.PRODUIT_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.CLIENT_FK, V.PRODUIT_FK
print 'Nombre de tuples {Client – Produit} : ' + cast(@@ROWCOUNT as varchar);

select V.LIEU_FK, V.DATE_VENTE_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.LIEU_FK, V.DATE_VENTE_FK
print 'Nombre de tuples {Lieu – Date} : ' + cast(@@ROWCOUNT as varchar);

select V.LIEU_FK, V.PRODUIT_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.LIEU_FK, V.PRODUIT_FK
print 'Nombre de tuples {Lieu – Produit} : ' + cast(@@ROWCOUNT as varchar);

select V.DATE_VENTE_FK, V.PRODUIT_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.DATE_VENTE_FK, V.PRODUIT_FK
print 'Nombre de tuples {Date – Produit} : ' + cast(@@ROWCOUNT as varchar);

select V.CLIENT_FK, V.DATE_VENTE_FK, V.LIEU_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.CLIENT_FK, V.DATE_VENTE_FK, V.LIEU_FK
print 'Nombre de tuples {Client – Date – Lieu} : ' + cast(@@ROWCOUNT as varchar);

select V.LIEU_FK, V.PRODUIT_FK, V.CLIENT_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by  V.LIEU_FK, V.PRODUIT_FK, V.CLIENT_FK
print 'Nombre de tuples {Lieu – Produit – Client} : ' + cast(@@ROWCOUNT as varchar);

select V.DATE_VENTE_FK, V.PRODUIT_FK, V.LIEU_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.DATE_VENTE_FK, V.PRODUIT_FK, V.LIEU_FK
print 'Nombre de tuples {Date – Produit – Lieu} : ' + cast(@@ROWCOUNT as varchar);

select V.DATE_VENTE_FK, V.PRODUIT_FK, V.CLIENT_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.DATE_VENTE_FK, V.PRODUIT_FK, V.CLIENT_FK
print 'Nombre de tuples {Date – Produit – Client} : ' + cast(@@ROWCOUNT as varchar);

select V.LIEU_FK, V.PRODUIT_FK, V.CLIENT_FK, V.DATE_VENTE_FK, COUNT(*) from ODE_DATAWAREHOUSE.FACT_VENTES V group by V.LIEU_FK, V.PRODUIT_FK, V.CLIENT_FK, V.DATE_VENTE_FK
print 'Nombre de tuples {Lieu – Produit – Client - Date} : ' + cast(@@ROWCOUNT as varchar);

GO