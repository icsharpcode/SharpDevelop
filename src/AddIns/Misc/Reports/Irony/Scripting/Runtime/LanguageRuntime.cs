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
using Irony.Scripting.Ast;

namespace Irony.Scripting.Runtime {
  using BigInteger = Microsoft.Scripting.Math.BigInteger;
  using Complex = Microsoft.Scripting.Math.Complex64;

  public class ConsoleWriteEventArgs : EventArgs {
    public string Text;
    public ConsoleWriteEventArgs(string text) {
      Text = text;
    }
  }

  public class TypeList : List<Type> { }

  public delegate object TypeConverter(object arg);
  public class TypeConverterTable : Dictionary<TypePair, TypeConverter> {
    public void Add(Type fromType, Type toType, TypeConverter converter) {
      TypePair key = new TypePair(fromType, toType);
      this[key] = converter;
    }
  }

  public class Unassigned {
    private Unassigned() { }

    public override string ToString() {
      return "(unassigned)";
    }
    public static readonly Unassigned Value = new Unassigned();

  }


  //Note: mark the derived language-specific class as sealed - important for JIT optimizations
  // details here: http://www.codeproject.com/KB/dotnet/JITOptimizations.aspx
  public partial class LanguageRuntime {
    public readonly TypeList BasicTypes = new TypeList();
    public readonly TypeConverterTable TypeConverters = new TypeConverterTable();
    public readonly CallDispatcherTable CallDispatchers = new CallDispatcherTable();
    public readonly FunctionBindingTable FunctionBindings = new FunctionBindingTable();
    //Converter of the result for comparison operation; converts bool value to values
    // specific for the language
    public TypeConverter BoolResultConverter = null; 

    public LanguageRuntime() {
      Init();
    }
    public virtual bool IsTrue(object value) {
      return value != NullObject;
    }

    public virtual object NullObject {
      get { return null; }
    }

    public virtual FunctionBindingInfo GetFunctionBindingInfo(string name, AstNodeList  parameters) {
      return FunctionBindings.Find(name, parameters.Count);
    }
    //Utility methods for adding library functions
    public FunctionBindingInfo AddFunction(string name, int paramCount, NodeEvaluate evaluator) {
      return AddFunction(name, paramCount, evaluator, FunctionFlags.IsExternal);
    }
    public FunctionBindingInfo AddFunction(string name, int paramCount, NodeEvaluate evaluator, FunctionFlags flags) {
      FunctionBindingInfo info = new FunctionBindingInfo(name, paramCount, evaluator, null, flags);
      FunctionBindings.Add(name, info);
      return info;
    }

    #region Dispatching
    public virtual CallDispatcher GetDispatcher(string op) {
      CallDispatcher result;
      if (CallDispatchers.TryGetValue(op, out result)) 
        return result;
      return null;
    }
    public virtual CallDispatcher CreateDispatcher(string op) {
      CallDispatcher result = new CallDispatcher(this, op);
      CallDispatchers[op] = result; 
      return result; 
    }
    public virtual DispatchRecord CreateDispatchRecord(CallDispatcher dispatcher, TypePair forKey) {
      Type commonType = GetCommonType(dispatcher.MethodName, forKey.Arg1Type, forKey.Arg2Type);
      if (commonType == null) return null;
      TypeConverter arg1Converter = GetConverter(forKey.Arg1Type, commonType);
      TypeConverter arg2Converter = GetConverter(forKey.Arg2Type, commonType);
      //Get base method for the operator and common type 
      TypePair baseKey = new TypePair(commonType, commonType);
      DispatchRecord rec = dispatcher.GetRecord(baseKey);
      if (rec == null)
        throw new RuntimeException("Operator or method " + dispatcher.MethodName + " is not defined for type " + commonType);
      rec = new DispatchRecord(forKey, commonType, arg1Converter, arg2Converter, rec.ResultConverter, rec.Implementation);
      dispatcher.Add(rec);
      return rec;
    }

    protected virtual Type GetCommonType(string opName, Type type1, Type type2) {
      //Find which one is first in our list
      foreach (Type t in BasicTypes)
        if (type1 == t || type2 == t) return t;
      return null;
    }

    //This is just a sketch
    //TODO: implement customizable behavior, using dictionary type->type, to specify to which  type to switch in case of overflow
    public virtual bool HandleOverflow(Exception ex, CallDispatcher dispatcher, DispatchRecord failedRecord, EvaluationContext context) {
      //get the common type and decide what to do...
      Type newType = null;
      switch (failedRecord.CommonType.Name) {
        case "Byte":
        case "SByte":
        case "Int16":
        case "UInt16":
        case "Int32":
        case "UInt32":
          newType = typeof(Int64);
          break;
        case "Int64":
        case "UInt64":
          newType = typeof(BigInteger);
          break;
        case "Single":
          newType = typeof(double);
          break;
      }
      if (newType == null)
        throw ex;
      context.CallArgs[0] = Convert.ChangeType(context.CallArgs[0], newType);
      context.CallArgs[1] = Convert.ChangeType(context.CallArgs[1], newType);
      return true; 
    }
    public virtual bool HandleException(Exception ex, CallDispatcher dispatcher, DispatchRecord failedTarget, EvaluationContext context) {
      return false;
    }
    #endregion

    #region Converters
    protected virtual TypeConverter GetConverter(Type fromType, Type toType) {
      if (fromType == toType) return null;
      TypePair key = new TypePair(fromType, toType);
      TypeConverter result;
      if (TypeConverters.TryGetValue(key, out result)) return result;
      string err = string.Format("Cannot convert from {0} to {1}.", fromType, toType);
      throw new RuntimeException(err);
    }
    #endregion








    public event EventHandler<ConsoleWriteEventArgs> ConsoleWrite;
    protected void OnConsoleWrite(string text) {
      if (ConsoleWrite != null) {
        ConsoleWriteEventArgs args = new ConsoleWriteEventArgs(text);
        ConsoleWrite(this, args);
      }
    }

    //Temporarily put it here
    public static void Check(bool condition, string message, params object[] args) {
      if (condition) return;
      if (args != null)
        message = string.Format(message, args);
      RuntimeException exc = new RuntimeException(message);
      throw exc;
    }



  }//class

}//namespace

