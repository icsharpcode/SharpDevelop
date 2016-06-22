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
using System.ComponentModel;
using System.IO;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public sealed class ComReferenceProjectItem : ReferenceProjectItem
	{
		
		public ComReferenceProjectItem(IProject project, TypeLibrary library)
			: base(project, ItemType.COMReference)
		{
			this.Include = library.Name;
			
			this.Guid         = library.Guid;
			this.VersionMajor = library.VersionMajor;
			this.VersionMinor = library.VersionMinor;
			this.Lcid         = library.Lcid;
			this.WrapperTool  = library.WrapperTool;
			this.Isolated     = library.Isolated;
			this.DefaultCopyLocalValue = true;
		}
		
		internal ComReferenceProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
			this.DefaultCopyLocalValue = true;
		}
		
		[ReadOnly(true)]
		public string Guid {
			get {
				return GetEvaluatedMetadata("Guid");
			}
			set {
				SetEvaluatedMetadata("Guid", value);
			}
		}
		
		[ReadOnly(true)]
		public int VersionMajor {
			get {
				return GetEvaluatedMetadata("VersionMajor", 1);
			}
			set {
				SetEvaluatedMetadata("VersionMajor", value);
			}
		}
		
		[ReadOnly(true)]
		public int VersionMinor {
			get {
				return GetEvaluatedMetadata("VersionMinor", 0);
			}
			set {
				SetEvaluatedMetadata("VersionMinor", value);
			}
		}
		
		[ReadOnly(true)]
		public string Lcid {
			get {
				return GetEvaluatedMetadata("Lcid");
			}
			set {
				SetEvaluatedMetadata("Lcid", value);
			}
		}
		
		[ReadOnly(true)]
		public string WrapperTool {
			get {
				return GetEvaluatedMetadata("WrapperTool");
			}
			set {
				SetEvaluatedMetadata("WrapperTool", value);
			}
		}
		
		[DefaultValue(false)]
		public bool Isolated {
			get {
				return GetEvaluatedMetadata("Isolated", false);
			}
			set {
				SetEvaluatedMetadata("Isolated", value);
			}
		}
		
		/// <summary>
		/// Gets the file name of the COM interop assembly.
		/// </summary>
		public override FileName FileName {
			get {
				try {
					if (Project != null && Project.OutputAssemblyFullPath != null) {
						DirectoryName outputFolder = Project.OutputAssemblyFullPath.GetParentDirectory();
						FileName interopFileName = outputFolder.CombineFile("Interop." + Include + ".dll");
						if (File.Exists(interopFileName)) {
							return interopFileName;
						}
						// Look for ActiveX interop.
						interopFileName = GetActiveXInteropFileName(outputFolder, Include);
						if (File.Exists(interopFileName)) {
							return interopFileName;
						}
						
						// look in obj\Debug:
						if (Project is CompilableProject) {
							outputFolder = (Project as CompilableProject).IntermediateOutputFullPath;
							interopFileName = outputFolder.CombineFile("Interop." + Include + ".dll");
							if (File.Exists(interopFileName)) {
								return interopFileName;
							}
						}
						// Look for ActiveX interop.
						interopFileName = GetActiveXInteropFileName(outputFolder, Include);
						if (File.Exists(interopFileName)) {
							return interopFileName;
						}
					}
				}
				catch (Exception) { }
				return FileName.Create(Include);
			}
			set {
			}
		}
		
		static FileName GetActiveXInteropFileName(DirectoryName outputFolder, string include)
		{
			if (include.StartsWith("ax", StringComparison.OrdinalIgnoreCase)) {
				return outputFolder.CombineFile(String.Concat("AxInterop.", include.Substring(2), ".dll"));
			}
			return null;
		}
	}
}
