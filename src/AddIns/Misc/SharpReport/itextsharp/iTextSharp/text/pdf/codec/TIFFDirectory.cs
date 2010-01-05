using System;
using System.Collections;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;
/*
 * Copyright 2003-2008 by Paulo Soares.
 * 
 * This code was originally released in 2001 by SUN (see class
 * com.sun.media.imageio.plugins.tiff.TIFFDirectory.java)
 * using the BSD license in a specific wording. In a mail dating from
 * January 23, 2008, Brian Burkhalter (@sun.com) gave us permission
 * to use the code under the following version of the BSD license:
 *
 * Copyright (c) 2006 Sun Microsystems, Inc. All  Rights Reserved.
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
    * A class representing an Image File Directory (IFD) from a TIFF 6.0
    * stream.  The TIFF file format is described in more detail in the
    * comments for the TIFFDescriptor class.
    *
    * <p> A TIFF IFD consists of a set of TIFFField tags.  Methods are
    * provided to query the set of tags and to obtain the raw field
    * array.  In addition, convenience methods are provided for acquiring
    * the values of tags that contain a single value that fits into a
    * byte, int, long, float, or double.
    *
    * <p> Every TIFF file is made up of one or more public IFDs that are
    * joined in a linked list, rooted in the file header.  A file may
    * also contain so-called private IFDs that are referenced from
    * tag data and do not appear in the main list.
    *
    * <p><b> This class is not a committed part of the JAI API.  It may
    * be removed or changed in future releases of JAI.</b>
    *
    * @see TIFFField
    */
    public class TIFFDirectory {
        
        /** A bool storing the endianness of the stream. */
        bool isBigEndian;
        
        /** The number of entries in the IFD. */
        int numEntries;
        
        /** An array of TIFFFields. */
        TIFFField[] fields;
        
        /** A Hashtable indexing the fields by tag number. */
        Hashtable fieldIndex = new Hashtable();
        
        /** The offset of this IFD. */
        long IFDOffset = 8;
        
        /** The offset of the next IFD. */
        long nextIFDOffset = 0;
        
        /** The default constructor. */
        TIFFDirectory() {}
        
        private static bool IsValidEndianTag(int endian) {
            return ((endian == 0x4949) || (endian == 0x4d4d));
        }
        
        /**
        * Constructs a TIFFDirectory from a SeekableStream.
        * The directory parameter specifies which directory to read from
        * the linked list present in the stream; directory 0 is normally
        * read but it is possible to store multiple images in a single
        * TIFF file by maintaing multiple directories.
        *
        * @param stream a SeekableStream to read from.
        * @param directory the index of the directory to read.
        */
        public TIFFDirectory(RandomAccessFileOrArray stream, int directory)
        {
            
            long global_save_offset = stream.FilePointer;
            long ifd_offset;
            
            // Read the TIFF header
            stream.Seek(0L);
            int endian = stream.ReadUnsignedShort();
            if (!IsValidEndianTag(endian)) {
                throw new
                ArgumentException("Bad endianness tag (not 0x4949 or 0x4d4d).");
            }
            isBigEndian = (endian == 0x4d4d);
            
            int magic = ReadUnsignedShort(stream);
            if (magic != 42) {
                throw new
                ArgumentException("Bad magic number, should be 42.");
            }
            
            // Get the initial ifd offset as an unsigned int (using a long)
            ifd_offset = ReadUnsignedInt(stream);
            
            for (int i = 0; i < directory; i++) {
                if (ifd_offset == 0L) {
                    throw new
                    ArgumentException("Directory number too large.");
                }
                
                stream.Seek(ifd_offset);
                int entries = ReadUnsignedShort(stream);
                stream.Skip(12*entries);
                
                ifd_offset = ReadUnsignedInt(stream);
            }
            
            stream.Seek(ifd_offset);
            Initialize(stream);
            stream.Seek(global_save_offset);
        }
        
        /**
        * Constructs a TIFFDirectory by reading a SeekableStream.
        * The ifd_offset parameter specifies the stream offset from which
        * to begin reading; this mechanism is sometimes used to store
        * private IFDs within a TIFF file that are not part of the normal
        * sequence of IFDs.
        *
        * @param stream a SeekableStream to read from.
        * @param ifd_offset the long byte offset of the directory.
        * @param directory the index of the directory to read beyond the
        *        one at the current stream offset; zero indicates the IFD
        *        at the current offset.
        */
        public TIFFDirectory(RandomAccessFileOrArray stream, long ifd_offset, int directory)
        {
            
            long global_save_offset = stream.FilePointer;
            stream.Seek(0L);
            int endian = stream.ReadUnsignedShort();
            if (!IsValidEndianTag(endian)) {
                throw new
                ArgumentException("Bad endianness tag (not 0x4949 or 0x4d4d).");
            }
            isBigEndian = (endian == 0x4d4d);
            
            // Seek to the first IFD.
            stream.Seek(ifd_offset);
            
            // Seek to desired IFD if necessary.
            int dirNum = 0;
            while (dirNum < directory) {
                // Get the number of fields in the current IFD.
                int numEntries = ReadUnsignedShort(stream);
                
                // Skip to the next IFD offset value field.
                stream.Seek(ifd_offset + 12*numEntries);
                
                // Read the offset to the next IFD beyond this one.
                ifd_offset = ReadUnsignedInt(stream);
                
                // Seek to the next IFD.
                stream.Seek(ifd_offset);
                
                // Increment the directory.
                dirNum++;
            }
            
            Initialize(stream);
            stream.Seek(global_save_offset);
        }
        
        private static int[] sizeOfType = {
            0, //  0 = n/a
            1, //  1 = byte
            1, //  2 = ascii
            2, //  3 = short
            4, //  4 = long
            8, //  5 = rational
            1, //  6 = sbyte
            1, //  7 = undefined
            2, //  8 = sshort
            4, //  9 = slong
            8, // 10 = srational
            4, // 11 = float
            8  // 12 = double
        };
        
        private void Initialize(RandomAccessFileOrArray stream) {
            long nextTagOffset = 0L;
            long maxOffset = (long) stream.Length;
            int i, j;
            
            IFDOffset = stream.FilePointer;
            
            numEntries = ReadUnsignedShort(stream);
            fields = new TIFFField[numEntries];
            
            for (i = 0; (i < numEntries) && (nextTagOffset < maxOffset); i++) {
                int tag = ReadUnsignedShort(stream);
                int type = ReadUnsignedShort(stream);
                int count = (int)(ReadUnsignedInt(stream));
                bool processTag = true;
                
                // The place to return to to read the next tag
                nextTagOffset = stream.FilePointer + 4;
                
                try {
                    // If the tag data can't fit in 4 bytes, the next 4 bytes
                    // contain the starting offset of the data
                    if (count*sizeOfType[type] > 4) {
                        long valueOffset = ReadUnsignedInt(stream);
                        
                        // bounds check offset for EOF
                        if (valueOffset < maxOffset) {
                    	    stream.Seek(valueOffset);
                        }
                        else {
                    	    // bad offset pointer .. skip tag
                    	    processTag = false;
                        }
                    }
                } catch (ArgumentOutOfRangeException) {
                    // if the data type is unknown we should skip this TIFF Field
                    processTag = false;
                }
                
                if (processTag) {
                fieldIndex[tag] = i;
                Object obj = null;
                
                switch (type) {
                    case TIFFField.TIFF_BYTE:
                    case TIFFField.TIFF_SBYTE:
                    case TIFFField.TIFF_UNDEFINED:
                    case TIFFField.TIFF_ASCII:
                        byte[] bvalues = new byte[count];
                        stream.ReadFully(bvalues, 0, count);
                        
                        if (type == TIFFField.TIFF_ASCII) {
                            
                            // Can be multiple strings
                            int index = 0, prevIndex = 0;
                            ArrayList v = new ArrayList();
                            
                            while (index < count) {
                                
                                while ((index < count) && (bvalues[index++] != 0));
                                
                                // When we encountered zero, means one string has ended
                                char[] cht = new char[index - prevIndex];
                                Array.Copy(bvalues, prevIndex, cht, 0, index - prevIndex);
                                v.Add(new String(cht));
                                prevIndex = index;
                            }
                            
                            count = v.Count;
                            String[] strings = new String[count];
                            for (int c = 0 ; c < count; c++) {
                                strings[c] = (String)v[c];
                            }
                            
                            obj = strings;
                        } else {
                            obj = bvalues;
                        }
                        
                        break;
                        
                    case TIFFField.TIFF_SHORT:
                        char[] cvalues = new char[count];
                        for (j = 0; j < count; j++) {
                            cvalues[j] = (char)(ReadUnsignedShort(stream));
                        }
                        obj = cvalues;
                        break;
                        
                    case TIFFField.TIFF_LONG:
                        long[] lvalues = new long[count];
                        for (j = 0; j < count; j++) {
                            lvalues[j] = ReadUnsignedInt(stream);
                        }
                        obj = lvalues;
                        break;
                        
                    case TIFFField.TIFF_RATIONAL:
                        long[][] llvalues = new long[count][];
                        for (j = 0; j < count; j++) {
                            long v0 = ReadUnsignedInt(stream);
                            long v1 = ReadUnsignedInt(stream);
                            llvalues[j] = new long[]{v0, v1};
                        }
                        obj = llvalues;
                        break;
                        
                    case TIFFField.TIFF_SSHORT:
                        short[] svalues = new short[count];
                        for (j = 0; j < count; j++) {
                            svalues[j] = ReadShort(stream);
                        }
                        obj = svalues;
                        break;
                        
                    case TIFFField.TIFF_SLONG:
                        int[] ivalues = new int[count];
                        for (j = 0; j < count; j++) {
                            ivalues[j] = ReadInt(stream);
                        }
                        obj = ivalues;
                        break;
                        
                    case TIFFField.TIFF_SRATIONAL:
                        int[,] iivalues = new int[count,2];
                        for (j = 0; j < count; j++) {
                            iivalues[j,0] = ReadInt(stream);
                            iivalues[j,1] = ReadInt(stream);
                        }
                        obj = iivalues;
                        break;
                        
                    case TIFFField.TIFF_FLOAT:
                        float[] fvalues = new float[count];
                        for (j = 0; j < count; j++) {
                            fvalues[j] = ReadFloat(stream);
                        }
                        obj = fvalues;
                        break;
                        
                    case TIFFField.TIFF_DOUBLE:
                        double[] dvalues = new double[count];
                        for (j = 0; j < count; j++) {
                            dvalues[j] = ReadDouble(stream);
                        }
                        obj = dvalues;
                        break;
                        
                    default:
                        break;
                }
                
                fields[i] = new TIFFField(tag, type, count, obj);
                }
                
                stream.Seek(nextTagOffset);
            }
            
            // Read the offset of the next IFD.
            try {
                nextIFDOffset = ReadUnsignedInt(stream);
            }
            catch {
                // broken tiffs may not have this pointer
                nextIFDOffset = 0;
            }
        }
        
        /** Returns the number of directory entries. */
        public int GetNumEntries() {
            return numEntries;
        }
        
        /**
        * Returns the value of a given tag as a TIFFField,
        * or null if the tag is not present.
        */
        public TIFFField GetField(int tag) {
            object i = fieldIndex[tag];
            if (i == null) {
                return null;
            } else {
                return fields[(int)i];
            }
        }
        
        /**
        * Returns true if a tag appears in the directory.
        */
        public bool IsTagPresent(int tag) {
            return fieldIndex.ContainsKey(tag);
        }
        
        /**
        * Returns an ordered array of ints indicating the tag
        * values.
        */
        public int[] GetTags() {
            int[] tags = new int[fieldIndex.Count];
            fieldIndex.Keys.CopyTo(tags, 0);
            return tags;
        }
        
        /**
        * Returns an array of TIFFFields containing all the fields
        * in this directory.
        */
        public TIFFField[] GetFields() {
            return fields;
        }
        
        /**
        * Returns the value of a particular index of a given tag as a
        * byte.  The caller is responsible for ensuring that the tag is
        * present and has type TIFFField.TIFF_SBYTE, TIFF_BYTE, or
        * TIFF_UNDEFINED.
        */
        public byte GetFieldAsByte(int tag, int index) {
            int i = (int)fieldIndex[tag];
            byte [] b = (fields[(int)i]).GetAsBytes();
            return b[index];
        }
        
        /**
        * Returns the value of index 0 of a given tag as a
        * byte.  The caller is responsible for ensuring that the tag is
        * present and has  type TIFFField.TIFF_SBYTE, TIFF_BYTE, or
        * TIFF_UNDEFINED.
        */
        public byte GetFieldAsByte(int tag) {
            return GetFieldAsByte(tag, 0);
        }
        
        /**
        * Returns the value of a particular index of a given tag as a
        * long.  The caller is responsible for ensuring that the tag is
        * present and has type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED,
        * TIFF_SHORT, TIFF_SSHORT, TIFF_SLONG or TIFF_LONG.
        */
        public long GetFieldAsLong(int tag, int index) {
            int i = (int)fieldIndex[tag];
            return (fields[i]).GetAsLong(index);
        }
        
        /**
        * Returns the value of index 0 of a given tag as a
        * long.  The caller is responsible for ensuring that the tag is
        * present and has type TIFF_BYTE, TIFF_SBYTE, TIFF_UNDEFINED,
        * TIFF_SHORT, TIFF_SSHORT, TIFF_SLONG or TIFF_LONG.
        */
        public long GetFieldAsLong(int tag) {
            return GetFieldAsLong(tag, 0);
        }
        
        /**
        * Returns the value of a particular index of a given tag as a
        * float.  The caller is responsible for ensuring that the tag is
        * present and has numeric type (all but TIFF_UNDEFINED and
        * TIFF_ASCII).
        */
        public float GetFieldAsFloat(int tag, int index) {
            int i = (int)fieldIndex[tag];
            return fields[i].GetAsFloat(index);
        }
        
        /**
        * Returns the value of index 0 of a given tag as a float.  The
        * caller is responsible for ensuring that the tag is present and
        * has numeric type (all but TIFF_UNDEFINED and TIFF_ASCII).
        */
        public float GetFieldAsFloat(int tag) {
            return GetFieldAsFloat(tag, 0);
        }
        
        /**
        * Returns the value of a particular index of a given tag as a
        * double.  The caller is responsible for ensuring that the tag is
        * present and has numeric type (all but TIFF_UNDEFINED and
        * TIFF_ASCII).
        */
        public double GetFieldAsDouble(int tag, int index) {
            int i = (int)fieldIndex[tag];
            return fields[i].GetAsDouble(index);
        }
        
        /**
        * Returns the value of index 0 of a given tag as a double.  The
        * caller is responsible for ensuring that the tag is present and
        * has numeric type (all but TIFF_UNDEFINED and TIFF_ASCII).
        */
        public double GetFieldAsDouble(int tag) {
            return GetFieldAsDouble(tag, 0);
        }
        
        // Methods to read primitive data types from the stream
        
        private short ReadShort(RandomAccessFileOrArray stream)
        {
            if (isBigEndian) {
                return stream.ReadShort();
            } else {
                return stream.ReadShortLE();
            }
        }
        
        private int ReadUnsignedShort(RandomAccessFileOrArray stream)
        {
            if (isBigEndian) {
                return stream.ReadUnsignedShort();
            } else {
                return stream.ReadUnsignedShortLE();
            }
        }
        
        private int ReadInt(RandomAccessFileOrArray stream)
        {
            if (isBigEndian) {
                return stream.ReadInt();
            } else {
                return stream.ReadIntLE();
            }
        }
        
        private long ReadUnsignedInt(RandomAccessFileOrArray stream)
        {
            if (isBigEndian) {
                return stream.ReadUnsignedInt();
            } else {
                return stream.ReadUnsignedIntLE();
            }
        }
        
        private long ReadLong(RandomAccessFileOrArray stream)
        {
            if (isBigEndian) {
                return stream.ReadLong();
            } else {
                return stream.ReadLongLE();
            }
        }
        
        private float ReadFloat(RandomAccessFileOrArray stream)
        {
            if (isBigEndian) {
                return stream.ReadFloat();
            } else {
                return stream.ReadFloatLE();
            }
        }
        
        private double ReadDouble(RandomAccessFileOrArray stream)
        {
            if (isBigEndian) {
                return stream.ReadDouble();
            } else {
                return stream.ReadDoubleLE();
            }
        }
        
        private static int ReadUnsignedShort(RandomAccessFileOrArray stream,
        bool isBigEndian)
        {
            if (isBigEndian) {
                return stream.ReadUnsignedShort();
            } else {
                return stream.ReadUnsignedShortLE();
            }
        }
        
        private static long ReadUnsignedInt(RandomAccessFileOrArray stream,
        bool isBigEndian)
        {
            if (isBigEndian) {
                return stream.ReadUnsignedInt();
            } else {
                return stream.ReadUnsignedIntLE();
            }
        }
        
        // Utilities
        
        /**
        * Returns the number of image directories (subimages) stored in a
        * given TIFF file, represented by a <code>SeekableStream</code>.
        */
        public static int GetNumDirectories(RandomAccessFileOrArray stream)
        {
            long pointer = stream.FilePointer; // Save stream pointer
            
            stream.Seek(0L);
            int endian = stream.ReadUnsignedShort();
            if (!IsValidEndianTag(endian)) {
                throw new ArgumentException("Bad endianness tag (not 0x4949 or 0x4d4d).");
            }
            bool isBigEndian = (endian == 0x4d4d);
            int magic = ReadUnsignedShort(stream, isBigEndian);
            if (magic != 42) {
                throw new
                ArgumentException("Bad magic number, should be 42.");
            }
            
            stream.Seek(4L);
            long offset = ReadUnsignedInt(stream, isBigEndian);
            
            int numDirectories = 0;
            while (offset != 0L) {
                ++numDirectories;
                
                // EOFException means IFD was probably not properly terminated.
                try {
                    stream.Seek(offset);
                    int entries = ReadUnsignedShort(stream, isBigEndian);
                    stream.Skip(12*entries);
                    offset = ReadUnsignedInt(stream, isBigEndian);
                } catch (EndOfStreamException) {
                    //numDirectories--;
                    break;
                }
            }
            
            stream.Seek(pointer); // Reset stream pointer
            return numDirectories;
        }
        
        /**
        * Returns a bool indicating whether the byte order used in the
        * the TIFF file is big-endian (i.e. whether the byte order is from
        * the most significant to the least significant)
        */
        public bool IsBigEndian() {
            return isBigEndian;
        }
        
        /**
        * Returns the offset of the IFD corresponding to this
        * <code>TIFFDirectory</code>.
        */
        public long GetIFDOffset() {
            return IFDOffset;
        }
        
        /**
        * Returns the offset of the next IFD after the IFD corresponding to this
        * <code>TIFFDirectory</code>.
        */
        public long GetNextIFDOffset() {
            return nextIFDOffset;
        }
    }
}