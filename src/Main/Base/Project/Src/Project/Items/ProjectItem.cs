using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Xml;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
	public enum ItemType {
		Unknown,
		
		// ReferenceProjectItem
		Reference,
		ProjectReference,
		COMReference,
		
		WebReferenceUrl,
		
		// FileProjectItem
		Compile,
		EmbeddedResource,
		None,
		Content,
		Folder,
		WebReferences,
		
		BootstrapperFile
	}
	
	public abstract class ProjectItem : IDisposable
	{
		string        include;
		PropertyGroup properties = new PropertyGroup();
		IProject      project    = null;
		
		[Browsable(false)]
		public abstract ItemType ItemType {
			get;
		}
		
		public IProject Project {
			get {
				return project;
			}
			set {
				project = value;
			}
		}
		
		[Browsable(false)]
		public string Include {
			get {
				return include;
			}
			set {
				include = value;
			}
		}
		
		[Browsable(false)]
		public PropertyGroup Properties {
			get {
				return properties;
			}
		}
		
		public virtual string FileName {
			get {
				return Path.Combine(project.Directory, Include);
			}
			set {
				Include = FileUtility.GetRelativePath(project.Directory, value);
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
		
		static ProjectItem ReadItem(XmlTextReader reader, IProject project, string itemType)
		{
			ProjectItem newItem = ProjectItemFactory.CreateProjectItem(project, itemType);
			newItem.Include  = reader.GetAttribute("Include");
			if (!reader.IsEmptyElement) {
				PropertyGroup.ReadProperties(reader, newItem.Properties, itemType);
			}
			return newItem;
		}
		
		public virtual string Tag {
			get {
				return ItemType.ToString();
			}
		}
		
		internal void WriteItem(XmlTextWriter writer)
		{
			writer.WriteStartElement(Tag);
			writer.WriteAttributeString("Include", Include);
			Properties.WriteProperties(writer);
			writer.WriteEndElement();
		}
		
		internal static void ReadItemGroup(XmlTextReader reader, IProject project, List<ProjectItem> items)
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
		
		internal static void WriteItemGroup(XmlTextWriter writer, List<ProjectItem> items)
		{
			writer.WriteStartElement("ItemGroup");
			foreach (ProjectItem item in items) {
				item.WriteItem(writer);
			}
			writer.WriteEndElement();
		}
	}
}
