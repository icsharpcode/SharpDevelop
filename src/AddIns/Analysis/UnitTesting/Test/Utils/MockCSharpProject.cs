// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Internal.Templates;
using ICSharpCode.SharpDevelop.Project;

namespace UnitTesting.Tests.Utils
{
	public class MockCSharpProject : CompilableProject
	{
		bool saved;
		
		public MockCSharpProject()
			: this(new Solution(), "MyTests")
		{
		}
		
		public MockCSharpProject(Solution solution, string name)
			: base(new ProjectCreateInformation {
					Solution = solution,
					ProjectName = name,
					TargetFramework = "v4.0",
					OutputProjectFileName = "c:\\projects\\" + name + "\\" + name + ".csproj"
				})
		{
			OutputType = OutputType.Library;
		}
		
		public override string Language {
			get { return "C#"; }
		}
		
		public override LanguageProperties LanguageProperties {
			get { return LanguageProperties.CSharp; }
		}
		
		public bool IsSaved {
			get { return saved; }
		}
		
		public override void Save(string fileName)
		{
			saved = true;
		}
	}
}
