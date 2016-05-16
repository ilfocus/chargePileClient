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
                try {
                    cpNum = Convert.ToInt32(txtCPNum.Text);
                } catch (Exception ex) {
                    MessageBox.Show("输入错误，请重新输入！");
                    return;
                }
                
            } else {
                MessageBox.Show("充电桩数量不能为空！");
                return;
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
        private void btnAdd_Click(object sender, EventArgs e) {
            int addNum = 0;
            string str = "";
            if (txtSlaveNum.Text != "") {
                try {
                    addNum = Convert.ToInt32(txtSlaveNum.Text);
                } catch (Exception ex) {
                    MessageBox.Show("输入有误，请重新输入！");
                }
                str = String.Format("{0:0000}", addNum) + "号充电桩";

                foreach (TreeNode node in interFace.tvChargePile.Nodes) {
                    foreach (TreeNode node1 in node.Nodes) {
                        Console.WriteLine(node1.Text);
                        if (node1.Text == str) {
                            MessageBox.Show("重复添加");
                            return;
                        }
                    }
                }
            } else {
                MessageBox.Show("充电桩地址不能为空！");
                return;
            }
            
            TreeNode node2 = new TreeNode(str);
            foreach (TreeNode node in interFace.tvChargePile.Nodes) {
                node.Nodes.Add(node2);
            }
            interFace.tvChargePile.ExpandAll();
        }

        private void btnSetOK_Click(object sender, EventArgs e) {
            this.Close();
        }

    }
}
