// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;

using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakePackageAssemblyReference : IPackageAssemblyReference
	{
		public FrameworkName TargetFramework {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string Name { get; set; }
		
		public string Path {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Stream GetStream()
		{
			throw new NotImplementedException();
		}
		
		public IEnumerable<FrameworkName> SupportedFrameworks {
			get {
				throw new NotImplementedException();
			}
		}
		
		public string EffectivePath {
			get {
				throw new NotImplementedException();
			}
		}
	}
}
