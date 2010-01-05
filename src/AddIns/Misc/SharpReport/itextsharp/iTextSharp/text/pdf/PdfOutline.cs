using System;
using System.Text;
using System.IO;
using System.Collections;

using iTextSharp.text;

/*
 * $Id: PdfOutline.cs,v 1.5 2008/05/13 11:25:21 psoares33 Exp $
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
     * <CODE>PdfOutline</CODE> is an object that represents a PDF outline entry.
     * <P>
     * An outline allows a user to access views of a document by name.<BR>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 6.7 (page 104-106)
     *
     * @see     PdfDictionary
     */

    public class PdfOutline : PdfDictionary {
    
        // membervariables
    
        /** the <CODE>PdfIndirectReference</CODE> of this object */
        private PdfIndirectReference reference;
    
        /** value of the <B>Count</B>-key */
        private int count = 0;
    
        /** value of the <B>Parent</B>-key */
        private PdfOutline parent;
    
        /** value of the <B>Destination</B>-key */
        private PdfDestination destination;
    
        /** The <CODE>PdfAction</CODE> for this outline.
         */
        private PdfAction action;
       
        protected ArrayList kids = new ArrayList();
    
        protected PdfWriter writer;
    
        /** Holds value of property tag. */
        private string tag;
    
        /** Holds value of property open. */
        private bool open;
    
        /** Holds value of property color. */
        private Color color;
        
        /** Holds value of property style. */
        private int style = 0;

        // constructors
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for the <CODE>outlines object</CODE>.
         */
    
        internal PdfOutline(PdfWriter writer) : base(OUTLINES) {
            open = true;
            parent = null;
            this.writer = writer;
        }
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>. The open mode is
         * <CODE>true</CODE>.
         *
         * @param parent the parent of this outline item
         * @param action the <CODE>PdfAction</CODE> for this outline item
         * @param title the title of this outline item
         */
    
        public PdfOutline(PdfOutline parent, PdfAction action, string title) : this(parent, action, title, true) {}
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>.
         *
         * @param parent the parent of this outline item
         * @param action the <CODE>PdfAction</CODE> for this outline item
         * @param title the title of this outline item
         * @param open <CODE>true</CODE> if the children are visible
         */
        public PdfOutline(PdfOutline parent, PdfAction action, string title, bool open) : base() {
            this.action = action;
            InitOutline(parent, title, open);
        }
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>. The open mode is
         * <CODE>true</CODE>.
         *
         * @param parent the parent of this outline item
         * @param destination the destination for this outline item
         * @param title the title of this outline item
         */
    
        public PdfOutline(PdfOutline parent, PdfDestination destination, string title) : this(parent, destination, title, true) {}
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>.
         *
         * @param parent the parent of this outline item
         * @param destination the destination for this outline item
         * @param title the title of this outline item
         * @param open <CODE>true</CODE> if the children are visible
         */
        public PdfOutline(PdfOutline parent, PdfDestination destination, string title, bool open) : base() {
            this.destination = destination;
            InitOutline(parent, title, open);
        }
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>. The open mode is
         * <CODE>true</CODE>.
         *
         * @param parent the parent of this outline item
         * @param action the <CODE>PdfAction</CODE> for this outline item
         * @param title the title of this outline item
         */
        public PdfOutline(PdfOutline parent, PdfAction action, PdfString title) : this(parent, action, title, true) {}
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>.
         *
         * @param parent the parent of this outline item
         * @param action the <CODE>PdfAction</CODE> for this outline item
         * @param title the title of this outline item
         * @param open <CODE>true</CODE> if the children are visible
         */
        public PdfOutline(PdfOutline parent, PdfAction action, PdfString title, bool open) : this(parent, action, title.ToString(), open) {}
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>. The open mode is
         * <CODE>true</CODE>.
         *
         * @param parent the parent of this outline item
         * @param destination the destination for this outline item
         * @param title the title of this outline item
         */
    
        public PdfOutline(PdfOutline parent, PdfDestination destination, PdfString title) : this(parent, destination, title, true) {}
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>.
         *
         * @param parent the parent of this outline item
         * @param destination the destination for this outline item
         * @param title the title of this outline item
         * @param open <CODE>true</CODE> if the children are visible
         */
        public PdfOutline(PdfOutline parent, PdfDestination destination, PdfString title, bool open) : this(parent, destination, title.ToString(), true) {}
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>. The open mode is
         * <CODE>true</CODE>.
         *
         * @param parent the parent of this outline item
         * @param action the <CODE>PdfAction</CODE> for this outline item
         * @param title the title of this outline item
         */
    
        public PdfOutline(PdfOutline parent, PdfAction action, Paragraph title) : this(parent, action, title, true) {}
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>.
         *
         * @param parent the parent of this outline item
         * @param action the <CODE>PdfAction</CODE> for this outline item
         * @param title the title of this outline item
         * @param open <CODE>true</CODE> if the children are visible
         */
        public PdfOutline(PdfOutline parent, PdfAction action, Paragraph title, bool open) : base() {
            StringBuilder buf = new StringBuilder();
            foreach (Chunk chunk in title.Chunks) {
                buf.Append(chunk.Content);
            }
            this.action = action;
            InitOutline(parent, buf.ToString(), open);
        }
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>. The open mode is
         * <CODE>true</CODE>.
         *
         * @param parent the parent of this outline item
         * @param destination the destination for this outline item
         * @param title the title of this outline item
         */
    
        public PdfOutline(PdfOutline parent, PdfDestination destination, Paragraph title) : this(parent, destination, title, true) {}
    
        /**
         * Constructs a <CODE>PdfOutline</CODE>.
         * <P>
         * This is the constructor for an <CODE>outline entry</CODE>.
         *
         * @param parent the parent of this outline item
         * @param destination the destination for this outline item
         * @param title the title of this outline item
         * @param open <CODE>true</CODE> if the children are visible
         */
        public PdfOutline(PdfOutline parent, PdfDestination destination, Paragraph title, bool open) : base() {
            StringBuilder buf = new StringBuilder();
            foreach (Chunk chunk in title.Chunks) {
                buf.Append(chunk.Content);
            }
            this.destination = destination;
            InitOutline(parent, buf.ToString(), open);
        }
    
    
        // methods
    
        /** Helper for the constructors.
         * @param parent the parent outline
         * @param title the title for this outline
         * @param open <CODE>true</CODE> if the children are visible
         */
        internal void InitOutline(PdfOutline parent, string title, bool open) {
            this.open = open;
            this.parent = parent;
            writer = parent.writer;
            Put(PdfName.TITLE, new PdfString(title, PdfObject.TEXT_UNICODE));
            parent.AddKid(this);
            if (destination != null && !destination.HasPage()) // bugfix Finn Bock
                SetDestinationPage(writer.CurrentPage);
        }
    
        /**
         * Gets the indirect reference of this <CODE>PdfOutline</CODE>.
         *
         * @return      the <CODE>PdfIndirectReference</CODE> to this outline.
         */
    
        public PdfIndirectReference IndirectReference {
            get {
                return reference;
            }

            set {
                this.reference = value;
            }
        }
    
        /**
         * Gets the parent of this <CODE>PdfOutline</CODE>.
         *
         * @return      the <CODE>PdfOutline</CODE> that is the parent of this outline.
         */
    
        public PdfOutline Parent {
            get {
                return parent;
            }
        }
    
        /**
         * Set the page of the <CODE>PdfDestination</CODE>-object.
         *
         * @param pageReference indirect reference to the page
         * @return <CODE>true</CODE> if this page was set as the <CODE>PdfDestination</CODE>-page.
         */
    
        public bool SetDestinationPage(PdfIndirectReference pageReference) {
            if (destination == null) {
                return false;
            }
            return destination.AddPage(pageReference);
        }
    
        /**
         * Gets the destination for this outline.
         * @return the destination
         */
        public PdfDestination PdfDestination {
            get {
                return destination;
            }
        }
    
        internal int Count {
            get {
                return count;
            }

            set {
                this.count = value;
            }
        }

        /**
         * returns the level of this outline.
         *
         * @return      a level
         */
    
        public int Level {
            get {
                if (parent == null) {
                    return 0;
                }
                return (parent.Level + 1);
            }
        }
    
        /**
        * Returns the PDF representation of this <CODE>PdfOutline</CODE>.
        *
        * @param writer the encryption information
        * @param os
        * @throws IOException
        */
        
        public override void ToPdf(PdfWriter writer, Stream os) {
            if (color != null && !color.Equals(Color.BLACK)) {
                Put(PdfName.C, new PdfArray(new float[]{color.R/255f,color.G/255f,color.B/255f}));
            }
            int flag = 0;
            if ((style & Font.BOLD) != 0)
                flag |= 2;
            if ((style & Font.ITALIC) != 0)
                flag |= 1;
            if (flag != 0)
                Put(PdfName.F, new PdfNumber(flag));
            if (parent != null) {
                Put(PdfName.PARENT, parent.IndirectReference);
            }
            if (destination != null && destination.HasPage()) {
                Put(PdfName.DEST, destination);
            }
            if (action != null)
                Put(PdfName.A, action);
            if (count != 0) {
                Put(PdfName.COUNT, new PdfNumber(count));
            }
            base.ToPdf(writer, os);
        }
    
        public void AddKid(PdfOutline outline) {
            kids.Add(outline);
        }
    
        public ArrayList Kids {
            get {
                return kids;
            }

            set {
                this.kids = value;
            }
        }
    
        /** Getter for property tag.
         * @return Value of property tag.
         */
        public string Tag {
            get {
                return tag;
            }

            set {
                this.tag = value;
            }
        }
    
        public string Title {
            get {
                PdfString title = (PdfString)Get(PdfName.TITLE);
                return title.ToString();
            }

            set {
                Put(PdfName.TITLE, new PdfString(value, PdfObject.TEXT_UNICODE));
            }
        }
    
        /** Setter for property open.
         * @param open New value of property open.
         */
        public bool Open {
            set {
                this.open = value;
            }
            get {
                return open;
            }
        }

        public Color Color {
            get {
                return color;
            }
            set {
                color = value;
            }
        }

        public int Style {
            get {
                return style;
            }
            set {
                style = value;
            }
        }
    }
}