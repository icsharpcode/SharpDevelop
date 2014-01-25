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
using System.IO;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Implementation of a thin interface to SharpDevelop's AddIn management.
	/// </summary>
	public class SDAddInManagement : ISDAddInManagement
	{
		public SDAddInManagement()
		{
		}
		
		public IReadOnlyList<AddIn> AddIns
		{
			get
			{
				return SD.AddInTree.AddIns;
			}
		}
		
		public string TempInstallDirectory
		{
			get
			{
				return ICSharpCode.Core.AddInManager.AddInInstallTemp;
			}
		}
		
		public string UserInstallDirectory
		{
			get
			{
				return ICSharpCode.Core.AddInManager.UserAddInPath;
			}
		}
		
		public string ConfigDirectory
		{
			get
			{
				return SD.PropertyService.ConfigDirectory;
			}
		}
		
		public void AddToTree(AddIn addIn)
		{
			if (addIn != null)
			{
				SD.Log.DebugFormatted(
					"[AddInManager2.SD] Added {0} AddIn {1} to tree.", ((addIn.Action == AddInAction.Update) ? "updated" : "new"), addIn.Name);
				
				((AddInTreeImpl)SD.AddInTree).InsertAddIn(addIn);
			}
		}
		
		public void AbortRemoveUserAddInOnNextStart(string identity)
		{
			SD.Log.DebugFormatted(
				"[AddInManager2.SD] Aborting removal of AddIn {0}", identity);
			
			ICSharpCode.Core.AddInManager.AbortRemoveUserAddInOnNextStart(identity);
		}
		
		public void Enable(IList<AddIn> addIns)
		{
			SD.Log.DebugFormatted(
				"[AddInManager2.SD] Enabling AddIn {0}", addIns[0].Name);
			
			ICSharpCode.Core.AddInManager.Enable(addIns);
		}
		
		public void Disable(IList<AddIn> addIns)
		{
			SD.Log.DebugFormatted(
				"[AddInManager2.SD] Disabling AddIn {0}", addIns[0].Name);
			
			ICSharpCode.Core.AddInManager.Disable(addIns);
		}
		
		public void RemoveExternalAddIns(IList<AddIn> addIns)
		{
			SD.Log.DebugFormatted(
				"[AddInManager2.SD] Removing external AddIn {0}", addIns[0]);
			
			ICSharpCode.Core.AddInManager.RemoveExternalAddIns(addIns);
		}
		
		public void RemoveUserAddInOnNextStart(string identity)
		{
			SD.Log.DebugFormatted(
				"[AddInManager2.SD] Marking AddIn {0} for removal", identity);
			
			ICSharpCode.Core.AddInManager.RemoveUserAddInOnNextStart(identity);
		}
		
		public AddIn Load(TextReader textReader)
		{
			return AddIn.Load(SD.AddInTree, textReader);
		}
		
		public AddIn Load(string fileName)
		{
			SD.Log.DebugFormatted(
				"[AddInManager2.SD] Loading manifest from '{0}'", fileName);
			
			return AddIn.Load(SD.AddInTree, fileName);
		}
		
		public void AddExternalAddIns(IList<AddIn> addIns)
		{
			SD.Log.DebugFormatted(
				"[AddInManager2.SD] Adding external AddIn {0}", addIns[0].Name);
			
			ICSharpCode.Core.AddInManager.AddExternalAddIns(addIns);
		}
		
		public bool IsAddInManifestInExternalPath(AddIn addIn)
		{
			if (addIn == null)
			{
				// Without a valid AddIn simply return false... really ok?
				return false;
			}
			
			return !FileUtility.IsBaseDirectory(FileUtility.ApplicationRootPath, addIn.FileName)
				&& !FileUtility.IsBaseDirectory(SD.PropertyService.ConfigDirectory, addIn.FileName);
		}
	}
}
