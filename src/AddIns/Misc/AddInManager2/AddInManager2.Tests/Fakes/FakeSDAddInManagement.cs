// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using ICSharpCode.AddInManager2.Model;
using ICSharpCode.Core;

namespace ICSharpCode.AddInManager2.Tests.Fakes
{
	public class FakeSDAddInManagement : ISDAddInManagement
	{
		private List<AddIn> _registeredAddIns;
		private List<string> _addInsMarkedForRemoval;
		private List<AddIn> _addedExternalAddIns;
		private List<AddIn> _removedExternalAddIns;
		
		public FakeSDAddInManagement()
		{
			_registeredAddIns = new List<AddIn>();
			_addInsMarkedForRemoval = new List<string>();
			_addedExternalAddIns = new List<AddIn>();
			_removedExternalAddIns = new List<AddIn>();
		}
		
		public List<AddIn> RegisteredAddIns
		{
			get
			{
				return _registeredAddIns;
			}
		}
		
		public List<string> AddInsMarkedForRemoval
		{
			get
			{
				return _addInsMarkedForRemoval;
			}
		}
		
		public List<AddIn> AddedExternalAddIns
		{
			get
			{
				return _addedExternalAddIns;
			}
		}
		
		public List<AddIn> RemovedExternalAddIns
		{
			get
			{
				return _removedExternalAddIns;
			}
		}
		
		public AddIn AddInToLoad
		{
			get;
			set;
		}
		
		public System.Collections.Generic.IReadOnlyList<ICSharpCode.Core.AddIn> AddIns
		{
			get
			{
				return _registeredAddIns.AsReadOnly();
			}
		}
		
		public string TempInstallDirectory
		{
			get;
			set;
		}
		
		public string UserInstallDirectory
		{
			get;
			set;
		}
		
		public string ConfigDirectory
		{
			get;
			set;
		}
		
		public void AddToTree(ICSharpCode.Core.AddIn addIn)
		{
			if (addIn != null)
			{
				_registeredAddIns.Add(addIn);
			}
		}
		
		public void AbortRemoveUserAddInOnNextStart(string identity)
		{
			if (identity != null)
			{
				_addInsMarkedForRemoval.Remove(identity);
			}
		}
		
		public void Enable(System.Collections.Generic.IList<ICSharpCode.Core.AddIn> addIns)
		{
			if (addIns != null)
			{
				foreach (var addIn in addIns)
				{
					addIn.Enabled = true;
				}
			}
		}
		
		public void Disable(System.Collections.Generic.IList<ICSharpCode.Core.AddIn> addIns)
		{
			if (addIns != null)
			{
				foreach (var addIn in addIns)
				{
					addIn.Enabled = false;
				}
			}
		}
		
		public void RemoveExternalAddIns(System.Collections.Generic.IList<ICSharpCode.Core.AddIn> addIns)
		{
			if (addIns != null)
			{
				_removedExternalAddIns.AddRange(addIns);
				foreach (var removedAddIn in addIns)
				{
					_registeredAddIns.Remove(removedAddIn);
					removedAddIn.Action = AddInAction.Uninstall;
				}
			}
		}
		
		public void RemoveUserAddInOnNextStart(string identity)
		{
			if (identity != null)
			{
				_addInsMarkedForRemoval.Add(identity);
			}
		}
		
		public ICSharpCode.Core.AddIn Load(System.IO.TextReader textReader)
		{
			return AddInToLoad;
		}
		
		public ICSharpCode.Core.AddIn Load(string fileName)
		{
			return AddInToLoad;
		}
		
		public void AddExternalAddIns(System.Collections.Generic.IList<ICSharpCode.Core.AddIn> addIns)
		{
			if (addIns != null)
			{
				_addedExternalAddIns.AddRange(addIns);
				_registeredAddIns.AddRange(addIns);
			}
		}
		
		public bool IsAddInManifestInExternalPath(AddIn addIn)
		{
			if (IsAddInManifestInExternalPathCallback != null)
			{
				return IsAddInManifestInExternalPathCallback(addIn);
			}
			else
			{
				return false;
			}
		}
		
		public Func<AddIn, bool> IsAddInManifestInExternalPathCallback
		{
			get;
			set;
		}
	}
}
