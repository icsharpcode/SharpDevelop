// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;

[assembly: ICSharpCode.NRefactory.TypeSystem.TestCase.TypeTestAttribute(
	42, typeof(System.Action<>), typeof(IDictionary<string, IList<NUnit.Framework.TestAttribute>>))]

namespace ICSharpCode.NRefactory.TypeSystem.TestCase
{
	public class SimplePublicClass
	{
		public void Method() {}
	}
	
	public class TypeTestAttribute : Attribute
	{
		public TypeTestAttribute(int a1, Type a2, Type a3) {}
	}
	
	public class DynamicTest
	{
		public List<dynamic> DynamicGenerics1(Action<object, dynamic[], object> param) { return null; }
		public void DynamicGenerics2(Action<object, dynamic, object> param) { }
		public void DynamicGenerics3(Action<int, dynamic, object> param) { }
		public void DynamicGenerics4(Action<int[], dynamic, object> param) { }
		public void DynamicGenerics5(Action<int*[], dynamic, object> param) { }
	}
}
