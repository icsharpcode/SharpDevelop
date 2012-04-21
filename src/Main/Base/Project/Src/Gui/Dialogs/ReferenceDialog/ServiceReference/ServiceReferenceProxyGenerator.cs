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
		ServiceReferenceGeneratorOptions options = new ServiceReferenceGeneratorOptions();
		
		public ServiceReferenceGeneratorOptions Options {
			get { return options; }
			set { options = value; }
		}
		
		public void GenerateProxyFile()
		{
			var runner = new SvcUtilRunner(options);
			runner.ProcessExited += OnComplete;
			runner.Run();
		}
		
		public event EventHandler Complete;
		
		void OnComplete(object sender, EventArgs e)
		{
			if (Complete != null) {
				Complete(this, e);
			}
		}
	}
}
