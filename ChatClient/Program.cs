using System;
using System.Runtime.InteropServices;
using System.Text;

namespace ChatClient
{
    class ChatNative
    {
        [DllImport("Chat.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int NativeInit();
        [DllImport("Chat.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void NativeSend(StringBuilder msg, int len);

        public delegate void RECEVE_CALLBACK(IntPtr msg, int len);
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
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine(ChatNative.Init().ToString());
            ChatNative.SetReceveCallback(ReceveCallback);
            string msg = "";
            while (msg != "exit")
            {
                msg = Console.ReadLine();
                ChatNative.Send(msg);
            }
            ChatNative.Shutdown();
        }

        static void ReceveCallback(IntPtr msg, int len)
        {
            string s = Marshal.PtrToStringAnsi(msg, len);
            Console.WriteLine(s);
        }
    }


}
