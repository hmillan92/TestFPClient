using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPUruNet;
using Entidades;
using HM_DataLayer;

namespace FuncionesLogicas
{

    public class Funciones
    {
        DBTrasaccion transaccion = new DBTrasaccion();
        Helper oHelper = new Helper();
        private const int PROBABILITY_ONE = 0x7fffffff;
              

        public bool ValidaConexionSQL()
        {          
            bool Exitosa = transaccion.ValidaConexionSQL();
            return Exitosa;
        }

        public string CrearOperador(Fmd presult, string pcodOperador, string pnombre,string pstatus)
        {
            Operador oOperador;
            string Mensaje;
            string huellaConvertida;

            oOperador = CompararHuella(presult, presult);

            if (oOperador.IdOperador == 0)
            {
                huellaConvertida = Convert.ToBase64String(presult.Bytes);

                oOperador = new Operador();
                oOperador.CodOperador = pcodOperador;
                oOperador.Nombre = pnombre;
                oOperador.Huella = huellaConvertida;
                oOperador.Status = pstatus;

                Mensaje = transaccion.CrearOperador(oOperador);
            }

            else
            {
                Mensaje = "Ya existe un operador registrado con esta huella";
            }
          
            return Mensaje;
        }

        public Operador CompararHuella(Fmd firstF, Fmd secondF)
        {
            ///Añadimos el parametro firstF a firstFinger un objeto de tipo Fmd que sera el objeto de la huella capturada
            Fmd firstFinger = firstF;
            
            ///creamos el segundo objeto de tipo Fmd y le pasamos los parametros que pide el constructor de esa clase, los parametros son tomados del objeto 1
            ///se hace esto porque la propiedad bytes de un mismo objeto al ser cambiada en una variable, se cambia en todas las demas como si fuese una variable estatica
            ///y necesitamos resetear solo los bytes para poder asignarle los bytes que se encuentran en la BD al objeto 2 para compararlas 
            ///solo necesitamos cambiar los bytes porque los otros parametros que pide el constructor y que el toma del objeto 1 no los vamos a necesitar para hacer la comparacion.
            Fmd secondFinger = new Fmd(firstFinger.Bytes, oHelper.Format , oHelper.Version);

            int count = 0;
            int idOp = 0;
            string codOp = string.Empty;
            string nombreOp = string.Empty;
            string statusOP = string.Empty;
            
            List<Operador> ListaHuellas = transaccion.ListarHuellas();

            ///un foreach para capturar los valores de cada registro
            foreach (var item in ListaHuellas)
            {

                ///los bytes que estan registrados en la BD son de tipo Base64 que es un varchar(max), le pasamos ese string al metodo convert
                ///para que nos convierta de nuevo a bytes y se la asignamos a la variable huellaConvertida
                byte[] huellaConvertida = Convert.FromBase64String(item.Huella);

                //le asignamos los bytes de huellaConvertida que representan los bytes que estan en la BD a la propiedad Bytes del objeto secondFinger para asi poder hacer la comparacion con el obj 1 que es la huella capturada
                secondFinger.Bytes = huellaConvertida;

                ///Una vez listo los 2 objetos a comparar se lo mandamos al metodo Compare de la clase Comparision, nos devolvera un objeto de tipo CompareResult
                ///Este objeto compareResult tendra una propiedad que se llama Score que es el puntaje que necesitaremos para ver si ambas huellas tienen similitud
                CompareResult compareResult = Comparison.Compare(firstFinger, 0, secondFinger, 0);
                
                //preguntamos si el Score es menor a la operacion establecida en el if, si el resultado es true entonces ambas huellas tienen similitud
                if (compareResult.Score < (PROBABILITY_ONE / 100000))
                {
                    count++;
                    idOp = item.IdOperador;
                    codOp = item.CodOperador;
                    nombreOp = item.Nombre;
                    statusOP = item.Status;

                }
            }

            Operador operadorEncontrado = new Operador();
            if (count > 0)
            {
                operadorEncontrado = new Operador();
                operadorEncontrado.IdOperador = idOp;
                operadorEncontrado.CodOperador = codOp;
                operadorEncontrado.Nombre = nombreOp;
                operadorEncontrado.Status = statusOP;
            }
           
            return operadorEncontrado;
        }

        public List<Operador> ListarHuellas()
        {
            List<Operador> listarOp = transaccion.ListarHuellas();

            return listarOp;
        }
    }
}
