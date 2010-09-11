// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
