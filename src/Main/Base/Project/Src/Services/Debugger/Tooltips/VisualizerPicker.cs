// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Description of VisualizerPicker.
	/// </summary>
	public class VisualizerPicker : ComboBox
	{
		public VisualizerPicker()
		{
			this.ItemsSource = new string[] { "Collection visualizer", "Object graph visualizer" };
		}
	}
}