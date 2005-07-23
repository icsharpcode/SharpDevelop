// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Class describing a contexts in which an expressions can be.
	/// Serves as filter for code completion results, but the contexts exposed as static fields
	/// can also be used as a kind of enumeration for special behaviour in the resolver.
	/// </summary>
	public abstract class ExpressionContext
	{
		public abstract bool ShowEntry(object o);
		
		#region Default contexts (public fields)
		/// <summary>Default/unknown context</summary>
		public static ExpressionContext Default = new DefaultExpressionContext();
		
		/// <summary>Context expects a namespace name</summary>
		/// <example>using *expr*;</example>
		public static ExpressionContext Namespace = new NamespaceExpressionContext();
		
		/// <summary>Context expects a type name</summary>
		/// <example>typeof(*expr*), is *expr*, using(*expr* ...)</example>
		public static ExpressionContext Type = new TypeExpressionContext(null, false);
		
		/// <summary>Context expects a non-abstract type that has accessible constructors</summary>
		/// <example>new *expr*();</example>
		/// <remarks>When using this context, a resolver should treat the expression as object creation,
		/// even when the keyword "new" is not part of the expression.</remarks>
		public static ExpressionContext ObjectCreation = new TypeExpressionContext(null, true);
		
		/// <summary>Context expects a non-abstract type deriving from System.Attribute.</summary>
		/// <example>[*expr*()]</example>
		/// <remarks>When using this context, a resolver should try resolving typenames with an
		/// appended "Attribute" suffix and treat "invocations" of the attribute type as
		/// object creation.</remarks>
		public static ExpressionContext Attribute = new TypeExpressionContext(ProjectContentRegistry.Mscorlib.GetClass("System.Attribute"), true);
		
		/// <summary>Context expects a type name which has special base type</summary>
		/// <param name="baseClass">The class the expression must derive from.</param>
		/// <param name="allowAbstract">Specifies whether classes must be constructable.</param>
		/// <example>catch(*expr* ...), using(*expr* ...), throw new ***</example>
		public static ExpressionContext TypeDerivingFrom(IClass baseClass, bool mustBeConstructable)
		{
			return new TypeExpressionContext(baseClass, mustBeConstructable);
		}
		
		/// <summary>Context expeacts an interface</summary>
		public static InterfaceExpressionContext Interface = new InterfaceExpressionContext();
		
		#endregion
		
		#region DefaultExpressionContext
		class DefaultExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				return true;
			}
			
			public override string ToString()
			{
				return "[" + GetType().Name + "]";
			}
		}
		#endregion
		
		#region NamespaceExpressionContext
		class NamespaceExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				return o is string;
			}
			
			public override string ToString()
			{
				return "[" + GetType().Name + "]";
			}
		}
		#endregion
		
		#region TypeExpressionContext
		class TypeExpressionContext : ExpressionContext
		{
			IClass baseClass;
			bool mustBeConstructable;
			
			public TypeExpressionContext(IClass baseClass, bool mustBeConstructable)
			{
				this.baseClass = baseClass;
				this.mustBeConstructable = mustBeConstructable;
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string)
					return true;
				IClass c = o as IClass;
				if (c == null)
					return false;
				if (mustBeConstructable) {
					if (c.IsAbstract || c.IsStatic)    return false;
					if (c.ClassType == ClassType.Enum) return false;
				}
				if (baseClass == null)
					return true;
				return c.IsTypeInInheritanceTree(baseClass);
			}
			
			public override string ToString()
			{
				if (baseClass != null)
					return "[" + GetType().Name + ": " + baseClass.FullyQualifiedName
						+ " mustBeConstructable=" + mustBeConstructable + "]";
				else
					return "[" + GetType().Name + " mustBeConstructable=" + mustBeConstructable + "]";
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
		
		class CombinedExpressionContext : ExpressionContext
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
		
		#region InterfaceExpressionContext
		public class InterfaceExpressionContext : ExpressionContext
		{
			IClass baseClass;
			
			public InterfaceExpressionContext()
			{
			}
			
			public override bool ShowEntry(object o)
			{
				if (o is string)
					return true;
				IClass c = o as IClass;
				if (c == null)
					return false;
				
				return c.ClassType == ClassType.Interface;
			}
			
			public override string ToString()
			{
				return "[" + GetType().Name + "]";
			}
		}
		#endregion
	}
}
