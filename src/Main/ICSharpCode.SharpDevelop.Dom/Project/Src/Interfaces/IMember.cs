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
	public interface IMember : IDecoration, ICloneable
	{
		string FullyQualifiedName {
			get;
		}
		
		/// <summary>
		/// Gets/Sets the declaring type reference (declaring type incl. type arguments).
		/// If set to null, the getter returns the default type reference to the <see cref="DeclaringType"/>.
		/// </summary>
		IReturnType DeclaringTypeReference {
			get;
			set;
		}
		
		/// <summary>
		/// Declaration region of the member (without body!)
		/// </summary>
		DomRegion Region {
			get;
		}
		
		string Name {
			get;
		}
		
		string Namespace {
			get;
		}
		
		string DotNetName {
			get;
		}
		
		IReturnType ReturnType {
			get;
			set;
		}
		
		DomRegion BodyRegion {
			get;
		}
		
		IList<ExplicitInterfaceImplementation> InterfaceImplementations {
			get;
		}
	}
}
