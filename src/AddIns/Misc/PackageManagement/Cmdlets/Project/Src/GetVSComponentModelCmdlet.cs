// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Management.Automation;
using ICSharpCode.PackageManagement.Scripting;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Shell;

namespace ICSharpCode.PackageManagement.Cmdlets
{
	[Cmdlet(VerbsCommon.Get, "VSComponentModel")]
	[OutputType(typeof(IComponentModel))]
	public class GetVSComponentModelCmdlet : PSCmdlet
	{
		public GetVSComponentModelCmdlet()
		{
		}
		
		protected override void ProcessRecord()
		{
			object service = Package.GetGlobalService(typeof(SComponentModel));
			WriteObject(service);
		}
	}
}
