// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.CodeDom;
using System.ServiceModel.Description;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceCodeDomBuilder : IServiceReferenceCodeDomBuilder
	{
		public CodeCompileUnit GenerateCompileUnit(MetadataSet metadata)
		{
			var importer = new WsdlImporter(metadata);
			var contractGenerator = new ServiceContractGenerator();
			contractGenerator.Options = ServiceContractGenerationOptions.ClientClass;
			
			foreach (ContractDescription contract in importer.ImportAllContracts()) {
				contractGenerator.GenerateServiceContractType(contract);
			}
			
			return contractGenerator.TargetCompileUnit;
		}
	}
}
