// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ICSharpCode.WixBinding
{
	public class WixDirectoryElement : WixDirectoryElementBase
	{
		public const string DirectoryElementName = "Directory";
		public const string RootDirectoryId = "TARGETDIR";
		
		public WixDirectoryElement(WixDocument document) 
			: base(DirectoryElementName, document)
		{
		}
		
		/// <summary>
		/// Determines whether the specified element name refers to a Directory element.
		/// </summary>
		public static bool IsDirectoryElement(string name)
		{
			return name == DirectoryElementName;
		}
		
		/// <summary>
		/// Returns the last directory specified in the path 
		/// </summary>
		public static string GetLastDirectoryName(string path)
		{
			int index = path.LastIndexOf(Path.DirectorySeparatorChar);
			return path.Substring(index + 1);
		}
		
		/// <summary>
		/// Creates the directory element and sets its Id and SourceName.
		/// </summary>
		public static WixDirectoryElement CreateRootDirectory(WixDocument document)
		{
			WixDirectoryElement rootDirectory = new WixDirectoryElement(document);
			rootDirectory.Id = RootDirectoryId;
			rootDirectory.SourceName = "SourceDir";
			return rootDirectory;
		}
			
		/// <summary>
		/// Adds a new component element to this directory element.
		/// </summary>
		public WixComponentElement AddComponent()
		{
			return AddComponent(String.Empty);
		}
		
		/// <summary>
		/// Adds a new component element to this directory element.
		/// </summary>
		public WixComponentElement AddComponent(string id)
		{
			WixComponentElement componentElement = new WixComponentElement((WixDocument)OwnerDocument);
			componentElement.GenerateNewGuid();
			componentElement.Id = id;
			return (WixComponentElement)AppendChild(componentElement);			
		}
		
		public string SourceName {
			get {
				return GetAttribute("SourceName");
			}
			set {
				SetAttribute("SourceName", value);
			}
		}
		
		public string LongName {
			get {
				return GetAttribute("LongName");
			}
			set {
				SetAttribute("LongName", value);
			}
		}
		
		public string ShortName {
			get {
				return GetAttribute("Name");
			}
			set {
				SetAttribute("Name", value);
			}
		}
			
		/// <summary>
		/// Gets the directory name.
		/// </summary>
		/// <returns>
		/// Returns the long name if defined, otherwise the name. If the directory Id
		/// is a special case (e.g. "ProgramFilesFolder") it returns this id slightly 
		/// modified (e.g. "Program Files").
		/// </returns>
		public string DirectoryName {
			get {
				string name = GetSystemDirectory(Id);
				if (name != null) {
					return name;
				}
				name = LongName;
				if (name.Length > 0) {
					return name;
				}
				return ShortName;
			}
		}
		
		/// <summary>
		/// Checks to see if a File element exists with the specified
		/// short filename.
		/// </summary>
		public bool ShortFileNameExists(string fileName)
		{
			string xpath = String.Concat("w:Component/w:File[@Name='", XmlEncoder.Encode(fileName, '\''), "']");
			XmlNodeList nodes = SelectNodes(xpath, new WixNamespaceManager(OwnerDocument.NameTable));
			return nodes.Count > 0;
		}
		
		/// <summary>
		///	Returns whether the specified name maps to a system directory.
		/// </summary>
		public static string GetSystemDirectory(string id)
		{
			switch (id) {
				case "ProgramFilesFolder":
				case "AdminToolsFolder":
				case "AppDataFolder":
				case "CommonAppDataFolder":
				case "CommonFiles64Folder":
				case "CommonFilesFolder":
				case "DesktopFolder":
				case "FavoritesFolder":
				case "FontsFolder":
				case "LocalAppDataFolder":
				case "MyPicturesFolder":
				case "PersonalFolder":
				case "ProgramFiles64Folder":
				case "ProgramMenuFolder":
				case "SendToFolder":
				case "StartMenuFolder":
				case "StartupFolder":
				case "System16Folder":
				case "System64Folder":
				case "SystemFolder":
				case "TempFolder":
				case "TemplateFolder":
				case "WindowsVolume":
					return StringParser.Parse(String.Concat("${res:ICSharpCode.WixBinding.WixDirectoryElement.", id, "}"));
				case "WindowsFolder":
					return "Windows";
			}
			return null;
		}
	}
}
