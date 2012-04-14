// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ServiceModel.Description;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ServiceReferenceDiscoveryClient
	{
		const int DiscoveryClientsUsed = 2;
		
		Uri discoveryUrl;
		BackgroundWorker mexDiscoveryBackgroundWorker = new BackgroundWorker();
		BackgroundWorker mexRelativePathDiscoveryBackgroundWorker = new BackgroundWorker();
		List<Exception> errors = new List<Exception>();
		
		public ServiceReferenceDiscoveryClient()
		{
		}
		
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
			DiscoverMexMetadata();
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
