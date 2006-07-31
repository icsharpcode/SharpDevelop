// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;

namespace ICSharpCode.WixBinding
{
	public class WixLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Wix";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public void Execute(string fileName, bool debug)
		{
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			return String.Empty;
		}
		
		public bool CanCompile(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			if (ext == null) {
				return false;
			}
			return ext.Equals(".wxs", StringComparison.OrdinalIgnoreCase);
		}
		
		public CompilerResults CompileFile(string fileName)
		{
			return null;
		}
		
		public IProject LoadProject(string fileName, string projectName)
		{
			return new WixProject(fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			WixProject p = new WixProject(info);
			if (projectOptions != null) {
				p.ImportOptions(projectOptions.Attributes);
			}
			return p;
		}
	}
}
