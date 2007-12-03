// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Language binding for Python.
	/// </summary>
	public class PythonLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "Python";
		
		public PythonLanguageBinding()
		{
		}
		
		public string Language {
			get { return LanguageName; }
		}
		
		public IProject LoadProject(IMSBuildEngineProvider engineProvider, string fileName, string projectName)
		{
			return new PythonProject(engineProvider, fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			return new PythonProject(info);
		}
	}
}
