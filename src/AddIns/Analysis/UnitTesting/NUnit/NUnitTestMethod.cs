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
using System.Collections.Generic;
using System.Windows.Input;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Parser;
using ICSharpCode.SharpDevelop.Widgets;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test method.
	/// </summary>
	public class NUnitTestMethod : TestBase
	{
		readonly ITestProject parentProject;
		FullTypeName derivedFixture;
		IUnresolvedMethod method;
		string displayName;
		
		public NUnitTestMethod(ITestProject parentProject, IUnresolvedMethod method, FullTypeName derivedFixture = default(FullTypeName))
		{
			if (parentProject == null)
				throw new ArgumentNullException("parentProject");
			this.parentProject = parentProject;
			UpdateTestMethod(method, derivedFixture);
		}
		
		public void UpdateTestMethod(IUnresolvedMethod method, FullTypeName derivedFixture = default(FullTypeName))
		{
			if (method == null)
				throw new ArgumentNullException("method");
			string oldDisplayName = this.displayName;
			this.derivedFixture = derivedFixture;
			this.method = method;
			if (this.IsInherited)
				displayName = method.DeclaringTypeDefinition.Name + "." + method.Name;
			else
				displayName = method.Name;
			
			if (displayName != oldDisplayName && DisplayNameChanged != null)
				DisplayNameChanged(this, EventArgs.Empty);
		}
		
		public override ITestProject ParentProject {
			get { return parentProject; }
		}
		
		public override string DisplayName {
			get { return displayName; }
		}
		
		public override event EventHandler DisplayNameChanged;
		
		public DomRegion Region {
			get { return method.Region; }
		}
		
		public bool IsInherited {
			get { return derivedFixture.Name != null; }
		}
		
		public string MethodName {
			get { return method.Name; }
		}
		
		public string MethodNameWithDeclaringTypeForInheritedTests {
			get { return displayName; }
		}
		
		public string FixtureReflectionName {
			get { return IsInherited ? derivedFixture.ReflectionName : method.DeclaringTypeDefinition.ReflectionName; }
		}
		
		public void UpdateTestResult(TestResult result)
		{
			this.Result = result.ResultType;
		}
		
		public IMethod Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(parentProject.Project);
			IMethod resolvedMethod = method.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
			return resolvedMethod;
		}
		
		public IMethod Resolve(ISolutionSnapshotWithProjectMapping solutionSnapshot)
		{
			ICompilation compilation = solutionSnapshot.GetCompilation(parentProject.Project);
			IMethod resolvedMethod = method.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
			return resolvedMethod;
		}
		
		public override ICommand GoToDefinition {
			get {
				return new RelayCommand(
					delegate {
						IMethod method = Resolve();
						if (method != null)
							NavigationService.NavigateTo(method);
					});
			}
		}
		
	}
}
