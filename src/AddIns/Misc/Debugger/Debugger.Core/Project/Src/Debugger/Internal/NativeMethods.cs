using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

using DebuggerInterop.Core;
using DebuggerInterop.MetaData;

namespace DebuggerLibrary
{        
    internal static class NativeMethods
    {
        [System.Runtime.ConstrainedExecution.ReliabilityContract(System.Runtime.ConstrainedExecution.Consistency.WillNotCorruptState, System.Runtime.ConstrainedExecution.CER.Success),
        DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("mscoree.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
        public static extern int CreateDebuggingInterfaceFromVersion(int debuggerVersion, string debuggeeVersion, out ICorDebug cordbg);

        [DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
        public static extern int GetCORVersion([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName, Int32 cchBuffer, out Int32 dwLength);
    }
}