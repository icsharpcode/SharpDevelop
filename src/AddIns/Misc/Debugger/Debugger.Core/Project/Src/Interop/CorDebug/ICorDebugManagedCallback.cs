// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorDebug
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, InterfaceType((short) 1), Guid("3D6F5F60-7538-11D3-8D5B-00104B35E7EF")]
    public interface ICorDebugManagedCallback
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Breakpoint([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pBreakpoint);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void StepComplete([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pStepper, [In] Debugger.Wrappers.CorDebug.CorDebugStepReason reason);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Break([In] IntPtr pAppDomain, [In] IntPtr thread);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void Exception([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] int unhandled);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EvalComplete([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pEval);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EvalException([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pEval);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateProcess([In] IntPtr pProcess);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ExitProcess([In] IntPtr pProcess);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateThread([In] IntPtr pAppDomain, [In] IntPtr thread);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ExitThread([In] IntPtr pAppDomain, [In] IntPtr thread);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void LoadModule([In] IntPtr pAppDomain, [In] IntPtr pModule);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void UnloadModule([In] IntPtr pAppDomain, [In] IntPtr pModule);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void LoadClass([In] IntPtr pAppDomain, [In] IntPtr c);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void UnloadClass([In] IntPtr pAppDomain, [In] IntPtr c);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void DebuggerError([In] IntPtr pProcess, [In, MarshalAs(UnmanagedType.Error)] int errorHR, [In] uint errorCode);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void LogMessage([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] int lLevel, [In] IntPtr pLogSwitchName, [In] IntPtr pMessage);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void LogSwitch([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] int lLevel, [In] uint ulReason, [In] IntPtr pLogSwitchName, [In] IntPtr pParentName);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void CreateAppDomain([In] IntPtr pProcess, [In] IntPtr pAppDomain);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ExitAppDomain([In] IntPtr pProcess, [In] IntPtr pAppDomain);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void LoadAssembly([In] IntPtr pAppDomain, [In] IntPtr pAssembly);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void UnloadAssembly([In] IntPtr pAppDomain, [In] IntPtr pAssembly);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void ControlCTrap([In] IntPtr pProcess);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void NameChange([In] IntPtr pAppDomain, [In] IntPtr pThread);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void UpdateModuleSymbols([In] IntPtr pAppDomain, [In] IntPtr pModule, [In] IntPtr pSymbolStream);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void EditAndContinueRemap([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pFunction, [In] int fAccurate);
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void BreakpointSetError([In] IntPtr pAppDomain, [In] IntPtr pThread, [In] IntPtr pBreakpoint, [In] uint dwError);
    }
}

#pragma warning restore 108, 1591