// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using IO = System.IO;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceMapFileName
	{
		string path;
		
		public ServiceReferenceMapFileName(string serviceReferencesFolder, string serviceName)
		{
			path = IO.Path.Combine(serviceReferencesFolder, serviceName, "Reference.svcmap");
		}
		
		public string Path {
			get { return path; }
		}
	}
}
