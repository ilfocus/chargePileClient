using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ChargingPile.WinForm
{
    public partial class SetChargePile : Form
    {
        private MonitoringInterface interFace = null;
        public SetChargePile(MonitoringInterface moniInterFace)
        {
            interFace = moniInterFace;
            InitializeComponent();
        }
        private void btnExit_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnQueckAdd_Click(object sender, EventArgs e) {
            int cpNum = 20;
            if (txtCPNum.Text != "") {
                cpNum = Convert.ToInt32(txtCPNum.Text);
            }
            for (int j = 1; j < cpNum; j++) {
                string str = String.Format("{0:0000}",j) + "号充电桩";
                TreeNode node2 = new TreeNode(str);
                foreach (TreeNode node in interFace.tvChargePile.Nodes) {
                    node.Nodes.Add(node2);
                }
            }
            interFace.tvChargePile.ExpandAll();
        }

    }
}
