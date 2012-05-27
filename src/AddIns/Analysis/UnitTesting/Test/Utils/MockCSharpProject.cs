// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
			: this(new Solution(new MockProjectChangeWatcher()), "MyTests")
		{
		}
		
		public MockCSharpProject(Solution solution, string name)
			: base(new ProjectCreateInformation {
					Solution = solution,
					ProjectName = name,
					Platform = "x86",
					TargetFramework = TargetFramework.Net40Client,
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
