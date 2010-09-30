// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.Reports.Core{
	
	
	public sealed class DisplayNameAttribute : Attribute{
		string name;
		
		public string Name{
			get { return name; }
		}
		
		public DisplayNameAttribute(string name){
			this.name = name;
		}
	}
}
