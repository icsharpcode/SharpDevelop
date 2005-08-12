// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

// created on 04.08.2003 at 17:49
using System;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using RefParser = ICSharpCode.NRefactory.Parser;
using AST = ICSharpCode.NRefactory.Parser.AST;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom.NRefactoryResolver
{
	public class NRefactoryASTConvertVisitor : RefParser.AbstractASTVisitor
	{
		ICompilationUnit cu;
		Stack currentNamespace = new Stack();
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
		
		ModifierEnum ConvertModifier(AST.Modifier m)
		{
			if (currentClass.Count > 0 && currentClass.Peek().ClassType == ClassType.Interface)
				return ConvertModifier(m, ModifierEnum.Public);
			else
				return ConvertModifier(m, ModifierEnum.Private);
		}
		
		ModifierEnum ConvertModifier(AST.Modifier m, ModifierEnum defaultModifier)
		{
			ModifierEnum r = (ModifierEnum)m;
			if (r == ModifierEnum.None)
				return defaultModifier;
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
		
		string GetDocumentation(int line)
		{
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
					m = left + (line - leftLine) * (right - left) / (rightLine - leftLine);
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
		
		public override object Visit(AST.CompilationUnit compilationUnit, object data)
		{
			//TODO: usings, Comments
			if (compilationUnit == null) {
				return null;
			}
			compilationUnit.AcceptChildren(this, data);
			return cu;
		}
		
		public override object Visit(AST.UsingDeclaration usingDeclaration, object data)
		{
			DefaultUsing us = new DefaultUsing(cu.ProjectContent, GetRegion(usingDeclaration.StartLocation, usingDeclaration.EndLocation));
			foreach (AST.Using u in usingDeclaration.Usings) {
				u.AcceptVisitor(this, us);
			}
			cu.Usings.Add(us);
			return data;
		}
		
		public override object Visit(AST.Using u, object data)
		{
			Debug.Assert(data is DefaultUsing);
			DefaultUsing us = (DefaultUsing)data;
			if (u.IsAlias) {
				IReturnType rt = CreateReturnType(u.Alias);
				if (rt != null) {
					us.Aliases[u.Name] = rt;
				}
			} else {
				us.Usings.Add(u.Name);
			}
			return data;
		}
		
		List<IAttribute> VisitAttributes(List<AST.AttributeSection> attributes)
		{
			// TODO Expressions???
			List<IAttribute> result = new List<IAttribute>();
			foreach (AST.AttributeSection section in attributes) {
				
				AttributeTarget target = AttributeTarget.None;
				if (section.AttributeTarget != null && section.AttributeTarget != "") {
					switch (section.AttributeTarget.ToUpper()) {
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
					//IAttribute a = new DefaultAttribute(attribute.Name, target, new ArrayList(attribute.PositionalArguments), new SortedList());
					//foreach (AST.NamedArgumentExpression n in attribute.NamedArguments) {
					//	a.NamedArguments[n.Name] = n.Expression;
					//}
					result.Add(new DefaultAttribute(attribute.Name, target));
				}
			}
			return result;
		}
		
		public override object Visit(AST.NamespaceDeclaration namespaceDeclaration, object data)
		{
			string name;
			if (currentNamespace.Count == 0) {
				name = namespaceDeclaration.Name;
			} else {
				name = (string)currentNamespace.Peek() + '.' + namespaceDeclaration.Name;
			}
			
			currentNamespace.Push(name);
			object ret = namespaceDeclaration.AcceptChildren(this, data);
			currentNamespace.Pop();
			return ret;
		}
		
		ClassType TranslateClassType(AST.Types type)
		{
			switch (type) {
				case AST.Types.Enum:
					return ClassType.Enum;
				case AST.Types.Interface:
					return ClassType.Interface;
				case AST.Types.Struct:
					return ClassType.Struct;
				case AST.Types.Module:
					return ClassType.Module;
				default:
					return ClassType.Class;
			}
		}
		
		DefaultRegion GetRegion(Point start, Point end)
		{
			return new DefaultRegion(start.Y, start.X, end.Y, end.X);
		}
		
		public override object Visit(AST.TypeDeclaration typeDeclaration, object data)
		{
			DefaultRegion region = GetRegion(typeDeclaration.StartLocation, typeDeclaration.EndLocation);
			DefaultClass c = new DefaultClass(cu, TranslateClassType(typeDeclaration.Type), ConvertModifier(typeDeclaration.Modifier, ModifierEnum.Internal), region, GetCurrentClass());
			c.Attributes.AddRange(VisitAttributes(typeDeclaration.Attributes));
			c.Documentation = GetDocumentation(region.BeginLine);
			
			if (currentClass.Count > 0) {
				DefaultClass cur = GetCurrentClass();
				cur.InnerClasses.Add(c);
				c.FullyQualifiedName = cur.FullyQualifiedName + '.' + typeDeclaration.Name;
			} else {
				if (currentNamespace.Count == 0) {
					c.FullyQualifiedName = typeDeclaration.Name;
				} else {
					c.FullyQualifiedName = (string)currentNamespace.Peek() + '.' + typeDeclaration.Name;
				}
				cu.Classes.Add(c);
			}
			currentClass.Push(c);
			
			if (c.ClassType != ClassType.Enum && typeDeclaration.BaseTypes != null) {
				foreach (AST.TypeReference type in typeDeclaration.BaseTypes) {
					c.BaseTypes.Add(CreateReturnType(type));
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
		
		void ConvertTemplates(List<AST.TemplateDefinition> templateList, IClass c)
		{
			int index = 0;
			foreach (AST.TemplateDefinition template in templateList) {
				c.TypeParameters.Add(ConvertConstraints(template, new DefaultTypeParameter(c, template.Name, index++)));
			}
		}
		
		void ConvertTemplates(List<AST.TemplateDefinition> templateList, IMethod m)
		{
			int index = 0;
			foreach (AST.TemplateDefinition template in templateList) {
				m.TypeParameters.Add(ConvertConstraints(template, new DefaultTypeParameter(m, template.Name, index++)));
			}
		}
		
		DefaultTypeParameter ConvertConstraints(AST.TemplateDefinition template, DefaultTypeParameter typeParameter)
		{
			foreach (AST.TypeReference typeRef in template.Bases) {
				typeParameter.Constraints.Add(CreateReturnType(typeRef));
			}
			return typeParameter;
		}
		
		public override object Visit(AST.DelegateDeclaration delegateDeclaration, object data)
		{
			DefaultRegion region = GetRegion(delegateDeclaration.StartLocation, delegateDeclaration.EndLocation);
			DefaultClass c = new DefaultClass(cu, ClassType.Delegate, ConvertModifier(delegateDeclaration.Modifier, ModifierEnum.Internal), region, GetCurrentClass());
			c.Documentation = GetDocumentation(region.BeginLine);
			c.Attributes.AddRange(VisitAttributes(delegateDeclaration.Attributes));
			c.BaseTypes.Add(ReflectionReturnType.CreatePrimitive(typeof(Delegate)));
			if (currentClass.Count > 0) {
				DefaultClass cur = GetCurrentClass();
				cur.InnerClasses.Add(c);
				c.FullyQualifiedName = cur.FullyQualifiedName + '.' + delegateDeclaration.Name;
			} else {
				if (currentNamespace.Count == 0) {
					c.FullyQualifiedName = delegateDeclaration.Name;
				} else {
					c.FullyQualifiedName = (string)currentNamespace.Peek() + '.' + delegateDeclaration.Name;
				}
				cu.Classes.Add(c);
			}
			currentClass.Push(c); // necessary for CreateReturnType
			ConvertTemplates(delegateDeclaration.Templates, c);
			DefaultMethod invokeMethod = new DefaultMethod("Invoke", CreateReturnType(delegateDeclaration.ReturnType), ModifierEnum.Public, null, null, c);
			if (delegateDeclaration.Parameters != null) {
				foreach (AST.ParameterDeclarationExpression par in delegateDeclaration.Parameters) {
					invokeMethod.Parameters.Add(CreateParameter(par));
				}
			}
			c.Methods.Add(invokeMethod);
			invokeMethod = new DefaultMethod("BeginInvoke", CreateReturnType(typeof(IAsyncResult)), ModifierEnum.Public, null, null, c);
			if (delegateDeclaration.Parameters != null) {
				foreach (AST.ParameterDeclarationExpression par in delegateDeclaration.Parameters) {
					invokeMethod.Parameters.Add(CreateParameter(par));
				}
			}
			invokeMethod.Parameters.Add(new DefaultParameter("callback", CreateReturnType(typeof(AsyncCallback)), null));
			invokeMethod.Parameters.Add(new DefaultParameter("object", ReflectionReturnType.Object, null));
			c.Methods.Add(invokeMethod);
			invokeMethod = new DefaultMethod("EndInvoke", CreateReturnType(delegateDeclaration.ReturnType), ModifierEnum.Public, null, null, c);
			invokeMethod.Parameters.Add(new DefaultParameter("result", CreateReturnType(typeof(IAsyncResult)), null));
			c.Methods.Add(invokeMethod);
			currentClass.Pop();
			return c;
		}
		
		IParameter CreateParameter(AST.ParameterDeclarationExpression par)
		{
			return CreateParameter(par, null);
		}
		
		IParameter CreateParameter(AST.ParameterDeclarationExpression par, IMethod method)
		{
			IReturnType parType = CreateReturnType(par.TypeReference, method);
			DefaultParameter p = new DefaultParameter(par.ParameterName, parType, new DefaultRegion(par.StartLocation, par.EndLocation));
			p.Modifiers = (ParameterModifiers)par.ParamModifier;
			return p;
		}
		
		public override object Visit(AST.MethodDeclaration methodDeclaration, object data)
		{
			DefaultRegion region     = GetRegion(methodDeclaration.StartLocation, methodDeclaration.EndLocation);
			DefaultRegion bodyRegion = GetRegion(methodDeclaration.EndLocation, methodDeclaration.Body != null ? methodDeclaration.Body.EndLocation : new Point(-1, -1));
			DefaultClass c  = GetCurrentClass();
			
			DefaultMethod method = new DefaultMethod(methodDeclaration.Name, null, ConvertModifier(methodDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
			method.Documentation = GetDocumentation(region.BeginLine);
			ConvertTemplates(methodDeclaration.Templates, method);
			method.ReturnType = CreateReturnType(methodDeclaration.TypeReference, method);
			method.Attributes.AddRange(VisitAttributes(methodDeclaration.Attributes));
			if (methodDeclaration.Parameters != null) {
				foreach (AST.ParameterDeclarationExpression par in methodDeclaration.Parameters) {
					method.Parameters.Add(CreateParameter(par, method));
				}
			}
			c.Methods.Add(method);
			return null;
		}
		
		public override object Visit(AST.OperatorDeclaration operatorDeclaration, object data)
		{
			DefaultClass c  = GetCurrentClass();
			DefaultRegion region     = GetRegion(operatorDeclaration.StartLocation, operatorDeclaration.EndLocation);
			DefaultRegion bodyRegion = GetRegion(operatorDeclaration.EndLocation, operatorDeclaration.Body != null ? operatorDeclaration.Body.EndLocation : new Point(-1, -1));
			
			DefaultMethod method = new DefaultMethod(operatorDeclaration.Name, CreateReturnType(operatorDeclaration.ConvertToType), ConvertModifier(operatorDeclaration.Modifier), region, bodyRegion, c);
			if(operatorDeclaration.Parameters != null)
			{
				foreach (AST.ParameterDeclarationExpression par in operatorDeclaration.Parameters) {
					method.Parameters.Add(CreateParameter(par, method));
				}
			}
			c.Methods.Add(method);
			return null;
		}
		
		public override object Visit(AST.ConstructorDeclaration constructorDeclaration, object data)
		{
			DefaultRegion region     = GetRegion(constructorDeclaration.StartLocation, constructorDeclaration.EndLocation);
			DefaultRegion bodyRegion = GetRegion(constructorDeclaration.EndLocation, constructorDeclaration.Body != null ? constructorDeclaration.Body.EndLocation : new Point(-1, -1));
			DefaultClass c = GetCurrentClass();
			
			Constructor constructor = new Constructor(ConvertModifier(constructorDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
			constructor.Documentation = GetDocumentation(region.BeginLine);
			constructor.Attributes.AddRange(VisitAttributes(constructorDeclaration.Attributes));
			if (constructorDeclaration.Parameters != null) {
				foreach (AST.ParameterDeclarationExpression par in constructorDeclaration.Parameters) {
					constructor.Parameters.Add(CreateParameter(par));
				}
			}
			c.Methods.Add(constructor);
			return null;
		}
		
		public override object Visit(AST.DestructorDeclaration destructorDeclaration, object data)
		{
			DefaultRegion region     = GetRegion(destructorDeclaration.StartLocation, destructorDeclaration.EndLocation);
			DefaultRegion bodyRegion = GetRegion(destructorDeclaration.EndLocation, destructorDeclaration.Body != null ? destructorDeclaration.Body.EndLocation : new Point(-1, -1));
			
			DefaultClass c = GetCurrentClass();
			
			Destructor destructor = new Destructor(region, bodyRegion, c);
			destructor.Attributes.AddRange(VisitAttributes(destructorDeclaration.Attributes));
			c.Methods.Add(destructor);
			return null;
		}
		
		
		public override object Visit(AST.FieldDeclaration fieldDeclaration, object data)
		{
			DefaultRegion region = GetRegion(fieldDeclaration.StartLocation, fieldDeclaration.EndLocation);
			DefaultClass c = GetCurrentClass();
			string doku = GetDocumentation(region.BeginLine);
			if (currentClass.Count > 0) {
				for (int i = 0; i < fieldDeclaration.Fields.Count; ++i) {
					AST.VariableDeclaration field = (AST.VariableDeclaration)fieldDeclaration.Fields[i];
					
					IReturnType retType;
					if (c.ClassType == ClassType.Enum)
						retType = c.DefaultReturnType;
					else
						retType = CreateReturnType(fieldDeclaration.GetTypeForField(i));
					DefaultField f = new DefaultField(retType, field.Name, ConvertModifier(fieldDeclaration.Modifier), region, c);
					f.Attributes.AddRange(VisitAttributes(fieldDeclaration.Attributes));
					f.Documentation = doku;
					if (c.ClassType == ClassType.Enum) {
						f.Modifiers = ModifierEnum.Const | ModifierEnum.Public;
					}
					
					c.Fields.Add(f);
				}
			}
			return null;
		}
		
		public override object Visit(AST.PropertyDeclaration propertyDeclaration, object data)
		{
			DefaultRegion region     = GetRegion(propertyDeclaration.StartLocation, propertyDeclaration.EndLocation);
			DefaultRegion bodyRegion = GetRegion(propertyDeclaration.BodyStart,     propertyDeclaration.BodyEnd);
			
			IReturnType type = CreateReturnType(propertyDeclaration.TypeReference);
			DefaultClass c = GetCurrentClass();
			
			DefaultProperty property = new DefaultProperty(propertyDeclaration.Name, type, ConvertModifier(propertyDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
			if (propertyDeclaration.GetRegion != null) {
				property.GetterRegion = GetRegion(propertyDeclaration.GetRegion.StartLocation, propertyDeclaration.GetRegion.EndLocation);
			}
			if (propertyDeclaration.SetRegion != null) {
				property.SetterRegion = GetRegion(propertyDeclaration.SetRegion.StartLocation, propertyDeclaration.SetRegion.EndLocation);
			}
			property.Documentation = GetDocumentation(region.BeginLine);
			property.Attributes.AddRange(VisitAttributes(propertyDeclaration.Attributes));
			c.Properties.Add(property);
			return null;
		}
		
		public override object Visit(AST.EventDeclaration eventDeclaration, object data)
		{
			DefaultRegion region     = GetRegion(eventDeclaration.StartLocation, eventDeclaration.EndLocation);
			DefaultRegion bodyRegion = GetRegion(eventDeclaration.BodyStart,     eventDeclaration.BodyEnd);
			IReturnType type = CreateReturnType(eventDeclaration.TypeReference);
			DefaultClass c = GetCurrentClass();
			DefaultEvent e = null;

			if (eventDeclaration.VariableDeclarators != null && eventDeclaration.VariableDeclarators.Count > 0) {
				foreach (ICSharpCode.NRefactory.Parser.AST.VariableDeclaration varDecl in eventDeclaration.VariableDeclarators) {
					e = new DefaultEvent(varDecl.Name, type, ConvertModifier(eventDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
					e.Attributes.AddRange(VisitAttributes(eventDeclaration.Attributes));
					c.Events.Add(e);
				}
			} else {
				e = new DefaultEvent(eventDeclaration.Name, type, ConvertModifier(eventDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
				e.Attributes.AddRange(VisitAttributes(eventDeclaration.Attributes));
				c.Events.Add(e);
			}
			if (e != null) {
				e.Documentation = GetDocumentation(region.BeginLine);
			} else {
				LoggingService.Warn("NRefactoryASTConvertVisitor: " + eventDeclaration + " has no events!");
			}
			return null;
		}
		
		public override object Visit(AST.IndexerDeclaration indexerDeclaration, object data)
		{
			DefaultRegion region     = GetRegion(indexerDeclaration.StartLocation, indexerDeclaration.EndLocation);
			DefaultRegion bodyRegion = GetRegion(indexerDeclaration.BodyStart,     indexerDeclaration.BodyEnd);
			List<IParameter> parameters = new List<IParameter>();
			DefaultIndexer i = new DefaultIndexer(CreateReturnType(indexerDeclaration.TypeReference), parameters, ConvertModifier(indexerDeclaration.Modifier), region, bodyRegion, GetCurrentClass());
			i.Documentation = GetDocumentation(region.BeginLine);
			i.Attributes.AddRange(VisitAttributes(indexerDeclaration.Attributes));
			if (indexerDeclaration.Parameters != null) {
				foreach (AST.ParameterDeclarationExpression par in indexerDeclaration.Parameters) {
					parameters.Add(CreateParameter(par));
				}
			}
			DefaultClass c = GetCurrentClass();
			c.Indexer.Add(i);
			return null;
		}
		
		IReturnType CreateReturnType(AST.TypeReference reference, IMethod method)
		{
			IClass c = GetCurrentClass();
			if (c == null) {
				return TypeVisitor.CreateReturnType(reference, new DefaultClass(cu, "___DummyClass"), method, 1, 1, cu.ProjectContent, true);
			} else {
				return TypeVisitor.CreateReturnType(reference, c, method, c.Region.BeginLine + 1, 1, cu.ProjectContent, true);
			}
		}
		
		IReturnType CreateReturnType(AST.TypeReference reference)
		{
			return CreateReturnType(reference, null);
		}
		
		IReturnType CreateReturnType(Type type)
		{
			return ReflectionReturnType.Create(ProjectContentRegistry.Mscorlib, type, false);
		}
	}
}
