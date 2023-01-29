namespace NostalgiaAnticheat
{
    partial class Main
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.lblConnectionState = new System.Windows.Forms.Label();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblGTAState = new System.Windows.Forms.Label();
            this.lnkSource = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // lblConnectionState
            // 
            this.lblConnectionState.AutoSize = true;
            this.lblConnectionState.Location = new System.Drawing.Point(12, 9);
            this.lblConnectionState.Name = "lblConnectionState";
            this.lblConnectionState.Size = new System.Drawing.Size(163, 13);
            this.lblConnectionState.TabIndex = 0;
            this.lblConnectionState.Text = "Estado da Conexão: null";
            // 
            // lblGTAState
            // 
            this.lblGTAState.AutoSize = true;
            this.lblGTAState.Location = new System.Drawing.Point(12, 22);
            this.lblGTAState.Name = "lblGTAState";
            this.lblGTAState.Size = new System.Drawing.Size(133, 13);
            this.lblGTAState.TabIndex = 1;
            this.lblGTAState.Text = "Estado do GTA: null";
            // 
            // lnkSource
            // 
            this.lnkSource.AutoSize = true;
            this.lnkSource.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkSource.Location = new System.Drawing.Point(12, 47);
            this.lnkSource.Name = "lnkSource";
            this.lnkSource.Size = new System.Drawing.Size(92, 13);
            this.lnkSource.TabIndex = 2;
            this.lnkSource.TabStop = true;
            this.lnkSource.Text = "Código Fonte";
            this.lnkSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSource_LinkClicked);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(313, 69);
            this.Controls.Add(this.lnkSource);
            this.Controls.Add(this.lblGTAState);
            this.Controls.Add(this.lblConnectionState);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.MaximizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Nostalgia - Anti-Cheat";
            this.Load += new System.EventHandler(this.Main_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblConnectionState;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Label lblGTAState;
        private System.Windows.Forms.LinkLabel lnkSource;
    }
}

