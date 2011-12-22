// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public interface IProjectWithServiceReferences
	{
		string ServiceReferencesFolder { get; }
		ICodeDomProvider CodeDomProvider { get; }
		
		string GetServiceReferenceFileName(string serviceReferenceName);
		void AddServiceReferenceProxyFile(string fileName);
		void Save();
	}
}
