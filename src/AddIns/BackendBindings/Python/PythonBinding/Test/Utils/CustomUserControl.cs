// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace PythonBinding.Tests.Utils
{
	public class CustomUserControl : UserControl
	{
		FooItemCollection fooItems = new FooItemCollection();
		BarItemCollectionParentComponent component = new BarItemCollectionParentComponent();
		
		public CustomUserControl()
		{
		}
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FooItemCollection FooItems {
			get { return fooItems; }
		}
				
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarItemCollectionParentComponent ParentComponent {
			get { return component; }
		}
	}
}
