// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision$</version>
// </file>

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation for IExplicitInterfaceImplementation.
	/// </summary>
	public sealed class DefaultExplicitInterfaceImplementation : IExplicitInterfaceImplementation
	{
		public ITypeReference InterfaceType { get; private set; }
		public string MemberName { get; private set; }
		
		public DefaultExplicitInterfaceImplementation(ITypeReference interfaceType, string memberName)
		{
			if (interfaceType == null)
				throw new ArgumentNullException("interfaceType");
			if (memberName == null)
				throw new ArgumentNullException("memberName");
			this.InterfaceType = interfaceType;
			this.MemberName = memberName;
		}
		
		bool IFreezable.IsFrozen {
			get { return true; }
		}
		
		void IFreezable.Freeze()
		{
		}
	}
}
