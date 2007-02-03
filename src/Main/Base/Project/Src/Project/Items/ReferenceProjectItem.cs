// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public class ReferenceProjectItem : ProjectItem
	{
		protected ReferenceProjectItem(IProject project, ItemType itemType)
			: base(project, itemType)
		{
		}
		
		public ReferenceProjectItem(IProject project)
			: base(project, ItemType.Reference)
		{
		}
		
		public ReferenceProjectItem(IProject project, string include)
			: base(project, ItemType.Reference, include)
		{
		}
		
		internal ReferenceProjectItem(IProject project, Microsoft.Build.BuildEngine.BuildItem buildItem)
			: base(project, buildItem)
		{
		}
		
		[Browsable(false)]
		public string HintPath {
			get {
				return GetEvaluatedMetadata("HintPath");
			}
			set {
				SetEvaluatedMetadata("HintPath", value);
			}
		}
		
		[DefaultValue("global")]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Aliases}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Aliases.Description}")]
		public string Aliases {
			get {
				return GetEvaluatedMetadata("Aliases", "global");
			}
			set {
				SetEvaluatedMetadata("Aliases", value);
			}
		}
		
		[DefaultValue(false)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.SpecificVersion}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.SpecificVersion.Description}")]
		public bool SpecificVersion {
			get {
				return GetEvaluatedMetadata("SpecificVersion", false);
			}
			set {
				SetEvaluatedMetadata("SpecificVersion", value);
			}
		}
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy.Description}")]
		public bool Private {
			get {
				return GetEvaluatedMetadata("Private", !IsGacReference);
			}
			set {
				SetEvaluatedMetadata("Private", value);
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Name}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Name.Description}")]
		public string Name {
			get {
				AssemblyName assemblyName = GetAssemblyName(Include);
				if (assemblyName != null) {
					return assemblyName.Name;
				}
				return Include;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Version}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Version.Description}")]
		public Version Version {
			get {
				AssemblyName assemblyName = GetAssemblyName(Include);
				if (assemblyName != null) {
					return assemblyName.Version;
				}
				return null;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Culture}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Culture.Description}")]
		public string Culture {
			get {
				AssemblyName assemblyName = GetAssemblyName(Include);
				if (assemblyName != null && assemblyName.CultureInfo != null) {
					return assemblyName.CultureInfo.Name;
				}
				return null;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.PublicKeyToken}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.PublicKeyToken.Description}")]
		public string PublicKeyToken {
			get {
				AssemblyName assemblyName = GetAssemblyName(Include);
				if (assemblyName != null) {
					byte[] bytes = assemblyName.GetPublicKeyToken();
					if (bytes != null) {
						StringBuilder token = new StringBuilder();
						foreach (byte b in bytes) {
							token.Append(b.ToString("x2"));
						}
						return token.ToString();
					}
				}
				return null;
			}
		}
		
		[ReadOnly(true)]
		public override string FileName {
			get {
				if (Project != null) {
					string projectDir = Project.Directory;
					string hintPath = HintPath;
					try {
						if (hintPath != null && hintPath.Length > 0) {
							return FileUtility.GetAbsolutePath(projectDir, hintPath);
						}
						string name = FileUtility.GetAbsolutePath(projectDir, Include);
						if (File.Exists(name)) {
							return name;
						}
						if (File.Exists(name + ".dll")) {
							return name + ".dll";
						}
						if (File.Exists(name + ".exe")) {
							return name + ".exe";
						}
					} catch {} // ignore errors when path is invalid
				}
				return Include;
			}
			set {
				// Set by file name is unsupported by references. (otherwise GAC references might have strange renaming effects ...)
			}
		}
		
		[Browsable(false)]
		public bool IsGacReference {
			get {
				return !Path.IsPathRooted(this.FileName);
			}
		}
		
		AssemblyName GetAssemblyName(string include)
		{
			try {
				if (this.ItemType == ItemType.Reference) {
					return new AssemblyName(include);
				}
			} catch (ArgumentException) { }
			
			return null;
		}
		
		protected override void FilterProperties(PropertyDescriptorCollection globalizedProps)
		{
			base.FilterProperties(globalizedProps);
			PropertyDescriptor privatePD = globalizedProps["Private"];
			globalizedProps.Remove(privatePD);
			globalizedProps.Add(new ReplaceDefaultValueDescriptor(privatePD, !IsGacReference));
		}
		
		sealed class ReplaceDefaultValueDescriptor : PropertyDescriptor
		{
			PropertyDescriptor baseDescriptor;
			bool newDefaultValue;
			
			public override bool ShouldSerializeValue(object component)
			{
				return (bool)GetValue(component) != newDefaultValue;
			}
			
			public override void ResetValue(object component)
			{
				SetValue(component, newDefaultValue);
			}
			
			public ReplaceDefaultValueDescriptor(PropertyDescriptor baseDescriptor, bool newDefaultValue)
				: base(baseDescriptor)
			{
				this.baseDescriptor = baseDescriptor;
				this.newDefaultValue = newDefaultValue;
			}
			
			public override string DisplayName {
				get { return baseDescriptor.DisplayName; }
			}
			
			public override string Description {
				get { return baseDescriptor.Description; }
			}
			
			public override Type ComponentType {
				get { return baseDescriptor.ComponentType; }
			}
			
			public override bool IsReadOnly {
				get { return baseDescriptor.IsReadOnly; }
			}
			
			public override bool CanResetValue(object component)
			{
				return baseDescriptor.CanResetValue(component);
			}
			
			public override object GetValue(object component)
			{
				return baseDescriptor.GetValue(component);
			}
			
			public override void SetValue(object component, object value)
			{
				baseDescriptor.SetValue(component, value);
			}
			
			public override Type PropertyType {
				get { return baseDescriptor.PropertyType; }
			}
		}
	}
}
