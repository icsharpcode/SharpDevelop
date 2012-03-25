// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;

namespace ICSharpCode.SharpDevelop.Gui.Dialogs.ReferenceDialog.ServiceReference
{
	public class ServiceReferenceGeneratorOptions
	{
		public ServiceReferenceGeneratorOptions()
		{
			this.OutputFileName = String.Empty;
			this.Namespace = String.Empty;
			this.Language = "CS";
			this.NoAppConfig = true;
		}
		
		public string Namespace { get; set; }
		public string OutputFileName { get; set; }
		public string Url { get; set; }
		public string Language { get; set; }
		public bool NoAppConfig { get; set; }
		
		public void MapProjectLanguage(string language)
		{
			if (language == "VBNet") {
				Language = "VB";
			} else {
				Language = "CS";
			}
		}
	}
}
