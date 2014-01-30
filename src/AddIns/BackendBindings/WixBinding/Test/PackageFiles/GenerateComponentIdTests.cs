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
