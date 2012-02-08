// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.Runtime.Serialization;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceCodeDomBuilder : IServiceReferenceCodeDomBuilder
	{
		public ServiceReferenceCodeDomBuilder()
		{
			this.Namespace = String.Empty;
		}
		
		public string Namespace { get; set; }
		
		public CodeCompileUnit GenerateCompileUnit(MetadataSet metadata)
		{
			ServiceContractGenerator contractGenerator = CreateServiceContractGenerator();
			WsdlImporter importer = CreateWsdlImporter(metadata);
			
			foreach (ContractDescription contract in importer.ImportAllContracts()) {
				contractGenerator.GenerateServiceContractType(contract);
			}
			
			return contractGenerator.TargetCompileUnit;
		}
		
		WsdlImporter CreateWsdlImporter(MetadataSet metadata)
		{
			var importer = new WsdlImporter(metadata);
			var contractImporter = new XsdDataContractImporter();
			contractImporter.Options = new ImportOptions();
			contractImporter.Options.Namespaces.Add("*", Namespace);
			importer.State.Add(typeof(XsdDataContractImporter), contractImporter);
			return importer;
		}
		
		ServiceContractGenerator CreateServiceContractGenerator()
		{
			var contractGenerator = new ServiceContractGenerator();
			contractGenerator.Options = 
				ServiceContractGenerationOptions.ClientClass | 
				ServiceContractGenerationOptions.ChannelInterface;
			contractGenerator.NamespaceMappings.Add("*", Namespace);
			return contractGenerator;
		}
	}
}
