using System;
using System.Collections;
using System.util;
using iTextSharp.text.factories;

/*
 * $Id: List.cs,v 1.20 2008/05/13 11:25:11 psoares33 Exp $
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
    /// A List contains several ListItems.
    /// </summary>
    /// <example>
    /// <B>Example 1:</B>
    /// <code>
    /// <strong>List list = new List(true, 20);
    /// list.Add(new ListItem("First line"));
    /// list.Add(new ListItem("The second line is longer to see what happens once the end of the line is reached. Will it start on a new line?"));
    /// list.Add(new ListItem("Third line"));</strong>
    /// </code>
    /// 
    /// The result of this code looks like this:
    /// <OL>
    ///     <LI>
    ///         First line
    ///     </LI>
    ///     <LI>
    ///         The second line is longer to see what happens once the end of the line is reached. Will it start on a new line?
    ///     </LI>
    ///     <LI>
    ///         Third line
    ///     </LI>
    /// </OL>
    /// 
    /// <B>Example 2:</B>
    /// <code>
    /// <strong>List overview = new List(false, 10);
    /// overview.Add(new ListItem("This is an item"));
    /// overview.Add("This is another item");</strong>
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
    /// <seealso cref="T:iTextSharp.text.ListItem"/>
    public class List : ITextElementArray {
    
        // membervariables
        /** a possible value for the numbered parameter */
        public const bool ORDERED = true;
        /** a possible value for the numbered parameter */
        public const bool UNORDERED = false;
        /** a possible value for the lettered parameter */
        public const bool NUMERICAL = false;
        /** a possible value for the lettered parameter */
        public const bool ALPHABETICAL = true;
        /** a possible value for the lettered parameter */
        public const bool UPPERCASE = false;
        /** a possible value for the lettered parameter */
        public const bool LOWERCASE = true;
    
        /// <summary> This is the ArrayList containing the different ListItems. </summary>
        protected ArrayList list = new ArrayList();
    
        /** Indicates if the list has to be numbered. */
        protected bool numbered = false;
        /** Indicates if the listsymbols are numerical or alphabetical. */
        protected bool lettered = false;
        /** Indicates if the listsymbols are lowercase or uppercase. */
        protected bool lowercase = false;
        /** Indicates if the indentation has to be set automatically. */
        protected bool autoindent = false;
        /** Indicates if the indentation of all the items has to be aligned. */
        protected bool alignindent = false;
    
        /// <summary> This variable indicates the first number of a numbered list. </summary>
        protected int first = 1;
    
        /// <summary> This is the listsymbol of a list that is not numbered. </summary>
        protected Chunk symbol = new Chunk("-");
        /**
        * In case you are using numbered/lettered lists, this String is added before the number/letter.
        * @since   iText 2.1.1
        */
        protected String preSymbol = "";
        /**
        * In case you are using numbered/lettered lists, this String is added after the number/letter. 
        * @since   iText 2.1.1
        */
        protected String postSymbol = ". ";
    
        /// <summary> The indentation of this list on the left side. </summary>
        protected float indentationLeft = 0;
    
        /// <summary> The indentation of this list on the right side. </summary>
        protected float indentationRight = 0;
    
        /// <summary> The indentation of the listitems. </summary>
        protected float symbolIndent = 0;

        // constructors
    
    /**
    * Constructs a <CODE>List</CODE>.
    */
        public List() : this(false, false) {
        }
        
        /**
        * Constructs a <CODE>List</CODE> with a specific symbol indentation.
        * @param   symbolIndent    the symbol indentation
        * @since   iText 2.0.8
        */
        public List(float symbolIndent) {
            this.symbolIndent = symbolIndent;
        }

    /**
    * Constructs a <CODE>List</CODE>.
    *
    * @param    numbered        a bool
    */
        public List(bool numbered) : this(numbered, false) {
        }
            
    /**
    * Constructs a <CODE>List</CODE>.
    *
    * @param    numbered        a bool
    * @param lettered has the list to be 'numbered' with letters
    */
        public List(bool numbered, bool lettered) {
            this.numbered = numbered;
            this.lettered = lettered;
            this.autoindent = true;
            this.alignindent = true;
        }
        

        /// <summary>
        /// Constructs a List.
        /// </summary>
        /// <remarks>
        /// the parameter symbolIndent is important for instance when
        /// generating PDF-documents; it indicates the indentation of the listsymbol.
        /// </remarks>
        /// <param name="numbered">a bool</param>
        /// <param name="symbolIndent">the indentation that has to be used for the listsymbol</param>
        public List(bool numbered, float symbolIndent) : this(numbered, false, symbolIndent) {
        }
    
        /// <summary>
        /// Constructs a List.
        /// </summary>
        /// <param name="numbered">a bool</param>
        /// <param name="lettered">a bool</param>
        /// <param name="symbolIndent">the indentation that has to be used for the listsymbol</param>
        public List(bool numbered, bool lettered, float symbolIndent ) {
            this.numbered = numbered;
            this.lettered = lettered;
            this.symbolIndent = symbolIndent;
        }
    
        // implementation of the Element-methods
    
        /// <summary>
        /// Processes the element by adding it (or the different parts) to an
        /// IElementListener.
        /// </summary>
        /// <param name="listener">an IElementListener</param>
        /// <returns>true if the element was processed successfully</returns>
        public bool Process(IElementListener listener) {
            try {
                foreach (IElement ele in list) {
                    listener.Add(ele);
                }
                return true;
            }
            catch (DocumentException) {
                return false;
            }
        }
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public int Type {
            get {
                return Element.LIST;
            }
        }
    
        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <value>an ArrayList</value>
        public ArrayList Chunks {
            get {
                ArrayList tmp = new ArrayList();
                foreach (IElement ele in list) {
                    tmp.AddRange(ele.Chunks);
                }
                return tmp;
            }
        }
    
        // methods to set the membervariables
    
        /// <summary>
        /// Adds an Object to the List.
        /// </summary>
        /// <param name="o">the object to add</param>
        /// <returns>true is successful</returns>
        public virtual bool Add(Object o) {
            if (o is ListItem) {
                ListItem item = (ListItem) o;
                if (numbered || lettered) {
                    Chunk chunk = new Chunk(preSymbol, symbol.Font);
                    int index = first + list.Count;
                    if (lettered)
                        chunk.Append(RomanAlphabetFactory.GetString(index, lowercase));
                    else
                        chunk.Append(index.ToString());
                    chunk.Append(postSymbol);
                    item.ListSymbol = chunk;
                }
                else {
                    item.ListSymbol = symbol;
                }
                item.SetIndentationLeft(symbolIndent, autoindent);
                item.IndentationRight = 0;
                list.Add(item);
                return true;
            }
            else if (o is List) {
                List nested = (List) o;
                nested.IndentationLeft = nested.IndentationLeft + symbolIndent;
                first--;
                list.Add(nested);
                return true;
            }
            else if (o is string) {
                return this.Add(new ListItem((string) o));
            }
            return false;
        }
    
        // extra methods
        
        /** Makes sure all the items in the list have the same indentation. */
        public void NormalizeIndentation() {
            float max = 0;
            foreach (IElement o in list) {
                if (o is ListItem) {
                    max = Math.Max(max, ((ListItem)o).IndentationLeft);
                }
            }
            foreach (IElement o in list) {
                if (o is ListItem) {
                    ((ListItem)o).IndentationLeft = max;
                }
            }
        }

        //setters/getters

        public bool Numbered {
            set {
                numbered = value;
            }
            get {
                return numbered;
            }
        }

        public bool Lettered {
            set {
                lettered = value;
            }
            get {
                return lettered;
            }
        }

        public bool Lowercase {
            set {
                lowercase = value;
            }
            get {
                return lowercase;
            }
        }

        public bool Autoindent {
            set {
                autoindent = value;
            }
            get {
                return autoindent;
            }
        }

        public bool Alignindent {
            set {
                alignindent = value;
            }
            get {
                return alignindent;
            }
        }

        /// <summary>
        /// Get/set the first number
        /// </summary>
        /// <value>an int</value>
        public int First {
            get {
                return first;
            }

            set {
                this.first = value;
            }
        }
    
        /// <summary>
        /// Sets the symbol
        /// </summary>
        /// <value>a Chunk</value>
        public Chunk ListSymbol {
            set {
                this.symbol = value;
            }
        }

        /// <summary>
        /// Sets the listsymbol.
        /// </summary>
        /// <remarks>
        /// This is a shortcut for SetListSymbol(Chunk symbol).
        /// </remarks>
        /// <param name="symbol">a string</param>
        public void SetListSymbol(string symbol) {
            this.symbol = new Chunk(symbol);
        }
    
        /// <summary>
        /// Get/set the indentation of this paragraph on the left side.
        /// </summary>
        /// <value>the indentation</value>
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
        /// <value>the indentation</value>
        public float IndentationRight {
            get {
                return indentationRight;
            }

            set {
                this.indentationRight = value;
            }
        }
    
        /// <summary>
        /// Gets the symbol indentation.
        /// </summary>
        /// <value>the symbol indentation</value>
        public float SymbolIndent {
            set {
                symbolIndent = value;
            }
            get {
                return symbolIndent;
            }
        }
    
        /**
        * @see com.lowagie.text.Element#isContent()
        * @since   iText 2.0.8
        */
        public bool IsContent() {
            return true;
        }

        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public bool IsNestable() {
            return true;
        }

        // methods to retrieve information
    
        /// <summary>
        /// Gets all the items in the list.
        /// </summary>
        /// <value>an ArrayList containing ListItems</value>
        public ArrayList Items {
            get {
                return list;
            }
        }
    
        /// <summary>
        /// Gets the size of the list.
        /// </summary>
        /// <value>a size</value>
        public int Size {
            get {
                return list.Count;
            }
        }
    
        /**
        * Returns <CODE>true</CODE> if the list is empty.
        * 
        * @return <CODE>true</CODE> if the list is empty
        */
        public virtual bool IsEmpty() {
    	    return list.Count == 0;
        }

        /// <summary>
        /// Gets the leading of the first listitem.
        /// </summary>
        /// <value>a leading</value>
        public float TotalLeading {
            get {
                if (list.Count < 1) {
                    return -1;
                }
                ListItem item = (ListItem)list[0];
                return item.TotalLeading;
            }
        }
    
        /// <summary>
        /// Get/set the symbol indentation.
        /// </summary>
        /// <value>a Chunk</value>
        public Chunk Symbol {
            get {
                return symbol;
            }
            set {
                this.symbol = value;
            }
        }

	    /**
	    * Returns the String that is after a number or letter in the list symbol.
	    * @return	the String that is after a number or letter in the list symbol
	    * @since	iText 2.1.1
	    */
	    public String getPostSymbol() {
		    return postSymbol;
	    }

	    /**
	    * Sets the String that has to be added after a number or letter in the list symbol.
	    * @since	iText 2.1.1
	    * @param	postSymbol the String that has to be added after a number or letter in the list symbol.
	    */
	    public String PostSymbol {
            set {
		        postSymbol = value;
            }
            get {
                return postSymbol;
            }
	    }

	    /**
	    * Sets the String that has to be added before a number or letter in the list symbol.
	    * @since	iText 2.1.1
	    * @param	preSymbol the String that has to be added before a number or letter in the list symbol.
	    */
	    public String PreSymbol {
            set {
		        preSymbol = value;
            }
            get {
                return preSymbol;
            }
	    }
    }
}
