// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PythonBinding.Tests.Utils
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
}
