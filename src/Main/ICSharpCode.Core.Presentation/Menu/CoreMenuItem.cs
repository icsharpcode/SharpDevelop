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
		
		public CoreMenuItem(Codon codon, object caller)
		{
			this.codon = codon;
			this.caller = caller;
			
			if (codon.Properties.Contains("icon")) {
				try {
					var image = PresentationResourceService.GetImage(codon.Properties["icon"]);
					image.Height = 16;
					this.Icon = new PixelSnapper(image);
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
			if (codon.GetFailedAction(caller) == ConditionFailedAction.Exclude)
				this.Visibility = Visibility.Collapsed;
			else
				this.Visibility = Visibility.Visible;
		}
	}
}
