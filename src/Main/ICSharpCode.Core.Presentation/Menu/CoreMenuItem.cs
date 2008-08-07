// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace ICSharpCode.Core.Presentation
{
	/// <summary>
	/// A menu item representing an AddIn-Tree element.
	/// </summary>
	class CoreMenuItem : MenuItem, IStatusUpdate
	{
		protected readonly Codon codon;
		protected readonly object caller;
		
		public CoreMenuItem(Codon codon, object caller)
		{
			this.codon = codon;
			this.caller = caller;
			
			if (codon.Properties.Contains("shortcut")) {
				InputGestureText = codon.Properties["shortcut"];
			}
			if (codon.Properties.Contains("icon")) {
				try {
					this.Icon = PresentationResourceService.GetImage(codon.Properties["icon"]);
				} catch (ResourceNotFoundException) {}
			}
			this.SubmenuOpened += CoreMenuItem_SubmenuOpened;
			UpdateText();
		}

		void CoreMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
		{
			MenuService.UpdateStatus(this.ItemsSource);
		}
		
		public void UpdateText()
		{
			if (codon != null) {
				Header = MenuService.ConvertLabel(StringParser.Parse(codon.Properties["label"]));
			}
		}
		
		public virtual void UpdateStatus()
		{
			this.IsEnabled = this.IsEnabledCore;
			if (this.IsEnabled) {
				this.Visibility = Visibility.Visible;
			} else {
				if (codon.GetFailedAction(caller) == ConditionFailedAction.Exclude)
					this.Visibility = Visibility.Collapsed;
				else
					this.Visibility = Visibility.Visible;
			}
		}
		
		protected override bool IsEnabledCore {
			get {
				ConditionFailedAction failedAction = codon.GetFailedAction(caller);
				bool isEnabled = failedAction == ConditionFailedAction.Nothing;
				return isEnabled;
			}
		}
	}
}
