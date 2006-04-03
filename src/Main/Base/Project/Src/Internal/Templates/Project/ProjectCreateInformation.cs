// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
	/// <summary>
	/// This class holds all information the language binding need to create
	/// a predefined project for their language, if no project template for a 
	/// specific language is avaiable, the language binding shouldn't care about
	/// this stuff.
	/// </summary>
	public class ProjectCreateInformation
	{
		string projectName;
		string combinePath;
		string projectBasePath;
		string outputProjectFileName;
		
		public List<string> CreatedProjects  = new List<string>();
		
		public ProjectCreateInformation()
		{
			SetDefaultCreateProjectOptions();
		}
		
		internal bool CreateProjectWithDefaultOutputPath;
		
		internal void SetDefaultCreateProjectOptions()
		{
			CreateProjectWithDefaultOutputPath = true;
		}
		
		public string OutputProjectFileName {
			get {
				return outputProjectFileName;
			}
			set {
				outputProjectFileName = value;
			}
		}
		
		public string ProjectName {
			get {
				return projectName;
			}
			set {
				projectName = value;
			}
		}
		
		public string BinPath {
			get {
				return Path.Combine(combinePath, "bin");
			}
		}
		
		public string CombinePath {
			get {
				return combinePath;
			}
			set {
				combinePath = value;
			}
		}
		
		public string ProjectBasePath {
			get {
				return projectBasePath;
			}
			set {
				projectBasePath = value;
			}
		}
	}
}
