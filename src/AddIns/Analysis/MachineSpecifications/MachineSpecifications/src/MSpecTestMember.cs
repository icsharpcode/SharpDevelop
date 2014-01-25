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
using System.Windows.Input;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Widgets;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class MSpecTestMember : TestBase, ITestWithAssociatedType
	{
		MSpecTestProject parentProject;
		string displayName;
		IMember member;
		
		MSpecTestMember(MSpecTestProject parentProject, string displayName)
		{
			this.parentProject = parentProject;
			this.displayName = displayName;
		}
		
		public MSpecTestMember(
			MSpecTestProject parentProject,
			IMember member)
			: this(parentProject, member.Name)
		{
			this.member = member;
		}

		public override ITestProject ParentProject {
			get { return parentProject; }
		}
		
		public override string DisplayName {
			get { return displayName; }
		}
		
		public void UpdateTestResult(TestResult result)
		{
			this.Result = result.ResultType;
		}
		
		public IMember Member {
			get { return this.member; }
		}
		
		public IMember Resolve()
		{
			ICompilation compilation = SD.ParserService.GetCompilation(parentProject.Project);
			return member.UnresolvedMember.Resolve(new SimpleTypeResolveContext(compilation.MainAssembly));
		}
		
		public override ICommand GoToDefinition {
			get {
				return new RelayCommand(
					delegate {
						IMember member = Resolve();
						if (member != null)
							NavigationService.NavigateTo(member);
					});
			}
		}
		
		public virtual string GetTypeName()
		{
			return member.DeclaringTypeDefinition.FullName;
		}
	}
}
