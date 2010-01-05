using System;
using System.IO;
using System.Net;
using System.util;
using iTextSharp.text.pdf;
/*
 * $Id: Jpeg2000.cs,v 1.5 2008/05/13 11:25:11 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text {

    /**
    * An <CODE>Jpeg2000</CODE> is the representation of a graphic element (JPEG)
    * that has to be inserted into the document
    *
    * @see		Element
    * @see		Image
    */

    public class Jpeg2000 : Image {
        
        // public static final membervariables
        
        public const int JP2_JP = 0x6a502020;
        public const int JP2_IHDR = 0x69686472;
        public const int JPIP_JPIP = 0x6a706970;

        public const int JP2_FTYP = 0x66747970;
        public const int JP2_JP2H = 0x6a703268;
        public const int JP2_COLR = 0x636f6c72;
        public const int JP2_JP2C = 0x6a703263;
        public const int JP2_URL = 0x75726c20;
        public const int JP2_DBTL = 0x6474626c;
        public const int JP2_BPCC = 0x62706363;
        public const int JP2_JP2 = 0x6a703220;

        private Stream inp;
        private int boxLength;
        private int boxType;
        
        // Constructors
        
        public Jpeg2000(Image image) : base(image) {
        }

        /**
        * Constructs a <CODE>Jpeg2000</CODE>-object, using an <VAR>url</VAR>.
        *
        * @param		url			the <CODE>URL</CODE> where the image can be found
        * @throws BadElementException
        * @throws IOException
        */
        public Jpeg2000(Uri url) : base(url) {
            ProcessParameters();
        }
        
        /**
        * Constructs a <CODE>Jpeg2000</CODE>-object from memory.
        *
        * @param		img		the memory image
        * @throws BadElementException
        * @throws IOException
        */
        
        public Jpeg2000(byte[] img) : base((Uri)null) {
            rawData = img;
            originalData = img;
            ProcessParameters();
        }
        
        /**
        * Constructs a <CODE>Jpeg2000</CODE>-object from memory.
        *
        * @param		img			the memory image.
        * @param		width		the width you want the image to have
        * @param		height		the height you want the image to have
        * @throws BadElementException
        * @throws IOException
        */
        
        public Jpeg2000(byte[] img, float width, float height) : this(img) {
            scaledWidth = width;
            scaledHeight = height;
        }
        
        private int Cio_read(int n) {
            int v = 0;
            for (int i = n - 1; i >= 0; i--) {
                v += inp.ReadByte() << (i << 3);
            }
            return v;
        }
        
        public void Jp2_read_boxhdr() {
            boxLength = Cio_read(4);
            boxType = Cio_read(4);
            if (boxLength == 1) {
                if (Cio_read(4) != 0) {
                    throw new IOException("Cannot handle box sizes higher than 2^32");
                }
                boxLength = Cio_read(4);
                if (boxLength == 0) 
                    throw new IOException("Unsupported box size == 0");
            }
            else if (boxLength == 0) {
                throw new IOException("Unsupported box size == 0");
            }
        }
        
        /**
        * This method checks if the image is a valid JPEG and processes some parameters.
        * @throws BadElementException
        * @throws IOException
        */
        private void ProcessParameters() {
            type = JPEG2000;
            originalType = ORIGINAL_JPEG2000;
            inp = null;
            try {
                string errorID;
                if (rawData == null){
                    WebRequest w = WebRequest.Create(url);
                    inp = w.GetResponse().GetResponseStream();
                    errorID = url.ToString();
                }
                else{
                    inp = new MemoryStream(rawData);
                    errorID = "Byte array";
                }
                boxLength = Cio_read(4);
                if (boxLength == 0x0000000c) {
                    boxType = Cio_read(4);
                    if (JP2_JP != boxType) {
                        throw new IOException("Expected JP Marker");
                    }
                    if (0x0d0a870a != Cio_read(4)) {
                        throw new IOException("Error with JP Marker");
                    }

                    Jp2_read_boxhdr();
                    if (JP2_FTYP != boxType) {
                        throw new IOException("Expected FTYP Marker");
                    }
                    Utilities.Skip(inp, boxLength - 8);
                    Jp2_read_boxhdr();
                    do {
                        if (JP2_JP2H != boxType) {
                            if (boxType == JP2_JP2C) {
                                throw new IOException("Expected JP2H Marker");
                            }
                            Utilities.Skip(inp, boxLength - 8);
                            Jp2_read_boxhdr();
                        }
                    } while (JP2_JP2H != boxType);
                    Jp2_read_boxhdr();
                    if (JP2_IHDR != boxType) {
                        throw new IOException("Expected IHDR Marker");
                    }
                    scaledHeight = Cio_read(4);
                    Top = scaledHeight;
                    scaledWidth = Cio_read(4);
                    Right = scaledWidth;
                    bpc = -1;
                }
                else if ((uint)boxLength == 0xff4fff51) {
                    Utilities.Skip(inp, 4);
                    int x1 = Cio_read(4);
                    int y1 = Cio_read(4);
                    int x0 = Cio_read(4);
                    int y0 = Cio_read(4);
                    Utilities.Skip(inp, 16);
                    colorspace = Cio_read(2);
                    bpc = 8;
                    scaledHeight = y1 - y0;
                    Top = scaledHeight;
                    scaledWidth = x1 - x0;
                    Right = scaledWidth;
                }
                else {
                    throw new IOException("Not a valid Jpeg2000 file");
                }
            }
            finally {
                if (inp != null) {
                    try{inp.Close();}catch{}
                    inp = null;
                }
            }
            plainWidth = this.Width;
            plainHeight = this.Height;
        }
    }
}