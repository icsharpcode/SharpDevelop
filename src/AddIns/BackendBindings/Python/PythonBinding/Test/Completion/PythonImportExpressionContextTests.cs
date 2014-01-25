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
using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class PythonImportExpressionContextTests
	{
		[Test]
		public void ShowEntry_IMethodPassed_ReturnsTrue()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			context.HasFromAndImport = true;
			
			IMethod method = CreateMethod();
			bool result = context.ShowEntry(method);
			
			Assert.IsTrue(result);
		}
		
		IMethod CreateMethod()
		{
			IClass c = CreateClass();
			return new DefaultMethod(c, "Test");
		}
		
		IClass CreateClass()
		{
			DefaultCompilationUnit unit = new DefaultCompilationUnit(new MockProjectContent());
			return new DefaultClass(unit, "ICSharpCode.MyClass");
		}
		
		[Test]
		public void ShowEntry_PassedNull_ReturnsFalse()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			context.HasFromAndImport = true;
			bool result = context.ShowEntry(null);
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ShowEntry_PassedIMethodWhenHasFromAndImportIsFalse_ReturnsFalse()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			IMethod method = CreateMethod();
			bool result = context.ShowEntry(method);
			Assert.IsFalse(result);
		}
		
		[Test]
		public void HasFromAndImport_NewPythonImportExpressionContextInstance_IsFalseByDefault()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			bool result = context.HasFromAndImport;
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ShowEntry_PassedIField_ReturnsTrue()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			context.HasFromAndImport = true;
			IField field = CreateField();
			bool result = context.ShowEntry(field);
			
			Assert.IsTrue(result);
		}
		
		IField CreateField()
		{
			IClass c = CreateClass();
			return new DefaultField(c, "field");
		}

		[Test]
		public void ShowEntry_PassedIFieldWhenHasFromAndImportIsFalse_ReturnsFalse()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();	
			IField field = CreateField();
			bool result = context.ShowEntry(field);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ShowEntry_PassedIClass_ReturnsTrue()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();	
			context.HasFromAndImport = true;
			IClass c = CreateClass();
			bool result = context.ShowEntry(c);
			
			Assert.IsTrue(result);
		}
		
		[Test]
		public void ShowEntry_PassedIClassWhenHasFromAndImportIsFalse_ReturnsFalse()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();	
			IClass c = CreateClass();
			bool result = context.ShowEntry(c);
			
			Assert.IsFalse(result);
		}
		
		[Test]
		public void ShowEntry_PassedImportAllNamespaceEntryWhenHasFromAndImportIsTrue_ReturnsTrue()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			context.HasFromAndImport = true;
			NamespaceEntry entry = new NamespaceEntry("*");
			bool result = context.ShowEntry(entry);
			
			Assert.IsTrue(result);
		}
	}
}
