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
		
		public ServiceReferenceFileGenerator()
			: this(
				new ServiceReferenceProxyGenerator(),
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
		
		public ServiceReferenceGeneratorOptions Options {
			get { return proxyGenerator.Options; }
		}
		
		public void GenerateProxyFile()
		{
			proxyGenerator.GenerateProxyFile();
		}
		
		public void GenerateServiceReferenceMapFile(ServiceReferenceMapFile mapFile)
		{
			mapGenerator.GenerateServiceReferenceMapFile(mapFile);
		}
	}
}
