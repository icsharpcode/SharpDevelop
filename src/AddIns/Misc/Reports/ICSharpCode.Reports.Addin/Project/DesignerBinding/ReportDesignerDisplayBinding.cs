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
using System.IO;
using ICSharpCode.Reports.Addin.Commands;
using ICSharpCode.Reports.Addin.ReportWizard;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Workbench;

namespace ICSharpCode.Reports.Addin
{
	/// <summary>
	/// Description of ReportDesignerDisplayBinding.
	/// </summary>
	public class ReportDesignerDisplayBinding:IDisplayBinding
	{
		
		public bool CanCreateContentForFile(ICSharpCode.Core.FileName fileName)
		{
			return Path.GetExtension(fileName).Equals(".srd", StringComparison.OrdinalIgnoreCase);
		}
		
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			if (file.IsDirty) {
				var cmd = new ReportWizardCommand(file);
				cmd.Run();
				if (cmd.Canceled) {
					return null;
				}
				file.SetData(cmd.GeneratedReport.ToArray());
			}
			var viewCmd = new CreateDesignViewCommand(file);
			viewCmd.Run();
			return viewCmd.DesignerView;
		}

		
		public bool IsPreferredBindingForFile(ICSharpCode.Core.FileName fileName)
		{
			return true;
		}
		
		
		public double AutoDetectFileContent(ICSharpCode.Core.FileName fileName, System.IO.Stream fileContent, string detectedMimeType)
		{
			throw new NotImplementedException();
		}
	}
}
