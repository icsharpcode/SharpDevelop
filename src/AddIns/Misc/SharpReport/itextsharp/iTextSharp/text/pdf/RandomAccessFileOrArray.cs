using System;
using System.IO;
using System.Text;
using System.Net;
/*
 * $Id: RandomAccessFileOrArray.cs,v 1.9 2008/05/13 11:25:23 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002 Paulo Soares
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
    /** An implementation of a RandomAccessFile for input only
    * that accepts a file or a byte array as data source.
    *
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class RandomAccessFileOrArray {
        
        internal FileStream rf;
        internal String filename;
        internal byte[] arrayIn;
        internal int arrayInPtr;
        internal byte back;
        internal bool isBack = false;
        
        /** Holds value of property startOffset. */
        private int startOffset = 0;
        
        public RandomAccessFileOrArray(String filename) : this(filename, false) {
        }
        
        public RandomAccessFileOrArray(String filename, bool forceRead) {
            if (!File.Exists(filename)) {
                if (filename.StartsWith("file:/") || filename.StartsWith("http://") || filename.StartsWith("https://")) {
                    Stream isp = WebRequest.Create(new Uri(filename)).GetResponse().GetResponseStream();
                    try {
                        this.arrayIn = InputStreamToArray(isp);
                        return;
                    }
                    finally {
                        try {isp.Close();}catch{}
                    }
                }
                else {
                    Stream isp = BaseFont.GetResourceStream(filename);
                    if (isp == null)
                        throw new IOException(filename + " not found as file or resource.");
                    try {
                        this.arrayIn = InputStreamToArray(isp);
                        return;
                    }
                    finally {
                        try {isp.Close();}catch{}
                    }
                }
            }
            else if (forceRead) {
                Stream s = null;
                try {
                    s = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                    this.arrayIn = InputStreamToArray(s);
                }
                finally {
                    try{if (s != null) s.Close();}catch{}
                }
                return;
            }
            this.filename = filename;
            rf = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        public RandomAccessFileOrArray(Uri url) {
            Stream isp = WebRequest.Create(url).GetResponse().GetResponseStream();
            try {
                this.arrayIn = InputStreamToArray(isp);
            }
            finally {
                try {isp.Close();}catch{}
            }
        }

        public RandomAccessFileOrArray(Stream isp) {
            this.arrayIn = InputStreamToArray(isp);
        }
        
        public static byte[] InputStreamToArray(Stream isp) {
            byte[] b = new byte[8192];
            MemoryStream outp = new MemoryStream();
            while (true) {
                int read = isp.Read(b, 0, b.Length);
                if (read < 1)
                    break;
                outp.Write(b, 0, read);
            }
            return outp.ToArray();
        }

        public RandomAccessFileOrArray(byte[] arrayIn) {
            this.arrayIn = arrayIn;
        }
        
        public RandomAccessFileOrArray(RandomAccessFileOrArray file) {
            filename = file.filename;
            arrayIn = file.arrayIn;
            startOffset = file.startOffset;
        }
        
        public void PushBack(byte b) {
            back = b;
            isBack = true;
        }
        
        public int Read() {
            if (isBack) {
                isBack = false;
                return back & 0xff;
            }
            if (arrayIn == null)
                return rf.ReadByte();
            else {
                if (arrayInPtr >= arrayIn.Length)
                    return -1;
                return arrayIn[arrayInPtr++] & 0xff;
            }
        }
        
        public int Read(byte[] b, int off, int len) {
            if (len == 0)
                return 0;
            int n = 0;
            if (isBack) {
                isBack = false;
                if (len == 1) {
                    b[off] = back;
                    return 1;
                }
                else {
                    n = 1;
                    b[off++] = back;
                    --len;
                }
            }
            if (arrayIn == null) {
                return rf.Read(b, off, len) + n;
            }
            else {
                if (arrayInPtr >= arrayIn.Length)
                    return -1;
                if (arrayInPtr + len > arrayIn.Length)
                    len = arrayIn.Length - arrayInPtr;
                Array.Copy(arrayIn, arrayInPtr, b, off, len);
                arrayInPtr += len;
                return len + n;
            }
        }
        
        public int Read(byte[] b) {
            return Read(b, 0, b.Length);
        }
        
        public void ReadFully(byte[] b) {
            ReadFully(b, 0, b.Length);
        }
        
        public void ReadFully(byte[] b, int off, int len) {
            if (len == 0)
                return;
            int n = 0;
            do {
                int count = Read(b, off + n, len - n);
                if (count <= 0)
                    throw new EndOfStreamException();
                n += count;
            } while (n < len);
        }
        
        public long Skip(long n) {
            return SkipBytes((int)n);
        }
        
        public int SkipBytes(int n) {
            if (n <= 0) {
                return 0;
            }
            int adj = 0;
            if (isBack) {
                isBack = false;
                if (n == 1) {
                    return 1;
                }
                else {
                    --n;
                    adj = 1;
                }
            }
            int pos;
            int len;
            int newpos;
            
            pos = FilePointer;
            len = Length;
            newpos = pos + n;
            if (newpos > len) {
                newpos = len;
            }
            Seek(newpos);
            
            /* return the actual number of bytes skipped */
            return newpos - pos + adj;
        }
        
        public void ReOpen() {
            if (filename != null && rf == null)
                rf = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            Seek(0);
        }
        
        protected void InsureOpen() {
            if (filename != null && rf == null) {
                ReOpen();
            }
        }
        
        public bool IsOpen() {
            return (filename == null || rf != null);
        }
        
        public void Close() {
            isBack = false;
            if (rf != null) {
                rf.Close();
                rf = null;
            }
        }
        
        public int Length {
            get {
                if (arrayIn == null) {
                    InsureOpen();
                    return (int)rf.Length - startOffset;
                }
                else
                    return arrayIn.Length - startOffset;
            }
        }
        
        public void Seek(int pos) {
            pos += startOffset;
            isBack = false;
            if (arrayIn == null) {
                InsureOpen();
                rf.Position = pos;
            }
            else
                arrayInPtr = pos;
        }
        
        public void Seek(long pos) {
            Seek((int)pos);
        }
        
        public int FilePointer {
            get {
                InsureOpen();
                int n = isBack ? 1 : 0;
                if (arrayIn == null) {
                    return (int)rf.Position - n - startOffset;
                }
                else
                    return arrayInPtr - n - startOffset;
            }
        }
        
        public bool ReadBoolean() {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return (ch != 0);
        }
        
        public byte ReadByte() {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return (byte)(ch);
        }
        
        public int ReadUnsignedByte() {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return ch;
        }
        
        public short ReadShort() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (short)((ch1 << 8) + ch2);
        }
        
        /**
        * Reads a signed 16-bit number from this stream in little-endian order.
        * The method reads two
        * bytes from this stream, starting at the current stream pointer.
        * If the two bytes read, in order, are
        * <code>b1</code> and <code>b2</code>, where each of the two values is
        * between <code>0</code> and <code>255</code>, inclusive, then the
        * result is equal to:
        * <blockquote><pre>
        *     (short)((b2 &lt;&lt; 8) | b1)
        * </pre></blockquote>
        * <p>
        * This method blocks until the two bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next two bytes of this stream, interpreted as a signed
        *             16-bit number.
        * @exception  EOFException  if this stream reaches the end before reading
        *               two bytes.
        * @exception  IOException   if an I/O error occurs.
        */
        public short ReadShortLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (short)((ch2 << 8) + (ch1 << 0));
        }
        
        public int ReadUnsignedShort() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (ch1 << 8) + ch2;
        }
        
        /**
        * Reads an unsigned 16-bit number from this stream in little-endian order.
        * This method reads
        * two bytes from the stream, starting at the current stream pointer.
        * If the bytes read, in order, are
        * <code>b1</code> and <code>b2</code>, where
        * <code>0&nbsp;&lt;=&nbsp;b1, b2&nbsp;&lt;=&nbsp;255</code>,
        * then the result is equal to:
        * <blockquote><pre>
        *     (b2 &lt;&lt; 8) | b1
        * </pre></blockquote>
        * <p>
        * This method blocks until the two bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next two bytes of this stream, interpreted as an
        *             unsigned 16-bit integer.
        * @exception  EOFException  if this stream reaches the end before reading
        *               two bytes.
        * @exception  IOException   if an I/O error occurs.
        */
        public int ReadUnsignedShortLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (ch2 << 8) + (ch1 << 0);
        }
        
        public char ReadChar() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (char)((ch1 << 8) + ch2);
        }
        
        /**
        * Reads a Unicode character from this stream in little-endian order.
        * This method reads two
        * bytes from the stream, starting at the current stream pointer.
        * If the bytes read, in order, are
        * <code>b1</code> and <code>b2</code>, where
        * <code>0&nbsp;&lt;=&nbsp;b1,&nbsp;b2&nbsp;&lt;=&nbsp;255</code>,
        * then the result is equal to:
        * <blockquote><pre>
        *     (char)((b2 &lt;&lt; 8) | b1)
        * </pre></blockquote>
        * <p>
        * This method blocks until the two bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next two bytes of this stream as a Unicode character.
        * @exception  EOFException  if this stream reaches the end before reading
        *               two bytes.
        * @exception  IOException   if an I/O error occurs.
        */
        public char ReadCharLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (char)((ch2 << 8) + (ch1 << 0));
        }
        
        public int ReadInt() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + ch4);
        }
        
        /**
        * Reads a signed 32-bit integer from this stream in little-endian order.
        * This method reads 4
        * bytes from the stream, starting at the current stream pointer.
        * If the bytes read, in order, are <code>b1</code>,
        * <code>b2</code>, <code>b3</code>, and <code>b4</code>, where
        * <code>0&nbsp;&lt;=&nbsp;b1, b2, b3, b4&nbsp;&lt;=&nbsp;255</code>,
        * then the result is equal to:
        * <blockquote><pre>
        *     (b4 &lt;&lt; 24) | (b3 &lt;&lt; 16) + (b2 &lt;&lt; 8) + b1
        * </pre></blockquote>
        * <p>
        * This method blocks until the four bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next four bytes of this stream, interpreted as an
        *             <code>int</code>.
        * @exception  EOFException  if this stream reaches the end before reading
        *               four bytes.
        * @exception  IOException   if an I/O error occurs.
        */
        public int ReadIntLE() {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1 << 0));
        }
        
        /**
        * Reads an unsigned 32-bit integer from this stream. This method reads 4
        * bytes from the stream, starting at the current stream pointer.
        * If the bytes read, in order, are <code>b1</code>,
        * <code>b2</code>, <code>b3</code>, and <code>b4</code>, where
        * <code>0&nbsp;&lt;=&nbsp;b1, b2, b3, b4&nbsp;&lt;=&nbsp;255</code>,
        * then the result is equal to:
        * <blockquote><pre>
        *     (b1 &lt;&lt; 24) | (b2 &lt;&lt; 16) + (b3 &lt;&lt; 8) + b4
        * </pre></blockquote>
        * <p>
        * This method blocks until the four bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next four bytes of this stream, interpreted as a
        *             <code>long</code>.
        * @exception  EOFException  if this stream reaches the end before reading
        *               four bytes.
        * @exception  IOException   if an I/O error occurs.
        */
        public long ReadUnsignedInt() {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
        }
        
        public long ReadUnsignedIntLE() {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1 << 0));
        }
        
        public long ReadLong() {
            return ((long)(ReadInt()) << 32) + (ReadInt() & 0xFFFFFFFFL);
        }
        
        public long ReadLongLE() {
            int i1 = ReadIntLE();
            int i2 = ReadIntLE();
            return ((long)i2 << 32) + (i1 & 0xFFFFFFFFL);
        }
        
        public float ReadFloat() {
            int[] a = {ReadInt()};
            float[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 4);
            return b[0];
        }
        
        public float ReadFloatLE() {
            int[] a = {ReadIntLE()};
            float[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 4);
            return b[0];
        }
        
        public double ReadDouble() {
            long[] a = {ReadLong()};
            double[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 8);
            return b[0];
        }
        
        public double ReadDoubleLE() {
            long[] a = {ReadLongLE()};
            double[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 8);
            return b[0];
        }

    public String ReadLine() {
            StringBuilder input = new StringBuilder();
            int c = -1;
            bool eol = false;
            
            while (!eol) {
                switch (c = Read()) {
                    case -1:
                    case '\n':
                        eol = true;
                        break;
                    case '\r':
                        eol = true;
                        int cur = FilePointer;
                        if ((Read()) != '\n') {
                            Seek(cur);
                        }
                        break;
                    default:
                        input.Append((char)c);
                        break;
                }
            }
            
            if ((c == -1) && (input.Length == 0)) {
                return null;
            }
            return input.ToString();
        }
        
        public int StartOffset {
            get {
                return startOffset;
            }
            set {
                startOffset = value;
            }
        }
    }
}
