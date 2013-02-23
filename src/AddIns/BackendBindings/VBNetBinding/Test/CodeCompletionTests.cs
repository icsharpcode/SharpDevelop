// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using NUnit.Framework;

namespace ICSharpCode.VBNetBinding.Tests
{
	[TestFixture]
	public class CodeCompletionTests : TextEditorBasedTests
	{
		[Test]
		public void TestEmptyFile()
		{
			TestKeyPress("|", 'o', CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion,
			             list => {
			             	Assert.IsTrue(list.Items.Any());
			             	Assert.IsTrue(list.Items.All(item => item.Image == ClassBrowserIconService.Keyword));
			             	ContainsAll(list.Items.Select(item => item.Text).ToArray(),
			             	            "Class", "Delegate", "Friend", "Imports", "Module",
			             	            "Namespace", "Option", "Private", "Protected", "Public",
			             	            "Shadows", "Structure", "Interface", "Enum", "Partial", "NotInheritable");
			             }
			            );
		}
		
		[Test]
		public void TestOptions()
		{
			TestKeyPress("Option|\n", ' ', CodeCompletionKeyPressResult.EatKey,
			             list => {
			             	Assert.IsTrue(list.Items.Any());
			             	Assert.IsTrue(list.Items.All(item => item.Image == ClassBrowserIconService.Keyword));
			             	ContainsAll(list.Items.Select(item => item.Text).ToArray(),
			             	            "Explicit", "Infer", "Compare", "Strict");
			             }
			            );
		}
		
		[Test]
		public void TestOptionCompare()
		{
			TestKeyPress("Option Compare|\n", ' ', CodeCompletionKeyPressResult.EatKey,
			             list => {
			             	Assert.IsTrue(list.Items.Any());
			             	Assert.IsTrue(list.Items.All(item => item.Image == ClassBrowserIconService.Keyword));
			             	ContainsAll(list.Items.Select(item => item.Text).ToArray(),
			             	            "Text", "Binary");
			             }
			            );
		}
		
		[Test]
		public void TestOnOffOptions()
		{
			foreach (string option in new[] { "Infer", "Strict", "Explicit" }) {
				TestKeyPress(string.Format("Option {0} |\n", option), 'o', CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion,
				             list => {
				             	Assert.IsTrue(list.Items.Any());
				             	Assert.IsTrue(list.Items.All(item => item.Image == ClassBrowserIconService.Keyword));
				             	ContainsAll(list.Items.Select(item => item.Text).ToArray(),
				             	            "On", "Off");
				             }
				            );
			}
		}
		
		void ContainsAll(ICollection items, params string[] expected)
		{
//			Assert.AreEqual(expected.Length, items.Count);
			
			foreach (string element in expected)
				Assert.Contains(element, items);
		}
		
		[Test]
		public void TestDotAfterDigit()
		{
			string text = @"Module Test
	Public Function fn() As Double
      Dim f As Double
      f = 1|
   End Function
End Module";
			
			TestKeyPress(text, '.', CodeCompletionKeyPressResult.None, list => Assert.IsFalse(list.Items.Any()));
		}
		
		[Test]
		public void TestDotAfterIdentifier()
		{
			string text = @"Module Test
	Public Function fn(x As Double) As Double
	      Dim f As Double
	      f = x|
   End Function
End Module";
			
			TestKeyPress(text, '.', CodeCompletionKeyPressResult.Completed, list => Assert.IsTrue(list.Items.Any()));
		}
		
		[Test]
		public void TestWithBlockCtrlSpace()
		{
			string text = @"Module Test
	Sub Test2(a As Exception)
		With a
			|
		End With
	End Sub
End Module";
			
			// TODO seems to be wrong!
			
			TestCtrlSpace(text, true, list => Assert.IsFalse(list.Items.Any(i => i.Text == "InnerException")));
		}
		
		[Test]
		public void TestLocalVariablesAvailableAtEndOfForLoop()
		{
			string text = @"Module Test
	Public Sub f()
		Dim cheeses = { ""cheddar"", ""brie"", ""edam"" }
		For Each cheese As String In cheeses
			Dim gouda = ""is tasty""
			|
		Next
   End Sub
End Module";
			
			TestKeyPress(text, 'g', CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion, list => Assert.IsTrue(list.Items.Any(i => i.Text == "gouda")));
		}
	}
}
