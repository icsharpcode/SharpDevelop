// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Forms;

namespace ICSharpCode.Scripting.Tests.Utils
{
	public class FooItem
	{
		string text = String.Empty;
		
		public FooItem()
		{
		}
		
		public FooItem(string text)
		{
			this.text = text;
		}
		
		public string Text {
			get { return text; }
			set { text = value; }
		}
	}
	
	public class FooItemCollection : Collection<FooItem>
	{
		public FooItemCollection()
		{
		}
		
		public void AddRange(FooItem[] items)
		{
			foreach (FooItem item in items) {
				Add(item);
			}
		}
	}
	
	public class FooItemCollectionParentComponent : Component
	{
		FooItemCollection fooItems = new FooItemCollection();
		
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public FooItemCollection ParentFooItems {
			get { return fooItems; }
		}
	}
}
