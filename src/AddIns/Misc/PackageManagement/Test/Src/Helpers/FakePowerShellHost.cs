// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePowerShellHost : IPowerShellHost
	{
		public bool IsSetRemoteSignedExecutionPolicyCalled;
		
		public void SetRemoteSignedExecutionPolicy()
		{
			IsSetRemoteSignedExecutionPolicyCalled = true;
		}
		
		public string CommandPassedToExecuteCommand;
		public List<string> AllCommandsPassedToExecuteCommand = new List<string>();
		
		public void ExecuteCommand(string command)
		{
			CommandPassedToExecuteCommand = command;
			AllCommandsPassedToExecuteCommand.Add(command);
		}
	}
}
