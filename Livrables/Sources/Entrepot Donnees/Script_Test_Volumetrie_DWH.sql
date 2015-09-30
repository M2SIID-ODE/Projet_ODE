select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_CATEGORIES; 
-- 1 212 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_CLIENTS;
-- 100 000 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_LIEUX;
-- 256 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_PRODUITS;
-- 24 000 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_TEMPS;
-- 3 652 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.DIM_VILLES;
-- 28 007 lignes

select count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.FACT_VENTES;
-- 5 260 144 lignes

select year(date_vente_fk), count(*) from DataWarehouseODE.ODE_DATAWAREHOUSE.FACT_VENTES group by year(date_vente_fk);
-- 2010 : 876554 lignes
-- 2011 : 876247 lignes
-- 2012 : 876792 lignes
-- 2013 : 876379 lignes
-- 2014 : 877432 lignes
-- 2015 : 876740 lignes
