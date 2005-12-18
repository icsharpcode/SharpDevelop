// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	/// <summary>
	/// Base class for PropertyGetRegion and PropertySetRegion.
	/// </summary>
	public abstract class PropertyGetSetRegion : AttributedNode, INullable
	{
		BlockStatement block = BlockStatement.Null; // can be null if only the definition is there (interface declaration)
		
		public BlockStatement Block {
			get {
				return block;
			}
			set {
				block = BlockStatement.CheckNull(value); ;
			}
		}
		
		public virtual bool IsNull {
			get {
				return false;
			}
		}
		
		public PropertyGetSetRegion(BlockStatement block, List<AttributeSection> attributes) : base(attributes)
		{
			this.Block = block;
		}
	}
	
	public class PropertyGetRegion : PropertyGetSetRegion
	{
		public static NullPropertyGetRegion Null {
			get {
				return NullPropertyGetRegion.Instance;
			}
		}
		
		public PropertyGetRegion(BlockStatement block, List<AttributeSection> attributes) : base(block, attributes)
		{}
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[PropertyGetRegion: Block={0}, Attributes={1}]",
			                     Block,
			                     GetCollectionString(Attributes));
		}
	}
	
	public class PropertySetRegion : PropertyGetSetRegion
	{
		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>(1);
		
		public List<ParameterDeclarationExpression> Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new List<ParameterDeclarationExpression>(1) : value;
			}
		}
		public static NullPropertySetRegion Null {
			get {
				return NullPropertySetRegion.Instance;
			}
		}
		
		public PropertySetRegion(BlockStatement block, List<AttributeSection> attributes) : base(block, attributes)
		{}
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[PropertySetRegion: Block={0}, Attributes={1}]",
			                     Block,
			                     GetCollectionString(Attributes));
		}
	}
	
	public class NullPropertyGetRegion : PropertyGetRegion
	{
		static NullPropertyGetRegion nullPropertyGetRegion = new NullPropertyGetRegion();
		
		public static NullPropertyGetRegion Instance {
			get {
				return nullPropertyGetRegion;
			}
		}
		
		NullPropertyGetRegion() : base(null, null)
		{
		}
		
		public override bool IsNull {
			get {
				return true;
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return data;
		}
		
		public override string ToString()
		{
			return String.Format("[NullPropertyGetSetRegion]");
		}
	}
	
	public class NullPropertySetRegion : PropertySetRegion
	{
		static NullPropertySetRegion nullPropertySetRegion = new NullPropertySetRegion();
		
		public static NullPropertySetRegion Instance {
			get {
				return nullPropertySetRegion;
			}
		}
		
		NullPropertySetRegion() : base(null, null)
		{
		}
		
		public override bool IsNull {
			get {
				return true;
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return data;
		}
		
		public override string ToString()
		{
			return String.Format("[NullPropertyGetSetRegion]");
		}
	}
	
}
