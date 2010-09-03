// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.MSL.CUDFunction
{
    public class FunctionParameterMapping
    {
        public FunctionParameter SSDLFunctionParameter { get; set; }
        public FunctionParameterVersion? Version { get; set; }
    }
}
