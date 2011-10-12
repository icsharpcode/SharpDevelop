// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	public class CurrentAppDomain : ITextTemplatingAppDomain
	{
		public AppDomain AppDomain {
			get { return AppDomain.CurrentDomain; }
		}
		
		public void Dispose()
		{
		}
	}
}
