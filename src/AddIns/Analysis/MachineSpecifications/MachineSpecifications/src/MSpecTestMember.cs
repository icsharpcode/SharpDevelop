// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
