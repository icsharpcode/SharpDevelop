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
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Utils.Tests
{
	[TestFixture]
	public class PythonCompletionItemsHelperTests
	{
		[Test]
		public void FindMethodFromArrayReturnsExpectedMethod()
		{
			DefaultClass c = CreateClass();
			DefaultMethod method = new DefaultMethod(c, "abc");
			
			ArrayList items = new ArrayList();
			items.Add(method);
			
			Assert.AreEqual(method, PythonCompletionItemsHelper.FindMethodFromCollection("abc", items));
		}
		
		DefaultClass CreateClass()
		{
			return new DefaultClass(new DefaultCompilationUnit(new DefaultProjectContent()), "Test");
		}
		
		[Test]
		public void FindMethodFromArrayReturnsNullForUnknownMethod()
		{
			DefaultClass c = new DefaultClass(new DefaultCompilationUnit(new DefaultProjectContent()), "Test");
			DefaultMethod method = new DefaultMethod(c, "abc");
			
			ArrayList items = new ArrayList();
			items.Add(method);
			
			Assert.IsNull(PythonCompletionItemsHelper.FindMethodFromCollection("unknown", items));
		}
		
		[Test]
		public void FindFieldFromArrayReturnsExpectedField()
		{
			DefaultClass c = CreateClass();
			DefaultField field = new DefaultField(c, "field");
			
			ArrayList items = new ArrayList();
			items.Add(field);
			
			Assert.AreEqual(field, PythonCompletionItemsHelper.FindFieldFromCollection("field", items));
		}
		
		[Test]
		public void FindFieldFromArrayReturnsExpectedNullForUnknownField()
		{
			DefaultClass c = CreateClass();
			DefaultField field = new DefaultField(c, "field");
			
			ArrayList items = new ArrayList();
			items.Add(field);
			
			Assert.IsNull(PythonCompletionItemsHelper.FindFieldFromCollection("unknown-field-name", items));
		}
		
		[Test]
		public void FindClassFromArrayReturnsExpectedClass()
		{
			DefaultClass c = CreateClass();
			
			ArrayList items = new ArrayList();
			items.Add(c);
			
			Assert.AreEqual(c, PythonCompletionItemsHelper.FindClassFromCollection("Test", items));
		}
		
		[Test]
		public void FindClassFromArrayReturnsExpectedNullForUnknownClassName()
		{
			DefaultClass c = CreateClass();
			
			ArrayList items = new ArrayList();
			items.Add(c);
			
			Assert.IsNull(PythonCompletionItemsHelper.FindClassFromCollection("unknown-class-name", items));
		}
		
		[Test]
		public void FindAllMethodsFromArrayReturnsExpectedMethods()
		{
			DefaultClass c = CreateClass();
			DefaultMethod method1 = new DefaultMethod(c, "abc");
			DefaultMethod method2 = new DefaultMethod(c, "abc");
			DefaultMethod method3 = new DefaultMethod(c, "def");
			
			ArrayList items = new ArrayList();
			items.Add(method1);
			items.Add(method2);
			items.Add(method3);
			
			List<IMethod> expectedMethods = new List<IMethod>();
			expectedMethods.Add(method1);
			expectedMethods.Add(method2);
			
			List<IMethod> methods = PythonCompletionItemsHelper.FindAllMethodsFromCollection("abc", items);
			Assert.AreEqual(expectedMethods, methods);
		}
		
		[Test]
		public void FindAllMethodsFromArrayWithParameterCountReturnsExpectedMethods()
		{
			DefaultClass c = CreateClass();
			DefaultMethod method1 = new DefaultMethod(c, "abc");
			method1.Parameters.Add(CreateParameter("a"));
			
			DefaultMethod method2 = new DefaultMethod(c, "abc");
			method2.Parameters.Add(CreateParameter("a"));
			method2.Parameters.Add(CreateParameter("b"));
			
			DefaultMethod method3 = new DefaultMethod(c, "abc");
			method3.Parameters.Add(CreateParameter("c"));
			
			ArrayList items = new ArrayList();
			items.Add(method1);
			items.Add(method2);
			items.Add(method3);
			
			List<IMethod> expectedMethods = new List<IMethod>();
			expectedMethods.Add(method1);
			expectedMethods.Add(method3);
			
			int parameterCount = 1;
			List<IMethod> methods = PythonCompletionItemsHelper.FindAllMethodsFromCollection("abc", parameterCount, items);
			Assert.AreEqual(expectedMethods, methods);
		}
		
		DefaultParameter CreateParameter(string name)
		{
			DefaultReturnType returnType = new DefaultReturnType(CreateClass());
			DomRegion region = new DomRegion(1, 1);
			return new DefaultParameter(name, returnType, region);
		}
	}
}
