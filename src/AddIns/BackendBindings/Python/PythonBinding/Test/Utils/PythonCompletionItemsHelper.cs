// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using ICSharpCode.SharpDevelop.Dom;

namespace PythonBinding.Tests.Utils
{
	public class PythonCompletionItemsHelper
	{
		public static IMethod FindMethodFromArray(string name, ArrayList items)
		{
			foreach (object item in items) {
				IMethod method = item as IMethod;
				if (method != null) {
					if (method.Name == name) {
						return method;
					}
				}
			}
			return null;
		}
		
		public static IField FindFieldFromArray(string name, ArrayList items)
		{
			foreach (object item in items) {
				IField field = item as IField;
				if (field != null) {
					if (field.Name == name) {
						return field;
					}
				}
			}
			return null;
		}
		
		public static IClass FindClassFromArray(string name, ArrayList items)
		{
			foreach (object item in items) {
				IClass c = item as IClass;
				if (c != null) {
					if (c.Name == name) {
						return c;
					}
				}
			}
			return null;
		}
	}
}
