// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Globalization;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum ItemType {
		Unknown,
		
		// ReferenceProjectItem
		Reference,
		ProjectReference,
		COMReference,
		
		Import,
		
		WebReferenceUrl,
		
		// FileProjectItem
		Compile,
		EmbeddedResource,
		Resource,
		None,
		Content,
		Folder,
		WebReferences,
		
		ApplicationDefinition,
		Page,
		
		BootstrapperFile
	}
	
	public abstract class ProjectItem : LocalizedObject, IDisposable, ICloneable
	{
		string        include;
		PropertyGroup properties = new PropertyGroup();
		IProject      project    = null;
		
		[Browsable(false)]
		public abstract ItemType ItemType {
			get;
		}
		
		[Browsable(false)]
		public IProject Project {
			get {
				return project;
			}
			set {
				project = value;
				fileNameCache = null;
			}
		}
		
		[Browsable(false)]
		public string Include {
			get {
				return include;
			}
			set {
				include = value;
				fileNameCache = null;
			}
		}
		
		public virtual void CopyExtraPropertiesTo(ProjectItem item)
		{
			string newInclude = item.Include;
			item.Properties.Merge(this.Properties);
			item.Include = newInclude;
		}
		
		public abstract ProjectItem Clone();
		
		object ICloneable.Clone()
		{
			return this.Clone();
		}
		
		[Browsable(false)]
		public PropertyGroup Properties {
			get {
				return properties;
			}
		}
		
		string fileNameCache;
		
		[Browsable(false)]
		public virtual string FileName {
			get {
				if (project == null)
					return Include;
				if (fileNameCache == null)
					fileNameCache = Path.Combine(project.Directory, include);
				return fileNameCache;
			}
			set {
				fileNameCache = null;
				Include = FileUtility.GetRelativePath(project.Directory, value);
			}
		}
		
		[Browsable(false)]
		public virtual string Tag {
			get {
				return ItemType.ToString();
			}
		}
		
		public ProjectItem(IProject project)
		{
			this.project = project;
		}
		
		#region System.IDisposable interface implementation
		public virtual void Dispose()
		{
		}
		#endregion
		public override string ToString()
		{
			return String.Format("[ProjectItem: ItemType={0}, Include={1}, Properties={2}]",
			                     ItemType,
			                     Include,
			                     Properties);
		}
		
		public static string MSBuildEscape(string text)
		{
			StringBuilder b = null;
			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				if (c == '%') {
					if (b == null) b = new StringBuilder(text, 0, i, text.Length + 6);
					b.Append("%25");
				} else if (c == ';') {
					if (b == null) b = new StringBuilder(text, 0, i, text.Length + 6);
					b.Append("%3b");
				} else {
					if (b != null) {
						b.Append(c);
					}
				}
			}
			if (b != null)
				return b.ToString();
			else
				return text;
		}
		
		public static string MSBuildUnescape(string text)
		{
			StringBuilder b = null;
			for (int i = 0; i < text.Length; i++) {
				char c = text[i];
				if (c == '%' && i + 2 < text.Length) {
					if (b == null) b = new StringBuilder(text, 0, i, text.Length + 6);
					string a = text[i + 1].ToString() + text[i + 2].ToString();
					int num;
					if (int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out num)) {
						b.Append((char)num);
						i += 2;
					} else {
						b.Append('%');
					}
				} else {
					if (b != null) {
						b.Append(c);
					}
				}
			}
			if (b != null)
				return b.ToString();
			else
				return text;
		}
		
		public static ProjectItem ReadItem(XmlReader reader, IProject project, string itemType)
		{
			ProjectItem newItem = project != null ? project.CreateProjectItem(itemType) : ProjectItemFactory.CreateProjectItem(project, itemType);
			newItem.Include = MSBuildUnescape(reader.GetAttribute("Include"));
			if (!reader.IsEmptyElement) {
				PropertyGroup.ReadProperties(reader, newItem.Properties, itemType);
			}
			return newItem;
		}
		
		
		internal void WriteItem(XmlWriter writer)
		{
			writer.WriteStartElement(Tag);
			writer.WriteAttributeString("Include", MSBuildEscape(Include));
			this.Properties.WriteProperties(writer);
			writer.WriteEndElement();
		}
		
		internal static void ReadItemGroup(XmlReader reader, IProject project, List<ProjectItem> items)
		{
			if (reader.IsEmptyElement) {
				return;
			}
			while (reader.Read()) {
				switch (reader.NodeType) {
					case XmlNodeType.EndElement:
						if (reader.LocalName == "ItemGroup") {
							return;
						}
						break;
					case XmlNodeType.Element:
						items.Add(ReadItem(reader, project, reader.LocalName));
						break;
				}
			}
		}
		
		internal static void WriteItemGroup(XmlWriter writer, List<ProjectItem> items)
		{
			writer.WriteStartElement("ItemGroup");
			foreach (ProjectItem item in items) {
				item.WriteItem(writer);
			}
			writer.WriteEndElement();
		}
		
		public override void InformSetValue(LocalizedPropertyDescriptor localizedPropertyDescriptor, object component, object value)
		{
			Project.Save();
		}
	}
}
