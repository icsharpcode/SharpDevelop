using System;
using System.Collections;
using System.IO;
using System.Net;
using System.util;
using iTextSharp.text;
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
 * This code was originally released in 2001 by SUN (see class
 * com.sun.media.imageioimpl.plugins.bmp.BMPImageReader.java)
 * using the BSD license in a specific wording. In a mail dating from
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
    /** Reads a BMP image. All types of BMP can be read.
    * <p>
    * It is based in the JAI codec.
    *
    * @author  Paulo Soares (psoares@consiste.pt)
    */
    public class BmpImage {
        
        // BMP variables
        private Stream inputStream;
        private long bitmapFileSize;
        private long bitmapOffset;
        private long compression;
        private long imageSize;
        private byte[] palette;
        private int imageType;
        private int numBands;
        private bool isBottomUp;
        private int bitsPerPixel;
        private int redMask, greenMask, blueMask, alphaMask;
        public Hashtable properties = new Hashtable();    
        private long xPelsPerMeter;
        private long yPelsPerMeter;
        // BMP Image types
        private const int VERSION_2_1_BIT = 0;
        private const int VERSION_2_4_BIT = 1;
        private const int VERSION_2_8_BIT = 2;
        private const int VERSION_2_24_BIT = 3;
        
        private const int VERSION_3_1_BIT = 4;
        private const int VERSION_3_4_BIT = 5;
        private const int VERSION_3_8_BIT = 6;
        private const int VERSION_3_24_BIT = 7;
        
        private const int VERSION_3_NT_16_BIT = 8;
        private const int VERSION_3_NT_32_BIT = 9;
        
        private const int VERSION_4_1_BIT = 10;
        private const int VERSION_4_4_BIT = 11;
        private const int VERSION_4_8_BIT = 12;
        private const int VERSION_4_16_BIT = 13;
        private const int VERSION_4_24_BIT = 14;
        private const int VERSION_4_32_BIT = 15;
        
        // Color space types
        private const int LCS_CALIBRATED_RGB = 0;
        private const int LCS_sRGB = 1;
        private const int LCS_CMYK = 2;
        
        // Compression Types
        private const int BI_RGB = 0;
        private const int BI_RLE8 = 1;
        private const int BI_RLE4 = 2;
        private const int BI_BITFIELDS = 3;
        
        int width;
        int height;
        
        internal BmpImage(Stream isp, bool noHeader, int size) {
            bitmapFileSize = size;
            bitmapOffset = 0;
            Process(isp, noHeader);
        }
        
        /** Reads a BMP from an url.
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
        
        /** Reads a BMP from a stream. The stream is not closed.
        * @param is the stream
        * @throws IOException on error
        * @return the image
        */    
        public static Image GetImage(Stream isp) {
            return GetImage(isp, false, 0);
        }
        
        /** Reads a BMP from a stream. The stream is not closed.
        * The BMP may not have a header and be considered as a plain DIB.
        * @param is the stream
        * @param noHeader true to process a plain DIB
        * @param size the size of the DIB. Not used for a BMP
        * @throws IOException on error
        * @return the image
        */    
        public static Image GetImage(Stream isp, bool noHeader, int size) {
            BmpImage bmp = new BmpImage(isp, noHeader, size);
            Image img = bmp.GetImage();
            img.SetDpi((int)((double)bmp.xPelsPerMeter * 0.0254 + 0.5), (int)((double)bmp.yPelsPerMeter * 0.0254 + 0.5));
            img.OriginalType = Image.ORIGINAL_BMP;
            return img;
        }
        
        /** Reads a BMP from a file.
        * @param file the file
        * @throws IOException on error
        * @return the image
        */    
        public static Image GetImage(String file) {
            return GetImage(Utilities.ToURL(file));
        }
        
        /** Reads a BMP from a byte array.
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

        
        protected void Process(Stream stream, bool noHeader) {
            if (noHeader || stream is BufferedStream) {
                inputStream = stream;
            } else {
                inputStream = new BufferedStream(stream);
            }
            if (!noHeader) {
                // Start File Header
                if (!(ReadUnsignedByte(inputStream) == 'B' &&
                ReadUnsignedByte(inputStream) == 'M')) {
                    throw new Exception("Invalid magic value for BMP file.");
                }

                // Read file size
                bitmapFileSize = ReadDWord(inputStream);

                // Read the two reserved fields
                ReadWord(inputStream);
                ReadWord(inputStream);

                // Offset to the bitmap from the beginning
                bitmapOffset = ReadDWord(inputStream);

                // End File Header
            }
            // Start BitmapCoreHeader
            long size = ReadDWord(inputStream);
            
            if (size == 12) {
                width = ReadWord(inputStream);
                height = ReadWord(inputStream);
            } else {
                width = ReadLong(inputStream);
                height = ReadLong(inputStream);
            }
            
            int planes = ReadWord(inputStream);
            bitsPerPixel = ReadWord(inputStream);
            
            properties["color_planes"] = planes;
            properties["bits_per_pixel"] = bitsPerPixel;
            
            // As BMP always has 3 rgb bands, except for Version 5,
            // which is bgra
            numBands = 3;
            if (bitmapOffset == 0)
                bitmapOffset = size;
            if (size == 12) {
                // Windows 2.x and OS/2 1.x
                properties["bmp_version"] = "BMP v. 2.x";
                
                // Classify the image type
                if (bitsPerPixel == 1) {
                    imageType = VERSION_2_1_BIT;
                } else if (bitsPerPixel == 4) {
                    imageType = VERSION_2_4_BIT;
                } else if (bitsPerPixel == 8) {
                    imageType = VERSION_2_8_BIT;
                } else if (bitsPerPixel == 24) {
                    imageType = VERSION_2_24_BIT;
                }
                
                // Read in the palette
                int numberOfEntries = (int)((bitmapOffset-14-size) / 3);
                int sizeOfPalette = numberOfEntries*3;
                if (bitmapOffset == size) {
                    switch (imageType) {
                        case VERSION_2_1_BIT:
                            sizeOfPalette = 2 * 3;
                            break;
                        case VERSION_2_4_BIT:
                            sizeOfPalette = 16 * 3;
                            break;
                        case VERSION_2_8_BIT:
                            sizeOfPalette = 256 * 3;
                            break;
                        case VERSION_2_24_BIT:
                            sizeOfPalette = 0;
                            break;
                    }
                    bitmapOffset = size + sizeOfPalette;
                }
                ReadPalette(sizeOfPalette);
            } else {
                
                compression = ReadDWord(inputStream);
                imageSize = ReadDWord(inputStream);
                xPelsPerMeter = ReadLong(inputStream);
                yPelsPerMeter = ReadLong(inputStream);
                long colorsUsed = ReadDWord(inputStream);
                long colorsImportant = ReadDWord(inputStream);
                
                switch ((int)compression) {
                    case BI_RGB:
                        properties["compression"] = "BI_RGB";
                        break;
                        
                    case BI_RLE8:
                        properties["compression"] = "BI_RLE8";
                        break;
                        
                    case BI_RLE4:
                        properties["compression"] = "BI_RLE4";
                        break;
                        
                    case BI_BITFIELDS:
                        properties["compression"] = "BI_BITFIELDS";
                        break;
                }
                
                properties["x_pixels_per_meter"] = xPelsPerMeter;
                properties["y_pixels_per_meter"] = yPelsPerMeter;
                properties["colors_used"] = colorsUsed;
                properties["colors_important"] = colorsImportant;
                
                if (size == 40) {
                    // Windows 3.x and Windows NT
                    switch ((int)compression) {
                        
                        case BI_RGB:  // No compression
                        case BI_RLE8:  // 8-bit RLE compression
                        case BI_RLE4:  // 4-bit RLE compression
                            
                            if (bitsPerPixel == 1) {
                                imageType = VERSION_3_1_BIT;
                            } else if (bitsPerPixel == 4) {
                                imageType = VERSION_3_4_BIT;
                            } else if (bitsPerPixel == 8) {
                                imageType = VERSION_3_8_BIT;
                            } else if (bitsPerPixel == 24) {
                                imageType = VERSION_3_24_BIT;
                            } else if (bitsPerPixel == 16) {
                                imageType = VERSION_3_NT_16_BIT;
                                redMask = 0x7C00;
                                greenMask = 0x3E0;
                                blueMask = 0x1F;
                                properties["red_mask"] = redMask;
                                properties["green_mask"] = greenMask;
                                properties["blue_mask"] = blueMask;
                            } else if (bitsPerPixel == 32) {
                                imageType = VERSION_3_NT_32_BIT;
                                redMask   = 0x00FF0000;
                                greenMask = 0x0000FF00;
                                blueMask  = 0x000000FF;
                                properties["red_mask"] = redMask;
                                properties["green_mask"] = greenMask;
                                properties["blue_mask"] = blueMask;
                            }

                            // Read in the palette
                            int numberOfEntries = (int)((bitmapOffset-14-size) / 4);
                            int sizeOfPalette = numberOfEntries*4;
                            if (bitmapOffset == size) {
                                switch (imageType) {
                                    case VERSION_3_1_BIT:
                                        sizeOfPalette = (int)(colorsUsed == 0 ? 2 : colorsUsed) * 4;
                                        break;
                                    case VERSION_3_4_BIT:
                                        sizeOfPalette = (int)(colorsUsed == 0 ? 16 : colorsUsed) * 4;
                                        break;
                                    case VERSION_3_8_BIT:
                                        sizeOfPalette = (int)(colorsUsed == 0 ? 256 : colorsUsed) * 4;
                                        break;
                                    default:
                                        sizeOfPalette = 0;
                                        break;
                                }
                                bitmapOffset = size + sizeOfPalette;
                            }
                            ReadPalette(sizeOfPalette);
                            properties["bmp_version"] = "BMP v. 3.x";
                            break;
                            
                        case BI_BITFIELDS:
                            
                            if (bitsPerPixel == 16) {
                                imageType = VERSION_3_NT_16_BIT;
                            } else if (bitsPerPixel == 32) {
                                imageType = VERSION_3_NT_32_BIT;
                            }
                            
                            // BitsField encoding
                            redMask = (int)ReadDWord(inputStream);
                            greenMask = (int)ReadDWord(inputStream);
                            blueMask = (int)ReadDWord(inputStream);
                            
                            properties["red_mask"] = redMask;
                            properties["green_mask"] = greenMask;
                            properties["blue_mask"] = blueMask;
                            
                            if (colorsUsed != 0) {
                                // there is a palette
                                sizeOfPalette = (int)colorsUsed*4;
                                ReadPalette(sizeOfPalette);
                            }
                            
                            properties["bmp_version"] = "BMP v. 3.x NT";
                            break;
                            
                        default:
                            throw new
                            Exception("Invalid compression specified in BMP file.");
                    }
                } else if (size == 108) {
                    // Windows 4.x BMP
                    
                    properties["bmp_version"] = "BMP v. 4.x";
                    
                    // rgb masks, valid only if comp is BI_BITFIELDS
                    redMask = (int)ReadDWord(inputStream);
                    greenMask = (int)ReadDWord(inputStream);
                    blueMask = (int)ReadDWord(inputStream);
                    // Only supported for 32bpp BI_RGB argb
                    alphaMask = (int)ReadDWord(inputStream);
                    long csType = ReadDWord(inputStream);
                    int redX = ReadLong(inputStream);
                    int redY = ReadLong(inputStream);
                    int redZ = ReadLong(inputStream);
                    int greenX = ReadLong(inputStream);
                    int greenY = ReadLong(inputStream);
                    int greenZ = ReadLong(inputStream);
                    int blueX = ReadLong(inputStream);
                    int blueY = ReadLong(inputStream);
                    int blueZ = ReadLong(inputStream);
                    long gammaRed = ReadDWord(inputStream);
                    long gammaGreen = ReadDWord(inputStream);
                    long gammaBlue = ReadDWord(inputStream);
                    
                    if (bitsPerPixel == 1) {
                        imageType = VERSION_4_1_BIT;
                    } else if (bitsPerPixel == 4) {
                        imageType = VERSION_4_4_BIT;
                    } else if (bitsPerPixel == 8) {
                        imageType = VERSION_4_8_BIT;
                    } else if (bitsPerPixel == 16) {
                        imageType = VERSION_4_16_BIT;
                        if ((int)compression == BI_RGB) {
                            redMask = 0x7C00;
                            greenMask = 0x3E0;
                            blueMask = 0x1F;
                        }
                    } else if (bitsPerPixel == 24) {
                        imageType = VERSION_4_24_BIT;
                    } else if (bitsPerPixel == 32) {
                        imageType = VERSION_4_32_BIT;
                        if ((int)compression == BI_RGB) {
                            redMask   = 0x00FF0000;
                            greenMask = 0x0000FF00;
                            blueMask  = 0x000000FF;
                        }
                    }
                    
                    properties["red_mask"] = redMask;
                    properties["green_mask"] = greenMask;
                    properties["blue_mask"] = blueMask;
                    properties["alpha_mask"] = alphaMask;

                    // Read in the palette
                    int numberOfEntries = (int)((bitmapOffset-14-size) / 4);
                    int sizeOfPalette = numberOfEntries*4;
                    if (bitmapOffset == size) {
                        switch (imageType) {
                            case VERSION_4_1_BIT:
                                sizeOfPalette = (int)(colorsUsed == 0 ? 2 : colorsUsed) * 4;
                                break;
                            case VERSION_4_4_BIT:
                                sizeOfPalette = (int)(colorsUsed == 0 ? 16 : colorsUsed) * 4;
                                break;
                            case VERSION_4_8_BIT:
                                sizeOfPalette = (int)(colorsUsed == 0 ? 256 : colorsUsed) * 4;
                                break;
                            default:
                                sizeOfPalette = 0;
                                break;
                        }
                        bitmapOffset = size + sizeOfPalette;
                    }
                    ReadPalette(sizeOfPalette);
                    
                    switch ((int)csType) {
                        case LCS_CALIBRATED_RGB:
                            // All the new fields are valid only for this case
                            properties["color_space"] = "LCS_CALIBRATED_RGB";
                            properties["redX"] = redX;
                            properties["redY"] = redY;
                            properties["redZ"] = redZ;
                            properties["greenX"] = greenX;
                            properties["greenY"] = greenY;
                            properties["greenZ"] = greenZ;
                            properties["blueX"] = blueX;
                            properties["blueY"] = blueY;
                            properties["blueZ"] = blueZ;
                            properties["gamma_red"] = gammaRed;
                            properties["gamma_green"] = gammaGreen;
                            properties["gamma_blue"] = gammaBlue;
                            
                            // break;
                            throw new
                            Exception("Not implemented yet.");
                            
                        case LCS_sRGB:
                            // Default Windows color space
                            properties["color_space"] = "LCS_sRGB";
                            break;
                            
                        case LCS_CMYK:
                            properties["color_space"] = "LCS_CMYK";
                            //		    break;
                            throw new
                            Exception("Not implemented yet.");
                    }
                    
                } else {
                    properties["bmp_version"] = "BMP v. 5.x";
                    throw new
                    Exception("BMP version 5 not implemented yet.");
                }
            }
            
            if (height > 0) {
                // bottom up image
                isBottomUp = true;
            } else {
                // top down image
                isBottomUp = false;
                height = Math.Abs(height);
            }
            // When number of bitsPerPixel is <= 8, we use IndexColorModel.
            if (bitsPerPixel == 1 || bitsPerPixel == 4 || bitsPerPixel == 8) {
                
                numBands = 1;
                
                
                // Create IndexColorModel from the palette.
                byte[] r;
                byte[] g;
                byte[] b;
                int sizep;
                if (imageType == VERSION_2_1_BIT ||
                imageType == VERSION_2_4_BIT ||
                imageType == VERSION_2_8_BIT) {
                    
                    sizep = palette.Length/3;
                    
                    if (sizep > 256) {
                        sizep = 256;
                    }
                    
                    int off;
                    r = new byte[sizep];
                    g = new byte[sizep];
                    b = new byte[sizep];
                    for (int i=0; i<sizep; i++) {
                        off = 3 * i;
                        b[i] = palette[off];
                        g[i] = palette[off+1];
                        r[i] = palette[off+2];
                    }
                } else {
                    sizep = palette.Length/4;
                    
                    if (sizep > 256) {
                        sizep = 256;
                    }
                    
                    int off;
                    r = new byte[sizep];
                    g = new byte[sizep];
                    b = new byte[sizep];
                    for (int i=0; i<sizep; i++) {
                        off = 4 * i;
                        b[i] = palette[off];
                        g[i] = palette[off+1];
                        r[i] = palette[off+2];
                    }
                }
                
            } else if (bitsPerPixel == 16) {
                numBands = 3;
            } else if (bitsPerPixel == 32) {
                numBands = alphaMask == 0 ? 3 : 4;
            } else {
                numBands = 3;
            }
        }
        
        private byte[] GetPalette(int group) {
            if (palette == null)
                return null;
            byte[] np = new byte[palette.Length / group * 3];
            int e = palette.Length / group;
            for (int k = 0; k < e; ++k) {
                int src = k * group;
                int dest = k * 3;
                np[dest + 2] = palette[src++];
                np[dest + 1] = palette[src++];
                np[dest] = palette[src];
            }
            return np;
        }
        
        private Image GetImage() {
            byte[] bdata = null; // buffer for byte data
            //short[] sdata = null; // buffer for short data
            //int[] idata = null; // buffer for int data
            
            //	if (sampleModel.GetDataType() == DataBuffer.TYPE_BYTE)
            //	    bdata = (byte[])((DataBufferByte)tile.GetDataBuffer()).GetData();
            //	else if (sampleModel.GetDataType() == DataBuffer.TYPE_USHORT)
            //	    sdata = (short[])((DataBufferUShort)tile.GetDataBuffer()).GetData();
            //	else if (sampleModel.GetDataType() == DataBuffer.TYPE_INT)
            //	    idata = (int[])((DataBufferInt)tile.GetDataBuffer()).GetData();
            
            // There should only be one tile.
            switch (imageType) {
                
                case VERSION_2_1_BIT:
                    // no compression
                    return Read1Bit(3);
                    
                case VERSION_2_4_BIT:
                    // no compression
                    return Read4Bit(3);
                    
                case VERSION_2_8_BIT:
                    // no compression
                    return Read8Bit(3);
                    
                case VERSION_2_24_BIT:
                    // no compression
                    bdata = new byte[width * height * 3];
                    Read24Bit(bdata);
                    return new ImgRaw(width, height, 3, 8, bdata);
                    
                case VERSION_3_1_BIT:
                    // 1-bit images cannot be compressed.
                    return Read1Bit(4);
                    
                case VERSION_3_4_BIT:
                    switch ((int)compression) {
                        case BI_RGB:
                            return Read4Bit(4);
                            
                        case BI_RLE4:
                            return ReadRLE4();
                            
                        default:
                            throw new
                            Exception("Invalid compression specified for BMP file.");
                    }
                    
                case VERSION_3_8_BIT:
                    switch ((int)compression) {
                        case BI_RGB:
                            return Read8Bit(4);
                            
                        case BI_RLE8:
                            return ReadRLE8();
                            
                        default:
                            throw new
                            Exception("Invalid compression specified for BMP file.");
                    }
                    
                case VERSION_3_24_BIT:
                    // 24-bit images are not compressed
                    bdata = new byte[width * height * 3];
                    Read24Bit(bdata);
                    return new ImgRaw(width, height, 3, 8, bdata);
                    
                case VERSION_3_NT_16_BIT:
                    return Read1632Bit(false);
                    
                case VERSION_3_NT_32_BIT:
                    return Read1632Bit(true);
                    
                case VERSION_4_1_BIT:
                    return Read1Bit(4);
                    
                case VERSION_4_4_BIT:
                    switch ((int)compression) {
                        
                        case BI_RGB:
                            return Read4Bit(4);
                            
                        case BI_RLE4:
                            return ReadRLE4();
                            
                        default:
                            throw new
                            Exception("Invalid compression specified for BMP file.");
                    }
                    
                case VERSION_4_8_BIT:
                    switch ((int)compression) {
                        
                        case BI_RGB:
                            return Read8Bit(4);
                            
                        case BI_RLE8:
                            return ReadRLE8();
                            
                        default:
                            throw new
                            Exception("Invalid compression specified for BMP file.");
                    }
                    
                case VERSION_4_16_BIT:
                    return Read1632Bit(false);
                    
                case VERSION_4_24_BIT:
                    bdata = new byte[width * height * 3];
                    Read24Bit(bdata);
                    return new ImgRaw(width, height, 3, 8, bdata);
                    
                case VERSION_4_32_BIT:
                    return Read1632Bit(true);
            }
            return null;
        }
        
        private Image IndexedModel(byte[] bdata, int bpc, int paletteEntries) {
            Image img = new ImgRaw(width, height, 1, bpc, bdata);
            PdfArray colorspace = new PdfArray();
            colorspace.Add(PdfName.INDEXED);
            colorspace.Add(PdfName.DEVICERGB);
            byte[] np = GetPalette(paletteEntries);
            int len = np.Length;
            colorspace.Add(new PdfNumber(len / 3 - 1));
            colorspace.Add(new PdfString(np));
            PdfDictionary ad = new PdfDictionary();
            ad.Put(PdfName.COLORSPACE, colorspace);
            img.Additional = ad;
            return img;
        }
        
        private void ReadPalette(int sizeOfPalette) {
            if (sizeOfPalette == 0) {
                return;
            }
            palette = new byte[sizeOfPalette];
            int bytesRead = 0;
            while (bytesRead < sizeOfPalette) {
                int r = inputStream.Read(palette, bytesRead, sizeOfPalette - bytesRead);
                if (r <= 0) {
                    throw new IOException("incomplete palette");
                }
                bytesRead += r;
            }
            properties["palette"] = palette;
        }

        // Deal with 1 Bit images using IndexColorModels
        private Image Read1Bit(int paletteEntries) {
            byte[] bdata = new byte[((width + 7) / 8) * height];
            int padding = 0;
            int bytesPerScanline = (int)Math.Ceiling((double)width/8.0);
            
            int remainder = bytesPerScanline % 4;
            if (remainder != 0) {
                padding = 4 - remainder;
            }
            
            int imSize = (bytesPerScanline + padding) * height;
            
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += inputStream.Read(values, bytesRead,
                imSize - bytesRead);
            }
            
            if (isBottomUp) {
                
                // Convert the bottom up image to a top down format by copying
                // one scanline from the bottom to the top at a time.
                
                for (int i=0; i<height; i++) {
                    Array.Copy(values,
                    imSize - (i+1)*(bytesPerScanline + padding),
                    bdata,
                    i*bytesPerScanline, bytesPerScanline);
                }
            } else {
                
                for (int i=0; i<height; i++) {
                    Array.Copy(values,
                    i * (bytesPerScanline + padding),
                    bdata,
                    i * bytesPerScanline,
                    bytesPerScanline);
                }
            }
            return IndexedModel(bdata, 1, paletteEntries);
        }
        
        // Method to read a 4 bit BMP image data
        private Image Read4Bit(int paletteEntries) {
            byte[] bdata = new byte[((width + 1) / 2) * height];
            
            // Padding bytes at the end of each scanline
            int padding = 0;
            
            int bytesPerScanline = (int)Math.Ceiling((double)width/2.0);
            int remainder = bytesPerScanline % 4;
            if (remainder != 0) {
                padding = 4 - remainder;
            }
            
            int imSize = (bytesPerScanline + padding) * height;
            
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += inputStream.Read(values, bytesRead,
                imSize - bytesRead);
            }
            
            if (isBottomUp) {
                
                // Convert the bottom up image to a top down format by copying
                // one scanline from the bottom to the top at a time.
                for (int i=0; i<height; i++) {
                    Array.Copy(values,
                    imSize - (i+1)*(bytesPerScanline + padding),
                    bdata,
                    i*bytesPerScanline,
                    bytesPerScanline);
                }
            } else {
                for (int i=0; i<height; i++) {
                    Array.Copy(values,
                    i * (bytesPerScanline + padding),
                    bdata,
                    i * bytesPerScanline,
                    bytesPerScanline);
                }
            }
            return IndexedModel(bdata, 4, paletteEntries);
        }
        
        // Method to read 8 bit BMP image data
        private Image Read8Bit(int paletteEntries) {
            byte[] bdata = new byte[width * height];
            // Padding bytes at the end of each scanline
            int padding = 0;
            
            // width * bitsPerPixel should be divisible by 32
            int bitsPerScanline = width * 8;
            if ( bitsPerScanline%32 != 0) {
                padding = (bitsPerScanline/32 + 1)*32 - bitsPerScanline;
                padding = (int)Math.Ceiling(padding/8.0);
            }
            
            int imSize = (width + padding) * height;
            
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += inputStream.Read(values, bytesRead, imSize - bytesRead);
            }
            
            if (isBottomUp) {
                
                // Convert the bottom up image to a top down format by copying
                // one scanline from the bottom to the top at a time.
                for (int i=0; i<height; i++) {
                    Array.Copy(values,
                    imSize - (i+1) * (width + padding),
                    bdata,
                    i * width,
                    width);
                }
            } else {
                for (int i=0; i<height; i++) {
                    Array.Copy(values,
                    i * (width + padding),
                    bdata,
                    i * width,
                    width);
                }
            }
            return IndexedModel(bdata, 8, paletteEntries);
        }
        
        // Method to read 24 bit BMP image data
        private void Read24Bit(byte[] bdata) {
            // Padding bytes at the end of each scanline
            int padding = 0;
            
            // width * bitsPerPixel should be divisible by 32
            int bitsPerScanline = width * 24;
            if ( bitsPerScanline%32 != 0) {
                padding = (bitsPerScanline/32 + 1)*32 - bitsPerScanline;
                padding = (int)Math.Ceiling(padding/8.0);
            }
            
            
            int imSize = ((width * 3 + 3) / 4 * 4) * height;
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                int r = inputStream.Read(values, bytesRead,
                imSize - bytesRead);
                if (r < 0)
                    break;
                bytesRead += r;
            }
            
            int l=0, count;
            
            if (isBottomUp) {
                int max = width*height*3-1;
                
                count = -padding;
                for (int i=0; i<height; i++) {
                    l = max - (i+1)*width*3 + 1;
                    count += padding;
                    for (int j=0; j<width; j++) {
                        bdata[l + 2] = values[count++];
                        bdata[l + 1] = values[count++];
                        bdata[l] = values[count++];
                        l += 3;
                    }
                }
            } else {
                count = -padding;
                for (int i=0; i<height; i++) {
                    count += padding;
                    for (int j=0; j<width; j++) {
                        bdata[l + 2] = values[count++];
                        bdata[l + 1] = values[count++];
                        bdata[l] = values[count++];
                        l += 3;
                    }
                }
            }
        }
        
        private int FindMask(int mask) {
            int k = 0;
            for (; k < 32; ++k) {
                if ((mask & 1) == 1)
                    break;
                mask = Util.USR(mask, 1);
            }
            return mask;
        }
        
        private int FindShift(int mask) {
            int k = 0;
            for (; k < 32; ++k) {
                if ((mask & 1) == 1)
                    break;
                mask = Util.USR(mask, 1);
            }
            return k;
        }
        
        private Image Read1632Bit(bool is32) {
            
            int red_mask = FindMask(redMask);
            int red_shift = FindShift(redMask);
            int red_factor = red_mask + 1;
            int green_mask = FindMask(greenMask);
            int green_shift = FindShift(greenMask);
            int green_factor = green_mask + 1;
            int blue_mask = FindMask(blueMask);
            int blue_shift = FindShift(blueMask);
            int blue_factor = blue_mask + 1;
            byte[] bdata = new byte[width * height * 3];
            // Padding bytes at the end of each scanline
            int padding = 0;
            
            if (!is32) {
            // width * bitsPerPixel should be divisible by 32
                int bitsPerScanline = width * 16;
                if ( bitsPerScanline%32 != 0) {
                    padding = (bitsPerScanline/32 + 1)*32 - bitsPerScanline;
                    padding = (int)Math.Ceiling(padding/8.0);
                }
            }
            
            int imSize = (int)imageSize;
            if (imSize == 0) {
                imSize = (int)(bitmapFileSize - bitmapOffset);
            }
            
            int l=0;
            int v;
            if (isBottomUp) {
                for (int i=height - 1; i >= 0; --i) {
                    l = width * 3 * i;
                    for (int j=0; j<width; j++) {
                        if (is32)
                            v = (int)ReadDWord(inputStream);
                        else
                            v = ReadWord(inputStream);
                        bdata[l++] = (byte)((Util.USR(v, red_shift) & red_mask) * 256 / red_factor);
                        bdata[l++] = (byte)((Util.USR(v, green_shift) & green_mask) * 256 / green_factor);
                        bdata[l++] = (byte)((Util.USR(v, blue_shift) & blue_mask) * 256 / blue_factor);
                    }
                    for (int m=0; m<padding; m++) {
                        inputStream.ReadByte();
                    }
                }
            } else {
                for (int i=0; i<height; i++) {
                    for (int j=0; j<width; j++) {
                        if (is32)
                            v = (int)ReadDWord(inputStream);
                        else
                            v = ReadWord(inputStream);
                        bdata[l++] = (byte)((Util.USR(v, red_shift) & red_mask) * 256 / red_factor);
                        bdata[l++] = (byte)((Util.USR(v, green_shift) & green_mask) * 256 / green_factor);
                        bdata[l++] = (byte)((Util.USR(v, blue_shift) & blue_mask) * 256 / blue_factor);
                    }
                    for (int m=0; m<padding; m++) {
                        inputStream.ReadByte();
                    }
                }
            }
            return new ImgRaw(width, height, 3, 8, bdata);
        }
        
        private Image ReadRLE8() {
            
            // If imageSize field is not provided, calculate it.
            int imSize = (int)imageSize;
            if (imSize == 0) {
                imSize = (int)(bitmapFileSize - bitmapOffset);
            }
                        
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += inputStream.Read(values, bytesRead,
                imSize - bytesRead);
            }
            
            // Since data is compressed, decompress it
            byte[] val = DecodeRLE(true, values);
            
            // Uncompressed data does not have any padding
            imSize = width * height;
            
            if (isBottomUp) {
                
                // Convert the bottom up image to a top down format by copying
                // one scanline from the bottom to the top at a time.
                // int bytesPerScanline = (int)Math.Ceil((double)width/8.0);
                byte[] temp = new byte[val.Length];
                int bytesPerScanline = width;
                for (int i=0; i<height; i++) {
                    Array.Copy(val,
                    imSize - (i+1)*(bytesPerScanline),
                    temp,
                    i*bytesPerScanline, bytesPerScanline);
                }
                val = temp;
            }
            return IndexedModel(val, 8, 4);
        }
        
        private Image ReadRLE4() {
            
            // If imageSize field is not specified, calculate it.
            int imSize = (int)imageSize;
            if (imSize == 0) {
                imSize = (int)(bitmapFileSize - bitmapOffset);
            }
            
            // Read till we have the whole image
            byte[] values = new byte[imSize];
            int bytesRead = 0;
            while (bytesRead < imSize) {
                bytesRead += inputStream.Read(values, bytesRead,
                imSize - bytesRead);
            }
            
            // Decompress the RLE4 compressed data.
            byte[] val = DecodeRLE(false, values);
            
            // Invert it as it is bottom up format.
            if (isBottomUp) {
                
                byte[] inverted = val;
                val = new byte[width * height];
                int l = 0, index, lineEnd;
                
                for (int i = height-1; i >= 0; i--) {
                    index = i * width;
                    lineEnd = l + width;
                    while (l != lineEnd) {
                        val[l++] = inverted[index++];
                    }
                }
            }
            int stride = ((width + 1) / 2);
            byte[] bdata = new byte[stride * height];
            int ptr = 0;
            int sh = 0;
            for (int h = 0; h < height; ++h) {
                for (int w = 0; w < width; ++w) {
                    if ((w & 1) == 0)
                        bdata[sh + w / 2] = (byte)(val[ptr++] << 4);
                    else
                        bdata[sh + w / 2] |= (byte)(val[ptr++] & 0x0f);
                }
                sh += stride;
            }
            return IndexedModel(bdata, 4, 4);
        }
        
        private byte[] DecodeRLE(bool is8, byte[] values) {
            byte[] val = new byte[width * height];
            try {
                int ptr = 0;
                int x = 0;
                int q = 0;
                for (int y = 0; y < height && ptr < values.Length;) {
                    int count = values[ptr++] & 0xff;
                    if (count != 0) {
                        // encoded mode
                        int bt = values[ptr++] & 0xff;
                        if (is8) {
                            for (int i = count; i != 0; --i) {
                                val[q++] = (byte)bt;
                            }
                        }
                        else {
                            for (int i = 0; i < count; ++i) {
                                val[q++] = (byte)((i & 1) == 1 ? (bt & 0x0f) : ((bt >> 4) & 0x0f));
                            }
                        }
                        x += count;
                    }
                    else {
                        // escape mode
                        count = values[ptr++] & 0xff;
                        if (count == 1)
                            break;
                        switch (count) {
                            case 0:
                                x = 0;
                                ++y;
                                q = y * width;
                                break;
                            case 2:
                                // delta mode
                                x += values[ptr++] & 0xff;
                                y += values[ptr++] & 0xff;
                                q = y * width + x;
                                break;
                            default:
                                // absolute mode
                                if (is8) {
                                    for (int i = count; i != 0; --i)
                                        val[q++] = (byte)(values[ptr++] & 0xff);
                                }
                                else {
                                    int bt = 0;
                                    for (int i = 0; i < count; ++i) {
                                        if ((i & 1) == 0)
                                            bt = values[ptr++] & 0xff;
                                        val[q++] = (byte)((i & 1) == 1 ? (bt & 0x0f) : ((bt >> 4) & 0x0f));
                                    }
                                }
                                x += count;
                                // read pad byte
                                if (is8) {
                                    if ((count & 1) == 1)
                                        ++ptr;
                                }
                                else {
                                    if ((count & 3) == 1 || (count & 3) == 2)
                                        ++ptr;
                                }
                                break;
                        }
                    }
                }
            }
            catch {
                //empty on purpose
            }
            
            return val;
        }
        
        // Windows defined data type reading methods - everything is little endian
        
        // Unsigned 8 bits
        private int ReadUnsignedByte(Stream stream) {
            return (stream.ReadByte() & 0xff);
        }
        
        // Unsigned 2 bytes
        private int ReadUnsignedShort(Stream stream) {
            int b1 = ReadUnsignedByte(stream);
            int b2 = ReadUnsignedByte(stream);
            return ((b2 << 8) | b1) & 0xffff;
        }
        
        // Signed 16 bits
        private int ReadShort(Stream stream) {
            int b1 = ReadUnsignedByte(stream);
            int b2 = ReadUnsignedByte(stream);
            return (b2 << 8) | b1;
        }
        
        // Unsigned 16 bits
        private int ReadWord(Stream stream) {
            return ReadUnsignedShort(stream);
        }
        
        // Unsigned 4 bytes
        private long ReadUnsignedInt(Stream stream) {
            int b1 = ReadUnsignedByte(stream);
            int b2 = ReadUnsignedByte(stream);
            int b3 = ReadUnsignedByte(stream);
            int b4 = ReadUnsignedByte(stream);
            long l = (long)((b4 << 24) | (b3 << 16) | (b2 << 8) | b1);
            return l & 0xffffffff;
        }
        
        // Signed 4 bytes
        private int ReadInt(Stream stream) {
            int b1 = ReadUnsignedByte(stream);
            int b2 = ReadUnsignedByte(stream);
            int b3 = ReadUnsignedByte(stream);
            int b4 = ReadUnsignedByte(stream);
            return (b4 << 24) | (b3 << 16) | (b2 << 8) | b1;
        }
        
        // Unsigned 4 bytes
        private long ReadDWord(Stream stream) {
            return ReadUnsignedInt(stream);
        }
        
        // 32 bit signed value
        private int ReadLong(Stream stream) {
            return ReadInt(stream);
        }
    }
}
