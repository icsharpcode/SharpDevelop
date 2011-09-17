// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Runtime.Remoting.Lifetime;

namespace ICSharpCode.FormsDesigner
{
	/// <summary>
	/// Description of FormsDesignerAppDomainServer.
	/// </summary>
	public class FormsDesignerAppDomainServer : MarshalByRefObject, ISponsor
	{
		public FormsDesignerAppDomainServer()
		{
		}
		
		public TimeSpan Renewal(ILease lease)
		{
			throw new NotImplementedException();
		}
	}
}
