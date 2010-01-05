using System;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.rtf;
using iTextSharp.text.rtf.document;
/*
 * $Id: RtfColor.cs,v 1.6 2008/05/16 19:31:10 psoares33 Exp $
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

namespace iTextSharp.text.rtf.style {

    /**
    * The RtfColor stores one rtf color value for a rtf document
    * 
    * @version $Version:$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfColor : RtfElement, IRtfExtendedElement {

        /**
        * Constant for RED value
        */
        private static byte[] COLOR_RED = DocWriter.GetISOBytes("\\red");
        /**
        * Constant for GREEN value
        */
        private static byte[] COLOR_GREEN = DocWriter.GetISOBytes("\\green");
        /**
        * Constant for BLUE value
        */
        private static byte[] COLOR_BLUE = DocWriter.GetISOBytes("\\blue");
        /**
        * Constant for the end of one color entry
        */
        private const byte COLON = (byte) ';';
        /**
        * Constant for the number of the colour in the list of colours
        */
        private static byte[] COLOR_NUMBER = DocWriter.GetISOBytes("\\cf");

        /**
        * The number of the colour in the list of colours
        */
        private int colorNumber = 0;
        /**
        * The red value
        */
        private int red = 0;
        /**
        * The green value
        */
        private int green = 0;
        /**
        * The blue value
        */
        private int blue = 0;
        
        /**
        * Constructor only for use when initializing the RtfColorList
        * 
        * @param doc The RtfDocument this RtfColor belongs to
        * @param red The red value to use
        * @param green The green value to use
        * @param blue The blue value to use
        * @param colorNumber The number of the colour in the colour list
        */
        protected internal RtfColor(RtfDocument doc, int red, int green, int blue, int colorNumber) : base(doc) {
            this.red = red;
            this.blue = blue;
            this.green = green;
            this.colorNumber = colorNumber;
        }
        
        /**
        * Constructs a RtfColor as a clone of an existing RtfColor
        * 
        * @param doc The RtfDocument this RtfColor belongs to
        * @param col The RtfColor to use as a base
        */
        public RtfColor(RtfDocument doc, RtfColor col) : base(doc) {
            if (col != null) {
                this.red = col.GetRed();
                this.green = col.GetGreen();
                this.blue = col.GetBlue();
            }
            if (this.document != null) {
                this.colorNumber = this.document.GetDocumentHeader().GetColorNumber(this);
            }
        }
        
        /**
        * Constructs a RtfColor based on the Color
        * 
        * @param doc The RtfDocument this RtfColor belongs to
        * @param col The Color to base this RtfColor on
        */
        public RtfColor(RtfDocument doc, Color col) : base(doc) {
            if (col != null) {
                this.red = col.R;
                this.blue = col.B;
                this.green = col.G;
            }
            if (this.document != null) {
                this.colorNumber = this.document.GetDocumentHeader().GetColorNumber(this);
            }
        }
        
        /**
        * Constructs a RtfColor based on the red/green/blue values
        * 
        * @param doc The RtfDocument this RtfColor belongs to
        * @param red The red value to use
        * @param green The green value to use
        * @param blue The blue value to use
        */
        public RtfColor(RtfDocument doc, int red, int green, int blue) : base(doc) {
            this.red = red;
            this.blue = blue;
            this.green = green;
            if (this.document != null) {
                this.colorNumber = this.document.GetDocumentHeader().GetColorNumber(this);
            }
        }

        /**
        * unused
        */
        public override void WriteContent(Stream outp) {       
        }
        
        /**
        * Write the definition part of this RtfColor.
        */
        public virtual void WriteDefinition(Stream result) {
            byte[] t;
            result.Write(COLOR_RED, 0, COLOR_RED.Length);
            result.Write(t = IntToByteArray(red), 0, t.Length);
            result.Write(COLOR_GREEN, 0, COLOR_GREEN.Length);
            result.Write(t = IntToByteArray(green), 0, t.Length);
            result.Write(COLOR_BLUE, 0, COLOR_BLUE.Length);
            result.Write(t = IntToByteArray(blue), 0, t.Length);
            result.WriteByte(COLON);
        }

        /**
        * Writes the beginning of this RtfColor
        * 
        */
        public void WriteBegin(Stream result) {
            byte[] t;
            try {
                result.Write(COLOR_NUMBER, 0, COLOR_NUMBER.Length);
                result.Write(t = IntToByteArray(colorNumber), 0, t.Length);
            } catch (IOException) {
            }
        }
        
        /**
        * Unused
        * 
        */
        public void WriteEnd(Stream result) {
        }
        
        /**
        * Tests if this RtfColor is equal to another RtfColor.
        * 
        * @param obj another RtfColor
        * @return <code>True</code> if red, green and blue values of the two colours match,
        *   <code>false</code> otherwise.
        */
        public override bool Equals(Object obj) {
            if (!(obj is RtfColor)) {
                return false;
            }
            RtfColor color = (RtfColor) obj;
            return (this.red == color.GetRed() && this.green == color.GetGreen() && this.blue == color.GetBlue());
        }

        /**
        * Returns the hash code of this RtfColor. The hash code is
        * an integer with the lowest three bytes containing the values
        * of red, green and blue.
        * 
        * @return The hash code of this RtfColor
        */
        public override int GetHashCode() {
            return (this.red << 16) | (this.green << 8) | this.blue;
        }
        
        /**
        * Get the blue value of this RtfColor
        * 
        * @return The blue value
        */
        public int GetBlue() {
            return blue;
        }

        /**
        * Get the green value of this RtfColor
        * 
        * @return The green value
        */
        public int GetGreen() {
            return green;
        }

        /**
        * Get the red value of this RtfColor
        * 
        * @return The red value
        */
        public int GetRed() {
            return red;
        }
        
        /**
        * Gets the number of this RtfColor in the list of colours
        * 
        * @return Returns the colorNumber.
        */
        public int GetColorNumber() {
            return colorNumber;
        }

        /**
        * Sets the RtfDocument this RtfColor belongs to
        * 
        * @param doc The RtfDocument to use
        */
        public override void SetRtfDocument(RtfDocument doc) {
            base.SetRtfDocument(doc);
            if (document != null) {
                this.colorNumber = document.GetDocumentHeader().GetColorNumber(this);
            }
        }
    }
}