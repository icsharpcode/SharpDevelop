// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.WixBinding;
using NUnit;
using System;
using System.Collections;
using System.IO;
using System.Xml;
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
