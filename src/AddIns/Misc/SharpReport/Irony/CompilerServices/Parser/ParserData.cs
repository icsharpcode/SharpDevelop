using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Irony.CompilerServices {
  // ParserData is a container for all information used by CoreParser in input processing.
  // ParserData is a field in LanguageData structure and is used by CoreParser when parsing intput. 
  // The state graph entry is InitialState state; the state graph encodes information usually contained 
  // in what is known in literature as transiton/goto tables.
  // The graph is built from the language grammar by ParserBuilder. 
  // See "Parsing Techniques", 2nd edition for introduction to non-canonical parsing algorithms
  using Irony.CompilerServices.Construction;
  public class ParserData {
    public readonly Grammar Grammar;
    public ParseMethod ParseMethod;
    public ParserState InitialState;
    public ParserState FinalState;
    public readonly ParserStateList States = new ParserStateList();
    public int LalrStateCount; //number of canonical LALR states; after this count all states are non-canonical
    public ParserData(Grammar grammar, ParseMethod method) {
      Grammar = grammar;
      ParseMethod = method;
    }
  }


  public class ParserState {
    public readonly string Name;
    public readonly ParserActionTable Actions = new ParserActionTable();
    //Defined for states with a single reduce item; Parser.GetAction returns this action if it is not null.
    public ParserAction DefaultReduceAction;
    //Action to jump to non-canonical state when there's no action for current lookahead in Actions table
    public ParserAction JumpAction;
    //Expected terms contains both terminals and non-terminals and is to be used in 
    //Parser-advise-to-Scanner facility would use it to filter current terminals when Scanner has more than one terminal for current char,
    //   it can ask Parser to filter the list using the ExpectedTerminals in current Parser state. 
    public readonly BnfTermSet ExpectedTerms = new BnfTermSet();
    //Used for error reporting, we would use it to include list of expected terms in error message 
    // It is reduced compared to ExpectedTerms - some terms are "merged" into other non-terminals (with non-empty DisplayName)
    //   to make message shorter and cleaner. It is computed on-demand in CoreParser
    public BnfTermSet ReportedExpectedSet;
    internal ParserStateData BuilderData; //transient, used only during automaton construction and may be cleared after that

    public ParserState(string name) {
      Name = name;
    }
    public void ClearData() {
      BuilderData = null;
    }
    public override string ToString() {
      return Name;
    }
    public override int GetHashCode() {
      return Name.GetHashCode();
    }

  }//class

  public class ParserStateList : List<ParserState> { }
  public class ParserStateSet : HashSet<ParserState> { }
  public class ParserStateHash : Dictionary<string, ParserState> { }

  public enum ParserActionType {
    Shift,
    Reduce,
    Operator,  //shift or reduce depending on operator associativity and precedence
    Code, //conflict resolution made in resolution method in grammar;  
    Jump, // transition to non-canonical state
    Accept,
  }

  public class ParserAction {
    public ParserActionType ActionType;
    public readonly ParserState NewState; //new state for shift or non-canonical transition
    public Production ReduceProduction;

    protected ParserAction(ParserActionType actionType, ParserState newState, Production reduceProduction) {
      this.ActionType = actionType;
      this.NewState = newState;
      this.ReduceProduction = reduceProduction;
    }
    public static ParserAction CreateShift(ParserState newState) {
      return new ParserAction(ParserActionType.Shift, newState, null);
    }
    public static ParserAction CreateReduce(Production reduceProduction) {
      return new ParserAction(ParserActionType.Reduce, null, reduceProduction);
    }
    public static ParserAction CreateOperator(ParserState newState, Production reduceProduction) {
      return new ParserAction(ParserActionType.Operator, newState, reduceProduction);
    }
    public static ParserAction CreateCodeAction(ParserState newState, Production reduceProduction) {
      var action = new ParserAction(ParserActionType.Code, newState, reduceProduction);
      return action; 
    }
    public static ParserAction CreateAccept() {
      return new ParserAction(ParserActionType.Accept, null, null);
    }
    public static ParserAction CreateJump(ParserState nonCanonicalState) {
      return new ParserAction(ParserActionType.Jump, nonCanonicalState, null);
    }
    public override string ToString() {
      switch (this.ActionType) {
        case ParserActionType.Shift: return "Shift to " + NewState.Name;
        case ParserActionType.Reduce: return "Reduce on " + ReduceProduction.ToString();
        case ParserActionType.Operator: return "Operator, shift to " + NewState.Name + "/reduce on " + ReduceProduction.ToString();
        case ParserActionType.Jump: return "Jump to " + NewState.Name;
        case ParserActionType.Accept: return "Accept";
      }
      return "(ERROR)"; //should never happen
    }
  }//class ActionRecord

  public class ParserActionTable : Dictionary<BnfTerm, ParserAction> { }

  [Flags]
  public enum ProductionFlags {
    None = 0,
    HasTerminals = 0x02, //contains terminal
    IsError = 0x04,      //contains Error terminal
    IsEmpty = 0x08,
  }

  public class Production {
    public ProductionFlags Flags;
    public readonly NonTerminal LValue;                              // left-side element
    public readonly BnfTermList RValues = new BnfTermList();         //the right-side elements sequence
    internal readonly Construction.LR0ItemList LR0Items = new Construction.LR0ItemList();        //LR0 items based on this production 
    public readonly BnfTermSet Firsts = new BnfTermSet();
    public readonly BnfTermSet DirectFirsts = new BnfTermSet();

    public Production(NonTerminal lvalue) {
      LValue = lvalue;
    }//constructor

    public bool IsSet(ProductionFlags flag) {
      return (Flags & flag) != ProductionFlags.None;
    }

    public override string ToString() {
      return ProductionToString(this, -1); //no dot
    }
    public static string ProductionToString(Production production, int dotPosition) {
      char dotChar = '\u00B7'; //dot in the middle of the line
      StringBuilder bld = new StringBuilder();
      bld.Append(production.LValue.Name);
      bld.Append(" -> ");
      for (int i = 0; i < production.RValues.Count; i++) {
        if (i == dotPosition)
          bld.Append(dotChar);
        bld.Append(production.RValues[i].ToString());
        bld.Append(" ");
      }//for i
      if (dotPosition == production.RValues.Count)
        bld.Append(dotChar);
      return bld.ToString();
    }

  }//Production class

  public class ProductionList : List<Production> { }

  /// <summary>
  /// The class provides arguments for custom conflict resolution grammar method.
  /// </summary>
  public class ConflictResolutionArgs {
    public readonly CompilerContext Context;
    public readonly Scanner Scanner; 
    public readonly ParserState CurrentParserState;
    public readonly ParseTreeNode CurrentParserInput;
    public readonly ParserState NewShiftState;
    //Results 
    public ParserActionType Result; //shift, reduce or operator
    public Production ReduceProduction; //defaulted to  
    //constructor
    internal ConflictResolutionArgs(CompilerContext context, ParserAction conflictAction) {
      Context = context;
      Scanner = context.Compiler.Parser.Scanner; 
      var coreParser = context.Compiler.Parser.CoreParser;
      CurrentParserState = coreParser.CurrentState;
      CurrentParserInput = coreParser.CurrentInput;
      NewShiftState = conflictAction.NewState;
      ReduceProduction = conflictAction.ReduceProduction;
      Result = ParserActionType.Shift;
    }
  }//class


}//namespace
