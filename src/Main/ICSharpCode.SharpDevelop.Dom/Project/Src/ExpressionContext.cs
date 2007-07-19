// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Class describing a context in which an expression can be.
	/// Serves as filter for code completion results, but the contexts exposed as static fields
	/// can also be used as a kind of enumeration for special behaviour in the resolver.
	/// </summary>
	public abstract class ExpressionContext
	{
		#region Instance members
		public abstract bool ShowEntry(object o);
		
		protected bool readOnly = true;
		object suggestedItem;
		
		/// <summary>
		/// Gets if the expression is in the context of an object creation.
		/// </summary>
		public virtual bool IsObjectCreation {
			get {
				return false;
			}
			set {
				if (value)
					throw new NotSupportedException();
			}
		}
		
		/// <summary>
		/// Gets/Sets the default item that should be included in a code completion popup
		/// in this context and selected as default value.
		/// </summary>
		/// <example>
		/// "List&lt;TypeName&gt; var = new *expr*();" has as suggested item the pseudo-class
		/// "List&lt;TypeName&gt;".
		/// </example>
		public object SuggestedItem {
			get {
				return suggestedItem;
			}
			set {
				if (readOnly)
					throw new NotSupportedException();
				suggestedItem = value;
			}
		}
		
		/// <summary>
		/// Gets if the context expects an attribute.
		/// </summary>
		public virtual bool IsAttributeContext {
			get {
				return false;
			}
		}
		
		public virtual bool IsTypeContext {
			get { return false; }
		}
		#endregion
		
		#region C# specific contexts (public static fields) * MOVE TO ANOTHER CLASS *
		/// <summary>The context expects a new identifier</summary>
		/// <example>class *expr* {}; string *expr*;</example>
		public static ExpressionContext IdentifierExpected = new DefaultExpressionContext("IdentifierExpected");
		
		/// <summary>The context is outside of any type declaration, expecting a global-level keyword.</summary>
		public static ExpressionContext Global = new DefaultExpressionContext("Global");
		
		/// <summary>The context is the body of a property declaration.</summary>
		/// <example>string Name { *expr* }</example>
		public static ExpressionContext PropertyDeclaration = new DefaultExpressionContext("PropertyDeclaration");
		
		/// <summary>The context is the body of a property declaration inside an interface.</summary>
		/// <example>string Name { *expr* }</example>
		public static ExpressionContext InterfacePropertyDeclaration = new DefaultExpressionContext("InterfacePropertyDeclaration");
		
		/// <summary>The context is the body of a event declaration.</summary>
		/// <example>event EventHandler NameChanged { *expr* }</example>
		public static ExpressionContext EventDeclaration = new DefaultExpressionContext("EventDeclaration");
		
		/// <summary>The context is the body of a method.</summary>
		/// <example>void Main () { *expr* }</example>
		public static ExpressionContext MethodBody = new DefaultExpressionContext("MethodBody");
		
		
		/// <summary>The context is the body of a type declaration.</summary>
		public static ExpressionContext TypeDeclaration = new NonStaticTypeExpressionContext("TypeDeclaration", true);
		
		/// <summary>The context is the body of an interface declaration.</summary>
		public static ExpressionContext InterfaceDeclaration = new NonStaticTypeExpressionContext("InterfaceDeclaration", true);
		
		/// <summary>Context expects "base" or "this".</summary>
		/// <example>public ClassName() : *expr*</example>
		public static ExpressionContext BaseConstructorCall = new DefaultExpressionContext("BaseConstructorCall");
		
		/// <summary>The first parameter</summary>
		/// <example>MethodName(*expr*)</example>
		public static ExpressionContext FirstParameterType = new NonStaticTypeExpressionContext("FirstParameterType", false);
		
		/// <summary>Another parameter</summary>
		/// <example>MethodName(..., *expr*)</example>
		public static ExpressionContext ParameterType = new NonStaticTypeExpressionContext("ParameterType", false);
		
		/// <summary>Context expects a fully qualified type name.</summary>
		/// <example>using Alias = *expr*;</example>
		public static ExpressionContext FullyQualifiedType = new DefaultExpressionContext("FullyQualifiedType");
		#endregion
		
		#region Default contexts (public static fields)
		/// <summary>Default/unknown context</summary>
		public static ExpressionContext Default = new DefaultExpressionContext("Default");
		
		/// <summary>The context expects the base type of an enum.</summary>
		/// <example>enum Name : *expr* {}</example>
		public static ExpressionContext EnumBaseType = new EnumBaseTypeExpressionContext();
		
		/// <summary>Context expects a non-sealed type or interface</summary>
		/// <example>class C : *expr* {}</example>
		public static ExpressionContext InheritableType = new InheritableTypeExpressionContext();
		
		/// <summary>Context expects a namespace name</summary>
		/// <example>using *expr*;</example>
		public static ExpressionContext Namespace = new ImportableExpressionContext(false);
		
		/// <summary>Context expects an importable type (namespace or class with public static members)</summary>
		/// <example>Imports *expr*;</example>
		public static ExpressionContext Importable = new ImportableExpressionContext(true);
		
		/// <summary>Context expects a type name</summary>
		/// <example>typeof(*expr*)</example>
		public static ExpressionContext Type = new TypeExpressionContext(null, false, true);
		
		/// <summary>Context expects the name of a non-static, non-void type</summary>
		/// <example>is *expr*, *expr* variableName</example>
		public static ExpressionContext NonStaticNonVoidType = new NonStaticTypeExpressionContext("NonStaticType", false);
		
		/// <summary>Context expects a non-abstract type that has accessible constructors</summary>
		/// <example>new *expr*();</example>
		/// <remarks>When using this context, a resolver should treat the expression as object creation,
		/// even when the keyword "new" is not part of the expression.</remarks>
		public static ExpressionContext ObjectCreation = new TypeExpressionContext(null, true, true);
		
		/// <summary>Context expects a type deriving from System.Attribute.</summary>
		/// <example>[*expr*()]</example>
		/// <remarks>When using this context, a resolver should try resolving typenames with an
		/// appended "Attribute" suffix and treat "invocations" of the attribute type as
		/// object creation.</remarks>
		public static ExpressionContext GetAttribute(IProjectContent projectContent)
		{
			return new TypeExpressionContext(projectContent.SystemTypes.Attribute, false, true);
		}
		
		/// <summary>Context expects a type name which has special base type</summary>
		/// <param name="baseClass">The class the expression must derive from.</param>
		/// <param name="isObjectCreation">Specifies whether classes must be constructable.</param>
		/// <example>catch(*expr* ...), using(*expr* ...), throw new ***</example>
		public static ExpressionContext TypeDerivingFrom(IReturnType baseType, bool isObjectCreation)
		{
			return new TypeExpressionContext(baseType, isObjectCreation, false);
		}
		
		/// <summary>Context expects an interface</summary>
		/// <example>interface C : *expr* {}</example>
		/// <example>Implements *expr*</example>
		public static ExpressionContext Interface = new ClassTypeExpressionContext(ClassType.Interface);
		
		/// <summary>Context expects a delegate</summary>
		/// <example>public event *expr*</example>
		public static ExpressionContext DelegateType = new ClassTypeExpressionContext(ClassType.Delegate);
		
		#endregion
		
		#region DefaultExpressionContext
		sealed class DefaultExpressionContext : ExpressionContext
		{
			string name;
			
			public DefaultExpressionContext(string name)
			{
				this.name = name;
			}
			
			public override bool ShowEntry(object o)
			{
				return true;
			}
			
			public override string ToString()
			{
				return "[DefaultExpressionContext: " + name + "]";
			}
		}
		#endregion
		
		#region NamespaceExpressionContext
		sealed class ImportableExpressionContext : ExpressionContext
		{
			bool allowImportClasses;
			
			public ImportableExpressionContext(bool allowImportClasses)
			{
				this.allowImportClasses = allowImportClasses;
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string)
					return true;
				IClass c = o as IClass;
				if (allowImportClasses && c != null) {
					return c.HasPublicOrInternalStaticMembers;
				}
				return false;
			}
			
			public override string ToString()
			{
				if (allowImportClasses)
					return "[ImportableExpressionContext]";
				else
					return "[NamespaceExpressionContext]";
			}
		}
		#endregion
		
		#region TypeExpressionContext
		sealed class TypeExpressionContext : ExpressionContext
		{
			IClass baseClass;
			bool isObjectCreation;
			
			public TypeExpressionContext(IReturnType baseType, bool isObjectCreation, bool readOnly)
			{
				if (baseType != null)
					baseClass = baseType.GetUnderlyingClass();
				this.isObjectCreation = isObjectCreation;
				this.readOnly = readOnly;
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string)
					return true;
				IClass c = o as IClass;
				if (c == null)
					return false;
				if (isObjectCreation) {
					if (c.IsAbstract || c.IsStatic)    return false;
					if (c.ClassType == ClassType.Enum || c.ClassType == ClassType.Interface)
						return false;
				}
				if (baseClass == null)
					return true;
				return c.IsTypeInInheritanceTree(baseClass);
			}
			
			public override bool IsObjectCreation {
				get {
					return isObjectCreation;
				}
				set {
					if (readOnly && value != isObjectCreation)
						throw new NotSupportedException();
					isObjectCreation = value;
				}
			}
			
			public override bool IsAttributeContext {
				get {
					return baseClass != null && baseClass.FullyQualifiedName == "System.Attribute";
				}
			}
			
			public override bool IsTypeContext {
				get { return true; }
			}
			
			public override string ToString()
			{
				if (baseClass != null)
					return "[" + GetType().Name + ": " + baseClass.FullyQualifiedName
						+ " IsObjectCreation=" + IsObjectCreation + "]";
				else
					return "[" + GetType().Name + " IsObjectCreation=" + IsObjectCreation + "]";
			}
		}
		#endregion
		
		#region CombinedExpressionContext
		public static ExpressionContext operator | (ExpressionContext a, ExpressionContext b)
		{
			return new CombinedExpressionContext(0, a, b);
		}
		
		public static ExpressionContext operator & (ExpressionContext a, ExpressionContext b)
		{
			return new CombinedExpressionContext(1, a, b);
		}
		
		public static ExpressionContext operator ^ (ExpressionContext a, ExpressionContext b)
		{
			return new CombinedExpressionContext(2, a, b);
		}
		
		sealed class CombinedExpressionContext : ExpressionContext
		{
			byte opType; // 0 = or ; 1 = and ; 2 = xor
			ExpressionContext a;
			ExpressionContext b;
			
			public CombinedExpressionContext(byte opType, ExpressionContext a, ExpressionContext b)
			{
				if (a == null)
					throw new ArgumentNullException("a");
				if (b == null)
					throw new ArgumentNullException("a");
				this.opType = opType;
				this.a = a;
				this.b = b;
			}
			
			public override bool ShowEntry(object o)
			{
				if (opType == 0)
					return a.ShowEntry(o) || b.ShowEntry(o);
				if (opType == 1)
					return a.ShowEntry(o) && b.ShowEntry(o);
				return a.ShowEntry(o) ^ b.ShowEntry(o);
			}
			
			public override string ToString()
			{
				string op = " XOR ";
				if (opType == 0)
					op = " OR ";
				else if (opType == 1)
					op = " AND ";
				return "[" + GetType().Name + ": " + a + op + b + "]";
			}
		}
		#endregion
		
		#region EnumBaseTypeExpressionContext
		sealed class EnumBaseTypeExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				IClass c = o as IClass;
				if (c != null) {
					// use this hack to show dummy classes like "short"
					if (c.Methods.Count > 0) {
						c = c.Methods[0].DeclaringType;
					}
					switch (c.FullyQualifiedName) {
						case "System.Byte":
						case "System.SByte":
						case "System.Int16":
						case "System.UInt16":
						case "System.Int32":
						case "System.UInt32":
						case "System.Int64":
						case "System.UInt64":
							return true;
						default:
							return false;
					}
				} else {
					return false;
				}
			}
		}
		#endregion
		
		#region InheritableTypeExpressionContext
		sealed class InheritableTypeExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				if (o is string) return true;
				IClass c = o as IClass;
				if (c != null) {
					foreach (IClass innerClass in c.InnerClasses) {
						if (ShowEntry(innerClass)) return true;
					}
					if (c.ClassType == ClassType.Interface) return true;
					if (c.ClassType == ClassType.Class) {
						if (!c.IsSealed && !c.IsStatic) return true;
					}
				}
				return false;
			}
		}
		#endregion
		
		#region ClassTypeExpressionContext
		sealed class ClassTypeExpressionContext : ExpressionContext
		{
			readonly ClassType expectedType;
			
			public ClassTypeExpressionContext(ClassType expectedType)
			{
				this.expectedType = expectedType;
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string) return true;
				IClass c = o as IClass;
				if (c != null) {
					foreach (IClass innerClass in c.InnerClasses) {
						if (ShowEntry(innerClass)) return true;
					}
					if (c.ClassType == expectedType) return true;
				}
				return false;
			}
			
			public override string ToString()
			{
				return "[" + GetType().Name + " expectedType=" + expectedType.ToString() + "]";
			}
		}
		#endregion
		
		#region NonStaticTypeExpressionContext
		sealed class NonStaticTypeExpressionContext : ExpressionContext
		{
			string name;
			bool allowVoid;
			
			public NonStaticTypeExpressionContext(string name, bool allowVoid)
			{
				this.name = name;
				this.allowVoid = allowVoid;
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string) return true;
				IClass c = o as IClass;
				if (c != null) {
					if (!allowVoid) {
						if (c.FullyQualifiedName == "System.Void" || c.FullyQualifiedName == "void") return false;
					}
					
					foreach (IClass innerClass in c.InnerClasses) {
						if (ShowEntry(innerClass)) return true;
					}
					if (!c.IsStatic && c.ClassType != ClassType.Module) return true;
				}
				return false;
			}
			
			public override string ToString()
			{
				return "[" + GetType().Name + " " + name + "]";
			}
		}
		#endregion
	}
}

