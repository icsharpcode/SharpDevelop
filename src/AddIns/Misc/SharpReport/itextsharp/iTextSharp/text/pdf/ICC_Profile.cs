using System;
using System.Text;
using System.Collections;
using System.IO;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// Summary description for ICC_Profile.
    /// </summary>
    public class ICC_Profile
    {
        protected byte[] data;
        protected int numComponents;
        private static Hashtable cstags = new Hashtable();
        
        protected ICC_Profile() {
        }
        
        public static ICC_Profile GetInstance(byte[] data) {
            if (data.Length < 128 | data[36] != 0x61 || data[37] != 0x63 
                || data[38] != 0x73 || data[39] != 0x70)
                throw new ArgumentException("Invalid ICC profile");
            ICC_Profile icc = new ICC_Profile();
            icc.data = data;
            object cs = cstags[Encoding.ASCII.GetString(data, 16, 4)];
            icc.numComponents = (cs == null ? 0 : (int)cs);
            return icc;
        }
        
        public static ICC_Profile GetInstance(Stream file) {
            byte[] head = new byte[128];
            int remain = head.Length;
            int ptr = 0;
            while (remain > 0) {
                int n = file.Read(head, ptr, remain);
                if (n <= 0)
                    throw new ArgumentException("Invalid ICC profile");
                remain -= n;
                ptr += n;
            }
            if (head[36] != 0x61 || head[37] != 0x63 
                || head[38] != 0x73 || head[39] != 0x70)
                throw new ArgumentException("Invalid ICC profile");
            remain = ((head[0] & 0xff) << 24) | ((head[1] & 0xff) << 16)
                      | ((head[2] & 0xff) <<  8) | (head[3] & 0xff);
            byte[] icc = new byte[remain];
            System.Array.Copy(head, 0, icc, 0, head.Length);
            remain -= head.Length;
            ptr = head.Length;
            while (remain > 0) {
                int n = file.Read(icc, ptr, remain);
                if (n <= 0)
                    throw new ArgumentException("Invalid ICC profile");
                remain -= n;
                ptr += n;
            }
            return GetInstance(icc);
        }

        public static ICC_Profile GetInstance(String fname) {
            FileStream fs = new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read);
            ICC_Profile icc = GetInstance(fs);
            fs.Close();
            return icc;
        }

        public byte[] Data {
            get {
                return data;
            }
        }
        
        public int NumComponents {
            get {
                return numComponents;
            }
        }

        static ICC_Profile() {
            cstags["XYZ "] = 3;
            cstags["Lab "] = 3;
            cstags["Luv "] = 3;
            cstags["YCbr"] = 3;
            cstags["Yxy "] = 3;
            cstags["RGB "] = 3;
            cstags["GRAY"] = 1;
            cstags["HSV "] = 3;
            cstags["HLS "] = 3;
            cstags["CMYK"] = 4;
            cstags["CMY "] = 3;
            cstags["2CLR"] = 2;
            cstags["3CLR"] = 3;
            cstags["4CLR"] = 4;
            cstags["5CLR"] = 5;
            cstags["6CLR"] = 6;
            cstags["7CLR"] = 7;
            cstags["8CLR"] = 8;
            cstags["9CLR"] = 9;
            cstags["ACLR"] = 10;
            cstags["BCLR"] = 11;
            cstags["CCLR"] = 12;
            cstags["DCLR"] = 13;
            cstags["ECLR"] = 14;
            cstags["FCLR"] = 15;
        }
    }
}
