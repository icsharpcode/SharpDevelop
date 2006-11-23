// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public abstract class AbstractMember : AbstractNamedEntity, IMember
	{
		IReturnType returnType;
		DomRegion region;
		DomRegion bodyRegion;
		List<ExplicitInterfaceImplementation> interfaceImplementations;
		IReturnType declaringTypeReference;
		
		public virtual DomRegion Region {
			get {
				return region;
			}
			set {
				region = value;
			}
		}
		
		public virtual DomRegion BodyRegion {
			get {
				return bodyRegion;
			}
			protected set {
				bodyRegion = value;
			}
		}
		
		public virtual IReturnType ReturnType {
			get {
				return returnType;
			}
			set {
				returnType = value;
			}
		}
		
		/// <summary>
		/// Gets the declaring type reference (declaring type incl. type arguments)
		/// </summary>
		public virtual IReturnType DeclaringTypeReference {
			get {
				return declaringTypeReference ?? this.DeclaringType.DefaultReturnType;
			}
			set {
				declaringTypeReference = value;
			}
		}
		
		public IList<ExplicitInterfaceImplementation> InterfaceImplementations {
			get {
				return interfaceImplementations ?? (interfaceImplementations = new List<ExplicitInterfaceImplementation>());
			}
		}
		
		public AbstractMember(IClass declaringType, string name) : base(declaringType, name)
		{
		}
		
		public abstract IMember Clone();
		
		object ICloneable.Clone()
		{
			return this.Clone();
		}
	}
}
