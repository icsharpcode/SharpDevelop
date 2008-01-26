// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Dom
{
	public sealed class ExplicitInterfaceImplementation : Immutable, IEquatable<ExplicitInterfaceImplementation>
	{
		readonly IReturnType interfaceReference;
		readonly string memberName;
		
		public ExplicitInterfaceImplementation(IReturnType interfaceReference, string memberName)
		{
			this.interfaceReference = interfaceReference;
			this.memberName = memberName;
		}
		
		public IReturnType InterfaceReference {
			get { return interfaceReference; }
		}
		
		public string MemberName {
			get { return memberName; }
		}
		
		public ExplicitInterfaceImplementation Clone()
		{
			return this; // object is immutable, no Clone() required
		}
		
		public override int GetHashCode()
		{
			return interfaceReference.GetHashCode() ^ memberName.GetHashCode();
		}
		
		public override bool Equals(object obj)
		{
			return Equals(obj as ExplicitInterfaceImplementation);
		}
		
		public bool Equals(ExplicitInterfaceImplementation other)
		{
			if (other == null)
				return false;
			else
				return this.interfaceReference == other.interfaceReference && this.memberName == other.memberName;
		}
	}
}
