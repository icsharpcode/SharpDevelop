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
