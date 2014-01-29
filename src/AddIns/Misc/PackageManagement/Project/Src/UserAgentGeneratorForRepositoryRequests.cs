// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class UserAgentGeneratorForRepositoryRequests
	{
		SharpDevelopHttpUserAgent userAgent = new SharpDevelopHttpUserAgent();
		
		public UserAgentGeneratorForRepositoryRequests(IPackageRepositoryFactoryEvents repositoryFactoryEvents)
		{
			repositoryFactoryEvents.RepositoryCreated += RepositoryCreated;
		}
		
		void RepositoryCreated(object sender, PackageRepositoryFactoryEventArgs e)
		{
			RegisterHttpClient(e.Repository as IHttpClientEvents);
		}
		
		void RegisterHttpClient(IHttpClientEvents clientEvents)
		{
			if (clientEvents != null) {
				clientEvents.SendingRequest += SendingRequest;
			}
		}
		
		void SendingRequest(object sender, WebRequestEventArgs e)
		{
			HttpUtility.SetUserAgent(e.Request, userAgent.ToString());
		}
	}
}
