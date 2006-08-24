// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
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
		///	Returns whether the specified name maps to a system directory.
		/// </summary>
		public static string GetSystemDirectory(string id)
		{
			switch (id) {
				case "ProgramFilesFolder":
					return "Program Files";
				case "AdminToolsFolder":
					return "Admin Tools";
				case "AppDataFolder":
					return "App Data";
				case "CommonAppDataFolder":
					return "Common App Data";		
				case "CommonFiles64Folder":
					return "Common Files 64";
				case "CommonFilesFolder":
					return "Common Files";
				case "DesktopFolder":
					return "Desktop";
				case "FavoritesFolder":
					return "Favorites";
				case "FontsFolder":
					return "Fonts";
				case "LocalAppDataFolder":
					return "Local App Data";
				case "MyPicturesFolder":
					return "My Pictures";		
				case "PersonalFolder":
					return "Personal";
				case "ProgramFiles64Folder":
					return "Program Files 64";
				case "ProgramMenuFolder":
					return "Program Menu";
				case "SendToFolder":
					return "Send To";
				case "StartMenuFolder":
					return "Start Menu";
				case "StartupFolder":
					return "Startup";
				case "System16Folder":
					return "System 16";
				case "System64Folder":
					return "System 64";
				case "SystemFolder":
					return "System";
				case "TempFolder":
					return "Temp";
				case "TemplateFolder":
					return "Templates";
				case "WindowsFolder":
					return "Windows";
				case "WindowsVolume":
					return "Windows Volume";
			}
			return null;
		}
	}
}
