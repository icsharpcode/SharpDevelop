// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.PythonBinding;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PythonBinding.Tests.Utils;

namespace PythonBinding.Tests.Completion
{
	[TestFixture]
	public class PythonImportExpressionContextTestFixture
	{
		[Test]
		public void ShowEntryReturnsTrueForIMethod()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			context.HasFromAndImport = true;
			
			IMethod method = CreateMethod();
			
			Assert.IsTrue(context.ShowEntry(method));
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
		public void ShowEntryReturnsFalseForProjectContent()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			context.HasFromAndImport = true;

			MockProjectContent projectContent = new MockProjectContent();
			Assert.IsFalse(context.ShowEntry(projectContent));
		}
		
		[Test]
		public void ShowEntryReturnsFalseForIMethodWhenHasFromAndImportIsFalse()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			IMethod method = CreateMethod();
			Assert.IsFalse(context.ShowEntry(method));
		}
		
		[Test]
		public void PythonImportExpressionContextHasFromAndImportIsFalseByDefault()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			Assert.IsFalse(context.HasFromAndImport);
		}
		
		[Test]
		public void ShowEntryReturnsTrueForIField()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();
			context.HasFromAndImport = true;
			IField field = CreateField();
			
			Assert.IsTrue(context.ShowEntry(field));
		}
		
		IField CreateField()
		{
			IClass c = CreateClass();
			return new DefaultField(c, "field");
		}

		[Test]
		public void ShowEntryReturnsFalseForIFieldWhenHasFromAndImportIsFalse()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();	
			IField field = CreateField();
			
			Assert.IsFalse(context.ShowEntry(field));
		}
		
		[Test]
		public void ShowEntryReturnsTrueForIClass()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();	
			context.HasFromAndImport = true;
			IClass c = CreateClass();
			
			Assert.IsTrue(context.ShowEntry(c));
		}
		
		[Test]
		public void ShowEntryReturnsFalseForIClassWhenHasFromAndImportIsFalse()
		{
			PythonImportExpressionContext context = new PythonImportExpressionContext();	
			IClass c = CreateClass();
			
			Assert.IsFalse(context.ShowEntry(c));
		}
	}
}
