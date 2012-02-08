// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.ServiceModel.Description;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceFileGenerator : IServiceReferenceFileGenerator
	{
		IServiceReferenceProxyGenerator proxyGenerator;
		IServiceReferenceMapGenerator mapGenerator;
		
		public ServiceReferenceFileGenerator(ICodeDomProvider codeDomProvider)
			: this(
				new ServiceReferenceProxyGenerator(codeDomProvider),
				new ServiceReferenceMapGenerator())
		{
		}
		
		public ServiceReferenceFileGenerator(
			IServiceReferenceProxyGenerator proxyGenerator,
			IServiceReferenceMapGenerator mapGenerator)
		{
			this.proxyGenerator = proxyGenerator;
			this.mapGenerator = mapGenerator;
		}
		
		public string ServiceReferenceNamespace {
			get { return proxyGenerator.ServiceReferenceNamespace; }
			set { proxyGenerator.ServiceReferenceNamespace = value; }
		}
		
		public void GenerateProxyFile(MetadataSet metadata, string proxyFileName)
		{
			proxyGenerator.GenerateProxyFile(metadata, proxyFileName);
		}
		
		public void GenerateServiceReferenceMapFile(ServiceReferenceMapFile mapFile)
		{
			mapGenerator.GenerateServiceReferenceMapFile(mapFile);
		}
	}
}
