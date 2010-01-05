using System;
using System.Collections;
using System.util;

using iTextSharp.text.html;
using iTextSharp.text.factories;

/*
 * $Id: Anchor.cs,v 1.9 2008/05/13 11:25:08 psoares33 Exp $
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
 * The Original Code is 'iTextSharp, a free JAVA-PDF library'.
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

namespace iTextSharp.text 
{
    /// <summary>
    /// An Anchor can be a reference or a destination of a reference.
    /// </summary>
    /// <remarks>
    /// An Anchor is a special kind of <see cref="T:iTextSharp.text.Phrase"/>.
    /// It is constructed in the same way.
    /// </remarks>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Phrase"/>
    public class Anchor : Phrase 
    {
    
        // membervariables
    
        /// <summary>
        /// This is the name of the Anchor.
        /// </summary>
        protected string name = null;
    
        /// <summary>
        /// This is the reference of the Anchor.
        /// </summary>
        protected string reference = null;
    
        // constructors
    
        /// <summary>
        /// Constructs an Anchor without specifying a leading.
        /// </summary>
        /// <overloads>
        /// Has nine overloads.
        /// </overloads>
        public Anchor() : base(16) {}
    
        /// <summary>
        /// Constructs an Anchor with a certain leading.
        /// </summary>
        /// <param name="leading">the leading</param>
        public Anchor(float leading) : base(leading) {}
    
        /// <summary>
        /// Constructs an Anchor with a certain Chunk.
        /// </summary>
        /// <param name="chunk">a Chunk</param>
        public Anchor(Chunk chunk) : base(chunk) {}
    
        /// <summary>
        /// Constructs an Anchor with a certain string.
        /// </summary>
        /// <param name="str">a string</param>
        public Anchor(string str) : base(str) {}
    
        /// <summary>
        /// Constructs an Anchor with a certain string
        /// and a certain Font.
        /// </summary>
        /// <param name="str">a string</param>
        /// <param name="font">a Font</param>
        public Anchor(string str, Font font) : base(str, font) {}
    
        /// <summary>
        /// Constructs an Anchor with a certain Chunk
        /// and a certain leading.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="chunk">a Chunk</param>
        public Anchor(float leading, Chunk chunk) : base(leading, chunk) {}
    
        /// <summary>
        /// Constructs an Anchor with a certain leading
        /// and a certain string.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="str">a string</param>
        public Anchor(float leading, string str) : base(leading, str) {}
    
        /// <summary>
        /// Constructs an Anchor with a certain leading,
        /// a certain string and a certain Font.
        /// </summary>
        /// <param name="leading">the leading</param>
        /// <param name="str">a string</param>
        /// <param name="font">a Font</param>
        public Anchor(float leading, string str, Font font) : base(leading, str, font) {}
    
        /**
        * Constructs an <CODE>Anchor</CODE> with a certain <CODE>Phrase</CODE>.
        *
        * @param   phrase      a <CODE>Phrase</CODE>
        */    
        public Anchor(Phrase phrase) : base(phrase) {
            if (phrase is Anchor) {
                Anchor a = (Anchor) phrase;
                Name = a.name;
                Reference = a.reference;
            }
        }
        // implementation of the Element-methods
    
        /// <summary>
        /// Processes the element by adding it (or the different parts) to an
        /// <see cref="T:iTextSharp.text.IElementListener"/>
        /// </summary>
        /// <param name="listener">an IElementListener</param>
        /// <returns>true if the element was processed successfully</returns>
        public override bool Process(IElementListener listener) 
        {
            try 
            {
                bool localDestination = (reference != null && reference.StartsWith("#"));
                bool notGotoOK = true;
                foreach (Chunk chunk in this.Chunks) 
                {
                    if (name != null && notGotoOK && !chunk.IsEmpty()) 
                    {
                        chunk.SetLocalDestination(name);
                        notGotoOK = false;
                    }
                    if (localDestination) 
                    {
                        chunk.SetLocalGoto(reference.Substring(1));
                    }
                    else if (reference != null)
                        chunk.SetAnchor(reference);
                    listener.Add(chunk);
                }
                return true;
            }
            catch (DocumentException) 
            {
                return false;
            }
        }
    
        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <value>an ArrayList</value>
        public override ArrayList Chunks 
        {
            get 
            {
                ArrayList tmp = new ArrayList();
                bool localDestination = (reference != null && reference.StartsWith("#"));
                bool notGotoOK = true;
                foreach (Chunk chunk in this) 
                {
                    if (name != null && notGotoOK && !chunk.IsEmpty()) 
                    {
                        chunk.SetLocalDestination(name);
                        notGotoOK = false;
                    }
                    if (localDestination) 
                    {
                        chunk.SetLocalGoto(reference.Substring(1));
                    }
                    else if (reference != null) 
                    {
                        chunk.SetAnchor(reference);
                    }

                    tmp.Add(chunk);
                }
                return tmp;
            }
        }
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public override int Type 
        {
            get 
            {
                return Element.ANCHOR;
            }
        }
    
        // methods
    
        /// <summary>
        /// Name of this Anchor.
        /// </summary>
        public string Name {
            get {
                return this.name;
            }

            set {
                this.name = value;
            }
        }
    
        // methods to retrieve information
    
        /// <summary>
        /// reference of this Anchor.
        /// </summary>
        public string Reference {
            get {
                return reference;
            }

            set {
                this.reference = value;
            }
        }
    
        /// <summary>
        /// reference of this Anchor.
        /// </summary>
        /// <value>an Uri</value>
        public Uri Url {
            get {
                try {
                    return new Uri(reference);
                }
                catch {
                    return null;
                }
            }
        }
    }
}
