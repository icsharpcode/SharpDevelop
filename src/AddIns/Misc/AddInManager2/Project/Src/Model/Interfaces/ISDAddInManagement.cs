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
