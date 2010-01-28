/*
 * Erstellt mit SharpDevelop.
 * Benutzer: Forstmeier
 * Datum: 25.03.2007
 * Zeit: 16:59
 * 
 * Sie können diese Vorlage unter Extras > Optionen > Codeerstellung > Standardheader ändern.
 */

using System;
using System.IO;

namespace ICSharpCode.Reports.Core
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
			System.Diagnostics.Trace.WriteLine("");
			System.Diagnostics.Trace.WriteLine (String.Format("FilePathConverter:AdjustReportName {0}",model.ReportSettings.FileName));
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
