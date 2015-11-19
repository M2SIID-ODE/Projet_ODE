/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package org.projetode;


/**
 *
 * @author olivier.essner
 */
public class Dimension
{
        // Membres public, pour être visibles sur le WSDL !
        public String dimensionName; // Nom de la dimension
        public long dimensionCount; // Nombre de lignes de la dimension
        public int dimensionMemory; // Taille d'une ligne, en octets
        public int dimensionOrder; // Indique la dimension du cuboide

        // Constructeur avec dimensionOrder comme argument
        Dimension(int inDimensionOrder)
        {
            SetDimensionName("");
            SetDimensionCount(1);
            SetDimensionMemory(0);
            this.dimensionOrder = inDimensionOrder;
        }

        // Constructeur avec (name, count, dimensionOrder) comme arguments
        Dimension(String inDimensionName, long inDimensionCount, int inDimensionOrder)
        {
            this.dimensionName = inDimensionName;
            this.dimensionCount = inDimensionCount;
            this.dimensionMemory = 0;
            this.dimensionOrder = inDimensionOrder;
        }
        
        // Default constructor
        Dimension(){}

        // Methodes
        public void SetDimensionName(String inDimensionName) { this.dimensionName = new String(inDimensionName); }
        public void SetDimensionCount(long inDimensionCount) { this.dimensionCount = inDimensionCount; }
        public void SetDimensionMemory(int inDimensionMemory) { this.dimensionMemory = inDimensionMemory; }
        // REM : Pas d'accesseur Set pour "dimensionOrder" car parametré à l'instanciation de classe

        public String GetDimensionName() { return (this.dimensionName); }
        public long GetDimensionCount() { return (this.dimensionCount); }
        public int GetDimensionMemory() { return (this.dimensionMemory); }
        public int GetDimensionOrder() { return (this.dimensionOrder); }
  }

