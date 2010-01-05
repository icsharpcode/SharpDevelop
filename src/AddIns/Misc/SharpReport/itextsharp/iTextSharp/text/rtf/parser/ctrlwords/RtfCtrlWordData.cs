using System;
using iTextSharp.text.rtf.parser.properties;
/* $Id: RtfCtrlWordData.cs,v 1.2 2008/05/13 11:25:58 psoares33 Exp $
 * 
 *
 * Copyright 2007 by Howard Shank (hgshank@yahoo.com)
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
 * the Initial Developer are Copyright (C) 1999-2006 by Bruno Lowagie.
 * All Rights Reserved.
 * Co-Developer of the code is Paulo Soares. Portions created by the Co-Developer
 * are Copyright (C) 2000-2006 by Paulo Soares. All Rights Reserved.
 *
 * Contributor(s): all the names of the contributors are added in the source code
 * where applicable.
 *
 * Alternatively, the contents of this file may be used under the terms of the
 * LGPL license (the ?GNU LIBRARY GENERAL PUBLIC LICENSE?), in which case the
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

namespace iTextSharp.text.rtf.parser.ctrlwords {

    /**
    * The control word and parameter information as parsed by the parser.
    * Contains the control word,
    * Flag indicating if there is a parameter. 
    * The parameter value as a string.
    * Flag indicating the parameter is positive or negative.
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    * @since 2.0.8
    */
    public class RtfCtrlWordData {
        public String prefix = "";
        public String suffix = "";
        /**
        * The control word found by the parser
        */
        public String ctrlWord = "";
        /**
        * Flag indicating if this keyword has a parameter.
        */
        public bool hasParam = false;
        /**
        * The parameter for the control word.
        */
        public String param = "";
        /**
        * Flag indicating if parameter is positive or negative.
        */
        public bool isNeg = false;
        /**
        * Flag indicating a new group
        */
        public bool newGroup = false;
        /**
        * Flag indicating if this object has been modified.
        */
        public bool modified = false;
        
        public int ctrlWordType = RtfCtrlWordType.UNIDENTIFIED;
        public String specialHandler = "";
        
        /**
        * Return the parameter value as an integer (int) value.
        * 
        * @return
        *      Returns the parameter value as an int vlaue.
        */
        public int IntValue() {
            int value;
            value = int.Parse(this.param);
            if (this.isNeg) value = (-value);
            return value;
        }

        /**
        *  Return the parameter value as a long value
        *  
        * @return
        *      Returns the parameter value as a long value
        */
        public long LongValue() {
            long value;
            value = long.Parse(this.param);
            if (this.isNeg) value = (-value);
            return value;
        }
        
        public override String ToString() {
            String outp = "";
            outp = this.prefix + this.ctrlWord;
            if (this.hasParam) {
                if (this.isNeg) outp += "-";
                outp += this.param; 
            }
            outp += this.suffix;
            return outp;
        }
    }
}