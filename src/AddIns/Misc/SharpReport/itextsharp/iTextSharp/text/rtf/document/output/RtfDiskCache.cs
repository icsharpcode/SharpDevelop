using System;
using System.IO;
/*
 * $Id: RtfDiskCache.cs,v 1.3 2008/05/16 19:30:53 psoares33 Exp $
 * 
 *
 * Copyright 2005 by Mark Hall
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

namespace iTextSharp.text.rtf.document.output {

    /**
    * The RtfFileCache is a RtfDataCache that uses a temporary file
    * to store the rtf document data. Not so fast, but doesn't use any
    * memory (just disk space).
    * 
    * @version $Revision$
    * @author Mark Hall (Mark.Hall@mail.room3b.eu)
    */
    public class RtfDiskCache : IRtfDataCache {

        /**
        * The BufferedOutputStream that stores the cache data.
        */
        private BufferedStream data = null;
        /**
        * The temporary file to store the data in.
        */
        private string tempFile = null;
        
        /**
        * Constructs a RtfFileCache. Creates the temp file.
        * 
        * @throws IOException If the temporary file could not be created.
        */
        public RtfDiskCache() {
            this.tempFile = Path.GetTempFileName();
            this.data = new BufferedStream(new FileStream(tempFile, FileMode.Create));
        }

        /**
        * Gets the BufferedOutputStream to write to.
        */
        public Stream GetOutputStream() {
            return this.data;
        }

        /**
        * Writes the content of the temporary file into the Stream.
        */
        public void WriteTo(Stream target) {
            this.data.Close();
            BufferedStream tempIn = new BufferedStream(new FileStream(this.tempFile, FileMode.Open));
            byte[] buffer = new byte[8192];
            int bytesRead = -1;
            while ((bytesRead = tempIn.Read(buffer, 0, buffer.Length)) > 0) {
                target.Write(buffer, 0, bytesRead);
            }
            tempIn.Close();
            File.Delete(this.tempFile);
        }

    }
}