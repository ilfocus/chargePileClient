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
// 网络和多线程
using System.Net;
using System.Net.Sockets;
using System.Threading;


using ChargingPile.Properties;
using ChargingPile.Class;

using ZedGraph;

namespace ChargingPile.WinForm
{
    
    public partial class MonitoringInterface : Form {
        #region 变量定义
        private const int NUM_OF_MAIN_DEVICE = 20;
        private const int NUM_OF_SUBDEVICE   = 6;

        Queue receiveByteQueue = Queue.Synchronized(new Queue());//线程安全的数据队列，用来中转串口来的数据

        PointPairList curAList = new PointPairList();
        PointPairList curBList = new PointPairList();

        LineItem curACurve;
        LineItem curBCurve;

        private Object thisLock = new Object();                 // 资源锁
        private Object stateLock = new Object();                // 状态显示资源锁

        private bool blDataFlag = false;                        // 数据接收完成标志

        SetComPara frmSetComPara = new SetComPara();            // 定义串口参数设置对象

        //SetChargePile frmOnOffSet = new SetChargePile();      // 定义开关机设置对象

        private byte[] byteArray = new byte[62];                // 因为串口接收数据会出现不连续
        private bool bQueryMsgFlg = false;                      // 查询数据数据包发送标志
        
        public MonitoringInterface()
        {
            InitializeComponent();

            skinEngine2.SkinFile = System.Windows.Forms.Application.StartupPath.Replace(@"\bin\Debug", "") + @"\Resources\GlassGreen.ssk";
            skinEngine2.SkinAllForm = true;
        }

        static bool ledStateFlg = true;

        static Socket clientSocket;
        private static byte[] result = new byte[1024];
        private bool receiveDataThreadFlg = false;

        static int count = 0;
        static int count2 = 0;

        private byte bHeartFrameResponeState = 0x00;
        private byte bSetTimeResponeState = 0x00;
        private byte bSetRateResponeState = 0x00;
        private byte bGetCPStateResponeState = 0x00;
        private byte bCPStartupResponeState = 0x00;
        private byte bCurInfoResponeState = 0x00;
        private byte chargeSOC = 38;
        private byte chargeTime = 30;
        private byte chargeSurplusTime = 40;
        private int[] iDataCnt = new int[100];
        private List<portRecod> iPortStore = new List<portRecod>();
        string strDisplay = "";

        private bool rtbDisplayFlg = true;
        #endregion
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
        #region form加载
        private void Form1_Load(object sender, EventArgs e)
        {
            //加载皮肤 
            skinEngine2.SkinFile = System.Windows.Forms.Application.StartupPath.Replace(@"\bin\Debug", "") + @"\skins\SteelBlue.ssk";
            skinEngine2.SkinAllForm = true;
            ////////////////////////串口端口初始化////////////////////////////////////
            try {
                string[] szPorts = SerialPort.GetPortNames(); //获取当前可用的串口列表
                //这里得到的可用的串口，是指电脑里面可用的串口。
                frmSetComPara.combSerialPort.Items.AddRange(szPorts);
                frmSetComPara.combSerialPort.SelectedIndex = 0;
            } catch (Win32Exception win32ex) // 获取串口出错
            {
                MessageBox.Show(win32ex.ToString());
            }
            ////////////////////////////////////////////////////////////////////////
           // //zedGraph 初始化
           // zedGraphControl1.GraphPane.Title.Text = "充电桩实时曲线";
           // zedGraphControl1.GraphPane.XAxis.Title.Text = "时间";
           // zedGraphControl1.GraphPane.YAxis.Title.Text = "电流";
           // zedGraphControl1.GraphPane.XAxis.Type = ZedGraph.AxisType.DateAsOrdinal;
           // zedGraphControl1.GraphPane.XAxis.Scale.MaxAuto = true;
           // curACurve = zedGraphControl1.GraphPane.AddCurve("电流A路", curAList, Color.DarkGreen, SymbolType.None);
           // curBCurve = zedGraphControl1.GraphPane.AddCurve("电流A路", curBList, Color.DarkRed, SymbolType.None);
           //// zedGraphControl1.
           // this.zedGraphControl1.AxisChange();
           // this.zedGraphControl1.Refresh();
           // ZedGraphTest();
            TimSysTime.Enabled = true;
            
            //heartLedTime.Enabled = false;
            // 设置winForm中的默认值
            cbEmergencyBtn.SelectedIndex = 0;
            cbMeterState.SelectedIndex = 0;
            cbChargePlug.SelectedIndex = 0;
            cbOutState.SelectedIndex = 0;
            //cbOpen.SelectedIndex = 1;
            cbCurState.SelectedIndex = 0;
            picBox1.Image = Resources.grey32;
            // 加入这行,容许跨线程访问控件
            Control.CheckForIllegalCrossThreadCalls = false;

        }
        #endregion
        #region 串口
        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] byteArray = new byte[serialPort1.ReadBufferSize]; // 创建串口接收数据数组
            int len = serialPort1.Read(byteArray, 0, byteArray.Length);
            if (  0xff == byteArray[0] && 0x5a == byteArray[1]) { //快速初步校验
               
                byte[] ubReceDataBuff = new byte[len];
                for (int i = 0; i < len; i++) {
                    ubReceDataBuff[i] = byteArray[i];
                }   // 把一帧数据暂存在ubReceiveDataBuffer数组中
                receiveByteQueue.Enqueue(ubReceDataBuff);//把数据放入队列中，先进先出
                blDataFlag = true;
            }
        }

        private void serialPortInit() {
            serialPort1.PortName = "COM1";
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
        }

        private void btnOpenPort_Click(object sender, EventArgs e) {
            if (btnOpenPort.Text == "打开串口") {
                this.btnOpenPort.Text = "关闭串口";
                serialPortInit();
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
        #endregion
        #region toolStrip
        private void TS_BtnSetComPara_Click(object sender, EventArgs e)
        {
            SetComPara frmSetComParaDisply = new SetComPara();          //定义串口参数设置对象
            frmSetComParaDisply.Show();
        }
        private void TS_BtnExit_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        #endregion
        #region socket通信
        private void clientReceiveData(object clientSocket) {
            Socket myClientSocket = (Socket)clientSocket;
            int port = ((System.Net.IPEndPoint)myClientSocket.LocalEndPoint).Port;
            Console.WriteLine("创建接收线程成功，端口号：" + port);
            while (receiveDataThreadFlg) {
                try {
                    // 通过clientSocket接收数据
                    Console.WriteLine("---------begin------------------" + port);
                    count2++;
                    int receiveNumber = myClientSocket.Receive(result);

                    count++;
                    byte[] tempArray = new byte[receiveNumber];
                    
                    for (int i = 0; i < receiveNumber; i++) {
                        tempArray[i] = result[i];
                    }

                    UInt32 addressH = (UInt32)((tempArray[2] << 24) | (tempArray[3] << 16)
                                    | (tempArray[4] << 8) | (tempArray[5]));
                    UInt32 addressL = (UInt32)((tempArray[6] << 24) | (tempArray[7] << 16)
                                        | (tempArray[8] << 8) | (tempArray[9]));
                    UInt64 cpAddress = ((UInt64)addressH) << 32 | addressL;
                    Console.WriteLine("接收到了数据---" + cpAddress + "端口号：" + port);

                    //lock (thisLock) {

                    try {
                        packageParser(tempArray, receiveNumber, myClientSocket);
                    } catch (Exception ex) {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine("packageParser  解析数据出错！！！");
                    }
                    //}
                   // Console.WriteLine("接收客户端{0}消息{1}", myClientSocket.RemoteEndPoint.ToString(), Encoding.ASCII.GetString(result, 0, receiveNumber));
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                    myClientSocket.Shutdown(SocketShutdown.Both);
                    myClientSocket.Close();
                    break;
                }
                Thread.Sleep(200);
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
            // 设定服务器IP地址
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                clientSocket.Connect(new IPEndPoint(ip, 8885)); //配置服务器IP与端口
                picBox1.Image = Resources.green32;
            } catch {
                picBox1.Image = Resources.red;
                return;
            }
            // 通过clientSocket接收数据

            // 连接之后，发送心跳包到服务器
            sendBaseDataSocket(0x20,clientSocket);

            heartLedTime.Enabled = true;
            closeheartTime.Enabled = true;

            Thread myThread = new Thread(clientReceiveData);
            myThread.IsBackground = true;
            myThread.Start(clientSocket);
        }
        #endregion
        #region 通信响应
        private int judgeByteIsZero(byte data) {
            return (bool)(data == 0) ? 0 : 1;
        }
        private byte judgeStringIsEqual(string str1, string str2) {
            return (byte)((str1 == str2) ? 0x00 : 0x01);
        }
        private bool checkBoxFault(UInt16 data, int bit) {
            return !((data & (1 << bit)) == 0);
        }
        private UInt16 boolleftShiftToUint16(bool blData, int bit) {
            return (UInt16)(boolToUint16(blData) << bit);
        }
        private UInt16 boolToUint16(bool bl) {
            return (UInt16)(bl ? 1 : 0);
        }
        private string chargeIntToString(UInt32 data) {
            return ((data / 100).ToString() + "." + String.Format("{0:00}", (data % 100)));
        }
        private bool addPortToArray(int port) {
            for (int i = 0; i < iPortStore.Count; i++) {
                if (port == iPortStore[i].port) return true; 
            }
            return false;
        }
        private string byteArrayToString(byte[] arr) {
            StringBuilder strB = new StringBuilder();
            for (int i = 0; i < arr.Length; i++) {
                strB.Append(arr[i].ToString("X2") + " ");
            }
            return strB.ToString();
        }
        private void respondRequestCmd(byte cmdCode)
        {
            const int QUERY_MSG_NUM = 16;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;
            if (txtChargingPileAddress.Text != "") {
                cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            }
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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd,QUERY_MSG_NUM,0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
            
        }
        private void respondRequestCmd(byte cmdCode, Socket clientSocket) {
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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }

        }
        private void respondRequestCmd(byte cmdCode, Socket clientSocket,UInt64 cpAddress) {
            const int QUERY_MSG_NUM = 16;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            //UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;
            //if (txtChargingPileAddress.Text != "") {
            //    cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            //}
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
            if (btnGetData.Text == "关闭连接") {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);

                int port = ((System.Net.IPEndPoint)clientSocket.LocalEndPoint).Port;
                string str = "";
                string str1 = string.Empty;
                str1 += byteArrayToString(bRequestCmd);


                portRecod temp = new portRecod();
                temp.port = port;
                temp.address = cpAddress;
                temp.arrString = str1;

                if (false == addPortAddress(cpAddress, iPortStore)) {
                    iPortStore.Add(temp);
                }
                Console.WriteLine("---发送数据成功：---" + port);
                strDisplay = str;

                if (rtbDisplayFlg) {
                    rtbDisplay.BeginInvoke(new Action(() => {
                        for (int i = 0; i < iPortStore.Count; i++) {
                            if (iPortStore[i].address == Convert.ToUInt64(txtChargingPileAddress.Text)
                                && iPortStore[i].port == port) {
                                rtbDisplay.SelectedText += string.Format("{0:0000}", iPortStore[i].address)
                                                                                    + "-Send:"
                                                                                    + str1
                                                                                    + "\r\n";
                                rtbDisplay.SelectionColor = Color.Green;
                                rtbDisplay.ScrollToCaret();
                            }
                        }
                    }));
                }
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        
        private void respondCP_StateCmd(byte cmdCode)
        {
            const int QUERY_MSG_NUM = 51;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;
            if (txtChargingPileAddress.Text != "") {
                cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            }
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
                //ThreadCPState(chargeIntToString(valtage));
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
                    cbOutState.SelectedIndex = 0;
                } else {
                    cbOutState.SelectedIndex = 1;
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

                if (cbOutState.Text == "有输出") {
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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        private void respondCP_StateCmd(byte cmdCode, Socket clientSocket) {
            const int QUERY_MSG_NUM = 55;
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
            bRequestCmd[9] = (byte)(cpAddress);
            //
            bRequestCmd[10] = 0x2c;               // 帧长度
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
            byte cpOutState = 0;
            UInt16 faultState = 0;
            byte faultStateH = 0;
            byte faultStateL = 0;
            byte currentState = 0;
            

            if (btnSetData.Text == "设置数据") {
                Random ran = new Random();
                valtage = (UInt32)ran.Next(18000, 26000);// 180 - 220V
                current = (UInt32)ran.Next(100, 10000);  // 1A - 100A
                // 充电时的尖电量  
                chargePointElect = (UInt32)ran.Next(0, 2500);
                // 充电时的峰电量
                chargePeakElect = (UInt32)ran.Next(0, 2500);
                // 充电时的平电量
                chargeFlatElect = (UInt32)ran.Next(0, 2500);
                // 充电时的谷电量
                chargeValleyElect = (UInt32)ran.Next(0, 2500);
                // 充电时的总电量
                chargeTotalElect = chargePointElect + chargePeakElect + chargeFlatElect + chargeValleyElect;

                emergencyStopButton = (byte)ran.Next(0, 2);
                electMeter = (byte)ran.Next(0, 2); ;
                chargePlug = (byte)ran.Next(0, 2); ;
                cpOutState = (byte)ran.Next(0, 2);

                faultStateH = (byte)ran.Next(0,4);
                faultStateL = (byte)ran.Next(0,256);
                currentState = (byte)ran.Next(0,6);
                faultState = (UInt16)(((UInt16)faultStateH) << 8 | faultStateL);


                int port = ((System.Net.IPEndPoint)clientSocket.LocalEndPoint).Port;

                Console.WriteLine("voltage:" + port + "--value:" + valtage);
                Console.WriteLine("current:" + port + "--value:" + current);

                txtValtage.BeginInvoke(new Action(() => {
                    txtValtage.Text = chargeIntToString(valtage);
                }));
                txtCurrent.Text = chargeIntToString(current);
                txtTotalElect.Text = chargeIntToString(chargeTotalElect);
                txtPointElect.Text = chargeIntToString(chargePointElect);
                txtPeakElect.Text = chargeIntToString(chargePeakElect);
                txtFlatElect.Text = chargeIntToString(chargeFlatElect);
                txtValleyElect.Text = chargeIntToString(chargeValleyElect);
                //(emergencyStopButton == 0) ? (cbEmergencyBtn.SelectedIndex = 0):(cbEmergencyBtn.SelectedIndex = 1);
                cbEmergencyBtn.SelectedIndex = judgeByteIsZero(emergencyStopButton);
                cbMeterState.SelectedIndex = judgeByteIsZero(electMeter);
                cbChargePlug.SelectedIndex = judgeByteIsZero(chargePlug);
                cbOutState.SelectedIndex = judgeByteIsZero(currentState);
                
                cbInOverVol.Checked = checkBoxFault(faultState,9);
                cbOutOverVol.Checked = checkBoxFault(faultState,8);
                cbInUnderVol.Checked = checkBoxFault(faultState, 7);
                cbOutUnderVol.Checked = checkBoxFault(faultState, 6);
                cbInOverCur.Checked = checkBoxFault(faultState, 5);
                cbOutOverCur.Checked = checkBoxFault(faultState, 4);
                cbInUnderCur.Checked = checkBoxFault(faultState, 3);
                cbOutUnderCur.Checked = checkBoxFault(faultState, 2);
                cbTempHigh.Checked = checkBoxFault(faultState, 1);
                cbOutShort.Checked = checkBoxFault(faultState, 0);
            } else {
                valtage = Convert.ToUInt32(txtValtage.Text);
                current = Convert.ToUInt32(txtCurrent.Text);
                chargePointElect = Convert.ToUInt32(txtPointElect.Text);
                chargePeakElect = Convert.ToUInt32(txtPeakElect.Text);
                chargeFlatElect = Convert.ToUInt32(txtFlatElect.Text);
                chargeValleyElect = Convert.ToUInt32(txtValtage.Text);

                emergencyStopButton = judgeStringIsEqual(cbEmergencyBtn.Text,"正常");
                electMeter = judgeStringIsEqual(cbMeterState.Text,"通信正常");
                chargePlug = judgeStringIsEqual(cbChargePlug.Text,"插好");
                currentState = judgeStringIsEqual(cbOutState.Text,"有输出");

                faultState = (UInt16)(boolleftShiftToUint16(cbInOverVol.Checked,9)
                            | boolleftShiftToUint16(cbInOverVol.Checked,8)
                            | boolleftShiftToUint16(cbInOverVol.Checked,7)
                            | boolleftShiftToUint16(cbInOverVol.Checked,6)
                            | boolleftShiftToUint16(cbInOverVol.Checked,5)
                            | boolleftShiftToUint16(cbInOverVol.Checked,4)
                            | boolleftShiftToUint16(cbInOverVol.Checked,3)
                            | boolleftShiftToUint16(cbInOverVol.Checked,2)
                            | boolleftShiftToUint16(cbInOverVol.Checked,1)
                            | boolleftShiftToUint16(cbInOverVol.Checked,0));

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
            // 输出状态
            bRequestCmd[48] = currentState;                                 // 输出状态

            bRequestCmd[49] = 0xaa;                                 // 故障状态位
            bRequestCmd[50] = 0x55;                                 // 故障状态位
            bRequestCmd[51] = 0x00;                                 // 充电桩当前状态
            bRequestCmd[52] = 0x00;                                 // 充电桩通信状态
            // 帧尾
            bRequestCmd[53] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[54] = 0xed;
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);

                int port = ((System.Net.IPEndPoint)clientSocket.LocalEndPoint).Port;

                portRecod temp = new portRecod();
                temp.port = port;
                temp.address = cpAddress;
                string str = "";
                string str1 = string.Empty;
                for (int i = 0; i < bRequestCmd.Length; i++) {
                    str1 += Convert.ToString(bRequestCmd[i], 16) + " ";
                }
                temp.arrString = str1;

                if (false == addPortAddress(cpAddress,iPortStore)) {
                    iPortStore.Add(temp);
                }

                //for (int i = 0; i < iPortStore.Count; i++) {
                //    str += "chargePileAddress:" 
                //            + iPortStore[i].address.ToString() 
                //            + "Send:"
                //            + iPortStore[i].arrString
                //            + "\r\n";
                    
                //}
                Console.WriteLine("---发送数据成功：---" + port);

                strDisplay = str;

                //tbDataTest.BeginInvoke(new Action(() => {
                //    //tbDataTest.ForeColor = Color.Green;
                //    for (int i = 0; i < iPortStore.Count; i++) {
                //        tbDataTest.Text += string.Format("{0:0000}", iPortStore[i].address)
                //                        + "-Send:"
                //                        + iPortStore[i].arrString
                //                        + "\r\n";
                //    }
                //}));
                rtbDisplay.BeginInvoke(new Action(() => {
                    for (int i = 0; i < iPortStore.Count; i++) {
                        rtbDisplay.SelectedText += string.Format("{0:0000}", iPortStore[i].address)
                                        + "-Send:"
                                        + iPortStore[i].arrString
                                        + "\r\n";
                        rtbDisplay.SelectionColor = Color.Green;
                        rtbDisplay.ScrollToCaret();
                    }

                }));


            }
            
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        private void respondCP_StateCmd(byte cmdCode, Socket clientSocket, UInt64 cpAddress) {
            const int QUERY_MSG_NUM = 55;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            //UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;
            //if (txtChargingPileAddress.Text != "") {
            //    cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            //}
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
            bRequestCmd[9] = (byte)(cpAddress);
            //
            bRequestCmd[10] = 0x2c;               // 帧长度
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
            byte cpOutState = 0;
            UInt16 faultState = 0;
            byte faultStateH = 0;
            byte faultStateL = 0;
            byte currentState = 0;


            if (btnSetData.Text == "设置数据") {
                Random ran = new Random();
                valtage = (UInt32)ran.Next(18000, 75000);// 180 - 220V
                current = (UInt32)ran.Next(100, 10000);  // 1A - 100A
                // 充电时的尖电量  
                chargePointElect = (UInt32)ran.Next(0, 2500);
                // 充电时的峰电量
                chargePeakElect = (UInt32)ran.Next(0, 2500);
                // 充电时的平电量
                chargeFlatElect = (UInt32)ran.Next(0, 2500);
                // 充电时的谷电量
                chargeValleyElect = (UInt32)ran.Next(0, 2500);
                // 充电时的总电量
                chargeTotalElect = chargePointElect + chargePeakElect + chargeFlatElect + chargeValleyElect;

                emergencyStopButton = (byte)ran.Next(0, 2);
                electMeter = (byte)ran.Next(0, 2); ;
                chargePlug = (byte)ran.Next(0, 2); ;
                cpOutState = (byte)ran.Next(0, 2);

                faultStateH = (byte)ran.Next(0, 4);
                faultStateL = (byte)ran.Next(0, 256);
                currentState = (byte)ran.Next(0, 6);
                faultState = (UInt16)(((UInt16)faultStateH) << 8 | faultStateL);


                int port = ((System.Net.IPEndPoint)clientSocket.LocalEndPoint).Port;

                Console.WriteLine("voltage:" + port + "--value:" + valtage);
                Console.WriteLine("current:" + port + "--value:" + current);




                for (int i = 0; i < portAddress.Count; i++) {
                    if (portAddress[i].address == Convert.ToUInt64(txtChargingPileAddress.Text)
                        && portAddress[i].port == port) {
                        
                        txtValtage.BeginInvoke(new Action(() => {
                            txtValtage.Text = chargeIntToString(valtage);
                        }));
                        txtCurrent.BeginInvoke(new Action(() => {
                            txtCurrent.Text = chargeIntToString(current);
                        }));

                        txtTotalElect.BeginInvoke(new Action(() => {
                            txtTotalElect.Text = chargeIntToString(chargeTotalElect);
                        }));
                        txtPointElect.BeginInvoke(new Action(() => {
                            txtPointElect.Text = chargeIntToString(chargePointElect);
                        }));
                        txtPeakElect.BeginInvoke(new Action(() => {
                            txtPeakElect.Text = chargeIntToString(chargePeakElect);
                        }));
                        txtFlatElect.BeginInvoke(new Action(() => {
                            txtFlatElect.Text = chargeIntToString(chargeFlatElect);
                        }));
                        txtValleyElect.BeginInvoke(new Action(() => {
                            txtValleyElect.Text = chargeIntToString(chargeValleyElect);
                        }));


                        cbEmergencyBtn.BeginInvoke(new Action(() => {
                            cbEmergencyBtn.SelectedIndex = judgeByteIsZero(emergencyStopButton);
                        }));
                        cbMeterState.BeginInvoke(new Action(() => {
                            cbMeterState.SelectedIndex = judgeByteIsZero(electMeter);
                        }));
                        cbChargePlug.BeginInvoke(new Action(() => {
                            cbChargePlug.SelectedIndex = judgeByteIsZero(chargePlug);
                        }));
                        cbOutState.BeginInvoke(new Action(() => {
                            cbOutState.SelectedIndex = judgeByteIsZero(cpOutState);
                        }));
                        cbCurState.BeginInvoke(new Action(() => {
                            cbCurState.SelectedIndex = judgeByteIsZero(currentState);
                        }));


                        cbInOverVol.BeginInvoke(new Action(() => {
                            cbInOverVol.Checked = checkBoxFault(faultState, 9);
                        }));
                        cbOutOverVol.BeginInvoke(new Action(() => {
                            cbOutOverVol.Checked = checkBoxFault(faultState, 8);
                        }));
                        cbInUnderVol.BeginInvoke(new Action(() => {
                            cbInUnderVol.Checked = checkBoxFault(faultState, 7);
                        }));
                        cbOutUnderVol.BeginInvoke(new Action(() => {
                            cbOutUnderVol.Checked = checkBoxFault(faultState, 6);
                        }));
                        cbInOverCur.BeginInvoke(new Action(() => {
                            cbInOverCur.Checked = checkBoxFault(faultState, 5);
                        }));
                        cbOutOverCur.BeginInvoke(new Action(() => {
                            cbOutOverCur.Checked = checkBoxFault(faultState, 4);
                        }));
                        cbInUnderCur.BeginInvoke(new Action(() => {
                            cbInUnderCur.Checked = checkBoxFault(faultState, 3);
                        }));
                        cbOutUnderCur.BeginInvoke(new Action(() => {
                            cbOutUnderCur.Checked = checkBoxFault(faultState, 2);
                        }));
                        cbTempHigh.BeginInvoke(new Action(() => {
                            cbTempHigh.Checked = checkBoxFault(faultState, 1);
                        }));
                        cbOutShort.BeginInvoke(new Action(() => {
                            cbOutShort.Checked = checkBoxFault(faultState, 0);
                        }));
//                         cbEmergencyBtn.SelectedIndex = judgeByteIsZero(emergencyStopButton);
//                         cbMeterState.SelectedIndex = judgeByteIsZero(electMeter);
//                         cbChargePlug.SelectedIndex = judgeByteIsZero(chargePlug);
//                         cbOutState.SelectedIndex = judgeByteIsZero(cpOutState);
//                         cbCurState.SelectedIndex = judgeByteIsZero(currentState);
// 
//                         cbInOverVol.Checked = checkBoxFault(faultState, 9);
//                         cbOutOverVol.Checked = checkBoxFault(faultState, 8);
//                         cbInUnderVol.Checked = checkBoxFault(faultState, 7);
//                         cbOutUnderVol.Checked = checkBoxFault(faultState, 6);
//                         cbInOverCur.Checked = checkBoxFault(faultState, 5);
//                         cbOutOverCur.Checked = checkBoxFault(faultState, 4);
//                         cbInUnderCur.Checked = checkBoxFault(faultState, 3);
//                         cbOutUnderCur.Checked = checkBoxFault(faultState, 2);
//                         cbTempHigh.Checked = checkBoxFault(faultState, 1);
//                         cbOutShort.Checked = checkBoxFault(faultState, 0);
                        
                    }
                }
                
            } else {

                try {
                    valtage = (UInt32)(Convert.ToDouble(txtValtage.Text) * 100);
                    current = (UInt32)(Convert.ToDouble(txtCurrent.Text) * 100);
                    chargePointElect = (UInt32)(Convert.ToDouble(txtPointElect.Text) * 100);
                    chargePeakElect = (UInt32)(Convert.ToDouble(txtPeakElect.Text) * 100);
                    chargeFlatElect = (UInt32)(Convert.ToDouble(txtFlatElect.Text) * 100);
                    chargeValleyElect = (UInt32)(Convert.ToDouble(txtValtage.Text) * 100);
                    chargeTotalElect = (UInt32)(Convert.ToDouble(txtTotalElect.Text) * 100);
                } catch (Exception ex) {
                    MessageBox.Show("输入数据有误");
                }
                

                emergencyStopButton = judgeStringIsEqual(cbEmergencyBtn.Text, "正常");
                electMeter = judgeStringIsEqual(cbMeterState.Text, "通信正常");
                chargePlug = judgeStringIsEqual(cbChargePlug.Text, "插好");
                cpOutState = judgeStringIsEqual(cbOutState.Text, "有输出");

                currentState = judgeStringIsEqual(cbOutState.Text, "有输出");
                switch (cbOutState.Text) {
                    case "故障状态": { currentState = 0; break; }
                    case "空闲状态": { currentState = 1; break; }
                    case "充电状态": { currentState = 2; break; }
                    case "停车状态": { currentState = 3; break; }
                    case "预约状态": { currentState = 4; break; }
                    case "维护状态": { currentState = 5; break; }
                    default: { currentState = 0; break; }
                }

                faultState = (UInt16)(boolleftShiftToUint16(cbInOverVol.Checked, 9)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 8)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 7)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 6)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 5)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 4)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 3)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 2)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 1)
                            | boolleftShiftToUint16(cbInOverVol.Checked, 0));

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
            bRequestCmd[45] = chargeTime;                                   // 备用---充电时间
            // 充电插头
            bRequestCmd[46] = chargePlug;                                   // 充电插头
            // 备用
            bRequestCmd[47] = chargeSurplusTime;                            // 备用 --- 充电剩余时间
            // 输出状态
            bRequestCmd[48] = cpOutState;                                   // 输出状态

            bRequestCmd[49] = (byte)(faultState >> 8);                      // 故障状态位
            bRequestCmd[50] = (byte)faultState;                             // 故障状态位
            bRequestCmd[51] = currentState;                                 // 充电桩当前状态
            bRequestCmd[52] = 0x00;                                         // 充电桩通信状态
            // 帧尾
            bRequestCmd[53] = dataCheck.GetBCC_Check(bRequestCmd, 10, bRequestCmd.Length - 2); // bcc校验
            bRequestCmd[54] = 0xed;
            if (btnGetData.Text == "关闭连接") {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);

                int port = ((System.Net.IPEndPoint)clientSocket.LocalEndPoint).Port;
                portRecod temp = new portRecod();
                temp.port = port;
                temp.address = cpAddress;
                string str = "";
                string str1 = string.Empty;
                str1 += byteArrayToString(bRequestCmd);
                temp.arrString = str1;
                if (false == addPortAddress(cpAddress, iPortStore)) {
                    iPortStore.Add(temp);
                }
                Console.WriteLine("---发送数据成功：---" + port);
                strDisplay = str;
                if (rtbDisplayFlg) {
                    rtbDisplay.BeginInvoke(new Action(() => {
                        for (int i = 0; i < iPortStore.Count; i++) {
                            if (iPortStore[i].address == Convert.ToUInt64(txtChargingPileAddress.Text)
                                && iPortStore[i].port == port) {
                                rtbDisplay.SelectedText += string.Format("{0:0000}", iPortStore[i].address)
                                                                                    + "-Send:"
                                                                                    + str1
                                                                                    + "\r\n";
                                rtbDisplay.SelectionColor = Color.Green;
                                rtbDisplay.ScrollToCaret();
                            }
                        }
                    }));
                }
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
            if (txtChargingPileAddress.Text != "") {
                cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            }
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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        private void respondStartAndStopCmd(byte cmdCode, byte para, Socket clientSocket) {
            const int QUERY_MSG_NUM = 17;
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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        private void respondStartAndStopCmd(byte cmdCode, byte para, Socket clientSocket,UInt64 cpAddress) {
            const int QUERY_MSG_NUM = 17;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            //UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;
            //if (txtChargingPileAddress.Text != "") {
            //    cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            //}
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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
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
            if (txtChargingPileAddress.Text != "") {
                cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            }
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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        private void respondChargeInfoCmd(byte cmdCode, Socket clientSocket) {
            const int QUERY_MSG_NUM = 72;
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

                chargePointElect = (UInt32)ran.Next(0, 100000);// 0 - 1000
                chargePeakElect = (UInt32)ran.Next(0, 100000);// 0 - 1000
                chargeFlatElect = (UInt32)ran.Next(0, 100000);// 0 - 1000
                chargeValleyElect = (UInt32)ran.Next(0, 100000);// 0 - 1000

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
                chargePointCost = chargePointElect * chargePointPrice;
                chargePeakCost = chargePeakElect * chargePeakPrice;
                chargeFlatCost = chargeFlatElect * chargeFlatPrice;
                chargeValleyCost = chargeValleyElect * chargeValleyPrice;

                chargeTotalPrice = chargePointCost + chargePeakCost + chargeFlatCost + chargeValleyCost;

                //  把随机数据显示在界面中
                txtCurTotalElect.Text = chargeIntToString(chargeTotalElect);
                txtCurTotalCost.Text = chargeIntToString(chargeTotalPrice);

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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);
            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        private void respondChargeInfoCmd(byte cmdCode, Socket clientSocket,UInt64 cpAddress) {
            const int QUERY_MSG_NUM = 72;
            byte[] bRequestCmd = new byte[QUERY_MSG_NUM];     // 设置数组，并进行初始化，保存发送数据数组

            CPDataCheck dataCheck = new CPDataCheck();
            //UInt64 cpAddress = dataCheck.CHARGING_PILE_ADDRESS;
            //if (txtChargingPileAddress.Text != "") {
            //    cpAddress = Convert.ToUInt64(txtChargingPileAddress.Text);
            //}
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

                chargePointElect = (UInt32)ran.Next(0, 2500);// 0 - 1000
                chargePeakElect = (UInt32)ran.Next(0, 2500);// 0 - 1000
                chargeFlatElect = (UInt32)ran.Next(0, 2500);// 0 - 1000
                chargeValleyElect = (UInt32)ran.Next(0, 2500);// 0 - 1000

                chargeTotalElect = chargePointElect + chargePeakElect + chargeFlatElect + chargeValleyElect;

                if (txtRatePointPrice.Text != "") {

                    chargePointPrice = (UInt32)(Convert.ToDouble(txtRatePointPrice.Text) * 100);
                    chargePeakPrice = (UInt32)(Convert.ToDouble(txtRatePeakPrice.Text) * 100);
                    chargeFlatPrice = (UInt32)(Convert.ToDouble(txtRateFlatPrice.Text) * 100);
                    chargeValleyPrice = (UInt32)(Convert.ToDouble(txtRateValleyPrice.Text) * 100);

                } else {
                    chargePointPrice = (UInt32)ran.Next(120, 130);
                    chargePeakPrice = (UInt32)ran.Next(100, 110);
                    chargeFlatPrice = (UInt32)ran.Next(70, 80);
                    chargeValleyPrice = (UInt32)ran.Next(30, 40);
                }
                chargePointCost = chargePointElect * chargePointPrice / 100;
                chargePeakCost = chargePeakElect * chargePeakPrice / 100;
                chargeFlatCost = chargeFlatElect * chargeFlatPrice / 100;
                chargeValleyCost = chargeValleyElect * chargeValleyPrice / 100;

                chargeTotalPrice = chargePointCost + chargePeakCost + chargeFlatCost + chargeValleyCost;
                int port = ((System.Net.IPEndPoint)clientSocket.LocalEndPoint).Port;

                for (int i = 0; i < portAddress.Count; i++) {
                    if (portAddress[i].address == Convert.ToUInt64(txtChargingPileAddress.Text)
                        && portAddress[i].port == port) {

                        txtCurTotalElect.BeginInvoke(new Action(() => {
                            txtCurTotalElect.Text = chargeIntToString(chargeTotalElect);
                        }));
                        txtCurTotalCost.BeginInvoke(new Action(() => {
                            txtCurTotalCost.Text = chargeIntToString(chargeTotalPrice);
                        }));
                        txtCurPointElect.BeginInvoke(new Action(() => {
                            txtCurPointElect.Text = chargeIntToString(chargePointElect);
                        }));
                        txtCurPeakElect.BeginInvoke(new Action(() => {
                            txtCurPeakElect.Text = chargeIntToString(chargePeakElect);
                        }));
                        txtCurFlatElect.BeginInvoke(new Action(() => {
                            txtCurFlatElect.Text = chargeIntToString(chargeFlatElect);
                        }));
                        txtCurValleyElect.BeginInvoke(new Action(() => {
                            txtCurValleyElect.Text = chargeIntToString(chargeValleyElect);
                        }));
                        txtCurPointPrice.BeginInvoke(new Action(() => {
                            txtCurPointPrice.Text = chargeIntToString(chargePointPrice);
                        }));
                        txtCurPeakPrice.BeginInvoke(new Action(() => {
                            txtCurPeakPrice.Text = chargeIntToString(chargePeakPrice);
                        }));
                        txtCurFlatPrice.BeginInvoke(new Action(() => {
                            txtCurFlatPrice.Text = chargeIntToString(chargeFlatPrice);
                        }));
                        txtCurValleyPrice.BeginInvoke(new Action(() => {
                            txtCurValleyPrice.Text = chargeIntToString(chargeValleyPrice);
                        }));
                        //  把随机数据显示在界面中
                        txtCurPointCost.BeginInvoke(new Action(() => {
                            txtCurPointCost.Text = chargeIntToString(chargePointCost);
                        }));
                        txtCurPeakCost.BeginInvoke(new Action(() => {
                            txtCurPeakCost.Text = chargeIntToString(chargePeakCost);
                        }));
                        txtCurFlatCost.BeginInvoke(new Action(() => {
                            txtCurFlatCost.Text = chargeIntToString(chargeFlatCost);
                        }));
                        txtCurValleyCost.BeginInvoke(new Action(() => {
                            txtCurValleyCost.Text = chargeIntToString(chargeValleyCost);
                        }));
                    }
                }

                //  把随机数据显示在界面中
//                 txtCurTotalElect.Text = chargeIntToString(chargeTotalElect);
//                 txtCurTotalCost.Text = chargeIntToString(chargeTotalPrice);
// 
//                 txtCurPointElect.Text = chargeIntToString(chargePointElect);
//                 txtCurPeakElect.Text = chargeIntToString(chargePeakElect);
//                 txtCurFlatElect.Text = chargeIntToString(chargeFlatElect);
//                 txtCurValleyElect.Text = chargeIntToString(chargeValleyElect);
// 
//                 txtCurPointPrice.Text = chargeIntToString(chargePointPrice);
//                 txtCurPeakPrice.Text = chargeIntToString(chargePeakPrice);
//                 txtCurFlatPrice.Text = chargeIntToString(chargeFlatPrice);
//                 txtCurValleyPrice.Text = chargeIntToString(chargeValleyPrice);
// 
//                 txtCurPointCost.Text = chargeIntToString(chargePointCost);
//                 txtCurPeakCost.Text = chargeIntToString(chargePeakCost);
//                 txtCurFlatCost.Text = chargeIntToString(chargeFlatCost);
//                 txtCurValleyCost.Text = chargeIntToString(chargeValleyCost);


            } else {

                try {
                    chargeTotalElect = (UInt32)(Convert.ToDouble(txtCurTotalElect.Text) * 100);
                    chargeTotalPrice = (UInt32)(Convert.ToDouble(txtCurTotalCost.Text) * 100);

                    chargePointElect = (UInt32)(Convert.ToDouble(txtCurPointElect.Text) * 100);
                    chargePeakElect = (UInt32)(Convert.ToDouble(txtCurPeakElect.Text) * 100);
                    chargeFlatElect = (UInt32)(Convert.ToDouble(txtCurFlatElect.Text) * 100);
                    chargeValleyElect = (UInt32)(Convert.ToDouble(txtCurValleyElect.Text) * 100);

                    chargePointPrice = (UInt32)(Convert.ToDouble(txtCurPointPrice.Text) * 100);
                    chargePeakPrice = (UInt32)(Convert.ToDouble(txtCurPeakPrice.Text) * 100);
                    chargeFlatPrice = (UInt32)(Convert.ToDouble(txtCurFlatPrice.Text) * 100);
                    chargeValleyPrice = (UInt32)(Convert.ToDouble(txtCurValleyPrice.Text) * 100);

                    chargePointCost = (UInt32)(Convert.ToDouble(txtCurPointCost.Text) * 100);
                    chargePeakCost = (UInt32)(Convert.ToDouble(txtCurPeakCost.Text) * 100);
                    chargeFlatCost = (UInt32)(Convert.ToDouble(txtCurFlatCost.Text) * 100);
                    chargeValleyCost = (UInt32)(Convert.ToDouble(txtCurValleyCost.Text) * 100);
                } catch (Exception ex) {
                    MessageBox.Show("输入数据在误！");
                }

                

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
            if (btnGetData.Text == "关闭连接") {
            //if (cbOpen.SelectedIndex == 1) {
                clientSocket.Send(bRequestCmd, QUERY_MSG_NUM, 0);

                int port = ((System.Net.IPEndPoint)clientSocket.LocalEndPoint).Port;
                string str = "";
                string str1 = string.Empty;
                str1 += byteArrayToString(bRequestCmd);


                portRecod temp = new portRecod();
                temp.port = port;
                temp.address = cpAddress;
                temp.arrString = str1;

                if (false == addPortAddress(cpAddress, iPortStore)) {
                    iPortStore.Add(temp);
                }
                Console.WriteLine("---发送数据成功：---" + port);
                strDisplay = str;

                if (rtbDisplayFlg) {
                    rtbDisplay.BeginInvoke(new Action(() => {
                        for (int i = 0; i < iPortStore.Count; i++) {
                            if (iPortStore[i].address == Convert.ToUInt64(txtChargingPileAddress.Text)
                                && iPortStore[i].port == port) {
                                rtbDisplay.SelectedText += string.Format("{0:0000}", iPortStore[i].address)
                                                                                    + "-Send:"
                                                                                    + str1
                                                                                    + "\r\n";
                                rtbDisplay.SelectionColor = Color.Green;
                                rtbDisplay.ScrollToCaret();
                            }
                        }
                    }));
                }

            }
            if (true == serialPort1.IsOpen) {
                serialPort1.Write(bRequestCmd, 0, QUERY_MSG_NUM);
            }
        }
        #endregion
        #region 通信数据解析
        public  byte ConvertBCDToInt(byte b) {
            // 高四位  
            byte b1 = (byte)((b >> 4) & 0xF);
            // 低四位  
            byte b2 = (byte)(b & 0xF);

            return (byte)(b1 * 10 + b2);
        }
        private int packageParser(byte[] arr, int length)
        {
            if (arr == null) return -1;

            CPDataCheck dataCheck = new CPDataCheck();
            bool addressCheck = false;
            if (txtChargingPileAddress.Text != "") {
                UInt64 temp = Convert.ToUInt64(txtChargingPileAddress.Text);
                addressCheck = dataCheck.AddressCheck(arr,temp);
                Console.WriteLine("addressCheck with address success!" + temp);
            } else {
                addressCheck = dataCheck.AddressCheck(arr);
                Console.WriteLine("addressCheck without address success!");
            }
            if (!addressCheck) return -1;
            Console.WriteLine("addressCheck success!");
            bool lenCheck = dataCheck.DataLengthCheck(arr, 10);
            if (!lenCheck) return -1;
            Console.WriteLine("lenCheck success!");
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
                        //ThreadData(arr);
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
                return 1;
            } else {
                return -1; 
            }
        }
        private int packageParser(byte[] arr, int length, Socket clientSocket) {
            if (arr == null) return -1;

            CPDataCheck dataCheck = new CPDataCheck();


            //bool addressCheck = false;
            //if (txtChargingPileAddress.Text != "") {
            //    UInt64 temp = Convert.ToUInt64(txtChargingPileAddress.Text);
            //    addressCheck = dataCheck.AddressCheck(arr, temp);
            //} else {
            //    addressCheck = dataCheck.AddressCheck(arr);
            //}
            //if (!addressCheck) return -1;
            bool lenCheck = dataCheck.DataLengthCheck(arr, 10);
            if (!lenCheck) return -1;
            //Console.WriteLine("lenCheck success!");
            byte bccCheckData = dataCheck.GetBCC_Check(arr, 10, arr.Length - 2);

            string str = string.Empty;
            //for (int i = 0; i < arr.Length; i++) {
            //    str += Convert.ToString(arr[i], 16) + " ";
            //}
            str += byteArrayToString(arr);
            UInt32 addressH = (UInt32)((arr[2] << 24) | (arr[3] << 16)
                                    | (arr[4] << 8) | (arr[5]));
            UInt32 addressL = (UInt32)((arr[6] << 24) | (arr[7] << 16)
                                | (arr[8] << 8) | (arr[9]));
            UInt64 cpAddress = ((UInt64)addressH) << 32 | addressL;
            int port = ((System.Net.IPEndPoint)clientSocket.LocalEndPoint).Port;

            portRecod temp1 = new portRecod();
            temp1.port = port;
            temp1.address = cpAddress;
            temp1.arrString = str;

            if (false == addPortAddress(cpAddress)) {
                portAddress.Add(temp1);
            }
            if (rtbDisplayFlg) {
                    rtbDisplay.BeginInvoke(new Action(() => {
                        for (int i = 0; i < portAddress.Count; i++) {
                            if (portAddress[i].address == Convert.ToUInt64(txtChargingPileAddress.Text)
                                && portAddress[i].port == port) {
                                rtbDisplay.SelectedText += string.Format("{0:0000}", portAddress[i].address)
                                                    + "-Receive:"
                                                    + str + "\r\n";
                                rtbDisplay.SelectionColor = Color.Red;
                                rtbDisplay.ScrollToCaret();
                            }
                        }
                    }));
                }
            if ((bccCheckData == arr[length - 2]) && (0xED == arr[length - 1])) {
                
                switch (arr[11]) {
                    case 0x11: {
                        respondRequestCmd(arr[11]);

                        break;
                    }
                    case 0x12: {
                            respondRequestCmd(arr[11]);
                            break;
                        }
                    case 0x20: {  // 
                        Console.WriteLine("心跳帧响应");
                        respondRequestCmd(arr[11], clientSocket,cpAddress);// 
                        // 响应状态00表示执行成功，01表示系统忙暂时不能执行
                        if (cpHeartFrameStateFlg) {
                            heartLedTime.Enabled = true;
                        } else {
                            heartLedTime.Enabled = false;
                            picBox1.Image = Resources.red;
                            heartFrameLed.ForeColor = Color.Red;
                        }
                        heartFrameCnt = 0;
                        break;
                    }
                    case 0x21: {
                        // 解析参数信息，设置时间
                        // 回送响应信息
                        respondRequestCmd(arr[11], clientSocket,cpAddress);
                        int yearInt = (int)(ConvertBCDToInt(arr[12]) * 100) + (int)ConvertBCDToInt(arr[13]);
                        string year = yearInt.ToString();

                        string month = ConvertBCDToInt(arr[14]).ToString();
                        string day = ConvertBCDToInt(arr[15]).ToString();
                        string hour = ConvertBCDToInt(arr[16]).ToString();
                        string minute = ConvertBCDToInt(arr[17]).ToString();
                        string second = ConvertBCDToInt(arr[18]).ToString();

                        txtChargePileTime.BeginInvoke(new Action(() => {
                            txtChargePileTime.Text = year + "/" + month + "/" + day + " " 
                                                    + hour + ":" + minute + ":" + second;
                        }));
                            
                        break;
                    }
                    case 0x22: {
                        respondRequestCmd(arr[11], clientSocket,cpAddress);

                        UInt32 pointPrice = (UInt32)((arr[12] << 24) | (arr[13] << 16)
                                                    | (arr[14] << 8) | (arr[15]));
                        UInt32 peakPrice = (UInt32)((arr[16] << 24) | (arr[17] << 16)
                                                    | (arr[18] << 8) | (arr[19]));
                        UInt32 flatPrice = (UInt32)((arr[20] << 24) | (arr[21] << 16)
                                                    | (arr[22] << 8) | (arr[23]));
                        UInt32 valleyPrice = (UInt32)((arr[24] << 24) | (arr[25] << 16)
                                                    | (arr[26] << 8) | (arr[27]));
                        try {
                            txtRatePointPrice.BeginInvoke(new Action(() => {
                                txtRatePointPrice.Text = chargeIntToString(pointPrice);
                            }));
                            txtRatePeakPrice.BeginInvoke(new Action(() => {
                                txtRatePeakPrice.Text = chargeIntToString(peakPrice);
                            }));
                            txtRateFlatPrice.BeginInvoke(new Action(() => {
                                txtRateFlatPrice.Text = chargeIntToString(flatPrice);
                            }));
                            txtRateValleyPrice.BeginInvoke(new Action(() => {
                                txtRateValleyPrice.Text = chargeIntToString(valleyPrice);
                            }));
                        } catch (Exception ex) {
                            Console.WriteLine(ex.Message);
                            Console.WriteLine("txtRatePointPrice  解析数据出错！！！");
                        }
                            
                        break;
                    }
                    case 0x23: { // 获得充电桩状态命令
                        respondCP_StateCmd(arr[11], clientSocket,cpAddress);
                        ChargeSocTime.Enabled = true;
                        ChargeTimeTimer.Enabled = true;
                        break;
                    }
                    case 0x24: {
                        respondStartAndStopCmd(arr[11], arr[12], clientSocket,cpAddress);
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
                    case 0x25: {
                            respondChargeInfoCmd(arr[11], clientSocket, cpAddress);
                            break;
                        }
                    case 0x26: {
                            respondRequestCmd(arr[11], clientSocket, cpAddress);
                            break;
                        }
                    default: {
                        break;
                    }
                }
                return 1;
            } else {
                return -1;
            }
        }
        private List<portRecod> portAddress = new List<portRecod>();
        private bool addPortAddress(UInt64 address) {
            for (int i = 0; i < portAddress.Count; i++) {
                if (address == portAddress[i].address) return true;
            }
            return false;
        }
        private bool addPortAddress(UInt64 address,List<portRecod> portAdd) {
            for (int i = 0; i < portAdd.Count; i++) {
                if (address == portAdd[i].address) return true;
            }
            return false;
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
        #endregion
        #region 界面按钮点击
        private List<int> nodeIndex = new List<int>();
        private void tvChargePile_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeView tv = (TreeView)sender;
            txtChargingPileAddress.Text = String.Format("{0:0000}", tv.SelectedNode.Index + 1);

            if (false == addNodeIndex(tv.SelectedNode.Index)) {
                if (btnGetData.Text == "关闭连接") {
                //if (cbOpen.SelectedIndex == 1) {
                    socketClient();

                }
                nodeIndex.Add(tv.SelectedNode.Index);
            }
        }
        private bool addNodeIndex(int index) {
            for (int i = 0; i < nodeIndex.Count; i++) {
                if (index == nodeIndex[i]) return true;
            }
            return false;
        }

        private void TimSysTime_Tick(object sender, EventArgs e) {
            DateTime dt = DateTime.Now;
            //TS_LableSystemTime.Text = dt.ToString();
            lblSystemTime.Text = dt.ToString();
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
        
        private void tsmAddCP_Click(object sender, EventArgs e) {
            SetChargePile frmOnOffSet = new SetChargePile(this);          //定义串口参数设置对象
            frmOnOffSet.Show();
        }

//         private void cbOpen_SelectedIndexChanged(object sender, EventArgs e) {
// 
//             if (cbOpen.SelectedIndex == 0) {
//                 Console.WriteLine("打开串口");
//                 // 在此打开串口
//                 serialPortInit();
//                 // 此下部分关闭网络
//                 receiveDataThreadFlg = false;
//             } else if (cbOpen.SelectedIndex == 1) {
//                 Console.WriteLine("初始化---打开网络");
//                 receiveDataThreadFlg = true;//关闭连接
//             } else {
//                 picBox1.Image = Resources.grey32;
//             }
//         }
        private void btnGetData_Click(object sender, EventArgs e)//轮询任务启动
        {
            if (btnGetData.Text == "打开连接") {
                btnGetData.Text = "关闭连接";
                receiveDataThreadFlg = true;
            } else {
                btnGetData.Text = "打开连接";
                picBox1.Image = Resources.grey32;
                heartFrameLed.ForeColor = Color.Gray;
                heartLedTime.Enabled = false;
                receiveDataThreadFlg = false;
            }
        }
        #endregion
        #region 定时器
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
            //heartFrameLed.ForeColor = Color.Green;
        }
        private int heartFrameCnt = 0;
        private void closeheartTime_Tick(object sender, EventArgs e) {
            heartFrameCnt++;
            if (heartFrameCnt > 15) {
                heartLedTime.Enabled = false;
                heartFrameLed.ForeColor = Color.Gray;
                picBox1.Image = Resources.grey32;
                heartFrameCnt = 0;
            }
        }

        private void ChargeSocTime_Tick(object sender, EventArgs e) {
            chargeSOC++;
            if (chargeSOC > 90) {
                chargeSOC = 38;
            }
        }

        private void ChargeTimeTimer_Tick(object sender, EventArgs e) {
            chargeTime++;
            chargeSurplusTime =  (byte)(70 - chargeTime);
            if (chargeTime > 60) {
                chargeTime = 30;
            }
        }
        #endregion

        private void btnClear_Click(object sender, EventArgs e) {
            rtbDisplay.Text = "";
        }
        private void btnPause_Click(object sender, EventArgs e) {
            if (btnPause.Text == "暂停") {
                btnPause.Text = "恢复";
                rtbDisplayFlg = false;
            } else {
                btnPause.Text = "暂停";
                rtbDisplayFlg = true;
            }
        }
    }
}