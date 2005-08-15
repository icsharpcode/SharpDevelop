namespace DebuggerInterop.Core
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [ComImport, Guid("5E0B54E7-D88A-4626-9420-A691E0A78B49"), InterfaceType((short) 1)]
    public interface ICorDebugValue2
    {
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType=MethodCodeType.Runtime)]
        void GetExactType([MarshalAs(UnmanagedType.Interface)] out ICorDebugType ppType);
    }
}

