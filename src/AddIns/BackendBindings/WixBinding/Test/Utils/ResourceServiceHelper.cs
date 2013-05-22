// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace WixBinding.Tests.Utils
{
	public static class ResourceServiceHelper
	{
		public static void InitializeForUnitTests()
		{
			var propertyService = SD.Services.GetService(typeof(IPropertyService)) as IPropertyService;
			SD.Services.AddService(typeof(IResourceService), new ResourceServiceImpl("", propertyService));
		}
	}
}
