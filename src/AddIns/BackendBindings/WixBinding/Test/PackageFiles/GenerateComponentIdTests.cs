// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;

namespace WixBinding.Tests.PackageFiles
{
	[TestFixture]
	public class GenerateComponentIdTests
	{
		WixComponentElement component;
		
		[SetUp]
		public void Init()
		{
			component = new WixComponentElement(new WixDocument());
		}
		
		[Test]
		public void SimpleFileName()
		{
			string fileName = "myapp.exe";
			Assert.AreEqual("MyappExe", component.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void NoExtension()
		{
			string fileName = "myapp";
			Assert.AreEqual("Myapp", component.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void OnlyExtension()
		{
			string fileName = ".bat";
			Assert.AreEqual("Bat", component.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void SingleCharacterFileName()
		{
			string fileName = "a.bat";
			Assert.AreEqual("ABat", component.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void EmptyString()
		{
			Assert.AreEqual(String.Empty, component.GenerateIdFromFileName(String.Empty));
		}
		
		[Test]
		public void Hyphen()
		{
			string fileName = "a-b.txt";
			Assert.AreEqual("A_bTxt", component.GenerateIdFromFileName(fileName));
		}
		
		[Test]
		public void DotsInFileName()
		{
			string fileName = "a.b.txt";
			Assert.AreEqual("AbTxt", component.GenerateIdFromFileName(fileName));
		}		
	}
}
