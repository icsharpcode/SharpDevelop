// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ICSharpCode.NRefactory.Parser.AST;

namespace ICSharpCode.SharpDevelop.Refactoring
{
	/// <summary>
	/// Provides code generation facilities.
	/// </summary>
	public abstract class CodeGenerator
	{
		#region DOM -> NRefactory conversion (static)
		public static TypeReference ConvertType(IReturnType returnType, ClassFinder context)
		{
			if (returnType == null)           return TypeReference.Null;
			if (returnType is NullReturnType) return TypeReference.Null;
			
			TypeReference typeRef;
			if (context != null && CanUseShortTypeName(returnType, context))
				typeRef = new TypeReference(returnType.Name);
			else
				typeRef = new TypeReference(returnType.FullyQualifiedName);
			while (returnType.ArrayDimensions > 0) {
				int[] rank = typeRef.RankSpecifier ?? new int[0];
				Array.Resize(ref rank, rank.Length + 1);
				rank[rank.Length - 1] = returnType.ArrayDimensions - 1;
				typeRef.RankSpecifier = rank;
				returnType = returnType.ArrayElementType;
			}
			if (returnType.TypeArguments != null) {
				foreach (IReturnType typeArgument in returnType.TypeArguments) {
					typeRef.GenericTypes.Add(ConvertType(typeArgument, context));
				}
			}
			return typeRef;
		}
		
		/// <summary>
		/// Returns true if the short name of a type is valid in the given context.
		/// Returns false for primitive types because they should be passed around using their
		/// fully qualified names to allow the ambience or output visitor to use the intrinsic
		/// type name.
		/// </summary>
		public static bool CanUseShortTypeName(IReturnType returnType, ClassFinder context)
		{
			switch (returnType.FullyQualifiedName) {
				case "System.Void":
				case "System.String":
				case "System.Char":
				case "System.Boolean":
				case "System.Single":
				case "System.Double":
				case "System.Decimal":
				case "System.Byte":
				case "System.SByte":
				case "System.Int16":
				case "System.Int32":
				case "System.Int64":
				case "System.UInt16":
				case "System.UInt32":
				case "System.UInt64":
					return false;
			}
			int typeArgumentCount = (returnType.TypeArguments != null) ? returnType.TypeArguments.Count : 0;
			IReturnType typeInTargetContext = context.SearchType(returnType.Name, typeArgumentCount);
			return typeInTargetContext != null && typeInTargetContext.FullyQualifiedName == returnType.FullyQualifiedName;
		}
		
		public static Modifier ConvertModifier(ModifierEnum m)
		{
			return (Modifier)m;
		}
		
		public static ParamModifier ConvertModifier(ParameterModifiers m)
		{
			return (ParamModifier)m;
		}
		
		public static List<ParameterDeclarationExpression> ConvertParameters(IList<IParameter> parameters, ClassFinder targetContext)
		{
			List<ParameterDeclarationExpression> l = new List<ParameterDeclarationExpression>(parameters.Count);
			foreach (IParameter p in parameters) {
				ParameterDeclarationExpression pd = new ParameterDeclarationExpression(ConvertType(p.ReturnType, targetContext),
				                                                                       p.Name,
				                                                                       ConvertModifier(p.Modifiers));
				pd.Attributes = ConvertAttributes(p.Attributes, targetContext);
				l.Add(pd);
			}
			return l;
		}
		
		public static List<AttributeSection> ConvertAttributes(IList<IAttribute> attributes, ClassFinder targetContext)
		{
			AttributeSection sec = new AttributeSection();
			foreach (IAttribute att in attributes) {
				sec.Attributes.Add(new ICSharpCode.NRefactory.Parser.AST.Attribute(att.Name, null, null));
			}
			List<AttributeSection> resultList = new List<AttributeSection>(1);
			if (sec.Attributes.Count > 0)
				resultList.Add(sec);
			return resultList;
		}
		
		public static List<TemplateDefinition> ConvertTemplates(IList<ITypeParameter> l, ClassFinder targetContext)
		{
			List<TemplateDefinition> o = new List<TemplateDefinition>(l.Count);
			foreach (ITypeParameter p in l) {
				TemplateDefinition td = new TemplateDefinition(p.Name, ConvertAttributes(p.Attributes, targetContext));
				foreach (IReturnType rt in p.Constraints) {
					td.Bases.Add(ConvertType(rt, targetContext));
				}
				o.Add(td);
			}
			return o;
		}
		
		public static BlockStatement CreateNotImplementedBlock()
		{
			BlockStatement b = new BlockStatement();
			b.AddChild(new ThrowStatement(new ObjectCreateExpression(new TypeReference("NotImplementedException"), null)));
			return b;
		}
		
		public static ParametrizedNode ConvertMember(IMethod m, ClassFinder targetContext)
		{
			if (m.IsConstructor) {
				return new ConstructorDeclaration(m.Name,
				                                  ConvertModifier(m.Modifiers),
				                                  ConvertParameters(m.Parameters, targetContext),
				                                  ConvertAttributes(m.Attributes, targetContext));
			} else {
				MethodDeclaration md;
				md = new MethodDeclaration(m.Name,
				                           ConvertModifier(m.Modifiers),
				                           ConvertType(m.ReturnType, targetContext),
				                           ConvertParameters(m.Parameters, targetContext),
				                           ConvertAttributes(m.Attributes, targetContext));
				md.Templates = ConvertTemplates(m.TypeParameters, targetContext);
				md.Body = CreateNotImplementedBlock();
				return md;
			}
		}
		
		public static AttributedNode ConvertMember(IMember m, ClassFinder targetContext)
		{
			if (m == null)
				throw new ArgumentNullException("m");
			if (m is IProperty)
				return ConvertMember((IProperty)m, targetContext);
			else if (m is IMethod)
				return ConvertMember((IMethod)m, targetContext);
			else if (m is IEvent)
				return ConvertMember((IEvent)m, targetContext);
			else if (m is IField)
				return ConvertMember((IField)m, targetContext);
			else
				throw new ArgumentException("Unknown member: " + m.GetType().FullName);
		}
		
		public static AttributedNode ConvertMember(IProperty p, ClassFinder targetContext)
		{
			if (p.IsIndexer) {
				return new IndexerDeclaration(ConvertType(p.ReturnType, targetContext),
				                              ConvertParameters(p.Parameters, targetContext),
				                              ConvertModifier(p.Modifiers),
				                              ConvertAttributes(p.Attributes, targetContext));
			} else {
				PropertyDeclaration md;
				md = new PropertyDeclaration(p.Name,
				                             ConvertType(p.ReturnType, targetContext),
				                             ConvertModifier(p.Modifiers),
				                             ConvertAttributes(p.Attributes, targetContext));
				md.Parameters = ConvertParameters(p.Parameters, targetContext);
				if (p.CanGet) md.GetRegion = new PropertyGetRegion(CreateNotImplementedBlock(), null);
				if (p.CanSet) md.SetRegion = new PropertySetRegion(CreateNotImplementedBlock(), null);
				return md;
			}
		}
		
		public static FieldDeclaration ConvertMember(IField f, ClassFinder targetContext)
		{
			TypeReference type = ConvertType(f.ReturnType, targetContext);
			FieldDeclaration fd = new FieldDeclaration(ConvertAttributes(f.Attributes, targetContext),
			                                           type, ConvertModifier(f.Modifiers));
			fd.Fields.Add(new VariableDeclaration(f.Name, null, type));
			return fd;
		}
		
		public static EventDeclaration ConvertMember(IEvent e, ClassFinder targetContext)
		{
			return new EventDeclaration(ConvertType(e.ReturnType, targetContext),
			                            e.Name,
			                            ConvertModifier(e.Modifiers),
			                            ConvertAttributes(e.Attributes, targetContext));
		}
		#endregion
		
		#region Code generation / insertion
		public virtual void InsertCodeAfter(IMember member, IDocument document, params AbstractNode[] nodes)
		{
			if (member is IMethodOrProperty) {
				InsertCodeAfter(((IMethodOrProperty)member).BodyRegion.EndLine, document,
				                GetIndentation(document, member.Region.BeginLine), nodes);
			} else {
				InsertCodeAfter(member.Region.EndLine, document,
				                GetIndentation(document, member.Region.BeginLine), nodes);
			}
		}
		
		public virtual void InsertCodeAtEnd(DomRegion region, IDocument document, params AbstractNode[] nodes)
		{
			InsertCodeAfter(region.EndLine - 1, document,
			                GetIndentation(document, region.BeginLine) + '\t', nodes);
		}
		
		public virtual void InsertCodeInClass(IClass c, IDocument document, int targetLine, params AbstractNode[] nodes)
		{
			InsertCodeAfter(targetLine, document,
			                GetIndentation(document, c.Region.BeginLine) + '\t', false, nodes);
		}
		
		protected string GetIndentation(IDocument document, int line)
		{
			LineSegment lineSegment = document.GetLineSegment(line - 1);
			string lineText = document.GetText(lineSegment.Offset, lineSegment.Length);
			return lineText.Substring(0, lineText.Length - lineText.TrimStart().Length);
		}
		
		/// <summary>
		/// Generates code for <paramref name="nodes"/> and inserts it into <paramref name="document"/>
		/// after the line <paramref name="insertLine"/>.
		/// </summary>
		protected void InsertCodeAfter(int insertLine, IDocument document, string indentation, params AbstractNode[] nodes)
		{
			InsertCodeAfter(insertLine, document, indentation, true, nodes);
		}
		
		/// <summary>
		/// Generates code for <paramref name="nodes"/> and inserts it into <paramref name="document"/>
		/// after the line <paramref name="insertLine"/>.
		/// </summary>
		protected void InsertCodeAfter(int insertLine, IDocument document, string indentation, bool startWithEmptyLine, params AbstractNode[] nodes)
		{
			// insert one line below field (text editor uses different coordinates)
			LineSegment lineSegment = document.GetLineSegment(insertLine);
			StringBuilder b = new StringBuilder();
			for (int i = 0; i < nodes.Length; i++) {
				if (startWithEmptyLine || i > 0)
					b.AppendLine(indentation);
				b.Append(GenerateCode(nodes[i], indentation));
			}
			document.Insert(lineSegment.Offset, b.ToString());
			document.RequestUpdate(new TextAreaUpdate(TextAreaUpdateType.WholeTextArea));
			document.CommitUpdate();
		}
		
		/// <summary>
		/// Generates code for the NRefactory node.
		/// </summary>
		public abstract string GenerateCode(AbstractNode node, string indentation);
		#endregion
		
		#region Generate property
		public virtual string GetPropertyName(string fieldName)
		{
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToUpper(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToUpper(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToUpper(fieldName[0]) + fieldName.Substring(1);
		}
		
		public virtual string GetParameterName(string fieldName)
		{
			if (fieldName.StartsWith("_") && fieldName.Length > 1)
				return Char.ToLower(fieldName[1]) + fieldName.Substring(2);
			else if (fieldName.StartsWith("m_") && fieldName.Length > 2)
				return Char.ToLower(fieldName[2]) + fieldName.Substring(3);
			else
				return Char.ToLower(fieldName[0]) + fieldName.Substring(1);
		}
		
		public virtual PropertyDeclaration CreateProperty(IField field, bool createGetter, bool createSetter)
		{
			string name = GetPropertyName(field.Name);
			PropertyDeclaration property = new PropertyDeclaration(name,
			                                                       ConvertType(field.ReturnType, new ClassFinder(field)),
			                                                       ConvertModifier(field.Modifiers), null);
			if (createGetter) {
				BlockStatement block = new BlockStatement();
				block.AddChild(new ReturnStatement(new IdentifierExpression(field.Name)));
				property.GetRegion = new PropertyGetRegion(block, null);
			}
			if (createSetter) {
				BlockStatement block = new BlockStatement();
				Expression left = new IdentifierExpression(field.Name);
				Expression right = new IdentifierExpression("value");
				block.AddChild(new StatementExpression(new AssignmentExpression(left, AssignmentOperatorType.Assign, right)));
				property.SetRegion = new PropertySetRegion(block, null);
			}
			
			property.Modifier = Modifier.Public | (property.Modifier & Modifier.Static);
			return property;
		}
		#endregion
		
		#region Generate Changed Event
		public virtual void CreateChangedEvent(IProperty property, IDocument document)
		{
			string name = property.Name + "Changed";
			EventDeclaration ed = new EventDeclaration(new TypeReference("EventHandler"), name,
			                                           ConvertModifier(property.Modifiers & (ModifierEnum.VisibilityMask | ModifierEnum.Static))
			                                           , null);
			InsertCodeAfter(property, document, ed);
			
			List<Expression> arguments = new List<Expression>(2);
			if (property.IsStatic)
				arguments.Add(new PrimitiveExpression(null, "null"));
			else
				arguments.Add(new ThisReferenceExpression());
			arguments.Add(new FieldReferenceExpression(new IdentifierExpression("EventArgs"), "Empty"));
			InsertCodeAtEnd(property.SetterRegion, document,
			                new RaiseEventStatement(name, arguments));
		}
		#endregion
		
		#region Generate OnEventMethod
		public virtual MethodDeclaration CreateOnEventMethod(IEvent e)
		{
			TypeReference type = ConvertType(e.ReturnType, new ClassFinder(e));
			if (type.Type.EndsWith("Handler"))
				type.Type = type.Type.Substring(0, type.Type.Length - 7) + "Args";
			
			List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>(1);
			parameters.Add(new ParameterDeclarationExpression(type, "e"));
			ModifierEnum modifier;
			if (e.IsStatic)
				modifier = ModifierEnum.Private | ModifierEnum.Static;
			else if (e.DeclaringType.IsSealed)
				modifier = ModifierEnum.Protected;
			else
				modifier = ModifierEnum.Protected | ModifierEnum.Virtual;
			MethodDeclaration method = new MethodDeclaration("On" + e.Name,
			                                                 ConvertModifier(modifier),
			                                                 new TypeReference("System.Void"),
			                                                 parameters, null);
			
			List<Expression> arguments = new List<Expression>(2);
			if (e.IsStatic)
				arguments.Add(new PrimitiveExpression(null, "null"));
			else
				arguments.Add(new ThisReferenceExpression());
			arguments.Add(new IdentifierExpression("e"));
			method.Body = new BlockStatement();
			method.Body.AddChild(new RaiseEventStatement(e.Name, arguments));
			
			return method;
		}
		#endregion
		
		#region Interface implementation
		protected string GetInterfaceName(IReturnType interf, IMember member, ClassFinder context)
		{
			if (CanUseShortTypeName(member.DeclaringType.DefaultReturnType, context))
				return member.DeclaringType.Name;
			else
				return member.DeclaringType.FullyQualifiedName;
		}
		
		public void ImplementInterface(IReturnType interf, IDocument document, bool explicitImpl, ModifierEnum implModifier, IClass targetClass)
		{
			List<AbstractNode> nodes = new List<AbstractNode>();
			InsertCodeAtEnd(targetClass.Region, document, nodes.ToArray());
		}
		
		/// <summary>
		/// Adds the methods implementing the <paramref name="interf"/> to the list
		/// <paramref name="nodes"/>.
		/// </summary>
		public virtual void ImplementInterface(IList<AbstractNode> nodes, IReturnType interf, bool explicitImpl, ModifierEnum implModifier, IClass targetClass)
		{
			ClassFinder context = new ClassFinder(targetClass, targetClass.Region.BeginLine + 1, 0);
			TypeReference interfaceReference = ConvertType(interf, context);
			Modifier modifier = ConvertModifier(implModifier);
			List<IEvent> targetClassEvents = targetClass.DefaultReturnType.GetEvents();
			foreach (IEvent e in interf.GetEvents()) {
				if (targetClassEvents.Find(delegate(IEvent te) { return e.Name == te.Name; }) == null) {
					EventDeclaration ed = ConvertMember(e, context);
					if (explicitImpl) {
						ed.InterfaceImplementations.Add(new InterfaceImplementation(interfaceReference, ed.Name));
					}
					ed.Modifier = modifier;
					nodes.Add(ed);
				}
			}
			List<IProperty> targetClassProperties = targetClass.DefaultReturnType.GetProperties();
			foreach (IProperty p in interf.GetProperties()) {
				if (targetClassProperties.Find(delegate(IProperty tp) { return p.Name == tp.Name; }) == null) {
					AttributedNode pd = ConvertMember(p, context);
					if (explicitImpl) {
						InterfaceImplementation impl = new InterfaceImplementation(interfaceReference, p.Name);
						if (pd is IndexerDeclaration) {
							((IndexerDeclaration)pd).InterfaceImplementations.Add(impl);
						} else {
							((PropertyDeclaration)pd).InterfaceImplementations.Add(impl);
						}
					}
					pd.Modifier = modifier;
					nodes.Add(pd);
				}
			}
			List<IMethod> targetClassMethods = targetClass.DefaultReturnType.GetMethods();
			foreach (IMethod m in interf.GetMethods()) {
				if (targetClassMethods.Find(delegate(IMethod mp) {
				                            	return m.Name == mp.Name && DiffUtility.Compare(m.Parameters, mp.Parameters) == 0;
				                            }) == null)
				{
					MethodDeclaration md = ConvertMember(m, context) as MethodDeclaration;
					if (md != null) {
						if (explicitImpl) {
							md.InterfaceImplementations.Add(new InterfaceImplementation(interfaceReference, md.Name));
						}
						md.Modifier = modifier;
						nodes.Add(md);
					}
				}
			}
		}
		#endregion
		
		#region Override member
		public virtual AttributedNode GetOverridingMethod(IMember baseMember, ClassFinder targetContext)
		{
			AttributedNode node = ConvertMember(baseMember, targetContext);
			node.Modifier &= ~(Modifier.Virtual | Modifier.Abstract);
			node.Modifier |= Modifier.Override;
			
			MethodDeclaration method = node as MethodDeclaration;
			if (method != null) {
				method.Body.Children.Clear();
				if (method.TypeReference.SystemType == "System.Void") {
					method.Body.AddChild(new StatementExpression(CreateForwardingMethodCall(method)));
				} else {
					method.Body.AddChild(new ReturnStatement(CreateForwardingMethodCall(method)));
				}
			}
			PropertyDeclaration property = node as PropertyDeclaration;
			if (property != null) {
				Expression field = new FieldReferenceExpression(new BaseReferenceExpression(),
				                                                property.Name);
				if (!property.GetRegion.Block.IsNull) {
					property.GetRegion.Block.Children.Clear();
					property.GetRegion.Block.AddChild(new ReturnStatement(field));
				}
				if (!property.SetRegion.Block.IsNull) {
					property.SetRegion.Block.Children.Clear();
					Expression expr = new AssignmentExpression(field,
					                                           AssignmentOperatorType.Assign,
					                                           new IdentifierExpression("value"));
					property.SetRegion.Block.AddChild(new StatementExpression(expr));
				}
			}
			return node;
		}
		
		static InvocationExpression CreateForwardingMethodCall(MethodDeclaration method)
		{
			Expression methodName = new FieldReferenceExpression(new BaseReferenceExpression(),
			                                                     method.Name);
			InvocationExpression ie = new InvocationExpression(methodName, null);
			foreach (ParameterDeclarationExpression param in method.Parameters) {
				Expression expr = new IdentifierExpression(param.ParameterName);
				if (param.ParamModifier == ParamModifier.Ref) {
					expr = new DirectionExpression(FieldDirection.Ref, expr);
				} else if (param.ParamModifier == ParamModifier.Out) {
					expr = new DirectionExpression(FieldDirection.Out, expr);
				}
				ie.Arguments.Add(expr);
			}
			return ie;
		}
		#endregion
	}
}
