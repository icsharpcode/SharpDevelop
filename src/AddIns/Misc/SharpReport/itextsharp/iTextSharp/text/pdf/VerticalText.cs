using System;
using System.Collections;

using iTextSharp.text;

/*
 *
 * Copyright 2002 by Paulo Soares.
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code is 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000, 2001, 2002 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), in which case the
 * provisions of LGPL are applicable instead of those above.  If you wish to
 * allow use of your version of this file only under the terms of the LGPL
 * License and not to allow others to use your version of this file under
 * the MPL, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the LGPL.
 * If you do not delete the provisions above, a recipient may use your version
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE.
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */
namespace iTextSharp.text.pdf {

	/** Writes text vertically. Note that the naming is done according
	 * to horizontal text although it referrs to vertical text.
	 * A line with the alignment Element.LEFT_ALIGN will actually
	 * be top aligned.
	 */
	public class VerticalText {

		/** Signals that there are no more text available. */    
		public static int NO_MORE_TEXT = 1;
	
		/** Signals that there is no more column. */    
		public static int NO_MORE_COLUMN = 2;

		/** The chunks that form the text. */    
		protected ArrayList chunks = new ArrayList();

		/** The <CODE>PdfContent</CODE> where the text will be written to. */    
		protected PdfContentByte text;
    
		/** The column Element. Default is left Element. */
		protected int alignment = Element.ALIGN_LEFT;

		/** Marks the chunks to be eliminated when the line is written. */
		protected int currentChunkMarker = -1;
    
		/** The chunk created by the splitting. */
		protected PdfChunk currentStandbyChunk;
    
		/** The chunk created by the splitting. */
		protected string splittedChunkText;

		/** The leading
		 */    
		protected float leading;
    
		/** The X coordinate.
		 */    
		protected float startX;
    
		/** The Y coordinate.
		 */    
		protected float startY;
    
		/** The maximum number of vertical lines.
		 */    
		protected int maxLines;
    
		/** The height of the text.
		 */    
		protected float height;
    
		/** Creates new VerticalText
		 * @param text the place where the text will be written to. Can
		 * be a template.
		 */
		public VerticalText(PdfContentByte text) {
			this.text = text;
		}
    
		/**
		 * Adds a <CODE>Phrase</CODE> to the current text array.
		 * @param phrase the text
		 */
		public void AddText(Phrase phrase) {
			foreach(Chunk c in phrase.Chunks) {
				chunks.Add(new PdfChunk(c, null));
			}
		}
    
		/**
		 * Adds a <CODE>Chunk</CODE> to the current text array.
		 * @param chunk the text
		 */
		public void AddText(Chunk chunk) {
			chunks.Add(new PdfChunk(chunk, null));
		}

		/** Sets the layout.
		 * @param startX the top right X line position
		 * @param startY the top right Y line position
		 * @param height the height of the lines
		 * @param maxLines the maximum number of lines
		 * @param leading the separation between the lines
		 */    
		public void SetVerticalLayout(float startX, float startY, float height, int maxLines, float leading) {
			this.startX = startX;
			this.startY = startY;
			this.height = height;
			this.maxLines = maxLines;
			Leading = leading;
		}
    
		/** Gets the separation between the vertical lines.
		 * @return the vertical line separation
		 */    
		public float Leading {
			get {
				return leading;
			}

			set {
				this.leading = value;
			}
		}
    
		/**
		 * Creates a line from the chunk array.
		 * @param width the width of the line
		 * @return the line or null if no more chunks
		 */
		protected PdfLine CreateLine(float width) {
			if (chunks.Count == 0)
				return null;
			splittedChunkText = null;
			currentStandbyChunk = null;
			PdfLine line = new PdfLine(0, width, alignment, 0);
			string total;
			for (currentChunkMarker = 0; currentChunkMarker < chunks.Count; ++currentChunkMarker) {
				PdfChunk original = (PdfChunk)(chunks[currentChunkMarker]);
				total = original.ToString();
				currentStandbyChunk = line.Add(original);
				if (currentStandbyChunk != null) {
					splittedChunkText = original.ToString();
					original.Value = total;
					return line;
				}
			}
			return line;
		}
    
		/**
		 * Normalizes the list of chunks when the line is accepted.
		 */
		protected void ShortenChunkArray() {
			if (currentChunkMarker < 0)
				return;
			if (currentChunkMarker >= chunks.Count) {
				chunks.Clear();
				return;
			}
			PdfChunk split = (PdfChunk)(chunks[currentChunkMarker]);
			split.Value = splittedChunkText;
			chunks[currentChunkMarker] = currentStandbyChunk;
			for (int j = currentChunkMarker - 1; j >= 0; --j)
				chunks.RemoveAt(j);
		}

		/**
		 * Outputs the lines to the document. It is equivalent to <CODE>go(false)</CODE>.
		 * @return returns the result of the operation. It can be <CODE>NO_MORE_TEXT</CODE>
		 * and/or <CODE>NO_MORE_COLUMN</CODE>
		 * @throws DocumentException on error
		 */
		public int Go() {
			return Go(false);
		}
    
		/**
		 * Outputs the lines to the document. The output can be simulated.
		 * @param simulate <CODE>true</CODE> to simulate the writting to the document
		 * @return returns the result of the operation. It can be <CODE>NO_MORE_TEXT</CODE>
		 * and/or <CODE>NO_MORE_COLUMN</CODE>
		 * @throws DocumentException on error
		 */
		public int Go(bool simulate) {
			bool dirty = false;
			PdfContentByte graphics = null;
			if (text != null) {
				graphics = text.Duplicate;
			}
			else if (!simulate)
				throw new Exception("VerticalText.go with simulate==false and text==null.");
			int status = 0;
			for (;;) {
				if (maxLines <= 0) {
					status = NO_MORE_COLUMN;
					if (chunks.Count == 0)
						status |= NO_MORE_TEXT;
					break;
				}
				if (chunks.Count == 0) {
					status = NO_MORE_TEXT;
					break;
				}
				PdfLine line = CreateLine(height);
				if (!simulate && !dirty) {
					text.BeginText();
					dirty = true;
				}
				ShortenChunkArray();
				if (!simulate) {
					text.SetTextMatrix(startX, startY - line.IndentLeft);
					WriteLine(line, text, graphics);
				}
				--maxLines;
				startX -= leading;
			}
			if (dirty) {
				text.EndText();
				text.Add(graphics);
			}
			return status;
		}
    
		internal void WriteLine(PdfLine line, PdfContentByte text, PdfContentByte graphics)  {
			PdfFont currentFont = null;
			foreach(PdfChunk chunk in line) {
				if (chunk.Font.CompareTo(currentFont) != 0) {
					currentFont = chunk.Font;
					text.SetFontAndSize(currentFont.Font, currentFont.Size);
				}
				Color color = chunk.Color;
				if (color != null)
					text.SetColorFill(color);
				text.ShowText(chunk.ToString());
				if (color != null)
					text.ResetRGBColorFill();
			}
		}
    
		/** Sets the new text origin.
		 * @param startX the X coordinate
		 * @param startY the Y coordinate
		 */    
		public void SetOrigin(float startX, float startY) {
			this.startX = startX;
			this.startY = startY;
		}
    
		/** Gets the X coordinate where the next line will be writen. This value will change
		 * after each call to <code>go()</code>.
		 * @return  the X coordinate
		 */    
		public float OriginX {
			get {
				return startX;
			}
		}

		/** Gets the Y coordinate where the next line will be writen.
		 * @return  the Y coordinate
		 */    
		public float OriginY {
			get {
				return startY;
			}
		}
    
		/** Gets the maximum number of available lines. This value will change
		 * after each call to <code>go()</code>.
		 * @return Value of property maxLines.
		 */
		public int MaxLines {
			get {
				return maxLines;
			}

			set {
				this.maxLines = value;
			}
		}
    
		/** Gets the height of the line
		 * @return the height
		 */
		public float Height {
			get {
				return height;
			}

			set {
				this.height = value;
			}
		}
    
		/**
		 * Gets the Element.
		 * @return the alignment
		 */
		public int Alignment {
			get {
				return alignment;
			}

			set {
				this.alignment = value;
			}
		}
	}
}