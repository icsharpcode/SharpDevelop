// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Project;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.Core;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Internal.Project.Collections;
using UI = WeifenLuo.WinFormsUI;

namespace ICSharpCode.SharpDevelop.AddIns.AssemblyScout
{
	internal class TempProject : IProject
	{
		public string ProjectType {
			get { return ""; }
		}
		
		public string BaseDirectory {
			get { return System.IO.Path.GetTempPath(); }
		}
		
		public bool IsDirty {
			get { return false; }
			set {}
		}
		
		public string Name {
			get { return "Temp"; }
			set {}
		}
		
		public string StandardNamespace {
			get { return "Temp"; }
			set {}
		}
		
		public string Description  {
			get { return ""; }
			set {}
		}
		
		public IConfiguration ActiveConfiguration {
			get { return null; }
			set {}
		}

		public IConfiguration GetConfiguration(int index) {
			return null;
		}
		
		public void AddConfiguration(IConfiguration c)
		{
		}
		
		public void RemoveConfiguration(IConfiguration c)
		{
		}
		
		public ConfigurationCollection Configurations
		{
			get { return null; }
		}
		
		public ProjectFileCollection ProjectFiles {
			get { return null; }
		}
		
		public ProjectReferenceCollection ProjectReferences {
			get { return new ProjectReferenceCollection(); }
		}
		
		public NewFileSearch NewFileSearch {
			get { return 0; }
			set {}
		}
		
		public bool EnableViewState {
			get { return false; }
			set {}
		}
		
		public string GetParseableFileContent(string fileContent)
		{ 
			return String.Empty;
		}
		public DeployInformation DeployInformation {
			get { return null; }
		}
		
		public bool IsCompileable(string fileName) { return false; }
		
		public void LoadProject(string fileName) { }
		
		public void SaveProject(string fileName) { }
		public void CopyReferencesToOutputPath(bool b) {}		
		public void CopyReferencesToPath(string destination, bool force){}
		public void CopyReferencesToPath(string destination, bool force, ArrayList alreadyCopiedReferences) {}
		public bool IsFileInProject(string fileName) { return false; }
		
		public IConfiguration CreateConfiguration(string name) { return null; }
		
		public IConfiguration CreateConfiguration() { return null; }
		public IConfiguration CloneConfiguration(IConfiguration configuration) { return null; }
		
		protected virtual void OnNameChanged(EventArgs e)
		{
			if (NameChanged != null) {
				NameChanged(this, e);
			}
		}
		
		public event EventHandler NameChanged;
		
		public void Dispose()
		{
			// nothing to do here
		}
	}
}
