// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections;
using System.Collections.Generic;
using System.ServiceModel.Description;
using System.Text;
using System.Web.Services.Description;
using System.Web.Services.Discovery;

using WebServices = System.Web.Services.Description;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class ServiceReferenceDiscoveryEventArgs : EventArgs
	{
		ServiceDescriptionCollection services = new ServiceDescriptionCollection();
		
		public ServiceReferenceDiscoveryEventArgs(IEnumerable<Exception> errors)
		{
			GenerateAggregateError(errors);
		}
		
		void GenerateAggregateError(IEnumerable<Exception> errors)
		{
			var message = new StringBuilder();
			foreach (Exception ex in errors) {
				message.AppendLine(ex.Message);
			}
			Error = new AggregateException(message.ToString(), errors);
		}
		
		public ServiceReferenceDiscoveryEventArgs(DiscoveryClientReferenceCollection references)
		{
			GetServices(references);
		}
		
		void GetServices(DiscoveryClientReferenceCollection references)
		{
			foreach (DictionaryEntry entry in references) {
				var contractRef = entry.Value as ContractReference;
				if (contractRef != null) {
					services.Add(contractRef.Contract);
				}
			}
		}
		
		public ServiceReferenceDiscoveryEventArgs(MetadataSet metadata)
		{
			GetServices(metadata);
		}
		
		void GetServices(MetadataSet metadata)
		{
			foreach (MetadataSection section in metadata.MetadataSections) {
				var service = section.Metadata as WebServices.ServiceDescription;
				if (service != null) {
					Services.Add(service);
				}
			}
		}
		
		public ServiceDescriptionCollection Services {
			get { return services; }
		}
		
		public Exception Error { get; private set; }
		
		public bool HasError {
			get { return Error != null; }
		}
	}
}
