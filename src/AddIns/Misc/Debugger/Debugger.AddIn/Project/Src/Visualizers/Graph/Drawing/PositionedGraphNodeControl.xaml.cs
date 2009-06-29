// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Debugger.AddIn.Visualizers.Graph.Drawing
{
	/// <summary>
	/// Interaction logic for PositionedGraphNodeControl.xaml
	/// </summary>
	public partial class PositionedGraphNodeControl : UserControl
	{
		/// <summary>
		/// Occurs when a <see cref="PositionedNodeProperty"/> is expanded.
		/// </summary>
		public event EventHandler<PositionedPropertyEventArgs> PropertyExpanded;
		/// <summary>
		/// Occurs when a <see cref="PositionedNodeProperty"/> is collaped.
		/// </summary>
		public event EventHandler<PositionedPropertyEventArgs> PropertyCollapsed;
		
		public PositionedGraphNodeControl()
		{
			InitializeComponent();
		}
	}
}