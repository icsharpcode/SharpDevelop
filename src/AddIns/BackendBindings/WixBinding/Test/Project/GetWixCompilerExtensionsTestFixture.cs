// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that the WixProject's WixCompilerExtensions, WixLinkerExtensions and
	/// WixLibraryExtensions properties return ProjectItems.
	/// </summary>
	[TestFixture]
	public class GetWixCompilerExtensionsTestFixture
	{
		WixExtensionProjectItem compilerExtensionItem;
		WixExtensionProjectItem linkerExtensionItem;
		WixExtensionProjectItem libraryExtensionItem;
		
		[SetUp]
		public void SetUpFixture()
		{
			WixProject p = WixBindingTestsHelper.CreateEmptyWixProject();
			
			WixCompilerExtensionProjectItem compilerItem = new WixCompilerExtensionProjectItem(p);
			compilerItem.Include = "WixNetFxExtension";
			compilerItem.ClassName = "Microsoft.Tools.WindowsInstallerXml.Extensions.NetFxCompiler";
			p.Items.Add(compilerItem);
			
			WixLinkerExtensionProjectItem linkerItem = new WixLinkerExtensionProjectItem(p);
			linkerItem.Include = "LinkerExtension";
			linkerItem.ClassName = "LinkerExtension.ClassName";
			p.Items.Add(linkerItem);
			
			WixLibraryExtensionProjectItem libraryItem = new WixLibraryExtensionProjectItem(p);
			libraryItem.Include = "LibraryExtension";
			libraryItem.ClassName = "LibraryExtension.ClassName";
			p.Items.Add(libraryItem);
			
			compilerExtensionItem = p.WixCompilerExtensions[0];
			libraryExtensionItem = p.WixLibraryExtensions[0];
			linkerExtensionItem = p.WixLinkerExtensions[0];
		}
		
		[Test]
		public void WixCompilerExtensionItemInclude()
		{
			Assert.AreEqual("WixNetFxExtension", compilerExtensionItem.Include);
		}
		
		[Test]
		public void IsCompilerExtensionType()
		{
			Assert.IsInstanceOfType(typeof(WixCompilerExtensionProjectItem), compilerExtensionItem);
		}
		
		[Test]
		public void WixLinkerExtensionItemInclude()
		{
			Assert.AreEqual("LinkerExtension", linkerExtensionItem.Include);
		}
		
		[Test]
		public void IsLinkerExtensionType()
		{
			Assert.IsInstanceOfType(typeof(WixLinkerExtensionProjectItem), linkerExtensionItem);
		}

		[Test]
		public void WixLibraryExtensionItemInclude()
		{
			Assert.AreEqual("LibraryExtension", libraryExtensionItem.Include);
		}
		
		[Test]
		public void IsLibraryExtensionType()
		{
			Assert.IsInstanceOfType(typeof(WixLibraryExtensionProjectItem), libraryExtensionItem);
		}
		
		[Test]
		public void GetQualifiedName()
		{
			Assert.AreEqual("Microsoft.Tools.WindowsInstallerXml.Extensions.NetFxCompiler, WixNetFxExtension", compilerExtensionItem.QualifiedName);
		}
		
		[Test]
		public void ChangeQualifiedName()
		{
			compilerExtensionItem.QualifiedName = "class,include";
			Assert.AreEqual("include", compilerExtensionItem.Include);
			Assert.AreEqual("class", compilerExtensionItem.ClassName);
		}
		
		[Test]
		public void ChangeQualifiedNameWithNoInclude()
		{
			compilerExtensionItem.QualifiedName = "class,";
			Assert.AreEqual(String.Empty, compilerExtensionItem.Include);
			Assert.AreEqual("class", compilerExtensionItem.ClassName);
		}
		
		[Test]
		public void ChangeQualifiedNameWithNoComma()
		{
			compilerExtensionItem.QualifiedName = "class";
			Assert.AreEqual(String.Empty, compilerExtensionItem.Include);
			Assert.AreEqual("class", compilerExtensionItem.ClassName);
			Assert.AreEqual("class", compilerExtensionItem.QualifiedName);
		}
	}
}
