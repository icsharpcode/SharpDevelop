// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Sebastian Ullrich" email=""/>
//     <version>$Revision: 5842 $</version>
// </file>
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
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
		const string url = "ms-xhelp://?method=children&id={3}&format=xml&product={0}&productVersion={1}&locale={2}";
		string id;
		WebClient client = new WebClient();

		public TocEntry(string id)
		{
			this.id = id;

			client.DownloadStringCompleted += (_, e) =>
			{
				LoggingService.Debug(string.Format("Help 3.0: title \"{0}\"", Title));
				var children = XElement.Parse(e.Result);
				Children     = children.Elements("topic")
					.Select(link => new TocEntry(link.Attribute("id").Value) { Title = link.Element("title").Value })
					.ToArray();
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
			get
			{
				Help3Catalog catalog = Help3Service.ActiveCatalog;
				if (children == null && !client.IsBusy)
					client.DownloadStringAsync(new Uri(Help3Environment.GetHttpFromMsXHelp(string.Format(url, catalog.ProductCode, catalog.ProductVersion, catalog.Locale, id))));
				return children ?? defaultChild;
			}
			private set
			{
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
