// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
