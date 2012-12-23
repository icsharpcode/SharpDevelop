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
	public class CodeVariableTests
	{
		CodeVariable codeVariable;
		FieldHelper helper;
		
		[SetUp]
		public void Init()
		{
			helper = new FieldHelper();
		}
				
		void CreatePublicVariable(string name)
		{
			helper.CreatePublicField(name);
			CreateVariable();
		}
		
		void CreatePrivateVariable(string name)
		{
			helper.CreatePrivateField(name);
			CreateVariable();
		}
		
		void CreateVariable()
		{
			codeVariable = new CodeVariable(helper.Field);
		}
		
		void VariableStartsAtColumn(int column)
		{
			helper.VariableStartsAtColumn(column);
		}
		
		void VariableStartsAtLine(int line)
		{
			helper.VariableStartsAtLine(line);
		}
		
		void VariableEndsAtColumn(int column)
		{
			helper.VariableEndsAtColumn(column);
		}
		
		void VariableEndsAtLine(int line)
		{
			helper.VariableEndsAtLine(line);
		}
		
		[Test]
		public void Access_PublicVariable_ReturnsPublic()
		{
			CreatePublicVariable("MyVariable");
			
			global::EnvDTE.vsCMAccess access = codeVariable.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateVariable_ReturnsPrivate()
		{
			CreatePrivateVariable("MyVariable");
			
			global::EnvDTE.vsCMAccess access = codeVariable.Access;
			
			Assert.AreEqual(global::EnvDTE.vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void GetStartPoint_VariableStartsAtColumn5_ReturnsTextPointWithLineCharOffset5()
		{
			CreatePublicVariable("MyVariable");
			VariableStartsAtColumn(5);
			
			global::EnvDTE.TextPoint point = codeVariable.GetStartPoint();
			int offset = point.LineCharOffset;
			
			Assert.AreEqual(5, offset);
		}
		
		[Test]
		public void GetStartPoint_VariableStartsAtLine1_ReturnsTextPointWithLine1()
		{
			CreatePublicVariable("MyVariable");
			VariableStartsAtLine(1);
			
			global::EnvDTE.TextPoint point = codeVariable.GetStartPoint();
			int line = point.Line;
			
			Assert.AreEqual(1, line);
		}
		
		[Test]
		public void GetEndPoint_VariableEndsAtColumn10_ReturnsTextPointWithLineCharOffset10()
		{
			CreatePublicVariable("MyVariable");
			VariableEndsAtColumn(10);
			
			global::EnvDTE.TextPoint point = codeVariable.GetEndPoint();
			int offset = point.LineCharOffset;
			
			Assert.AreEqual(10, offset);
		}
		
		[Test]
		public void GetEndPoint_VariableEndsAtLine2_ReturnsTextPointWithLine2()
		{
			CreatePublicVariable("MyVariable");
			VariableEndsAtLine(2);
			
			global::EnvDTE.TextPoint point = codeVariable.GetEndPoint();
			int line = point.Line;
			
			Assert.AreEqual(2, line);
		}
		
		[Test]
		public void Kind_PublicVariable_ReturnsVariable()
		{
			CreatePublicVariable("MyVariable");
			
			global::EnvDTE.vsCMElement kind = codeVariable.Kind;
			
			Assert.AreEqual(global::EnvDTE.vsCMElement.vsCMElementVariable, kind);
		}
	}
}
