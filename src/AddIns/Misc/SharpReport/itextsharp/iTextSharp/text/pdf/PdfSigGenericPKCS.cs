using System;
using System.IO;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Crypto;
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
    * A signature dictionary representation for the standard filters.
    */
    public abstract class PdfSigGenericPKCS : PdfSignature {
        /**
        * The hash algorith, for example "SHA1"
        */    
        protected String hashAlgorithm;
        /**
        * The class instance that calculates the PKCS#1 and PKCS#7
        */    
        protected PdfPKCS7 pkcs;
        /**
        * The subject name in the signing certificate (the element "CN")
        */    
        protected String   name;

        private byte[] externalDigest;
        private byte[] externalRSAdata;
        private String digestEncryptionAlgorithm;

        /**
        * Creates a generic standard filter.
        * @param filter the filter name
        * @param subFilter the sub-filter name
        */    
        public PdfSigGenericPKCS(PdfName filter, PdfName subFilter) : base(filter, subFilter) {
        }

        /**
        * Sets the crypto information to sign.
        * @param privKey the private key
        * @param certChain the certificate chain
        * @param crlList the certificate revocation list. It can be <CODE>null</CODE>
        */    
        public void SetSignInfo(ICipherParameters privKey, X509Certificate[] certChain, object[] crlList) {
            pkcs = new PdfPKCS7(privKey, certChain, crlList, hashAlgorithm, PdfName.ADBE_PKCS7_SHA1.Equals(Get(PdfName.SUBFILTER)));
            pkcs.SetExternalDigest(externalDigest, externalRSAdata, digestEncryptionAlgorithm);
            if (PdfName.ADBE_X509_RSA_SHA1.Equals(Get(PdfName.SUBFILTER))) {
                MemoryStream bout = new MemoryStream();
                for (int k = 0; k < certChain.Length; ++k) {
                    byte[] tmp = certChain[k].GetEncoded();
                    bout.Write(tmp, 0, tmp.Length);
                }
                bout.Close();
                Cert = bout.ToArray();
                Contents = pkcs.GetEncodedPKCS1();
            }
            else
                Contents = pkcs.GetEncodedPKCS7();
            name = PdfPKCS7.GetSubjectFields(pkcs.SigningCertificate).GetField("CN");
            if (name != null)
                Put(PdfName.NAME, new PdfString(name, PdfObject.TEXT_UNICODE));
            pkcs = new PdfPKCS7(privKey, certChain, crlList, hashAlgorithm, PdfName.ADBE_PKCS7_SHA1.Equals(Get(PdfName.SUBFILTER)));
            pkcs.SetExternalDigest(externalDigest, externalRSAdata, digestEncryptionAlgorithm);
        }

        /**
        * Sets the digest/signature to an external calculated value.
        * @param digest the digest. This is the actual signature
        * @param RSAdata the extra data that goes into the data tag in PKCS#7
        * @param digestEncryptionAlgorithm the encryption algorithm. It may must be <CODE>null</CODE> if the <CODE>digest</CODE>
        * is also <CODE>null</CODE>. If the <CODE>digest</CODE> is not <CODE>null</CODE>
        * then it may be "RSA" or "DSA"
        */    
        public void SetExternalDigest(byte[] digest, byte[] RSAdata, String digestEncryptionAlgorithm) {
            externalDigest = digest;
            externalRSAdata = RSAdata;
            this.digestEncryptionAlgorithm = digestEncryptionAlgorithm;
        }

        /**
        * Gets the subject name in the signing certificate (the element "CN")
        * @return the subject name in the signing certificate (the element "CN")
        */    
        public new String Name {
            get {
                return name;
            }
        }

        /**
        * Gets the class instance that does the actual signing.
        * @return the class instance that does the actual signing
        */    
        public PdfPKCS7 Signer {
            get {
                return pkcs;
            }
        }

        /**
        * Gets the signature content. This can be a PKCS#1 or a PKCS#7. It corresponds to
        * the /Contents key.
        * @return the signature content
        */    
        public byte[] SignerContents {
            get {
                if (PdfName.ADBE_X509_RSA_SHA1.Equals(Get(PdfName.SUBFILTER)))
                    return pkcs.GetEncodedPKCS1();
                else
                    return pkcs.GetEncodedPKCS7();
            }
        }

        /**
        * Creates a standard filter of the type VeriSign.
        */    
        public class VeriSign : PdfSigGenericPKCS {
            /**
            * The constructor for the default provider.
            */        
            public VeriSign() : base(PdfName.VERISIGN_PPKVS, PdfName.ADBE_PKCS7_DETACHED) {
                hashAlgorithm = "MD5";
                Put(PdfName.R, new PdfNumber(65537));
            }
        }

        /**
        * Creates a standard filter of the type self signed.
        */    
        public class PPKLite : PdfSigGenericPKCS {
            /**
            * The constructor for the default provider.
            */        
            public PPKLite() : base(PdfName.ADOBE_PPKLITE, PdfName.ADBE_X509_RSA_SHA1) {
                hashAlgorithm = "SHA1";
                Put(PdfName.R, new PdfNumber(65541));
            }
        }

        /**
        * Creates a standard filter of the type Windows Certificate.
        */    
        public class PPKMS : PdfSigGenericPKCS {
            /**
            * The constructor for the default provider.
            */        
            public PPKMS() : base(PdfName.ADOBE_PPKMS, PdfName.ADBE_PKCS7_SHA1) {
                hashAlgorithm = "SHA1";
            }
        }
    }
}