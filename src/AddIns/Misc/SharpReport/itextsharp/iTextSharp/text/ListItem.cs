using System;
using System.Collections;
using System.util;
using iTextSharp.text.factories;
/*
 * $Id: ListItem.cs,v 1.11 2008/05/13 11:25:11 psoares33 Exp $
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
    /// A ListItem is a Paragraph
    /// that can be added to a List.
    /// </summary>
    /// <example>
    /// <B>Example 1:</B>
    /// <code>
    /// List list = new List(true, 20);
    /// list.Add(<strong>new ListItem("First line")</strong>);
    /// list.Add(<strong>new ListItem("The second line is longer to see what happens once the end of the line is reached. Will it start on a new line?")</strong>);
    /// list.Add(<strong>new ListItem("Third line")</strong>);
    /// </code>
    /// 
    /// The result of this code looks like this:
    /// <OL>
    ///        <LI>
    ///            First line
    ///        </LI>
    ///        <LI>
    ///            The second line is longer to see what happens once the end of the line is reached. Will it start on a new line?
    ///        </LI>
    ///        <LI>
    ///            Third line
    ///        </LI>
    ///    </OL>
    ///    
    /// <B>Example 2:</B>
    /// <code>
    /// List overview = new List(false, 10);
    /// overview.Add(<strong>new ListItem("This is an item")</strong>);
    /// overview.Add("This is another item");
    /// </code>
    /// 
    /// The result of this code looks like this:
    /// <UL>
    ///        <LI>
    ///            This is an item
    ///        </LI>
    ///        <LI>
    ///            This is another item
    ///        </LI>
    ///    </UL>
    /// </example>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.List"/>
    /// <seealso cref="T:iTextSharp.text.Paragraph"/>
    public class ListItem : Paragraph {
    
        // membervariables
    
        /// <summary> this is the symbol that wil proceed the listitem. </summary>
        private Chunk symbol;
    
        // constructors
    
        /// <summary>
        /// Constructs a ListItem.
        /// </summary>
        public ListItem() : base() {}
    
        /// <summary>
        ///    Constructs a ListItem with a certain leading.
        /// </summary>
        /// <param name="leading">the leading</param>
        public ListItem(float leading) : base(leading) {}
    
        /// <summary>
        /// Constructs a ListItem with a certain Chunk.
        /// </summary>
        /// <param name="chunk">a Chunk</param>
        public ListItem(Chunk chunk) : base(chunk) {}
    
        /// <summary>
        /// Constructs a ListItem with a certain string.
        /// </summary>
        /// <param name="str">a string</param>
        public ListItem(string str) : base(str) {}
    
        /// <summary>
        /// Constructs a ListItem with a certain string
        /// and a certain Font.
        /// </summary>
        /// <param name="str">a string</param>
        /// <param name="font">a string</param>
        public ListItem(string str, Font font) : base(str, font) {}
    
        /// <summary>
        /// Constructs a ListItem with a certain Chunk
        /// and a certain leading.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="chunk">a Chunk</param>
        public ListItem(float leading, Chunk chunk) : base(leading, chunk) {}
    
        /// <summary>
        /// Constructs a ListItem with a certain string
        /// and a certain leading.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="str">a string</param>
        public ListItem(float leading, string str) : base(leading, str) {}
    
        /**
         * Constructs a ListItem with a certain leading, string
         * and Font.
         *
         * @param    leading        the leading
         * @param    string        a string
         * @param    font        a Font
         */
        /// <summary>
        /// Constructs a ListItem with a certain leading, string
        /// and Font.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="str">a string</param>
        /// <param name="font">a Font</param>
        public ListItem(float leading, string str, Font font) : base(leading, str, font) {}
    
        /// <summary>
        /// Constructs a ListItem with a certain Phrase.
        /// </summary>
        /// <param name="phrase">a Phrase</param>
        public ListItem(Phrase phrase) : base(phrase) {}
    
        // implementation of the Element-methods
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public override int Type {
            get {
                return Element.LISTITEM;
            }
        }
    
        // methods
    
        // methods to retrieve information
    
        /// <summary>
        /// Get/set the listsymbol.
        /// </summary>
        /// <value>a Chunk</value>
        public Chunk ListSymbol {
            get {
                return symbol;
            }

            set {
                if (this.symbol == null) {
                    this.symbol = value;
                    if (this.symbol.Font.IsStandardFont()) {
                        this.symbol.Font = font;
                    }
                }
            }
        }
    
        /// <summary>
        /// Checks if a given tag corresponds with this object.
        /// </summary>
        /// <param name="tag">the given tag</param>
        /// <returns>true if the tag corresponds</returns>
        public new static bool IsTag(string tag) {
            return ElementTags.LISTITEM.Equals(tag);
        }

        /**
        * Sets the indentation of this paragraph on the left side.
        *
        * @param	indentation		the new indentation
        */        
        public void SetIndentationLeft(float indentation, bool autoindent) {
            if (autoindent) {
            	IndentationLeft = ListSymbol.GetWidthPoint();
            }
            else {
            	IndentationLeft = indentation;
            }
        }
    }
}
