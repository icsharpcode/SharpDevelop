// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.PackageManagement.Scripting;

namespace PackageManagement.Tests.Helpers
{
	public class FakePackageScriptSession : IPackageScriptSession
	{
		public string EnvironmentPath = String.Empty;
		
		public void SetEnvironmentPath(string path)
		{
			EnvironmentPath = path;
		}
		
		public string GetEnvironmentPath()
		{
			return EnvironmentPath;
		}
		
		public Dictionary<string, object> VariablesAdded = new Dictionary<string, object>();
		
		public void AddVariable(string name, object value)
		{
			VariablesAdded.Add(name, value);
		}
		
		public List<string> VariablesRemoved = new List<string>();
		
		public void RemoveVariable(string name)
		{
			VariablesRemoved.Add(name);
		}
		
		public bool IsScriptExecuted;
		public string ScriptPassedToInvokeScript;
		
		public void InvokeScript(string script)
		{
			IsScriptExecuted = true;
			ScriptPassedToInvokeScript = script;
		}
	}
}
