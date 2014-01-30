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
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.PackageManagement.EnvDTE
{
	public class Reference : MarshalByRefObject, global::EnvDTE.Reference
	{
		ReferenceProjectItem referenceProjectItem;
		Project project;
		
		public Reference(Project project, ReferenceProjectItem referenceProjectItem)
		{
			this.project = project;
			this.referenceProjectItem = referenceProjectItem;
		}
		
		public string Name {
			get { return referenceProjectItem.Name; }
		}
		
		public string Path {
			get { return referenceProjectItem.FileName; }
		}
		
		public void Remove()
		{
			project.RemoveReference(referenceProjectItem);
			project.Save();
		}
		
		public global::EnvDTE.Project SourceProject {
			get {
				var projectReference = referenceProjectItem as ProjectReferenceProjectItem;
				if (projectReference != null) {
					return new Project(projectReference.ReferencedProject as MSBuildBasedProject);
				}
				return null;
			}
		}
		
		public string Identity {
			get { return referenceProjectItem.ShortName; }
		}
		
		public string PublicKeyToken {
			get {
				if (referenceProjectItem.PublicKeyToken != "null") {
					return referenceProjectItem.PublicKeyToken;
				}
				return String.Empty;
			}
		}
		
		public bool StrongName {
			get { return HasVersion() && HasPublicKeyToken(); }
		}
		
		bool HasVersion()
		{
			return referenceProjectItem.Version != null;
		}
		
		bool HasPublicKeyToken()
		{
			return !String.IsNullOrEmpty(PublicKeyToken);
		}
	}
}
