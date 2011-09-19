// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;

namespace ICSharpCode.FormsDesigner.Services
{
	sealed class DesignerResourceService : System.ComponentModel.Design.IResourceService
	{
		readonly IResourceStore store;
		FormsDesignerManager host;
		
		public DesignerResourceService(IResourceStore store, FormsDesignerManager host)
		{
			if (store == null)
				throw new ArgumentNullException("store");
			this.store = store;
			this.host = host;
		}
		
		#region System.ComponentModel.Design.IResourceService interface implementation
		public System.Resources.IResourceWriter GetResourceWriter(CultureInfo info)
		{
			try {
				host.LoggingService.Debug("ResourceWriter requested for culture: " + info.ToString());
				ResourceType type;
				Stream stream = store.GetResourceAsStreamForWriting(info, out type);
				return CreateResourceWriter(stream, type);
			} catch (Exception e) {
				host.MessageService.ShowException(e, "");
				return null;
			}
		}
		
		public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info)
		{
			try {
				host.LoggingService.Debug("ResourceReader requested for culture: "+info.ToString());
				ResourceType type;
				Stream stream = store.GetResourceAsStreamForReading(info, out type);
				return CreateResourceReader(stream, type);
			} catch (Exception e) {
				host.MessageService.ShowException(e, "");
				return null;
			}
		}
		#endregion
		
		internal static IResourceReader CreateResourceReader(Stream stream, ResourceType type)
		{
			Debug.Assert(DesignerAppDomainManager.IsDesignerDomain, "not in Designer AppDomain!");
			if (stream.Length == 0)
				return null;
			if (type == ResourceType.Resources) {
				return new ResourceReader(stream);
			}
			return new ResXResourceReader(stream);
		}
		
		internal static IResourceWriter CreateResourceWriter(Stream stream, ResourceType type)
		{
			if (type == ResourceType.Resources) {
				return new ResourceWriter(stream);
			}
			return new ResXResourceWriter(stream);
		}
	}
}
