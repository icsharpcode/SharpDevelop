// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Martin Koníček" email="martin.konicek@gmail.com"/>
//     <version>$Revision$</version>
// </file>
using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Debugger.AddIn.Visualizers.TextVisualizer
{
	/// <summary>
	/// Interaction logic for TextVisualizerWindow.xaml
	/// </summary>
	public partial class TextVisualizerWindow : Window
	{
		public TextVisualizerWindow()
		{
			InitializeComponent();
		}
		
		public TextVisualizerWindow(string title, string text)
		{
			InitializeComponent();
			
			Title= title;
			Text = text;
		}
		
		public string Text
		{
			get { return this.txtText.Text; }
			set { this.txtText.Text = value; }
		}
	}
}