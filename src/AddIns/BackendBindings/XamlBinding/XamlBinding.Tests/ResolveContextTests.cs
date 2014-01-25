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
using System.Linq;
using System.Threading;
using ICSharpCode.Core;
using ICSharpCode.NRefactory.CSharp;
using ICSharpCode.NRefactory.Editor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Workbench;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System.IO;
using Rhino.Mocks;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	[RequiresSTA]
	public class ResolveContextTests : TextEditorBasedTests
	{
		void SetUpWithCode(FileName fileName, ITextSource textSource)
		{
			IProject project = MockRepository.GenerateStrictMock<IProject>();
			var parseInfo = new XamlParser() { TaskListTokens = TaskListTokens }.Parse(fileName, textSource, true, project, CancellationToken.None);
			
			var pc = new CSharpProjectContent().AddOrUpdateFiles(parseInfo.UnresolvedFile);
			pc = pc.AddAssemblyReferences(new[] { Corlib, PresentationCore, PresentationFramework, SystemXaml });
			var compilation = pc.CreateCompilation();
			SD.Services.AddService(typeof(IParserService), MockRepository.GenerateStrictMock<IParserService>());
			
			SD.ParserService.Stub(p => p.GetCachedParseInformation(fileName)).Return(parseInfo);
			SD.ParserService.Stub(p => p.GetCompilation(project)).Return(compilation);
			SD.ParserService.Stub(p => p.GetCompilationForFile(fileName)).Return(compilation);
			SD.ParserService.Stub(p => p.Parse(fileName, textSource)).WhenCalled(
				i => {
					i.ReturnValue = new XamlParser() { TaskListTokens = TaskListTokens }.Parse(fileName, textSource, true, project, CancellationToken.None);
				}).Return(parseInfo); // fake Return to make it work
			SD.Services.AddService(typeof(IFileService), MockRepository.GenerateStrictMock<IFileService>());
			IViewContent view = MockRepository.GenerateStrictMock<IViewContent>();
			SD.FileService.Stub(f => f.OpenFile(fileName, false)).Return(view);
		}
		
		XamlContext TestContext(string xaml, int offset)
		{
			var fileName = new FileName("test.xaml");
			var textSource = new StringTextSource(xaml);
			SetUpWithCode(fileName, textSource);
			return XamlContextResolver.ResolveContext(fileName, textSource, offset);
		}
		
		[Test]
		public void ContextNoneDescriptionTest()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>\n".Length;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextNoneDescriptionTest2()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>".Length;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextNoneDescriptionTest3()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n".Length;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<G".Length;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest1()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" /> <\n</Grid>";
			int offset = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" /> <".Length;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest2()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>\n".Length + 10;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest4()
		{
			string xaml = "<Grid>\n\t<\n</Grid>";
			int offset = "<Grid>\n\t<".Length;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextInTagDescriptionTest()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>\n".Length + 26;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.InTag, context.Description);
		}
		
		[Test]
		public void ContextInTagDescriptionTest2()
		{
			string xaml = @"<Window x:Class='Vokabeltrainer.Window1'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	Title=''>
	<Grid>
		<StackPanel>
			<RadioButton
		</StackPanel>
	</Grid>
</Window>";
			
			int offset = @"<Window x:Class='Vokabeltrainer.Window1'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	Title=''>
	<Grid>
		<StackPanel>
			<RadioButton ".Length;
			
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.InTag, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest5()
		{
			string xaml = @"<Window x:Class='Vokabeltrainer.Window1'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	Title=''>
	<Grid>
		<StackPanel>
			<RadioButton
		</StackPanel>
	</Grid>
</Window>";
			
			int offset = @"<Window x:Class='Vokabeltrainer.Window1'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
	Title=''>
	<Grid>
		<StackPanel>
			<RadioButton
		</Stack".Length;
			
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}

		
		[Test]
		public void ElementNameWithDotTest1()
		{
			string xaml = "<Grid>\n\t<Grid.ColumnDefinitions />\n</Grid>";
			int offset = "<Grid>\n".Length + 12;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual("Grid.ColumnDefinitions", context.ActiveElement.Name);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest3()
		{
			string xaml = File.ReadAllText("Test4.xaml");
			int offset = 413;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextInMarkupExtensionTest()
		{
			string xaml = "<Test attr=\"{Test}\" />";
			int offset = "<Test attr=\"{Te".Length;
			
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.InMarkupExtension, context.Description);
		}
		
		[Test]
		public void ContextInAttributeValueTest()
		{
			string xaml = "<Test attr=\"Test\" />";
			int offset = "<Test attr=\"Te".Length;
			
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
		
		[Test]
		public void ContextInMarkupExtensionTest2()
		{
			string xaml = "<Test attr=\"{}{Test}\" />";
			int offset = "<Test attr=\"{}{Te".Length;
			
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
		
		[Test]
		public void ContextInAttributeValueTest2()
		{
			string xaml = "<Test attr=\"Test />";
			int offset = "<Test attr=\"Te".Length;
			
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
		
		[Test]
		public void ContextInAttributeValueTest3()
		{
			string xaml = "<Test attr=\"Test />";
			int offset = "<Test attr=\"".Length;
			
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
		
		[Test]
		public void ParentElementTestSimple1()
		{
			string xaml = File.ReadAllText("Test1.xaml");
			int offset = 272;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual("CheckBox", context.ActiveElement.Name);
			Assert.AreEqual("Grid", context.ParentElement.Name);
		}
		
		[Test]
		public void ParentElementTestSimple2()
		{
			string xaml = File.ReadAllText("Test4.xaml");
			int offset = 413;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual("Grid", context.ActiveElement.Name);
			Assert.AreEqual("Grid", context.ParentElement.Name);
		}

		[Test]
		public void RootElementTest()
		{
			string xaml = File.ReadAllText("Test1.xaml");
			int offset = 31;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual("Window", context.ActiveElement.Name);
			Assert.AreEqual(null, context.ParentElement);
		}
		
		[Test]
		public void IgnoredXmlnsTest1()
		{
			string xaml = File.ReadAllText("Test2.xaml");
			int offset = 447;
			XamlContext context = TestContext(xaml, offset);
			
			Assert.AreEqual(1, context.IgnoredXmlns.Count);
			Assert.AreEqual("d", context.IgnoredXmlns[0]);
		}
		
		[Test]
		public void AncestorDetectionTest1()
		{
			string xaml = File.ReadAllText("Test5.xaml");
			int offset = 881;
			XamlContext context = TestContext(xaml, offset);
			
			string[] ancestors = new string[] {
				"DoubleAnimation", "Storyboard",
				"BeginStoryboard", "EventTrigger",
				"Button.Triggers", "Button",
				"Grid", "Window"
			};
			
			Assert.AreEqual("DoubleAnimation", context.ActiveElement.Name);
			Assert.AreEqual("Storyboard", context.ParentElement.Name);
			Assert.AreEqual(8, context.Ancestors.Count);
			Assert.AreEqual(ancestors, context.Ancestors.Select(item => item.Name).ToArray());
		}
		
		[Test]
		public void InValueTestWithOpenValue()
		{
			string fileHeader = @"<Window x:Class='ICSharpCode.XamlBinding.Tests.CompletionTestsBase'
	xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
	xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
	<Grid>
		<Grid.ColumnDefinitions>
			<ColumnDefinition Width='";
			string fileFooter = @"
		<Button AllowDrop='True' Grid.Row='0' />
	</Grid>
</Window>";
			
			XamlContext context = TestContext(fileHeader + fileFooter, fileHeader.Length);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
	}
}
