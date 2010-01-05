using System;
using System.IO;
/*
 * $Id: ZInflaterInputStream.cs,v 1.4 2007/06/22 14:14:16 psoares33 Exp $
 *
 * Copyright 2006 by Paulo Soares.
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

namespace System.util.zlib {
    /// <summary>
    /// Summary description for DeflaterOutputStream.
    /// </summary>
    public class ZInflaterInputStream : Stream {
        protected ZStream z=new ZStream();
        protected int flushLevel=JZlib.Z_NO_FLUSH;
        private const int BUFSIZE = 4192;
        protected byte[] buf=new byte[BUFSIZE];
        private byte[] buf1=new byte[1];

        protected Stream inp=null;
        private bool nomoreinput=false;

        public ZInflaterInputStream(Stream inp) : this(inp, false) {
        }
    
        public ZInflaterInputStream(Stream inp, bool nowrap) {
            this.inp=inp;
            z.inflateInit(nowrap);
            z.next_in=buf;
            z.next_in_index=0;
            z.avail_in=0;
        }
    
        public override bool CanRead {
            get {
                // TODO:  Add DeflaterOutputStream.CanRead getter implementation
                return true;
            }
        }
    
        public override bool CanSeek {
            get {
                // TODO:  Add DeflaterOutputStream.CanSeek getter implementation
                return false;
            }
        }
    
        public override bool CanWrite {
            get {
                // TODO:  Add DeflaterOutputStream.CanWrite getter implementation
                return false;
            }
        }
    
        public override long Length {
            get {
                // TODO:  Add DeflaterOutputStream.Length getter implementation
                return 0;
            }
        }
    
        public override long Position {
            get {
                // TODO:  Add DeflaterOutputStream.Position getter implementation
                return 0;
            }
            set {
                // TODO:  Add DeflaterOutputStream.Position setter implementation
            }
        }
    
        public override void Write(byte[] b, int off, int len) {
        }
    
        public override long Seek(long offset, SeekOrigin origin) {
            // TODO:  Add DeflaterOutputStream.Seek implementation
            return 0;
        }
    
        public override void SetLength(long value) {
            // TODO:  Add DeflaterOutputStream.SetLength implementation

        }
    
        public override int Read(byte[] b, int off, int len) {
            if(len==0)
                return(0);
            int err;
            z.next_out=b;
            z.next_out_index=off;
            z.avail_out=len;
            do {
                if((z.avail_in==0)&&(!nomoreinput)) { // if buffer is empty and more input is avaiable, refill it
                    z.next_in_index=0;
                    z.avail_in=inp.Read(buf, 0, BUFSIZE);//(BUFSIZE<z.avail_out ? BUFSIZE : z.avail_out));
                    if(z.avail_in==0) {
                        z.avail_in=0;
                        nomoreinput=true;
                    }
                }
                err=z.inflate(flushLevel);
                if(nomoreinput&&(err==JZlib.Z_BUF_ERROR))
                    return(-1);
                if(err!=JZlib.Z_OK && err!=JZlib.Z_STREAM_END)
                    throw new IOException("inflating: "+z.msg);
                if((nomoreinput||err==JZlib.Z_STREAM_END)&&(z.avail_out==len))
                    return(0);
            } 
            while(z.avail_out==len&&err==JZlib.Z_OK);
            //System.err.print("("+(len-z.avail_out)+")");
            return(len-z.avail_out);
        }
    
        public override void Flush() {
            inp.Flush();
        }
    
        public override void WriteByte(byte b) {
        }

        public override void Close() {
            inp.Close();
        }
    
        public override int ReadByte() {
            if(Read(buf1, 0, 1)<=0)
                return -1;
            return(buf1[0]&0xFF);
        }
    }
}
