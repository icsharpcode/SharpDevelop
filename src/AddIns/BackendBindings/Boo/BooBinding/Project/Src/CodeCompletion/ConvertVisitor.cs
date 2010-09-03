// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using Boo.Lang.Compiler.Steps;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Dom;
using AST = Boo.Lang.Compiler.Ast;

namespace Grunwald.BooBinding.CodeCompletion
{
	public class ConvertVisitor : AbstractVisitorCompilerStep
	{
		int[] _lineLength;
		
		public ConvertVisitor(int[] _lineLength, IProjectContent pc)
		{
			this._lineLength = _lineLength;
			this._cu = new DefaultCompilationUnit(pc);
		}
		
		DefaultCompilationUnit _cu;

		public DefaultCompilationUnit Cu {
			get {
				return _cu;
			}
		}
		
		Stack<DefaultClass> _currentClass = new Stack<DefaultClass>();
		bool _firstModule = true;
		
		public override void Run()
		{
			try {
				_cu.Tag = CompileUnit;
				Visit(CompileUnit);
			} catch (Exception ex) {
				MessageService.ShowException(ex);
			}
		}
		
		protected override void OnError(AST.Node node, Exception error)
		{
			MessageService.ShowException(error, "error processing " + node.ToCodeString());
		}
		
		private ModifierEnum GetModifier(AST.TypeMember m)
		{
			ModifierEnum r = ModifierEnum.None;
			if (m.IsPublic)    r |= ModifierEnum.Public;
			if (m.IsProtected) r |= ModifierEnum.Protected;
			if (m.IsPrivate)   r |= ModifierEnum.Private;
			if (m.IsInternal)  r |= ModifierEnum.Internal;
			if (!m.IsVisibilitySet) {
				if (IsStrictMode(_cu.ProjectContent))
					r |= ModifierEnum.Private;
				else if (m is AST.Field)
					r |= ModifierEnum.Protected;
				else
					r |= ModifierEnum.Public;
			}
			
			if (m.IsStatic) r |= ModifierEnum.Static;
			if (m is AST.Field) {
				if (m.IsFinal) r |= ModifierEnum.Readonly;
			} else {
				if (m.IsFinal) r |= ModifierEnum.Sealed;
			}
			if (m.IsAbstract)  r |= ModifierEnum.Abstract;
			if (m.IsOverride)  r |= ModifierEnum.Override;
			if (m.IsSynthetic) r |= ModifierEnum.Synthetic;
			if (m.IsPartial)   r |= ModifierEnum.Partial;
			
			if (m.LexicalInfo.IsValid && m.DeclaringType != null
			    && m.LexicalInfo.Line < m.DeclaringType.LexicalInfo.Line)
			{ // member added through attribute
				r |= ModifierEnum.Synthetic;
			}
			return r;
		}
		
		public static AST.TypeMemberModifiers ConvertVisibilityBack(ModifierEnum modifier)
		{
			AST.TypeMemberModifiers r = AST.TypeMemberModifiers.None;
			if ((modifier & ModifierEnum.Public) == ModifierEnum.Public)
				r |= AST.TypeMemberModifiers.Public;
			if ((modifier & ModifierEnum.Protected) == ModifierEnum.Protected)
				r |= AST.TypeMemberModifiers.Protected;
			if ((modifier & ModifierEnum.Internal) == ModifierEnum.Internal)
				r |= AST.TypeMemberModifiers.Internal;
			if ((modifier & ModifierEnum.Private) == ModifierEnum.Private)
				r |= AST.TypeMemberModifiers.Private;
			return r;
		}
		
		private int GetLineEnd(int line)
		{
			if (_lineLength == null || line < 1 || line > _lineLength.Length)
				return 0;
			else
				return _lineLength[line - 1] + 1;
		}
		
		private DomRegion GetRegion(AST.Node m)
		{
			AST.LexicalInfo l = m.LexicalInfo;
			if (l.Line < 0)
				return DomRegion.Empty;
			else
				return new DomRegion(l.Line, 0 /*l.Column*/, l.Line, GetLineEnd(l.Line));
		}
		
		private DomRegion GetClientRegion(AST.Node m)
		{
			AST.LexicalInfo l = m.LexicalInfo;
			if (l.Line < 0)
				return DomRegion.Empty;
			AST.SourceLocation l2;
			if (m is AST.Method) {
				l2 = ((AST.Method)m).Body.EndSourceLocation;
			} else if (m is AST.Property) {
				AST.Property p = (AST.Property)m;
				if (p.Getter != null && p.Getter.Body != null) {
					l2 = p.Getter.Body.EndSourceLocation;
					if (p.Setter != null && p.Setter.Body != null) {
						if (p.Setter.Body.EndSourceLocation.Line > l2.Line)
							l2 = p.Setter.Body.EndSourceLocation;
					}
				} else if (p.Setter != null && p.Setter.Body != null) {
					l2 = p.Setter.Body.EndSourceLocation;
				} else {
					l2 = p.EndSourceLocation;
				}
			} else {
				l2 = m.EndSourceLocation;
			}
			if (l2 == null || l2.Line < 0 || l.Line == l2.Line)
				return DomRegion.Empty;
			// TODO: use l.Column / l2.Column when the tab-bug has been fixed
			return new DomRegion(l.Line, GetLineEnd(l.Line), l2.Line, GetLineEnd(l2.Line));
		}
		
		public override void OnImport(AST.Import p)
		{
			DefaultUsing u = new DefaultUsing(_cu.ProjectContent);
			if (p.Alias == null)
				u.Usings.Add(p.Namespace);
			else
				u.AddAlias(p.Alias.Name, new GetClassReturnType(_cu.ProjectContent, p.Namespace, 0));
			_cu.UsingScope.Usings.Add(u);
		}
		
		private IClass OuterClass {
			get {
				if (_currentClass.Count > 0)
					return _currentClass.Peek();
				else
					return null;
			}
		}
		
		void ConvertTemplates(AST.Node node, DefaultClass c)
		{
			c.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
		}
		
		void ConvertTemplates(AST.Node node, DefaultMethod m)
		{
			m.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
		}
		
		void ConvertAttributes(AST.TypeMember node, AbstractEntity to)
		{
			if (node.Attributes.Count == 0) {
				to.Attributes = DefaultAttribute.EmptyAttributeList;
			} else {
				ClassFinder context;
				if (to is IClass) {
					context = new ClassFinder((IClass)to, node.LexicalInfo.Line, node.LexicalInfo.Column);
				} else {
					context = new ClassFinder(to.DeclaringType, node.LexicalInfo.Line, node.LexicalInfo.Column);
				}
				foreach (AST.Attribute a in node.Attributes) {
					to.Attributes.Add(new DefaultAttribute(new AttributeReturnType(context, a.Name)) {
					                  	CompilationUnit = _cu,
					                  	Region = GetRegion(a)
					                  });
				}
			}
			to.Documentation = node.Documentation;
		}
		
		void ConvertParameters(AST.ParameterDeclarationCollection parameters, DefaultMethod m)
		{
			if (parameters == null || parameters.Count == 0) {
				m.Parameters = DefaultParameter.EmptyParameterList;
			} else {
				AddParameters(parameters, m.Parameters, m, m.DeclaringType);
			}
		}
		void ConvertParameters(AST.ParameterDeclarationCollection parameters, DefaultProperty p)
		{
			if (parameters == null || parameters.Count == 0) {
				p.Parameters = DefaultParameter.EmptyParameterList;
			} else {
				AddParameters(parameters, p.Parameters, p, p.DeclaringType);
			}
		}
		internal static void AddParameters(AST.ParameterDeclarationCollection parameters, IList<IParameter> output, IMethodOrProperty method, IClass c)
		{
			if (c == null) throw new ArgumentNullException("c");
			DefaultParameter p = null;
			foreach (AST.ParameterDeclaration par in parameters) {
				p = new DefaultParameter(par.Name,
				                         CreateReturnType(par.Type, c, method as IMethod, c.Region.BeginLine + 1, 1, c.ProjectContent),
				                         new DomRegion(par.LexicalInfo.Line, par.LexicalInfo.Column));
				if (par.IsByRef) p.Modifiers |= ParameterModifiers.Ref;
				output.Add(p);
			}
			if (parameters.HasParamArray) {
				p.Modifiers |= ParameterModifiers.Params;
			}
		}
		
		IReturnType CreateReturnType(AST.TypeReference reference, IMethod method)
		{
			IClass c = OuterClass;
			if (c == null) {
				return CreateReturnType(reference, new DefaultClass(_cu, "___DummyClass"), method, 1, 1, _cu.ProjectContent);
			} else {
				return CreateReturnType(reference, c, method, c.Region.BeginLine + 1, 1, _cu.ProjectContent);
			}
		}
		
		internal static bool IsStrictMode(IProjectContent projectContent)
		{
			BooProject project = projectContent.Project as BooProject;
			if (project != null)
				return project.Strict;
			else
				return false;
		}
		
		internal static IReturnType GetDefaultReturnType(IProjectContent projectContent)
		{
			BooProject project = projectContent.Project as BooProject;
			if (project != null && project.Ducky)
				return new BooResolver.DuckClass(new DefaultCompilationUnit(projectContent)).DefaultReturnType;
			else
				return projectContent.SystemTypes.Object;
		}
		
		public static IReturnType CreateReturnType(AST.TypeReference reference, IClass callingClass,
		                                           IMethodOrProperty callingMember, int caretLine, int caretColumn,
		                                           IProjectContent projectContent)
		{
			System.Diagnostics.Debug.Assert(projectContent != null);
			if (reference == null) {
				return GetDefaultReturnType(projectContent);
			}
			if (reference is AST.ArrayTypeReference) {
				AST.ArrayTypeReference arr = (AST.ArrayTypeReference)reference;
				return new ArrayReturnType(projectContent,
				                           CreateReturnType(arr.ElementType, callingClass, callingMember,
				                                            caretLine, caretColumn, projectContent),
				                           (arr.Rank != null) ? (int)arr.Rank.Value : 1);
			} else if (reference is AST.SimpleTypeReference) {
				string name = ((AST.SimpleTypeReference)reference).Name;
				IReturnType rt;
				int typeParameterCount = (reference is AST.GenericTypeReference) ? ((AST.GenericTypeReference)reference).GenericArguments.Count : 0;
				if (name == "duck")
					rt = new BooResolver.DuckClass(new DefaultCompilationUnit(projectContent)).DefaultReturnType;
				else if (BooAmbience.ReverseTypeConversionTable.ContainsKey(name))
					rt = new GetClassReturnType(projectContent, BooAmbience.ReverseTypeConversionTable[name], typeParameterCount);
				else if (callingClass == null)
					rt = new GetClassReturnType(projectContent, name, typeParameterCount);
				else
					rt = new SearchClassReturnType(projectContent, callingClass, caretLine, caretColumn,
					                               name, typeParameterCount);
				if (typeParameterCount > 0) {
					AST.TypeReferenceCollection arguments = ((AST.GenericTypeReference)reference).GenericArguments;
					// GenericTypeReference derives from SimpleTypeReference
					IReturnType[] typeArguments = new IReturnType[arguments.Count];
					for (int i = 0; i < typeArguments.Length; i++) {
						typeArguments[i] = CreateReturnType(arguments[i], callingClass, callingMember, caretLine, caretColumn,
						                                    projectContent);
					}
					rt = new ConstructedReturnType(rt, typeArguments);
				}
				return rt;
			} else if (reference is AST.CallableTypeReference) {
				AST.CallableTypeReference ctr = (AST.CallableTypeReference)reference;
				AnonymousMethodReturnType amrt = new AnonymousMethodReturnType(new DefaultCompilationUnit(projectContent));
				if (ctr.ReturnType != null) {
					amrt.MethodReturnType = CreateReturnType(ctr.ReturnType, callingClass, callingMember, caretLine, caretColumn, projectContent);
				}
				amrt.MethodParameters = new List<IParameter>();
				AddParameters(ctr.Parameters, amrt.MethodParameters, callingMember, callingClass ?? new DefaultClass(new DefaultCompilationUnit(projectContent), "__Dummy"));
				return amrt;
			} else {
				throw new NotSupportedException("unknown reference type: " + reference.ToString());
			}
		}
		IReturnType CreateReturnType(AST.TypeReference reference)
		{
			return CreateReturnType(reference, null);
		}
		IReturnType CreateReturnType(AST.Field field)
		{
			if (field.Type == null) {
				if (field.Initializer != null)
					return new BooInferredReturnType(field.Initializer, OuterClass);
				else
					return GetDefaultReturnType(_cu.ProjectContent);
			} else {
				return CreateReturnType(field.Type);
			}
		}
		IReturnType CreateReturnType(AST.Method node, IMethod method)
		{
			if (node.ReturnType == null)
				return new BooInferredReturnType(node.Body, OuterClass, false);
			return CreateReturnType(node.ReturnType, method);
		}
		IReturnType CreateReturnType(AST.Property property)
		{
			if (property.Type == null && property.Getter != null && property.Getter.Body != null)
				return new BooInferredReturnType(property.Getter.Body, OuterClass, false);
			return CreateReturnType(property.Type);
		}
		
		public override void OnCallableDefinition(AST.CallableDefinition node)
		{
			LoggingService.Debug("OnCallableDefinition: " + node.FullName);
			DomRegion region = GetRegion(node);
			DefaultClass c = new DefaultClass(_cu, ClassType.Delegate, GetModifier(node), region, OuterClass);
			ConvertAttributes(node, c);
			c.BaseTypes.Add(c.ProjectContent.SystemTypes.Delegate);
			c.FullyQualifiedName = node.FullName;
			if (_currentClass.Count > 0) {
				OuterClass.InnerClasses.Add(c);
			} else {
				_cu.Classes.Add(c);
			}
			_currentClass.Push(c); // necessary for CreateReturnType
			ConvertTemplates(node, c);
			IReturnType returnType = CreateReturnType(node.ReturnType);
			DefaultMethod invokeMethod = new DefaultMethod("Invoke", returnType, ModifierEnum.Public, DomRegion.Empty, DomRegion.Empty, c);
			ConvertParameters(node.Parameters, invokeMethod);
			c.Methods.Add(invokeMethod);
			invokeMethod = new DefaultMethod("BeginInvoke", c.ProjectContent.SystemTypes.IAsyncResult, ModifierEnum.Public, DomRegion.Empty, DomRegion.Empty, c);
			ConvertParameters(node.Parameters, invokeMethod);
			if (invokeMethod.Parameters == DefaultParameter.EmptyParameterList) {
				invokeMethod.Parameters = new List<IParameter>();
			}
			invokeMethod.Parameters.Add(new DefaultParameter("callback", c.ProjectContent.SystemTypes.AsyncCallback, DomRegion.Empty));
			invokeMethod.Parameters.Add(new DefaultParameter("object", c.ProjectContent.SystemTypes.Object, DomRegion.Empty));
			c.Methods.Add(invokeMethod);
			invokeMethod = new DefaultMethod("EndInvoke", returnType, ModifierEnum.Public, DomRegion.Empty, DomRegion.Empty, c);
			invokeMethod.Parameters.Add(new DefaultParameter("result", c.ProjectContent.SystemTypes.IAsyncResult, DomRegion.Empty));
			c.Methods.Add(invokeMethod);
			_currentClass.Pop();
		}
		
		public override bool EnterClassDefinition(AST.ClassDefinition node)
		{
			EnterTypeDefinition(node, ClassType.Class);
			return base.EnterClassDefinition(node);
		}
		
		public override bool EnterInterfaceDefinition(AST.InterfaceDefinition node)
		{
			EnterTypeDefinition(node, ClassType.Interface);
			return base.EnterInterfaceDefinition(node);
		}
		
		public override bool EnterEnumDefinition(AST.EnumDefinition node)
		{
			EnterTypeDefinition(node, ClassType.Enum);
			return base.EnterEnumDefinition(node);
		}
		
		// cannot override OnNamespaceDeclaration - it's visited too late (after the type definitions)
		void HandleNamespaceDeclaration(AST.NamespaceDeclaration node)
		{
			if (node == null)
				return;
			string[] namespaceName = node.Name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
			foreach (string namePart in namespaceName) {
				_cu.UsingScope = new DefaultUsingScope {
					NamespaceName = PrependCurrentNamespace(namePart),
					Parent = _cu.UsingScope
				};
			}
		}
		
		public override bool EnterModule(AST.Module node)
		{
			HandleNamespaceDeclaration(node.Namespace);
			if (!_firstModule && node.Members.Count > 0) {
				EnterTypeDefinition(node, ClassType.Module);
			}
			_firstModule = false;
			return base.EnterModule(node);
		}
		
		private void EnterTypeDefinition(AST.TypeDefinition node, ClassType classType)
		{
			//LoggingService.Debug("Enter " + node.GetType().Name + " (" + node.FullName + ")");
			foreach (AST.Attribute att in node.Attributes) {
				if (att.Name == "Boo.Lang.ModuleAttribute")
					classType = ClassType.Module;
			}
			DomRegion region = GetClientRegion(node);
			DefaultClass c = new DefaultClass(_cu, classType, GetModifier(node), region, OuterClass);
			c.FullyQualifiedName = node.FullName;
			if (_currentClass.Count > 0)
				_currentClass.Peek().InnerClasses.Add(c);
			else
				_cu.Classes.Add(c);
			_currentClass.Push(c);
			ConvertAttributes(node, c);
			ConvertTemplates(node, c);
			if (node.BaseTypes != null) {
				foreach (AST.TypeReference r in node.BaseTypes) {
					c.BaseTypes.Add(CreateReturnType(r));
				}
			}
		}
		
		public override void LeaveClassDefinition(AST.ClassDefinition node)
		{
			LeaveTypeDefinition(node);
			base.LeaveClassDefinition(node);
		}
		
		public override void LeaveInterfaceDefinition(AST.InterfaceDefinition node)
		{
			LeaveTypeDefinition(node);
			base.LeaveInterfaceDefinition(node);
		}
		
		public override void LeaveEnumDefinition(AST.EnumDefinition node)
		{
			LeaveTypeDefinition(node);
			base.LeaveEnumDefinition(node);
		}
		
		public override void LeaveModule(AST.Module node)
		{
			if (_currentClass.Count != 0) LeaveTypeDefinition(node);
			base.LeaveModule(node);
		}
		
		private void LeaveTypeDefinition(AST.TypeDefinition node)
		{
			DefaultClass c = _currentClass.Pop();
			foreach (AST.Attribute att in node.Attributes) {
				if (att.Name == "System.Reflection.DefaultMemberAttribute" && att.Arguments.Count == 1) {
					AST.StringLiteralExpression sle = att.Arguments[0] as AST.StringLiteralExpression;
					if (sle != null) {
						foreach (DefaultProperty p in c.Properties) {
							if (p.Name == sle.Value) {
								p.IsIndexer = true;
							}
						}
					}
				}
			}
			//LoggingService.Debug("Leave "+node.GetType().Name+" "+node.FullName+" (Class = "+c.FullyQualifiedName+")");
		}
		
		public override void OnMethod(AST.Method node)
		{
			//LoggingService.Debug("Method: " + node.FullName + " (" + node.Modifiers + ")");
			DefaultMethod method = new DefaultMethod(node.Name, null, GetModifier(node), GetRegion(node), GetClientRegion(node), OuterClass);
			
			foreach (AST.Attribute a in node.Attributes) {
				if (a.Name == "Extension" || a.Name == "Boo.Lang.Extension"
				    || a.Name == "ExtensionAttribute" || a.Name == "Boo.Lang.ExtensionAttribute")
				{
					method.IsExtensionMethod = true;
				}
			}
			
			ConvertAttributes(node, method);
			ConvertTemplates(node, method);
			// return type must be assigned AFTER ConvertTemplates
			method.ReturnType = CreateReturnType(node, method);
			ConvertParameters(node.Parameters, method);
			_currentClass.Peek().Methods.Add(method);
			method.UserData = node;
		}
		
		public override void OnConstructor(AST.Constructor node)
		{
			if (node.IsSynthetic && node.Parameters.Count == 0) return;
			Constructor ctor = new Constructor(GetModifier(node), GetRegion(node), GetClientRegion(node), OuterClass);
			ConvertAttributes(node, ctor);
			ConvertParameters(node.Parameters, ctor);
			_currentClass.Peek().Methods.Add(ctor);
			ctor.UserData = node;
		}
		
		public override void OnEnumMember(AST.EnumMember node)
		{
			DefaultField field = new DefaultField(OuterClass.DefaultReturnType, node.Name, ModifierEnum.Const | ModifierEnum.Public, GetRegion(node), OuterClass);
			ConvertAttributes(node, field);
			OuterClass.Fields.Add(field);
		}
		
		public override void OnField(AST.Field node)
		{
			DefaultField field = new DefaultField(CreateReturnType(node), node.Name, GetModifier(node), GetRegion(node), OuterClass);
			ConvertAttributes(node, field);
			OuterClass.Fields.Add(field);
		}
		
		public override void OnEvent(AST.Event node)
		{
			DomRegion region = GetRegion(node);
			DefaultEvent e = new DefaultEvent(node.Name, CreateReturnType(node.Type), GetModifier(node), region, region, OuterClass);
			ConvertAttributes(node, e);
			OuterClass.Events.Add(e);
		}
		
		public override void OnProperty(AST.Property node)
		{
			DefaultProperty property = new DefaultProperty(node.Name, CreateReturnType(node), GetModifier(node), GetRegion(node), GetClientRegion(node), OuterClass);
			ConvertAttributes(node, property);
			ConvertParameters(node.Parameters, property);
			if (node.Getter != null && node.Getter.Body != null) {
				property.GetterRegion = GetClientRegion(node.Getter);
			}
			if (node.Setter != null && node.Setter.Body != null) {
				property.SetterRegion = GetClientRegion(node.Setter);
			}
			property.IsIndexer = (node.Name == "self");
			OuterClass.Properties.Add(property);
			property.UserData = node;
		}
		
		string PrependCurrentNamespace(string name)
		{
			if (string.IsNullOrEmpty(_cu.UsingScope.NamespaceName))
				return name;
			else
				return _cu.UsingScope.NamespaceName + "." + name;
		}
	}
}
