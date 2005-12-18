// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
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
		Update,
		InstalledTwice,
		DependencyError
	}
	
	public static class AddInManager
	{
		static string configurationFileName;
		static string addInInstallTemp;
		static string userAddInPath;
		
		public static string UserAddInPath {
			get {
				return userAddInPath;
			}
			set {
				userAddInPath = value;
			}
		}
		
		public static string AddInInstallTemp {
			get {
				return addInInstallTemp;
			}
			set {
				addInInstallTemp = value;
			}
		}
		
		public static string ConfigurationFileName {
			get {
				return configurationFileName;
			}
			set {
				configurationFileName = value;
			}
		}
		
		/// <summary>
		/// Installs the AddIns from AddInInstallTemp to the UserAddInPath.
		/// In case of installation errors, a error message is displayed to the user
		/// and the affected AddIn is added to the disabled list.
		/// </summary>
		public static void InstallAddIns(List<string> disabled)
		{
			if (!Directory.Exists(addInInstallTemp))
				return;
			LoggingService.Info("AddInManager.InstallAddIns started");
			if (!Directory.Exists(userAddInPath))
				Directory.CreateDirectory(userAddInPath);
			string removeFile = Path.Combine(addInInstallTemp, "remove.txt");
			bool allOK = true;
			List<string> notRemoved = new List<string>();
			if (File.Exists(removeFile)) {
				using (StreamReader r = new StreamReader(removeFile)) {
					string addInName;
					while ((addInName = r.ReadLine()) != null) {
						addInName = addInName.Trim();
						if (addInName.Length == 0)
							continue;
						string targetDir = Path.Combine(userAddInPath, addInName);
						if (!UninstallAddIn(disabled, addInName, targetDir)) {
							notRemoved.Add(addInName);
							allOK = false;
						}
					}
				}
				if (notRemoved.Count == 0) {
					LoggingService.Info("Deleting remove.txt");
					File.Delete(removeFile);
				} else {
					LoggingService.Info("Rewriting remove.txt");
					using (StreamWriter w = new StreamWriter(removeFile)) {
						notRemoved.ForEach(w.WriteLine);
					}
				}
			}
			foreach (string sourceDir in Directory.GetDirectories(addInInstallTemp)) {
				string addInName = Path.GetFileName(sourceDir);
				string targetDir = Path.Combine(userAddInPath, addInName);
				if (notRemoved.Contains(addInName)) {
					LoggingService.Info("Skipping installation of " + addInName + " because deinstallation failed.");
					continue;
				}
				if (UninstallAddIn(disabled, addInName, targetDir)) {
					LoggingService.Info("Installing " + addInName + "...");
					Directory.Move(sourceDir, targetDir);
				} else {
					allOK = false;
				}
			}
			if (allOK) {
				try {
					Directory.Delete(addInInstallTemp, false);
				} catch (Exception ex) {
					LoggingService.Warn("Error removing install temp", ex);
				}
			}
			LoggingService.Info("AddInManager.InstallAddIns finished");
		}
		
		static bool UninstallAddIn(List<string> disabled, string addInName, string targetDir)
		{
			if (Directory.Exists(targetDir)) {
				LoggingService.Info("Removing " + addInName + "...");
				try {
					Directory.Delete(targetDir, true);
				} catch (Exception ex) {
					disabled.Add(addInName);
					MessageService.ShowError("Error removing " + addInName + ":\n" +
					                         ex.Message + "\nThe AddIn will be " +
					                         "removed on the next start of SharpDevelop and is disabled " +
					                         "for now.");
					return false;
				}
			}
			return true;
		}
		
		public static void RemoveUserAddInOnNextStart(string identity)
		{
			List<string> removeEntries = new List<string>();
			string removeFile = Path.Combine(addInInstallTemp, "remove.txt");
			if (File.Exists(removeFile)) {
				using (StreamReader r = new StreamReader(removeFile)) {
					string addInName;
					while ((addInName = r.ReadLine()) != null) {
						addInName = addInName.Trim();
						if (addInName.Length > 0)
							removeEntries.Add(addInName);
					}
				}
				if (removeEntries.Contains(identity))
					return;
			}
			removeEntries.Add(identity);
			if (!Directory.Exists(addInInstallTemp))
				Directory.CreateDirectory(addInInstallTemp);
			using (StreamWriter w = new StreamWriter(removeFile)) {
				removeEntries.ForEach(w.WriteLine);
			}
		}
		
		public static void AbortRemoveUserAddInOnNextStart(string identity)
		{
			string removeFile = Path.Combine(addInInstallTemp, "remove.txt");
			if (!File.Exists(removeFile)) {
				return;
			}
			List<string> removeEntries = new List<string>();
			using (StreamReader r = new StreamReader(removeFile)) {
				string addInName;
				while ((addInName = r.ReadLine()) != null) {
					addInName = addInName.Trim();
					if (addInName.Length > 0)
						removeEntries.Add(addInName);
				}
			}
			if (removeEntries.Remove(identity)) {
				using (StreamWriter w = new StreamWriter(removeFile)) {
					removeEntries.ForEach(w.WriteLine);
				}
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
					if (FileUtility.IsBaseDirectory(userAddInPath, addIn.FileName)) {
						foreach (string identity in addIn.Manifest.Identities.Keys) {
							AbortRemoveUserAddInOnNextStart(identity);
						}
					} else {
						if (!addInFiles.Contains(addIn.FileName))
							addInFiles.Add(addIn.FileName);
					}
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
