// <file>
//     <copyright see="prj:///doc/copyright.txt">2002-2005 AlphaSierraPapa</copyright>
//     <license see="prj:///doc/license.txt">GNU General Public License</license>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

namespace ICSharpCode.Core
{
	public enum AddInAction
	{
		Enable,
		Disable,
		Install,
		Uninstall,
		Update
	}
	
	public static class AddInManager
	{
		static string configurationFileName;
		
		public static string ConfigurationFileName {
			get {
				return configurationFileName;
			}
			set {
				configurationFileName = value;
			}
		}
		
		public static void AddExternalAddIns(IList<AddIn> addIns)
		{
			List<string> addInFiles = new List<string>();
			List<string> disabled = new List<string>();
			LoadAddInConfiguration(addInFiles, disabled);
			
			foreach (AddIn addIn in addIns) {
				if (!addInFiles.Contains(addIn.FileName))
					addInFiles.Add(addIn.FileName);
				addIn.Enabled = false;
				addIn.Action = AddInAction.Install;
				AddInTree.InsertAddIn(addIn);
			}
			
			SaveAddInConfiguration(addInFiles, disabled);
		}
		
		public static void RemoveExternalAddIns(IList<AddIn> addIns)
		{
			List<string> addInFiles = new List<string>();
			List<string> disabled = new List<string>();
			LoadAddInConfiguration(addInFiles, disabled);
			
			foreach (AddIn addIn in addIns) {
				foreach (string identity in addIn.Manifest.Identities.Keys) {
					disabled.Remove(identity);
				}
				addInFiles.Remove(addIn.FileName);
				addIn.Action = AddInAction.Uninstall;
				if (!addIn.Enabled) {
					AddInTree.RemoveAddIn(addIn);
				}
			}
			
			SaveAddInConfiguration(addInFiles, disabled);
		}
		
		public static void Enable(IList<AddIn> addIns)
		{
			List<string> addInFiles = new List<string>();
			List<string> disabled = new List<string>();
			LoadAddInConfiguration(addInFiles, disabled);
			
			foreach (AddIn addIn in addIns) {
				foreach (string identity in addIn.Manifest.Identities.Keys) {
					disabled.Remove(identity);
				}
				if (addIn.Action == AddInAction.Uninstall) {
					if (!addInFiles.Contains(addIn.FileName))
						addInFiles.Add(addIn.FileName);
				}
				addIn.Action = AddInAction.Enable;
			}
			
			SaveAddInConfiguration(addInFiles, disabled);
		}
		
		public static void Disable(IList<AddIn> addIns)
		{
			List<string> addInFiles = new List<string>();
			List<string> disabled = new List<string>();
			LoadAddInConfiguration(addInFiles, disabled);
			
			foreach (AddIn addIn in addIns) {
				string identity = addIn.Manifest.PrimaryIdentity;
				if (identity == null)
					throw new ArgumentException("The AddIn cannot be disabled because it has no identity.");
				
				if (!disabled.Contains(identity))
					disabled.Add(identity);
				addIn.Action = AddInAction.Disable;
			}
			
			SaveAddInConfiguration(addInFiles, disabled);
		}
		
		/// <summary>
		/// Loads a configuration file.
		/// The 'file' from XML elements in the form "&lt;AddIn file='full path to .addin file'&gt;" will
		/// be added to <paramref name="addInFiles"/>, the 'addin' element from
		/// "&lt;Disable addin='addin identity'&gt;" will be added to <paramref name="disabledAddIns"/>,
		/// all other XML elements are ignored.
		/// </summary>
		public static void LoadAddInConfiguration(List<string> addInFiles, List<string> disabledAddIns)
		{
			if (!File.Exists(configurationFileName))
				return;
			using (XmlTextReader reader = new XmlTextReader(configurationFileName)) {
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						if (reader.Name == "AddIn") {
							string fileName = reader.GetAttribute("file");
							if (fileName != null && fileName.Length > 0) {
								addInFiles.Add(fileName);
							}
						} else if (reader.Name == "Disable") {
							string addIn = reader.GetAttribute("addin");
							if (addIn != null && addIn.Length > 0) {
								disabledAddIns.Add(addIn);
							}
						}
					}
				}
			}
		}
		
		public static void SaveAddInConfiguration(List<string> addInFiles, List<string> disabledAddIns)
		{
			using (XmlTextWriter writer = new XmlTextWriter(configurationFileName, Encoding.UTF8)) {
				writer.Formatting = Formatting.Indented;
				writer.WriteStartDocument();
				writer.WriteStartElement("AddInConfiguration");
				foreach (string file in addInFiles) {
					writer.WriteStartElement("AddIn");
					writer.WriteAttributeString("file", file);
					writer.WriteEndElement();
				}
				foreach (string name in disabledAddIns) {
					writer.WriteStartElement("Disable");
					writer.WriteAttributeString("addin", name);
					writer.WriteEndElement();
				}
				writer.WriteEndDocument();
			}
		}
	}
}
