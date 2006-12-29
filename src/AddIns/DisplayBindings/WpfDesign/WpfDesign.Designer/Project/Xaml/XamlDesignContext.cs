// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlDesignContext : DesignContext
	{
		readonly XamlDocument _doc;
		readonly XamlDesignItem _rootItem;
		readonly XamlComponentService _componentService;
		
		public XamlDesignContext(XamlDocument doc)
		{
			if (doc == null)
				throw new ArgumentNullException("doc");
			this._doc = doc;
			
			this.Services.AddService(typeof(IVisualDesignService), new DefaultVisualDesignService());
			this.Services.AddService(typeof(ISelectionService), new DefaultSelectionService());
			this.Services.AddService(typeof(IToolService), new DefaultToolService());
			
			_componentService = new XamlComponentService(this);
			this.Services.AddService(typeof(IComponentService), _componentService);
			
			_rootItem = _componentService.RegisterXamlComponentRecursive(doc.RootElement);
		}
		
		public override void Save(System.Xml.XmlWriter writer)
		{
			_doc.Save(writer);
		}
		
		public override DesignItem RootItem {
			get { return _rootItem; }
		}
	}
}
