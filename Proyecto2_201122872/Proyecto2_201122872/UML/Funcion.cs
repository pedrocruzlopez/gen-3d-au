using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto2_201122872.UML
{
    class Funcion
    {
        private string clase;
        private string nombre;
        private string tipo;
        private LinkedList<variable> parametros;

        /*
         class Metodo:
  def __init__(self, clase, nombre, tipo, parametros):
    self.clase = clase
    self.nombre = nombre
    self.tipo = tipo
    self.parametros = parametros

  def esConstructor(self):
    return (self.nombre == self.clase.nombre and self.tipo == self.clase.nombre)

  def generarNombre(self):
    firma = ""
    for parametro in self.parametros:
      firma = "%s__%s" % (firma, parametro.tipo)
    return "%s__%s__%s%s" % (self.clase.nombre, self.nombre, self.tipo, firma)

  def generar(self):
    nombre = "void %s() {" % (self.generarNombre())
    cuerpo = ""
    if self.esConstructor() and self.clase.claseBase != None:
      constructorBase = self.clase.claseBase.devolverConstructorPorDefecto()
      if constructorBase != None:
        cuerpo = "\n  %s();\n" % (constructorBase.generarNombre())
    return "%s%s}\n" % (nombre, cuerpo)
         
         */





    }
}
