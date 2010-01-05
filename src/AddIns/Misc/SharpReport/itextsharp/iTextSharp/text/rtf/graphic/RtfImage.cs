using System;
using System.IO;
using System.Net;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
using iTextSharp.text.rtf.document.output;
using iTextSharp.text.rtf.text;
using iTextSharp.text.rtf.style;
using iTextSharp.text.pdf.codec.wmf;
/*
 * $Id: RtfImage.cs,v 1.11 2008/05/16 19:30:59 psoares33 Exp $
 * 
 *
 * Copyright 2001, 2002, 2003, 2004 by Mark Hall
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

namespace iTextSharp.text.rtf.graphic {

    /**
    * The RtfImage contains one image. Supported image types are jpeg, png, wmf, bmp.
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    * @author Paulo Soares
    */
    public class RtfImage : RtfElement {
        
        /**
        * Constant for the shape/picture group
        */
        private static byte[] PICTURE_GROUP = DocWriter.GetISOBytes("\\*\\shppict");
        /**
        * Constant for a picture
        */
        private static byte[] PICTURE = DocWriter.GetISOBytes("\\pict");
        /**
        * Constant for a jpeg image
        */
        private static byte[] PICTURE_JPEG = DocWriter.GetISOBytes("\\jpegblip");
        /**
        * Constant for a png image
        */
        private static byte[] PICTURE_PNG = DocWriter.GetISOBytes("\\pngblip");
        /**
        * Constant for a wmf image
        */
        private static byte[] PICTURE_WMF = DocWriter.GetISOBytes("\\wmetafile8");
        /**
        * Constant for the picture width
        */
        private static byte[] PICTURE_WIDTH = DocWriter.GetISOBytes("\\picw");
        /**
        * Constant for the picture height
        */
        private static byte[] PICTURE_HEIGHT = DocWriter.GetISOBytes("\\pich");
        /**
        * Constant for the picture width scale
        */
        private static byte[] PICTURE_SCALED_WIDTH = DocWriter.GetISOBytes("\\picwgoal");
        /**
        * Constant for the picture height scale
        */
        private static byte[] PICTURE_SCALED_HEIGHT = DocWriter.GetISOBytes("\\pichgoal");
        /**
        * Constant for horizontal picture scaling
        */
        private static byte[] PICTURE_SCALE_X = DocWriter.GetISOBytes("\\picscalex");
        /**
        * Constant for vertical picture scaling
        */
        private static byte[] PICTURE_SCALE_Y = DocWriter.GetISOBytes("\\picscaley");
        /**
        * "\bin" constant
        */
        private static byte[] PICTURE_BINARY_DATA = DocWriter.GetISOBytes("\\bin");
        /**
        * Constant for converting pixels to twips
        */
        private const int PIXEL_TWIPS_FACTOR = 15;
        
        /**
        * The type of image this is.
        */
        private int imageType;
        /**
        * Binary image data.
        */
        private byte[][] imageData;
        /**
        * The alignment of this picture
        */
        private int alignment = Element.ALIGN_LEFT;
        /**
        * The width of this picture
        */
        private float width = 0;
        /**
        * The height of this picutre
        */
        private float height = 0;
        /**
        * The intended display width of this picture
        */
        private float plainWidth = 0;
        /**
        * The intended display height of this picture
        */
        private float plainHeight = 0;
        /**
        * Whether this RtfImage is a top level element and should
        * be an extra paragraph.
        */
        private bool topLevelElement = false;
        
        /**
        * Constructs a RtfImage for an Image.
        * 
        * @param doc The RtfDocument this RtfImage belongs to
        * @param image The Image that this RtfImage wraps
        * @throws DocumentException If an error occured accessing the image content
        */
        public RtfImage(RtfDocument doc, Image image) : base(doc) {
            imageType = image.OriginalType;
            if (!(imageType == Image.ORIGINAL_JPEG || imageType == Image.ORIGINAL_BMP
                    || imageType == Image.ORIGINAL_PNG || imageType == Image.ORIGINAL_WMF || imageType == Image.ORIGINAL_GIF)) {
                throw new DocumentException("Only BMP, PNG, WMF, GIF and JPEG images are supported by the RTF Writer");
            }
            alignment = image.Alignment;
            width = image.Width;
            height = image.Height;
            plainWidth = image.PlainWidth;
            plainHeight = image.PlainHeight;
            this.imageData = GetImageData(image);
        }
        
        /**
        * Extracts the image data from the Image.
        * 
        * @param image The image for which to extract the content
        * @return The raw image data, not formated
        * @throws DocumentException If an error occurs accessing the image content
        */
        private byte[][] GetImageData(Image image) {
            int WMF_PLACEABLE_HEADER_SIZE = 22;
            RtfByteArrayBuffer bab = new RtfByteArrayBuffer();
            
            try {
                if (imageType == Image.ORIGINAL_BMP) {
                    bab.Append(MetaDo.WrapBMP(image));
                } else {                
                    byte[] iod = image.OriginalData;
                    if (iod == null) {
                        Stream imageIn = WebRequest.Create(image.Url).GetResponse().GetResponseStream();
                        if (imageType == Image.ORIGINAL_WMF) { //remove the placeable header first
                            for (int k = 0; k < WMF_PLACEABLE_HEADER_SIZE; k++) {
                                if (imageIn.ReadByte() < 0) throw (new IOException("while removing wmf placeable header"));
                            }
                        }
                        bab.Write(imageIn);
                        imageIn.Close();
                        
                    } else {
                        
                        if (imageType == Image.ORIGINAL_WMF) {
                            //remove the placeable header                       
                            bab.Write(iod, WMF_PLACEABLE_HEADER_SIZE, iod.Length - WMF_PLACEABLE_HEADER_SIZE);
                        } else {
                            bab.Append(iod);
                        }
                        
                    }
                }
                return bab.ToArrayArray();
            } catch (IOException ioe) {
                throw new DocumentException(ioe.Message);
            }
        }
        
        
        /**
        * lookup table used for converting bytes to hex chars.
        * TODO Should probably be refactored into a helper class
        */
        public static byte[] byte2charLUT = new byte[512]; //'0001020304050607 ... fafbfcfdfeff'
        static RtfImage() {
            char c = '0';
            for (int k = 0; k < 16; k++) {
                for (int x = 0; x < 16; x++) {
                    byte2charLUT[((k*16)+x)*2] = byte2charLUT[(((x*16)+k)*2)+1] = (byte)c;
                }
                if (++c == ':') c = 'a';
            }
        }
        /**
        * Writes the image data to the given buffer as hex encoded text.
        * 
        * @param binary
        * @param bab
        * @
        */
        private void WriteImageDataHexEncoded(Stream bab) {
            int cnt = 0;
            for (int k = 0; k < imageData.Length; k++) {
                byte[] chunk = imageData[k];
                for (int x = 0; x < chunk.Length; x++) {
                    bab.Write(byte2charLUT, (chunk[x]&0xff)*2, 2);
                    if (++cnt == 64) {
                        bab.WriteByte((byte)'\n');
                        cnt = 0;
                    }
                }
            }       
            if (cnt > 0) bab.WriteByte((byte)'\n');
        }
        /**
        * Returns the image raw data size in bytes.
        * 
        * @return
        */
        private int ImageDataSize() {
            int size = 0;
            for (int k = 0; k < imageData.Length; k++) {
                size += imageData[k].Length;
            }   
            return size;
        }
        
        /**
        * Writes the RtfImage content
        */ 
        public override void WriteContent(Stream result) 
        {
            byte[] t;
            if (this.topLevelElement) {
                result.Write(RtfParagraph.PARAGRAPH_DEFAULTS, 0, RtfParagraph.PARAGRAPH_DEFAULTS.Length);
                switch (alignment) {
                    case Element.ALIGN_LEFT:
                        result.Write(RtfParagraphStyle.ALIGN_LEFT, 0, RtfParagraphStyle.ALIGN_LEFT.Length);
                        break;
                    case Element.ALIGN_RIGHT:
                        result.Write(RtfParagraphStyle.ALIGN_RIGHT, 0, RtfParagraphStyle.ALIGN_RIGHT.Length);
                        break;
                    case Element.ALIGN_CENTER:
                        result.Write(RtfParagraphStyle.ALIGN_CENTER, 0, RtfParagraphStyle.ALIGN_CENTER.Length);
                        break;
                    case Element.ALIGN_JUSTIFIED:
                        result.Write(RtfParagraphStyle.ALIGN_JUSTIFY, 0, RtfParagraphStyle.ALIGN_JUSTIFY.Length);
                        break;
                }
            }
            result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
            result.Write(PICTURE_GROUP, 0, PICTURE_GROUP.Length);
            result.Write(OPEN_GROUP, 0, OPEN_GROUP.Length);
            result.Write(PICTURE, 0, PICTURE.Length);
            switch (imageType) {
                case Image.ORIGINAL_JPEG:
                    result.Write(PICTURE_JPEG, 0, PICTURE_JPEG.Length);
                    break;
                case Image.ORIGINAL_PNG:
                case Image.ORIGINAL_GIF:
                    result.Write(PICTURE_PNG, 0, PICTURE_PNG.Length);
                    break;
                case Image.ORIGINAL_WMF:
                case Image.ORIGINAL_BMP:
                    result.Write(PICTURE_WMF, 0, PICTURE_WMF.Length);
                    break;
            }
            result.Write(PICTURE_WIDTH, 0, PICTURE_WIDTH.Length);
            result.Write(t = IntToByteArray((int) width), 0, t.Length);
            result.Write(PICTURE_HEIGHT, 0, PICTURE_HEIGHT.Length);
            result.Write(t = IntToByteArray((int) height), 0, t.Length);
            if (this.document.GetDocumentSettings().IsWriteImageScalingInformation()) {
                result.Write(PICTURE_SCALE_X, 0, PICTURE_SCALE_X.Length);
                result.Write(t = IntToByteArray((int)(100 * plainWidth / width)), 0, t.Length);
                result.Write(PICTURE_SCALE_Y, 0, PICTURE_SCALE_Y.Length);
                result.Write(t = IntToByteArray((int)(100 * plainHeight / height)), 0, t.Length);
            }
            if (this.document.GetDocumentSettings().IsImagePDFConformance()) {
                result.Write(PICTURE_SCALED_WIDTH, 0, PICTURE_SCALED_WIDTH.Length);
                result.Write(t = IntToByteArray((int) (plainWidth * RtfElement.TWIPS_FACTOR)), 0, t.Length);
                result.Write(PICTURE_SCALED_HEIGHT, 0, PICTURE_SCALED_HEIGHT.Length);
                result.Write(t = IntToByteArray((int) (plainHeight * RtfElement.TWIPS_FACTOR)), 0, t.Length);
            } else {
                if (this.width != this.plainWidth || this.imageType == Image.ORIGINAL_BMP) {
                    result.Write(PICTURE_SCALED_WIDTH, 0, PICTURE_SCALED_WIDTH.Length);
                    result.Write(t = IntToByteArray((int) (plainWidth * PIXEL_TWIPS_FACTOR)), 0, t.Length);
                }
                if (this.height != this.plainHeight || this.imageType == Image.ORIGINAL_BMP) {
                    result.Write(PICTURE_SCALED_HEIGHT, 0, PICTURE_SCALED_HEIGHT.Length);
                    result.Write(t = IntToByteArray((int) (plainHeight * PIXEL_TWIPS_FACTOR)), 0, t.Length);
                }
            }

            if (this.document.GetDocumentSettings().IsImageWrittenAsBinary()) {
                //binary
                result.WriteByte((byte)'\n');
                result.Write(PICTURE_BINARY_DATA, 0, PICTURE_BINARY_DATA.Length);
                result.Write(t = IntToByteArray(ImageDataSize()), 0, t.Length);
                result.Write(DELIMITER, 0, DELIMITER.Length);
                if (result is RtfByteArrayBuffer) {
                    ((RtfByteArrayBuffer)result).Append(imageData);
                } else {
                    for (int k = 0; k < imageData.Length; k++) {
                        result.Write(imageData[k], 0, imageData[k].Length);
                    }
                }
            } else {
                //hex encoded
                result.Write(DELIMITER, 0, DELIMITER.Length);
                result.WriteByte((byte)'\n');
                WriteImageDataHexEncoded(result);
            }
            result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
            result.Write(CLOSE_GROUP, 0, CLOSE_GROUP.Length);
            if (this.topLevelElement) {
                result.Write(RtfParagraph.PARAGRAPH, 0, RtfParagraph.PARAGRAPH.Length);
            }
            result.WriteByte((byte)'\n');
        }
        
        /**
        * Sets the alignment of this RtfImage. Uses the alignments from com.lowagie.text.Element.
        * 
        * @param alignment The alignment to use.
        */
        public void SetAlignment(int alignment) {
            this.alignment = alignment;
        }

        /**
        * Set whether this RtfImage should behave like a top level element
        * and enclose itself in a paragraph.
        * 
        * @param topLevelElement Whether to behave like a top level element.
        */
        public void SetTopLevelElement(bool topLevelElement) {
            this.topLevelElement = topLevelElement;
        }
    }
}