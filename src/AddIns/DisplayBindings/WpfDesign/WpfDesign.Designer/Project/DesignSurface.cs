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
using System.Windows.Markup;
using System.Xml;
using ICSharpCode.WpfDesign.XamlDom;
using ICSharpCode.WpfDesign.Designer.Services;
using ICSharpCode.WpfDesign.Designer.Controls;

namespace ICSharpCode.WpfDesign.Designer
{
	/// <summary>
	/// Surface hosting the WPF designer.
	/// </summary>
	public sealed class DesignSurface : SingleVisualChildElement
	{
		readonly DefaultServiceProvider _services;
		readonly DefaultComponentService _componentService;
		readonly ScrollViewer _scrollViewer;
		readonly DesignPanel _designPanel;
		
		/// <summary>
		/// Create a new DesignSurface instance.
		/// </summary>
		public DesignSurface()
		{
			DesignServiceContainer serviceContainer = new DesignServiceContainer();
			_services = new DefaultServiceProvider(serviceContainer);
			
			serviceContainer.AddService(typeof(IVisualDesignService), new DefaultVisualDesignService());
			serviceContainer.AddService(typeof(ISelectionService), new DefaultSelectionService());
			serviceContainer.AddService(typeof(IToolService), new DefaultToolService());
			_componentService = new DefaultComponentService(this);
			serviceContainer.AddService(typeof(IComponentService), _componentService);
			
			_scrollViewer = new ScrollViewer();
			_designPanel = new DesignPanel(_services);
			_scrollViewer.Content = _designPanel;
			this.VisualChild = _scrollViewer;
		}
		
		/// <summary>
		/// Gets the service provider.
		/// </summary>
		public DefaultServiceProvider Services {
			get { return _services; }
		}
		
		/// <summary>
		/// Gets the designed element.
		/// </summary>
		public UIElement DesignedElement {
			get {
				return _designPanel.DesignedElement;
			}
		}
		
		/// <summary>
		/// Initializes the designer content from the specified XmlReader.
		/// </summary>
		public void LoadDesigner(XmlReader xamlReader)
		{
			UnloadDesigner();
			InitializeDesigner(XamlParser.Parse(xamlReader));
		}
		
		/// <summary>
		/// Saves the designer content into the specified XmlWriter.
		/// </summary>
		public void SaveDesigner(XmlWriter writer)
		{
			_currentDocument.Save(writer);
		}
		
		XamlDocument _currentDocument;
		
		void InitializeDesigner(XamlDocument document)
		{
			_currentDocument = document;
			DesignSite rootSite = _componentService.RegisterXamlComponentRecursive(document.RootElement);
			_designPanel.DesignedElement = DefaultVisualDesignService.CreateUIElementFor(rootSite);
		}
		
		/// <summary>
		/// Unloads the designer content.
		/// </summary>
		public void UnloadDesigner()
		{
			_currentDocument = null;
			UIElement designedElement = this.DesignedElement;
			if (designedElement != null) {
				_componentService.UnregisterAllComponents();
				_designPanel.DesignedElement = null;
			}
		}
	}
}
