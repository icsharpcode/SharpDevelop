// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
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
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 2, 1);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextNoneDescriptionTest2()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 1, 7);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextNoneDescriptionTest3()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 3, 1);
			
			Assert.AreEqual(XamlContextDescription.None, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 1, 2);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest2()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 2, 11);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ContextInTagDescriptionTest()
		{
			string xaml = "<Grid>\n\t<CheckBox x:Name=\"asdf\" Background=\"Aqua\" Content=\"{x:Static Cursors.Arrow}\" />\n</Grid>";
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 2, 26);
			
			Assert.AreEqual(XamlContextDescription.InTag, context.Description);
		}
		
		[Test]
		public void ElementNameWithDotTest1()
		{
			string xaml = "<Grid>\n\t<Grid.ColumnDefinitions />\n</Grid>";
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 2, 12);
			
			Assert.AreEqual("Grid.ColumnDefinitions", context.ActiveElement.Name);
		}
		
		[Test]
		public void ContextAtTagDescriptionTest3()
		{
			string xaml = File.ReadAllText("Test4.xaml");
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 13, 8);
			
			Assert.AreEqual(XamlContextDescription.AtTag, context.Description);
		}
		
		[Test]
		public void ParentElementTestSimple1()
		{
			string xaml = File.ReadAllText("Test1.xaml");
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 8, 12);
			
			Assert.AreEqual("CheckBox", context.ActiveElement.Name);
			Assert.AreEqual("Grid", context.ParentElement.Name);
		}
		
		[Test]
		public void ParentElementTestSimple2()
		{
			string xaml = File.ReadAllText("Test4.xaml");
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 13, 8);
			
			Assert.AreEqual("Grid", context.ActiveElement.Name);
			Assert.AreEqual("Grid", context.ParentElement.Name);
		}

		[Test]
		public void RootElementTest()
		{
			string xaml = File.ReadAllText("Test1.xaml");
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 2, 9);
			
			Assert.AreEqual("Window", context.ActiveElement.Name);
			Assert.AreEqual(null, context.ParentElement);
		}
		
		[Test]
		public void IgnoredXmlnsTest1()
		{
			string xaml = File.ReadAllText("Test2.xaml");
			XamlContext context = CompletionDataHelper.ResolveContext(xaml, "", 11, 24);
			
			Assert.AreEqual(1, context.IgnoredXmlns.Count);
			Assert.AreEqual("d", context.IgnoredXmlns[0]);
		}
	}
}