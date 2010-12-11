// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.PythonBinding;
using ICSharpCode.Scripting.Tests.Utils;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Dom.CSharp;
using NUnit.Framework;
using PythonBinding.Tests.Utils;
using UnitTestingUtils = UnitTesting.Tests.Utils;

namespace PythonBinding.Tests.Resolver
{
	[TestFixture]
	public class ResolveMemberWhenFieldHasNoReturnTypeTests
	{
		[Test]
		public void Resolve_FieldHasNoReturnType_DoesNotThrowNullReferenceException()
		{
			MockProjectContent projectContent = new MockProjectContent();
			UnitTestingUtils.MockClass c = new UnitTestingUtils.MockClass(projectContent, "Test");
			projectContent.SetClassToReturnFromGetClass("self", c);
			DefaultField field = c.AddField("randomNumber");
			field.ReturnType = null;
			ParseInformation parseInfo = new ParseInformation(c.CompilationUnit);
			
			ExpressionResult expression = new ExpressionResult("self.randomNumber.randint", ExpressionContext.Default);
			PythonClassResolver classResolver = new PythonClassResolver();
			PythonLocalVariableResolver localVariableResolver = new PythonLocalVariableResolver(classResolver);
			PythonMemberResolver resolver = new PythonMemberResolver(classResolver, localVariableResolver);
			
			PythonResolverContext context = new PythonResolverContext(parseInfo, expression, "class Test:\r\npass");
			Assert.DoesNotThrow(delegate { resolver.Resolve(context); });
		}
	}
}
