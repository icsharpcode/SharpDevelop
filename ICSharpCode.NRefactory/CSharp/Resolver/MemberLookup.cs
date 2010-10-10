// Copyright (c) 2010 AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.NRefactory.CSharp.Resolver
{
	/// <summary>
	/// Implementation of member lookup (C# 4.0 spec, §7.4).
	/// </summary>
	public class MemberLookup
	{
		ITypeResolveContext context;
		
		public MemberLookup(ITypeResolveContext context)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			this.context = context;
		}
		
		/// <summary>
		/// Gets whether the member is considered to be invocable.
		/// </summary>
		public bool IsInvocable(IMember member)
		{
			if (member is IEvent || member is IMethod)
				return true;
			if (member.ReturnType == SharedTypes.Dynamic)
				return true;
			return member.ReturnType.Resolve(context).IsDelegate();
		}
		
	}
}
