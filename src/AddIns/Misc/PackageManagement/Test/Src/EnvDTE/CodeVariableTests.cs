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
		
		void VariableEndsAtColumn(int column)
		{
			helper.VariableEndsAtColumn(column);
		}
		
		[Test]
		public void Access_PublicVariable_ReturnsPublic()
		{
			CreatePublicVariable("MyVariable");
			
			vsCMAccess access = codeVariable.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPublic, access);
		}
		
		[Test]
		public void Access_PrivateVariable_ReturnsPrivate()
		{
			CreatePrivateVariable("MyVariable");
			
			vsCMAccess access = codeVariable.Access;
			
			Assert.AreEqual(vsCMAccess.vsCMAccessPrivate, access);
		}
		
		[Test]
		public void GetStartPoint_VariableStartsAtColumn5_ReturnsTextPointWithLineCharOffset5()
		{
			CreatePublicVariable("MyVariable");
			VariableStartsAtColumn(5);
			
			TextPoint point = codeVariable.GetStartPoint();
			int offset = point.LineCharOffset;
			
			Assert.AreEqual(5, offset);
		}
		
		[Test]
		public void GetEndPoint_VariableEndsAtColumn10_ReturnsTextPointWithLineCharOffset10()
		{
			CreatePublicVariable("MyVariable");
			VariableEndsAtColumn(10);
			
			TextPoint point = codeVariable.GetEndPoint();
			int offset = point.LineCharOffset;
			
			Assert.AreEqual(10, offset);
		}
	}
}
