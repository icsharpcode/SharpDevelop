// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	sealed class DefaultComponentService : IComponentService
	{
		readonly DesignSurface _surface;
		
		public DefaultComponentService(DesignSurface surface)
		{
			this._surface = surface;
		}
		
		public event EventHandler<SiteEventArgs> ComponentRegistered;
		public event EventHandler<SiteEventArgs> ComponentUnregistered;
		
		Dictionary<object, XamlDesignSite> _sites = new Dictionary<object, XamlDesignSite>();
		
		public DesignSite GetSite(object component)
		{
			if (component == null)
				throw new ArgumentNullException("component");
			XamlDesignSite site;
			_sites.TryGetValue(component, out site);
			return site;
		}
		
		public DesignSite RegisterComponentForDesigner(object component)
		{
			if (component == null)
				throw new ArgumentNullException("component");
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// currently for use by UnregisterAllComponents only because it doesn't update the XAML
		/// </summary>
		void UnregisterComponentFromDesigner(DesignSite site)
		{
			if (site == null)
				throw new ArgumentNullException("site");
			
			if (!_sites.Remove(site.Component))
				throw new ArgumentException("The site was not registered here!");
			
			if (ComponentUnregistered != null) {
				ComponentUnregistered(this, new SiteEventArgs(site));
			}
		}
		
		/// <summary>
		/// registers components from an existing XAML tree
		/// </summary>
		internal XamlDesignSite RegisterXamlComponentRecursive(XamlObject obj)
		{
			if (obj == null) return null;
			
			foreach (XamlProperty prop in obj.Properties) {
				RegisterXamlComponentRecursive(prop.PropertyValue as XamlObject);
				foreach (XamlPropertyValue val in prop.CollectionElements) {
					RegisterXamlComponentRecursive(val as XamlObject);
				}
			}
			
			XamlDesignSite site = new XamlDesignSite(obj, _surface);
			_sites.Add(site.Component, site);
			if (ComponentRegistered != null) {
				ComponentRegistered(this, new SiteEventArgs(site));
			}
			return site;
		}
		
		/// <summary>
		/// unregisters all components
		/// </summary>
		internal void UnregisterAllComponents()
		{
			Array.ForEach(Linq.ToArray(_sites.Values), UnregisterComponentFromDesigner);
			_sites.Clear();
		}
	}
}
