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
//Authors: Roman Ivantsov - initial implementation and some later edits
//         Philipp Serr - implementation of advanced features for c#, python, VB

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Diagnostics;

namespace Irony.CompilerServices {
  using BigInteger = Microsoft.Scripting.Math.BigInteger;
  using Complex64 = Microsoft.Scripting.Math.Complex64;

  [Flags]
  public enum NumberFlags {
    None = 0,
    AllowStartEndDot  = 0x01,     //python : http://docs.python.org/ref/floating.html
    IntOnly           = 0x02,
    AvoidPartialFloat = 0x04,     //for use with IntOnly flag; essentially tells terminal to avoid matching integer if 
                                  // it is followed by dot (or exp symbol) - leave to another terminal that will handle float numbers
    AllowSign   = 0x08,
    DisableQuickParse = 0x10,

    Binary = 0x0100, //e.g. GNU GCC C Extension supports binary number literals
    Octal =  0x0200,
    Hex =    0x0400,
    HasDot = 0x1000,
    HasExp = 0x2000,
  }


  //TODO: For VB, we may need to add a flag to automatically use long instead of int (default) when number is too large
  public class NumberLiteral : CompoundTerminalBase {
    #region Public Consts
    //currently using TypeCodes for identifying numeric types
    public const TypeCode TypeCodeBigInt = (TypeCode)30;
    public const TypeCode TypeCodeImaginary = (TypeCode)31;
    #endregion

    #region constructors and initialization
    public NumberLiteral(string name) : this(name, null, NumberFlags.None) {
    }
    public NumberLiteral(string name, NumberFlags flags)  : this(name, null, flags) {
    }
    public NumberLiteral(string name, string displayName)  : this(name, displayName, NumberFlags.None) {
    }
    public NumberLiteral(string name, string displayName, NumberFlags flags) : base(name) {
      base.DisplayName = displayName;
      Flags |= flags;
      base.Category = TokenCategory.Literal;
    }
    public void AddPrefix(string prefix, NumberFlags flags) {
      PrefixFlags.Add(prefix, (short) flags);
      Prefixes.Add(prefix);
   }
    #endregion

    #region Public fields/properties: ExponentSymbols, Suffixes
    public NumberFlags Flags;
    public string QuickParseTerminators;
    public string ExponentSymbols = "eE"; //most of the time; in some languages (Scheme) we have more
    public char DecimalSeparator = '.';

    //Default types are assigned to literals without suffixes; first matching type used
    public TypeCode[] DefaultIntTypes = new TypeCode[] { TypeCode.Int32 };
    public TypeCode DefaultFloatType = TypeCode.Double;
    private TypeCode[] _defaultFloatTypes;

    public bool IsSet(NumberFlags flag) {
      return (Flags & flag) != 0;
    }
    #endregion

    #region Private fields: _quickParseTerminators
    #endregion

    #region overrides
    public override void Init(GrammarData grammarData) {
      base.Init(grammarData);
      if (string.IsNullOrEmpty(QuickParseTerminators))
        QuickParseTerminators = OwnerGrammar.WhitespaceChars + OwnerGrammar.Delimiters;
      _defaultFloatTypes = new TypeCode[] { DefaultFloatType };
      if (this.EditorInfo == null) 
        this.EditorInfo = new TokenEditorInfo(TokenType.Literal, TokenColor.Number, TokenTriggers.None);
    }

    public override IList<string> GetFirsts() {
      StringList result = new StringList();
      result.AddRange(base.Prefixes);
      //we assume that prefix is always optional, so number can always start with plain digit
      result.AddRange(new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
      // Python float numbers can start with a dot
      if (IsSet(NumberFlags.AllowStartEndDot))
        result.Add(DecimalSeparator.ToString());
      if (IsSet(NumberFlags.AllowSign))
        result.AddRange(new string[] {"-", "+"} );
      return result;
    }

    //Most numbers in source programs are just one-digit instances of 0, 1, 2, and maybe others until 9
    // so we try to do a quick parse for these, without starting the whole general process
    protected override Token QuickParse(CompilerContext context, ISourceStream source) {
      if (IsSet(NumberFlags.DisableQuickParse)) return null;
      char current = source.CurrentChar;
      if (char.IsDigit(current) && QuickParseTerminators.IndexOf(source.NextChar) >= 0) {
        int iValue = current - '0';
        object value = null;
        switch (DefaultIntTypes[0]) {
          case TypeCode.Int32: value = iValue; break;
          case TypeCode.UInt32: value = (UInt32)iValue; break;
          case TypeCode.Byte: value = (byte)iValue; break;
          case TypeCode.SByte: value = (sbyte) iValue; break;
          case TypeCode.Int16: value = (Int16)iValue; break;
          case TypeCode.UInt16: value = (UInt16)iValue; break;
          default: return null; 
        }
        Token token = new Token(this, source.TokenStart, current.ToString(), value);
        source.Position++;
        return token;
      } else
        return null;
    }
    protected override void InitDetails(CompilerContext context, CompoundTokenDetails details) {
      base.InitDetails(context, details);
      details.Flags = (short) this.Flags;
    }

    protected override void ReadPrefix(ISourceStream source, CompoundTokenDetails details) {
      //check that is not a  0 followed by dot; 
      //this may happen in Python for number "0.123" - we can mistakenly take "0" as octal prefix
      if (source.CurrentChar == '0' && source.NextChar == '.') return;
      base.ReadPrefix(source, details);
    }//method

    protected override void ReadSuffix(ISourceStream source, CompoundTokenDetails details) {
      base.ReadSuffix(source, details);
      if (string.IsNullOrEmpty(details.Suffix))
        details.TypeCodes = details.IsSet((short) (NumberFlags.HasDot | NumberFlags.HasExp)) ? _defaultFloatTypes : DefaultIntTypes;
    }

    protected override bool ReadBody(ISourceStream source, CompoundTokenDetails details) {
      //remember start - it may be different from source.TokenStart, we may have skipped prefix
      int start = source.Position;
      char current = source.CurrentChar;
      if (current == '-' || current == '+') {
        details.Sign = current.ToString();
        source.Position++;
      }
      //Figure out digits set
      string digits = GetDigits(details);
      bool isDecimal = !details.IsSet((short) (NumberFlags.Binary | NumberFlags.Octal | NumberFlags.Hex));
      bool allowFloat = !IsSet(NumberFlags.IntOnly);
      bool foundDigits = false;

      while (!source.EOF()) {
        current = source.CurrentChar;
        //1. If it is a digit, just continue going
        if (digits.IndexOf(current) >= 0) {
          source.Position++;
          foundDigits = true; 
          continue;
        }
        //2. Check if it is a dot in float number
        bool isDot = current == DecimalSeparator;
        if (allowFloat && isDot) {
          //If we had seen already a dot or exponent, don't accept this one;
          bool hasDotOrExp = details.IsSet((short) (NumberFlags.HasDot | NumberFlags.HasExp));
          if (hasDotOrExp) break; //from while loop
          //In python number literals (NumberAllowPointFloat) a point can be the first and last character,
          //We accept dot only if it is followed by a digit
          if (digits.IndexOf(source.NextChar) < 0 && !IsSet(NumberFlags.AllowStartEndDot))
            break; //from while loop
          details.Flags |= (int) NumberFlags.HasDot;
          source.Position++;
          continue;
        }
        //3. Check if it is int number followed by dot or exp symbol
        bool isExpSymbol = (details.ExponentSymbol == null) && ExponentSymbols.IndexOf(current) >= 0;
        if (!allowFloat && foundDigits && (isDot || isExpSymbol)) {
          //If no partial float allowed then return false - it is not integer, let float terminal recognize it as float
          if (IsSet(NumberFlags.AvoidPartialFloat)) return false;  
          //otherwise break, it is integer and we're done reading digits
          break;
        }


        //4. Only for decimals - check if it is (the first) exponent symbol
        if (allowFloat && isDecimal && isExpSymbol) {
          char next = source.NextChar;
          bool nextIsSign = next == '-' || next == '+';
          bool nextIsDigit = digits.IndexOf(next) >= 0;
          if (!nextIsSign && !nextIsDigit)
            break;  //Exponent should be followed by either sign or digit
          //ok, we've got real exponent
          details.ExponentSymbol = current.ToString(); //remember the exp char
          details.Flags |= (int) NumberFlags.HasExp;
          source.Position++;
          if (nextIsSign)
            source.Position++; //skip +/- explicitly so we don't have to deal with them on the next iteration
          continue;
        }
        //4. It is something else (not digit, not dot or exponent) - we're done
        break; //from while loop
      }//while
      int end = source.Position;
      if (!foundDigits) 
        return false; 
      details.Body = source.Text.Substring(start, end - start);
      return true;
    }

    protected override bool ConvertValue(CompoundTokenDetails details) {
      if (String.IsNullOrEmpty(details.Body)) {
        details.Error = "Invalid number.";
        return false;
      }

      //Try quick paths
      switch (details.TypeCodes[0]) {
        case TypeCode.Int32: 
          if (QuickConvertToInt32(details)) return true;
          break;
        case TypeCode.Double:
          if (QuickConvertToDouble(details)) return true;
          break;
      }

      //Go full cycle
      details.Value = null;
      foreach (TypeCode typeCode in details.TypeCodes) {
        switch (typeCode) {
          case TypeCode.Single:   case TypeCode.Double:  case TypeCode.Decimal:  case TypeCodeImaginary:
            return ConvertToFloat(typeCode, details);
          case TypeCode.SByte:    case TypeCode.Byte:    case TypeCode.Int16:    case TypeCode.UInt16:
          case TypeCode.Int32:    case TypeCode.UInt32:  case TypeCode.Int64:    case TypeCode.UInt64:
            if (details.Value == null) //if it is not done yet
              TryConvertToUlong(details); //try to convert to ULong and place the result into details.Value field;
            if(TryCastToIntegerType(typeCode, details)) //now try to cast the ULong value to the target type 
              return true;
            break;
          case TypeCodeBigInt:
            if (ConvertToBigInteger(details)) return true;
            break; 
        }//switch
      }
      return false; 
    }//method

    #endregion

    #region private utilities
    private bool QuickConvertToInt32(CompoundTokenDetails details) {
      int radix = GetRadix(details);
      if (radix == 10 && details.Body.Length > 10) return false;    //10 digits is maximum for int32; int32.MaxValue = 2 147 483 647
      try {
        //workaround for .Net FX bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=278448
        int iValue = 0;
        if (radix == 10)
          iValue =  Convert.ToInt32(details.Body, CultureInfo.InvariantCulture);
        else
          iValue = Convert.ToInt32(details.Body, radix);
        details.Value = iValue;
        return true;
      } catch {
        return false;
      }
    }//method

    private bool QuickConvertToDouble(CompoundTokenDetails details) {
      if (details.IsSet((short)(NumberFlags.Binary | NumberFlags.Octal | NumberFlags.Hex | NumberFlags.HasExp))) return false; 
      if (DecimalSeparator != '.') return false;
      double dvalue;
      if (!double.TryParse(details.Body, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out dvalue)) return false;
      details.Value = dvalue;
      return true; 
    }
    private bool ConvertToFloat(TypeCode typeCode, CompoundTokenDetails details) {
      //only decimal numbers can be fractions
      if (details.IsSet((short)(NumberFlags.Binary | NumberFlags.Octal | NumberFlags.Hex))) {
        details.Error = "Invalid number.";
        return false;
      }
      string body = details.Body;
      //Some languages allow exp symbols other than E. Check if it is the case, and change it to E
      // - otherwise .NET conversion methods may fail
      if (details.IsSet((short)NumberFlags.HasExp) && details.ExponentSymbol.ToUpper() != "E")
        body = body.Replace(details.ExponentSymbol, "E");

      //'.' decimal seperator required by invariant culture
      if (details.IsSet((short)NumberFlags.HasDot) && DecimalSeparator != '.')
        body = body.Replace(DecimalSeparator, '.');

      switch (typeCode) {
        case TypeCode.Double:
        case TypeCodeImaginary:
          double dValue;
          if (!Double.TryParse(body, NumberStyles.Float, CultureInfo.InvariantCulture, out dValue)) return false;
          if (typeCode == TypeCodeImaginary)
            details.Value = new Complex64(0, dValue);
          else
            details.Value = dValue; 
          return true;
        case TypeCode.Single:
          float fValue;
          if (!Single.TryParse(body, NumberStyles.Float, CultureInfo.InvariantCulture, out fValue)) return false;
          details.Value = fValue;
          return true; 
        case TypeCode.Decimal:
          decimal decValue;
          if (!Decimal.TryParse(body, NumberStyles.Float, CultureInfo.InvariantCulture, out decValue)) return false;
          details.Value = decValue;
          return true;  
      }//switch
      return false; 
    }
    private bool TryCastToIntegerType(TypeCode typeCode, CompoundTokenDetails details) {
      if (details.Value == null) return false;
      try {
        if (typeCode != TypeCode.UInt64)
          details.Value = Convert.ChangeType(details.Value, typeCode, CultureInfo.InvariantCulture);
        return true;
      } catch (Exception e) {
        Trace.WriteLine("Error converting to integer: text=[" + details.Body + "], type=" + typeCode + ", error: " + e.Message); 
        return false;
      }
    }//method

    private bool TryConvertToUlong(CompoundTokenDetails details) {
      try {
        int radix = GetRadix(details);
        //workaround for .Net FX bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=278448
        if (radix == 10)
          details.Value = Convert.ToUInt64(details.Body, CultureInfo.InvariantCulture);
        else
          details.Value = Convert.ToUInt64(details.Body, radix);
        return true; 
      } catch(OverflowException) {
        return false;
      }
    }


    private bool ConvertToBigInteger(CompoundTokenDetails details) {
      //ignore leading zeros and sign
      details.Body = details.Body.TrimStart('+').TrimStart('-').TrimStart('0');
      int bodyLength = details.Body.Length;
      int radix = GetRadix(details);
      int wordLength = GetSafeWordLength(details);
      int sectionCount = GetSectionCount(bodyLength, wordLength);
      ulong[] numberSections = new ulong[sectionCount]; //big endian

      try {
        int startIndex = details.Body.Length - wordLength;
        for (int sectionIndex = sectionCount - 1; sectionIndex >= 0; sectionIndex--) {
          if (startIndex < 0) {
            wordLength += startIndex;
            startIndex = 0;
          }
          //workaround for .Net FX bug: http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=278448
          if (radix == 10)
            numberSections[sectionIndex] = Convert.ToUInt64(details.Body.Substring(startIndex, wordLength));
          else
            numberSections[sectionIndex] = Convert.ToUInt64(details.Body.Substring(startIndex, wordLength), radix);

          startIndex -= wordLength;
        }
      } catch {
        details.Error = "Invalid number.";
        return false;
      }
      //produce big integer
      ulong safeWordRadix = GetSafeWordRadix(details);
      BigInteger bigIntegerValue = numberSections[0];
      for (int i = 1; i < sectionCount; i++)
        bigIntegerValue = checked(bigIntegerValue * safeWordRadix + numberSections[i]);
      if (details.Sign == "-")
        bigIntegerValue = -bigIntegerValue;
      details.Value = bigIntegerValue;
      return true;
    }

    private int GetRadix(CompoundTokenDetails details) {
      if (details.IsSet((short)NumberFlags.Hex))
        return 16;
      if (details.IsSet((short)NumberFlags.Octal))
        return 8;
      if (details.IsSet((short)NumberFlags.Binary))
        return 2;
      return 10;
    }
    private string GetDigits(CompoundTokenDetails details) {
      if (details.IsSet((short)NumberFlags.Hex))
        return Strings.HexDigits;
      if (details.IsSet((short)NumberFlags.Octal))
        return Strings.OctalDigits;
      if (details.IsSet((short)NumberFlags.Binary))
        return Strings.BinaryDigits;
      return Strings.DecimalDigits;
    }
    private int GetSafeWordLength(CompoundTokenDetails details) {
      if (details.IsSet((short)NumberFlags.Hex))
        return 15;
      if (details.IsSet((short)NumberFlags.Octal))
        return 21; //maxWordLength 22
      if (details.IsSet((short)NumberFlags.Binary))
        return 63;
      return 19; //maxWordLength 20
    }
    private int GetSectionCount(int stringLength, int safeWordLength) {
      int remainder;
      int quotient = Math.DivRem(stringLength, safeWordLength, out remainder);
      return remainder == 0 ? quotient : quotient + 1;
    }
    //radix^safeWordLength
    private ulong GetSafeWordRadix(CompoundTokenDetails details) {
      if (details.IsSet((short)NumberFlags.Hex))
        return 1152921504606846976;
      if (details.IsSet((short)NumberFlags.Octal))
        return 9223372036854775808;
      if (details.IsSet((short) NumberFlags.Binary))
        return 9223372036854775808;
      return 10000000000000000000;
    }
    private static bool IsIntegerCode(TypeCode code) {
      return (code >= TypeCode.SByte && code <= TypeCode.UInt64);
    }
    #endregion


  }//class


}
