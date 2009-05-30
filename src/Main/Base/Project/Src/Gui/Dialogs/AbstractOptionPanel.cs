// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Siegfried Pammer" email="sie_pam@gmx.at"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public virtual object Owner { get; set; }
		
		IList<OptionBinding> bindings;
		
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
		
		public OptionPanel()
		{
			this.bindings = new List<OptionBinding>();
		}
		
		public void AddBinding(OptionBinding binding)
		{
			this.bindings.Add(binding);
		}
	}
}
