// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.NRefactory.TypeSystem.Implementation
{
	/// <summary>
	/// Default implementation for IExplicitInterfaceImplementation.
	/// </summary>
	public sealed class DefaultExplicitInterfaceImplementation : Immutable, IExplicitInterfaceImplementation
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
	}
}
