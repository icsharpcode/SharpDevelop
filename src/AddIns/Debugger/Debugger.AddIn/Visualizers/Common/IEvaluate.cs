// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

using System;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Object that has its state partially uninitialized, 
	/// and can initialize its state fully on demand.
	/// </summary>
	public interface IEvaluate
	{
		/// <summary>
		/// On-demand evaluation of internal state.
		/// </summary>
		void Evaluate();

		/// <summary>
		/// Is this object fully evaluated?
		/// </summary>
		bool IsEvaluated { get; }
	}
}
