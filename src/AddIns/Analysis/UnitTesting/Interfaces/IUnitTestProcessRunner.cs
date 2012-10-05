// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.UnitTesting
{
	public interface IUnitTestProcessRunner : IDisposable
	{
		bool LogStandardOutputAndError { get; set; }
		string WorkingDirectory { get; set; }
		
		Dictionary<string, string> EnvironmentVariables { get; }
		
		void Start(string command, string arguments);
		void Kill();
		
		event EventHandler<LineReceivedEventArgs> OutputLineReceived;
		event EventHandler<LineReceivedEventArgs> ErrorLineReceived;
		event EventHandler ProcessExited;
	}
}
