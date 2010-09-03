// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Util;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestProcessRunner
	{
		bool LogStandardOutputAndError { get; set; }
		string WorkingDirectory { get; set; }
		
		Dictionary<string, string> EnvironmentVariables { get; }
		
		void Start(string command, string arguments);
		void Kill();
		
		event LineReceivedEventHandler OutputLineReceived;
		event LineReceivedEventHandler ErrorLineReceived;
		event EventHandler ProcessExited;
	}
}
