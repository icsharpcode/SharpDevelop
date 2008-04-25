// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;

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
		
		internal ComReferenceProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem buildItem)
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
		
		[ReadOnly(true)]
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
		public override string FileName {
			get {
				try {
					if (Project != null && Project.OutputAssemblyFullPath != null) {
						string outputFolder = Path.GetDirectoryName(Project.OutputAssemblyFullPath);
						string interopFileName = Path.Combine(outputFolder, String.Concat("Interop.", Include, ".dll"));
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
							interopFileName = Path.Combine(outputFolder, String.Concat("Interop.", Include, ".dll"));
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
				return Include;
			}
			set {
			}
		}
		
		static string GetActiveXInteropFileName(string outputFolder, string include)
		{
			if (include.ToLowerInvariant().StartsWith("ax")) {
				return Path.Combine(outputFolder, String.Concat("AxInterop.", include.Substring(2), ".dll"));
			}
			return null;
		}
	}
}
