// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Tests.XamlDom
{
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
	}
}
