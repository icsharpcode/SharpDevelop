// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

using ICSharpCode.Core.Presentation;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Simple implementation of IOptionPanel with support for OptionBinding markup extensions.
	/// </summary>
	public class OptionPanel : UserControl, IOptionPanel, IOptionBindingContainer
	{
		static OptionPanel()
		{
			MarginProperty.OverrideMetadata(typeof(OptionPanel),
			                                new FrameworkPropertyMetadata(new Thickness(2, 0, 4, 0)));
		}
		
		public OptionPanel()
		{
			this.Resources.Add(
				typeof(GroupBox),
				new Style(typeof(GroupBox)) { Setters = {
						new Setter(GroupBox.PaddingProperty, new Thickness(3, 3, 3, 7))
					}});
			this.Resources.Add(typeof(CheckBox), GlobalStyles.WordWrapCheckBoxStyle);
			this.Resources.Add(typeof(RadioButton), GlobalStyles.WordWrapCheckBoxStyle);
		}
		
		public virtual object Owner { get; set; }
		
		readonly List<OptionBinding> bindings = new List<OptionBinding>();
		
		void IOptionBindingContainer.AddBinding(OptionBinding binding)
		{
			this.bindings.Add(binding);
		}
		
		public virtual object Control {
			get {
				return this;
			}
		}
		
		public virtual void LoadOptions()
		{
		}
		
		public virtual bool SaveOptions()
		{
			foreach (OptionBinding b in bindings) {
				if (!b.Save())
					return false;
			}
			
			return true;
		}
	}
}
