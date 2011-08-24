// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

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
