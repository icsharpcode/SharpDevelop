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
