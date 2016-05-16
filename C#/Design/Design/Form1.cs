using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.IO;
using System.IO.Ports;

using System.Threading;
using System.Net;
using System.Net.Sockets;
using Design;
using KeyBoardHock;

namespace eHealth
{


    public partial class Form1 : Form
    {
        SerialPort com;
        bool isConnect;
        SerialPort rfidCom;
        bool isRFIDconnect;
        
        Thread _readThread;
        bool _keepReading;


        Thread readRFIDThread;
        bool _keepRFIDReading;

        String value = "0";
        String longitude = "0";
        String latitudeX = "0";

        BardCodeHooK BarCode = new BardCodeHooK();
        private System.Timers.Timer timer = new System.Timers.Timer();

        Locker locker1 = new Locker();
        Locker locker2 = new Locker();
        Locker locker3 = new Locker();

        public DateTime Time;    //扫描时间   


        private void debug(String str)
        {
            Console.WriteLine(str);
        }


        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;           
            BarCode.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            com = new SerialPort();
            rfidCom = new SerialPort();
            isConnect = false;
            


            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
  

 
            Console.WriteLine("serial port");

            String[] str = SerialPort.GetPortNames();
            if (str == null)
            {
                MessageBox.Show("no port ", "error");
                return;
            }
            foreach (String s in SerialPort.GetPortNames())
            {
                System.Console.WriteLine(s);
                serialBox.Items.Add(s);
                comboBox5.Items.Add(s);
            }
            if (str.Length != 0)
            {
                serialBox.SelectedIndex = 0;
                comboBox5.SelectedIndex = 0;
            }
            //serialBox.SelectedIndex = 0;
            button1.Text = "未打开";
            //com.ReceivedBytesThreshold = 0;

            /***************************设置所有锁列表为第一个********************************/
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            //locker1.setBardCode(111111111111);
            Console.WriteLine("locker1 barcode = "+locker1.getBardCode());

        }
        /*********系统键盘钩子**********/
        private delegate void ShowInfoDelegate(BardCodeHooK.BarCodes barCode);
        private void ShowInfo(BardCodeHooK.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(ShowInfo), new object[] { barCode });
            }
            else
            {
                Console.WriteLine("bardcodeString = " + BarCode.bardcodeString);
                if (BarCode.bardcodeString == locker1.getBardCode()+"")
                {
                    Console.WriteLine("locker 1 open");
                    textBox1.Text = "1号锁";
                    setLocker1Open();
                }
                else if (BarCode.bardcodeString == locker2.getBardCode() + "")
                {
                    textBox1.Text = "2号锁";
                    Console.WriteLine("locker 2 open");
                    setLocker2Open();
                }
                else if (BarCode.bardcodeString == locker3.getBardCode() + "")
                {
                    textBox1.Text = "3号锁";
                    Console.WriteLine("locker 3 open");
                    setLocker3Open();
                }


                textBox4.Text = BarCode.bardcodeString;
            }
        }

        private void OnDataRecivice(object sender ,SerialDataReceivedEventArgs e) {
            byte[] readBuffer = new byte[com.ReadBufferSize];
            com.Read(readBuffer, 0, readBuffer.Length);
            //this.Invoke(interfaceUpdateHandle, new string[] { Encoding.Unicode.GetString(readBuffer) });
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (isConnect == false)
            {
                try
                {
                    if (serialBox.Text == "" || serialBox.Text == null) return;
                    com.BaudRate = 57600;
                    Console.WriteLine(serialBox.Text);
                    com.PortName = serialBox.Text;
                    com.DataBits = 8;
                    com.Open();
                    isConnect = true;
                    _keepReading = true;
                    _readThread = new Thread(ReadPort);
                    _readThread.Start();
                    button1.Text = "已打开";
                }
                catch (System.IO.IOException)
                {
                    button1.Text = "未打开";
                    isConnect = false;
                    MessageBox.Show("SerialPort Open FAIL", "ERROR");
                }
                catch (System.NullReferenceException)
                {
                    button1.Text = "未打开";
                    isConnect = false;
                    MessageBox.Show("Open SerialPort", "SerialPort FAIL");
                }
                catch (System.UnauthorizedAccessException)
                {
                    button1.Text = "未打开";
                    isConnect = false;
                    MessageBox.Show("串口打不开", "错误");
                }
                catch (System.Threading.ThreadStateException)
                {
                    button1.Text = "未打开";
                    _keepReading = false;
                    com.Close();
                    isConnect = false;
                    MessageBox.Show("不能打开线程", "错误");
                }
            }
            else {
                if (isConnect == true)
                {
                    _keepReading = false;
                    com.Close();
                    isConnect = false;
                    button1.Text = "未打开";
                }
            }
        }

        /*private void button2_Click(object sender, EventArgs e)
        {
            com.WriteLine("1234");
            Console.WriteLine("1234");
        }*/
        private void UpdateTextBox(string text)
        {
            Console.WriteLine(text);
            
        }
        private void ReadPort() {
            while (_keepReading) {

                if (isConnect == true){

                    try
                    {

                        String SerialIn = com.ReadLine();
                        Console.WriteLine(SerialIn);
                        if (SerialIn.IndexOf("L1A") != -1)
                        {
                            label16.BackColor = Color.Red;
                        }
                        else if (SerialIn.IndexOf("L2A") != -1)
                        {
                            label17.BackColor = Color.Red;
                        }
                        else if (SerialIn.IndexOf("L3A") != -1)
                        {
                            label18.BackColor = Color.Red;
                        }
                        else if (SerialIn.IndexOf("L1B") != -1)
                        {
                            label16.BackColor = Color.Green;
                        }
                        else if (SerialIn.IndexOf("L2B") != -1)
                        {
                            label17.BackColor = Color.Green;
                        }
                        else if (SerialIn.IndexOf("L3B") != -1)
                        {
                            label17.BackColor = Color.Green;
                        }

                    }
                    catch (TimeoutException) { }
                    catch (IOException) { }
                }
                else {
                    TimeSpan waitTime = new TimeSpan(0,0,0,50);
                    Thread.Sleep(waitTime);
                }
            } 
        }


        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {


            //模拟的做一些耗时的操作

            System.Threading.Thread.Sleep(1000000);
            System.Threading.Thread.Sleep(1000000);
        }

        void BarCode_BarCodeEvent(BardCodeHooK.BarCodes barCode)
        {
            ShowInfo(barCode);
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
           // BarCode.Start();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
          //  BarCode.Stop();
        }

 
        /*************************************锁密码控制****************************************/
        /*
         设置密码按钮
         */
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "" && textBox2.Text != null)
            {
                int lockerNumber = comboBox1.SelectedIndex;
                debug("锁为" + lockerNumber + "  设置的密码为：" + textBox2.Text);
                switch (lockerNumber)
                {
                    case 0:
                        locker1.setPassword(textBox2.Text);
                        break;
                    case 1:
                        locker2.setPassword(textBox2.Text);
                        break;
                    case 2:
                        locker3.setPassword(textBox2.Text);
                        break;
                }
                MessageBox.Show("锁号为：" + (lockerNumber + 1) + "号锁" + "\n密码为：" + textBox2.Text, "设置锁密码成功");
                textBox2.Text = "";
            }
            else 
            {
                MessageBox.Show("密码不能为空","密码设置错误");
            }
        }

        /*
         读取所有锁的密码
         */
        private void button5_Click(object sender, EventArgs e)
        {
            String p1 = locker1.getPassword();
            String p2 = locker2.getPassword();
            String p3 = locker3.getPassword();

            if (p1 != "" || p1!= null)  p1 = "未设密码";
            if (p2 != "" || p2!= null)  p2 = "未设密码";
            if (p3 != "" || p3 != null) p3 = "未设密码";

            String msg = "1号锁" + "  密码为：" + p1
                + "\n2号锁" + "  密码为：" + p2
                + "\n3号锁" + "  密码为：" + p3;

            MessageBox.Show(msg, "读所有锁");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            locker1.cleanPassword();
            locker2.cleanPassword();
            locker3.cleanPassword();
            MessageBox.Show("所有密码成功清除", "清除锁密码");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            int lockerNumber = comboBox2.SelectedIndex;
            String password = textBox3.Text;
            debug("锁为" + lockerNumber + "  设置的密码为：" + textBox3.Text);
            if (isConnect == false)
            {
                MessageBox.Show("串口未和单片机连接", "错误");
                return;
            }


            if (textBox3.Text != "" || textBox3.Text != null)
            {
                switch (lockerNumber)
                {
                    case 0:
                        if (locker1.getPassword() == null || locker1.getPassword() == "")
                        {
                            MessageBox.Show("锁1没有密码", "错误");
                            return;
                        }
                        if (password == locker1.getPassword())
                        {
                            MessageBox.Show("密码正确", "正确");
                            debug("锁1密码正确");
                            com.WriteLine("UNLOCK1");
                        }
                        else
                        {
                            MessageBox.Show("密码错误", "错误");
                        }
                    break;
                    case 1:
                        if (locker2.getPassword() == null || locker2.getPassword() == "")
                        {
                            MessageBox.Show("锁2没有密码", "错误");
                            return;
                        }
                        if (password == locker1.getPassword())
                        {
                            MessageBox.Show("密码正确", "正确");
                            debug("锁2密码正确");
                            com.WriteLine("UNLOCK2");
                        }
                        else
                        {
                            MessageBox.Show("密码错误", "错误");
                        }
                    break;
                    case 2:
                        if (locker3.getPassword() == null || locker3.getPassword() == "")
                        {
                            MessageBox.Show("锁3没有密码", "错误");
                            return;
                        }
                        if (password == locker1.getPassword())
                        {
                            MessageBox.Show("密码正确", "正确");
                            debug("锁3密码正确");
                            com.WriteLine("UNLOCK3");
                        }
                        else
                        {
                            MessageBox.Show("密码错误", "错误");
                        }
                    break;
                }
            }            
        }

        /*************************************锁条码控制****************************************/
        private void button8_Click(object sender, EventArgs e)
        {
            Random rad = new Random();//实例化随机数产生器rad；
            int value1 = rad.Next(100000, 1000000);//用rad生成大于等于1000，小于等于9999的随机数；
            int value2 = rad.Next(100000, 1000000);//用rad生成大于等于1000，小于等于9999的随机数；
            debug("value1 " + value1);
            debug("value2 " + value2);
            ulong bardcode = (ulong)value1 * 1000000 + (ulong)value2;
            debug("bardcode = " + bardcode);
            
            int lockerNumber = comboBox3.SelectedIndex;
            debug("锁为" + lockerNumber + "  设置的条形码为：" + bardcode);
            switch (lockerNumber)
            {
                case 0:
                    locker1.setBardCode(bardcode);
                    break;
                case 1:
                    locker2.setBardCode(bardcode);
                    break;
                case 2:
                    locker3.setBardCode(bardcode);
                    break;
            }

            BardCodeMessageBox box = new BardCodeMessageBox();
            box.setBardCode(bardcode);
            box.ShowDialog();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ShowAllBardCodeWindows w = new ShowAllBardCodeWindows();
            if (locker1.getBardCode() != 0)
            {                
                w.setLocker1(locker1.getBardCode());                
            }
            if (locker2.getBardCode() != 0)
            {
                w.setLocker2(locker2.getBardCode());                
            }
            if (locker3.getBardCode() != 0)
            {
                w.setLocker3(locker3.getBardCode());
            }
            w.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            locker1.setBardCode(0);
            locker2.setBardCode(0);
            locker3.setBardCode(0);
        }

        private void button11_Click(object sender, EventArgs e)
        {
            if (isRFIDconnect == false)
            {
                try
                {
                    if (comboBox5.Text == "" || comboBox5.Text == null) return;
                    rfidCom.BaudRate = 9600;
                    Console.WriteLine(comboBox5.Text);
                    rfidCom.PortName = comboBox5.Text;
                    rfidCom.DataBits = 8;
                    rfidCom.Open();
                    isRFIDconnect = true;
                    readRFIDThread = new Thread(ReadRFID);      
                    _keepRFIDReading = true;
                    readRFIDThread.Start();
                    button11.Text = "已打开";
                }
                catch (System.IO.IOException)
                {
                    button11.Text = "未打开";
                    isRFIDconnect = false;
                    MessageBox.Show("SerialPort RFID Open FAIL", "ERROR");
                }
                catch (System.NullReferenceException)
                {
                    button11.Text = "未打开";
                    isRFIDconnect = false;
                    MessageBox.Show("Open RFID SerialPort ", "SerialPort FAIL");
                }
                catch (System.UnauthorizedAccessException)
                {
                    button1.Text = "未打开";
                    isRFIDconnect = false;
                    MessageBox.Show("串口打不开", "错误");
                }
            }
            else
            {
                if (isRFIDconnect == true)
                {
                    readRFIDThread.Abort();
                    _keepRFIDReading = false;
                    rfidCom.Close();
                    isRFIDconnect = false;
                    button11.Text = "未打开";
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int lockerNumber = comboBox2.SelectedIndex;
            String password = textBox3.Text;
            debug("锁为" + lockerNumber + "  设置的密码为：" + textBox3.Text);
            if (isConnect == false)
            {
                MessageBox.Show("串口未和单片机连接", "错误");
                return;
            }



            if (textBox3.Text != "" || textBox3.Text != null)
            {
                switch (lockerNumber)
                {
                    case 0:
                        if (locker1.getPassword() == null || locker1.getPassword() == "")
                        {
                            MessageBox.Show("锁1没有密码", "错误");
                            return;
                        }
                        if (password == locker1.getPassword())
                        {
                            MessageBox.Show("密码正确", "正确");
                            debug("锁1密码正确");
                            com.WriteLine("UNLOCK1");
                        }
                        else
                        {
                            MessageBox.Show("密码错误", "错误");
                        }
                    break;
                    case 1:
                        if (locker2.getPassword() == null || locker2.getPassword() == "")
                        {
                            MessageBox.Show("锁2没有密码", "错误");
                            return;
                        }
                        if (password == locker1.getPassword())
                        {
                            MessageBox.Show("密码正确", "正确");
                            debug("锁2密码正确");
                            com.WriteLine("UNLOCK2");
                        }
                        else
                        {
                            MessageBox.Show("密码错误", "错误");
                        }
                    break;
                    case 2:
                        if (locker3.getPassword() == null || locker3.getPassword() == "")
                        {
                            MessageBox.Show("锁3没有密码", "错误");
                            return;
                        }
                        if (password == locker1.getPassword())
                        {
                            MessageBox.Show("密码正确", "正确");
                            debug("锁3密码正确");
                            com.WriteLine("UNLOCK3");
                        }
                        else
                        {
                            MessageBox.Show("密码错误", "错误");
                        }
                    break;
                }
            }        
        }
        //
        private void Form1_Load(object sender, EventArgs e)
        {
            System.Console.WriteLine("Start");
            BarCode.Start();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Console.WriteLine("Stop");
            BarCode.Stop();

            if (isConnect == true)
            {
                _keepReading = false;
                com.Close();
                isConnect = false;
            }
            if (readRFIDThread != null)
            {
                if (readRFIDThread.ThreadState == ThreadState.Running)
                    readRFIDThread.Abort();      
            }
      

            if (isRFIDconnect == true)
            {
                _keepRFIDReading = false;
                rfidCom.Close();
                isRFIDconnect = false;
            }
            if (_readThread != null)
            {
                if (_readThread.ThreadState == ThreadState.Running)
                    _readThread.Abort();
            }

            
        }
        
        private void ReadRFID()
        {
            Console.WriteLine("ReadRFID()");
            while (_keepRFIDReading)
            {
                if (isRFIDconnect == true)
                {
                    //byte[] readBuffer = new byte[com.ReadBufferSize + 1];
                    /*
06 12 FE 00 15 03
06 22 FF 00 24 03
06 32 00 00 CB 03
08 12 00 02 04 00 E3 03
0A 22 00 04 94 9C 5E 19 9C 03
06 32 00 00 CB 03
06 32 00 00 CB 03
06 32 00 00 CB 03
06 32 00 00 CB 03
08 12 00 02 04 00 E3 03
0A 22 00 04 84 AA 3C 19 D8 03
06 32 00 00 CB 03
                     */
                    try
                    {
                        Byte[] b=new Byte[100];
                        Byte[] read = new Byte[100];
                        b[0] = 0x07;
                        b[1] = 0x12;
                        b[2] = 0x41;
                        b[3] = 0x01;
                        b[4] = 0x52;
                        b[5] = 0xf8;
                        b[6] = 0x03;                        
                        rfidCom.Write(b,0,7);
                        //Console.WriteLine("SerialInReq");
                        Thread.Sleep(100);
                        rfidCom.Read(read, 0, 10);
                        for (int i = 0; i < read.Length; i++)
                        {
                            //Console.Write(" {0:x2}" + " ", read[i]);
                        }
                        b[0] = 0x0c;
                        b[1] = 0x22;
                        b[2] = 0x42;
                        b[3] = 0x06;
                        b[4] = 0x93;
                        b[5] = 0x00;
                        b[6] = 0x78;
                        b[7] = 0x01;
                        b[8] = 0xa6;
                        b[9] = 0x00;
                        b[10] = 0xd9;
                        b[11] = 0x03;
                        rfidCom.Write(b, 0, 12);
                        //Console.WriteLine("SerialInCMMC");
                        Thread.Sleep(100);
                        rfidCom.Read(read, 0, 10);
                        for (int i = 0; i < read.Length; i++)
                        {
                            //Console.Write(" {0:x2}" + " ", read[i]);
                        }
                        if (read[5] == 0xAA)
                        {
                            setLocker1Open();
                        }
                        else if (read[5] == 0x9C)
                        {
                            setLocker2Open();
                        }
                        b[0] = 0x06;
                        b[1] = 0x32;
                        b[2] = 0x44;
                        b[3] = 0x00;
                        b[4] = 0x8f;
                        b[5] = 0x03;                        
                        rfidCom.Write(b, 0, 6);
                        //Console.WriteLine("SerialInEND");
                        Thread.Sleep(100);
                        rfidCom.Read(read, 0, 10);
                        for (int i = 0; i < read.Length; i++)
                        {
                           // Console.Write(" {0:x2}" + " ", read[i]);
                        }
                    }
                    catch (TimeoutException) { }
                    catch (IOException) { }
                }
                else
                {
                    TimeSpan waitTime = new TimeSpan(0, 0, 0, 500);
                    Thread.Sleep(waitTime);
                }
            } 
        
        
        }        
   
        private void setLocker1Open()
        {
            Console.WriteLine("setLocker1Open");
            if (isConnect == false)
            {
                MessageBox.Show("串口未和单片机连接", "错误");
                return;
            }

            com.WriteLine("1");
        }
        private void setLocker2Open()
        {
            Console.WriteLine("setLocker2Open");
            com.WriteLine("2");
        }

        private void setLocker3Open()
        {
            Console.WriteLine("setLocker3Open");
            com.WriteLine("3");
        }
    }
}
