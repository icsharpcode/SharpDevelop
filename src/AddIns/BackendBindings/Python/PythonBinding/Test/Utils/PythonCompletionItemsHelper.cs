// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections;
using System.Collections.Generic;
using ICSharpCode.SharpDevelop.Dom;

namespace PythonBinding.Tests.Utils
{
	public class PythonCompletionItemsHelper
	{
		delegate bool IsMatch(object item, string name);
		
		public static IMethod FindMethodFromCollection(string name, ICollection items)
		{
			return Find(name, items, IsMethodMatch) as IMethod;
		}
		
		static object Find(string name, ICollection items, IsMatch match)
		{
			foreach (object item in items) {
				if (match(item, name)) {
					return item;
				}
			}
			return null;
		}
		
		static bool IsMethodMatch(object obj, string name)
		{
			return IsMethodMatch(obj, name, -1);
		}
		
		static bool IsMethodMatch(object obj, string name, int parameterCount)
		{
			IMethod method = obj as IMethod;
			if (method != null) {
				if (method.Name == name) {
					if (parameterCount >= 0) {
						return method.Parameters.Count == parameterCount;
					}
					return true;
				}
			}
			return false;
		}
		
		public static List<IMethod> FindAllMethodsFromCollection(string name, int parameterCount, ICollection items)
		{
			List<IMethod> matchedMethods = new List<IMethod>();
			foreach (object item in items) {
				if (IsMethodMatch(item, name, parameterCount)) {
					matchedMethods.Add((IMethod)item);
				}
			}
			return matchedMethods;
		}
		
		public static List<IMethod> FindAllMethodsFromCollection(string name, ICollection items)
		{
			return FindAllMethodsFromCollection(name, -1, items);
		}
		
		public static IField FindFieldFromCollection(string name, ICollection items)
		{
			return Find(name, items, IsFieldMatch) as IField;
		}
		
		static bool IsFieldMatch(object obj, string name)
		{
			IField field = obj as IField;
			if (field != null) {
				return field.Name == name;
			}
			return false;
		}
		
		public static IClass FindClassFromCollection(string name, ICollection items)
		{
			return Find(name, items, IsClassMatch) as IClass;
		}
		
		static bool IsClassMatch(object obj, string name)
		{
			IClass c = obj as IClass;
			if (c != null) {
				return c.Name == name;
			}
			return false;
		}
	}
}
