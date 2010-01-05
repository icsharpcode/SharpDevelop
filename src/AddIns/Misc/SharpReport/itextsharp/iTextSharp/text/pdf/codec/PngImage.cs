using System;
using System.IO;
using System.Net;
using System.Text;
using System.util.zlib;
using System.util;
using iTextSharp.text;
using iTextSharp.text.pdf;
/*
 * Copyright 2003-2008 by Paulo Soares.
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
 *
 * This code is based on a series of source files originally released
 * by SUN in the context of the JAI project. The original code was released 
 * under the BSD license in a specific wording. In a mail dating from
 * January 23, 2008, Brian Burkhalter (@sun.com) gave us permission
 * to use the code under the following version of the BSD license:
 *
 * Copyright (c) 2005 Sun Microsystems, Inc. All  Rights Reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met: 
 * 
 * - Redistribution of source code must retain the above copyright 
 *   notice, this  list of conditions and the following disclaimer.
 * 
 * - Redistribution in binary form must reproduce the above copyright
 *   notice, this list of conditions and the following disclaimer in 
 *   the documentation and/or other materials provided with the
 *   distribution.
 * 
 * Neither the name of Sun Microsystems, Inc. or the names of 
 * contributors may be used to endorse or promote products derived 
 * from this software without specific prior written permission.
 * 
 * This software is provided "AS IS," without a warranty of any 
 * kind. ALL EXPRESS OR IMPLIED CONDITIONS, REPRESENTATIONS AND 
 * WARRANTIES, INCLUDING ANY IMPLIED WARRANTY OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE OR NON-INFRINGEMENT, ARE HEREBY
 * EXCLUDED. SUN MIDROSYSTEMS, INC. ("SUN") AND ITS LICENSORS SHALL 
 * NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A RESULT OF 
 * USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
 * DERIVATIVES. IN NO EVENT WILL SUN OR ITS LICENSORS BE LIABLE FOR 
 * ANY LOST REVENUE, PROFIT OR DATA, OR FOR DIRECT, INDIRECT, SPECIAL,
 * CONSEQUENTIAL, INCIDENTAL OR PUNITIVE DAMAGES, HOWEVER CAUSED AND
 * REGARDLESS OF THE THEORY OF LIABILITY, ARISING OUT OF THE USE OF OR
 * INABILITY TO USE THIS SOFTWARE, EVEN IF SUN HAS BEEN ADVISED OF THE
 * POSSIBILITY OF SUCH DAMAGES. 
 * 
 * You acknowledge that this software is not designed or intended for 
 * use in the design, construction, operation or maintenance of any 
 * nuclear facility.
 */

namespace iTextSharp.text.pdf.codec {
    /** Reads a PNG image. All types of PNG can be read.
    * <p>
    * It is based in part in the JAI codec.
    *
    * @author  Paulo Soares (psoares@consiste.pt)
    */
    public class PngImage {
    /** Some PNG specific values. */
        public static int[] PNGID = {137, 80, 78, 71, 13, 10, 26, 10};
        
    /** A PNG marker. */
        public const String IHDR = "IHDR";
        
    /** A PNG marker. */
        public const String PLTE = "PLTE";
        
    /** A PNG marker. */
        public const String IDAT = "IDAT";
        
    /** A PNG marker. */
        public const String IEND = "IEND";
        
    /** A PNG marker. */
        public const String tRNS = "tRNS";
        
    /** A PNG marker. */
        public const String pHYs = "pHYs";
        
    /** A PNG marker. */
        public const String gAMA = "gAMA";
        
    /** A PNG marker. */
        public const String cHRM = "cHRM";
        
    /** A PNG marker. */
        public const String sRGB = "sRGB";
        
    /** A PNG marker. */
        public const String iCCP = "iCCP";
        
        private const int TRANSFERSIZE = 4096;
        private const int PNG_FILTER_NONE = 0;
        private const int PNG_FILTER_SUB = 1;
        private const int PNG_FILTER_UP = 2;
        private const int PNG_FILTER_AVERAGE = 3;
        private const int PNG_FILTER_PAETH = 4;
        private static PdfName[] intents = {PdfName.PERCEPTUAL,
            PdfName.RELATIVECALORIMETRIC,PdfName.SATURATION,PdfName.ABSOLUTECALORIMETRIC};
        
        Stream isp;
        Stream dataStream;
        int width;
        int height;
        int bitDepth;
        int colorType;
        int compressionMethod;
        int filterMethod;
        int interlaceMethod;
        PdfDictionary additional = new PdfDictionary();
        byte[] image;
        byte[] smask;
        byte[] trans;
        MemoryStream idat = new MemoryStream();
        int dpiX;
        int dpiY;
        float XYRatio;
        bool genBWMask;
        bool palShades;
        int transRedGray = -1;
        int transGreen = -1;
        int transBlue = -1;
        int inputBands;
        int bytesPerPixel; // number of bytes per input pixel
        byte[] colorTable;
        float gamma = 1f;
        bool hasCHRM = false;
        float xW, yW, xR, yR, xG, yG, xB, yB;
        PdfName intent;
        ICC_Profile icc_profile;

        
        
        /** Creates a new instance of PngImage */
        PngImage(Stream isp) {
            this.isp = isp;
        }
        
        /** Reads a PNG from an url.
        * @param url the url
        * @throws IOException on error
        * @return the image
        */    
        public static Image GetImage(Uri url) {
            Stream isp = null;
            try {
                isp = WebRequest.Create(url).GetResponse().GetResponseStream();
                Image img = GetImage(isp);
                img.Url = url;
                return img;
            }
            finally {
                if (isp != null) {
                    isp.Close();
                }
            }
        }
        
        /** Reads a PNG from a stream.
        * @param is the stream
        * @throws IOException on error
        * @return the image
        */    
        public static Image GetImage(Stream isp) {
            PngImage png = new PngImage(isp);
            return png.GetImage();
        }
        
        /** Reads a PNG from a file.
        * @param file the file
        * @throws IOException on error
        * @return the image
        */    
        public static Image GetImage(String file) {
            return GetImage(Utilities.ToURL(file));
        }
        
        /** Reads a PNG from a byte array.
        * @param data the byte array
        * @throws IOException on error
        * @return the image
        */    
        public static Image GetImage(byte[] data) {
            Stream isp = new MemoryStream(data);
            Image img = GetImage(isp);
            img.OriginalData = data;
            return img;
        }
        
        static bool CheckMarker(String s) {
            if (s.Length != 4)
                return false;
            for (int k = 0; k < 4; ++k) {
                char c = s[k];
                if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z'))
                    return false;
            }
            return true;
        }
        
        void ReadPng() {
            for (int i = 0; i < PNGID.Length; i++) {
                if (PNGID[i] != isp.ReadByte())	{
                    throw new IOException("File is not a valid PNG.");
                }
            }
            byte[] buffer = new byte[TRANSFERSIZE];
            while (true) {
                int len = GetInt(isp);
                String marker = GetString(isp);
                if (len < 0 || !CheckMarker(marker))
                    throw new IOException("Corrupted PNG file.");
                if (IDAT.Equals(marker)) {
                    int size;
                    while (len != 0) {
                        size = isp.Read(buffer, 0, Math.Min(len, TRANSFERSIZE));
                        if (size <= 0)
                            return;
                        idat.Write(buffer, 0, size);
                        len -= size;
                    }
                }
                else if (tRNS.Equals(marker)) {
                    switch (colorType) {
                        case 0:
                            if (len >= 2) {
                                len -= 2;
                                int gray = GetWord(isp);
                                if (bitDepth == 16)
                                    transRedGray = gray;
                                else
                                    additional.Put(PdfName.MASK, new PdfLiteral("["+gray+" "+gray+"]"));
                            }
                            break;
                        case 2:
                            if (len >= 6) {
                                len -= 6;
                                int red = GetWord(isp);
                                int green = GetWord(isp);
                                int blue = GetWord(isp);
                                if (bitDepth == 16) {
                                    transRedGray = red;
                                    transGreen = green;
                                    transBlue = blue;
                                }
                                else
                                    additional.Put(PdfName.MASK, new PdfLiteral("["+red+" "+red+" "+green+" "+green+" "+blue+" "+blue+"]"));
                            }
                            break;
                        case 3:
                            if (len > 0) {
                                trans = new byte[len];
                                for (int k = 0; k < len; ++k)
                                    trans[k] = (byte)isp.ReadByte();
                                len = 0;
                            }
                            break;
                    }
                    Utilities.Skip(isp, len);
                }
                else if (IHDR.Equals(marker)) {
                    width = GetInt(isp);
                    height = GetInt(isp);
                    
                    bitDepth = isp.ReadByte();
                    colorType = isp.ReadByte();
                    compressionMethod = isp.ReadByte();
                    filterMethod = isp.ReadByte();
                    interlaceMethod = isp.ReadByte();
                }
                else if (PLTE.Equals(marker)) {
                    if (colorType == 3) {
                        PdfArray colorspace = new PdfArray();
                        colorspace.Add(PdfName.INDEXED);
                        colorspace.Add(GetColorspace());
                        colorspace.Add(new PdfNumber(len / 3 - 1));
                        ByteBuffer colortable = new ByteBuffer();
                        while ((len--) > 0) {
                            colortable.Append_i(isp.ReadByte());
                        }
                        colorspace.Add(new PdfString(colorTable = colortable.ToByteArray()));
                        additional.Put(PdfName.COLORSPACE, colorspace);
                    }
                    else {
                        Utilities.Skip(isp, len);
                    }
                }
                else if (pHYs.Equals(marker)) {
                    int dx = GetInt(isp);
                    int dy = GetInt(isp);
                    int unit = isp.ReadByte();
                    if (unit == 1) {
                        dpiX = (int)((float)dx * 0.0254f + 0.5f);
                        dpiY = (int)((float)dy * 0.0254f + 0.5f);
                    }
                    else {
                        if (dy != 0)
                            XYRatio = (float)dx / (float)dy;
                    }
                }
                else if (cHRM.Equals(marker)) {
                    xW = (float)GetInt(isp) / 100000f;
                    yW = (float)GetInt(isp) / 100000f;
                    xR = (float)GetInt(isp) / 100000f;
                    yR = (float)GetInt(isp) / 100000f;
                    xG = (float)GetInt(isp) / 100000f;
                    yG = (float)GetInt(isp) / 100000f;
                    xB = (float)GetInt(isp) / 100000f;
                    yB = (float)GetInt(isp) / 100000f;
                    hasCHRM = !(Math.Abs(xW)<0.0001f||Math.Abs(yW)<0.0001f||Math.Abs(xR)<0.0001f||Math.Abs(yR)<0.0001f||Math.Abs(xG)<0.0001f||Math.Abs(yG)<0.0001f||Math.Abs(xB)<0.0001f||Math.Abs(yB)<0.0001f);
                }
                else if (sRGB.Equals(marker)) {
                    int ri = isp.ReadByte();
                    intent = intents[ri];
                    gamma = 2.2f;
                    xW = 0.3127f;
                    yW = 0.329f;
                    xR = 0.64f;
                    yR = 0.33f;
                    xG = 0.3f;
                    yG = 0.6f;
                    xB = 0.15f;
                    yB = 0.06f;
                    hasCHRM = true;
                }
                else if (gAMA.Equals(marker)) {
                    int gm = GetInt(isp);
                    if (gm != 0) {
                        gamma = 100000f / (float)gm;
                        if (!hasCHRM) {
                            xW = 0.3127f;
                            yW = 0.329f;
                            xR = 0.64f;
                            yR = 0.33f;
                            xG = 0.3f;
                            yG = 0.6f;
                            xB = 0.15f;
                            yB = 0.06f;
                            hasCHRM = true;
                        }
                    }
                }
                else if (iCCP.Equals(marker)) {
                    do {
                        --len;
                    } while (isp.ReadByte() != 0);
                    isp.ReadByte();
                    --len;
                    byte[] icccom = new byte[len];
                    int p = 0;
                    while (len > 0) {
                        int r = isp.Read(icccom, p, len);
                        if (r < 0)
                            throw new IOException("Premature end of file.");
                        p += r;
                        len -= r;
                    }
                    byte[] iccp = PdfReader.FlateDecode(icccom, true);
                    icccom = null;
                    try {
                        icc_profile = ICC_Profile.GetInstance(iccp);
                    }
                    catch {
                        icc_profile = null;
                    }
                }
                else if (IEND.Equals(marker)) {
                    break;
                }
                else {
                    Utilities.Skip(isp, len);
                }
                Utilities.Skip(isp, 4);
            }
        }
        
        PdfObject GetColorspace() {
            if (icc_profile != null) {
                if ((colorType & 2) == 0)
                    return PdfName.DEVICEGRAY;
                else
                    return PdfName.DEVICERGB;
            }
            if (gamma == 1f && !hasCHRM) {
                if ((colorType & 2) == 0)
                    return PdfName.DEVICEGRAY;
                else
                    return PdfName.DEVICERGB;
            }
            else {
                PdfArray array = new PdfArray();
                PdfDictionary dic = new PdfDictionary();
                if ((colorType & 2) == 0) {
                    if (gamma == 1f)
                        return PdfName.DEVICEGRAY;
                    array.Add(PdfName.CALGRAY);
                    dic.Put(PdfName.GAMMA, new PdfNumber(gamma));
                    dic.Put(PdfName.WHITEPOINT, new PdfLiteral("[1 1 1]"));
                    array.Add(dic);
                }
                else {
                    PdfObject wp = new PdfLiteral("[1 1 1]");
                    array.Add(PdfName.CALRGB);
                    if (gamma != 1f) {
                        PdfArray gm = new PdfArray();
                        PdfNumber n = new PdfNumber(gamma);
                        gm.Add(n);
                        gm.Add(n);
                        gm.Add(n);
                        dic.Put(PdfName.GAMMA, gm);
                    }
                    if (hasCHRM) {
                        float z = yW*((xG-xB)*yR-(xR-xB)*yG+(xR-xG)*yB);
                        float YA = yR*((xG-xB)*yW-(xW-xB)*yG+(xW-xG)*yB)/z;
                        float XA = YA*xR/yR;
                        float ZA = YA*((1-xR)/yR-1);
                        float YB = -yG*((xR-xB)*yW-(xW-xB)*yR+(xW-xR)*yB)/z;
                        float XB = YB*xG/yG;
                        float ZB = YB*((1-xG)/yG-1);
                        float YC = yB*((xR-xG)*yW-(xW-xG)*yW+(xW-xR)*yG)/z;
                        float XC = YC*xB/yB;
                        float ZC = YC*((1-xB)/yB-1);
                        float XW = XA+XB+XC;
                        float YW = 1;//YA+YB+YC;
                        float ZW = ZA+ZB+ZC;
                        PdfArray wpa = new PdfArray();
                        wpa.Add(new PdfNumber(XW));
                        wpa.Add(new PdfNumber(YW));
                        wpa.Add(new PdfNumber(ZW));
                        wp = wpa;
                        PdfArray matrix = new PdfArray();
                        matrix.Add(new PdfNumber(XA));
                        matrix.Add(new PdfNumber(YA));
                        matrix.Add(new PdfNumber(ZA));
                        matrix.Add(new PdfNumber(XB));
                        matrix.Add(new PdfNumber(YB));
                        matrix.Add(new PdfNumber(ZB));
                        matrix.Add(new PdfNumber(XC));
                        matrix.Add(new PdfNumber(YC));
                        matrix.Add(new PdfNumber(ZC));
                        dic.Put(PdfName.MATRIX, matrix);
                    }
                    dic.Put(PdfName.WHITEPOINT, wp);
                    array.Add(dic);
                }
                return array;
            }
        }
        
        Image GetImage() {
            ReadPng();
            int pal0 = 0;
            int palIdx = 0;
            palShades = false;
            if (trans != null) {
                for (int k = 0; k < trans.Length; ++k) {
                    int n = trans[k] & 0xff;
                    if (n == 0) {
                        ++pal0;
                        palIdx = k;
                    }
                    if (n != 0 && n != 255) {
                        palShades = true;
                        break;
                    }
                }
            }
            if ((colorType & 4) != 0)
                palShades = true;
            genBWMask = (!palShades && (pal0 > 1 || transRedGray >= 0));
            if (!palShades && !genBWMask && pal0 == 1) {
                additional.Put(PdfName.MASK, new PdfLiteral("["+palIdx+" "+palIdx+"]"));
            }
            bool needDecode = (interlaceMethod == 1) || (bitDepth == 16) || ((colorType & 4) != 0) || palShades || genBWMask;
            switch (colorType) {
                case 0:
                    inputBands = 1;
                    break;
                case 2:
                    inputBands = 3;
                    break;
                case 3:
                    inputBands = 1;
                    break;
                case 4:
                    inputBands = 2;
                    break;
                case 6:
                    inputBands = 4;
                    break;
            }
            if (needDecode)
                DecodeIdat();
            int components = inputBands;
            if ((colorType & 4) != 0)
                --components;
            int bpc = bitDepth;
            if (bpc == 16)
                bpc = 8;
            Image img;
            if (image != null)
                img = Image.GetInstance(width, height, components, bpc, image);
            else {
                img = new ImgRaw(width, height, components, bpc, idat.ToArray());
                img.Deflated = true;
                PdfDictionary decodeparms = new PdfDictionary();
                decodeparms.Put(PdfName.BITSPERCOMPONENT, new PdfNumber(bitDepth));
                decodeparms.Put(PdfName.PREDICTOR, new PdfNumber(15));
                decodeparms.Put(PdfName.COLUMNS, new PdfNumber(width));
                decodeparms.Put(PdfName.COLORS, new PdfNumber((colorType == 3 || (colorType & 2) == 0) ? 1 : 3));
                additional.Put(PdfName.DECODEPARMS, decodeparms);
            }
            if (additional.Get(PdfName.COLORSPACE) == null)
                additional.Put(PdfName.COLORSPACE, GetColorspace());
            if (intent != null)
                additional.Put(PdfName.INTENT, intent);
            if (additional.Size > 0)
                img.Additional = additional;
            if (icc_profile != null)
                img.TagICC = icc_profile;
            if (palShades) {
                Image im2 = Image.GetInstance(width, height, 1, 8, smask);
                im2.MakeMask();
                img.ImageMask = im2;
            }
            if (genBWMask) {
                Image im2 = Image.GetInstance(width, height, 1, 1, smask);
                im2.MakeMask();
                img.ImageMask = im2;
            }
            img.SetDpi(dpiX, dpiY);
            img.XYRatio = XYRatio;
            img.OriginalType = Image.ORIGINAL_PNG;
            return img;
        }
        
        void DecodeIdat() {
            int nbitDepth = bitDepth;
            if (nbitDepth == 16)
                nbitDepth = 8;
            int size = -1;
            bytesPerPixel = (bitDepth == 16) ? 2 : 1;
            switch (colorType) {
                case 0:
                    size = (nbitDepth * width + 7) / 8 * height;
                    break;
                case 2:
                    size = width * 3 * height;
                    bytesPerPixel *= 3;
                    break;
                case 3:
                    if (interlaceMethod == 1)
                        size = (nbitDepth * width + 7) / 8 * height;
                    bytesPerPixel = 1;
                    break;
                case 4:
                    size = width * height;
                    bytesPerPixel *= 2;
                    break;
                case 6:
                    size = width * 3 * height;
                    bytesPerPixel *= 4;
                    break;
            }
            if (size >= 0)
                image = new byte[size];
            if (palShades)
                smask = new byte[width * height];
            else if (genBWMask)
                smask = new byte[(width + 7) / 8 * height];
            idat.Position = 0;
            dataStream = new ZInflaterInputStream(idat);
            
            if (interlaceMethod != 1) {
                DecodePass(0, 0, 1, 1, width, height);
            }
            else {
                DecodePass(0, 0, 8, 8, (width + 7)/8, (height + 7)/8);
                DecodePass(4, 0, 8, 8, (width + 3)/8, (height + 7)/8);
                DecodePass(0, 4, 4, 8, (width + 3)/4, (height + 3)/8);
                DecodePass(2, 0, 4, 4, (width + 1)/4, (height + 3)/4);
                DecodePass(0, 2, 2, 4, (width + 1)/2, (height + 1)/4);
                DecodePass(1, 0, 2, 2, width/2, (height + 1)/2);
                DecodePass(0, 1, 1, 2, width, height/2);
            }
            
        }
        
        void DecodePass( int xOffset, int yOffset,
        int xStep, int yStep,
        int passWidth, int passHeight) {
            if ((passWidth == 0) || (passHeight == 0)) {
                return;
            }
            
            int bytesPerRow = (inputBands*passWidth*bitDepth + 7)/8;
            byte[] curr = new byte[bytesPerRow];
            byte[] prior = new byte[bytesPerRow];
            
            // Decode the (sub)image row-by-row
            int srcY, dstY;
            for (srcY = 0, dstY = yOffset;
            srcY < passHeight;
            srcY++, dstY += yStep) {
                // Read the filter type byte and a row of data
                int filter = 0;
                try {
                    filter = dataStream.ReadByte();
                    ReadFully(dataStream,curr, 0, bytesPerRow);
                } catch {
                    // empty on purpose
                }
                
                switch (filter) {
                    case PNG_FILTER_NONE:
                        break;
                    case PNG_FILTER_SUB:
                        DecodeSubFilter(curr, bytesPerRow, bytesPerPixel);
                        break;
                    case PNG_FILTER_UP:
                        DecodeUpFilter(curr, prior, bytesPerRow);
                        break;
                    case PNG_FILTER_AVERAGE:
                        DecodeAverageFilter(curr, prior, bytesPerRow, bytesPerPixel);
                        break;
                    case PNG_FILTER_PAETH:
                        DecodePaethFilter(curr, prior, bytesPerRow, bytesPerPixel);
                        break;
                    default:
                        // Error -- uknown filter type
                        throw new Exception("PNG filter unknown.");
                }
                
                ProcessPixels(curr, xOffset, xStep, dstY, passWidth);
                
                // Swap curr and prior
                byte[] tmp = prior;
                prior = curr;
                curr = tmp;
            }
        }
        
        void ProcessPixels(byte[] curr, int xOffset, int step, int y, int width) {
            int srcX, dstX;

            int[] outp = GetPixel(curr);
            int sizes = 0;
            switch (colorType) {
                case 0:
                case 3:
                case 4:
                    sizes = 1;
                    break;
                case 2:
                case 6:
                    sizes = 3;
                    break;
            }
            if (image != null) {
                dstX = xOffset;
                int yStride = (sizes*this.width*(bitDepth == 16 ? 8 : bitDepth)+ 7)/8;
                for (srcX = 0; srcX < width; srcX++) {
                    SetPixel(image, outp, inputBands * srcX, sizes, dstX, y, bitDepth, yStride);
                    dstX += step;
                }
            }
            if (palShades) {
                if ((colorType & 4) != 0) {
                    if (bitDepth == 16) {
                        for (int k = 0; k < width; ++k) {
                            int t = k * inputBands + sizes;
                            outp[t] = Util.USR(outp[t], 8);
                        }
                    }
                    int yStride = this.width;
                    dstX = xOffset;
                    for (srcX = 0; srcX < width; srcX++) {
                        SetPixel(smask, outp, inputBands * srcX + sizes, 1, dstX, y, 8, yStride);
                        dstX += step;
                    }
                }
                else { //colorType 3
                    int yStride = this.width;
                    int[] v = new int[1];
                    dstX = xOffset;
                    for (srcX = 0; srcX < width; srcX++) {
                        int idx = outp[srcX];
                        if (idx < trans.Length)
                            v[0] = trans[idx];
                        SetPixel(smask, v, 0, 1, dstX, y, 8, yStride);
                        dstX += step;
                    }
                }
            }
            else if (genBWMask) {
                switch (colorType) {
                    case 3: {
                        int yStride = (this.width + 7) / 8;
                        int[] v = new int[1];
                        dstX = xOffset;
                        for (srcX = 0; srcX < width; srcX++) {
                            int idx = outp[srcX];
                            if (idx < trans.Length)
                                v[0] = (trans[idx] == 0 ? 1 : 0);
                            SetPixel(smask, v, 0, 1, dstX, y, 1, yStride);
                            dstX += step;
                        }
                        break;
                    }
                    case 0: {
                        int yStride = (this.width + 7) / 8;
                        int[] v = new int[1];
                        dstX = xOffset;
                        for (srcX = 0; srcX < width; srcX++) {
                            int g = outp[srcX];
                            v[0] = (g == transRedGray ? 1 : 0);
                            SetPixel(smask, v, 0, 1, dstX, y, 1, yStride);
                            dstX += step;
                        }
                        break;
                    }
                    case 2: {
                        int yStride = (this.width + 7) / 8;
                        int[] v = new int[1];
                        dstX = xOffset;
                        for (srcX = 0; srcX < width; srcX++) {
                            int markRed = inputBands * srcX;
                            v[0] = (outp[markRed] == transRedGray && outp[markRed + 1] == transGreen 
                                && outp[markRed + 2] == transBlue ? 1 : 0);
                            SetPixel(smask, v, 0, 1, dstX, y, 1, yStride);
                            dstX += step;
                        }
                        break;
                    }
                }
            }
        }
        
        static int GetPixel(byte[] image, int x, int y, int bitDepth, int bytesPerRow) {
            if (bitDepth == 8) {
                int pos = bytesPerRow * y + x;
                return image[pos] & 0xff;
            }
            else {
                int pos = bytesPerRow * y + x / (8 / bitDepth);
                int v = image[pos] >> (8 - bitDepth * (x % (8 / bitDepth))- bitDepth);
                return v & ((1 << bitDepth) - 1);
            }
        }
        
        static void SetPixel(byte[] image, int[] data, int offset, int size, int x, int y, int bitDepth, int bytesPerRow) {
            if (bitDepth == 8) {
                int pos = bytesPerRow * y + size * x;
                for (int k = 0; k < size; ++k)
                    image[pos + k] = (byte)data[k + offset];
            }
            else if (bitDepth == 16) {
                int pos = bytesPerRow * y + size * x;
                for (int k = 0; k < size; ++k)
                    image[pos + k] = (byte)(data[k + offset] >> 8);
            }
            else {
                int pos = bytesPerRow * y + x / (8 / bitDepth);
                int v = data[offset] << (8 - bitDepth * (x % (8 / bitDepth))- bitDepth);
                image[pos] |= (byte)v;
            }
        }
        
        int[] GetPixel(byte[] curr) {
            switch (bitDepth) {
                case 8: {
                    int[] outp = new int[curr.Length];
                    for (int k = 0; k < outp.Length; ++k)
                        outp[k] = curr[k] & 0xff;
                    return outp;
                }
                case 16: {
                    int[] outp = new int[curr.Length / 2];
                    for (int k = 0; k < outp.Length; ++k)
                        outp[k] = ((curr[k * 2] & 0xff) << 8) + (curr[k * 2 + 1] & 0xff);
                    return outp;
                }
                default: {
                    int[] outp = new int[curr.Length * 8 / bitDepth];
                    int idx = 0;
                    int passes = 8 / bitDepth;
                    int mask = (1 << bitDepth) - 1;
                    for (int k = 0; k < curr.Length; ++k) {
                        for (int j = passes - 1; j >= 0; --j) {
                            outp[idx++] = Util.USR(curr[k], bitDepth * j) & mask; 
                        }
                    }
                    return outp;
                }
            }
        }
        
        private static void DecodeSubFilter(byte[] curr, int count, int bpp) {
            for (int i = bpp; i < count; i++) {
                int val;
                
                val = curr[i] & 0xff;
                val += curr[i - bpp] & 0xff;
                
                curr[i] = (byte)val;
            }
        }
        
        private static void DecodeUpFilter(byte[] curr, byte[] prev,
        int count) {
            for (int i = 0; i < count; i++) {
                int raw = curr[i] & 0xff;
                int prior = prev[i] & 0xff;
                
                curr[i] = (byte)(raw + prior);
            }
        }
        
        private static void DecodeAverageFilter(byte[] curr, byte[] prev,
        int count, int bpp) {
            int raw, priorPixel, priorRow;
            
            for (int i = 0; i < bpp; i++) {
                raw = curr[i] & 0xff;
                priorRow = prev[i] & 0xff;
                
                curr[i] = (byte)(raw + priorRow/2);
            }
            
            for (int i = bpp; i < count; i++) {
                raw = curr[i] & 0xff;
                priorPixel = curr[i - bpp] & 0xff;
                priorRow = prev[i] & 0xff;
                
                curr[i] = (byte)(raw + (priorPixel + priorRow)/2);
            }
        }
        
        private static int PaethPredictor(int a, int b, int c) {
            int p = a + b - c;
            int pa = Math.Abs(p - a);
            int pb = Math.Abs(p - b);
            int pc = Math.Abs(p - c);
            
            if ((pa <= pb) && (pa <= pc)) {
                return a;
            } else if (pb <= pc) {
                return b;
            } else {
                return c;
            }
        }
        
        private static void DecodePaethFilter(byte[] curr, byte[] prev,
        int count, int bpp) {
            int raw, priorPixel, priorRow, priorRowPixel;
            
            for (int i = 0; i < bpp; i++) {
                raw = curr[i] & 0xff;
                priorRow = prev[i] & 0xff;
                
                curr[i] = (byte)(raw + priorRow);
            }
            
            for (int i = bpp; i < count; i++) {
                raw = curr[i] & 0xff;
                priorPixel = curr[i - bpp] & 0xff;
                priorRow = prev[i] & 0xff;
                priorRowPixel = prev[i - bpp] & 0xff;
                
                curr[i] = (byte)(raw + PaethPredictor(priorPixel,
                priorRow,
                priorRowPixel));
            }
        }
        
    /**
    * Gets an <CODE>int</CODE> from an <CODE>Stream</CODE>.
    *
    * @param		is      an <CODE>Stream</CODE>
    * @return		the value of an <CODE>int</CODE>
    */
        
        public static int GetInt(Stream isp) {
            return (isp.ReadByte() << 24) + (isp.ReadByte() << 16) + (isp.ReadByte() << 8) + isp.ReadByte();
        }
        
    /**
    * Gets a <CODE>word</CODE> from an <CODE>Stream</CODE>.
    *
    * @param		is      an <CODE>Stream</CODE>
    * @return		the value of an <CODE>int</CODE>
    */
        
        public static int GetWord(Stream isp) {
            return (isp.ReadByte() << 8) + isp.ReadByte();
        }
        
    /**
    * Gets a <CODE>String</CODE> from an <CODE>Stream</CODE>.
    *
    * @param		is      an <CODE>Stream</CODE>
    * @return		the value of an <CODE>int</CODE>
    */
        
        public static String GetString(Stream isp) {
            StringBuilder buf = new StringBuilder();
            for (int i = 0; i < 4; i++) {
                buf.Append((char)isp.ReadByte());
            }
            return buf.ToString();
        }

        private static void ReadFully(Stream inp, byte[] b, int offset, int count) {
            while (count > 0) {
                int n = inp.Read(b, offset, count);
                if (n <= 0)
                    throw new IOException("Insufficient data.");
                count -= n;
                offset += n;
            }
        }
    }
}
