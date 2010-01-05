using System;
using System.Collections;

/*
 * $Id: PageResources.cs,v 1.6 2006/09/24 15:48:24 psoares33 Exp $
 *
 * Copyright 2003-2005 by Paulo Soares.
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
    public class PageResources {
        
        protected PdfDictionary fontDictionary = new PdfDictionary();
        protected PdfDictionary xObjectDictionary = new PdfDictionary();
        protected PdfDictionary colorDictionary = new PdfDictionary();
        protected PdfDictionary patternDictionary = new PdfDictionary();
        protected PdfDictionary shadingDictionary = new PdfDictionary();
        protected PdfDictionary extGStateDictionary = new PdfDictionary();
        protected PdfDictionary propertyDictionary = new PdfDictionary();
        protected Hashtable forbiddenNames;
        protected PdfDictionary originalResources;
        protected int[] namePtr = {0};
        protected Hashtable usedNames;

        internal PageResources() {
        }
        
        internal void SetOriginalResources(PdfDictionary resources, int[] newNamePtr) {
            if (newNamePtr != null)
                namePtr = newNamePtr;
            forbiddenNames = new Hashtable();
            usedNames = new Hashtable();
            if (resources == null)
                return;
            originalResources = new PdfDictionary();
            originalResources.Merge(resources);
            foreach (PdfName key in resources.Keys) {
                PdfObject sub = PdfReader.GetPdfObject(resources.Get(key));
                if (sub != null && sub.IsDictionary()) {
                    PdfDictionary dic = (PdfDictionary)sub;
                    foreach (PdfName name in dic.Keys) {
                        forbiddenNames[name] = null;
                    }
                    PdfDictionary dic2 = new PdfDictionary();
                    dic2.Merge(dic);
                    originalResources.Put(key, dic2);
                }
            }
        }
        
        internal PdfName TranslateName(PdfName name) {
            PdfName translated = name;
            if (forbiddenNames != null) {
                translated = (PdfName)usedNames[name];
                if (translated == null) {
                    while (true) {
                        translated = new PdfName("Xi" + (namePtr[0]++));
                        if (!forbiddenNames.ContainsKey(translated))
                            break;
                    }
                    usedNames[name] = translated;
                }
            }
            return translated;
        }
        
        internal PdfName AddFont(PdfName name, PdfIndirectReference reference) {
            name = TranslateName(name);
            fontDictionary.Put(name, reference);
            return name;
        }

        internal PdfName AddXObject(PdfName name, PdfIndirectReference reference) {
            name = TranslateName(name);
            xObjectDictionary.Put(name, reference);
            return name;
        }

        internal PdfName AddColor(PdfName name, PdfIndirectReference reference) {
            name = TranslateName(name);
            colorDictionary.Put(name, reference);
            return name;
        }

        internal void AddDefaultColor(PdfName name, PdfObject obj) {
            if (obj == null || obj.IsNull())
                colorDictionary.Remove(name);
            else
                colorDictionary.Put(name, obj);
        }

        internal void AddDefaultColor(PdfDictionary dic) {
            colorDictionary.Merge(dic);
        }

        internal void AddDefaultColorDiff(PdfDictionary dic) {
            colorDictionary.MergeDifferent(dic);
        }

        internal PdfName AddShading(PdfName name, PdfIndirectReference reference) {
            name = TranslateName(name);
            shadingDictionary.Put(name, reference);
            return name;
        }
        
        internal PdfName AddPattern(PdfName name, PdfIndirectReference reference) {
            name = TranslateName(name);
            patternDictionary.Put(name, reference);
            return name;
        }

        internal PdfName AddExtGState(PdfName name, PdfIndirectReference reference) {
            name = TranslateName(name);
            extGStateDictionary.Put(name, reference);
            return name;
        }

        internal PdfName AddProperty(PdfName name, PdfIndirectReference reference) {
            name = TranslateName(name);
            propertyDictionary.Put(name, reference);
            return name;
        }

        internal PdfDictionary Resources {
            get {
                PdfResources resources = new PdfResources();
                if (originalResources != null)
                    resources.Merge(originalResources);
                resources.Put(PdfName.PROCSET, new PdfLiteral("[/PDF /Text /ImageB /ImageC /ImageI]"));
                resources.Add(PdfName.FONT, fontDictionary);
                resources.Add(PdfName.XOBJECT, xObjectDictionary);
                resources.Add(PdfName.COLORSPACE, colorDictionary);
                resources.Add(PdfName.PATTERN, patternDictionary);
                resources.Add(PdfName.SHADING, shadingDictionary);
                resources.Add(PdfName.EXTGSTATE, extGStateDictionary);
                resources.Add(PdfName.PROPERTIES, propertyDictionary);
                return resources;
            }
        }
    
        internal bool HasResources() {
            return (fontDictionary.Size > 0
                || xObjectDictionary.Size > 0
                || colorDictionary.Size > 0
                || patternDictionary.Size > 0
                || shadingDictionary.Size > 0
                || extGStateDictionary.Size > 0
                || propertyDictionary.Size > 0);
        }
    }
}
