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
