namespace DebuggerInterop.MetaData
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct COR_FIELD_OFFSET
    {
        public uint ridOfField;
        public uint ulOffset;
    }
}

