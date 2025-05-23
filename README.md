🛡️ C# Keylogger Simulation Tool (Client–Server)
This project is a red team–oriented malware simulation tool written in C#. It replicates core behaviors of advanced keylogging malware in a safe, controlled environment for the purpose of cybersecurity training, malware analysis, and reverse engineering demonstrations.

The simulation was developed and tested in a virtualized lab environment. A Windows 10 Pro virtual machine was manually set up and configured using VMware Workstation to safely contain and observe the behavior of the client-side keylogger and server interactions.

The solution includes a persistent keylogger client and a TCP-based command-and-control (C2) server. Logs are AES-encrypted, Base64-encoded, and streamed over a socket connection, simulating real-world exfiltration behavior. The project also supports dynamic analysis using Wireshark and debugger-based reverse engineering via x32dbg.

⚠️ This tool is strictly for educational and ethical use within isolated virtual machines or lab environments. Never run or distribute this code in a production environment or on unapproved systems.

🔧 Key Features
Captures Keystrokes
Hooks into the keyboard using Windows API to log everything typed.

Tracks Active Window
Logs the name of the program or window the user is typing in.

Encrypts Logs with AES-128
Protects the log data using AES encryption before sending it.

Encodes Data with Base64
Converts the encrypted log data into Base64 format for easier transmission.

Sends Logs Over TCP
Keeps a live connection to the server and sends logs every few seconds.

Server Decrypts and Saves Logs
The server receives the logs, decrypts them, adds a timestamp and IP address, then saves them.

Designed for Reverse Engineering
The project can be analyzed with tools like Wireshark and x32dbg.

Includes Blue Team Demo
The video shows how to detect, analyze, and stop the keylogger using reverse engineering tools.
