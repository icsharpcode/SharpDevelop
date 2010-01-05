using System;
using System.IO;
using System.Net;
using System.util;
using iTextSharp.text.pdf;

/*
 * $Id: Jpeg.cs,v 1.11 2008/05/13 11:25:11 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 by Bruno Lowagie.
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

namespace iTextSharp.text {
    /// <summary>
    /// An Jpeg is the representation of a graphic element (JPEG)
    /// that has to be inserted into the document
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Image"/>
    /// <seealso cref="T:iTextSharp.text.Gif"/>
    /// <seealso cref="T:iTextSharp.text.Png"/>
    public class Jpeg : Image {
    
        // public static membervariables
    
        /// <summary> This is a type of marker. </summary>
        public const int NOT_A_MARKER = -1;
    
        /// <summary> This is a type of marker. </summary>
        public const int VALID_MARKER = 0;
    
        /// <summary> Acceptable Jpeg markers. </summary>
        public static int[] VALID_MARKERS = {0xC0, 0xC1, 0xC2};
    
        /// <summary> This is a type of marker. </summary>
        public const int UNSUPPORTED_MARKER = 1;
    
        /// <summary> Unsupported Jpeg markers. </summary>
        public static int[] UNSUPPORTED_MARKERS = {0xC3, 0xC5, 0xC6, 0xC7, 0xC8, 0xC9, 0xCA, 0xCB, 0xCD, 0xCE, 0xCF};
    
        /// <summary> This is a type of marker. </summary>
        public const int NOPARAM_MARKER = 2;
    
        /// <summary> Jpeg markers without additional parameters. </summary>
        public static int[] NOPARAM_MARKERS = {0xD0, 0xD1, 0xD2, 0xD3, 0xD4, 0xD5, 0xD6, 0xD7, 0xD8, 0x01};
    
        public const int M_APP0 = 0xE0;
        public const int M_APP2 = 0xE2;
        public const int M_APPE = 0xEE;
    
        public static byte[] JFIF_ID = {0x4A, 0x46, 0x49, 0x46, 0x00};

        private byte[][] icc;

        // Constructors
    
        /// <summary>
        /// Construct a Jpeg-object, using a Image
        /// </summary>
        /// <param name="image">a Image</param>
        public Jpeg(Image image) : base(image) {}

        /// <summary>
        /// Constructs a Jpeg-object, using an Uri.
        /// </summary>
        /// <remarks>
        /// Deprecated, use Image.GetInstance(...) to create an Image
        /// </remarks>
        /// <param name="Uri">the Uri where the image can be found</param>
        public Jpeg(Uri Uri) : base(Uri) {
            ProcessParameters();
        }
    
        /// <summary>
        /// Constructs a Jpeg-object from memory.
        /// </summary>
        /// <param name="img">the memory image</param>
        public Jpeg(byte[] img) : base((Uri)null) {
            rawData = img;
            originalData = img;
            ProcessParameters();
        }

        /// <summary>
        /// Constructs a Jpeg-object from memory.
        /// </summary>
        /// <param name="img">the memory image.</param>
        /// <param name="width">the width you want the image to have</param>
        /// <param name="height">the height you want the image to have</param>
        public Jpeg(byte[] img, float width, float height) : this(img) {
            scaledWidth = width;
            scaledHeight = height;
        }
    
        // private static methods
    
        /// <summary>
        /// Reads a short from the Stream.
        /// </summary>
        /// <param name="istr">the Stream</param>
        /// <returns>an int</returns>
        private static int GetShort(Stream istr) {
            return (istr.ReadByte() << 8) + istr.ReadByte();
        }
    
        /// <summary>
        /// Reads an inverted short from the Stream.
        /// </summary>
        /// <param name="istr">the Stream</param>
        /// <returns>an int</returns>
        private static int GetShortInverted(Stream istr) {
            return (istr.ReadByte() + istr.ReadByte() << 8);
        }

        /// <summary>
        /// Returns a type of marker.
        /// </summary>
        /// <param name="marker">an int</param>
        /// <returns>a type: VALID_MARKER, UNSUPPORTED_MARKER or NOPARAM_MARKER</returns>
        private static int MarkerType(int marker) {
            for (int i = 0; i < VALID_MARKERS.Length; i++) {
                if (marker == VALID_MARKERS[i]) {
                    return VALID_MARKER;
                }
            }
            for (int i = 0; i < NOPARAM_MARKERS.Length; i++) {
                if (marker == NOPARAM_MARKERS[i]) {
                    return NOPARAM_MARKER;
                }
            }
            for (int i = 0; i < UNSUPPORTED_MARKERS.Length; i++) {
                if (marker == UNSUPPORTED_MARKERS[i]) {
                    return UNSUPPORTED_MARKER;
                }
            }
            return NOT_A_MARKER;
        }
    
        // private methods
    
        /// <summary>
        /// This method checks if the image is a valid JPEG and processes some parameters.
        /// </summary>
        private void ProcessParameters() {
            type = Element.JPEG;
            originalType = ORIGINAL_JPEG;
            Stream istr = null;
            try {
                string errorID;
                if (rawData == null){
                    WebRequest w = WebRequest.Create(url);
                    istr = w.GetResponse().GetResponseStream();
                    errorID = url.ToString();
                }
                else{
                    istr = new MemoryStream(rawData);
                    errorID = "Byte array";
                }
                if (istr.ReadByte() != 0xFF || istr.ReadByte() != 0xD8)    {
                    throw new BadElementException(errorID + " is not a valid JPEG-file.");
                }
                bool firstPass = true;
                int len;
                while (true) {
                    int v = istr.ReadByte();
                    if (v < 0)
                        throw new IOException("Premature EOF while reading JPG.");
                    if (v == 0xFF) {
                        int marker = istr.ReadByte();
                        if (firstPass && marker == M_APP0) {
                            firstPass = false;
                            len = GetShort(istr);
                            if (len < 16) {
                                Utilities.Skip(istr, len - 2);
                                continue;
                            }
                            byte[] bcomp = new byte[JFIF_ID.Length];
                            int r = istr.Read(bcomp, 0, bcomp.Length);
                            if (r != bcomp.Length)
                                throw new BadElementException(errorID + " corrupted JFIF marker.");
                            bool found = true;
                            for (int k = 0; k < bcomp.Length; ++k) {
                                if (bcomp[k] != JFIF_ID[k]) {
                                    found = false;
                                    break;
                                }
                            }
                            if (!found) {
                                Utilities.Skip(istr, len - 2 - bcomp.Length);
                                continue;
                            }
                            Utilities.Skip(istr, 2);
                            int units = istr.ReadByte();
                            int dx = GetShort(istr);
                            int dy = GetShort(istr);
                            if (units == 1) {
                                dpiX = dx;
                                dpiY = dy;
                            }
                            else if (units == 2) {
                                dpiX = (int)((float)dx * 2.54f + 0.5f);
                                dpiY = (int)((float)dy * 2.54f + 0.5f);
                            }
                            Utilities.Skip(istr, len - 2 - bcomp.Length - 7);
                            continue;
                        }
                        if (marker == M_APPE) {
                            len = GetShort(istr) - 2;
                            byte[] byteappe = new byte[len];
                            for (int k = 0; k < len; ++k) {
                                byteappe[k] = (byte)istr.ReadByte();
                            }
                            if (byteappe.Length >= 12) {
                                string appe = System.Text.ASCIIEncoding.ASCII.GetString(byteappe,0,5);
                                if (Util.EqualsIgnoreCase(appe, "adobe")) {
                                    invert = true;
                                }
                            }
                            continue;
                        }
                        if (marker == M_APP2) {
                            len = GetShort(istr) - 2;
                            byte[] byteapp2 = new byte[len];
                            for (int k = 0; k < len; ++k) {
                                byteapp2[k] = (byte)istr.ReadByte();
                            }
                            if (byteapp2.Length >= 14) {
                                String app2 = System.Text.ASCIIEncoding.ASCII.GetString(byteapp2, 0, 11);
                                if (app2.Equals("ICC_PROFILE")) {
                                    int order = byteapp2[12] & 0xff;
                                    int count = byteapp2[13] & 0xff;
                                    if (icc == null)
                                        icc = new byte[count][];
                                    icc[order - 1] = byteapp2;
                                }
                            }
                            continue;
                        }
                        firstPass = false;
                        int markertype = MarkerType(marker);
                        if (markertype == VALID_MARKER) {
                            Utilities.Skip(istr, 2);
                            if (istr.ReadByte() != 0x08) {
                                throw new BadElementException(errorID + " must have 8 bits per component.");
                            }
                            scaledHeight = GetShort(istr);
                            Top = scaledHeight;
                            scaledWidth = GetShort(istr);
                            Right = scaledWidth;
                             colorspace = istr.ReadByte();
                            bpc = 8;
                            break;
                        }
                        else if (markertype == UNSUPPORTED_MARKER) {
                            throw new BadElementException(errorID + ": unsupported JPEG marker: " + marker);
                        }
                        else if (markertype != NOPARAM_MARKER) {
                            Utilities.Skip(istr, GetShort(istr) - 2);
                        }
                    }
                }
            }
            finally {
                if (istr != null) {
                    istr.Close();
                }
            }
            plainWidth = this.Width;
            plainHeight = this.Height;
            if (icc != null) {
                int total = 0;
                for (int k = 0; k < icc.Length; ++k) {
                    if (icc[k] == null) {
                        icc = null;
                        return;
                    }
                    total += icc[k].Length - 14;
                }
                byte[] ficc = new byte[total];
                total = 0;
                for (int k = 0; k < icc.Length; ++k) {
                    System.Array.Copy(icc[k], 14, ficc, total, icc[k].Length - 14);
                    total += icc[k].Length - 14;
                }
                ICC_Profile icc_prof = ICC_Profile.GetInstance(ficc);
                TagICC = icc_prof;
                icc = null;
            }
        }
    }
}
