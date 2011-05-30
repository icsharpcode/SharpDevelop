// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.PackageManagement.Scripting
{
	public interface IPackageScriptSession
	{
		void SetEnvironmentPath(string path);
		string GetEnvironmentPath();
		
		void AddVariable(string name, object value);
		void RemoveVariable(string name);
		
		void InvokeScript(string script);
	}
}
