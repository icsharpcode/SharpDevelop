// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceMapFileProjectItem : FileProjectItem
	{
		public ServiceReferenceMapFileProjectItem(
			IProject project,
			string fileName)
			: base(project, ItemType.None)
		{
			this.FileName = fileName;
			AddMetadata();
		}
		
		void AddMetadata()
		{
			SetMetadata("LastGenOutput", "Reference.cs");
			SetMetadata("Generator", "WCF Proxy Generator");
		}
	}
}
