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

//using System;
//using ICSharpCode.PackageManagement.EnvDTE;
//using ICSharpCode.SharpDevelop.Dom;
//using NUnit.Framework;
//using PackageManagement.Tests.Helpers;
//using Rhino.Mocks;
//
//namespace PackageManagement.Tests.EnvDTE
//{
//	[TestFixture]
//	public class CodeVariableTests
//	{
//		CodeVariable codeVariable;
//		FieldHelper helper;
//		
//		[SetUp]
//		public void Init()
//		{
//			helper = new FieldHelper();
//		}
//				
//		void CreatePublicVariable(string name)
//		{
//			helper.CreatePublicField(name);
//			CreateVariable();
//		}
//		
//		void CreatePrivateVariable(string name)
//		{
//			helper.CreatePrivateField(name);
//			CreateVariable();
//		}
//		
//		void CreateVariable()
//		{
//			codeVariable = new CodeVariable(helper.Field);
//		}
//		
//		void VariableStartsAtColumn(int column)
//		{
//			helper.VariableStartsAtColumn(column);
//		}
//		
//		void VariableStartsAtLine(int line)
//		{
//			helper.VariableStartsAtLine(line);
//		}
//		
//		void VariableEndsAtColumn(int column)
//		{
//			helper.VariableEndsAtColumn(column);
//		}
//		
//		void VariableEndsAtLine(int line)
//		{
//			helper.VariableEndsAtLine(line);
//		}
//		
//		[Test]
//		public void Access_PublicVariable_ReturnsPublic()
//		{
//			CreatePublicVariable("MyVariable");
//			
//			global::EnvDTE.vsCMAccess access = codeVariable.Access;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
//		}
//		
//		[Test]
//		public void Access_PrivateVariable_ReturnsPrivate()
//		{
//			CreatePrivateVariable("MyVariable");
//			
//			global::EnvDTE.vsCMAccess access = codeVariable.Access;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
//		}
//		
//		[Test]
//		public void GetStartPoint_VariableStartsAtColumn5_ReturnsTextPointWithLineCharOffset5()
//		{
//			CreatePublicVariable("MyVariable");
//			VariableStartsAtColumn(5);
//			
//			global::EnvDTE.TextPoint point = codeVariable.GetStartPoint();
//			int offset = point.LineCharOffset;
//			
//			Assert.AreEqual(5, offset);
//		}
//		
//		[Test]
//		public void GetStartPoint_VariableStartsAtLine1_ReturnsTextPointWithLine1()
//		{
//			CreatePublicVariable("MyVariable");
//			VariableStartsAtLine(1);
//			
//			global::EnvDTE.TextPoint point = codeVariable.GetStartPoint();
//			int line = point.Line;
//			
//			Assert.AreEqual(1, line);
//		}
//		
//		[Test]
//		public void GetEndPoint_VariableEndsAtColumn10_ReturnsTextPointWithLineCharOffset10()
//		{
//			CreatePublicVariable("MyVariable");
//			VariableEndsAtColumn(10);
//			
//			global::EnvDTE.TextPoint point = codeVariable.GetEndPoint();
//			int offset = point.LineCharOffset;
//			
//			Assert.AreEqual(10, offset);
//		}
//		
//		[Test]
//		public void GetEndPoint_VariableEndsAtLine2_ReturnsTextPointWithLine2()
//		{
//			CreatePublicVariable("MyVariable");
//			VariableEndsAtLine(2);
//			
//			global::EnvDTE.TextPoint point = codeVariable.GetEndPoint();
//			int line = point.Line;
//			
//			Assert.AreEqual(2, line);
//		}
//		
//		[Test]
//		public void Kind_PublicVariable_ReturnsVariable()
//		{
//			CreatePublicVariable("MyVariable");
//			
//			global::EnvDTE.vsCMElement kind = codeVariable.Kind;
//			
//			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementVariable, kind);
//		}
//	}
//}
