// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Holds information needed when an async web discovery call has completed.
	/// </summary>
	public class AsyncDiscoveryState 
	{
		WebServiceDiscoveryClientProtocol protocol;
		Uri uri;
		DiscoveryNetworkCredential credential;
		
		public WebServiceDiscoveryClientProtocol Protocol {
			get {
				return protocol;
			}
		}
		
		public Uri Uri {
			get {
				return uri;
			}
		}
		
		public DiscoveryNetworkCredential Credential {
			get {
				return credential;
			}
		}
		
		public AsyncDiscoveryState(WebServiceDiscoveryClientProtocol protocol, Uri uri, DiscoveryNetworkCredential credential)
		{
			this.protocol = protocol;
			this.uri = uri;
			this.credential = credential;
		}
	}
}
