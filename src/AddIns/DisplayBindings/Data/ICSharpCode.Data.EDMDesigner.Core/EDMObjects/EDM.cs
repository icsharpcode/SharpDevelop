// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.CSDL;
using System.Xml.Linq;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects
{
    public class EDM
    {
        public SSDLContainer SSDLContainer { get; set; }
        public CSDLContainer CSDLContainer { get; set; }
        public bool IsEmpty 
        {
        	get 
        	{
        		if (SSDLContainer == null || CSDLContainer == null)
        			return true;
				else
					return false;
        	}
        }

        public IEnumerable<DesignerProperty> DesignerProperties { get; internal set; }
        public IEnumerable<DesignerProperty> EDMXDesignerDesignerProperties { get; internal set; }
        public IEnumerable<XElement> EDMXDesignerDiagrams { get; internal set; }
    }
}
