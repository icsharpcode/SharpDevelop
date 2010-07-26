/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 05.01.2009
 * Zeit: 19:27
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
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
