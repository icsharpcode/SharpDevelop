// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	sealed class DefaultVisualDesignService : IVisualDesignService
	{
		public UIElement CreateWrapper(DesignSite site)
		{
			if (site == null)
				throw new ArgumentNullException("site");
			
			object obj = site.Component;
			
			if (obj is UIElement)
				return null;
			else
				return new FallbackObjectWrapper(site);
		}
		
		internal static UIElement CreateUIElementFor(DesignSite site)
		{
			UIElement element = site.Services.VisualDesign.CreateWrapper(site);
			if (element != null) {
				if (!(element is IVisualDesignObjectWrapper)) {
					throw new DesignerException("IVisualDesignService.CreateWrapper must return null or UIElement implementing IVisualDesignObjectWrapper");
				}
			} else {
				element = site.Component as UIElement;
				if (element == null) {
					throw new DesignerException("IVisualDesignService.CreateWrapper may not return null if site.Component is no UIElement");
				}
			}
			return element;
		}
	}
	
	sealed class FallbackObjectWrapper : ContentControl, IVisualDesignObjectWrapper
	{
		DesignSite _site;
		
		public FallbackObjectWrapper(DesignSite site)
		{
			this._site = site;
			
			this.BorderThickness = new Thickness(1);
			this.BorderBrush = Brushes.Black;
			this.Background = Brushes.White;
			this.Foreground = Brushes.Black;
			this.Content = site.Component;
		}
		
		public DesignSite WrappedSite {
			get { return _site; }
		}
	}
}
