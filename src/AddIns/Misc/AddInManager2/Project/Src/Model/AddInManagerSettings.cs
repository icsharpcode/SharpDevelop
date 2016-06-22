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
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.AddInManager2.Model
{
	/// <summary>
	/// Handler for managing all persisted settings of AddInManager AddIn.
	/// </summary>
	public class AddInManagerSettings : IAddInManagerSettings
	{
		public string[] PackageRepositories
		{
			get
			{
				return SD.PropertyService.Get<string[]>("AddInManager2.PackageRepositories", null);
			}
			set
			{
				SD.PropertyService.Set<string[]>("AddInManager2.PackageRepositories", value ?? new string[0]);
			}
		}
		
		public bool ShowPreinstalledAddIns
		{
			get
			{
				return SD.PropertyService.Get<bool>("AddInManager2.ShowPreinstalledAddIns", false);
			}
			set
			{
				SD.PropertyService.Set<bool>("AddInManager2.ShowPreinstalledAddIns", value);
			}
		}
		
		public bool ShowPrereleases
		{
			get
			{
				return SD.PropertyService.Get<bool>("AddInManager2.ShowPrereleases", false);
			}
			set
			{
				SD.PropertyService.Set<bool>("AddInManager2.ShowPrereleases", value);
			}
		}
		
		public bool AutoSearchForUpdates
		{
			get
			{
				return SD.PropertyService.Get<bool>("AddInManager2.AutoSearchForUpdates", true);
			}
			set
			{
				SD.PropertyService.Set<bool>("AddInManager2.AutoSearchForUpdates", value);
			}
		}
	}
}
