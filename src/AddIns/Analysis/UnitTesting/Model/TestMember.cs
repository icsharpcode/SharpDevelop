// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.NRefactory.TypeSystem;

namespace ICSharpCode.UnitTesting
{
	/// <summary>
	/// Represents a member that can be tested. 
	/// </summary>
	public class TestMember
	{
		IUnresolvedMethod method;
		
		public IUnresolvedMethod Method {
			get { return method; }
		}
		
		public TestMember(IUnresolvedMethod method)
		{
			if (method == null)
				throw new ArgumentNullException("method");
			this.method = method;
		}
	}
}
