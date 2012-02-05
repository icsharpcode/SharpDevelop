// $ANTLR 3.3 Nov 30, 2010 12:45:30 D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3 2011-07-31 12:36:33

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162

 #pragma warning disable 219, 162 

using System;
using System.Collections.Generic;
using Antlr.Runtime;
using Stack = System.Collections.Generic.Stack<object>;
using List = System.Collections.IList;
using ArrayList = System.Collections.Generic.List<object>;

namespace  Xebic.Parsers.ES3 
{
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "3.3 Nov 30, 2010 12:45:30")]
public partial class ES3Lexer : Antlr.Runtime.Lexer
{
	public const int EOF=-1;
	public const int NULL=4;
	public const int TRUE=5;
	public const int FALSE=6;
	public const int BREAK=7;
	public const int CASE=8;
	public const int CATCH=9;
	public const int CONTINUE=10;
	public const int DEFAULT=11;
	public const int DELETE=12;
	public const int DO=13;
	public const int ELSE=14;
	public const int FINALLY=15;
	public const int FOR=16;
	public const int FUNCTION=17;
	public const int IF=18;
	public const int IN=19;
	public const int INSTANCEOF=20;
	public const int NEW=21;
	public const int RETURN=22;
	public const int SWITCH=23;
	public const int THIS=24;
	public const int THROW=25;
	public const int TRY=26;
	public const int TYPEOF=27;
	public const int VAR=28;
	public const int VOID=29;
	public const int WHILE=30;
	public const int WITH=31;
	public const int ABSTRACT=32;
	public const int BOOLEAN=33;
	public const int BYTE=34;
	public const int CHAR=35;
	public const int CLASS=36;
	public const int CONST=37;
	public const int DEBUGGER=38;
	public const int DOUBLE=39;
	public const int ENUM=40;
	public const int EXPORT=41;
	public const int EXTENDS=42;
	public const int FINAL=43;
	public const int FLOAT=44;
	public const int GOTO=45;
	public const int IMPLEMENTS=46;
	public const int IMPORT=47;
	public const int INT=48;
	public const int INTERFACE=49;
	public const int LONG=50;
	public const int NATIVE=51;
	public const int PACKAGE=52;
	public const int PRIVATE=53;
	public const int PROTECTED=54;
	public const int PUBLIC=55;
	public const int SHORT=56;
	public const int STATIC=57;
	public const int SUPER=58;
	public const int SYNCHRONIZED=59;
	public const int THROWS=60;
	public const int TRANSIENT=61;
	public const int VOLATILE=62;
	public const int LBRACE=63;
	public const int RBRACE=64;
	public const int LPAREN=65;
	public const int RPAREN=66;
	public const int LBRACK=67;
	public const int RBRACK=68;
	public const int DOT=69;
	public const int SEMIC=70;
	public const int COMMA=71;
	public const int LT=72;
	public const int GT=73;
	public const int LTE=74;
	public const int GTE=75;
	public const int EQ=76;
	public const int NEQ=77;
	public const int SAME=78;
	public const int NSAME=79;
	public const int ADD=80;
	public const int SUB=81;
	public const int MUL=82;
	public const int MOD=83;
	public const int INC=84;
	public const int DEC=85;
	public const int SHL=86;
	public const int SHR=87;
	public const int SHU=88;
	public const int AND=89;
	public const int OR=90;
	public const int XOR=91;
	public const int NOT=92;
	public const int INV=93;
	public const int LAND=94;
	public const int LOR=95;
	public const int QUE=96;
	public const int COLON=97;
	public const int ASSIGN=98;
	public const int ADDASS=99;
	public const int SUBASS=100;
	public const int MULASS=101;
	public const int MODASS=102;
	public const int SHLASS=103;
	public const int SHRASS=104;
	public const int SHUASS=105;
	public const int ANDASS=106;
	public const int ORASS=107;
	public const int XORASS=108;
	public const int DIV=109;
	public const int DIVASS=110;
	public const int ARGS=111;
	public const int ARRAY=112;
	public const int BLOCK=113;
	public const int BYFIELD=114;
	public const int BYINDEX=115;
	public const int CALL=116;
	public const int CEXPR=117;
	public const int EXPR=118;
	public const int FORITER=119;
	public const int FORSTEP=120;
	public const int ITEM=121;
	public const int LABELLED=122;
	public const int NAMEDVALUE=123;
	public const int NEG=124;
	public const int OBJECT=125;
	public const int PAREXPR=126;
	public const int PDEC=127;
	public const int PINC=128;
	public const int POS=129;
	public const int BSLASH=130;
	public const int DQUOTE=131;
	public const int SQUOTE=132;
	public const int TAB=133;
	public const int VT=134;
	public const int FF=135;
	public const int SP=136;
	public const int NBSP=137;
	public const int USP=138;
	public const int WhiteSpace=139;
	public const int LF=140;
	public const int CR=141;
	public const int LS=142;
	public const int PS=143;
	public const int LineTerminator=144;
	public const int EOL=145;
	public const int MultiLineComment=146;
	public const int SingleLineComment=147;
	public const int Identifier=148;
	public const int StringLiteral=149;
	public const int HexDigit=150;
	public const int IdentifierStartASCII=151;
	public const int DecimalDigit=152;
	public const int IdentifierPart=153;
	public const int IdentifierNameASCIIStart=154;
	public const int RegularExpressionLiteral=155;
	public const int OctalDigit=156;
	public const int ExponentPart=157;
	public const int DecimalIntegerLiteral=158;
	public const int DecimalLiteral=159;
	public const int OctalIntegerLiteral=160;
	public const int HexIntegerLiteral=161;
	public const int CharacterEscapeSequence=162;
	public const int ZeroToThree=163;
	public const int OctalEscapeSequence=164;
	public const int HexEscapeSequence=165;
	public const int UnicodeEscapeSequence=166;
	public const int EscapeSequence=167;
	public const int BackslashSequence=168;
	public const int RegularExpressionFirstChar=169;
	public const int RegularExpressionChar=170;

    // delegates
    // delegators

	public ES3Lexer()
	{
		OnCreated();
	}

	public ES3Lexer(ICharStream input )
		: this(input, new RecognizerSharedState())
	{
	}

	public ES3Lexer(ICharStream input, RecognizerSharedState state)
		: base(input, state)
	{


		OnCreated();
	}
	public override string GrammarFileName { get { return "D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3"; } }

	private static readonly bool[] decisionCanBacktrack = new bool[0];

 
	protected virtual void OnCreated() {}
	protected virtual void EnterRule(string ruleName, int ruleIndex) {}
	protected virtual void LeaveRule(string ruleName, int ruleIndex) {}

    protected virtual void Enter_NULL() {}
    protected virtual void Leave_NULL() {}

    // $ANTLR start "NULL"
    [GrammarRule("NULL")]
    private void mNULL()
    {

    		try
    		{
    		int _type = NULL;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:10:6: ( 'null' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:10:8: 'null'
    		{
    		DebugLocation(10, 8);
    		Match("null"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "NULL"

    protected virtual void Enter_TRUE() {}
    protected virtual void Leave_TRUE() {}

    // $ANTLR start "TRUE"
    [GrammarRule("TRUE")]
    private void mTRUE()
    {

    		try
    		{
    		int _type = TRUE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:11:6: ( 'true' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:11:8: 'true'
    		{
    		DebugLocation(11, 8);
    		Match("true"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "TRUE"

    protected virtual void Enter_FALSE() {}
    protected virtual void Leave_FALSE() {}

    // $ANTLR start "FALSE"
    [GrammarRule("FALSE")]
    private void mFALSE()
    {

    		try
    		{
    		int _type = FALSE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:12:7: ( 'false' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:12:9: 'false'
    		{
    		DebugLocation(12, 9);
    		Match("false"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "FALSE"

    protected virtual void Enter_BREAK() {}
    protected virtual void Leave_BREAK() {}

    // $ANTLR start "BREAK"
    [GrammarRule("BREAK")]
    private void mBREAK()
    {

    		try
    		{
    		int _type = BREAK;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:13:7: ( 'break' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:13:9: 'break'
    		{
    		DebugLocation(13, 9);
    		Match("break"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "BREAK"

    protected virtual void Enter_CASE() {}
    protected virtual void Leave_CASE() {}

    // $ANTLR start "CASE"
    [GrammarRule("CASE")]
    private void mCASE()
    {

    		try
    		{
    		int _type = CASE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:14:6: ( 'case' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:14:8: 'case'
    		{
    		DebugLocation(14, 8);
    		Match("case"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "CASE"

    protected virtual void Enter_CATCH() {}
    protected virtual void Leave_CATCH() {}

    // $ANTLR start "CATCH"
    [GrammarRule("CATCH")]
    private void mCATCH()
    {

    		try
    		{
    		int _type = CATCH;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:15:7: ( 'catch' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:15:9: 'catch'
    		{
    		DebugLocation(15, 9);
    		Match("catch"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "CATCH"

    protected virtual void Enter_CONTINUE() {}
    protected virtual void Leave_CONTINUE() {}

    // $ANTLR start "CONTINUE"
    [GrammarRule("CONTINUE")]
    private void mCONTINUE()
    {

    		try
    		{
    		int _type = CONTINUE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:16:10: ( 'continue' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:16:12: 'continue'
    		{
    		DebugLocation(16, 12);
    		Match("continue"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "CONTINUE"

    protected virtual void Enter_DEFAULT() {}
    protected virtual void Leave_DEFAULT() {}

    // $ANTLR start "DEFAULT"
    [GrammarRule("DEFAULT")]
    private void mDEFAULT()
    {

    		try
    		{
    		int _type = DEFAULT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:17:9: ( 'default' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:17:11: 'default'
    		{
    		DebugLocation(17, 11);
    		Match("default"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DEFAULT"

    protected virtual void Enter_DELETE() {}
    protected virtual void Leave_DELETE() {}

    // $ANTLR start "DELETE"
    [GrammarRule("DELETE")]
    private void mDELETE()
    {

    		try
    		{
    		int _type = DELETE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:18:8: ( 'delete' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:18:10: 'delete'
    		{
    		DebugLocation(18, 10);
    		Match("delete"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DELETE"

    protected virtual void Enter_DO() {}
    protected virtual void Leave_DO() {}

    // $ANTLR start "DO"
    [GrammarRule("DO")]
    private void mDO()
    {

    		try
    		{
    		int _type = DO;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:19:4: ( 'do' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:19:6: 'do'
    		{
    		DebugLocation(19, 6);
    		Match("do"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DO"

    protected virtual void Enter_ELSE() {}
    protected virtual void Leave_ELSE() {}

    // $ANTLR start "ELSE"
    [GrammarRule("ELSE")]
    private void mELSE()
    {

    		try
    		{
    		int _type = ELSE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:20:6: ( 'else' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:20:8: 'else'
    		{
    		DebugLocation(20, 8);
    		Match("else"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ELSE"

    protected virtual void Enter_FINALLY() {}
    protected virtual void Leave_FINALLY() {}

    // $ANTLR start "FINALLY"
    [GrammarRule("FINALLY")]
    private void mFINALLY()
    {

    		try
    		{
    		int _type = FINALLY;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:21:9: ( 'finally' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:21:11: 'finally'
    		{
    		DebugLocation(21, 11);
    		Match("finally"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "FINALLY"

    protected virtual void Enter_FOR() {}
    protected virtual void Leave_FOR() {}

    // $ANTLR start "FOR"
    [GrammarRule("FOR")]
    private void mFOR()
    {

    		try
    		{
    		int _type = FOR;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:22:5: ( 'for' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:22:7: 'for'
    		{
    		DebugLocation(22, 7);
    		Match("for"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "FOR"

    protected virtual void Enter_FUNCTION() {}
    protected virtual void Leave_FUNCTION() {}

    // $ANTLR start "FUNCTION"
    [GrammarRule("FUNCTION")]
    private void mFUNCTION()
    {

    		try
    		{
    		int _type = FUNCTION;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:23:10: ( 'function' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:23:12: 'function'
    		{
    		DebugLocation(23, 12);
    		Match("function"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "FUNCTION"

    protected virtual void Enter_IF() {}
    protected virtual void Leave_IF() {}

    // $ANTLR start "IF"
    [GrammarRule("IF")]
    private void mIF()
    {

    		try
    		{
    		int _type = IF;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:24:4: ( 'if' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:24:6: 'if'
    		{
    		DebugLocation(24, 6);
    		Match("if"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "IF"

    protected virtual void Enter_IN() {}
    protected virtual void Leave_IN() {}

    // $ANTLR start "IN"
    [GrammarRule("IN")]
    private void mIN()
    {

    		try
    		{
    		int _type = IN;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:25:4: ( 'in' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:25:6: 'in'
    		{
    		DebugLocation(25, 6);
    		Match("in"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "IN"

    protected virtual void Enter_INSTANCEOF() {}
    protected virtual void Leave_INSTANCEOF() {}

    // $ANTLR start "INSTANCEOF"
    [GrammarRule("INSTANCEOF")]
    private void mINSTANCEOF()
    {

    		try
    		{
    		int _type = INSTANCEOF;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:26:12: ( 'instanceof' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:26:14: 'instanceof'
    		{
    		DebugLocation(26, 14);
    		Match("instanceof"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "INSTANCEOF"

    protected virtual void Enter_NEW() {}
    protected virtual void Leave_NEW() {}

    // $ANTLR start "NEW"
    [GrammarRule("NEW")]
    private void mNEW()
    {

    		try
    		{
    		int _type = NEW;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:27:5: ( 'new' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:27:7: 'new'
    		{
    		DebugLocation(27, 7);
    		Match("new"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "NEW"

    protected virtual void Enter_RETURN() {}
    protected virtual void Leave_RETURN() {}

    // $ANTLR start "RETURN"
    [GrammarRule("RETURN")]
    private void mRETURN()
    {

    		try
    		{
    		int _type = RETURN;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:28:8: ( 'return' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:28:10: 'return'
    		{
    		DebugLocation(28, 10);
    		Match("return"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "RETURN"

    protected virtual void Enter_SWITCH() {}
    protected virtual void Leave_SWITCH() {}

    // $ANTLR start "SWITCH"
    [GrammarRule("SWITCH")]
    private void mSWITCH()
    {

    		try
    		{
    		int _type = SWITCH;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:29:8: ( 'switch' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:29:10: 'switch'
    		{
    		DebugLocation(29, 10);
    		Match("switch"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SWITCH"

    protected virtual void Enter_THIS() {}
    protected virtual void Leave_THIS() {}

    // $ANTLR start "THIS"
    [GrammarRule("THIS")]
    private void mTHIS()
    {

    		try
    		{
    		int _type = THIS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:30:6: ( 'this' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:30:8: 'this'
    		{
    		DebugLocation(30, 8);
    		Match("this"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "THIS"

    protected virtual void Enter_THROW() {}
    protected virtual void Leave_THROW() {}

    // $ANTLR start "THROW"
    [GrammarRule("THROW")]
    private void mTHROW()
    {

    		try
    		{
    		int _type = THROW;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:31:7: ( 'throw' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:31:9: 'throw'
    		{
    		DebugLocation(31, 9);
    		Match("throw"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "THROW"

    protected virtual void Enter_TRY() {}
    protected virtual void Leave_TRY() {}

    // $ANTLR start "TRY"
    [GrammarRule("TRY")]
    private void mTRY()
    {

    		try
    		{
    		int _type = TRY;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:32:5: ( 'try' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:32:7: 'try'
    		{
    		DebugLocation(32, 7);
    		Match("try"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "TRY"

    protected virtual void Enter_TYPEOF() {}
    protected virtual void Leave_TYPEOF() {}

    // $ANTLR start "TYPEOF"
    [GrammarRule("TYPEOF")]
    private void mTYPEOF()
    {

    		try
    		{
    		int _type = TYPEOF;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:33:8: ( 'typeof' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:33:10: 'typeof'
    		{
    		DebugLocation(33, 10);
    		Match("typeof"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "TYPEOF"

    protected virtual void Enter_VAR() {}
    protected virtual void Leave_VAR() {}

    // $ANTLR start "VAR"
    [GrammarRule("VAR")]
    private void mVAR()
    {

    		try
    		{
    		int _type = VAR;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:34:5: ( 'var' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:34:7: 'var'
    		{
    		DebugLocation(34, 7);
    		Match("var"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "VAR"

    protected virtual void Enter_VOID() {}
    protected virtual void Leave_VOID() {}

    // $ANTLR start "VOID"
    [GrammarRule("VOID")]
    private void mVOID()
    {

    		try
    		{
    		int _type = VOID;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:35:6: ( 'void' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:35:8: 'void'
    		{
    		DebugLocation(35, 8);
    		Match("void"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "VOID"

    protected virtual void Enter_WHILE() {}
    protected virtual void Leave_WHILE() {}

    // $ANTLR start "WHILE"
    [GrammarRule("WHILE")]
    private void mWHILE()
    {

    		try
    		{
    		int _type = WHILE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:36:7: ( 'while' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:36:9: 'while'
    		{
    		DebugLocation(36, 9);
    		Match("while"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "WHILE"

    protected virtual void Enter_WITH() {}
    protected virtual void Leave_WITH() {}

    // $ANTLR start "WITH"
    [GrammarRule("WITH")]
    private void mWITH()
    {

    		try
    		{
    		int _type = WITH;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:37:6: ( 'with' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:37:8: 'with'
    		{
    		DebugLocation(37, 8);
    		Match("with"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "WITH"

    protected virtual void Enter_ABSTRACT() {}
    protected virtual void Leave_ABSTRACT() {}

    // $ANTLR start "ABSTRACT"
    [GrammarRule("ABSTRACT")]
    private void mABSTRACT()
    {

    		try
    		{
    		int _type = ABSTRACT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:38:10: ( 'abstract' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:38:12: 'abstract'
    		{
    		DebugLocation(38, 12);
    		Match("abstract"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ABSTRACT"

    protected virtual void Enter_BOOLEAN() {}
    protected virtual void Leave_BOOLEAN() {}

    // $ANTLR start "BOOLEAN"
    [GrammarRule("BOOLEAN")]
    private void mBOOLEAN()
    {

    		try
    		{
    		int _type = BOOLEAN;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:39:9: ( 'boolean' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:39:11: 'boolean'
    		{
    		DebugLocation(39, 11);
    		Match("boolean"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "BOOLEAN"

    protected virtual void Enter_BYTE() {}
    protected virtual void Leave_BYTE() {}

    // $ANTLR start "BYTE"
    [GrammarRule("BYTE")]
    private void mBYTE()
    {

    		try
    		{
    		int _type = BYTE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:40:6: ( 'byte' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:40:8: 'byte'
    		{
    		DebugLocation(40, 8);
    		Match("byte"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "BYTE"

    protected virtual void Enter_CHAR() {}
    protected virtual void Leave_CHAR() {}

    // $ANTLR start "CHAR"
    [GrammarRule("CHAR")]
    private void mCHAR()
    {

    		try
    		{
    		int _type = CHAR;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:41:6: ( 'char' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:41:8: 'char'
    		{
    		DebugLocation(41, 8);
    		Match("char"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "CHAR"

    protected virtual void Enter_CLASS() {}
    protected virtual void Leave_CLASS() {}

    // $ANTLR start "CLASS"
    [GrammarRule("CLASS")]
    private void mCLASS()
    {

    		try
    		{
    		int _type = CLASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:42:7: ( 'class' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:42:9: 'class'
    		{
    		DebugLocation(42, 9);
    		Match("class"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "CLASS"

    protected virtual void Enter_CONST() {}
    protected virtual void Leave_CONST() {}

    // $ANTLR start "CONST"
    [GrammarRule("CONST")]
    private void mCONST()
    {

    		try
    		{
    		int _type = CONST;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:43:7: ( 'const' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:43:9: 'const'
    		{
    		DebugLocation(43, 9);
    		Match("const"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "CONST"

    protected virtual void Enter_DEBUGGER() {}
    protected virtual void Leave_DEBUGGER() {}

    // $ANTLR start "DEBUGGER"
    [GrammarRule("DEBUGGER")]
    private void mDEBUGGER()
    {

    		try
    		{
    		int _type = DEBUGGER;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:44:10: ( 'debugger' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:44:12: 'debugger'
    		{
    		DebugLocation(44, 12);
    		Match("debugger"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DEBUGGER"

    protected virtual void Enter_DOUBLE() {}
    protected virtual void Leave_DOUBLE() {}

    // $ANTLR start "DOUBLE"
    [GrammarRule("DOUBLE")]
    private void mDOUBLE()
    {

    		try
    		{
    		int _type = DOUBLE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:45:8: ( 'double' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:45:10: 'double'
    		{
    		DebugLocation(45, 10);
    		Match("double"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DOUBLE"

    protected virtual void Enter_ENUM() {}
    protected virtual void Leave_ENUM() {}

    // $ANTLR start "ENUM"
    [GrammarRule("ENUM")]
    private void mENUM()
    {

    		try
    		{
    		int _type = ENUM;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:46:6: ( 'enum' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:46:8: 'enum'
    		{
    		DebugLocation(46, 8);
    		Match("enum"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ENUM"

    protected virtual void Enter_EXPORT() {}
    protected virtual void Leave_EXPORT() {}

    // $ANTLR start "EXPORT"
    [GrammarRule("EXPORT")]
    private void mEXPORT()
    {

    		try
    		{
    		int _type = EXPORT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:47:8: ( 'export' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:47:10: 'export'
    		{
    		DebugLocation(47, 10);
    		Match("export"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "EXPORT"

    protected virtual void Enter_EXTENDS() {}
    protected virtual void Leave_EXTENDS() {}

    // $ANTLR start "EXTENDS"
    [GrammarRule("EXTENDS")]
    private void mEXTENDS()
    {

    		try
    		{
    		int _type = EXTENDS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:48:9: ( 'extends' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:48:11: 'extends'
    		{
    		DebugLocation(48, 11);
    		Match("extends"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "EXTENDS"

    protected virtual void Enter_FINAL() {}
    protected virtual void Leave_FINAL() {}

    // $ANTLR start "FINAL"
    [GrammarRule("FINAL")]
    private void mFINAL()
    {

    		try
    		{
    		int _type = FINAL;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:49:7: ( 'final' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:49:9: 'final'
    		{
    		DebugLocation(49, 9);
    		Match("final"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "FINAL"

    protected virtual void Enter_FLOAT() {}
    protected virtual void Leave_FLOAT() {}

    // $ANTLR start "FLOAT"
    [GrammarRule("FLOAT")]
    private void mFLOAT()
    {

    		try
    		{
    		int _type = FLOAT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:50:7: ( 'float' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:50:9: 'float'
    		{
    		DebugLocation(50, 9);
    		Match("float"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "FLOAT"

    protected virtual void Enter_GOTO() {}
    protected virtual void Leave_GOTO() {}

    // $ANTLR start "GOTO"
    [GrammarRule("GOTO")]
    private void mGOTO()
    {

    		try
    		{
    		int _type = GOTO;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:51:6: ( 'goto' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:51:8: 'goto'
    		{
    		DebugLocation(51, 8);
    		Match("goto"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "GOTO"

    protected virtual void Enter_IMPLEMENTS() {}
    protected virtual void Leave_IMPLEMENTS() {}

    // $ANTLR start "IMPLEMENTS"
    [GrammarRule("IMPLEMENTS")]
    private void mIMPLEMENTS()
    {

    		try
    		{
    		int _type = IMPLEMENTS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:52:12: ( 'implements' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:52:14: 'implements'
    		{
    		DebugLocation(52, 14);
    		Match("implements"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "IMPLEMENTS"

    protected virtual void Enter_IMPORT() {}
    protected virtual void Leave_IMPORT() {}

    // $ANTLR start "IMPORT"
    [GrammarRule("IMPORT")]
    private void mIMPORT()
    {

    		try
    		{
    		int _type = IMPORT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:53:8: ( 'import' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:53:10: 'import'
    		{
    		DebugLocation(53, 10);
    		Match("import"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "IMPORT"

    protected virtual void Enter_INT() {}
    protected virtual void Leave_INT() {}

    // $ANTLR start "INT"
    [GrammarRule("INT")]
    private void mINT()
    {

    		try
    		{
    		int _type = INT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:54:5: ( 'int' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:54:7: 'int'
    		{
    		DebugLocation(54, 7);
    		Match("int"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "INT"

    protected virtual void Enter_INTERFACE() {}
    protected virtual void Leave_INTERFACE() {}

    // $ANTLR start "INTERFACE"
    [GrammarRule("INTERFACE")]
    private void mINTERFACE()
    {

    		try
    		{
    		int _type = INTERFACE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:55:11: ( 'interface' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:55:13: 'interface'
    		{
    		DebugLocation(55, 13);
    		Match("interface"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "INTERFACE"

    protected virtual void Enter_LONG() {}
    protected virtual void Leave_LONG() {}

    // $ANTLR start "LONG"
    [GrammarRule("LONG")]
    private void mLONG()
    {

    		try
    		{
    		int _type = LONG;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:56:6: ( 'long' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:56:8: 'long'
    		{
    		DebugLocation(56, 8);
    		Match("long"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LONG"

    protected virtual void Enter_NATIVE() {}
    protected virtual void Leave_NATIVE() {}

    // $ANTLR start "NATIVE"
    [GrammarRule("NATIVE")]
    private void mNATIVE()
    {

    		try
    		{
    		int _type = NATIVE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:57:8: ( 'native' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:57:10: 'native'
    		{
    		DebugLocation(57, 10);
    		Match("native"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "NATIVE"

    protected virtual void Enter_PACKAGE() {}
    protected virtual void Leave_PACKAGE() {}

    // $ANTLR start "PACKAGE"
    [GrammarRule("PACKAGE")]
    private void mPACKAGE()
    {

    		try
    		{
    		int _type = PACKAGE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:58:9: ( 'package' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:58:11: 'package'
    		{
    		DebugLocation(58, 11);
    		Match("package"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "PACKAGE"

    protected virtual void Enter_PRIVATE() {}
    protected virtual void Leave_PRIVATE() {}

    // $ANTLR start "PRIVATE"
    [GrammarRule("PRIVATE")]
    private void mPRIVATE()
    {

    		try
    		{
    		int _type = PRIVATE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:59:9: ( 'private' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:59:11: 'private'
    		{
    		DebugLocation(59, 11);
    		Match("private"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "PRIVATE"

    protected virtual void Enter_PROTECTED() {}
    protected virtual void Leave_PROTECTED() {}

    // $ANTLR start "PROTECTED"
    [GrammarRule("PROTECTED")]
    private void mPROTECTED()
    {

    		try
    		{
    		int _type = PROTECTED;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:60:11: ( 'protected' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:60:13: 'protected'
    		{
    		DebugLocation(60, 13);
    		Match("protected"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "PROTECTED"

    protected virtual void Enter_PUBLIC() {}
    protected virtual void Leave_PUBLIC() {}

    // $ANTLR start "PUBLIC"
    [GrammarRule("PUBLIC")]
    private void mPUBLIC()
    {

    		try
    		{
    		int _type = PUBLIC;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:61:8: ( 'public' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:61:10: 'public'
    		{
    		DebugLocation(61, 10);
    		Match("public"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "PUBLIC"

    protected virtual void Enter_SHORT() {}
    protected virtual void Leave_SHORT() {}

    // $ANTLR start "SHORT"
    [GrammarRule("SHORT")]
    private void mSHORT()
    {

    		try
    		{
    		int _type = SHORT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:62:7: ( 'short' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:62:9: 'short'
    		{
    		DebugLocation(62, 9);
    		Match("short"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SHORT"

    protected virtual void Enter_STATIC() {}
    protected virtual void Leave_STATIC() {}

    // $ANTLR start "STATIC"
    [GrammarRule("STATIC")]
    private void mSTATIC()
    {

    		try
    		{
    		int _type = STATIC;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:63:8: ( 'static' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:63:10: 'static'
    		{
    		DebugLocation(63, 10);
    		Match("static"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "STATIC"

    protected virtual void Enter_SUPER() {}
    protected virtual void Leave_SUPER() {}

    // $ANTLR start "SUPER"
    [GrammarRule("SUPER")]
    private void mSUPER()
    {

    		try
    		{
    		int _type = SUPER;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:64:7: ( 'super' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:64:9: 'super'
    		{
    		DebugLocation(64, 9);
    		Match("super"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SUPER"

    protected virtual void Enter_SYNCHRONIZED() {}
    protected virtual void Leave_SYNCHRONIZED() {}

    // $ANTLR start "SYNCHRONIZED"
    [GrammarRule("SYNCHRONIZED")]
    private void mSYNCHRONIZED()
    {

    		try
    		{
    		int _type = SYNCHRONIZED;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:65:14: ( 'synchronized' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:65:16: 'synchronized'
    		{
    		DebugLocation(65, 16);
    		Match("synchronized"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SYNCHRONIZED"

    protected virtual void Enter_THROWS() {}
    protected virtual void Leave_THROWS() {}

    // $ANTLR start "THROWS"
    [GrammarRule("THROWS")]
    private void mTHROWS()
    {

    		try
    		{
    		int _type = THROWS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:66:8: ( 'throws' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:66:10: 'throws'
    		{
    		DebugLocation(66, 10);
    		Match("throws"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "THROWS"

    protected virtual void Enter_TRANSIENT() {}
    protected virtual void Leave_TRANSIENT() {}

    // $ANTLR start "TRANSIENT"
    [GrammarRule("TRANSIENT")]
    private void mTRANSIENT()
    {

    		try
    		{
    		int _type = TRANSIENT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:67:11: ( 'transient' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:67:13: 'transient'
    		{
    		DebugLocation(67, 13);
    		Match("transient"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "TRANSIENT"

    protected virtual void Enter_VOLATILE() {}
    protected virtual void Leave_VOLATILE() {}

    // $ANTLR start "VOLATILE"
    [GrammarRule("VOLATILE")]
    private void mVOLATILE()
    {

    		try
    		{
    		int _type = VOLATILE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:68:10: ( 'volatile' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:68:12: 'volatile'
    		{
    		DebugLocation(68, 12);
    		Match("volatile"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "VOLATILE"

    protected virtual void Enter_LBRACE() {}
    protected virtual void Leave_LBRACE() {}

    // $ANTLR start "LBRACE"
    [GrammarRule("LBRACE")]
    private void mLBRACE()
    {

    		try
    		{
    		int _type = LBRACE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:69:8: ( '{' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:69:10: '{'
    		{
    		DebugLocation(69, 10);
    		Match('{'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LBRACE"

    protected virtual void Enter_RBRACE() {}
    protected virtual void Leave_RBRACE() {}

    // $ANTLR start "RBRACE"
    [GrammarRule("RBRACE")]
    private void mRBRACE()
    {

    		try
    		{
    		int _type = RBRACE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:70:8: ( '}' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:70:10: '}'
    		{
    		DebugLocation(70, 10);
    		Match('}'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "RBRACE"

    protected virtual void Enter_LPAREN() {}
    protected virtual void Leave_LPAREN() {}

    // $ANTLR start "LPAREN"
    [GrammarRule("LPAREN")]
    private void mLPAREN()
    {

    		try
    		{
    		int _type = LPAREN;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:71:8: ( '(' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:71:10: '('
    		{
    		DebugLocation(71, 10);
    		Match('('); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LPAREN"

    protected virtual void Enter_RPAREN() {}
    protected virtual void Leave_RPAREN() {}

    // $ANTLR start "RPAREN"
    [GrammarRule("RPAREN")]
    private void mRPAREN()
    {

    		try
    		{
    		int _type = RPAREN;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:72:8: ( ')' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:72:10: ')'
    		{
    		DebugLocation(72, 10);
    		Match(')'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "RPAREN"

    protected virtual void Enter_LBRACK() {}
    protected virtual void Leave_LBRACK() {}

    // $ANTLR start "LBRACK"
    [GrammarRule("LBRACK")]
    private void mLBRACK()
    {

    		try
    		{
    		int _type = LBRACK;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:73:8: ( '[' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:73:10: '['
    		{
    		DebugLocation(73, 10);
    		Match('['); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LBRACK"

    protected virtual void Enter_RBRACK() {}
    protected virtual void Leave_RBRACK() {}

    // $ANTLR start "RBRACK"
    [GrammarRule("RBRACK")]
    private void mRBRACK()
    {

    		try
    		{
    		int _type = RBRACK;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:74:8: ( ']' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:74:10: ']'
    		{
    		DebugLocation(74, 10);
    		Match(']'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "RBRACK"

    protected virtual void Enter_DOT() {}
    protected virtual void Leave_DOT() {}

    // $ANTLR start "DOT"
    [GrammarRule("DOT")]
    private void mDOT()
    {

    		try
    		{
    		int _type = DOT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:75:5: ( '.' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:75:7: '.'
    		{
    		DebugLocation(75, 7);
    		Match('.'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DOT"

    protected virtual void Enter_SEMIC() {}
    protected virtual void Leave_SEMIC() {}

    // $ANTLR start "SEMIC"
    [GrammarRule("SEMIC")]
    private void mSEMIC()
    {

    		try
    		{
    		int _type = SEMIC;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:76:7: ( ';' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:76:9: ';'
    		{
    		DebugLocation(76, 9);
    		Match(';'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SEMIC"

    protected virtual void Enter_COMMA() {}
    protected virtual void Leave_COMMA() {}

    // $ANTLR start "COMMA"
    [GrammarRule("COMMA")]
    private void mCOMMA()
    {

    		try
    		{
    		int _type = COMMA;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:77:7: ( ',' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:77:9: ','
    		{
    		DebugLocation(77, 9);
    		Match(','); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "COMMA"

    protected virtual void Enter_LT() {}
    protected virtual void Leave_LT() {}

    // $ANTLR start "LT"
    [GrammarRule("LT")]
    private void mLT()
    {

    		try
    		{
    		int _type = LT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:78:4: ( '<' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:78:6: '<'
    		{
    		DebugLocation(78, 6);
    		Match('<'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LT"

    protected virtual void Enter_GT() {}
    protected virtual void Leave_GT() {}

    // $ANTLR start "GT"
    [GrammarRule("GT")]
    private void mGT()
    {

    		try
    		{
    		int _type = GT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:79:4: ( '>' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:79:6: '>'
    		{
    		DebugLocation(79, 6);
    		Match('>'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "GT"

    protected virtual void Enter_LTE() {}
    protected virtual void Leave_LTE() {}

    // $ANTLR start "LTE"
    [GrammarRule("LTE")]
    private void mLTE()
    {

    		try
    		{
    		int _type = LTE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:80:5: ( '<=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:80:7: '<='
    		{
    		DebugLocation(80, 7);
    		Match("<="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LTE"

    protected virtual void Enter_GTE() {}
    protected virtual void Leave_GTE() {}

    // $ANTLR start "GTE"
    [GrammarRule("GTE")]
    private void mGTE()
    {

    		try
    		{
    		int _type = GTE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:81:5: ( '>=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:81:7: '>='
    		{
    		DebugLocation(81, 7);
    		Match(">="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "GTE"

    protected virtual void Enter_EQ() {}
    protected virtual void Leave_EQ() {}

    // $ANTLR start "EQ"
    [GrammarRule("EQ")]
    private void mEQ()
    {

    		try
    		{
    		int _type = EQ;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:82:4: ( '==' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:82:6: '=='
    		{
    		DebugLocation(82, 6);
    		Match("=="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "EQ"

    protected virtual void Enter_NEQ() {}
    protected virtual void Leave_NEQ() {}

    // $ANTLR start "NEQ"
    [GrammarRule("NEQ")]
    private void mNEQ()
    {

    		try
    		{
    		int _type = NEQ;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:83:5: ( '!=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:83:7: '!='
    		{
    		DebugLocation(83, 7);
    		Match("!="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "NEQ"

    protected virtual void Enter_SAME() {}
    protected virtual void Leave_SAME() {}

    // $ANTLR start "SAME"
    [GrammarRule("SAME")]
    private void mSAME()
    {

    		try
    		{
    		int _type = SAME;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:84:6: ( '===' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:84:8: '==='
    		{
    		DebugLocation(84, 8);
    		Match("==="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SAME"

    protected virtual void Enter_NSAME() {}
    protected virtual void Leave_NSAME() {}

    // $ANTLR start "NSAME"
    [GrammarRule("NSAME")]
    private void mNSAME()
    {

    		try
    		{
    		int _type = NSAME;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:85:7: ( '!==' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:85:9: '!=='
    		{
    		DebugLocation(85, 9);
    		Match("!=="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "NSAME"

    protected virtual void Enter_ADD() {}
    protected virtual void Leave_ADD() {}

    // $ANTLR start "ADD"
    [GrammarRule("ADD")]
    private void mADD()
    {

    		try
    		{
    		int _type = ADD;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:86:5: ( '+' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:86:7: '+'
    		{
    		DebugLocation(86, 7);
    		Match('+'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ADD"

    protected virtual void Enter_SUB() {}
    protected virtual void Leave_SUB() {}

    // $ANTLR start "SUB"
    [GrammarRule("SUB")]
    private void mSUB()
    {

    		try
    		{
    		int _type = SUB;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:87:5: ( '-' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:87:7: '-'
    		{
    		DebugLocation(87, 7);
    		Match('-'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SUB"

    protected virtual void Enter_MUL() {}
    protected virtual void Leave_MUL() {}

    // $ANTLR start "MUL"
    [GrammarRule("MUL")]
    private void mMUL()
    {

    		try
    		{
    		int _type = MUL;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:88:5: ( '*' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:88:7: '*'
    		{
    		DebugLocation(88, 7);
    		Match('*'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "MUL"

    protected virtual void Enter_MOD() {}
    protected virtual void Leave_MOD() {}

    // $ANTLR start "MOD"
    [GrammarRule("MOD")]
    private void mMOD()
    {

    		try
    		{
    		int _type = MOD;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:89:5: ( '%' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:89:7: '%'
    		{
    		DebugLocation(89, 7);
    		Match('%'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "MOD"

    protected virtual void Enter_INC() {}
    protected virtual void Leave_INC() {}

    // $ANTLR start "INC"
    [GrammarRule("INC")]
    private void mINC()
    {

    		try
    		{
    		int _type = INC;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:90:5: ( '++' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:90:7: '++'
    		{
    		DebugLocation(90, 7);
    		Match("++"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "INC"

    protected virtual void Enter_DEC() {}
    protected virtual void Leave_DEC() {}

    // $ANTLR start "DEC"
    [GrammarRule("DEC")]
    private void mDEC()
    {

    		try
    		{
    		int _type = DEC;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:91:5: ( '--' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:91:7: '--'
    		{
    		DebugLocation(91, 7);
    		Match("--"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DEC"

    protected virtual void Enter_SHL() {}
    protected virtual void Leave_SHL() {}

    // $ANTLR start "SHL"
    [GrammarRule("SHL")]
    private void mSHL()
    {

    		try
    		{
    		int _type = SHL;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:92:5: ( '<<' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:92:7: '<<'
    		{
    		DebugLocation(92, 7);
    		Match("<<"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SHL"

    protected virtual void Enter_SHR() {}
    protected virtual void Leave_SHR() {}

    // $ANTLR start "SHR"
    [GrammarRule("SHR")]
    private void mSHR()
    {

    		try
    		{
    		int _type = SHR;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:93:5: ( '>>' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:93:7: '>>'
    		{
    		DebugLocation(93, 7);
    		Match(">>"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SHR"

    protected virtual void Enter_SHU() {}
    protected virtual void Leave_SHU() {}

    // $ANTLR start "SHU"
    [GrammarRule("SHU")]
    private void mSHU()
    {

    		try
    		{
    		int _type = SHU;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:94:5: ( '>>>' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:94:7: '>>>'
    		{
    		DebugLocation(94, 7);
    		Match(">>>"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SHU"

    protected virtual void Enter_AND() {}
    protected virtual void Leave_AND() {}

    // $ANTLR start "AND"
    [GrammarRule("AND")]
    private void mAND()
    {

    		try
    		{
    		int _type = AND;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:95:5: ( '&' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:95:7: '&'
    		{
    		DebugLocation(95, 7);
    		Match('&'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "AND"

    protected virtual void Enter_OR() {}
    protected virtual void Leave_OR() {}

    // $ANTLR start "OR"
    [GrammarRule("OR")]
    private void mOR()
    {

    		try
    		{
    		int _type = OR;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:96:4: ( '|' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:96:6: '|'
    		{
    		DebugLocation(96, 6);
    		Match('|'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "OR"

    protected virtual void Enter_XOR() {}
    protected virtual void Leave_XOR() {}

    // $ANTLR start "XOR"
    [GrammarRule("XOR")]
    private void mXOR()
    {

    		try
    		{
    		int _type = XOR;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:97:5: ( '^' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:97:7: '^'
    		{
    		DebugLocation(97, 7);
    		Match('^'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "XOR"

    protected virtual void Enter_NOT() {}
    protected virtual void Leave_NOT() {}

    // $ANTLR start "NOT"
    [GrammarRule("NOT")]
    private void mNOT()
    {

    		try
    		{
    		int _type = NOT;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:98:5: ( '!' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:98:7: '!'
    		{
    		DebugLocation(98, 7);
    		Match('!'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "NOT"

    protected virtual void Enter_INV() {}
    protected virtual void Leave_INV() {}

    // $ANTLR start "INV"
    [GrammarRule("INV")]
    private void mINV()
    {

    		try
    		{
    		int _type = INV;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:99:5: ( '~' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:99:7: '~'
    		{
    		DebugLocation(99, 7);
    		Match('~'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "INV"

    protected virtual void Enter_LAND() {}
    protected virtual void Leave_LAND() {}

    // $ANTLR start "LAND"
    [GrammarRule("LAND")]
    private void mLAND()
    {

    		try
    		{
    		int _type = LAND;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:100:6: ( '&&' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:100:8: '&&'
    		{
    		DebugLocation(100, 8);
    		Match("&&"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LAND"

    protected virtual void Enter_LOR() {}
    protected virtual void Leave_LOR() {}

    // $ANTLR start "LOR"
    [GrammarRule("LOR")]
    private void mLOR()
    {

    		try
    		{
    		int _type = LOR;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:101:5: ( '||' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:101:7: '||'
    		{
    		DebugLocation(101, 7);
    		Match("||"); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LOR"

    protected virtual void Enter_QUE() {}
    protected virtual void Leave_QUE() {}

    // $ANTLR start "QUE"
    [GrammarRule("QUE")]
    private void mQUE()
    {

    		try
    		{
    		int _type = QUE;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:102:5: ( '?' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:102:7: '?'
    		{
    		DebugLocation(102, 7);
    		Match('?'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "QUE"

    protected virtual void Enter_COLON() {}
    protected virtual void Leave_COLON() {}

    // $ANTLR start "COLON"
    [GrammarRule("COLON")]
    private void mCOLON()
    {

    		try
    		{
    		int _type = COLON;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:103:7: ( ':' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:103:9: ':'
    		{
    		DebugLocation(103, 9);
    		Match(':'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "COLON"

    protected virtual void Enter_ASSIGN() {}
    protected virtual void Leave_ASSIGN() {}

    // $ANTLR start "ASSIGN"
    [GrammarRule("ASSIGN")]
    private void mASSIGN()
    {

    		try
    		{
    		int _type = ASSIGN;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:104:8: ( '=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:104:10: '='
    		{
    		DebugLocation(104, 10);
    		Match('='); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ASSIGN"

    protected virtual void Enter_ADDASS() {}
    protected virtual void Leave_ADDASS() {}

    // $ANTLR start "ADDASS"
    [GrammarRule("ADDASS")]
    private void mADDASS()
    {

    		try
    		{
    		int _type = ADDASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:105:8: ( '+=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:105:10: '+='
    		{
    		DebugLocation(105, 10);
    		Match("+="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ADDASS"

    protected virtual void Enter_SUBASS() {}
    protected virtual void Leave_SUBASS() {}

    // $ANTLR start "SUBASS"
    [GrammarRule("SUBASS")]
    private void mSUBASS()
    {

    		try
    		{
    		int _type = SUBASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:106:8: ( '-=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:106:10: '-='
    		{
    		DebugLocation(106, 10);
    		Match("-="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SUBASS"

    protected virtual void Enter_MULASS() {}
    protected virtual void Leave_MULASS() {}

    // $ANTLR start "MULASS"
    [GrammarRule("MULASS")]
    private void mMULASS()
    {

    		try
    		{
    		int _type = MULASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:107:8: ( '*=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:107:10: '*='
    		{
    		DebugLocation(107, 10);
    		Match("*="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "MULASS"

    protected virtual void Enter_MODASS() {}
    protected virtual void Leave_MODASS() {}

    // $ANTLR start "MODASS"
    [GrammarRule("MODASS")]
    private void mMODASS()
    {

    		try
    		{
    		int _type = MODASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:108:8: ( '%=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:108:10: '%='
    		{
    		DebugLocation(108, 10);
    		Match("%="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "MODASS"

    protected virtual void Enter_SHLASS() {}
    protected virtual void Leave_SHLASS() {}

    // $ANTLR start "SHLASS"
    [GrammarRule("SHLASS")]
    private void mSHLASS()
    {

    		try
    		{
    		int _type = SHLASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:109:8: ( '<<=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:109:10: '<<='
    		{
    		DebugLocation(109, 10);
    		Match("<<="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SHLASS"

    protected virtual void Enter_SHRASS() {}
    protected virtual void Leave_SHRASS() {}

    // $ANTLR start "SHRASS"
    [GrammarRule("SHRASS")]
    private void mSHRASS()
    {

    		try
    		{
    		int _type = SHRASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:110:8: ( '>>=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:110:10: '>>='
    		{
    		DebugLocation(110, 10);
    		Match(">>="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SHRASS"

    protected virtual void Enter_SHUASS() {}
    protected virtual void Leave_SHUASS() {}

    // $ANTLR start "SHUASS"
    [GrammarRule("SHUASS")]
    private void mSHUASS()
    {

    		try
    		{
    		int _type = SHUASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:111:8: ( '>>>=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:111:10: '>>>='
    		{
    		DebugLocation(111, 10);
    		Match(">>>="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SHUASS"

    protected virtual void Enter_ANDASS() {}
    protected virtual void Leave_ANDASS() {}

    // $ANTLR start "ANDASS"
    [GrammarRule("ANDASS")]
    private void mANDASS()
    {

    		try
    		{
    		int _type = ANDASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:112:8: ( '&=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:112:10: '&='
    		{
    		DebugLocation(112, 10);
    		Match("&="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ANDASS"

    protected virtual void Enter_ORASS() {}
    protected virtual void Leave_ORASS() {}

    // $ANTLR start "ORASS"
    [GrammarRule("ORASS")]
    private void mORASS()
    {

    		try
    		{
    		int _type = ORASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:113:7: ( '|=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:113:9: '|='
    		{
    		DebugLocation(113, 9);
    		Match("|="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ORASS"

    protected virtual void Enter_XORASS() {}
    protected virtual void Leave_XORASS() {}

    // $ANTLR start "XORASS"
    [GrammarRule("XORASS")]
    private void mXORASS()
    {

    		try
    		{
    		int _type = XORASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:114:8: ( '^=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:114:10: '^='
    		{
    		DebugLocation(114, 10);
    		Match("^="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "XORASS"

    protected virtual void Enter_DIV() {}
    protected virtual void Leave_DIV() {}

    // $ANTLR start "DIV"
    [GrammarRule("DIV")]
    private void mDIV()
    {

    		try
    		{
    		int _type = DIV;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:115:5: ( '/' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:115:7: '/'
    		{
    		DebugLocation(115, 7);
    		Match('/'); 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DIV"

    protected virtual void Enter_DIVASS() {}
    protected virtual void Leave_DIVASS() {}

    // $ANTLR start "DIVASS"
    [GrammarRule("DIVASS")]
    private void mDIVASS()
    {

    		try
    		{
    		int _type = DIVASS;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:116:8: ( '/=' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:116:10: '/='
    		{
    		DebugLocation(116, 10);
    		Match("/="); 


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DIVASS"

    protected virtual void Enter_BSLASH() {}
    protected virtual void Leave_BSLASH() {}

    // $ANTLR start "BSLASH"
    [GrammarRule("BSLASH")]
    private void mBSLASH()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:203:2: ( '\\\\' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:203:4: '\\\\'
    		{
    		DebugLocation(203, 4);
    		Match('\\'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "BSLASH"

    protected virtual void Enter_DQUOTE() {}
    protected virtual void Leave_DQUOTE() {}

    // $ANTLR start "DQUOTE"
    [GrammarRule("DQUOTE")]
    private void mDQUOTE()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:207:2: ( '\"' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:207:4: '\"'
    		{
    		DebugLocation(207, 4);
    		Match('\"'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DQUOTE"

    protected virtual void Enter_SQUOTE() {}
    protected virtual void Leave_SQUOTE() {}

    // $ANTLR start "SQUOTE"
    [GrammarRule("SQUOTE")]
    private void mSQUOTE()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:211:2: ( '\\'' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:211:4: '\\''
    		{
    		DebugLocation(211, 4);
    		Match('\''); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SQUOTE"

    protected virtual void Enter_TAB() {}
    protected virtual void Leave_TAB() {}

    // $ANTLR start "TAB"
    [GrammarRule("TAB")]
    private void mTAB()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:217:2: ( '\\u0009' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:217:4: '\\u0009'
    		{
    		DebugLocation(217, 4);
    		Match('\t'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "TAB"

    protected virtual void Enter_VT() {}
    protected virtual void Leave_VT() {}

    // $ANTLR start "VT"
    [GrammarRule("VT")]
    private void mVT()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:221:2: ( '\\u000b' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:221:4: '\\u000b'
    		{
    		DebugLocation(221, 4);
    		Match('\u000B'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "VT"

    protected virtual void Enter_FF() {}
    protected virtual void Leave_FF() {}

    // $ANTLR start "FF"
    [GrammarRule("FF")]
    private void mFF()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:225:2: ( '\\u000c' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:225:4: '\\u000c'
    		{
    		DebugLocation(225, 4);
    		Match('\f'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "FF"

    protected virtual void Enter_SP() {}
    protected virtual void Leave_SP() {}

    // $ANTLR start "SP"
    [GrammarRule("SP")]
    private void mSP()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:229:2: ( '\\u0020' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:229:4: '\\u0020'
    		{
    		DebugLocation(229, 4);
    		Match(' '); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SP"

    protected virtual void Enter_NBSP() {}
    protected virtual void Leave_NBSP() {}

    // $ANTLR start "NBSP"
    [GrammarRule("NBSP")]
    private void mNBSP()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:233:2: ( '\\u00a0' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:233:4: '\\u00a0'
    		{
    		DebugLocation(233, 4);
    		Match('\u00A0'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "NBSP"

    protected virtual void Enter_USP() {}
    protected virtual void Leave_USP() {}

    // $ANTLR start "USP"
    [GrammarRule("USP")]
    private void mUSP()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:237:2: ( '\\u1680' | '\\u180E' | '\\u2000' | '\\u2001' | '\\u2002' | '\\u2003' | '\\u2004' | '\\u2005' | '\\u2006' | '\\u2007' | '\\u2008' | '\\u2009' | '\\u200A' | '\\u202F' | '\\u205F' | '\\u3000' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:
    		{
    		DebugLocation(237, 2);
    		if (input.LA(1)=='\u1680'||input.LA(1)=='\u180E'||(input.LA(1)>='\u2000' && input.LA(1)<='\u200A')||input.LA(1)=='\u202F'||input.LA(1)=='\u205F'||input.LA(1)=='\u3000')
    		{
    			input.Consume();

    		}
    		else
    		{
    			MismatchedSetException mse = new MismatchedSetException(null,input);
    			DebugRecognitionException(mse);
    			Recover(mse);
    			throw mse;}


    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "USP"

    protected virtual void Enter_WhiteSpace() {}
    protected virtual void Leave_WhiteSpace() {}

    // $ANTLR start "WhiteSpace"
    [GrammarRule("WhiteSpace")]
    private void mWhiteSpace()
    {

    		try
    		{
    		int _type = WhiteSpace;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:256:2: ( ( TAB | VT | FF | SP | NBSP | USP )+ )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:256:4: ( TAB | VT | FF | SP | NBSP | USP )+
    		{
    		DebugLocation(256, 4);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:256:4: ( TAB | VT | FF | SP | NBSP | USP )+
    		int cnt1=0;
    		try { DebugEnterSubRule(1);
    		while (true)
    		{
    			int alt1=2;
    			try { DebugEnterDecision(1, decisionCanBacktrack[1]);
    			int LA1_0 = input.LA(1);

    			if ((LA1_0=='\t'||(LA1_0>='\u000B' && LA1_0<='\f')||LA1_0==' '||LA1_0=='\u00A0'||LA1_0=='\u1680'||LA1_0=='\u180E'||(LA1_0>='\u2000' && LA1_0<='\u200A')||LA1_0=='\u202F'||LA1_0=='\u205F'||LA1_0=='\u3000'))
    			{
    				alt1=1;
    			}


    			} finally { DebugExitDecision(1); }
    			switch (alt1)
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:
    				{
    				DebugLocation(256, 4);
    				if (input.LA(1)=='\t'||(input.LA(1)>='\u000B' && input.LA(1)<='\f')||input.LA(1)==' '||input.LA(1)=='\u00A0'||input.LA(1)=='\u1680'||input.LA(1)=='\u180E'||(input.LA(1)>='\u2000' && input.LA(1)<='\u200A')||input.LA(1)=='\u202F'||input.LA(1)=='\u205F'||input.LA(1)=='\u3000')
    				{
    					input.Consume();

    				}
    				else
    				{
    					MismatchedSetException mse = new MismatchedSetException(null,input);
    					DebugRecognitionException(mse);
    					Recover(mse);
    					throw mse;}


    				}
    				break;

    			default:
    				if (cnt1 >= 1)
    					goto loop1;

    				EarlyExitException eee1 = new EarlyExitException( 1, input );
    				DebugRecognitionException(eee1);
    				throw eee1;
    			}
    			cnt1++;
    		}
    		loop1:
    			;

    		} finally { DebugExitSubRule(1); }

    		DebugLocation(256, 41);
    		 _channel = Hidden; 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "WhiteSpace"

    protected virtual void Enter_LF() {}
    protected virtual void Leave_LF() {}

    // $ANTLR start "LF"
    [GrammarRule("LF")]
    private void mLF()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:264:2: ( '\\n' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:264:4: '\\n'
    		{
    		DebugLocation(264, 4);
    		Match('\n'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LF"

    protected virtual void Enter_CR() {}
    protected virtual void Leave_CR() {}

    // $ANTLR start "CR"
    [GrammarRule("CR")]
    private void mCR()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:268:2: ( '\\r' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:268:4: '\\r'
    		{
    		DebugLocation(268, 4);
    		Match('\r'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "CR"

    protected virtual void Enter_LS() {}
    protected virtual void Leave_LS() {}

    // $ANTLR start "LS"
    [GrammarRule("LS")]
    private void mLS()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:272:2: ( '\\u2028' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:272:4: '\\u2028'
    		{
    		DebugLocation(272, 4);
    		Match('\u2028'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LS"

    protected virtual void Enter_PS() {}
    protected virtual void Leave_PS() {}

    // $ANTLR start "PS"
    [GrammarRule("PS")]
    private void mPS()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:276:2: ( '\\u2029' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:276:4: '\\u2029'
    		{
    		DebugLocation(276, 4);
    		Match('\u2029'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "PS"

    protected virtual void Enter_LineTerminator() {}
    protected virtual void Leave_LineTerminator() {}

    // $ANTLR start "LineTerminator"
    [GrammarRule("LineTerminator")]
    private void mLineTerminator()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:280:2: ( CR | LF | LS | PS )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:
    		{
    		DebugLocation(280, 2);
    		if (input.LA(1)=='\n'||input.LA(1)=='\r'||(input.LA(1)>='\u2028' && input.LA(1)<='\u2029'))
    		{
    			input.Consume();

    		}
    		else
    		{
    			MismatchedSetException mse = new MismatchedSetException(null,input);
    			DebugRecognitionException(mse);
    			Recover(mse);
    			throw mse;}


    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "LineTerminator"

    protected virtual void Enter_EOL() {}
    protected virtual void Leave_EOL() {}

    // $ANTLR start "EOL"
    [GrammarRule("EOL")]
    private void mEOL()
    {

    		try
    		{
    		int _type = EOL;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:2: ( ( ( CR ( LF )? ) | LF | LS | PS ) )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:4: ( ( CR ( LF )? ) | LF | LS | PS )
    		{
    		DebugLocation(284, 4);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:4: ( ( CR ( LF )? ) | LF | LS | PS )
    		int alt3=4;
    		try { DebugEnterSubRule(3);
    		try { DebugEnterDecision(3, decisionCanBacktrack[3]);
    		switch (input.LA(1))
    		{
    		case '\r':
    			{
    			alt3=1;
    			}
    			break;
    		case '\n':
    			{
    			alt3=2;
    			}
    			break;
    		case '\u2028':
    			{
    			alt3=3;
    			}
    			break;
    		case '\u2029':
    			{
    			alt3=4;
    			}
    			break;
    		default:
    			{
    				NoViableAltException nvae = new NoViableAltException("", 3, 0, input);

    				DebugRecognitionException(nvae);
    				throw nvae;
    			}
    		}

    		} finally { DebugExitDecision(3); }
    		switch (alt3)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:6: ( CR ( LF )? )
    			{
    			DebugLocation(284, 6);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:6: ( CR ( LF )? )
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:8: CR ( LF )?
    			{
    			DebugLocation(284, 8);
    			mCR(); 
    			DebugLocation(284, 11);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:11: ( LF )?
    			int alt2=2;
    			try { DebugEnterSubRule(2);
    			try { DebugEnterDecision(2, decisionCanBacktrack[2]);
    			int LA2_0 = input.LA(1);

    			if ((LA2_0=='\n'))
    			{
    				alt2=1;
    			}
    			} finally { DebugExitDecision(2); }
    			switch (alt2)
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:11: LF
    				{
    				DebugLocation(284, 11);
    				mLF(); 

    				}
    				break;

    			}
    			} finally { DebugExitSubRule(2); }


    			}


    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:19: LF
    			{
    			DebugLocation(284, 19);
    			mLF(); 

    			}
    			break;
    		case 3:
    			DebugEnterAlt(3);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:24: LS
    			{
    			DebugLocation(284, 24);
    			mLS(); 

    			}
    			break;
    		case 4:
    			DebugEnterAlt(4);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:284:29: PS
    			{
    			DebugLocation(284, 29);
    			mPS(); 

    			}
    			break;

    		}
    		} finally { DebugExitSubRule(3); }

    		DebugLocation(284, 34);
    		 _channel = Hidden; 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "EOL"

    protected virtual void Enter_MultiLineComment() {}
    protected virtual void Leave_MultiLineComment() {}

    // $ANTLR start "MultiLineComment"
    [GrammarRule("MultiLineComment")]
    private void mMultiLineComment()
    {

    		try
    		{
    		int _type = MultiLineComment;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:291:2: ( '/*' ( options {greedy=false; } : . )* '*/' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:291:4: '/*' ( options {greedy=false; } : . )* '*/'
    		{
    		DebugLocation(291, 4);
    		Match("/*"); 

    		DebugLocation(291, 9);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:291:9: ( options {greedy=false; } : . )*
    		try { DebugEnterSubRule(4);
    		while (true)
    		{
    			int alt4=2;
    			try { DebugEnterDecision(4, decisionCanBacktrack[4]);
    			int LA4_0 = input.LA(1);

    			if ((LA4_0=='*'))
    			{
    				int LA4_1 = input.LA(2);

    				if ((LA4_1=='/'))
    				{
    					alt4=2;
    				}
    				else if (((LA4_1>='\u0000' && LA4_1<='.')||(LA4_1>='0' && LA4_1<='\uFFFF')))
    				{
    					alt4=1;
    				}


    			}
    			else if (((LA4_0>='\u0000' && LA4_0<=')')||(LA4_0>='+' && LA4_0<='\uFFFF')))
    			{
    				alt4=1;
    			}


    			} finally { DebugExitDecision(4); }
    			switch ( alt4 )
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:291:41: .
    				{
    				DebugLocation(291, 41);
    				MatchAny(); 

    				}
    				break;

    			default:
    				goto loop4;
    			}
    		}

    		loop4:
    			;

    		} finally { DebugExitSubRule(4); }

    		DebugLocation(291, 46);
    		Match("*/"); 

    		DebugLocation(291, 51);
    		 _channel = Hidden; 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "MultiLineComment"

    protected virtual void Enter_SingleLineComment() {}
    protected virtual void Leave_SingleLineComment() {}

    // $ANTLR start "SingleLineComment"
    [GrammarRule("SingleLineComment")]
    private void mSingleLineComment()
    {

    		try
    		{
    		int _type = SingleLineComment;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:295:2: ( '//' (~ ( LineTerminator ) )* )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:295:4: '//' (~ ( LineTerminator ) )*
    		{
    		DebugLocation(295, 4);
    		Match("//"); 

    		DebugLocation(295, 9);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:295:9: (~ ( LineTerminator ) )*
    		try { DebugEnterSubRule(5);
    		while (true)
    		{
    			int alt5=2;
    			try { DebugEnterDecision(5, decisionCanBacktrack[5]);
    			int LA5_0 = input.LA(1);

    			if (((LA5_0>='\u0000' && LA5_0<='\t')||(LA5_0>='\u000B' && LA5_0<='\f')||(LA5_0>='\u000E' && LA5_0<='\u2027')||(LA5_0>='\u202A' && LA5_0<='\uFFFF')))
    			{
    				alt5=1;
    			}


    			} finally { DebugExitDecision(5); }
    			switch ( alt5 )
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:295:11: ~ ( LineTerminator )
    				{
    				DebugLocation(295, 11);
    				if ((input.LA(1)>='\u0000' && input.LA(1)<='\t')||(input.LA(1)>='\u000B' && input.LA(1)<='\f')||(input.LA(1)>='\u000E' && input.LA(1)<='\u2027')||(input.LA(1)>='\u202A' && input.LA(1)<='\uFFFF'))
    				{
    					input.Consume();

    				}
    				else
    				{
    					MismatchedSetException mse = new MismatchedSetException(null,input);
    					DebugRecognitionException(mse);
    					Recover(mse);
    					throw mse;}


    				}
    				break;

    			default:
    				goto loop5;
    			}
    		}

    		loop5:
    			;

    		} finally { DebugExitSubRule(5); }

    		DebugLocation(295, 34);
    		 _channel = Hidden; 

    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "SingleLineComment"

    protected virtual void Enter_IdentifierStartASCII() {}
    protected virtual void Leave_IdentifierStartASCII() {}

    // $ANTLR start "IdentifierStartASCII"
    [GrammarRule("IdentifierStartASCII")]
    private void mIdentifierStartASCII()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:396:2: ( 'a' .. 'z' | 'A' .. 'Z' | '$' | '_' | BSLASH 'u' HexDigit HexDigit HexDigit HexDigit )
    		int alt6=5;
    		try { DebugEnterDecision(6, decisionCanBacktrack[6]);
    		switch (input.LA(1))
    		{
    		case 'a':
    		case 'b':
    		case 'c':
    		case 'd':
    		case 'e':
    		case 'f':
    		case 'g':
    		case 'h':
    		case 'i':
    		case 'j':
    		case 'k':
    		case 'l':
    		case 'm':
    		case 'n':
    		case 'o':
    		case 'p':
    		case 'q':
    		case 'r':
    		case 's':
    		case 't':
    		case 'u':
    		case 'v':
    		case 'w':
    		case 'x':
    		case 'y':
    		case 'z':
    			{
    			alt6=1;
    			}
    			break;
    		case 'A':
    		case 'B':
    		case 'C':
    		case 'D':
    		case 'E':
    		case 'F':
    		case 'G':
    		case 'H':
    		case 'I':
    		case 'J':
    		case 'K':
    		case 'L':
    		case 'M':
    		case 'N':
    		case 'O':
    		case 'P':
    		case 'Q':
    		case 'R':
    		case 'S':
    		case 'T':
    		case 'U':
    		case 'V':
    		case 'W':
    		case 'X':
    		case 'Y':
    		case 'Z':
    			{
    			alt6=2;
    			}
    			break;
    		case '$':
    			{
    			alt6=3;
    			}
    			break;
    		case '_':
    			{
    			alt6=4;
    			}
    			break;
    		case '\\':
    			{
    			alt6=5;
    			}
    			break;
    		default:
    			{
    				NoViableAltException nvae = new NoViableAltException("", 6, 0, input);

    				DebugRecognitionException(nvae);
    				throw nvae;
    			}
    		}

    		} finally { DebugExitDecision(6); }
    		switch (alt6)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:396:4: 'a' .. 'z'
    			{
    			DebugLocation(396, 4);
    			MatchRange('a','z'); 

    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:396:15: 'A' .. 'Z'
    			{
    			DebugLocation(396, 15);
    			MatchRange('A','Z'); 

    			}
    			break;
    		case 3:
    			DebugEnterAlt(3);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:397:4: '$'
    			{
    			DebugLocation(397, 4);
    			Match('$'); 

    			}
    			break;
    		case 4:
    			DebugEnterAlt(4);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:398:4: '_'
    			{
    			DebugLocation(398, 4);
    			Match('_'); 

    			}
    			break;
    		case 5:
    			DebugEnterAlt(5);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:399:4: BSLASH 'u' HexDigit HexDigit HexDigit HexDigit
    			{
    			DebugLocation(399, 4);
    			mBSLASH(); 
    			DebugLocation(399, 11);
    			Match('u'); 
    			DebugLocation(399, 15);
    			mHexDigit(); 
    			DebugLocation(399, 24);
    			mHexDigit(); 
    			DebugLocation(399, 33);
    			mHexDigit(); 
    			DebugLocation(399, 42);
    			mHexDigit(); 

    			}
    			break;

    		}
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "IdentifierStartASCII"

    protected virtual void Enter_IdentifierPart() {}
    protected virtual void Leave_IdentifierPart() {}

    // $ANTLR start "IdentifierPart"
    [GrammarRule("IdentifierPart")]
    private void mIdentifierPart()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:407:2: ( DecimalDigit | IdentifierStartASCII | {...}?)
    		int alt7=3;
    		try { DebugEnterDecision(7, decisionCanBacktrack[7]);
    		switch (input.LA(1))
    		{
    		case '0':
    		case '1':
    		case '2':
    		case '3':
    		case '4':
    		case '5':
    		case '6':
    		case '7':
    		case '8':
    		case '9':
    			{
    			alt7=1;
    			}
    			break;
    		case '$':
    		case 'A':
    		case 'B':
    		case 'C':
    		case 'D':
    		case 'E':
    		case 'F':
    		case 'G':
    		case 'H':
    		case 'I':
    		case 'J':
    		case 'K':
    		case 'L':
    		case 'M':
    		case 'N':
    		case 'O':
    		case 'P':
    		case 'Q':
    		case 'R':
    		case 'S':
    		case 'T':
    		case 'U':
    		case 'V':
    		case 'W':
    		case 'X':
    		case 'Y':
    		case 'Z':
    		case '\\':
    		case '_':
    		case 'a':
    		case 'b':
    		case 'c':
    		case 'd':
    		case 'e':
    		case 'f':
    		case 'g':
    		case 'h':
    		case 'i':
    		case 'j':
    		case 'k':
    		case 'l':
    		case 'm':
    		case 'n':
    		case 'o':
    		case 'p':
    		case 'q':
    		case 'r':
    		case 's':
    		case 't':
    		case 'u':
    		case 'v':
    		case 'w':
    		case 'x':
    		case 'y':
    		case 'z':
    			{
    			alt7=2;
    			}
    			break;
    		default:
    			alt7=3;
    			break;
    		}

    		} finally { DebugExitDecision(7); }
    		switch (alt7)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:407:4: DecimalDigit
    			{
    			DebugLocation(407, 4);
    			mDecimalDigit(); 

    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:408:4: IdentifierStartASCII
    			{
    			DebugLocation(408, 4);
    			mIdentifierStartASCII(); 

    			}
    			break;
    		case 3:
    			DebugEnterAlt(3);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:409:4: {...}?
    			{
    			DebugLocation(409, 4);
    			if (!(( IsIdentifierPartUnicode(input.LA(1)) )))
    			{
    				throw new FailedPredicateException(input, "IdentifierPart", " IsIdentifierPartUnicode(input.LA(1)) ");
    			}
    			DebugLocation(409, 46);
    			 MatchAny(); 

    			}
    			break;

    		}
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "IdentifierPart"

    protected virtual void Enter_IdentifierNameASCIIStart() {}
    protected virtual void Leave_IdentifierNameASCIIStart() {}

    // $ANTLR start "IdentifierNameASCIIStart"
    [GrammarRule("IdentifierNameASCIIStart")]
    private void mIdentifierNameASCIIStart()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:413:2: ( IdentifierStartASCII ( IdentifierPart )* )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:413:4: IdentifierStartASCII ( IdentifierPart )*
    		{
    		DebugLocation(413, 4);
    		mIdentifierStartASCII(); 
    		DebugLocation(413, 25);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:413:25: ( IdentifierPart )*
    		try { DebugEnterSubRule(8);
    		while (true)
    		{
    			int alt8=2;
    			try { DebugEnterDecision(8, decisionCanBacktrack[8]);
    			int LA8_0 = input.LA(1);

    			if ((LA8_0=='$'||(LA8_0>='0' && LA8_0<='9')||(LA8_0>='A' && LA8_0<='Z')||LA8_0=='\\'||LA8_0=='_'||(LA8_0>='a' && LA8_0<='z')))
    			{
    				alt8=1;
    			}
    			else if ((( IsIdentifierPartUnicode(input.LA(1)) )))
    			{
    				alt8=1;
    			}


    			} finally { DebugExitDecision(8); }
    			switch ( alt8 )
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:413:25: IdentifierPart
    				{
    				DebugLocation(413, 25);
    				mIdentifierPart(); 

    				}
    				break;

    			default:
    				goto loop8;
    			}
    		}

    		loop8:
    			;

    		} finally { DebugExitSubRule(8); }


    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "IdentifierNameASCIIStart"

    protected virtual void Enter_Identifier() {}
    protected virtual void Leave_Identifier() {}

    // $ANTLR start "Identifier"
    [GrammarRule("Identifier")]
    private void mIdentifier()
    {

    		try
    		{
    		int _type = Identifier;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:425:2: ( IdentifierNameASCIIStart | )
    		int alt9=2;
    		try { DebugEnterDecision(9, decisionCanBacktrack[9]);
    		int LA9_0 = input.LA(1);

    		if ((LA9_0=='$'||(LA9_0>='A' && LA9_0<='Z')||LA9_0=='\\'||LA9_0=='_'||(LA9_0>='a' && LA9_0<='z')))
    		{
    			alt9=1;
    		}
    		else
    		{
    			alt9=2;}
    		} finally { DebugExitDecision(9); }
    		switch (alt9)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:425:4: IdentifierNameASCIIStart
    			{
    			DebugLocation(425, 4);
    			mIdentifierNameASCIIStart(); 

    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:426:4: 
    			{
    			DebugLocation(426, 4);
    			 ConsumeIdentifierUnicodeStart(); 

    			}
    			break;

    		}
    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "Identifier"

    protected virtual void Enter_DecimalDigit() {}
    protected virtual void Leave_DecimalDigit() {}

    // $ANTLR start "DecimalDigit"
    [GrammarRule("DecimalDigit")]
    private void mDecimalDigit()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:509:2: ( '0' .. '9' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:509:4: '0' .. '9'
    		{
    		DebugLocation(509, 4);
    		MatchRange('0','9'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DecimalDigit"

    protected virtual void Enter_HexDigit() {}
    protected virtual void Leave_HexDigit() {}

    // $ANTLR start "HexDigit"
    [GrammarRule("HexDigit")]
    private void mHexDigit()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:513:2: ( DecimalDigit | 'a' .. 'f' | 'A' .. 'F' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:
    		{
    		DebugLocation(513, 2);
    		if ((input.LA(1)>='0' && input.LA(1)<='9')||(input.LA(1)>='A' && input.LA(1)<='F')||(input.LA(1)>='a' && input.LA(1)<='f'))
    		{
    			input.Consume();

    		}
    		else
    		{
    			MismatchedSetException mse = new MismatchedSetException(null,input);
    			DebugRecognitionException(mse);
    			Recover(mse);
    			throw mse;}


    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "HexDigit"

    protected virtual void Enter_OctalDigit() {}
    protected virtual void Leave_OctalDigit() {}

    // $ANTLR start "OctalDigit"
    [GrammarRule("OctalDigit")]
    private void mOctalDigit()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:517:2: ( '0' .. '7' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:517:4: '0' .. '7'
    		{
    		DebugLocation(517, 4);
    		MatchRange('0','7'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "OctalDigit"

    protected virtual void Enter_ExponentPart() {}
    protected virtual void Leave_ExponentPart() {}

    // $ANTLR start "ExponentPart"
    [GrammarRule("ExponentPart")]
    private void mExponentPart()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:521:2: ( ( 'e' | 'E' ) ( '+' | '-' )? ( DecimalDigit )+ )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:521:4: ( 'e' | 'E' ) ( '+' | '-' )? ( DecimalDigit )+
    		{
    		DebugLocation(521, 4);
    		if (input.LA(1)=='E'||input.LA(1)=='e')
    		{
    			input.Consume();

    		}
    		else
    		{
    			MismatchedSetException mse = new MismatchedSetException(null,input);
    			DebugRecognitionException(mse);
    			Recover(mse);
    			throw mse;}

    		DebugLocation(521, 18);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:521:18: ( '+' | '-' )?
    		int alt10=2;
    		try { DebugEnterSubRule(10);
    		try { DebugEnterDecision(10, decisionCanBacktrack[10]);
    		int LA10_0 = input.LA(1);

    		if ((LA10_0=='+'||LA10_0=='-'))
    		{
    			alt10=1;
    		}
    		} finally { DebugExitDecision(10); }
    		switch (alt10)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:
    			{
    			DebugLocation(521, 18);
    			if (input.LA(1)=='+'||input.LA(1)=='-')
    			{
    				input.Consume();

    			}
    			else
    			{
    				MismatchedSetException mse = new MismatchedSetException(null,input);
    				DebugRecognitionException(mse);
    				Recover(mse);
    				throw mse;}


    			}
    			break;

    		}
    		} finally { DebugExitSubRule(10); }

    		DebugLocation(521, 33);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:521:33: ( DecimalDigit )+
    		int cnt11=0;
    		try { DebugEnterSubRule(11);
    		while (true)
    		{
    			int alt11=2;
    			try { DebugEnterDecision(11, decisionCanBacktrack[11]);
    			int LA11_0 = input.LA(1);

    			if (((LA11_0>='0' && LA11_0<='9')))
    			{
    				alt11=1;
    			}


    			} finally { DebugExitDecision(11); }
    			switch (alt11)
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:521:33: DecimalDigit
    				{
    				DebugLocation(521, 33);
    				mDecimalDigit(); 

    				}
    				break;

    			default:
    				if (cnt11 >= 1)
    					goto loop11;

    				EarlyExitException eee11 = new EarlyExitException( 11, input );
    				DebugRecognitionException(eee11);
    				throw eee11;
    			}
    			cnt11++;
    		}
    		loop11:
    			;

    		} finally { DebugExitSubRule(11); }


    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ExponentPart"

    protected virtual void Enter_DecimalIntegerLiteral() {}
    protected virtual void Leave_DecimalIntegerLiteral() {}

    // $ANTLR start "DecimalIntegerLiteral"
    [GrammarRule("DecimalIntegerLiteral")]
    private void mDecimalIntegerLiteral()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:525:2: ( '0' | '1' .. '9' ( DecimalDigit )* )
    		int alt13=2;
    		try { DebugEnterDecision(13, decisionCanBacktrack[13]);
    		int LA13_0 = input.LA(1);

    		if ((LA13_0=='0'))
    		{
    			alt13=1;
    		}
    		else if (((LA13_0>='1' && LA13_0<='9')))
    		{
    			alt13=2;
    		}
    		else
    		{
    			NoViableAltException nvae = new NoViableAltException("", 13, 0, input);

    			DebugRecognitionException(nvae);
    			throw nvae;
    		}
    		} finally { DebugExitDecision(13); }
    		switch (alt13)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:525:4: '0'
    			{
    			DebugLocation(525, 4);
    			Match('0'); 

    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:526:4: '1' .. '9' ( DecimalDigit )*
    			{
    			DebugLocation(526, 4);
    			MatchRange('1','9'); 
    			DebugLocation(526, 13);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:526:13: ( DecimalDigit )*
    			try { DebugEnterSubRule(12);
    			while (true)
    			{
    				int alt12=2;
    				try { DebugEnterDecision(12, decisionCanBacktrack[12]);
    				int LA12_0 = input.LA(1);

    				if (((LA12_0>='0' && LA12_0<='9')))
    				{
    					alt12=1;
    				}


    				} finally { DebugExitDecision(12); }
    				switch ( alt12 )
    				{
    				case 1:
    					DebugEnterAlt(1);
    					// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:526:13: DecimalDigit
    					{
    					DebugLocation(526, 13);
    					mDecimalDigit(); 

    					}
    					break;

    				default:
    					goto loop12;
    				}
    			}

    			loop12:
    				;

    			} finally { DebugExitSubRule(12); }


    			}
    			break;

    		}
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DecimalIntegerLiteral"

    protected virtual void Enter_DecimalLiteral() {}
    protected virtual void Leave_DecimalLiteral() {}

    // $ANTLR start "DecimalLiteral"
    [GrammarRule("DecimalLiteral")]
    private void mDecimalLiteral()
    {

    		try
    		{
    		int _type = DecimalLiteral;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:530:2: ( DecimalIntegerLiteral '.' ( DecimalDigit )* ( ExponentPart )? | '.' ( DecimalDigit )+ ( ExponentPart )? | DecimalIntegerLiteral ( ExponentPart )? )
    		int alt19=3;
    		try { DebugEnterDecision(19, decisionCanBacktrack[19]);
    		try
    		{
    			alt19 = dfa19.Predict(input);
    		}
    		catch (NoViableAltException nvae)
    		{
    			DebugRecognitionException(nvae);
    			throw;
    		}
    		} finally { DebugExitDecision(19); }
    		switch (alt19)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:530:4: DecimalIntegerLiteral '.' ( DecimalDigit )* ( ExponentPart )?
    			{
    			DebugLocation(530, 4);
    			mDecimalIntegerLiteral(); 
    			DebugLocation(530, 26);
    			Match('.'); 
    			DebugLocation(530, 30);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:530:30: ( DecimalDigit )*
    			try { DebugEnterSubRule(14);
    			while (true)
    			{
    				int alt14=2;
    				try { DebugEnterDecision(14, decisionCanBacktrack[14]);
    				int LA14_0 = input.LA(1);

    				if (((LA14_0>='0' && LA14_0<='9')))
    				{
    					alt14=1;
    				}


    				} finally { DebugExitDecision(14); }
    				switch ( alt14 )
    				{
    				case 1:
    					DebugEnterAlt(1);
    					// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:530:30: DecimalDigit
    					{
    					DebugLocation(530, 30);
    					mDecimalDigit(); 

    					}
    					break;

    				default:
    					goto loop14;
    				}
    			}

    			loop14:
    				;

    			} finally { DebugExitSubRule(14); }

    			DebugLocation(530, 44);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:530:44: ( ExponentPart )?
    			int alt15=2;
    			try { DebugEnterSubRule(15);
    			try { DebugEnterDecision(15, decisionCanBacktrack[15]);
    			int LA15_0 = input.LA(1);

    			if ((LA15_0=='E'||LA15_0=='e'))
    			{
    				alt15=1;
    			}
    			} finally { DebugExitDecision(15); }
    			switch (alt15)
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:530:44: ExponentPart
    				{
    				DebugLocation(530, 44);
    				mExponentPart(); 

    				}
    				break;

    			}
    			} finally { DebugExitSubRule(15); }


    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:531:4: '.' ( DecimalDigit )+ ( ExponentPart )?
    			{
    			DebugLocation(531, 4);
    			Match('.'); 
    			DebugLocation(531, 8);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:531:8: ( DecimalDigit )+
    			int cnt16=0;
    			try { DebugEnterSubRule(16);
    			while (true)
    			{
    				int alt16=2;
    				try { DebugEnterDecision(16, decisionCanBacktrack[16]);
    				int LA16_0 = input.LA(1);

    				if (((LA16_0>='0' && LA16_0<='9')))
    				{
    					alt16=1;
    				}


    				} finally { DebugExitDecision(16); }
    				switch (alt16)
    				{
    				case 1:
    					DebugEnterAlt(1);
    					// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:531:8: DecimalDigit
    					{
    					DebugLocation(531, 8);
    					mDecimalDigit(); 

    					}
    					break;

    				default:
    					if (cnt16 >= 1)
    						goto loop16;

    					EarlyExitException eee16 = new EarlyExitException( 16, input );
    					DebugRecognitionException(eee16);
    					throw eee16;
    				}
    				cnt16++;
    			}
    			loop16:
    				;

    			} finally { DebugExitSubRule(16); }

    			DebugLocation(531, 22);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:531:22: ( ExponentPart )?
    			int alt17=2;
    			try { DebugEnterSubRule(17);
    			try { DebugEnterDecision(17, decisionCanBacktrack[17]);
    			int LA17_0 = input.LA(1);

    			if ((LA17_0=='E'||LA17_0=='e'))
    			{
    				alt17=1;
    			}
    			} finally { DebugExitDecision(17); }
    			switch (alt17)
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:531:22: ExponentPart
    				{
    				DebugLocation(531, 22);
    				mExponentPart(); 

    				}
    				break;

    			}
    			} finally { DebugExitSubRule(17); }


    			}
    			break;
    		case 3:
    			DebugEnterAlt(3);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:532:4: DecimalIntegerLiteral ( ExponentPart )?
    			{
    			DebugLocation(532, 4);
    			mDecimalIntegerLiteral(); 
    			DebugLocation(532, 26);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:532:26: ( ExponentPart )?
    			int alt18=2;
    			try { DebugEnterSubRule(18);
    			try { DebugEnterDecision(18, decisionCanBacktrack[18]);
    			int LA18_0 = input.LA(1);

    			if ((LA18_0=='E'||LA18_0=='e'))
    			{
    				alt18=1;
    			}
    			} finally { DebugExitDecision(18); }
    			switch (alt18)
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:532:26: ExponentPart
    				{
    				DebugLocation(532, 26);
    				mExponentPart(); 

    				}
    				break;

    			}
    			} finally { DebugExitSubRule(18); }


    			}
    			break;

    		}
    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "DecimalLiteral"

    protected virtual void Enter_OctalIntegerLiteral() {}
    protected virtual void Leave_OctalIntegerLiteral() {}

    // $ANTLR start "OctalIntegerLiteral"
    [GrammarRule("OctalIntegerLiteral")]
    private void mOctalIntegerLiteral()
    {

    		try
    		{
    		int _type = OctalIntegerLiteral;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:536:2: ( '0' ( OctalDigit )+ )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:536:4: '0' ( OctalDigit )+
    		{
    		DebugLocation(536, 4);
    		Match('0'); 
    		DebugLocation(536, 8);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:536:8: ( OctalDigit )+
    		int cnt20=0;
    		try { DebugEnterSubRule(20);
    		while (true)
    		{
    			int alt20=2;
    			try { DebugEnterDecision(20, decisionCanBacktrack[20]);
    			int LA20_0 = input.LA(1);

    			if (((LA20_0>='0' && LA20_0<='7')))
    			{
    				alt20=1;
    			}


    			} finally { DebugExitDecision(20); }
    			switch (alt20)
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:536:8: OctalDigit
    				{
    				DebugLocation(536, 8);
    				mOctalDigit(); 

    				}
    				break;

    			default:
    				if (cnt20 >= 1)
    					goto loop20;

    				EarlyExitException eee20 = new EarlyExitException( 20, input );
    				DebugRecognitionException(eee20);
    				throw eee20;
    			}
    			cnt20++;
    		}
    		loop20:
    			;

    		} finally { DebugExitSubRule(20); }


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "OctalIntegerLiteral"

    protected virtual void Enter_HexIntegerLiteral() {}
    protected virtual void Leave_HexIntegerLiteral() {}

    // $ANTLR start "HexIntegerLiteral"
    [GrammarRule("HexIntegerLiteral")]
    private void mHexIntegerLiteral()
    {

    		try
    		{
    		int _type = HexIntegerLiteral;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:540:2: ( ( '0x' | '0X' ) ( HexDigit )+ )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:540:4: ( '0x' | '0X' ) ( HexDigit )+
    		{
    		DebugLocation(540, 4);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:540:4: ( '0x' | '0X' )
    		int alt21=2;
    		try { DebugEnterSubRule(21);
    		try { DebugEnterDecision(21, decisionCanBacktrack[21]);
    		int LA21_0 = input.LA(1);

    		if ((LA21_0=='0'))
    		{
    			int LA21_1 = input.LA(2);

    			if ((LA21_1=='x'))
    			{
    				alt21=1;
    			}
    			else if ((LA21_1=='X'))
    			{
    				alt21=2;
    			}
    			else
    			{
    				NoViableAltException nvae = new NoViableAltException("", 21, 1, input);

    				DebugRecognitionException(nvae);
    				throw nvae;
    			}
    		}
    		else
    		{
    			NoViableAltException nvae = new NoViableAltException("", 21, 0, input);

    			DebugRecognitionException(nvae);
    			throw nvae;
    		}
    		} finally { DebugExitDecision(21); }
    		switch (alt21)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:540:6: '0x'
    			{
    			DebugLocation(540, 6);
    			Match("0x"); 


    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:540:13: '0X'
    			{
    			DebugLocation(540, 13);
    			Match("0X"); 


    			}
    			break;

    		}
    		} finally { DebugExitSubRule(21); }

    		DebugLocation(540, 20);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:540:20: ( HexDigit )+
    		int cnt22=0;
    		try { DebugEnterSubRule(22);
    		while (true)
    		{
    			int alt22=2;
    			try { DebugEnterDecision(22, decisionCanBacktrack[22]);
    			int LA22_0 = input.LA(1);

    			if (((LA22_0>='0' && LA22_0<='9')||(LA22_0>='A' && LA22_0<='F')||(LA22_0>='a' && LA22_0<='f')))
    			{
    				alt22=1;
    			}


    			} finally { DebugExitDecision(22); }
    			switch (alt22)
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:540:20: HexDigit
    				{
    				DebugLocation(540, 20);
    				mHexDigit(); 

    				}
    				break;

    			default:
    				if (cnt22 >= 1)
    					goto loop22;

    				EarlyExitException eee22 = new EarlyExitException( 22, input );
    				DebugRecognitionException(eee22);
    				throw eee22;
    			}
    			cnt22++;
    		}
    		loop22:
    			;

    		} finally { DebugExitSubRule(22); }


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "HexIntegerLiteral"

    protected virtual void Enter_CharacterEscapeSequence() {}
    protected virtual void Leave_CharacterEscapeSequence() {}

    // $ANTLR start "CharacterEscapeSequence"
    [GrammarRule("CharacterEscapeSequence")]
    private void mCharacterEscapeSequence()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:559:2: (~ ( DecimalDigit | 'x' | 'u' | LineTerminator ) )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:559:4: ~ ( DecimalDigit | 'x' | 'u' | LineTerminator )
    		{
    		DebugLocation(559, 4);
    		if ((input.LA(1)>='\u0000' && input.LA(1)<='\t')||(input.LA(1)>='\u000B' && input.LA(1)<='\f')||(input.LA(1)>='\u000E' && input.LA(1)<='/')||(input.LA(1)>=':' && input.LA(1)<='t')||(input.LA(1)>='v' && input.LA(1)<='w')||(input.LA(1)>='y' && input.LA(1)<='\u2027')||(input.LA(1)>='\u202A' && input.LA(1)<='\uFFFF'))
    		{
    			input.Consume();

    		}
    		else
    		{
    			MismatchedSetException mse = new MismatchedSetException(null,input);
    			DebugRecognitionException(mse);
    			Recover(mse);
    			throw mse;}


    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "CharacterEscapeSequence"

    protected virtual void Enter_ZeroToThree() {}
    protected virtual void Leave_ZeroToThree() {}

    // $ANTLR start "ZeroToThree"
    [GrammarRule("ZeroToThree")]
    private void mZeroToThree()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:563:2: ( '0' .. '3' )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:563:4: '0' .. '3'
    		{
    		DebugLocation(563, 4);
    		MatchRange('0','3'); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "ZeroToThree"

    protected virtual void Enter_OctalEscapeSequence() {}
    protected virtual void Leave_OctalEscapeSequence() {}

    // $ANTLR start "OctalEscapeSequence"
    [GrammarRule("OctalEscapeSequence")]
    private void mOctalEscapeSequence()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:567:2: ( OctalDigit | ZeroToThree OctalDigit | '4' .. '7' OctalDigit | ZeroToThree OctalDigit OctalDigit )
    		int alt23=4;
    		try { DebugEnterDecision(23, decisionCanBacktrack[23]);
    		int LA23_0 = input.LA(1);

    		if (((LA23_0>='0' && LA23_0<='3')))
    		{
    			int LA23_1 = input.LA(2);

    			if (((LA23_1>='0' && LA23_1<='7')))
    			{
    				int LA23_4 = input.LA(3);

    				if (((LA23_4>='0' && LA23_4<='7')))
    				{
    					alt23=4;
    				}
    				else
    				{
    					alt23=2;}
    			}
    			else
    			{
    				alt23=1;}
    		}
    		else if (((LA23_0>='4' && LA23_0<='7')))
    		{
    			int LA23_2 = input.LA(2);

    			if (((LA23_2>='0' && LA23_2<='7')))
    			{
    				alt23=3;
    			}
    			else
    			{
    				alt23=1;}
    		}
    		else
    		{
    			NoViableAltException nvae = new NoViableAltException("", 23, 0, input);

    			DebugRecognitionException(nvae);
    			throw nvae;
    		}
    		} finally { DebugExitDecision(23); }
    		switch (alt23)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:567:4: OctalDigit
    			{
    			DebugLocation(567, 4);
    			mOctalDigit(); 

    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:568:4: ZeroToThree OctalDigit
    			{
    			DebugLocation(568, 4);
    			mZeroToThree(); 
    			DebugLocation(568, 16);
    			mOctalDigit(); 

    			}
    			break;
    		case 3:
    			DebugEnterAlt(3);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:569:4: '4' .. '7' OctalDigit
    			{
    			DebugLocation(569, 4);
    			MatchRange('4','7'); 
    			DebugLocation(569, 13);
    			mOctalDigit(); 

    			}
    			break;
    		case 4:
    			DebugEnterAlt(4);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:570:4: ZeroToThree OctalDigit OctalDigit
    			{
    			DebugLocation(570, 4);
    			mZeroToThree(); 
    			DebugLocation(570, 16);
    			mOctalDigit(); 
    			DebugLocation(570, 27);
    			mOctalDigit(); 

    			}
    			break;

    		}
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "OctalEscapeSequence"

    protected virtual void Enter_HexEscapeSequence() {}
    protected virtual void Leave_HexEscapeSequence() {}

    // $ANTLR start "HexEscapeSequence"
    [GrammarRule("HexEscapeSequence")]
    private void mHexEscapeSequence()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:574:2: ( 'x' HexDigit HexDigit )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:574:4: 'x' HexDigit HexDigit
    		{
    		DebugLocation(574, 4);
    		Match('x'); 
    		DebugLocation(574, 8);
    		mHexDigit(); 
    		DebugLocation(574, 17);
    		mHexDigit(); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "HexEscapeSequence"

    protected virtual void Enter_UnicodeEscapeSequence() {}
    protected virtual void Leave_UnicodeEscapeSequence() {}

    // $ANTLR start "UnicodeEscapeSequence"
    [GrammarRule("UnicodeEscapeSequence")]
    private void mUnicodeEscapeSequence()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:578:2: ( 'u' HexDigit HexDigit HexDigit HexDigit )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:578:4: 'u' HexDigit HexDigit HexDigit HexDigit
    		{
    		DebugLocation(578, 4);
    		Match('u'); 
    		DebugLocation(578, 8);
    		mHexDigit(); 
    		DebugLocation(578, 17);
    		mHexDigit(); 
    		DebugLocation(578, 26);
    		mHexDigit(); 
    		DebugLocation(578, 35);
    		mHexDigit(); 

    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "UnicodeEscapeSequence"

    protected virtual void Enter_EscapeSequence() {}
    protected virtual void Leave_EscapeSequence() {}

    // $ANTLR start "EscapeSequence"
    [GrammarRule("EscapeSequence")]
    private void mEscapeSequence()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:582:2: ( BSLASH ( CharacterEscapeSequence | OctalEscapeSequence | HexEscapeSequence | UnicodeEscapeSequence ) )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:583:2: BSLASH ( CharacterEscapeSequence | OctalEscapeSequence | HexEscapeSequence | UnicodeEscapeSequence )
    		{
    		DebugLocation(583, 2);
    		mBSLASH(); 
    		DebugLocation(584, 2);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:584:2: ( CharacterEscapeSequence | OctalEscapeSequence | HexEscapeSequence | UnicodeEscapeSequence )
    		int alt24=4;
    		try { DebugEnterSubRule(24);
    		try { DebugEnterDecision(24, decisionCanBacktrack[24]);
    		int LA24_0 = input.LA(1);

    		if (((LA24_0>='\u0000' && LA24_0<='\t')||(LA24_0>='\u000B' && LA24_0<='\f')||(LA24_0>='\u000E' && LA24_0<='/')||(LA24_0>=':' && LA24_0<='t')||(LA24_0>='v' && LA24_0<='w')||(LA24_0>='y' && LA24_0<='\u2027')||(LA24_0>='\u202A' && LA24_0<='\uFFFF')))
    		{
    			alt24=1;
    		}
    		else if (((LA24_0>='0' && LA24_0<='7')))
    		{
    			alt24=2;
    		}
    		else if ((LA24_0=='x'))
    		{
    			alt24=3;
    		}
    		else if ((LA24_0=='u'))
    		{
    			alt24=4;
    		}
    		else
    		{
    			NoViableAltException nvae = new NoViableAltException("", 24, 0, input);

    			DebugRecognitionException(nvae);
    			throw nvae;
    		}
    		} finally { DebugExitDecision(24); }
    		switch (alt24)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:585:3: CharacterEscapeSequence
    			{
    			DebugLocation(585, 3);
    			mCharacterEscapeSequence(); 

    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:586:5: OctalEscapeSequence
    			{
    			DebugLocation(586, 5);
    			mOctalEscapeSequence(); 

    			}
    			break;
    		case 3:
    			DebugEnterAlt(3);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:587:5: HexEscapeSequence
    			{
    			DebugLocation(587, 5);
    			mHexEscapeSequence(); 

    			}
    			break;
    		case 4:
    			DebugEnterAlt(4);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:588:5: UnicodeEscapeSequence
    			{
    			DebugLocation(588, 5);
    			mUnicodeEscapeSequence(); 

    			}
    			break;

    		}
    		} finally { DebugExitSubRule(24); }


    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "EscapeSequence"

    protected virtual void Enter_StringLiteral() {}
    protected virtual void Leave_StringLiteral() {}

    // $ANTLR start "StringLiteral"
    [GrammarRule("StringLiteral")]
    private void mStringLiteral()
    {

    		try
    		{
    		int _type = StringLiteral;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:593:2: ( SQUOTE (~ ( SQUOTE | BSLASH | LineTerminator ) | EscapeSequence )* SQUOTE | DQUOTE (~ ( DQUOTE | BSLASH | LineTerminator ) | EscapeSequence )* DQUOTE )
    		int alt27=2;
    		try { DebugEnterDecision(27, decisionCanBacktrack[27]);
    		int LA27_0 = input.LA(1);

    		if ((LA27_0=='\''))
    		{
    			alt27=1;
    		}
    		else if ((LA27_0=='\"'))
    		{
    			alt27=2;
    		}
    		else
    		{
    			NoViableAltException nvae = new NoViableAltException("", 27, 0, input);

    			DebugRecognitionException(nvae);
    			throw nvae;
    		}
    		} finally { DebugExitDecision(27); }
    		switch (alt27)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:593:4: SQUOTE (~ ( SQUOTE | BSLASH | LineTerminator ) | EscapeSequence )* SQUOTE
    			{
    			DebugLocation(593, 4);
    			mSQUOTE(); 
    			DebugLocation(593, 11);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:593:11: (~ ( SQUOTE | BSLASH | LineTerminator ) | EscapeSequence )*
    			try { DebugEnterSubRule(25);
    			while (true)
    			{
    				int alt25=3;
    				try { DebugEnterDecision(25, decisionCanBacktrack[25]);
    				int LA25_0 = input.LA(1);

    				if (((LA25_0>='\u0000' && LA25_0<='\t')||(LA25_0>='\u000B' && LA25_0<='\f')||(LA25_0>='\u000E' && LA25_0<='&')||(LA25_0>='(' && LA25_0<='[')||(LA25_0>=']' && LA25_0<='\u2027')||(LA25_0>='\u202A' && LA25_0<='\uFFFF')))
    				{
    					alt25=1;
    				}
    				else if ((LA25_0=='\\'))
    				{
    					alt25=2;
    				}


    				} finally { DebugExitDecision(25); }
    				switch ( alt25 )
    				{
    				case 1:
    					DebugEnterAlt(1);
    					// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:593:13: ~ ( SQUOTE | BSLASH | LineTerminator )
    					{
    					DebugLocation(593, 13);
    					if ((input.LA(1)>='\u0000' && input.LA(1)<='\t')||(input.LA(1)>='\u000B' && input.LA(1)<='\f')||(input.LA(1)>='\u000E' && input.LA(1)<='&')||(input.LA(1)>='(' && input.LA(1)<='[')||(input.LA(1)>=']' && input.LA(1)<='\u2027')||(input.LA(1)>='\u202A' && input.LA(1)<='\uFFFF'))
    					{
    						input.Consume();

    					}
    					else
    					{
    						MismatchedSetException mse = new MismatchedSetException(null,input);
    						DebugRecognitionException(mse);
    						Recover(mse);
    						throw mse;}


    					}
    					break;
    				case 2:
    					DebugEnterAlt(2);
    					// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:593:53: EscapeSequence
    					{
    					DebugLocation(593, 53);
    					mEscapeSequence(); 

    					}
    					break;

    				default:
    					goto loop25;
    				}
    			}

    			loop25:
    				;

    			} finally { DebugExitSubRule(25); }

    			DebugLocation(593, 71);
    			mSQUOTE(); 

    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:594:4: DQUOTE (~ ( DQUOTE | BSLASH | LineTerminator ) | EscapeSequence )* DQUOTE
    			{
    			DebugLocation(594, 4);
    			mDQUOTE(); 
    			DebugLocation(594, 11);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:594:11: (~ ( DQUOTE | BSLASH | LineTerminator ) | EscapeSequence )*
    			try { DebugEnterSubRule(26);
    			while (true)
    			{
    				int alt26=3;
    				try { DebugEnterDecision(26, decisionCanBacktrack[26]);
    				int LA26_0 = input.LA(1);

    				if (((LA26_0>='\u0000' && LA26_0<='\t')||(LA26_0>='\u000B' && LA26_0<='\f')||(LA26_0>='\u000E' && LA26_0<='!')||(LA26_0>='#' && LA26_0<='[')||(LA26_0>=']' && LA26_0<='\u2027')||(LA26_0>='\u202A' && LA26_0<='\uFFFF')))
    				{
    					alt26=1;
    				}
    				else if ((LA26_0=='\\'))
    				{
    					alt26=2;
    				}


    				} finally { DebugExitDecision(26); }
    				switch ( alt26 )
    				{
    				case 1:
    					DebugEnterAlt(1);
    					// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:594:13: ~ ( DQUOTE | BSLASH | LineTerminator )
    					{
    					DebugLocation(594, 13);
    					if ((input.LA(1)>='\u0000' && input.LA(1)<='\t')||(input.LA(1)>='\u000B' && input.LA(1)<='\f')||(input.LA(1)>='\u000E' && input.LA(1)<='!')||(input.LA(1)>='#' && input.LA(1)<='[')||(input.LA(1)>=']' && input.LA(1)<='\u2027')||(input.LA(1)>='\u202A' && input.LA(1)<='\uFFFF'))
    					{
    						input.Consume();

    					}
    					else
    					{
    						MismatchedSetException mse = new MismatchedSetException(null,input);
    						DebugRecognitionException(mse);
    						Recover(mse);
    						throw mse;}


    					}
    					break;
    				case 2:
    					DebugEnterAlt(2);
    					// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:594:53: EscapeSequence
    					{
    					DebugLocation(594, 53);
    					mEscapeSequence(); 

    					}
    					break;

    				default:
    					goto loop26;
    				}
    			}

    			loop26:
    				;

    			} finally { DebugExitSubRule(26); }

    			DebugLocation(594, 71);
    			mDQUOTE(); 

    			}
    			break;

    		}
    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "StringLiteral"

    protected virtual void Enter_BackslashSequence() {}
    protected virtual void Leave_BackslashSequence() {}

    // $ANTLR start "BackslashSequence"
    [GrammarRule("BackslashSequence")]
    private void mBackslashSequence()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:602:2: ( BSLASH ~ ( LineTerminator ) )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:602:4: BSLASH ~ ( LineTerminator )
    		{
    		DebugLocation(602, 4);
    		mBSLASH(); 
    		DebugLocation(602, 11);
    		if ((input.LA(1)>='\u0000' && input.LA(1)<='\t')||(input.LA(1)>='\u000B' && input.LA(1)<='\f')||(input.LA(1)>='\u000E' && input.LA(1)<='\u2027')||(input.LA(1)>='\u202A' && input.LA(1)<='\uFFFF'))
    		{
    			input.Consume();

    		}
    		else
    		{
    			MismatchedSetException mse = new MismatchedSetException(null,input);
    			DebugRecognitionException(mse);
    			Recover(mse);
    			throw mse;}


    		}

    	}
    	finally
    	{
        }
    }
    // $ANTLR end "BackslashSequence"

    protected virtual void Enter_RegularExpressionFirstChar() {}
    protected virtual void Leave_RegularExpressionFirstChar() {}

    // $ANTLR start "RegularExpressionFirstChar"
    [GrammarRule("RegularExpressionFirstChar")]
    private void mRegularExpressionFirstChar()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:606:2: (~ ( LineTerminator | MUL | BSLASH | DIV ) | BackslashSequence )
    		int alt28=2;
    		try { DebugEnterDecision(28, decisionCanBacktrack[28]);
    		int LA28_0 = input.LA(1);

    		if (((LA28_0>='\u0000' && LA28_0<='\t')||(LA28_0>='\u000B' && LA28_0<='\f')||(LA28_0>='\u000E' && LA28_0<=')')||(LA28_0>='+' && LA28_0<='.')||(LA28_0>='0' && LA28_0<='[')||(LA28_0>=']' && LA28_0<='\u2027')||(LA28_0>='\u202A' && LA28_0<='\uFFFF')))
    		{
    			alt28=1;
    		}
    		else if ((LA28_0=='\\'))
    		{
    			alt28=2;
    		}
    		else
    		{
    			NoViableAltException nvae = new NoViableAltException("", 28, 0, input);

    			DebugRecognitionException(nvae);
    			throw nvae;
    		}
    		} finally { DebugExitDecision(28); }
    		switch (alt28)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:606:4: ~ ( LineTerminator | MUL | BSLASH | DIV )
    			{
    			DebugLocation(606, 4);
    			if ((input.LA(1)>='\u0000' && input.LA(1)<='\t')||(input.LA(1)>='\u000B' && input.LA(1)<='\f')||(input.LA(1)>='\u000E' && input.LA(1)<=')')||(input.LA(1)>='+' && input.LA(1)<='.')||(input.LA(1)>='0' && input.LA(1)<='[')||(input.LA(1)>=']' && input.LA(1)<='\u2027')||(input.LA(1)>='\u202A' && input.LA(1)<='\uFFFF'))
    			{
    				input.Consume();

    			}
    			else
    			{
    				MismatchedSetException mse = new MismatchedSetException(null,input);
    				DebugRecognitionException(mse);
    				Recover(mse);
    				throw mse;}


    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:607:4: BackslashSequence
    			{
    			DebugLocation(607, 4);
    			mBackslashSequence(); 

    			}
    			break;

    		}
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "RegularExpressionFirstChar"

    protected virtual void Enter_RegularExpressionChar() {}
    protected virtual void Leave_RegularExpressionChar() {}

    // $ANTLR start "RegularExpressionChar"
    [GrammarRule("RegularExpressionChar")]
    private void mRegularExpressionChar()
    {

    		try
    		{
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:611:2: (~ ( LineTerminator | BSLASH | DIV ) | BackslashSequence )
    		int alt29=2;
    		try { DebugEnterDecision(29, decisionCanBacktrack[29]);
    		int LA29_0 = input.LA(1);

    		if (((LA29_0>='\u0000' && LA29_0<='\t')||(LA29_0>='\u000B' && LA29_0<='\f')||(LA29_0>='\u000E' && LA29_0<='.')||(LA29_0>='0' && LA29_0<='[')||(LA29_0>=']' && LA29_0<='\u2027')||(LA29_0>='\u202A' && LA29_0<='\uFFFF')))
    		{
    			alt29=1;
    		}
    		else if ((LA29_0=='\\'))
    		{
    			alt29=2;
    		}
    		else
    		{
    			NoViableAltException nvae = new NoViableAltException("", 29, 0, input);

    			DebugRecognitionException(nvae);
    			throw nvae;
    		}
    		} finally { DebugExitDecision(29); }
    		switch (alt29)
    		{
    		case 1:
    			DebugEnterAlt(1);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:611:4: ~ ( LineTerminator | BSLASH | DIV )
    			{
    			DebugLocation(611, 4);
    			if ((input.LA(1)>='\u0000' && input.LA(1)<='\t')||(input.LA(1)>='\u000B' && input.LA(1)<='\f')||(input.LA(1)>='\u000E' && input.LA(1)<='.')||(input.LA(1)>='0' && input.LA(1)<='[')||(input.LA(1)>=']' && input.LA(1)<='\u2027')||(input.LA(1)>='\u202A' && input.LA(1)<='\uFFFF'))
    			{
    				input.Consume();

    			}
    			else
    			{
    				MismatchedSetException mse = new MismatchedSetException(null,input);
    				DebugRecognitionException(mse);
    				Recover(mse);
    				throw mse;}


    			}
    			break;
    		case 2:
    			DebugEnterAlt(2);
    			// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:612:4: BackslashSequence
    			{
    			DebugLocation(612, 4);
    			mBackslashSequence(); 

    			}
    			break;

    		}
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "RegularExpressionChar"

    protected virtual void Enter_RegularExpressionLiteral() {}
    protected virtual void Leave_RegularExpressionLiteral() {}

    // $ANTLR start "RegularExpressionLiteral"
    [GrammarRule("RegularExpressionLiteral")]
    private void mRegularExpressionLiteral()
    {

    		try
    		{
    		int _type = RegularExpressionLiteral;
    		int _channel = DefaultTokenChannel;
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:616:2: ({...}? => DIV RegularExpressionFirstChar ( RegularExpressionChar )* DIV ( IdentifierPart )* )
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:616:4: {...}? => DIV RegularExpressionFirstChar ( RegularExpressionChar )* DIV ( IdentifierPart )*
    		{
    		DebugLocation(616, 4);
    		if (!(( AreRegularExpressionsEnabled )))
    		{
    			throw new FailedPredicateException(input, "RegularExpressionLiteral", " AreRegularExpressionsEnabled ");
    		}
    		DebugLocation(616, 40);
    		mDIV(); 
    		DebugLocation(616, 44);
    		mRegularExpressionFirstChar(); 
    		DebugLocation(616, 71);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:616:71: ( RegularExpressionChar )*
    		try { DebugEnterSubRule(30);
    		while (true)
    		{
    			int alt30=2;
    			try { DebugEnterDecision(30, decisionCanBacktrack[30]);
    			int LA30_0 = input.LA(1);

    			if (((LA30_0>='\u0000' && LA30_0<='\t')||(LA30_0>='\u000B' && LA30_0<='\f')||(LA30_0>='\u000E' && LA30_0<='.')||(LA30_0>='0' && LA30_0<='\u2027')||(LA30_0>='\u202A' && LA30_0<='\uFFFF')))
    			{
    				alt30=1;
    			}


    			} finally { DebugExitDecision(30); }
    			switch ( alt30 )
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:616:71: RegularExpressionChar
    				{
    				DebugLocation(616, 71);
    				mRegularExpressionChar(); 

    				}
    				break;

    			default:
    				goto loop30;
    			}
    		}

    		loop30:
    			;

    		} finally { DebugExitSubRule(30); }

    		DebugLocation(616, 94);
    		mDIV(); 
    		DebugLocation(616, 98);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:616:98: ( IdentifierPart )*
    		try { DebugEnterSubRule(31);
    		while (true)
    		{
    			int alt31=2;
    			try { DebugEnterDecision(31, decisionCanBacktrack[31]);
    			int LA31_0 = input.LA(1);

    			if ((LA31_0=='$'||(LA31_0>='0' && LA31_0<='9')||(LA31_0>='A' && LA31_0<='Z')||LA31_0=='\\'||LA31_0=='_'||(LA31_0>='a' && LA31_0<='z')))
    			{
    				alt31=1;
    			}
    			else if ((( IsIdentifierPartUnicode(input.LA(1)) )))
    			{
    				alt31=1;
    			}


    			} finally { DebugExitDecision(31); }
    			switch ( alt31 )
    			{
    			case 1:
    				DebugEnterAlt(1);
    				// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:616:98: IdentifierPart
    				{
    				DebugLocation(616, 98);
    				mIdentifierPart(); 

    				}
    				break;

    			default:
    				goto loop31;
    			}
    		}

    		loop31:
    			;

    		} finally { DebugExitSubRule(31); }


    		}

    		state.type = _type;
    		state.channel = _channel;
    	}
    	finally
    	{
        }
    }
    // $ANTLR end "RegularExpressionLiteral"

    public override void mTokens()
    {
    	// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:8: ( NULL | TRUE | FALSE | BREAK | CASE | CATCH | CONTINUE | DEFAULT | DELETE | DO | ELSE | FINALLY | FOR | FUNCTION | IF | IN | INSTANCEOF | NEW | RETURN | SWITCH | THIS | THROW | TRY | TYPEOF | VAR | VOID | WHILE | WITH | ABSTRACT | BOOLEAN | BYTE | CHAR | CLASS | CONST | DEBUGGER | DOUBLE | ENUM | EXPORT | EXTENDS | FINAL | FLOAT | GOTO | IMPLEMENTS | IMPORT | INT | INTERFACE | LONG | NATIVE | PACKAGE | PRIVATE | PROTECTED | PUBLIC | SHORT | STATIC | SUPER | SYNCHRONIZED | THROWS | TRANSIENT | VOLATILE | LBRACE | RBRACE | LPAREN | RPAREN | LBRACK | RBRACK | DOT | SEMIC | COMMA | LT | GT | LTE | GTE | EQ | NEQ | SAME | NSAME | ADD | SUB | MUL | MOD | INC | DEC | SHL | SHR | SHU | AND | OR | XOR | NOT | INV | LAND | LOR | QUE | COLON | ASSIGN | ADDASS | SUBASS | MULASS | MODASS | SHLASS | SHRASS | SHUASS | ANDASS | ORASS | XORASS | DIV | DIVASS | WhiteSpace | EOL | MultiLineComment | SingleLineComment | Identifier | DecimalLiteral | OctalIntegerLiteral | HexIntegerLiteral | StringLiteral | RegularExpressionLiteral )
    	int alt32=117;
    	try { DebugEnterDecision(32, decisionCanBacktrack[32]);
    	try
    	{
    		alt32 = dfa32.Predict(input);
    	}
    	catch (NoViableAltException nvae)
    	{
    		DebugRecognitionException(nvae);
    		throw;
    	}
    	} finally { DebugExitDecision(32); }
    	switch (alt32)
    	{
    	case 1:
    		DebugEnterAlt(1);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:10: NULL
    		{
    		DebugLocation(1, 10);
    		mNULL(); 

    		}
    		break;
    	case 2:
    		DebugEnterAlt(2);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:15: TRUE
    		{
    		DebugLocation(1, 15);
    		mTRUE(); 

    		}
    		break;
    	case 3:
    		DebugEnterAlt(3);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:20: FALSE
    		{
    		DebugLocation(1, 20);
    		mFALSE(); 

    		}
    		break;
    	case 4:
    		DebugEnterAlt(4);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:26: BREAK
    		{
    		DebugLocation(1, 26);
    		mBREAK(); 

    		}
    		break;
    	case 5:
    		DebugEnterAlt(5);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:32: CASE
    		{
    		DebugLocation(1, 32);
    		mCASE(); 

    		}
    		break;
    	case 6:
    		DebugEnterAlt(6);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:37: CATCH
    		{
    		DebugLocation(1, 37);
    		mCATCH(); 

    		}
    		break;
    	case 7:
    		DebugEnterAlt(7);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:43: CONTINUE
    		{
    		DebugLocation(1, 43);
    		mCONTINUE(); 

    		}
    		break;
    	case 8:
    		DebugEnterAlt(8);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:52: DEFAULT
    		{
    		DebugLocation(1, 52);
    		mDEFAULT(); 

    		}
    		break;
    	case 9:
    		DebugEnterAlt(9);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:60: DELETE
    		{
    		DebugLocation(1, 60);
    		mDELETE(); 

    		}
    		break;
    	case 10:
    		DebugEnterAlt(10);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:67: DO
    		{
    		DebugLocation(1, 67);
    		mDO(); 

    		}
    		break;
    	case 11:
    		DebugEnterAlt(11);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:70: ELSE
    		{
    		DebugLocation(1, 70);
    		mELSE(); 

    		}
    		break;
    	case 12:
    		DebugEnterAlt(12);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:75: FINALLY
    		{
    		DebugLocation(1, 75);
    		mFINALLY(); 

    		}
    		break;
    	case 13:
    		DebugEnterAlt(13);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:83: FOR
    		{
    		DebugLocation(1, 83);
    		mFOR(); 

    		}
    		break;
    	case 14:
    		DebugEnterAlt(14);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:87: FUNCTION
    		{
    		DebugLocation(1, 87);
    		mFUNCTION(); 

    		}
    		break;
    	case 15:
    		DebugEnterAlt(15);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:96: IF
    		{
    		DebugLocation(1, 96);
    		mIF(); 

    		}
    		break;
    	case 16:
    		DebugEnterAlt(16);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:99: IN
    		{
    		DebugLocation(1, 99);
    		mIN(); 

    		}
    		break;
    	case 17:
    		DebugEnterAlt(17);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:102: INSTANCEOF
    		{
    		DebugLocation(1, 102);
    		mINSTANCEOF(); 

    		}
    		break;
    	case 18:
    		DebugEnterAlt(18);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:113: NEW
    		{
    		DebugLocation(1, 113);
    		mNEW(); 

    		}
    		break;
    	case 19:
    		DebugEnterAlt(19);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:117: RETURN
    		{
    		DebugLocation(1, 117);
    		mRETURN(); 

    		}
    		break;
    	case 20:
    		DebugEnterAlt(20);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:124: SWITCH
    		{
    		DebugLocation(1, 124);
    		mSWITCH(); 

    		}
    		break;
    	case 21:
    		DebugEnterAlt(21);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:131: THIS
    		{
    		DebugLocation(1, 131);
    		mTHIS(); 

    		}
    		break;
    	case 22:
    		DebugEnterAlt(22);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:136: THROW
    		{
    		DebugLocation(1, 136);
    		mTHROW(); 

    		}
    		break;
    	case 23:
    		DebugEnterAlt(23);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:142: TRY
    		{
    		DebugLocation(1, 142);
    		mTRY(); 

    		}
    		break;
    	case 24:
    		DebugEnterAlt(24);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:146: TYPEOF
    		{
    		DebugLocation(1, 146);
    		mTYPEOF(); 

    		}
    		break;
    	case 25:
    		DebugEnterAlt(25);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:153: VAR
    		{
    		DebugLocation(1, 153);
    		mVAR(); 

    		}
    		break;
    	case 26:
    		DebugEnterAlt(26);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:157: VOID
    		{
    		DebugLocation(1, 157);
    		mVOID(); 

    		}
    		break;
    	case 27:
    		DebugEnterAlt(27);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:162: WHILE
    		{
    		DebugLocation(1, 162);
    		mWHILE(); 

    		}
    		break;
    	case 28:
    		DebugEnterAlt(28);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:168: WITH
    		{
    		DebugLocation(1, 168);
    		mWITH(); 

    		}
    		break;
    	case 29:
    		DebugEnterAlt(29);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:173: ABSTRACT
    		{
    		DebugLocation(1, 173);
    		mABSTRACT(); 

    		}
    		break;
    	case 30:
    		DebugEnterAlt(30);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:182: BOOLEAN
    		{
    		DebugLocation(1, 182);
    		mBOOLEAN(); 

    		}
    		break;
    	case 31:
    		DebugEnterAlt(31);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:190: BYTE
    		{
    		DebugLocation(1, 190);
    		mBYTE(); 

    		}
    		break;
    	case 32:
    		DebugEnterAlt(32);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:195: CHAR
    		{
    		DebugLocation(1, 195);
    		mCHAR(); 

    		}
    		break;
    	case 33:
    		DebugEnterAlt(33);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:200: CLASS
    		{
    		DebugLocation(1, 200);
    		mCLASS(); 

    		}
    		break;
    	case 34:
    		DebugEnterAlt(34);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:206: CONST
    		{
    		DebugLocation(1, 206);
    		mCONST(); 

    		}
    		break;
    	case 35:
    		DebugEnterAlt(35);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:212: DEBUGGER
    		{
    		DebugLocation(1, 212);
    		mDEBUGGER(); 

    		}
    		break;
    	case 36:
    		DebugEnterAlt(36);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:221: DOUBLE
    		{
    		DebugLocation(1, 221);
    		mDOUBLE(); 

    		}
    		break;
    	case 37:
    		DebugEnterAlt(37);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:228: ENUM
    		{
    		DebugLocation(1, 228);
    		mENUM(); 

    		}
    		break;
    	case 38:
    		DebugEnterAlt(38);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:233: EXPORT
    		{
    		DebugLocation(1, 233);
    		mEXPORT(); 

    		}
    		break;
    	case 39:
    		DebugEnterAlt(39);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:240: EXTENDS
    		{
    		DebugLocation(1, 240);
    		mEXTENDS(); 

    		}
    		break;
    	case 40:
    		DebugEnterAlt(40);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:248: FINAL
    		{
    		DebugLocation(1, 248);
    		mFINAL(); 

    		}
    		break;
    	case 41:
    		DebugEnterAlt(41);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:254: FLOAT
    		{
    		DebugLocation(1, 254);
    		mFLOAT(); 

    		}
    		break;
    	case 42:
    		DebugEnterAlt(42);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:260: GOTO
    		{
    		DebugLocation(1, 260);
    		mGOTO(); 

    		}
    		break;
    	case 43:
    		DebugEnterAlt(43);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:265: IMPLEMENTS
    		{
    		DebugLocation(1, 265);
    		mIMPLEMENTS(); 

    		}
    		break;
    	case 44:
    		DebugEnterAlt(44);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:276: IMPORT
    		{
    		DebugLocation(1, 276);
    		mIMPORT(); 

    		}
    		break;
    	case 45:
    		DebugEnterAlt(45);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:283: INT
    		{
    		DebugLocation(1, 283);
    		mINT(); 

    		}
    		break;
    	case 46:
    		DebugEnterAlt(46);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:287: INTERFACE
    		{
    		DebugLocation(1, 287);
    		mINTERFACE(); 

    		}
    		break;
    	case 47:
    		DebugEnterAlt(47);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:297: LONG
    		{
    		DebugLocation(1, 297);
    		mLONG(); 

    		}
    		break;
    	case 48:
    		DebugEnterAlt(48);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:302: NATIVE
    		{
    		DebugLocation(1, 302);
    		mNATIVE(); 

    		}
    		break;
    	case 49:
    		DebugEnterAlt(49);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:309: PACKAGE
    		{
    		DebugLocation(1, 309);
    		mPACKAGE(); 

    		}
    		break;
    	case 50:
    		DebugEnterAlt(50);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:317: PRIVATE
    		{
    		DebugLocation(1, 317);
    		mPRIVATE(); 

    		}
    		break;
    	case 51:
    		DebugEnterAlt(51);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:325: PROTECTED
    		{
    		DebugLocation(1, 325);
    		mPROTECTED(); 

    		}
    		break;
    	case 52:
    		DebugEnterAlt(52);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:335: PUBLIC
    		{
    		DebugLocation(1, 335);
    		mPUBLIC(); 

    		}
    		break;
    	case 53:
    		DebugEnterAlt(53);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:342: SHORT
    		{
    		DebugLocation(1, 342);
    		mSHORT(); 

    		}
    		break;
    	case 54:
    		DebugEnterAlt(54);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:348: STATIC
    		{
    		DebugLocation(1, 348);
    		mSTATIC(); 

    		}
    		break;
    	case 55:
    		DebugEnterAlt(55);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:355: SUPER
    		{
    		DebugLocation(1, 355);
    		mSUPER(); 

    		}
    		break;
    	case 56:
    		DebugEnterAlt(56);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:361: SYNCHRONIZED
    		{
    		DebugLocation(1, 361);
    		mSYNCHRONIZED(); 

    		}
    		break;
    	case 57:
    		DebugEnterAlt(57);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:374: THROWS
    		{
    		DebugLocation(1, 374);
    		mTHROWS(); 

    		}
    		break;
    	case 58:
    		DebugEnterAlt(58);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:381: TRANSIENT
    		{
    		DebugLocation(1, 381);
    		mTRANSIENT(); 

    		}
    		break;
    	case 59:
    		DebugEnterAlt(59);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:391: VOLATILE
    		{
    		DebugLocation(1, 391);
    		mVOLATILE(); 

    		}
    		break;
    	case 60:
    		DebugEnterAlt(60);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:400: LBRACE
    		{
    		DebugLocation(1, 400);
    		mLBRACE(); 

    		}
    		break;
    	case 61:
    		DebugEnterAlt(61);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:407: RBRACE
    		{
    		DebugLocation(1, 407);
    		mRBRACE(); 

    		}
    		break;
    	case 62:
    		DebugEnterAlt(62);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:414: LPAREN
    		{
    		DebugLocation(1, 414);
    		mLPAREN(); 

    		}
    		break;
    	case 63:
    		DebugEnterAlt(63);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:421: RPAREN
    		{
    		DebugLocation(1, 421);
    		mRPAREN(); 

    		}
    		break;
    	case 64:
    		DebugEnterAlt(64);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:428: LBRACK
    		{
    		DebugLocation(1, 428);
    		mLBRACK(); 

    		}
    		break;
    	case 65:
    		DebugEnterAlt(65);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:435: RBRACK
    		{
    		DebugLocation(1, 435);
    		mRBRACK(); 

    		}
    		break;
    	case 66:
    		DebugEnterAlt(66);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:442: DOT
    		{
    		DebugLocation(1, 442);
    		mDOT(); 

    		}
    		break;
    	case 67:
    		DebugEnterAlt(67);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:446: SEMIC
    		{
    		DebugLocation(1, 446);
    		mSEMIC(); 

    		}
    		break;
    	case 68:
    		DebugEnterAlt(68);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:452: COMMA
    		{
    		DebugLocation(1, 452);
    		mCOMMA(); 

    		}
    		break;
    	case 69:
    		DebugEnterAlt(69);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:458: LT
    		{
    		DebugLocation(1, 458);
    		mLT(); 

    		}
    		break;
    	case 70:
    		DebugEnterAlt(70);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:461: GT
    		{
    		DebugLocation(1, 461);
    		mGT(); 

    		}
    		break;
    	case 71:
    		DebugEnterAlt(71);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:464: LTE
    		{
    		DebugLocation(1, 464);
    		mLTE(); 

    		}
    		break;
    	case 72:
    		DebugEnterAlt(72);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:468: GTE
    		{
    		DebugLocation(1, 468);
    		mGTE(); 

    		}
    		break;
    	case 73:
    		DebugEnterAlt(73);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:472: EQ
    		{
    		DebugLocation(1, 472);
    		mEQ(); 

    		}
    		break;
    	case 74:
    		DebugEnterAlt(74);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:475: NEQ
    		{
    		DebugLocation(1, 475);
    		mNEQ(); 

    		}
    		break;
    	case 75:
    		DebugEnterAlt(75);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:479: SAME
    		{
    		DebugLocation(1, 479);
    		mSAME(); 

    		}
    		break;
    	case 76:
    		DebugEnterAlt(76);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:484: NSAME
    		{
    		DebugLocation(1, 484);
    		mNSAME(); 

    		}
    		break;
    	case 77:
    		DebugEnterAlt(77);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:490: ADD
    		{
    		DebugLocation(1, 490);
    		mADD(); 

    		}
    		break;
    	case 78:
    		DebugEnterAlt(78);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:494: SUB
    		{
    		DebugLocation(1, 494);
    		mSUB(); 

    		}
    		break;
    	case 79:
    		DebugEnterAlt(79);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:498: MUL
    		{
    		DebugLocation(1, 498);
    		mMUL(); 

    		}
    		break;
    	case 80:
    		DebugEnterAlt(80);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:502: MOD
    		{
    		DebugLocation(1, 502);
    		mMOD(); 

    		}
    		break;
    	case 81:
    		DebugEnterAlt(81);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:506: INC
    		{
    		DebugLocation(1, 506);
    		mINC(); 

    		}
    		break;
    	case 82:
    		DebugEnterAlt(82);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:510: DEC
    		{
    		DebugLocation(1, 510);
    		mDEC(); 

    		}
    		break;
    	case 83:
    		DebugEnterAlt(83);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:514: SHL
    		{
    		DebugLocation(1, 514);
    		mSHL(); 

    		}
    		break;
    	case 84:
    		DebugEnterAlt(84);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:518: SHR
    		{
    		DebugLocation(1, 518);
    		mSHR(); 

    		}
    		break;
    	case 85:
    		DebugEnterAlt(85);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:522: SHU
    		{
    		DebugLocation(1, 522);
    		mSHU(); 

    		}
    		break;
    	case 86:
    		DebugEnterAlt(86);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:526: AND
    		{
    		DebugLocation(1, 526);
    		mAND(); 

    		}
    		break;
    	case 87:
    		DebugEnterAlt(87);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:530: OR
    		{
    		DebugLocation(1, 530);
    		mOR(); 

    		}
    		break;
    	case 88:
    		DebugEnterAlt(88);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:533: XOR
    		{
    		DebugLocation(1, 533);
    		mXOR(); 

    		}
    		break;
    	case 89:
    		DebugEnterAlt(89);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:537: NOT
    		{
    		DebugLocation(1, 537);
    		mNOT(); 

    		}
    		break;
    	case 90:
    		DebugEnterAlt(90);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:541: INV
    		{
    		DebugLocation(1, 541);
    		mINV(); 

    		}
    		break;
    	case 91:
    		DebugEnterAlt(91);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:545: LAND
    		{
    		DebugLocation(1, 545);
    		mLAND(); 

    		}
    		break;
    	case 92:
    		DebugEnterAlt(92);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:550: LOR
    		{
    		DebugLocation(1, 550);
    		mLOR(); 

    		}
    		break;
    	case 93:
    		DebugEnterAlt(93);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:554: QUE
    		{
    		DebugLocation(1, 554);
    		mQUE(); 

    		}
    		break;
    	case 94:
    		DebugEnterAlt(94);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:558: COLON
    		{
    		DebugLocation(1, 558);
    		mCOLON(); 

    		}
    		break;
    	case 95:
    		DebugEnterAlt(95);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:564: ASSIGN
    		{
    		DebugLocation(1, 564);
    		mASSIGN(); 

    		}
    		break;
    	case 96:
    		DebugEnterAlt(96);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:571: ADDASS
    		{
    		DebugLocation(1, 571);
    		mADDASS(); 

    		}
    		break;
    	case 97:
    		DebugEnterAlt(97);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:578: SUBASS
    		{
    		DebugLocation(1, 578);
    		mSUBASS(); 

    		}
    		break;
    	case 98:
    		DebugEnterAlt(98);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:585: MULASS
    		{
    		DebugLocation(1, 585);
    		mMULASS(); 

    		}
    		break;
    	case 99:
    		DebugEnterAlt(99);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:592: MODASS
    		{
    		DebugLocation(1, 592);
    		mMODASS(); 

    		}
    		break;
    	case 100:
    		DebugEnterAlt(100);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:599: SHLASS
    		{
    		DebugLocation(1, 599);
    		mSHLASS(); 

    		}
    		break;
    	case 101:
    		DebugEnterAlt(101);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:606: SHRASS
    		{
    		DebugLocation(1, 606);
    		mSHRASS(); 

    		}
    		break;
    	case 102:
    		DebugEnterAlt(102);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:613: SHUASS
    		{
    		DebugLocation(1, 613);
    		mSHUASS(); 

    		}
    		break;
    	case 103:
    		DebugEnterAlt(103);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:620: ANDASS
    		{
    		DebugLocation(1, 620);
    		mANDASS(); 

    		}
    		break;
    	case 104:
    		DebugEnterAlt(104);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:627: ORASS
    		{
    		DebugLocation(1, 627);
    		mORASS(); 

    		}
    		break;
    	case 105:
    		DebugEnterAlt(105);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:633: XORASS
    		{
    		DebugLocation(1, 633);
    		mXORASS(); 

    		}
    		break;
    	case 106:
    		DebugEnterAlt(106);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:640: DIV
    		{
    		DebugLocation(1, 640);
    		mDIV(); 

    		}
    		break;
    	case 107:
    		DebugEnterAlt(107);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:644: DIVASS
    		{
    		DebugLocation(1, 644);
    		mDIVASS(); 

    		}
    		break;
    	case 108:
    		DebugEnterAlt(108);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:651: WhiteSpace
    		{
    		DebugLocation(1, 651);
    		mWhiteSpace(); 

    		}
    		break;
    	case 109:
    		DebugEnterAlt(109);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:662: EOL
    		{
    		DebugLocation(1, 662);
    		mEOL(); 

    		}
    		break;
    	case 110:
    		DebugEnterAlt(110);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:666: MultiLineComment
    		{
    		DebugLocation(1, 666);
    		mMultiLineComment(); 

    		}
    		break;
    	case 111:
    		DebugEnterAlt(111);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:683: SingleLineComment
    		{
    		DebugLocation(1, 683);
    		mSingleLineComment(); 

    		}
    		break;
    	case 112:
    		DebugEnterAlt(112);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:701: Identifier
    		{
    		DebugLocation(1, 701);
    		mIdentifier(); 

    		}
    		break;
    	case 113:
    		DebugEnterAlt(113);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:712: DecimalLiteral
    		{
    		DebugLocation(1, 712);
    		mDecimalLiteral(); 

    		}
    		break;
    	case 114:
    		DebugEnterAlt(114);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:727: OctalIntegerLiteral
    		{
    		DebugLocation(1, 727);
    		mOctalIntegerLiteral(); 

    		}
    		break;
    	case 115:
    		DebugEnterAlt(115);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:747: HexIntegerLiteral
    		{
    		DebugLocation(1, 747);
    		mHexIntegerLiteral(); 

    		}
    		break;
    	case 116:
    		DebugEnterAlt(116);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:765: StringLiteral
    		{
    		DebugLocation(1, 765);
    		mStringLiteral(); 

    		}
    		break;
    	case 117:
    		DebugEnterAlt(117);
    		// D:\\downloads\\antlr\\ES3\\CSharp\\ES3.g3:1:779: RegularExpressionLiteral
    		{
    		DebugLocation(1, 779);
    		mRegularExpressionLiteral(); 

    		}
    		break;

    	}

    }


	#region DFA
	DFA19 dfa19;
	DFA32 dfa32;

	protected override void InitDFAs()
	{
		base.InitDFAs();
		dfa19 = new DFA19(this);
		dfa32 = new DFA32(this, SpecialStateTransition32);
	}

	private class DFA19 : DFA
	{
		private const string DFA19_eotS =
			"\x01\uffff\x02\x04\x03\uffff\x01\x04";
		private const string DFA19_eofS =
			"\x07\uffff";
		private const string DFA19_minS =
			"\x03\x2e\x03\uffff\x01\x2e";
		private const string DFA19_maxS =
			"\x01\x39\x01\x2e\x01\x39\x03\uffff\x01\x39";
		private const string DFA19_acceptS =
			"\x03\uffff\x01\x02\x01\x03\x01\x01\x01\uffff";
		private const string DFA19_specialS =
			"\x07\uffff}>";
		private static readonly string[] DFA19_transitionS =
			{
				"\x01\x03\x01\uffff\x01\x01\x09\x02",
				"\x01\x05",
				"\x01\x05\x01\uffff\x0a\x06",
				"",
				"",
				"",
				"\x01\x05\x01\uffff\x0a\x06"
			};

		private static readonly short[] DFA19_eot = DFA.UnpackEncodedString(DFA19_eotS);
		private static readonly short[] DFA19_eof = DFA.UnpackEncodedString(DFA19_eofS);
		private static readonly char[] DFA19_min = DFA.UnpackEncodedStringToUnsignedChars(DFA19_minS);
		private static readonly char[] DFA19_max = DFA.UnpackEncodedStringToUnsignedChars(DFA19_maxS);
		private static readonly short[] DFA19_accept = DFA.UnpackEncodedString(DFA19_acceptS);
		private static readonly short[] DFA19_special = DFA.UnpackEncodedString(DFA19_specialS);
		private static readonly short[][] DFA19_transition;

		static DFA19()
		{
			int numStates = DFA19_transitionS.Length;
			DFA19_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA19_transition[i] = DFA.UnpackEncodedString(DFA19_transitionS[i]);
			}
		}

		public DFA19( BaseRecognizer recognizer )
		{
			this.recognizer = recognizer;
			this.decisionNumber = 19;
			this.eot = DFA19_eot;
			this.eof = DFA19_eof;
			this.min = DFA19_min;
			this.max = DFA19_max;
			this.accept = DFA19_accept;
			this.special = DFA19_special;
			this.transition = DFA19_transition;
		}

		public override string Description { get { return "529:1: DecimalLiteral : ( DecimalIntegerLiteral '.' ( DecimalDigit )* ( ExponentPart )? | '.' ( DecimalDigit )+ ( ExponentPart )? | DecimalIntegerLiteral ( ExponentPart )? );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private class DFA32 : DFA
	{
		private const string DFA32_eotS =
			"\x11\x2b\x06\uffff\x01\x59\x02\uffff\x01\x5c\x01\x5f\x01\x61\x01\x63"+
			"\x01\x66\x01\x69\x01\x6b\x01\x6d\x01\x70\x01\x73\x01\x75\x03\uffff\x01"+
			"\x79\x03\uffff\x01\x2d\x02\uffff\x13\x2b\x01\u0097\x03\x2b\x01\u009c"+
			"\x01\u009f\x11\x2b\x02\uffff\x01\u00b4\x02\uffff\x01\u00b7\x01\uffff"+
			"\x01\u00b9\x01\uffff\x01\u00bb\x13\uffff\x01\u00bc\x06\uffff\x01\x2b"+
			"\x01\u00be\x02\x2b\x01\u00c1\x06\x2b\x01\u00c8\x0e\x2b\x01\uffff\x04"+
			"\x2b\x01\uffff\x01\x2b\x01\u00de\x01\uffff\x07\x2b\x01\u00e7\x0b\x2b"+
			"\x02\uffff\x01\u00f4\x07\uffff\x01\u00f5\x01\uffff\x01\x2b\x01\u00f7"+
			"\x01\uffff\x01\x2b\x01\u00f9\x04\x2b\x01\uffff\x04\x2b\x01\u0102\x01"+
			"\u0103\x03\x2b\x01\u0107\x05\x2b\x01\u010d\x01\u010e\x04\x2b\x01\uffff"+
			"\x08\x2b\x01\uffff\x01\u011b\x02\x2b\x01\u011e\x01\x2b\x01\u0120\x01"+
			"\u0121\x04\x2b\x03\uffff\x01\x2b\x01\uffff\x01\x2b\x01\uffff\x01\u0129"+
			"\x01\x2b\x01\u012b\x01\u012d\x01\x2b\x01\u012f\x01\u0130\x01\x2b\x02"+
			"\uffff\x01\u0132\x01\x2b\x01\u0134\x01\uffff\x01\u0135\x04\x2b\x02\uffff"+
			"\x08\x2b\x01\u0142\x01\x2b\x01\u0144\x01\x2b\x01\uffff\x01\x2b\x01\u0147"+
			"\x01\uffff\x01\x2b\x02\uffff\x04\x2b\x01\u014d\x01\x2b\x01\u014f\x01"+
			"\uffff\x01\u0150\x01\uffff\x01\x2b\x01\uffff\x01\x2b\x02\uffff\x01\x2b"+
			"\x01\uffff\x01\x2b\x02\uffff\x01\x2b\x01\u0156\x01\x2b\x01\u0158\x01"+
			"\u0159\x04\x2b\x01\u015e\x01\u015f\x01\u0160\x01\uffff\x01\u0161\x01"+
			"\uffff\x02\x2b\x01\uffff\x04\x2b\x01\u0168\x01\uffff\x01\x2b\x02\uffff"+
			"\x01\u016a\x01\x2b\x01\u016c\x01\x2b\x01\u016e\x01\uffff\x01\x2b\x02"+
			"\uffff\x01\u0170\x03\x2b\x04\uffff\x03\x2b\x01\u0177\x01\u0178\x01\x2b"+
			"\x01\uffff\x01\x2b\x01\uffff\x01\u017b\x01\uffff\x01\u017c\x01\uffff"+
			"\x01\u017d\x01\uffff\x04\x2b\x01\u0182\x01\u0183\x02\uffff\x01\x2b\x01"+
			"\u0185\x03\uffff\x01\x2b\x01\u0187\x02\x2b\x02\uffff\x01\u018a\x01\uffff"+
			"\x01\u018b\x01\uffff\x01\u018c\x01\x2b\x03\uffff\x01\x2b\x01\u018f\x01"+
			"\uffff";
		private const string DFA32_eofS =
			"\u0190\uffff";
		private const string DFA32_minS =
			"\x01\x09\x01\x61\x01\x68\x01\x61\x01\x6f\x01\x61\x01\x65\x01\x6c\x01"+
			"\x66\x01\x65\x01\x68\x01\x61\x01\x68\x01\x62\x02\x6f\x01\x61\x06\uffff"+
			"\x01\x30\x02\uffff\x01\x3c\x03\x3d\x01\x2b\x01\x2d\x02\x3d\x01\x26\x02"+
			"\x3d\x03\uffff\x01\x00\x03\uffff\x01\x30\x02\uffff\x01\x6c\x01\x77\x01"+
			"\x74\x01\x61\x01\x69\x01\x70\x01\x6c\x01\x6e\x01\x72\x01\x6e\x01\x6f"+
			"\x01\x65\x01\x6f\x01\x74\x01\x73\x01\x6e\x02\x61\x01\x62\x01\x24\x01"+
			"\x73\x01\x75\x01\x70\x02\x24\x01\x70\x01\x74\x01\x69\x01\x6f\x01\x61"+
			"\x01\x70\x01\x6e\x01\x72\x02\x69\x01\x74\x01\x73\x01\x74\x01\x6e\x01"+
			"\x63\x01\x69\x01\x62\x02\uffff\x01\x3d\x02\uffff\x01\x3d\x01\uffff\x01"+
			"\x3d\x01\uffff\x01\x3d\x13\uffff\x01\x00\x06\uffff\x01\x6c\x01\x24\x01"+
			"\x69\x01\x65\x01\x24\x01\x6e\x01\x73\x01\x6f\x01\x65\x01\x73\x01\x61"+
			"\x01\x24\x01\x63\x02\x61\x01\x6c\x02\x65\x01\x63\x01\x73\x01\x72\x01"+
			"\x73\x01\x61\x01\x65\x01\x75\x01\x62\x01\uffff\x01\x65\x01\x6d\x01\x6f"+
			"\x01\x65\x01\uffff\x01\x74\x01\x24\x01\uffff\x01\x6c\x01\x75\x01\x74"+
			"\x01\x72\x01\x74\x01\x65\x01\x63\x01\x24\x01\x64\x01\x61\x01\x6c\x01"+
			"\x68\x01\x74\x01\x6f\x01\x67\x01\x6b\x01\x76\x01\x74\x01\x6c\x02\uffff"+
			"\x01\x3d\x07\uffff\x01\x24\x01\uffff\x01\x76\x01\x24\x01\uffff\x01\x73"+
			"\x01\x24\x01\x77\x01\x6f\x01\x65\x01\x6c\x01\uffff\x02\x74\x01\x6b\x01"+
			"\x65\x02\x24\x01\x68\x01\x69\x01\x74\x01\x24\x01\x73\x01\x75\x01\x74"+
			"\x01\x67\x01\x6c\x02\x24\x01\x72\x01\x6e\x01\x61\x01\x72\x01\uffff\x01"+
			"\x65\x02\x72\x01\x63\x01\x74\x01\x69\x01\x72\x01\x68\x01\uffff\x01\x24"+
			"\x01\x74\x01\x65\x01\x24\x01\x72\x02\x24\x02\x61\x01\x65\x01\x69\x03"+
			"\uffff\x01\x65\x01\uffff\x01\x69\x01\uffff\x01\x24\x01\x66\x02\x24\x01"+
			"\x69\x02\x24\x01\x61\x02\uffff\x01\x24\x01\x6e\x01\x24\x01\uffff\x01"+
			"\x24\x01\x6c\x01\x65\x01\x67\x01\x65\x02\uffff\x01\x74\x01\x64\x01\x6e"+
			"\x01\x66\x01\x6d\x01\x74\x01\x6e\x01\x68\x01\x24\x01\x63\x01\x24\x01"+
			"\x72\x01\uffff\x01\x69\x01\x24\x01\uffff\x01\x61\x02\uffff\x01\x67\x01"+
			"\x74\x02\x63\x01\x24\x01\x65\x01\x24\x01\uffff\x01\x24\x01\uffff\x01"+
			"\x79\x01\uffff\x01\x6f\x02\uffff\x01\x6e\x01\uffff\x01\x75\x02\uffff"+
			"\x01\x74\x01\x24\x01\x65\x02\x24\x01\x73\x01\x63\x01\x61\x01\x65\x03"+
			"\x24\x01\uffff\x01\x24\x01\uffff\x01\x6f\x01\x6c\x01\uffff\x01\x63\x02"+
			"\x65\x01\x74\x01\x24\x01\uffff\x01\x6e\x02\uffff\x01\x24\x01\x6e\x01"+
			"\x24\x01\x65\x01\x24\x01\uffff\x01\x72\x02\uffff\x01\x24\x01\x65\x01"+
			"\x63\x01\x6e\x04\uffff\x01\x6e\x01\x65\x01\x74\x02\x24\x01\x65\x01\uffff"+
			"\x01\x74\x01\uffff\x01\x24\x01\uffff\x01\x24\x01\uffff\x01\x24\x01\uffff"+
			"\x01\x6f\x01\x65\x01\x74\x01\x69\x02\x24\x02\uffff\x01\x64\x01\x24\x03"+
			"\uffff\x01\x66\x01\x24\x01\x73\x01\x7a\x02\uffff\x01\x24\x01\uffff\x01"+
			"\x24\x01\uffff\x01\x24\x01\x65\x03\uffff\x01\x64\x01\x24\x01\uffff";
		private const string DFA32_maxS =
			"\x01\u3000\x01\x75\x01\x79\x01\x75\x01\x79\x02\x6f\x01\x78\x01\x6e\x01"+
			"\x65\x01\x79\x01\x6f\x01\x69\x01\x62\x02\x6f\x01\x75\x06\uffff\x01\x39"+
			"\x02\uffff\x01\x3d\x01\x3e\x07\x3d\x01\x7c\x01\x3d\x03\uffff\x01\uffff"+
			"\x03\uffff\x01\x78\x02\uffff\x01\x6c\x01\x77\x01\x74\x01\x79\x01\x72"+
			"\x01\x70\x01\x6c\x01\x6e\x01\x72\x01\x6e\x01\x6f\x01\x65\x01\x6f\x02"+
			"\x74\x01\x6e\x02\x61\x01\x6c\x01\x7a\x01\x73\x01\x75\x01\x74\x02\x7a"+
			"\x01\x70\x01\x74\x01\x69\x01\x6f\x01\x61\x01\x70\x01\x6e\x01\x72\x01"+
			"\x6c\x01\x69\x01\x74\x01\x73\x01\x74\x01\x6e\x01\x63\x01\x6f\x01\x62"+
			"\x02\uffff\x01\x3d\x02\uffff\x01\x3e\x01\uffff\x01\x3d\x01\uffff\x01"+
			"\x3d\x13\uffff\x01\uffff\x06\uffff\x01\x6c\x01\x7a\x01\x69\x01\x65\x01"+
			"\x7a\x01\x6e\x01\x73\x01\x6f\x01\x65\x01\x73\x01\x61\x01\x7a\x01\x63"+
			"\x02\x61\x01\x6c\x02\x65\x01\x63\x01\x74\x01\x72\x01\x73\x01\x61\x01"+
			"\x65\x01\x75\x01\x62\x01\uffff\x01\x65\x01\x6d\x01\x6f\x01\x65\x01\uffff"+
			"\x01\x74\x01\x7a\x01\uffff\x01\x6f\x01\x75\x01\x74\x01\x72\x01\x74\x01"+
			"\x65\x01\x63\x01\x7a\x01\x64\x01\x61\x01\x6c\x01\x68\x01\x74\x01\x6f"+
			"\x01\x67\x01\x6b\x01\x76\x01\x74\x01\x6c\x02\uffff\x01\x3d\x07\uffff"+
			"\x01\x7a\x01\uffff\x01\x76\x01\x7a\x01\uffff\x01\x73\x01\x7a\x01\x77"+
			"\x01\x6f\x01\x65\x01\x6c\x01\uffff\x02\x74\x01\x6b\x01\x65\x02\x7a\x01"+
			"\x68\x01\x69\x01\x74\x01\x7a\x01\x73\x01\x75\x01\x74\x01\x67\x01\x6c"+
			"\x02\x7a\x01\x72\x01\x6e\x01\x61\x01\x72\x01\uffff\x01\x65\x02\x72\x01"+
			"\x63\x01\x74\x01\x69\x01\x72\x01\x68\x01\uffff\x01\x7a\x01\x74\x01\x65"+
			"\x01\x7a\x01\x72\x02\x7a\x02\x61\x01\x65\x01\x69\x03\uffff\x01\x65\x01"+
			"\uffff\x01\x69\x01\uffff\x01\x7a\x01\x66\x02\x7a\x01\x69\x02\x7a\x01"+
			"\x61\x02\uffff\x01\x7a\x01\x6e\x01\x7a\x01\uffff\x01\x7a\x01\x6c\x01"+
			"\x65\x01\x67\x01\x65\x02\uffff\x01\x74\x01\x64\x01\x6e\x01\x66\x01\x6d"+
			"\x01\x74\x01\x6e\x01\x68\x01\x7a\x01\x63\x01\x7a\x01\x72\x01\uffff\x01"+
			"\x69\x01\x7a\x01\uffff\x01\x61\x02\uffff\x01\x67\x01\x74\x02\x63\x01"+
			"\x7a\x01\x65\x01\x7a\x01\uffff\x01\x7a\x01\uffff\x01\x79\x01\uffff\x01"+
			"\x6f\x02\uffff\x01\x6e\x01\uffff\x01\x75\x02\uffff\x01\x74\x01\x7a\x01"+
			"\x65\x02\x7a\x01\x73\x01\x63\x01\x61\x01\x65\x03\x7a\x01\uffff\x01\x7a"+
			"\x01\uffff\x01\x6f\x01\x6c\x01\uffff\x01\x63\x02\x65\x01\x74\x01\x7a"+
			"\x01\uffff\x01\x6e\x02\uffff\x01\x7a\x01\x6e\x01\x7a\x01\x65\x01\x7a"+
			"\x01\uffff\x01\x72\x02\uffff\x01\x7a\x01\x65\x01\x63\x01\x6e\x04\uffff"+
			"\x01\x6e\x01\x65\x01\x74\x02\x7a\x01\x65\x01\uffff\x01\x74\x01\uffff"+
			"\x01\x7a\x01\uffff\x01\x7a\x01\uffff\x01\x7a\x01\uffff\x01\x6f\x01\x65"+
			"\x01\x74\x01\x69\x02\x7a\x02\uffff\x01\x64\x01\x7a\x03\uffff\x01\x66"+
			"\x01\x7a\x01\x73\x01\x7a\x02\uffff\x01\x7a\x01\uffff\x01\x7a\x01\uffff"+
			"\x01\x7a\x01\x65\x03\uffff\x01\x64\x01\x7a\x01\uffff";
		private const string DFA32_acceptS =
			"\x11\uffff\x01\x3c\x01\x3d\x01\x3e\x01\x3f\x01\x40\x01\x41\x01\uffff"+
			"\x01\x43\x01\x44\x0b\uffff\x01\x5a\x01\x5d\x01\x5e\x01\uffff\x01\x6c"+
			"\x01\x6d\x01\x70\x01\uffff\x01\x71\x01\x74\x2a\uffff\x01\x42\x01\x47"+
			"\x01\uffff\x01\x45\x01\x48\x01\uffff\x01\x46\x01\uffff\x01\x5f\x01\uffff"+
			"\x01\x59\x01\x51\x01\x60\x01\x4d\x01\x52\x01\x61\x01\x4e\x01\x62\x01"+
			"\x4f\x01\x63\x01\x50\x01\x5b\x01\x67\x01\x56\x01\x5c\x01\x68\x01\x57"+
			"\x01\x69\x01\x58\x01\uffff\x01\x6e\x01\x6f\x01\x6a\x01\x75\x01\x73\x01"+
			"\x72\x1a\uffff\x01\x0a\x04\uffff\x01\x0f\x02\uffff\x01\x10\x13\uffff"+
			"\x01\x64\x01\x53\x01\uffff\x01\x65\x01\x54\x01\x4b\x01\x49\x01\x4c\x01"+
			"\x4a\x01\x6b\x01\uffff\x01\x12\x02\uffff\x01\x17\x06\uffff\x01\x0d\x15"+
			"\uffff\x01\x2d\x08\uffff\x01\x19\x0b\uffff\x01\x66\x01\x55\x01\x01\x01"+
			"\uffff\x01\x02\x01\uffff\x01\x15\x08\uffff\x01\x1f\x01\x05\x03\uffff"+
			"\x01\x20\x05\uffff\x01\x0b\x01\x25\x0c\uffff\x01\x1a\x02\uffff\x01\x1c"+
			"\x01\uffff\x01\x2a\x01\x2f\x07\uffff\x01\x16\x01\uffff\x01\x03\x01\uffff"+
			"\x01\x28\x01\uffff\x01\x29\x01\x04\x01\uffff\x01\x06\x01\uffff\x01\x22"+
			"\x01\x21\x0c\uffff\x01\x35\x01\uffff\x01\x37\x02\uffff\x01\x1b\x05\uffff"+
			"\x01\x30\x01\uffff\x01\x39\x01\x18\x05\uffff\x01\x09\x01\uffff\x01\x24"+
			"\x01\x26\x04\uffff\x01\x2c\x01\x13\x01\x14\x01\x36\x06\uffff\x01\x34"+
			"\x01\uffff\x01\x0c\x01\uffff\x01\x1e\x01\uffff\x01\x08\x01\uffff\x01"+
			"\x27\x06\uffff\x01\x31\x01\x32\x02\uffff\x01\x0e\x01\x07\x01\x23\x04"+
			"\uffff\x01\x3b\x01\x1d\x01\uffff\x01\x3a\x01\uffff\x01\x2e\x02\uffff"+
			"\x01\x33\x01\x11\x01\x2b\x02\uffff\x01\x38";
		private const string DFA32_specialS =
			"\x28\uffff\x01\x00\x4d\uffff\x01\x01\u0119\uffff}>";
		private static readonly string[] DFA32_transitionS =
			{
				"\x01\x29\x01\x2a\x02\x29\x01\x2a\x12\uffff\x01\x29\x01\x1d\x01\x2e"+
				"\x02\uffff\x01\x21\x01\x22\x01\x2e\x01\x13\x01\x14\x01\x20\x01\x1e\x01"+
				"\x19\x01\x1f\x01\x17\x01\x28\x01\x2c\x09\x2d\x01\x27\x01\x18\x01\x1a"+
				"\x01\x1c\x01\x1b\x01\x26\x1b\uffff\x01\x15\x01\uffff\x01\x16\x01\x24"+
				"\x02\uffff\x01\x0d\x01\x04\x01\x05\x01\x06\x01\x07\x01\x03\x01\x0e\x01"+
				"\uffff\x01\x08\x02\uffff\x01\x0f\x01\uffff\x01\x01\x01\uffff\x01\x10"+
				"\x01\uffff\x01\x09\x01\x0a\x01\x02\x01\uffff\x01\x0b\x01\x0c\x03\uffff"+
				"\x01\x11\x01\x23\x01\x12\x01\x25\x21\uffff\x01\x29\u15df\uffff\x01\x29"+
				"\u018d\uffff\x01\x29\u07f1\uffff\x0b\x29\x1d\uffff\x02\x2a\x05\uffff"+
				"\x01\x29\x2f\uffff\x01\x29\u0fa0\uffff\x01\x29",
				"\x01\x31\x03\uffff\x01\x30\x0f\uffff\x01\x2f",
				"\x01\x33\x09\uffff\x01\x32\x06\uffff\x01\x34",
				"\x01\x35\x07\uffff\x01\x36\x02\uffff\x01\x39\x02\uffff\x01\x37\x05"+
				"\uffff\x01\x38",
				"\x01\x3b\x02\uffff\x01\x3a\x06\uffff\x01\x3c",
				"\x01\x3d\x06\uffff\x01\x3f\x03\uffff\x01\x40\x02\uffff\x01\x3e",
				"\x01\x41\x09\uffff\x01\x42",
				"\x01\x43\x01\uffff\x01\x44\x09\uffff\x01\x45",
				"\x01\x46\x06\uffff\x01\x48\x01\x47",
				"\x01\x49",
				"\x01\x4b\x0b\uffff\x01\x4c\x01\x4d\x01\uffff\x01\x4a\x01\uffff\x01"+
				"\x4e",
				"\x01\x4f\x0d\uffff\x01\x50",
				"\x01\x51\x01\x52",
				"\x01\x53",
				"\x01\x54",
				"\x01\x55",
				"\x01\x56\x10\uffff\x01\x57\x02\uffff\x01\x58",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x0a\x2d",
				"",
				"",
				"\x01\x5b\x01\x5a",
				"\x01\x5d\x01\x5e",
				"\x01\x60",
				"\x01\x62",
				"\x01\x64\x11\uffff\x01\x65",
				"\x01\x67\x0f\uffff\x01\x68",
				"\x01\x6a",
				"\x01\x6c",
				"\x01\x6e\x16\uffff\x01\x6f",
				"\x01\x72\x3e\uffff\x01\x71",
				"\x01\x74",
				"",
				"",
				"",
				"\x0a\x7a\x01\uffff\x02\x7a\x01\uffff\x1c\x7a\x01\x77\x04\x7a\x01\x78"+
				"\x0d\x7a\x01\x76\u1fea\x7a\x02\uffff\udfd6\x7a",
				"",
				"",
				"",
				"\x08\x7c\x20\uffff\x01\x7b\x1f\uffff\x01\x7b",
				"",
				"",
				"\x01\x7d",
				"\x01\x7e",
				"\x01\x7f",
				"\x01\u0082\x13\uffff\x01\u0080\x03\uffff\x01\u0081",
				"\x01\u0083\x08\uffff\x01\u0084",
				"\x01\u0085",
				"\x01\u0086",
				"\x01\u0087",
				"\x01\u0088",
				"\x01\u0089",
				"\x01\u008a",
				"\x01\u008b",
				"\x01\u008c",
				"\x01\u008d",
				"\x01\u008e\x01\u008f",
				"\x01\u0090",
				"\x01\u0091",
				"\x01\u0092",
				"\x01\u0095\x03\uffff\x01\u0093\x05\uffff\x01\u0094",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x14\x2b\x01\u0096\x05\x2b",
				"\x01\u0098",
				"\x01\u0099",
				"\x01\u009a\x03\uffff\x01\u009b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x12\x2b\x01\u009d\x01\u009e\x06\x2b",
				"\x01\u00a0",
				"\x01\u00a1",
				"\x01\u00a2",
				"\x01\u00a3",
				"\x01\u00a4",
				"\x01\u00a5",
				"\x01\u00a6",
				"\x01\u00a7",
				"\x01\u00a8\x02\uffff\x01\u00a9",
				"\x01\u00aa",
				"\x01\u00ab",
				"\x01\u00ac",
				"\x01\u00ad",
				"\x01\u00ae",
				"\x01\u00af",
				"\x01\u00b0\x05\uffff\x01\u00b1",
				"\x01\u00b2",
				"",
				"",
				"\x01\u00b3",
				"",
				"",
				"\x01\u00b6\x01\u00b5",
				"",
				"\x01\u00b8",
				"",
				"\x01\u00ba",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x0a\x7a\x01\uffff\x02\x7a\x01\uffff\u201a\x7a\x02\uffff\udfd6\x7a",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x01\u00bd",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u00bf",
				"\x01\u00c0",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u00c2",
				"\x01\u00c3",
				"\x01\u00c4",
				"\x01\u00c5",
				"\x01\u00c6",
				"\x01\u00c7",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u00c9",
				"\x01\u00ca",
				"\x01\u00cb",
				"\x01\u00cc",
				"\x01\u00cd",
				"\x01\u00ce",
				"\x01\u00cf",
				"\x01\u00d1\x01\u00d0",
				"\x01\u00d2",
				"\x01\u00d3",
				"\x01\u00d4",
				"\x01\u00d5",
				"\x01\u00d6",
				"\x01\u00d7",
				"",
				"\x01\u00d8",
				"\x01\u00d9",
				"\x01\u00da",
				"\x01\u00db",
				"",
				"\x01\u00dc",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x04\x2b\x01\u00dd\x15\x2b",
				"",
				"\x01\u00df\x02\uffff\x01\u00e0",
				"\x01\u00e1",
				"\x01\u00e2",
				"\x01\u00e3",
				"\x01\u00e4",
				"\x01\u00e5",
				"\x01\u00e6",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u00e8",
				"\x01\u00e9",
				"\x01\u00ea",
				"\x01\u00eb",
				"\x01\u00ec",
				"\x01\u00ed",
				"\x01\u00ee",
				"\x01\u00ef",
				"\x01\u00f0",
				"\x01\u00f1",
				"\x01\u00f2",
				"",
				"",
				"\x01\u00f3",
				"",
				"",
				"",
				"",
				"",
				"",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\u00f6",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\u00f8",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u00fa",
				"\x01\u00fb",
				"\x01\u00fc",
				"\x01\u00fd",
				"",
				"\x01\u00fe",
				"\x01\u00ff",
				"\x01\u0100",
				"\x01\u0101",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0104",
				"\x01\u0105",
				"\x01\u0106",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0108",
				"\x01\u0109",
				"\x01\u010a",
				"\x01\u010b",
				"\x01\u010c",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u010f",
				"\x01\u0110",
				"\x01\u0111",
				"\x01\u0112",
				"",
				"\x01\u0113",
				"\x01\u0114",
				"\x01\u0115",
				"\x01\u0116",
				"\x01\u0117",
				"\x01\u0118",
				"\x01\u0119",
				"\x01\u011a",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u011c",
				"\x01\u011d",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u011f",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0122",
				"\x01\u0123",
				"\x01\u0124",
				"\x01\u0125",
				"",
				"",
				"",
				"\x01\u0126",
				"",
				"\x01\u0127",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x12\x2b\x01\u0128\x07\x2b",
				"\x01\u012a",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x0b\x2b\x01\u012c\x0e\x2b",
				"\x01\u012e",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0131",
				"",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0133",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0136",
				"\x01\u0137",
				"\x01\u0138",
				"\x01\u0139",
				"",
				"",
				"\x01\u013a",
				"\x01\u013b",
				"\x01\u013c",
				"\x01\u013d",
				"\x01\u013e",
				"\x01\u013f",
				"\x01\u0140",
				"\x01\u0141",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0143",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0145",
				"",
				"\x01\u0146",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\u0148",
				"",
				"",
				"\x01\u0149",
				"\x01\u014a",
				"\x01\u014b",
				"\x01\u014c",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u014e",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\u0151",
				"",
				"\x01\u0152",
				"",
				"",
				"\x01\u0153",
				"",
				"\x01\u0154",
				"",
				"",
				"\x01\u0155",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0157",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u015a",
				"\x01\u015b",
				"\x01\u015c",
				"\x01\u015d",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\u0162",
				"\x01\u0163",
				"",
				"\x01\u0164",
				"\x01\u0165",
				"\x01\u0166",
				"\x01\u0167",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\u0169",
				"",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u016b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u016d",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\u016f",
				"",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0171",
				"\x01\u0172",
				"\x01\u0173",
				"",
				"",
				"",
				"",
				"\x01\u0174",
				"\x01\u0175",
				"\x01\u0176",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0179",
				"",
				"\x01\u017a",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\u017e",
				"\x01\u017f",
				"\x01\u0180",
				"\x01\u0181",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"",
				"\x01\u0184",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"",
				"",
				"\x01\u0186",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u0188",
				"\x01\u0189",
				"",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				"\x01\u018d",
				"",
				"",
				"",
				"\x01\u018e",
				"\x01\x2b\x0b\uffff\x0a\x2b\x07\uffff\x1a\x2b\x01\uffff\x01\x2b\x02"+
				"\uffff\x01\x2b\x01\uffff\x1a\x2b",
				""
			};

		private static readonly short[] DFA32_eot = DFA.UnpackEncodedString(DFA32_eotS);
		private static readonly short[] DFA32_eof = DFA.UnpackEncodedString(DFA32_eofS);
		private static readonly char[] DFA32_min = DFA.UnpackEncodedStringToUnsignedChars(DFA32_minS);
		private static readonly char[] DFA32_max = DFA.UnpackEncodedStringToUnsignedChars(DFA32_maxS);
		private static readonly short[] DFA32_accept = DFA.UnpackEncodedString(DFA32_acceptS);
		private static readonly short[] DFA32_special = DFA.UnpackEncodedString(DFA32_specialS);
		private static readonly short[][] DFA32_transition;

		static DFA32()
		{
			int numStates = DFA32_transitionS.Length;
			DFA32_transition = new short[numStates][];
			for ( int i=0; i < numStates; i++ )
			{
				DFA32_transition[i] = DFA.UnpackEncodedString(DFA32_transitionS[i]);
			}
		}

		public DFA32( BaseRecognizer recognizer, SpecialStateTransitionHandler specialStateTransition )
			: base(specialStateTransition)
		{
			this.recognizer = recognizer;
			this.decisionNumber = 32;
			this.eot = DFA32_eot;
			this.eof = DFA32_eof;
			this.min = DFA32_min;
			this.max = DFA32_max;
			this.accept = DFA32_accept;
			this.special = DFA32_special;
			this.transition = DFA32_transition;
		}

		public override string Description { get { return "1:1: Tokens : ( NULL | TRUE | FALSE | BREAK | CASE | CATCH | CONTINUE | DEFAULT | DELETE | DO | ELSE | FINALLY | FOR | FUNCTION | IF | IN | INSTANCEOF | NEW | RETURN | SWITCH | THIS | THROW | TRY | TYPEOF | VAR | VOID | WHILE | WITH | ABSTRACT | BOOLEAN | BYTE | CHAR | CLASS | CONST | DEBUGGER | DOUBLE | ENUM | EXPORT | EXTENDS | FINAL | FLOAT | GOTO | IMPLEMENTS | IMPORT | INT | INTERFACE | LONG | NATIVE | PACKAGE | PRIVATE | PROTECTED | PUBLIC | SHORT | STATIC | SUPER | SYNCHRONIZED | THROWS | TRANSIENT | VOLATILE | LBRACE | RBRACE | LPAREN | RPAREN | LBRACK | RBRACK | DOT | SEMIC | COMMA | LT | GT | LTE | GTE | EQ | NEQ | SAME | NSAME | ADD | SUB | MUL | MOD | INC | DEC | SHL | SHR | SHU | AND | OR | XOR | NOT | INV | LAND | LOR | QUE | COLON | ASSIGN | ADDASS | SUBASS | MULASS | MODASS | SHLASS | SHRASS | SHUASS | ANDASS | ORASS | XORASS | DIV | DIVASS | WhiteSpace | EOL | MultiLineComment | SingleLineComment | Identifier | DecimalLiteral | OctalIntegerLiteral | HexIntegerLiteral | StringLiteral | RegularExpressionLiteral );"; } }

		public override void Error(NoViableAltException nvae)
		{
			DebugRecognitionException(nvae);
		}
	}

	private int SpecialStateTransition32(DFA dfa, int s, IIntStream _input)
	{
		IIntStream input = _input;
		int _s = s;
		switch (s)
		{
			case 0:
				int LA32_40 = input.LA(1);


				int index32_40 = input.Index;
				input.Rewind();
				s = -1;
				if ( (LA32_40=='=') ) {s = 118;}

				else if ( (LA32_40=='*') ) {s = 119;}

				else if ( (LA32_40=='/') ) {s = 120;}

				else if ( ((LA32_40>='\u0000' && LA32_40<='\t')||(LA32_40>='\u000B' && LA32_40<='\f')||(LA32_40>='\u000E' && LA32_40<=')')||(LA32_40>='+' && LA32_40<='.')||(LA32_40>='0' && LA32_40<='<')||(LA32_40>='>' && LA32_40<='\u2027')||(LA32_40>='\u202A' && LA32_40<='\uFFFF')) && (( AreRegularExpressionsEnabled ))) {s = 122;}

				else s = 121;


				input.Seek(index32_40);
				if ( s>=0 ) return s;
				break;
			case 1:
				int LA32_118 = input.LA(1);


				int index32_118 = input.Index;
				input.Rewind();
				s = -1;
				if ( ((LA32_118>='\u0000' && LA32_118<='\t')||(LA32_118>='\u000B' && LA32_118<='\f')||(LA32_118>='\u000E' && LA32_118<='\u2027')||(LA32_118>='\u202A' && LA32_118<='\uFFFF')) && (( AreRegularExpressionsEnabled ))) {s = 122;}

				else s = 188;


				input.Seek(index32_118);
				if ( s>=0 ) return s;
				break;
		}
		NoViableAltException nvae = new NoViableAltException(dfa.Description, 32, _s, input);
		dfa.Error(nvae);
		throw nvae;
	}
 
	#endregion

}
}