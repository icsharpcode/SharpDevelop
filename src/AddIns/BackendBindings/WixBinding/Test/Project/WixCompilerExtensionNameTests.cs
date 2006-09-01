// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.Project
{
	[TestFixture]
	public class WixCompilerExtensionNameTests
	{
		[Test]
		public void DisplayName()
		{
			WixCompilerExtensionName name = WixCompilerExtensionName.CreateFromString("ClassName, AssemblyName|DisplayName");
			Assert.AreEqual("DisplayName", name.DisplayName);
			Assert.AreEqual("ClassName", name.ClassName);
			Assert.AreEqual("AssemblyName", name.AssemblyName);
		}
		
		[Test]
		public void NoDisplayName()
		{
			WixCompilerExtensionName name = WixCompilerExtensionName.CreateFromString("ClassName, AssemblyName");
			Assert.AreEqual(String.Empty, name.DisplayName);
			Assert.AreEqual("ClassName", name.ClassName);
			Assert.AreEqual("AssemblyName", name.AssemblyName);
		}
		
		[Test]
		public void ExtraSpaces()
		{
			WixCompilerExtensionName name = new WixCompilerExtensionName(" ClassName , AssemblyName ");
			Assert.AreEqual("ClassName", name.ClassName);
			Assert.AreEqual("AssemblyName", name.AssemblyName);
		}
		
		[Test]
		public void Equals()
		{
			WixCompilerExtensionName name1 = new WixCompilerExtensionName("foo, bar");
			WixCompilerExtensionName name2 = new WixCompilerExtensionName("foo, bar");
			Assert.IsTrue(name1.Equals(name2));
		}
		
		[Test]
		public void NotEqualsDifferentAssemblyName()
		{
			WixCompilerExtensionName name1 = new WixCompilerExtensionName("foo");
			WixCompilerExtensionName name2 = new WixCompilerExtensionName("foo, bar");
			Assert.IsFalse(name1.Equals(name2));
		}
		
		[Test]
		public void NotEqualsDifferentClassName()
		{
			WixCompilerExtensionName name1 = new WixCompilerExtensionName("foo, bar");
			WixCompilerExtensionName name2 = new WixCompilerExtensionName("class, bar");
			Assert.IsFalse(name1.Equals(name2));
		}
	}
}
