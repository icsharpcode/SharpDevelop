// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Shell.Flavor
{
	public class FlavoredProject : MarshalByRefObject, IVsAggregatableProject, IVsHierarchy
	{
		public FlavoredProject()
		{
		}
	    
        public int GetAggregateProjectTypeGuids(out string projTypeGuids)
        {
            throw new NotImplementedException();
        }
	}
}
