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
            this.setNodeNumber.Size = new System.Drawing.Size(97, 43);
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
            // Nodix
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(346, 329);
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
    }
}

