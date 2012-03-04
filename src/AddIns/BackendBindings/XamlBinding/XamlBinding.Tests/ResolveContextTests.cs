// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.XmlEditor;
using NUnit.Framework;
using System.IO;

namespace ICSharpCode.XamlBinding.Tests
{
	[TestFixture]
	public class ResolveContextTests
	{
		[Test]
		public void ContextNoneDescriptionTest()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>\n".Length;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextNoneDescriptionTest2()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>".Length;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextNoneDescriptionTest3()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n".Length;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<G".Length;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest1()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" /> <\n</Grid>";
			int offset = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" /> <".Length;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest2()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>\n".Length + 10;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest4()
		{
			string xaml = "<Grid>\n\t<\n</Grid>";
			int offset = "<Grid>\n\t<".Length;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextInTagDescriptionTest()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			int offset = "<Grid>\n".Length + 26;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
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
			
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
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
			
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}

		
		[Test]
		public void ElementNameWithDotTest1()
		{
			string xaml = "<Grid>\n\t<Grid.ColumnDefinitions />\n</Grid>";
			int offset = "<Grid>\n".Length + 12;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual("Grid.ColumnDefinitions", context.ActiveElement.Name);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest3()
		{
			string xaml = File.ReadAllText("Test4.xaml");
			int offset = 413;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextInMarkupExtensionTest()
		{
			string xaml = "<Test attr=\"{Test}\" />";
			int offset = "<Test attr=\"{Te".Length;
			
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.InMarkupExtension, context.Description);
		}
		
		[Test]
		public void ContextInAttributeValueTest()
		{
			string xaml = "<Test attr=\"Test\" />";
			int offset = "<Test attr=\"Te".Length;
			
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
		
		[Test]
		public void ContextInMarkupExtensionTest2()
		{
			string xaml = "<Test attr=\"{}{Test}\" />";
			int offset = "<Test attr=\"{}{Te".Length;
			
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
		
		[Test]
		public void ContextInAttributeValueTest2()
		{
			string xaml = "<Test attr=\"Test />";
			int offset = "<Test attr=\"Te".Length;
			
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
		
		[Test]
		public void ContextInAttributeValueTest3()
		{
			string xaml = "<Test attr=\"Test />";
			int offset = "<Test attr=\"".Length;
			
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(XamlContextDescription.InAttributeValue, context.Description);
		}
		
		[Test]
		public void ParentElementTestSimple1()
		{
			string xaml = File.ReadAllText("Test1.xaml");
			int offset = 272;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual("CheckBox", context.ActiveElement.Name);
			Assert.AreEqual("Grid", context.ParentElement.Name);
		}
		
		[Test]
		public void ParentElementTestSimple2()
		{
			string xaml = File.ReadAllText("Test4.xaml");
			int offset = 413;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual("Grid", context.ActiveElement.Name);
			Assert.AreEqual("Grid", context.ParentElement.Name);
		}

		[Test]
		public void RootElementTest()
		{
			string xaml = File.ReadAllText("Test1.xaml");
			int offset = 31;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual("Window", context.ActiveElement.Name);
			Assert.AreEqual(null, context.ParentElement);
		}
		
		[Test]
		public void IgnoredXmlnsTest1()
		{
			string xaml = File.ReadAllText("Test2.xaml");
			int offset = 447;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
			Assert.AreEqual(1, context.IgnoredXmlns.Count);
			Assert.AreEqual("d", context.IgnoredXmlns[0]);
		}
		
		[Test]
		public void AncestorDetectionTest1()
		{
			string xaml = File.ReadAllText("Test5.xaml");
			int offset = 881;
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", offset);
			
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
	}
}
