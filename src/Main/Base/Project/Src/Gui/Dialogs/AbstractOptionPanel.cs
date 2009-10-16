// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

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
		
		public virtual object Owner { get; set; }
		
		IList<OptionBinding> bindings = new List<OptionBinding>();
		
		public IList<OptionBinding> Bindings {
			get {
				return bindings;
			}
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
			foreach (OptionBinding b in Bindings) {
				if (!b.Save())
					return false;
			}
			
			return true;
		}
		
		public void AddBinding(OptionBinding binding)
		{
			this.bindings.Add(binding);
		}
	}
}
