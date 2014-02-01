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
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using ICSharpCode.Core;

namespace Hornung.ResourceToolkit.ResourceFileContent
{
	/// <summary>
	/// Finds localized resources that follow the standard .NET pattern
	/// MyResources.(culture).(extension).
	/// </summary>
	public class DefaultFileLocalizedResourcesFinder : ILocalizedResourcesFinder
	{
		/// <summary>
		/// Gets localized resources that belong to the master resource file.
		/// </summary>
		/// <param name="fileName">The name of the master resource file.</param>
		/// <returns>A dictionary of culture names and associated resource file contents, or <c>null</c>, if there are none.</returns>
		public IDictionary<string, IResourceFileContent> GetLocalizedContents(string fileName)
		{
			#if DEBUG
			LoggingService.Debug("ResourceToolkit: DefaultFileLocalizedResourcesFinder.GetLocalizedContents called, fileName: '"+fileName+"'");
			#endif
			
			string fileNameWithoutExtension = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
			string culture = Path.GetExtension(fileNameWithoutExtension);
			if (!String.IsNullOrEmpty(culture)) {
				try {
					CultureInfo.GetCultureInfo(culture);
					// the specified file is a localized resource file itself
					LoggingService.Debug("ResourceToolkit: DefaultFileLocalizedResourcesFinder.GetLocalizedContents: Returning null for file '"+fileName+"' because it has been detected as being a localized resource file itself.");
					return null;
				} catch (ArgumentException) {
				}
			}
			
			return FindLocalizedResourceFiles(fileNameWithoutExtension, Path.GetExtension(fileName));
		}
		
		/// <summary>
		/// Finds all localized resource files that follow the pattern
		/// &lt;<paramref name="fileNameWithoutExtension"/>&gt;.&lt;culture&gt;&lt;<paramref name="extension"/>&gt;.
		/// </summary>
		/// <param name="fileNameWithoutExtension">The full path and name of the master resource file without extension.</param>
		/// <param name="extension">The extension of the master resource file (with leading dot).</param>
		/// <returns>A dictionary of culture names and associated resource file contents.</returns>
		public static IDictionary<string, IResourceFileContent> FindLocalizedResourceFiles(string fileNameWithoutExtension, string extension)
		{
			Dictionary<string, IResourceFileContent> list = new Dictionary<string, IResourceFileContent>();
			
			#if DEBUG
			LoggingService.Debug("ResourceToolkit: Finding localized resource files (DefaultFileLocalizedResourcesFinder.FindLocalizedResourceFiles).");
			LoggingService.Debug(" -> fileNameWithoutExtension: '"+fileNameWithoutExtension+"'");
			LoggingService.Debug(" -> extension:                '"+extension+"'");
			#endif
			
			foreach (string fileName in Directory.GetFiles(Path.GetDirectoryName(fileNameWithoutExtension), Path.GetFileName(fileNameWithoutExtension)+".*"+extension)) {
				#if DEBUG
				LoggingService.Debug(" -> possible file: '"+fileName+"'");
				#endif
				
				string culture = Path.GetExtension(Path.GetFileNameWithoutExtension(fileName));
				#if DEBUG
				LoggingService.Debug("    -> culture = '"+(culture ?? "<null>")+"'");
				#endif
				
				if (!String.IsNullOrEmpty(culture)) {
					
					try {
						
						CultureInfo ci = CultureInfo.GetCultureInfo(culture.Substring(1));	// need to remove leading dot from culture
						IResourceFileContent content = ResourceFileContentRegistry.GetResourceFileContent(fileName);
						if (content != null) {
							#if DEBUG
							LoggingService.Debug("    -> culture name: '"+ci.Name+"'");
							#endif
							list.Add(ci.Name, content);
						}
						
					} catch (ArgumentException) {
						continue;
					}
					
				}
				
			}
			
			return list;
		}
		
		// ********************************************************************************************************************************
		
		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultFileLocalizedResourcesFinder"/> class.
		/// </summary>
		public DefaultFileLocalizedResourcesFinder()
		{
		}
	}
}
