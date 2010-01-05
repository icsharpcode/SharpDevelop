using System;
using System.IO;
using System.Text;
using System.Globalization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.rtf.direct;
using iTextSharp.text.rtf.graphic;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.parser.ctrlwords;
/*
 * $Id: RtfDestinationShppict.cs,v 1.2 2008/05/13 11:26:00 psoares33 Exp $
 * 
 *
 * Copyright 2007 by Howard Shank (hgshank@yahoo.com)
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
 * the Initial Developer are Copyright (C) 1999-2006 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2006 by Paulo Soares. All Rights Reserved.
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
 
namespace iTextSharp.text.rtf.parser.destinations {

    /**
    * <code>RtfDestinationShppict</code> handles data destined for picture destinations
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    * @since 2.0.8
    */
    public class RtfDestinationShppict : RtfDestination {
        private ByteBuffer data = null;

        private StringBuilder hexChars = new StringBuilder(0);
        private StringBuilder buffer = new StringBuilder();

        /* picttype */
        private int pictureType = Image.ORIGINAL_NONE;
        public const int ORIGINAL_NONE = 0;
        public const int ORIGINAL_GIF = 3;
        public const int ORIGINAL_TIFF = 5;
        public const int ORIGINAL_PS = 7;

        // emfblip - EMF (nhanced metafile) - NOT HANDLED
        // pngblip int ORIGINAL_PNG = 2;
        // jpegblip Image.ORIGINAL_JPEG = 1; ORIGINAL_JPEG2000 = 8;

        // shppict - Destination
        // nonshpict - Destination - SKIP THIS
        // macpict - Mac QuickDraw- NOT HANDLED
        // pmmetafileN - OS/2 Metafile - NOT HANDLED
            // N * Meaning
            // 0x0004 PU_ARBITRARY
            // 0x0008 PU_PELS
            // 0x000C PU_LOMETRIC
            // 0x0010 PU_HIMETRIC
            // 0x0014 PU_LOENGLISH
            // 0x0018 PU_HIENGLISH
            // 0x001C PU_TWIPS
        //private int pmmetafile = 0;
        // wmetafileN Image.RIGINAL_WMF = 6;
        // N * Type
        // 1 = MM_TEXT
        // 2 = M_LOMETRIC
        // 3 = MM_HIMETRIC
        // 4 = MM_LOENGLISH
        // 5 = MM_HIENGLISH
        // 6 = MM_TWIPS
        // 7 = MM_ISOTROPIC
        // 8 = MM_ANISOTROPIC
        // dibitmapN - DIB - Convert to BMP?
        // wbitmapN Image.ORIGINAL_BMP = 4;
        
        /* bitapinfo */
        // wbmbitspixelN - number of bits per pixel - 1 monochrome, 4 16 color, 8 256 color, 24 RGB - Default 1
        //private int bitsPerPixel = 1;
        // wbmplanesN - number of color planes - must be 1
        //private int planes = 1;
        // wbmwidthbytesN - number of bytes in each raster line
        //private int widthBytes = 0;
        
        
        
        /* pictsize */
        // picwN Ext field if the picture is a Windows metafile; picture width in pixels if the picture is a bitmap or
        // from quickdraw
        private long width = 0;
        // pichN
        private long height = 0;
        // picwgoalN
        private long desiredWidth = 0;
        // picgoalN
        private long desiredHeight = 0;
        // picscalexN
        private int scaleX = 100;
        // picscaleyN
        private int scaleY = 100;
        // picscaled - macpict setting
        //private bool scaled = false;
        // picprop
        //private bool inlinePicture = false;
        // defshp
        //private bool wordArt = false;
        // piccroptN
        private int cropTop = 0;
        // piccropbN
        private int cropBottom = 0;
        // piccroplN
        private int cropLeft = 0;
        // piccroprN
        private int cropRight = 0;
        
        /* metafileinfo */
        // picbmp
        //private bool bitmap = false;
        //picbppN - Valid 1,4,8,24
        //private int bbp = 1;
        
        /* data */
        // binN
        // 0 = HEX, 1 = BINARY
        public const int FORMAT_HEXADECIMAL = 0;
        public const int FORMAT_BINARY = 1;
        private int dataFormat = FORMAT_HEXADECIMAL;
        private long binaryLength = 0;
        // blipupiN
        //private int unitsPerInch = 0;
        // bliptagN
        //private String tag = "";
        private const int NORMAL = 0;
        private const int BLIPUID = 1;
        
        //private int state = NORMAL;
        /**
        * Constant for converting pixels to twips
        */
        private const int PIXEL_TWIPS_FACTOR = 15;
        
        private MemoryStream dataOS = null;
        
        public RtfDestinationShppict() : base(null) {
            this.pictureType = pictureType; //get rid of a warning
        }

        /**
        * Constructs a new RtfDestinationShppict.
        */
        public RtfDestinationShppict(RtfParser parser) : base(parser) {
        }
        
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#closeDestination()
        */
        public override bool CloseDestination() {
            if (this.rtfParser.IsImport()) {
                if (this.buffer.Length>0) {
                    WriteBuffer();
                }
            }
            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupEnd()
        */
        public override bool HandleCloseGroup() {
            this.OnCloseGroup();    // event handler
            
            if (this.rtfParser.IsImport()) {
                if (this.buffer.Length>0) {
                    WriteBuffer();
                }
                if (dataOS != null) {
                    AddImage();
                    dataOS = null;
                }
                this.WriteText("}");
                return true;
            }
            if (this.rtfParser.IsConvert()) {
            }
            return true;
        }

        private bool AddImage() {
            Image img = null;
            try {
                img = Image.GetInstance(dataOS.ToArray());
            } catch {
            }
//                if (img != null) {
//                    FileOutputStream out =null;
//                    try {
//                        out = new FileOutputStream("c:\\test.png");
//                        out.Write(img.GetOriginalData());
//                        out.Close();
//                    } catch (FileNotFoundException e1) {
//                        // TODO Auto-generated catch block
//                        e1.PrintStackTrace();
//                    } catch (IOException e1) {
//                        // TODO Auto-generated catch block
//                        e1.PrintStackTrace();
//                    }

                if (img != null) {      
                    img.ScaleAbsolute((float)this.desiredWidth/PIXEL_TWIPS_FACTOR, (float)this.desiredHeight/PIXEL_TWIPS_FACTOR);
                    img.ScaleAbsolute((float)this.width/PIXEL_TWIPS_FACTOR, (float)this.height/PIXEL_TWIPS_FACTOR);
                    img.ScalePercent((float)this.scaleX, this.scaleY);
                    
                    try {
                        if (this.rtfParser.IsImport()) {
                            RtfDocument rtfDoc = this.rtfParser.GetRtfDocument();
                            RtfImage rtfImage = new RtfImage(rtfDoc, img);
                            rtfDoc.Add(rtfImage);
                        }
                        if (this.rtfParser.IsConvert()) {
                            this.rtfParser.GetDocument().Add(img);
                        }
                    } catch {
                    }
                }
                dataFormat = FORMAT_HEXADECIMAL;
                return true;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleGroupStart()
        */
        public override bool HandleOpenGroup() {
            this.OnOpenGroup(); // event handler
            
            if (this.rtfParser.IsImport()) {
            }
            if (this.rtfParser.IsConvert()) {
            }
            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.parser.destinations.RtfDestination#handleOpenNewGroup()
        */
        public override bool HandleOpeningSubGroup() {
            if (this.rtfParser.IsImport()) {
                if (this.buffer.Length>0) {
                    WriteBuffer();
                }
            }
            return true;
        }
        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#handleCharacter(int)
        */
        public override bool HandleCharacter(int ch) {
            
            if (this.rtfParser.IsImport()) {
                if (buffer.Length > 254)
                    WriteBuffer();
            }
            if (data == null) data = new ByteBuffer();
            switch (dataFormat) {
            case FORMAT_HEXADECIMAL:
                hexChars.Append(ch);
                if (hexChars.Length == 2) {
                    try {
                        data.Append((char)int.Parse(hexChars.ToString(), NumberStyles.HexNumber));
                    } catch  {
                    }
                    hexChars = new StringBuilder();
                }
                break;
            case FORMAT_BINARY:
                if (dataOS == null) { 
                    dataOS = new MemoryStream();
                }
                // HGS - FIX ME IF PROBLEM!
                dataOS.WriteByte((byte)ch);
                // PNG signature should be.
    //             (decimal)              137  80  78  71  13  10  26  10
    //             (hexadecimal)           89  50  4e  47  0d  0a  1a  0a
    //             (ASCII C notation)    \211   P   N   G  \r  \n \032 \n

                binaryLength--;
                if (binaryLength == 0) { dataFormat = FORMAT_HEXADECIMAL; }
                break;
            }
            
            return true;
        }

        public override bool HandleControlWord(RtfCtrlWordData ctrlWordData) {
            bool result = false;
            bool skipCtrlWord = false;
            if (this.rtfParser.IsImport()) {
                skipCtrlWord = true;
                if (ctrlWordData.ctrlWord.Equals("shppict")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("nonshppict")) {    // never gets here because this is a destination set to null
                    skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;
                }
                if (ctrlWordData.ctrlWord.Equals("blipuid")) { skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("picprop")) { skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("pict")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("emfblip")) { result = true; pictureType = Image.ORIGINAL_NONE;}
                if (ctrlWordData.ctrlWord.Equals("pngblip")) { result = true; pictureType = Image.ORIGINAL_PNG;}
                if (ctrlWordData.ctrlWord.Equals("jepgblip")) { result = true; pictureType = Image.ORIGINAL_JPEG;}
                if (ctrlWordData.ctrlWord.Equals("macpict")) { result = true; pictureType = Image.ORIGINAL_NONE;}
                if (ctrlWordData.ctrlWord.Equals("pmmetafile")) { result = true; pictureType = Image.ORIGINAL_NONE;}
                if (ctrlWordData.ctrlWord.Equals("wmetafile")) { result = true; pictureType = Image.ORIGINAL_WMF;}
                if (ctrlWordData.ctrlWord.Equals("dibitmap")) { result = true; pictureType = Image.ORIGINAL_NONE;}
                if (ctrlWordData.ctrlWord.Equals("wbitmap")) { result = true; pictureType = Image.ORIGINAL_BMP;}
                /* bitmap information */
                if (ctrlWordData.ctrlWord.Equals("wbmbitspixel")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("wbmplanes")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("wbmwidthbytes")) { result = true;}
                /* picture size, scaling and cropping */
                if (ctrlWordData.ctrlWord.Equals("picw")) { this.width = ctrlWordData.LongValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("pich")) { this.height = ctrlWordData.LongValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("picwgoal")) { this.desiredWidth = ctrlWordData.LongValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("pichgoal")) { this.desiredHeight = ctrlWordData.LongValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("picscalex")) { this.scaleX = ctrlWordData.IntValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("picscaley")) { this.scaleY = ctrlWordData.IntValue();result = true;}
                if (ctrlWordData.ctrlWord.Equals("picscaled")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("picprop")) { skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("defshp")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("piccropt")) { this.cropTop = ctrlWordData.IntValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("piccropb")) { this.cropBottom = ctrlWordData.IntValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("piccropl")) { this.cropLeft = ctrlWordData.IntValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("piccropr")) { this.cropRight = ctrlWordData.IntValue(); result = true;}
                /* metafile information */
                if (ctrlWordData.ctrlWord.Equals("picbmp")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("picbpp")) { result = true;}
                /* picture data */
                if (ctrlWordData.ctrlWord.Equals("bin")) { 
                    this.dataFormat = FORMAT_BINARY;
                    // set length to param
                    this.binaryLength = ctrlWordData.LongValue();
                    this.rtfParser.SetTokeniserStateBinary(binaryLength);
                    result = true;
                }
                if (ctrlWordData.ctrlWord.Equals("blipupi")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("blipuid")) { skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("bliptag")) { result = true;}
            }
            if (this.rtfParser.IsConvert()) {
                if (ctrlWordData.ctrlWord.Equals("shppict")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("nonshppict")) { skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("blipuid")) { result = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("pict")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("emfblip")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("pngblip")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("jepgblip")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("macpict")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("pmmetafile")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("wmetafile")) {  skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("dibitmap")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("wbitmap")) { result = true;}
                /* bitmap information */
                if (ctrlWordData.ctrlWord.Equals("wbmbitspixel")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("wbmplanes")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("wbmwidthbytes")) { result = true;}
                /* picture size, scaling and cropping */
                if (ctrlWordData.ctrlWord.Equals("picw")) { this.width = ctrlWordData.LongValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("pich")) { this.height = ctrlWordData.LongValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("picwgoal")) { this.desiredWidth = ctrlWordData.LongValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("pichgoal")) { this.desiredHeight = ctrlWordData.LongValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("picscalex")) { this.scaleX = ctrlWordData.IntValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("picscaley")) { this.scaleY = ctrlWordData.IntValue();result = true;}
                if (ctrlWordData.ctrlWord.Equals("picscaled")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("picprop")) { skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("defshp")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("piccropt")) { this.cropTop = ctrlWordData.IntValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("piccropb")) { this.cropBottom = ctrlWordData.IntValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("piccropl")) { this.cropLeft = ctrlWordData.IntValue(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("piccropr")) { this.cropRight = ctrlWordData.IntValue(); result = true;}
                /* metafile information */
                if (ctrlWordData.ctrlWord.Equals("picbmp")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("picbpp")) { result = true;}
                /* picture data */
                if(ctrlWordData.ctrlWord.Equals("bin")) { 
                    dataFormat = FORMAT_BINARY; result = true;
                }
                if (ctrlWordData.ctrlWord.Equals("blipupi")) { result = true;}
                if (ctrlWordData.ctrlWord.Equals("blipuid")) { skipCtrlWord = true; this.rtfParser.SetTokeniserStateSkipGroup(); result = true;}
                if (ctrlWordData.ctrlWord.Equals("bliptag")) { result = true;}
            
            }
            if (!skipCtrlWord) {
            switch (this.rtfParser.GetConversionType()) {
            case RtfParser.TYPE_IMPORT_FULL:
                    WriteBuffer();
                    WriteText(ctrlWordData.ToString());
                result = true;
                break;      
            case RtfParser.TYPE_IMPORT_FRAGMENT:
                    WriteBuffer();
                    WriteText(ctrlWordData.ToString());
                result = true;
                break;
            case RtfParser.TYPE_CONVERT:
                result = true;
                break;
            default:    // error because is should be an import or convert
                result = false;
                break;
            }
            }
            return result;
        }

        /* (non-Javadoc)
        * @see com.lowagie.text.rtf.direct.RtfDestination#setDefaults()
        */
        public override void SetToDefaults() {
            
            this.buffer = new StringBuilder();
            this.data = null;
            this.width = 0;
            this.height = 0;
            this.desiredWidth = 0;
            this.desiredHeight = 0;
            this.scaleX = 100;
            this.scaleY = 100;
            //this.scaled = false;
            //this.inlinePicture = false;
            //this.wordArt = false;
            this.cropTop = 0;
            this.cropBottom = 0;
            this.cropLeft = 0;
            this.cropRight = 0;
            //this.bitmap = false;
            //this.bbp = 1;
            this.dataFormat = FORMAT_HEXADECIMAL;
            this.binaryLength = 0;
            //this.unitsPerInch = 0;
            //this.tag = "";
        }

        private void WriteBuffer() {
            WriteText(this.buffer.ToString());
        }
        private void WriteText(String value) {
            if (this.rtfParser.GetState().newGroup) {
                this.rtfParser.GetRtfDocument().Add(new RtfDirectContent("{"));
                this.rtfParser.GetState().newGroup = false;
            }
            if (value.Length > 0) {
                this.rtfParser.GetRtfDocument().Add(new RtfDirectContent(value));
            }
        }

    }
}