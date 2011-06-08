// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	[TestFixture]
	public class ScriptingConsoleTests : ScriptingConsoleTestsBase
	{
		[Test]
		public void GetMaximumVisibleColumns_TextEditorMaximumVisibleColumnsnIsTen_ReturnsTen()
		{
			CreateConsole();
			FakeConsoleTextEditor.MaximumVisibleColumns = 10;
			
			int columns = TestableScriptingConsole.GetMaximumVisibleColumns();
			
			Assert.AreEqual(10, columns);
		}
	}
}
