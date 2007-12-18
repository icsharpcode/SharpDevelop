// SharpDevelop samples
// Copyright (c) 2006, AlphaSierraPapa
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are
// permitted provided that the following conditions are met:
//
// - Redistributions of source code must retain the above copyright notice, this list
//   of conditions and the following disclaimer.
//
// - Redistributions in binary form must reproduce the above copyright notice, this list
//   of conditions and the following disclaimer in the documentation and/or other materials
//   provided with the distribution.
//
// - Neither the name of the SharpDevelop team nor the names of its contributors may be used to
//   endorse or promote products derived from this software without specific prior written
//   permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &AS IS& AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;

namespace NRefactoryExample
{
	public class WrapperGeneratorVisitor : ICSharpCode.NRefactory.Visitors.AbstractAstVisitor
	{
		public override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data)
		{
			base.VisitTypeDeclaration(typeDeclaration, data); // visit methods
			typeDeclaration.Attributes.Clear();
			typeDeclaration.BaseTypes.Clear();
			
			// add constructor accepting the wrapped object and the field holding the object
			FieldDeclaration fd = new FieldDeclaration(null, // no attributes
			                                           new TypeReference(typeDeclaration.Name),
			                                           Modifiers.Private);
			fd.Fields.Add(new VariableDeclaration("wrappedObject"));
			typeDeclaration.AddChild(fd);
			
			typeDeclaration.Name += "Wrapper";
			if (typeDeclaration.Type == ClassType.Interface) {
				typeDeclaration.Type = ClassType.Class;
				typeDeclaration.Name = typeDeclaration.Name.Substring(1);
			}
			ConstructorDeclaration cd = new ConstructorDeclaration(typeDeclaration.Name,
			                                                       Modifiers.Public,
			                                                       new List<ParameterDeclarationExpression>(),
			                                                       null);
			cd.Parameters.Add(new ParameterDeclarationExpression(fd.TypeReference,
			                                                     "wrappedObject"));
			// this.wrappedObject = wrappedObject;
			Expression fieldReference = new MemberReferenceExpression(new ThisReferenceExpression(),
			                                                         "wrappedObject");
			Expression assignment = new AssignmentExpression(fieldReference,
			                                                 AssignmentOperatorType.Assign,
			                                                 new IdentifierExpression("wrappedObject"));
			cd.Body = new BlockStatement();
			cd.Body.AddChild(new ExpressionStatement(assignment));
			typeDeclaration.AddChild(cd);
			
			for (int i = 0; i < typeDeclaration.Children.Count; i++) {
				object child = typeDeclaration.Children[i];
				if (child is MethodDeclaration) {
					MethodDeclaration method = (MethodDeclaration)child;
					if (method.Parameters.Count == 0 &&
					    (method.Name.StartsWith("Is") || method.Name.StartsWith("Get")))
					{
						// replace the method with a property
						PropertyDeclaration prop = new PropertyDeclaration(method.Modifier,
						                                                   method.Attributes,
						                                                   method.Name,
						                                                   null);
						prop.TypeReference = method.TypeReference;
						prop.GetRegion = new PropertyGetRegion(method.Body, null);
						typeDeclaration.Children[i] = prop;
					}
				}
			}
			
			return null;
		}
		public override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data)
		{
			base.VisitMethodDeclaration(methodDeclaration, data); // visit parameters
			methodDeclaration.Attributes.Clear();
			methodDeclaration.Body = new BlockStatement();
			methodDeclaration.Modifier = Modifiers.Public;
			
			if (methodDeclaration.Parameters.Count > 0) {
				ParameterDeclarationExpression lastParameter = methodDeclaration.Parameters[methodDeclaration.Parameters.Count - 1];
				if (lastParameter.ParamModifier == ParameterModifiers.Out) {
					methodDeclaration.TypeReference = lastParameter.TypeReference;
					methodDeclaration.Parameters.RemoveAt(methodDeclaration.Parameters.Count - 1);
					
					VariableDeclaration tmpVarDecl = new VariableDeclaration("tmp");
					tmpVarDecl.TypeReference = methodDeclaration.TypeReference;
					methodDeclaration.Body.AddChild(new LocalVariableDeclaration(tmpVarDecl));
					
					IdentifierExpression tmpIdent = new IdentifierExpression("tmp");
					InvocationExpression ie = CreateMethodCall(methodDeclaration);
					ie.Arguments.Add(new DirectionExpression(FieldDirection.Out, tmpIdent));
					methodDeclaration.Body.AddChild(new ExpressionStatement(ie));
					
					methodDeclaration.Body.AddChild(new ReturnStatement(tmpIdent));
					return null;
				}
			}
			
			methodDeclaration.Body.AddChild(new ExpressionStatement(CreateMethodCall(methodDeclaration)));
			return null;
		}
		static InvocationExpression CreateMethodCall(MethodDeclaration method)
		{
			IdentifierExpression wrappedObject = new IdentifierExpression("wrappedObject");
			MemberReferenceExpression methodName = new MemberReferenceExpression(wrappedObject, method.Name);
			InvocationExpression ie = new InvocationExpression(methodName, null);
			foreach (ParameterDeclarationExpression param in method.Parameters) {
				Expression expr = new IdentifierExpression(param.ParameterName);
				if (param.ParamModifier == ParameterModifiers.Ref) {
					expr = new DirectionExpression(FieldDirection.Ref, expr);
				}
				ie.Arguments.Add(expr);
			}
			return ie;
		}
		
		
		public override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data)
		{
			parameterDeclarationExpression.Attributes.Clear();
			return null;
		}
	}
}
