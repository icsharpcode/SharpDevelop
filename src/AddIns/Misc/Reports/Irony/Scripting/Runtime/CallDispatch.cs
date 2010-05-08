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
using Irony.CompilerServices;

namespace Irony.Scripting.Runtime {

  public delegate object BinaryCallTarget(object arg1, object arg2);
  
  /// <summary>
  /// The CallDispatcher is responsible for fast dispatching a function call to the implementation based on argument types
  /// </summary>
  /// <remarks> 
  /// For example, the "+" operator execution involves a call to the specific "add" function which is different 
  /// for each arg types combination. So the BinOpNode for "+" operator would hold a reference 
  /// to the instance of "+" CallDispatcher. At runtime, the Evaluate method would call into the dispatcher.Evaluate(arg1, arg2)
  /// method. The method would in turn lookup a dispatch record for argument types in the DispatchRecords dictionary. 
  /// If found, it will call record.Evaluate(arg1, arg2) to actually execute the operation; if the record is not 
  /// found, the dispatcher would call the LanguageRuntime's CreateDispatchRecord method to create a new record 
  /// for the argument types combination. 
  /// </remarks>
  public class CallDispatcher {
    public readonly string MethodName;
    public readonly LanguageRuntime Runtime;
    //A table of DispatchRecord objects, indexed by type pairs; 
    // declare it volatile  to tip compiler about multi-threaded access - not sure if it is needed since we use interlocked 
    private volatile DispatchRecordTable DispatchRecords = new DispatchRecordTable();
    //a clone used for thread-safe table updates
    private DispatchRecordTable DispatchRecordsClone = new DispatchRecordTable();

    public CallDispatcher(LanguageRuntime  runtime, string methodName) {
      this.Runtime = runtime;
      MethodName = methodName;
    }
    public void Evaluate(EvaluationContext context) {
      DispatchRecord record = null;
      try {
        Type arg1Type = (context.Arg1 == null ? null : context.Arg1.GetType());
        Type arg2Type = (context.Arg2 == null ? null : context.Arg2.GetType());
        TypePair key = new TypePair(arg1Type, arg2Type);
        //FIND RECORD for types - remember threads!!! We don't use any locks here - see below Add(record) method.
        if (!DispatchRecords.TryGetValue(key, out record))
          record = this.Runtime.CreateDispatchRecord(this, key);
        context.CurrentResult = record.Evaluate(context.Arg1, context.Arg2);
      } catch (OverflowException ex) {
        if (this.Runtime.HandleOverflow(ex, this, record, context))
        Evaluate(context); //recursively call self again, with new arg types 
      } catch (Exception ex) {
        if (!this.Runtime.HandleException(ex, this, record,context))
          throw;
      }
    }

    private void EvaluateSafe(EvaluationContext context) {
    }
    //Adds a base record with identical argument types
    public void Add(Type commonType, BinaryCallTarget implementation) {
      Add(commonType, implementation, null);
    }
    public void Add(Type commonType, BinaryCallTarget implementation, TypeConverter resultConverter) {
      TypePair key = new TypePair(commonType, commonType);
      DispatchRecord rec = new DispatchRecord(key, commonType, null, null, resultConverter, implementation);
      Add(rec);
    }

    #region Comment on thread safety
    // Thread safety. We don't want to slow down operator evaluation, so in Evaluate method 
    // we call DispatchRecords.TryGetValue without any locks. 
    // However, we need to update the DispatchRecords table from time to time
    // adding records for new combinations of arg types.
    // Here's how we do it. We maintain two identical copies of DispatchRecords table: primary and clone. 
    // When we need to add a record, we first add it to the clone;
    // next, in atomic operation (using Interlocked) we swap primary and clone tables.
    // Finally, we add the new record to the clone table, which is the former primary.
    #endregion
    internal void Add(DispatchRecord record) {
      lock (this) {//lock the whole operation
        DispatchRecordsClone[record.Key] = record; //add the record to clone
        //swap primary and clone tables in atomic operation
        //disable message "a reference to a volatile field will not be treated as volatile"; interlocked operations are exceptions
        #pragma warning disable 0420
        DispatchRecordsClone = System.Threading.Interlocked.Exchange(ref DispatchRecords, DispatchRecordsClone);
        DispatchRecordsClone[record.Key] = record; //update new clone/former primary
      }
    }

    internal DispatchRecord GetRecord(TypePair key) {
      DispatchRecord result;
      if (DispatchRecords.TryGetValue(key, out result)) return result;
      return null;
    }
    public void SetResultConverter(TypeConverter converter) {
      foreach (DispatchRecord rec in DispatchRecords.Values) {
        rec.ResultConverter = converter;
        rec.SetupEvaluationMethod();
      }
    }

  }//class

  public class CallDispatcherTable : Dictionary<string, CallDispatcher> { }


  /// <summary>
  /// The struct is used as a key for the dictionary of call dispatch records. Contains types of arguments for a method or operator
  /// implementation.
  /// </summary>
  public struct TypePair {
    public Type Arg1Type;
    public Type Arg2Type;
    public int HashCode;
    public TypePair(Type arg1Type, Type arg2Type) {
      Arg1Type = arg1Type;
      Arg2Type = arg2Type;
      int h1 = (arg1Type == null ? 0 : arg1Type.GetHashCode());
      int h2 = (arg2Type == null ? 0 : arg2Type.GetHashCode());
      //shift is for assymetry
      HashCode = unchecked( (h1 >> 1) + h2 ); 
    }//OpKey

    public override int GetHashCode() {
      return HashCode;
    }
    public override string ToString() {
      return "(" + Arg1Type + "," + Arg2Type + ")";
    }
  }//class

  //A helper class implementing keys comparisons for DispatchRecordTable dictionary. 
  class TypePairComparer : IEqualityComparer<TypePair> {
    public bool Equals(TypePair x, TypePair y) {
      return x.Arg1Type == y.Arg1Type && x.Arg2Type == y.Arg2Type;
    }
    public int GetHashCode(TypePair obj) {
      return obj.HashCode;
    }
  }

  public class DispatchRecordTable : Dictionary<TypePair, DispatchRecord> {
    public DispatchRecordTable() : base(new TypePairComparer()) { }
  }

  
  
  ///<summary>
  ///The DispatchRecord class represents an implementation of an operator or method with specific argument types.
  ///</summary>
  ///<remarks>
  /// The DispatchRecord holds 4 method execution components, which are simply delegate references: 
  /// converters for both arguments, implementation method and converter for the result. 
  /// Each operator/method implementation (CallDispatch object)contains a dictionary of DispatchRecord objects,
  /// one for each arg type pairs. 
  ///</remarks>
  public sealed class DispatchRecord {
    public BinaryCallTarget Evaluate;  //A reference to the actual method - one of EvaluateConvXXX 
    public readonly TypePair Key;  
    public readonly Type CommonType;
    internal TypeConverter Arg1Converter;
    internal TypeConverter Arg2Converter;
    internal TypeConverter ResultConverter; 
    // no-conversion operator implementation. 
    // It has to be public so it can be accessible for creating records with conversion from "no-conversion" records
    public readonly BinaryCallTarget Implementation;

    public DispatchRecord(TypePair key, Type commonType, TypeConverter arg1Converter, TypeConverter arg2Converter,
             TypeConverter resultConverter, BinaryCallTarget implementation) {
      Key = key;
      CommonType = commonType;
      Arg1Converter = arg1Converter;
      Arg2Converter = arg2Converter;
      ResultConverter = resultConverter;
      Implementation = implementation;
      SetupEvaluationMethod();
    }

    public void SetupEvaluationMethod() {
      if (ResultConverter == null) {
        if (Arg1Converter == null && Arg2Converter == null)
          Evaluate = EvaluateConvNone;
        else if (Arg1Converter != null && Arg2Converter == null)
          Evaluate = EvaluateConvLeft;
        else if (Arg1Converter == null && Arg2Converter != null)
          Evaluate = EvaluateConvRight;
        else // if (Arg1Converter != null && arg2Converter != null)
          Evaluate = EvaluateConvBoth;
      } else {
        //with result converter
        if (Arg1Converter == null && Arg2Converter == null)
          Evaluate = EvaluateConvNoneConvResult;
        else if (Arg1Converter != null && Arg2Converter == null)
          Evaluate = EvaluateConvLeftConvResult;
        else if (Arg1Converter == null && Arg2Converter != null)
          Evaluate = EvaluateConvRightConvResult;
        else // if (Arg1Converter != null && Arg2Converter != null)
          Evaluate = EvaluateConvBothConvResult;
      }
    }

    private object EvaluateConvNone(object arg1, object arg2) {
      return Implementation(arg1, arg2);
    }
    private object EvaluateConvLeft(object arg1, object arg2) {
      return Implementation(Arg1Converter(arg1), arg2);
    }
    private object EvaluateConvRight(object arg1, object arg2) {
      return Implementation(arg1, Arg2Converter(arg2));
    }
    private object EvaluateConvBoth(object arg1, object arg2) {
      return Implementation(Arg1Converter(arg1), Arg2Converter(arg2));
    }

    private object EvaluateConvNoneConvResult(object arg1, object arg2) {
      return ResultConverter(Implementation(arg1, arg2));
    }
    private object EvaluateConvLeftConvResult(object arg1, object arg2) {
      return ResultConverter(Implementation(Arg1Converter(arg1), arg2));
    }
    private object EvaluateConvRightConvResult(object arg1, object arg2) {
      return ResultConverter(Implementation(arg1, Arg2Converter(arg2)));
    }
    private object EvaluateConvBothConvResult(object arg1, object arg2) {
      return ResultConverter(Implementation(Arg1Converter(arg1), Arg2Converter(arg2)));
    }


  }//class



}//namespace
