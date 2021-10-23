using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text;

namespace ChatClient
{
    public delegate void RECEVE_CALLBACK(IntPtr msg, int len);
    public class ChatNative
    {
        [DllImport("Chat.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NativeInit();
        [DllImport("Chat.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeSend(StringBuilder msg, int len);
        [DllImport("Chat.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeSetReceveCallback(RECEVE_CALLBACK callback);
        [DllImport("Chat.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeShutdown();
        public static int Init()
        {
            return NativeInit();
        }
        public static void Send(string msg)
        {
            StringBuilder sb = new StringBuilder(msg);
            NativeSend(sb, sb.Length);
        }
        public static void SetReceveCallback(RECEVE_CALLBACK callback)
        {
            NativeSetReceveCallback(callback);
        }
        public static void Shutdown()
        {
            NativeShutdown();
        }

    }

    class Program
    {
        private static Form1 form = null;
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Chat client");
            Console.WriteLine(ChatNative.Init().ToString());
            RECEVE_CALLBACK callbackDelegate = null;
            callbackDelegate += ReceveCallback;
            ChatNative.SetReceveCallback(callbackDelegate);
            string msg = "Connected";
            ChatNative.Send(msg);

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            form = new Form1();
            Application.Run(form);
        }

        static void ReceveCallback(IntPtr msg, int len)
        {
            string s = Marshal.PtrToStringAnsi(msg, len);
            Console.WriteLine("From server: {0}", s);
            form.OnReceveMsg(s);
        }
    }
}
