// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Andrea Paatz" email="andrea@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public enum ConstructorInitializerType {
		None,
		Base,
		This
	}
	
	public class ConstructorInitializer : AbstractNode, INullable
	{
		ConstructorInitializerType constructorInitializerType = ConstructorInitializerType.None;
		List<Expression>           arguments                  = new List<Expression>(1);
		
		public ConstructorInitializerType ConstructorInitializerType {
			get {
				return constructorInitializerType;
			}
			set {
				constructorInitializerType = value;
			}
		}
		
		public List<Expression> Arguments {
			get {
				return arguments;
			}
			set {
				arguments = value ?? new List<Expression>(1);
			}
		}
		
		public virtual bool IsNull {
			get {
				return false;
			}
		}
		
		public static NullConstructorInitializer Null {
			get {
				return NullConstructorInitializer.Instance;
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[ConstructorInitializer: ConstructorInitializerType = {0}, Arguments = {1}]",
			                     ConstructorInitializerType,
			                     GetCollectionString(arguments));
		}
	}
	
	public class NullConstructorInitializer : ConstructorInitializer
	{
		static NullConstructorInitializer nullConstructorInitializer = new NullConstructorInitializer();
		
		public static NullConstructorInitializer Instance {
			get {
				return nullConstructorInitializer;
			}
		}
		
		public override bool IsNull {
			get {
				return true;
			}
		}
		
		NullConstructorInitializer()
		{
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return data;
		}
		
		public override string ToString()
		{
			return String.Format("[NullConstructorInitializer]");
		}
	}
}
