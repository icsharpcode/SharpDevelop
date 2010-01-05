using System;
using System.IO;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.style;
using ST = iTextSharp.text.rtf.style;

/*
 * $Id: RtfPhrase.cs,v 1.10 2008/05/16 19:31:24 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002, 2003, 2004 by Mark Hall
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
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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

namespace iTextSharp.text.rtf.text {

    /**
    * The RtfPhrase contains multiple RtfChunks
    * 
    * @version $Id: RtfPhrase.cs,v 1.10 2008/05/16 19:31:24 psoares33 Exp $
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfPhrase : RtfElement {

        /**
        * Constant for the resetting of the paragraph defaults
        */
        public static byte[] PARAGRAPH_DEFAULTS = DocWriter.GetISOBytes("\\pard");
        /**
        * Constant for resetting of font settings to their defaults
        */
        public static byte[] PLAIN = DocWriter.GetISOBytes("\\plain");
        /**
        * Constant for phrase in a table indication
        */
        public static byte[] IN_TABLE = DocWriter.GetISOBytes("\\intbl");
        /**
        * Constant for the line spacing.
        */
        public static byte[] LINE_SPACING = DocWriter.GetISOBytes("\\sl");
        
        /**
        * ArrayList containing the RtfChunks of this RtfPhrase
        */
        protected ArrayList chunks = new ArrayList();
        /**
        * The height of each line.
        */
        private int lineLeading = 0; 
        
        /**
        * A basically empty constructor that is used by the RtfParagraph.
        * 
        * @param doc The RtfDocument this RtfPhrase belongs to.
        */
        protected internal RtfPhrase(RtfDocument doc) : base(doc) {
        }
        
        /**
        * Constructs a new RtfPhrase for the RtfDocument with the given Phrase
        * 
        * @param doc The RtfDocument this RtfPhrase belongs to
        * @param phrase The Phrase this RtfPhrase is based on
        */
        public RtfPhrase(RtfDocument doc, Phrase phrase) : base(doc) {
            
            if (phrase == null) {
                return;
            }
            
            if (phrase.HasLeading()) {
                this.lineLeading = (int) (phrase.Leading * TWIPS_FACTOR);
            } else {
                this.lineLeading = 0;
            }
            
            ST.RtfFont phraseFont = new ST.RtfFont(null, phrase.Font);
            for (int i = 0; i < phrase.Count; i++) {
                IElement chunk = (IElement) phrase[i];
                if (chunk is Chunk) {
                    ((Chunk) chunk).Font = phraseFont.Difference(((Chunk) chunk).Font);
                }
                try {
                    IRtfBasicElement[] rtfElements = doc.GetMapper().MapElement(chunk);
                    for (int j = 0; j < rtfElements.Length; j++) {
                        chunks.Add(rtfElements[j]);
                    }
                } catch (DocumentException) {
                }
            }
        }
        
        /**
        * Write the content of this RtfPhrase. First resets to the paragraph defaults
        * then if the RtfPhrase is in a RtfCell a marker for this is written and finally
        * the RtfChunks of this RtfPhrase are written.
        */    
        public override void WriteContent(Stream result) {
            byte[] t;
            result.Write(PARAGRAPH_DEFAULTS, 0, PARAGRAPH_DEFAULTS.Length);
            result.Write(PLAIN, 0, PLAIN.Length);
            if (inTable) {
                result.Write(IN_TABLE, 0, IN_TABLE.Length);
            }
            if (this.lineLeading > 0) {
                result.Write(LINE_SPACING, 0, LINE_SPACING.Length);
                result.Write(t = IntToByteArray(this.lineLeading), 0, t.Length);
            }
            foreach (IRtfBasicElement rbe in chunks) {
                rbe.WriteContent(result);
            }
        }
        
        /**
        * Sets whether this RtfPhrase is in a table. Sets the correct inTable setting for all
        * child elements.
        * 
        * @param inTable <code>True</code> if this RtfPhrase is in a table, <code>false</code> otherwise
        */
        public override void SetInTable(bool inTable) {
            base.SetInTable(inTable);
            for (int i = 0; i < this.chunks.Count; i++) {
                ((IRtfBasicElement) this.chunks[i]).SetInTable(inTable);
            }
        }
        
        /**
        * Sets whether this RtfPhrase is in a header. Sets the correct inTable setting for all
        * child elements.
        * 
        * @param inHeader <code>True</code> if this RtfPhrase is in a header, <code>false</code> otherwise
        */
        public override void SetInHeader(bool inHeader) {
            base.SetInHeader(inHeader);
            for (int i = 0; i < this.chunks.Count; i++) {
                ((IRtfBasicElement) this.chunks[i]).SetInHeader(inHeader);
            }
        }
        
        /**
        * Sets the RtfDocument this RtfPhrase belongs to. Also sets the RtfDocument for all child
        * elements.
        * 
        * @param doc The RtfDocument to use
        */
        public override void SetRtfDocument(RtfDocument doc) {
            base.SetRtfDocument(doc);
            for (int i = 0; i < this.chunks.Count; i++) {
                ((IRtfBasicElement) this.chunks[i]).SetRtfDocument(this.document);
            }
        }
    }
}