using System;
using System.Text;
/*
 * $Id: EncodingNoPreamble.cs,v 1.2 2008/05/13 11:26:16 psoares33 Exp $
 * 
 *
 * Copyright 2007 Paulo Soares.
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
 * the Initial Developer are Copyright (C) 1999-2005 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2005 by Paulo Soares. All Rights Reserved.
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
 * of this file under either the MPL or the GNU LIBRARY GENERAL PUBLIC LICENSE 
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU LIBRARY GENERAL PUBLIC LICENSE for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.xml.xmp {

    /// <summary>
    /// A wrapper for an Encoding to suppress the preamble.
    /// </summary>
    public class EncodingNoPreamble : Encoding {

        private Encoding encoding;
        private static byte[] emptyPreamble = new byte[0];

        public EncodingNoPreamble(Encoding encoding) {
            this.encoding = encoding;
        }
    
        public override int GetByteCount(char[] chars, int index, int count) {
            return encoding.GetByteCount(chars, index, count);
        }
    
        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex) {
            return encoding.GetBytes(chars, charIndex, charCount, bytes, byteIndex);
        }
    
        public override int GetCharCount(byte[] bytes, int index, int count) {
            return encoding.GetCharCount(bytes, index, count);
        }
    
        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex) {
            return encoding.GetChars(bytes, byteIndex, byteCount, chars, charIndex);
        }
    
        public override int GetMaxByteCount(int charCount) {
            return encoding.GetMaxByteCount(charCount);
        }
    
        public override int GetMaxCharCount(int byteCount) {
            return encoding.GetMaxCharCount(byteCount);
        }
    
        public override string BodyName {
            get {
                return encoding.BodyName;
            }
        }
    
        public override int CodePage {
            get {
                return encoding.CodePage;
            }
        }
    
        public override string EncodingName {
            get {
                return encoding.EncodingName;
            }
        }
    
        public override string HeaderName {
            get {
                return encoding.HeaderName;
            }
        }
    
        public override bool IsBrowserDisplay {
            get {
                return encoding.IsBrowserDisplay;
            }
        }
    
        public override bool IsBrowserSave {
            get {
                return encoding.IsBrowserSave;
            }
        }
    
        public override bool IsMailNewsDisplay {
            get {
                return encoding.IsMailNewsDisplay;
            }
        }
    
        public override bool IsMailNewsSave {
            get {
                return encoding.IsMailNewsSave;
            }
        }
    
        public override string WebName {
            get {
                return encoding.WebName;
            }
        }
    
        public override int WindowsCodePage {
            get {
                return encoding.WindowsCodePage;
            }
        }
    
        public override Decoder GetDecoder() {
            return encoding.GetDecoder ();
        }
    
        public override Encoder GetEncoder() {
            return encoding.GetEncoder ();
        }
    
        public override byte[] GetPreamble() {
            return emptyPreamble;
        }
    }
}