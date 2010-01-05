using System;

/*
 * $Id: IDocListener.cs,v 1.5 2008/05/13 11:25:10 psoares33 Exp $
 * 
 *
 * Copyright (c) 1999, 2000, 2001, 2002 Bruno Lowagie.
 *
 * The contents of this file are subject to the Mozilla License Version 1.1
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
 * LGPL license (the "GNU LIBRARY GENERAL LICENSE"), in which case the
 * provisions of LGPL are applicable instead of those above.  If you wish to
 * allow use of your version of this file only under the terms of the LGPL
 * License and not to allow others to use your version of this file under
 * the MPL, indicate your decision by deleting the provisions above and
 * replace them with the notice and other provisions required by the LGPL.
 * If you do not delete the provisions above, a recipient may use your version
 * of this file under either the MPL or the GNU LIBRARY GENERAL LICENSE.
 *
 * This library is free software; you can redistribute it and/or modify it
 * under the terms of the MPL as stated above or under the terms of the GNU
 * Library General License as published by the Free Software Foundation;
 * either version 2 of the License, or any later version.
 *
 * This library is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE. See the GNU Library general License for more
 * details.
 *
 * If you didn't download this code from the following link, you should check if
 * you aren't using an obsolete version:
 * http://www.lowagie.com/iText/
 */

namespace iTextSharp.text {
    /// <summary>
    /// A class that implements DocListener will perform some
    /// actions when some actions are performed on a Document.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.IElementListener"/>
    /// <seealso cref="T:iTextSharp.text.Document"/>
    /// <seealso cref="T:iTextSharp.text.DocWriter"/>
    public interface IDocListener : IElementListener {
    
        // methods
    
        /// <summary>
        /// Signals that the Document has been opened and that
        /// Elements can be added.
        /// </summary>
        void Open();
    
        /// <summary>
        /// Signals that the Document was closed and that no other
        /// Elements will be added.
        /// </summary>
        /// <remarks>
        /// The output stream of every writer implementing IDocListener will be closed.
        /// </remarks>
        void Close();

        /// <summary>
        /// Signals that an new page has to be started.
        /// </summary>
        /// <returns>true if the page was added, false if not.</returns>
        bool NewPage();
    
        /// <summary>
        /// Sets the pagesize.
        /// </summary>
        /// <param name="pageSize">the new pagesize</param>
        /// <returns>a boolean</returns>
        bool SetPageSize(Rectangle pageSize);
    
        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="marginLeft">the margin on the left</param>
        /// <param name="marginRight">the margin on the right</param>
        /// <param name="marginTop">the margin on the top</param>
        /// <param name="marginBottom">the margin on the bottom</param>
        /// <returns></returns>
        bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom);
    
        /**
        * Parameter that allows you to do margin mirroring (odd/even pages)
        * @param marginMirroring
        * @return true if succesfull
        */
        bool SetMarginMirroring(bool marginMirroring);

        /// <summary>
        /// Sets the page number.
        /// </summary>
        /// <value>the new page number</value>
        int PageCount {
            set;
        }
    
        /// <summary>
        /// Sets the page number to 0.
        /// </summary>
        void ResetPageCount();
    
        /// <summary>
        /// Changes the header of this document.
        /// </summary>
        /// <value>a Header</value>
        HeaderFooter Header {
            set;
        }
    
        /// <summary>
        /// Resets the header of this document.
        /// </summary>
        void ResetHeader();
    
        /// <summary>
        /// Changes the footer of this document.
        /// </summary>
        /// <value>a Footer</value>
        HeaderFooter Footer {
            set;
        }
    
        /// <summary>
        /// Resets the footer of this document.
        /// </summary>
        void ResetFooter();
    }
}
