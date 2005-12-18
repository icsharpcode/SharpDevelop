// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Text;

using Debugger.Interop.CorDebug;
using Debugger.Interop.MetaData;

namespace Debugger
{        
    internal static class NativeMethods
    {
//        [System.Runtime.ConstrainedExecution.ReliabilityContract(System.Runtime.ConstrainedExecution.Consistency.WillNotCorruptState, System.Runtime.ConstrainedExecution.CER.Success),
		[DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("mscoree.dll", CharSet=CharSet.Unicode, PreserveSig=false)]
        public static extern int CreateDebuggingInterfaceFromVersion(int debuggerVersion, string debuggeeVersion, out ICorDebug cordbg);

        [DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
        public static extern int GetCORVersion([Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder szName, Int32 cchBuffer, out Int32 dwLength);
        
        [DllImport("mscoree.dll", CharSet=CharSet.Unicode)]
        public static extern int GetRequestedRuntimeVersion(string exeFilename, [Out, MarshalAs(UnmanagedType.LPWStr)] StringBuilder pVersion, Int32 cchBuffer, out Int32 dwLength);
    }
}
