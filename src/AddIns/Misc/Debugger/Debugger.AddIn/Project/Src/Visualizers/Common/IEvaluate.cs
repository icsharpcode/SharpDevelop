// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
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
