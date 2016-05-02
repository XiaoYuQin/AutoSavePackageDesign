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

namespace eHealth
{


    public partial class Form1 : Form
    {
        SerialPort com;
        bool isConnect;
        Thread _readThread;
        bool _keepReading;

        String value = "0";
        String longitude = "0";
        String latitudeX = "0";

        bool isLocated = false;

        int mapflag = 0;
        //BardCodeHooK BarCode = new BardCodeHooK();
        private System.Timers.Timer timer = new System.Timers.Timer();

        Locker locker1 = new Locker();
        Locker locker2 = new Locker();
        Locker locker3 = new Locker();


        private void debug(String str)
        {
            Console.WriteLine(str);
        }


        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            //BarCode.BarCodeEvent += new BardCodeHooK.BardCodeDeletegate(BarCode_BarCodeEvent);
            com = new SerialPort();
            isConnect = false;
            


            timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);
            timer.Enabled = true;

 
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
            }
            //serialBox.SelectedIndex = 0;
            button1.Text = "closed";
            //com.ReceivedBytesThreshold = 0;

            /***************************设置所有锁列表为第一个********************************/
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
        }
        private delegate void ShowInfoDelegate(BardCodeHooK.BarCodes barCode);
        private void ShowInfo(BardCodeHooK.BarCodes barCode)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new ShowInfoDelegate(ShowInfo), new object[] { barCode });
            }
            else
            {
                /*textBox1.Text = barCode.KeyName;
                textBox2.Text = barCode.VirtKey.ToString();
                textBox3.Text = barCode.ScanCode.ToString();
                textBox4.Text = barCode.Ascll.ToString();
                textBox5.Text = barCode.Chr.ToString();
                textBox6.Text = barCode.IsValid ? barCode.BarCode : "";//是否为扫描枪输入，如果为true则是 否则为键盘输入
                textBox7.Text += barCode.KeyName;*/

                MessageBox.Show(barCode.IsValid.ToString());

                //Console.WriteLine(text);
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
                    com.BaudRate = 9600;
                    Console.WriteLine(serialBox.Text);
                    com.PortName = serialBox.Text;
                    com.DataBits = 8;
                    com.Open();
                    isConnect = true;
                    _keepReading = true;
                    _readThread = new Thread(ReadPort);
                    _readThread.Start();
                    button1.Text = "opened";
                }
                catch (System.IO.IOException) {
                    button1.Text = "closed";
                    isConnect = false;
                    MessageBox.Show("SerialPort Open FAIL", "ERROR");                
                }
                catch (System.NullReferenceException){
                    button1.Text = "closed";
                    isConnect = false;
                    MessageBox.Show("Open SerialPort", "SerialPort FAIL");
                }
            }
            else { 
                
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

                    //byte[] readBuffer = new byte[com.ReadBufferSize + 1];
                    try
                    {

                        String SerialIn = com.ReadLine();
                        Console.WriteLine(SerialIn);

                        String sssss = SerialIn;
                        //textBox1.Text = textBox1.Text + sssss + "\n\r";
                        //textBox1.SelectionStart = textBox1.Text.Length - 1;
                        //textBox1.ScrollToCaret();
                        //sssss = "AT+BT=60\r\n";
                        if (sssss.IndexOf("AT+ST=") == 0)
                        {
                            int i = sssss.IndexOf("AT+ST=") + 6;
                            int j = sssss.IndexOf("\r\n");
                            string strx = sssss.Substring(i, j - i + 2);
                            Console.WriteLine("ST = " + strx);
                           // label8.Text = strx;
                        }
                        else if (sssss.IndexOf("AT+LO=") == 0)
                        {
                            int i = sssss.IndexOf("AT+LO=") + 6;
                            int j = sssss.IndexOf('\n');
                            string strx = sssss.Substring(i, sssss.Length - 6);
                            Console.WriteLine("LO = " + strx);
                            //label6.Text = strx;
                            longitude = strx;
                            if (strx == "0")
                            {
                                //label5.Text = "no locate";
                                //this.label5.ForeColor = Color.Red;
                                isLocated = false;
                            }
                            else
                            {
                                //label5.Text = "located";
                               // this.label5.ForeColor = Color.Green;
                                isLocated = true;
                            }
                        }
                        else if (sssss.IndexOf("AT+LA=") == 0)
                        {
                            int i = sssss.IndexOf("AT+LA=") + 6;
                            int j = sssss.IndexOf('\n');
                            string strx = sssss.Substring(i, sssss.Length - 6);
                            Console.WriteLine("LA = " + strx);

   
                            //label7.Text = "-"+strx;
                            latitudeX = strx;


           

                           /* mapflag++;
                            if (mapflag > 20)
                            {
                                mapflag = 0;
                                HttpPost();
                            }*/
                            //HttpPost();
                        }
                        else if (sssss.IndexOf("AT+BT=") == 0)
                        {
                            int i = sssss.IndexOf("AT+BT=") + 6;
                            int j = sssss.IndexOf('\n');
                            Console.WriteLine("j = " + j);
                            Console.WriteLine("i = " + i);
                            Console.WriteLine(sssss.Length);
                            string strx = sssss.Substring(i, sssss.Length - 6);
                            Console.WriteLine("BT = " + strx);

                            //   if (isLocated == true)
                            {
                                try
                                {
                                    int btInt = Int32.Parse(strx);
                                     //if (btInt < 150)
                                    {
                                        //label10.Text = strx;
                                        value = strx;
                                        Console.WriteLine(btInt);
                                       // HttpPost();
                                    }
                                }
                                catch (System.FormatException)
                                {
                                    Console.WriteLine("conver error");
                                }

                            }
                        }


                        //HttpPost(value, longitude, latitude);

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


        private void button2_Click(object sender, EventArgs e)
        {
            if (isConnect == true) {
                _keepReading = false;
                com.Close();
                isConnect = false;
            }
            this.Close();
            /*com.WriteLine("1234");
            Console.WriteLine("1234");*/
        }
        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            //模拟的做一些耗时的操作
            Console.WriteLine("1234");
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
                MessageBox.Show("锁号为：" + (lockerNumber+1)+"号锁" + "\n密码为：" + textBox2.Text,"设置锁密码成功");
                textBox2.Text = "";
            }
        }

        /*
         读取所有锁的密码
         */
        private void button5_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1号锁" + "  密码为：" + locker1.getPassword()
                + "\n2号锁" + "  密码为：" + locker2.getPassword()
                + "\n3号锁" + "  密码为：" + locker3.getPassword(), "读所有锁");
        }

        private void button6_Click(object sender, EventArgs e)
        {
            locker1.cleanPassword();
            locker2.cleanPassword();
            locker3.cleanPassword();
            MessageBox.Show("所有密码成功清除", "清除锁密码");
        }

        private void button8_Click(object sender, EventArgs e)
        {
            Random rad = new Random();//实例化随机数产生器rad；
            int value1 = rad.Next(100000, 1000000);//用rad生成大于等于1000，小于等于9999的随机数；
            int value2 = rad.Next(100000, 1000000);//用rad生成大于等于1000，小于等于9999的随机数；
            debug("value1 " + value1);
            debug("value2 " + value2);
            ulong bardcode = (ulong)value1 * 1000000 + (ulong)value2;
            debug("bardcode = " + bardcode);
            BardCodeMessageBox box = new BardCodeMessageBox();
            box.ShowDialog();
        }




    }
}
