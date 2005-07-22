// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using ICSharpCode.NRefactory.Parser;
using ICSharpCode.NRefactory.Parser.VB;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.NRefactory.Parser
{
	/// <summary>
	/// This class converts VB.NET constructs to their C# equivalents.
	/// </summary>
	public class VBNetToCSharpConvertVisitor : AbstractASTVisitor
	{
		// The following conversions are implemented:
		//   MyBase.New() and MyClass.New() calls inside the constructor are converted to :base() and :this()
		
		// The following conversions should be implemented in the future:
		//   Public Event EventName(param As String) -> automatic delegate declaration
		//   Function A() \n A = SomeValue \n End Function -> convert to return statement
		
		public override object Visit(ConstructorDeclaration constructorDeclaration, object data)
		{
			// MyBase.New() and MyClass.New() calls inside the constructor are converted to :base() and :this()
			BlockStatement body = constructorDeclaration.Body;
			if (body != null && body.Children.Count > 0) {
				StatementExpression se = body.Children[0] as StatementExpression;
				if (se != null) {
					InvocationExpression ie = se.Expression as InvocationExpression;
					if (ie != null) {
						FieldReferenceExpression fre = ie.TargetObject as FieldReferenceExpression;
						if (fre != null && "New".Equals(fre.FieldName, StringComparison.InvariantCultureIgnoreCase)) {
							if (fre.TargetObject is BaseReferenceExpression || fre.TargetObject is ClassReferenceExpression) {
								body.Children.RemoveAt(0);
								ConstructorInitializer ci = new ConstructorInitializer();
								ci.Arguments = ie.Parameters;
								if (fre.TargetObject is BaseReferenceExpression)
									ci.ConstructorInitializerType = ConstructorInitializerType.Base;
								else
									ci.ConstructorInitializerType = ConstructorInitializerType.This;
								constructorDeclaration.ConstructorInitializer = ci;
							}
						}
					}
				}
			}
			return base.Visit(constructorDeclaration, data);
		}
	}
}
