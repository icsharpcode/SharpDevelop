// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version value="$version"/>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Dom
{
	/// <summary>
	/// Class describing a contexts in which an expressions can be.
	/// Serves as filter for code completion results.
	/// </summary>
	public abstract class ExpressionContext
	{
		public abstract bool ShowEntry(object o);
		
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
		public static ExpressionContext ObjectCreation = new TypeExpressionContext(null, true);
		
		/// <summary>Context expects a non-abstract type deriving from System.Attribute.</summary>
		/// <example>[*expr*()]</example>
		public static ExpressionContext Attribute = new TypeExpressionContext(ProjectContentRegistry.Mscorlib.GetClass("System.Attribute"), true);
		
		/// <summary>Context expects a type name which has special base type</summary>
		/// <param name="baseClass">The class the expression must derive from.</param>
		/// <param name="allowAbstract">Specifies whether classes must be constructable.</param>
		/// <example>catch(*expr* ...), using(*expr* ...), throw new ***</example>
		public static ExpressionContext TypeDerivingFrom(IClass baseClass, bool mustBeConstructable)
		{
			return new TypeExpressionContext(baseClass, mustBeConstructable);
		}
		
		class DefaultExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				return true;
			}
		}
		
		class NamespaceExpressionContext : ExpressionContext
		{
			public override bool ShowEntry(object o)
			{
				return o is string;
			}
		}
		
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
		}
	}
}
