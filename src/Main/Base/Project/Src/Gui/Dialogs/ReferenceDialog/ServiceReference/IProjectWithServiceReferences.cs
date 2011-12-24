// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public interface IProjectWithServiceReferences
	{
		string ServiceReferencesFolder { get; }
		ICodeDomProvider CodeDomProvider { get; }
		
		ServiceReferenceFileName GetServiceReferenceFileName(string serviceReferenceName);
		ServiceReferenceMapFileName GetServiceReferenceMapFileName(string serviceReferenceName);
		void AddServiceReferenceProxyFile(ServiceReferenceFileName fileName);
		void AddServiceReferenceMapFile(ServiceReferenceMapFileName fileName);
		void Save();
	}
}
