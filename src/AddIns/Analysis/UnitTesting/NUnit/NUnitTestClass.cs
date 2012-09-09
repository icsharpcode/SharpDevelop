// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// NUnit test fixture.
	/// </summary>
	public class NUnitTestClass : TestBase
	{
		ITestProject parentProject;
		IUnresolvedTypeDefinition primaryPart;
		
		public NUnitTestClass(ITestProject parentProject, ITypeDefinition typeDefinition)
		{
			this.parentProject = parentProject;
			UpdateTestClass(typeDefinition);
			BindResultToCompositeResultOfNestedTests();
		}
		
		public override ITestProject ParentProject {
			get { return parentProject; }
		}
		
		public override string DisplayName {
			get { return primaryPart.Name; }
		}
		
		public string ReflectionName {
			get { return primaryPart.ReflectionName; }
		}
		
		public override bool SupportsGoToDefinition {
			get { return true; }
		}
		
		ITypeDefinition Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(parentProject.Project);
			IType type = primaryPart.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
			return type.GetDefinition();
		}
		
		public override void GoToDefinition()
		{
			ITypeDefinition typeDefinition = Resolve();
			if (typeDefinition != null)
				NavigationService.NavigateTo(typeDefinition);
		}
		
		public void UpdateTestClass(ITypeDefinition typeDefinition)
		{
			primaryPart = typeDefinition.Parts[0];
			if (this.NestedTestsInitialized) {
				this.NestedTests.Clear();
				this.NestedTests.AddRange(from nt in typeDefinition.NestedTypes
				                          where NUnitTestFramework.IsTestClass(nt)
				                          select new NUnitTestClass(parentProject, nt));
				this.NestedTests.AddRange(from m in typeDefinition.Methods
				                          where NUnitTestFramework.IsTestMember(m)
				                          select new NUnitTestMethod(parentProject, (IUnresolvedMethod)m.UnresolvedMember));
			}
		}
		
		public override bool CanExpandNestedTests {
			get { return true; }
		}
		
		protected override void OnNestedTestsInitialized()
		{
			ITypeDefinition typeDefinition = Resolve();
			if (typeDefinition != null) {
				UpdateTestClass(typeDefinition);
			}
			base.OnNestedTestsInitialized();
		}
	}
}
