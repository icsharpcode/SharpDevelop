// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ICSharpCode.Core;

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Provides all debugger visualizer Descriptors.
	/// </summary>
	public static class VisualizerDescriptors
	{
		static IList<IVisualizerDescriptor> CreateAllDescriptors()
		{
			return AddInTree.BuildItems<IVisualizerDescriptor>("/SharpDevelop/Services/DebuggerService/Visualizers", null);
		}
		
		static ReadOnlyCollection<IVisualizerDescriptor> descriptors;
		public static ReadOnlyCollection<IVisualizerDescriptor> GetAllDescriptors()
		{
			if (descriptors == null) {
				descriptors = CreateAllDescriptors().ToList().AsReadOnly();
			}
			return descriptors;
		}
	}
}
