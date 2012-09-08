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
	public class TestMember : ViewModelBase
	{
		IUnresolvedMember member;

		public IUnresolvedMember Member {
			get { return member; }
		}
		
		public TestProject Project { get; private set; }
		
		public TestMember(TestProject project, IUnresolvedMember member)
		{
			if (member == null)
				throw new ArgumentNullException("member");
			this.member = member;
			this.Project = project;
		}

		TestResultType testResult;

		public virtual TestResultType TestResult {
			get { return testResult; }
			set {
				if (testResult < value) {
					testResult = value;
					OnPropertyChanged();
				}
			}
		}
		
		public virtual void ResetTestResult()
		{
			testResult = TestResultType.None;
		}
		
		public IMember Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(Project.Project);
			return member.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
		}
		
		public override string ToString()
		{
			return string.Format("[TestMember Method={0}, TestResult={1}]", member, testResult);
		}
	}
}
