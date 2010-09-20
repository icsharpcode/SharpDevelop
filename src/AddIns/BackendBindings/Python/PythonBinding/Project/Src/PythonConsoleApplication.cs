// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.PythonBinding
{
	public class PythonConsoleApplication
	{
		PythonAddInOptions options;
		StringBuilder arguments;
		bool debug;
		string pythonScriptFileName = String.Empty;
		string pythonScriptCommandLineArguments = String.Empty;
		string workingDirectory = String.Empty;
		
		public PythonConsoleApplication(PythonAddInOptions options)
		{
			this.options = options;
		}
		
		public string FileName {
			get { return options.PythonFileName; }
		}
		
		public bool Debug {
			get { return debug; }
			set { debug = value; }
		}
		
		public string PythonScriptFileName {
			get { return pythonScriptFileName; }
			set { pythonScriptFileName = value; }
		}
		
		public string PythonScriptCommandLineArguments {
			get { return pythonScriptCommandLineArguments; }
			set { pythonScriptCommandLineArguments = value; }
		}
		
		public string WorkingDirectory {
			get { return workingDirectory; }
			set { workingDirectory = value; }
		}
		
		public ProcessStartInfo GetProcessStartInfo()
		{
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = FileName;
			processStartInfo.Arguments = GetArguments();
			processStartInfo.WorkingDirectory = workingDirectory;
			return processStartInfo;
		}
		
		public string GetArguments()
		{
			arguments = new StringBuilder();
			
			AppendBooleanOptionIfTrue("-X:Debug", debug);
			AppendQuotedStringIfNotEmpty(pythonScriptFileName);
			AppendStringIfNotEmpty(pythonScriptCommandLineArguments);
			
			return arguments.ToString().TrimEnd();
		}
		
		void AppendBooleanOptionIfTrue(string option, bool flag)
		{
			if (flag) {
				AppendOption(option);
			}
		}
		
		void AppendOption(string option)
		{
			arguments.Append(option + " ");
		}
		
		void AppendQuotedStringIfNotEmpty(string option)
		{
			if (!String.IsNullOrEmpty(option)) {
				AppendQuotedString(option);
			}
		}
		
		void AppendQuotedString(string option)
		{
			string quotedOption = String.Format("\"{0}\"", option);
			AppendOption(quotedOption);
		}
		
		void AppendStringIfNotEmpty(string option)
		{
			if (!String.IsNullOrEmpty(option)) {
				AppendOption(option);
			}
		}
	}
}
