// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Threading;

using ICSharpCode.Scripting;

namespace ICSharpCode.PackageManagement.Scripting
{
	public class PowerShellHost : PSHost, IPowerShellHost
	{
		IScriptingConsole scriptingConsole;
		CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
		CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
		Guid instanceId = Guid.NewGuid();
		Version version = new Version(0, 1);
		Runspace runspace;
		PowerShellHostUserInterface userInterface;
		//SharpDevelopEnvDTE dte = new SharpDevelopEnvDTE();
		
		public PowerShellHost(IScriptingConsole scriptingConsole)
		{
			this.scriptingConsole = scriptingConsole;
			userInterface = new PowerShellHostUserInterface(scriptingConsole);
		}
		
		public void SetRemoteSignedExecutionPolicy()
		{
			ExecuteCommand("Set-ExecutionPolicy RemoteSigned -Scope 0 -Force");
		}
		
		public void ExecuteCommand(string command)
		{
			try {
				CreateRunspace();
				
				Pipeline pipeline = CreatePipeline(command);
				pipeline.Invoke();
				
			} catch (Exception ex) {
				scriptingConsole.WriteLine(ex.Message, ScriptingStyle.Error);
			}
		}
		
		Pipeline CreatePipeline(string command)
		{
			Pipeline pipeline = runspace.CreatePipeline();
			pipeline.Commands.AddScript(command);
			pipeline.Commands.Add("out-default");
			pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
			return pipeline;
		}
		
		void CreateRunspace()
		{
			if (runspace == null) {
				var initialSessionState = InitialSessionState.CreateDefault();
				//var options = ScopedItemOptions.AllScope | ScopedItemOptions.Constant;
				//var variable = new SessionStateVariableEntry("DTE", dte, "SharpDevelop DTE object", options);
				//initialSessionState.Variables.Add(variable);

				runspace = RunspaceFactory.CreateRunspace(this); //, initialSessionState);
				runspace.Open();
			}
		}
		
		public override Version Version {
			get { return version; }
		}
		
		public override PSHostUserInterface UI {
			get { return userInterface; }
		}
		
		public override void SetShouldExit(int exitCode)
		{
		}
		
		public override void NotifyEndApplication()
		{
		}
		
		public override void NotifyBeginApplication()
		{
		}
		
		public override string Name {
			get { return "PowerShell Host"; }
		}
		
		public override Guid InstanceId {
			get { return instanceId; }
		}
		
		public override void ExitNestedPrompt()
		{
			throw new NotImplementedException();
		}
		
		public override void EnterNestedPrompt()
		{
			throw new NotImplementedException();
		}
		
		public override CultureInfo CurrentUICulture {
			get { return currentUICulture; }
		}
		
		public override CultureInfo CurrentCulture {
			get { return currentCulture; }
		}
		
		public void RunScript(string fileName, IEnumerable<object> input)
		{
			try {
				CreateRunspace();
				
				string command = 
					"$__args = @(); " +
					"$input | ForEach-Object {$__args += $_}; " +
					"& '" + fileName + "' $__args[0] $__args[1] $__args[2] $__args[3]" +
					"Remove-Variable __args -Scope 0";
				Pipeline pipeline = CreatePipeline(command);
				pipeline.Invoke(input);
				
			} catch (Exception ex) {
				scriptingConsole.WriteLine(ex.Message, ScriptingStyle.Error);
			}
		}
	}
}
