// PropertyGetRegion.cs
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
		
		public PropertyGetSetRegion(BlockStatement block, ArrayList attributes) : base(attributes)
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
		
		public PropertyGetRegion(BlockStatement block, ArrayList attributes) : base(block, attributes)
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
//		List<ParameterDeclarationExpression> parameters = new List<ParameterDeclarationExpression>(1);
		ArrayList parameters = new ArrayList(1);
		
		public ArrayList Parameters {
			get {
				return parameters;
			}
			set {
				parameters = value == null ? new ArrayList(1) : value;
			}
		}
		public static NullPropertySetRegion Null {
			get {
				return NullPropertySetRegion.Instance;
			}
		}
		
		public PropertySetRegion(BlockStatement block, ArrayList attributes) : base(block, attributes)
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
