// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
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
			DefaultClass c = new DefaultClass(new DefaultCompilationUnit(new DefaultProjectContent()), "Test");
			DefaultMethod method = new DefaultMethod(c, "abc");
			
			ArrayList items = new ArrayList();
			items.Add(method);
			
			Assert.AreEqual(method, PythonCompletionItemsHelper.FindMethodFromArray("abc", items));
		}
		
		[Test]
		public void FindMethodFromArrayReturnsNullForUnknownMethod()
		{
			DefaultClass c = new DefaultClass(new DefaultCompilationUnit(new DefaultProjectContent()), "Test");
			DefaultMethod method = new DefaultMethod(c, "abc");
			
			ArrayList items = new ArrayList();
			items.Add(method);
			
			Assert.IsNull(PythonCompletionItemsHelper.FindMethodFromArray("unknown", items));
		}
		
		[Test]
		public void FindFieldFromArrayReturnsExpectedField()
		{
			DefaultClass c = new DefaultClass(new DefaultCompilationUnit(new DefaultProjectContent()), "Test");
			DefaultField field = new DefaultField(c, "field");
			
			ArrayList items = new ArrayList();
			items.Add(field);
			
			Assert.AreEqual(field, PythonCompletionItemsHelper.FindFieldFromArray("field", items));
		}
		
		[Test]
		public void FindFieldFromArrayReturnsExpectedNullForUnknownField()
		{
			DefaultClass c = new DefaultClass(new DefaultCompilationUnit(new DefaultProjectContent()), "Test");
			DefaultField field = new DefaultField(c, "field");
			
			ArrayList items = new ArrayList();
			items.Add(field);
			
			Assert.IsNull(PythonCompletionItemsHelper.FindFieldFromArray("unknown-field-name", items));
		}
		
		[Test]
		public void FindClassFromArrayReturnsExpectedClass()
		{
			DefaultClass c = new DefaultClass(new DefaultCompilationUnit(new DefaultProjectContent()), "Test");
			
			ArrayList items = new ArrayList();
			items.Add(c);
			
			Assert.AreEqual(c, PythonCompletionItemsHelper.FindClassFromArray("Test", items));
		}
		
		[Test]
		public void FindClassFromArrayReturnsExpectedNullForUnknownClassName()
		{
			DefaultClass c = new DefaultClass(new DefaultCompilationUnit(new DefaultProjectContent()), "Test");
			
			ArrayList items = new ArrayList();
			items.Add(c);
			
			Assert.IsNull(PythonCompletionItemsHelper.FindClassFromArray("unknown-class-name", items));
		}

	}
}
