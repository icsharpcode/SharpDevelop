// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

//ToDo with NET2.0 change this Interface to the one provided by NET2.0
using System;

namespace ICSharpCode.Reports.Core {
	
	public interface IHierarchyData{
		
		IndexList GetChildren {
			get;
		}

		bool HasChildren {
			get;
		}
		
		object Item {
			get;
		}
		string Path {
			get;
		}
		
		string Type {
			get;
		}
	}
}
