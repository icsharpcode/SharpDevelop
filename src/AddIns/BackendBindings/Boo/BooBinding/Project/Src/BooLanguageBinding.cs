// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Xml;
using System.CodeDom.Compiler;
using System.Threading;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;

namespace Grunwald.BooBinding
{
	public class BooLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Boo";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		#region routines for single file compilation
		public bool CanCompile(string fileName)
		{
			string ext = Path.GetExtension(fileName);
			if (ext == null)
				return false;
			return string.Equals(ext, ".BOO", StringComparison.InvariantCultureIgnoreCase);
		}
		
		public string GetCompiledOutputName(string fileName)
		{
			return Path.ChangeExtension(fileName, ".exe");
		}
		
		public CompilerResults CompileFile(string fileName)
		{
			throw new NotImplementedException();
		}
		
		public void Execute(string fileName, bool debug)
		{
			throw new NotImplementedException(); // only needed for single-file compilation
		}
		#endregion
		
		public IProject LoadProject(string fileName, string projectName)
		{
			return new BooProject(fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info, XmlElement projectOptions)
		{
			BooProject p = new BooProject(info);
			if (projectOptions != null)
				p.ImportOptions(projectOptions.Attributes);
			return p;
		}
	}
}
