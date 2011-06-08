// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.Cmdlets;
using ICSharpCode.PackageManagement.Design;
using PackageManagement.Tests.Helpers;

namespace PackageManagement.Cmdlets.Tests.Helpers
{
	public class TestablePackageManagementCmdlet : PackageManagementCmdlet
	{
		public TestablePackageManagementCmdlet()
			: this(new FakeCmdletTerminatingError())
		{
		}
		
		public TestablePackageManagementCmdlet(FakeCmdletTerminatingError terminatingError)
			: base(
				new FakePackageManagementConsoleHost(),
				terminatingError)
		{
			this.CommandRuntime = new FakeCommandRuntime();
		}
		
		public void CallThrowProjectNotOpenTerminatingError()
		{
			base.ThrowProjectNotOpenTerminatingError();
		}
		
		public object GetSessionVariableReturnValue;
		public string NamePassedToGetSessionVariable;
		
		protected override object GetSessionVariable(string name)
		{
			NamePassedToGetSessionVariable = name;
			return GetSessionVariableReturnValue;
		}
		
		public object ValuePassedToSetSessionVariable;
		public string NamePassedToSetSessionVariable;
		
		protected override void SetSessionVariable(string name, object value)
		{
			NamePassedToSetSessionVariable = name;
			ValuePassedToSetSessionVariable = value;
		}
		
		public string NamePassedToRemoveVariable;
		
		protected override void RemoveSessionVariable(string name)
		{
			NamePassedToRemoveVariable = name;
		}
	}
}
