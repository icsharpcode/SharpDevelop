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
		readonly DefaultServiceProvider _defaultServiceProvider;
		readonly ScrollViewer _scrollViewer;
		readonly DesignPanel _designPanel;
		
		/// <summary>
		/// Create a new DesignSurface instance.
		/// </summary>
		public DesignSurface()
		{
			DesignServiceContainer serviceContainer = new DesignServiceContainer();
			serviceContainer.AddService(typeof(IVisualDesignService), new DefaultVisualDesignService());
			serviceContainer.AddService(typeof(ISelectionService), new DefaultSelectionService());
			
			_defaultServiceProvider = new DefaultServiceProvider(serviceContainer);
			
			_scrollViewer = new ScrollViewer();
			_designPanel = new DesignPanel();
			_scrollViewer.Content = _designPanel;
			this.VisualChild = _scrollViewer;
		}
		
		/// <summary>
		/// Gets the service provider.
		/// </summary>
		public DefaultServiceProvider DefaultServiceProvider {
			get { return _defaultServiceProvider; }
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
		
		void InitializeDesigner(XamlDocument document)
		{
			DesignSite rootSite = new XamlDesignSite(document.RootElement, this);
			_designPanel.DesignedElement = DefaultVisualDesignService.CreateUIElementFor(rootSite);
		}
		
		/// <summary>
		/// Unloads the designer content.
		/// </summary>
		public void UnloadDesigner()
		{
			UIElement designedElement = this.DesignedElement;
			if (designedElement != null) {
				_designPanel.DesignedElement = null;
				
			}
		}
	}
}
