// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Linq;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceUrlHistory
	{
		public const int MaxItems = 10;
		
		const string ServiceReferencePropertyName = "ServiceReference.Urls";
		
		public ServiceReferenceUrlHistory()
		{
			Urls = new List<string>();
			ReadSavedServiceReferenceUrls();
		}
		
		void ReadSavedServiceReferenceUrls()
		{
			Urls.AddRange(PropertyService.Get(ServiceReferencePropertyName, new string[0]));
		}
		
		public List<string> Urls { get; private set; }
		
		public void AddUrl(Uri uri)
		{
			AddUrl(uri.ToString());
		}
		
		public void AddUrl(string url)
		{
			if (Contains(url)) {
				return;
			}
			
			if (Urls.Count >= MaxItems) {
				Urls.RemoveAt(Urls.Count - 1);
			}
			Urls.Insert(0, url);
			PropertyService.Set(ServiceReferencePropertyName, Urls.ToArray());
		}
		
		bool Contains(string url)
		{
			return Urls.Any(item => String.Equals(item, url, StringComparison.OrdinalIgnoreCase));
		}
	}
}
