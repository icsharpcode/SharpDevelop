// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
		
		public void AddToTree(AddIn addIn)
		{
			if (addIn != null)
			{
				((AddInTreeImpl)SD.AddInTree).InsertAddIn(addIn);
			}
		}
		
		public void AbortRemoveUserAddInOnNextStart(string identity)
		{
			ICSharpCode.Core.AddInManager.AbortRemoveUserAddInOnNextStart(identity);
		}
		
		public void Enable(IList<AddIn> addIns)
		{
			ICSharpCode.Core.AddInManager.Enable(addIns);
		}
		
		public void Disable(IList<AddIn> addIns)
		{
			ICSharpCode.Core.AddInManager.Disable(addIns);
		}
		
		public void RemoveExternalAddIns(IList<AddIn> addIns)
		{
			ICSharpCode.Core.AddInManager.RemoveExternalAddIns(addIns);
		}
		
		public void RemoveUserAddInOnNextStart(string identity)
		{
			ICSharpCode.Core.AddInManager.RemoveUserAddInOnNextStart(identity);
		}
		
		public AddIn Load(TextReader textReader)
		{
			return AddIn.Load(SD.AddInTree, textReader);
		}
		
		public AddIn Load(string fileName)
		{
			return AddIn.Load(SD.AddInTree, fileName);
		}
	}
}
