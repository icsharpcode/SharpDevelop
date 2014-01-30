// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

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
