using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Irony.Compiler {

  [Flags]
  public enum ProductionFlags {
    None = 0,
    IsInitial = 0x01,    //is initial production
    HasTerminals = 0x02, //contains terminal
    IsError = 0x04,      //contains Error terminal
    IsEmpty = 0x08,
  }

  public class LR0ItemList : List<LR0Item> { }
  public class ProductionList : List<Production> { }

  public class Production {
    public ProductionFlags Flags;
    public readonly NonTerminal LValue;                              // left-side element
    public readonly BnfTermList RValues = new BnfTermList();         //the right-side elements sequence
    public readonly GrammarHintList Hints = new GrammarHintList();
    public readonly LR0ItemList LR0Items = new LR0ItemList();        //LR0 items based on this production 
    public Production(NonTerminal lvalue) {
      LValue = lvalue;
    }//constructor

    public bool IsSet(ProductionFlags flag) {
      return (Flags & flag) != ProductionFlags.None;
    }

    public override string ToString() {
      return TextUtils.ProductionToString(this, -1); //no dot
    }

  }//Production class

  public partial class LR0Item {
    public readonly Production Production;
    public readonly int Position;

    public readonly StringSet TailFirsts = new StringSet(); //tail is a set of elements after the Current element
    public bool TailIsNullable = false;

    //automatically generated IDs - used for building keys for lists of kernel LR0Items
    // which in turn are used to quickly lookup parser states in hash
    internal readonly int ID;

    public LR0Item(Production production, int position, int id) {
      Production = production;
      Position = position;
      ID = id;
    }
    //The after-dot element
    public BnfTerm Current {
      get {
        if (Position < Production.RValues.Count)
          return Production.RValues[Position];
        else
          return null;
      }
    }
    public bool IsKernel {
      get { return Position > 0 || (Production.IsSet(ProductionFlags.IsInitial) && Position == 0); }
    }
    public override string ToString() {
      return TextUtils.ProductionToString(this.Production, Position);
    }
  }//LR0Item


}//namespace
