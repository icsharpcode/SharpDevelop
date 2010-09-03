// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Xml.Linq;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer
{
    public class EDMController
    {
        public static EDMView CreateView(string path)
        {
            return new EDMView(path, ReadMoreAction);
        }

        public static Action<XElement> ReadMoreAction { get; set; }
    }
}
