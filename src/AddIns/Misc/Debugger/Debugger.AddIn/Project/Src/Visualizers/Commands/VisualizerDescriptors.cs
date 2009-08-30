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

namespace Debugger.AddIn.Visualizers
{
	/// <summary>
	/// Provides all debugger visualizer Descriptors.
	/// </summary>
	public static class VisualizerDescriptors
	{
		static ReadOnlyCollection<IVisualizerDescriptor> allDescriptors;
			
		static IEnumerable<IVisualizerDescriptor> CreateAllDescriptors()
		{
			// these should be obtained from AddIn tree so that it is possible to write add-in for Debugger.AddIn with new visualizers
			yield return new TextVisualizerDescriptor();
			yield return new XmlVisualizerDescriptor();
			yield return new ObjectGraphVisualizerDescriptor();
			yield return new GridVisualizerDescriptor();
		}
		
		public static ReadOnlyCollection<IVisualizerDescriptor> GetAllDescriptors()
		{
			if (allDescriptors == null) {
				allDescriptors = new List<IVisualizerDescriptor>(CreateAllDescriptors()).AsReadOnly();
			}
			return allDescriptors;
		}
	}
}
