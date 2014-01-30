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

using System;
using System.IO;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PythonBinding
{
	/// <summary>
	/// Represents an MSBuild project that can build python code.
	/// </summary>
	public class PythonProject : CompilableProject
	{
		public const string DefaultTargetsFile = @"$(PythonBinPath)\SharpDevelop.Build.Python.targets";
		
		public PythonProject(ProjectLoadInformation info)
			: base(info)
		{
		}
		
		public PythonProject(ProjectCreateInformation info)
			: base(info)
		{
			AddImport(DefaultTargetsFile, null);
		}
		
		/// <summary>
		/// Gets the language associated with the project.
		/// </summary>
		public override string Language {
			get { return PythonProjectBinding.LanguageName; }
		}
		
		/// <summary>
		/// Gets the language properties associated with this project.
		/// </summary>
		public override LanguageProperties LanguageProperties {
			get { return PythonLanguageProperties.Default; }
		}
		
		public void AddMainFile(string fileName)
		{
			SetProperty(null, null, "MainFile", fileName, PropertyStorageLocations.Base, true);
		}
		
		/// <summary>
		/// Returns true if a main file is already defined for this project.
		/// </summary>
		public bool HasMainFile {
			get { return GetProperty(null, null, "MainFile") != null; }
		}
		
		protected override ProjectBehavior CreateDefaultBehavior()
		{
			return new PythonProjectBehavior(this, base.CreateDefaultBehavior());
		}
	}
	
	public class PythonProjectBehavior : ProjectBehavior
	{
		public PythonProjectBehavior(PythonProject project, ProjectBehavior next = null)
			: base(project, next)
		{
			
		}
		
		/// <summary>
		/// Returns ItemType.Compile if the filename has a
		/// python extension (.py).
		/// </summary>
		public override ItemType GetDefaultItemType(string fileName)
		{
			if (fileName != null) {
				string extension = Path.GetExtension(fileName);
				if (extension.ToLowerInvariant() == ".py") {
					return ItemType.Compile;
				}
			}
			return base.GetDefaultItemType(fileName);
		}
	}
}
