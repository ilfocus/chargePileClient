namespace ChargingPile
{
    partial class MonitoringInterface
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonitoringInterface));
            this.btnOpenPort = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton_Config = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmAddCP = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.串口设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton_Look = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Alarm = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_UpDown = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_SHTV = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Card = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_FileMng = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_User = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_Curve = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton_Report = new System.Windows.Forms.ToolStripDropDownButton();
            this.tsmi_ChargeRecordReport = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton_About = new System.Windows.Forms.ToolStripButton();
            this.TSBtnExist = new System.Windows.Forms.ToolStripButton();
            this.TS_LableSystemTime = new System.Windows.Forms.ToolStripLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labeltx = new System.Windows.Forms.Label();
            this.picBox1 = new System.Windows.Forms.PictureBox();
            this.TimeSendData = new System.Windows.Forms.Timer(this.components);
            this.btnGetData = new System.Windows.Forms.Button();
            this.timeDealRevData = new System.Windows.Forms.Timer(this.components);
            this.updateFrameTimer = new System.Windows.Forms.Timer(this.components);
            this.skinEngine2 = new Sunisoft.IrisSkin.SkinEngine();
            this.panel33 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtChargePileTime = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnSetTime = new System.Windows.Forms.Button();
            this.TimSysTime = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.btnPileNormal = new System.Windows.Forms.Button();
            this.btnSetRate = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.btnCPState = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.btnStartup = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.btnCurInfo = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.txtCPStartup = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label17 = new System.Windows.Forms.Label();
            this.txtRateValleyPrice = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtRateFlatPrice = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtRatePeakPrice = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtRatePointPrice = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.heartFrameLed = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtChargingPileAddress = new System.Windows.Forms.TextBox();
            this.panel6 = new System.Windows.Forms.Panel();
            this.cbCurState = new System.Windows.Forms.ComboBox();
            this.cbChargePlug = new System.Windows.Forms.ComboBox();
            this.cbMeterState = new System.Windows.Forms.ComboBox();
            this.cbEmergencyBtn = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.txtValleyElect = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txtFlatElect = new System.Windows.Forms.TextBox();
            this.label26 = new System.Windows.Forms.Label();
            this.txtPeakElect = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtPointElect = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtTotalElect = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtCurrent = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtValtage = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.btnSetData = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnSetCurInfo = new System.Windows.Forms.Button();
            this.txtCurValleyCost = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.txtCurFlatCost = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.txtCurPeakCost = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.txtCurPointCost = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txtCurValleyPrice = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.txtCurFlatPrice = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.txtCurPeakPrice = new System.Windows.Forms.TextBox();
            this.label43 = new System.Windows.Forms.Label();
            this.txtCurPointPrice = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.txtCurValleyElect = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.txtCurFlatElect = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.txtCurPeakElect = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.txtCurPointElect = new System.Windows.Forms.TextBox();
            this.label51 = new System.Windows.Forms.Label();
            this.txtCurTotalCost = new System.Windows.Forms.TextBox();
            this.label52 = new System.Windows.Forms.Label();
            this.txtCurTotalElect = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.heartLedTime = new System.Windows.Forms.Timer(this.components);
            this.closeheartTime = new System.Windows.Forms.Timer(this.components);
            this.ChargeSocTime = new System.Windows.Forms.Timer(this.components);
            this.ChargeTimeTimer = new System.Windows.Forms.Timer(this.components);
            this.tvChargePile = new System.Windows.Forms.TreeView();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBox1)).BeginInit();
            this.panel33.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOpenPort
            // 
            this.btnOpenPort.Location = new System.Drawing.Point(560, 8);
            this.btnOpenPort.Name = "btnOpenPort";
            this.btnOpenPort.Size = new System.Drawing.Size(104, 27);
            this.btnOpenPort.TabIndex = 28;
            this.btnOpenPort.Text = "打开串口";
            this.btnOpenPort.UseVisualStyleBackColor = true;
            this.btnOpenPort.Click += new System.EventHandler(this.btnOpenPort_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.ReadBufferSize = 100;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Location = new System.Drawing.Point(122, 362);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(956, 216);
            this.zedGraphControl1.TabIndex = 30;
            this.zedGraphControl1.Load += new System.EventHandler(this.zedGraphControl1_Load);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton_Config,
            this.toolStripButton_Look,
            this.toolStripButton_Alarm,
            this.toolStripButton_UpDown,
            this.toolStripButton_SHTV,
            this.toolStripSeparator1,
            this.toolStripButton_Card,
            this.toolStripButton_FileMng,
            this.toolStripButton_User,
            this.toolStripSeparator2,
            this.toolStripButton_Curve,
            this.toolStripButton_Report,
            this.toolStripSeparator3,
            this.toolStripButton_About,
            this.TSBtnExist,
            this.TS_LableSystemTime});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1081, 72);
            this.toolStrip1.TabIndex = 31;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton_Config
            // 
            this.toolStripButton_Config.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmAddCP,
            this.toolStripMenuItem2,
            this.串口设置ToolStripMenuItem});
            this.toolStripButton_Config.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Config.Image")));
            this.toolStripButton_Config.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Config.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Config.Name = "toolStripButton_Config";
            this.toolStripButton_Config.Size = new System.Drawing.Size(69, 69);
            this.toolStripButton_Config.Text = "参数配置";
            this.toolStripButton_Config.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // tsmAddCP
            // 
            this.tsmAddCP.Name = "tsmAddCP";
            this.tsmAddCP.Size = new System.Drawing.Size(152, 22);
            this.tsmAddCP.Text = "充电桩配置";
            this.tsmAddCP.Click += new System.EventHandler(this.tsmAddCP_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(152, 22);
            this.toolStripMenuItem2.Text = "告警阈值配置";
            // 
            // 串口设置ToolStripMenuItem
            // 
            this.串口设置ToolStripMenuItem.Name = "串口设置ToolStripMenuItem";
            this.串口设置ToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.串口设置ToolStripMenuItem.Text = "串口设置";
            this.串口设置ToolStripMenuItem.Click += new System.EventHandler(this.串口设置ToolStripMenuItem_Click);
            // 
            // toolStripButton_Look
            // 
            this.toolStripButton_Look.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Look.Image")));
            this.toolStripButton_Look.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Look.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Look.Name = "toolStripButton_Look";
            this.toolStripButton_Look.Size = new System.Drawing.Size(60, 69);
            this.toolStripButton_Look.Text = "查看事件";
            this.toolStripButton_Look.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton_Alarm
            // 
            this.toolStripButton_Alarm.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Alarm.Image")));
            this.toolStripButton_Alarm.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Alarm.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Alarm.Name = "toolStripButton_Alarm";
            this.toolStripButton_Alarm.Size = new System.Drawing.Size(60, 69);
            this.toolStripButton_Alarm.Text = "音效告警";
            this.toolStripButton_Alarm.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton_UpDown
            // 
            this.toolStripButton_UpDown.Image = global::ChargingPile.Properties.Resources.hideup;
            this.toolStripButton_UpDown.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_UpDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_UpDown.Name = "toolStripButton_UpDown";
            this.toolStripButton_UpDown.Size = new System.Drawing.Size(60, 69);
            this.toolStripButton_UpDown.Text = "隐藏总览";
            this.toolStripButton_UpDown.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton_SHTV
            // 
            this.toolStripButton_SHTV.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_SHTV.Image")));
            this.toolStripButton_SHTV.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_SHTV.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_SHTV.Name = "toolStripButton_SHTV";
            this.toolStripButton_SHTV.Size = new System.Drawing.Size(72, 69);
            this.toolStripButton_SHTV.Text = "隐藏树视图";
            this.toolStripButton_SHTV.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.toolStripButton_SHTV.Visible = false;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 72);
            // 
            // toolStripButton_Card
            // 
            this.toolStripButton_Card.Image = global::ChargingPile.Properties.Resources.card;
            this.toolStripButton_Card.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Card.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Card.Name = "toolStripButton_Card";
            this.toolStripButton_Card.Size = new System.Drawing.Size(60, 69);
            this.toolStripButton_Card.Text = "开卡充值";
            this.toolStripButton_Card.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton_FileMng
            // 
            this.toolStripButton_FileMng.Image = global::ChargingPile.Properties.Resources.files48;
            this.toolStripButton_FileMng.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_FileMng.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_FileMng.Name = "toolStripButton_FileMng";
            this.toolStripButton_FileMng.Size = new System.Drawing.Size(60, 69);
            this.toolStripButton_FileMng.Text = "档案管理";
            this.toolStripButton_FileMng.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton_User
            // 
            this.toolStripButton_User.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_User.Image")));
            this.toolStripButton_User.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_User.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_User.Name = "toolStripButton_User";
            this.toolStripButton_User.Size = new System.Drawing.Size(60, 69);
            this.toolStripButton_User.Text = "用户管理";
            this.toolStripButton_User.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 72);
            // 
            // toolStripButton_Curve
            // 
            this.toolStripButton_Curve.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Curve.Image")));
            this.toolStripButton_Curve.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Curve.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Curve.Name = "toolStripButton_Curve";
            this.toolStripButton_Curve.Size = new System.Drawing.Size(60, 69);
            this.toolStripButton_Curve.Text = "曲线查看";
            this.toolStripButton_Curve.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // toolStripButton_Report
            // 
            this.toolStripButton_Report.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmi_ChargeRecordReport});
            this.toolStripButton_Report.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_Report.Image")));
            this.toolStripButton_Report.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_Report.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_Report.Name = "toolStripButton_Report";
            this.toolStripButton_Report.Size = new System.Drawing.Size(69, 69);
            this.toolStripButton_Report.Text = "报表浏览";
            this.toolStripButton_Report.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // tsmi_ChargeRecordReport
            // 
            this.tsmi_ChargeRecordReport.Name = "tsmi_ChargeRecordReport";
            this.tsmi_ChargeRecordReport.Size = new System.Drawing.Size(148, 22);
            this.tsmi_ChargeRecordReport.Text = "充电记录报表";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 72);
            // 
            // toolStripButton_About
            // 
            this.toolStripButton_About.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton_About.Image")));
            this.toolStripButton_About.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton_About.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton_About.Name = "toolStripButton_About";
            this.toolStripButton_About.Size = new System.Drawing.Size(60, 69);
            this.toolStripButton_About.Text = "关于系统";
            this.toolStripButton_About.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            // 
            // TSBtnExist
            // 
            this.TSBtnExist.Image = ((System.Drawing.Image)(resources.GetObject("TSBtnExist.Image")));
            this.TSBtnExist.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.TSBtnExist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.TSBtnExist.Name = "TSBtnExist";
            this.TSBtnExist.Size = new System.Drawing.Size(60, 69);
            this.TSBtnExist.Text = "退出系统";
            this.TSBtnExist.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.TSBtnExist.Click += new System.EventHandler(this.TSBtnExist_Click);
            // 
            // TS_LableSystemTime
            // 
            this.TS_LableSystemTime.AutoSize = false;
            this.TS_LableSystemTime.Font = new System.Drawing.Font("幼圆", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.TS_LableSystemTime.Name = "TS_LableSystemTime";
            this.TS_LableSystemTime.Size = new System.Drawing.Size(230, 56);
            this.TS_LableSystemTime.Text = "toolStripLabel1";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labeltx);
            this.panel1.Controls.Add(this.picBox1);
            this.panel1.Location = new System.Drawing.Point(0, 580);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(959, 38);
            this.panel1.TabIndex = 34;
            // 
            // labeltx
            // 
            this.labeltx.AutoSize = true;
            this.labeltx.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.labeltx.Location = new System.Drawing.Point(11, 8);
            this.labeltx.Name = "labeltx";
            this.labeltx.Size = new System.Drawing.Size(89, 20);
            this.labeltx.TabIndex = 467;
            this.labeltx.Text = "通讯状态";
            // 
            // picBox1
            // 
            this.picBox1.Image = global::ChargingPile.Properties.Resources.green32;
            this.picBox1.Location = new System.Drawing.Point(106, 2);
            this.picBox1.Name = "picBox1";
            this.picBox1.Size = new System.Drawing.Size(41, 31);
            this.picBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picBox1.TabIndex = 3;
            this.picBox1.TabStop = false;
            // 
            // TimeSendData
            // 
            this.TimeSendData.Interval = 500;
            this.TimeSendData.Tick += new System.EventHandler(this.TimeSendData_Tick);
            // 
            // btnGetData
            // 
            this.btnGetData.Location = new System.Drawing.Point(445, 8);
            this.btnGetData.Name = "btnGetData";
            this.btnGetData.Size = new System.Drawing.Size(104, 27);
            this.btnGetData.TabIndex = 457;
            this.btnGetData.Text = "打开监听";
            this.btnGetData.UseVisualStyleBackColor = true;
            this.btnGetData.Click += new System.EventHandler(this.btnGetData_Click);
            // 
            // timeDealRevData
            // 
            this.timeDealRevData.Enabled = true;
            this.timeDealRevData.Tick += new System.EventHandler(this.timeDealRevData_Tick);
            // 
            // updateFrameTimer
            // 
            this.updateFrameTimer.Enabled = true;
            this.updateFrameTimer.Interval = 500;
            this.updateFrameTimer.Tick += new System.EventHandler(this.updateFrameTimer_Tick);
            // 
            // skinEngine2
            // 
            this.skinEngine2.@__DrawButtonFocusRectangle = true;
            this.skinEngine2.DisabledButtonTextColor = System.Drawing.Color.Gray;
            this.skinEngine2.DisabledMenuFontColor = System.Drawing.SystemColors.GrayText;
            this.skinEngine2.InactiveCaptionColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.skinEngine2.SerialNumber = "";
            this.skinEngine2.SkinFile = null;
            // 
            // panel33
            // 
            this.panel33.BackColor = System.Drawing.Color.White;
            this.panel33.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel33.Controls.Add(this.label1);
            this.panel33.Controls.Add(this.txtChargePileTime);
            this.panel33.Controls.Add(this.label3);
            this.panel33.Controls.Add(this.btnSetTime);
            this.panel33.Location = new System.Drawing.Point(122, 141);
            this.panel33.Name = "panel33";
            this.panel33.Size = new System.Drawing.Size(134, 215);
            this.panel33.TabIndex = 466;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 47);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 502;
            this.label1.Text = "充电桩设置时间显示";
            // 
            // txtChargePileTime
            // 
            this.txtChargePileTime.Location = new System.Drawing.Point(4, 67);
            this.txtChargePileTime.Name = "txtChargePileTime";
            this.txtChargePileTime.Size = new System.Drawing.Size(123, 21);
            this.txtChargePileTime.TabIndex = 493;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 12);
            this.label3.TabIndex = 491;
            this.label3.Text = "时间:";
            // 
            // btnSetTime
            // 
            this.btnSetTime.Location = new System.Drawing.Point(42, 4);
            this.btnSetTime.Name = "btnSetTime";
            this.btnSetTime.Size = new System.Drawing.Size(85, 27);
            this.btnSetTime.TabIndex = 492;
            this.btnSetTime.Text = "充电桩正常";
            this.btnSetTime.UseVisualStyleBackColor = true;
            this.btnSetTime.Click += new System.EventHandler(this.btnSetTime_Click);
            // 
            // TimSysTime
            // 
            this.TimSysTime.Interval = 1000;
            this.TimSysTime.Tick += new System.EventHandler(this.TimSysTime_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(237, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 469;
            this.label2.Text = "桩心跳帧:";
            // 
            // btnPileNormal
            // 
            this.btnPileNormal.Location = new System.Drawing.Point(302, 8);
            this.btnPileNormal.Name = "btnPileNormal";
            this.btnPileNormal.Size = new System.Drawing.Size(108, 27);
            this.btnPileNormal.TabIndex = 490;
            this.btnPileNormal.Text = "充电桩正常";
            this.btnPileNormal.UseVisualStyleBackColor = true;
            this.btnPileNormal.Click += new System.EventHandler(this.btnPileNormal_Click);
            // 
            // btnSetRate
            // 
            this.btnSetRate.Location = new System.Drawing.Point(42, 4);
            this.btnSetRate.Name = "btnSetRate";
            this.btnSetRate.Size = new System.Drawing.Size(85, 27);
            this.btnSetRate.TabIndex = 494;
            this.btnSetRate.Text = "充电桩正常";
            this.btnSetRate.UseVisualStyleBackColor = true;
            this.btnSetRate.Click += new System.EventHandler(this.btnSetRate_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 11);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(35, 12);
            this.label11.TabIndex = 493;
            this.label11.Text = "费率:";
            // 
            // btnCPState
            // 
            this.btnCPState.Location = new System.Drawing.Point(88, 10);
            this.btnCPState.Name = "btnCPState";
            this.btnCPState.Size = new System.Drawing.Size(85, 27);
            this.btnCPState.TabIndex = 496;
            this.btnCPState.Text = "充电桩正常";
            this.btnCPState.UseVisualStyleBackColor = true;
            this.btnCPState.Click += new System.EventHandler(this.btnCPState_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(65, 12);
            this.label12.TabIndex = 495;
            this.label12.Text = "充电桩状态";
            // 
            // btnStartup
            // 
            this.btnStartup.Location = new System.Drawing.Point(740, 11);
            this.btnStartup.Name = "btnStartup";
            this.btnStartup.Size = new System.Drawing.Size(85, 27);
            this.btnStartup.TabIndex = 498;
            this.btnStartup.Text = "充电桩正常";
            this.btnStartup.UseVisualStyleBackColor = true;
            this.btnStartup.Click += new System.EventHandler(this.btnStartup_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(663, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(71, 12);
            this.label13.TabIndex = 497;
            this.label13.Text = "充电桩启停:";
            // 
            // btnCurInfo
            // 
            this.btnCurInfo.Location = new System.Drawing.Point(111, 4);
            this.btnCurInfo.Name = "btnCurInfo";
            this.btnCurInfo.Size = new System.Drawing.Size(108, 27);
            this.btnCurInfo.TabIndex = 500;
            this.btnCurInfo.Text = "充电桩正常";
            this.btnCurInfo.UseVisualStyleBackColor = true;
            this.btnCurInfo.Click += new System.EventHandler(this.btnCurInfo_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(3, 10);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(89, 12);
            this.label14.TabIndex = 499;
            this.label14.Text = "当前充电桩信息";
            // 
            // txtCPStartup
            // 
            this.txtCPStartup.Location = new System.Drawing.Point(831, 15);
            this.txtCPStartup.Name = "txtCPStartup";
            this.txtCPStartup.Size = new System.Drawing.Size(120, 21);
            this.txtCPStartup.TabIndex = 501;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label17);
            this.panel2.Controls.Add(this.txtRateValleyPrice);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.txtRateFlatPrice);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.txtRatePeakPrice);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.txtRatePointPrice);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.btnSetRate);
            this.panel2.Location = new System.Drawing.Point(256, 141);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(169, 215);
            this.panel2.TabIndex = 502;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(14, 47);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(113, 12);
            this.label17.TabIndex = 507;
            this.label17.Text = "充电桩电价设置显示";
            // 
            // txtRateValleyPrice
            // 
            this.txtRateValleyPrice.Location = new System.Drawing.Point(56, 157);
            this.txtRateValleyPrice.Name = "txtRateValleyPrice";
            this.txtRateValleyPrice.Size = new System.Drawing.Size(100, 21);
            this.txtRateValleyPrice.TabIndex = 502;
            this.txtRateValleyPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 161);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(47, 12);
            this.label9.TabIndex = 501;
            this.label9.Text = "谷电价:";
            // 
            // txtRateFlatPrice
            // 
            this.txtRateFlatPrice.Location = new System.Drawing.Point(56, 127);
            this.txtRateFlatPrice.Name = "txtRateFlatPrice";
            this.txtRateFlatPrice.Size = new System.Drawing.Size(100, 21);
            this.txtRateFlatPrice.TabIndex = 500;
            this.txtRateFlatPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 131);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(47, 12);
            this.label6.TabIndex = 499;
            this.label6.Text = "平电价:";
            // 
            // txtRatePeakPrice
            // 
            this.txtRatePeakPrice.Location = new System.Drawing.Point(56, 97);
            this.txtRatePeakPrice.Name = "txtRatePeakPrice";
            this.txtRatePeakPrice.Size = new System.Drawing.Size(100, 21);
            this.txtRatePeakPrice.TabIndex = 498;
            this.txtRatePeakPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 101);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(47, 12);
            this.label7.TabIndex = 497;
            this.label7.Text = "峰电价:";
            // 
            // txtRatePointPrice
            // 
            this.txtRatePointPrice.Location = new System.Drawing.Point(56, 67);
            this.txtRatePointPrice.Name = "txtRatePointPrice";
            this.txtRatePointPrice.Size = new System.Drawing.Size(100, 21);
            this.txtRatePointPrice.TabIndex = 496;
            this.txtRatePointPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 71);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 12);
            this.label8.TabIndex = 495;
            this.label8.Text = "尖电价:";
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.heartFrameLed);
            this.panel3.Controls.Add(this.txtCPStartup);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Controls.Add(this.btnStartup);
            this.panel3.Controls.Add(this.label13);
            this.panel3.Controls.Add(this.txtChargingPileAddress);
            this.panel3.Controls.Add(this.btnPileNormal);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.btnGetData);
            this.panel3.Controls.Add(this.btnOpenPort);
            this.panel3.Location = new System.Drawing.Point(122, 80);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(956, 55);
            this.panel3.TabIndex = 503;
            // 
            // heartFrameLed
            // 
            this.heartFrameLed.AutoSize = true;
            this.heartFrameLed.ForeColor = System.Drawing.Color.DarkGray;
            this.heartFrameLed.Location = new System.Drawing.Point(416, 18);
            this.heartFrameLed.Name = "heartFrameLed";
            this.heartFrameLed.Size = new System.Drawing.Size(17, 12);
            this.heartFrameLed.TabIndex = 42;
            this.heartFrameLed.Text = "●";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 12);
            this.label4.TabIndex = 491;
            this.label4.Text = "充电桩地址:";
            // 
            // txtChargingPileAddress
            // 
            this.txtChargingPileAddress.Location = new System.Drawing.Point(76, 11);
            this.txtChargingPileAddress.Name = "txtChargingPileAddress";
            this.txtChargingPileAddress.Size = new System.Drawing.Size(153, 21);
            this.txtChargingPileAddress.TabIndex = 492;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.Color.White;
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.cbCurState);
            this.panel6.Controls.Add(this.cbChargePlug);
            this.panel6.Controls.Add(this.cbMeterState);
            this.panel6.Controls.Add(this.cbEmergencyBtn);
            this.panel6.Controls.Add(this.label16);
            this.panel6.Controls.Add(this.label22);
            this.panel6.Controls.Add(this.label23);
            this.panel6.Controls.Add(this.label24);
            this.panel6.Controls.Add(this.txtValleyElect);
            this.panel6.Controls.Add(this.label25);
            this.panel6.Controls.Add(this.txtFlatElect);
            this.panel6.Controls.Add(this.label26);
            this.panel6.Controls.Add(this.txtPeakElect);
            this.panel6.Controls.Add(this.label18);
            this.panel6.Controls.Add(this.txtPointElect);
            this.panel6.Controls.Add(this.label20);
            this.panel6.Controls.Add(this.txtTotalElect);
            this.panel6.Controls.Add(this.label5);
            this.panel6.Controls.Add(this.txtCurrent);
            this.panel6.Controls.Add(this.label10);
            this.panel6.Controls.Add(this.txtValtage);
            this.panel6.Controls.Add(this.label15);
            this.panel6.Controls.Add(this.btnSetData);
            this.panel6.Controls.Add(this.btnCPState);
            this.panel6.Controls.Add(this.label12);
            this.panel6.Location = new System.Drawing.Point(425, 141);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(290, 215);
            this.panel6.TabIndex = 506;
            // 
            // cbCurState
            // 
            this.cbCurState.FormattingEnabled = true;
            this.cbCurState.Items.AddRange(new object[] {
            "有输出",
            "无输出"});
            this.cbCurState.Location = new System.Drawing.Point(135, 182);
            this.cbCurState.Name = "cbCurState";
            this.cbCurState.Size = new System.Drawing.Size(71, 20);
            this.cbCurState.TabIndex = 547;
            // 
            // cbChargePlug
            // 
            this.cbChargePlug.FormattingEnabled = true;
            this.cbChargePlug.Items.AddRange(new object[] {
            "插好",
            "没准备好"});
            this.cbChargePlug.Location = new System.Drawing.Point(208, 157);
            this.cbChargePlug.Name = "cbChargePlug";
            this.cbChargePlug.Size = new System.Drawing.Size(71, 20);
            this.cbChargePlug.TabIndex = 546;
            // 
            // cbMeterState
            // 
            this.cbMeterState.FormattingEnabled = true;
            this.cbMeterState.Items.AddRange(new object[] {
            "通信正常",
            "通信异常"});
            this.cbMeterState.Location = new System.Drawing.Point(208, 127);
            this.cbMeterState.Name = "cbMeterState";
            this.cbMeterState.Size = new System.Drawing.Size(71, 20);
            this.cbMeterState.TabIndex = 545;
            // 
            // cbEmergencyBtn
            // 
            this.cbEmergencyBtn.FormattingEnabled = true;
            this.cbEmergencyBtn.Items.AddRange(new object[] {
            "正常",
            "按下"});
            this.cbEmergencyBtn.Location = new System.Drawing.Point(208, 101);
            this.cbEmergencyBtn.Name = "cbEmergencyBtn";
            this.cbEmergencyBtn.Size = new System.Drawing.Size(71, 20);
            this.cbEmergencyBtn.TabIndex = 544;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(77, 185);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(59, 12);
            this.label16.TabIndex = 535;
            this.label16.Text = "当前状态:";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(147, 158);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(59, 12);
            this.label22.TabIndex = 534;
            this.label22.Text = "充电插头:";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(147, 132);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(59, 12);
            this.label23.TabIndex = 533;
            this.label23.Text = "电表状态:";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(147, 104);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(59, 12);
            this.label24.TabIndex = 532;
            this.label24.Text = "急停控钮:";
            // 
            // txtValleyElect
            // 
            this.txtValleyElect.Location = new System.Drawing.Point(197, 73);
            this.txtValleyElect.Name = "txtValleyElect";
            this.txtValleyElect.Size = new System.Drawing.Size(82, 21);
            this.txtValleyElect.TabIndex = 531;
            this.txtValleyElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(147, 77);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(47, 12);
            this.label25.TabIndex = 530;
            this.label25.Text = "谷电量:";
            // 
            // txtFlatElect
            // 
            this.txtFlatElect.Location = new System.Drawing.Point(197, 47);
            this.txtFlatElect.Name = "txtFlatElect";
            this.txtFlatElect.Size = new System.Drawing.Size(82, 21);
            this.txtFlatElect.TabIndex = 529;
            this.txtFlatElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(147, 51);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(47, 12);
            this.label26.TabIndex = 528;
            this.label26.Text = "平电量:";
            // 
            // txtPeakElect
            // 
            this.txtPeakElect.Location = new System.Drawing.Point(57, 154);
            this.txtPeakElect.Name = "txtPeakElect";
            this.txtPeakElect.Size = new System.Drawing.Size(82, 21);
            this.txtPeakElect.TabIndex = 527;
            this.txtPeakElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(7, 158);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(47, 12);
            this.label18.TabIndex = 526;
            this.label18.Text = "峰电量:";
            // 
            // txtPointElect
            // 
            this.txtPointElect.Location = new System.Drawing.Point(57, 128);
            this.txtPointElect.Name = "txtPointElect";
            this.txtPointElect.Size = new System.Drawing.Size(82, 21);
            this.txtPointElect.TabIndex = 525;
            this.txtPointElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(7, 132);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(47, 12);
            this.label20.TabIndex = 524;
            this.label20.Text = "尖电量:";
            // 
            // txtTotalElect
            // 
            this.txtTotalElect.Location = new System.Drawing.Point(57, 100);
            this.txtTotalElect.Name = "txtTotalElect";
            this.txtTotalElect.Size = new System.Drawing.Size(82, 21);
            this.txtTotalElect.TabIndex = 523;
            this.txtTotalElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 104);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(47, 12);
            this.label5.TabIndex = 522;
            this.label5.Text = "总电量:";
            // 
            // txtCurrent
            // 
            this.txtCurrent.Location = new System.Drawing.Point(57, 73);
            this.txtCurrent.Name = "txtCurrent";
            this.txtCurrent.Size = new System.Drawing.Size(82, 21);
            this.txtCurrent.TabIndex = 521;
            this.txtCurrent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 77);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(35, 12);
            this.label10.TabIndex = 520;
            this.label10.Text = "电流:";
            // 
            // txtValtage
            // 
            this.txtValtage.Location = new System.Drawing.Point(57, 47);
            this.txtValtage.Name = "txtValtage";
            this.txtValtage.Size = new System.Drawing.Size(82, 21);
            this.txtValtage.TabIndex = 519;
            this.txtValtage.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(7, 51);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 12);
            this.label15.TabIndex = 518;
            this.label15.Text = "电压:";
            // 
            // btnSetData
            // 
            this.btnSetData.Location = new System.Drawing.Point(198, 11);
            this.btnSetData.Name = "btnSetData";
            this.btnSetData.Size = new System.Drawing.Size(62, 27);
            this.btnSetData.TabIndex = 497;
            this.btnSetData.Text = "设置数据";
            this.btnSetData.UseVisualStyleBackColor = true;
            this.btnSetData.Click += new System.EventHandler(this.btnSetData_Click);
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.btnSetCurInfo);
            this.panel4.Controls.Add(this.txtCurValleyCost);
            this.panel4.Controls.Add(this.label37);
            this.panel4.Controls.Add(this.txtCurFlatCost);
            this.panel4.Controls.Add(this.label38);
            this.panel4.Controls.Add(this.txtCurPeakCost);
            this.panel4.Controls.Add(this.label39);
            this.panel4.Controls.Add(this.txtCurPointCost);
            this.panel4.Controls.Add(this.label40);
            this.panel4.Controls.Add(this.txtCurValleyPrice);
            this.panel4.Controls.Add(this.label41);
            this.panel4.Controls.Add(this.txtCurFlatPrice);
            this.panel4.Controls.Add(this.label42);
            this.panel4.Controls.Add(this.txtCurPeakPrice);
            this.panel4.Controls.Add(this.label43);
            this.panel4.Controls.Add(this.txtCurPointPrice);
            this.panel4.Controls.Add(this.label35);
            this.panel4.Controls.Add(this.txtCurValleyElect);
            this.panel4.Controls.Add(this.label36);
            this.panel4.Controls.Add(this.txtCurFlatElect);
            this.panel4.Controls.Add(this.label49);
            this.panel4.Controls.Add(this.txtCurPeakElect);
            this.panel4.Controls.Add(this.label50);
            this.panel4.Controls.Add(this.txtCurPointElect);
            this.panel4.Controls.Add(this.label51);
            this.panel4.Controls.Add(this.txtCurTotalCost);
            this.panel4.Controls.Add(this.label52);
            this.panel4.Controls.Add(this.txtCurTotalElect);
            this.panel4.Controls.Add(this.label53);
            this.panel4.Controls.Add(this.label14);
            this.panel4.Controls.Add(this.btnCurInfo);
            this.panel4.Location = new System.Drawing.Point(721, 141);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(353, 215);
            this.panel4.TabIndex = 504;
            // 
            // btnSetCurInfo
            // 
            this.btnSetCurInfo.Location = new System.Drawing.Point(259, 4);
            this.btnSetCurInfo.Name = "btnSetCurInfo";
            this.btnSetCurInfo.Size = new System.Drawing.Size(62, 27);
            this.btnSetCurInfo.TabIndex = 553;
            this.btnSetCurInfo.Text = "设置数据";
            this.btnSetCurInfo.UseVisualStyleBackColor = true;
            this.btnSetCurInfo.Click += new System.EventHandler(this.btnSetCurInfo_Click);
            // 
            // txtCurValleyCost
            // 
            this.txtCurValleyCost.Location = new System.Drawing.Point(230, 185);
            this.txtCurValleyCost.Name = "txtCurValleyCost";
            this.txtCurValleyCost.Size = new System.Drawing.Size(82, 21);
            this.txtCurValleyCost.TabIndex = 552;
            this.txtCurValleyCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label37
            // 
            this.label37.AutoSize = true;
            this.label37.Location = new System.Drawing.Point(180, 189);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(47, 12);
            this.label37.TabIndex = 551;
            this.label37.Text = "谷费用:";
            // 
            // txtCurFlatCost
            // 
            this.txtCurFlatCost.Location = new System.Drawing.Point(230, 160);
            this.txtCurFlatCost.Name = "txtCurFlatCost";
            this.txtCurFlatCost.Size = new System.Drawing.Size(82, 21);
            this.txtCurFlatCost.TabIndex = 550;
            this.txtCurFlatCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label38
            // 
            this.label38.AutoSize = true;
            this.label38.Location = new System.Drawing.Point(180, 164);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(47, 12);
            this.label38.TabIndex = 549;
            this.label38.Text = "平费用:";
            // 
            // txtCurPeakCost
            // 
            this.txtCurPeakCost.Location = new System.Drawing.Point(230, 136);
            this.txtCurPeakCost.Name = "txtCurPeakCost";
            this.txtCurPeakCost.Size = new System.Drawing.Size(82, 21);
            this.txtCurPeakCost.TabIndex = 548;
            this.txtCurPeakCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(180, 140);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(47, 12);
            this.label39.TabIndex = 547;
            this.label39.Text = "峰费用:";
            // 
            // txtCurPointCost
            // 
            this.txtCurPointCost.Location = new System.Drawing.Point(230, 112);
            this.txtCurPointCost.Name = "txtCurPointCost";
            this.txtCurPointCost.Size = new System.Drawing.Size(82, 21);
            this.txtCurPointCost.TabIndex = 546;
            this.txtCurPointCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label40
            // 
            this.label40.AutoSize = true;
            this.label40.Location = new System.Drawing.Point(180, 116);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(47, 12);
            this.label40.TabIndex = 545;
            this.label40.Text = "尖费用:";
            // 
            // txtCurValleyPrice
            // 
            this.txtCurValleyPrice.Location = new System.Drawing.Point(230, 88);
            this.txtCurValleyPrice.Name = "txtCurValleyPrice";
            this.txtCurValleyPrice.Size = new System.Drawing.Size(82, 21);
            this.txtCurValleyPrice.TabIndex = 544;
            this.txtCurValleyPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label41
            // 
            this.label41.AutoSize = true;
            this.label41.Location = new System.Drawing.Point(180, 92);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(47, 12);
            this.label41.TabIndex = 543;
            this.label41.Text = "谷电价:";
            // 
            // txtCurFlatPrice
            // 
            this.txtCurFlatPrice.Location = new System.Drawing.Point(230, 65);
            this.txtCurFlatPrice.Name = "txtCurFlatPrice";
            this.txtCurFlatPrice.Size = new System.Drawing.Size(82, 21);
            this.txtCurFlatPrice.TabIndex = 542;
            this.txtCurFlatPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(180, 69);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(47, 12);
            this.label42.TabIndex = 541;
            this.label42.Text = "平电价:";
            // 
            // txtCurPeakPrice
            // 
            this.txtCurPeakPrice.Location = new System.Drawing.Point(230, 41);
            this.txtCurPeakPrice.Name = "txtCurPeakPrice";
            this.txtCurPeakPrice.Size = new System.Drawing.Size(82, 21);
            this.txtCurPeakPrice.TabIndex = 540;
            this.txtCurPeakPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label43
            // 
            this.label43.AutoSize = true;
            this.label43.Location = new System.Drawing.Point(180, 45);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(47, 12);
            this.label43.TabIndex = 539;
            this.label43.Text = "峰电价:";
            // 
            // txtCurPointPrice
            // 
            this.txtCurPointPrice.Location = new System.Drawing.Point(83, 185);
            this.txtCurPointPrice.Name = "txtCurPointPrice";
            this.txtCurPointPrice.Size = new System.Drawing.Size(82, 21);
            this.txtCurPointPrice.TabIndex = 538;
            this.txtCurPointPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(33, 189);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(47, 12);
            this.label35.TabIndex = 537;
            this.label35.Text = "尖电价:";
            // 
            // txtCurValleyElect
            // 
            this.txtCurValleyElect.Location = new System.Drawing.Point(83, 160);
            this.txtCurValleyElect.Name = "txtCurValleyElect";
            this.txtCurValleyElect.Size = new System.Drawing.Size(82, 21);
            this.txtCurValleyElect.TabIndex = 536;
            this.txtCurValleyElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label36
            // 
            this.label36.AutoSize = true;
            this.label36.Location = new System.Drawing.Point(33, 164);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(47, 12);
            this.label36.TabIndex = 535;
            this.label36.Text = "谷电量:";
            // 
            // txtCurFlatElect
            // 
            this.txtCurFlatElect.Location = new System.Drawing.Point(83, 136);
            this.txtCurFlatElect.Name = "txtCurFlatElect";
            this.txtCurFlatElect.Size = new System.Drawing.Size(82, 21);
            this.txtCurFlatElect.TabIndex = 534;
            this.txtCurFlatElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label49
            // 
            this.label49.AutoSize = true;
            this.label49.Location = new System.Drawing.Point(33, 140);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(47, 12);
            this.label49.TabIndex = 533;
            this.label49.Text = "平电量:";
            // 
            // txtCurPeakElect
            // 
            this.txtCurPeakElect.Location = new System.Drawing.Point(83, 112);
            this.txtCurPeakElect.Name = "txtCurPeakElect";
            this.txtCurPeakElect.Size = new System.Drawing.Size(82, 21);
            this.txtCurPeakElect.TabIndex = 532;
            this.txtCurPeakElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label50
            // 
            this.label50.AutoSize = true;
            this.label50.Location = new System.Drawing.Point(33, 116);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(47, 12);
            this.label50.TabIndex = 531;
            this.label50.Text = "峰电量:";
            // 
            // txtCurPointElect
            // 
            this.txtCurPointElect.Location = new System.Drawing.Point(83, 88);
            this.txtCurPointElect.Name = "txtCurPointElect";
            this.txtCurPointElect.Size = new System.Drawing.Size(82, 21);
            this.txtCurPointElect.TabIndex = 530;
            this.txtCurPointElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label51
            // 
            this.label51.AutoSize = true;
            this.label51.Location = new System.Drawing.Point(33, 92);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(47, 12);
            this.label51.TabIndex = 529;
            this.label51.Text = "尖电量:";
            // 
            // txtCurTotalCost
            // 
            this.txtCurTotalCost.Location = new System.Drawing.Point(83, 65);
            this.txtCurTotalCost.Name = "txtCurTotalCost";
            this.txtCurTotalCost.Size = new System.Drawing.Size(82, 21);
            this.txtCurTotalCost.TabIndex = 528;
            this.txtCurTotalCost.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(33, 69);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(47, 12);
            this.label52.TabIndex = 527;
            this.label52.Text = "总费用:";
            // 
            // txtCurTotalElect
            // 
            this.txtCurTotalElect.Location = new System.Drawing.Point(83, 41);
            this.txtCurTotalElect.Name = "txtCurTotalElect";
            this.txtCurTotalElect.Size = new System.Drawing.Size(82, 21);
            this.txtCurTotalElect.TabIndex = 526;
            this.txtCurTotalElect.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label53
            // 
            this.label53.AutoSize = true;
            this.label53.Location = new System.Drawing.Point(33, 45);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(47, 12);
            this.label53.TabIndex = 525;
            this.label53.Text = "总电量:";
            // 
            // heartLedTime
            // 
            this.heartLedTime.Interval = 1000;
            this.heartLedTime.Tick += new System.EventHandler(this.heartLedTime_Tick);
            // 
            // closeheartTime
            // 
            this.closeheartTime.Interval = 9500;
            this.closeheartTime.Tick += new System.EventHandler(this.closeheartTime_Tick);
            // 
            // ChargeSocTime
            // 
            this.ChargeSocTime.Interval = 5000;
            this.ChargeSocTime.Tick += new System.EventHandler(this.ChargeSocTime_Tick);
            // 
            // ChargeTimeTimer
            // 
            this.ChargeTimeTimer.Interval = 10000;
            this.ChargeTimeTimer.Tick += new System.EventHandler(this.ChargeTimeTimer_Tick);
            // 
            // tvChargePile
            // 
            this.tvChargePile.Location = new System.Drawing.Point(2, 75);
            this.tvChargePile.Name = "tvChargePile";
            this.tvChargePile.Size = new System.Drawing.Size(114, 503);
            this.tvChargePile.TabIndex = 508;
            // 
            // MonitoringInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1081, 619);
            this.Controls.Add(this.tvChargePile);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel33);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.zedGraphControl1);
            this.Name = "MonitoringInterface";
            this.RightToLeftLayout = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "充电桩";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picBox1)).EndInit();
            this.panel33.ResumeLayout(false);
            this.panel33.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOpenPort;
        private ZedGraph.ZedGraphControl zedGraphControl1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.Timer TimeSendData;
        private System.Windows.Forms.Button btnGetData;
        private System.Windows.Forms.Timer timeDealRevData;
        private Sunisoft.IrisSkin.SkinEngine skinEngine2;
        public System.IO.Ports.SerialPort serialPort1;
        public System.Windows.Forms.Timer updateFrameTimer;
        private System.Windows.Forms.Panel panel33;
        private System.Windows.Forms.Label labeltx;
        private System.Windows.Forms.PictureBox picBox1;
        private System.Windows.Forms.Timer TimSysTime;
        private System.Windows.Forms.ToolStripLabel TS_LableSystemTime;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnPileNormal;
        private System.Windows.Forms.Button btnSetTime;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnSetRate;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button btnCPState;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnStartup;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnCurInfo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtCPStartup;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtChargePileTime;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtRateValleyPrice;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtRateFlatPrice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtRatePeakPrice;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtRatePointPrice;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtChargingPileAddress;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton_Config;
        private System.Windows.Forms.ToolStripMenuItem tsmAddCP;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem 串口设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton toolStripButton_Look;
        private System.Windows.Forms.ToolStripButton toolStripButton_Alarm;
        private System.Windows.Forms.ToolStripButton toolStripButton_UpDown;
        private System.Windows.Forms.ToolStripButton toolStripButton_SHTV;
        private System.Windows.Forms.ToolStripButton toolStripButton_User;
        private System.Windows.Forms.ToolStripButton toolStripButton_FileMng;
        private System.Windows.Forms.ToolStripButton toolStripButton_Card;
        private System.Windows.Forms.ToolStripDropDownButton toolStripButton_Report;
        private System.Windows.Forms.ToolStripMenuItem tsmi_ChargeRecordReport;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton toolStripButton_Curve;
        private System.Windows.Forms.ToolStripButton toolStripButton_About;
        private System.Windows.Forms.ToolStripButton TSBtnExist;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Button btnSetData;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.TextBox txtValleyElect;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtFlatElect;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.TextBox txtPeakElect;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtPointElect;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtTotalElect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtCurrent;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtValtage;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.ComboBox cbEmergencyBtn;
        private System.Windows.Forms.ComboBox cbCurState;
        private System.Windows.Forms.ComboBox cbChargePlug;
        private System.Windows.Forms.ComboBox cbMeterState;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label heartFrameLed;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnSetCurInfo;
        private System.Windows.Forms.TextBox txtCurValleyCost;
        private System.Windows.Forms.Label label37;
        private System.Windows.Forms.TextBox txtCurFlatCost;
        private System.Windows.Forms.Label label38;
        private System.Windows.Forms.TextBox txtCurPeakCost;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.TextBox txtCurPointCost;
        private System.Windows.Forms.Label label40;
        private System.Windows.Forms.TextBox txtCurValleyPrice;
        private System.Windows.Forms.Label label41;
        private System.Windows.Forms.TextBox txtCurFlatPrice;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.TextBox txtCurPeakPrice;
        private System.Windows.Forms.Label label43;
        private System.Windows.Forms.TextBox txtCurPointPrice;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox txtCurValleyElect;
        private System.Windows.Forms.Label label36;
        private System.Windows.Forms.TextBox txtCurFlatElect;
        private System.Windows.Forms.Label label49;
        private System.Windows.Forms.TextBox txtCurPeakElect;
        private System.Windows.Forms.Label label50;
        private System.Windows.Forms.TextBox txtCurPointElect;
        private System.Windows.Forms.Label label51;
        private System.Windows.Forms.TextBox txtCurTotalCost;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.TextBox txtCurTotalElect;
        private System.Windows.Forms.Label label53;
        private System.Windows.Forms.Timer heartLedTime;
        private System.Windows.Forms.Timer closeheartTime;
        private System.Windows.Forms.Timer ChargeSocTime;
        private System.Windows.Forms.Timer ChargeTimeTimer;
        private System.Windows.Forms.TreeView tvChargePile;
    }
}

