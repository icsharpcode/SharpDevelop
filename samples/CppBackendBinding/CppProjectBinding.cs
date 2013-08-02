// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace CppBackendBinding
{
	public class CppProjectBinding : IProjectBinding
	{
		public const string LanguageName = "C++";
		
		public string Language {
			get {
				return LanguageName;
			}
		}
		
		public IProject LoadProject(ProjectLoadInformation info)
		{
			return new CppProject(info);
		}
		
		public IProject CreateProject(ProjectCreateInformation info)
		{
			throw new NotImplementedException();
		}
		
		public bool HandlingMissingProject {
			get {
				return false;
			}
		}
	}
}
