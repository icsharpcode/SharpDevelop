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
using System.Linq;

using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class MSpecTestClass : TestBase, ITestWithAssociatedType
	{
		MSpecTestProject parentProject;
		FullTypeName fullTypeName;
		
		public MSpecTestClass(MSpecTestProject parentProject, FullTypeName fullTypeName)
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
				MSpecTestMember existingTest = FindTestMember(test);
				if (existingTest == null) {
					NestedTestCollection.Add(test);
					newOrUpdatedTests.Add(test);
				} else {
					newOrUpdatedTests.Add(existingTest);
				}
			}
			
			NestedTestCollection.RemoveAll(t => !newOrUpdatedTests.Contains(t));
		}
		
		MSpecTestMember FindTestMember(ITest test)
		{
			var testMember = test as MSpecTestMember;
			return FindTestMember(testMember.DisplayName);
		}
		
		public MSpecTestMember FindTestMember(string name)
		{
			return NestedTestCollection
				.OfType<MSpecTestMember>()
				.LastOrDefault(member => member.DisplayName == name);
		}
		
		public override System.Windows.Input.ICommand GoToDefinition {
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
