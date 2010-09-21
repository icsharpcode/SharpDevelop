// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;

namespace ICSharpCode.Reports.Addin.ReportWizard
{
	/// <summary>
	/// Description of WizardCommands.
	/// </summary>
	public class XsdSchemaAndDataCommand:AbstractMenuCommand
	{
		
	
		public override void Run()
		{
			ResultPanel panel = (ResultPanel)base.Owner;
			if (panel != null) {
				panel.SaveXsdFile(false);
			}
			
		}
		
	}
	
	
	public class XsdSchemaOnlyCommand:AbstractMenuCommand
	{
		public XsdSchemaOnlyCommand ()
		{
			
		}
		
		public override void Run()
		{
			ResultPanel panel = (ResultPanel)base.Owner;
			if (panel != null) {
				panel.SaveXsdFile(true);
			}
		}
		
	}
}
