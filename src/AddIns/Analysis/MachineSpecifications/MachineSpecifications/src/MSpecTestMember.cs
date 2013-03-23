// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.UnitTesting;

namespace ICSharpCode.MachineSpecifications
{
	public class MSpecTestMember : TestBase
	{
		MSpecTestProject parentProject;
		string displayName;
		
		public MSpecTestMember(MSpecTestProject parentProject, string displayName)
		{
			this.parentProject = parentProject;
			this.displayName = displayName;
		}
		
		public MSpecTestMember(
			MSpecTestProject parentProject,
			IMember member)
			: this(parentProject, member.DeclaringType.Name + "." + member.Name)
		{
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
	}
}
