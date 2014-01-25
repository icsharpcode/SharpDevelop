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
using NuGet;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakePackage : IPackage
	{
		public FakePackage()
		{
		}
		
		public bool IsAbsoluteLatestVersion
		{
			get;
			set;
		}
		
		public bool IsLatestVersion
		{
			get;
			set;
		}
		
		public bool Listed
		{
			get;
			set;
		}
		
		public Nullable<DateTimeOffset> Published
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<IPackageAssemblyReference> AssemblyReferences
		{
			get;
			set;
		}
		
		public string Id
		{
			get;
			set;
		}
		
		public SemanticVersion Version
		{
			get;
			set;
		}
		
		public string Title
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<string> Authors
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<string> Owners
		{
			get;
			set;
		}
		
		public Uri IconUrl
		{
			get;
			set;
		}
		
		public Uri LicenseUrl
		{
			get;
			set;
		}
		
		public Uri ProjectUrl
		{
			get;
			set;
		}
		
		public bool RequireLicenseAcceptance
		{
			get;
			set;
		}
		
		public string Description
		{
			get;
			set;
		}
		
		public string Summary
		{
			get;
			set;
		}
		
		public string ReleaseNotes
		{
			get;
			set;
		}
		
		public string Language
		{
			get;
			set;
		}
		
		public string Tags
		{
			get;
			set;
		}
		
		public string Copyright
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<PackageDependencySet> DependencySets
		{
			get;
			set;
		}
		
		public Uri ReportAbuseUrl
		{
			get;
			set;
		}
		
		public int DownloadCount
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IEnumerable<IPackageFile> GetFiles()
		{
			return null;
		}
		
		public System.Collections.Generic.IEnumerable<System.Runtime.Versioning.FrameworkName> GetSupportedFrameworks()
		{
			return null;
		}
		
		public System.IO.Stream GetStream()
		{
			return null;
		}
		
		public System.Collections.Generic.ICollection<PackageReferenceSet> PackageAssemblyReferences {
			get {
				throw new NotImplementedException();
			}
		}
		
		public Version MinClientVersion {
			get {
				throw new NotImplementedException();
			}
		}
		
		public override string ToString()
		{
			return string.Format("[FakePackage Id={0}, Version={1}]", Id, Version);
		}
	}
}
