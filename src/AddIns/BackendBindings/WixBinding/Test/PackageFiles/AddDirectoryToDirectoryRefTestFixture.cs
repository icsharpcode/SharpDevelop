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

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit.Framework;
using System;
using System.IO;
using System.Xml;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Adds a directory to a package with no directories.
	/// </summary>
	[TestFixture]
	public class AddDirectoryToDirectoryRefTestFixture : PackageFilesTestFixtureBase
	{
		XmlElement directoryElement;
		XmlElement childDirectoryElement;
		XmlElement firstSelectedElement;
		
		[SetUp]
		public void SetUpFixture()
		{
			base.InitFixture();
			editor.AddElement("Directory");
			directoryElement = (XmlElement)editor.Document.GetRootDirectoryRef().ChildNodes[0];
			firstSelectedElement = view.SelectedElement;
			editor.AddElement("Directory");
			childDirectoryElement = (XmlElement)directoryElement.ChildNodes[0];
		}
		
		[Test]
		public void IsDirty()
		{
			Assert.IsTrue(view.IsDirty);
		}
		
		[Test]
		public void DirectoryAdded()
		{
			Assert.IsNotNull(directoryElement);
			Assert.AreSame(directoryElement, view.ElementsAdded[0]);
		}
		
		[Test]
		public void DirectoryId()
		{
			Assert.AreEqual("NewDirectory", directoryElement.GetAttribute("Id"));
		}
			
		[Test]
		public void ChildDirectoryAdded()
		{
			Assert.IsNotNull(childDirectoryElement);
			Assert.AreSame(childDirectoryElement, view.ElementsAdded[1]);
		}
		
		[Test]
		public void FirstSelectedElementIsFirstDirectory()
		{
			Assert.AreSame(directoryElement, firstSelectedElement);
		}
		
		[Test]
		public void DirectoryAttributesDisplayed()
		{
			Assert.AreEqual(String.Empty, base.GetAttribute(view.Attributes, "FileSource").Value);
		}
		
		[Test]
		public void TwoElementsAddedToView()
		{
			Assert.AreEqual(2, view.ElementsAdded.Length);
		}
		
		protected override string GetWixXml()
		{
			return "<Wix xmlns=\"http://schemas.microsoft.com/wix/2006/wi\">\r\n" +
				"\t<Fragment>\r\n" +
				"\t\t<DirectoryRef Id=\"TARGETDIR\"/>\r\n" +
				"\t</Fragment>\r\n" +
				"</Wix>";
		}
	}
}
