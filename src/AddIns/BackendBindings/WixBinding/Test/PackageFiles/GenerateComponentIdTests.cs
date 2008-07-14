// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class GenerateComponentIdTests
	{
		[Test]
		public void SimpleFileName()
		{
			string fileName = "myapp.exe";
			Assert.AreEqual("MyappExe", WixComponentElement.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void NoExtension()
		{
			string fileName = "myapp";
			Assert.AreEqual("Myapp", WixComponentElement.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void OnlyExtension()
		{
			string fileName = ".bat";
			Assert.AreEqual("Bat", WixComponentElement.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void SingleCharacterFileName()
		{
			string fileName = "a.bat";
			Assert.AreEqual("ABat", WixComponentElement.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.AreEqual(String.Empty, WixComponentElement.GenerateIdFromFileName(String.Empty));
		}
		
		[Test]
		public void Hyphen()
		{
			string fileName = "a-b.txt";
			Assert.AreEqual("A_bTxt", WixComponentElement.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void DotsInFileName()
		{
			string fileName = "a.b.txt";
			Assert.AreEqual("AbTxt", WixComponentElement.GenerateIdFromFileName(fileName));
		}		
	}
}
