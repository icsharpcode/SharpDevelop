// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using Microsoft.Scripting.Hosting;

using ICSharpCode.PythonBinding;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Gui.CompletionWindow;
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
		PythonConsoleCompletionDataProvider provider;
		MockMemberProvider memberProvider;
		
		[TestFixtureSetUp]
		public void SetUpFixture()
		{			
			using (TextEditorControl textEditorControl = new TextEditorControl()) {
				textEditorControl.Text = ">>> __builtins__";
				TextEditor textEditor = new TextEditor(textEditorControl);
					
				memberProvider = new MockMemberProvider();
				memberProvider.SetMemberNames(new string[] {"a", "b", "c"});
				expectedCompletionItems = CreateCompletionItems(memberProvider.GetMemberNames("__builtins__"));
		
				provider = new PythonConsoleCompletionDataProvider(memberProvider);
				completionItems = provider.GenerateCompletionData(String.Empty, textEditorControl.ActiveTextAreaControl.TextArea, '.');
	
			}
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
		public void PreSelectionIsNull()
		{
			Assert.IsNull(provider.PreSelection);
		}
		
		[Test]
		public void DefaultIndexIsZero()
		{
			Assert.AreEqual(0, provider.DefaultIndex);
		}
		
		[Test]
		public void BuiltinsPassedToMemberProvider()
		{
			Assert.AreEqual("__builtins__", memberProvider.GetMemberNamesParameter);
		}
		
		ICompletionData[] CreateCompletionItems(IList<string> memberNames)
		{
			List<DefaultCompletionData> items = new List<DefaultCompletionData>();
			foreach (string memberName in memberNames) {
				items.Add(new DefaultCompletionData(memberName, String.Empty, 0));
			}
			return items.ToArray();
		}
	}
}
