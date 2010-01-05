using System;
using System.Collections;
using System.util;
using iTextSharp.text.factories;

/*
 * $Id: Paragraph.cs,v 1.13 2008/05/13 11:25:12 psoares33 Exp $
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
    /// A Paragraph is a series of Chunks and/or Phrases.
    /// </summary>
    /// <remarks>
    /// A Paragraph has the same qualities of a Phrase, but also
    /// some additional layout-parameters:
    /// <UL>
    /// <LI/>the indentation
    /// <LI/>the alignment of the text
    /// </UL>
    /// </remarks>
    /// <example>
    /// <code>
    /// <strong>Paragraph p = new Paragraph("This is a paragraph",
    ///                FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLDITALIC, new Color(0, 0, 255)));</strong>
    ///    </code>
    /// </example>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Phrase"/>
    /// <seealso cref="T:iTextSharp.text.ListItem"/>
    public class Paragraph : Phrase {
    
        // membervariables
    
        ///<summary> The alignment of the text. </summary>
        protected int alignment = Element.ALIGN_UNDEFINED;
    
        /** The text leading that is multiplied by the biggest font size in the line. */
        protected float multipliedLeading = 0;
        
        ///<summary> The indentation of this paragraph on the left side. </summary>
        protected float indentationLeft;
    
        ///<summary> The indentation of this paragraph on the right side. </summary>
        protected float indentationRight;
    
        /**
        * Holds value of property firstLineIndent.
        */
        private float firstLineIndent = 0;

    /** The spacing before the paragraph. */
        protected float spacingBefore;
        
    /** The spacing after the paragraph. */
        protected float spacingAfter;
        
        
        /**
        * Holds value of property extraParagraphSpace.
        */
        private float extraParagraphSpace = 0;
        
        ///<summary> Does the paragraph has to be kept together on 1 page. </summary>
        protected bool keeptogether = false;
    
        // constructors
    
        /// <summary>
        /// Constructs a Paragraph.
        /// </summary>
        public Paragraph() : base() {}
    
        /// <summary>
        /// Constructs a Paragraph with a certain leading.
        /// </summary>
        /// <param name="leading">the leading</param>
        public Paragraph(float leading) : base(leading) {}
    
        /// <summary>
        /// Constructs a Paragraph with a certain Chunk.
        /// </summary>
        /// <param name="chunk">a Chunk</param>
        public Paragraph(Chunk chunk) : base(chunk) {}
    
        /// <summary>
        /// Constructs a Paragraph with a certain Chunk
        /// and a certain leading.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="chunk">a Chunk</param>
        public Paragraph(float leading, Chunk chunk) : base(leading, chunk) {}
    
        /// <summary>
        /// Constructs a Paragraph with a certain string.
        /// </summary>
        /// <param name="str">a string</param>
        public Paragraph(string str) : base(str) {}
    
        /// <summary>
        /// Constructs a Paragraph with a certain string
        /// and a certain Font.
        /// </summary>
        /// <param name="str">a string</param>
        /// <param name="font">a Font</param>
        public Paragraph(string str, Font font) : base(str, font) {}
    
        /// <summary>
        /// Constructs a Paragraph with a certain string
        /// and a certain leading.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="str">a string</param>
        public Paragraph(float leading, string str) : base(leading, str) {}
    
        /// <summary>
        /// Constructs a Paragraph with a certain leading, string
        /// and Font.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="str">a string</param>
        /// <param name="font">a Font</param>
        public Paragraph(float leading, string str, Font font) : base(leading, str, font) {}
    
        /// <summary>
        /// Constructs a Paragraph with a certain Phrase.
        /// </summary>
        /// <param name="phrase">a Phrase</param>
        public Paragraph(Phrase phrase) : base(phrase) {
            if (phrase is Paragraph) {
                Paragraph p = (Paragraph)phrase;
                Alignment = p.Alignment;
                ExtraParagraphSpace = p.ExtraParagraphSpace;
                FirstLineIndent = p.FirstLineIndent;
                IndentationLeft = p.IndentationLeft;
                IndentationRight = p.IndentationRight;
                SpacingAfter = p.SpacingAfter;
                SpacingBefore = p.SpacingBefore;
            }
        }
    
        // implementation of the Element-methods
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public override int Type {
            get {
                return Element.PARAGRAPH;
            }
        }
    
        // methods
    
        /// <summary>
        /// Adds an Object to the Paragraph.
        /// </summary>
        /// <param name="o">the object to add</param>
        /// <returns>a bool</returns>
        public override bool Add(Object o) {
            if (o is List) {
                List list = (List) o;
                list.IndentationLeft = list.IndentationLeft + indentationLeft;
                list.IndentationRight = indentationRight;
                base.Add(list);
                return true;
            }
            else if (o is Image) {
                base.AddSpecial((Image) o);
                return true;
            }
            else if (o is Paragraph) {
                base.Add(o);
                base.Add(Chunk.NEWLINE);
                return true;
            }
            base.Add(o);
            return true;
        }
    
        // setting the membervariables
    
        /// <summary>
        /// Sets the alignment of this paragraph.
        /// </summary>
        /// <param name="alignment">the new alignment as a string</param>
        public void SetAlignment(string alignment) {
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_CENTER)) {
                this.alignment = Element.ALIGN_CENTER;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_RIGHT)) {
                this.alignment = Element.ALIGN_RIGHT;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED)) {
                this.alignment = Element.ALIGN_JUSTIFIED;
                return;
            }
            if (Util.EqualsIgnoreCase(alignment, ElementTags.ALIGN_JUSTIFIED_ALL)) {
                this.alignment = Element.ALIGN_JUSTIFIED_ALL;
                return;
            }
            this.alignment = Element.ALIGN_LEFT;
        }
    
        public override float Leading {
            set {
                this.leading = value;
                this.multipliedLeading = 0;
            }
        }

        /**
        * Sets the leading fixed and variable. The resultant leading will be
        * fixedLeading+multipliedLeading*maxFontSize where maxFontSize is the
        * size of the bigest font in the line.
        * @param fixedLeading the fixed leading
        * @param multipliedLeading the variable leading
        */
        public void SetLeading(float fixedLeading, float multipliedLeading) {
            this.leading = fixedLeading;
            this.multipliedLeading = multipliedLeading;
        }

    /**
     * Sets the variable leading. The resultant leading will be
     * multipliedLeading*maxFontSize where maxFontSize is the
     * size of the bigest font in the line.
     * @param multipliedLeading the variable leading
     */
        public float MultipliedLeading {
            get {
                return this.multipliedLeading;
            }
            set {
                this.leading = 0;
                this.multipliedLeading = value;
            }
        }

    
        /// <summary>
        /// Get/set the alignment of this paragraph.
        /// </summary>
        /// <value>a integer</value>
        public int Alignment{
            get {
                return alignment;
            }
            set {
                this.alignment = value;
            }
        }
    
        /// <summary>
        /// Get/set the indentation of this paragraph on the left side.
        /// </summary>
        /// <value>a float</value>
        public float IndentationLeft {
            get {
                return indentationLeft;
            }

            set {
                this.indentationLeft = value;
            }
        }
    
        /// <summary>
        /// Get/set the indentation of this paragraph on the right side.
        /// </summary>
        /// <value>a float</value>
        public float IndentationRight {
            get {
                return indentationRight;
            }
            
            set {
                this.indentationRight = value;
            }
        }
    
        /// <summary>
        /// Checks if a given tag corresponds with this object.
        /// </summary>
        /// <param name="tag">the given tag</param>
        /// <returns>true if the tag corresponds</returns>
        public new static bool IsTag(string tag) {
            return ElementTags.PARAGRAPH.Equals(tag);
        }

        public float SpacingBefore {
            get {
                return spacingBefore;
            }
            set {
                spacingBefore = value;
            }
        }

        public float SpacingAfter {
            get {
                return spacingAfter;
            }
            set {
                spacingAfter = value;
            }
        }

        /// <summary>
        /// Set/get if this paragraph has to be kept together on one page.
        /// </summary>
        /// <value>a bool</value>
        public bool KeepTogether {
            get {
                return keeptogether;
            }
            set {
                this.keeptogether = value;
            }
        }    

        /**
        * Gets the total leading.
        * This method is based on the assumption that the
        * font of the Paragraph is the font of all the elements
        * that make part of the paragraph. This isn't necessarily
        * true.
        * @return the total leading (fixed and multiplied)
        */
        public float TotalLeading {
            get {
                float m = font == null ?
                        Font.DEFAULTSIZE * multipliedLeading : font.GetCalculatedLeading(multipliedLeading);
                if (m > 0 && !HasLeading()) {
                    return m;
                }
                return Leading + m;
            }
        }

        public float FirstLineIndent {
            get {
                return this.firstLineIndent;
            }
            set {
                this.firstLineIndent = value;
            }
        }

        public float ExtraParagraphSpace {
            get {
                return this.extraParagraphSpace;
            }
            set {
                this.extraParagraphSpace = value;
            }
        }
    }
}
