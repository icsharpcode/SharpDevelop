// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using IO = System.IO;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceFileName
	{
		string serviceReferencesFolder;
		string serviceName;
		string path;
		
		public ServiceReferenceFileName()
			: this(String.Empty, String.Empty)
		{
		}
		
		public ServiceReferenceFileName(
			string serviceReferencesFolder,
			string serviceName)
		{
			this.serviceReferencesFolder = serviceReferencesFolder;
			this.serviceName = serviceName;
		}
		
		public string Path {
			get {
				if (path == null) {
					GetPath();
				}
				return path;
			}
		}
		
		void GetPath()
		{
			path = IO.Path.Combine(serviceReferencesFolder, serviceName, "Reference.cs");
		}
		
		public string ServiceName {
			get { return serviceName; }
			set { serviceName = value; }
		}
		
		public string ServiceReferencesFolder {
			get { return serviceReferencesFolder; }
			set { serviceReferencesFolder = value; }
		}
	}
}
