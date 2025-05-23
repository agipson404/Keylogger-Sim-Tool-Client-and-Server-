# 🛡️ C# Keylogger Simulation Tool (Client–Server)  
**Created By:** Arthur Gipson  
📺 **YouTube Video:** [https://youtu.be/caqFF6DSHxQ](https://youtu.be/caqFF6DSHxQ)

This project is a red team–oriented malware simulation tool written in C#. It replicates core behaviors of advanced keylogging malware in a safe, controlled environment for the purpose of cybersecurity training, malware analysis, and reverse engineering demonstrations.

The simulation was developed and tested in a virtualized lab environment. A Windows 10 Pro virtual machine was manually set up and configured using VMware Workstation to safely contain and observe the behavior of the client-side keylogger and server interactions.

The solution includes a persistent keylogger client and a TCP-based command-and-control (C2) server. Logs are AES-encrypted, Base64-encoded, and streamed over a socket connection, simulating real-world exfiltration behavior. The project also supports dynamic analysis using Wireshark and debugger-based reverse engineering via x32dbg.

> ⚠️ **This tool is strictly for educational and ethical use within isolated virtual machines or lab environments.** Never run or distribute this code in a production environment or on unapproved systems.

---

## 🧠 Simulated Behaviors (MITRE ATT&CK Mapping)

This project emulates common malware behaviors, aligned with the MITRE ATT&CK® framework.

| Behavior                                      | Description                                                                 | MITRE ID     |
|----------------------------------------------|-----------------------------------------------------------------------------|--------------|
| Keyboard Input Capture                        | Captures user keystrokes via API hook                                       | `T1056.001`  |
| Active Window Title Logging                   | Records the foreground application for context-aware logging                | `T1056`      |
| Encrypted Exfiltration via Custom Protocol    | Sends AES-encrypted logs over custom TCP socket                             | `T1041`      |
| Base64 Encoding for Obfuscation               | Encodes payloads before transmission to mimic benign traffic                | `T1027`      |
| Persistent Beacon-like Communication          | Maintains a consistent socket connection for regular data exfil             | `T1071.001`  |
| In-Memory Function Hooking                    | Uses `SetWindowsHookEx` to install runtime hooks                            | `T1055`      |
| Manual Analysis with Static & Dynamic Tools   | Supports reverse engineering with tools like Wireshark, x32dbg, dnSpy       | `T1140`*     |

> *T1140 typically applies to obfuscated files or information and is referenced here for decryption analysis.

---

## 🔧 Key Features

- **Captures Keystrokes**  
  Hooks into the keyboard using Windows API to log everything typed.

- **Tracks Active Window**  
  Logs the name of the program or window the user is typing in.

- **Encrypts Logs with AES-128**  
  Protects the log data using AES encryption before sending it.

- **Encodes Data with Base64**  
  Converts the encrypted log data into Base64 format for easier transmission.

- **Sends Logs Over TCP**  
  Keeps a live connection to the server and sends logs every few seconds.

- **Server Decrypts and Saves Logs**  
  The server receives the logs, decrypts them, adds a timestamp and IP address, then saves them.

- **Designed for Reverse Engineering**  
  The project can be analyzed with tools like Wireshark and x32dbg.

- **Includes Blue Team Demo**  
  The video shows how to detect, analyze, and stop the keylogger using reverse engineering tools.

---

## 📁 Project Structure

```bash
Keylogger-Sim-Tool-Client-and-Server/
│
├── Keylogger Sim Tool (Client)/         # C# keylogger client project
│   ├── Program.cs                       # Hooks, logs, encrypts, sends keystrokes
│   ├── Keylogger Sim Tool (Client).csproj
│
├── Keylogger Sim Tool (Server)/         # C# TCP server project
│   ├── Program.cs                       # Receives, decrypts, and logs data
│   ├── Keylogger Sim Tool (Server).csproj
│
├── .gitignore                           # Ignores bin/, obj/, temp files, etc.
├── .gitattributes                       # Git configuration
├── LICENSE                              # MIT license
├── README.md                            # Project description and usage guide
└── Keylogger Sim Tool (Client and Server).sln  # Visual Studio solution file
