// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Extensions;

namespace ICSharpCode.WpfDesign.Designer.Xaml
{
	sealed class XamlDesignContext : DesignContext
	{
		readonly XamlDocument _doc;
		readonly XamlDesignItem _rootItem;
		readonly XamlComponentService _componentService;
		
		public XamlDesignContext(XmlReader xamlReader)
		{
			if (xamlReader == null)
				throw new ArgumentNullException("xamlReader");
			
			this.Services.AddService(typeof(IVisualDesignService), new DefaultVisualDesignService());
			this.Services.AddService(typeof(ISelectionService), new DefaultSelectionService());
			this.Services.AddService(typeof(IToolService), new DefaultToolService());
			
			_componentService = new XamlComponentService(this);
			this.Services.AddService(typeof(IComponentService), _componentService);
			
			// register extensions from this assembly:
			this.Services.ExtensionManager.RegisterAssembly(typeof(XamlDesignContext).Assembly);
			
			XamlParserSettings xamlParseSettings = new XamlParserSettings();
			xamlParseSettings.CreateInstanceCallback = OnXamlParserCreateInstance;
			_doc = XamlParser.Parse(xamlReader, xamlParseSettings);
			_rootItem = _componentService.RegisterXamlComponentRecursive(_doc.RootElement);
		}
		
		object OnXamlParserCreateInstance(Type instanceType, object[] arguments)
		{
			foreach (Type extensionType in this.Services.ExtensionManager.GetExtensionTypes(instanceType)) {
				if (typeof(CustomInstanceFactory).IsAssignableFrom(extensionType)) {
					CustomInstanceFactory factory = (CustomInstanceFactory)Activator.CreateInstance(extensionType);
					return factory.CreateInstance(instanceType, arguments);
				}
			}
			return CustomInstanceFactory.DefaultInstanceFactory.CreateInstance(instanceType, arguments);
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
