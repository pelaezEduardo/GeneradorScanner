﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalizadorLexico
{
    class Codigo
    {
        Dictionary<string, List<int>> alfabeto = new Dictionary<string, List<int>>();
        Dictionary<int, string> actions = new Dictionary<int, string>();
        List<Estado> estados = new List<Estado>();
        string outPutClass = "";

        public void setEstados(List<Estado> estados) {
            this.estados = estados;
        }

        public void setAlfabeto(Dictionary<string, List<int>> alfabeto) {
            this.alfabeto = alfabeto;
        }

        public void setActions(Dictionary<string, List<KeyValuePair<int, string>>> original) {            
            for (int i = 0; i < original.Count; i++)
            {
                List<KeyValuePair<int, string>> auxiliar = original.ElementAt(i).Value;

                for (int j = 0; j < auxiliar.Count; j++)
                {
                    actions.Add(auxiliar[j].Key, auxiliar[j].Value);
                }
            }   
        }

        public string getClass() {
            return outPutClass;
        }

        public void escribirClase() {
            outPutClass += "using System;\n" +
                           "using System.IO;\n" +
                           "using System.Collections;\n" +
                           "using System.Collections.Generic;\n\n" +
                           "class Automata {\n" +
                           "static void Main(string[] args) {\n" +
                           "\tint estado = 1;\n" +
                           "\tstring auxiliar = \"\";\n" +
                           "\tbool error = false;\n";                        
        }

        public void terminarClase() {
            outPutClass += "\n}\n}";
        }
        public void escribirConjuntos(Dictionary<string, List<int>> alfabeto) {            
            for (int i = 0; i < alfabeto.Count; i++)
            {
                outPutClass += escribirLista(alfabeto.ElementAt(i));
            }
        }

        public string escribirLista(KeyValuePair<string, List<int>> elemento) {
            return "\tList<int> " + elemento.Key.ToString() + " = new List<int> {" + string.Join(",", elemento.Value.ToArray()) + "};\n";
        }

        public void escribirInterfaz() {
            outPutClass += "\tstring path = @\"\";\n" +
                           "\tConsole.WriteLine(\"Ingrese la direccion del archivo de entrada: \");\n" +
                           "\tpath += Console.ReadLine();\n" +
                           "\tstring cadena = System.IO.File.ReadAllText(path);\n" +
                           "\n\tfor(int i = 0; i < cadena.Length; i++) {\n";
        }

        public void terminarFor() {
            outPutClass += "\t}\n\n" +
                "\tConsole.WriteLine(auxiliar + \",\" + estado.ToString() + \",no. de token\");\n\n";
        }

        public void escribirSwitch() {
            outPutClass += "\t\tswitch(estado){\n";
        }

        public void terminarSwitch() {
            outPutClass += "\t\t}\n";
        }

        public void escribirCase(int num, List<Transicion> transicion) {
            string condicion = "";
            outPutClass += "\t\t\tcase " + num.ToString() + ":\n";
            
            if (transicion.Count != 0) // si el estado tiene transiciones
            {
                for (int i = 0; i < transicion.Count; i++)
                {
                    if (alfabeto.Keys.Contains(transicion[i].simbolo))
                    {
                        condicion = transicion[i].simbolo.ToString() + ".Contains(cadena[i])";
                    }
                    else
                    {
                        condicion = "cadena[i] == " + (int)Convert.ToChar(transicion[i].simbolo);
                    }

                    //manejar formato de condicion
                    if (i == 0)
                    {
                        outPutClass += "\t\t\t\tif(" + condicion + "){\n" +
                                       "\t\t\t\t\testado = " + transicion[i].destino.numero.ToString() + ";\n" +
                                       "\t\t\t\t\tauxiliar += cadena[i];\n\t\t\t\t}\n";
                    }
                    else
                    {
                        outPutClass += "\t\t\t\telse if(" + condicion + "){\n" +
                                       "\t\t\t\t\testado = " + transicion[i].destino.numero.ToString() + ";\n" +
                                       "\t\t\t\t\tauxiliar += cadena[i];\n\t\t\t\t}\n";
                    }

                    if (transicion[0].origen.numero == 1)
                    {
                        //validacion de espacios 
                        outPutClass += "\t\t\t\telse if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){\n" +
                                       "\t\t\t\t\ti++;\n" +
                                       "\t\t\t\t}\n";
                    }
                    else
                    {
                        //validacion de espacios 
                        outPutClass += "\t\t\t\telse if(cadena[i] == 9 || cadena[i] == 10 || cadena[i] == 13 || cadena[i] == 26 || cadena[i] == 32){\n" +
                                       "\t\t\t\t\tConsole.WriteLine(auxiliar + \",\" + estado.ToString() + \",no. de token\");\n" +
                                       "\t\t\t\t\tauxiliar = \"\";\n" +
                                       "\t\t\t\t\testado = 1;\n" +
                                       "\t\t\t\t}\n";
                    }
                }

                //agregar else 
                //si es el estado inicial 
                if (transicion[0].origen.numero.Equals(1))
                {
                    outPutClass += "\t\t\t\telse{\n" +
                                   "\t\t\t\t\terror = true;\n\t\t\t\t}\n";
                }
                else
                {
                    outPutClass += "\t\t\t\telse{\n" +
                   "\t\t\t\t\t//en caso de que venga cualquier otro simbolo que no pertenece a las transiciones del estado\n" +
                   "\t\t\t\t\tConsole.WriteLine(auxiliar + \",\" + estado.ToString() + \",no. de token\");\n" +
                   "\t\t\t\t\testado = 1;\n" +
                   "\t\t\t\t\ti--;\n" +
                   "\t\t\t\t\tauxiliar = \"\";\n" +
                   "\t\t\t\t}\n";
                }

            }
            else
            {// si el estado no tiene transiciones
                outPutClass +=
               "\t\t\t\t\t//en caso de que venga cualquier otro simbolo que no pertenece a las transiciones del estado\n" +
               "\t\t\t\t\tConsole.WriteLine(auxiliar + \",\" + estado.ToString() + \",no. de token\");\n" +
               "\t\t\t\t\testado = 1;\n" +
               "\t\t\t\t\ti--;\n" +
               "\t\t\t\t\tauxiliar = \"\";\n";                   
            }


            
            outPutClass += "\t\t\tbreak;\n";
        }

        public void escribirSalida() {
            outPutClass += "\tif(error){\n" +
                           "\t\tConsole.WriteLine(\"ERROR EN EL ARCHIVO DE ENTRADA.\");\n\t}\n" +
                           "\telse{\n" +
                           "\t\tConsole.WriteLine(\"PROCESO TERMINADO CON EXITO\");\n\t}\n";
        }

        public void escribirCaseToken() { 
            
        }
    }
}
