// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MSTest
{
	public class MSTestClass : TestBase
	{
		readonly MSTestProject parentProject;
		readonly FullTypeName fullTypeName;
		
		public MSTestClass(MSTestProject parentProject, FullTypeName fullTypeName)
		{
			this.parentProject = parentProject;
			this.fullTypeName = fullTypeName;
			BindResultToCompositeResultOfNestedTests();
		}
		
		public string GetTypeName()
		{
			return fullTypeName.ReflectionName;
		}
		
		public override ITestProject ParentProject {
			get { return parentProject; }
		}
		
		public override string DisplayName {
			get { return fullTypeName.Name; }
		}
		
		protected override void OnNestedTestsInitialized()
		{
			ITypeDefinition typeDefinition = Resolve();
			if (typeDefinition != null) {
				Update(typeDefinition);
			}
			base.OnNestedTestsInitialized();
		}
		
		public void Update(ITypeDefinition typeDefinition)
		{
			if (!NestedTestsInitialized)
				return;
			
			var newOrUpdatedTests = new HashSet<ITest>();
			foreach (ITest test in parentProject.GetTestMembersFor(typeDefinition)) {
				MSTestMember existingTest = FindTestMember(test);
				if (existingTest == null) {
					NestedTestCollection.Add(test);
					newOrUpdatedTests.Add(test);
				} else {
					newOrUpdatedTests.Add(existingTest);
				}
			}
			
			NestedTestCollection.RemoveAll(t => !newOrUpdatedTests.Contains(t));
		}
		
		MSTestMember FindTestMember(ITest test)
		{
			var testMember = test as MSTestMember;
			return FindTestMember(testMember.DisplayName);
		}
		
		public MSTestMember FindTestMember(string name)
		{
			return NestedTestCollection
				.OfType<MSTestMember>()
				.LastOrDefault(member => member.DisplayName == name);
		}
		
		public override ICommand GoToDefinition {
			get {
				return new RelayCommand(
					delegate {
						ITypeDefinition typeDefinition = Resolve();
						if (typeDefinition != null)
							NavigationService.NavigateTo(typeDefinition);
					});
			}
		}
		
		public ITypeDefinition Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(parentProject.Project);
			IType type = compilation.MainAssembly.GetTypeDefinition(fullTypeName);
			return type.GetDefinition();
		}
	}
}
