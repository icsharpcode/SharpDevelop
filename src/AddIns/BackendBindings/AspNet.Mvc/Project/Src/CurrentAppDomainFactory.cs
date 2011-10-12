// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.TextTemplating;

namespace ICSharpCode.AspNet.Mvc
{
	[Serializable]
	public class CurrentAppDomainFactory : ITextTemplatingAppDomainFactory
	{
		public ITextTemplatingAppDomain CreateTextTemplatingAppDomain(string applicationBase)
		{
			return new CurrentAppDomain();
		}
	}
}
