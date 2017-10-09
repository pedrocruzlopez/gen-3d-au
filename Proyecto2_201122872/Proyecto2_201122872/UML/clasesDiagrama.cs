using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
   public  class clasesDiagrama
    {
        public List<Clase> listaClases;
        public Clase claseActual;


        public clasesDiagrama()
        {
            this.claseActual = new Clase();
            this.listaClases = new List<Clase>();
        }


        private Boolean existeClase(String nombre)
        {
            foreach (Clase item in this.listaClases)
            {
                if (item.getNombre().ToUpper().Equals(nombre.ToUpper()))
                    return true;
            }

            return false;
        }


        public Boolean insertarClase(Clase nueva)
        {
            if(!existeClase(nueva.getNombre()))
            {
                this.listaClases.Add(nueva);
                return true;
            }
            return false;
        }


        public Boolean actualizarClase(Clase actualizar)
        {
            Clase temporal;
            for (int i = 0; i < this.listaClases.Count; i++)
            {
                temporal = this.listaClases.ElementAt(i);
                if (temporal.getNombre().ToUpper().Equals(actualizar.getNombre().ToUpper()))
                {
                    this.listaClases.RemoveAt(i);
                    this.listaClases.Add(actualizar);
                    return true;
                }
            }

            return false;
        }



        public Boolean seleccionarClaseActual(String nombreClase)
        {
            Clase temporal;
            if(this.claseActual.getNombre()!=null)
                actualizarClase(this.claseActual);

            for (int i = 0; i < this.listaClases.Count; i++)
            {
                temporal = this.listaClases.ElementAt(i);
                if (temporal.getNombre().ToUpper().Equals(nombreClase.ToUpper()))
                {
                    this.claseActual = temporal;
                    return true;
                }
                
            }

            return false;
        }




        public int getSize()
        {
            return this.listaClases.Count;
        }



       /*    Atributos    */

        public Boolean addAtributoActual(Atributo atr)
        {
            return this.claseActual.addAtributo(atr);
        }



        private string getCadenaGraphiz()
        {
            string cadena = "digraph structs{\n node [shape= record];";
            Clase actual;
            for (int i = 0; i < this.listaClases.Count; i++)
            {
                actual = listaClases.ElementAt(i);
                cadena += "struct" + i + actual.getCadenaGraphivz();

            }
            cadena += "\n}";

            return cadena;

        }


        public void generarGrafo()
        {
            String path = @"C:\";
            String arbolDerivacion = path + "diagrama.txt";
            String texto = getCadenaGraphiz();
            System.IO.File.WriteAllText(arbolDerivacion, texto);
            Console.WriteLine(arbolDerivacion);
            try
            {
                var command = string.Format("dot -Tjpg {0} -o {1}", "\"" + arbolDerivacion + "\"", "\"" + arbolDerivacion.Replace(".txt", ".jpg") + "\"");
                var procStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/C " + command);
                var proc = new System.Diagnostics.Process();
                proc.StartInfo = procStartInfo;
                proc.Start();
                proc.WaitForExit();


            }
            catch (Exception x)
            {
                Console.WriteLine(x.ToString());
            }
        }



    }
}
