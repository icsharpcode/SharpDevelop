// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
