// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.PackageManagement.EnvDTE;
using ICSharpCode.SharpDevelop.Dom;
using NUnit.Framework;
using PackageManagement.Tests.Helpers;
using Rhino.Mocks;

namespace PackageManagement.Tests.EnvDTE
{
	[TestFixture]
	public class CodeFunctionTests
	{
		CodeFunction codeFunction;
		MethodHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new MethodHelper();
		}
		
		void CreatePublicFunction(string name)
		{
			helper.CreatePublicMethod(name);
			CreateFunction();
		}
		
		void CreatePrivateFunction(string name)
		{
			helper.CreatePrivateMethod(name);
			CreateFunction();
		}
		
		void CreateFunction()
		{
			codeFunction = new CodeFunction(helper.Method);
		}
		
		void SetDeclaringTypeAsInterface(string name)
		{
			helper.AddDeclaringTypeAsInterface(name);
		}
		
		void SetDeclaringType(string name)
		{
			helper.AddDeclaringType(name);
		}
		
		[Test]
		public void Access_PublicFunction_ReturnsPublic()
		{
			CreatePublicFunction("Class1.MyFunction");
			
			vsCMAccess access = codeFunction.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateFunction_ReturnsPrivate()
		{
			CreatePrivateFunction("Class1.MyFunction");
			
			vsCMAccess access = codeFunction.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void GetStartPoint_FunctionStartsAtColumn3_ReturnsPointWithOffset3()
		{
			CreatePublicFunction("Class1.MyFunction");
			SetDeclaringType("Class1");
			helper.FunctionStartsAtColumn(3);
			
			TextPoint point = codeFunction.GetStartPoint();
			int offset = point.LineCharOffset;
			
			Assert.AreEqual(3, offset);
		}
		
		[Test]
		public void GetStartPoint_FunctionStartsAtLine2_ReturnsPointWithLine2()
		{
			CreatePublicFunction("Class1.MyFunction");
			SetDeclaringType("Class1");
			helper.FunctionStartsAtLine(2);
			
			TextPoint point = codeFunction.GetStartPoint();
			int line = point.Line;
			
			Assert.AreEqual(2, line);
		}
		
		[Test]
		public void GetEndPoint_FunctionBodyEndsAtColumn4_ReturnsPointWithOffset4()
		{
			CreatePublicFunction("Class1.MyFunction");
			SetDeclaringType("Class1");
			helper.FunctionBodyEndsAtColumn(4);
			
			TextPoint point = codeFunction.GetEndPoint();
			int offset = point.LineCharOffset;
			
			Assert.AreEqual(4, offset);
		}
		
		[Test]
		public void GetEndPoint_FunctionBodyEndsAtLine4_ReturnsPointWithLine4()
		{
			CreatePublicFunction("Class1.MyFunction");
			SetDeclaringType("Class1");
			helper.FunctionBodyEndsAtLine(4);
			
			TextPoint point = codeFunction.GetEndPoint();
			int line = point.Line;
			
			Assert.AreEqual(4, line);
		}
		
		[Test]
		public void Kind_PublicFunction_ReturnsFunction()
		{
			CreatePublicFunction("MyFunction");
			
			vsCMElement kind = codeFunction.Kind;
			
			Assert.AreEqual(vsCMElement.vsCMElementFunction, kind);
		}
				
		[Test]
		public void GetEndPoint_InterfaceMethod_ReturnsMethodsDeclarationRegionEndNotBodyRegionEnd()
		{
			CreatePublicFunction("MyInterface.MyMethod");
			SetDeclaringTypeAsInterface("MyInterface");
			helper.SetRegion(new DomRegion(1, 1, 1, 10));
			helper.SetBodyRegion(new DomRegion(1, 10, 0, 0));
			
			TextPoint point = codeFunction.GetEndPoint();
			
			Assert.AreEqual(1, point.Line);
			Assert.AreEqual(10, point.LineCharOffset);
		}
	}
}
