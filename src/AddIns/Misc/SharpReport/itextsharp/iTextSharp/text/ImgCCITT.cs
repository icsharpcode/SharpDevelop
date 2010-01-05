using System;
using iTextSharp.text.pdf.codec;

/*
 * $Id: ImgCCITT.cs,v 1.6 2008/05/13 11:25:11 psoares33 Exp $
 * 
 *
 * Copyright 2000, 2001, 2002 by Paulo Soares.
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
    /**
     * CCITT Image data that has to be inserted into the document
     *
     * @see        Element
     * @see        Image
     *
     * @author  Paulo Soares
     */
    /// <summary>
    /// CCITT Image data that has to be inserted into the document
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Image"/>
    public class ImgCCITT : Image {
        public ImgCCITT(Image image) : base(image) {}

        /// <summary>
        /// Creats an Image in CCITT mode.
        /// </summary>
        /// <param name="width">the exact width of the image</param>
        /// <param name="height">the exact height of the image</param>
        /// <param name="reverseBits">
        /// reverses the bits in data.
        /// Bit 0 is swapped with bit 7 and so on
        /// </param>
        /// <param name="typeCCITT">
        /// the type of compression in data. It can be
        /// CCITTG4, CCITTG31D, CCITTG32D
        /// </param>
        /// <param name="parameters">
        /// parameters associated with this stream. Possible values are
        /// CCITT_BLACKIS1, CCITT_ENCODEDBYTEALIGN, CCITT_ENDOFLINE and CCITT_ENDOFBLOCK or a
        /// combination of them
        /// </param>
        /// <param name="data">the image data</param>
        public ImgCCITT(int width, int height, bool reverseBits, int typeCCITT, int parameters, byte[] data) : base((Uri)null) {
            if (typeCCITT != Element.CCITTG4 && typeCCITT != Element.CCITTG3_1D && typeCCITT != Element.CCITTG3_2D)
                throw new BadElementException("The CCITT compression type must be CCITTG4, CCITTG3_1D or CCITTG3_2D");
            if (reverseBits)
                TIFFFaxDecoder.ReverseBits(data);
            type = Element.IMGRAW;
            scaledHeight = height;
            this.Top = scaledHeight;
            scaledWidth = width;
            this.Right = scaledWidth;
            colorspace = parameters;
            bpc = typeCCITT;
            rawData = data;
            plainWidth = this.Width;
            plainHeight = this.Height;
        }
    }
}
