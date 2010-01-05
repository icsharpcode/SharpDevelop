using System;
using System.Text;
using System.Collections;
/*
 *
 * Copyright 2003 Sivan Toledo
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
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
 */

namespace iTextSharp.text.pdf {
    public class CFFFont {
        
        internal static String[] operatorNames = {
            "version", "Notice", "FullName", "FamilyName",
            "Weight", "FontBBox", "BlueValues", "OtherBlues",
            "FamilyBlues", "FamilyOtherBlues", "StdHW", "StdVW",
            "UNKNOWN_12", "UniqueID", "XUID", "charset",
            "Encoding", "CharStrings", "Private", "Subrs",
            "defaultWidthX", "nominalWidthX", "UNKNOWN_22", "UNKNOWN_23",
            "UNKNOWN_24", "UNKNOWN_25", "UNKNOWN_26", "UNKNOWN_27",
            "UNKNOWN_28", "UNKNOWN_29", "UNKNOWN_30", "UNKNOWN_31",
            "Copyright", "isFixedPitch", "ItalicAngle", "UnderlinePosition",
            "UnderlineThickness", "PaintType", "CharstringType", "FontMatrix",
            "StrokeWidth", "BlueScale", "BlueShift", "BlueFuzz",
            "StemSnapH", "StemSnapV", "ForceBold", "UNKNOWN_12_15",
            "UNKNOWN_12_16", "LanguageGroup", "ExpansionFactor", "initialRandomSeed",
            "SyntheticBase", "PostScript", "BaseFontName", "BaseFontBlend",
            "UNKNOWN_12_24", "UNKNOWN_12_25", "UNKNOWN_12_26", "UNKNOWN_12_27",
            "UNKNOWN_12_28", "UNKNOWN_12_29", "ROS", "CIDFontVersion",
            "CIDFontRevision", "CIDFontType", "CIDCount", "UIDBase",
            "FDArray", "FDSelect", "FontName"
        };
        
        internal static String[] standardStrings = {
            // Automatically generated from Appendix A of the CFF specification; do
            // not edit. Size should be 391.
            ".notdef", "space", "exclam", "quotedbl", "numbersign", "dollar",
            "percent", "ampersand", "quoteright", "parenleft", "parenright",
            "asterisk", "plus", "comma", "hyphen", "period", "slash", "zero", "one",
            "two", "three", "four", "five", "six", "seven", "eight", "nine", "colon",
            "semicolon", "less", "equal", "greater", "question", "at", "A", "B", "C",
            "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R",
            "S", "T", "U", "V", "W", "X", "Y", "Z", "bracketleft", "backslash",
            "bracketright", "asciicircum", "underscore", "quoteleft", "a", "b", "c",
            "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r",
            "s", "t", "u", "v", "w", "x", "y", "z", "braceleft", "bar", "braceright",
            "asciitilde", "exclamdown", "cent", "sterling", "fraction", "yen",
            "florin", "section", "currency", "quotesingle", "quotedblleft",
            "guillemotleft", "guilsinglleft", "guilsinglright", "fi", "fl", "endash",
            "dagger", "daggerdbl", "periodcentered", "paragraph", "bullet",
            "quotesinglbase", "quotedblbase", "quotedblright", "guillemotright",
            "ellipsis", "perthousand", "questiondown", "grave", "acute", "circumflex",
            "tilde", "macron", "breve", "dotaccent", "dieresis", "ring", "cedilla",
            "hungarumlaut", "ogonek", "caron", "emdash", "AE", "ordfeminine", "Lslash",
            "Oslash", "OE", "ordmasculine", "ae", "dotlessi", "lslash", "oslash", "oe",
            "germandbls", "onesuperior", "logicalnot", "mu", "trademark", "Eth",
            "onehalf", "plusminus", "Thorn", "onequarter", "divide", "brokenbar",
            "degree", "thorn", "threequarters", "twosuperior", "registered", "minus",
            "eth", "multiply", "threesuperior", "copyright", "Aacute", "Acircumflex",
            "Adieresis", "Agrave", "Aring", "Atilde", "Ccedilla", "Eacute",
            "Ecircumflex", "Edieresis", "Egrave", "Iacute", "Icircumflex", "Idieresis",
            "Igrave", "Ntilde", "Oacute", "Ocircumflex", "Odieresis", "Ograve",
            "Otilde", "Scaron", "Uacute", "Ucircumflex", "Udieresis", "Ugrave",
            "Yacute", "Ydieresis", "Zcaron", "aacute", "acircumflex", "adieresis",
            "agrave", "aring", "atilde", "ccedilla", "eacute", "ecircumflex",
            "edieresis", "egrave", "iacute", "icircumflex", "idieresis", "igrave",
            "ntilde", "oacute", "ocircumflex", "odieresis", "ograve", "otilde",
            "scaron", "uacute", "ucircumflex", "udieresis", "ugrave", "yacute",
            "ydieresis", "zcaron", "exclamsmall", "Hungarumlautsmall",
            "dollaroldstyle", "dollarsuperior", "ampersandsmall", "Acutesmall",
            "parenleftsuperior", "parenrightsuperior", "twodotenleader",
            "onedotenleader", "zerooldstyle", "oneoldstyle", "twooldstyle",
            "threeoldstyle", "fouroldstyle", "fiveoldstyle", "sixoldstyle",
            "sevenoldstyle", "eightoldstyle", "nineoldstyle", "commasuperior",
            "threequartersemdash", "periodsuperior", "questionsmall", "asuperior",
            "bsuperior", "centsuperior", "dsuperior", "esuperior", "isuperior",
            "lsuperior", "msuperior", "nsuperior", "osuperior", "rsuperior",
            "ssuperior", "tsuperior", "ff", "ffi", "ffl", "parenleftinferior",
            "parenrightinferior", "Circumflexsmall", "hyphensuperior", "Gravesmall",
            "Asmall", "Bsmall", "Csmall", "Dsmall", "Esmall", "Fsmall", "Gsmall",
            "Hsmall", "Ismall", "Jsmall", "Ksmall", "Lsmall", "Msmall", "Nsmall",
            "Osmall", "Psmall", "Qsmall", "Rsmall", "Ssmall", "Tsmall", "Usmall",
            "Vsmall", "Wsmall", "Xsmall", "Ysmall", "Zsmall", "colonmonetary",
            "onefitted", "rupiah", "Tildesmall", "exclamdownsmall", "centoldstyle",
            "Lslashsmall", "Scaronsmall", "Zcaronsmall", "Dieresissmall", "Brevesmall",
            "Caronsmall", "Dotaccentsmall", "Macronsmall", "figuredash",
            "hypheninferior", "Ogoneksmall", "Ringsmall", "Cedillasmall",
            "questiondownsmall", "oneeighth", "threeeighths", "fiveeighths",
            "seveneighths", "onethird", "twothirds", "zerosuperior", "foursuperior",
            "fivesuperior", "sixsuperior", "sevensuperior", "eightsuperior",
            "ninesuperior", "zeroinferior", "oneinferior", "twoinferior",
            "threeinferior", "fourinferior", "fiveinferior", "sixinferior",
            "seveninferior", "eightinferior", "nineinferior", "centinferior",
            "dollarinferior", "periodinferior", "commainferior", "Agravesmall",
            "Aacutesmall", "Acircumflexsmall", "Atildesmall", "Adieresissmall",
            "Aringsmall", "AEsmall", "Ccedillasmall", "Egravesmall", "Eacutesmall",
            "Ecircumflexsmall", "Edieresissmall", "Igravesmall", "Iacutesmall",
            "Icircumflexsmall", "Idieresissmall", "Ethsmall", "Ntildesmall",
            "Ogravesmall", "Oacutesmall", "Ocircumflexsmall", "Otildesmall",
            "Odieresissmall", "OEsmall", "Oslashsmall", "Ugravesmall", "Uacutesmall",
            "Ucircumflexsmall", "Udieresissmall", "Yacutesmall", "Thornsmall",
            "Ydieresissmall", "001.000", "001.001", "001.002", "001.003", "Black",
            "Bold", "Book", "Light", "Medium", "Regular", "Roman", "Semibold"
        };
        
        //private String[] strings;
        public String GetString(char sid) {
            if (sid < standardStrings.Length) return standardStrings[sid];
            if (sid >= standardStrings.Length+(stringOffsets.Length-1)) return null;
            int j = sid - standardStrings.Length;
            int p = GetPosition();
            Seek(stringOffsets[j]);
            StringBuilder s = new StringBuilder();
            for (int k=stringOffsets[j]; k<stringOffsets[j+1]; k++) {
                s.Append(GetCard8());
            }
            Seek(p);
            return s.ToString();
        }
        
        internal char GetCard8() {
            byte i = buf.ReadByte();
            return (char)(i & 0xff);
        }
        
        internal char GetCard16() {
            return buf.ReadChar();
        }
        
        internal int GetOffset(int offSize) {
            int offset = 0;
            for (int i=0; i<offSize; i++) {
                offset *= 256;
                offset += GetCard8();
            }
            return offset;
        }
        
        internal void Seek(int offset) {
            buf.Seek(offset);
        }
        
        internal short GetShort() {
            return buf.ReadShort();
        }
        
        internal int GetInt() {
            return buf.ReadInt();
        }
        
        internal int GetPosition() {
            return buf.FilePointer;
        }

        internal int nextIndexOffset;
        // read the offsets in the next index
        // data structure, convert to global
        // offsets, and return them.
        // Sets the nextIndexOffset.
        internal int[] GetIndex(int nextIndexOffset) {
            int count, indexOffSize;
            
            Seek(nextIndexOffset);
            count = GetCard16();
            int[] offsets = new int[count+1];
            
            if (count==0) {
                offsets[0] = -1;
                nextIndexOffset += 2;
                return offsets;
            }
            
            indexOffSize = GetCard8();
            
            for (int j=0; j<=count; j++) {
                //nextIndexOffset = ofset to relative segment
                offsets[j] = nextIndexOffset
                //2-> count in the index header. 1->offset size in index header
                + 2+1
                //offset array size * offset size 
                + (count+1)*indexOffSize
                //???zero <-> one base
                - 1
                // read object offset relative to object array base 
                + GetOffset(indexOffSize);
            }
            //nextIndexOffset = offsets[count];
            return offsets;
        }
        
        protected String   key;
        protected Object[] args      = new Object[48];
        protected int      arg_count = 0;
        
        protected void GetDictItem() {
            for (int i=0; i<arg_count; i++) args[i]=null;
            arg_count = 0;
            key = null;
            bool gotKey = false;
            
            while (!gotKey) {
                char b0 = GetCard8();
                if (b0 == 29) {
                    int item = GetInt();
                    args[arg_count] = item;
                    arg_count++;
                    //System.err.Println(item+" ");
                    continue;
                }
                if (b0 == 28) {
                    short item = GetShort();
                    args[arg_count] = (int)item;
                    arg_count++;
                    //System.err.Println(item+" ");
                    continue;
                }
                if (b0 >= 32 && b0 <= 246) {
                    sbyte item = (sbyte) ((int)b0-139);
                    args[arg_count] = (int)item;
                    arg_count++;
                    //System.err.Println(item+" ");
                    continue;
                }
                if (b0 >= 247 && b0 <= 250) {
                    char b1 = GetCard8();
                    short item = (short) (((int)b0-247)*256+(int)b1+108);
                    args[arg_count] = (int)item;
                    arg_count++;
                    //System.err.Println(item+" ");
                    continue;
                }
                if (b0 >= 251 && b0 <= 254) {
                    char b1 = GetCard8();
                    short item = (short) (-((int)b0-251)*256-(int)b1-108);
                    args[arg_count] = (int)item;
                    arg_count++;
                    //System.err.Println(item+" ");
                    continue;
                }
                if (b0 == 30) {
                    String item = "";
                    bool done = false;
                    char buffer = (char)0;
                    byte avail = 0;
                    int  nibble = 0;
                    while (!done) {
                        // get a nibble
                        if (avail==0) { buffer = GetCard8(); avail=2; }
                        if (avail==1) { nibble = (buffer / 16); avail--; }
                        if (avail==2) { nibble = (buffer % 16); avail--; }
                        switch (nibble) {
                            case 0xa: item += "." ; break;
                            case 0xb: item += "E" ; break;
                            case 0xc: item += "E-"; break;
                            case 0xe: item += "-" ; break;
                            case 0xf: done=true   ; break;
                            default:
                                if (nibble >= 0 && nibble <= 9)
                                    item += nibble.ToString();
                                else {
                                    item += "<NIBBLE ERROR: "+nibble.ToString()+">";
                                    done = true;
                                }
                                break;
                        }
                    }
                    args[arg_count] = item;
                    arg_count++;
                    //System.err.Println(" real=["+item+"]");
                    continue;
                }
                if (b0 <= 21) {
                    gotKey=true;
                    if (b0 != 12) key = operatorNames[b0];
                    else key = operatorNames[32 + GetCard8()];
                    //for (int i=0; i<arg_count; i++)
                    //  System.err.Print(args[i].ToString()+" ");
                    //System.err.Println(key+" ;");
                    continue;
                }
            }
        }
        
        /** List items for the linked list that builds the new CID font.
        */
        
        protected internal abstract class Item {
            protected internal int myOffset = -1;
            /** remember the current offset and increment by item's size in bytes. */
            public virtual void Increment(int[] currentOffset) {
                myOffset = currentOffset[0];
            }
            /** Emit the byte stream for this item. */
            public virtual void Emit(byte[] buffer) {}
            /** Fix up cross references to this item (applies only to markers). */
            public virtual void Xref() {}
        }
        
        protected internal abstract class OffsetItem : Item {
            public int value;
            /** set the value of an offset item that was initially unknown.
            * It will be fixed up latex by a call to xref on some marker.
            */
            public void Set(int offset) { this.value = offset; }
        }
        
        
        /** A range item.
        */
        
        protected internal class RangeItem : Item {
            public int offset, length;
            private RandomAccessFileOrArray buf;
            public RangeItem(RandomAccessFileOrArray buf, int offset, int length) {
                this.offset = offset;
                this.length = length;
                this.buf = buf;
            }
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += length;
            }
            public override void Emit(byte[] buffer) {
                //System.err.Println("range emit offset "+offset+" size="+length);
                buf.Seek(offset);
                for (int i=myOffset; i<myOffset+length; i++)
                    buffer[i] = buf.ReadByte();
                //System.err.Println("finished range emit");
            }
        }
        
        /** An index-offset item for the list.
        * The size denotes the required size in the CFF. A positive
        * value means that we need a specific size in bytes (for offset arrays)
        * and a negative value means that this is a dict item that uses a
        * variable-size representation.
        */
        protected internal class IndexOffsetItem : OffsetItem {
            public int size;
            public IndexOffsetItem(int size, int value) {this.size=size; this.value=value;}
            public IndexOffsetItem(int size) {this.size=size; }
            
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += size;
            }
            public override void Emit(byte[] buffer) {
                int i=0;
                switch (size) {
                    case 4:
                        buffer[myOffset+i] = (byte) ((value >> 24) & 0xff);
                        i++;
                        goto case 3;
                    case 3:
                        buffer[myOffset+i] = (byte) ((value >> 16) & 0xff);
                        i++;
                        goto case 2;
                    case 2:
                        buffer[myOffset+i] = (byte) ((value >>  8) & 0xff);
                        i++;
                        goto case 1;
                    case 1:
                        buffer[myOffset+i] = (byte) ((value >>  0) & 0xff);
                        i++;
                        break;
                }
                /*
                int mask = 0xff;
                for (int i=size-1; i>=0; i--) {
                    buffer[myOffset+i] = (byte) (value & mask);
                    mask <<= 8;
                }
                */
            }
        }
        
        protected internal class IndexBaseItem : Item {
            public IndexBaseItem() {}
        }
        
        protected internal class IndexMarkerItem : Item {
            private OffsetItem offItem;
            private IndexBaseItem indexBase;
            public IndexMarkerItem(OffsetItem offItem, IndexBaseItem indexBase) {
                this.offItem   = offItem;
                this.indexBase = indexBase;
            }
            public override void Xref() {
                //System.err.Println("index marker item, base="+indexBase.myOffset+" my="+this.myOffset);
                offItem.Set(this.myOffset-indexBase.myOffset+1);
            }
        }
        /**
        * 
        * @author orly manor
        *
        * TODO To change the template for this generated type comment go to
        * Window - Preferences - Java - Code Generation - Code and Comments
        */
        protected internal class SubrMarkerItem : Item {
            private OffsetItem offItem;
            private IndexBaseItem indexBase;
            public SubrMarkerItem(OffsetItem offItem, IndexBaseItem indexBase) {
                this.offItem   = offItem;
                this.indexBase = indexBase;
            }
            public override void Xref() {
                //System.err.Println("index marker item, base="+indexBase.myOffset+" my="+this.myOffset);
                offItem.Set(this.myOffset-indexBase.myOffset);
            }
        }
        
        
        /** an unknown offset in a dictionary for the list.
        * We will fix up the offset later; for now, assume it's large.
        */
        protected internal class DictOffsetItem : OffsetItem {
            public int size;
            public DictOffsetItem() {this.size=5; }
            
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += size;
            }
            // this is incomplete!
            public override void Emit(byte[] buffer) {
                if (size==5) {
                    buffer[myOffset]   = 29;
                    buffer[myOffset+1] = (byte) ((value >> 24) & 0xff);
                    buffer[myOffset+2] = (byte) ((value >> 16) & 0xff);
                    buffer[myOffset+3] = (byte) ((value >>  8) & 0xff);
                    buffer[myOffset+4] = (byte) ((value >>  0) & 0xff);
                }
            }
        }
        
        /** Card24 item.
        */
        
        protected internal class UInt24Item : Item {
            public int value;
            public UInt24Item(int value) {this.value=value;}
            
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += 3;
            }
            // this is incomplete!
            public override void Emit(byte[] buffer) {
                buffer[myOffset+0] = (byte) ((value >> 16) & 0xff);
                buffer[myOffset+1] = (byte) ((value >> 8) & 0xff);
                buffer[myOffset+2] = (byte) ((value >> 0) & 0xff);
            }
        }
        
        /** Card32 item.
        */
        
        protected internal class UInt32Item : Item {
            public int value;
            public UInt32Item(int value) {this.value=value;}
            
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += 4;
            }
            // this is incomplete!
            public override void Emit(byte[] buffer) {
                buffer[myOffset+0] = (byte) ((value >> 24) & 0xff);
                buffer[myOffset+1] = (byte) ((value >> 16) & 0xff);
                buffer[myOffset+2] = (byte) ((value >> 8) & 0xff);
                buffer[myOffset+3] = (byte) ((value >> 0) & 0xff);
            }
        }

        /** A SID or Card16 item.
        */
        
        protected internal class UInt16Item : Item {
            public char value;
            public UInt16Item(char value) {this.value=value;}
            
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += 2;
            }
            // this is incomplete!
            public override void Emit(byte[] buffer) {
                buffer[myOffset+0] = (byte) ((value >> 8) & 0xff);
                buffer[myOffset+1] = (byte) ((value >> 0) & 0xff);
            }
        }
        
        /** A Card8 item.
        */
        
        protected internal class UInt8Item : Item {
            public char value;
            public UInt8Item(char value) {this.value=value;}
            
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += 1;
            }
            // this is incomplete!
            public override void Emit(byte[] buffer) {
                buffer[myOffset+0] = (byte) ((value >> 0) & 0xff);
            }
        }
        
        protected internal class StringItem : Item {
            public String s;
            public StringItem(String s) {this.s=s;}
            
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += s.Length;
            }
            public override void Emit(byte[] buffer) {
                for (int i=0; i<s.Length; i++)
                    buffer[myOffset+i] = (byte) (s[i] & 0xff);
            }
        }
        
        
        /** A dictionary number on the list.
        * This implementation is inefficient: it doesn't use the variable-length
        * representation.
        */
        
        protected internal class DictNumberItem : Item {
            public int value;
            public int size = 5;
            public DictNumberItem(int value) {this.value=value;}
            public override void Increment(int[] currentOffset) {
                base.Increment(currentOffset);
                currentOffset[0] += size;
            }
            // this is imcomplete!
            public override void Emit(byte[] buffer) {
                if (size==5) {
                    buffer[myOffset]   = 29;
                    buffer[myOffset+1] = (byte) ((value >> 24) & 0xff);
                    buffer[myOffset+2] = (byte) ((value >> 16) & 0xff);
                    buffer[myOffset+3] = (byte) ((value >>  8) & 0xff);
                    buffer[myOffset+4] = (byte) ((value >>  0) & 0xff);
                }
            }
        }
        
        /** An offset-marker item for the list.
        * It is used to mark an offset and to set the offset list item.
        */
        
        protected internal class MarkerItem : Item {
            OffsetItem p;
            public MarkerItem(OffsetItem pointerToMarker) {p=pointerToMarker;}
            public override void Xref() {
                p.Set(this.myOffset);
            }
        }
        
        /** a utility that creates a range item for an entire index
        *
        * @param indexOffset where the index is
        * @return a range item representing the entire index
        */
        
        protected virtual RangeItem GetEntireIndexRange(int indexOffset) {
            Seek(indexOffset);
            int count = GetCard16();
            if (count==0) {
                return new RangeItem(buf,indexOffset,2);
            } else {
                int indexOffSize = GetCard8();
                Seek(indexOffset+2+1+count*indexOffSize);
                int size = GetOffset(indexOffSize)-1;
                return new RangeItem(buf,indexOffset,
                2+1+(count+1)*indexOffSize+size);
            }
        }
        
        
        /** get a single CID font. The PDF architecture (1.4)
        * supports 16-bit strings only with CID CFF fonts, not
        * in Type-1 CFF fonts, so we convert the font to CID if
        * it is in the Type-1 format.
        * Two other tasks that we need to do are to select
        * only a single font from the CFF package (this again is
        * a PDF restriction) and to subset the CharStrings glyph
        * description.
        */
        
        
        public byte[] GetCID(String fontName)
        //throws java.io.FileNotFoundException
        {
            int j;
            for (j=0; j<fonts.Length; j++)
                if (fontName.Equals(fonts[j].name)) break;
            if (j==fonts.Length) return null;
            
            ArrayList l = new ArrayList();
            
            // copy the header
            
            Seek(0);
            
            int major = GetCard8();
            int minor = GetCard8();
            int hdrSize = GetCard8();
            int offSize = GetCard8();
            nextIndexOffset = hdrSize;
            
            l.Add(new RangeItem(buf,0,hdrSize));
            
            int nglyphs=-1, nstrings=-1;
            if ( ! fonts[j].isCID ) {
                // count the glyphs
                Seek(fonts[j].charstringsOffset);
                nglyphs = GetCard16();
                Seek(stringIndexOffset);
                nstrings = GetCard16()+standardStrings.Length;
                //System.err.Println("number of glyphs = "+nglyphs);
            }
            
            // create a name index
            
            l.Add(new UInt16Item((char)1)); // count
            l.Add(new UInt8Item((char)1)); // offSize
            l.Add(new UInt8Item((char)1)); // first offset
            l.Add(new UInt8Item((char)( 1+fonts[j].name.Length )));
            l.Add(new StringItem(fonts[j].name));
            
            // create the topdict Index
            
            
            l.Add(new UInt16Item((char)1)); // count
            l.Add(new UInt8Item((char)2)); // offSize
            l.Add(new UInt16Item((char)1)); // first offset
            OffsetItem topdictIndex1Ref = new IndexOffsetItem(2);
            l.Add(topdictIndex1Ref);
            IndexBaseItem topdictBase = new IndexBaseItem();
            l.Add(topdictBase);
            
            /*
            int maxTopdictLen = (topdictOffsets[j+1]-topdictOffsets[j])
                                + 9*2 // at most 9 new keys
                                + 8*5 // 8 new integer arguments
                                + 3*2;// 3 new SID arguments
            */
            
            //int    topdictNext = 0;
            //byte[] topdict = new byte[maxTopdictLen];
            
            OffsetItem charsetRef     = new DictOffsetItem();
            OffsetItem charstringsRef = new DictOffsetItem();
            OffsetItem fdarrayRef     = new DictOffsetItem();
            OffsetItem fdselectRef    = new DictOffsetItem();
            
            if ( !fonts[j].isCID ) {
                // create a ROS key
                l.Add(new DictNumberItem(nstrings));
                l.Add(new DictNumberItem(nstrings+1));
                l.Add(new DictNumberItem(0));
                l.Add(new UInt8Item((char)12));
                l.Add(new UInt8Item((char)30));
                // create a CIDCount key
                l.Add(new DictNumberItem(nglyphs));
                l.Add(new UInt8Item((char)12));
                l.Add(new UInt8Item((char)34));
                // What about UIDBase (12,35)? Don't know what is it.
                // I don't think we need FontName; the font I looked at didn't have it.
            }
            
            // create an FDArray key
            l.Add(fdarrayRef);
            l.Add(new UInt8Item((char)12));
            l.Add(new UInt8Item((char)36));
            // create an FDSelect key
            l.Add(fdselectRef);
            l.Add(new UInt8Item((char)12));
            l.Add(new UInt8Item((char)37));
            // create an charset key
            l.Add(charsetRef);
            l.Add(new UInt8Item((char)15));
            // create a CharStrings key
            l.Add(charstringsRef);
            l.Add(new UInt8Item((char)17));
            
            Seek(topdictOffsets[j]);
            while (GetPosition() < topdictOffsets[j+1]) {
                int p1 = GetPosition();
                GetDictItem();
                int p2 = GetPosition();
                if (key=="Encoding"
                || key=="Private"
                || key=="FDSelect"
                || key=="FDArray"
                || key=="charset"
                || key=="CharStrings"
                ) {
                    // just drop them
                } else {
                    l.Add(new RangeItem(buf,p1,p2-p1));
                }
            }
            
            l.Add(new IndexMarkerItem(topdictIndex1Ref,topdictBase));
            
            // Copy the string index and append new strings.
            // We need 3 more strings: Registry, Ordering, and a FontName for one FD.
            // The total length is at most "Adobe"+"Identity"+63 = 76
            
            if (fonts[j].isCID) {
                l.Add(GetEntireIndexRange(stringIndexOffset));
            } else {
                String fdFontName = fonts[j].name+"-OneRange";
                if (fdFontName.Length > 127)
                    fdFontName = fdFontName.Substring(0,127);
                String extraStrings = "Adobe"+"Identity"+fdFontName;
                
                int origStringsLen = stringOffsets[stringOffsets.Length-1]
                - stringOffsets[0];
                int stringsBaseOffset = stringOffsets[0]-1;
                
                byte stringsIndexOffSize;
                if (origStringsLen+extraStrings.Length <= 0xff) stringsIndexOffSize = 1;
                else if (origStringsLen+extraStrings.Length <= 0xffff) stringsIndexOffSize = 2;
                else if (origStringsLen+extraStrings.Length <= 0xffffff) stringsIndexOffSize = 3;
                else stringsIndexOffSize = 4;
                
                l.Add(new UInt16Item((char)((stringOffsets.Length-1)+3))); // count
                l.Add(new UInt8Item((char)stringsIndexOffSize)); // offSize
                for (int i=0; i<stringOffsets.Length; i++)
                    l.Add(new IndexOffsetItem(stringsIndexOffSize,
                    stringOffsets[i]-stringsBaseOffset));
                int currentStringsOffset = stringOffsets[stringOffsets.Length-1]
                - stringsBaseOffset;
                //l.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
                currentStringsOffset += ("Adobe").Length;
                l.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
                currentStringsOffset += ("Identity").Length;
                l.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
                currentStringsOffset += fdFontName.Length;
                l.Add(new IndexOffsetItem(stringsIndexOffSize,currentStringsOffset));
                
                l.Add(new RangeItem(buf,stringOffsets[0],origStringsLen));
                l.Add(new StringItem(extraStrings));
            }
            
            // copy the global subroutine index
            
            l.Add(GetEntireIndexRange(gsubrIndexOffset));
            
            // deal with fdarray, fdselect, and the font descriptors
            
            if (fonts[j].isCID) {
                // copy the FDArray, FDSelect, charset
            } else {
                // create FDSelect
                l.Add(new MarkerItem(fdselectRef));
                l.Add(new UInt8Item((char)3)); // format identifier
                l.Add(new UInt16Item((char)1)); // nRanges
                
                l.Add(new UInt16Item((char)0)); // Range[0].firstGlyph
                l.Add(new UInt8Item((char)0)); // Range[0].fd
                
                l.Add(new UInt16Item((char)nglyphs)); // sentinel
                
                // recreate a new charset
                // This format is suitable only for fonts without subsetting
                
                l.Add(new MarkerItem(charsetRef));
                l.Add(new UInt8Item((char)2)); // format identifier
                
                l.Add(new UInt16Item((char)1)); // first glyph in range (ignore .notdef)
                l.Add(new UInt16Item((char)(nglyphs-1))); // nLeft
                // now all are covered, the data structure is complete.
                
                // create a font dict index (fdarray)
                
                l.Add(new MarkerItem(fdarrayRef));
                l.Add(new UInt16Item((char)1));
                l.Add(new UInt8Item((char)1)); // offSize
                l.Add(new UInt8Item((char)1)); // first offset
                
                OffsetItem privateIndex1Ref = new IndexOffsetItem(1);
                l.Add(privateIndex1Ref);
                IndexBaseItem privateBase = new IndexBaseItem();
                l.Add(privateBase);
                
                // looking at the PS that acrobat generates from a PDF with
                // a CFF opentype font embeded with an identity-H encoding,
                // it seems that it does not need a FontName.
                //l.Add(new DictNumberItem((standardStrings.length+(stringOffsets.length-1)+2)));
                //l.Add(new UInt8Item((char)12));
                //l.Add(new UInt8Item((char)38)); // FontName
                
                l.Add(new DictNumberItem(fonts[j].privateLength));
                OffsetItem privateRef = new DictOffsetItem();
                l.Add(privateRef);
                l.Add(new UInt8Item((char)18)); // Private
                
                l.Add(new IndexMarkerItem(privateIndex1Ref,privateBase));
                
                // copy the private index & local subroutines
                
                l.Add(new MarkerItem(privateRef));
                // copy the private dict and the local subroutines.
                // the length of the private dict seems to NOT include
                // the local subroutines.
                l.Add(new RangeItem(buf,fonts[j].privateOffset,fonts[j].privateLength));
                if (fonts[j].privateSubrs >= 0) {
                    //System.err.Println("has subrs="+fonts[j].privateSubrs+" ,len="+fonts[j].privateLength);
                    l.Add(GetEntireIndexRange(fonts[j].privateSubrs));
                }
            }
            
            // copy the charstring index
            
            l.Add(new MarkerItem(charstringsRef));
            l.Add(GetEntireIndexRange(fonts[j].charstringsOffset));
            
            // now create the new CFF font
            
            int[] currentOffset = new int[1];
            currentOffset[0] = 0;
            
            foreach (Item item in l) {
                item.Increment(currentOffset);
            }
            
            foreach (Item item in l) {
                item.Xref();
            }
            
            int size = currentOffset[0];
            byte[] b = new byte[size];
            
            foreach (Item item in l) {
                item.Emit(b);
            }
            
            return b;
        }
        
        
        public bool IsCID(String fontName) {
            int j;
            for (j=0; j<fonts.Length; j++)
                if (fontName.Equals(fonts[j].name)) return fonts[j].isCID;
            return false;
        }
        
        public bool Exists(String fontName) {
            int j;
            for (j=0; j<fonts.Length; j++)
                if (fontName.Equals(fonts[j].name)) return true;
            return false;
        }
        
        
        public String[] GetNames() {
            String[] names = new String[ fonts.Length ];
            for (int i=0; i<fonts.Length; i++)
                names[i] = fonts[i].name;
            return names;
        }
        /**
        * A random Access File or an array
        * (contributed by orly manor)
        */
        protected RandomAccessFileOrArray buf;
        private int offSize;
        
        protected int nameIndexOffset;
        protected int topdictIndexOffset;
        protected int stringIndexOffset;
        protected int gsubrIndexOffset;
        protected int[] nameOffsets;
        protected int[] topdictOffsets;
        protected int[] stringOffsets;
        protected int[] gsubrOffsets;
        
        /**
        * @author orly manor
        * TODO Changed from private to protected by Ygal&Oren
        */
        protected internal class Font {
            public String    name;
            public String    fullName;
            public bool   isCID = false;
            public int       privateOffset     = -1; // only if not CID
            public int       privateLength     = -1; // only if not CID
            public int       privateSubrs      = -1;
            public int       charstringsOffset = -1;
            public int       encodingOffset    = -1;
            public int       charsetOffset     = -1;
            public int       fdarrayOffset     = -1; // only if CID
            public int       fdselectOffset    = -1; // only if CID
            public int[]     fdprivateOffsets;
            public int[]     fdprivateLengths;
            public int[]     fdprivateSubrs;
            
            // Added by Oren & Ygal
            public int nglyphs;
            public int nstrings;
            public int CharsetLength;
            public int[]    charstringsOffsets;
            public int[]    charset;
            public int[]     FDSelect;
            public int FDSelectLength;
            public int FDSelectFormat;
            public int         CharstringType = 2;
            public int FDArrayCount;
            public int FDArrayOffsize;
            public int[] FDArrayOffsets;
            public int[] PrivateSubrsOffset;
            public int[][] PrivateSubrsOffsetsArray;
            public int[]       SubrsOffsets;
        }
        // Changed from private to protected by Ygal&Oren
        protected Font[] fonts;
        
        public CFFFont(RandomAccessFileOrArray inputbuffer) {
            
            //System.err.Println("CFF: nStdString = "+standardStrings.length);
            buf = inputbuffer;
            Seek(0);
            
            int major, minor;
            major = GetCard8();
            minor = GetCard8();
            
            //System.err.Println("CFF Major-Minor = "+major+"-"+minor);
            
            int hdrSize = GetCard8();
            
            offSize = GetCard8();
            
            //System.err.Println("offSize = "+offSize);
            
            //int count, indexOffSize, indexOffset, nextOffset;
            
            nameIndexOffset    = hdrSize;
            nameOffsets        = GetIndex(nameIndexOffset);
            topdictIndexOffset = nameOffsets[nameOffsets.Length-1];
            topdictOffsets     = GetIndex(topdictIndexOffset);
            stringIndexOffset  = topdictOffsets[topdictOffsets.Length-1];
            stringOffsets      = GetIndex(stringIndexOffset);
            gsubrIndexOffset   = stringOffsets[stringOffsets.Length-1];
            gsubrOffsets       = GetIndex(gsubrIndexOffset);
            
            fonts = new Font[nameOffsets.Length-1];
            
            // now get the name index
            
            /*
            names             = new String[nfonts];
            privateOffset     = new int[nfonts];
            charsetOffset     = new int[nfonts];
            encodingOffset    = new int[nfonts];
            charstringsOffset = new int[nfonts];
            fdarrayOffset     = new int[nfonts];
            fdselectOffset    = new int[nfonts];
            */
            
            for (int j=0; j<nameOffsets.Length-1; j++) {
                fonts[j] = new Font();
                Seek(nameOffsets[j]);
                fonts[j].name = "";
                for (int k=nameOffsets[j]; k<nameOffsets[j+1]; k++) {
                    fonts[j].name += (char)GetCard8();
                }
                //System.err.Println("name["+j+"]=<"+fonts[j].name+">");
            }
            
            // string index
            
            //strings = new String[stringOffsets.length-1];
            /*
            System.err.Println("std strings = "+standardStrings.length);
            System.err.Println("fnt strings = "+(stringOffsets.length-1));
            for (char j=0; j<standardStrings.length+(stringOffsets.length-1); j++) {
                //Seek(stringOffsets[j]);
                //strings[j] = "";
                //for (int k=stringOffsets[j]; k<stringOffsets[j+1]; k++) {
                //    strings[j] += (char)getCard8();
                //}
                System.err.Println("j="+(int)j+" <? "+(standardStrings.length+(stringOffsets.length-1)));
                System.err.Println("strings["+(int)j+"]=<"+getString(j)+">");
            }
            */
            
            // top dict
            
            for (int j=0; j<topdictOffsets.Length-1; j++) {
                Seek(topdictOffsets[j]);
                while (GetPosition() < topdictOffsets[j+1]) {                
                    GetDictItem();
                    if (key=="FullName") {
                        //System.err.Println("getting fullname sid = "+((Integer)args[0]).IntValue);
                        fonts[j].fullName = GetString((char)((int)args[0]));
                        //System.err.Println("got it");
                    } else if (key=="ROS")
                        fonts[j].isCID = true;
                    else if (key=="Private") {
                        fonts[j].privateLength  = (int)args[0];
                        fonts[j].privateOffset  = (int)args[1];
                    }
                    else if (key=="charset"){
                        fonts[j].charsetOffset = (int)args[0];
                        
                    }
                    else if (key=="Encoding"){
                        fonts[j].encodingOffset = (int)args[0];
                        ReadEncoding(fonts[j].encodingOffset);
                    }
                    else if (key=="CharStrings") {
                        fonts[j].charstringsOffset = (int)args[0];
                        //System.err.Println("charstrings "+fonts[j].charstringsOffset);
                        // Added by Oren & Ygal
                        int p = GetPosition();
                        fonts[j].charstringsOffsets = GetIndex(fonts[j].charstringsOffset);
                        Seek(p);
                    } else if (key=="FDArray")
                        fonts[j].fdarrayOffset = (int)args[0];
                    else if (key=="FDSelect")
                        fonts[j].fdselectOffset = (int)args[0];
                    else if (key=="CharstringType")
                        fonts[j].CharstringType = (int)args[0];
                }
                
                // private dict
                if (fonts[j].privateOffset >= 0) {
                    //System.err.Println("PRIVATE::");
                    Seek(fonts[j].privateOffset);
                    while (GetPosition() < fonts[j].privateOffset+fonts[j].privateLength) {
                        GetDictItem();
                        if (key=="Subrs")
                            //Add the private offset to the lsubrs since the offset is 
                            // relative to the begining of the PrivateDict
                            fonts[j].privateSubrs = (int)args[0]+fonts[j].privateOffset;
                    }
                }
                
                // fdarray index
                if (fonts[j].fdarrayOffset >= 0) {
                    int[] fdarrayOffsets = GetIndex(fonts[j].fdarrayOffset);
                    
                    fonts[j].fdprivateOffsets = new int[fdarrayOffsets.Length-1];
                    fonts[j].fdprivateLengths = new int[fdarrayOffsets.Length-1];
                    
                    //System.err.Println("FD Font::");
                    
                    for (int k=0; k<fdarrayOffsets.Length-1; k++) {
                        Seek(fdarrayOffsets[k]);
                        while (GetPosition() < fdarrayOffsets[k+1])
                            GetDictItem();
                        if (key=="Private") {
                            fonts[j].fdprivateLengths[k]  = (int)args[0];
                            fonts[j].fdprivateOffsets[k]  = (int)args[1];
                        }
                        
                    }
                }
            }
            //System.err.Println("CFF: done");
        }
        
        // ADDED BY Oren & Ygal
        
        internal void ReadEncoding(int nextIndexOffset){
            int format;
            Seek(nextIndexOffset);
            format = GetCard8();
        }    
    }
}
