/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 14.08.2009
 * Zeit: 20:04
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */
using System;

namespace ICSharpCode.Reports.Core.Events
{
	
	public class NewPageEventArgs : System.EventArgs {
		
		private ExporterCollection itemsList;
		
		public NewPageEventArgs(ExporterCollection itemsList)
		{
			this.itemsList = itemsList;
		}
		
		public ExporterCollection ItemsList {
			get { return itemsList; }
		}
	}
}
