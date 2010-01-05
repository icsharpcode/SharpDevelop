using System;
using System.Collections;
using System.Reflection;
using iTextSharp.text.rtf.parser;
using iTextSharp.text.rtf.parser.ctrlwords;
/*
 * $Id: RtfDestinationMgr.cs,v 1.4 2008/05/13 11:26:00 psoares33 Exp $
 * 
 *
 * Copyright 2007 by Howard Shank
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
namespace iTextSharp.text.rtf.parser.destinations {

    /**
    * <code>RtfDestinationMgr</code> manages destination objects for the parser
    * 
    * @author Howard Shank (hgshank@yahoo.com)
    * @since 2.0.8
    */
    public sealed class RtfDestinationMgr {
        /*
        * Destinations
        */
        private static RtfDestinationMgr instance = null;
        
        /**
        * CtrlWord <-> Destination map object.
        * 
        * Maps control words to their destinations objects.
        * Null destination is a special destination used for
        * discarding unwanted data. This is primarily used when
        * skipping groups, binary data or unwanted/unknown data.
        */
        private static Hashtable destinations = new Hashtable(300, 0.95f);
        /**
        * Destination objects.
        * There is only one of each destination.
        */
        private static Hashtable destinationObjects = new Hashtable(10, 0.95f);
        
        private static bool ignoreUnknownDestinations = false;
        
        private static RtfParser rtfParser = null;

        /**
        * String representation of null destination.
        */
        public const String DESTINATION_NULL = "null";
        /**
        * String representation of document destination.
        */
        public const String DESTINATION_DOCUMENT = "document";
        
        /**
        * Hidden default constructor becuase
        */
        private RtfDestinationMgr() {
        }
        
        public static void SetParser(RtfParser parser) {
            rtfParser = parser;
        }
        public static RtfDestinationMgr GetInstance() {
            lock(destinations) {
                if (instance == null) {
                    instance = new RtfDestinationMgr();
                    // 2 required destinations for all documents
                    RtfDestinationMgr.AddDestination(RtfDestinationMgr.DESTINATION_DOCUMENT, new Object[] { "RtfDestinationDocument", "" } );
                    RtfDestinationMgr.AddDestination(RtfDestinationMgr.DESTINATION_NULL, new Object[] { "RtfDestinationNull", "" } );
                }
                return instance;
            }
        }
        public static RtfDestinationMgr GetInstance(RtfParser parser) {
            lock(destinations) {
                RtfDestinationMgr.SetParser(parser);
                if (instance == null) {
                    instance = new RtfDestinationMgr();
                    // 2 required destinations for all documents
                    RtfDestinationMgr.AddDestination(RtfDestinationMgr.DESTINATION_DOCUMENT, new Object[] { "RtfDestinationDocument", "" } );
                    RtfDestinationMgr.AddDestination(RtfDestinationMgr.DESTINATION_NULL, new Object[] { "RtfDestinationNull", "" } );
                }
                return instance;
            }
        }
        
        public static RtfDestination GetDestination(String destination) {
            RtfDestination dest = null;
            if (destinations.ContainsKey(destination)) {
                dest = (RtfDestination)destinations[destination];
            } else {
                if (ignoreUnknownDestinations) {
                    dest = (RtfDestination)destinations[DESTINATION_NULL];
                } else {
                    dest = (RtfDestination)destinations[DESTINATION_DOCUMENT];
                }
            }
            dest.SetParser(RtfDestinationMgr.rtfParser);
            return dest;
        }
        
        public static bool AddDestination(String destination, Object[] args) {
            if (destinations.ContainsKey(destination)) {
                return true;
            }
            
            String thisClass =  "iTextSharp.text.rtf.parser.destinations." + (String)args[0];

            if (thisClass.IndexOf("RtfDestinationNull") >= 0) {
                destinations[destination] = RtfDestinationNull.GetInstance();
                return true;
            }
            
            Type value = null;
        
            try {
                value = Type.GetType(thisClass);
            } catch {
                return false;
            }
            if (value == null)
                return false;
            
            RtfDestination c = null;
            
            if (destinationObjects.ContainsKey(value.Name)) {
                c = (RtfDestination)destinationObjects[value.Name];        
            } else {
                try {
                    c = (RtfDestination)value.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new Type[0], null).Invoke(null);
                } catch  {
                    return false;
                }
            }
            
            c.SetParser(rtfParser);
            
            if (value.Equals(typeof(RtfDestinationInfo))) {
                    ((RtfDestinationInfo)c).SetElementName(destination);
            }
            
            if (value.Equals(typeof(RtfDestinationStylesheetTable))) {
                    ((RtfDestinationStylesheetTable)c).SetElementName(destination);
                    ((RtfDestinationStylesheetTable)c).SetType((String)args[1]);
            }

            destinations[destination] = c;
            destinationObjects[value.Name] = c;
            return true;
        }
        
        // listener methods

        /**
        * Adds a <CODE>RtfDestinationListener</CODE> to the appropriate <CODE>RtfDestination</CODE>.
        *
        * @param destination the destination string for the listener
        * @param listener
        *            the new RtfDestinationListener.
        */
        public static bool AddListener(String destination, IRtfDestinationListener listener) {
            RtfDestination dest = GetDestination(destination);
            if (dest != null) {
                return dest.AddListener(listener);
            }
            return false;
        }

        /**
        * Removes a <CODE>RtfDestinationListener</CODE> from the appropriate <CODE>RtfDestination</CODE>.
        *
        * @param destination the destination string for the listener
        * @param listener
        *            the RtfCtrlWordListener that has to be removed.
        */
        public static bool RemoveListener(String destination, IRtfDestinationListener listener) {
            RtfDestination dest = GetDestination(destination);
            if (dest != null) {
                return dest.RemoveListener(listener);
            }
            return false;
        }
    }
}