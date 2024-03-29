﻿using System.Windows.Forms;

namespace WorldsHardestGameView.MainGameForm
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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


        private Timer updateTimer;

        private const int screenWidth = 1440;
        private const int screenHeight = 800;

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(screenWidth, screenHeight);
            this.Text = "World's Hardest Game";
            this.FormBorderStyle = FormBorderStyle.Fixed3D;

            this.updateTimer = new Timer(this.components);
            this.updateTimer.Interval = 40;
            this.updateTimer.Tick += new System.EventHandler(this.UpdateTimer_Tick);
        }

        #endregion
    }
}

