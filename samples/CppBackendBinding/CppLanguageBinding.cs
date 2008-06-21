// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;
using System.IO;

namespace CppBackendBinding
{
	public class CppLanguageBinding : ILanguageBinding
	{
		public const string LanguageName = "C++";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(IMSBuildEngineProvider provider, string fileName, string projectName)
		{
			return new CppProject(fileName, projectName);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			throw new NotImplementedException();
		}
	}
}
