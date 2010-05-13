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



partial class Parser {

	const bool T = true;
	const bool x = false;

int currentState = 1;

	readonly Stack<int> stateStack = new Stack<int>();
	
	public Parser()
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
		switchlbl: switch (currentState) {
			case 0: {
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 1: { // start of ParserHelper
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
				goto case 23; // AttributeBlock
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
				goto case 31; // NamespaceMemberDeclaration
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
				currentState = 21;
				break;
			}
			case 20: {
				currentState = 21;
				break;
			}
			case 21: {
				if (set[1, t.kind]) {
					goto case 20;
				} else {
					goto case 22;
				}
			}
			case 22: {
				goto case 13; // StatementTerminator
			}
			case 23: { // start of AttributeBlock
				Expect(28, t); // "<"
				currentState = 25;
				break;
			}
			case 24: {
				currentState = 25;
				break;
			}
			case 25: {
				if (set[2, t.kind]) {
					goto case 24;
				} else {
					goto case 26;
				}
			}
			case 26: {
				Expect(27, t); // ">"
				currentState = 28;
				break;
			}
			case 27: {
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 28: {
				if (t.kind == 1) {
					goto case 27;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 29: {
				goto case 33; // NamespaceDeclaration
			}
			case 30: {
				goto case 43; // TypeDeclaration
			}
			case 31: { // start of NamespaceMemberDeclaration
				if (t.kind == 146) {
					goto case 29;
				} else {
					goto case 32;
				}
			}
			case 32: {
				if (set[3, t.kind]) {
					goto case 30;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 33: { // start of NamespaceDeclaration
				Expect(146, t); // "Namespace"
				currentState = 35;
				break;
			}
			case 34: {
				currentState = 35;
				break;
			}
			case 35: {
				if (set[1, t.kind]) {
					goto case 34;
				} else {
					goto case 36;
				}
			}
			case 36: {
				stateStack.Push(38);
				goto case 13; // StatementTerminator
			}
			case 37: {
				stateStack.Push(38);
				goto case 31; // NamespaceMemberDeclaration
			}
			case 38: {
				if (set[4, t.kind]) {
					goto case 37;
				} else {
					goto case 39;
				}
			}
			case 39: {
				Expect(100, t); // "End"
				currentState = 40;
				break;
			}
			case 40: {
				Expect(146, t); // "Namespace"
				currentState = 41;
				break;
			}
			case 41: {
				goto case 13; // StatementTerminator
			}
			case 42: {
				stateStack.Push(43);
				goto case 23; // AttributeBlock
			}
			case 43: { // start of TypeDeclaration
				if (t.kind == 28) {
					goto case 42;
				} else {
					goto case 45;
				}
			}
			case 44: {
				stateStack.Push(45);
				goto case 277; // TypeModifier
			}
			case 45: {
				if (set[5, t.kind]) {
					goto case 44;
				} else {
					goto case 46;
				}
			}
			case 46: {
				Expect(141, t); // "Module"
				currentState = 48;
				break;
			}
			case 47: {
				currentState = 48;
				break;
			}
			case 48: {
				if (set[1, t.kind]) {
					goto case 47;
				} else {
					goto case 49;
				}
			}
			case 49: {
				stateStack.Push(50);
				goto case 13; // StatementTerminator
			}
			case 50: {
				PushContext(Context.Type); 
				goto case 52;
			}
			case 51: {
				stateStack.Push(52);
				goto case 57; // MemberDeclaration
			}
			case 52: {
				if (set[6, t.kind]) {
					goto case 51;
				} else {
					goto case 53;
				}
			}
			case 53: {
				Expect(100, t); // "End"
				currentState = 54;
				break;
			}
			case 54: {
				Expect(141, t); // "Module"
				currentState = 55;
				break;
			}
			case 55: {
				stateStack.Push(56);
				goto case 13; // StatementTerminator
			}
			case 56: {
				PopContext(); 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 57: { // start of MemberDeclaration
				PushContext(Context.Member); 
				goto case 58;
			}
			case 58: {
				stateStack.Push(59);
				goto case 61; // SubOrFunctionDeclaration
			}
			case 59: {
				PopContext(); 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 60: {
				stateStack.Push(61);
				goto case 23; // AttributeBlock
			}
			case 61: { // start of SubOrFunctionDeclaration
				if (t.kind == 28) {
					goto case 60;
				} else {
					goto case 63;
				}
			}
			case 62: {
				stateStack.Push(63);
				goto case 281; // MemberModifier
			}
			case 63: {
				if (set[7, t.kind]) {
					goto case 62;
				} else {
					goto case 66;
				}
			}
			case 64: {
				Expect(195, t); // "Sub"
				currentState = 68;
				break;
			}
			case 65: {
				Expect(114, t); // "Function"
				currentState = 68;
				break;
			}
			case 66: {
				if (t.kind == 195) {
					goto case 64;
				} else {
					goto case 67;
				}
			}
			case 67: {
				if (t.kind == 114) {
					goto case 65;
				} else {
					Error(t);
					goto case 68;
				}
			}
			case 68: {
				PushContext(Context.IdentifierExpected); 
				goto case 69;
			}
			case 69: {
				currentState = 70;
				break;
			}
			case 70: {
				PopContext(); 
				goto case 75;
			}
			case 71: {
				Expect(25, t); // "("
				currentState = 73;
				break;
			}
			case 72: {
				stateStack.Push(74);
				goto case 88; // ParameterList
			}
			case 73: {
				if (set[8, t.kind]) {
					goto case 72;
				} else {
					goto case 74;
				}
			}
			case 74: {
				Expect(26, t); // ")"
				currentState = 78;
				break;
			}
			case 75: {
				if (t.kind == 25) {
					goto case 71;
				} else {
					goto case 78;
				}
			}
			case 76: {
				Expect(50, t); // "As"
				currentState = 77;
				break;
			}
			case 77: {
				stateStack.Push(79);
				goto case 159; // TypeName
			}
			case 78: {
				if (t.kind == 50) {
					goto case 76;
				} else {
					goto case 79;
				}
			}
			case 79: {
				Expect(1, t); // EOL
				currentState = 81;
				break;
			}
			case 80: {
				stateStack.Push(82);
				goto case 103; // Block
			}
			case 81: {
				if (t.kind == 1) {
					goto case 80;
				} else {
					goto case 82;
				}
			}
			case 82: {
				Expect(100, t); // "End"
				currentState = 85;
				break;
			}
			case 83: {
				Expect(195, t); // "Sub"
				currentState = 87;
				break;
			}
			case 84: {
				Expect(114, t); // "Function"
				currentState = 87;
				break;
			}
			case 85: {
				if (t.kind == 195) {
					goto case 83;
				} else {
					goto case 86;
				}
			}
			case 86: {
				if (t.kind == 114) {
					goto case 84;
				} else {
					Error(t);
					goto case 87;
				}
			}
			case 87: {
				goto case 13; // StatementTerminator
			}
			case 88: { // start of ParameterList
				stateStack.Push(91);
				goto case 93; // Parameter
			}
			case 89: {
				Expect(12, t); // ","
				currentState = 90;
				break;
			}
			case 90: {
				stateStack.Push(91);
				goto case 93; // Parameter
			}
			case 91: {
				if (t.kind == 12) {
					goto case 89;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 92: {
				stateStack.Push(93);
				goto case 23; // AttributeBlock
			}
			case 93: { // start of Parameter
				if (t.kind == 28) {
					goto case 92;
				} else {
					goto case 95;
				}
			}
			case 94: {
				stateStack.Push(95);
				goto case 297; // ParameterModifier
			}
			case 95: {
				if (set[9, t.kind]) {
					goto case 94;
				} else {
					goto case 96;
				}
			}
			case 96: {
				stateStack.Push(99);
				goto case 210; // Identifier
			}
			case 97: {
				Expect(50, t); // "As"
				currentState = 98;
				break;
			}
			case 98: {
				stateStack.Push(102);
				goto case 159; // TypeName
			}
			case 99: {
				if (t.kind == 50) {
					goto case 97;
				} else {
					goto case 102;
				}
			}
			case 100: {
				Expect(10, t); // "="
				currentState = 101;
				break;
			}
			case 101: {
				goto case 108; // Expression
			}
			case 102: {
				if (t.kind == 10) {
					goto case 100;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 103: { // start of Block
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 104: {
				goto case 184; // Literal
			}
			case 105: {
				Expect(25, t); // "("
				currentState = 106;
				break;
			}
			case 106: {
				stateStack.Push(107);
				goto case 108; // Expression
			}
			case 107: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 108: { // start of Expression
				if (set[10, t.kind]) {
					goto case 104;
				} else {
					goto case 109;
				}
			}
			case 109: {
				if (t.kind == 25) {
					goto case 105;
				} else {
					goto case 119;
				}
			}
			case 110: {
				stateStack.Push(118);
				goto case 210; // Identifier
			}
			case 111: {
				Expect(25, t); // "("
				currentState = 112;
				break;
			}
			case 112: {
				Expect(155, t); // "Of"
				currentState = 113;
				break;
			}
			case 113: {
				stateStack.Push(116);
				goto case 159; // TypeName
			}
			case 114: {
				Expect(12, t); // ","
				currentState = 115;
				break;
			}
			case 115: {
				stateStack.Push(116);
				goto case 159; // TypeName
			}
			case 116: {
				if (t.kind == 12) {
					goto case 114;
				} else {
					goto case 117;
				}
			}
			case 117: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 118: {
				if (t.kind == 25) {
					goto case 111;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 119: {
				if (set[11, t.kind]) {
					goto case 110;
				} else {
					goto case 122;
				}
			}
			case 120: {
				Expect(44, t); // "AddressOf"
				currentState = 121;
				break;
			}
			case 121: {
				goto case 108; // Expression
			}
			case 122: {
				if (t.kind == 44) {
					goto case 120;
				} else {
					goto case 124;
				}
			}
			case 123: {
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 124: {
				if (false) {
					goto case 123;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 125: {
				Expect(58, t); // "Byte"
				currentState = stateStack.Pop();
				break;
			}
			case 126: {
				Expect(181, t); // "SByte"
				currentState = stateStack.Pop();
				break;
			}
			case 127: { // start of PrimitiveTypeName
				if (t.kind == 58) {
					goto case 125;
				} else {
					goto case 128;
				}
			}
			case 128: {
				if (t.kind == 181) {
					goto case 126;
				} else {
					goto case 130;
				}
			}
			case 129: {
				Expect(210, t); // "UShort"
				currentState = stateStack.Pop();
				break;
			}
			case 130: {
				if (t.kind == 210) {
					goto case 129;
				} else {
					goto case 132;
				}
			}
			case 131: {
				Expect(186, t); // "Short"
				currentState = stateStack.Pop();
				break;
			}
			case 132: {
				if (t.kind == 186) {
					goto case 131;
				} else {
					goto case 134;
				}
			}
			case 133: {
				Expect(206, t); // "UInteger"
				currentState = stateStack.Pop();
				break;
			}
			case 134: {
				if (t.kind == 206) {
					goto case 133;
				} else {
					goto case 136;
				}
			}
			case 135: {
				Expect(128, t); // "Integer"
				currentState = stateStack.Pop();
				break;
			}
			case 136: {
				if (t.kind == 128) {
					goto case 135;
				} else {
					goto case 138;
				}
			}
			case 137: {
				Expect(207, t); // "ULong"
				currentState = stateStack.Pop();
				break;
			}
			case 138: {
				if (t.kind == 207) {
					goto case 137;
				} else {
					goto case 140;
				}
			}
			case 139: {
				Expect(137, t); // "Long"
				currentState = stateStack.Pop();
				break;
			}
			case 140: {
				if (t.kind == 137) {
					goto case 139;
				} else {
					goto case 142;
				}
			}
			case 141: {
				Expect(187, t); // "Single"
				currentState = stateStack.Pop();
				break;
			}
			case 142: {
				if (t.kind == 187) {
					goto case 141;
				} else {
					goto case 144;
				}
			}
			case 143: {
				Expect(96, t); // "Double"
				currentState = stateStack.Pop();
				break;
			}
			case 144: {
				if (t.kind == 96) {
					goto case 143;
				} else {
					goto case 146;
				}
			}
			case 145: {
				Expect(87, t); // "Decimal"
				currentState = stateStack.Pop();
				break;
			}
			case 146: {
				if (t.kind == 87) {
					goto case 145;
				} else {
					goto case 148;
				}
			}
			case 147: {
				Expect(55, t); // "Boolean"
				currentState = stateStack.Pop();
				break;
			}
			case 148: {
				if (t.kind == 55) {
					goto case 147;
				} else {
					goto case 150;
				}
			}
			case 149: {
				Expect(86, t); // "Date"
				currentState = stateStack.Pop();
				break;
			}
			case 150: {
				if (t.kind == 86) {
					goto case 149;
				} else {
					goto case 152;
				}
			}
			case 151: {
				Expect(69, t); // "Char"
				currentState = stateStack.Pop();
				break;
			}
			case 152: {
				if (t.kind == 69) {
					goto case 151;
				} else {
					goto case 154;
				}
			}
			case 153: {
				Expect(193, t); // "String"
				currentState = stateStack.Pop();
				break;
			}
			case 154: {
				if (t.kind == 193) {
					goto case 153;
				} else {
					goto case 156;
				}
			}
			case 155: {
				Expect(154, t); // "Object"
				currentState = stateStack.Pop();
				break;
			}
			case 156: {
				if (t.kind == 154) {
					goto case 155;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 157: {
				Expect(117, t); // "Global"
				currentState = 164;
				break;
			}
			case 158: {
				stateStack.Push(164);
				goto case 210; // Identifier
			}
			case 159: { // start of TypeName
				if (t.kind == 117) {
					goto case 157;
				} else {
					goto case 160;
				}
			}
			case 160: {
				if (set[11, t.kind]) {
					goto case 158;
				} else {
					goto case 162;
				}
			}
			case 161: {
				stateStack.Push(164);
				goto case 127; // PrimitiveTypeName
			}
			case 162: {
				if (set[12, t.kind]) {
					goto case 161;
				} else {
					Error(t);
					goto case 164;
				}
			}
			case 163: {
				stateStack.Push(164);
				goto case 170; // TypeSuffix
			}
			case 164: {
				if (t.kind == 25) {
					goto case 163;
				} else {
					goto case 169;
				}
			}
			case 165: {
				Expect(16, t); // "."
				currentState = 166;
				break;
			}
			case 166: {
				stateStack.Push(168);
				goto case 181; // IdentifierOrKeyword
			}
			case 167: {
				stateStack.Push(168);
				goto case 170; // TypeSuffix
			}
			case 168: {
				if (t.kind == 25) {
					goto case 167;
				} else {
					goto case 169;
				}
			}
			case 169: {
				if (t.kind == 16) {
					goto case 165;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 170: { // start of TypeSuffix
				Expect(25, t); // "("
				currentState = 178;
				break;
			}
			case 171: {
				Expect(155, t); // "Of"
				currentState = 172;
				break;
			}
			case 172: {
				stateStack.Push(175);
				goto case 159; // TypeName
			}
			case 173: {
				Expect(12, t); // ","
				currentState = 174;
				break;
			}
			case 174: {
				stateStack.Push(175);
				goto case 159; // TypeName
			}
			case 175: {
				if (t.kind == 12) {
					goto case 173;
				} else {
					goto case 180;
				}
			}
			case 176: {
				Expect(12, t); // ","
				currentState = 177;
				break;
			}
			case 177: {
				if (t.kind == 12) {
					goto case 176;
				} else {
					goto case 180;
				}
			}
			case 178: {
				if (t.kind == 155) {
					goto case 171;
				} else {
					goto case 179;
				}
			}
			case 179: {
				if (t.kind == 12 || t.kind == 26) {
					goto case 177;
				} else {
					Error(t);
					goto case 180;
				}
			}
			case 180: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 181: { // start of IdentifierOrKeyword
				currentState = stateStack.Pop();
				break;
			}
			case 182: {
				Expect(3, t); // LiteralString
				currentState = stateStack.Pop();
				break;
			}
			case 183: {
				Expect(4, t); // LiteralCharacter
				currentState = stateStack.Pop();
				break;
			}
			case 184: { // start of Literal
				if (t.kind == 3) {
					goto case 182;
				} else {
					goto case 185;
				}
			}
			case 185: {
				if (t.kind == 4) {
					goto case 183;
				} else {
					goto case 187;
				}
			}
			case 186: {
				Expect(5, t); // LiteralInteger
				currentState = stateStack.Pop();
				break;
			}
			case 187: {
				if (t.kind == 5) {
					goto case 186;
				} else {
					goto case 189;
				}
			}
			case 188: {
				Expect(6, t); // LiteralDouble
				currentState = stateStack.Pop();
				break;
			}
			case 189: {
				if (t.kind == 6) {
					goto case 188;
				} else {
					goto case 191;
				}
			}
			case 190: {
				Expect(7, t); // LiteralSingle
				currentState = stateStack.Pop();
				break;
			}
			case 191: {
				if (t.kind == 7) {
					goto case 190;
				} else {
					goto case 193;
				}
			}
			case 192: {
				Expect(8, t); // LiteralDecimal
				currentState = stateStack.Pop();
				break;
			}
			case 193: {
				if (t.kind == 8) {
					goto case 192;
				} else {
					goto case 195;
				}
			}
			case 194: {
				Expect(9, t); // LiteralDate
				currentState = stateStack.Pop();
				break;
			}
			case 195: {
				if (t.kind == 9) {
					goto case 194;
				} else {
					goto case 197;
				}
			}
			case 196: {
				Expect(202, t); // "True"
				currentState = stateStack.Pop();
				break;
			}
			case 197: {
				if (t.kind == 202) {
					goto case 196;
				} else {
					goto case 199;
				}
			}
			case 198: {
				Expect(109, t); // "False"
				currentState = stateStack.Pop();
				break;
			}
			case 199: {
				if (t.kind == 109) {
					goto case 198;
				} else {
					goto case 201;
				}
			}
			case 200: {
				Expect(151, t); // "Nothing"
				currentState = stateStack.Pop();
				break;
			}
			case 201: {
				if (t.kind == 151) {
					goto case 200;
				} else {
					goto case 203;
				}
			}
			case 202: {
				Expect(139, t); // "Me"
				currentState = stateStack.Pop();
				break;
			}
			case 203: {
				if (t.kind == 139) {
					goto case 202;
				} else {
					goto case 205;
				}
			}
			case 204: {
				Expect(144, t); // "MyBase"
				currentState = stateStack.Pop();
				break;
			}
			case 205: {
				if (t.kind == 144) {
					goto case 204;
				} else {
					goto case 207;
				}
			}
			case 206: {
				Expect(145, t); // "MyClass"
				currentState = stateStack.Pop();
				break;
			}
			case 207: {
				if (t.kind == 145) {
					goto case 206;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 208: {
				stateStack.Push(212);
				goto case 215; // IdentifierForFieldDeclaration
			}
			case 209: {
				Expect(85, t); // "Custom"
				currentState = 212;
				break;
			}
			case 210: { // start of Identifier
		PushContext(Context.IdentifierExpected); 
				if (set[13, t.kind]) {
					goto case 208;
				} else {
					goto case 211;
				}
			}
			case 211: {
				if (t.kind == 85) {
					goto case 209;
				} else {
					Error(t);
					goto case 212;
				}
			}
			case 212: {
				PopContext(); 
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 213: {
				Expect(2, t); // ident
				currentState = stateStack.Pop();
				break;
			}
			case 214: {
				Expect(45, t); // "Aggregate"
				currentState = stateStack.Pop();
				break;
			}
			case 215: { // start of IdentifierForFieldDeclaration
				if (t.kind == 2) {
					goto case 213;
				} else {
					goto case 216;
				}
			}
			case 216: {
				if (t.kind == 45) {
					goto case 214;
				} else {
					goto case 218;
				}
			}
			case 217: {
				Expect(49, t); // "Ansi"
				currentState = stateStack.Pop();
				break;
			}
			case 218: {
				if (t.kind == 49) {
					goto case 217;
				} else {
					goto case 220;
				}
			}
			case 219: {
				Expect(51, t); // "Ascending"
				currentState = stateStack.Pop();
				break;
			}
			case 220: {
				if (t.kind == 51) {
					goto case 219;
				} else {
					goto case 222;
				}
			}
			case 221: {
				Expect(52, t); // "Assembly"
				currentState = stateStack.Pop();
				break;
			}
			case 222: {
				if (t.kind == 52) {
					goto case 221;
				} else {
					goto case 224;
				}
			}
			case 223: {
				Expect(53, t); // "Auto"
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				if (t.kind == 53) {
					goto case 223;
				} else {
					goto case 226;
				}
			}
			case 225: {
				Expect(54, t); // "Binary"
				currentState = stateStack.Pop();
				break;
			}
			case 226: {
				if (t.kind == 54) {
					goto case 225;
				} else {
					goto case 228;
				}
			}
			case 227: {
				Expect(57, t); // "By"
				currentState = stateStack.Pop();
				break;
			}
			case 228: {
				if (t.kind == 57) {
					goto case 227;
				} else {
					goto case 230;
				}
			}
			case 229: {
				Expect(74, t); // "Compare"
				currentState = stateStack.Pop();
				break;
			}
			case 230: {
				if (t.kind == 74) {
					goto case 229;
				} else {
					goto case 232;
				}
			}
			case 231: {
				Expect(91, t); // "Descending"
				currentState = stateStack.Pop();
				break;
			}
			case 232: {
				if (t.kind == 91) {
					goto case 231;
				} else {
					goto case 234;
				}
			}
			case 233: {
				Expect(94, t); // "Distinct"
				currentState = stateStack.Pop();
				break;
			}
			case 234: {
				if (t.kind == 94) {
					goto case 233;
				} else {
					goto case 236;
				}
			}
			case 235: {
				Expect(103, t); // "Equals"
				currentState = stateStack.Pop();
				break;
			}
			case 236: {
				if (t.kind == 103) {
					goto case 235;
				} else {
					goto case 238;
				}
			}
			case 237: {
				Expect(108, t); // "Explicit"
				currentState = stateStack.Pop();
				break;
			}
			case 238: {
				if (t.kind == 108) {
					goto case 237;
				} else {
					goto case 240;
				}
			}
			case 239: {
				Expect(113, t); // "From"
				currentState = stateStack.Pop();
				break;
			}
			case 240: {
				if (t.kind == 113) {
					goto case 239;
				} else {
					goto case 242;
				}
			}
			case 241: {
				Expect(120, t); // "Group"
				currentState = stateStack.Pop();
				break;
			}
			case 242: {
				if (t.kind == 120) {
					goto case 241;
				} else {
					goto case 244;
				}
			}
			case 243: {
				Expect(126, t); // "Infer"
				currentState = stateStack.Pop();
				break;
			}
			case 244: {
				if (t.kind == 126) {
					goto case 243;
				} else {
					goto case 246;
				}
			}
			case 245: {
				Expect(130, t); // "Into"
				currentState = stateStack.Pop();
				break;
			}
			case 246: {
				if (t.kind == 130) {
					goto case 245;
				} else {
					goto case 248;
				}
			}
			case 247: {
				Expect(133, t); // "Join"
				currentState = stateStack.Pop();
				break;
			}
			case 248: {
				if (t.kind == 133) {
					goto case 247;
				} else {
					goto case 250;
				}
			}
			case 249: {
				Expect(156, t); // "Off"
				currentState = stateStack.Pop();
				break;
			}
			case 250: {
				if (t.kind == 156) {
					goto case 249;
				} else {
					goto case 252;
				}
			}
			case 251: {
				Expect(162, t); // "Order"
				currentState = stateStack.Pop();
				break;
			}
			case 252: {
				if (t.kind == 162) {
					goto case 251;
				} else {
					goto case 254;
				}
			}
			case 253: {
				Expect(169, t); // "Preserve"
				currentState = stateStack.Pop();
				break;
			}
			case 254: {
				if (t.kind == 169) {
					goto case 253;
				} else {
					goto case 256;
				}
			}
			case 255: {
				Expect(188, t); // "Skip"
				currentState = stateStack.Pop();
				break;
			}
			case 256: {
				if (t.kind == 188) {
					goto case 255;
				} else {
					goto case 258;
				}
			}
			case 257: {
				Expect(197, t); // "Take"
				currentState = stateStack.Pop();
				break;
			}
			case 258: {
				if (t.kind == 197) {
					goto case 257;
				} else {
					goto case 260;
				}
			}
			case 259: {
				Expect(198, t); // "Text"
				currentState = stateStack.Pop();
				break;
			}
			case 260: {
				if (t.kind == 198) {
					goto case 259;
				} else {
					goto case 262;
				}
			}
			case 261: {
				Expect(208, t); // "Unicode"
				currentState = stateStack.Pop();
				break;
			}
			case 262: {
				if (t.kind == 208) {
					goto case 261;
				} else {
					goto case 264;
				}
			}
			case 263: {
				Expect(209, t); // "Until"
				currentState = stateStack.Pop();
				break;
			}
			case 264: {
				if (t.kind == 209) {
					goto case 263;
				} else {
					goto case 266;
				}
			}
			case 265: {
				Expect(215, t); // "Where"
				currentState = stateStack.Pop();
				break;
			}
			case 266: {
				if (t.kind == 215) {
					goto case 265;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 267: {
				Expect(173, t); // "Public"
				currentState = stateStack.Pop();
				break;
			}
			case 268: {
				Expect(112, t); // "Friend"
				currentState = stateStack.Pop();
				break;
			}
			case 269: { // start of AccessModifier
				if (t.kind == 173) {
					goto case 267;
				} else {
					goto case 270;
				}
			}
			case 270: {
				if (t.kind == 112) {
					goto case 268;
				} else {
					goto case 272;
				}
			}
			case 271: {
				Expect(172, t); // "Protected"
				currentState = stateStack.Pop();
				break;
			}
			case 272: {
				if (t.kind == 172) {
					goto case 271;
				} else {
					goto case 274;
				}
			}
			case 273: {
				Expect(170, t); // "Private"
				currentState = stateStack.Pop();
				break;
			}
			case 274: {
				if (t.kind == 170) {
					goto case 273;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 275: {
				goto case 269; // AccessModifier
			}
			case 276: {
				Expect(184, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 277: { // start of TypeModifier
				if (set[14, t.kind]) {
					goto case 275;
				} else {
					goto case 278;
				}
			}
			case 278: {
				if (t.kind == 184) {
					goto case 276;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 279: {
				goto case 269; // AccessModifier
			}
			case 280: {
				Expect(184, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 281: { // start of MemberModifier
				if (set[14, t.kind]) {
					goto case 279;
				} else {
					goto case 282;
				}
			}
			case 282: {
				if (t.kind == 184) {
					goto case 280;
				} else {
					goto case 284;
				}
			}
			case 283: {
				Expect(185, t); // "Shared"
				currentState = stateStack.Pop();
				break;
			}
			case 284: {
				if (t.kind == 185) {
					goto case 283;
				} else {
					goto case 286;
				}
			}
			case 285: {
				Expect(165, t); // "Overridable"
				currentState = stateStack.Pop();
				break;
			}
			case 286: {
				if (t.kind == 165) {
					goto case 285;
				} else {
					goto case 288;
				}
			}
			case 287: {
				Expect(153, t); // "NotOverridable"
				currentState = stateStack.Pop();
				break;
			}
			case 288: {
				if (t.kind == 153) {
					goto case 287;
				} else {
					goto case 290;
				}
			}
			case 289: {
				Expect(166, t); // "Overrides"
				currentState = stateStack.Pop();
				break;
			}
			case 290: {
				if (t.kind == 166) {
					goto case 289;
				} else {
					goto case 292;
				}
			}
			case 291: {
				Expect(164, t); // "Overloads"
				currentState = stateStack.Pop();
				break;
			}
			case 292: {
				if (t.kind == 164) {
					goto case 291;
				} else {
					goto case 294;
				}
			}
			case 293: {
				Expect(168, t); // "Partial"
				currentState = stateStack.Pop();
				break;
			}
			case 294: {
				if (t.kind == 168) {
					goto case 293;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 295: {
				Expect(59, t); // "ByVal"
				currentState = stateStack.Pop();
				break;
			}
			case 296: {
				Expect(56, t); // "ByRef"
				currentState = stateStack.Pop();
				break;
			}
			case 297: { // start of ParameterModifier
				if (t.kind == 59) {
					goto case 295;
				} else {
					goto case 298;
				}
			}
			case 298: {
				if (t.kind == 56) {
					goto case 296;
				} else {
					goto case 300;
				}
			}
			case 299: {
				Expect(160, t); // "Optional"
				currentState = stateStack.Pop();
				break;
			}
			case 300: {
				if (t.kind == 160) {
					goto case 299;
				} else {
					goto case 302;
				}
			}
			case 301: {
				Expect(167, t); // "ParamArray"
				currentState = stateStack.Pop();
				break;
			}
			case 302: {
				if (t.kind == 167) {
					goto case 301;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
		}

	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,T,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};

} // end Parser


}