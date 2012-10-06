// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Implements content auto detection and opens the appropriate IViewContent.
	/// </summary>
	sealed class AutoDetectDisplayBinding : IDisplayBinding
	{
		public bool IsPreferredBindingForFile(FileName fileName)
		{
			return false;
		}
		
		public bool CanCreateContentForFile(FileName fileName)
		{
			return true;
		}
		
		public double AutoDetectFileContent(FileName fileName, Stream fileContent, string detectedMimeType)
		{
			return double.NegativeInfinity;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			var codons = SD.DisplayBindingService.GetCodonsPerFileName(file.FileName);
			DisplayBindingDescriptor bestMatch = null;
			double max = double.NegativeInfinity;
			const int BUFFER_LENGTH = 4 * 1024;
			
			using (var stream = file.OpenRead()) {
				string mime = "text/plain";
				if (stream.Length > 0) {
					stream.Position = 0;
					mime = MimeTypeDetection.FindMimeType(new BinaryReader(stream).ReadBytes(BUFFER_LENGTH));
				}
				foreach (var codon in codons) {
					stream.Position = 0;
					double value = codon.Binding.AutoDetectFileContent(file.FileName, stream, mime);
					if (value > max) {
						max = value;
						bestMatch = codon;
					}
				}
			}
			
			if (bestMatch == null)
				throw new InvalidOperationException();
			
			return bestMatch.Binding.CreateContentForFile(file);
		}
	}
}
