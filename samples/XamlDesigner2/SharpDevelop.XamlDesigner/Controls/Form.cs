using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace SharpDevelop.XamlDesigner.Controls
{
	public class Form : ItemsControl
	{
		public Form()
		{
			SetResourceReference(ItemContainerStyleProperty, FormItem.DefaultStyleKey);
			Grid.SetIsSharedSizeScope(this, true);
		}
	}
}
