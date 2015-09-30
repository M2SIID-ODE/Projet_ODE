select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_CATEGORIES; 
-- Constante : 1 212 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_CLIENTS;
-- Variable : 100 000 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_LIEUX;
-- Constante : 256 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_PRODUITS;
-- Constante : 24 000 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_TEMPS;
-- Constante : 3 652 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_VILLES;
-- Constante : 28 007 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.FACT_VENTES;
-- Variable

select year(date_vente_fk), count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.FACT_VENTES group by year(date_vente_fk);
-- Répartition identique de tout FACT_VENTE de 2010 à 2015 inclus
