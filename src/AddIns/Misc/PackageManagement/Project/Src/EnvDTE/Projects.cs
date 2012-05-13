// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Projects : MarshalByRefObject, IEnumerable<Project>
	{
		public Projects()
		{
		}
	    
        public IEnumerator<Project> GetEnumerator()
        {
            throw new NotImplementedException();
        }
	    
        IEnumerator IEnumerable.GetEnumerator()
        {
        	return GetEnumerator();
        }
	}
}
