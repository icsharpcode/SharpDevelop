// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.WpfDesign.Designer.Services
{
	sealed class DefaultViewService : ViewService
	{
		readonly DesignContext context;
		
		public DefaultViewService(DesignContext context)
		{
			this.context = context;
		}
		
		public override DesignItem GetModel(System.Windows.DependencyObject view)
		{
			// In the WPF designer, we do not support having a different view for a component
			return context.Services.Component.GetDesignItem(view);
		}
	}
}
