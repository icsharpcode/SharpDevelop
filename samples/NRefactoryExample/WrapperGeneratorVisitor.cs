/*
 * Created by SharpDevelop.
 * User: Daniel Grunwald
 * Date: 09.11.2005
 * Time: 18:15
 */

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
			Expression fieldReference = new FieldReferenceExpression(new ThisReferenceExpression(),
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
			FieldReferenceExpression methodName = new FieldReferenceExpression(wrappedObject, method.Name);
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
