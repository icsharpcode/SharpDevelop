// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Oakland Software Incorporated" email="general@oaklandsoftware.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel;

namespace NoGoop.ObjBrowser.GuiDesigner
{
	public class DesignerContainer : IContainer
	{
		DesignerHost				_host;
		Container				   _container;
		ArrayList				   _sites;

		public DesignerContainer(DesignerHost host)
		{
			_host = host;
			_container = new Container();
			_sites = new ArrayList();
		}

		public ComponentCollection Components {
			get {
				return _container.Components;
			}
		}

		public void Add(IComponent comp)
		{
			_container.Add(comp);
		}

		public void Add(IComponent comp, String name)
		{
			Add(comp);
		}

		public void Remove(IComponent comp)
		{
			_container.Remove(comp);
		}

		public void Dispose()
		{
			_container.Dispose();
		}

		public Object GetService(Type type)
		{
			return _host.GetService(type);
		}

		public ISite CreateSite(IComponent comp, String name)
		{
			ISite site = new DesignerSite(_host, comp, this, name);
			Add(comp, name);
			comp.Site = site;
			_sites.Add(site);
			return site;
		}
	}
}
