// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.Refactoring;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class EditPointTests
	{
		FieldHelper fieldHelper;
		MethodHelper methodHelper;
		TextPoint endPoint;
		EditPoint editPoint;
		IRefactoringDocument document;
		IDocumentLoader documentLoader;
		
		[SetUp]
		public void Init()
		{
			fieldHelper = new FieldHelper();
			methodHelper = new MethodHelper();
			CreateDocumentLoader();
		}
		
		void CreateDocumentLoader()
		{
			document = MockRepository.GenerateStub<IRefactoringDocument>();
			documentLoader = MockRepository.GenerateStub<IDocumentLoader>();
		}

		void CreateField(string fileName, DomRegion region)
		{
			fieldHelper.CreateField("Class1.MyField");
			fieldHelper.SetRegion(region);
			fieldHelper.SetCompilationUnitFileName(fileName);
		}
		
		void CreateMethod(string fileName, DomRegion region, DomRegion bodyRegion)
		{
			methodHelper.CreateMethod("Class1.MyMethod");
			methodHelper.SetRegion(region);
			methodHelper.SetBodyRegion(bodyRegion);
			methodHelper.SetCompilationUnitFileName(fileName);
		}
		
		void DocumentOffsetToReturn(int line, int column, int offset)
		{
			document.Stub(d => d.PositionToOffset(line, column)).Return(offset);
		}
		
		void CreateFieldEditPoint()
		{
			var codeVariable = new CodeVariable(fieldHelper.Field, documentLoader);
			TextPoint startPoint = codeVariable.GetStartPoint();
			endPoint = codeVariable.GetEndPoint();
			editPoint = startPoint.CreateEditPoint();
		}
		
		void CreateMethodEditPoint()
		{
			var codeFunction = new CodeFunction(methodHelper.Method, documentLoader);
			TextPoint startPoint = codeFunction.GetStartPoint();
			endPoint = codeFunction.GetEndPoint();
			editPoint = startPoint.CreateEditPoint();
		}
		
		void ReplaceText(string text)
		{
			editPoint.ReplaceText(endPoint, text, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
		}
		
		void DocumentFileName(string fileName)
		{
			documentLoader.Stub(loader => loader.LoadRefactoringDocument(fileName)).Return(document);
		}
		
		[Test]
		public void ReplaceText_FieldEndPointCreatedFromStartPoint_ReplacesTextBetweenStartAndEndPoint()
		{
			string fileName = @"d:\projects\test.cs";
			var fieldRegion = new DomRegion(1, 5, 3, 12);
			CreateField(fileName, fieldRegion);
			DocumentOffsetToReturn(line: 1, column: 5, offset: 5);
			DocumentOffsetToReturn(line: 3, column: 12, offset: 20);
			DocumentFileName(fileName);
			CreateFieldEditPoint();
			
			ReplaceText("Test");
			
			document.AssertWasCalled(d => d.Replace(5, 15, "Test"));
		}
		
		[Test]
		public void ReplaceText_MethodEndPointCreatedFromStartPoint_ReplacesTextBetweenStartAndEndPoint()
		{
			string fileName = @"d:\projects\test.cs";
			var methodRegion = new DomRegion(1, 5, 1, 10);
			var methodBodyRegion = new DomRegion(1, 10, 3, 12);
			CreateMethod(fileName, methodRegion, methodBodyRegion);
			methodHelper.AddDeclaringType("MyClass");
			DocumentOffsetToReturn(line: 1, column: 5, offset: 5);
			DocumentOffsetToReturn(line: 3, column: 12, offset: 20);
			DocumentFileName(fileName);
			CreateMethodEditPoint();
			
			ReplaceText("Test");
			
			document.AssertWasCalled(d => d.Replace(5, 15, "Test"));
		}
	}
}
