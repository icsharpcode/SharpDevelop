// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows;

using ICSharpCode.Core.Presentation;

namespace ICSharpCode.AvalonEdit.AddIn.Commands
{
	public partial class SortOptionsDialog : Window, IOptionBindingContainer
	{
		public SortOptionsDialog()
		{
			InitializeComponent();
		}
		
		List<OptionBinding> bindings = new List<OptionBinding>();
		
		public void AddBinding(OptionBinding binding)
		{
			bindings.Add(binding);
		}
		
		void okButton_Click(object sender, RoutedEventArgs e)
		{
			foreach (OptionBinding b in bindings)
				if (!b.Save())
					return;
			DialogResult = true;
			Close();
		}
	}
}