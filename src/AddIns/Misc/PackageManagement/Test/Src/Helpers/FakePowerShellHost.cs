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
