using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;



using System;
using System.Collections.Generic;

namespace ICSharpCode.NRefactory.Parser.VBNet.Experimental {



partial class ExpressionFinder {

	const bool T = true;
	const bool x = false;

int currentState = 1;

	readonly Stack<int> stateStack = new Stack<int>();
	bool nextTokenIsPotentialStartOfXmlMode = false;
	
	public ExpressionFinder()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token t)
	{
		if (t.kind != expectedKind)
			Error(t);
	}
	
	void Error(Token t) 
	{
	}

	public void InformToken(Token t) 
	{
		nextTokenIsPotentialStartOfXmlMode = false;
		switchlbl: switch (currentState) {
			case 0: {
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 1: { // start of ExpressionFinder
				PushContext(Context.Global); 
				goto case 3;
			}
			case 2: {
				stateStack.Push(3);
				goto case 15; // OptionStatement
			}
			case 3: {
				if (t.kind == 159) {
					goto case 2;
				} else {
					goto case 5;
				}
			}
			case 4: {
				stateStack.Push(5);
				goto case 19; // ImportsStatement
			}
			case 5: {
				if (t.kind == 124) {
					goto case 4;
				} else {
					goto case 7;
				}
			}
			case 6: {
				stateStack.Push(7);
				goto case 24; // AttributeBlock
			}
			case 7: {
				if (t.kind == 28) {
					goto case 6;
				} else {
					goto case 9;
				}
			}
			case 8: {
				stateStack.Push(9);
				goto case 32; // NamespaceMemberDeclaration
			}
			case 9: {
				if (set[0, t.kind]) {
					goto case 8;
				} else {
					goto case 10;
				}
			}
			case 10: {
				PopContext(); 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 11: {
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 12: {
				Expect(11, t); // ":"
				currentState = stateStack.Pop();
				break;
			}
			case 13: { // start of StatementTerminator
				if (t.kind == 1) {
					goto case 11;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (t.kind == 11) {
					goto case 12;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 15: { // start of OptionStatement
				Expect(159, t); // "Option"
				currentState = 17;
				break;
			}
			case 16: {
				currentState = 17;
				break;
			}
			case 17: {
				if (set[1, t.kind]) {
					goto case 16;
				} else {
					goto case 18;
				}
			}
			case 18: {
				goto case 13; // StatementTerminator
			}
			case 19: { // start of ImportsStatement
				Expect(124, t); // "Imports"
				currentState = 20;
				break;
			}
			case 20: {
				nextTokenIsPotentialStartOfXmlMode = true; 
				goto case 22;
			}
			case 21: {
				currentState = 22;
				break;
			}
			case 22: {
				if (set[1, t.kind]) {
					goto case 21;
				} else {
					goto case 23;
				}
			}
			case 23: {
				goto case 13; // StatementTerminator
			}
			case 24: { // start of AttributeBlock
				Expect(28, t); // "<"
				currentState = 26;
				break;
			}
			case 25: {
				currentState = 26;
				break;
			}
			case 26: {
				if (set[2, t.kind]) {
					goto case 25;
				} else {
					goto case 27;
				}
			}
			case 27: {
				Expect(27, t); // ">"
				currentState = 29;
				break;
			}
			case 28: {
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 29: {
				if (t.kind == 1) {
					goto case 28;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 30: {
				goto case 34; // NamespaceDeclaration
			}
			case 31: {
				goto case 44; // TypeDeclaration
			}
			case 32: { // start of NamespaceMemberDeclaration
				if (t.kind == 146) {
					goto case 30;
				} else {
					goto case 33;
				}
			}
			case 33: {
				if (set[3, t.kind]) {
					goto case 31;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 34: { // start of NamespaceDeclaration
				Expect(146, t); // "Namespace"
				currentState = 36;
				break;
			}
			case 35: {
				currentState = 36;
				break;
			}
			case 36: {
				if (set[1, t.kind]) {
					goto case 35;
				} else {
					goto case 37;
				}
			}
			case 37: {
				stateStack.Push(39);
				goto case 13; // StatementTerminator
			}
			case 38: {
				stateStack.Push(39);
				goto case 32; // NamespaceMemberDeclaration
			}
			case 39: {
				if (set[4, t.kind]) {
					goto case 38;
				} else {
					goto case 40;
				}
			}
			case 40: {
				Expect(100, t); // "End"
				currentState = 41;
				break;
			}
			case 41: {
				Expect(146, t); // "Namespace"
				currentState = 42;
				break;
			}
			case 42: {
				goto case 13; // StatementTerminator
			}
			case 43: {
				stateStack.Push(44);
				goto case 24; // AttributeBlock
			}
			case 44: { // start of TypeDeclaration
				if (t.kind == 28) {
					goto case 43;
				} else {
					goto case 46;
				}
			}
			case 45: {
				stateStack.Push(46);
				goto case 338; // TypeModifier
			}
			case 46: {
				if (set[5, t.kind]) {
					goto case 45;
				} else {
					goto case 49;
				}
			}
			case 47: {
				Expect(141, t); // "Module"
				currentState = 52;
				break;
			}
			case 48: {
				Expect(71, t); // "Class"
				currentState = 52;
				break;
			}
			case 49: {
				if (t.kind == 141) {
					goto case 47;
				} else {
					goto case 50;
				}
			}
			case 50: {
				if (t.kind == 71) {
					goto case 48;
				} else {
					Error(t);
					goto case 52;
				}
			}
			case 51: {
				currentState = 52;
				break;
			}
			case 52: {
				if (set[1, t.kind]) {
					goto case 51;
				} else {
					goto case 53;
				}
			}
			case 53: {
				stateStack.Push(54);
				goto case 13; // StatementTerminator
			}
			case 54: {
				PushContext(Context.Type); 
				goto case 56;
			}
			case 55: {
				stateStack.Push(56);
				goto case 64; // MemberDeclaration
			}
			case 56: {
				if (set[6, t.kind]) {
					goto case 55;
				} else {
					goto case 57;
				}
			}
			case 57: {
				Expect(100, t); // "End"
				currentState = 60;
				break;
			}
			case 58: {
				Expect(141, t); // "Module"
				currentState = 62;
				break;
			}
			case 59: {
				Expect(71, t); // "Class"
				currentState = 62;
				break;
			}
			case 60: {
				if (t.kind == 141) {
					goto case 58;
				} else {
					goto case 61;
				}
			}
			case 61: {
				if (t.kind == 71) {
					goto case 59;
				} else {
					Error(t);
					goto case 62;
				}
			}
			case 62: {
				stateStack.Push(63);
				goto case 13; // StatementTerminator
			}
			case 63: {
				PopContext(); 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 64: { // start of MemberDeclaration
				PushContext(Context.Member); 
				goto case 65;
			}
			case 65: {
				stateStack.Push(66);
				goto case 68; // SubOrFunctionDeclaration
			}
			case 66: {
				PopContext(); 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 67: {
				stateStack.Push(68);
				goto case 24; // AttributeBlock
			}
			case 68: { // start of SubOrFunctionDeclaration
				if (t.kind == 28) {
					goto case 67;
				} else {
					goto case 70;
				}
			}
			case 69: {
				stateStack.Push(70);
				goto case 342; // MemberModifier
			}
			case 70: {
				if (set[7, t.kind]) {
					goto case 69;
				} else {
					goto case 73;
				}
			}
			case 71: {
				Expect(195, t); // "Sub"
				currentState = 75;
				break;
			}
			case 72: {
				Expect(114, t); // "Function"
				currentState = 75;
				break;
			}
			case 73: {
				if (t.kind == 195) {
					goto case 71;
				} else {
					goto case 74;
				}
			}
			case 74: {
				if (t.kind == 114) {
					goto case 72;
				} else {
					Error(t);
					goto case 75;
				}
			}
			case 75: {
				PushContext(Context.IdentifierExpected); 
				goto case 76;
			}
			case 76: {
				currentState = 77;
				break;
			}
			case 77: {
				PopContext(); 
				goto case 82;
			}
			case 78: {
				Expect(25, t); // "("
				currentState = 80;
				break;
			}
			case 79: {
				stateStack.Push(81);
				goto case 93; // ParameterList
			}
			case 80: {
				if (set[8, t.kind]) {
					goto case 79;
				} else {
					goto case 81;
				}
			}
			case 81: {
				Expect(26, t); // ")"
				currentState = 85;
				break;
			}
			case 82: {
				if (t.kind == 25) {
					goto case 78;
				} else {
					goto case 85;
				}
			}
			case 83: {
				Expect(50, t); // "As"
				currentState = 84;
				break;
			}
			case 84: {
				stateStack.Push(86);
				goto case 177; // TypeName
			}
			case 85: {
				if (t.kind == 50) {
					goto case 83;
				} else {
					goto case 86;
				}
			}
			case 86: {
				stateStack.Push(87);
				goto case 108; // Block
			}
			case 87: {
				Expect(100, t); // "End"
				currentState = 90;
				break;
			}
			case 88: {
				Expect(195, t); // "Sub"
				currentState = 92;
				break;
			}
			case 89: {
				Expect(114, t); // "Function"
				currentState = 92;
				break;
			}
			case 90: {
				if (t.kind == 195) {
					goto case 88;
				} else {
					goto case 91;
				}
			}
			case 91: {
				if (t.kind == 114) {
					goto case 89;
				} else {
					Error(t);
					goto case 92;
				}
			}
			case 92: {
				goto case 13; // StatementTerminator
			}
			case 93: { // start of ParameterList
				stateStack.Push(96);
				goto case 98; // Parameter
			}
			case 94: {
				Expect(12, t); // ","
				currentState = 95;
				break;
			}
			case 95: {
				stateStack.Push(96);
				goto case 98; // Parameter
			}
			case 96: {
				if (t.kind == 12) {
					goto case 94;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 97: {
				stateStack.Push(98);
				goto case 24; // AttributeBlock
			}
			case 98: { // start of Parameter
				if (t.kind == 28) {
					goto case 97;
				} else {
					goto case 100;
				}
			}
			case 99: {
				stateStack.Push(100);
				goto case 358; // ParameterModifier
			}
			case 100: {
				if (set[9, t.kind]) {
					goto case 99;
				} else {
					goto case 101;
				}
			}
			case 101: {
				stateStack.Push(104);
				goto case 268; // Identifier
			}
			case 102: {
				Expect(50, t); // "As"
				currentState = 103;
				break;
			}
			case 103: {
				stateStack.Push(107);
				goto case 177; // TypeName
			}
			case 104: {
				if (t.kind == 50) {
					goto case 102;
				} else {
					goto case 107;
				}
			}
			case 105: {
				Expect(10, t); // "="
				currentState = 106;
				break;
			}
			case 106: {
				goto case 117; // Expression
			}
			case 107: {
				if (t.kind == 10) {
					goto case 105;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 108: { // start of Block
				PushContext(Context.Body); 
				goto case 109;
			}
			case 109: {
				stateStack.Push(111);
				goto case 13; // StatementTerminator
			}
			case 110: {
				stateStack.Push(111);
				goto case 13; // StatementTerminator
			}
			case 111: {
				if (t.kind == 1 || t.kind == 11) {
					goto case 110;
				} else {
					goto case 113;
				}
			}
			case 112: {
				stateStack.Push(113);
				goto case 266; // Statement
			}
			case 113: {
				if (set[10, t.kind]) {
					goto case 112;
				} else {
					goto case 115;
				}
			}
			case 114: {
				stateStack.Push(115);
				goto case 13; // StatementTerminator
			}
			case 115: {
				if (t.kind == 1 || t.kind == 11) {
					goto case 114;
				} else {
					goto case 116;
				}
			}
			case 116: {
				PopContext(); 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 117: { // start of Expression
				isExpressionStart = true; 
				goto case 124;
			}
			case 118: {
				stateStack.Push(119);
				goto case 202; // Literal
			}
			case 119: {
				isExpressionStart = false; 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 120: {
				Expect(25, t); // "("
				currentState = 121;
				break;
			}
			case 121: {
				isExpressionStart = false; 
				goto case 122;
			}
			case 122: {
				stateStack.Push(123);
				goto case 117; // Expression
			}
			case 123: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 124: {
				if (set[11, t.kind]) {
					goto case 118;
				} else {
					goto case 125;
				}
			}
			case 125: {
				if (t.kind == 25) {
					goto case 120;
				} else {
					goto case 136;
				}
			}
			case 126: {
				stateStack.Push(127);
				goto case 268; // Identifier
			}
			case 127: {
				isExpressionStart = false; 
				goto case 135;
			}
			case 128: {
				Expect(25, t); // "("
				currentState = 129;
				break;
			}
			case 129: {
				Expect(155, t); // "Of"
				currentState = 130;
				break;
			}
			case 130: {
				stateStack.Push(133);
				goto case 177; // TypeName
			}
			case 131: {
				Expect(12, t); // ","
				currentState = 132;
				break;
			}
			case 132: {
				stateStack.Push(133);
				goto case 177; // TypeName
			}
			case 133: {
				if (t.kind == 12) {
					goto case 131;
				} else {
					goto case 134;
				}
			}
			case 134: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 135: {
				if (t.kind == 25) {
					goto case 128;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 136: {
				if (set[12, t.kind]) {
					goto case 126;
				} else {
					goto case 140;
				}
			}
			case 137: {
				Expect(44, t); // "AddressOf"
				currentState = 138;
				break;
			}
			case 138: {
				isExpressionStart = false; 
				goto case 139;
			}
			case 139: {
				goto case 117; // Expression
			}
			case 140: {
				if (t.kind == 44) {
					goto case 137;
				} else {
					goto case 142;
				}
			}
			case 141: {
				Expect(28, t); // "<"
				currentState = stateStack.Pop();
				break;
			}
			case 142: {
				if (t.kind == 28) {
					goto case 141;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 143: {
				Expect(58, t); // "Byte"
				currentState = stateStack.Pop();
				break;
			}
			case 144: {
				Expect(181, t); // "SByte"
				currentState = stateStack.Pop();
				break;
			}
			case 145: { // start of PrimitiveTypeName
				if (t.kind == 58) {
					goto case 143;
				} else {
					goto case 146;
				}
			}
			case 146: {
				if (t.kind == 181) {
					goto case 144;
				} else {
					goto case 148;
				}
			}
			case 147: {
				Expect(210, t); // "UShort"
				currentState = stateStack.Pop();
				break;
			}
			case 148: {
				if (t.kind == 210) {
					goto case 147;
				} else {
					goto case 150;
				}
			}
			case 149: {
				Expect(186, t); // "Short"
				currentState = stateStack.Pop();
				break;
			}
			case 150: {
				if (t.kind == 186) {
					goto case 149;
				} else {
					goto case 152;
				}
			}
			case 151: {
				Expect(206, t); // "UInteger"
				currentState = stateStack.Pop();
				break;
			}
			case 152: {
				if (t.kind == 206) {
					goto case 151;
				} else {
					goto case 154;
				}
			}
			case 153: {
				Expect(128, t); // "Integer"
				currentState = stateStack.Pop();
				break;
			}
			case 154: {
				if (t.kind == 128) {
					goto case 153;
				} else {
					goto case 156;
				}
			}
			case 155: {
				Expect(207, t); // "ULong"
				currentState = stateStack.Pop();
				break;
			}
			case 156: {
				if (t.kind == 207) {
					goto case 155;
				} else {
					goto case 158;
				}
			}
			case 157: {
				Expect(137, t); // "Long"
				currentState = stateStack.Pop();
				break;
			}
			case 158: {
				if (t.kind == 137) {
					goto case 157;
				} else {
					goto case 160;
				}
			}
			case 159: {
				Expect(187, t); // "Single"
				currentState = stateStack.Pop();
				break;
			}
			case 160: {
				if (t.kind == 187) {
					goto case 159;
				} else {
					goto case 162;
				}
			}
			case 161: {
				Expect(96, t); // "Double"
				currentState = stateStack.Pop();
				break;
			}
			case 162: {
				if (t.kind == 96) {
					goto case 161;
				} else {
					goto case 164;
				}
			}
			case 163: {
				Expect(87, t); // "Decimal"
				currentState = stateStack.Pop();
				break;
			}
			case 164: {
				if (t.kind == 87) {
					goto case 163;
				} else {
					goto case 166;
				}
			}
			case 165: {
				Expect(55, t); // "Boolean"
				currentState = stateStack.Pop();
				break;
			}
			case 166: {
				if (t.kind == 55) {
					goto case 165;
				} else {
					goto case 168;
				}
			}
			case 167: {
				Expect(86, t); // "Date"
				currentState = stateStack.Pop();
				break;
			}
			case 168: {
				if (t.kind == 86) {
					goto case 167;
				} else {
					goto case 170;
				}
			}
			case 169: {
				Expect(69, t); // "Char"
				currentState = stateStack.Pop();
				break;
			}
			case 170: {
				if (t.kind == 69) {
					goto case 169;
				} else {
					goto case 172;
				}
			}
			case 171: {
				Expect(193, t); // "String"
				currentState = stateStack.Pop();
				break;
			}
			case 172: {
				if (t.kind == 193) {
					goto case 171;
				} else {
					goto case 174;
				}
			}
			case 173: {
				Expect(154, t); // "Object"
				currentState = stateStack.Pop();
				break;
			}
			case 174: {
				if (t.kind == 154) {
					goto case 173;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 175: {
				Expect(117, t); // "Global"
				currentState = 182;
				break;
			}
			case 176: {
				stateStack.Push(182);
				goto case 268; // Identifier
			}
			case 177: { // start of TypeName
				if (t.kind == 117) {
					goto case 175;
				} else {
					goto case 178;
				}
			}
			case 178: {
				if (set[12, t.kind]) {
					goto case 176;
				} else {
					goto case 180;
				}
			}
			case 179: {
				stateStack.Push(182);
				goto case 145; // PrimitiveTypeName
			}
			case 180: {
				if (set[13, t.kind]) {
					goto case 179;
				} else {
					Error(t);
					goto case 182;
				}
			}
			case 181: {
				stateStack.Push(182);
				goto case 188; // TypeSuffix
			}
			case 182: {
				if (t.kind == 25) {
					goto case 181;
				} else {
					goto case 187;
				}
			}
			case 183: {
				Expect(16, t); // "."
				currentState = 184;
				break;
			}
			case 184: {
				stateStack.Push(186);
				goto case 199; // IdentifierOrKeyword
			}
			case 185: {
				stateStack.Push(186);
				goto case 188; // TypeSuffix
			}
			case 186: {
				if (t.kind == 25) {
					goto case 185;
				} else {
					goto case 187;
				}
			}
			case 187: {
				if (t.kind == 16) {
					goto case 183;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 188: { // start of TypeSuffix
				Expect(25, t); // "("
				currentState = 196;
				break;
			}
			case 189: {
				Expect(155, t); // "Of"
				currentState = 190;
				break;
			}
			case 190: {
				stateStack.Push(193);
				goto case 177; // TypeName
			}
			case 191: {
				Expect(12, t); // ","
				currentState = 192;
				break;
			}
			case 192: {
				stateStack.Push(193);
				goto case 177; // TypeName
			}
			case 193: {
				if (t.kind == 12) {
					goto case 191;
				} else {
					goto case 198;
				}
			}
			case 194: {
				Expect(12, t); // ","
				currentState = 195;
				break;
			}
			case 195: {
				if (t.kind == 12) {
					goto case 194;
				} else {
					goto case 198;
				}
			}
			case 196: {
				if (t.kind == 155) {
					goto case 189;
				} else {
					goto case 197;
				}
			}
			case 197: {
				if (t.kind == 12 || t.kind == 26) {
					goto case 195;
				} else {
					Error(t);
					goto case 198;
				}
			}
			case 198: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 199: { // start of IdentifierOrKeyword
				currentState = stateStack.Pop();
				break;
			}
			case 200: {
				Expect(3, t); // LiteralString
				currentState = stateStack.Pop();
				break;
			}
			case 201: {
				Expect(4, t); // LiteralCharacter
				currentState = stateStack.Pop();
				break;
			}
			case 202: { // start of Literal
				if (t.kind == 3) {
					goto case 200;
				} else {
					goto case 203;
				}
			}
			case 203: {
				if (t.kind == 4) {
					goto case 201;
				} else {
					goto case 205;
				}
			}
			case 204: {
				Expect(5, t); // LiteralInteger
				currentState = stateStack.Pop();
				break;
			}
			case 205: {
				if (t.kind == 5) {
					goto case 204;
				} else {
					goto case 207;
				}
			}
			case 206: {
				Expect(6, t); // LiteralDouble
				currentState = stateStack.Pop();
				break;
			}
			case 207: {
				if (t.kind == 6) {
					goto case 206;
				} else {
					goto case 209;
				}
			}
			case 208: {
				Expect(7, t); // LiteralSingle
				currentState = stateStack.Pop();
				break;
			}
			case 209: {
				if (t.kind == 7) {
					goto case 208;
				} else {
					goto case 211;
				}
			}
			case 210: {
				Expect(8, t); // LiteralDecimal
				currentState = stateStack.Pop();
				break;
			}
			case 211: {
				if (t.kind == 8) {
					goto case 210;
				} else {
					goto case 213;
				}
			}
			case 212: {
				Expect(9, t); // LiteralDate
				currentState = stateStack.Pop();
				break;
			}
			case 213: {
				if (t.kind == 9) {
					goto case 212;
				} else {
					goto case 215;
				}
			}
			case 214: {
				Expect(202, t); // "True"
				currentState = stateStack.Pop();
				break;
			}
			case 215: {
				if (t.kind == 202) {
					goto case 214;
				} else {
					goto case 217;
				}
			}
			case 216: {
				Expect(109, t); // "False"
				currentState = stateStack.Pop();
				break;
			}
			case 217: {
				if (t.kind == 109) {
					goto case 216;
				} else {
					goto case 219;
				}
			}
			case 218: {
				Expect(151, t); // "Nothing"
				currentState = stateStack.Pop();
				break;
			}
			case 219: {
				if (t.kind == 151) {
					goto case 218;
				} else {
					goto case 221;
				}
			}
			case 220: {
				Expect(139, t); // "Me"
				currentState = stateStack.Pop();
				break;
			}
			case 221: {
				if (t.kind == 139) {
					goto case 220;
				} else {
					goto case 223;
				}
			}
			case 222: {
				Expect(144, t); // "MyBase"
				currentState = stateStack.Pop();
				break;
			}
			case 223: {
				if (t.kind == 144) {
					goto case 222;
				} else {
					goto case 225;
				}
			}
			case 224: {
				Expect(145, t); // "MyClass"
				currentState = stateStack.Pop();
				break;
			}
			case 225: {
				if (t.kind == 145) {
					goto case 224;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: {
				stateStack.Push(230);
				goto case 268; // Identifier
			}
			case 227: {
				Expect(5, t); // LiteralInteger
				currentState = 230;
				break;
			}
			case 228: {
				if (set[12, t.kind]) {
					goto case 226;
				} else {
					goto case 229;
				}
			}
			case 229: {
				if (t.kind == 5) {
					goto case 227;
				} else {
					Error(t);
					goto case 230;
				}
			}
			case 230: {
				Expect(11, t); // ":"
				currentState = stateStack.Pop();
				break;
			}
			case 231: {
				Expect(92, t); // "Dim"
				currentState = 237;
				break;
			}
			case 232: {
				Expect(189, t); // "Static"
				currentState = 237;
				break;
			}
			case 233: {
				if (t.kind == 92) {
					goto case 231;
				} else {
					goto case 234;
				}
			}
			case 234: {
				if (t.kind == 189) {
					goto case 232;
				} else {
					goto case 236;
				}
			}
			case 235: {
				Expect(75, t); // "Const"
				currentState = 237;
				break;
			}
			case 236: {
				if (t.kind == 75) {
					goto case 235;
				} else {
					Error(t);
					goto case 237;
				}
			}
			case 237: {
				stateStack.Push(239);
				goto case 268; // Identifier
			}
			case 238: {
				Expect(21, t); // "?"
				currentState = 244;
				break;
			}
			case 239: {
				if (t.kind == 21) {
					goto case 238;
				} else {
					goto case 244;
				}
			}
			case 240: {
				Expect(25, t); // "("
				currentState = 242;
				break;
			}
			case 241: {
				Expect(12, t); // ","
				currentState = 242;
				break;
			}
			case 242: {
				if (t.kind == 12) {
					goto case 241;
				} else {
					goto case 243;
				}
			}
			case 243: {
				Expect(26, t); // ")"
				currentState = 254;
				break;
			}
			case 244: {
				if (t.kind == 25) {
					goto case 240;
				} else {
					goto case 254;
				}
			}
			case 245: {
				Expect(12, t); // ","
				currentState = 246;
				break;
			}
			case 246: {
				stateStack.Push(248);
				goto case 268; // Identifier
			}
			case 247: {
				Expect(21, t); // "?"
				currentState = 253;
				break;
			}
			case 248: {
				if (t.kind == 21) {
					goto case 247;
				} else {
					goto case 253;
				}
			}
			case 249: {
				Expect(25, t); // "("
				currentState = 251;
				break;
			}
			case 250: {
				Expect(12, t); // ","
				currentState = 251;
				break;
			}
			case 251: {
				if (t.kind == 12) {
					goto case 250;
				} else {
					goto case 252;
				}
			}
			case 252: {
				Expect(26, t); // ")"
				currentState = 254;
				break;
			}
			case 253: {
				if (t.kind == 25) {
					goto case 249;
				} else {
					goto case 254;
				}
			}
			case 254: {
				if (t.kind == 12) {
					goto case 245;
				} else {
					goto case 262;
				}
			}
			case 255: {
				Expect(50, t); // "As"
				currentState = 257;
				break;
			}
			case 256: {
				Expect(148, t); // "New"
				currentState = 258;
				break;
			}
			case 257: {
				if (t.kind == 148) {
					goto case 256;
				} else {
					goto case 258;
				}
			}
			case 258: {
				stateStack.Push(261);
				goto case 177; // TypeName
			}
			case 259: {
				Expect(25, t); // "("
				currentState = 260;
				break;
			}
			case 260: {
				Expect(26, t); // ")"
				currentState = 265;
				break;
			}
			case 261: {
				if (t.kind == 25) {
					goto case 259;
				} else {
					goto case 265;
				}
			}
			case 262: {
				if (t.kind == 50) {
					goto case 255;
				} else {
					goto case 265;
				}
			}
			case 263: {
				Expect(10, t); // "="
				currentState = 264;
				break;
			}
			case 264: {
				goto case 117; // Expression
			}
			case 265: {
				if (t.kind == 10) {
					goto case 263;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 266: { // start of Statement
				if (set[14, t.kind]) {
					goto case 228;
				} else {
					goto case 267;
				}
			}
			case 267: {
				if (t.kind == 75 || t.kind == 92 || t.kind == 189) {
					goto case 233;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 268: { // start of Identifier
				PushContext(Context.IdentifierExpected); 
				goto case 271;
			}
			case 269: {
				stateStack.Push(273);
				goto case 276; // IdentifierForFieldDeclaration
			}
			case 270: {
				Expect(85, t); // "Custom"
				currentState = 273;
				break;
			}
			case 271: {
				if (set[15, t.kind]) {
					goto case 269;
				} else {
					goto case 272;
				}
			}
			case 272: {
				if (t.kind == 85) {
					goto case 270;
				} else {
					Error(t);
					goto case 273;
				}
			}
			case 273: {
				PopContext(); 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 274: {
				Expect(2, t); // ident
				currentState = stateStack.Pop();
				break;
			}
			case 275: {
				Expect(45, t); // "Aggregate"
				currentState = stateStack.Pop();
				break;
			}
			case 276: { // start of IdentifierForFieldDeclaration
				if (t.kind == 2) {
					goto case 274;
				} else {
					goto case 277;
				}
			}
			case 277: {
				if (t.kind == 45) {
					goto case 275;
				} else {
					goto case 279;
				}
			}
			case 278: {
				Expect(49, t); // "Ansi"
				currentState = stateStack.Pop();
				break;
			}
			case 279: {
				if (t.kind == 49) {
					goto case 278;
				} else {
					goto case 281;
				}
			}
			case 280: {
				Expect(51, t); // "Ascending"
				currentState = stateStack.Pop();
				break;
			}
			case 281: {
				if (t.kind == 51) {
					goto case 280;
				} else {
					goto case 283;
				}
			}
			case 282: {
				Expect(52, t); // "Assembly"
				currentState = stateStack.Pop();
				break;
			}
			case 283: {
				if (t.kind == 52) {
					goto case 282;
				} else {
					goto case 285;
				}
			}
			case 284: {
				Expect(53, t); // "Auto"
				currentState = stateStack.Pop();
				break;
			}
			case 285: {
				if (t.kind == 53) {
					goto case 284;
				} else {
					goto case 287;
				}
			}
			case 286: {
				Expect(54, t); // "Binary"
				currentState = stateStack.Pop();
				break;
			}
			case 287: {
				if (t.kind == 54) {
					goto case 286;
				} else {
					goto case 289;
				}
			}
			case 288: {
				Expect(57, t); // "By"
				currentState = stateStack.Pop();
				break;
			}
			case 289: {
				if (t.kind == 57) {
					goto case 288;
				} else {
					goto case 291;
				}
			}
			case 290: {
				Expect(74, t); // "Compare"
				currentState = stateStack.Pop();
				break;
			}
			case 291: {
				if (t.kind == 74) {
					goto case 290;
				} else {
					goto case 293;
				}
			}
			case 292: {
				Expect(91, t); // "Descending"
				currentState = stateStack.Pop();
				break;
			}
			case 293: {
				if (t.kind == 91) {
					goto case 292;
				} else {
					goto case 295;
				}
			}
			case 294: {
				Expect(94, t); // "Distinct"
				currentState = stateStack.Pop();
				break;
			}
			case 295: {
				if (t.kind == 94) {
					goto case 294;
				} else {
					goto case 297;
				}
			}
			case 296: {
				Expect(103, t); // "Equals"
				currentState = stateStack.Pop();
				break;
			}
			case 297: {
				if (t.kind == 103) {
					goto case 296;
				} else {
					goto case 299;
				}
			}
			case 298: {
				Expect(108, t); // "Explicit"
				currentState = stateStack.Pop();
				break;
			}
			case 299: {
				if (t.kind == 108) {
					goto case 298;
				} else {
					goto case 301;
				}
			}
			case 300: {
				Expect(113, t); // "From"
				currentState = stateStack.Pop();
				break;
			}
			case 301: {
				if (t.kind == 113) {
					goto case 300;
				} else {
					goto case 303;
				}
			}
			case 302: {
				Expect(120, t); // "Group"
				currentState = stateStack.Pop();
				break;
			}
			case 303: {
				if (t.kind == 120) {
					goto case 302;
				} else {
					goto case 305;
				}
			}
			case 304: {
				Expect(126, t); // "Infer"
				currentState = stateStack.Pop();
				break;
			}
			case 305: {
				if (t.kind == 126) {
					goto case 304;
				} else {
					goto case 307;
				}
			}
			case 306: {
				Expect(130, t); // "Into"
				currentState = stateStack.Pop();
				break;
			}
			case 307: {
				if (t.kind == 130) {
					goto case 306;
				} else {
					goto case 309;
				}
			}
			case 308: {
				Expect(133, t); // "Join"
				currentState = stateStack.Pop();
				break;
			}
			case 309: {
				if (t.kind == 133) {
					goto case 308;
				} else {
					goto case 311;
				}
			}
			case 310: {
				Expect(156, t); // "Off"
				currentState = stateStack.Pop();
				break;
			}
			case 311: {
				if (t.kind == 156) {
					goto case 310;
				} else {
					goto case 313;
				}
			}
			case 312: {
				Expect(162, t); // "Order"
				currentState = stateStack.Pop();
				break;
			}
			case 313: {
				if (t.kind == 162) {
					goto case 312;
				} else {
					goto case 315;
				}
			}
			case 314: {
				Expect(169, t); // "Preserve"
				currentState = stateStack.Pop();
				break;
			}
			case 315: {
				if (t.kind == 169) {
					goto case 314;
				} else {
					goto case 317;
				}
			}
			case 316: {
				Expect(188, t); // "Skip"
				currentState = stateStack.Pop();
				break;
			}
			case 317: {
				if (t.kind == 188) {
					goto case 316;
				} else {
					goto case 319;
				}
			}
			case 318: {
				Expect(197, t); // "Take"
				currentState = stateStack.Pop();
				break;
			}
			case 319: {
				if (t.kind == 197) {
					goto case 318;
				} else {
					goto case 321;
				}
			}
			case 320: {
				Expect(198, t); // "Text"
				currentState = stateStack.Pop();
				break;
			}
			case 321: {
				if (t.kind == 198) {
					goto case 320;
				} else {
					goto case 323;
				}
			}
			case 322: {
				Expect(208, t); // "Unicode"
				currentState = stateStack.Pop();
				break;
			}
			case 323: {
				if (t.kind == 208) {
					goto case 322;
				} else {
					goto case 325;
				}
			}
			case 324: {
				Expect(209, t); // "Until"
				currentState = stateStack.Pop();
				break;
			}
			case 325: {
				if (t.kind == 209) {
					goto case 324;
				} else {
					goto case 327;
				}
			}
			case 326: {
				Expect(215, t); // "Where"
				currentState = stateStack.Pop();
				break;
			}
			case 327: {
				if (t.kind == 215) {
					goto case 326;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 328: {
				Expect(173, t); // "Public"
				currentState = stateStack.Pop();
				break;
			}
			case 329: {
				Expect(112, t); // "Friend"
				currentState = stateStack.Pop();
				break;
			}
			case 330: { // start of AccessModifier
				if (t.kind == 173) {
					goto case 328;
				} else {
					goto case 331;
				}
			}
			case 331: {
				if (t.kind == 112) {
					goto case 329;
				} else {
					goto case 333;
				}
			}
			case 332: {
				Expect(172, t); // "Protected"
				currentState = stateStack.Pop();
				break;
			}
			case 333: {
				if (t.kind == 172) {
					goto case 332;
				} else {
					goto case 335;
				}
			}
			case 334: {
				Expect(170, t); // "Private"
				currentState = stateStack.Pop();
				break;
			}
			case 335: {
				if (t.kind == 170) {
					goto case 334;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 336: {
				goto case 330; // AccessModifier
			}
			case 337: {
				Expect(184, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 338: { // start of TypeModifier
				if (set[16, t.kind]) {
					goto case 336;
				} else {
					goto case 339;
				}
			}
			case 339: {
				if (t.kind == 184) {
					goto case 337;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 340: {
				goto case 330; // AccessModifier
			}
			case 341: {
				Expect(184, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 342: { // start of MemberModifier
				if (set[16, t.kind]) {
					goto case 340;
				} else {
					goto case 343;
				}
			}
			case 343: {
				if (t.kind == 184) {
					goto case 341;
				} else {
					goto case 345;
				}
			}
			case 344: {
				Expect(185, t); // "Shared"
				currentState = stateStack.Pop();
				break;
			}
			case 345: {
				if (t.kind == 185) {
					goto case 344;
				} else {
					goto case 347;
				}
			}
			case 346: {
				Expect(165, t); // "Overridable"
				currentState = stateStack.Pop();
				break;
			}
			case 347: {
				if (t.kind == 165) {
					goto case 346;
				} else {
					goto case 349;
				}
			}
			case 348: {
				Expect(153, t); // "NotOverridable"
				currentState = stateStack.Pop();
				break;
			}
			case 349: {
				if (t.kind == 153) {
					goto case 348;
				} else {
					goto case 351;
				}
			}
			case 350: {
				Expect(166, t); // "Overrides"
				currentState = stateStack.Pop();
				break;
			}
			case 351: {
				if (t.kind == 166) {
					goto case 350;
				} else {
					goto case 353;
				}
			}
			case 352: {
				Expect(164, t); // "Overloads"
				currentState = stateStack.Pop();
				break;
			}
			case 353: {
				if (t.kind == 164) {
					goto case 352;
				} else {
					goto case 355;
				}
			}
			case 354: {
				Expect(168, t); // "Partial"
				currentState = stateStack.Pop();
				break;
			}
			case 355: {
				if (t.kind == 168) {
					goto case 354;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 356: {
				Expect(59, t); // "ByVal"
				currentState = stateStack.Pop();
				break;
			}
			case 357: {
				Expect(56, t); // "ByRef"
				currentState = stateStack.Pop();
				break;
			}
			case 358: { // start of ParameterModifier
				if (t.kind == 59) {
					goto case 356;
				} else {
					goto case 359;
				}
			}
			case 359: {
				if (t.kind == 56) {
					goto case 357;
				} else {
					goto case 361;
				}
			}
			case 360: {
				Expect(160, t); // "Optional"
				currentState = stateStack.Pop();
				break;
			}
			case 361: {
				if (t.kind == 160) {
					goto case 360;
				} else {
					goto case 363;
				}
			}
			case 362: {
				Expect(167, t); // "ParamArray"
				currentState = stateStack.Pop();
				break;
			}
			case 363: {
				if (t.kind == 167) {
					goto case 362;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
		}

	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};

} // end Parser


}