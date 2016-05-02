using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices; 
using System.Windows.Forms;
namespace Design
{
    /// <summary>
    /// 获取键盘输入或者USB扫描枪数据 可以是没有焦点 应为使用的是全局钩子
    /// USB扫描枪 是模拟键盘按下
    /// 这里主要处理扫描枪的值，手动输入的值不太好处理
    /// </summary>
    public class BardCodeHooK
    {
        public delegate void BardCodeDeletegate(BarCodes barCode);
        public event BardCodeDeletegate BarCodeEvent;

        //定义成静态，这样不会抛出回收异常
        private static HookProc hookproc;


        public struct BarCodes
        {
            public int VirtKey;//虚拟吗
            public int ScanCode;//扫描码
            public string KeyName;//键名
            public uint Ascll;//Ascll
            public char Chr;//字符

            public string OriginalChrs; //原始 字符
            public string OriginalAsciis;//原始 ASCII

            public string OriginalBarCode;  //原始数据条码

            public bool IsValid;//条码是否有效
            public DateTime Time;//扫描时间,

            public string BarCode;//条码信息   保存最终的条码
        }

        private struct EventMsg
        {
            public int message;
            public int paramL;
            public int paramH;
            public int Time;
            public int hwnd;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

        [DllImport("user32", EntryPoint = "GetKeyNameText")]
        private static extern int GetKeyNameText(int IParam, StringBuilder lpBuffer, int nSize);

        [DllImport("user32", EntryPoint = "GetKeyboardState")]
        private static extern int GetKeyboardState(byte[] pbKeyState);

        [DllImport("user32", EntryPoint = "ToAscii")]
        private static extern bool ToAscii(int VirtualKey, int ScanCode, byte[] lpKeySate, ref uint lpChar, int uFlags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name);


        delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);
        BarCodes barCode = new BarCodes();
        int hKeyboardHook = 0;
        // string strBarCode = "";
        StringBuilder sbBarCode = new StringBuilder();




        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)
        {
            int i_calledNext = -10;
            if (nCode == 0)
            {
                EventMsg msg = (EventMsg)Marshal.PtrToStructure(lParam, typeof(EventMsg));
                if (wParam == 0x100)//WM_KEYDOWN=0x100
                {
                    barCode.VirtKey = msg.message & 0xff;//虚拟吗
                    barCode.ScanCode = msg.paramL & 0xff;//扫描码
                    StringBuilder strKeyName = new StringBuilder(225);
                    if (GetKeyNameText(barCode.ScanCode * 65536, strKeyName, 255) > 0)
                    {
                        barCode.KeyName = strKeyName.ToString().Trim(new char[] { ' ', '\0' });
                    }
                    else
                    {
                        barCode.KeyName = "";
                    }
                    byte[] kbArray = new byte[256];
                    uint uKey = 0;
                    GetKeyboardState(kbArray);

                    //  if (barCode.KeyName.Equals("A") || barCode.KeyName.Equals("a"))
                    //  {
                    if (ToAscii(barCode.VirtKey, barCode.ScanCode, kbArray, ref uKey, 0))
                    {
                        barCode.Ascll = uKey;
                        barCode.Chr = Convert.ToChar(uKey);

                        barCode.OriginalChrs += " " + Convert.ToString(barCode.Chr);
                        barCode.OriginalAsciis += " " + Convert.ToString(barCode.Ascll);

                        barCode.OriginalBarCode += Convert.ToString(barCode.Chr);

                    }
                    // }

                    TimeSpan ts = DateTime.Now.Subtract(barCode.Time);

                    if (ts.TotalMilliseconds > 30)
                    {//时间戳，大于50 毫秒表示手动输入
                        // strBarCode = barCode.Chr.ToString();
                        sbBarCode.Remove(0, sbBarCode.Length);
                        sbBarCode.Append(barCode.Chr.ToString());



                        barCode.OriginalChrs = " " + Convert.ToString(barCode.Chr);
                        barCode.OriginalAsciis = " " + Convert.ToString(barCode.Ascll);

                        barCode.OriginalBarCode = Convert.ToString(barCode.Chr);

                    }
                    else
                    {
                        if ((msg.message & 0xff) == 13 && sbBarCode.Length > 3)
                        {//回车
                            // barCode.BarCode = strBarCode;
                            // barCode.BarCode = sbBarCode.ToString();

                            barCode.BarCode = barCode.OriginalBarCode;

                            barCode.IsValid = true;
                            //strBarCode = "";
                            sbBarCode.Remove(0, sbBarCode.Length);
                        }
                        //

                        //strBarCode += barCode.Chr.ToString();//  updated by dragon 20141124
                        sbBarCode.Append(barCode.Chr.ToString());
                        //strBarCode += barCode.KeyName;
                        // strBarCode += barCode.VirtKey.ToString();
                    }


                    try
                    {
                        if (BarCodeEvent != null && barCode.IsValid)
                        {

                            //barCode.BarCode = barCode.BarCode.Replace("\b", "").Replace("\0","");  可以不需要 因为大于50毫秒已经处理
                            //先进行 WINDOWS事件往下传
                            i_calledNext = CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);


                            BarCodeEvent(barCode);//触发事件

                            barCode.BarCode = "";

                            barCode.OriginalChrs = "";
                            barCode.OriginalAsciis = "";
                            barCode.OriginalBarCode = "";

                        }

                    }
                    catch { }
                    finally
                    {
                        barCode.IsValid = false; //最后一定要 设置barCode无效
                        barCode.Time = DateTime.Now;
                    }
                }
            }
            if (i_calledNext == -10)
            {
                i_calledNext = CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);
            }
            return i_calledNext;
        }

        //安装钩子
        public bool Start()
        {
            if (hKeyboardHook == 0)
            {
                hookproc = new HookProc(KeyboardHookProc);


                //GetModuleHandle 函数 替代 Marshal.GetHINSTANCE
                //防止在 framework4.0中 注册钩子不成功
                IntPtr modulePtr = GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName);
                //IntPtr modulePtr = GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
                //IntPtr modulePtr = ;
                //WH_KEYBOARD_LL=13
                //全局钩子 WH_KEYBOARD_LL
                //  hKeyboardHook = SetWindowsHookEx(13, hookproc, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);

                hKeyboardHook = SetWindowsHookEx(13, hookproc, modulePtr, 0);
            }
            return (hKeyboardHook != 0);
        }

        //卸载钩子
        public bool Stop()
        {
            if (hKeyboardHook != 0)
            {
                return UnhookWindowsHookEx(hKeyboardHook);
            }
            return true;
        }

    }
}
