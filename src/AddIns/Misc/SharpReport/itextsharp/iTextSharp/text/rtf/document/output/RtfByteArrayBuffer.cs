using System;
using System.IO;
using System.Collections;
/*
 * Copyright 2007 Thomas Bickel
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
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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

namespace iTextSharp.text.rtf.document.output {

    public class RtfByteArrayBuffer	: Stream {
        private ArrayList arrays = new ArrayList();
        private byte[] buffer;
        private int pos = 0;
        private int size = 0;

        public RtfByteArrayBuffer() : this(256) {
        }
    
        /**
        * Creates a new buffer with the given initial size.
        * 
        * @param bufferSize desired initial size in bytes
        */
        public RtfByteArrayBuffer(int bufferSize) {
            if ((bufferSize <= 0) || (bufferSize > 1<<30)) throw(new ArgumentException("bufferSize "+bufferSize));
            
            int n = 1<<5;
            while(n < bufferSize) {
                n <<= 1;
            }
            buffer = new byte[n];
        }

        public override bool CanRead {
            get {
                return false;
            }
        }
    
        public override bool CanSeek {
            get {
                return false;
            }
        }
    
        public override bool CanWrite {
            get {
                return true;
            }
        }
    
        public override long Length {
            get {
                throw new NotSupportedException();
            }
        }
    
        public override long Position {
            get {
                throw new NotSupportedException();
            }
            set {
                throw new NotSupportedException();
            }
        }
    
        public override void Close() {
        }
    
        public override void Flush() {
        }
    
        public override int Read(byte[] buffer, int offset, int count) {
            throw new NotSupportedException();
        }
    
        public override long Seek(long offset, SeekOrigin origin) {
            throw new NotSupportedException();
        }
    
        public override void SetLength(long value) {
            throw new NotSupportedException();
        }
    
        public override void Write(byte[] src, int off, int len) {
            if(src == null) throw(new ArgumentNullException());
            if((off < 0) || (off > src.Length) || (len < 0) || ((off + len) > src.Length) || ((off + len) < 0)) throw new IndexOutOfRangeException();

            WriteLoop(src, off, len);       
        }
    
        private void WriteLoop(byte[] src, int off, int len) {
            while(len > 0) {
                int room = buffer.Length - pos;
                int n = len > room ? room : len;
                System.Array.Copy(src, off, buffer, pos, n);
                len -= n;
                off += n;
                pos += n;
                size += n;
                if(pos == buffer.Length) FlushBuffer(len);
            }       
        }
        
        /**
        * Writes all bytes available in the given inputstream to this buffer. 
        * 
        * @param in
        * @return number of bytes written
        * @throws IOException
        */
        public long Write(Stream inp) {
            if (inp == null) throw(new ArgumentNullException());
            long sizeStart = size;
            while (true) {
                int n = inp.Read(buffer, pos, buffer.Length - pos);
                if (n <= 0) break;
                pos += n;
                size += n;
                if(pos == buffer.Length) FlushBuffer();
            }
            return(size - sizeStart);
        }
        
        /**
        * Appends the given array to this buffer without copying (if possible). 
        * 
        * @param a
        */
        public void Append(byte[] a) {
            if(a == null) throw(new ArgumentNullException());
            if(a.Length == 0) return;
            
            if(a.Length <= 8) {
                Write(a, 0, a.Length);      
            } else if((a.Length <= 16) && (pos > 0) && ((buffer.Length - pos) > a.Length)) {
                Write(a, 0, a.Length);
            } else {
                FlushBuffer();
                arrays.Add(a);
                size += a.Length;
            }
        }
        /**
        * Appends all arrays to this buffer without copying (if possible).
        * 
        * @param a
        */
        public void Append(byte[][] a) {
            if(a == null) throw(new ArgumentNullException());

            for(int k = 0; k < a.Length; k++) {
                Append(a[k]);
            }
        }
        
        /**
        * Returns the internal list of byte array buffers without copying the buffer contents. 
        * 
        * @return an byte aray of buffers
        */
        public byte[][] ToArrayArray()
        {
            FlushBuffer();
            byte[][] a = new byte[arrays.Count][];
            arrays.CopyTo(a);
            return a;
        }
        
        /**
        * Allocates a new array and copies all data that has been written to this buffer to the newly allocated array.
        * 
        * @return a new byte array
        */
        public byte[] ToArray()
        {
            byte[] r = new byte[size];
            int off = 0;
            int n = arrays.Count;
            for(int k = 0; k < n; k++) {
                byte[] src = (byte[])arrays[k];
                System.Array.Copy(src, 0, r, off, src.Length);
                off += src.Length;
            }
            if(pos > 0) System.Array.Copy(buffer, 0, r, off, pos);
            return(r);
        }

        /**
        * Writes all data that has been written to this buffer to the given output stream.
        * 
        * @param out
        * @throws IOException
        */
        public void WriteTo(Stream outp) {
            if(outp == null) throw(new ArgumentNullException());
            
            int n = arrays.Count;
            for(int k = 0; k < n; k++) {
                byte[] src = (byte[])arrays[k];
                outp.Write(src, 0, src.Length);
            }
            if(pos > 0) outp.Write(buffer, 0, pos);
        }

        public override void WriteByte(byte value) {
            buffer[pos] = value;
            size++;
            if(++pos == buffer.Length) FlushBuffer();
        }
    
        public override string ToString() {
            return("RtfByteArrayBuffer: size="+Size()+" #arrays="+arrays.Count+" pos="+pos);
        }

        /**
        * Resets this buffer.
        */
        public void Reset() {
            arrays.Clear();
            pos = 0;
            size = 0;
        }

        /**
        * Returns the number of bytes that have been written to this buffer so far.
        * 
        * @return number of bytes written to this buffer
        */
        public long Size() {
            return(size);
        }
        
        private void FlushBuffer() {
            FlushBuffer(1);
        }

        private void FlushBuffer(int reqSize) {
            if(reqSize < 0) throw(new ArgumentException());
            
            if(pos == 0) return;

            if(pos == buffer.Length) {
                //add old buffer, alloc new (possibly larger) buffer
                arrays.Add(buffer);
                int newSize = buffer.Length;
                buffer = null;
                int MAX = Math.Max(1, size>>24) << 16;
                while(newSize < MAX) {
                    newSize <<= 1;
                    if(newSize >= reqSize) break;
                }
                buffer = new byte[newSize];
            } else {
                //copy buffer contents to newly allocated buffer
                byte[] c = new byte[pos];
                System.Array.Copy(buffer, 0, c, 0, pos);
                arrays.Add(c);              
            }
            pos = 0;            
        }
    }
}
