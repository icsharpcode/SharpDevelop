using System;
using System.IO;
using System.Text;
/*
 * Copyright 2005 by Paulo Soares.
 *
 * The contents of this file are subject to the Mozilla Public License Version 1.1
 * (the "License"); you may not use this file except inp compliance with the License.
 * You may obtain a copy of the License at http://www.mozilla.org/MPL/
 *
 * Software distributed under the License is distributed on an "AS IS" basis,
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License
 * for the specific language governing rights and limitations under the License.
 *
 * The Original Code is 'iText, a free JAVA-PDF library'.
 *
 * The Initial Developer of the Original Code is Bruno Lowagie. Portions created by
 * the Initial Developer are Copyright (C) 1999-2007 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2007 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added inp the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the "GNU LIBRARY GENERAL PUBLIC LICENSE"), inp which case the
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
 * This library is distributed inp the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general Public License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */
/********************************************************************
 *                                                                  *
 *  Title:  pfm2afm - Convert Windows .pfm files to .afm files      *
 *                                                                  *
 *  Author: Ken Borgendale   10/9/91  Version 1.0                   *
 *                                                                  *
 *  Function:                                                       *
 *      Convert a Windows .pfm (Printer Font Metrics) file to a     *
 *      .afm (Adobe Font Metrics) file.  The purpose of this is     *
 *      to allow fonts put outp for Windows to be used with OS/2.    *
 *                                                                  *
 *  Syntax:                                                         *
 *      pfm2afm  infile  [outfile] -a                               *
 *                                                                  *
 *  Copyright:                                                      *
 *      pfm2afm - Copyright (C) IBM Corp., 1991                     *
 *                                                                  *
 *      This code is released for public use as long as the         *
 *      copyright remains intact.  This code is provided asis       *
 *      without any warrenties, express or implied.                 *
 *                                                                  *
 *  Notes:                                                          *
 *      1. Much of the information inp the original .afm file is     *
 *         lost when the .pfm file is created, and thus cannot be   *
 *         reconstructed by this utility.  This is especially true  *
 *         of data for characters not inp the Windows character set. *
 *                                                                  *
 *      2. This module is coded to be compiled by the MSC 6.0.      *
 *         For other compilers, be careful of the packing of the    *
 *         PFM structure.                                           *
 *                                                                  *
 ********************************************************************/

/********************************************************************
 *                                                                  *
 *  Modifications by Rod Smith, 5/22/96                             *
 *                                                                  *
 *  These changes look for the strings "italic", "bold", "black",   *
 *  and "light" inp the font's name and set the weight accordingly   *
 *  and adds an ItalicAngle line with a value of "0" or "-12.00".   *
 *  This allows OS/2 programs such as DeScribe to handle the bold   *
 *  and italic attributes appropriately, which was not the case     *
 *  when I used the original version on fonts from the KeyFonts     *
 *  Pro 2002 font CD.                                               *
 *                                                                  *
 *  I've also increased the size of the buffer used to load the     *
 *  .PFM file; the old size was inadequate for most of the fonts    *
 *  from the SoftKey collection.                                    *
 *                                                                  *
 *  Compiled with Watcom C 10.6                                     *
 *                                                                  *
 ********************************************************************/
 
/********************************************************************
 *                                                                  *
 *  Further modifications, 4/21/98, by Rod Smith                    *
 *                                                                  *
 *  Minor changes to get the program to compile with gcc under      *
 *  Linux (Red Hat 5.0, to be precise).  I had to add an itoa       *
 *  function from the net (the function was buggy, so I had to fix  *
 *  it, too!).  I also made the program more friendly towards       *
 *  files with mixed-case filenames.                                *
 *                                                                  *
 ********************************************************************/

/********************************************************************
 *                                                                  *
 *  1/31/2005, by Paulo Soares                                      *
 *                                                                  *
 *  This code was integrated into iText.                            * 
 *  Note that the itoa function mentioned in the comment by Rod     *
 *  Smith is no longer in the code because Java has native support  *
 *  in PrintWriter to convert integers to strings                   *
 *                                                                  *
 ********************************************************************/

/********************************************************************
 *                                                                  *
 *  7/16/2005, by Bruno Lowagie                                     *
 *                                                                  *
 *  I solved an Eclipse Warning.                                    *
 *                                                                  *
 ********************************************************************/

/********************************************************************
 *                                                                  *
 *  9/14/2006, by Xavier Le Vourch                                  *
 *                                                                  *
 *  expand import clauses (import java.io.*)                        *                                           
 *  the removal of an exception in readString was restored on 9/16  *
 *                                                                  *
 ********************************************************************/

namespace iTextSharp.text.pdf {
    /**
    * Converts a PFM file into an AFM file.
    */
    public sealed class Pfm2afm {
        private RandomAccessFileOrArray inp;
        private StreamWriter outp;
        private Encoding encoding;
        
        /** Creates a new instance of Pfm2afm */
        private Pfm2afm(RandomAccessFileOrArray inp, Stream outp) {
            this.inp = inp;
            encoding = Encoding.GetEncoding(1252);
            this.outp = new StreamWriter(outp, encoding);
        }
        
        /**
        * Converts a PFM file into an AFM file.
        * @param inp the PFM file
        * @param outp the AFM file
        * @throws IOException on error
        */    
        public static void Convert(RandomAccessFileOrArray inp, Stream outp) {
            Pfm2afm p = new Pfm2afm(inp, outp);
            p.Openpfm();
            p.Putheader();
            p.Putchartab();
            p.Putkerntab();
            p.Puttrailer();
            p.outp.Flush();
        }
        
/*        public static void Main(String[] args) {
            try {
                RandomAccessFileOrArray inp = new RandomAccessFileOrArray(args[0]);
                Stream outp = new FileOutputStream(args[1]);
                Convert(inp, outp);
                inp.Close();
                outp.Close();
            }
            catch (Exception e) {
                e.PrintStackTrace();
            }
        }*/
        
        private String ReadString(int n) {
            byte[] b = new byte[n];
            inp.ReadFully(b);
            int k;
            for (k = 0; k < b.Length; ++k) {
                if (b[k] == 0)
                    break;
            }
            return encoding.GetString(b, 0, k);
        }
        
        private String ReadString() {
            StringBuilder buf = new StringBuilder();
            while (true) {
                int c = inp.Read();
                if (c <= 0)
                    break;
                buf.Append((char)c);
            }
            return buf.ToString();
        }
        
        private void Outval(int n) {
            outp.Write(' ');
            outp.Write(n);
        }
        
        /*
        *  Output a character entry
        */
        private void  Outchar(int code, int width, String name) {
            outp.Write("C ");
            Outval(code);
            outp.Write(" ; WX ");
            Outval(width);
            if (name != null) {
                outp.Write(" ; N ");
                outp.Write(name);
            }
            outp.Write(" ;\n");
        }
        
        private void Openpfm() {
            inp.Seek(0);
            vers = inp.ReadShortLE();
            h_len = inp.ReadIntLE();
            copyright = ReadString(60);
            type = inp.ReadShortLE();
            points = inp.ReadShortLE();
            verres = inp.ReadShortLE();
            horres = inp.ReadShortLE();
            ascent = inp.ReadShortLE();
            intleading = inp.ReadShortLE();
            extleading = inp.ReadShortLE();
            italic = (byte)inp.Read();
            uline = (byte)inp.Read();
            overs = (byte)inp.Read();
            weight = inp.ReadShortLE();
            charset = (byte)inp.Read();
            pixwidth = inp.ReadShortLE();
            pixheight = inp.ReadShortLE();
            kind = (byte)inp.Read();
            avgwidth = inp.ReadShortLE();
            maxwidth = inp.ReadShortLE();
            firstchar = inp.Read();
            lastchar = inp.Read();
            defchar = (byte)inp.Read();
            brkchar = (byte)inp.Read();
            widthby = inp.ReadShortLE();
            device = inp.ReadIntLE();
            face = inp.ReadIntLE();
            bits = inp.ReadIntLE();
            bitoff = inp.ReadIntLE();
            extlen = inp.ReadShortLE();
            psext = inp.ReadIntLE();
            chartab = inp.ReadIntLE();
            res1 = inp.ReadIntLE();
            kernpairs = inp.ReadIntLE();
            res2 = inp.ReadIntLE();
            fontname = inp.ReadIntLE();
            if (h_len != inp.Length || extlen != 30 || fontname < 75 || fontname > 512)
                throw new IOException("Not a valid PFM file.");
            inp.Seek(psext + 14);
            capheight = inp.ReadShortLE();
            xheight = inp.ReadShortLE();
            ascender = inp.ReadShortLE();
            descender = inp.ReadShortLE();
        }
        
        private void Putheader() {
            outp.Write("StartFontMetrics 2.0\n");
            if (copyright.Length > 0)
                outp.Write("Comment " + copyright + '\n');
            outp.Write("FontName ");
            inp.Seek(fontname);
            String fname = ReadString();
            outp.Write(fname);
            outp.Write("\nEncodingScheme ");
            if (charset != 0)
                outp.Write("FontSpecific\n");
            else
                outp.Write("AdobeStandardEncoding\n");
            /*
            * The .pfm is missing full name, so construct from font name by
            * changing the hyphen to a space.  This actually works inp a lot
            * of cases.
            */
            outp.Write("FullName " + fname.Replace('-', ' '));
            if (face != 0) {
                inp.Seek(face);
                outp.Write("\nFamilyName " + ReadString());
            }

            outp.Write("\nWeight ");
            if (weight > 475 || fname.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf("bold") >= 0)
            outp.Write("Bold");
            else if ((weight < 325 && weight != 0) || fname.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf("light") >= 0)
                outp.Write("Light");
            else if (fname.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf("black") >= 0)
                outp.Write("Black");
            else 
                outp.Write("Medium");

            outp.Write("\nItalicAngle ");
            if (italic != 0 || fname.ToLower(System.Globalization.CultureInfo.InvariantCulture).IndexOf("italic") >= 0)
                outp.Write("-12.00");
                /* this is a typical value; something else may work better for a
                specific font */
            else
                outp.Write("0");

            /*
            *  The mono flag inp the pfm actually indicates whether there is a
            *  table of font widths, not if they are all the same.
            */
            outp.Write("\nIsFixedPitch ");
            if ((kind & 1) == 0 ||                  /* Flag for mono */
                avgwidth == maxwidth ) {  /* Avg width = max width */
                outp.Write("true");
                isMono = true;
            }
            else {
                outp.Write("false");
                isMono = false;
            }

            /*
            * The font bounding box is lost, but try to reconstruct it.
            * Much of this is just guess work.  The bounding box is required inp
            * the .afm, but is not used by the PM font installer.
            */
            outp.Write("\nFontBBox");
            if (isMono)
                Outval(-20);      /* Just guess at left bounds */
            else 
                Outval(-100);
            Outval(-(descender+5));  /* Descender is given as positive value */
            Outval(maxwidth+10);
            Outval(ascent+5);

            /*
            * Give other metrics that were kept
            */
            outp.Write("\nCapHeight");
            Outval(capheight);
            outp.Write("\nXHeight");
            Outval(xheight);
            outp.Write("\nDescender");
            Outval(descender);
            outp.Write("\nAscender");
            Outval(ascender);
            outp.Write('\n');
        }
        
        private void Putchartab() {
            int count = lastchar - firstchar + 1;
            int[] ctabs = new int[count];
            inp.Seek(chartab);
            for (int k = 0; k < count; ++k)
                ctabs[k] = inp.ReadUnsignedShortLE();
            int[] back = new int[256];
            if (charset == 0) {
                for (int i = firstchar; i <= lastchar; ++i) {
                    if (Win2PSStd[i] != 0)
                        back[Win2PSStd[i]] = i;
                }
            }
            /* Put outp the header */
            outp.Write("StartCharMetrics");
            Outval(count);
            outp.Write('\n');

            /* Put outp all encoded chars */
            if (charset != 0) {
            /*
            * If the charset is not the Windows standard, just put outp
            * unnamed entries.
            */
                for (int i = firstchar; i <= lastchar; i++) {
                    if (ctabs[i - firstchar] != 0) {
                        Outchar(i, ctabs[i - firstchar], null);
                    }
                }
            }
            else {
                for (int i = 0; i < 256; i++) {
                    int j = back[i];
                    if (j != 0) {
                        Outchar(i, ctabs[j - firstchar], WinChars[j]);
                        ctabs[j - firstchar] = 0;
                    }
                }
                /* Put outp all non-encoded chars */
                for (int i = firstchar; i <= lastchar; i++) {
                    if (ctabs[i - firstchar] != 0) {
                        Outchar(-1, ctabs[i - firstchar], WinChars[i]);
                    }
                }
            }
            /* Put outp the trailer */
            outp.Write("EndCharMetrics\n");
            
        }
        
        private void Putkerntab() {
            if (kernpairs == 0)
                return;
            inp.Seek(kernpairs);
            int count = inp.ReadUnsignedShortLE();
            int nzero = 0;
            int[] kerns = new int[count * 3];
            for (int k = 0; k < kerns.Length;) {
                kerns[k++] = inp.Read();
                kerns[k++] = inp.Read();
                if ((kerns[k++] = inp.ReadShortLE()) != 0)
                    ++nzero;
            }
            if (nzero == 0)
                return;
            outp.Write("StartKernData\nStartKernPairs");
            Outval(nzero);
            outp.Write('\n');
            for (int k = 0; k < kerns.Length; k += 3) {
                if (kerns[k + 2] != 0) {
                    outp.Write("KPX ");
                    outp.Write(WinChars[kerns[k]]);
                    outp.Write(' ');
                    outp.Write(WinChars[kerns[k + 1]]);
                    Outval(kerns[k + 2]);
                    outp.Write('\n');
                }
            }
            /* Put outp trailer */
            outp.Write("EndKernPairs\nEndKernData\n");
        }
        

        private void  Puttrailer() {
            outp.Write("EndFontMetrics\n");
        }

        private short  vers;
        private int   h_len;             /* Total length of .pfm file */
        private String   copyright;   /* Copyright string [60]*/
        private short  type;
        private short  points;
        private short  verres;
        private short  horres;
        private short  ascent;
        private short  intleading;
        private short  extleading;
        private byte   italic;
        private byte   uline;
        private byte   overs;
        private short  weight;
        private byte   charset;         /* 0=windows, otherwise nomap */
        private short  pixwidth;        /* Width for mono fonts */
        private short  pixheight;
        private byte   kind;            /* Lower bit off inp mono */
        private short  avgwidth;        /* Mono if avg=max width */
        private short  maxwidth;        /* Use to compute bounding box */
        private int   firstchar;       /* First char inp table */
        private int   lastchar;        /* Last char inp table */
        private byte   defchar;
        private byte   brkchar;
        private short  widthby;
        private int   device;
        private int   face;            /* Face name */
        private int   bits;
        private int   bitoff;
        private short  extlen;
        private int   psext;           /* PostScript extension */
        private int   chartab;         /* Character width tables */
        private int   res1;
        private int   kernpairs;       /* Kerning pairs */
        private int   res2;
        private int   fontname;        /* Font name */

    /*
    *  Some metrics from the PostScript extension
    */
        private short  capheight;       /* Cap height */
        private short  xheight;         /* X height */
        private short  ascender;        /* Ascender */
        private short  descender;       /* Descender (positive) */

        
        private bool isMono;
    /*
    * Translate table from 1004 to psstd.  1004 is an extension of the
    * Windows translate table used inp PM.
    */
        private int[] Win2PSStd = {
            0,   0,   0,   0, 197, 198, 199,   0, 202,   0, 205, 206, 207,   0,   0,   0,
            0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,   0,
            32,  33,  34,  35,  36,  37,  38, 169,  40,  41,  42,  43,  44,  45,  46,  47,
            48,  49,  50,  51,  52,  53,  54,  55,  56,  57,  58,  59,  60,  61,  62,  63,
            64,  65,  66,  67,  68,  69,  70,  71,  72,  73,  74,  75,  76,  77,  78,  79,
            80,  81,  82,  83,  84,  85,  86,  87,  88,  89,  90,  91,  92,  93,  94,  95,
            193,  97,  98,  99, 100, 101, 102, 103, 104, 105, 106, 107, 108, 109, 110, 111,
            112, 113, 114, 115, 116, 117, 118, 119, 120, 121, 122, 123, 124, 125, 126, 127,
            0,   0, 184,   0, 185, 188, 178, 179,  94, 189,   0, 172, 234,   0,   0,   0,
            0,  96,   0, 170, 186,   0, 177, 208, 126,   0,   0, 173, 250,   0,   0,   0,
            0, 161, 162, 163, 168, 165,   0, 167, 200,   0, 227, 171,   0,   0,   0,   0,
            0,   0,   0,   0, 194,   0, 182, 180, 203,   0, 235, 187,   0,   0,   0, 191,
            0,   0,   0,   0,   0,   0, 225,   0,   0,   0,   0,   0,   0,   0,   0,   0,
            0,   0,   0,   0,   0,   0,   0,   0, 233,   0,   0,   0,   0,   0,   0, 251,
            0,   0,   0,   0,   0,   0, 241,   0,   0,   0,   0,   0,   0,   0,   0,   0,
            0,   0,   0,   0,   0,   0,   0,   0, 249,   0,   0,   0,   0,   0,   0,   0
        };
        
    /*
    *  Character class.  This is a minor attempt to overcome the problem that
    *  inp the pfm file, all unused characters are given the width of space.
    */
        private int[] WinClass = {
            0, 0, 0, 0, 2, 2, 2, 0, 2, 0, 2, 2, 2, 0, 0, 0,   /* 00 */
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,   /* 10 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 20 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 30 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 40 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 50 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* 60 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2,   /* 70 */
            0, 0, 2, 0, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 0,   /* 80 */
            0, 3, 3, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 0, 0, 2,   /* 90 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* a0 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* b0 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* c0 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* d0 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,   /* e0 */
            1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1   /* f0 */
        };
        
    /*
    *  Windows chararacter names.  Give a name to the usused locations
    *  for when the all flag is specified.
    */
        private String[] WinChars = {
            "W00",              /*   00    */
            "W01",              /*   01    */
            "W02",              /*   02    */
            "W03",              /*   03    */
            "macron",           /*   04    */
            "breve",            /*   05    */
            "dotaccent",        /*   06    */
            "W07",              /*   07    */
            "ring",             /*   08    */
            "W09",              /*   09    */
            "W0a",              /*   0a    */
            "W0b",              /*   0b    */
            "W0c",              /*   0c    */
            "W0d",              /*   0d    */
            "W0e",              /*   0e    */
            "W0f",              /*   0f    */
            "hungarumlaut",     /*   10    */
            "ogonek",           /*   11    */
            "caron",            /*   12    */
            "W13",              /*   13    */
            "W14",              /*   14    */
            "W15",              /*   15    */
            "W16",              /*   16    */
            "W17",              /*   17    */
            "W18",              /*   18    */
            "W19",              /*   19    */
            "W1a",              /*   1a    */
            "W1b",              /*   1b    */
            "W1c",              /*   1c    */
            "W1d",              /*   1d    */
            "W1e",              /*   1e    */
            "W1f",              /*   1f    */
            "space",            /*   20    */
            "exclam",           /*   21    */
            "quotedbl",         /*   22    */
            "numbersign",       /*   23    */
            "dollar",           /*   24    */
            "percent",          /*   25    */
            "ampersand",        /*   26    */
            "quotesingle",      /*   27    */
            "parenleft",        /*   28    */
            "parenright",       /*   29    */
            "asterisk",         /*   2A    */
            "plus",             /*   2B    */
            "comma",            /*   2C    */
            "hyphen",           /*   2D    */
            "period",           /*   2E    */
            "slash",            /*   2F    */
            "zero",             /*   30    */
            "one",              /*   31    */
            "two",              /*   32    */
            "three",            /*   33    */
            "four",             /*   34    */
            "five",             /*   35    */
            "six",              /*   36    */
            "seven",            /*   37    */
            "eight",            /*   38    */
            "nine",             /*   39    */
            "colon",            /*   3A    */
            "semicolon",        /*   3B    */
            "less",             /*   3C    */
            "equal",            /*   3D    */
            "greater",          /*   3E    */
            "question",         /*   3F    */
            "at",               /*   40    */
            "A",                /*   41    */
            "B",                /*   42    */
            "C",                /*   43    */
            "D",                /*   44    */
            "E",                /*   45    */
            "F",                /*   46    */
            "G",                /*   47    */
            "H",                /*   48    */
            "I",                /*   49    */
            "J",                /*   4A    */
            "K",                /*   4B    */
            "L",                /*   4C    */
            "M",                /*   4D    */
            "N",                /*   4E    */
            "O",                /*   4F    */
            "P",                /*   50    */
            "Q",                /*   51    */
            "R",                /*   52    */
            "S",                /*   53    */
            "T",                /*   54    */
            "U",                /*   55    */
            "V",                /*   56    */
            "W",                /*   57    */
            "X",                /*   58    */
            "Y",                /*   59    */
            "Z",                /*   5A    */
            "bracketleft",      /*   5B    */
            "backslash",        /*   5C    */
            "bracketright",     /*   5D    */
            "asciicircum",      /*   5E    */
            "underscore",       /*   5F    */
            "grave",            /*   60    */
            "a",                /*   61    */
            "b",                /*   62    */
            "c",                /*   63    */
            "d",                /*   64    */
            "e",                /*   65    */
            "f",                /*   66    */
            "g",                /*   67    */
            "h",                /*   68    */
            "i",                /*   69    */
            "j",                /*   6A    */
            "k",                /*   6B    */
            "l",                /*   6C    */
            "m",                /*   6D    */
            "n",                /*   6E    */
            "o",                /*   6F    */
            "p",                /*   70    */
            "q",                /*   71    */
            "r",                /*   72    */
            "s",                /*   73    */
            "t",                /*   74    */
            "u",                /*   75    */
            "v",                /*   76    */
            "w",                /*   77    */
            "x",                /*   78    */
            "y",                /*   79    */
            "z",                /*   7A    */
            "braceleft",        /*   7B    */
            "bar",              /*   7C    */
            "braceright",       /*   7D    */
            "asciitilde",       /*   7E    */
            "W7f",              /*   7F    */
            "W80",              /*   80    */
            "W81",              /*   81    */
            "quotesinglbase",   /*   82    */
            "W83",              /*   83    */
            "quotedblbase",     /*   84    */
            "ellipsis",         /*   85    */
            "dagger",           /*   86    */
            "daggerdbl",        /*   87    */
            "asciicircum",      /*   88    */
            "perthousand",      /*   89    */
            "Scaron",           /*   8A    */
            "guilsinglleft",    /*   8B    */
            "OE",               /*   8C    */
            "W8d",              /*   8D    */
            "W8e",              /*   8E    */
            "W8f",              /*   8F    */
            "W90",              /*   90    */
            "quoteleft",        /*   91    */
            "quoteright",       /*   92    */
            "quotedblleft",     /*   93    */
            "quotedblright",    /*   94    */
            "bullet1",          /*   95    */
            "endash",           /*   96    */
            "emdash",           /*   97    */
            "asciitilde",       /*   98    */
            "trademark",        /*   99    */
            "scaron",           /*   9A    */
            "guilsinglright",   /*   9B    */
            "oe",               /*   9C    */
            "W9d",              /*   9D    */
            "W9e",              /*   9E    */
            "Ydieresis",        /*   9F    */
            "reqspace",         /*   A0    */
            "exclamdown",       /*   A1    */
            "cent",             /*   A2    */
            "sterling",         /*   A3    */
            "currency",         /*   A4    */
            "yen",              /*   A5    */
            "brokenbar",        /*   A6    */
            "section",          /*   A7    */
            "dieresis",         /*   A8    */
            "copyright",        /*   A9    */
            "ordfeminine",      /*   AA    */
            "guillemotleft",    /*   AB    */
            "logicalnot",       /*   AC    */
            "syllable",         /*   AD    */
            "registered",       /*   AE    */
            "overbar",          /*   AF    */
            "degree",           /*   B0    */
            "plusminus",        /*   B1    */
            "twosuperior",      /*   B2    */
            "threesuperior",    /*   B3    */
            "acute",            /*   B4    */
            "mu",               /*   B5    */
            "paragraph",        /*   B6    */
            "periodcentered",   /*   B7    */
            "cedilla",          /*   B8    */
            "onesuperior",      /*   B9    */
            "ordmasculine",     /*   BA    */
            "guillemotright",   /*   BB    */
            "onequarter",       /*   BC    */
            "onehalf",          /*   BD    */
            "threequarters",    /*   BE    */
            "questiondown",     /*   BF    */
            "Agrave",           /*   C0    */
            "Aacute",           /*   C1    */
            "Acircumflex",      /*   C2    */
            "Atilde",           /*   C3    */
            "Adieresis",        /*   C4    */
            "Aring",            /*   C5    */
            "AE",               /*   C6    */
            "Ccedilla",         /*   C7    */
            "Egrave",           /*   C8    */
            "Eacute",           /*   C9    */
            "Ecircumflex",      /*   CA    */
            "Edieresis",        /*   CB    */
            "Igrave",           /*   CC    */
            "Iacute",           /*   CD    */
            "Icircumflex",      /*   CE    */
            "Idieresis",        /*   CF    */
            "Eth",              /*   D0    */
            "Ntilde",           /*   D1    */
            "Ograve",           /*   D2    */
            "Oacute",           /*   D3    */
            "Ocircumflex",      /*   D4    */
            "Otilde",           /*   D5    */
            "Odieresis",        /*   D6    */
            "multiply",         /*   D7    */
            "Oslash",           /*   D8    */
            "Ugrave",           /*   D9    */
            "Uacute",           /*   DA    */
            "Ucircumflex",      /*   DB    */
            "Udieresis",        /*   DC    */
            "Yacute",           /*   DD    */
            "Thorn",            /*   DE    */
            "germandbls",       /*   DF    */
            "agrave",           /*   E0    */
            "aacute",           /*   E1    */
            "acircumflex",      /*   E2    */
            "atilde",           /*   E3    */
            "adieresis",        /*   E4    */
            "aring",            /*   E5    */
            "ae",               /*   E6    */
            "ccedilla",         /*   E7    */
            "egrave",           /*   E8    */
            "eacute",           /*   E9    */
            "ecircumflex",      /*   EA    */
            "edieresis",        /*   EB    */
            "igrave",           /*   EC    */
            "iacute",           /*   ED    */
            "icircumflex",      /*   EE    */
            "idieresis",        /*   EF    */
            "eth",              /*   F0    */
            "ntilde",           /*   F1    */
            "ograve",           /*   F2    */
            "oacute",           /*   F3    */
            "ocircumflex",      /*   F4    */
            "otilde",           /*   F5    */
            "odieresis",        /*   F6    */
            "divide",           /*   F7    */
            "oslash",           /*   F8    */
            "ugrave",           /*   F9    */
            "uacute",           /*   FA    */
            "ucircumflex",      /*   FB    */
            "udieresis",        /*   FC    */
            "yacute",           /*   FD    */
            "thorn",            /*   FE    */
            "ydieresis"        /*   FF    */
        };
    }
}
