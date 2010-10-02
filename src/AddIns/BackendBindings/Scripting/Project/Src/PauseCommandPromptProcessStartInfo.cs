// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;

namespace ICSharpCode.Scripting
{
	public class PauseCommandPromptProcessStartInfo
	{
		ProcessStartInfo processStartInfo;
		
		public PauseCommandPromptProcessStartInfo(ProcessStartInfo processStartInfo)
		{
			this.processStartInfo = processStartInfo;
			ChangeProcessStartInfoToPauseCommandPrompt();
		}
		
		public ProcessStartInfo ProcessStartInfo {
			get { return processStartInfo; }
		}
		
		void ChangeProcessStartInfoToPauseCommandPrompt()
		{
			processStartInfo.Arguments = GetArguments();
			processStartInfo.FileName = "cmd.exe";
		}
		
		string GetArguments()
		{
			return String.Format("/c \"\"{0}\" {1}\" & pause", 
				processStartInfo.FileName, processStartInfo.Arguments);
		}
	}
}
