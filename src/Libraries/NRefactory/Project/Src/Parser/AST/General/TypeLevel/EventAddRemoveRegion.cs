// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public abstract class EventAddRemoveRegion : AttributedNode, INullable
	{
		BlockStatement block = BlockStatement.Null;
		List<ParameterDeclarationExpression> parameters;
		
		public BlockStatement Block {
			get {
				return block;
			}
			set {
				Debug.Assert(value != null);
				block = value;
			}
		}
		
		public List<ParameterDeclarationExpression> Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new List<ParameterDeclarationExpression>(1) : value;
			}
		}
		
		public virtual bool IsNull {
			get {
				return false;
			}
		}
		
		public EventAddRemoveRegion(List<AttributeSection> attributes) : base(attributes)
		{
			Parameters = null;
		}
		
		protected EventAddRemoveRegion() : base(null)
		{
			Parameters = null;
		}
	}
	
	public class EventAddRegion : EventAddRemoveRegion
	{
		public static EventAddRegion Null {
			get {
				return NullEventAddRegion.Instance;
			}
		}
		
		public EventAddRegion(List<AttributeSection> attributes) : base (attributes)
		{}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[EventAddRegion: Attributes = {0}, Block = {1}]",
			                     GetCollectionString(Attributes),
			                     Block);
		}
	}
	
	public class EventRemoveRegion : EventAddRemoveRegion
	{
		
		public static EventRemoveRegion Null {
			get {
				return NullEventRemoveRegion.Instance;
			}
		}
		
		public EventRemoveRegion(List<AttributeSection> attributes) : base (attributes)
		{}
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[EventRemoveRegion: Attributes = {0}, Block = {1}]",
			                     GetCollectionString(Attributes),
			                     Block);
		}
	}
	
	public class EventRaiseRegion : EventAddRemoveRegion
	{
		
		public static EventRaiseRegion Null {
			get {
				return NullEventRaiseRegion.Instance;
			}
		}
		
		public EventRaiseRegion(List<AttributeSection> attributes) : base (attributes)
		{}
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[EventRaiseRegion: Attributes = {0}, Block = {1}]",
			                     GetCollectionString(Attributes),
			                     Block);
		}
	}
	
	public class NullEventAddRegion : EventAddRegion
	{
		static NullEventAddRegion nullEventAddRegion = new NullEventAddRegion();
		
		public static NullEventAddRegion Instance {
			get {
				return nullEventAddRegion;
			}
		}
		
		NullEventAddRegion() : base(new List<AttributeSection>(1))
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
			return String.Format("[NullEventRemoveRegion]");
		}
	}
	
	public class NullEventRemoveRegion : EventRemoveRegion
	{
		static NullEventRemoveRegion nullEventRemoveRegion = new NullEventRemoveRegion();
		
		public static NullEventRemoveRegion Instance {
			get {
				return nullEventRemoveRegion;
			}
		}
		
		NullEventRemoveRegion() : base(new List<AttributeSection>(1))
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
			return String.Format("[NullEventRemoveRegion]");
		}
	}
	
	public class NullEventRaiseRegion : EventRaiseRegion
	{
		static NullEventRaiseRegion nullEventRaiseRegion = new NullEventRaiseRegion();
		
		public static NullEventRaiseRegion Instance {
			get {
				return nullEventRaiseRegion;
			}
		}
		
		NullEventRaiseRegion() : base(new List<AttributeSection>(1))
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
			return String.Format("[NullEventRaiseRegion]");
		}
	}
}
