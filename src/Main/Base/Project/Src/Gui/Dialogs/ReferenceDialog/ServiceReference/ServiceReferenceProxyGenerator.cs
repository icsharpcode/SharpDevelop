// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.IO;
using System.ServiceModel.Description;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceProxyGenerator : IServiceReferenceProxyGenerator
	{
		SvcUtilOptions options = new SvcUtilOptions();
		
		public ServiceReferenceGeneratorOptions Options {
			get { return options; }
		}
		
		public void GenerateProxyFile()
		{
			var runner = new SvcUtilRunner(options);
			runner.Run();
		}
	}
}
