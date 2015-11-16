/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package org.projetode;


import javax.jws.WebMethod;
import javax.jws.WebParam;
import javax.jws.WebResult;
import javax.jws.WebService;
import javax.jws.WebParam.Mode;
import java.util.*;

/**
 *
 * @author olivier.essner
 */
@WebService(endpointInterface = "org.projetode.OdeService")

public class OdeServiceImpl implements OdeService {

    DimensionUtils dimUtils = new DimensionUtils();

    
    public List<Dimension> GetCombinaisons(List<Dimension> listDim1D, List<Dimension> listCuboides)
    {
        return dimUtils.GetCombinaisons(listDim1D, listCuboides);
    }
    
    
    public List<Integer> Metropolis(List<Dimension> listCuboides, double seuil_poids, int nb_boucle)
    {
        return dimUtils.Metropolis(listCuboides, seuil_poids, nb_boucle);
    }
      
      
    public List<Integer> MaterialisationPartielle(List<Dimension> listCuboides, double seuil_poids)
    {
        return dimUtils.MaterialisationPartielle(listCuboides, seuil_poids);
    }
          
}

