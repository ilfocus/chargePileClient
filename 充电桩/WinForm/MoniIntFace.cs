using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Data.OleDb;    // 数据库对象
using System.Collections;
using System.Reflection;    // missing.values
using System.IO;            //File.Exists
using ChargingPile.Properties;
using ChargingPile.WinForm;
using ZedGraph;
using ChargingPile.Class;

using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;

// 网络和多线程
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChargingPile
{
    
    public partial class MonitoringInterface : Form
    {
        private const int NUM_OF_MAIN_DEVICE = 20;
        private const int NUM_OF_SUBDEVICE   = 6;
        // 创建标签数组
        private System.Windows.Forms.Label[] subMachineErrorStatus_1 = new System.Windows.Forms.Label[30];
        private System.Windows.Forms.Label[] subMachineErrorStatus_2 = new System.Windows.Forms.Label[30];
        private System.Windows.Forms.Label[] subMachineErrorStatus_3 = new System.Windows.Forms.Label[30];
        private System.Windows.Forms.Label[] subMachineErrorStatus_4 = new System.Windows.Forms.Label[30];
        private System.Windows.Forms.Label[] subMachineErrorStatus_5 = new System.Windows.Forms.Label[30];
        private System.Windows.Forms.Label[] subMachineErrorStatus_6 = new System.Windows.Forms.Label[30];
        // 把标签数组加入泛型队列中
        private List<System.Windows.Forms.Label[]> subMachineErrorStatusList = new List<System.Windows.Forms.Label[]>();

        private List<MainDevice> mainDev = new List<MainDevice>();// 创建主机对象集合

        Queue receiveByteQueue = Queue.Synchronized(new Queue());//线程安全的数据队列，用来中转串口来的数据

        PointPairList curAList = new PointPairList();
        PointPairList curBList = new PointPairList();

        LineItem curACurve;
        LineItem curBCurve;

        private int lastScanMainDevNum = 0;
        private int lastScanMainDevCount = 0;
        private bool blDataFlag = false;                        // 数据接收完成标志
        private byte ubNumberTestMachine = 0x00;                // 主机号

        TreeNode node1 = new TreeNode();

        SetComPara frmSetComPara = new SetComPara();            // 定义串口参数设置对象
        SetChargePile frmOnOffSet = new SetChargePile();                  // 定义开关机设置对象
        private ParameterSet paraSetForm = null;                // 参数显示对象

        private byte[] byteArray = new byte[62];                // 因为串口接收数据会出现不连续
        private int iDataLenRecrod = 0;                         // 所以用数组把数据连接起来 
        UInt16 uiCheckCnt = 0;                                  // 数据校验错误后，重传计数
        private bool bQueryMsgFlg = false;                      // 查询数据数据包发送标志

        public string mFilename;
        public Excel.Application app;
        public Excel.Workbooks wbs;
        public Excel.Workbook wb;
        public Excel.Worksheets wss;
        public Excel.Worksheet ws;
        private UInt16 uwExcelDataCnt = 0;
        private bool FileExistsFlag = true;

        static readonly UInt16[] wCRC16Table = {
         0x0000, 0xC0C1, 0xC181, 0x0140, 0xC301, 0x03C0, 0x0280, 0xC241,
         0xC601, 0x06C0, 0x0780, 0xC741, 0x0500, 0xC5C1, 0xC481, 0x0440,
         0xCC01, 0x0CC0, 0x0D80, 0xCD41, 0x0F00, 0xCFC1, 0xCE81, 0x0E40, 
         0x0A00, 0xCAC1, 0xCB81, 0x0B40, 0xC901, 0x09C0, 0x0880, 0xC841,
         0xD801, 0x18C0, 0x1980, 0xD941, 0x1B00, 0xDBC1, 0xDA81, 0x1A40,  
         0x1E00, 0xDEC1, 0xDF81, 0x1F40, 0xDD01, 0x1DC0, 0x1C80, 0xDC41,
         0x1400, 0xD4C1, 0xD581, 0x1540, 0xD701, 0x17C0, 0x1680, 0xD641,
         0xD201, 0x12C0, 0x1380, 0xD341, 0x1100, 0xD1C1, 0xD081, 0x1040,
         0xF001, 0x30C0, 0x3180, 0xF141, 0x3300, 0xF3C1, 0xF281, 0x3240,
         0x3600, 0xF6C1, 0xF781, 0x3740, 0xF501, 0x35C0, 0x3480, 0xF441,   
         0x3C00, 0xFCC1, 0xFD81, 0x3D40, 0xFF01, 0x3FC0, 0x3E80, 0xFE41,
         0xFA01, 0x3AC0, 0x3B80, 0xFB41, 0x3900, 0xF9C1, 0xF881, 0x3840,
         0x2800, 0xE8C1, 0xE981, 0x2940, 0xEB01, 0x2BC0, 0x2A80, 0xEA41,
         0xEE01, 0x2EC0, 0x2F80, 0xEF41, 0x2D00, 0xEDC1, 0xEC81, 0x2C40,
         0xE401, 0x24C0, 0x2580, 0xE541, 0x2700, 0xE7C1, 0xE681, 0x2640,   
         0x2200, 0xE2C1, 0xE381, 0x2340, 0xE101, 0x21C0, 0x2080, 0xE041,
         0xA001, 0x60C0, 0x6180, 0xA141, 0x6300, 0xA3C1, 0xA281, 0x6240,
         0x6600, 0xA6C1, 0xA781, 0x6740, 0xA501, 0x65C0, 0x6480, 0xA441,
         0x6C00, 0xACC1, 0xAD81, 0x6D40, 0xAF01, 0x6FC0, 0x6E80, 0xAE41,
         0xAA01, 0x6AC0, 0x6B80, 0xAB41, 0x6900, 0xA9C1, 0xA881, 0x6840,   
         0x7800, 0xB8C1, 0xB981, 0x7940, 0xBB01, 0x7BC0, 0x7A80, 0xBA41,
         0xBE01, 0x7EC0, 0x7F80, 0xBF41, 0x7D00, 0xBDC1, 0xBC81, 0x7C40,
         0xB401, 0x74C0, 0x7580, 0xB541, 0x7700, 0xB7C1, 0xB681, 0x7640,
         0x7200, 0xB2C1, 0xB381, 0x7340, 0xB101, 0x71C0, 0x7080, 0xB041,
         0x5000, 0x90C1, 0x9181, 0x5140, 0x9301, 0x53C0, 0x5280, 0x9241,  
         0x9601, 0x56C0, 0x5780, 0x9741, 0x5500, 0x95C1, 0x9481, 0x5440,
         0x9C01, 0x5CC0, 0x5D80, 0x9D41, 0x5F00, 0x9FC1, 0x9E81, 0x5E40,
         0x5A00, 0x9AC1, 0x9B81, 0x5B40, 0x9901, 0x59C0, 0x5880, 0x9841,
         0x8801, 0x48C0, 0x4980, 0x8941, 0x4B00, 0x8BC1, 0x8A81, 0x4A40,
         0x4E00, 0x8EC1, 0x8F81, 0x4F40, 0x8D01, 0x4DC0, 0x4C80, 0x8C41,  
         0x4400, 0x84C1, 0x8581, 0x4540, 0x8701, 0x47C0, 0x4680, 0x8641,
         0x8201, 0x42C0, 0x4380, 0x8341, 0x4100, 0x81C1, 0x8081, 0x4040,
        };
        public MonitoringInterface()
        {
            InitializeComponent();

            skinEngine2.SkinFile = System.Windows.Forms.Application.StartupPath.Replace(@"\bin\Debug", "") + @"\Resources\GlassGreen.ssk";
            skinEngine2.SkinAllForm = true;
        }
        public void CRC_16(byte[] pDataIn, int iLenIn, out  UInt16 pCRCOut)
        {

            UInt16 wResult = 0;
            UInt16 wTableNo = 0;
            UInt16 i = 0;

            for (i = 0; i < iLenIn; i++)
            {
                wTableNo = (UInt16)((wResult & 0xff) ^ (pDataIn[i] & 0xff));
                wResult = (UInt16)(((wResult >> 8) & 0xff) ^ wCRC16Table[(UInt16)wTableNo]);
            }

            pCRCOut = wResult;
        }
        // 设备数据模型初始化，main_device为老化车，subDevice为老化车上的分机
        private void DevicesInit()
        {
            for (int j = 0; j < NUM_OF_MAIN_DEVICE; j++) {   // 20个老化车（主机）
            
                MainDevice m = new MainDevice();            // 创建一个老化车对象
                
                m.Id = j;
                m.Name = "mainDev_" + j;
                m.DevList = new List<SubDevice>();          // 老化车中的分机设备，分机设备泛型表
                
                for (int i = 0; i < NUM_OF_SUBDEVICE; i++) { // 六个分机设备
                
                    SubDevice sub = new SubDevice();        // 创建分机对象
                    
                    sub.Id = i;
                    sub.Name = "subDev_" + i;
                    sub.CurrentAList = new PointPairList(); // ?
                    sub.CurrentBList = new PointPairList();
                    sub.EStatus = 0;                        // 分机参数初始化

                    m.DevList.Add(sub);                     // 把分机添加进分机设备泛型表
                }
                mainDev.Add(m);                             // 添加主机泛型队列
            }
        }
        private void testNode()
        {//测试Model数据的使用
            Queue queue = new Queue();

            for (int j = 0; j < NUM_OF_MAIN_DEVICE; j++)
            {
                MainDevice m = mainDev[j];
                Console.WriteLine("|-i--{0}",m.Name);
                for (int i = 0; i < NUM_OF_SUBDEVICE; i++)
                {
                    SubDevice sub = m.DevList[i];
                    Console.WriteLine("  |-{0}", sub.Name);
                }
            }
            for (int i = 0; i < 6; i++)
            {
                String s = "num_" + i;
                queue.Enqueue(s);
            }
            for (int i = 0; i < 6; i++)
            {
                String s = (String)queue.Dequeue();
                Console.WriteLine(s);
            }

        }
        /// <summary>
        /// 对界面上故障点数据进行更新
        /// </summary>
        

        /*************************************************************************/
        /*  
        *   功能描述：各个窗体对象，用于父子窗体间资源共享
        *   修改日期：无
        *   修改内容：无
        */
        /*************************************************************************/

        private void Form1_Load(object sender, EventArgs e)
        {
            //加载皮肤 
            skinEngine2.SkinFile = System.Windows.Forms.Application.StartupPath.Replace(@"\bin\Debug", "") + @"\skins\SteelBlue.ssk";
            skinEngine2.SkinAllForm = true;

            ////////////////////////串口端口初始化////////////////////////////////////
            try
            {
                string[] szPorts = SerialPort.GetPortNames(); //获取当前可用的串口列表
                //这里得到的可用的串口，是指电脑里面可用的串口。
                frmSetComPara.combSerialPort.Items.AddRange(szPorts);
                frmSetComPara.combSerialPort.SelectedIndex = 0;
            }
            catch (Win32Exception win32ex) // 获取串口出错
            {
                MessageBox.Show(win32ex.ToString());
            }
            ////////////////////////////////////////////////////////////////////////
            
            //设备数据模型初始化
            DevicesInit();

            frmSetComPara.combBaudRate.Text     = "9600";
            frmSetComPara.combDataBit.Text      = "8";
            frmSetComPara.combStopBits.Text     = "1";
            frmSetComPara.SerialParity.Text     = "None";
            frmSetComPara.combSerialPort.Text   = "COM1";

            // 以上部分为故障点初始化

            serialPort1.Encoding = Encoding.GetEncoding("gb2312");//接收发送转换编码
            Console.WriteLine("buff size:{0}", serialPort1.ReadBufferSize);
            // 测试模块
            for (int j = 0; j < 20; j++)
            {
                //TreeNode node2 = new TreeNode((j + 1).ToString()+"号老化车");
                TreeNode node2 = new TreeNode((j + 1).ToString() + "号老化车");
                node1.Nodes.Add(node2);
            }

            //zedGraph 初始化
            zedGraphControl1.GraphPane.Title.Text = "A/B路电流实时曲线";
            zedGraphControl1.GraphPane.XAxis.Title.Text = "时间";
            zedGraphControl1.GraphPane.YAxis.Title.Text = "电流";
            zedGraphControl1.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;
            zedGraphControl1.GraphPane.XAxis.Scale.MaxAuto = true;

            curACurve = zedGraphControl1.GraphPane.AddCurve("电流A路", curAList, Color.DarkGreen, SymbolType.None);
            curBCurve = zedGraphControl1.GraphPane.AddCurve("电流A路", curBList, Color.DarkRed, SymbolType.None);
           // zedGraphControl1.
            this.zedGraphControl1.AxisChange();
            this.zedGraphControl1.Refresh();

           // ZedGraphTest();
            TimSysTime.Enabled = true;

            // 设置winForm中的默认值
            cbEmergencyBtn.SelectedIndex = 0;
            cbMeterState.SelectedIndex = 0;
            cbChargePlug.SelectedIndex = 0;
            cbCurState.SelectedIndex = 0;

            // 加入这行,容许跨线程访问控件
            Control.CheckForIllegalCrossThreadCalls = false;

        }

        /* 充机桩模拟器
         * 1、充电桩与地锁通信
         *    1> 地锁：请求充电桩动作
         *    2> 地锁：查询充电桩动作
         * 2、充电桩与后台通信
         *    1> 后台：桩心跳
         *    2> 后台：设置时间
         *    3> 后台：设置费率
         *    4> 后台：获得充电桩状态
         *    5> 后台: 控制充电桩启停
         *    6> 后台: 获取当前充电信息
         * 3、帧格式分成请求帧和响应帧
         *    1> 请求帧(byte)
         *      帧头：2
         *      桩地址:8
         *      帧长度:1
         *      命令码:1
         *      参数:N
         *      BCC校验:1
         *      帧尾:1
         *    2> 应答帧
         *      帧头：2
         *      桩地址:8
         *      帧长度:1
         *      命令码:1
         *      响应帧标志:1
         *      参数:N
         *      BCC校验:1
         *      帧尾:1
         */
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] byteArray = new byte[serialPort1.ReadBufferSize];        // 创建串口接收数据数组

            int len = serialPort1.Read(byteArray, 0, byteArray.Length);

            // 对接收到的数据进行预处理
            if (  0xff == byteArray[0] && 0x5a == byteArray[1]) { //快速初步校验
               
                byte[] ubReceDataBuff = new byte[len];
                for (int i = 0; i < len; i++) {
                    ubReceDataBuff[i] = byteArray[i];
                }   // 把一帧数据暂存在ubReceiveDataBuffer数组中
                receiveByteQueue.Enqueue(ubReceDataBuff);//把数据放入队列中，先进先出
                blDataFlag = true;
            }
        }
       
        private void btnOpenPort_Click(object sender, EventArgs e) {
            /* 代码测试 */
            
            //int a =  10;
            //picBox1.Image = Properties.Resources.red;
            //txtCurrent.Text = a.ToString();
 
            if (btnOpenPort.Text == "打开串口") {
                //this.serialPort1.Open();//打开串口，用的是控件！

                this.btnOpenPort.Text = "关闭串口";
                //serialPort1.PortName = frmSetComPara.combSerialPort.Text;               //给出串口号，字符型

                serialPort1.PortName = "COM3";
                serialPort1.BaudRate = int.Parse(frmSetComPara.combBaudRate.Text);
                serialPort1.DataBits = int.Parse(frmSetComPara.combDataBit.Text);
                string szStopBits = frmSetComPara.combStopBits.SelectedItem.ToString();

                switch (szStopBits) {
                    case "1":
                        serialPort1.StopBits = StopBits.One;
                        break;
                    case "1.5":
                        serialPort1.StopBits = StopBits.OnePointFive;
                        break;
                    case "2":
                        serialPort1.StopBits = StopBits.Two;
                        break;
                    default:
                        serialPort1.StopBits = StopBits.One;
                        break;
                }
                serialPort1.Parity = (Parity)Enum.Parse(typeof(Parity), frmSetComPara.SerialParity.SelectedItem.ToString());
                //此句要好好理解。Enum  提供一个指向枚举器（该枚举器可枚举复合名字对象的组件）的指针。
                serialPort1.Open();
                return;
            }
            if (btnOpenPort.Text == "关闭串口") {
                serialPort1.Close();

                this.btnOpenPort.Text = "打开串口";
            }
            if (serialPort1.IsOpen == true) {
                MessageBox.Show("串口已打开");
            } else {
                MessageBox.Show("串口已关闭");
            }
        }
        private void TS_BtnSetComPara_Click(object sender, EventArgs e)
        {
            SetComPara frmSetComParaDisply = new SetComPara();          //定义串口参数设置对象
            frmSetComParaDisply.Show();
        }
        private void TS_BtnOnOffSet_Click(object sender, EventArgs e)
        {
            SetChargePile frmOnOffSetDisply = new SetChargePile();

            frmOnOffSetDisply.Show();

        }
        private void TS_BtnExit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        private void TS_BtnOpenPort_Click(object sender, EventArgs e)
        {
            if (paraSetForm == null || paraSetForm.IsDisposed)    //对象为空或对象没有被占用
            {
                paraSetForm = new ParameterSet();
            }
            paraSetForm.Owner = this;
            paraSetForm.Show();
        }
        
        public void Create()//创建一个Excel对象
        {
            app = new Excel.Application();
            wbs = app.Workbooks;
            wb = wbs.Add(true);
        }
        public void Open(string FileName)//打开一个Excel文件
        {
            app = new Excel.Application();
            wbs = app.Workbooks;
            wb = wbs.Add(FileName);
            mFilename = FileName;
        }
        public Excel.Worksheet GetSheet(string SheetName)//获取一个工作表  
        {
            Excel.Worksheet s = (Excel.Worksheet)wb.Worksheets[SheetName];
            return s;
        }
        public Excel.Worksheet AddSheet(string SheetName)//添加一个工作表
        {
            Excel.Worksheet s = (Excel.Worksheet)wb.Worksheets.Add();
            s.Name = SheetName;
            return s;
        } 
        private void BtnSaveData_Click(object sender, EventArgs e)
        {   // EXCEL表自动创建并保存 1分钟保存一次
            
            string path = System.Environment.CurrentDirectory + "\\" + "老化车历史数据记录";  //设置保存路径

            Excel.Application excel = new Excel.Application();  //引用Excel对象
            Excel.Workbook xBook = null;                        // Excel对象的excel文件
            Excel._Worksheet workSheet;                         // excel文件的一个工作表
            if (File.Exists(path + ".XLS ")) { // 存在
                xBook = excel.Workbooks._Open(path,
                  Missing.Value, Missing.Value, Missing.Value, Missing.Value
                , Missing.Value, Missing.Value, Missing.Value, Missing.Value
                , Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                
                workSheet = (Excel.Worksheet)xBook.Sheets[1];
                workSheet = (Excel.Worksheet)xBook.ActiveSheet;
                FileExistsFlag = true;
            } else { // 不存在
                excel.Workbooks.Add();
                workSheet = (Excel.Worksheet)excel.ActiveSheet;                // 激活当前工作文档      
                FileExistsFlag = false;
            }
            //////////////////////////////////////////////////////////////////////////
            //写入数据
            //Excel.Range rng3 = workSheet.get_Range("C6", Missing.Value);
            //rng3.Value2 = "Hello";
            //rng3.Interior.ColorIndex = 6; //设置Range的背景色
                                         
            //workSheet.Cells[1, "A"] = "ID Number";
            //workSheet.Cells[1, "B"] = "Current Balance";

            workSheet.Cells[3, "A"] = "记录时间";
            workSheet.Cells[3, "B"] = "老化车号";
            workSheet.Cells[3, "C"] = "工作电流";
            workSheet.Cells[3, "D"] = "1号分机故障点";
            workSheet.Cells[3, "E"] = "2号分机故障点";
            workSheet.Cells[3, "F"] = "3号分机故障点";
            workSheet.Cells[3, "G"] = "4号分机故障点";
            workSheet.Cells[3, "H"] = "5号分机故障点";
            workSheet.Cells[3, "I"] = "6号分机故障点";
            //string strNowTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            string strNowTime = DateTime.Now.ToString();
            workSheet.Cells[4 + uwExcelDataCnt, "A"] = strNowTime;
            workSheet.Cells[4 + uwExcelDataCnt, "B"] = "1号老化车";
            workSheet.Cells[4 + uwExcelDataCnt, "C"] = "1000mA";
            workSheet.Cells[4 + uwExcelDataCnt, "D"] = "1,5,28,30";
            workSheet.Cells[4 + uwExcelDataCnt, "E"] = "2,5,28,30";
            workSheet.Cells[4 + uwExcelDataCnt, "F"] = "3,5,18,30";
            workSheet.Cells[4 + uwExcelDataCnt, "G"] = "4,5,28,30";
            workSheet.Cells[4 + uwExcelDataCnt, "H"] = "5,5,28,30";
            workSheet.Cells[4 + uwExcelDataCnt, "I"] = "6,5,28,30";
            uwExcelDataCnt++;
            if (uwExcelDataCnt > 7200) { // 可以保护5天的数据量
                uwExcelDataCnt = 7200;
            }
            for (int i = 1; i < 10;i++) {
                workSheet.Columns[i].AutoFit();
            }
            
            //////////////////////////////////////////////////////////////////////////

            ////////////////excel文件保存//////////////////////////////
            excel.Visible = false;
            if (true == FileExistsFlag) {
                xBook.Save();
            } else {
                excel.ActiveWorkbook.SaveAs(path + ".XLS ",
                   Excel.XlFileFormat.xlExcel7, null, null, false, false,
                   Excel.XlSaveAsAccessMode.xlNoChange, null, null, null, null, null);
            }
            excel.Quit();
            excel = null;
            GC.Collect();//垃圾回收
            ///////////////excel文件保存结束//////////////////////////
        }

        private void SendQueryMsg() {
            int mainId = lastScanMainDevNum;
            const int QUERY_MSG_NUM = 12;
            byte[] cScanCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组
            MainDevice m = mainDev[mainId];     // 老化车泛型集合
            UInt16 uwCrcResult;                 // CRC校验
            if (m.isActive == false && lastScanMainDevCount < 2) {  // 这里设置每个老化车查询次数，当前3次，如果3次内成功，则不会重试
                lastScanMainDevCount++;
            } else {
                mainId++;                                           // 老化车查询计数变量
                if (mainId >= 4) {                                  // 设置轮询台数，这里假设只有4台，应该换成 NUM_OF_MAIN_DEVICE
                    mainId = 0;                                     // 清空状态，准备下一轮查询
                    for (int i = 0; i < NUM_OF_MAIN_DEVICE; i++) { // 20台老化车设备，在线状态清空
                        MainDevice md = mainDev[i];
                        md.isActive = false;                        // isActive---每一台老化车设备的状态，true则为设备在线
                    }
                }
                lastScanMainDevCount = 0;                           // 老化车查询计数清零
                lastScanMainDevNum = mainId;                        // 老化车号自加后再把值送给当前要查询的设备号
            }
            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                cScanCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            cScanCmd[0] = 0xaa;             // 起始字符高位
            cScanCmd[1] = 0x55;             // 起始字符低位
            cScanCmd[2] = QUERY_MSG_NUM;    // 数据长度
            cScanCmd[3] = 0xff;             // 源地址(PC主机地址)
            cScanCmd[4] = (byte)(mainId+0x01);     // 目标地址-主机号，发送主机号到数据采样器，数据采集器由根据主机号去查询相应的主机。

            cScanCmd[5] = 0x01;             // 功能码
            cScanCmd[6] = 0x00;
            cScanCmd[7] = 0x00;
            cScanCmd[8] = 0x00;
            cScanCmd[9] = 0x00;
            CRC_16(cScanCmd, QUERY_MSG_NUM - 2, out uwCrcResult);   // 对前10个数作校验
            cScanCmd[10] = (byte)(uwCrcResult >> 8); ;               // 校验码高位
            cScanCmd[11] = (byte)uwCrcResult;                        // 校验码低位
            serialPort1.Write(cScanCmd, 0, QUERY_MSG_NUM);
        }
        private void TimeSendData_Tick(object sender, EventArgs e)// 轮询向分机请求数据定时器任务
        {
            if (bQueryMsgFlg == false) {
                //SendQueryMsg();
                bQueryMsgFlg = true;
            }
            
        }
        static Socket clientSocket;
        private static byte[] result = new byte[1024];
        private bool receiveDataThreadFlg = false;
        private void clientReceiveData(object clientSocket) {
            Socket myClientSocket = (Socket)clientSocket;
            while (receiveDataThreadFlg) {
                try {
                    // 通过clientSocket接收数据
                    int receiveNumber = myClientSocket.Receive(result);
                    byte[] tempArray = new byte[receiveNumber];

                    for (int i = 0; i < receiveNumber; i++) {
                        tempArray[i] = result[i];
                    }
                    packageParser(tempArray, receiveNumber);
                   // Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
            }
        }

        private void sendBaseDataSocket(byte cmdCode,Socket socket) {

            const int QUERY_MSG_NUM = 16;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;
            if (txtChargingPileAddress.Text != "") {
                cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            }
            for (int i = 0; i < QUERY_MSG_NUM; i++) {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48);
            bRequestCmd[4] = (byte)(cpAddress >> 40);
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24); ;
            bRequestCmd[7] = (byte)(cpAddress >> 16); ;
            bRequestCmd[8] = (byte)(cpAddress >> 8); ;
            bRequestCmd[9] = (byte)(cpAddress); ;
            //
            bRequestCmd[10] = 0x05;               // 帧长度
            bRequestCmd[11] = cmdCode;            // 命令码
            bRequestCmd[12] = 0xAC;               // 响应帧标志

            /* 参数详情
             * 0x00 空闲
             * 0x01 开始充电
             * 0x02 充电结束
             * 0x03 未知状态
             * 0x04 未知状态
             * 0x10 故障
             * 0x11 故障1
             * 0x12 故障2
             */
            bRequestCmd[13] = 0x00;               // 响应状态/参数
            if (cmdCode == 0x20) {
                bRequestCmd[13] = bHeartFrameResponeState;               // 响应状态
            } else if (cmdCode == 0x21) {
                bRequestCmd[13] = bSetTimeResponeState;
            } else if (cmdCode == 0x22) {
                bRequestCmd[13] = bSetRateResponeState;
            }

            // 帧尾
            bRequestCmd[14] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[15] = 0xed;
            //serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            clientSocket.Send(bRequestCmd,QUERY_MSG_NUM,0);
        }
        private void socketClient() {
            //设定服务器IP地址
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                clientSocket.Connect(new IPEndPoint(ip, 8885)); //配置服务器IP与端口
            } catch {
                return;
            }
            //通过clientSocket接收数据

            // 连接之后，发送心跳包到服务器
            sendBaseDataSocket(0x20,clientSocket);
            Thread myThread = new Thread(clientReceiveData);
            myThread.Start(clientSocket);
        }

        private void btnGetData_Click(object sender, EventArgs e)//轮询任务启动
        {
            if (btnGetData.Text == "打开监听") {
                btnGetData.Text = "关闭监听";
                //TimeSendData.Enabled = true;            // 打开定时器作扫描功能，向数据采集器发送扫描指令
                socketClient();
                receiveDataThreadFlg = true;
            } else {
                btnGetData.Text = "打开监听";
                //TimeSendData.Enabled = false;           // 关闭定时器扫描功能，停止向数据采集器发送扫描指令 
                receiveDataThreadFlg = false;
            }
        }
        
       
        private void DealDisplayMsg(byte[] arr, int length)
        {
            int mainId = arr[3];            // 主机号
            MainDevice m = mainDev[mainId]; // 主机号对应的设备类
            m.isActive = true;              // 主机激活
            // 1、如果主机是第一次激活，要记录下此时的时间，作为开始工作时间
            // 2、用开始加上需要运行的时间，得到工作结束时间
            // 3、用系统此时时间减去开始工作时间得到已经工作时间
            // 4、用工作结束时间减去此时时间得到工作剩余时间

            //Console.WriteLine("MainID: {1}, len:{0}aa 55", length,mainId);
            
            double xTimeAxis = (double)new XDate(DateTime.Now);//使用当前时间作为X轴坐标

            for (int i = 0; i < 6; i++) {
                int subId = arr[6 + 9 * i + 0];
                int curA  = arr[6 + 9 * i + 1] << 8 | arr[6 + 9 * i + 2];
                int curB  = arr[6 + 9 * i + 3] << 8 | arr[6 + 9 * i + 4];
                int error = arr[6 + 9 * i + 8] << 24 | arr[6 + 9 * i + 7] << 16 | arr[6 + 9 * i + 6] << 8 | arr[6 + 9 * i + 5];
                SubDevice sub = m.DevList[subId];
                sub.CurrentAList.Add(xTimeAxis, curA);//A路电流值
                sub.CurrentBList.Add(xTimeAxis, curB);//B路电流值
                sub.EStatus = error;                  //故障点信息
                //txtCurrent.Text = curA.ToString();
            }
            m.iWorkCurrent = 1000;
            //m.iWorkCurrent = 1000;
            //txtBeginTime.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        }
        private byte bHeartFrameResponeState = 0x00;
        private byte bSetTimeResponeState = 0x00;
        private byte bSetRateResponeState = 0x00;
        private byte bGetCPStateResponeState = 0x00;
        private byte bCPStartupResponeState = 0x00;
        private byte bCurInfoResponeState = 0x00;
        private void respondRequestCmd(byte cmdCode)
        {
            const int QUERY_MSG_NUM = 16;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++)
            {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48); 
            bRequestCmd[4] = (byte)(cpAddress >> 40); 
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24); ;
            bRequestCmd[7] = (byte)(cpAddress >> 16); ;
            bRequestCmd[8] = (byte)(cpAddress >> 8); ;
            bRequestCmd[9] = (byte)(cpAddress); ;
            //
            bRequestCmd[10] = 0x05;               // 帧长度
            bRequestCmd[11] = cmdCode;            // 命令码
            bRequestCmd[12] = 0xAC;               // 响应帧标志

            /* 参数详情
             * 0x00 空闲
             * 0x01 开始充电
             * 0x02 充电结束
             * 0x03 未知状态
             * 0x04 未知状态
             * 0x10 故障
             * 0x11 故障1
             * 0x12 故障2
             */
            bRequestCmd[13] = 0x00;               // 响应状态/参数
            if (cmdCode == 0x20) {
                bRequestCmd[13] = bHeartFrameResponeState;               // 响应状态
            } else if (cmdCode == 0x21) {
                bRequestCmd[13] = bSetTimeResponeState;
            } else if (cmdCode == 0x22) {
                bRequestCmd[13] = bSetRateResponeState;
            }

                          // 帧尾
            bRequestCmd[14] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[15] = 0xed; 
            if (btnGetData.Text == "关闭监听") {
                clientSocket.Send(bRequestCmd,QUERY_MSG_NUM,0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
            
        }

        private byte chargeSOC = 0;
        private byte chargeTime = 0;
        private byte chargeSurplusTime = 58;

        private string chargeIntToString(UInt32 data) {
            return ((data / 100).ToString() + "." + (data % 100).ToString());
        }
        private void respondCP_StateCmd(byte cmdCode)
        {
            const int QUERY_MSG_NUM = 51;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++)
            {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48);
            bRequestCmd[4] = (byte)(cpAddress >> 40);
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24); ;
            bRequestCmd[7] = (byte)(cpAddress >> 16); ;
            bRequestCmd[8] = (byte)(cpAddress >> 8); ;
            bRequestCmd[9] = (byte)(cpAddress);
            //
            bRequestCmd[10] = 0x28;               // 帧长度
            bRequestCmd[11] = cmdCode;            // 命令码
            bRequestCmd[12] = 0xAC;               // 响应帧标志

            /* 参数详情
             * 0x00 空闲
             * 0x01 开始充电
             * 0x02 充电结束
             * 0x03 未知状态
             * 0x04 未知状态
             * 0x10 故障
             * 0x11 故障1
             * 0x12 故障2
             */
            //bRequestCmd[13] = 0x00;               // 响应状态
            bRequestCmd[13] = bGetCPStateResponeState;
            //if (cmdCode == 0x20) {
            //    bRequestCmd[13] = 0x00;               // 响应状态
            //}

            //Random ran = new Random();
            //int RandKey = ran.Next(100, 9999);// 1A - 99A

            // 此处有34个数据
            // 电压，精确到小数点后2位，比如220.13V,实际传输数据22013，即是00 00 55 FD 

            //UInt32 valtage = 22013;
            UInt32 valtage = 0;
            UInt32 current = 0;
            UInt32 chargePointElect = 0;
            UInt32 chargePeakElect = 0;
            UInt32 chargeFlatElect = 0;
            UInt32 chargeValleyElect = 0;
            UInt32 chargeTotalElect = 0;
            byte emergencyStopButton = 0;
            byte electMeter = 0;
            byte chargePlug = 0;
            byte currentState = 0;


            if (btnSetData.Text == "设置数据") {
                Random ran = new Random();
                valtage = (UInt32)ran.Next(18000, 26000);// 180 - 220V
                current = (UInt32)ran.Next(100, 10000);  // 1A - 100A
                // 充电时的尖电量  
                chargePointElect = (UInt32)ran.Next(0, 100000);
                // 充电时的峰电量
                chargePeakElect = (UInt32)ran.Next(0, 100000);
                // 充电时的平电量
                chargeFlatElect = (UInt32)ran.Next(0, 100000);
                // 充电时的谷电量
                chargeValleyElect = (UInt32)ran.Next(0, 100000);
                // 充电时的总电量
                chargeTotalElect = chargePointElect + chargePeakElect + chargeFlatElect + chargeValleyElect;
                
                emergencyStopButton = (byte)ran.Next(0, 2);
                electMeter = (byte)ran.Next(0, 2); ;
                chargePlug = (byte)ran.Next(0, 2); ;
                currentState = (byte)ran.Next(0, 2);

                // 把随机数据显示在界面中

                //Thread myThread = new Thread()
                txtValtage.Text = chargeIntToString(valtage);
                txtCurrent.Text = chargeIntToString(current);

                txtTotalElect.Text = chargeIntToString(chargeTotalElect);

                txtPointElect.Text = chargeIntToString(chargePointElect);
                txtPeakElect.Text = chargeIntToString(chargePeakElect);
                txtFlatElect.Text = chargeIntToString(chargeFlatElect);
                txtValleyElect.Text = chargeIntToString(chargeValleyElect);

                if (emergencyStopButton == 0) {
                    cbEmergencyBtn.SelectedIndex = 0;
                } else {
                    cbEmergencyBtn.SelectedIndex = 1;
                }

                if (electMeter == 0) {
                    cbMeterState.SelectedIndex = 0;
                } else {
                    cbMeterState.SelectedIndex = 1;
                }

                if (chargePlug == 0) {
                    cbChargePlug.SelectedIndex = 0;
                } else {
                    cbChargePlug.SelectedIndex = 1;
                }

                if (currentState == 0) {
                    cbCurState.SelectedIndex = 0;
                } else {
                    cbCurState.SelectedIndex = 1;
                }

            } else {
                valtage = Convert.ToUInt32(txtValtage.Text);
                current = Convert.ToUInt32(txtCurrent.Text);
                chargePointElect = Convert.ToUInt32(txtPointElect.Text);
                chargePeakElect = Convert.ToUInt32(txtPeakElect.Text);
                chargeFlatElect = Convert.ToUInt32(txtFlatElect.Text);
                chargeValleyElect = Convert.ToUInt32(txtValtage.Text);

                if (cbEmergencyBtn.Text == "正常") {
                    emergencyStopButton = 0x00;
                } else {
                    emergencyStopButton = 0x01;
                }

                if (cbMeterState.Text == "通信正常") {
                    electMeter = 0x00;
                } else {
                    electMeter = 0x01;
                }

                if (cbChargePlug.Text == "插好") {
                    chargePlug = 0x00;
                } else {
                    chargePlug = 0x01;
                }

                if (cbCurState.Text == "有输出") {
                    currentState = 0x00;
                } else {
                    currentState = 0x01;
                }

            }
            bRequestCmd[14] = (byte)(valtage >> 24);               // 电压
            bRequestCmd[15] = (byte)(valtage >> 16);               // 电压
            bRequestCmd[16] = (byte)(valtage >> 8);               // 电压
            bRequestCmd[17] = (byte)(valtage >> 0);               // 电压
            // 电流，同上
            //UInt32 current = 11111;

            bRequestCmd[18] = (byte)(current >> 24);               // 电流
            bRequestCmd[19] = (byte)(current >> 16);               // 电流
            bRequestCmd[20] = (byte)(current >> 8);               // 电流
            bRequestCmd[21] = (byte)(current >> 0);               // 电流
            // 充电时的总电量
            //UInt32 chargeTotalElect = 22222;
            bRequestCmd[22] = (byte)(chargeTotalElect >> 24);               // 充电时的总电量
            bRequestCmd[23] = (byte)(chargeTotalElect >> 16);               // 充电时的总电量
            bRequestCmd[24] = (byte)(chargeTotalElect >> 8);               // 充电时的总电量
            bRequestCmd[25] = (byte)(chargeTotalElect >> 0);               // 充电时的总电量
            // 充电时的尖电量  
            //UInt32 chargePointElect = 33333;
            bRequestCmd[26] = (byte)(chargePointElect >> 24);               // 充电时的尖电量
            bRequestCmd[27] = (byte)(chargePointElect >> 16);               // 充电时的尖电量
            bRequestCmd[28] = (byte)(chargePointElect >> 8);               // 充电时的尖电量
            bRequestCmd[29] = (byte)(chargePointElect >> 0);               // 充电时的尖电量
            // 充电时的峰电量
            //UInt32 chargePeakElect = 44444;
            bRequestCmd[30] = (byte)(chargePeakElect >> 24);               // 充电时的峰电量
            bRequestCmd[31] = (byte)(chargePeakElect >> 16);               // 充电时的峰电量
            bRequestCmd[32] = (byte)(chargePeakElect >> 8);               // 充电时的峰电量
            bRequestCmd[33] = (byte)(chargePeakElect >> 0);               // 充电时的峰电量压
            // 充电时的平电量
            //UInt32 chargeFlatElect = 55555;
            bRequestCmd[34] = (byte)(chargeFlatElect >> 24);               // 充电时的平电量
            bRequestCmd[35] = (byte)(chargeFlatElect >> 16);               // 充电时的平电量
            bRequestCmd[36] = (byte)(chargeFlatElect >> 8);               // 充电时的平电量
            bRequestCmd[37] = (byte)(chargeFlatElect >> 0);               // 充电时的平电量
            // 充电时的谷电量
            // UInt32 chargeValleyElect = 66666;
            bRequestCmd[38] = (byte)(chargeValleyElect >> 24);               // 充电时的谷电量
            bRequestCmd[39] = (byte)(chargeValleyElect >> 16);               // 充电时的谷电量
            bRequestCmd[40] = (byte)(chargeValleyElect >> 8);               // 充电时的谷电量
            bRequestCmd[41] = (byte)(chargeValleyElect >> 0);               // 充电时的谷电量 
            // 急停按钮状态
            bRequestCmd[42] = emergencyStopButton;                          // 急停按钮状态
            // 备用
            bRequestCmd[43] = chargeSOC;                                    // 备用,添加成当前SOC数
            // 电表状态
            bRequestCmd[44] = electMeter;                                   // 电表状态
            // 备用
            bRequestCmd[45] = chargeTime;                                         // 备用---充电时间
            // 充电插头
            bRequestCmd[46] = chargePlug;                                   // 充电插头
            // 备用
            bRequestCmd[47] = chargeSurplusTime;                           // 备用 --- 充电剩余时间
            // 当前状态
            bRequestCmd[48] = currentState;                                 // 当前状态
            // 帧尾
            bRequestCmd[49] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[50] = 0xed;
            if (btnGetData.Text == "关闭监听") {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        private void respondStartAndStopCmd(byte cmdCode,byte para)
        {
            const int QUERY_MSG_NUM = 17;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++)
            {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48);
            bRequestCmd[4] = (byte)(cpAddress >> 40);
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24);
            bRequestCmd[7] = (byte)(cpAddress >> 16);
            bRequestCmd[8] = (byte)(cpAddress >> 8);
            bRequestCmd[9] = (byte)(cpAddress);
            //
            bRequestCmd[10] = 0x06;               // 帧长度
            bRequestCmd[11] = cmdCode;               // 命令码
            bRequestCmd[12] = 0xAC;               // 响应帧标志

            /* 参数详情
             * 0x00 空闲
             * 0x01 开始充电
             * 0x02 充电结束
             * 0x03 未知状态
             * 0x04 未知状态
             * 0x10 故障
             * 0x11 故障1
             * 0x12 故障2
             */
            //bRequestCmd[13] = 0x00;               // 响应状态
            bRequestCmd[13] = bCPStartupResponeState;
            bRequestCmd[14] = para;               // 参数
            // 帧尾
            bRequestCmd[15] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[16] = 0xed;
            if (btnGetData.Text == "关闭监听") {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        private void respondChargeInfoCmd(byte cmdCode)
        {
            const int QUERY_MSG_NUM = 72;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;

            for (int i = 0; i < QUERY_MSG_NUM; i++)
            {
                bRequestCmd[i] = 0x00;
            }   // 要发送的数据初始化                      
            bRequestCmd[0] = 0xff;             // 起始字符高位
            bRequestCmd[1] = 0x5a;             // 起始字符低位
            // 充电桩地址
            bRequestCmd[2] = (byte)(cpAddress >> 56);
            bRequestCmd[3] = (byte)(cpAddress >> 48);
            bRequestCmd[4] = (byte)(cpAddress >> 40);
            bRequestCmd[5] = (byte)(cpAddress >> 32);
            bRequestCmd[6] = (byte)(cpAddress >> 24);
            bRequestCmd[7] = (byte)(cpAddress >> 16);
            bRequestCmd[8] = (byte)(cpAddress >> 8);
            bRequestCmd[9] = (byte)(cpAddress);
            //
            bRequestCmd[10] = 0x3D;               // 帧长度
            bRequestCmd[11] = cmdCode;            // 命令码
            bRequestCmd[12] = 0xAC;               // 响应帧标志

            /* 参数详情
             * 0x00 空闲
             * 0x01 开始充电
             * 0x02 充电结束
             * 0x03 未知状态
             * 0x04 未知状态
             * 0x10 故障
             * 0x11 故障1
             * 0x12 故障2
             */
            //bRequestCmd[13] = 0x00;               // 响应状态
            bRequestCmd[13] = bCurInfoResponeState;

            // 此处有56个数据
            // 充电时的总电量，精确到小数点后2位，比如220.13V,实际传输数据22013，即是00 00 55 FD
 
            UInt32 chargeTotalElect = 0;
            // 充电时的总费用
            UInt32 chargeTotalPrice = 0;
            // 充电尖电量
            UInt32 chargePointElect = 0;
            // 充电峰电量 
            UInt32 chargePeakElect = 0;
            // 充电平电量
            UInt32 chargeFlatElect = 0;
            // 充电谷电量
            UInt32 chargeValleyElect = 0;
            // 尖电价
            UInt32 chargePointPrice = 0;
            // 峰电价
            UInt32 chargePeakPrice = 0;
            // 平电价
            UInt32 chargeFlatPrice = 0;
            // 谷电价
            UInt32 chargeValleyPrice = 0;
            // 尖时段费用
            UInt32 chargePointCost = 0;
            // 峰时段费用
            UInt32 chargePeakCost = 0;
            // 平时段费用
            UInt32 chargeFlatCost = 0;
            // 谷时段费用
            UInt32 chargeValleyCost = 0;


            if (btnSetCurInfo.Text == "设置数据") {
                // 数据随机
                Random ran = new Random();

                chargePointElect    = (UInt32)ran.Next(0, 100000);// 0 - 1000
                chargePeakElect     = (UInt32)ran.Next(0, 100000);// 0 - 1000
                chargeFlatElect     = (UInt32)ran.Next(0, 100000);// 0 - 1000
                chargeValleyElect   = (UInt32)ran.Next(0, 100000);// 0 - 1000

                chargeTotalElect = chargePointElect + chargePeakElect + chargeFlatElect + chargeValleyElect;

                if (txtRatePointPrice.Text != "") {
                    chargePointPrice = Convert.ToUInt32(txtRatePointPrice.Text);
                    chargePeakPrice = Convert.ToUInt32(txtRatePeakPrice.Text);
                    chargeFlatPrice = Convert.ToUInt32(txtRateFlatPrice.Text);
                    chargeValleyPrice = Convert.ToUInt32(txtRateValleyPrice.Text);
                } else {
                    chargePointPrice = (UInt32)ran.Next(50, 200);
                    chargePeakPrice = (UInt32)ran.Next(50, 200);
                    chargeFlatPrice = (UInt32)ran.Next(50, 200);
                    chargeValleyPrice = (UInt32)ran.Next(50, 200);
                }
                chargePointCost     = chargePointElect * chargePointPrice;
                chargePeakCost      = chargePeakElect * chargePeakPrice;
                chargeFlatCost      = chargeFlatElect * chargeFlatPrice;
                chargeValleyCost    = chargeValleyElect * chargeValleyPrice;

                chargeTotalPrice = chargePointCost + chargePeakCost + chargeFlatCost + chargeValleyCost;

                //  把随机数据显示在界面中
                txtCurTotalElect.Text = chargeIntToString(chargeTotalElect);
                txtCurTotalCost.Text =  chargeIntToString(chargeTotalPrice);

                txtCurPointElect.Text = chargeIntToString(chargePointElect);
                txtCurPeakElect.Text = chargeIntToString(chargePeakElect);
                txtCurFlatElect.Text = chargeIntToString(chargeFlatElect);
                txtCurValleyElect.Text = chargeIntToString(chargeValleyElect);

                txtCurPointPrice.Text = chargeIntToString(chargePointPrice);
                txtCurPeakPrice.Text = chargeIntToString(chargePeakPrice);
                txtCurFlatPrice.Text = chargeIntToString(chargeFlatPrice);
                txtCurValleyPrice.Text = chargeIntToString(chargeValleyPrice);

                txtCurPointCost.Text = chargeIntToString(chargePointCost);
                txtCurPeakCost.Text = chargeIntToString(chargePeakCost);
                txtCurFlatCost.Text = chargeIntToString(chargeFlatCost);
                txtCurValleyCost.Text = chargeIntToString(chargeValleyCost);


            } else {

                chargeTotalElect = Convert.ToUInt32(txtCurTotalElect.Text);
                chargeTotalPrice = Convert.ToUInt32(txtCurTotalCost.Text);

                chargePointElect = Convert.ToUInt32(txtCurPointElect.Text);
                chargePeakElect = Convert.ToUInt32(txtCurPeakElect.Text);
                chargeFlatElect = Convert.ToUInt32(txtCurFlatElect.Text);
                chargeValleyElect = Convert.ToUInt32(txtCurValleyElect.Text);

                chargePointPrice = Convert.ToUInt32(txtCurPointPrice.Text);
                chargePeakPrice = Convert.ToUInt32(txtCurPeakPrice.Text);
                chargeFlatPrice = Convert.ToUInt32(txtCurFlatPrice.Text);
                chargeValleyPrice = Convert.ToUInt32(txtCurValleyPrice.Text);

                chargePointCost = Convert.ToUInt32(txtCurPointCost.Text);
                chargePeakCost = Convert.ToUInt32(txtCurPeakCost.Text);
                chargeFlatCost = Convert.ToUInt32(txtCurFlatCost.Text);
                chargeValleyCost = Convert.ToUInt32(txtCurValleyCost.Text);

            }

            bRequestCmd[14] = (byte)(chargeTotalElect >> 24);               // 充电时的总电量
            bRequestCmd[15] = (byte)(chargeTotalElect >> 16);               // 充电时的总电量
            bRequestCmd[16] = (byte)(chargeTotalElect >> 8);                // 充电时的总电量
            bRequestCmd[17] = (byte)(chargeTotalElect >> 0);                // 充电时的总电量
            
            bRequestCmd[18] = (byte)(chargeTotalPrice >> 24);               // 充电时的总费用
            bRequestCmd[19] = (byte)(chargeTotalPrice >> 16);               // 充电时的总费用
            bRequestCmd[20] = (byte)(chargeTotalPrice >> 8);               // 充电时的总费用
            bRequestCmd[21] = (byte)(chargeTotalPrice >> 0);               // 充电时的总费用
            
            bRequestCmd[22] = (byte)(chargePointElect >> 24);               // 充电尖电量
            bRequestCmd[23] = (byte)(chargePointElect >> 16);               // 充电尖电量
            bRequestCmd[24] = (byte)(chargePointElect >> 8);               // 充电尖电量
            bRequestCmd[25] = (byte)(chargePointElect >> 0);               // 充电尖电量
            
            bRequestCmd[26] = (byte)(chargePeakElect >> 24);               // 充电峰电量
            bRequestCmd[27] = (byte)(chargePeakElect >> 16);               // 充电峰电量
            bRequestCmd[28] = (byte)(chargePeakElect >> 8);               // 充电峰电量
            bRequestCmd[29] = (byte)(chargePeakElect >> 0);               // 充电峰电量
            
            bRequestCmd[30] = (byte)(chargeFlatElect >> 24);               // 充电平电量
            bRequestCmd[31] = (byte)(chargeFlatElect >> 16);               // 充电平电量
            bRequestCmd[32] = (byte)(chargeFlatElect >> 8);               // 充电平电量
            bRequestCmd[33] = (byte)(chargeFlatElect >> 0);               // 充电平电量
            
            bRequestCmd[34] = (byte)(chargeValleyElect >> 24);               // 充电谷电量
            bRequestCmd[35] = (byte)(chargeValleyElect >> 16);               // 充电谷电量
            bRequestCmd[36] = (byte)(chargeValleyElect >> 8);               // 充电谷电量
            bRequestCmd[37] = (byte)(chargeValleyElect >> 0);               // 充电谷电量
            
            bRequestCmd[38] = (byte)(chargePointPrice >> 24);               // 尖电价
            bRequestCmd[39] = (byte)(chargePointPrice >> 16);               // 尖电价
            bRequestCmd[40] = (byte)(chargePointPrice >> 8);               // 尖电价
            bRequestCmd[41] = (byte)(chargePointPrice >> 0);               // 尖电价

            
            bRequestCmd[42] = (byte)(chargePeakPrice >> 24);               // 峰电价
            bRequestCmd[43] = (byte)(chargePeakPrice >> 16);               // 峰电价
            bRequestCmd[44] = (byte)(chargePeakPrice >> 8);               // 峰电价
            bRequestCmd[45] = (byte)(chargePeakPrice >> 0);               // 峰电价

            
            bRequestCmd[46] = (byte)(chargeFlatPrice >> 24);               // 平电价
            bRequestCmd[47] = (byte)(chargeFlatPrice >> 16);               // 平电价
            bRequestCmd[48] = (byte)(chargeFlatPrice >> 8);               // 平电价
            bRequestCmd[49] = (byte)(chargeFlatPrice >> 0);               // 平电价

            
            bRequestCmd[50] = (byte)(chargeValleyPrice >> 24);               // 谷电价
            bRequestCmd[51] = (byte)(chargeValleyPrice >> 16);               // 谷电价
            bRequestCmd[52] = (byte)(chargeValleyPrice >> 8);               // 谷电价
            bRequestCmd[53] = (byte)(chargeValleyPrice >> 0);               // 谷电价

            
            bRequestCmd[54] = (byte)(chargePointCost >> 24);               // 尖时段费用
            bRequestCmd[55] = (byte)(chargePointCost >> 16);               // 尖时段费用
            bRequestCmd[56] = (byte)(chargePointCost >> 8);               // 尖时段费用
            bRequestCmd[57] = (byte)(chargePointCost >> 0);               // 尖时段费用

            
            bRequestCmd[58] = (byte)(chargePeakCost >> 24);               // 峰时段费用
            bRequestCmd[59] = (byte)(chargePeakCost >> 16);               // 峰时段费用
            bRequestCmd[60] = (byte)(chargePeakCost >> 8);               // 峰时段费用
            bRequestCmd[61] = (byte)(chargePeakCost >> 0);               // 峰时段费用

            
            bRequestCmd[62] = (byte)(chargeFlatCost >> 24);               // 平时段费用
            bRequestCmd[63] = (byte)(chargeFlatCost >> 16);               // 平时段费用
            bRequestCmd[64] = (byte)(chargeFlatCost >> 8);               // 平时段费用
            bRequestCmd[65] = (byte)(chargeFlatCost >> 0);               // 平时段费用

            
            bRequestCmd[66] = (byte)(chargeValleyCost >> 24);               // 谷时段费用
            bRequestCmd[67] = (byte)(chargeValleyCost >> 16);               // 谷时段费用
            bRequestCmd[68] = (byte)(chargeValleyCost >> 8);               // 谷时段费用
            bRequestCmd[69] = (byte)(chargeValleyCost >> 0);               // 谷时段费用

            // 帧尾
            bRequestCmd[70] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[71] = 0xed;
            if (btnGetData.Text == "关闭监听") {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        /* 
         * 接收数据处理
         */
        public byte ConvertBCDToInt(byte b) {
            // 高四位  
            byte b1 = (byte)((b >> 4) & 0xF);
            // 低四位  
            byte b2 = (byte)(b & 0xF);

            return (byte)(b1 * 10 + b2);
        } 
        private int  packageParser(byte[] arr, int length)
        {
            UInt16 uwCrcResult = 0;

            if (arr == null) return -1;

            CPDataCheck dataCheck = new CPDataCheck();
            bool addressCheck = false;
            if (txtChargingPileAddress.Text != "") {
                UInt64 temp = Convert.ToUInt64(txtChargingPileAddress.Text);
                addressCheck = dataCheck.AddressCheck(arr,temp);
            } else {
                addressCheck = dataCheck.AddressCheck(arr); 
            }
            if (!addressCheck) return -1;
            bool lenCheck = dataCheck.DataLengthCheck(arr, 10);
            if (!lenCheck) return -1;

            byte bccCheckData = dataCheck.GetBCC_Check(arr,10,arr.Length - 2);
            if (  (bccCheckData == arr[length - 2])
               && (0xED == arr[length - 1]) ) {   // 参数校验正确
                Console.WriteLine("data check success!");
                /*
                 * 在这里处理数据 
                 */
                switch (arr[11]) {
                    case 0x11:{
                        respondRequestCmd(arr[11]);
                        
                        break;
                    }
                    case 0x12:{
                        respondRequestCmd(arr[11]);
                        break;
                    }
                    case 0x20:{  // 
                        Console.WriteLine("心跳帧响应");
                        respondRequestCmd(arr[11]);// 
                        // 响应状态00表示执行成功，01表示系统忙暂时不能执行

                        if (cpHeartFrameStateFlg) {
                            heartLedTime.Enabled = true;
                        } else {
                            heartLedTime.Enabled = false;
                            picBox1.Image = Resources.red;
                            heartFrameLed.ForeColor = Color.Red;
                        }

                        
                        //closeheartTime.Enabled = true;
                        break;
                    }
                    case 0x21: {
                        // 解析参数信息，设置时间
                        // 回送响应信息
                        respondRequestCmd(arr[11]);

                        int yearInt = (int)(ConvertBCDToInt(arr[12]) * 100) + (int)ConvertBCDToInt(arr[13]);
                        string year = yearInt.ToString();

                        string month = ConvertBCDToInt(arr[14]).ToString();
                        string day = ConvertBCDToInt(arr[15]).ToString();
                        string hour = ConvertBCDToInt(arr[16]).ToString();
                        string minute = ConvertBCDToInt(arr[17]).ToString();
                        string second = ConvertBCDToInt(arr[18]).ToString();

                        txtChargePileTime.Text = year + "/" + month + "/" + day + " " + hour + ":" + minute + ":" + second;
                        break;
                    }
                    case 0x22:{
                        respondRequestCmd(arr[11]);

                        UInt32 pointPrice = (UInt32)((arr[12] << 24) | (arr[13] << 16)
                                                   | (arr[14] << 8) | (arr[15]));
                        UInt32 peakPrice = (UInt32)((arr[16] << 24) | (arr[17] << 16)
                                                   | (arr[18] << 8) | (arr[19]));
                        UInt32 flatPrice = (UInt32)((arr[20] << 24) | (arr[21] << 16)
                                                   | (arr[22] << 8) | (arr[23]));
                        UInt32 valleyPrice = (UInt32)((arr[24] << 24) | (arr[25] << 16)
                                                   | (arr[26] << 8) | (arr[27]));

                        txtRatePointPrice.Text = pointPrice.ToString();
                        txtRatePeakPrice.Text = peakPrice.ToString();
                        txtRateFlatPrice.Text = flatPrice.ToString();
                        txtRateValleyPrice.Text = valleyPrice.ToString();
                        break;
                    }
                    case 0x23:{ // 获得充电桩状态命令
                        respondCP_StateCmd(arr[11]);
                        ChargeSocTime.Enabled = true;
                        ChargeTimeTimer.Enabled = true;
                        break;
                    }
                    case 0x24:{
                        respondStartAndStopCmd(arr[11],arr[12]);
                        if (bCPStartupResponeState == 0x01) {
                            //txtCPStartup.Text = "charge pile busy";
                            txtCPStartup.Text = "充电桩忙";
                            break;
                        }
                        switch (arr[12]) {
                            case 0x01: {
                                // start charge
                                //txtCPStartup.Text = "start charge";
                                txtCPStartup.Text = "开始充电";
                                break;
                            }
                            case 0x02: { 
                                // pause charge
                                //txtCPStartup.Text = "pause charge";
                                txtCPStartup.Text = "暂停充电";
                                break;
                            }
                            case 0x03: {
                                // stop charge
                                //txtCPStartup.Text = "stop charge";
                                txtCPStartup.Text = "停止充电";
                                break;
                            }
                            case 0x22: {
                                // recover charge
                                //txtCPStartup.Text = "recover charge";
                                txtCPStartup.Text = "恢复充电";
                                break;
                            }
                            default: {
                                 break;
                            }
                        }
                        break;
                    }
                    case 0x25:{
                        respondChargeInfoCmd(arr[11]);
                        break;
                    }
                    case 0x26: {
                        respondRequestCmd(arr[11]);
                        break;
                    }
                    default:{
                        break;
                    }


                }
                /*
                switch (arr[5]) {
                    case 0x01: {   // 数据显示
                        arr[3] = (byte)(arr[3] - 1); // 要显示的主机号
                        DealDisplayMsg(arr, length);
                        break;
                    }
                    case 0x08: {   // 校验正确
                        uiCheckCnt = 0;
                        paraSetForm.bMsgSengFlg = false;  // 设置正确，充许再次设置
                        bQueryMsgFlg = false;             // 查询正确，充许再次查询
                        break;
                    }
                    case 0x09: {   // 校验错误，重新发送数据，发送三次
                        if (true == paraSetForm.bMsgSengFlg) { // 发送了设置指令
                            uiCheckCnt++;
                            if (uiCheckCnt < 3) {
                                paraSetForm.setSendData();
                            } else {
                                uiCheckCnt = 0;
                                paraSetForm.bMsgSengFlg = false; // 对目标设置不成功，可进行其他目标的设置 
                                MessageBox.Show("通信错误，请检查硬件连接");
                            }
                        }
                        if (true == bQueryMsgFlg) {
                            uiCheckCnt++;
                            if (uiCheckCnt < 3) {
                                SendQueryMsg();
                            } else {
                                uiCheckCnt = 0;
                                bQueryMsgFlg = false;  // 对目标查询错误，可对其他目标进行查询
                                // 要在界面上显示对应的主机通信错误
                            }
                        }
                        break;
                    }
                    default: {
                        break;
                    }
                }*/
                return 1;
            } else {
                return -1; 
            }
        }
        private void timeDealRevData_Tick(object sender, EventArgs e)   // 处理接收到的数据
        {
            if (true == blDataFlag) {   // 数据处理标志
                blDataFlag = false;
                int count = receiveByteQueue.Count;
                for (int i = 0; i < count; i++ ) {
                    byte[] arr = (byte[])receiveByteQueue.Dequeue();//通过队列收到数据
                    int status = packageParser(arr, arr.Length);
                }
            }
        }
        private void updateFrameTimer_Tick(object sender, EventArgs e) {
            Random ran = new Random();
        } 
        
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e) {
        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView tv = (TreeView)sender;
           // Console.WriteLine("type: {0}",tv.SelectedNode.Index);
            int num = tv.SelectedNode.Index + 1;
        }
        
        private void zedGraphControl1_Load(object sender, EventArgs e) 
        {

        }
        /// <summary>
        /// ZedGraph动态曲线初始化函数1，定时器此时默认使能状态，调试用
        /// </summary>
        private void ZedGraphTest()
        {
            double y = 5;
            double k = 5;
            for (int i = 0; i <= 1000; i++) {
                double x = (double)new XDate(DateTime.Now.AddSeconds(-(1000 - i)));
                // double y = ran.NextDouble();
                curAList.Add(x, (y++)%10);
                curBList.Add(x, (k++)%15); 
            }
            DateTime dt = DateTime.Now;
        }

        private void TimSysTime_Tick(object sender, EventArgs e) {
            DateTime dt = DateTime.Now;
            TS_LableSystemTime.Text = dt.ToString();
        }
        static bool cpHeartFrameStateFlg = true;
        private void btnPileNormal_Click(object sender, EventArgs e) {
            if (btnPileNormal.Text == "充电桩正常") {
                btnPileNormal.Text = "充电桩繁忙";
                bHeartFrameResponeState = 0x01;
                cpHeartFrameStateFlg = false;
                return;
            }
            if (btnPileNormal.Text == "充电桩繁忙") {
                btnPileNormal.Text = "充电桩正常";
                bHeartFrameResponeState = 0x00;
                cpHeartFrameStateFlg = true;
            }
            return;
        }

        private void btnSetTime_Click(object sender, EventArgs e) {
            if (btnSetTime.Text == "充电桩正常") {
                btnSetTime.Text = "充电桩繁忙";
                bSetTimeResponeState = 0x01;
                return;
            }
            if (btnSetTime.Text == "充电桩繁忙") {
                btnSetTime.Text = "充电桩正常";
                bSetTimeResponeState = 0x00;
            }
            return;
        }

        private void btnSetRate_Click(object sender, EventArgs e) {
            if (btnSetRate.Text == "充电桩正常") {
                btnSetRate.Text = "充电桩繁忙";
                bSetRateResponeState = 0x01;
                return;
            }
            if (btnSetRate.Text == "充电桩繁忙") {
                btnSetRate.Text = "充电桩正常";
                bSetRateResponeState = 0x00;
            }
            return;
        }

        private void btnCPState_Click(object sender, EventArgs e) {
            if (btnCPState.Text == "充电桩正常") {
                btnCPState.Text = "充电桩繁忙";
                bGetCPStateResponeState = 0x01;
                return;
            }
            if (btnCPState.Text == "充电桩繁忙") {
                btnCPState.Text = "充电桩正常";
                bGetCPStateResponeState = 0x00;
            }
            return;
        }

        private void btnStartup_Click(object sender, EventArgs e) {
            if (btnStartup.Text == "充电桩正常") {
                btnStartup.Text = "充电桩繁忙";
                bCPStartupResponeState = 0x01;
                return;
            }
            if (btnStartup.Text == "充电桩繁忙") {
                btnStartup.Text = "充电桩正常";
                bCPStartupResponeState = 0x00;
            }
            return;
        }

        private void btnCurInfo_Click(object sender, EventArgs e) {
            if (btnCurInfo.Text == "充电桩正常") {
                btnCurInfo.Text = "充电桩繁忙";
                bCurInfoResponeState = 0x01;
                return;
            }
            if (btnCurInfo.Text == "充电桩繁忙") {
                btnCurInfo.Text = "充电桩正常";
                bCurInfoResponeState = 0x00;
            }
            return;
        }

        private void 串口设置ToolStripMenuItem_Click(object sender, EventArgs e) {
            SetComPara frmSetComParaDisply = new SetComPara();          //定义串口参数设置对象
            frmSetComParaDisply.Show();
        }

        private void TSBtnExist_Click(object sender, EventArgs e) {
            System.Windows.Forms.Application.Exit();
        }

        private void btnSetData_Click(object sender, EventArgs e) {
            if (btnSetData.Text == "设置数据") {
                btnSetData.Text = "随机数据";

                if (txtValtage.Text == "") {
                    MessageBox.Show("电压数据不能为空请重新设置！");
                    btnSetData.Text = "设置数据";
                    return;
                }
                if (txtCurrent.Text == "") {
                    MessageBox.Show("电流数据不能为空请重新设置！");
                    btnSetData.Text = "设置数据";
                    return;
                }
                if (txtTotalElect.Text == "") {
                    MessageBox.Show("总电量不能为空请重新设置！");
                    btnSetData.Text = "设置数据";
                    return;
                }
                if (txtPointElect.Text == "") {
                    MessageBox.Show("尖电量不能为空请重新设置！");
                    btnSetData.Text = "设置数据";
                    return;
                }
                if (txtPeakElect.Text == "") {
                    MessageBox.Show("峰电量不能为空请重新设置！");
                    btnSetData.Text = "设置数据";
                    return;
                }
                if (txtFlatElect.Text == "") {
                    MessageBox.Show("平电量不能为空请重新设置！");
                    btnSetData.Text = "设置数据";
                    return;
                }
                if (txtValleyElect.Text == "") {
                    MessageBox.Show("谷电量不能为空请重新设置！");
                    btnSetData.Text = "设置数据";
                    return;
                }
                return;
            }
            if (btnSetData.Text == "随机数据") {
                btnSetData.Text = "设置数据";
            }
        }

        private void btnSetCurInfo_Click(object sender, EventArgs e) {
            if (btnSetCurInfo.Text == "设置数据") {
                btnSetCurInfo.Text = "随机数据";

                if (txtCurTotalElect.Text == "") {
                    MessageBox.Show("总电量不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurTotalCost.Text == "") {
                    MessageBox.Show("总费用不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }

                ////////////////////////////电量//////////////////////////////////////
                if (txtCurPointElect.Text == "") {
                    MessageBox.Show("尖电量不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurPeakElect.Text == "") {
                    MessageBox.Show("峰电量不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurFlatElect.Text == "") {
                    MessageBox.Show("平电量不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurValleyElect.Text == "") {
                    MessageBox.Show("谷电量不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }

                ////////////////////////////电价///////////////////////////
                if (txtCurPointPrice.Text == "") {
                    MessageBox.Show("尖电价不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurPeakPrice.Text == "") {
                    MessageBox.Show("峰电价不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurFlatPrice.Text == "") {
                    MessageBox.Show("平电价不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurValleyPrice.Text == "") {
                    MessageBox.Show("谷电价不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                ////////////////////////////费用///////////////////////////
                if (txtCurPointCost.Text == "") {
                    MessageBox.Show("尖费用不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurPeakCost.Text == "") {
                    MessageBox.Show("峰费用不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurFlatCost.Text == "") {
                    MessageBox.Show("平费用不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                if (txtCurValleyCost.Text == "") {
                    MessageBox.Show("谷费用不能为空请重新设置！");
                    btnSetCurInfo.Text = "设置数据";
                    return;
                }
                return;
            }
            if (btnSetCurInfo.Text == "随机数据") {
                btnSetCurInfo.Text = "设置数据";
            }
        }
        static bool ledStateFlg = true;
        private void heartLedTime_Tick(object sender, EventArgs e) {
            if (ledStateFlg) {
                heartFrameLed.ForeColor = Color.Green;
                picBox1.Image = Resources.green32;
                ledStateFlg = false;
            } else {
                heartFrameLed.ForeColor = Color.Gray;
                picBox1.Image = Resources.grey32;
                ledStateFlg = true;
            }
        }

        private void closeheartTime_Tick(object sender, EventArgs e) {
            //heartLedTime.Enabled = false;
            //picBox1.Image = Resources.red;
        }

        private void ChargeSocTime_Tick(object sender, EventArgs e) {
            chargeSOC++;
            if (chargeSOC > 100) {
                chargeSOC = 0;
            }
        }

        private void ChargeTimeTimer_Tick(object sender, EventArgs e) {
            chargeTime++;
            chargeSurplusTime--;
            if (chargeTime > 58) {
                chargeTime = 0;
            }
            if (chargeSurplusTime <= 0) {
                chargeSurplusTime = 58;
            }
        }

        private void tsmAddCP_Click(object sender, EventArgs e) {
            SetChargePile frmOnOffSet = new SetChargePile();          //定义串口参数设置对象
            frmOnOffSet.Show();
        }

    }
}
