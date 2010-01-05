using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.style;
using ST = iTextSharp.text.rtf.style;
/*
 * $Id: RtfChunk.cs,v 1.7 2008/05/16 19:31:24 psoares33 Exp $
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
    * The RtfChunk contains one piece of text. The smallest text element available
    * in iText.
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfChunk : RtfElement {

        /**
        * Constant for the subscript flag
        */
        private static byte[] FONT_SUBSCRIPT = DocWriter.GetISOBytes("\\sub");
        /**
        * Constant for the superscript flag
        */
        private static byte[] FONT_SUPERSCRIPT = DocWriter.GetISOBytes("\\super");
        /**
        * Constant for the end of sub / superscript flag
        */
        private static byte[] FONT_END_SUPER_SUBSCRIPT = DocWriter.GetISOBytes("\\nosupersub");
        /**
        * Constant for background colour.
        */
        private static byte[] BACKGROUND_COLOR = DocWriter.GetISOBytes("\\chcbpat");

        /**
        * The font of this RtfChunk
        */
        private ST.RtfFont font = null;
        /**
        * The actual content of this RtfChunk
        */
        private String content = "";
        /**
        * Whether to use soft line breaks instead of hard ones.
        */
        private bool softLineBreaks = false;
        /**
        * The super / subscript of this RtfChunk
        */
        private float superSubScript = 0;
        /**
        * An optional background colour.
        */
        private RtfColor background = null;

        /**
        * Constructs a RtfChunk based on the content of a Chunk
        * 
        * @param doc The RtfDocument that this Chunk belongs to
        * @param chunk The Chunk that this RtfChunk is based on
        */
        public RtfChunk(RtfDocument doc, Chunk chunk) : base(doc) {
            
            if (chunk == null) {
                return;
            }
            
            if (chunk.Attributes != null && chunk.Attributes[Chunk.SUBSUPSCRIPT] != null) {
                this.superSubScript = (float)chunk.Attributes[Chunk.SUBSUPSCRIPT];
            }
            if (chunk.Attributes != null && chunk.Attributes[Chunk.BACKGROUND] != null) {
                this.background = new RtfColor(this.document, (Color) ((Object[]) chunk.Attributes[Chunk.BACKGROUND])[0]);
            }
            font = new ST.RtfFont(doc, chunk.Font);
            content = chunk.Content;
        }
        
        /**
        * Writes the content of this RtfChunk. First the font information
        * is written, then the content, and then more font information
        */ 
        public override void WriteContent(Stream result) {
            byte[] t;
            if (this.background != null) {
                result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
            }
            
            font.WriteBegin(result);
            if (superSubScript < 0) {
                result.Write(FONT_SUBSCRIPT, 0, FONT_SUBSCRIPT.Length);
            } else if (superSubScript > 0) {
                result.Write(FONT_SUPERSCRIPT, 0, FONT_SUPERSCRIPT.Length);
            }
            if (this.background != null) {
                result.Write(BACKGROUND_COLOR, 0, BACKGROUND_COLOR.Length);
                result.Write(t = IntToByteArray(this.background.GetColorNumber()), 0, t.Length);
            }
            result.Write(DELIMITER, 0, DELIMITER.Length);
            
            document.FilterSpecialChar(result, content, false, softLineBreaks || this.document.GetDocumentSettings().IsAlwaysGenerateSoftLinebreaks());
            
            if (superSubScript != 0) {
                result.Write(FONT_END_SUPER_SUBSCRIPT, 0, FONT_END_SUPER_SUBSCRIPT.Length);
            }
            font.WriteEnd(result);
            
            if (this.background != null) {
                result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
            }
        }
        
        /**
        * Sets the RtfDocument this RtfChunk belongs to.
        * 
        * @param doc The RtfDocument to use
        */
        public override void SetRtfDocument(RtfDocument doc) {
            base.SetRtfDocument(doc);
            this.font.SetRtfDocument(this.document);
        }
        
        /**
        * Sets whether to use soft line breaks instead of default hard ones.
        * 
        * @param softLineBreaks whether to use soft line breaks instead of default hard ones.
        */
        public void SetSoftLineBreaks(bool softLineBreaks) {
            this.softLineBreaks = softLineBreaks;
        }
        
        /**
        * Gets whether to use soft line breaks instead of default hard ones.
        * 
        * @return whether to use soft line breaks instead of default hard ones.
        */
        public bool GetSoftLineBreaks() {
            return this.softLineBreaks;
        }
    }
}