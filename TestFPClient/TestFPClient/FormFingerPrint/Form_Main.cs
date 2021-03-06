﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using DPUruNet;
using System.Reflection;
using System.ComponentModel;
using FormFingerPrint;
using FuncionesLogicas;

namespace UareUSampleCSharp
{
    public partial class Form_Main : Form
    {

        /// <summary>
        /// Holds fmds enrolled by the enrollment GUI.
        /// </summary>
        public Dictionary<int, Fmd> Fmds
        {
            get { return fmds; }
            set { fmds = value; }
        }
        private Dictionary<int, Fmd> fmds = new Dictionary<int, Fmd>();
        
        /// <summary>
        /// Reset the UI causing the user to reselect a reader.
        /// </summary>
        public bool Reset
        {
            get { return reset; }
            set { reset = value; }
        }
        private bool reset;

        public Form_Main()
        {
            InitializeComponent();
        }

        // When set by child forms, shows s/n and enables buttons.
        public Reader CurrentReader
        {
            get { return currentReader; }
            set
            {
                currentReader = value;
                SendMessage(Action.UpdateReaderState, value);
            }
        }
        private Reader currentReader;

        #region Click Event Handlers
        private ReaderSelection _readerSelection;
        private Verification _verification;


        private void SeleccionarLector()
        {
            if (_readerSelection == null)
            {
                _readerSelection = new ReaderSelection();
                _readerSelection.Sender = this;
            }
            
            _readerSelection.ShowDialog();

            _readerSelection.Dispose();
            _readerSelection = null;
        }
        private void VerifyHuella()
        {
            if (_verification == null)
            {
                _verification = new Verification();
                _verification._sender = this;
            }

            _verification.ShowDialog();

            _verification.Dispose();
            _verification = null;
        }
      

        #endregion

        /// <summary>
        /// Open a device and check result for errors.
        /// </summary>
        /// <returns>Returns true if successful; false if unsuccessful</returns>
        public bool OpenReader()
        {
            reset = false;
            Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;

            // Open reader
            try
            {
                result = currentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

                if (result != Constants.ResultCode.DP_SUCCESS)
                {
                    MessageBox.Show("Error:  " + result);
                    reset = true;
                    return false;
                }
                
            }
            catch (Exception ex)
            {

            }
            return true;
        }

        /// <summary>
        /// Hookup capture handler and start capture.
        /// </summary>
        /// <param name="OnCaptured">Delegate to hookup as handler of the On_Captured event</param>
        /// <returns>Returns true if successful; false if unsuccessful</returns>
        public bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
        {
            try
            {
                currentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);

                // Call capture
                if (!CaptureFingerAsync())
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                
            }
            // Activate capture handler
            

            return true;
        }

        /// <summary>
        /// Cancel the capture and then close the reader.
        /// </summary>
        /// <param name="OnCaptured">Delegate to unhook as handler of the On_Captured event </param>
        public void CancelCaptureAndCloseReader(Reader.CaptureCallback OnCaptured)
        {
            
            if (currentReader != null)
            {
                currentReader.CancelCapture();

                // Dispose of reader handle and unhook reader events.
                currentReader.Dispose();

                if (reset)
                {
                    CurrentReader = null;
                }
            }
        }

        /// <summary>
        /// Check the device status before starting capture.
        /// </summary>
        /// <returns></returns>
        public void GetStatus()
        {
            Constants.ResultCode result = currentReader.GetStatus();

            if ((result != Constants.ResultCode.DP_SUCCESS))
            {
                reset = true;
                throw new Exception("" + result);   
            }

            if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_BUSY))
            {
                Thread.Sleep(50);
            }
            else if ((currentReader.Status.Status == Constants.ReaderStatuses.DP_STATUS_NEED_CALIBRATION))
            {
                currentReader.Calibrate();
            }
            else if ((currentReader.Status.Status != Constants.ReaderStatuses.DP_STATUS_READY))
            {
                throw new Exception("Reader Status - " + currentReader.Status.Status);   
            }
        }
        
        /// <summary>
        /// Check quality of the resulting capture.
        /// </summary>
        public bool CheckCaptureResult(CaptureResult captureResult)
        {
            if (captureResult.Data == null)
            {
                if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
                {
                    reset = true;
                    throw new Exception(captureResult.ResultCode.ToString());
                }

                // Send message if quality shows fake finger
                if ((captureResult.Quality != Constants.CaptureQuality.DP_QUALITY_CANCELED))
                {
                    throw new Exception("Quality - " + captureResult.Quality);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Function to capture a finger. Always get status first and calibrate or wait if necessary.  Always check status and capture errors.
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public bool CaptureFingerAsync()
        {
            try
            {
                GetStatus();

                Constants.ResultCode captureResult = currentReader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, currentReader.Capabilities.Resolutions[0]);
                if (captureResult != Constants.ResultCode.DP_SUCCESS)
                {
                    reset = true;
                    throw new Exception("" + captureResult);            
                }

                return true;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error:  " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Create a bitmap from raw data in row/column format.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        //public Bitmap CreateBitmap(byte[] bytes, int width, int height)
        //{
        //    byte[] rgbBytes = new byte[bytes.Length * 3];

        //    for (int i = 0; i <= bytes.Length - 1; i++)
        //    {
        //        rgbBytes[(i * 3)] = bytes[i];
        //        rgbBytes[(i * 3) + 1] = bytes[i];
        //        rgbBytes[(i * 3) + 2] = bytes[i];
        //    }
        //    Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);

        //    BitmapData data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);

        //    for (int i = 0; i <= bmp.Height - 1; i++)
        //    {
        //        IntPtr p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
        //        System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
        //    }

        //    bmp.UnlockBits(data);

        //    return bmp;
        //}

        #region SendMessage
        private enum Action
        {
           UpdateReaderState 
        }
        private delegate void SendMessageCallback(Action state, object payload);
        private void SendMessage(Action state, object payload)
        {
            if (this.txtReaderSelected.InvokeRequired)
            {
                SendMessageCallback d = new SendMessageCallback(SendMessage);
                this.Invoke(d, new object[] { state, payload });
            }
            else
            {
                switch (state)
                {
                    case Action.UpdateReaderState:
                        if ((Reader)payload != null)
                        {
                            txtReaderSelected.Text = ((Reader)payload).Description.SerialNumber;                           
                        }
                        else
                        {
                            txtReaderSelected.Text = String.Empty;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        private void Form_Main_Load(object sender, EventArgs e)
        {
            //metodo que abre el form de ReaderSelection que es donde detecta busca y selecciona si hay lector
            SeleccionarLector();

            //si el lector es diferente de null se activa el metodo que abre el verificationform
            if (currentReader != null)
            {               
                VerifyHuella();
                
                ///si intenta salir del formulario sin presionar el boton de salir de la aplicacion, el seguira cargando
                ///este load del main, lo sabemos porque txtReaderSelected lo usamos desde el form Verification y desde
                ///alla mandamos el switch con el que identificaremos si cerro el form o la application
                if (txtReaderSelected.Text == "CerrandoVerificationForm")
                {
                    Form_Main_Load(sender, e);
                }
            }

            ///si no es porque el lector es null y no esta conectado o no lo detecta, y mandamos un messagebox para
            ///preguntarle al usuario si desea volver a cargar el form o desea salirse del sistema
            else
            {
               DialogResult r = MessageBox.Show("Lector no encontrado", "Pesaje", MessageBoxButtons.RetryCancel, MessageBoxIcon.Exclamation) ;

                //si presiona Reintentar vuelve a cargar el load del main 
                if (r == DialogResult.Retry)
                {
                    Form_Main_Load(sender, e);
                }

                //si presiona cancelar se sale del sistema
                else if (r == DialogResult.Cancel)
                {
                    this.Close();
                }
            }
          
        }

    }
}