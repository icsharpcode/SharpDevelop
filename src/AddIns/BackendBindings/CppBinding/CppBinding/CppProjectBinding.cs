// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using ICSharpCode.CppBinding.Project;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.CppBinding
{
	/// <summary>
	/// Description of CppProjectBinding.
	/// </summary>
	public class CppProjectBinding : IProjectBinding
	{
		public const string LanguageName = "C++";
		
		public string Language {
			get {
				return LanguageName;
			}
		}

		public readonly static CppLanguageProperties LanguageProperties = new CppLanguageProperties();

		public IProject LoadProject(ProjectLoadInformation info) {
			return new CppProject(info);
		}
		
		public IProject CreateProject(ProjectCreateInformation info) {
			info.Platform = "Win32";
			return new CppProject(info);
		}
		
		public bool HandlingMissingProject {
			get { 
				return false; 
			}
		}
	}
}
