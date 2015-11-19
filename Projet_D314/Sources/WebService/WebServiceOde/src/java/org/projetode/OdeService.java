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
import javax.jws.soap.SOAPBinding;
import javax.jws.soap.SOAPBinding.ParameterStyle;
import javax.jws.soap.SOAPBinding.Style;
import javax.jws.soap.SOAPBinding.Use;
import javax.xml.bind.annotation.XmlSeeAlso;
import java.util.*;

/**
 *
 * @author olivier.essner
 */
@WebService
@SOAPBinding(style=Style.DOCUMENT,use=Use.LITERAL, parameterStyle=ParameterStyle.WRAPPED)

public interface OdeService
{  
    @WebMethod
    public List<Dimension> GetCombinaisons(List<Dimension> listDim1D);
    
    @WebMethod
    public boolean TestGetCombinaisons();
    
    @WebMethod
    public List<String> GetCombinaisonsIndex(List<Dimension> listDim1D);
    
    @WebMethod
    public boolean TestGetCombinaisonsIndex();
    
    @WebMethod
    public List<Integer> Metropolis(List<Dimension> listCuboides, double seuil_poids, int nb_boucle);
    
    @WebMethod
    public boolean TestMetropolis();
    
    @WebMethod
    public List<Integer> MaterialisationPartielle(List<Dimension> listCuboides, double seuil_poids);
    
    @WebMethod
    public boolean TestMaterialisationPartielle();
}

