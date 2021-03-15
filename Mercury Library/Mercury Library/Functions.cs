using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MercuryLibrary
{
    public class Functions
    {
        public bool IsAttached(string LuaPipe)
        {
            return NamedPipes.NamedPipeExist(LuaPipe);
        }

        public void Inject(string LuaPipe, string DllName)
        {
            if (NamedPipes.NamedPipeExist(LuaPipe))
            {
                MessageBox.Show("Already Attached", "Error: Already Attached", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                return;
            }
            else if (!NamedPipes.NamedPipeExist(LuaPipe))
            {
                switch (Injector.DllInjector.GetInstance.Inject("RobloxPlayerBeta", AppDomain.CurrentDomain.BaseDirectory + DllName))
                {
                    case Injector.DllInjectionResult.DllNotFound:
                        MessageBox.Show("Dll Missing Or Wasn't Found", "Error: Dll Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    case Injector.DllInjectionResult.GameProcessNotFound:
                        MessageBox.Show("Couldn't Find Roblox Game Client", "Error: Roblox Not Found Or Not Opened", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    case Injector.DllInjectionResult.InjectionFailed:
                        MessageBox.Show("Injection Failed", "Error: Injection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                }
            }
        }

        public void ExecuteScript(string LuaPipe, string DllName, string Script)
        {
            if (NamedPipes.NamedPipeExist(LuaPipe))
            {
                using (NamedPipeClientStream namedPipeClientStream = new NamedPipeClientStream(".", LuaPipe, PipeDirection.Out))
                {
                    namedPipeClientStream.Connect();
                    using (StreamWriter streamWriter = new StreamWriter(namedPipeClientStream, Encoding.Default, 999999))
                    {
                        streamWriter.Write(Script);
                        streamWriter.Dispose();
                    }
                    namedPipeClientStream.Dispose();
                }
                return;
            }
            if (File.Exists(DllName))
            {
                MessageBox.Show("Not Attached", "Error: Not Attached", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            MessageBox.Show("Dll Not Found", "Error: Dll Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    internal class NamedPipes
    {
        public static bool NamedPipeExist(string PipeName)
        {
            bool result;
            try
            {
                if (!WaitNamedPipe(Path.GetFullPath(string.Format("\\\\.\\pipe\\{0}", PipeName)), 0))
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    if (lastWin32Error == 0)
                    {
                        result = false;
                        return result;
                    }
                    if (lastWin32Error == 2)
                    {
                        result = false;
                        return result;
                    }
                }
                result = true;
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool WaitNamedPipe(string name, int timeout);
    }

    internal class Injector
    {
        public enum DllInjectionResult
        {
            DllNotFound,
            GameProcessNotFound,
            InjectionFailed,
            Success
        }

        public sealed class DllInjector
        {
            static readonly IntPtr INTPTR_ZERO = (IntPtr)0;

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr OpenProcess(uint dwDesiredAccess, int bInheritHandle, uint dwProcessId);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern int CloseHandle(IntPtr hObject);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern int WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] buffer, uint size, int lpNumberOfBytesWritten);

            [DllImport("kernel32.dll", SetLastError = true)]
            static extern IntPtr CreateRemoteThread(IntPtr hProcess, IntPtr lpThreadAttribute, IntPtr dwStackSize, IntPtr lpStartAddress,
                IntPtr lpParameter, uint dwCreationFlags, IntPtr lpThreadId);

            static DllInjector _instance;

            public static DllInjector GetInstance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new DllInjector();
                    }
                    return _instance;
                }
            }

            DllInjector() { }

            public DllInjectionResult Inject(string sProcName, string sDllPath)
            {
                if (!File.Exists(sDllPath))
                {
                    return DllInjectionResult.DllNotFound;
                }

                uint _procId = 0;

                Process[] _procs = Process.GetProcesses();
                for (int i = 0; i < _procs.Length; i++)
                {
                    if (_procs[i].ProcessName == sProcName)
                    {
                        _procId = (uint)_procs[i].Id;
                        break;
                    }
                }

                if (_procId == 0)
                {
                    return DllInjectionResult.GameProcessNotFound;
                }

                if (!bInject(_procId, sDllPath))
                {
                    return DllInjectionResult.InjectionFailed;
                }

                return DllInjectionResult.Success;
            }

            bool bInject(uint pToBeInjected, string sDllPath)
            {
                IntPtr hndProc = OpenProcess((0x2 | 0x8 | 0x10 | 0x20 | 0x400), 1, pToBeInjected);

                if (hndProc == INTPTR_ZERO)
                {
                    return false;
                }

                IntPtr lpLLAddress = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryA");

                if (lpLLAddress == INTPTR_ZERO)
                {
                    return false;
                }

                IntPtr lpAddress = VirtualAllocEx(hndProc, (IntPtr)null, (IntPtr)sDllPath.Length, (0x1000 | 0x2000), 0X40);

                if (lpAddress == INTPTR_ZERO)
                {
                    return false;
                }

                byte[] bytes = Encoding.ASCII.GetBytes(sDllPath);

                if (WriteProcessMemory(hndProc, lpAddress, bytes, (uint)bytes.Length, 0) == 0)
                {
                    return false;
                }

                if (CreateRemoteThread(hndProc, (IntPtr)null, INTPTR_ZERO, lpLLAddress, lpAddress, 0, (IntPtr)null) == INTPTR_ZERO)
                {
                    return false;
                }

                CloseHandle(hndProc);

                return true;
            }
        }
    }
}
