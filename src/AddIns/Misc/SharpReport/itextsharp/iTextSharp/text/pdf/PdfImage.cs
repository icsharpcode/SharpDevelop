using System;
using System.IO;
using System.Net;
using iTextSharp.text;
/*
 * $Id: PdfImage.cs,v 1.6 2008/05/13 11:25:21 psoares33 Exp $
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
    * <CODE>PdfImage</CODE> is a <CODE>PdfStream</CODE> containing an image-<CODE>Dictionary</CODE> and -stream.
    */

    public class PdfImage : PdfStream {
        
        internal const int TRANSFERSIZE = 4096;
        // membervariables
        
        /** This is the <CODE>PdfName</CODE> of the image. */
        protected PdfName name = null;
        
        // constructor
        
        /**
        * Constructs a <CODE>PdfImage</CODE>-object.
        *
        * @param image the <CODE>Image</CODE>-object
        * @param name the <CODE>PdfName</CODE> for this image
        * @throws BadPdfFormatException on error
        */
        
        public PdfImage(Image image, String name, PdfIndirectReference maskRef) {
            this.name = new PdfName(name);
            Put(PdfName.TYPE, PdfName.XOBJECT);
            Put(PdfName.SUBTYPE, PdfName.IMAGE);
            Put(PdfName.WIDTH, new PdfNumber(image.Width));
            Put(PdfName.HEIGHT, new PdfNumber(image.Height));
            if (image.Layer != null)
                Put(PdfName.OC, image.Layer.Ref);
            if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 0xff))
                Put(PdfName.IMAGEMASK, PdfBoolean.PDFTRUE);
            if (maskRef != null) {
                if (image.Smask)
                    Put(PdfName.SMASK, maskRef);
                else
                    Put(PdfName.MASK, maskRef);
            }
            if (image.IsMask() && image.Inverted)
                Put(PdfName.DECODE, new PdfLiteral("[1 0]"));
            if (image.Interpolation)
                Put(PdfName.INTERPOLATE, PdfBoolean.PDFTRUE);
            Stream isp = null;
            try {
                // Raw Image data
                if (image.IsImgRaw()) {
                    // will also have the CCITT parameters
                    int colorspace = image.Colorspace;
                    int[] transparency = image.Transparency;
                    if (transparency != null && !image.IsMask() && maskRef == null) {
                        String s = "[";
                        for (int k = 0; k < transparency.Length; ++k)
                            s += transparency[k] + " ";
                        s += "]";
                        Put(PdfName.MASK, new PdfLiteral(s));
                    }
                    bytes = image.RawData;
                    Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                    int bpc = image.Bpc;
                    if (bpc > 0xff) {
                        if (!image.IsMask())
                            Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                        Put(PdfName.BITSPERCOMPONENT, new PdfNumber(1));
                        Put(PdfName.FILTER, PdfName.CCITTFAXDECODE);
                        int k = bpc - Image.CCITTG3_1D;
                        PdfDictionary decodeparms = new PdfDictionary();
                        if (k != 0)
                            decodeparms.Put(PdfName.K, new PdfNumber(k));
                        if ((colorspace & Image.CCITT_BLACKIS1) != 0)
                            decodeparms.Put(PdfName.BLACKIS1, PdfBoolean.PDFTRUE);
                        if ((colorspace & Image.CCITT_ENCODEDBYTEALIGN) != 0)
                            decodeparms.Put(PdfName.ENCODEDBYTEALIGN, PdfBoolean.PDFTRUE);
                        if ((colorspace & Image.CCITT_ENDOFLINE) != 0)
                            decodeparms.Put(PdfName.ENDOFLINE, PdfBoolean.PDFTRUE);
                        if ((colorspace & Image.CCITT_ENDOFBLOCK) != 0)
                            decodeparms.Put(PdfName.ENDOFBLOCK, PdfBoolean.PDFFALSE);
                        decodeparms.Put(PdfName.COLUMNS, new PdfNumber(image.Width));
                        decodeparms.Put(PdfName.ROWS, new PdfNumber(image.Height));
                        Put(PdfName.DECODEPARMS, decodeparms);
                    }
                    else {
                        switch (colorspace) {
                            case 1:
                                Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                                if (image.Inverted)
                                    Put(PdfName.DECODE, new PdfLiteral("[1 0]"));
                                break;
                            case 3:
                                Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                                if (image.Inverted)
                                    Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0]"));
                                break;
                            case 4:
                            default:
                                Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                                if (image.Inverted)
                                    Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                                break;
                        }
                        PdfDictionary additional = image.Additional;
                        if (additional != null)
                            Merge(additional);
                        if (image.IsMask() && (image.Bpc == 1 || image.Bpc > 8))
                            Remove(PdfName.COLORSPACE);
                        Put(PdfName.BITSPERCOMPONENT, new PdfNumber(image.Bpc));
                        if (image.Deflated)
                            Put(PdfName.FILTER, PdfName.FLATEDECODE);
                        else {
                            FlateCompress();
                        }
                    }
                    return;
                }
                
                // GIF, JPEG or PNG
                String errorID;
                if (image.RawData == null){
                    isp = WebRequest.Create(image.Url).GetResponse().GetResponseStream();
                    errorID = image.Url.ToString();
                }
                else{
                    isp = new MemoryStream(image.RawData);
                    errorID = "Byte array";
                }
                switch (image.Type) {
                    case Image.JPEG:
                        Put(PdfName.FILTER, PdfName.DCTDECODE);
                        switch (image.Colorspace) {
                            case 1:
                                Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                                break;
                            case 3:
                                Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                                break;
                            default:
                                Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                                if (image.Inverted) {
                                    Put(PdfName.DECODE, new PdfLiteral("[1 0 1 0 1 0 1 0]"));
                                }
                                break;
                        }
                        Put(PdfName.BITSPERCOMPONENT, new PdfNumber(8));
                        if (image.RawData != null){
                            bytes = image.RawData;
                            Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                            return;
                        }
                        streamBytes = new MemoryStream();
                        TransferBytes(isp, streamBytes, -1);
                        break;
                    case Image.JPEG2000:
                        Put(PdfName.FILTER, PdfName.JPXDECODE);
                        if (image.Colorspace > 0) {
                            switch (image.Colorspace) {
                                case 1:
                                    Put(PdfName.COLORSPACE, PdfName.DEVICEGRAY);
                                    break;
                                case 3:
                                    Put(PdfName.COLORSPACE, PdfName.DEVICERGB);
                                    break;
                                default:
                                    Put(PdfName.COLORSPACE, PdfName.DEVICECMYK);
                                    break;
                            }
                            Put(PdfName.BITSPERCOMPONENT, new PdfNumber(image.Bpc));
                        }
                        if (image.RawData != null){
                            bytes = image.RawData;
                            Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
                            return;
                        }
                        streamBytes = new MemoryStream();
                        TransferBytes(isp, streamBytes, -1);
                        break;
                    default:
                        throw new IOException(errorID + " is an unknown Image format.");
                }
                Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
            }
            finally {
                if (isp != null) {
                    try{
                        isp.Close();
                    }
                    catch  {
                        // empty on purpose
                    }
                }
            }
        }
        
        /**
        * Returns the <CODE>PdfName</CODE> of the image.
        *
        * @return        the name
        */
        
        public PdfName Name {
            get {
                return name;
            }
        }
        
        internal static void TransferBytes(Stream inp, Stream outp, int len) {
            byte[] buffer = new byte[TRANSFERSIZE];
            if (len < 0)
                len = 0x7ffffff;
            int size;
            while (len != 0) {
                size = inp.Read(buffer, 0, Math.Min(len, TRANSFERSIZE));
                if (size <= 0)
                    return;
                outp.Write(buffer, 0, size);
                len -= size;
            }
        }
        
        protected void ImportAll(PdfImage dup) {
            name = dup.name;
            compressed = dup.compressed;
            streamBytes = dup.streamBytes;
            bytes = dup.bytes;
            hashMap = dup.hashMap;
        }
    }
}