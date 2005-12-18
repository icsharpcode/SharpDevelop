// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision$</version>
// </file>

namespace Debugger.Interop.CorDebug
{
    using System;

    public enum CorDebugMappingResult
    {
        // Fields
        MAPPING_APPROXIMATE = 0x20,
        MAPPING_EPILOG = 2,
        MAPPING_EXACT = 0x10,
        MAPPING_NO_INFO = 4,
        MAPPING_PROLOG = 1,
        MAPPING_UNMAPPED_ADDRESS = 8
    }
}
