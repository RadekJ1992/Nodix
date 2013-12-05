namespace Nodix {
    partial class Nodix {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.cloudIPField = new System.Windows.Forms.TextBox();
            this.cloudIPLabel = new System.Windows.Forms.Label();
            this.cloudPortLabel = new System.Windows.Forms.Label();
            this.cloudPortField = new System.Windows.Forms.TextBox();
            this.managerPortLabel = new System.Windows.Forms.Label();
            this.managerPortField = new System.Windows.Forms.TextBox();
            this.managerIPLabel = new System.Windows.Forms.Label();
            this.managerIPField = new System.Windows.Forms.TextBox();
            this.connectToCloudButton = new System.Windows.Forms.Button();
            this.connectToManagerButton = new System.Windows.Forms.Button();
            this.log = new System.Windows.Forms.TextBox();
            this.setNodeNumber = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.NodeNumberField = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.inPortTextBox = new System.Windows.Forms.TextBox();
            this.inVPITextBox = new System.Windows.Forms.TextBox();
            this.inVCITextBox = new System.Windows.Forms.TextBox();
            this.outVCITextBox = new System.Windows.Forms.TextBox();
            this.outVPITextBox = new System.Windows.Forms.TextBox();
            this.outPortTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.addEntryButton = new System.Windows.Forms.Button();
            this.chooseTextFile = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cloudIPField
            // 
            this.cloudIPField.Location = new System.Drawing.Point(12, 29);
            this.cloudIPField.Name = "cloudIPField";
            this.cloudIPField.Size = new System.Drawing.Size(100, 20);
            this.cloudIPField.TabIndex = 0;
            this.cloudIPField.Text = "127.0.0.1";
            // 
            // cloudIPLabel
            // 
            this.cloudIPLabel.AutoSize = true;
            this.cloudIPLabel.Location = new System.Drawing.Point(12, 13);
            this.cloudIPLabel.Name = "cloudIPLabel";
            this.cloudIPLabel.Size = new System.Drawing.Size(54, 13);
            this.cloudIPLabel.TabIndex = 1;
            this.cloudIPLabel.Text = "IP chmury";
            // 
            // cloudPortLabel
            // 
            this.cloudPortLabel.AutoSize = true;
            this.cloudPortLabel.Location = new System.Drawing.Point(12, 52);
            this.cloudPortLabel.Name = "cloudPortLabel";
            this.cloudPortLabel.Size = new System.Drawing.Size(63, 13);
            this.cloudPortLabel.TabIndex = 3;
            this.cloudPortLabel.Text = "Port chmury";
            // 
            // cloudPortField
            // 
            this.cloudPortField.Location = new System.Drawing.Point(12, 68);
            this.cloudPortField.Name = "cloudPortField";
            this.cloudPortField.Size = new System.Drawing.Size(100, 20);
            this.cloudPortField.TabIndex = 2;
            this.cloudPortField.Text = "13000";
            // 
            // managerPortLabel
            // 
            this.managerPortLabel.AutoSize = true;
            this.managerPortLabel.Location = new System.Drawing.Point(118, 52);
            this.managerPortLabel.Name = "managerPortLabel";
            this.managerPortLabel.Size = new System.Drawing.Size(71, 13);
            this.managerPortLabel.TabIndex = 7;
            this.managerPortLabel.Text = "Port zarządcy";
            // 
            // managerPortField
            // 
            this.managerPortField.Location = new System.Drawing.Point(118, 68);
            this.managerPortField.Name = "managerPortField";
            this.managerPortField.Size = new System.Drawing.Size(100, 20);
            this.managerPortField.TabIndex = 6;
            // 
            // managerIPLabel
            // 
            this.managerIPLabel.AutoSize = true;
            this.managerIPLabel.Location = new System.Drawing.Point(118, 13);
            this.managerIPLabel.Name = "managerIPLabel";
            this.managerIPLabel.Size = new System.Drawing.Size(62, 13);
            this.managerIPLabel.TabIndex = 5;
            this.managerIPLabel.Text = "IP zarządcy";
            // 
            // managerIPField
            // 
            this.managerIPField.Location = new System.Drawing.Point(118, 29);
            this.managerIPField.Name = "managerIPField";
            this.managerIPField.Size = new System.Drawing.Size(100, 20);
            this.managerIPField.TabIndex = 4;
            this.managerIPField.Text = "127.0.0.1";
            // 
            // connectToCloudButton
            // 
            this.connectToCloudButton.Location = new System.Drawing.Point(15, 95);
            this.connectToCloudButton.Name = "connectToCloudButton";
            this.connectToCloudButton.Size = new System.Drawing.Size(97, 43);
            this.connectToCloudButton.TabIndex = 8;
            this.connectToCloudButton.Text = "Połącz z chmurą";
            this.connectToCloudButton.UseVisualStyleBackColor = true;
            this.connectToCloudButton.Click += new System.EventHandler(this.connectToCloud);
            // 
            // connectToManagerButton
            // 
            this.connectToManagerButton.Location = new System.Drawing.Point(118, 95);
            this.connectToManagerButton.Name = "connectToManagerButton";
            this.connectToManagerButton.Size = new System.Drawing.Size(100, 43);
            this.connectToManagerButton.TabIndex = 9;
            this.connectToManagerButton.Text = "Połącz z zarządcą";
            this.connectToManagerButton.UseVisualStyleBackColor = true;
            this.connectToManagerButton.Click += new System.EventHandler(this.connectToManager);
            // 
            // log
            // 
            this.log.BackColor = System.Drawing.SystemColors.Window;
            this.log.Location = new System.Drawing.Point(12, 145);
            this.log.Multiline = true;
            this.log.Name = "log";
            this.log.ReadOnly = true;
            this.log.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.log.Size = new System.Drawing.Size(322, 172);
            this.log.TabIndex = 13;
            // 
            // setNodeNumber
            // 
            this.setNodeNumber.Location = new System.Drawing.Point(227, 52);
            this.setNodeNumber.Name = "setNodeNumber";
            this.setNodeNumber.Size = new System.Drawing.Size(97, 37);
            this.setNodeNumber.TabIndex = 18;
            this.setNodeNumber.Text = "Ustal numer węzła";
            this.setNodeNumber.UseVisualStyleBackColor = true;
            this.setNodeNumber.Click += new System.EventHandler(this.setNodeNumber_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(224, 13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Numer Węzła";
            // 
            // NodeNumberField
            // 
            this.NodeNumberField.Location = new System.Drawing.Point(224, 29);
            this.NodeNumberField.Name = "NodeNumberField";
            this.NodeNumberField.Size = new System.Drawing.Size(100, 20);
            this.NodeNumberField.TabIndex = 14;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 336);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 19;
            this.label1.Text = "Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(55, 320);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "WEJŚCIE:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(55, 336);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "VPI";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(101, 336);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "VCI";
            // 
            // inPortTextBox
            // 
            this.inPortTextBox.Location = new System.Drawing.Point(12, 352);
            this.inPortTextBox.Name = "inPortTextBox";
            this.inPortTextBox.Size = new System.Drawing.Size(40, 20);
            this.inPortTextBox.TabIndex = 23;
            // 
            // inVPITextBox
            // 
            this.inVPITextBox.Location = new System.Drawing.Point(58, 352);
            this.inVPITextBox.Name = "inVPITextBox";
            this.inVPITextBox.Size = new System.Drawing.Size(40, 20);
            this.inVPITextBox.TabIndex = 24;
            // 
            // inVCITextBox
            // 
            this.inVCITextBox.Location = new System.Drawing.Point(104, 352);
            this.inVCITextBox.Name = "inVCITextBox";
            this.inVCITextBox.Size = new System.Drawing.Size(40, 20);
            this.inVCITextBox.TabIndex = 25;
            // 
            // outVCITextBox
            // 
            this.outVCITextBox.Location = new System.Drawing.Point(243, 352);
            this.outVCITextBox.Name = "outVCITextBox";
            this.outVCITextBox.Size = new System.Drawing.Size(40, 20);
            this.outVCITextBox.TabIndex = 32;
            // 
            // outVPITextBox
            // 
            this.outVPITextBox.Location = new System.Drawing.Point(197, 352);
            this.outVPITextBox.Name = "outVPITextBox";
            this.outVPITextBox.Size = new System.Drawing.Size(40, 20);
            this.outVPITextBox.TabIndex = 31;
            // 
            // outPortTextBox
            // 
            this.outPortTextBox.Location = new System.Drawing.Point(151, 352);
            this.outPortTextBox.Name = "outPortTextBox";
            this.outPortTextBox.Size = new System.Drawing.Size(40, 20);
            this.outPortTextBox.TabIndex = 30;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(240, 336);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 13);
            this.label6.TabIndex = 29;
            this.label6.Text = "VCI";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(194, 336);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 13);
            this.label7.TabIndex = 28;
            this.label7.Text = "VPI";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(194, 320);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 13);
            this.label8.TabIndex = 27;
            this.label8.Text = "WYJŚCIE:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(151, 336);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(26, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Port";
            // 
            // addEntryButton
            // 
            this.addEntryButton.Location = new System.Drawing.Point(290, 323);
            this.addEntryButton.Name = "addEntryButton";
            this.addEntryButton.Size = new System.Drawing.Size(44, 48);
            this.addEntryButton.TabIndex = 33;
            this.addEntryButton.Text = "Dodaj";
            this.addEntryButton.UseVisualStyleBackColor = true;
            this.addEntryButton.Click += new System.EventHandler(this.addEntryButton_Click);
            // 
            // chooseTextFile
            // 
            this.chooseTextFile.Location = new System.Drawing.Point(227, 95);
            this.chooseTextFile.Name = "chooseTextFile";
            this.chooseTextFile.Size = new System.Drawing.Size(97, 43);
            this.chooseTextFile.TabIndex = 34;
            this.chooseTextFile.Text = "Wybierz plik konfiguracyjny";
            this.chooseTextFile.UseVisualStyleBackColor = true;
            this.chooseTextFile.Click += new System.EventHandler(this.chooseTextFile_Click);
            // 
            // Nodix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(341, 380);
            this.Controls.Add(this.chooseTextFile);
            this.Controls.Add(this.addEntryButton);
            this.Controls.Add(this.outVCITextBox);
            this.Controls.Add(this.outVPITextBox);
            this.Controls.Add(this.outPortTextBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.inVCITextBox);
            this.Controls.Add(this.inVPITextBox);
            this.Controls.Add(this.inPortTextBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.setNodeNumber);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.NodeNumberField);
            this.Controls.Add(this.log);
            this.Controls.Add(this.connectToManagerButton);
            this.Controls.Add(this.connectToCloudButton);
            this.Controls.Add(this.managerPortLabel);
            this.Controls.Add(this.managerPortField);
            this.Controls.Add(this.managerIPLabel);
            this.Controls.Add(this.managerIPField);
            this.Controls.Add(this.cloudPortLabel);
            this.Controls.Add(this.cloudPortField);
            this.Controls.Add(this.cloudIPLabel);
            this.Controls.Add(this.cloudIPField);
            this.Name = "Nodix";
            this.Text = "Nodix";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox cloudIPField;
        private System.Windows.Forms.Label cloudIPLabel;
        private System.Windows.Forms.Label cloudPortLabel;
        private System.Windows.Forms.TextBox cloudPortField;
        private System.Windows.Forms.Label managerPortLabel;
        private System.Windows.Forms.TextBox managerPortField;
        private System.Windows.Forms.Label managerIPLabel;
        private System.Windows.Forms.TextBox managerIPField;
        private System.Windows.Forms.Button connectToCloudButton;
        private System.Windows.Forms.Button connectToManagerButton;
        private System.Windows.Forms.TextBox log;
        private System.Windows.Forms.Button setNodeNumber;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox NodeNumberField;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox inPortTextBox;
        private System.Windows.Forms.TextBox inVPITextBox;
        private System.Windows.Forms.TextBox inVCITextBox;
        private System.Windows.Forms.TextBox outVCITextBox;
        private System.Windows.Forms.TextBox outVPITextBox;
        private System.Windows.Forms.TextBox outPortTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button addEntryButton;
        private System.Windows.Forms.Button chooseTextFile;
    }
}

