// 
// CreateClassDeclarationAction.cs
//  
// Author:
//       Mike Krüger <mkrueger@xamarin.com>
// 
// Copyright (c) 2012 Xamarin <http://xamarin.com>
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System.Collections.Generic;
using ICSharpCode.NRefactory.Semantics;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Refactoring
{
	[ContextAction("Create class", Description = "Creates a class declaration out of an object creation.")]
	public class CreateClassDeclarationAction : CodeActionProvider
	{
		public override IEnumerable<CodeAction> GetActions(RefactoringContext context)
		{
			var simpleType = context.GetNode<SimpleType>();
			if (simpleType != null && !(simpleType.Parent is EventDeclaration || simpleType.Parent is CustomEventDeclaration)) 
				return GetActions(context, simpleType);

			var createExpression = context.GetNode<ObjectCreateExpression>();
			if (createExpression != null) 
				return GetActions(context, createExpression);

			var identifier = context.GetNode<IdentifierExpression>();
			if (identifier != null && (identifier.Parent is MemberReferenceExpression)) 
				return GetActions(context, identifier);

			return Enumerable.Empty<CodeAction>();
		}

		static IEnumerable<CodeAction> GetActions(RefactoringContext context, AstNode node)
		{
			var resolveResult = context.Resolve(node) as UnknownIdentifierResolveResult;
			if (resolveResult == null)
				yield break;

			var service = (NamingConventionService)context.GetService(typeof(NamingConventionService));
			if (service != null && !service.IsValidName(resolveResult.Identifier, AffectedEntity.Class)) { 
				yield break;
			}
			ClassType classType = GuessClassTypeByName(service, node);
			ModifyClassTypeBasedOnTypeGuessing(context, node, ref classType);

			string message;
			switch (classType) {
				case ClassType.Struct:
					message = context.TranslateString("Create struct");
					break;
				case ClassType.Interface:
					message = context.TranslateString("Create interface");
					break;
				default:
					message = context.TranslateString("Create class");
					break;
			}
			yield return new CodeAction(message, script => {
				script.CreateNewType(CreateType(context, service, node, classType));
			}, node);

			if (node.Parent is TypeDeclaration || classType != ClassType.Class)
				yield break;
			yield return new CodeAction(context.TranslateString("Create nested class"), script => {
				script.InsertWithCursor(
					context.TranslateString("Create nested class"),
					Script.InsertPosition.Before,
					CreateType(context, service, node, classType)
				);
			}, node);
		}

		static void ModifyClassTypeBasedOnTypeGuessing(RefactoringContext context, AstNode node, ref ClassType classType)
		{
			var guessedType = TypeGuessing.GuessType(context, node);
			if (guessedType.Kind == TypeKind.TypeParameter) {
				var tp = (ITypeParameter)guessedType;
				if (tp.HasValueTypeConstraint)
					classType = ClassType.Struct;
				if (tp.HasReferenceTypeConstraint)
					classType = ClassType.Class;
			}
		}
		
		static ClassType GuessClassTypeByName(NamingConventionService service,  string identifier)
		{
			if (service == null)
				return ClassType.Class;
			if (service.IsValidName (identifier, AffectedEntity.Interface, Modifiers.Public))
				return ClassType.Interface;
			if (!service.IsValidName (identifier, AffectedEntity.Class, Modifiers.Public) &&
			     service.IsValidName (identifier, AffectedEntity.Struct, Modifiers.Public))
				return ClassType.Struct;
			return ClassType.Class;
		}

		static ClassType GuessClassTypeByName(NamingConventionService service, AstNode node)
		{
			if (node is SimpleType) 
				return GuessClassTypeByName (service, ((SimpleType)node).Identifier);
			if (node is IdentifierExpression) 
				return GuessClassTypeByName (service, ((IdentifierExpression)node).Identifier);
			return ClassType.Class;
		}

		static TypeDeclaration CreateType(RefactoringContext context, NamingConventionService service, AstNode node, ClassType classType)
		{
			TypeDeclaration result;
			if (node is SimpleType) {
				result = CreateClassFromType(context, classType, (SimpleType)node);
			} else if (node is ObjectCreateExpression) {
				result = CreateClassFromObjectCreation(context, (ObjectCreateExpression)node);
			} else {
				result = CreateClassFromIdentifier(context, classType, (IdentifierExpression)node);
			}

			return AddBaseTypesAccordingToNamingRules(context, service, result);
		}

		static TypeDeclaration CreateClassFromIdentifier(RefactoringContext context, ClassType classType, IdentifierExpression identifierExpression)
		{
			var result = new TypeDeclaration { Name = identifierExpression.Identifier, ClassType = classType };
			var entity = identifierExpression.GetParent<EntityDeclaration>();
			if (entity != null)
				result.Modifiers |= entity.Modifiers & Modifiers.Public;
			return result;
		}

		static TypeDeclaration CreateClassFromType(RefactoringContext context, ClassType classType, SimpleType simpleType)
		{
			TypeDeclaration result;
			string className = simpleType.Identifier;

			if (simpleType.Parent is Attribute && classType == ClassType.Class) {
				if (!className.EndsWith("Attribute", System.StringComparison.Ordinal))
					className += "Attribute";
			}

			result = new TypeDeclaration { Name = className, ClassType = classType };
			var entity = simpleType.GetParent<EntityDeclaration>();
			if (entity != null)
				result.Modifiers |= entity.Modifiers & Modifiers.Public;

			var guessedType = TypeGuessing.GuessType (context, simpleType);
			if (guessedType.Kind == TypeKind.TypeParameter)
				ImplementConstraints (context, result, (ITypeParameter)guessedType);
			return result;
		}

		static void ImplementConstraints(RefactoringContext context, TypeDeclaration result, ITypeParameter tp)
		{
			if (tp.HasValueTypeConstraint)
				result.ClassType = ClassType.Struct;
			if (tp.HasReferenceTypeConstraint)
				result.ClassType = ClassType.Class;
			if (tp.HasDefaultConstructorConstraint)
				result.AddChild (new ConstructorDeclaration { Modifiers = Modifiers.Public, Body = new BlockStatement () }, Roles.TypeMemberRole);
			foreach (var baseType in tp.DirectBaseTypes) {
				if (baseType.Namespace == "System") {
					if (baseType.Name == "Object" || baseType.Name == "ValueType")
						continue;
				}
				result.BaseTypes.Add (context.CreateShortType (baseType));
			}
		}

		static TypeDeclaration CreateClassFromObjectCreation(RefactoringContext context, ObjectCreateExpression createExpression)
		{
			TypeDeclaration result;
			string className = createExpression.Type.ToString();
			if (!createExpression.Arguments.Any()) {
				result = new TypeDeclaration { Name = className };
			} else {
				var decl = new ConstructorDeclaration {
					Name = className,
					Modifiers = Modifiers.Public,
					Body = new BlockStatement {
						new ThrowStatement(new ObjectCreateExpression(context.CreateShortType("System", "NotImplementedException")))
					}
				};
				result = new TypeDeclaration {
					Name = className,
					Members = {
						decl
					}
				};
				decl.Parameters.AddRange(CreateMethodDeclarationAction.GenerateParameters(context, createExpression.Arguments));
			}
			var guessedType = TypeGuessing.GuessType(context, createExpression);
			if (guessedType.Kind == TypeKind.Interface || guessedType.Kind == TypeKind.Class && guessedType.GetDefinition ().IsAbstract) {
				result.BaseTypes.Add(context.CreateShortType(guessedType));
				AddImplementation(context, result, guessedType);
			}
			
			return result;
		}

		static Modifiers GetModifiers(IEntity property)
		{
			if (property.DeclaringType.Kind == TypeKind.Interface)
				return Modifiers.Public;
			switch (property.Accessibility) {
				case Accessibility.Public:
					return Modifiers.Public | Modifiers.Override;
				case Accessibility.Protected:
					return Modifiers.Protected | Modifiers.Override;
				case Accessibility.Internal:
					return Modifiers.Internal | Modifiers.Override;
				case Accessibility.ProtectedOrInternal:
					// TODO: oops
					return Modifiers.Internal | Modifiers.Protected | Modifiers.Override;
				case Accessibility.ProtectedAndInternal:
					// TODO: oops
					return Modifiers.Internal | Modifiers.Protected | Modifiers.Override;
			}
			return Modifiers.Override;
		}

		static void AddImplementation(RefactoringContext context, TypeDeclaration result, IType guessedType)
		{
			foreach (var property in guessedType.GetProperties ()) {
				if (!property.IsAbstract)
					continue;
				if (property.IsIndexer) {
					var indexerDecl = new IndexerDeclaration {
						ReturnType = context.CreateShortType(property.ReturnType),
						Modifiers = GetModifiers(property),
						Name = property.Name
					};
					indexerDecl.Parameters.AddRange(ConvertParameters(context, property.Parameters));
					if (property.CanGet)
						indexerDecl.Getter = new Accessor();
					if (property.CanSet)
						indexerDecl.Setter = new Accessor();
					result.AddChild(indexerDecl, Roles.TypeMemberRole);
					continue;
				}
				var propDecl = new PropertyDeclaration {
					ReturnType = context.CreateShortType(property.ReturnType),
					Modifiers = GetModifiers (property),
					Name = property.Name
				};
				if (property.CanGet)
					propDecl.Getter = new Accessor();
				if (property.CanSet)
					propDecl.Setter = new Accessor();
				result.AddChild(propDecl, Roles.TypeMemberRole);
			}
			
			foreach (var method in guessedType.GetMethods ()) {
				if (!method.IsAbstract)
					continue;
				var decl = new MethodDeclaration {
					ReturnType = context.CreateShortType(method.ReturnType),
					Modifiers = GetModifiers (method),
					Name = method.Name,
					Body = new BlockStatement {
						new ThrowStatement(new ObjectCreateExpression(context.CreateShortType("System", "NotImplementedException")))
					}
				};
				decl.Parameters.AddRange(ConvertParameters(context, method.Parameters));
				result.AddChild(decl, Roles.TypeMemberRole);
			}

			foreach (var evt in guessedType.GetEvents ()) {
				if (!evt.IsAbstract)
					continue;
				var decl = new EventDeclaration {
					ReturnType = context.CreateShortType(evt.ReturnType),
					Modifiers = GetModifiers (evt),
					Variables = {
						new VariableInitializer {
							Name = evt.Name
						}
					}
				};
				decl.Variables.Add(new VariableInitializer(evt.Name));
				result.AddChild(decl, Roles.TypeMemberRole);
			}
		}

		static IEnumerable<ParameterDeclaration> ConvertParameters(RefactoringContext context, IList<IParameter> parameters)
		{
			foreach (var param in parameters) {
				ParameterModifier mod = ParameterModifier.None;
				if (param.IsOut) {
					mod = ParameterModifier.Out;
				} else if (param.IsRef) {
					mod = ParameterModifier.Ref;
				} else if (param.IsParams) {
					mod = ParameterModifier.Params;
				}
				yield return new ParameterDeclaration(context.CreateShortType(param.Type), param.Name, mod);
			}
		}

		static TypeDeclaration AddBaseTypesAccordingToNamingRules(RefactoringContext context, NamingConventionService service, TypeDeclaration result)
		{
			if (service.HasValidRule(result.Name, AffectedEntity.CustomAttributes, Modifiers.Public)) {
				result.BaseTypes.Add(context.CreateShortType("System", "Attribute"));
			} else if (service.HasValidRule(result.Name, AffectedEntity.CustomEventArgs, Modifiers.Public)) {
				result.BaseTypes.Add(context.CreateShortType("System", "EventArgs"));
			} else if (service.HasValidRule(result.Name, AffectedEntity.CustomExceptions, Modifiers.Public)) {
				result.BaseTypes.Add(context.CreateShortType("System", "Exception"));
			}
			return result;
		}
	}
}