// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ICSharpCode.RubyBinding
{
	public class RubyConsoleApplication
	{
		RubyAddInOptions options;
		bool debug;
		List<string> loadPaths = new List<string>();
		string rubyScriptFileName = String.Empty;
		string rubyScriptCommandLineArguments = String.Empty;
		string workingDirectory = String.Empty;
		string loadPath = String.Empty;
		StringBuilder arguments;
		
		public RubyConsoleApplication(RubyAddInOptions options)
		{
			this.options = options;
		}
		
		public string FileName {
			get { return options.RubyFileName; }
		}
		
		public bool Debug {
			get { return debug; }
			set { debug = value; }
		}
		
		public void AddLoadPath(string path)
		{
			loadPaths.Add(path);
		}
		
		public string RubyScriptFileName {
			get { return rubyScriptFileName; }
			set { rubyScriptFileName = value; }
		}
		
		public string RubyScriptCommandLineArguments {
			get { return rubyScriptCommandLineArguments; }
			set { rubyScriptCommandLineArguments = value; }
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
			
			AppendBooleanOptionIfTrue("-D", debug);
			AppendLoadPaths();
			AppendQuotedStringIfNotEmpty(rubyScriptFileName);
			AppendStringIfNotEmpty(rubyScriptCommandLineArguments);
			
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
		
		void AppendLoadPaths()
		{
			foreach (string path in loadPaths) {
				AppendQuotedString("-I" + path);
			}
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
