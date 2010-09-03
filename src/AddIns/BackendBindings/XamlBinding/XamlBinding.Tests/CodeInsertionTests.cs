// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using ICSharpCode.SharpDevelop.Editor.CodeCompletion;
using NUnit.Framework;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	[RequiresSTA]
	public class CodeInsertionTests : TextEditorBasedTests
	{
		#region TextInsertionTests		
		[Test]
		public void CtrlSpaceClosingAttributeValueWithEqualsInsertionTest()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Button ";
			string fileFooter = @"
	</Grid>
	</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	
			              	TestTextInsert(fileHeader, fileFooter, '=', list, list.Items.First(i => i.Text == "Content"), "Content=\"\"", "Content=\"".Length);
			              });
		}	
		
		[Test]
		public void CtrlSpaceInsertionTest()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		";
			string fileFooter = @"
	</Grid>
	</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	
			              	TestTextInsert(fileHeader, fileFooter, '\n', list, list.Items.First(i => i.Text == "!--"), "<!--  -->", "<!-- ".Length);
			              	TestTextInsert(fileHeader, fileFooter, '\n', list, list.Items.First(i => i.Text == "Button"), "<Button", "<Button".Length);
			              	TestTextInsert(fileHeader, fileFooter, '\n', list, list.Items.First(i => i.Text == "/Grid"), "</Grid>", "</Grid>".Length);
			              });
		}
		
		[Test]
		public void CtrlSpaceClosingTagWithGreaterThanInsertionTest()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		";
			string fileFooter = @"
	</Grid>
	</Window>";
			
			TestCtrlSpace(fileHeader, fileFooter, true,
			              list => {
			              	Assert.AreEqual(0, list.PreselectionLength);
			              	Assert.IsNull(list.SuggestedItem);
			              	Assert.IsTrue(list.Items.Any());
			              	
			              	TestTextInsert(fileHeader, fileFooter, '>', list, list.Items.First(i => i.Text == "/Grid"), "</Grid>", "</Grid>".Length);
			              });
		}
		#endregion
	}
}
