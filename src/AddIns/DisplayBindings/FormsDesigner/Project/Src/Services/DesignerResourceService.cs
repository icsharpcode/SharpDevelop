// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Globalization;
using ICSharpCode.Core;

namespace ICSharpCode.FormsDesigner.Services
{
	sealed class DesignerResourceService : System.ComponentModel.Design.IResourceService
	{
		readonly ResourceStore store;
		
		public DesignerResourceService(ResourceStore store)
		{
			if (store == null)
				throw new ArgumentNullException("store");
			this.store = store;
		}
		
		#region System.ComponentModel.Design.IResourceService interface implementation
		public System.Resources.IResourceWriter GetResourceWriter(CultureInfo info)
		{
			try {
				LoggingService.Debug("ResourceWriter requested for culture: " + info.ToString());
				return this.store.GetWriter(info);
			} catch (Exception e) {
				MessageService.ShowException(e);
				return null;
			}
		}
		
		public System.Resources.IResourceReader GetResourceReader(System.Globalization.CultureInfo info)
		{
			try {
				LoggingService.Debug("ResourceReader requested for culture: "+info.ToString());
				return this.store.GetReader(info);
			} catch (Exception e) {
				MessageService.ShowException(e);
				return null;
			}
		}
		#endregion
	}
}
