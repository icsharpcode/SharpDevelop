// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePowerShellHost : IPowerShellHost
	{
		public FakePowerShellHost()
		{
			Version = new Version(1, 0);
		}
		
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
		
		public List<string> modulesToImport = new List<string>();
		
		public IList<string> ModulesToImport {
			get { return modulesToImport; }
		}
		
		public IEnumerable<string> FormattingFilesPassedToUpdateFormatting;
		
		public void UpdateFormatting(IEnumerable<string> formattingFiles)
		{
			FormattingFilesPassedToUpdateFormatting = formattingFiles;
		}
		
		public Version Version { get; set; }
		
		public void SetEnvironmentPath(string path)
		{
			throw new NotImplementedException();
		}
		
		public string GetEnvironmentPath()
		{
			throw new NotImplementedException();
		}
		
		public void AddVariable(string name, object value)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveVariable(string name)
		{
			throw new NotImplementedException();
		}
		
		public bool IsSetDefaultRunspaceCalled;
		
		public void SetDefaultRunspace()
		{
			IsSetDefaultRunspaceCalled = true;
		}
	}
}
