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
		private ISDAddInManagement _sdAddInManagement = null;
		
		private List<ManagedAddIn> _addInsMarkedForInstall;
		
		public AddInSetup(IAddInManagerEvents events, INuGetPackageManager nuGet, ISDAddInManagement sdAddInManagement)
		{
			_events = events;
			_nuGet = nuGet;
			_sdAddInManagement = sdAddInManagement;
			
			_addInsMarkedForInstall = new List<ManagedAddIn>();
			
			// Register event handlers
			_events.AddInPackageDownloaded += events_AddInPackageDownloaded;
		}
		
		public IEnumerable<ManagedAddIn> AddInsWithMarkedForInstallation
		{
			get
			{
				return _sdAddInManagement.AddIns.Select(a => new ManagedAddIn(a) { IsTemporary = false, IsUpdate = false })
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
				_events.OnAddInOperationError(new AddInOperationErrorEventArgs(ex));
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
						_events.OnAddInOperationError(
							new AddInOperationErrorEventArgs(
								SD.ResourceService.GetString("AddInManager2.InvalidPackage")));
						return null;
					}
					addInEntry = entry;
				}
			}
			if (addInEntry == null)
			{
				_events.OnAddInOperationError(
					new AddInOperationErrorEventArgs(
						SD.ResourceService.GetString("AddInManager2.InvalidPackage")));
				return null;
			}
			using (Stream s = file.GetInputStream(addInEntry))
			{
				using (StreamReader r = new StreamReader(s))
				{
					resultAddIn = _sdAddInManagement.Load(r);
				}
			}
			
			return resultAddIn;
		}
		
		public AddIn InstallAddIn(string fileName)
		{
			if (fileName != null)
			{
				AddIn addIn = null;
				
				bool installAsExternal = false;
				
				switch (Path.GetExtension(fileName).ToLowerInvariant())
				{
					case ".addin":
						
						SD.Log.DebugFormatted("[AddInManager2] Loading {0} as manifest.", fileName);
						
						if (FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, fileName))
						{
							// Don't allow to install AddIns from application root path
							_events.OnAddInOperationError(
								new AddInOperationErrorEventArgs(
									SD.ResourceService.GetString("AddInManager.CannotInstallIntoApplicationDirectory")));
							return null;
						}
						
						// Load directly from location
						addIn = _sdAddInManagement.Load(fileName);
						installAsExternal = true;
						
						break;
						
					case ".sdaddin":
					case ".zip":
						
						SD.Log.DebugFormatted("[AddInManager2] Trying to load {0} as local AddIn package.", fileName);
						
						// Try to load the *.sdaddin file as ZIP archive
						ZipFile zipFile = null;
						try
						{
							zipFile = new ZipFile(fileName);
							addIn = LoadAddInFromZip(zipFile);
						}
						catch (Exception)
						{
							// ZIP file seems not to be valid
							_events.OnAddInOperationError(
								new AddInOperationErrorEventArgs(
									SD.ResourceService.GetString("AddInManager2.InvalidPackage")));
							return null;
						}
						finally
						{
							if (zipFile != null)
							{
								zipFile.Close();
							}
						}
						
						break;
						
					default:
						// Unknown format of file
						_events.OnAddInOperationError(
							new AddInOperationErrorEventArgs(
								SD.ResourceService.GetString("AddInManager.UnknownFileFormat") + " " + Path.GetExtension(fileName)));
						return null;
				}
				
				if (addIn != null)
				{
					if ((addIn.Manifest == null) || (addIn.Manifest.PrimaryIdentity == null))
					{
						_events.OnAddInOperationError(
							new AddInOperationErrorEventArgs(
								new AddInLoadException(SD.ResourceService.GetString("AddInManager.AddInMustHaveIdentity"))));
						return null;
					}

					// Try to find this AddIn in current registry
					string identity = addIn.Manifest.PrimaryIdentity;
					AddIn foundAddIn = null;
					foreach (AddIn treeAddIn in _sdAddInManagement.AddIns)
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
						_sdAddInManagement.AbortRemoveUserAddInOnNextStart(installedIdentity);
					}
					
					if (!installAsExternal)
					{
						// Create target directory for AddIn in user profile & copy package contents there
						CopyAddInFromZip(addIn, fileName);
						
						// Install the AddIn using manifest
						if (foundAddIn != null)
						{
							addIn.Action = AddInAction.Update;
						}
						else
						{
							addIn.Action = AddInAction.Install;
							_sdAddInManagement.AddToTree(addIn);
						}
					}
					else
					{
						// Only add a reference to an external manifest
						_sdAddInManagement.AddExternalAddIns(new AddIn[] { addIn });
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
			}

			// In successful cases we should have exited somewhere else, in error cases the error event was already fired.
			return null;
		}
		
		public AddIn InstallAddIn(IPackage package, string packageDirectory)
		{
			// Lookup for .addin file in package output
			var addInManifestFile = Directory.EnumerateFiles(packageDirectory, "*.addin", SearchOption.TopDirectoryOnly).FirstOrDefault();
			if (addInManifestFile != null)
			{
				SD.Log.DebugFormatted("[AddInManager2] Installing AddIn from package {0} {1}", package.Id, package.Version.ToString());
				
				AddIn addIn = _sdAddInManagement.Load(addInManifestFile);
				if (addIn.Manifest.PrimaryIdentity == null)
				{
					_events.OnAddInOperationError(
						new AddInOperationErrorEventArgs(
							new AddInLoadException(SD.ResourceService.GetString("AddInManager.AddInMustHaveIdentity"))));
					return null;
				}

				// Try to find this AddIn in current registry
				string identity = addIn.Manifest.PrimaryIdentity;
				AddIn foundAddIn = null;
				foreach (AddIn treeAddIn in _sdAddInManagement.AddIns)
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
					_sdAddInManagement.AbortRemoveUserAddInOnNextStart(installedIdentity);
				}
				
				// Create target directory for AddIn in user profile & copy package contents there
				CopyAddInFromPackage(addIn, packageDirectory);

				// Install the AddIn using manifest
				if (foundAddIn != null)
				{
					addIn.Action = AddInAction.Update;
					
					SD.Log.DebugFormatted("[AddInManager2] Marked AddIn {0} for update.", addIn.Name);
				}
				else
				{
					addIn.Action = AddInAction.Install;
					_sdAddInManagement.AddToTree(addIn);
				}
				
				// Some debug output about AddIn's manifest
				if ((addIn.Manifest != null) && !String.IsNullOrEmpty(addIn.Manifest.PrimaryIdentity))
				{
					SD.Log.DebugFormatted("[AddInManager2] AddIn's manifest states identity '{0}'",
					                      addIn.Manifest.PrimaryIdentity);
				}
				else
				{
					SD.Log.DebugFormatted("[AddInManager2] AddIn's manifest states no identity.");
				}
				
				if (addIn.Properties.Contains("nuGetPackageID"))
				{
					SD.Log.DebugFormatted("[AddInManager2] AddIn's manifest states NuGet ID '{0}'",
					                      addIn.Properties["nuGetPackageID"]);
				}
				else
				{
					SD.Log.DebugFormatted("[AddInManager2] AddIn's manifest states no NuGet ID.");
				}
				
				// Mark this AddIn
				ManagedAddIn markedAddIn = new ManagedAddIn(addIn)
				{
					InstallationSource = AddInInstallationSource.NuGetRepository,
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
				_events.OnAddInOperationError(
					new AddInOperationErrorEventArgs(
						SD.ResourceService.GetString("AddInManager2.InvalidPackage")));
			}

			return null;
		}
		
		private bool CopyAddInFromZip(AddIn addIn, string zipFile)
		{
			if ((addIn != null) && (addIn.Manifest != null))
			{
				try
				{
					string targetDir = Path.Combine(_sdAddInManagement.TempInstallDirectory,
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
			else
			{
				return false;
			}
		}
		
		private bool CopyAddInFromPackage(AddIn addIn, string packageDirectory)
		{
			try
			{
				string targetDir = Path.Combine(_sdAddInManagement.TempInstallDirectory,
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
			if ((addIn != null) && (addIn.Action == AddInAction.Update))
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
			if ((addIn != null) && (addIn.Manifest != null))
			{
				SD.Log.DebugFormatted("[AddInManager2] Cancelling update for {0}", addIn.Name);
				
				foreach (string identity in addIn.Manifest.Identities.Keys)
				{
					// Delete from installation temp (if installation or update is pending)
					string targetDir = Path.Combine(_sdAddInManagement.TempInstallDirectory, identity);
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
				SD.Log.DebugFormatted("[AddInManager2] Cancelling installation of {0}", addIn.Name);
				
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
			if ((addIn != null) && (addIn.Manifest != null))
			{
				SD.Log.DebugFormatted("[AddInManager2] Cancelling uninstallation of {0}", addIn.Name);
				
				// Abort uninstallation of this AddIn
				foreach (string identity in addIn.Manifest.Identities.Keys)
				{
					_sdAddInManagement.AbortRemoveUserAddInOnNextStart(identity);
				}
				_sdAddInManagement.Enable(new AddIn[] { addIn });
				_events.OnAddInStateChanged(new AddInInstallationEventArgs(addIn));
			}
		}

		public void UninstallAddIn(AddIn addIn)
		{
			if ((addIn != null) && (addIn.Manifest.PrimaryIdentity != null))
			{
				SD.Log.DebugFormatted("[AddInManager2] Uninstalling AddIn {0}", addIn.Name);
				
				List<AddIn> addInList = new List<AddIn>();
				addInList.Add(addIn);
				_sdAddInManagement.RemoveExternalAddIns(addInList);
				
				CancelPendingUpdate(addIn);
				foreach (string identity in addIn.Manifest.Identities.Keys)
				{
					// Remove the user AddIn
					string targetDir = Path.Combine(_sdAddInManagement.UserInstallDirectory, identity);
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
						
						_sdAddInManagement.RemoveUserAddInOnNextStart(identity);
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
					_sdAddInManagement.Disable(new AddIn[] { addIn });
				}
				else
				{
					_sdAddInManagement.Enable(new AddIn[] { addIn });
				}
				
				_events.OnAddInStateChanged(new AddInInstallationEventArgs(addIn));
			}
		}
		
		public AddIn GetInstalledAddInByIdentity(string identity)
		{
			if (!String.IsNullOrEmpty(identity))
			{
				return _sdAddInManagement.AddIns
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
			if (addIn == null)
			{
				// Without a valid AddIn instance we can't do anything...
				return false;
			}
			
			string identity = null;
			if (addIn.Manifest != null)
			{
				identity = addIn.Manifest.PrimaryIdentity;
			}
			return (_sdAddInManagement.AddIns
			        .Where(a => (addIn == a) || ((identity != null) && (a.Manifest != null) && a.Manifest.Identities.ContainsKey(identity)))
			        .FirstOrDefault() != null);
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
			
			if (_nuGet.Packages.LocalRepository == null)
			{
				// Without a local NuGet repository we can't find a package
				return null;
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
			
			AddIn foundAddIn = _sdAddInManagement.AddIns.Where(
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
						SD.Log.InfoFormatted("[AddInManager2] Removing unreferenced NuGet package {0} {1}.",
						                     installedPackage.Id, installedPackage.Version.ToString());
						_nuGet.Packages.UninstallPackage(installedPackage, true, false);
					}
				}
			}
		}
	}
}
