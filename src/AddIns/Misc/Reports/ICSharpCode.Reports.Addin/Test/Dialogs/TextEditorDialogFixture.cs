/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Peter
 * Datum: 30.10.2008
 * Zeit: 20:00
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using NUnit.Framework;
using ICSharpCode.Reports.Core;

namespace ICSharpCode.Reports.Addin.Test.Dialogs
{
	[TestFixture]
	public class TextEditorDialogFixture
	{
		[Test]
		public void ConstructorWithEmptyString()
		{
			ITextEditorDialog ed = new DummyEditorDialog(String.Empty);
			Assert.AreEqual(ed.TextValue,String.Empty,"TextValue should be empty");
		}
		
		[Test]
		public void ConstructorWithText ()
		{
			string s = "myteststring";
			ITextEditorDialog ed = new DummyEditorDialog(s);
			Assert.AreEqual (ed.TextValue,s);
		}
		
		[Test]
		public void tt ()
		{
			ReportModel m = ReportModel.Create ();
		}
	}
}
