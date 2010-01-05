using System;
using System.IO;

using iTextSharp.text;

using System.util.zlib;

/*
 * $Id: PdfContents.cs,v 1.4 2008/05/13 11:25:19 psoares33 Exp $
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
     * <CODE>PdfContents</CODE> is a <CODE>PdfStream</CODE> containing the contents (text + graphics) of a <CODE>PdfPage</CODE>.
     */

    public class PdfContents : PdfStream {
    
        internal static byte[] SAVESTATE = DocWriter.GetISOBytes("q\n");
        internal static byte[] RESTORESTATE = DocWriter.GetISOBytes("Q\n");
        internal static byte[] ROTATE90 = DocWriter.GetISOBytes("0 1 -1 0 ");
        internal static byte[] ROTATE180 = DocWriter.GetISOBytes("-1 0 0 -1 ");
        internal static byte[] ROTATE270 = DocWriter.GetISOBytes("0 -1 1 0 ");
        internal static byte[] ROTATEFINAL = DocWriter.GetISOBytes(" cm\n");
        // constructor
    
        /**
         * Constructs a <CODE>PdfContents</CODE>-object, containing text and general graphics.
         *
         * @param under the direct content that is under all others
         * @param content the graphics in a page
         * @param text the text in a page
         * @param secondContent the direct content that is over all others
         * @throws BadPdfFormatException on error
         */
    
        internal PdfContents(PdfContentByte under, PdfContentByte content, PdfContentByte text, PdfContentByte secondContent, Rectangle page) : base() {
            Stream ostr = null;
            streamBytes = new MemoryStream();
            if (Document.Compress) {
                compressed = true;
                ostr = new ZDeflaterOutputStream(streamBytes);
            }
            else
                ostr = streamBytes;
            int rotation = page.Rotation;
            byte[] tmp;
            switch (rotation) {
                case 90:
                    ostr.Write(ROTATE90, 0, ROTATE90.Length);
                    tmp = DocWriter.GetISOBytes(ByteBuffer.FormatDouble(page.Top));
                    ostr.Write(tmp, 0, tmp.Length);
                    ostr.WriteByte((byte)' ');
                    ostr.WriteByte((byte)'0');
                    ostr.Write(ROTATEFINAL, 0, ROTATEFINAL.Length);
                    break;
                case 180:
                    ostr.Write(ROTATE180, 0, ROTATE180.Length);
                    tmp = DocWriter.GetISOBytes(ByteBuffer.FormatDouble(page.Right));
                    ostr.Write(tmp, 0, tmp.Length);
                    ostr.WriteByte((byte)' ');
                    tmp = DocWriter.GetISOBytes(ByteBuffer.FormatDouble(page.Top));
                    ostr.Write(tmp, 0, tmp.Length);
                    ostr.Write(ROTATEFINAL, 0, ROTATEFINAL.Length);
                    break;
                case 270:
                    ostr.Write(ROTATE270, 0, ROTATE270.Length);
                    ostr.WriteByte((byte)'0');
                    ostr.WriteByte((byte)' ');
                    tmp = DocWriter.GetISOBytes(ByteBuffer.FormatDouble(page.Right));
                    ostr.Write(tmp, 0, tmp.Length);
                    ostr.Write(ROTATEFINAL, 0, ROTATEFINAL.Length);
                    break;
            }
            if (under.Size > 0) {
                ostr.Write(SAVESTATE, 0, SAVESTATE.Length);
                under.InternalBuffer.WriteTo(ostr);
                ostr.Write(RESTORESTATE, 0, RESTORESTATE.Length);
            }
            if (content.Size > 0) {
                ostr.Write(SAVESTATE, 0, SAVESTATE.Length);
                content.InternalBuffer.WriteTo(ostr);
                ostr.Write(RESTORESTATE, 0, RESTORESTATE.Length);
            }
            if (text != null) {
                ostr.Write(SAVESTATE, 0, SAVESTATE.Length);
                text.InternalBuffer.WriteTo(ostr);
                ostr.Write(RESTORESTATE, 0, RESTORESTATE.Length);
            }
            if (secondContent.Size > 0) {
                secondContent.InternalBuffer.WriteTo(ostr);
            }

            if (ostr is ZDeflaterOutputStream)
                ((ZDeflaterOutputStream)ostr).Finish();
            Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
            if (compressed)
                Put(PdfName.FILTER, PdfName.FLATEDECODE);
        }
    }
}