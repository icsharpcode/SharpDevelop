// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

#pragma warning disable 108, 1591 

namespace Debugger.Interop.CorSym
{
    using System;

    public enum CorSymAddrKind
    {
        // Fields
        ADDR_BITFIELD = 9,
        ADDR_IL_OFFSET = 1,
        ADDR_NATIVE_ISECTOFFSET = 10,
        ADDR_NATIVE_OFFSET = 5,
        ADDR_NATIVE_REGISTER = 3,
        ADDR_NATIVE_REGREG = 6,
        ADDR_NATIVE_REGREL = 4,
        ADDR_NATIVE_REGSTK = 7,
        ADDR_NATIVE_RVA = 2,
        ADDR_NATIVE_STKREG = 8
    }
}

#pragma warning restore 108, 1591