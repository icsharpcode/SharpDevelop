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

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// A service provider that provides the pad services (as declared by the 'serviceInterface' attribute in the &lt;Pad&gt; codon).
	/// </summary>
	/// <remarks>
	/// We register this as a fallback provider for the SD.Services container instead of directly registering the pads (using ServiceCreationCallback)
	/// so that the thread-safety check in the .PadContent property accessor runs on every access to the service.
	/// </remarks>
	class PadServiceProvider : IServiceProvider
	{
		readonly List<PadDescriptor> pads = new List<PadDescriptor>();
		
		public PadServiceProvider(IEnumerable<PadDescriptor> descriptors)
		{
			foreach (var descriptor in descriptors) {
				if (descriptor.ServiceInterface != null) {
					pads.Add(descriptor);
				}
			}
		}
		
		public object GetService(Type serviceType)
		{
			foreach (var pad in pads) {
				if (serviceType == pad.ServiceInterface)
					return pad.PadContent;
			}
			return null;
		}
	}
}
