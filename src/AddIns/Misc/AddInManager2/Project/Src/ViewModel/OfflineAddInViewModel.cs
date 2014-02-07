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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using NuGet;

namespace ICSharpCode.AddInManager2.ViewModel
{
	/// <summary>
	/// View model representing an already installed real SharpDevelop AddIn.
	/// </summary>
	public class OfflineAddInsViewModel : AddInPackageViewModelBase
	{
		private AddIn _addIn;
		private ManagedAddIn _markedAddIn;
		
		private string _name;
		private Uri _licenseUrl;
		private Uri _projectUrl;
		private Uri _reportAbuseUrl;
		private IEnumerable<AddInDependency> _dependencies;
		private IEnumerable<string> _authors;
		private bool _hasDownloadCount;
		private string _id;
		private Uri _iconUrl;
		private string _summary;
		private Version _version;
		private Version _oldVersion;
		private int _downloadCount;
		private string _description;
		private DateTime? _lastUpdated;
		
		public OfflineAddInsViewModel(ManagedAddIn addIn)
			: base()
		{
			Initialize(addIn);
		}
		
		public OfflineAddInsViewModel(IAddInManagerServices services, ManagedAddIn addIn)
			: base(services)
		{
			Initialize(addIn);
		}
		
		private void Initialize(ManagedAddIn addIn)
		{
			_markedAddIn = addIn;
			if (_markedAddIn != null)
			{
				_addIn = addIn.AddIn;
			}
			if (_addIn != null)
			{
				UpdateMembers();
			}
		}
		
		public void UpdateMembers()
		{
			if ((_addIn == null) || (_markedAddIn == null))
			{
				return;
			}
			
			_id = _addIn.Manifest.PrimaryIdentity;
			_name = _addIn.Name;
			if (HasNuGetConnection && (_markedAddIn.LinkedNuGetPackageVersion != null))
			{
				_version = new Version(_markedAddIn.LinkedNuGetPackageVersion);
			}
			else
			{
				if (_addIn.Version != null)
				{
					_version = _addIn.Version;
				}
			}
			_description = _addIn.Properties["description"];
			_summary = _addIn.Properties["description"];
			if (!String.IsNullOrEmpty(_addIn.Properties["url"]))
			{
				_projectUrl = new Uri(_addIn.Properties["url"]);
			}
			if (!String.IsNullOrEmpty(_addIn.Properties["license"]))
			{
				_licenseUrl = new Uri(_addIn.Properties["license"]);
			}
			if (!String.IsNullOrEmpty(_addIn.Properties["author"]))
			{
				_authors = new string[] { _addIn.Properties["author"] };
			}
			
			if ((_addIn.Manifest != null) && (_addIn.Manifest.Dependencies != null))
			{
				_dependencies = _addIn.Manifest.Dependencies.Select(d => new AddInDependency(d));
			}
			
			if (_markedAddIn.IsUpdate)
			{
				_oldVersion = _markedAddIn.OldVersion;
			}
			
			_iconUrl = null;
			_hasDownloadCount = false;
			_downloadCount = 0;
			_lastUpdated = null;
			_reportAbuseUrl = null;
		}
		
		public AddIn AddIn
		{
			get
			{
				return _addIn;
			}
		}
		
		public override string Name
		{
			get
			{
				return _name;
			}
		}
		
		public override Uri LicenseUrl
		{
			get
			{
				return _licenseUrl;
			}
		}

		public override Uri ProjectUrl
		{
			get
			{
				return _projectUrl;
			}
		}

		public override Uri ReportAbuseUrl
		{
			get
			{
				return _reportAbuseUrl;
			}
		}
		
		public override bool IsOffline
		{
			get
			{
				return true;
			}
		}
		
		public override bool IsExternallyReferenced
		{
			get
			{
				if (_addIn != null)
				{
					// The AddIn is externally referenced, if it's .addin file doesn't reside in
					// somewhere in application path (preinstalled AddIns) or in config path (usually installed AddIns).
					return AddInManager.SDAddInManagement.IsAddInManifestInExternalPath(_addIn);
				}
				else
				{
					return false;
				}
			}
		}
		
		public override bool IsPreinstalled
		{
			get
			{
				if (_addIn != null)
				{
					return AddInManager.Setup.IsAddInPreinstalled(_addIn);
				}
				else
				{
					return false;
				}
			}
		}

		public override bool IsAdded
		{
			get
			{
				if (_addIn != null)
				{
//					return (_addIn.Action == AddInAction.Install) || (_addIn.Action == AddInAction.Update);
					return _markedAddIn.IsTemporary;
				}
				else
				{
					return false;
				}
			}
		}
		
		public override bool IsUpdate
		{
			get
			{
				return _markedAddIn.IsUpdate;
			}
		}
		
		public override bool IsInstalled
		{
			get
			{
				if (_addIn != null)
				{
					return AddInManager.Setup.IsAddInInstalled(_addIn);
				}
				else
				{
					return false;
				}
			}
		}
		
		public override bool IsInstallable
		{
			get
			{
				return false;
			}
		}
		
		public override bool IsUninstallable
		{
			get
			{
				return !IsPreinstalled && (Id != null);
			}
		}
		
		public override bool IsDisablingPossible
		{
			get
			{
				// Disabling is only possible if this AddIn has an identity and is not the AddInManager itself
				return (Id != null) && (_addIn.Manifest.PrimaryIdentity != "ICSharpCode.AddInManager2");
			}
		}
		
		public override bool IsEnabled
		{
			get
			{
				if (_addIn != null)
				{
					return (_addIn.Action != AddInAction.Disable);
				}
				else
				{
					return false;
				}
			}
		}
		
		public override bool IsRemoved
		{
			get
			{
				if (_addIn != null)
				{
					return !_markedAddIn.IsTemporary && (_addIn.Action == AddInAction.Uninstall);
				}
				else
				{
					return false;
				}
			}
		}
		
		public override IEnumerable<AddInDependency> Dependencies
		{
			get
			{
				return _dependencies;
			}
		}

		public override IEnumerable<string> Authors
		{
			get
			{
				return _authors;
			}
		}

		public override bool HasDownloadCount
		{
			get
			{
				return _hasDownloadCount;
			}
		}

		public override string Id
		{
			get
			{
				return _id;
			}
		}

		public override Uri IconUrl
		{
			get
			{
				return _iconUrl;
			}
		}

		public override string Summary
		{
			get
			{
				if (_addIn != null)
				{
					if (_addIn.Action == AddInAction.Install)
					{
						return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInInstalled"));
					}
					else if (_addIn.Action == AddInAction.Update)
					{
						return SD.ResourceService.GetString("AddInManager.AddInUpdated");
					}
					else if (HasDependencyConflicts)
					{
						return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInDependencyFailed"));
					}
					else if (IsRemoved)
					{
						return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInRemoved"));
					}
					else if (IsEnabled && !_addIn.Enabled)
					{
						return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInEnabled"));
					}
					else if (!IsEnabled)
					{
						if (_addIn.Enabled)
						{
							return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInWillBeDisabled"));
						}
						else
						{
							return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInDisabled"));
						}
					}
					else if (_addIn.Action == AddInAction.InstalledTwice)
					{
						return SurroundWithParantheses(SD.ResourceService.GetString("AddInManager.AddInInstalledTwice"));
					}
					else
					{
						return _summary;
					}
				}
				else
				{
					return null;
				}
			}
		}

		public override Version Version
		{
			get
			{
				return _version;
			}
		}
		
		public override Version OldVersion
		{
			get
			{
				return _oldVersion;
			}
		}
		
		public override bool ShowSplittedVersions
		{
			get
			{
				return IsUpdate;
			}
		}

		public override int DownloadCount
		{
			get
			{
				return _downloadCount;
			}
		}

		public override string Description
		{
			get
			{
				return _description;
			}
		}

		public override DateTime? LastUpdated
		{
			get
			{
				return _lastUpdated;
			}
		}
		
		public override string FileName
		{
			get
			{
				if (_addIn != null)
				{
					return FileUtility.NormalizePath(_addIn.FileName);
				}
				else
				{
					return null;
				}
			}
		}
		
		public override bool HasDependencyConflicts
		{
			get
			{
				if (_addIn != null)
				{
					return (_addIn.Action == AddInAction.DependencyError);
				}
				else
				{
					return false;
				}
			}
		}
		
		public override bool HasNuGetConnection
		{
			get
			{
				if (_markedAddIn != null)
				{
					return (_markedAddIn.InstallationSource == AddInInstallationSource.NuGetRepository);
				}
				else
				{
					return false;
				}
			}
		}

		public override void AddPackage()
		{
		}

		public override void RemovePackage()
		{
			ClearReportedMessages();
			
			if (_addIn.Manifest.PrimaryIdentity == "ICSharpCode.AddInManager2")
			{
				MessageService.ShowMessage("${res:AddInManager2.CannotRemoveAddInManager}", "${res:AddInManager.Title}");
				return;
			}
			
			if (!this.IsRemoved)
			{
				var dependentAddIns = AddInManager.Setup.GetDependentAddIns(_addIn);
				if ((dependentAddIns != null) && dependentAddIns.Any())
				{
					string addInNames = "";
					foreach (var dependentAddIn in dependentAddIns)
					{
						addInNames += "\t " + dependentAddIn.AddIn.Name + Environment.NewLine;
					}
					if (!MessageService.AskQuestionFormatted(
						"${res:AddInManager.Title}", "${res:AddInManager2.DisableDependentWarning}", _addIn.Name, addInNames))
					{
						return;
					}
				}
			}
			
			AddInManager.Setup.UninstallAddIn(_addIn);
		}
		
		public override void CancelInstallation()
		{
			ClearReportedMessages();
			
			AddInManager.Setup.CancelInstallation(_addIn);
		}
		
		public override void CancelUpdate()
		{
			ClearReportedMessages();
			
			AddInManager.Setup.CancelUpdate(_addIn);
		}
		
		public override void CancelUninstallation()
		{
			ClearReportedMessages();
			
			AddInManager.Setup.CancelUninstallation(_addIn);
		}

		public override void DisablePackage()
		{
			ClearReportedMessages();
			
			if (_addIn == null)
			{
				return;
			}
			if (_addIn.Manifest.PrimaryIdentity == "ICSharpCode.AddInManager2")
			{
				MessageService.ShowMessage("${res:AddInManager.CannotDisableAddInManager}", "${res:AddInManager.Title}");
				return;
			}
			
			if (this.IsEnabled)
			{
				var dependentAddIns = AddInManager.Setup.GetDependentAddIns(_addIn);
				if ((dependentAddIns != null) && dependentAddIns.Any())
				{
					string addInNames = "";
					foreach (var dependentAddIn in dependentAddIns)
					{
						addInNames += "\t " + dependentAddIn.AddIn.Name + Environment.NewLine;
					}
					if (!MessageService.AskQuestionFormatted(
						"${res:AddInManager.Title}", "${res:AddInManager2.DisableDependentWarning}", _addIn.Name, addInNames))
					{
						return;
					}
				}
			}
			
			AddInManager.Setup.SwitchAddInActivation(_addIn);
		}
		
		public override bool HasOptions
		{
			get
			{
				if (_addIn.Enabled)
				{
					foreach (KeyValuePair<string, ExtensionPath> pair in _addIn.Paths)
					{
						if (pair.Key.StartsWith("/SharpDevelop/Dialogs/OptionsDialog"))
						{
							return true;
						}
					}
				}
				return false;
			}
		}
		
		public override void ShowOptions()
		{
			ClearReportedMessages();
			
			AddInTreeNode dummyNode = new AddInTreeNode();
			foreach (KeyValuePair<string, ExtensionPath> pair in _addIn.Paths)
			{
				if (pair.Key.StartsWith("/SharpDevelop/Dialogs/OptionsDialog"))
				{
					dummyNode.AddCodons(pair.Value.Codons);
				}
			}
			ICSharpCode.SharpDevelop.Commands.OptionsCommand.ShowTabbedOptions(
				_addIn.Name + " " + SD.ResourceService.GetString("AddInManager.Options"),
				dummyNode);
		}
		
		private void ClearReportedMessages()
		{
			// Notify about new operation
			AddInManager.Events.OnOperationStarted();
		}
	}
}
