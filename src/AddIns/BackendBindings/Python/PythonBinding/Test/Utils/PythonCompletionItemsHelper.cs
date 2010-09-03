// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
