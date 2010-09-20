// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the BSD license (for details please see \src\AddIns\Debugger\Debugger.AddIn\license.txt)

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
