// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using ICSharpCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace ICSharpCode.SharpDevelop.Debugging
{
	/// <summary>
	/// Control displaying and executing <see cref="IVisualizerCommand">visualizer commands</see>.
	/// </summary>
	public class VisualizerPicker : ComboBox
	{
		public VisualizerPicker()
		{
			this.SelectionChanged += VisualizerPicker_SelectionChanged;
		}

		void VisualizerPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.SelectedItem == null) {
				return;
			}
			var clickedCommand = this.SelectedItem as IVisualizerCommand;
			if (clickedCommand == null) {
				throw new InvalidOperationException(
					string.Format("{0} clicked, only instances of {1} must be present in {2}.",
					              this.SelectedItem.GetType().ToString(), typeof(IVisualizerCommand).Name, typeof(VisualizerPicker).Name));
				}
			
			clickedCommand.Execute();
			// make no item selected, so that multiple selections of the same item always execute the command
			// this triggers VisualizerPicker_SelectionChanged again, which returns immediately
			this.SelectedIndex = -1;
		}
		
		public new IEnumerable<IVisualizerCommand> ItemsSource
		{
			get { return (IEnumerable<IVisualizerCommand>)base.ItemsSource; }
			set { base.ItemsSource = value; }
		}
	}
}