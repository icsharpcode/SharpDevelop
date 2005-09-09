// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.CodeDom;
using MbUnit.Framework;
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
