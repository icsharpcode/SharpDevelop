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
using ICSharpCode.Core;
using ICSharpCode.NRefactory.Utils;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Project
{
	/// <summary>
	/// Represents the registration of a project binding with the project service.
	/// </summary>
	public class ProjectBindingDescriptor
	{
		readonly AddIn addIn;
		readonly string className;
		readonly string language;
		readonly Guid typeGuid;
		readonly string projectFileExtension;
		readonly string[] codeFileExtensions;
		IProjectBinding binding;
		
		public IProjectBinding Binding {
			get {
				if (binding != null)
					return binding;
				if (addIn == null)
					return null;
				return LazyInit.GetOrSet(ref binding, (IProjectBinding)addIn.CreateObject(className));
			}
		}
		
		public string ProjectFileExtension {
			get {
				return projectFileExtension;
			}
		}
		
		public Guid TypeGuid {
			get {
				return typeGuid;
			}
		}
		
		public string Language {
			get {
				return language;
			}
		}
		
		public string[] CodeFileExtensions {
			get {
				return codeFileExtensions;
			}
		}
		
		public ProjectBindingDescriptor(IProjectBinding binding, string language, string projectFileExtension, Guid typeGuid, string[] codeFileExtensions)
		{
			if (binding == null)
				throw new ArgumentNullException("binding");
			if (language == null)
				throw new ArgumentNullException("language");
			if (projectFileExtension == null)
				throw new ArgumentNullException("projectFileExtension");
			if (codeFileExtensions == null)
				throw new ArgumentNullException("codeFileExtensions");
			this.binding = binding;
			this.projectFileExtension = projectFileExtension;
			this.typeGuid = typeGuid;
			this.language = language;
			this.codeFileExtensions = codeFileExtensions;
		}
		
		public ProjectBindingDescriptor(Codon codon)
		{
			if (codon == null)
				throw new ArgumentNullException("codon");
			this.addIn = codon.AddIn;
			this.language = codon.Id;
			this.className = codon.Properties["class"];
			this.projectFileExtension = codon.Properties["projectfileextension"];
			if (string.IsNullOrEmpty(codon.Properties["supportedextensions"]))
				this.codeFileExtensions = new string[0];
			else
				this.codeFileExtensions = codon.Properties["supportedextensions"].ToLowerInvariant().Split(';');
			if (!string.IsNullOrEmpty(codon.Properties["guid"]))
				this.typeGuid = Guid.Parse(codon.Properties["guid"]);
		}
	}
}
