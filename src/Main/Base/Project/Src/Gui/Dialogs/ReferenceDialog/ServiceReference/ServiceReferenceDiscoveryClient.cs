// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.ServiceModel.Description;
using System.Web.Services.Discovery;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ServiceReferenceDiscoveryClient
	{
		const int DiscoveryClientsUsed = 3;
		
		Uri discoveryUrl;
		BackgroundWorker webServiceDiscoveryBackgroundWorker = new BackgroundWorker();
		BackgroundWorker mexDiscoveryBackgroundWorker = new BackgroundWorker();
		BackgroundWorker mexRelativePathDiscoveryBackgroundWorker = new BackgroundWorker();
		List<Exception> errors = new List<Exception>();
		
		public event EventHandler<ServiceReferenceDiscoveryEventArgs> DiscoveryComplete;
		
		protected virtual void OnDiscoveryComplete(ServiceReferenceDiscoveryEventArgs e)
		{
			if (DiscoveryComplete != null) {
				DiscoveryComplete(this, e);
			}
		}
		
		public void Discover(Uri url)
		{
			this.discoveryUrl = url;
			DiscoverWebService();
			DiscoverMexMetadata();
		}
		
		void DiscoverWebService()
		{
			webServiceDiscoveryBackgroundWorker.DoWork += DiscoverWebServiceMetadata;
			webServiceDiscoveryBackgroundWorker.RunWorkerCompleted += DiscoveryCompleted;
			webServiceDiscoveryBackgroundWorker.RunWorkerAsync(discoveryUrl);
		}
		
		void DiscoverWebServiceMetadata(object sender, DoWorkEventArgs e)
		{
			Uri url = (Uri)e.Argument;
			var client = new DiscoveryClientProtocol();
			client.Credentials = GetCredentials();
			DiscoveryDocument document = client.DiscoverAny(url.AbsoluteUri);
			client.ResolveOneLevel();
			
			e.Result = new ServiceReferenceDiscoveryEventArgs(client.References);
		}
		
		DiscoveryNetworkCredential GetCredentials()
		{
			return new DiscoveryNetworkCredential(
				CredentialCache.DefaultNetworkCredentials, 
				DiscoveryNetworkCredential.DefaultAuthenticationType);
		}
		
		void DiscoverMexMetadata()
		{
			DiscoverMexMetadata(mexDiscoveryBackgroundWorker, discoveryUrl);
			DiscoverMexMetadata(mexRelativePathDiscoveryBackgroundWorker, GetRelativeUrl("mex"));
		}
		
		Uri GetRelativeUrl(string relativeUrl)
		{
			return new Uri(GetDiscoveryUrlWithTrailingSlash(), relativeUrl);
		}
		
		Uri GetDiscoveryUrlWithTrailingSlash()
		{
			if (discoveryUrl.AbsoluteUri.EndsWith("/")) {
				return discoveryUrl;
			}
			return new Uri(discoveryUrl.AbsoluteUri + "/");
		}
		
		void DiscoverMexMetadata(BackgroundWorker worker, Uri url)
		{
			worker.DoWork += DiscoverMexMetadata;
			worker.RunWorkerCompleted += DiscoveryCompleted;
			worker.RunWorkerAsync(url);
		}

		void DiscoverMexMetadata(object sender, DoWorkEventArgs e)
		{
			Uri url = (Uri)e.Argument;
			var client = new MetadataExchangeClient(url, MetadataExchangeClientMode.MetadataExchange);
			MetadataSet metadata = client.GetMetadata();
			e.Result = new ServiceReferenceDiscoveryEventArgs(metadata);
		}
		
		void DiscoveryCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null) {
				OnDiscoveryError(e.Error);
			} else {
				OnDiscoveryComplete((ServiceReferenceDiscoveryEventArgs)e.Result);
			}
		}
		
		void OnDiscoveryError(Exception ex)
		{
			errors.Add(ex);
			if (errors.Count == DiscoveryClientsUsed) {
				OnDiscoveryComplete(new ServiceReferenceDiscoveryEventArgs(errors));
			}
		}
	}
}
