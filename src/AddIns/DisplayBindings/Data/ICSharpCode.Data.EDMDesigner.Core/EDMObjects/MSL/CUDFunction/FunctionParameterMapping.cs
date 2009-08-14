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
