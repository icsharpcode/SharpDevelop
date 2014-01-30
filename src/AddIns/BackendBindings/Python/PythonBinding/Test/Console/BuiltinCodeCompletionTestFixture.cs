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
using System.Collections.Generic;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting;
using ICSharpCode.Scripting.Tests.Console;
using ICSharpCode.Scripting.Tests.Utils;
using Microsoft.Scripting.Hosting;
using NUnit.Framework;

namespace PythonBinding.Tests.Console
{
	/// <summary>
	/// Tests the code completion when the user has typed in __builtins__.
	/// </summary>
	[TestFixture]
	public class BuiltinCodeCompletionTestFixture
	{
		ICompletionData[] completionItems;
		ICompletionData[] expectedCompletionItems;
		ScriptingConsoleCompletionDataProvider provider;
		MockMemberProvider memberProvider;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{			
			TextEditor textEditorControl = new TextEditor();
			textEditorControl.Text = ">>> __builtins__";
			ScriptingConsoleTextEditor textEditor = new ScriptingConsoleTextEditor(textEditorControl);
				
			memberProvider = new MockMemberProvider();
			memberProvider.SetMemberNames(new string[] {"a", "b", "c"});
			expectedCompletionItems = CreateCompletionItems(memberProvider.GetMemberNames("__builtins__"));
			
			provider = new ScriptingConsoleCompletionDataProvider(memberProvider);
			completionItems = provider.GenerateCompletionData(textEditor);
		}
		
		[Test]
		public void MoreThanOneCompletionItem()
		{
			Assert.IsTrue(completionItems.Length > 0);
		}
		
		[Test]
		public void ExpectedCompletionItems()
		{
			for (int i = 0; i < expectedCompletionItems.Length; ++i) {
				Assert.AreEqual(expectedCompletionItems[i].Text, completionItems[i].Text);
			}
		}
		
		[Test]
		public void ExpectedCompletionItemsCountEqualsActualCompletionItemsCount()
		{
			Assert.AreEqual(expectedCompletionItems.Length, completionItems.Length);
		}
		
		[Test]
		public void BuiltinsPassedToMemberProvider()
		{
			Assert.AreEqual("__builtins__", memberProvider.GetMemberNamesParameter);
		}
		
		ICompletionData[] CreateCompletionItems(IList<string> memberNames)
		{
			List<ScriptingConsoleCompletionData> items = new List<ScriptingConsoleCompletionData>();
			foreach (string memberName in memberNames) {
				items.Add(new ScriptingConsoleCompletionData(memberName));
			}
			return items.ToArray();
		}
	}
}
