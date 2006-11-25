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

    public enum CorSymSearchPolicyAttributes
    {
        // Fields
        AllowOriginalPathAccess = 4,
        AllowReferencePathAccess = 8,
        AllowRegistryAccess = 1,
        AllowSymbolServerAccess = 2
    }
}

#pragma warning restore 108, 1591