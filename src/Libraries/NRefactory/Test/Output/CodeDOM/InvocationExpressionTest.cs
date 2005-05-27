/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 23.05.2005
 * Time: 19:06
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Collections;
using System.CodeDom;
using NUnit.Framework;
using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Tests.Output.CodeDom.Tests
{
	[TestFixture]
	public class InvocationExpressionsTests
	{
		[Test]
		public void IdentifierOnlyInvocation()
		{
			// InitializeComponents();
			IdentifierExpression identifier = new IdentifierExpression("InitializeComponents");
			InvocationExpression invocation = new InvocationExpression(identifier, new ArrayList());
			object output = invocation.AcceptVisitor(new CodeDOMVisitor(), null);
			Assert.IsTrue(output is CodeMethodInvokeExpression);
			CodeMethodInvokeExpression mie = (CodeMethodInvokeExpression)output;
			Assert.AreEqual("InitializeComponents", mie.Method.MethodName);
			Assert.IsTrue(mie.Method.TargetObject is CodeThisReferenceExpression);
		}
		
		[Test]
		public void MethodOnThisReferenceInvocation()
		{
			// InitializeComponents();
			FieldReferenceExpression field = new FieldReferenceExpression(new ThisReferenceExpression(), "InitializeComponents");
			InvocationExpression invocation = new InvocationExpression(field, new ArrayList());
			object output = invocation.AcceptVisitor(new CodeDOMVisitor(), null);
			Assert.IsTrue(output is CodeMethodInvokeExpression);
			CodeMethodInvokeExpression mie = (CodeMethodInvokeExpression)output;
			Assert.AreEqual("InitializeComponents", mie.Method.MethodName);
			Assert.IsTrue(mie.Method.TargetObject is CodeThisReferenceExpression);
		}
	}
}
