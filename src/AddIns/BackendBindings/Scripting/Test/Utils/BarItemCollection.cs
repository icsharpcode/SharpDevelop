// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class BarItem
	{
		string text = String.Empty;
		
		public BarItem()
		{
		}
		
		public BarItem(string text)
		{
			this.text = text;
		}
		
		public string Text {
			get { return text; }
			set { text = value; }
		}
	}
	
	public class BarItemCollection : Collection<BarItem>
	{
		public BarItemCollection()
		{
		}
		
		public void AddRange(BarItem[] items)
		{
			foreach (BarItem item in items) {
				Add(item);
			}
		}
	}
	
	public class BarItemCollectionParentComponent : Component
	{
		BarItemCollection barItems = new BarItemCollection();
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BarItemCollection ParentBarItems {
			get { return barItems; }
		}
	}
}
