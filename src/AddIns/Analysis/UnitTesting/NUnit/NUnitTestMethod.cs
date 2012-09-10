// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test method.
	/// </summary>
	public class NUnitTestMethod : TestBase
	{
		readonly ITestProject parentProject;
		IUnresolvedTypeDefinition derivedFixture;
		IUnresolvedMethod method;
		string displayName;
		
		public NUnitTestMethod(ITestProject parentProject, IUnresolvedMethod method, IUnresolvedTypeDefinition fixture = null)
		{
			if (parentProject == null)
				throw new ArgumentNullException("parentProject");
			this.parentProject = parentProject;
			UpdateTestMethod(method, fixture);
		}
		
		public void UpdateTestMethod(IUnresolvedMethod method, IUnresolvedTypeDefinition derivedFixture = null)
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
		
		public bool IsInherited {
			get { return derivedFixture != null; }
		}
		
		public string MethodName {
			get { return method.Name; }
		}
		
		public string MethodNameWithDeclaringTypeForInheritedTests {
			get { return displayName; }
		}
		
		public string FixtureReflectionName {
			get { return (derivedFixture ?? method.DeclaringTypeDefinition).ReflectionName; }
		}
		
		public void UpdateTestResult(TestResult result)
		{
			this.Result = result.ResultType;
		}
		
		IMethod Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(parentProject.Project);
			IMethod resolvedMethod = method.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
			return resolvedMethod;
		}
		
		public override bool SupportsGoToDefinition {
			get { return true; }
		}
		
		public override void GoToDefinition()
		{
			IMethod resolvedMethod = Resolve();
			if (resolvedMethod != null)
				NavigationService.NavigateTo(resolvedMethod);
		}
	}
}
