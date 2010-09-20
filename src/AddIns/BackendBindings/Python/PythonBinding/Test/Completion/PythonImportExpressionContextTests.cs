// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
