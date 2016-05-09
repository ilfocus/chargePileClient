namespace ChargingPile.WinForm
{
    partial class SetChargePile
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
            this.btnExit = new System.Windows.Forms.Button();
            this.txtSlaveNum = new System.Windows.Forms.TextBox();
            this.txtCPNum = new System.Windows.Forms.TextBox();
            this.lblMasterNum = new System.Windows.Forms.Label();
            this.btnSetOK = new System.Windows.Forms.Button();
            this.panel33 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnQueckAdd = new System.Windows.Forms.Button();
            this.panel33.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(179, 221);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(82, 29);
            this.btnExit.TabIndex = 521;
            this.btnExit.Text = "取消";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // txtSlaveNum
            // 
            this.txtSlaveNum.Location = new System.Drawing.Point(93, 21);
            this.txtSlaveNum.Name = "txtSlaveNum";
            this.txtSlaveNum.Size = new System.Drawing.Size(130, 21);
            this.txtSlaveNum.TabIndex = 513;
            // 
            // txtCPNum
            // 
            this.txtCPNum.Location = new System.Drawing.Point(94, 34);
            this.txtCPNum.Name = "txtCPNum";
            this.txtCPNum.Size = new System.Drawing.Size(59, 21);
            this.txtCPNum.TabIndex = 511;
            // 
            // lblMasterNum
            // 
            this.lblMasterNum.AutoSize = true;
            this.lblMasterNum.Location = new System.Drawing.Point(4, 24);
            this.lblMasterNum.Name = "lblMasterNum";
            this.lblMasterNum.Size = new System.Drawing.Size(71, 12);
            this.lblMasterNum.TabIndex = 510;
            this.lblMasterNum.Text = "充电桩数量:";
            // 
            // btnSetOK
            // 
            this.btnSetOK.Location = new System.Drawing.Point(19, 221);
            this.btnSetOK.Name = "btnSetOK";
            this.btnSetOK.Size = new System.Drawing.Size(82, 29);
            this.btnSetOK.TabIndex = 522;
            this.btnSetOK.Text = "确定";
            this.btnSetOK.UseVisualStyleBackColor = true;
            // 
            // panel33
            // 
            this.panel33.BackColor = System.Drawing.Color.White;
            this.panel33.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel33.Controls.Add(this.panel1);
            this.panel33.Controls.Add(this.lblMasterNum);
            this.panel33.Location = new System.Drawing.Point(12, 12);
            this.panel33.Name = "panel33";
            this.panel33.Size = new System.Drawing.Size(260, 59);
            this.panel33.TabIndex = 523;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnQueckAdd);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(-1, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(260, 59);
            this.panel1.TabIndex = 524;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 12);
            this.label1.TabIndex = 510;
            this.label1.Text = "充电桩数量:";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btnAdd);
            this.panel2.Controls.Add(this.txtSlaveNum);
            this.panel2.Location = new System.Drawing.Point(12, 77);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(260, 138);
            this.panel2.TabIndex = 525;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(93, 62);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(130, 21);
            this.textBox1.TabIndex = 520;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 519;
            this.label3.Text = "充电桩IP地址:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 12);
            this.label2.TabIndex = 510;
            this.label2.Text = "充电桩机号:";
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(93, 104);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(82, 29);
            this.btnAdd.TabIndex = 518;
            this.btnAdd.Text = "添加";
            this.btnAdd.UseVisualStyleBackColor = true;
            // 
            // btnQueckAdd
            // 
            this.btnQueckAdd.Location = new System.Drawing.Point(166, 16);
            this.btnQueckAdd.Name = "btnQueckAdd";
            this.btnQueckAdd.Size = new System.Drawing.Size(82, 29);
            this.btnQueckAdd.TabIndex = 523;
            this.btnQueckAdd.Text = "快速添加";
            this.btnQueckAdd.UseVisualStyleBackColor = true;
            this.btnQueckAdd.Click += new System.EventHandler(this.btnQueckAdd_Click);
            // 
            // SetChargePile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnSetOK);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.txtCPNum);
            this.Controls.Add(this.panel33);
            this.Name = "SetChargePile";
            this.Text = "添加充电桩";
            this.panel33.ResumeLayout(false);
            this.panel33.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.TextBox txtSlaveNum;
        private System.Windows.Forms.TextBox txtCPNum;
        private System.Windows.Forms.Label lblMasterNum;
        private System.Windows.Forms.Button btnSetOK;
        private System.Windows.Forms.Panel panel33;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnQueckAdd;
    }
}