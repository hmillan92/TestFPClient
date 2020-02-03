using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using DPUruNet;
using Entidades;
using FuncionesLogicas;

namespace UareUSampleCSharp
{
    public partial class Verification : Form
    {
        /// <summary>
        /// Holds the main form with many functions common to all of SDK actions.
        /// </summary>
        public Form_Main _sender;

        private const int PROBABILITY_ONE = 0x7fffffff;
        private Fmd firstFinger;
        private Fmd secondFinger;
        private int count;
        private Operador OperadorEncontrado = new Operador();
        Funciones funciones = new Funciones();
        Helper oHelper = new Helper();

        public Verification()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Verification_Load(object sender, System.EventArgs e)
        {
            txtVerify.Text = string.Empty;
            firstFinger = null;
            secondFinger = null;
            count = 0;

            SendMessage(Action.SendMessage, oHelper.Mensaje1);

            if (!_sender.OpenReader())
            {
                this.Close();
            }

            if (!_sender.StartCaptureAsync(this.OnCaptured))
            {
                this.Close();
            }
        }

        /// <summary>
        /// Handler for when a fingerprint is captured.
        /// </summary>
        /// <param name="captureResult">contains info and data on the fingerprint capture</param>
        private void OnCaptured(CaptureResult captureResult)
        {
            try
            {
                // Check capture quality and throw an error if bad.
                if (!_sender.CheckCaptureResult(captureResult)) return;

                SendMessage(Action.SendMessage, oHelper.Mensaje2);

                DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);
                if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    _sender.Reset = true;
                    throw new Exception(resultConversion.ResultCode.ToString());
                }

                if (count == 0)
                {

                    firstFinger = resultConversion.Data;
                        
                    OperadorEncontrado = funciones.CompararHuella(firstFinger, secondFinger);

                    if (OperadorEncontrado.IdOperador != 0)
                    {
                        SendMessage(Action.SendMessage, "Huella coincide con operador " + OperadorEncontrado.Nombre+ " y su status es "+ OperadorEncontrado.Status);
                        SendMessage(Action.SendMessage, oHelper.Mensaje1);
                    }

                    else
                    {
                        SendMessage(Action.SendMessage, oHelper.Mensaje5);
                        SendMessage(Action.SendMessage, oHelper.Mensaje1);
                    }
                    
                    count = 0;
                }
            }
            catch (Exception ex)
            {
                // Send error message, then close form
                SendMessage(Action.SendMessage, oHelper.Error + ex.Message);

            }

            ///si el lector es desconectado con la ventana de verification abierta, mandamos a cerrarla automaticamente
            ///para llamar al evento Verification_Closed y que se cargue el load del main para que salga el mensajeBox de lector
            ///desconectado (Reintentar/Salir)
            if (captureResult.ResultCode == Constants.ResultCode.DP_DEVICE_FAILURE)
            {
                Thread t = new Thread(cerrar);
                t.Start();
            }
        }

        /// <summary>
        /// Close window.
        /// </summary>
        private void btnBack_Click(System.Object sender, System.EventArgs e)
        {

             Application.Exit();
           
        }
        
        /// <summary>
        /// Close window.
        /// </summary>

        private void Verification_Closed(object sender, System.EventArgs e)
        {
            _sender.CancelCaptureAndCloseReader(this.OnCaptured);
            ///cuando el form verification es cerrado con this.close(), es decir sin cerrar la aplicacion, el manda
            ///este parametro al sender que esta en el main y nos servira de switche para hacer la condicion en el main
            ///que nos ayudara a capturar que paso por este metodo de cerrar ventana y no de salir de la aplicacion y
            ///volver a cargar el evento load del main 
            _sender.txtReaderSelected.Text = "CerrandoVerificationForm";
        }

        void cerrar()
        {
            if (InvokeRequired)
            {
                Invoke(new System.Action(()=>this.Close()));
            }
        }


        #region SendMessage
        private enum Action
        {
            SendMessage
        }
        private delegate void SendMessageCallback(Action action, string payload);
        private void SendMessage(Action action, string payload)
        {
            try
            {
                if (this.txtVerify.InvokeRequired)
                {
                    SendMessageCallback d = new SendMessageCallback(SendMessage);
                    this.Invoke(d, new object[] { action, payload });
                }
                else
                {
                    switch (action)
                    {
                        case Action.SendMessage:
                            txtVerify.Text += payload + "\r\n\r\n";
                            txtVerify.SelectionStart = txtVerify.TextLength;
                            txtVerify.ScrollToCaret();
                            break;
                    }
                }
            }
            catch (Exception)
            {
            }
        }
        #endregion

        private void btnConectar_Click(object sender, EventArgs e)
        {
            bool msj;
            Funciones funciones = new Funciones();
            msj = funciones.ValidaConexionSQL();

            if (msj != true)
            {
                MessageBox.Show("Error al conectar con el servidor.","Pesaje",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }

            else
            {
                MessageBox.Show("Conexion con el servidor exitosa.", "Pesaje", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}