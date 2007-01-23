// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace ICSharpCode.Core
{
	/// <summary>
	///   Specifies the action to be taken for a specific <see cref="AddIn"/>.
	/// </summary>
	public enum AddInAction
	{
		/// <summary>
		///   Enable the <see cref="AddIn"/>.
		/// </summary>
		Enable,
		/// <summary>
		///     Disable the <see cref="AddIn"/>.
		/// </summary>
		Disable,
		/// <summary>
		///     Install the <see cref="AddIn"/>.
		/// </summary>
		Install,
		/// <summary>
		///     Uninstall the <see cref="AddIn"/>.
		/// </summary>
		Uninstall,
		/// <summary>
		///     Update the <see cref="AddIn"/>.
		/// </summary>
		Update,
		/// <summary>
		/// The <see cref="AddIn"/> is disabled because it has been installed
		/// twice (duplicate identity).
		/// </summary>
		InstalledTwice,
		/// <summary>
		///     Tells that the <see cref="AddIn"/> cannot be loaded because not all dependencies are satisfied.
		/// </summary>
		DependencyError,
		/// <summary>
		/// A custom error has occurred (e.g. the AddIn disabled itself using a condition).
		/// </summary>
		CustomError
	}
	
	/// <summary>
	/// Manages all actions performed on <see cref="AddIn"/>s.
	/// An AddInManager GUI can use the methods here to install/update/uninstall
	/// <see cref="AddIn"/>s.
	/// 
	/// There are three types of AddIns:
	/// - Preinstalled AddIns (added by host application) -> can only be disabled
	/// - External AddIns -> can be added, disabled and removed
	/// 	Removing external AddIns only removes the reference to the .addin file
	///     but does not delete the AddIn.
	/// - User AddIns -> are installed to UserAddInPath, can be installed, disabled
	///     and uninstalled
	/// </summary>
	public static class AddInManager
	{
		static string configurationFileName;
		static string addInInstallTemp;
		static string userAddInPath;
		
		/// <summary>
		/// Gets or sets the user addin path.
		/// This is the path where user AddIns are installed to.
		/// This property is normally initialized by <see cref="CoreStartup.ConfigureUserAddIns"/>.
		/// </summary>
		public static string UserAddInPath {
			get {
				return userAddInPath;
			}
			set {
				userAddInPath = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the addin install temporary directory.
		/// This is a directory used to store AddIns that should be installed on
		/// the next start of the application.
		/// This property is normally initialized by <see cref="CoreStartup.ConfigureUserAddIns"/>.
		/// </summary>
		public static string AddInInstallTemp {
			get {
				return addInInstallTemp;
			}
			set {
				addInInstallTemp = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the full name of the configuration file.
		/// In this file, the AddInManager stores the list of disabled AddIns
		/// and the list of installed external AddIns.
		/// This property is normally initialized by <see cref="CoreStartup.ConfigureExternalAddIns"/>.
		/// </summary>
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
		/// This method is normally called by <see cref="CoreStartup.ConfigureUserAddIns"/>
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
					                         "removed on the next start of " + MessageService.ProductName +
					                         " and is disabled for now.");
					return false;
				}
			}
			return true;
		}
		
		/// <summary>
		/// Uninstalls the user addin on next start.
		/// <see cref="RemoveUserAddInOnNextStart"/> schedules the AddIn for
		/// deinstallation, you can unschedule it using
		/// <see cref="AbortRemoveUserAddInOnNextStart"/>
		/// </summary>
		/// <param name="identity">The identity of the addin to remove.</param>
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
		
		/// <summary>
		/// Prevents a user AddIn from being uninstalled.
		/// <see cref="RemoveUserAddInOnNextStart"/> schedules the AddIn for
		/// deinstallation, you can unschedule it using
		/// <see cref="AbortRemoveUserAddInOnNextStart"/>
		/// </summary>
		/// <param name="identity">The identity of which to abort the removal.</param>
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
		
		/// <summary>
		/// Adds the specified external AddIns to the list of registered external
		/// AddIns.
		/// </summary>
		/// <param name="addIns">
		/// The list of AddIns to add. (use <see cref="AddIn"/> instances
		/// created by <see cref="AddIn.Load(TextReader)"/>).
		/// </param>
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
		
		/// <summary>
		/// Removes the specified external AddIns from the list of registered external
		/// AddIns.
		/// </summary>
		/// The list of AddIns to remove.
		/// (use external AddIns from the <see cref="AddInTree.AddIns"/> collection).
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
		
		/// <summary>
		/// Marks the specified AddIns as enabled (will take effect after
		/// next application restart).
		/// </summary>
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
		
		/// <summary>
		/// Marks the specified AddIns as disabled (will take effect after
		/// next application restart).
		/// </summary>
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
		/// <param name="addInFiles">File names of external AddIns are added to this collection.</param>
		/// <param name="disabledAddIns">Identities of disabled addins are added to this collection.</param>
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
		
		/// <summary>
		/// Saves the AddIn configuration in the format expected by
		/// <see cref="LoadAddInConfiguration"/>.
		/// </summary>
		/// <param name="addInFiles">List of file names of external AddIns.</param>
		/// <param name="disabledAddIns">List of Identities of disabled addins.</param>
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
