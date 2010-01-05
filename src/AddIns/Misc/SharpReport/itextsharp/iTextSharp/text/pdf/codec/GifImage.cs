using System;
using System.Net;
using System.IO;
using System.Collections;
using iTextSharp.text;
using iTextSharp.text.pdf;
/*
 * Copyright 2003 by Paulo Soares.
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License isp distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code isp 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code isp Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999, 2000, 2001, 2002 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code isp Paulo Soares. Portions created by the Co-Developer
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
 * This library isp free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General Public License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library isp distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text.pdf.codec {
    /** Reads gif images of all types. All the images in a gif are read in the constructors
    * and can be retrieved with other methods.
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class GifImage {
        
        protected Stream inp;
        protected int width;            // full image width
        protected int height;           // full image height
        protected bool gctFlag;      // global color table used

        protected int bgIndex;          // background color index
        protected int bgColor;          // background color
        protected int pixelAspect;      // pixel aspect ratio

        protected bool lctFlag;      // local color table flag
        protected bool interlace;    // interlace flag
        protected int lctSize;          // local color table size

        protected int ix, iy, iw, ih;   // current image rectangle

        protected byte[] block = new byte[256];  // current data block
        protected int blockSize = 0;    // block size

        // last graphic control extension info
        protected int dispose = 0;   // 0=no action; 1=leave in place; 2=restore to bg; 3=restore to prev
        protected bool transparency = false;   // use transparent color
        protected int delay = 0;        // delay in milliseconds
        protected int transIndex;       // transparent color index

        protected const int MaxStackSize = 4096;   // max decoder pixel stack size

        // LZW decoder working arrays
        protected short[] prefix;
        protected byte[] suffix;
        protected byte[] pixelStack;
        protected byte[] pixels;

        protected byte[] m_out;
        protected int m_bpc;
        protected int m_gbpc;
        protected byte[] m_global_table;
        protected byte[] m_local_table;
        protected byte[] m_curr_table;
        protected int m_line_stride;
        protected byte[] fromData;
        protected Uri fromUrl;


        protected ArrayList frames = new ArrayList();     // frames read from current file

        /** Reads gif images from an URL.
        * @param url the URL
        * @throws IOException on error
        */    
        public GifImage(Uri url) {
            fromUrl = url;
            Stream isp = null;
            try {
                isp = WebRequest.Create(url).GetResponse().GetResponseStream();
                Process(isp);
            }
            finally {
                if (isp != null) {
                    isp.Close();
                }
            }
        }
        
        /** Reads gif images from a file.
        * @param file the file
        * @throws IOException on error
        */    
        public GifImage(String file) : this(Utilities.ToURL(file)) {
        }
        
        /** Reads gif images from a byte array.
        * @param data the byte array
        * @throws IOException on error
        */    
        public GifImage(byte[] data) {
            fromData = data;
            Stream isp = null;
            try {
                isp = new MemoryStream(data);
                Process(isp);
            }
            finally {
                if (isp != null) {
                    isp.Close();
                }
            }
        }
        
        /** Reads gif images from a stream. The stream isp not closed.
        * @param isp the stream
        * @throws IOException on error
        */    
        public GifImage(Stream isp) {
            Process(isp);
        }
        
        /** Gets the number of frames the gif has.
        * @return the number of frames the gif has
        */    
        public int GetFrameCount() {
            return frames.Count;
        }
        
        /** Gets the image from a frame. The first frame isp 1.
        * @param frame the frame to get the image from
        * @return the image
        */    
        public Image GetImage(int frame) {
            GifFrame gf = (GifFrame)frames[frame - 1];
            return gf.image;
        }
        
        /** Gets the [x,y] position of the frame in reference to the
        * logical screen.
        * @param frame the frame
        * @return the [x,y] position of the frame
        */    
        public int[] GetFramePosition(int frame) {
            GifFrame gf = (GifFrame)frames[frame - 1];
            return new int[]{gf.ix, gf.iy};
            
        }
        
        /** Gets the logical screen. The images may be smaller and placed
        * in some position in this screen to playback some animation.
        * No image will be be bigger that this.
        * @return the logical screen dimensions as [x,y]
        */    
        public int[] GetLogicalScreen() {
            return new int[]{width, height};
        }
        
        internal void Process(Stream isp) {
            inp = new BufferedStream(isp);
            ReadHeader();
            ReadContents();
            if (frames.Count == 0)
                throw new IOException("The file does not contain any valid image.");
        }
        
        /**
        * Reads GIF file header information.
        */
        protected void ReadHeader() {
            String id = "";
            for (int i = 0; i < 6; i++)
                id += (char)inp.ReadByte();
            if (!id.StartsWith("GIF8")) {
                throw new IOException("Gif signature nor found.");
            }
            
            ReadLSD();
            if (gctFlag) {
                m_global_table = ReadColorTable(m_gbpc);
            }
        }

        /**
        * Reads Logical Screen Descriptor
        */
        protected void ReadLSD() {
            
            // logical screen size
            width = ReadShort();
            height = ReadShort();
            
            // packed fields
            int packed = inp.ReadByte();
            gctFlag = (packed & 0x80) != 0;      // 1   : global color table flag
            m_gbpc = (packed & 7) + 1;
            bgIndex = inp.ReadByte();        // background color index
            pixelAspect = inp.ReadByte();    // pixel aspect ratio
        }

        /**
        * Reads next 16-bit value, LSB first
        */
        protected int ReadShort() {
            // read 16-bit value, LSB first
            return inp.ReadByte() | (inp.ReadByte() << 8);
        }

        /**
        * Reads next variable length block from input.
        *
        * @return number of bytes stored in "buffer"
        */
        protected int ReadBlock() {
            blockSize = inp.ReadByte();
            if (blockSize <= 0)
                return blockSize = 0;
            for (int k = 0; k < blockSize; ++k) {
                int v = inp.ReadByte();
                if (v < 0) {
                    return blockSize = k;
                }
                block[k] = (byte)v;
            }
            return blockSize;
        }

        protected byte[] ReadColorTable(int bpc) {
            int ncolors = 1 << bpc;
            int nbytes = 3*ncolors;
            bpc = NewBpc(bpc);
            byte[] table = new byte[(1 << bpc) * 3];
            ReadFully(table, 0, nbytes);
            return table;
        }
     
        
        static protected int NewBpc(int bpc) {
            switch (bpc) {
                case 1:
                case 2:
                case 4:
                    break;
                case 3:
                    return 4;
                default:
                    return 8;
            }
            return bpc;
        }
        
        protected void ReadContents() {
            // read GIF file content blocks
            bool done = false;
            while (!done) {
                int code = inp.ReadByte();
                switch (code) {
                    
                    case 0x2C:    // image separator
                        ReadImage();
                        break;
                        
                    case 0x21:    // extension
                        code = inp.ReadByte();
                        switch (code) {
                            
                            case 0xf9:    // graphics control extension
                                ReadGraphicControlExt();
                                break;
                                
                            case 0xff:    // application extension
                                ReadBlock();
                                Skip();        // don't care
                                break;
                                
                            default:    // uninteresting extension
                                Skip();
                                break;
                        }
                        break;
                        
                    default:
                        done = true;
                        break;
                }
            }
        }

        /**
        * Reads next frame image
        */
        protected void ReadImage() {
            ix = ReadShort();    // (sub)image position & size
            iy = ReadShort();
            iw = ReadShort();
            ih = ReadShort();
            
            int packed = inp.ReadByte();
            lctFlag = (packed & 0x80) != 0;     // 1 - local color table flag
            interlace = (packed & 0x40) != 0;   // 2 - interlace flag
            // 3 - sort flag
            // 4-5 - reserved
            lctSize = 2 << (packed & 7);        // 6-8 - local color table size
            m_bpc = NewBpc(m_gbpc);
            if (lctFlag) {
                m_curr_table = ReadColorTable((packed & 7) + 1);   // read table
                m_bpc = NewBpc((packed & 7) + 1);
            }
            else {
                m_curr_table = m_global_table;
            }
            if (transparency && transIndex >= m_curr_table.Length / 3)
                transparency = false;
            if (transparency && m_bpc == 1) { // Acrobat 5.05 doesn't like this combination
                byte[] tp = new byte[12];
                Array.Copy(m_curr_table, 0, tp, 0, 6);
                m_curr_table = tp;
                m_bpc = 2;
            }
            bool skipZero = DecodeImageData();   // decode pixel data
            if (!skipZero)
                Skip();
            
            Image img = null;
            img = new ImgRaw(iw, ih, 1, m_bpc, m_out);
            PdfArray colorspace = new PdfArray();
            colorspace.Add(PdfName.INDEXED);
            colorspace.Add(PdfName.DEVICERGB);
            int len = m_curr_table.Length;
            colorspace.Add(new PdfNumber(len / 3 - 1));
            colorspace.Add(new PdfString(m_curr_table));
            PdfDictionary ad = new PdfDictionary();
            ad.Put(PdfName.COLORSPACE, colorspace);
            img.Additional = ad;
            if (transparency) {
                img.Transparency = new int[]{transIndex, transIndex};
            }
            img.OriginalType = Image.ORIGINAL_GIF;
            img.OriginalData = fromData;
            img.Url = fromUrl;
            GifFrame gf = new GifFrame();
            gf.image = img;
            gf.ix = ix;
            gf.iy = iy;
            frames.Add(gf);   // add image to frame list
            
            //ResetFrame();
            
        }
        
        protected bool DecodeImageData() {
            int NullCode = -1;
            int npix = iw * ih;
            int available, clear, code_mask, code_size, end_of_information, in_code, old_code,
            bits, code, count, i, datum, data_size, first, top, bi;
            bool skipZero = false;
            
            if (prefix == null)
                prefix = new short[MaxStackSize];
            if (suffix == null)
                suffix = new byte[MaxStackSize];
            if (pixelStack == null)
                pixelStack = new byte[MaxStackSize+1];
            
            m_line_stride = (iw * m_bpc + 7) / 8;
            m_out = new byte[m_line_stride * ih];
            int pass = 1;
            int inc = interlace ? 8 : 1;
            int line = 0;
            int xpos = 0;
            
            //  Initialize GIF data stream decoder.
            
            data_size = inp.ReadByte();
            clear = 1 << data_size;
            end_of_information = clear + 1;
            available = clear + 2;
            old_code = NullCode;
            code_size = data_size + 1;
            code_mask = (1 << code_size) - 1;
            for (code = 0; code < clear; code++) {
                prefix[code] = 0;
                suffix[code] = (byte) code;
            }
            
            //  Decode GIF pixel stream.
            
            datum = bits = count = first = top = bi = 0;
            
            for (i = 0; i < npix; ) {
                if (top == 0) {
                    if (bits < code_size) {
                        //  Load bytes until there are enough bits for a code.
                        if (count == 0) {
                            // Read a new data block.
                            count = ReadBlock();
                            if (count <= 0) {
                                skipZero = true;
                                break;
                            }
                            bi = 0;
                        }
                        datum += (((int) block[bi]) & 0xff) << bits;
                        bits += 8;
                        bi++;
                        count--;
                        continue;
                    }
                    
                    //  Get the next code.
                    
                    code = datum & code_mask;
                    datum >>= code_size;
                    bits -= code_size;
                    
                    //  Interpret the code
                    
                    if ((code > available) || (code == end_of_information))
                        break;
                    if (code == clear) {
                        //  Reset decoder.
                        code_size = data_size + 1;
                        code_mask = (1 << code_size) - 1;
                        available = clear + 2;
                        old_code = NullCode;
                        continue;
                    }
                    if (old_code == NullCode) {
                        pixelStack[top++] = suffix[code];
                        old_code = code;
                        first = code;
                        continue;
                    }
                    in_code = code;
                    if (code == available) {
                        pixelStack[top++] = (byte) first;
                        code = old_code;
                    }
                    while (code > clear) {
                        pixelStack[top++] = suffix[code];
                        code = prefix[code];
                    }
                    first = ((int) suffix[code]) & 0xff;
                    
                    //  Add a new string to the string table,
                    
                    if (available >= MaxStackSize)
                        break;
                    pixelStack[top++] = (byte) first;
                    prefix[available] = (short) old_code;
                    suffix[available] = (byte) first;
                    available++;
                    if (((available & code_mask) == 0) && (available < MaxStackSize)) {
                        code_size++;
                        code_mask += available;
                    }
                    old_code = in_code;
                }
                
                //  Pop a pixel off the pixel stack.
                
                top--;
                i++;
                
                SetPixel(xpos, line, pixelStack[top]);
                ++xpos;
                if (xpos >= iw) {
                    xpos = 0;
                    line += inc;
                    if (line >= ih) {
                        if (interlace) {
                            do {
                                pass++;
                                switch (pass) {
                                    case 2:
                                        line = 4;
                                        break;
                                    case 3:
                                        line = 2;
                                        inc = 4;
                                        break;
                                    case 4:
                                        line = 1;
                                        inc = 2;
                                        break;
                                    default: // this shouldn't happen
                                        line = ih - 1;
                                        inc = 0;
                                        break;
                                }
                            } while (line >= ih);
                        }
                        else {
                            line = ih - 1; // this shouldn't happen
                            inc = 0;
                        }
                    }
                }
            }
            return skipZero;
        }
        
        
        protected void SetPixel(int x, int y, int v) {
            if (m_bpc == 8) {
                int pos = x + iw * y;
                m_out[pos] = (byte)v;
            }
            else {
                int pos = m_line_stride * y + x / (8 / m_bpc);
                int vout = v << (8 - m_bpc * (x % (8 / m_bpc))- m_bpc);
                m_out[pos] |= (byte)vout;
            }
        }
        
        /**
        * Resets frame state for reading next image.
        */
        protected void ResetFrame() {
        }

        /**
        * Reads Graphics Control Extension values
        */
        protected void ReadGraphicControlExt() {
            inp.ReadByte();    // block size
            int packed = inp.ReadByte();   // packed fields
            dispose = (packed & 0x1c) >> 2;   // disposal method
            if (dispose == 0)
                dispose = 1;   // elect to keep old image if discretionary
            transparency = (packed & 1) != 0;
            delay = ReadShort() * 10;   // delay inp milliseconds
            transIndex = inp.ReadByte();        // transparent color index
            inp.ReadByte();                     // block terminator
        }
        
        /**
        * Skips variable length blocks up to and including
        * next zero length block.
        */
        protected void Skip() {
            do {
                ReadBlock();
            } while (blockSize > 0);
        }

        private void ReadFully(byte[] b, int offset, int count) {
            while (count > 0) {
                int n = inp.Read(b, offset, count);
                if (n <= 0)
                    throw new IOException("Insufficient data.");
                count -= n;
                offset += n;
            }
        }

        internal class GifFrame {
            internal Image image;
            internal int ix;
            internal int iy;
        }
    }
}
