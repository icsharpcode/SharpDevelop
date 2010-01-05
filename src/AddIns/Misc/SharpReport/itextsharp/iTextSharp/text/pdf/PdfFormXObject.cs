using System;

/*
 * $Id: PdfFormXObject.cs,v 1.3 2008/05/13 11:25:21 psoares33 Exp $
 * 
 *
 * Copyright 1999, 2000, 2001, 2002 Bruno Lowagie
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

    /**
     * <CODE>PdfFormObject</CODE> is a type of XObject containing a template-object.
     */

    public class PdfFormXObject : PdfStream {
    
        // public static variables
    
        /** This is a PdfNumber representing 0. */
        public static PdfNumber ZERO = new PdfNumber(0);
    
        /** This is a PdfNumber representing 1. */
        public static PdfNumber ONE = new PdfNumber(1);
    
        /** This is the 1 - matrix. */
        public static PdfLiteral MATRIX = new PdfLiteral("[1 0 0 1 0 0]");
    
        // membervariables
    
    
        // constructor
    
        /**
         * Constructs a <CODE>PdfFormXObject</CODE>-object.
         *
         * @param        template        the template
         */
    
        internal PdfFormXObject(PdfTemplate template) : base() {
            Put(PdfName.TYPE, PdfName.XOBJECT);
            Put(PdfName.SUBTYPE, PdfName.FORM);
            Put(PdfName.RESOURCES, template.Resources);
            Put(PdfName.BBOX, new PdfRectangle(template.BoundingBox));
            Put(PdfName.FORMTYPE, ONE);
            PdfArray matrix = template.Matrix;
            if (template.Layer != null)
                Put(PdfName.OC, template.Layer.Ref);
            if (template.Group != null)
                Put(PdfName.GROUP, template.Group);
            if (matrix == null)
                Put(PdfName.MATRIX, MATRIX);
            else
                Put(PdfName.MATRIX, matrix);
            bytes = template.ToPdf(null);
            Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
            FlateCompress();
        }
    
    }
}