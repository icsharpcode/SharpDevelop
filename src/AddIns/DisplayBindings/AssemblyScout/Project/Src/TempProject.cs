// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Georg Brandl" email="g.brandl@gmx.net"/>
//     <version value="$version"/>
// </file>

using System;
using System.Drawing;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using System.Diagnostics;
using System.CodeDom.Compiler;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui;
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
		
		public List<ProjectItem> Items {
			get {
				return null;
			}
		}
		
		public bool IsDirty {
			get {
				return false;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string Language {
			get {
				return String.Empty;
			}
		}
		
		public LanguageProperties LanguageProperties {
			get {
				return null;
			}
		}
		
		public IAmbience Ambience {
			get {
				return null;
			}
		}
		
		public string FileName {
			get {
				return String.Empty;
			}
		}
		
		public string Directory {
			get {
				return String.Empty;
			}
		}
		
		public string Configuration {
			get {
				return String.Empty;
			}
		}
		
		public string Platform {
			get {
				return String.Empty;
			}
		}
		
		public string OutputAssemblyFullPath {
			get {
				return String.Empty;
			}
		}
		
		public OutputType OutputType {
			get {
				return OutputType.Library;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string RootNamespace {
			get {
				return String.Empty;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string AppDesignerFolder {
			get {
				return String.Empty;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool IsStartable {
			get {
				return false;
			}
		}
		
		public ISolutionFolderContainer Parent {
			get {
				return null;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string TypeGuid {
			get {
				return String.Empty;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string IdGuid {
			get {
				return String.Empty;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public string Location {
			get {
				return String.Empty;
			}
			set {
				throw new NotImplementedException();
			}
		}
		
		public bool CanCompile(string fileName) {
			throw new NotImplementedException();
		}
		
		public void Save() {
			throw new NotImplementedException();
		}
		
		public void Save(string fileName) {
			throw new NotImplementedException();
		}
		
		public void Start(bool withDebugging) {
			throw new NotImplementedException();
		}
		
		public ParseProjectContent CreateProjectContent() {
			throw new NotImplementedException();
		}
		
		public CompilerResults Build() {
			throw new NotImplementedException();
		}
		
		public CompilerResults Rebuild() {
			throw new NotImplementedException();
		}
		
		public CompilerResults Clean() {
			throw new NotImplementedException();
		}
		
		public CompilerResults Publish() {
			throw new NotImplementedException();
		}
		
		public Properties CreateMemento() {
			throw new NotImplementedException();
		}
		
		public void SetMemento(Properties memento) {
			throw new NotImplementedException();
		}
		
		public bool EnableViewState {
			get { return false; }
			set {}
		}
		
		public string GetParseableFileContent(string fileContent)
		{ 
			return String.Empty;
		}
		
		public bool IsCompileable(string fileName) { return false; }
		
		public void LoadProject(string fileName) { }
		
		public void SaveProject(string fileName) { }
		public void CopyReferencesToOutputPath(bool b) {}		
		public void CopyReferencesToPath(string destination, bool force){}
		public void CopyReferencesToPath(string destination, bool force, ArrayList alreadyCopiedReferences) {}
		public bool IsFileInProject(string fileName) { return false; }
		
//		public IConfiguration CreateConfiguration(string name) { return null; }
//		
//		public IConfiguration CreateConfiguration() { return null; }
//		public IConfiguration CloneConfiguration(IConfiguration configuration) { return null; }
//		
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
