using System;
using System.IO;

/*
 * $Id: PdfString.cs,v 1.6 2008/05/13 11:25:23 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
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

    /**
     * A <CODE>PdfString</CODE>-class is the PDF-equivalent of a JAVA-<CODE>string</CODE>-object.
     * <P>
     * A string is a sequence of characters delimited by parenthesis. If a string is too long
     * to be conveniently placed on a single line, it may be split across multiple lines by using
     * the backslash character (\) at the end of a line to indicate that the string continues
     * on the following line. Within a string, the backslash character is used as an escape to
     * specify unbalanced parenthesis, non-printing ASCII characters, and the backslash character
     * itself. Use of the \<I>ddd</I> escape sequence is the preferred way to represent characters
     * outside the printable ASCII character set.<BR>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 4.4 (page 37-39).
     *
     * @see        PdfObject
     * @see        BadPdfFormatException
     */

    public class PdfString : PdfObject {
    
        // membervariables
    
        /** The value of this object. */
        protected string value = NOTHING;
        protected string originalValue = null;
    
        /** The encoding. */
        protected string encoding = TEXT_PDFDOCENCODING;
        protected int objNum = 0;
        protected int objGen = 0;
        protected bool hexWriting = false;
        // constructors
    
        /**
         * Constructs an empty <CODE>PdfString</CODE>-object.
         */
    
        public PdfString() : base(STRING) {}
    
        /**
         * Constructs a <CODE>PdfString</CODE>-object.
         *
         * @param        value        the content of the string
         */
    
        public PdfString(string value) : base(STRING) {
            this.value = value;
        }
    
        /**
         * Constructs a <CODE>PdfString</CODE>-object.
         *
         * @param        value        the content of the string
         * @param        encoding    an encoding
         */
    
        public PdfString(string value, string encoding) : base(STRING) {
            this.value = value;
            this.encoding = encoding;
        }
    
        /**
         * Constructs a <CODE>PdfString</CODE>-object.
         *
         * @param        bytes    an array of <CODE>byte</CODE>
         */
    
        public PdfString(byte[] bytes) : base(STRING) {
            value = PdfEncodings.ConvertToString(bytes, null);
            encoding = NOTHING;
        }
    
        // methods overriding some methods in PdfObject
    
        /**
         * Returns the PDF representation of this <CODE>PdfString</CODE>.
         *
         * @return        an array of <CODE>byte</CODE>s
         */
    
        public override void ToPdf(PdfWriter writer, Stream os) {
            byte[] b = GetBytes();
            PdfEncryption crypto = null;
            if (writer != null)
                crypto = writer.Encryption;
            if (crypto != null) {
                b = crypto.EncryptByteArray(b);
            }
            if (hexWriting) {
                ByteBuffer buf = new ByteBuffer();
                buf.Append('<');
                int len = b.Length;
                for (int k = 0; k < len; ++k)
                    buf.AppendHex(b[k]);
                buf.Append('>');
                os.Write(buf.ToByteArray(), 0, buf.Size);
            }
            else {
                b = PdfContentByte.EscapeString(b);
                os.Write(b, 0, b.Length);
            }
        }
    
        /**
         * Returns the <CODE>string</CODE> value of the <CODE>PdfString</CODE>-object.
         *
         * @return        a <CODE>string</CODE>
         */
    
        public override string ToString() {
            return value;
        }
    
        // other methods
    
        /**
         * Gets the encoding of this string.
         *
         * @return        a <CODE>string</CODE>
         */
    
        public string Encoding {
            get {
                return encoding;
            }
        }
        public String ToUnicodeString() {
            if (encoding != null && encoding.Length != 0)
                return value;
            GetBytes();
            if (bytes.Length >= 2 && bytes[0] == (byte)254 && bytes[1] == (byte)255)
                return PdfEncodings.ConvertToString(bytes, PdfObject.TEXT_UNICODE);
            else
                return PdfEncodings.ConvertToString(bytes, PdfObject.TEXT_PDFDOCENCODING);
        }
    
        internal void SetObjNum(int objNum, int objGen) {
            this.objNum = objNum;
            this.objGen = objGen;
        }
    
        internal void Decrypt(PdfReader reader) {
            PdfEncryption decrypt = reader.Decrypt;
            if (decrypt != null) {
                originalValue = value;
                decrypt.SetHashKey(objNum, objGen);
                bytes = PdfEncodings.ConvertToBytes(value, null);
                bytes = decrypt.DecryptByteArray(bytes);
                value = PdfEncodings.ConvertToString(bytes, null);
            }
        }
   
        public override byte[] GetBytes() {
            if (bytes == null) {
                if (encoding != null && encoding.Equals(TEXT_UNICODE) && PdfEncodings.IsPdfDocEncoding(value))
                    bytes = PdfEncodings.ConvertToBytes(value, TEXT_PDFDOCENCODING);
                else
                    bytes = PdfEncodings.ConvertToBytes(value, encoding);
            }
            return bytes;
        }
    
        public byte[] GetOriginalBytes() {
            if (originalValue == null)
                return GetBytes();
            return PdfEncodings.ConvertToBytes(originalValue, null);
        }
    
        public PdfString SetHexWriting(bool hexWriting) {
            this.hexWriting = hexWriting;
            return this;
        }
    
        public bool IsHexWriting() {
            return hexWriting;
        }
    }
}