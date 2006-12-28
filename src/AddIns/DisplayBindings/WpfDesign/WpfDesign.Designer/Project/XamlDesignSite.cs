// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.WpfDesign.XamlDom;

namespace ICSharpCode.WpfDesign.Designer
{
	sealed class XamlDesignSite : DesignSite
	{
		readonly XamlObject xamlObject;
		readonly DesignSurface designSurface;
		
		public XamlDesignSite(XamlObject xamlObject, DesignSurface designSurface)
		{
			this.xamlObject = xamlObject;
			this.designSurface = designSurface;
		}
		
		public override object Component {
			get {
				return xamlObject.Instance;
			}
		}
		
		public override object GetService(Type serviceType)
		{
			return designSurface.DefaultServiceProvider.GetService(serviceType);
		}
	}
}
