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
			set { proxyGenerator.Options = value; }
		}
		
		public void GenerateProxyFile()
		{
			proxyGenerator.GenerateProxyFile();
		}
		
		public void GenerateServiceReferenceMapFile(ServiceReferenceMapFile mapFile)
		{
			mapGenerator.GenerateServiceReferenceMapFile(mapFile);
		}
		
		public event EventHandler<GeneratorCompleteEventArgs> Complete {
			add { proxyGenerator.Complete += value; }
			remove { proxyGenerator.Complete += value; }
		}
	}
}
