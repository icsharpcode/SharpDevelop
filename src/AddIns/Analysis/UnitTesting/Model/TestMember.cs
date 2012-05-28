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
		IUnresolvedMethod method;

		public IUnresolvedMethod Method {
			get { return method; }
		}
		
		public TestProject Project { get; private set; }
		
		public bool IsTestCase { get; private set; }
		
		public TestMember(TestProject project, IUnresolvedMethod method, bool isTestCase)
		{
			if (method == null)
				throw new ArgumentNullException("method");
			this.method = method;
			this.Project = project;
			this.IsTestCase = isTestCase;
		}

		TestResultType testResult;

		public TestResultType TestResult {
			get { return testResult; }
			set {
				if (testResult < value) {
					testResult = value;
					OnPropertyChanged();
				}
			}
		}
		
		public void ResetTestResult()
		{
			testResult = TestResultType.None;
		}
		
		public IMethod Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(Project.Project);
			return method.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
		}
		
		public override string ToString()
		{
			return string.Format("[TestMember Method={0}, TestResult={1}]", method, testResult);
		}
	}
}
