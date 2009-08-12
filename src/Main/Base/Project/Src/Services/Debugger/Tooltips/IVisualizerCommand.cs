// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Command called from <see cref="VisualizerPicker"/>.
	/// </summary>
	public interface IVisualizerCommand
	{
		/// <summary>
		/// Can this command execute?
		/// </summary>
		bool CanExecute { get; }
		
		/// <summary>
		/// Executes this visualizer command.
		/// </summary>
		void Execute();
	}
}
