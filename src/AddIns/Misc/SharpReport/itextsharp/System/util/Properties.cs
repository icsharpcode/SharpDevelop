using System;
using System.Text;
using System.IO;
using System.Collections;

namespace System.util
{
    /// <summary>
    /// Summary description for Properties.
    /// </summary>
    public class Properties
    {
        private Hashtable _col;
        private const string whiteSpaceChars = " \t\r\n\f";
        private const string keyValueSeparators = "=: \t\r\n\f";
        private const string strictKeyValueSeparators = "=:";

        public Properties()
        {
            _col = new Hashtable();
        }

        public string Remove(string key) {
            string retval = (string)_col[key];
            _col.Remove(key);
            return retval;
        }

        public IEnumerator GetEnumerator() {
            return _col.GetEnumerator();
        }

        public bool ContainsKey(string key) {
            return _col.ContainsKey(key);            
        }

        public virtual void Add(string key, string value) {
            _col[key] = value;        
        }

        public void AddAll(Properties col) {
            foreach (string itm in col.Keys) {
                _col[itm] = col[itm];
            }
        }

        public int Count {
            get {
                return _col.Count;
            }
        }

        public virtual string this[string key] {
            get {
                return (string)_col[key];
            }

            set {
                _col[key] = value;
            }
        }

        public ICollection Keys {
            get {
                return _col.Keys;
            }
        }

        public void Clear() {
            _col.Clear();
        }

        public void Load(Stream inStream) {
            StreamReader inp = new StreamReader(inStream, Encoding.GetEncoding(1252));
            while (true) {
                // Get next line
                String line = inp.ReadLine();
                if (line == null)
                    return;

                if (line.Length > 0) {
                
                    // Find start of key
                    int len = line.Length;
                    int keyStart;
                    for (keyStart=0; keyStart<len; keyStart++)
                        if (whiteSpaceChars.IndexOf(line[keyStart]) == -1)
                            break;

                    // Blank lines are ignored
                    if (keyStart == len)
                        continue;

                    // Continue lines that end in slashes if they are not comments
                    char firstChar = line[keyStart];
                    if ((firstChar != '#') && (firstChar != '!')) {
                        while (ContinueLine(line)) {
                            String nextLine = inp.ReadLine();
                            if (nextLine == null)
                                nextLine = "";
                            String loppedLine = line.Substring(0, len-1);
                            // Advance beyond whitespace on new line
                            int startIndex;
                            for (startIndex=0; startIndex<nextLine.Length; startIndex++)
                                if (whiteSpaceChars.IndexOf(nextLine[startIndex]) == -1)
                                    break;
                            nextLine = nextLine.Substring(startIndex,nextLine.Length - startIndex);
                            line = loppedLine+nextLine;
                            len = line.Length;
                        }

                        // Find separation between key and value
                        int separatorIndex;
                        for (separatorIndex=keyStart; separatorIndex<len; separatorIndex++) {
                            char currentChar = line[separatorIndex];
                            if (currentChar == '\\')
                                separatorIndex++;
                            else if (keyValueSeparators.IndexOf(currentChar) != -1)
                                break;
                        }

                        // Skip over whitespace after key if any
                        int valueIndex;
                        for (valueIndex=separatorIndex; valueIndex<len; valueIndex++)
                            if (whiteSpaceChars.IndexOf(line[valueIndex]) == -1)
                                break;

                        // Skip over one non whitespace key value separators if any
                        if (valueIndex < len)
                            if (strictKeyValueSeparators.IndexOf(line[valueIndex]) != -1)
                                valueIndex++;

                        // Skip over white space after other separators if any
                        while (valueIndex < len) {
                            if (whiteSpaceChars.IndexOf(line[valueIndex]) == -1)
                                break;
                            valueIndex++;
                        }
                        String key = line.Substring(keyStart, separatorIndex - keyStart);
                        String value = (separatorIndex < len) ? line.Substring(valueIndex, len - valueIndex) : "";

                        // Convert then store key and value
                        key = LoadConvert(key);
                        value = LoadConvert(value);
                        Add(key, value);
                    }
                }
            }
        }

        /*
        * Converts encoded &#92;uxxxx to unicode chars
        * and changes special saved chars to their original forms
        */
        private String LoadConvert(String theString) {
            char aChar;
            int len = theString.Length;
            StringBuilder outBuffer = new StringBuilder(len);

            for (int x=0; x<len; ) {
                aChar = theString[x++];
                if (aChar == '\\') {
                    aChar = theString[x++];
                    if (aChar == 'u') {
                        // Read the xxxx
                        int value=0;
                        for (int i=0; i<4; i++) {
                            aChar = theString[x++];
                            switch (aChar) {
                                case '0': case '1': case '2': case '3': case '4':
                                case '5': case '6': case '7': case '8': case '9':
                                    value = (value << 4) + aChar - '0';
                                    break;
                                case 'a': case 'b': case 'c':
                                case 'd': case 'e': case 'f':
                                    value = (value << 4) + 10 + aChar - 'a';
                                    break;
                                case 'A': case 'B': case 'C':
                                case 'D': case 'E': case 'F':
                                    value = (value << 4) + 10 + aChar - 'A';
                                    break;
                                default:
                                    throw new ArgumentException(
                                        "Malformed \\uxxxx encoding.");
                            }
                        }
                        outBuffer.Append((char)value);
                    } else {
                        if (aChar == 't') aChar = '\t';
                        else if (aChar == 'r') aChar = '\r';
                        else if (aChar == 'n') aChar = '\n';
                        else if (aChar == 'f') aChar = '\f';
                        outBuffer.Append(aChar);
                    }
                } else
                    outBuffer.Append(aChar);
            }
            return outBuffer.ToString();
        }

        private bool ContinueLine(String line) {
            int slashCount = 0;
            int index = line.Length - 1;
            while ((index >= 0) && (line[index--] == '\\'))
                slashCount++;
            return (slashCount % 2 == 1);
        }
    }
}
