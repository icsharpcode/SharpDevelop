// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.EntityType;
using System.Collections.ObjectModel;
using ICSharpCode.Data.EDMDesigner.Core.EDMObjects.SSDL.Function;

namespace ICSharpCode.Data.EDMDesigner.Core.EDMObjects.Designer.SSDL
{
    public class SSDLView
    {
        internal SSDLContainer SSDL { get; set; }

        public ObservableCollection<EntityType> Tables
        {
            get
            {
                return SSDL.EntityTypes;
            }
        }

        public ObservableCollection<Function> Functions
        {
            get
            {
                return SSDL.Functions;
            }
        }
    }
}
