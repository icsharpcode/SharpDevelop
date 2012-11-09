// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class SourceControl : MarshalByRefObject, global::EnvDTE.SourceControl
	{
		public SourceControl()
		{
		}
		
		public bool IsItemCheckedOut(string itemName)
		{
			throw new NotImplementedException();
		}
		
		public bool IsItemUnderSCC(string itemName)
		{
			throw new NotImplementedException();
		}
		
		public bool CheckOutItem(string itemName)
		{
			throw new NotImplementedException();
		}
	}
}
