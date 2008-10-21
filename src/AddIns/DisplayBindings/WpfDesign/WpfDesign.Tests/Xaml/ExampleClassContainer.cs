// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2421 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace ICSharpCode.WpfDesign.Tests.Xaml
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
