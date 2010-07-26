/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 30.10.2008
 * Zeit: 19:54
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.Windows.Forms;
using NUnit.Framework;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin.Test.Dialogs
{
	
	public class DummyEditorDialog:ITextEditorDialog
	{
		string textValue;
		
		public DummyEditorDialog(string textValue)
		{
			this.textValue = textValue;
		}
		
		
		public string TextValue {
			get {
				return this.textValue;
			}
		}
		
		public System.Windows.Forms.DialogResult ShowDialog()
		{
			return DialogResult.OK;
		}
		
	}
}
