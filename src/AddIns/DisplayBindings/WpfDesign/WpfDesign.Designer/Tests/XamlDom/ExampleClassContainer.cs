// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
	public class ExampleClassList : List<ExampleClass>
	{
	}
	
	[ContentProperty("List")]
	public class ExampleClassContainer : ExampleClass
	{
		List<ExampleClass> list = new List<ExampleClass>();
		
		public List<ExampleClass> List {
			get {
				TestHelperLog.Log("List.get " + Identity);
				return list;
			}
		}
		
		ExampleClassList otherList = new ExampleClassList();
		
		public ExampleClassList OtherList {
			get {
				TestHelperLog.Log("OtherList.get " + Identity);
				return otherList;
			}
			set {
				TestHelperLog.Log("OtherList.set " + Identity);
				otherList = value;
			}
		}
	}
}
