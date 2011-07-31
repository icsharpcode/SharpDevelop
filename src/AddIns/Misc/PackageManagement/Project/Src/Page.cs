// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement
{
	public class Page
	{
		public int Number { get; set; }
		public bool IsSelected { get; set; }
		
		public override string ToString()
		{
			return String.Format("[Page] Number={0}, IsSelected={1}", Number, IsSelected);
		}
	}
}
