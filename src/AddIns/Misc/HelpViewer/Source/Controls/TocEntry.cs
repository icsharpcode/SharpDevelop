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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using System.Xml.XPath;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using MSHelpSystem.Core;
using MSHelpSystem.Core.Native;

namespace MSHelpSystem.Controls
{
	class TocEntry : INotifyPropertyChanged
	{
		const string url = "ms-xhelp://?method=children&id={1}&format=xml&{0}";
		string id;
		WebClient client = new WebClient();

		public TocEntry(string id)
		{
			this.id = id;

			client.DownloadStringCompleted += (_, e) =>
			{
				try {
					LoggingService.Debug(string.Format("HelpViewer: TocEntry \"{0}\" found", Title));
					var children = XElement.Parse(e.Result);
					Children     = children.Elements("topic")
						.Select(link => new TocEntry(link.Attribute("id").Value) { Title = WebUtility.HtmlDecode(link.Element("title").Value) })
						.ToArray();
				} catch (TargetInvocationException ex) {
					// Exception when fetching e.Result:
					LoggingService.Error(ex.ToString());
					this.children = defaultChild;
				}
				client.Dispose();
			};
			RaisePropertyChanged("Children");
		}

		public string Title { get; set; }
		public string Id { get { return id; } }

		static object[] defaultChild = new object[] { null };
		IEnumerable children;

		public IEnumerable Children
		{
			get {
				if (Help3Service.ActiveCatalog != null) {
					if (children == null && !client.IsBusy && HelpLibraryAgent.PortIsReady) {
						client.DownloadStringAsync(new Uri(Help3Environment.GetHttpFromMsXHelp(string.Format(url, Help3Service.ActiveCatalog.AsMsXHelpParam, id))));
					}
				}
				return children ?? defaultChild;
			}
			private set {
				children = value;
				RaisePropertyChanged("Children");
			}
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected void RaisePropertyChanged(string name)
		{
			System.ComponentModel.PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null) handler(this, new System.ComponentModel.PropertyChangedEventArgs(name));
		}
  	}
}
