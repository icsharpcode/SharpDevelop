// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ServiceModel.Description;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public interface IServiceReferenceProxyGenerator
	{
		string ServiceReferenceNamespace { get; set; }
		
		void GenerateProxyFile(MetadataSet metadata, string proxyFileName);
	}
}
