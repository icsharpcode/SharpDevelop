// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Interfaces;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Common;

#endregion

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Association
{
    public class Association : EDMObjectBase
    {
        public string AssociationSetName { get; set; }
        public Role Role1 { get; set; }
        public Role Role2 { get; set; }
        public Role PrincipalRole { get; set; }
        public Role DependantRole { get; set; }

        public SSDLContainer Container { get; internal set; }
    }
}
