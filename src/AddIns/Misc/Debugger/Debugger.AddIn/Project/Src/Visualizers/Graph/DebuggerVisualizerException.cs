// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníèek" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;

namespace Debugger.AddIn.Visualizers.Graph
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
