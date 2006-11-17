// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.SharpDevelop.Project;

namespace UnitTesting.Tests.Utils
{
	public class MockCSharpProject : CompilableProject
	{
		public MockCSharpProject()
			: base(new Solution())
		{
		}
		
		public MockCSharpProject(Solution solution)
			: base(solution)
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
