using System;
using System.Collections;
/*
 * Copyright 2004 by Paulo Soares.
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
    * An optional content group is a dictionary representing a collection of graphics
    * that can be made visible or invisible dynamically by users of viewer applications.
    * In iText they are referenced as layers.
    *
    * @author Paulo Soares (psoares@consiste.pt)
    */
    public class PdfLayer : PdfDictionary, IPdfOCG {
        protected PdfIndirectReference refi;
        protected ArrayList children;
        protected PdfLayer parent;
        protected String title;

        /**
        * Holds value of property on.
        */
        private bool on = true;
        
        /**
        * Holds value of property onPanel.
        */
        private bool onPanel = true;
        
        internal PdfLayer(String title) {
            this.title = title;
        }
        
        /**
        * Creates a title layer. A title layer is not really a layer but a collection of layers
        * under the same title heading.
        * @param title the title text
        * @param writer the <CODE>PdfWriter</CODE>
        * @return the title layer
        */    
        public static PdfLayer CreateTitle(String title, PdfWriter writer) {
            if (title == null)
                throw new ArgumentNullException("Title cannot be null.");
            PdfLayer layer = new PdfLayer(title);
            writer.RegisterLayer(layer);
            return layer;
        }
        /**
        * Creates a new layer.
        * @param name the name of the layer
        * @param writer the writer
        */    
        public PdfLayer(String name, PdfWriter writer) : base(PdfName.OCG) {
            Name = name;
            refi = writer.PdfIndirectReference;
            writer.RegisterLayer(this);
        }
        
        internal String Title {
            get {
                return title;
            }
        }
        
        /**
        * Adds a child layer. Nested layers can only have one parent.
        * @param child the child layer
        */    
        public void AddChild(PdfLayer child) {
            if (child.parent != null)
                throw new ArgumentException("The layer '" + ((PdfString)child.Get(PdfName.NAME)).ToUnicodeString() + "' already has a parent.");
            child.parent = this;
            if (children == null)
                children = new ArrayList();
            children.Add(child);
        }

        
        /**
        * Gets the parent layer.
        * @return the parent layer or <CODE>null</CODE> if the layer has no parent
        */    
        public PdfLayer Parent {
            get {
                return parent;
            }
        }
        
        /**
        * Gets the children layers.
        * @return the children layers or <CODE>null</CODE> if the layer has no children
        */    
        public ArrayList Children {
            get {
                return children;
            }
        }
        
        /**
        * Gets the <CODE>PdfIndirectReference</CODE> that represents this layer.
        * @return the <CODE>PdfIndirectReference</CODE> that represents this layer
        */    
        public PdfIndirectReference Ref {
            get {
                return refi;
            }
            set {
                refi = value;
            }
        }
        
        /**
        * Sets the name of this layer.
        * @param name the name of this layer
        */    
        public string Name {
            set {
                Put(PdfName.NAME, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }
        
        /**
        * Gets the dictionary representing the layer. It just returns <CODE>this</CODE>.
        * @return the dictionary representing the layer
        */    
        public PdfObject PdfObject {
            get {
                return this;
            }
        }
        
        /**
        * Gets the initial visibility of the layer.
        * @return the initial visibility of the layer
        */
        public bool On {
            get {
                return this.on;
            }
            set {
                on = value;
            }
        }
        
        
        private PdfDictionary Usage {
            get {
                PdfDictionary usage = (PdfDictionary)Get(PdfName.USAGE);
                if (usage == null) {
                    usage = new PdfDictionary();
                    Put(PdfName.USAGE, usage);
                }
                return usage;
            }
        }
        
        /**
        * Used by the creating application to store application-specific
        * data associated with this optional content group.
        * @param creator a text string specifying the application that created the group
        * @param subtype a string defining the type of content controlled by the group. Suggested
        * values include but are not limited to <B>Artwork</B>, for graphic-design or publishing
        * applications, and <B>Technical</B>, for technical designs such as building plans or
        * schematics
        */    
        public void SetCreatorInfo(String creator, String subtype) {
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.CREATOR, new PdfString(creator, PdfObject.TEXT_UNICODE));
            dic.Put(PdfName.SUBTYPE, new PdfName(subtype));
            usage.Put(PdfName.CREATORINFO, dic);
        }
        
        /**
        * Specifies the language of the content controlled by this
        * optional content group
        * @param lang a language string which specifies a language and possibly a locale
        * (for example, <B>es-MX</B> represents Mexican Spanish)
        * @param preferred used by viewer applications when there is a partial match but no exact
        * match between the system language and the language strings in all usage dictionaries
        */    
        public void SetLanguage(String lang, bool preferred) {
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.LANG, new PdfString(lang, PdfObject.TEXT_UNICODE));
            if (preferred)
                dic.Put(PdfName.PREFERRED, PdfName.ON);
            usage.Put(PdfName.LANGUAGE, dic);
        }
        
        /**
        * Specifies the recommended state for content in this
        * group when the document (or part of it) is saved by a viewer application to a format
        * that does not support optional content (for example, an earlier version of
        * PDF or a raster image format).
        * @param export the export state
        */    
        public bool Export {
            set {
                PdfDictionary usage = Usage;
                PdfDictionary dic = new PdfDictionary();
                dic.Put(PdfName.EXPORTSTATE, value ? PdfName.ON : PdfName.OFF);
                usage.Put(PdfName.EXPORT, dic);
            }
        }
        
        /**
        * Specifies a range of magnifications at which the content
        * in this optional content group is best viewed.
        * @param min the minimum recommended magnification factors at which the group
        * should be ON. A negative value will set the default to 0
        * @param max the maximum recommended magnification factor at which the group
        * should be ON. A negative value will set the largest possible magnification supported by the
        * viewer application
        */    
        public void SetZoom(float min, float max) {
            if (min <= 0 && max < 0)
                return;
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            if (min > 0)
                dic.Put(PdfName.MIN, new PdfNumber(min));
            if (max >= 0)
                dic.Put(PdfName.MAX, new PdfNumber(max));
            usage.Put(PdfName.ZOOM, dic);
        }

        /**
        * Specifies that the content in this group is intended for
        * use in printing
        * @param subtype a name specifying the kind of content controlled by the group;
        * for example, <B>Trapping</B>, <B>PrintersMarks</B> and <B>Watermark</B>
        * @param printstate indicates that the group should be
        * set to that state when the document is printed from a viewer application
        */    
        public void SetPrint(String subtype, bool printstate) {
            PdfDictionary usage = Usage;
            PdfDictionary dic = new PdfDictionary();
            dic.Put(PdfName.SUBTYPE, new PdfName(subtype));
            dic.Put(PdfName.PRINTSTATE, printstate ? PdfName.ON : PdfName.OFF);
            usage.Put(PdfName.PRINT, dic);
        }

        /**
        * Indicates that the group should be set to that state when the
        * document is opened in a viewer application.
        * @param view the view state
        */    
        public bool View {
            set {
                PdfDictionary usage = Usage;
                PdfDictionary dic = new PdfDictionary();
                dic.Put(PdfName.VIEWSTATE, value ? PdfName.ON : PdfName.OFF);
                usage.Put(PdfName.VIEW, dic);
            }
        }
        
        /**
        * Gets the layer visibility in Acrobat's layer panel
        * @return the layer visibility in Acrobat's layer panel
        */
        /**
        * Sets the visibility of the layer in Acrobat's layer panel. If <CODE>false</CODE>
        * the layer cannot be directly manipulated by the user. Note that any children layers will
        * also be absent from the panel.
        * @param onPanel the visibility of the layer in Acrobat's layer panel
        */
        public bool OnPanel {
            get {
                return this.onPanel;
            }
            set {
                onPanel = value;
            }
        }
    }
}
