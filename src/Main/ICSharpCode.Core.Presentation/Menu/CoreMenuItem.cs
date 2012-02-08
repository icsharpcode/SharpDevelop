// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A menu item representing an AddIn-Tree element.
	/// </summary>
	class CoreMenuItem : MenuItem, IStatusUpdate
	{
		protected readonly Codon codon;
		protected readonly object caller;
		protected readonly IEnumerable<ICondition> conditions;
		
		/// <summary>
		/// If true, UpdateStatus() sets the enabled flag.
		/// Used for type=Menu, but not for type=MenuItem - for menu items, Enabled is controlled by the WPF ICommand.
		/// </summary>
		internal bool SetEnabled;
		
		public CoreMenuItem(Codon codon, object caller, IEnumerable<ICondition> conditions)
		{
			this.codon = codon;
			this.caller = caller;
			this.conditions = conditions;
			
			if (codon.Properties.Contains("icon")) {
				try {
					var image = PresentationResourceService.GetImage(codon.Properties["icon"]);
					image.Height = 16;
					this.Icon = image;
				} catch (ResourceNotFoundException) {}
			}
			UpdateText();
		}

		public void UpdateText()
		{
			if (codon != null) {
				Header = MenuService.ConvertLabel(StringParser.Parse(codon.Properties["label"]));
			}
		}
		
		public virtual void UpdateStatus()
		{
			ConditionFailedAction result = Condition.GetFailedAction(conditions, caller);
			if (result == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
			if (SetEnabled)
				this.IsEnabled = result == ConditionFailedAction.Nothing;
		}
	}
}
