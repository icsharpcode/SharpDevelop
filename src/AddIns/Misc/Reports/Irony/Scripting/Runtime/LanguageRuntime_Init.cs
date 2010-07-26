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
using System.Linq;
using System.Text;
//using Irony.Compiler;

namespace Irony.Scripting.Runtime {
  using BigInteger = Microsoft.Scripting.Math.BigInteger;
  using Complex = Microsoft.Scripting.Math.Complex64;

  //Initialization of Runtime
  public partial class LanguageRuntime {

    public virtual void Init() {
      InitBasicTypeList();
      InitTypeConverters();
      InitOperatorImplementations();
    }

    public virtual void InitBasicTypeList() {
      BasicTypes.AddRange(new Type[] {
        typeof(string), typeof(Complex), typeof(Double), typeof(Single), typeof(Decimal), 
        typeof(BigInteger), 
        typeof(UInt64), typeof(Int64), typeof(UInt32), typeof(Int32), typeof(UInt16), typeof(Int16), typeof(byte), typeof(sbyte), typeof(bool) 
      });
    }

    public virtual void InitTypeConverters() {
      bool useComplex = BasicTypes.Contains(typeof(Complex));
      bool useBigInt = BasicTypes.Contains(typeof(BigInteger));
      //->string
      Type T = typeof(string);
      foreach (Type t in BasicTypes)
        if (t != T)
          TypeConverters.Add(t, T, ConvertAnyToString);
      //->Complex
      if (useComplex) {
        TypeConverters.Add(typeof(sbyte), typeof(Complex), ConvertAnyToComplex);
        TypeConverters.Add(typeof(byte), typeof(Complex), ConvertAnyToComplex);
        TypeConverters.Add(typeof(Int16), typeof(Complex), ConvertAnyToComplex);
        TypeConverters.Add(typeof(UInt16), typeof(Complex), ConvertAnyToComplex);
        TypeConverters.Add(typeof(Int32), typeof(Complex), ConvertAnyToComplex);
        TypeConverters.Add(typeof(UInt32), typeof(Complex), ConvertAnyToComplex);
        TypeConverters.Add(typeof(Int64), typeof(Complex), ConvertAnyToComplex);
        TypeConverters.Add(typeof(UInt64), typeof(Complex), ConvertAnyToComplex);
        TypeConverters.Add(typeof(Single), typeof(Complex), ConvertAnyToComplex);
        if (useBigInt) 
          TypeConverters.Add(typeof(BigInteger), typeof(Complex), ConvertBigIntToComplex);
      }
      //->BigInteger
      if (useBigInt) {
        TypeConverters.Add(typeof(sbyte), typeof(BigInteger), ConvertAnyIntToBigInteger);
        TypeConverters.Add(typeof(byte), typeof(BigInteger), ConvertAnyIntToBigInteger);
        TypeConverters.Add(typeof(Int16), typeof(BigInteger), ConvertAnyIntToBigInteger);
        TypeConverters.Add(typeof(UInt16), typeof(BigInteger), ConvertAnyIntToBigInteger);
        TypeConverters.Add(typeof(Int32), typeof(BigInteger), ConvertAnyIntToBigInteger);
        TypeConverters.Add(typeof(UInt32), typeof(BigInteger), ConvertAnyIntToBigInteger);
        TypeConverters.Add(typeof(Int64), typeof(BigInteger), ConvertAnyIntToBigInteger);
        TypeConverters.Add(typeof(UInt64), typeof(BigInteger), ConvertAnyIntToBigInteger);
      }
 
      //->Double
      TypeConverters.Add(typeof(sbyte), typeof(double), value => (double)(sbyte)value);
      TypeConverters.Add(typeof(byte), typeof(double), value => (double)(byte)value);
      TypeConverters.Add(typeof(Int16), typeof(double), value => (double)(Int16)value);
      TypeConverters.Add(typeof(UInt16), typeof(double), value => (double)(UInt16)value);
      TypeConverters.Add(typeof(Int32), typeof(double), value => (double)(Int32)value);
      TypeConverters.Add(typeof(UInt32), typeof(double), value => (double)(UInt32)value);
      TypeConverters.Add(typeof(Int64), typeof(double), value => (double)(Int64)value);
      TypeConverters.Add(typeof(UInt64), typeof(double), value => (double)(UInt64)value);
      TypeConverters.Add(typeof(Single), typeof(double), value => (double)(Single)value);
      if (useBigInt)
        TypeConverters.Add(typeof(BigInteger), typeof(double), value => ((BigInteger)value).ToDouble(null));
      //->Single
      TypeConverters.Add(typeof(sbyte), typeof(Single), value => (Single)(sbyte)value);
      TypeConverters.Add(typeof(byte), typeof(Single), value => (Single)(byte)value);
      TypeConverters.Add(typeof(Int16), typeof(Single), value => (Single)(Int16)value);
      TypeConverters.Add(typeof(UInt16), typeof(Single), value => (Single)(UInt16)value);
      TypeConverters.Add(typeof(Int32), typeof(Single), value => (Single)(Int32)value);
      TypeConverters.Add(typeof(UInt32), typeof(Single), value => (Single)(UInt32)value);
      TypeConverters.Add(typeof(Int64), typeof(Single), value => (Single)(Int64)value);
      TypeConverters.Add(typeof(UInt64), typeof(Single), value => (Single)(UInt64)value);
      if (useBigInt)
        TypeConverters.Add(typeof(BigInteger), typeof(Single), value => (Single)((BigInteger)value).ToDouble(null));
      
      //->UInt64
      TypeConverters.Add(typeof(sbyte), typeof(UInt64), value => (UInt64)(sbyte)value);
      TypeConverters.Add(typeof(byte), typeof(UInt64), value => (UInt64)(byte)value);
      TypeConverters.Add(typeof(Int16), typeof(UInt64), value => (UInt64)(Int16)value);
      TypeConverters.Add(typeof(UInt16), typeof(UInt64), value => (UInt64)(UInt16)value);
      TypeConverters.Add(typeof(Int32), typeof(UInt64), value => (UInt64)(Int32)value);
      TypeConverters.Add(typeof(UInt32), typeof(UInt64), value => (UInt64)(UInt32)value);
      TypeConverters.Add(typeof(Int64), typeof(UInt64), value => (UInt64)(Int64)value);
      //->Int64
      TypeConverters.Add(typeof(sbyte), typeof(Int64), value => (Int64)(sbyte)value);
      TypeConverters.Add(typeof(byte), typeof(Int64), value => (Int64)(byte)value);
      TypeConverters.Add(typeof(Int16), typeof(Int64), value => (Int64)(Int16)value);
      TypeConverters.Add(typeof(UInt16), typeof(Int64), value => (Int64)(UInt16)value);
      TypeConverters.Add(typeof(Int32), typeof(Int64), value => (Int64)(Int32)value);
      TypeConverters.Add(typeof(UInt32), typeof(Int64), value => (Int64)(UInt32)value);
      //->UInt32
      TypeConverters.Add(typeof(sbyte), typeof(UInt32), value => (UInt32)(sbyte)value);
      TypeConverters.Add(typeof(byte), typeof(UInt32), value => (UInt32)(byte)value);
      TypeConverters.Add(typeof(Int16), typeof(UInt32), value => (UInt32)(Int16)value);
      TypeConverters.Add(typeof(UInt16), typeof(UInt32), value => (UInt32)(UInt16)value);
      TypeConverters.Add(typeof(Int32), typeof(UInt32), value => (UInt32)(Int32)value);
      //->Int32
      TypeConverters.Add(typeof(sbyte), typeof(Int32), value => (Int32)(sbyte)value);
      TypeConverters.Add(typeof(byte), typeof(Int32), value => (Int32)(byte)value);
      TypeConverters.Add(typeof(Int16), typeof(Int32), value => (Int32)(Int16)value);
      TypeConverters.Add(typeof(UInt16), typeof(Int32), value => (Int32)(UInt16)value);
      //->UInt16
      TypeConverters.Add(typeof(sbyte), typeof(UInt16), value => (UInt16)(sbyte)value);
      TypeConverters.Add(typeof(byte), typeof(UInt16), value => (UInt16)(byte)value);
      TypeConverters.Add(typeof(Int16), typeof(UInt16), value => (UInt16)(Int16)value);
      //->Int16
      TypeConverters.Add(typeof(sbyte), typeof(Int16), value => (Int16)(sbyte)value);
      TypeConverters.Add(typeof(byte), typeof(Int16), value => (Int16)(byte)value);
      //->byte
      TypeConverters.Add(typeof(sbyte), typeof(byte), value => (byte)(sbyte)value);
    }

    public static object ConvertAnyToString(object value) {
      return value == null ? string.Empty : value.ToString();
    }

    public static object ConvertBigIntToComplex(object value) {
      BigInteger bi = (BigInteger) value;
      return new Complex(bi.ToFloat64());
    }

    public static object ConvertAnyToComplex(object value) {
      double d = Convert.ToDouble(value);
      return new Complex(d);
    }
    public static object ConvertAnyIntToBigInteger(object value) {
      long l = Convert.ToInt64(value);
      return BigInteger.Create(l);
    }

    public virtual void InitOperatorImplementations() {
      //note that arithmetics on byte, sbyte, int16, uint16 are performed in Int32 format, so the result is always Int32
      // we don't force the result back to original type - I don't think it's necessary
      // For each operator, we add a series of implementation methods for same-type operands. They are saved as DispatchRecords in 
      // operator dispatchers. This happens at initialization time. Dispatch records for mismatched argument types (ex: int + double)
      // are created on-the-fly at execution time. 
      CallDispatcher disp;

      disp = CreateDispatcher("+");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x + (sbyte)y);
      disp.Add(typeof(byte), (x, y) => (byte)x + (byte)y);
      disp.Add(typeof(Int16), (x, y) => (Int16)x + (Int16)y);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x + (UInt16)y);
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x + (Int32)y));
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x + (UInt32)y));
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x + (Int64)y));
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x + (UInt64)y));
      disp.Add(typeof(Single), (x, y) => (Single)x + (Single)y);
      disp.Add(typeof(double), (x, y) => (double)x + (double)y);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x + (BigInteger)y);
      disp.Add(typeof(Complex), (x, y) => (Complex)x + (Complex)y);
      disp.Add(typeof(string), (x, y) => (string)x + (string)y);

      disp = CreateDispatcher("-");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x - (sbyte)y);
      disp.Add(typeof(byte), (x, y) => (byte)x - (byte)y);
      disp.Add(typeof(Int16), (x, y) => (Int16)x - (Int16)y);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x - (UInt16)y);
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x - (Int32)y));
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x - (UInt32)y));
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x - (Int64)y));
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x - (UInt64)y));
      disp.Add(typeof(Single), (x, y) => (Single)x - (Single)y);
      disp.Add(typeof(double), (x, y) => (double)x - (double)y);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x - (BigInteger)y);
      disp.Add(typeof(Complex), (x, y) => (Complex)x - (Complex)y);

      disp = CreateDispatcher("*");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x * (sbyte)y);
      disp.Add(typeof(byte), (x, y) => (byte)x * (byte)y);
      disp.Add(typeof(Int16), (x, y) => checked((Int16)x * (Int16)y));
      disp.Add(typeof(UInt16), (x, y) => checked((UInt16)x * (UInt16)y));
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x * (Int32)y));
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x * (UInt32)y));
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x * (Int64)y));
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x * (UInt64)y));
      disp.Add(typeof(Single), (x, y) => (Single)x * (Single)y);
      disp.Add(typeof(double), (x, y) => (double)x * (double)y);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x * (BigInteger)y);
      disp.Add(typeof(Complex), (x, y) => (Complex)x * (Complex)y);

      disp = CreateDispatcher("/");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x / (sbyte)y);
      disp.Add(typeof(byte), (x, y) => (byte)x / (byte)y);
      disp.Add(typeof(Int16), (x, y) => checked((Int16)x / (Int16)y));
      disp.Add(typeof(UInt16), (x, y) => checked((UInt16)x / (UInt16)y));
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x / (Int32)y));
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x / (UInt32)y));
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x / (Int64)y));
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x / (UInt64)y));
      disp.Add(typeof(Single), (x, y) => (Single)x / (Single)y);
      disp.Add(typeof(double), (x, y) => (double)x / (double)y);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x / (BigInteger)y);
      disp.Add(typeof(Complex), (x, y) => (Complex)x / (Complex)y);

      disp = CreateDispatcher("&");
      disp.Add(typeof(bool), (x, y) => (bool)x & (bool)y);
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x & (sbyte)y);
      disp.Add(typeof(byte), (x, y) => (byte)x & (byte)y);
      disp.Add(typeof(Int16), (x, y) => (Int16)x & (Int16)y);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x & (UInt16)y);
      disp.Add(typeof(Int32), (x, y) => (Int32)x & (Int32)y);
      disp.Add(typeof(UInt32), (x, y) => (UInt32)x & (UInt32)y);
      disp.Add(typeof(Int64), (x, y) => (Int64)x & (Int64)y);
      disp.Add(typeof(UInt64), (x, y) => (UInt64)x & (UInt64)y);

      disp = CreateDispatcher("|");
      disp.Add(typeof(bool), (x, y) => (bool)x | (bool)y);
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x | (sbyte)y);
      disp.Add(typeof(byte), (x, y) => (byte)x | (byte)y);
      disp.Add(typeof(Int16), (x, y) => (Int16)x | (Int16)y);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x | (UInt16)y);
      disp.Add(typeof(Int32), (x, y) => (Int32)x | (Int32)y);
      disp.Add(typeof(UInt32), (x, y) => (UInt32)x | (UInt32)y);
      disp.Add(typeof(Int64), (x, y) => (Int64)x | (Int64)y);
      disp.Add(typeof(UInt64), (x, y) => (UInt64)x | (UInt64)y);

      disp = CreateDispatcher("^"); //XOR
      disp.Add(typeof(bool), (x, y) => (bool)x ^ (bool)y);
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x ^ (sbyte)y);
      disp.Add(typeof(byte), (x, y) => (byte)x ^ (byte)y);
      disp.Add(typeof(Int16), (x, y) => (Int16)x ^ (Int16)y);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x ^ (UInt16)y);
      disp.Add(typeof(Int32), (x, y) => (Int32)x ^ (Int32)y);
      disp.Add(typeof(UInt32), (x, y) => (UInt32)x ^ (UInt32)y);
      disp.Add(typeof(Int64), (x, y) => (Int64)x ^ (Int64)y);
      disp.Add(typeof(UInt64), (x, y) => (UInt64)x ^ (UInt64)y);

      //Note that && and || are special forms, not binary operators

      disp = CreateDispatcher("<");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x < (sbyte)y, BoolResultConverter);
      disp.Add(typeof(byte), (x, y) => (byte)x < (byte)y, BoolResultConverter);
      disp.Add(typeof(Int16), (x, y) => (Int16)x < (Int16)y, BoolResultConverter);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x < (UInt16)y, BoolResultConverter);
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x < (Int32)y), BoolResultConverter);
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x < (UInt32)y), BoolResultConverter);
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x < (Int64)y), BoolResultConverter);
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x < (UInt64)y), BoolResultConverter);
      disp.Add(typeof(Single), (x, y) => (Single)x < (Single)y, BoolResultConverter);
      disp.Add(typeof(double), (x, y) => (double)x < (double)y, BoolResultConverter);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x < (BigInteger)y, BoolResultConverter);

      disp = CreateDispatcher(">");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x > (sbyte)y, BoolResultConverter);
      disp.Add(typeof(byte), (x, y) => (byte)x > (byte)y, BoolResultConverter);
      disp.Add(typeof(Int16), (x, y) => (Int16)x > (Int16)y, BoolResultConverter);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x > (UInt16)y, BoolResultConverter);
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x > (Int32)y), BoolResultConverter);
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x > (UInt32)y), BoolResultConverter);
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x > (Int64)y), BoolResultConverter);
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x > (UInt64)y), BoolResultConverter);
      disp.Add(typeof(Single), (x, y) => (Single)x > (Single)y, BoolResultConverter);
      disp.Add(typeof(double), (x, y) => (double)x > (double)y, BoolResultConverter);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x > (BigInteger)y, BoolResultConverter);

      disp = CreateDispatcher("<=");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x <= (sbyte)y, BoolResultConverter);
      disp.Add(typeof(byte), (x, y) => (byte)x <= (byte)y, BoolResultConverter);
      disp.Add(typeof(Int16), (x, y) => (Int16)x <= (Int16)y, BoolResultConverter);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x <= (UInt16)y, BoolResultConverter);
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x <= (Int32)y), BoolResultConverter);
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x <= (UInt32)y), BoolResultConverter);
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x <= (Int64)y), BoolResultConverter);
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x <= (UInt64)y), BoolResultConverter);
      disp.Add(typeof(Single), (x, y) => (Single)x <= (Single)y, BoolResultConverter);
      disp.Add(typeof(double), (x, y) => (double)x <= (double)y, BoolResultConverter);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x <= (BigInteger)y, BoolResultConverter);

      disp = CreateDispatcher(">=");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x >= (sbyte)y, BoolResultConverter);
      disp.Add(typeof(byte), (x, y) => (byte)x >= (byte)y, BoolResultConverter);
      disp.Add(typeof(Int16), (x, y) => (Int16)x >= (Int16)y, BoolResultConverter);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x >= (UInt16)y, BoolResultConverter);
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x >= (Int32)y), BoolResultConverter);
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x >= (UInt32)y), BoolResultConverter);
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x >= (Int64)y), BoolResultConverter);
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x >= (UInt64)y), BoolResultConverter);
      disp.Add(typeof(Single), (x, y) => (Single)x >= (Single)y, BoolResultConverter);
      disp.Add(typeof(double), (x, y) => (double)x >= (double)y, BoolResultConverter);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x >= (BigInteger)y, BoolResultConverter);

      disp = CreateDispatcher("==");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x == (sbyte)y, BoolResultConverter);
      disp.Add(typeof(byte), (x, y) => (byte)x == (byte)y, BoolResultConverter);
      disp.Add(typeof(Int16), (x, y) => (Int16)x == (Int16)y, BoolResultConverter);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x == (UInt16)y, BoolResultConverter);
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x == (Int32)y), BoolResultConverter);
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x == (UInt32)y), BoolResultConverter);
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x == (Int64)y), BoolResultConverter);
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x == (UInt64)y), BoolResultConverter);
      disp.Add(typeof(Single), (x, y) => (Single)x == (Single)y, BoolResultConverter);
      disp.Add(typeof(double), (x, y) => (double)x == (double)y, BoolResultConverter);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x == (BigInteger)y, BoolResultConverter);

      disp = CreateDispatcher("!=");
      disp.Add(typeof(sbyte), (x, y) => (sbyte)x != (sbyte)y, BoolResultConverter);
      disp.Add(typeof(byte), (x, y) => (byte)x != (byte)y, BoolResultConverter);
      disp.Add(typeof(Int16), (x, y) => (Int16)x != (Int16)y, BoolResultConverter);
      disp.Add(typeof(UInt16), (x, y) => (UInt16)x != (UInt16)y, BoolResultConverter);
      disp.Add(typeof(Int32), (x, y) => checked((Int32)x != (Int32)y), BoolResultConverter);
      disp.Add(typeof(UInt32), (x, y) => checked((UInt32)x != (UInt32)y), BoolResultConverter);
      disp.Add(typeof(Int64), (x, y) => checked((Int64)x != (Int64)y), BoolResultConverter);
      disp.Add(typeof(UInt64), (x, y) => checked((UInt64)x != (UInt64)y), BoolResultConverter);
      disp.Add(typeof(Single), (x, y) => (Single)x != (Single)y, BoolResultConverter);
      disp.Add(typeof(double), (x, y) => (double)x != (double)y, BoolResultConverter);
      disp.Add(typeof(BigInteger), (x, y) => (BigInteger)x != (BigInteger)y, BoolResultConverter);


    }//method

  }
  


}//namespace
