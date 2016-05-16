using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;    
using System.Reflection;
using System.Diagnostics;

namespace KeyBoardHock
{
  class BardCodeHooK    
    {
      public delegate void BardCodeDeletegate(BarCodes barCode);
      public event BardCodeDeletegate BarCodeEvent;

      public String bardcodeString;
    
        public struct BarCodes    
        {    
            public int VirtKey;      //虚拟码    
            public int ScanCode;     //扫描码    
            public string KeyName;   //键名    
            public uint AscII;       //AscII    
            public char Chr;         //字符    
    
            public string BarCode;   //条码信息    
            public bool IsValid;     //条码是否有效    
            public DateTime Time;    //扫描时间    
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
        private static extern int GetKeyNameText(int lParam, StringBuilder lpBuffer, int nSize);    
    
        [DllImport("user32", EntryPoint = "GetKeyboardState")]    
        private static extern int GetKeyboardState(byte[] pbKeyState);    
    
        [DllImport("user32", EntryPoint = "ToAscii")]    
        private static extern bool ToAscii(int VirtualKey, int ScanCode, byte[] lpKeyState, ref uint lpChar, int uFlags);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetModuleHandle(string name); 

        delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);    
        BarCodes barCode = new BarCodes();    
        int hKeyboardHook = 0;    
        string strBarCode = "";    
    
        private int KeyboardHookProc(int nCode, Int32 wParam, IntPtr lParam)    
        {    
            if (nCode == 0)    
            {    
                EventMsg msg = (EventMsg)Marshal.PtrToStructure(lParam, typeof(EventMsg));    
    
                if (wParam == 0x100)   //WM_KEYDOWN = 0x100    
                {    
                    barCode.VirtKey = msg.message & 0xff;  //虚拟码    
                    barCode.ScanCode = msg.paramL & 0xff;  //扫描码    
    
                    StringBuilder strKeyName = new StringBuilder(255);    
                    if (GetKeyNameText(barCode.ScanCode * 65536, strKeyName, 255) > 0)    
                    {
                        //Console.WriteLine("strKeyName = " + strKeyName.ToString());
                        barCode.KeyName = strKeyName.ToString().Trim(new char[] { ' ','\0' });    
                    }    
                    else    
                    {
                        Console.WriteLine(" clean ");
                        barCode.KeyName = "";    
                    }    
    
                    byte[] kbArray = new byte[256];    
                    uint uKey = 0;    
                    GetKeyboardState(kbArray);                                            
                    if (ToAscii(barCode.VirtKey, barCode.ScanCode, kbArray, ref uKey, 0))    
                    {    
                        barCode.AscII = uKey;    
                        barCode.Chr = Convert.ToChar(uKey);
                        Console.WriteLine("barCode.Chr = " + barCode.Chr);
                    }    
    
                    if (DateTime.Now.Subtract(barCode.Time).TotalMilliseconds > 100)     
                    {    
                        strBarCode = barCode.Chr.ToString();
                        Console.WriteLine("strBarCode = " + strBarCode);
                    }    
                    else    
                    {    
                        if ((msg.message & 0xff) == 13 && strBarCode.Length > 3)   //回车    
                        {
                            Console.WriteLine("strBarCode 2 = " + strBarCode);
                            barCode.BarCode = strBarCode;    
                            barCode.IsValid = true;    
                        }    
                        strBarCode += barCode.Chr.ToString();
                        bardcodeString = strBarCode;
                        Console.WriteLine("strBarCode 3= " + strBarCode);
                    }    
    
                    barCode.Time = DateTime.Now;    
                    if (BarCodeEvent != null) BarCodeEvent(barCode);    //触发事件    
                    barCode.IsValid = false;    
                }    
            }    
            return CallNextHookEx(hKeyboardHook, nCode, wParam, lParam);              
        }    
            
        // 安装钩子     
        public bool Start()    
        {    
            if (hKeyboardHook == 0)    
            {
                //WH_KEYBOARD_LL = 13                                       GetModuleHandle(Process.GetCurrentProcess().ModuleName)
                hKeyboardHook = SetWindowsHookEx(13, new HookProc(KeyboardHookProc), GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);    
            }
            Console.WriteLine("hKeyboardHook = " + hKeyboardHook);
            Console.WriteLine("GetLastWin32Error = "+Marshal.GetLastWin32Error());
            
            return (hKeyboardHook != 0);    
        }    
    
        // 卸载钩子     
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
