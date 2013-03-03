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
	/// Offers a thin interface to SharpDevelop's AddIn management.
	/// </summary>
	public interface ISDAddInManagement
	{
		IReadOnlyList<AddIn> AddIns
		{
			get;
		}
		string TempInstallDirectory
		{
			get;
		}
		string UserInstallDirectory
		{
			get;
		}
		string ConfigDirectory
		{
			get;
		}
		
		void AddToTree(AddIn addIn);
		void AbortRemoveUserAddInOnNextStart(string identity);
		void Enable(IList<AddIn> addIns);
		void Disable(IList<AddIn> addIns);
		void RemoveExternalAddIns(IList<AddIn> addIns);
		void RemoveUserAddInOnNextStart(string identity);
		AddIn Load(TextReader textReader);
		AddIn Load(string fileName);
		void AddExternalAddIns(IList<AddIn> addIns);
		bool IsAddInManifestInExternalPath(AddIn addIn);
	}
}
