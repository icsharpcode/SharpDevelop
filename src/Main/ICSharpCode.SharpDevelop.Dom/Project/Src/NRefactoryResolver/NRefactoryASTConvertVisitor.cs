// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

// created on 04.08.2003 at 17:49
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

using ICSharpCode.NRefactory.Visitors;
using AST = ICSharpCode.NRefactory.Ast;
using RefParser = ICSharpCode.NRefactory;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class NRefactoryASTConvertVisitor : AbstractAstVisitor
	{
		ICompilationUnit cu;
		Stack<string> currentNamespace = new Stack<string>();
		Stack<DefaultClass> currentClass = new Stack<DefaultClass>();
		
		public ICompilationUnit Cu {
			get {
				return cu;
			}
		}
		
		public NRefactoryASTConvertVisitor(IProjectContent projectContent)
		{
			cu = new DefaultCompilationUnit(projectContent);
		}
		
		DefaultClass GetCurrentClass()
		{
			return currentClass.Count == 0 ? null : currentClass.Peek();
		}
		
		ModifierEnum ConvertModifier(AST.Modifiers m)
		{
			if (this.IsVisualBasic)
				return ConvertModifier(m, ModifierEnum.Public);
			else if (currentClass.Count > 0 && currentClass.Peek().ClassType == ClassType.Interface)
				return ConvertModifier(m, ModifierEnum.Public);
			else
				return ConvertModifier(m, ModifierEnum.Private);
		}
		
		ModifierEnum ConvertTypeModifier(AST.Modifiers m)
		{
			if (this.IsVisualBasic)
				return ConvertModifier(m, ModifierEnum.Public);
			if (currentClass.Count > 0)
				return ConvertModifier(m, ModifierEnum.Private);
			else
				return ConvertModifier(m, ModifierEnum.Internal);
		}
		
		ModifierEnum ConvertModifier(AST.Modifiers m, ModifierEnum defaultVisibility)
		{
			ModifierEnum r = (ModifierEnum)m;
			if ((r & ModifierEnum.VisibilityMask) == ModifierEnum.None)
				return r | defaultVisibility;
			else
				return r;
		}
		
		List<RefParser.ISpecial> specials;
		
		/// <summary>
		/// Gets/Sets the list of specials used to read the documentation.
		/// The list must be sorted by the start position of the specials!
		/// </summary>
		public List<RefParser.ISpecial> Specials {
			get {
				return specials;
			}
			set {
				specials = value;
			}
		}
		
		string GetDocumentation(int line, IList<AST.AttributeSection> attributes)
		{
			foreach (AST.AttributeSection att in attributes) {
				if (att.StartLocation.Y > 0 && att.StartLocation.Y < line)
					line = att.StartLocation.Y;
			}
			List<string> lines = new List<string>();
			int length = 0;
			while (line > 0) {
				string doku = GetDocumentationFromLine(--line);
				if (doku == null)
					break;
				length += 2 + doku.Length;
				lines.Add(doku);
			}
			StringBuilder b = new StringBuilder(length);
			for (int i = lines.Count - 1; i >= 0; --i) {
				b.AppendLine(lines[i]);
			}
			return b.ToString();
		}
		
		string GetDocumentationFromLine(int line)
		{
			if (specials == null) return null;
			if (line < 0) return null;
			// specials is a sorted list: use interpolation search
			int left = 0;
			int right = specials.Count - 1;
			int m;
			
			while (left <= right) {
				int leftLine  = specials[left].StartPosition.Y;
				if (line < leftLine)
					break;
				int rightLine = specials[right].StartPosition.Y;
				if (line > rightLine)
					break;
				if (leftLine == rightLine) {
					if (leftLine == line)
						m = left;
					else
						break;
				} else {
					m = (int)(left + Math.BigMul((line - leftLine), (right - left)) / (rightLine - leftLine));
				}
				
				int mLine = specials[m].StartPosition.Y;
				if (mLine < line) { // found line smaller than line we are looking for
					left = m + 1;
				} else if (mLine > line) {
					right = m - 1;
				} else {
					// correct line found,
					// look for first special in that line
					while (--m >= 0 && specials[m].StartPosition.Y == line);
					// look at all specials in that line: find doku-comment
					while (++m < specials.Count && specials[m].StartPosition.Y == line) {
						RefParser.Comment comment = specials[m] as RefParser.Comment;
						if (comment != null && comment.CommentType == RefParser.CommentType.Documentation) {
							return comment.CommentText;
						}
					}
					break;
				}
			}
			return null;
		}
		
		public override object VisitCompilationUnit(AST.CompilationUnit compilationUnit, object data)
		{
			if (compilationUnit == null) {
				return null;
			}
			compilationUnit.AcceptChildren(this, data);
			return cu;
		}
		
		public override object VisitUsingDeclaration(AST.UsingDeclaration usingDeclaration, object data)
		{
			DefaultUsing us = new DefaultUsing(cu.ProjectContent, GetRegion(usingDeclaration.StartLocation, usingDeclaration.EndLocation));
			foreach (AST.Using u in usingDeclaration.Usings) {
				u.AcceptVisitor(this, us);
			}
			cu.Usings.Add(us);
			return data;
		}
		
		public override object VisitUsing(AST.Using u, object data)
		{
			Debug.Assert(data is DefaultUsing);
			DefaultUsing us = (DefaultUsing)data;
			if (u.IsAlias) {
				IReturnType rt = CreateReturnType(u.Alias);
				if (rt != null) {
					us.AddAlias(u.Name, rt);
				}
			} else {
				us.Usings.Add(u.Name);
			}
			return data;
		}
		
		void ConvertAttributes(AST.AttributedNode from, AbstractDecoration to)
		{
			if (from.Attributes.Count == 0) {
				to.Attributes = DefaultAttribute.EmptyAttributeList;
			} else {
				ICSharpCode.NRefactory.Location location = from.Attributes[0].StartLocation;
				ClassFinder context;
				if (to is IClass) {
					context = new ClassFinder((IClass)to, location.Line, location.Column);
				} else {
					context = new ClassFinder(to.DeclaringType, location.Line, location.Column);
				}
				to.Attributes = VisitAttributes(from.Attributes, context);
			}
		}
		
		List<IAttribute> VisitAttributes(List<AST.AttributeSection> attributes, ClassFinder context)
		{
			// TODO Expressions???
			List<IAttribute> result = new List<IAttribute>();
			foreach (AST.AttributeSection section in attributes) {
				
				AttributeTarget target = AttributeTarget.None;
				if (section.AttributeTarget != null && section.AttributeTarget != "") {
					switch (section.AttributeTarget.ToUpperInvariant()) {
						case "ASSEMBLY":
							target = AttributeTarget.Assembly;
							break;
						case "FIELD":
							target = AttributeTarget.Field;
							break;
						case "EVENT":
							target = AttributeTarget.Event;
							break;
						case "METHOD":
							target = AttributeTarget.Method;
							break;
						case "MODULE":
							target = AttributeTarget.Module;
							break;
						case "PARAM":
							target = AttributeTarget.Param;
							break;
						case "PROPERTY":
							target = AttributeTarget.Property;
							break;
						case "RETURN":
							target = AttributeTarget.Return;
							break;
						case "TYPE":
							target = AttributeTarget.Type;
							break;
						default:
							target = AttributeTarget.None;
							break;
							
					}
				}
				
				foreach (AST.Attribute attribute in section.Attributes) {
					result.Add(new DefaultAttribute(new AttributeReturnType(context, attribute.Name), target));
				}
			}
			return result;
		}
		
		public override object VisitNamespaceDeclaration(AST.NamespaceDeclaration namespaceDeclaration, object data)
		{
			string name;
			if (currentNamespace.Count == 0) {
				name = namespaceDeclaration.Name;
			} else {
				name = currentNamespace.Peek() + '.' + namespaceDeclaration.Name;
			}
			
			currentNamespace.Push(name);
			object ret = namespaceDeclaration.AcceptChildren(this, data);
			currentNamespace.Pop();
			return ret;
		}
		
		ClassType TranslateClassType(AST.ClassType type)
		{
			switch (type) {
				case AST.ClassType.Enum:
					return ClassType.Enum;
				case AST.ClassType.Interface:
					return ClassType.Interface;
				case AST.ClassType.Struct:
					return ClassType.Struct;
				case AST.ClassType.Module:
					return ClassType.Module;
				default:
					return ClassType.Class;
			}
		}
		
		static DomRegion GetRegion(RefParser.Location start, RefParser.Location end)
		{
			return new DomRegion(start, end);
		}
		
		public override object VisitTypeDeclaration(AST.TypeDeclaration typeDeclaration, object data)
		{
			DomRegion region = GetRegion(typeDeclaration.StartLocation, typeDeclaration.EndLocation);
			DomRegion bodyRegion = GetRegion(typeDeclaration.BodyStartLocation, typeDeclaration.EndLocation);
			
			DefaultClass c = new DefaultClass(cu, TranslateClassType(typeDeclaration.Type), ConvertTypeModifier(typeDeclaration.Modifier), region, GetCurrentClass());
			c.BodyRegion = bodyRegion;
			ConvertAttributes(typeDeclaration, c);
			c.Documentation = GetDocumentation(region.BeginLine, typeDeclaration.Attributes);
			
			if (currentClass.Count > 0) {
				DefaultClass cur = GetCurrentClass();
				cur.InnerClasses.Add(c);
				c.FullyQualifiedName = cur.FullyQualifiedName + '.' + typeDeclaration.Name;
			} else {
				if (currentNamespace.Count == 0) {
					c.FullyQualifiedName = typeDeclaration.Name;
				} else {
					c.FullyQualifiedName = currentNamespace.Peek() + '.' + typeDeclaration.Name;
				}
				cu.Classes.Add(c);
			}
			currentClass.Push(c);
			
			if (c.ClassType != ClassType.Enum && typeDeclaration.BaseTypes != null) {
				foreach (AST.TypeReference type in typeDeclaration.BaseTypes) {
					IReturnType rt = CreateReturnType(type);
					if (rt != null) {
						c.BaseTypes.Add(rt);
					}
				}
			}
			
			ConvertTemplates(typeDeclaration.Templates, c); // resolve constrains in context of the class
			
			object ret = typeDeclaration.AcceptChildren(this, data);
			currentClass.Pop();
			
			if (c.ClassType == ClassType.Module) {
				foreach (IField f in c.Fields) {
					f.Modifiers |= ModifierEnum.Static;
				}
				foreach (IMethod m in c.Methods) {
					m.Modifiers |= ModifierEnum.Static;
				}
				foreach (IProperty p in c.Properties) {
					p.Modifiers |= ModifierEnum.Static;
				}
				foreach (IEvent e in c.Events) {
					e.Modifiers |= ModifierEnum.Static;
				}
			}
			
			return ret;
		}
		
		void ConvertTemplates(IList<AST.TemplateDefinition> templateList, DefaultClass c)
		{
			int index = 0;
			if (templateList.Count == 0) {
				c.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
			} else {
				foreach (AST.TemplateDefinition template in templateList) {
					c.TypeParameters.Add(ConvertConstraints(template, new DefaultTypeParameter(c, template.Name, index++)));
				}
			}
		}
		
		void ConvertTemplates(List<AST.TemplateDefinition> templateList, DefaultMethod m)
		{
			int index = 0;
			if (templateList.Count == 0) {
				m.TypeParameters = DefaultTypeParameter.EmptyTypeParameterList;
			} else {
				foreach (AST.TemplateDefinition template in templateList) {
					m.TypeParameters.Add(ConvertConstraints(template, new DefaultTypeParameter(m, template.Name, index++)));
				}
			}
		}
		
		DefaultTypeParameter ConvertConstraints(AST.TemplateDefinition template, DefaultTypeParameter typeParameter)
		{
			foreach (AST.TypeReference typeRef in template.Bases) {
				if (typeRef == AST.TypeReference.NewConstraint) {
					typeParameter.HasConstructableConstraint = true;
				} else if (typeRef == AST.TypeReference.ClassConstraint) {
					typeParameter.HasReferenceTypeConstraint = true;
				} else if (typeRef == AST.TypeReference.StructConstraint) {
					typeParameter.HasValueTypeConstraint = true;
				} else {
					IReturnType rt = CreateReturnType(typeRef);
					if (rt != null) {
						typeParameter.Constraints.Add(rt);
					}
				}
			}
			return typeParameter;
		}
		
		public override object VisitDelegateDeclaration(AST.DelegateDeclaration delegateDeclaration, object data)
		{
			DomRegion region = GetRegion(delegateDeclaration.StartLocation, delegateDeclaration.EndLocation);
			DefaultClass c = new DefaultClass(cu, ClassType.Delegate, ConvertTypeModifier(delegateDeclaration.Modifier), region, GetCurrentClass());
			c.Documentation = GetDocumentation(region.BeginLine, delegateDeclaration.Attributes);
			ConvertAttributes(delegateDeclaration, c);
			CreateDelegate(c, delegateDeclaration.Name, delegateDeclaration.ReturnType,
			               delegateDeclaration.Templates, delegateDeclaration.Parameters);
			return c;
		}
		
		void CreateDelegate(DefaultClass c, string name, AST.TypeReference returnType, IList<AST.TemplateDefinition> templates, IList<AST.ParameterDeclarationExpression> parameters)
		{
			c.BaseTypes.Add(c.ProjectContent.SystemTypes.Delegate);
			if (currentClass.Count > 0) {
				DefaultClass cur = GetCurrentClass();
				cur.InnerClasses.Add(c);
				c.FullyQualifiedName = cur.FullyQualifiedName + '.' + name;
			} else {
				if (currentNamespace.Count == 0) {
					c.FullyQualifiedName = name;
				} else {
					c.FullyQualifiedName = currentNamespace.Peek() + '.' + name;
				}
				cu.Classes.Add(c);
			}
			currentClass.Push(c); // necessary for CreateReturnType
			ConvertTemplates(templates, c);
			
			List<IParameter> p = new List<IParameter>();
			if (parameters != null) {
				foreach (AST.ParameterDeclarationExpression param in parameters) {
					p.Add(CreateParameter(param));
				}
			}
			AnonymousMethodReturnType.AddDefaultDelegateMethod(c, CreateReturnType(returnType), p);
			
			currentClass.Pop();
		}
		
		IParameter CreateParameter(AST.ParameterDeclarationExpression par)
		{
			return CreateParameter(par, null);
		}
		
		IParameter CreateParameter(AST.ParameterDeclarationExpression par, IMethod method)
		{
			return CreateParameter(par, method, GetCurrentClass(), cu);
		}
		
		internal static IParameter CreateParameter(AST.ParameterDeclarationExpression par, IMethod method, IClass currentClass, ICompilationUnit cu)
		{
			IReturnType parType = CreateReturnType(par.TypeReference, method, currentClass, cu);
			DefaultParameter p = new DefaultParameter(par.ParameterName, parType, GetRegion(par.StartLocation, par.EndLocation));
			p.Modifiers = (ParameterModifiers)par.ParamModifier;
			return p;
		}
		
		public override object VisitMethodDeclaration(AST.MethodDeclaration methodDeclaration, object data)
		{
			DomRegion region     = GetRegion(methodDeclaration.StartLocation, methodDeclaration.EndLocation);
			DomRegion bodyRegion = GetRegion(methodDeclaration.EndLocation, methodDeclaration.Body != null ? methodDeclaration.Body.EndLocation : RefParser.Location.Empty);
			DefaultClass c  = GetCurrentClass();
			
			DefaultMethod method = new DefaultMethod(methodDeclaration.Name, null, ConvertModifier(methodDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
			method.IsExtensionMethod = methodDeclaration.IsExtensionMethod;
			method.Documentation = GetDocumentation(region.BeginLine, methodDeclaration.Attributes);
			ConvertTemplates(methodDeclaration.Templates, method);
			method.ReturnType = CreateReturnType(methodDeclaration.TypeReference, method);
			ConvertAttributes(methodDeclaration, method);
			if (methodDeclaration.Parameters != null && methodDeclaration.Parameters.Count > 0) {
				foreach (AST.ParameterDeclarationExpression par in methodDeclaration.Parameters) {
					method.Parameters.Add(CreateParameter(par, method));
				}
			} else {
				method.Parameters = DefaultParameter.EmptyParameterList;
			}
			c.Methods.Add(method);
			return null;
		}
		
		public override object VisitOperatorDeclaration(AST.OperatorDeclaration operatorDeclaration, object data)
		{
			DefaultClass c  = GetCurrentClass();
			DomRegion region     = GetRegion(operatorDeclaration.StartLocation, operatorDeclaration.EndLocation);
			DomRegion bodyRegion = GetRegion(operatorDeclaration.EndLocation, operatorDeclaration.Body != null ? operatorDeclaration.Body.EndLocation : RefParser.Location.Empty);
			
			DefaultMethod method = new DefaultMethod(operatorDeclaration.Name, CreateReturnType(operatorDeclaration.TypeReference), ConvertModifier(operatorDeclaration.Modifier), region, bodyRegion, c);
			ConvertAttributes(operatorDeclaration, method);
			if(operatorDeclaration.Parameters != null)
			{
				foreach (AST.ParameterDeclarationExpression par in operatorDeclaration.Parameters) {
					method.Parameters.Add(CreateParameter(par, method));
				}
			}
			c.Methods.Add(method);
			return null;
		}
		
		public override object VisitConstructorDeclaration(AST.ConstructorDeclaration constructorDeclaration, object data)
		{
			DomRegion region     = GetRegion(constructorDeclaration.StartLocation, constructorDeclaration.EndLocation);
			DomRegion bodyRegion = GetRegion(constructorDeclaration.EndLocation, constructorDeclaration.Body != null ? constructorDeclaration.Body.EndLocation : RefParser.Location.Empty);
			DefaultClass c = GetCurrentClass();
			
			Constructor constructor = new Constructor(ConvertModifier(constructorDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
			constructor.Documentation = GetDocumentation(region.BeginLine, constructorDeclaration.Attributes);
			ConvertAttributes(constructorDeclaration, constructor);
			if (constructorDeclaration.Parameters != null) {
				foreach (AST.ParameterDeclarationExpression par in constructorDeclaration.Parameters) {
					constructor.Parameters.Add(CreateParameter(par));
				}
			}
			c.Methods.Add(constructor);
			return null;
		}
		
		public override object VisitDestructorDeclaration(AST.DestructorDeclaration destructorDeclaration, object data)
		{
			DomRegion region     = GetRegion(destructorDeclaration.StartLocation, destructorDeclaration.EndLocation);
			DomRegion bodyRegion = GetRegion(destructorDeclaration.EndLocation, destructorDeclaration.Body != null ? destructorDeclaration.Body.EndLocation : RefParser.Location.Empty);
			
			DefaultClass c = GetCurrentClass();
			
			Destructor destructor = new Destructor(region, bodyRegion, c);
			ConvertAttributes(destructorDeclaration, destructor);
			c.Methods.Add(destructor);
			return null;
		}
		
		bool IsVisualBasic {
			get {
				return cu.ProjectContent.Language == LanguageProperties.VBNet;
			}
		}
		
		public override object VisitFieldDeclaration(AST.FieldDeclaration fieldDeclaration, object data)
		{
			DomRegion region = GetRegion(fieldDeclaration.StartLocation, fieldDeclaration.EndLocation);
			DefaultClass c = GetCurrentClass();
			ModifierEnum modifier = ConvertModifier(fieldDeclaration.Modifier,
			                                        (c.ClassType == ClassType.Struct && this.IsVisualBasic)
			                                        ? ModifierEnum.Public : ModifierEnum.Private);
			string doku = GetDocumentation(region.BeginLine, fieldDeclaration.Attributes);
			if (currentClass.Count > 0) {
				for (int i = 0; i < fieldDeclaration.Fields.Count; ++i) {
					AST.VariableDeclaration field = (AST.VariableDeclaration)fieldDeclaration.Fields[i];
					
					IReturnType retType;
					if (c.ClassType == ClassType.Enum) {
						retType = c.DefaultReturnType;
					} else {
						retType = CreateReturnType(fieldDeclaration.GetTypeForField(i));
						if (!field.FixedArrayInitialization.IsNull)
							retType = new ArrayReturnType(cu.ProjectContent, retType, 1);
					}
					DefaultField f = new DefaultField(retType, field.Name, modifier, region, c);
					ConvertAttributes(fieldDeclaration, f);
					f.Documentation = doku;
					if (c.ClassType == ClassType.Enum) {
						f.Modifiers = ModifierEnum.Const | ModifierEnum.Public;
					}
					
					c.Fields.Add(f);
				}
			}
			return null;
		}
		
		public override object VisitPropertyDeclaration(AST.PropertyDeclaration propertyDeclaration, object data)
		{
			DomRegion region     = GetRegion(propertyDeclaration.StartLocation, propertyDeclaration.EndLocation);
			DomRegion bodyRegion = GetRegion(propertyDeclaration.BodyStart,     propertyDeclaration.BodyEnd);
			
			IReturnType type = CreateReturnType(propertyDeclaration.TypeReference);
			DefaultClass c = GetCurrentClass();
			
			DefaultProperty property = new DefaultProperty(propertyDeclaration.Name, type, ConvertModifier(propertyDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
			if (propertyDeclaration.HasGetRegion) {
				property.GetterRegion = GetRegion(propertyDeclaration.GetRegion.StartLocation, propertyDeclaration.GetRegion.EndLocation);
				property.CanGet = true;
				property.GetterModifiers = ConvertModifier(propertyDeclaration.GetRegion.Modifier, ModifierEnum.None);
			}
			if (propertyDeclaration.HasSetRegion) {
				property.SetterRegion = GetRegion(propertyDeclaration.SetRegion.StartLocation, propertyDeclaration.SetRegion.EndLocation);
				property.CanSet = true;
				property.SetterModifiers = ConvertModifier(propertyDeclaration.SetRegion.Modifier, ModifierEnum.None);
			}
			property.Documentation = GetDocumentation(region.BeginLine, propertyDeclaration.Attributes);
			ConvertAttributes(propertyDeclaration, property);
			c.Properties.Add(property);
			return null;
		}
		
		public override object VisitEventDeclaration(AST.EventDeclaration eventDeclaration, object data)
		{
			DomRegion region     = GetRegion(eventDeclaration.StartLocation, eventDeclaration.EndLocation);
			DomRegion bodyRegion = GetRegion(eventDeclaration.BodyStart,     eventDeclaration.BodyEnd);
			DefaultClass c = GetCurrentClass();
			
			IReturnType type;
			if (eventDeclaration.TypeReference.IsNull) {
				DefaultClass del = new DefaultClass(cu, ClassType.Delegate,
				                                    ConvertModifier(eventDeclaration.Modifier),
				                                    region, c);
				del.Modifiers |= ModifierEnum.Synthetic;
				CreateDelegate(del, eventDeclaration.Name + "EventHandler",
				               new AST.TypeReference("System.Void"),
				               new AST.TemplateDefinition[0],
				               eventDeclaration.Parameters);
				type = del.DefaultReturnType;
			} else {
				type = CreateReturnType(eventDeclaration.TypeReference);
			}
			DefaultEvent e = new DefaultEvent(eventDeclaration.Name, type, ConvertModifier(eventDeclaration.Modifier), region, bodyRegion, c);
			ConvertAttributes(eventDeclaration, e);
			c.Events.Add(e);
			if (e != null) {
				e.Documentation = GetDocumentation(region.BeginLine, eventDeclaration.Attributes);
			} else {
				LoggingService.Warn("NRefactoryASTConvertVisitor: " + eventDeclaration + " has no events!");
			}
			return null;
		}
		
		public override object VisitIndexerDeclaration(AST.IndexerDeclaration indexerDeclaration, object data)
		{
			DomRegion region     = GetRegion(indexerDeclaration.StartLocation, indexerDeclaration.EndLocation);
			DomRegion bodyRegion = GetRegion(indexerDeclaration.BodyStart,     indexerDeclaration.BodyEnd);
			DefaultProperty i = new DefaultProperty("Indexer", CreateReturnType(indexerDeclaration.TypeReference), ConvertModifier(indexerDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
			i.IsIndexer = true;
			i.Documentation = GetDocumentation(region.BeginLine, indexerDeclaration.Attributes);
			ConvertAttributes(indexerDeclaration, i);
			if (indexerDeclaration.Parameters != null) {
				foreach (AST.ParameterDeclarationExpression par in indexerDeclaration.Parameters) {
					i.Parameters.Add(CreateParameter(par));
				}
			}
			DefaultClass c = GetCurrentClass();
			c.Properties.Add(i);
			return null;
		}
		
		IReturnType CreateReturnType(AST.TypeReference reference, IMethod method)
		{
			return CreateReturnType(reference, method, GetCurrentClass(), cu);
		}
		
		static IReturnType CreateReturnType(AST.TypeReference reference, IMethod method, IClass currentClass, ICompilationUnit cu)
		{
			if (currentClass == null) {
				return TypeVisitor.CreateReturnType(reference, new DefaultClass(cu, "___DummyClass"), method, 1, 1, cu.ProjectContent, true);
			} else {
				return TypeVisitor.CreateReturnType(reference, currentClass, method, currentClass.Region.BeginLine + 1, 1, cu.ProjectContent, true);
			}
		}
		
		IReturnType CreateReturnType(AST.TypeReference reference)
		{
			return CreateReturnType(reference, null);
		}
	}
}

