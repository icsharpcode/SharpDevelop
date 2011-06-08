// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using NuGet;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PackageScript : IPackageScript
	{
		public PackageScript(IPackage package, IPackageScriptFileName fileName)
		{
			this.Package = package;
			this.ScriptFileName = fileName;
		}
		
		protected IPackageScriptFileName ScriptFileName { get; private set; }
		protected IPackageScriptSession Session { get; private set; }
		
		public IPackage Package { get; set; }
		public IPackageManagementProject Project { get; set; }
		
		public bool Exists()
		{
			return ScriptFileName.FileExists();
		}
		
		public void Run(IPackageScriptSession session)
		{
			this.Session = session;
			Run();
		}
		
		void Run()
		{
			BeforeRun();
			if (Exists()) {
				AddSessionVariables();
				RunScript();
				RemoveSessionVariables();
			}
		}
		
		protected virtual void BeforeRun()
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
		
		void RunScript()
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
