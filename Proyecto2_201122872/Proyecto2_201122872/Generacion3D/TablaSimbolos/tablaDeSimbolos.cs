using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Proyecto2_201122872.Generacion3D.TablaSimbolos
{
    public class tablaDeSimbolos
    {

       public List<Simbolo> tabla;


        public tablaDeSimbolos()
        {
            this.tabla = new List<Simbolo>();
        }

        public Boolean addSimbolo(Simbolo nuevo)
        {

            return false;

        }


        public void addLista(List<Simbolo> lista)
        {
            foreach (Simbolo item in lista)
            {
                tabla.Add(item);
            }
        }


        public int sizeClase(String nombre)
        {
            foreach (Simbolo simb in this.tabla)
            {
                if(simb.tipoElemento.Equals("clase", StringComparison.OrdinalIgnoreCase) &&
                    simb.rol.Equals("clase", StringComparison.OrdinalIgnoreCase) &&
                    simb.nombreReal.Equals(nombre, StringComparison.OrdinalIgnoreCase))
                {
                    return simb.tamanho;
                }
            }
            return -1;
        }


        public int sizeFuncion(String nombreClase, String firmaMetodo)
        {
            foreach ( Simbolo item in this.tabla)
            {
                if(item.ambito.Equals(nombreClase, StringComparison.OrdinalIgnoreCase) &&
                    item.nombreReal.Equals(firmaMetodo, StringComparison.OrdinalIgnoreCase))
                {
                    return item.tamanho;
                }
                
            }
            return -1;
        }


        public List<List<String>> existeConstructor(string nombreClase, int noParametros)
        {
            List<List<String>> listas = new List<List<string>>();
            foreach (Simbolo item in this.tabla)
            {
                if (item.rol.Equals(Constantes3D.constructor, StringComparison.OrdinalIgnoreCase) &&
                    item.nombreReal.Equals(nombreClase,StringComparison.OrdinalIgnoreCase) &&
                    item.tamanhoParametros==noParametros)
                {
                    //listas.Add(item.tipoParametros);
                }
                
            }
            return listas;
        }
      

        public int getPosicionDeClase(string id, Ambitos ambito)//buca la posicion de un id dentro de los atributos
        {
            for (int i = 0; i < ambito.ambitos.Count; i++)
            {
                if (i == ambito.ambitos.Count - 1)
                {
                    string amb = ambito.ambitos.ElementAt(i);
                    foreach (Simbolo simb in this.tabla)
                    {
                        if (simb.ambito.Equals(amb, StringComparison.OrdinalIgnoreCase) &&
                            simb.nombreReal.Equals(id, StringComparison.OrdinalIgnoreCase))
                        {
                            return simb.apuntador;
                        }
                    }

                }

            }



            return -1;

        }
        public int getPosicion(string id, Ambitos ambito) //busca la posicion de un id dentro de un ambiente local
        {
            for (int i = 0; i < ambito.ambitos.Count; i++)
            {
                if (i != ambito.ambitos.Count - 1)
                {
                    string amb = ambito.ambitos.ElementAt(i);
                    foreach (Simbolo simb in this.tabla)
                    {
                        if (simb.ambito.Equals(amb, StringComparison.OrdinalIgnoreCase) &&
                            simb.nombreReal.Equals(id, StringComparison.OrdinalIgnoreCase))
                        {
                            return simb.apuntador;
                        }
                    }

                }
                    
            }
            
            
           
            return -1;
        }



        public List<Simbolo> obtenerAtributosClase(String nombreClase)
        {
            List<Simbolo> listaRetorno = new List<Simbolo>();
            foreach (Simbolo item in this.tabla)
            {
                if(item.rol.Equals(Constantes3D.variableDeClase, StringComparison.OrdinalIgnoreCase) &&
                    item.ambito.Equals(nombreClase, StringComparison.OrdinalIgnoreCase))
                {
                    listaRetorno.Add(item);
                }
                
            }
            return listaRetorno;
        }



        public string getFirmaMetodo(String nombreClase, String cadenaParametros, string nombreMetodo)
        {
           
            foreach (Simbolo item in tabla)
            {
               
                Console.WriteLine(item.ambito + " " + item.tipoParametrosCadena + "  " + item.nombreFuncion);
                if(item.ambito.Equals(nombreClase,StringComparison.OrdinalIgnoreCase) &&
                    item.tipoParametrosCadena.ToUpper().Equals(cadenaParametros.ToUpper(),StringComparison.OrdinalIgnoreCase) &&
                    item.nombreFuncion.Equals(item.ambito, StringComparison.OrdinalIgnoreCase))
                {
                    return item.nombreReal;
                }
                
            }
            return "";
        }

        public string getTipo(string id, Ambitos ambito) //obtiene el tipo de una variable
        {
            foreach (String amb in ambito.ambitos)
            {
                foreach (Simbolo simb in this.tabla)
                {
                    if (simb.ambito.Equals(amb, StringComparison.OrdinalIgnoreCase) &&
                        simb.nombreReal.Equals(id, StringComparison.OrdinalIgnoreCase))
                    {
                        return simb.tipo;
                    }
                }

            }
            return "";
        }



    }
}
