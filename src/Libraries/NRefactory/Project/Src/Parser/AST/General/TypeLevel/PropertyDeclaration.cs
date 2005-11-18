// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Drawing;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.AST
{
	public class PropertyDeclaration : ParametrizedNode
	{
		TypeReference   typeReference = TypeReference.Null;
		Point           bodyStart = new Point(-1, -1);
		Point           bodyEnd = new Point(-1, -1);
		List<InterfaceImplementation> interfaceImplementations = new List<InterfaceImplementation>();
		
		PropertyGetRegion  propertyGetRegion = PropertyGetRegion.Null;
		PropertySetRegion  propertySetRegion = PropertySetRegion.Null;
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				Debug.Assert(value != null);
				typeReference = value;
			}
		}
		
		public PropertyGetRegion GetRegion {
			get {
				return propertyGetRegion;
			}
			set {
				if (value == null) {
					propertyGetRegion = PropertyGetRegion.Null;
				} else {
					propertyGetRegion = value;
				}
			}
		}
		public PropertySetRegion SetRegion {
			get {
				return propertySetRegion;
			}
			set {
				if (value == null) {
					propertySetRegion = PropertySetRegion.Null;
				} else {
					propertySetRegion = value;
				}
			}
		}
		
		public bool HasGetRegion {
			get {
				return !propertyGetRegion.IsNull;
			}
		}
		
		public bool HasSetRegion {
			get {
				return !propertySetRegion.IsNull;
			}
		}
		
		public bool IsReadOnly {
			get {
				return HasGetRegion && !HasSetRegion;
			}
		}
		
		public bool IsWriteOnly {
			get {
				return !HasGetRegion && HasSetRegion;
			}
		}
		
		
		public Point BodyStart {
			get {
				return bodyStart;
			}
			set {
				bodyStart = value;
			}
		}
		public Point BodyEnd {
			get {
				return bodyEnd;
			}
			set {
				bodyEnd = value;
			}
		}
		
		public List<InterfaceImplementation> InterfaceImplementations {
			get {
				return interfaceImplementations;
			}
			set {
				interfaceImplementations = value ?? new List<InterfaceImplementation>();
			}
		}
		
		public PropertyDeclaration(string name, TypeReference typeReference, Modifier modifier, List<AttributeSection> attributes) : base(modifier, attributes, name)
		{
			Debug.Assert(typeReference != null);
			this.typeReference = typeReference;
			if ((modifier & Modifier.ReadOnly) == Modifier.ReadOnly) {
				this.GetRegion = new PropertyGetRegion(null, null);
			} else if ((modifier & Modifier.WriteOnly) == Modifier.WriteOnly) {
				this.SetRegion = new PropertySetRegion(null, null);
			}
		}
		
		public override object AcceptVisitor(IASTVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
		public override string ToString()
		{
			return String.Format("[PropertyDeclaration: Name = {0}, Modifier = {1}, TypeReference = {2}, Attributes = {3}, GetRegion = {4}, SetRegion = {5}]",
			                     Name,
			                     Modifier,
			                     TypeReference,
			                     GetCollectionString(Attributes),
			                     GetRegion,
			                     SetRegion);
		}
	}
}
