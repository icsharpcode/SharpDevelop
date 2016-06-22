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
using System.Reflection;
using System.Text;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Parser;

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
		
		internal ReferenceProjectItem(IProject project, IProjectItemBackendStore buildItem)
			: base(project, buildItem)
		{
		}
		
		[ReadOnly(true)]
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
		public virtual bool SpecificVersion {
			get {
				return this.Include.Contains(",");
			}
		}
		
		[DefaultValue(false)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.EmbedInteropTypes}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.EmbedInteropTypes.Description}")]
		public bool EmbedInteropTypes {
			get {
				return GetEvaluatedMetadata("EmbedInteropTypes", false);
			}
			set {
				SetEvaluatedMetadata("EmbedInteropTypes", value);
				ReFilterProperties();
			}
		}
		
		internal const string CopyLocalMetadataName = "Private";
		
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy}",
		                   Description = "${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.LocalCopy.Description}")]
		public bool CopyLocal {
			get {
				return GetEvaluatedMetadata(CopyLocalMetadataName, defaultCopyLocalValue ?? true);
			}
			set {
				SetEvaluatedMetadata(CopyLocalMetadataName, value);
			}
		}
		
		bool? defaultCopyLocalValue;
		
		[Browsable(false)]
		public bool? DefaultCopyLocalValue {
			get { return defaultCopyLocalValue; }
			set {
				defaultCopyLocalValue = value;
				ReFilterProperties();
			}
		}
		
		DomAssemblyName assemblyName;
		
		/// <summary>
		/// Gets the assembly name.
		/// </summary>
		[Browsable(false)]
		public DomAssemblyName AssemblyName {
			get { return assemblyName ?? new DomAssemblyName(Include); }
			internal set { assemblyName = value; }
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Name}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Name.Description}")]
		public string Name {
			get {
				return Include;
			}
		}
		
		[Browsable(false)]
		public virtual string ShortName {
			get {
				return this.AssemblyName.ShortName;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Version}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Version.Description}")]
		public virtual Version Version {
			get {
				return this.AssemblyName.Version;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Culture}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.Culture.Description}")]
		public virtual string Culture {
			get {
				return this.AssemblyName.Culture;
			}
		}
		
		[ReadOnly(true)]
		[LocalizedProperty("${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.PublicKeyToken}",
		                   Description="${res:ICSharpCode.SharpDevelop.Internal.Project.ProjectReference.PublicKeyToken.Description}")]
		public virtual string PublicKeyToken {
			get {
				return this.AssemblyName.PublicKeyToken;
			}
		}
		
		string redist;
		
		/// <summary>
		/// The name of the package in which the assembly is redistributed to the user.
		/// "Microsoft-Windows-CLRCoreComp" = .NET 2.0
		/// "Microsoft-Windows-CLRCoreComp-v3.5" = .NET 3.5
		/// </summary>
		[Browsable(false)]
		public string Redist {
			get { return redist; }
			set { redist = value; }
		}
		
		FileName fullPath;
		
		[ReadOnly(true)]
		[Browsable(true)]
		public override FileName FileName {
			get {
				if (fullPath != null) {
					return fullPath;
				}
				
				if (Project != null) {
					DirectoryName projectDir = Project.Directory;
					string hintPath = HintPath;
					try {
						if (hintPath != null && hintPath.Length > 0) {
							return projectDir.CombineFile(hintPath);
						}
						FileName name = projectDir.CombineFile(Include);
						if (File.Exists(name)) {
							return name;
						}
						name = projectDir.CombineFile(Include + ".dll");
						if (File.Exists(name)) {
							return name;
						}
						name = projectDir.CombineFile(Include + ".exe");
						if (File.Exists(name)) {
							return name;
						}
					} catch {} // ignore errors when path is invalid
				}
				return FileName.Create(Include);
			}
			set {
				fullPath = value;
			}
		}
		
		protected override void FilterProperties(PropertyDescriptorCollection globalizedProps)
		{
			base.FilterProperties(globalizedProps);
			PropertyDescriptor copyLocalPD = globalizedProps["CopyLocal"];
			globalizedProps.Remove(copyLocalPD);
			if (defaultCopyLocalValue != null && !EmbedInteropTypes) {
				globalizedProps.Add(new ReplaceDefaultValueDescriptor(copyLocalPD, defaultCopyLocalValue.Value));
			} else {
				globalizedProps.Add(new DummyValueDescriptor(copyLocalPD));
			}
			
			if (string.IsNullOrEmpty(HintPath))
				globalizedProps.Remove(globalizedProps["HintPath"]);
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
		
		sealed class DummyValueDescriptor : PropertyDescriptor
		{
			PropertyDescriptor baseDescriptor;
			
			public override bool ShouldSerializeValue(object component)
			{
				return false;
			}
			
			public override void ResetValue(object component)
			{
			}
			
			public DummyValueDescriptor(PropertyDescriptor baseDescriptor)
				: base(baseDescriptor)
			{
				this.baseDescriptor = baseDescriptor;
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
				get { return true; }
			}
			
			public override bool CanResetValue(object component)
			{
				return false;
			}
			
			public override object GetValue(object component)
			{
				return null;
			}
			
			public override void SetValue(object component, object value)
			{
			}
			
			public override Type PropertyType {
				get { return baseDescriptor.PropertyType; }
			}
		}
	}
}
