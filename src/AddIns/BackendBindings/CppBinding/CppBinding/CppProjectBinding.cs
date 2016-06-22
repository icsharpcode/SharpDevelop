// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using ICSharpCode.CppBinding.Project;
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

//		public readonly static CppLanguageProperties LanguageProperties = new CppLanguageProperties();

		public IProject LoadProject(ProjectLoadInformation info) {
			return new CppProject(info);
		}
		
		public IProject CreateProject(ProjectCreateInformation info) {
			info.ActiveProjectConfiguration = new ConfigurationAndPlatform("Debug", "Win32");
			return new CppProject(info);
		}
		
		public bool HandlingMissingProject {
			get { 
				return false; 
			}
		}
	}
}
