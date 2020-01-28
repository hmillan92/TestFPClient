namespace UareUSampleCSharp
{
    partial class Form_Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtReaderSelected = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.btnReaderSelect = new System.Windows.Forms.Button();
            this.btnConectar = new System.Windows.Forms.Button();
            this.btnVerify = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtReaderSelected
            // 
            this.txtReaderSelected.Font = new System.Drawing.Font("Tahoma", 8F);
            this.txtReaderSelected.Location = new System.Drawing.Point(15, 27);
            this.txtReaderSelected.Name = "txtReaderSelected";
            this.txtReaderSelected.ReadOnly = true;
            this.txtReaderSelected.Size = new System.Drawing.Size(233, 20);
            this.txtReaderSelected.TabIndex = 7;
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(12, 9);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(236, 15);
            this.Label1.TabIndex = 18;
            this.Label1.Text = "Selected Reader:";
            // 
            // btnReaderSelect
            // 
            this.btnReaderSelect.Location = new System.Drawing.Point(15, 53);
            this.btnReaderSelect.Name = "btnReaderSelect";
            this.btnReaderSelect.Size = new System.Drawing.Size(233, 23);
            this.btnReaderSelect.TabIndex = 8;
            this.btnReaderSelect.Text = "Reader Selection";
            this.btnReaderSelect.Click += new System.EventHandler(this.btnReaderSelect_Click);
            // 
            // btnConectar
            // 
            this.btnConectar.Enabled = false;
            this.btnConectar.Location = new System.Drawing.Point(136, 90);
            this.btnConectar.Name = "btnConectar";
            this.btnConectar.Size = new System.Drawing.Size(112, 23);
            this.btnConectar.TabIndex = 21;
            this.btnConectar.Text = "ConectarBd";
            this.btnConectar.UseVisualStyleBackColor = true;
            this.btnConectar.Click += new System.EventHandler(this.btnConectar_Click);
            // 
            // btnVerify
            // 
            this.btnVerify.Enabled = false;
            this.btnVerify.Location = new System.Drawing.Point(12, 90);
            this.btnVerify.Name = "btnVerify";
            this.btnVerify.Size = new System.Drawing.Size(115, 23);
            this.btnVerify.TabIndex = 23;
            this.btnVerify.Text = "Verificar Huella.";
            this.btnVerify.Click += new System.EventHandler(this.btnVerify_Click);
            // 
            // Form_Main
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(257, 130);
            this.Controls.Add(this.btnVerify);
            this.Controls.Add(this.btnConectar);
            this.Controls.Add(this.txtReaderSelected);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.btnReaderSelect);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "U.are.U Sample C#";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.TextBox txtReaderSelected;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.Button btnReaderSelect;
        private System.Windows.Forms.Button btnConectar;
        internal System.Windows.Forms.Button btnVerify;
    }
}