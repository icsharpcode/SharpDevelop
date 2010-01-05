using System;
/*
 * Copyright 2003-2008 by Paulo Soares.
 * 
 * This code was originally released in 2001 by SUN (see class
 * com.sun.media.imageioimpl.plugins.tiff.TIFFLZWDecompressor.java)
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
    /**
    * A class for performing LZW decoding.
    *
    *
    */
    public class TIFFLZWDecoder {
        
        byte[][] stringTable;
        byte[] data = null;
        byte[] uncompData;
        int tableIndex, bitsToGet = 9;
        int bytePointer;
        int dstIndex;
        int w, h;
        int predictor, samplesPerPixel;
        int nextData = 0;
        int nextBits = 0;
        
        int[] andTable = {
            511,
            1023,
            2047,
            4095
        };
        
        public TIFFLZWDecoder(int w, int predictor, int samplesPerPixel) {
            this.w = w;
            this.predictor = predictor;
            this.samplesPerPixel = samplesPerPixel;
        }
        
        /**
        * Method to decode LZW compressed data.
        *
        * @param data            The compressed data.
        * @param uncompData      Array to return the uncompressed data in.
        * @param h               The number of rows the compressed data contains.
        */
        public byte[] Decode(byte[] data, byte[] uncompData, int h) {
            
            if (data[0] == (byte)0x00 && data[1] == (byte)0x01) {
                throw new InvalidOperationException("TIFF 5.0-style LZW codes are not supported.");
            }
            
            InitializeStringTable();
            
            this.data = data;
            this.h = h;
            this.uncompData = uncompData;
            
            // Initialize pointers
            bytePointer = 0;
            dstIndex = 0;
            
            
            nextData = 0;
            nextBits = 0;
            
            int code, oldCode = 0;
            byte[] strn;
            
            while ( ((code = GetNextCode()) != 257) &&
            dstIndex < uncompData.Length) {
                
                if (code == 256) {
                    
                    InitializeStringTable();
                    code = GetNextCode();
                    
                    if (code == 257) {
                        break;
                    }
                    
                    WriteString(stringTable[code]);
                    oldCode = code;
                    
                } else {
                    
                    if (code < tableIndex) {
                        
                        strn = stringTable[code];
                        
                        WriteString(strn);
                        AddStringToTable(stringTable[oldCode], strn[0]);
                        oldCode = code;
                        
                    } else {
                        
                        strn = stringTable[oldCode];
                        strn = ComposeString(strn, strn[0]);
                        WriteString(strn);
                        AddStringToTable(strn);
                        oldCode = code;
                    }
                    
                }
                
            }
            
            // Horizontal Differencing Predictor
            if (predictor == 2) {
                
                int count;
                for (int j = 0; j < h; j++) {
                    
                    count = samplesPerPixel * (j * w + 1);
                    
                    for (int i = samplesPerPixel; i < w * samplesPerPixel; i++) {
                        
                        uncompData[count] += uncompData[count - samplesPerPixel];
                        count++;
                    }
                }
            }
            
            return uncompData;
        }
        
        
        /**
        * Initialize the string table.
        */
        public void InitializeStringTable() {
            
            stringTable = new byte[4096][];
            
            for (int i=0; i<256; i++) {
                stringTable[i] = new byte[1];
                stringTable[i][0] = (byte)i;
            }
            
            tableIndex = 258;
            bitsToGet = 9;
        }
        
        /**
        * Write out the string just uncompressed.
        */
        public void WriteString(byte[] strn) {
            // Fix for broken tiff files
            int max = uncompData.Length - dstIndex;
            if (strn.Length < max)
                max = strn.Length;
            System.Array.Copy(strn, 0, uncompData, dstIndex, max);
            dstIndex += max;
        }
        
        /**
        * Add a new string to the string table.
        */
        public void AddStringToTable(byte[] oldString, byte newString) {
            int length = oldString.Length;
            byte[] strn = new byte[length + 1];
            Array.Copy(oldString, 0, strn, 0, length);
            strn[length] = newString;
            
            // Add this new String to the table
            stringTable[tableIndex++] = strn;
            
            if (tableIndex == 511) {
                bitsToGet = 10;
            } else if (tableIndex == 1023) {
                bitsToGet = 11;
            } else if (tableIndex == 2047) {
                bitsToGet = 12;
            }
        }
        
        /**
        * Add a new string to the string table.
        */
        public void AddStringToTable(byte[] strn) {
            
            // Add this new String to the table
            stringTable[tableIndex++] = strn;
            
            if (tableIndex == 511) {
                bitsToGet = 10;
            } else if (tableIndex == 1023) {
                bitsToGet = 11;
            } else if (tableIndex == 2047) {
                bitsToGet = 12;
            }
        }
        
        /**
        * Append <code>newString</code> to the end of <code>oldString</code>.
        */
        public byte[] ComposeString(byte[] oldString, byte newString) {
            int length = oldString.Length;
            byte[] strn = new byte[length + 1];
            Array.Copy(oldString, 0, strn, 0, length);
            strn[length] = newString;
            
            return strn;
        }
        
        // Returns the next 9, 10, 11 or 12 bits
        public int GetNextCode() {
            // Attempt to get the next code. The exception is caught to make
            // this robust to cases wherein the EndOfInformation code has been
            // omitted from a strip. Examples of such cases have been observed
            // in practice.
            try {
                nextData = (nextData << 8) | (data[bytePointer++] & 0xff);
                nextBits += 8;
                
                if (nextBits < bitsToGet) {
                    nextData = (nextData << 8) | (data[bytePointer++] & 0xff);
                    nextBits += 8;
                }
                
                int code =
                (nextData >> (nextBits - bitsToGet)) & andTable[bitsToGet-9];
                nextBits -= bitsToGet;
                
                return code;
            } catch (IndexOutOfRangeException) {
                // Strip not terminated as expected: return EndOfInformation code.
                return 257;
            }
        }
    }
}
