// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer
{
	sealed class XamlDesignItem : DesignItem
	{
		readonly XamlObject xamlObject;
		readonly DesignSurface designSurface;
		UIElement _view;
		
		public XamlDesignItem(XamlObject xamlObject, DesignSurface designSurface)
		{
			this.xamlObject = xamlObject;
			this.designSurface = designSurface;
		}
		
		public override object Component {
			get {
				return xamlObject.Instance;
			}
		}
		
		public override UIElement View {
			get {
				if (_view != null)
					return _view;
				else
					return this.Component as UIElement;
			}
		}
		
		internal void SetView(UIElement newView)
		{
			_view = newView;
		}
		
		public override object GetService(Type serviceType)
		{
			return designSurface.Services.GetService(serviceType);
		}
	}
}
