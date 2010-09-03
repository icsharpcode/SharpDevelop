// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.Scripting.Tests.Utils
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
