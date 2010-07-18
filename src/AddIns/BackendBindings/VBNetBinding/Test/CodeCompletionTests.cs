// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="siegfriedpammer@gmail.com"/>
//     <version>$Revision: 6077 $</version>
// </file>

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
			TestKeyPress("", "", 'o', CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion,
			             list => {
			             	Assert.IsTrue(list.Items.Any());
			             	Assert.IsTrue(list.Items.All(item => item.Image == ClassBrowserIconService.Keyword));
			             	ContainsAll(list.Items.Select(item => item.Text).ToArray(),
			             	            "Class", "Delegate", "Friend", "Imports", "Module",
			             	            "Namespace", "Option", "Private", "Protected", "Public",
			             	            "Shadows", "Structure", "Interface", "Enum");
			             }
			            );
		}
		
		[Test]
		public void TestOptions()
		{
			TestKeyPress("Option", "\n", ' ', CodeCompletionKeyPressResult.EatKey,
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
			TestKeyPress("Option Compare", "\n", ' ', CodeCompletionKeyPressResult.EatKey,
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
				TestKeyPress(string.Format("Option {0} ", option), "\n", 'o', CodeCompletionKeyPressResult.CompletedIncludeKeyInCompletion,
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
			Assert.AreEqual(expected.Length, items.Count);
			
			foreach (string element in expected)
				Assert.Contains(element, items);
		}
	}
}
