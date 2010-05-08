#region License
/* **********************************************************************************
 * Copyright (c) Roman Ivantsov
 * This source code is subject to terms and conditions of the MIT License
 * for Irony. A copy of the license can be found in the License.txt file
 * at the root of this distribution. 
 * By using this source code in any fashion, you are agreeing to be bound by the terms of the 
 * MIT License.
 * You must not remove this notice from this software.
 * **********************************************************************************/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Irony.CompilerServices;

namespace Irony {
  //using Compiler.Lalr;

  public class EscapeTable : Dictionary<char, char> { }
  public static class TextUtils {
    public const string AllLatinLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
    public const string DecimalDigits = "1234567890";
    public const string OctalDigits = "12345670";
    public const string HexDigits = "1234567890aAbBcCdDeEfF";
    public const string BinaryDigits = "01";

    public static EscapeTable GetDefaultEscapes() {
      EscapeTable escapes = new EscapeTable();
      escapes.Add('a', '\u0007');
      escapes.Add('b', '\b');
      escapes.Add('t', '\t');
      escapes.Add('n', '\n');
      escapes.Add('v', '\v');
      escapes.Add('f', '\f');
      escapes.Add('r', '\r');
      escapes.Add('"', '"');
      escapes.Add('\'', '\'');
      escapes.Add('\\', '\\');
      escapes.Add(' ', ' ');
      escapes.Add('\n', '\n'); //this is a special escape of the linebreak itself, 
      // when string ends with "\" char and continues on the next line
      return escapes;
    }

    public static string JoinStrings( string separator, IEnumerable<string> values) {
      StringList list = new StringList();
      list.AddRange(values);
      string[] arr = new string[list.Count];
      list.CopyTo(arr, 0);
      return string.Join(separator, arr);
    }

  }//class


}
