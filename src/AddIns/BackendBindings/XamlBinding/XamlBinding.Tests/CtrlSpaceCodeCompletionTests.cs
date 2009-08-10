// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>


using System;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class CodeCompletionTests
	{
		MockTextEditor textEditor;
		
		string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		";
		string fileFooter = @"
	</Grid>
</Window>";
		
		[SetUp]
		[STAThread]
		public void SetupTest()
		{
			this.textEditor = new MockTextEditor();
		}
		
		[Test]
		[STAThread]
		public void CtrlSpaceTest1()
		{
			this.textEditor.Document.Text = fileHeader + fileFooter;
			this.textEditor.Caret.Offset = fileHeader.Length;
			this.textEditor.CreateParseInformation();
						
			bool invoked = XamlCodeCompletionBinding.Instance.CtrlSpace(textEditor);
			
			Assert.IsTrue(invoked);
			
			ICompletionItemList list = this.textEditor.LastCompletionItemList;
			
			Assert.AreEqual(0, list.PreselectionLength);
			Assert.IsNull(list.SuggestedItem);
			Assert.IsTrue(list.Items.Any());
			var items = list.Items.Select(item => item.Text).ToArray();
			Assert.Contains("Window", items);
			Assert.Contains("x:TypeExtension", items);
			Assert.Contains("!--", items);
			Assert.Contains("/Grid", items);
		}
		
//		[Test]
//		[STAThread]
//		public void CtrlSpaceTest2()
//		{
//			this.textEditor.Document.Text = fileHeader + fileFooter;
//			this.textEditor.Caret.Offset = fileHeader.Length;
//			this.textEditor.CreateParseInformation();
//						
//			XamlCodeCompletionBinding.Instance.CtrlSpace(textEditor);
//			
//			ICompletionItemList list = this.textEditor.LastCompletionItemList;
//			
//			Assert.AreEqual(0, list.PreselectionLength);
//			Assert.IsNull(list.SuggestedItem);
//			Assert.IsTrue(list.Items.Any());
//			var items = list.Items.Select(item => item.Text).ToArray();
//			Assert.Contains("Window", items);
//			Assert.Contains("x:TypeExtension", items);
//			Assert.Contains("!--", items);
//			Assert.Contains("/Grid", items);
//		}
	}
}
