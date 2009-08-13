// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Dom;
using System;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	//[TestFixture]
	public class DependencyPropertiesTests : TextEditorBasedTests
	{
		[Test]
		[Ignore]
		public void NormalDependencyPropertiesTest()
		{
			string fileContent = @"<Window x:Class='XamlTest.Window1'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	Title='XamlTest' Height='300' Width='300'>
	<Grid>
		<Button />
	</Grid>
</Window>";
			
			int line = 6;
			int column = 11;
			
			this.textEditor.Document.Text = fileContent;
			this.textEditor.Caret.Line = line;
			this.textEditor.Caret.Column = column;
			
			XamlCompletionContext context = CompletionDataHelper.ResolveCompletionContext(textEditor, ' ');
			
			var cu = context.ParseInformation.BestCompilationUnit as XamlCompilationUnit;
			
			if (cu == null)
				Assert.Fail("cu invalid");
			
			string xmlns = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
			
			IReturnType type = cu.CreateType(xmlns, "Grid");
		}
	}
}
