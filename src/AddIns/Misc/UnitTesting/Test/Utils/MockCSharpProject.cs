// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.SharpDevelop.Internal.Templates;
using System;
using ICSharpCode.SharpDevelop.Project;

namespace UnitTesting.Tests.Utils
{
	public class MockCSharpProject : CompilableProject
	{
		public MockCSharpProject()
			: this(new Solution(), "Dummy")
		{
		}
		
		public MockCSharpProject(Solution solution, string name)
			: base(new ProjectCreateInformation {
			       	Solution = solution,
			       	ProjectName = name,
			       	OutputProjectFileName = "c:\\temp\\" + name + ".csproj"
			       })
		{
		}
		
		public override string Language {
			get { return "C#"; }
		}
		
		public override ICSharpCode.SharpDevelop.Dom.LanguageProperties LanguageProperties {
			get { return ICSharpCode.SharpDevelop.Dom.LanguageProperties.CSharp; }
		}
	}
}
