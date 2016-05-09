using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;          // 系统空间里面包含串口

namespace ChargingPile.WinForm
{
    public partial class SetComPara : Form
    {
        public SetComPara()
        {
            InitializeComponent();
        }

        private void BtnSCP_OK_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void BtnSCP_Apply_Click(object sender, EventArgs e)
        {

        }
        private void BtnSCP_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void SetComPara_Load(object sender, EventArgs e)
        {
            try {
                string[] szPorts = SerialPort.GetPortNames();
                combSerialPort.Items.AddRange(szPorts);
                combSerialPort.SelectedIndex = 0;
            } catch (Win32Exception win32ex)  {
                MessageBox.Show(win32ex.ToString());
            }
            combBaudRate.Text = "9600";
            combDataBit.Text = "8";
            combStopBits.Text = "1";
            SerialParity.Text = "None";
            combSerialPort.Text = "COM1";
        }
    }
}
