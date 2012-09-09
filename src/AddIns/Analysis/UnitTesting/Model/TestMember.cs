// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.TypeSystem.Implementation;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a member that can be tested.
	/// </summary>
	public class TestMember
	{
		readonly IUnresolvedMember member;

		public IUnresolvedMember Member {
			get { return member; }
		}
		
		public TestMember(IUnresolvedMember member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			this.member = member;
		}
		
		public string Name {
			get { return member.Name; }
		}

		public event EventHandler TestResultChanged;
		
		TestResultType testResult;

		public TestResultType TestResult {
			get { return testResult; }
			set {
				if (testResult != value) {
					testResult = value;
					if (TestResultChanged != null)
						TestResultChanged(this, EventArgs.Empty);
				}
			}
		}
		
		public virtual void ResetTestResults()
		{
			testResult = TestResultType.None;
		}
		
		public IMember Resolve(TestProject project)
		{
			if (project == null)
				return null;
			ICompilation compilation = SD.ParserService.GetCompilation(project.Project);
			return member.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
		}
		
		public override string ToString()
		{
			return string.Format("[TestMember Method={0}, TestResult={1}]", member, testResult);
		}
	}
}
