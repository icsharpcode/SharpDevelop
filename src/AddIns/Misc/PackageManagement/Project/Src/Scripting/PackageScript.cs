// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageScript : IPackageScript
	{
		public PackageScript(
			PackageScriptFileName fileName,
			IPackageScriptSession session)
		{
			this.ScriptFileName = fileName;
			this.Session = session;
		}
		
		protected PackageScriptFileName ScriptFileName { get; private set; }
		protected IPackageScriptSession Session { get; private set; }
		
		public IPackage Package { get; set; }
		public IPackageManagementProject Project { get; set; }
		
		public void Execute()
		{
			BeforeExecute();
			AddSessionVariables();
			ExecuteScript();
			RemoveSessionVariables();
		}
		
		protected virtual void BeforeExecute()
		{
		}
		
		void AddSessionVariables()
		{
			Session.AddVariable("__rootPath", ScriptFileName.PackageInstallDirectory);
			Session.AddVariable("__toolsPath", ScriptFileName.GetScriptDirectory());
			Session.AddVariable("__package", Package);
			Session.AddVariable("__project", GetProject());
		}
		
		Project GetProject()
		{
			if (Project != null) {
				return Project.ConvertToDTEProject();
			}
			return null;
		}
		
		void ExecuteScript()
		{
			string script = GetScript();
			Session.InvokeScript(script);
		}
		
		string GetScript()
		{
			return String.Format(
				"& '{0}' $__rootPath $__toolsPath $__package $__project",
				ScriptFileName);
		}
		
		void RemoveSessionVariables()
		{
			Session.RemoveVariable("__rootPath");
			Session.RemoveVariable("__toolsPath");
			Session.RemoveVariable("__package");
			Session.RemoveVariable("__project");
		}
	}
}
