// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows.Input;

using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Utils;
using NUnit.Framework;

namespace ICSharpCode.Scripting.Tests.Console
{
	/// <summary>
	/// Tests the ScriptingConsole's GetUnreadLines method.
	/// </summary>
	[TestFixture]
	public class ScriptingConsoleUnreadLinesTests : ScriptingConsoleTestsBase
	{
		[SetUp]
		public void Init()
		{
			base.CreateConsole();
		}
		
		[Test]
		public void NoUnreadLinesAtStart()
		{
			int length = TestableScriptingConsole.GetUnreadLines().Length;
			Assert.AreEqual(0, length);
		}
	
		[Test]
		public void HasUnreadLines()
		{
			Assert.IsFalse(TestableScriptingConsole.IsLineAvailable);
		}
		
		[Test]
		public void AddOneLine()
		{
			FakeConsoleTextEditor.RaisePreviewKeyDownEvent(System.Windows.Input.Key.A);
			FakeConsoleTextEditor.RaisePreviewKeyDownEventForDialogKey(System.Windows.Input.Key.Enter);
			
			string[] lines = TestableScriptingConsole.GetUnreadLines();
			string[] expectedLines = new string[] {"A"};
			
			Assert.AreEqual(expectedLines, lines);
			Assert.IsTrue(TestableScriptingConsole.IsLineAvailable);
		}
	}
}
