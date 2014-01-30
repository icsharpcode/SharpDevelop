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
using System.Collections;
using System.IO;
using System.Xml;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit;
using WixBinding.Tests.Utils;

namespace WixBinding.Tests.PackageFiles
{
	/// <summary>
	/// Base class for package files editor tests. Creates a Wix project containing
	/// one setup.wxs file, creates the WixPackageEditor and calls its ShowPackageFiles
	/// method.
	/// </summary>
	public class PackageFilesTestFixtureBase : ITextFileReader, IWixDocumentWriter, IDirectoryReader
	{
		protected WixPackageFilesEditor editor;
		protected MockWixPackageFilesView view;
		protected WixProject project;

		public void InitFixture()
		{
			SD.InitializeForUnitTests();
			project = WixBindingTestsHelper.CreateEmptyWixProject();
			project.Name = "MySetup";
			FileProjectItem item = new FileProjectItem(project, ItemType.Compile);
			item.Include = "Setup.wxs";
			ProjectService.AddProjectItem(project, item);
			view = new MockWixPackageFilesView();
			editor = new WixPackageFilesEditor(view, this, this, this);
			editor.ShowFiles(project);
		}
		
		public virtual TextReader Create(string fileName)
		{
			return new StringReader(GetWixXml());
		}
		
		public virtual void Write(WixDocument document)
		{
		}
		
		protected virtual string GetWixXml()
		{
			return String.Empty;
		}
		
		protected WixXmlAttribute GetAttribute(IList attributes, string name)
		{
			foreach (WixXmlAttribute attribute in attributes) {
				if (attribute.Name == name) {
					return attribute;
				}
			}
			return null;
		}
	
		public virtual string[] GetFiles(string path)
		{
			return new string[0];
		}
		
		public virtual string[] GetDirectories(string path)
		{
			return new string[0];
		}
		
		public bool DirectoryExists(string path)
		{
			return true;
		}
	}
}
