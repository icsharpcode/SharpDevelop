// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public interface IProjectWithServiceReferences
	{
		string ServiceReferencesFolder { get; }
		string Language { get; }
		string RootNamespace { get; }
		
		ServiceReferenceFileName GetServiceReferenceFileName(string serviceReferenceName);
		ServiceReferenceMapFileName GetServiceReferenceMapFileName(string serviceReferenceName);
		void AddServiceReferenceProxyFile(ServiceReferenceFileName fileName);
		void AddServiceReferenceMapFile(ServiceReferenceMapFileName fileName);
		void Save();
		void AddAssemblyReference(string referenceName);
		bool HasAppConfigFile();
		string GetAppConfigFileName();
		void AddAppConfigFile();
		IEnumerable<ReferenceProjectItem> GetReferences();
	}
}
