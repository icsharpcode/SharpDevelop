// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.XamlDom.Tests
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
