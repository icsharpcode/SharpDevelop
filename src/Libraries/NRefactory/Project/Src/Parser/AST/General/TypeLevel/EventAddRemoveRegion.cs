// EventAddRegion.cs
// Copyright (C) 2003 Mike Krueger (mike@icsharpcode.net)
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public abstract class EventAddRemoveRegion : AttributedNode, INullable
	{
		BlockStatement block = BlockStatement.Null;
		ArrayList parameters;
		
		public BlockStatement Block {
			get {
				return block;
			}
			set {
				Debug.Assert(value != null);
				block = value;
			}
		}
		
		public ArrayList Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new ArrayList(1) : value;
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
}
