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

namespace WixBinding.Tests.Project
{
	/// <summary>
	/// Tests that the project's OutputType is correctly mapped to the file 
	/// extension of the binary output file that will be created.
	/// </summary>
	[TestFixture]
	public class WixProjectOutputTypeFileExtensionTests
	{
		[Test]
		public void MsiFile()
		{
			Assert.AreEqual(".msi", WixProject.GetInstallerExtension("package"));
		}
	
		[Test]
		public void MsmFile()
		{
			Assert.AreEqual(".msm", WixProject.GetInstallerExtension("module"));
		}
		
		[Test]
		public void WixLib()
		{
			Assert.AreEqual(".wixlib", WixProject.GetInstallerExtension("library"));
		}
		
		[Test]
		public void MsmFileUppercase()
		{
			Assert.AreEqual(".msm", WixProject.GetInstallerExtension("MODULE"));
		}
		
		[Test]
		public void UnknownOutputType()
		{
			Assert.AreEqual(".msi", WixProject.GetInstallerExtension("unknown"));
		}
	}
}
