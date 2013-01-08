// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpZipLib.Zip;
using NuGet;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Helper class for AddIn setup operations.
	/// </summary>
	public class AddInSetup : IAddInSetup
	{
		private IAddInManagerEvents _events = null;
		private INuGetPackageManager _nuGet = null;
		
		private List<ManagedAddIn> _addInsMarkedForInstall;
		
		public AddInSetup(IAddInManagerEvents events, INuGetPackageManager nuGet)
		{
			_events = events;
			_nuGet = nuGet;
			
			_addInsMarkedForInstall = new List<ManagedAddIn>();
			
			// Register event handlers
			_events.AddInPackageDownloaded += events_AddInPackageDownloaded;
		}
		
		public IEnumerable<ManagedAddIn> AddInsWithMarkedForInstallation
		{
			get
			{
				return SD.AddInTree.AddIns.Select(a => new ManagedAddIn(a) { IsTemporary = false, IsUpdate = false })
					.GroupJoin(
						_addInsMarkedForInstall,
						installedAddIn => installedAddIn.AddIn.Manifest.PrimaryIdentity,
						markedAddIn => markedAddIn.AddIn.Manifest.PrimaryIdentity,
						(installedAddIn, e) => e.ElementAtOrDefault(0) ?? installedAddIn);
			}
		}

		private void events_AddInPackageDownloaded(object sender, PackageOperationEventArgs e)
		{
			try
			{
				InstallAddIn(e.Package, e.InstallPath);
			}
			catch (Exception ex)
			{
				_events.OnAddInOperationError(new AddInExceptionEventArgs(ex));
			}
		}
		
		private AddIn LoadAddInFromZip(ZipFile file)
		{
			AddIn resultAddIn = null;
			ZipEntry addInEntry = null;
			foreach (ZipEntry entry in file)
			{
				if (entry.Name.EndsWith(".addin"))
				{
					if (addInEntry != null)
					{
						throw new AddInLoadException("The package may only contain one .addin file.");
					}
					addInEntry = entry;
				}
			}
			if (addInEntry == null)
			{
				throw new AddInLoadException("The package must contain one .addin file.");
			}
			using (Stream s = file.GetInputStream(addInEntry))
			{
				using (StreamReader r = new StreamReader(s))
				{
					resultAddIn = AddIn.Load(SD.AddInTree, r);
				}
			}
			
			return resultAddIn;
		}
		
		public AddIn InstallAddIn(string archiveFileName)
		{
			if (archiveFileName != null)
			{
				// Try to load the *.sdaddin file as ZIP archive
				AddIn addIn = null;
				ZipFile zipFile = new ZipFile(archiveFileName);
				try
				{
					addIn = LoadAddInFromZip(zipFile);
				}
				finally
				{
					zipFile.Close();
				}
				
				if (addIn != null)
				{
					if (addIn.Manifest.PrimaryIdentity == null)
					{
						throw new AddInLoadException(ResourceService.GetString("AddInManager.AddInMustHaveIdentity"));
					}

					// Try to find this AddIn in current registry
					string identity = addIn.Manifest.PrimaryIdentity;
					AddIn foundAddIn = null;
					foreach (AddIn treeAddIn in SD.AddInTree.AddIns)
					{
						if (treeAddIn.Manifest.Identities.ContainsKey(identity))
						{
							foundAddIn = treeAddIn;
							break;
						}
					}

					// Prevent this AddIn from being uninstalled, if marked for deinstallation
					foreach (string installedIdentity in addIn.Manifest.Identities.Keys)
					{
						ICSharpCode.Core.AddInManager.AbortRemoveUserAddInOnNextStart(installedIdentity);
					}
					
					// Create target directory for AddIn in user profile & copy package contents there
					CopyAddInFromZip(addIn, archiveFileName);
					
					// Install the AddIn using manifest
					if (foundAddIn != null)
					{
						addIn.Action = AddInAction.Update;
					}
					else
					{
						addIn.Action = AddInAction.Install;
						((AddInTreeImpl)SD.AddInTree).InsertAddIn(addIn);
					}
					
					// Mark this AddIn
					ManagedAddIn markedAddIn = new ManagedAddIn(addIn)
					{
						IsTemporary = true,
						IsUpdate = (foundAddIn != null),
						OldVersion = (foundAddIn != null) ? foundAddIn.Version : null
					};
					_addInsMarkedForInstall.Add(markedAddIn);
					
					// Successful installation
					AddInInstallationEventArgs eventArgs = new AddInInstallationEventArgs(addIn);
					_events.OnAddInInstalled(eventArgs);
					_events.OnAddInStateChanged(eventArgs);
					
					return addIn;
				}
				else
				{
					// This is not a valid SharpDevelop AddIn package!
					// TODO Throw something.
				}
			}

			return null;
		}
		
		public AddIn InstallAddIn(IPackage package, string packageDirectory)
		{
			// Lookup for .addin file in package output
			var addInManifestFile = Directory.EnumerateFiles(packageDirectory, "*.addin", SearchOption.TopDirectoryOnly).FirstOrDefault();
			if (addInManifestFile != null)
			{
				AddIn addIn = AddIn.Load(SD.AddInTree, addInManifestFile);
				if (addIn.Manifest.PrimaryIdentity == null)
				{
					throw new AddInLoadException(ResourceService.GetString("AddInManager.AddInMustHaveIdentity"));
				}

				// Try to find this AddIn in current registry
				string identity = addIn.Manifest.PrimaryIdentity;
				AddIn foundAddIn = null;
				foreach (AddIn treeAddIn in SD.AddInTree.AddIns)
				{
					if (treeAddIn.Manifest.Identities.ContainsKey(identity))
					{
						foundAddIn = treeAddIn;
						break;
					}
				}

				// Prevent this AddIn from being uninstalled, if marked for deinstallation
				foreach (string installedIdentity in addIn.Manifest.Identities.Keys)
				{
					ICSharpCode.Core.AddInManager.AbortRemoveUserAddInOnNextStart(installedIdentity);
				}
				
				// Create target directory for AddIn in user profile & copy package contents there
				CopyAddInFromPackage(addIn, packageDirectory);

				// Install the AddIn using manifest
				if (foundAddIn != null)
				{
					addIn.Action = AddInAction.Update;
				}
				else
				{
					addIn.Action = AddInAction.Install;
					((AddInTreeImpl)SD.AddInTree).InsertAddIn(addIn);
				}
				
				// Mark this AddIn
				ManagedAddIn markedAddIn = new ManagedAddIn(addIn)
				{
					IsTemporary = true,
					IsUpdate = (foundAddIn != null),
					OldVersion = (foundAddIn != null) ? foundAddIn.Version : null
				};
				_addInsMarkedForInstall.Add(markedAddIn);
				
				// Successful installation
				AddInInstallationEventArgs eventArgs = new AddInInstallationEventArgs(addIn);
				_events.OnAddInInstalled(eventArgs);
				_events.OnAddInStateChanged(eventArgs);
				
				return addIn;
			}
			else
			{
				// This is not a valid SharpDevelop AddIn package!
				// TODO Throw something.
			}

			return null;
		}
		
		private bool CopyAddInFromZip(AddIn addIn, string zipFile)
		{
			try
			{
				string targetDir = Path.Combine(ICSharpCode.Core.AddInManager.AddInInstallTemp,
				                                addIn.Manifest.PrimaryIdentity);
				if (Directory.Exists(targetDir))
				{
					Directory.Delete(targetDir, true);
				}
				Directory.CreateDirectory(targetDir);
				FastZip fastZip = new FastZip();
				fastZip.CreateEmptyDirectories = true;
				fastZip.ExtractZip(zipFile, targetDir, null);
				
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
		
		private bool CopyAddInFromPackage(AddIn addIn, string packageDirectory)
		{
			try
			{
				string targetDir = Path.Combine(ICSharpCode.Core.AddInManager.AddInInstallTemp,
				                                addIn.Manifest.PrimaryIdentity);
				if (Directory.Exists(targetDir))
				{
					Directory.Delete(targetDir, true);
				}
				Directory.CreateDirectory(targetDir);
				var packageContentsFiles = Directory.EnumerateFiles(packageDirectory, "*.*", SearchOption.TopDirectoryOnly);
				if (packageContentsFiles != null)
				{
					foreach (var file in packageContentsFiles)
					{
						// Don't copy the .nupkg file
						FileInfo fileInfo = new FileInfo(file);
						if (fileInfo.Extension != ".nupkg")
						{
							File.Copy(file, Path.Combine(targetDir, fileInfo.Name));
						}
					}
				}
				
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void CancelUpdate(AddIn addIn)
		{
			if (addIn != null)
			{
				CancelPendingUpdate(addIn);
				
				// If there is also a NuGet package installed for this, delete it as well
				IPackage addInPackage = GetNuGetPackageForAddIn(addIn, true);
				if (addInPackage != null)
				{
					_nuGet.Packages.UninstallPackage(addInPackage, true, false);
				}
				
				AddInInstallationEventArgs eventArgs = new AddInInstallationEventArgs(addIn);
				eventArgs.PreviousVersionRemains = true;
				_events.OnAddInUninstalled(eventArgs);
				_events.OnAddInStateChanged(eventArgs);
			}
		}
		
		private void CancelPendingUpdate(AddIn addIn)
		{
			if (addIn != null)
			{
				foreach (string identity in addIn.Manifest.Identities.Keys)
				{
					// Delete from installation temp (if installation or update is pending)
					string targetDir = Path.Combine(ICSharpCode.Core.AddInManager.AddInInstallTemp, identity);
					if (Directory.Exists(targetDir))
					{
						Directory.Delete(targetDir, true);
					}
				}
				
				_addInsMarkedForInstall.RemoveAll(markedAddIn => markedAddIn.AddIn == addIn);
			}
		}
		
		public void CancelInstallation(AddIn addIn)
		{
			if (addIn != null)
			{
				_addInsMarkedForInstall.RemoveAll(markedAddIn => markedAddIn.AddIn == addIn);
				UninstallAddIn(addIn);
				
				// If there is also a NuGet package installed for this, delete it as well
				IPackage addInPackage = GetNuGetPackageForAddIn(addIn, true);
				if (addInPackage != null)
				{
					_nuGet.Packages.UninstallPackage(addInPackage, true, false);
				}
			}
		}
		
		public void CancelUninstallation(AddIn addIn)
		{
			if (addIn != null)
			{
				// Abort uninstallation of this AddIn
				foreach (string identity in addIn.Manifest.Identities.Keys)
				{
					ICSharpCode.Core.AddInManager.AbortRemoveUserAddInOnNextStart(identity);
				}
				ICSharpCode.Core.AddInManager.Enable(new AddIn[] { addIn });
				_events.OnAddInStateChanged(new AddInInstallationEventArgs(addIn));
			}
		}

		public void UninstallAddIn(AddIn addIn)
		{
			if (addIn != null)
			{
				List<AddIn> addInList = new List<AddIn>();
				addInList.Add(addIn);
				ICSharpCode.Core.AddInManager.RemoveExternalAddIns(addInList);
				
				CancelPendingUpdate(addIn);
				foreach (string identity in addIn.Manifest.Identities.Keys)
				{
					// Remove the user AddIn
					string targetDir = Path.Combine(ICSharpCode.Core.AddInManager.UserAddInPath, identity);
					if (Directory.Exists(targetDir))
					{
						if (!addIn.Enabled)
						{
							try
							{
								Directory.Delete(targetDir, true);
								continue;
							}
							catch
							{
								// TODO Throw something?
							}
						}
						
						ICSharpCode.Core.AddInManager.RemoveUserAddInOnNextStart(identity);
					}
				}
				
				// Successfully uninstalled
				AddInInstallationEventArgs eventArgs = new AddInInstallationEventArgs(addIn);
				_events.OnAddInUninstalled(eventArgs);
				_events.OnAddInStateChanged(eventArgs);
			}
		}
		
		public void SwitchAddInActivation(AddIn addIn)
		{
			if (addIn != null)
			{
				// Decide whether to enable or to disable the AddIn
				bool disable = addIn.Enabled;
				if (addIn.Action == AddInAction.Disable)
				{
					disable = false;
				}
				else if (addIn.Action == AddInAction.Enable)
				{
					disable = true;
				}
				
				if (disable)
				{
					ICSharpCode.Core.AddInManager.Disable(new AddIn[] { addIn });
				}
				else
				{
					ICSharpCode.Core.AddInManager.Enable(new AddIn[] { addIn });
				}
				
				_events.OnAddInStateChanged(new AddInInstallationEventArgs(addIn));
			}
		}
		
		public AddIn GetInstalledAddInByIdentity(string identity)
		{
			if (!String.IsNullOrEmpty(identity))
			{
				return SD.AddInTree.AddIns
					.Where(a => (a.Manifest != null) && a.Manifest.Identities.ContainsKey(identity))
					.FirstOrDefault();
			}
			else
			{
				return null;
			}
		}
		
		public bool IsAddInInstalled(AddIn addIn)
		{
			if ((addIn == null) || (addIn.Manifest == null))
			{
				// Without a valid AddIn instance or a manifest we can't do anything...
				return false;
			}
			
			string identity = addIn.Manifest.PrimaryIdentity;
			if (!String.IsNullOrEmpty(identity))
			{
				return (SD.AddInTree.AddIns
				        .Where(a => (addIn == a) || ((a.Manifest != null) && a.Manifest.Identities.ContainsKey(identity)))
				        .FirstOrDefault() != null);
			}
			else
			{
				return false;
			}
		}
		
		public bool IsAddInPreinstalled(AddIn addIn)
		{
			if (addIn != null)
			{
				return
					String.Equals(addIn.Properties["addInManagerHidden"], "preinstalled", StringComparison.OrdinalIgnoreCase)
					&& FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, addIn.FileName);
			}
			else
			{
				return false;
			}
		}
		
		public IPackage GetNuGetPackageForAddIn(AddIn addIn, bool getLatest)
		{
			if (addIn == null)
			{
				throw new ArgumentNullException("addIn");
			}
			
			IPackage package = null;
			string nuGetPackageID = null;
			if (addIn.Properties.Contains("nuGetPackageID"))
			{
				nuGetPackageID = addIn.Properties["nuGetPackageID"];
			}
			string primaryIdentity = null;
			if (addIn.Manifest != null)
			{
				primaryIdentity = addIn.Manifest.PrimaryIdentity;
			}
			
			if (!String.IsNullOrEmpty(nuGetPackageID))
			{
				// Find installed package with mapped NuGet package ID
				var matchingPackages = _nuGet.Packages.LocalRepository.GetPackages()
					.Where(p => (p.Id == primaryIdentity) || (p.Id == nuGetPackageID))
					.OrderBy(p => p.Version);
				if (getLatest)
				{
					// Return latest package version
					package = matchingPackages.LastOrDefault();
				}
				else
				{
					// Return oldest installed package version
					package = matchingPackages.FirstOrDefault();
				}
			}
			
			return package;
		}
		
		public AddIn GetAddInForNuGetPackage(IPackage package)
		{
			return GetAddInForNuGetPackage(package, false);
		}
		
		public AddIn GetAddInForNuGetPackage(IPackage package, bool withAddInsMarkedForInstallation)
		{
			if (withAddInsMarkedForInstallation)
			{
				return GetInstalledOrMarkedAddInForNuGetPackage(package);
			}
			else
			{
				return GetInstalledAddInForNuGetPackage(package);
			}
		}
		
		private AddIn GetInstalledAddInForNuGetPackage(IPackage package)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}
			
			AddIn foundAddIn = SD.AddInTree.AddIns.Where(
				a => ((a.Manifest != null) && (a.Manifest.PrimaryIdentity == package.Id))
				|| (a.Properties.Contains("nuGetPackageID") && (a.Properties["nuGetPackageID"] == package.Id)))
				.FirstOrDefault();
			
			return foundAddIn;
		}
		
		private AddIn GetInstalledOrMarkedAddInForNuGetPackage(IPackage package)
		{
			if (package == null)
			{
				throw new ArgumentNullException("package");
			}
			
			ManagedAddIn foundAddIn = AddInsWithMarkedForInstallation.Where(
				a => ((a.AddIn.Manifest != null) && (a.AddIn.Manifest.PrimaryIdentity == package.Id))
				|| (a.AddIn.Properties.Contains("nuGetPackageID") && (a.AddIn.Properties["nuGetPackageID"] == package.Id)))
				.FirstOrDefault();
			
			return (foundAddIn != null) ? foundAddIn.AddIn : null;
		}
		
		public IEnumerable<ManagedAddIn> GetDependentAddIns(AddIn addIn)
		{
			if ((addIn != null) && (addIn.Manifest != null) && (addIn.Manifest.PrimaryIdentity != null))
			{
				// Get all AddIns which are dependent from given AddIn
				var dependentAddIns = AddInsWithMarkedForInstallation.Where(
					a => (a.AddIn.Manifest != null) && a.AddIn.Manifest.Dependencies.Any(
						reference => reference.Name == addIn.Manifest.PrimaryIdentity));
				return dependentAddIns;
			}
			
			return null;
		}
		
		public void RemoveUnreferencedNuGetPackages()
		{
			// Get list of installed NuGet packages
			var localRepositoryPackages = _nuGet.Packages.LocalRepository.GetPackages();
			if (localRepositoryPackages != null)
			{
				List<IPackage> installedNuGetPackages = new List<IPackage>(localRepositoryPackages);
				foreach (var installedPackage in installedNuGetPackages)
				{
					bool removeThisPackage = false;
					AddIn addIn = GetAddInForNuGetPackage(installedPackage);
					if (addIn == null)
					{
						// There is no AddIn for this package -> remove it
						removeThisPackage = true;
					}
					else
					{
						// Try to get the most recent (= with highest version) package for this AddIn
						IPackage latestPackage = installedNuGetPackages
							.Where(p => p.Id == installedPackage.Id)
							.OrderBy(p => p.Version)
							.LastOrDefault();
						if (latestPackage.Version != installedPackage.Version)
						{
							// This is not the most recent installed package for this AddIn -> remove it
							removeThisPackage = true;
						}
					}
					
					if (removeThisPackage)
					{
						// We decided to remove this package
						LoggingService.InfoFormatted("Removing unreferenced NuGet package {0} {1}.",
						                             installedPackage.Id, installedPackage.Version.ToString());
						_nuGet.Packages.UninstallPackage(installedPackage, true, false);
					}
				}
			}
		}
		
//		private void LoadMappings()
//		{
//			var savedMappings = PropertyService.Get<string[]>("AddInManager2.AddInPackageMappings", null);
//			if (savedMappings != null)
//			{
//				foreach (var mapping in savedMappings)
//				{
//					string[] mappingParts = mapping.Split(new char[] { '|' }, 2);
//					if ((mappingParts != null) && (mappingParts.Length == 2))
//					{
//						_addInToNuGetMapping[mappingParts[0]] = mappingParts[1];
//					}
//				}
//			}
//			else
//			{
//				// Save empty mapping
//				SaveMappings();
//			}
//		}
//
//		private void SaveMappings()
//		{
//			List<string> mappingList = new List<string>();
//			foreach (var mapping in _addInToNuGetMapping)
//			{
//				mappingList.Add(mapping.Key + "|" + mapping.Value);
//			}
//
//			PropertyService.Set<string[]>("AddInManager2.AddInPackageMappings", mappingList.ToArray());
//		}
	}
}
