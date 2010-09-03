// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Description of DebuggerVisualizerException.
	/// </summary>
	public class DebuggerVisualizerException : ApplicationException
	{
		public DebuggerVisualizerException()
            : base()
        {
        }

        public DebuggerVisualizerException(string message)
            : base(message)
        {
        }

        public DebuggerVisualizerException(string message, System.Exception innerException)
            : base(message, innerException)
        {
        }
	}
}
