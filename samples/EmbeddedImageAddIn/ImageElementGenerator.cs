// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under MIT X11 license (for details please see \doc\license.txt)

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.Core;

namespace EmbeddedImageAddIn
{
	/// <summary>
	/// Implementation of AvalonEdit element generator.
	/// This class looks for image tags and produces ImageElements to visually represent those images.
	/// </summary>
	public class ImageElementGenerator : VisualLineElementGenerator
	{
		readonly static Regex imageRegex = new Regex(@"<<IMAGE:([\w\d\\./,\- :]+)>>");
		
		readonly string baseDirectory;
		
		public ImageElementGenerator(string baseDirectory)
		{
			this.baseDirectory = baseDirectory;
		}
		
		Match GetMatch(int startOffset)
		{
			DocumentLine endLine = CurrentContext.VisualLine.LastDocumentLine;
			string relevantText = CurrentContext.Document.GetText(startOffset, endLine.EndOffset - startOffset);
			return imageRegex.Match(relevantText);
		}
		
		/// Gets the first offset >= startOffset where the generator wants to construct
		/// an element.
		/// Return -1 to signal no interest.
		public override int GetFirstInterestedOffset(int startOffset)
		{
			Match m = GetMatch(startOffset);
			return m.Success ? startOffset + m.Index : -1;
		}
		
		/// Constructs an element at the specified offset.
		/// May return null if no element should be constructed.
		public override VisualLineElement ConstructElement(int offset)
		{
			Match m = GetMatch(offset);
			// check whether there's a match exactly at offset
			if (m.Success && m.Index == 0) {
				string fileName = Path.Combine(baseDirectory, m.Groups[1].Value);
				ImageSource image = ImageCache.GetImage(FileName.Create(fileName));
				if (image != null) {
					// Pass the length of the match to the 'documentLength' parameter of ImageElement.
					return new ImageElement(fileName, image, m.Length);
				}
			}
			return null;
		}
	}
}
