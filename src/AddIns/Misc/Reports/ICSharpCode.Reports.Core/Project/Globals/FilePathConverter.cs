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

namespace ICSharpCode.Reports.Core.Globals
{
	/// <summary>
	/// Description of FileHelper.
	/// </summary>
	public sealed class FilePathConverter
	{
		
		private FilePathConverter()
		{
		}
		
		public static void Absolut2RelativePath (ReportModel model)
		{
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			
			foreach (BaseSection section in model.SectionCollection) {
				foreach (BaseReportItem item in section.Items) {
					BaseImageItem baseImageItem = item as BaseImageItem;
					if (baseImageItem != null) {
						baseImageItem.ReportFileName = model.ReportSettings.FileName;
						
						if (Path.IsPathRooted(baseImageItem.ImageFileName)) {
							string d = FileUtility.GetRelativePath(
								Path.GetDirectoryName(model.ReportSettings.FileName),
								Path.GetDirectoryName(baseImageItem.ImageFileName));

							baseImageItem.RelativeFileName = d + Path.DirectorySeparatorChar + Path.GetFileName(baseImageItem.ImageFileName);
						}
					}
				}
			}
		}
		
		
		public static void AdjustReportName (ReportModel model)
		{
			if (model == null) {
				throw new ArgumentNullException("model");
			}
			foreach (BaseSection section in model.SectionCollection) {
				foreach (BaseReportItem item in section.Items) {
					BaseImageItem baseImageItem = item as BaseImageItem;
					if (baseImageItem != null) {
						baseImageItem.ReportFileName = model.ReportSettings.FileName;
					}
				}
			}
		}
	}
}
