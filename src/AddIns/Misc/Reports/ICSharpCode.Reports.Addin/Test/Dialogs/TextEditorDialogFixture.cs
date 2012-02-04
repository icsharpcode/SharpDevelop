// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Reports.Core;
using ICSharpCode.Reports.Core.Interfaces;
using NUnit.Framework;

namespace ICSharpCode.Reports.Addin.Test.Dialogs
{
	[TestFixture]
	public class TextEditorDialogFixture
	{
		[Test]
		public void ConstructorWithEmptyString()
		{
			IStringBasedEditorDialog ed = new DummyEditorDialog(String.Empty);
			Assert.AreEqual(ed.TextValue,String.Empty,"TextValue should be empty");
		}
		
		[Test]
		public void ConstructorWithText ()
		{
			string s = "myteststring";
			IStringBasedEditorDialog ed = new DummyEditorDialog(s);
			Assert.AreEqual (ed.TextValue,s);
		}
		
		[Test]
		public void tt ()
		{
			ReportModel m = ReportModel.Create ();
		}
	}
}
