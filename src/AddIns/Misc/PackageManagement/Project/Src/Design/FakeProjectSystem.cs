// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;

using NuGet;

namespace ICSharpCode.PackageManagement.Design
{
	public class FakeProjectSystem : FakeFileSystem, IProjectSystem
	{
		public FrameworkName TargetFramework {
			get { return new FrameworkName(".NETFramework, Version=v4.0"); }
		}
		
		public string ProjectName {
			get { return String.Empty; }
		}
		
		public dynamic GetPropertyValue(string propertyName)
		{
			throw new NotImplementedException();
		}
		
		public void AddReference(string referencePath, Stream stream)
		{
			throw new NotImplementedException();
		}
		
		public bool ReferenceExists(string name)
		{
			throw new NotImplementedException();
		}
		
		public void RemoveReference(string name)
		{
			throw new NotImplementedException();
		}
		
		public bool IsSupportedFile(string path)
		{
			throw new NotImplementedException();
		}
		
		public void AddFrameworkReference(string name)
		{
			throw new NotImplementedException();
		}
		
		public string ResolvePath(string path)
		{
			throw new NotImplementedException();
		}
		
		public bool IsBindingRedirectSupported { get; set; }
	}
}
