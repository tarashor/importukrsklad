﻿namespace UkrskladImporter
{
    partial class MainForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.clientsComboBox = new System.Windows.Forms.ComboBox();
            this.skladsComboBox = new System.Windows.Forms.ComboBox();
            this.pricesComboBox = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.tovars = new System.Windows.Forms.DataGridView();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.activeFirmComboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.enableFirmCheckBox = new System.Windows.Forms.CheckBox();
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.billDate = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.tovars)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.Location = new System.Drawing.Point(421, 577);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Зберегти";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 99);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Клієнт";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 126);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Склад";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 157);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Ціна";
            // 
            // clientsComboBox
            // 
            this.clientsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.clientsComboBox.FormattingEnabled = true;
            this.clientsComboBox.Location = new System.Drawing.Point(101, 96);
            this.clientsComboBox.Name = "clientsComboBox";
            this.clientsComboBox.Size = new System.Drawing.Size(395, 21);
            this.clientsComboBox.TabIndex = 4;
            this.clientsComboBox.SelectedValueChanged += new System.EventHandler(this.clientsComboBox_SelectedValueChanged);
            // 
            // skladsComboBox
            // 
            this.skladsComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.skladsComboBox.FormattingEnabled = true;
            this.skladsComboBox.Location = new System.Drawing.Point(101, 123);
            this.skladsComboBox.Name = "skladsComboBox";
            this.skladsComboBox.Size = new System.Drawing.Size(395, 21);
            this.skladsComboBox.TabIndex = 5;
            this.skladsComboBox.SelectedValueChanged += new System.EventHandler(this.skladsComboBox_SelectedValueChanged);
            // 
            // pricesComboBox
            // 
            this.pricesComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pricesComboBox.FormattingEnabled = true;
            this.pricesComboBox.Location = new System.Drawing.Point(101, 151);
            this.pricesComboBox.Name = "pricesComboBox";
            this.pricesComboBox.Size = new System.Drawing.Size(395, 21);
            this.pricesComboBox.TabIndex = 6;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.Location = new System.Drawing.Point(340, 577);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 7;
            this.button2.Text = "Завантажити";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // tovars
            // 
            this.tovars.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tovars.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.tovars.Location = new System.Drawing.Point(13, 181);
            this.tovars.Name = "tovars";
            this.tovars.Size = new System.Drawing.Size(483, 241);
            this.tovars.TabIndex = 9;
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "vnakl";
            this.openFileDialog.Filter = "Вихідна накладна (.vnakl)|*.vnakl";
            // 
            // activeFirmComboBox
            // 
            this.activeFirmComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.activeFirmComboBox.FormattingEnabled = true;
            this.activeFirmComboBox.Location = new System.Drawing.Point(101, 42);
            this.activeFirmComboBox.Name = "activeFirmComboBox";
            this.activeFirmComboBox.Size = new System.Drawing.Size(395, 21);
            this.activeFirmComboBox.TabIndex = 11;
            this.activeFirmComboBox.SelectedValueChanged += new System.EventHandler(this.activeFirmComboBox_SelectedValueChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 45);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Активна фірма";
            // 
            // enableFirmCheckBox
            // 
            this.enableFirmCheckBox.AutoSize = true;
            this.enableFirmCheckBox.Location = new System.Drawing.Point(16, 19);
            this.enableFirmCheckBox.Name = "enableFirmCheckBox";
            this.enableFirmCheckBox.Size = new System.Drawing.Size(89, 17);
            this.enableFirmCheckBox.TabIndex = 13;
            this.enableFirmCheckBox.Text = "Змінити дані";
            this.enableFirmCheckBox.UseVisualStyleBackColor = true;
            this.enableFirmCheckBox.CheckedChanged += new System.EventHandler(this.enableFirmCheckBox_CheckedChanged);
            // 
            // logTextBox
            // 
            this.logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logTextBox.Location = new System.Drawing.Point(13, 452);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logTextBox.Size = new System.Drawing.Size(483, 119);
            this.logTextBox.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 436);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 13);
            this.label5.TabIndex = 15;
            this.label5.Text = "Лог:";
            // 
            // progress
            // 
            this.progress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progress.Location = new System.Drawing.Point(13, 577);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(321, 23);
            this.progress.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 73);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "Дата";
            // 
            // billDate
            // 
            this.billDate.Location = new System.Drawing.Point(101, 69);
            this.billDate.Name = "billDate";
            this.billDate.Size = new System.Drawing.Size(395, 20);
            this.billDate.TabIndex = 18;
            this.billDate.ValueChanged += new System.EventHandler(this.billDate_ValueChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(508, 612);
            this.Controls.Add(this.billDate);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.logTextBox);
            this.Controls.Add(this.enableFirmCheckBox);
            this.Controls.Add(this.activeFirmComboBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tovars);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.pricesComboBox);
            this.Controls.Add(this.skladsComboBox);
            this.Controls.Add(this.clientsComboBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Name = "MainForm";
            this.Text = "Зберігання в УКРСКЛАД";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.tovars)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox clientsComboBox;
        private System.Windows.Forms.ComboBox skladsComboBox;
        private System.Windows.Forms.ComboBox pricesComboBox;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView tovars;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ComboBox activeFirmComboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox enableFirmCheckBox;
        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DateTimePicker billDate;
    }
}

