// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ServiceModel.Description;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public interface IServiceReferenceProxyGenerator
	{
		ServiceReferenceGeneratorOptions Options { get; set; }
		void GenerateProxyFile();
		
		event EventHandler<GeneratorCompleteEventArgs> Complete;
	}
}
