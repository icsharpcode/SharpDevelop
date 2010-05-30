using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;



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
				goto case 34; // NamespaceMemberDeclaration
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
				currentState = 25;
				break;
			}
			case 25: {
				PushContext(Context.Attribute);
				goto case 27;
			}
			case 26: {
				currentState = 27;
				break;
			}
			case 27: {
				if (set[2, t.kind]) {
					goto case 26;
				} else {
					goto case 28;
				}
			}
			case 28: {
				Expect(27, t); // ">"
				currentState = 29;
				break;
			}
			case 29: {
				PopContext();
				goto case 31;
			}
			case 30: {
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 31: {
				if (t.kind == 1) {
					goto case 30;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 32: {
				goto case 36; // NamespaceDeclaration
			}
			case 33: {
				goto case 46; // TypeDeclaration
			}
			case 34: { // start of NamespaceMemberDeclaration
				if (t.kind == 146) {
					goto case 32;
				} else {
					goto case 35;
				}
			}
			case 35: {
				if (set[3, t.kind]) {
					goto case 33;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 36: { // start of NamespaceDeclaration
				Expect(146, t); // "Namespace"
				currentState = 38;
				break;
			}
			case 37: {
				currentState = 38;
				break;
			}
			case 38: {
				if (set[1, t.kind]) {
					goto case 37;
				} else {
					goto case 39;
				}
			}
			case 39: {
				stateStack.Push(41);
				goto case 13; // StatementTerminator
			}
			case 40: {
				stateStack.Push(41);
				goto case 34; // NamespaceMemberDeclaration
			}
			case 41: {
				if (set[4, t.kind]) {
					goto case 40;
				} else {
					goto case 42;
				}
			}
			case 42: {
				Expect(100, t); // "End"
				currentState = 43;
				break;
			}
			case 43: {
				Expect(146, t); // "Namespace"
				currentState = 44;
				break;
			}
			case 44: {
				goto case 13; // StatementTerminator
			}
			case 45: {
				stateStack.Push(46);
				goto case 24; // AttributeBlock
			}
			case 46: { // start of TypeDeclaration
				if (t.kind == 28) {
					goto case 45;
				} else {
					goto case 48;
				}
			}
			case 47: {
				stateStack.Push(48);
				goto case 357; // TypeModifier
			}
			case 48: {
				if (set[5, t.kind]) {
					goto case 47;
				} else {
					goto case 51;
				}
			}
			case 49: {
				Expect(141, t); // "Module"
				currentState = 54;
				break;
			}
			case 50: {
				Expect(71, t); // "Class"
				currentState = 54;
				break;
			}
			case 51: {
				if (t.kind == 141) {
					goto case 49;
				} else {
					goto case 52;
				}
			}
			case 52: {
				if (t.kind == 71) {
					goto case 50;
				} else {
					Error(t);
					goto case 54;
				}
			}
			case 53: {
				currentState = 54;
				break;
			}
			case 54: {
				if (set[1, t.kind]) {
					goto case 53;
				} else {
					goto case 55;
				}
			}
			case 55: {
				stateStack.Push(56);
				goto case 13; // StatementTerminator
			}
			case 56: {
				PushContext(Context.Type);
				goto case 58;
			}
			case 57: {
				stateStack.Push(58);
				goto case 66; // MemberDeclaration
			}
			case 58: {
				if (set[6, t.kind]) {
					goto case 57;
				} else {
					goto case 59;
				}
			}
			case 59: {
				Expect(100, t); // "End"
				currentState = 62;
				break;
			}
			case 60: {
				Expect(141, t); // "Module"
				currentState = 64;
				break;
			}
			case 61: {
				Expect(71, t); // "Class"
				currentState = 64;
				break;
			}
			case 62: {
				if (t.kind == 141) {
					goto case 60;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (t.kind == 71) {
					goto case 61;
				} else {
					Error(t);
					goto case 64;
				}
			}
			case 64: {
				stateStack.Push(65);
				goto case 13; // StatementTerminator
			}
			case 65: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 66: { // start of MemberDeclaration
				PushContext(Context.Member);
				goto case 68;
			}
			case 67: {
				stateStack.Push(68);
				goto case 24; // AttributeBlock
			}
			case 68: {
				if (t.kind == 28) {
					goto case 67;
				} else {
					goto case 70;
				}
			}
			case 69: {
				stateStack.Push(70);
				goto case 361; // MemberModifier
			}
			case 70: {
				if (set[7, t.kind]) {
					goto case 69;
				} else {
					goto case 73;
				}
			}
			case 71: {
				stateStack.Push(75);
				goto case 99; // MemberVariableOrConstantDeclaration
			}
			case 72: {
				stateStack.Push(75);
				goto case 78; // SubOrFunctionDeclaration
			}
			case 73: {
				if (set[8, t.kind]) {
					goto case 71;
				} else {
					goto case 74;
				}
			}
			case 74: {
				if (t.kind == 114 || t.kind == 195) {
					goto case 72;
				} else {
					Error(t);
					goto case 75;
				}
			}
			case 75: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 76: {
				Expect(195, t); // "Sub"
				currentState = 80;
				break;
			}
			case 77: {
				Expect(114, t); // "Function"
				currentState = 80;
				break;
			}
			case 78: { // start of SubOrFunctionDeclaration
				if (t.kind == 195) {
					goto case 76;
				} else {
					goto case 79;
				}
			}
			case 79: {
				if (t.kind == 114) {
					goto case 77;
				} else {
					Error(t);
					goto case 80;
				}
			}
			case 80: {
				PushContext(Context.IdentifierExpected);
				goto case 81;
			}
			case 81: {
				currentState = 82;
				break;
			}
			case 82: {
				PopContext();
				goto case 87;
			}
			case 83: {
				Expect(25, t); // "("
				currentState = 85;
				break;
			}
			case 84: {
				stateStack.Push(86);
				goto case 108; // ParameterList
			}
			case 85: {
				if (set[9, t.kind]) {
					goto case 84;
				} else {
					goto case 86;
				}
			}
			case 86: {
				Expect(26, t); // ")"
				currentState = 90;
				break;
			}
			case 87: {
				if (t.kind == 25) {
					goto case 83;
				} else {
					goto case 90;
				}
			}
			case 88: {
				Expect(50, t); // "As"
				currentState = 89;
				break;
			}
			case 89: {
				stateStack.Push(91);
				goto case 196; // TypeName
			}
			case 90: {
				if (t.kind == 50) {
					goto case 88;
				} else {
					goto case 91;
				}
			}
			case 91: {
				stateStack.Push(92);
				goto case 123; // Block
			}
			case 92: {
				Expect(100, t); // "End"
				currentState = 95;
				break;
			}
			case 93: {
				Expect(195, t); // "Sub"
				currentState = 97;
				break;
			}
			case 94: {
				Expect(114, t); // "Function"
				currentState = 97;
				break;
			}
			case 95: {
				if (t.kind == 195) {
					goto case 93;
				} else {
					goto case 96;
				}
			}
			case 96: {
				if (t.kind == 114) {
					goto case 94;
				} else {
					Error(t);
					goto case 97;
				}
			}
			case 97: {
				goto case 13; // StatementTerminator
			}
			case 98: {
				Expect(75, t); // "Const"
				currentState = 100;
				break;
			}
			case 99: { // start of MemberVariableOrConstantDeclaration
				if (t.kind == 75) {
					goto case 98;
				} else {
					goto case 100;
				}
			}
			case 100: {
				stateStack.Push(103);
				goto case 287; // Identifier
			}
			case 101: {
				Expect(50, t); // "As"
				currentState = 102;
				break;
			}
			case 102: {
				stateStack.Push(106);
				goto case 196; // TypeName
			}
			case 103: {
				if (t.kind == 50) {
					goto case 101;
				} else {
					goto case 106;
				}
			}
			case 104: {
				Expect(10, t); // "="
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(107);
				goto case 132; // Expression
			}
			case 106: {
				if (t.kind == 10) {
					goto case 104;
				} else {
					goto case 107;
				}
			}
			case 107: {
				goto case 13; // StatementTerminator
			}
			case 108: { // start of ParameterList
				stateStack.Push(111);
				goto case 113; // Parameter
			}
			case 109: {
				Expect(12, t); // ","
				currentState = 110;
				break;
			}
			case 110: {
				stateStack.Push(111);
				goto case 113; // Parameter
			}
			case 111: {
				if (t.kind == 12) {
					goto case 109;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 112: {
				stateStack.Push(113);
				goto case 24; // AttributeBlock
			}
			case 113: { // start of Parameter
				if (t.kind == 28) {
					goto case 112;
				} else {
					goto case 115;
				}
			}
			case 114: {
				stateStack.Push(115);
				goto case 381; // ParameterModifier
			}
			case 115: {
				if (set[10, t.kind]) {
					goto case 114;
				} else {
					goto case 116;
				}
			}
			case 116: {
				stateStack.Push(119);
				goto case 287; // Identifier
			}
			case 117: {
				Expect(50, t); // "As"
				currentState = 118;
				break;
			}
			case 118: {
				stateStack.Push(122);
				goto case 196; // TypeName
			}
			case 119: {
				if (t.kind == 50) {
					goto case 117;
				} else {
					goto case 122;
				}
			}
			case 120: {
				Expect(10, t); // "="
				currentState = 121;
				break;
			}
			case 121: {
				goto case 132; // Expression
			}
			case 122: {
				if (t.kind == 10) {
					goto case 120;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 123: { // start of Block
				PushContext(Context.Body);
				goto case 124;
			}
			case 124: {
				stateStack.Push(126);
				goto case 13; // StatementTerminator
			}
			case 125: {
				stateStack.Push(126);
				goto case 13; // StatementTerminator
			}
			case 126: {
				if (t.kind == 1 || t.kind == 11) {
					goto case 125;
				} else {
					goto case 128;
				}
			}
			case 127: {
				stateStack.Push(128);
				goto case 285; // Statement
			}
			case 128: {
				if (set[11, t.kind]) {
					goto case 127;
				} else {
					goto case 130;
				}
			}
			case 129: {
				stateStack.Push(130);
				goto case 13; // StatementTerminator
			}
			case 130: {
				if (t.kind == 1 || t.kind == 11) {
					goto case 129;
				} else {
					goto case 131;
				}
			}
			case 131: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 132: { // start of Expression
				isExpressionStart = true;
				goto case 139;
			}
			case 133: {
				stateStack.Push(134);
				goto case 221; // Literal
			}
			case 134: {
				isExpressionStart = false;
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 135: {
				Expect(25, t); // "("
				currentState = 136;
				break;
			}
			case 136: {
				isExpressionStart = false;
				goto case 137;
			}
			case 137: {
				stateStack.Push(138);
				goto case 132; // Expression
			}
			case 138: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 139: {
				if (set[12, t.kind]) {
					goto case 133;
				} else {
					goto case 140;
				}
			}
			case 140: {
				if (t.kind == 25) {
					goto case 135;
				} else {
					goto case 151;
				}
			}
			case 141: {
				stateStack.Push(142);
				goto case 287; // Identifier
			}
			case 142: {
				isExpressionStart = false;
				goto case 150;
			}
			case 143: {
				Expect(25, t); // "("
				currentState = 144;
				break;
			}
			case 144: {
				Expect(155, t); // "Of"
				currentState = 145;
				break;
			}
			case 145: {
				stateStack.Push(148);
				goto case 196; // TypeName
			}
			case 146: {
				Expect(12, t); // ","
				currentState = 147;
				break;
			}
			case 147: {
				stateStack.Push(148);
				goto case 196; // TypeName
			}
			case 148: {
				if (t.kind == 12) {
					goto case 146;
				} else {
					goto case 149;
				}
			}
			case 149: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 150: {
				if (t.kind == 25) {
					goto case 143;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 151: {
				if (set[13, t.kind]) {
					goto case 141;
				} else {
					goto case 155;
				}
			}
			case 152: {
				Expect(44, t); // "AddressOf"
				currentState = 153;
				break;
			}
			case 153: {
				isExpressionStart = false;
				goto case 154;
			}
			case 154: {
				goto case 132; // Expression
			}
			case 155: {
				if (t.kind == 44) {
					goto case 152;
				} else {
					goto case 161;
				}
			}
			case 156: {
				Expect(28, t); // "<"
				currentState = 157;
				break;
			}
			case 157: {
				PushContext(Context.Xml);
				goto case 158;
			}
			case 158: {
				currentState = 159;
				break;
			}
			case 159: {
				Expect(27, t); // ">"
				currentState = 160;
				break;
			}
			case 160: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 161: {
				if (t.kind == 28) {
					goto case 156;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 162: {
				Expect(58, t); // "Byte"
				currentState = stateStack.Pop();
				break;
			}
			case 163: {
				Expect(181, t); // "SByte"
				currentState = stateStack.Pop();
				break;
			}
			case 164: { // start of PrimitiveTypeName
				if (t.kind == 58) {
					goto case 162;
				} else {
					goto case 165;
				}
			}
			case 165: {
				if (t.kind == 181) {
					goto case 163;
				} else {
					goto case 167;
				}
			}
			case 166: {
				Expect(210, t); // "UShort"
				currentState = stateStack.Pop();
				break;
			}
			case 167: {
				if (t.kind == 210) {
					goto case 166;
				} else {
					goto case 169;
				}
			}
			case 168: {
				Expect(186, t); // "Short"
				currentState = stateStack.Pop();
				break;
			}
			case 169: {
				if (t.kind == 186) {
					goto case 168;
				} else {
					goto case 171;
				}
			}
			case 170: {
				Expect(206, t); // "UInteger"
				currentState = stateStack.Pop();
				break;
			}
			case 171: {
				if (t.kind == 206) {
					goto case 170;
				} else {
					goto case 173;
				}
			}
			case 172: {
				Expect(128, t); // "Integer"
				currentState = stateStack.Pop();
				break;
			}
			case 173: {
				if (t.kind == 128) {
					goto case 172;
				} else {
					goto case 175;
				}
			}
			case 174: {
				Expect(207, t); // "ULong"
				currentState = stateStack.Pop();
				break;
			}
			case 175: {
				if (t.kind == 207) {
					goto case 174;
				} else {
					goto case 177;
				}
			}
			case 176: {
				Expect(137, t); // "Long"
				currentState = stateStack.Pop();
				break;
			}
			case 177: {
				if (t.kind == 137) {
					goto case 176;
				} else {
					goto case 179;
				}
			}
			case 178: {
				Expect(187, t); // "Single"
				currentState = stateStack.Pop();
				break;
			}
			case 179: {
				if (t.kind == 187) {
					goto case 178;
				} else {
					goto case 181;
				}
			}
			case 180: {
				Expect(96, t); // "Double"
				currentState = stateStack.Pop();
				break;
			}
			case 181: {
				if (t.kind == 96) {
					goto case 180;
				} else {
					goto case 183;
				}
			}
			case 182: {
				Expect(87, t); // "Decimal"
				currentState = stateStack.Pop();
				break;
			}
			case 183: {
				if (t.kind == 87) {
					goto case 182;
				} else {
					goto case 185;
				}
			}
			case 184: {
				Expect(55, t); // "Boolean"
				currentState = stateStack.Pop();
				break;
			}
			case 185: {
				if (t.kind == 55) {
					goto case 184;
				} else {
					goto case 187;
				}
			}
			case 186: {
				Expect(86, t); // "Date"
				currentState = stateStack.Pop();
				break;
			}
			case 187: {
				if (t.kind == 86) {
					goto case 186;
				} else {
					goto case 189;
				}
			}
			case 188: {
				Expect(69, t); // "Char"
				currentState = stateStack.Pop();
				break;
			}
			case 189: {
				if (t.kind == 69) {
					goto case 188;
				} else {
					goto case 191;
				}
			}
			case 190: {
				Expect(193, t); // "String"
				currentState = stateStack.Pop();
				break;
			}
			case 191: {
				if (t.kind == 193) {
					goto case 190;
				} else {
					goto case 193;
				}
			}
			case 192: {
				Expect(154, t); // "Object"
				currentState = stateStack.Pop();
				break;
			}
			case 193: {
				if (t.kind == 154) {
					goto case 192;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 194: {
				Expect(117, t); // "Global"
				currentState = 201;
				break;
			}
			case 195: {
				stateStack.Push(201);
				goto case 287; // Identifier
			}
			case 196: { // start of TypeName
				if (t.kind == 117) {
					goto case 194;
				} else {
					goto case 197;
				}
			}
			case 197: {
				if (set[13, t.kind]) {
					goto case 195;
				} else {
					goto case 199;
				}
			}
			case 198: {
				stateStack.Push(201);
				goto case 164; // PrimitiveTypeName
			}
			case 199: {
				if (set[14, t.kind]) {
					goto case 198;
				} else {
					Error(t);
					goto case 201;
				}
			}
			case 200: {
				stateStack.Push(201);
				goto case 207; // TypeSuffix
			}
			case 201: {
				if (t.kind == 25) {
					goto case 200;
				} else {
					goto case 206;
				}
			}
			case 202: {
				Expect(16, t); // "."
				currentState = 203;
				break;
			}
			case 203: {
				stateStack.Push(205);
				goto case 218; // IdentifierOrKeyword
			}
			case 204: {
				stateStack.Push(205);
				goto case 207; // TypeSuffix
			}
			case 205: {
				if (t.kind == 25) {
					goto case 204;
				} else {
					goto case 206;
				}
			}
			case 206: {
				if (t.kind == 16) {
					goto case 202;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 207: { // start of TypeSuffix
				Expect(25, t); // "("
				currentState = 215;
				break;
			}
			case 208: {
				Expect(155, t); // "Of"
				currentState = 209;
				break;
			}
			case 209: {
				stateStack.Push(212);
				goto case 196; // TypeName
			}
			case 210: {
				Expect(12, t); // ","
				currentState = 211;
				break;
			}
			case 211: {
				stateStack.Push(212);
				goto case 196; // TypeName
			}
			case 212: {
				if (t.kind == 12) {
					goto case 210;
				} else {
					goto case 217;
				}
			}
			case 213: {
				Expect(12, t); // ","
				currentState = 214;
				break;
			}
			case 214: {
				if (t.kind == 12) {
					goto case 213;
				} else {
					goto case 217;
				}
			}
			case 215: {
				if (t.kind == 155) {
					goto case 208;
				} else {
					goto case 216;
				}
			}
			case 216: {
				if (t.kind == 12 || t.kind == 26) {
					goto case 214;
				} else {
					Error(t);
					goto case 217;
				}
			}
			case 217: {
				Expect(26, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 218: { // start of IdentifierOrKeyword
				currentState = stateStack.Pop();
				break;
			}
			case 219: {
				Expect(3, t); // LiteralString
				currentState = stateStack.Pop();
				break;
			}
			case 220: {
				Expect(4, t); // LiteralCharacter
				currentState = stateStack.Pop();
				break;
			}
			case 221: { // start of Literal
				if (t.kind == 3) {
					goto case 219;
				} else {
					goto case 222;
				}
			}
			case 222: {
				if (t.kind == 4) {
					goto case 220;
				} else {
					goto case 224;
				}
			}
			case 223: {
				Expect(5, t); // LiteralInteger
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				if (t.kind == 5) {
					goto case 223;
				} else {
					goto case 226;
				}
			}
			case 225: {
				Expect(6, t); // LiteralDouble
				currentState = stateStack.Pop();
				break;
			}
			case 226: {
				if (t.kind == 6) {
					goto case 225;
				} else {
					goto case 228;
				}
			}
			case 227: {
				Expect(7, t); // LiteralSingle
				currentState = stateStack.Pop();
				break;
			}
			case 228: {
				if (t.kind == 7) {
					goto case 227;
				} else {
					goto case 230;
				}
			}
			case 229: {
				Expect(8, t); // LiteralDecimal
				currentState = stateStack.Pop();
				break;
			}
			case 230: {
				if (t.kind == 8) {
					goto case 229;
				} else {
					goto case 232;
				}
			}
			case 231: {
				Expect(9, t); // LiteralDate
				currentState = stateStack.Pop();
				break;
			}
			case 232: {
				if (t.kind == 9) {
					goto case 231;
				} else {
					goto case 234;
				}
			}
			case 233: {
				Expect(202, t); // "True"
				currentState = stateStack.Pop();
				break;
			}
			case 234: {
				if (t.kind == 202) {
					goto case 233;
				} else {
					goto case 236;
				}
			}
			case 235: {
				Expect(109, t); // "False"
				currentState = stateStack.Pop();
				break;
			}
			case 236: {
				if (t.kind == 109) {
					goto case 235;
				} else {
					goto case 238;
				}
			}
			case 237: {
				Expect(151, t); // "Nothing"
				currentState = stateStack.Pop();
				break;
			}
			case 238: {
				if (t.kind == 151) {
					goto case 237;
				} else {
					goto case 240;
				}
			}
			case 239: {
				Expect(139, t); // "Me"
				currentState = stateStack.Pop();
				break;
			}
			case 240: {
				if (t.kind == 139) {
					goto case 239;
				} else {
					goto case 242;
				}
			}
			case 241: {
				Expect(144, t); // "MyBase"
				currentState = stateStack.Pop();
				break;
			}
			case 242: {
				if (t.kind == 144) {
					goto case 241;
				} else {
					goto case 244;
				}
			}
			case 243: {
				Expect(145, t); // "MyClass"
				currentState = stateStack.Pop();
				break;
			}
			case 244: {
				if (t.kind == 145) {
					goto case 243;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 245: {
				stateStack.Push(249);
				goto case 287; // Identifier
			}
			case 246: {
				Expect(5, t); // LiteralInteger
				currentState = 249;
				break;
			}
			case 247: {
				if (set[13, t.kind]) {
					goto case 245;
				} else {
					goto case 248;
				}
			}
			case 248: {
				if (t.kind == 5) {
					goto case 246;
				} else {
					Error(t);
					goto case 249;
				}
			}
			case 249: {
				Expect(11, t); // ":"
				currentState = stateStack.Pop();
				break;
			}
			case 250: {
				Expect(92, t); // "Dim"
				currentState = 256;
				break;
			}
			case 251: {
				Expect(189, t); // "Static"
				currentState = 256;
				break;
			}
			case 252: {
				if (t.kind == 92) {
					goto case 250;
				} else {
					goto case 253;
				}
			}
			case 253: {
				if (t.kind == 189) {
					goto case 251;
				} else {
					goto case 255;
				}
			}
			case 254: {
				Expect(75, t); // "Const"
				currentState = 256;
				break;
			}
			case 255: {
				if (t.kind == 75) {
					goto case 254;
				} else {
					Error(t);
					goto case 256;
				}
			}
			case 256: {
				stateStack.Push(258);
				goto case 287; // Identifier
			}
			case 257: {
				Expect(21, t); // "?"
				currentState = 263;
				break;
			}
			case 258: {
				if (t.kind == 21) {
					goto case 257;
				} else {
					goto case 263;
				}
			}
			case 259: {
				Expect(25, t); // "("
				currentState = 261;
				break;
			}
			case 260: {
				Expect(12, t); // ","
				currentState = 261;
				break;
			}
			case 261: {
				if (t.kind == 12) {
					goto case 260;
				} else {
					goto case 262;
				}
			}
			case 262: {
				Expect(26, t); // ")"
				currentState = 273;
				break;
			}
			case 263: {
				if (t.kind == 25) {
					goto case 259;
				} else {
					goto case 273;
				}
			}
			case 264: {
				Expect(12, t); // ","
				currentState = 265;
				break;
			}
			case 265: {
				stateStack.Push(267);
				goto case 287; // Identifier
			}
			case 266: {
				Expect(21, t); // "?"
				currentState = 272;
				break;
			}
			case 267: {
				if (t.kind == 21) {
					goto case 266;
				} else {
					goto case 272;
				}
			}
			case 268: {
				Expect(25, t); // "("
				currentState = 270;
				break;
			}
			case 269: {
				Expect(12, t); // ","
				currentState = 270;
				break;
			}
			case 270: {
				if (t.kind == 12) {
					goto case 269;
				} else {
					goto case 271;
				}
			}
			case 271: {
				Expect(26, t); // ")"
				currentState = 273;
				break;
			}
			case 272: {
				if (t.kind == 25) {
					goto case 268;
				} else {
					goto case 273;
				}
			}
			case 273: {
				if (t.kind == 12) {
					goto case 264;
				} else {
					goto case 281;
				}
			}
			case 274: {
				Expect(50, t); // "As"
				currentState = 276;
				break;
			}
			case 275: {
				Expect(148, t); // "New"
				currentState = 277;
				break;
			}
			case 276: {
				if (t.kind == 148) {
					goto case 275;
				} else {
					goto case 277;
				}
			}
			case 277: {
				stateStack.Push(280);
				goto case 196; // TypeName
			}
			case 278: {
				Expect(25, t); // "("
				currentState = 279;
				break;
			}
			case 279: {
				Expect(26, t); // ")"
				currentState = 284;
				break;
			}
			case 280: {
				if (t.kind == 25) {
					goto case 278;
				} else {
					goto case 284;
				}
			}
			case 281: {
				if (t.kind == 50) {
					goto case 274;
				} else {
					goto case 284;
				}
			}
			case 282: {
				Expect(10, t); // "="
				currentState = 283;
				break;
			}
			case 283: {
				goto case 132; // Expression
			}
			case 284: {
				if (t.kind == 10) {
					goto case 282;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 285: { // start of Statement
				if (set[15, t.kind]) {
					goto case 247;
				} else {
					goto case 286;
				}
			}
			case 286: {
				if (t.kind == 75 || t.kind == 92 || t.kind == 189) {
					goto case 252;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 287: { // start of Identifier
				PushContext(Context.IdentifierExpected);
				goto case 290;
			}
			case 288: {
				stateStack.Push(292);
				goto case 295; // IdentifierForFieldDeclaration
			}
			case 289: {
				Expect(85, t); // "Custom"
				currentState = 292;
				break;
			}
			case 290: {
				if (set[16, t.kind]) {
					goto case 288;
				} else {
					goto case 291;
				}
			}
			case 291: {
				if (t.kind == 85) {
					goto case 289;
				} else {
					Error(t);
					goto case 292;
				}
			}
			case 292: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 293: {
				Expect(2, t); // ident
				currentState = stateStack.Pop();
				break;
			}
			case 294: {
				Expect(45, t); // "Aggregate"
				currentState = stateStack.Pop();
				break;
			}
			case 295: { // start of IdentifierForFieldDeclaration
				if (t.kind == 2) {
					goto case 293;
				} else {
					goto case 296;
				}
			}
			case 296: {
				if (t.kind == 45) {
					goto case 294;
				} else {
					goto case 298;
				}
			}
			case 297: {
				Expect(49, t); // "Ansi"
				currentState = stateStack.Pop();
				break;
			}
			case 298: {
				if (t.kind == 49) {
					goto case 297;
				} else {
					goto case 300;
				}
			}
			case 299: {
				Expect(51, t); // "Ascending"
				currentState = stateStack.Pop();
				break;
			}
			case 300: {
				if (t.kind == 51) {
					goto case 299;
				} else {
					goto case 302;
				}
			}
			case 301: {
				Expect(52, t); // "Assembly"
				currentState = stateStack.Pop();
				break;
			}
			case 302: {
				if (t.kind == 52) {
					goto case 301;
				} else {
					goto case 304;
				}
			}
			case 303: {
				Expect(53, t); // "Auto"
				currentState = stateStack.Pop();
				break;
			}
			case 304: {
				if (t.kind == 53) {
					goto case 303;
				} else {
					goto case 306;
				}
			}
			case 305: {
				Expect(54, t); // "Binary"
				currentState = stateStack.Pop();
				break;
			}
			case 306: {
				if (t.kind == 54) {
					goto case 305;
				} else {
					goto case 308;
				}
			}
			case 307: {
				Expect(57, t); // "By"
				currentState = stateStack.Pop();
				break;
			}
			case 308: {
				if (t.kind == 57) {
					goto case 307;
				} else {
					goto case 310;
				}
			}
			case 309: {
				Expect(74, t); // "Compare"
				currentState = stateStack.Pop();
				break;
			}
			case 310: {
				if (t.kind == 74) {
					goto case 309;
				} else {
					goto case 312;
				}
			}
			case 311: {
				Expect(91, t); // "Descending"
				currentState = stateStack.Pop();
				break;
			}
			case 312: {
				if (t.kind == 91) {
					goto case 311;
				} else {
					goto case 314;
				}
			}
			case 313: {
				Expect(94, t); // "Distinct"
				currentState = stateStack.Pop();
				break;
			}
			case 314: {
				if (t.kind == 94) {
					goto case 313;
				} else {
					goto case 316;
				}
			}
			case 315: {
				Expect(103, t); // "Equals"
				currentState = stateStack.Pop();
				break;
			}
			case 316: {
				if (t.kind == 103) {
					goto case 315;
				} else {
					goto case 318;
				}
			}
			case 317: {
				Expect(108, t); // "Explicit"
				currentState = stateStack.Pop();
				break;
			}
			case 318: {
				if (t.kind == 108) {
					goto case 317;
				} else {
					goto case 320;
				}
			}
			case 319: {
				Expect(113, t); // "From"
				currentState = stateStack.Pop();
				break;
			}
			case 320: {
				if (t.kind == 113) {
					goto case 319;
				} else {
					goto case 322;
				}
			}
			case 321: {
				Expect(120, t); // "Group"
				currentState = stateStack.Pop();
				break;
			}
			case 322: {
				if (t.kind == 120) {
					goto case 321;
				} else {
					goto case 324;
				}
			}
			case 323: {
				Expect(126, t); // "Infer"
				currentState = stateStack.Pop();
				break;
			}
			case 324: {
				if (t.kind == 126) {
					goto case 323;
				} else {
					goto case 326;
				}
			}
			case 325: {
				Expect(130, t); // "Into"
				currentState = stateStack.Pop();
				break;
			}
			case 326: {
				if (t.kind == 130) {
					goto case 325;
				} else {
					goto case 328;
				}
			}
			case 327: {
				Expect(133, t); // "Join"
				currentState = stateStack.Pop();
				break;
			}
			case 328: {
				if (t.kind == 133) {
					goto case 327;
				} else {
					goto case 330;
				}
			}
			case 329: {
				Expect(156, t); // "Off"
				currentState = stateStack.Pop();
				break;
			}
			case 330: {
				if (t.kind == 156) {
					goto case 329;
				} else {
					goto case 332;
				}
			}
			case 331: {
				Expect(162, t); // "Order"
				currentState = stateStack.Pop();
				break;
			}
			case 332: {
				if (t.kind == 162) {
					goto case 331;
				} else {
					goto case 334;
				}
			}
			case 333: {
				Expect(169, t); // "Preserve"
				currentState = stateStack.Pop();
				break;
			}
			case 334: {
				if (t.kind == 169) {
					goto case 333;
				} else {
					goto case 336;
				}
			}
			case 335: {
				Expect(188, t); // "Skip"
				currentState = stateStack.Pop();
				break;
			}
			case 336: {
				if (t.kind == 188) {
					goto case 335;
				} else {
					goto case 338;
				}
			}
			case 337: {
				Expect(197, t); // "Take"
				currentState = stateStack.Pop();
				break;
			}
			case 338: {
				if (t.kind == 197) {
					goto case 337;
				} else {
					goto case 340;
				}
			}
			case 339: {
				Expect(198, t); // "Text"
				currentState = stateStack.Pop();
				break;
			}
			case 340: {
				if (t.kind == 198) {
					goto case 339;
				} else {
					goto case 342;
				}
			}
			case 341: {
				Expect(208, t); // "Unicode"
				currentState = stateStack.Pop();
				break;
			}
			case 342: {
				if (t.kind == 208) {
					goto case 341;
				} else {
					goto case 344;
				}
			}
			case 343: {
				Expect(209, t); // "Until"
				currentState = stateStack.Pop();
				break;
			}
			case 344: {
				if (t.kind == 209) {
					goto case 343;
				} else {
					goto case 346;
				}
			}
			case 345: {
				Expect(215, t); // "Where"
				currentState = stateStack.Pop();
				break;
			}
			case 346: {
				if (t.kind == 215) {
					goto case 345;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 347: {
				Expect(173, t); // "Public"
				currentState = stateStack.Pop();
				break;
			}
			case 348: {
				Expect(112, t); // "Friend"
				currentState = stateStack.Pop();
				break;
			}
			case 349: { // start of AccessModifier
				if (t.kind == 173) {
					goto case 347;
				} else {
					goto case 350;
				}
			}
			case 350: {
				if (t.kind == 112) {
					goto case 348;
				} else {
					goto case 352;
				}
			}
			case 351: {
				Expect(172, t); // "Protected"
				currentState = stateStack.Pop();
				break;
			}
			case 352: {
				if (t.kind == 172) {
					goto case 351;
				} else {
					goto case 354;
				}
			}
			case 353: {
				Expect(170, t); // "Private"
				currentState = stateStack.Pop();
				break;
			}
			case 354: {
				if (t.kind == 170) {
					goto case 353;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 355: {
				goto case 349; // AccessModifier
			}
			case 356: {
				Expect(184, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 357: { // start of TypeModifier
				if (set[17, t.kind]) {
					goto case 355;
				} else {
					goto case 358;
				}
			}
			case 358: {
				if (t.kind == 184) {
					goto case 356;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 359: {
				goto case 349; // AccessModifier
			}
			case 360: {
				Expect(184, t); // "Shadows"
				currentState = stateStack.Pop();
				break;
			}
			case 361: { // start of MemberModifier
				if (set[17, t.kind]) {
					goto case 359;
				} else {
					goto case 362;
				}
			}
			case 362: {
				if (t.kind == 184) {
					goto case 360;
				} else {
					goto case 364;
				}
			}
			case 363: {
				Expect(185, t); // "Shared"
				currentState = stateStack.Pop();
				break;
			}
			case 364: {
				if (t.kind == 185) {
					goto case 363;
				} else {
					goto case 366;
				}
			}
			case 365: {
				Expect(165, t); // "Overridable"
				currentState = stateStack.Pop();
				break;
			}
			case 366: {
				if (t.kind == 165) {
					goto case 365;
				} else {
					goto case 368;
				}
			}
			case 367: {
				Expect(153, t); // "NotOverridable"
				currentState = stateStack.Pop();
				break;
			}
			case 368: {
				if (t.kind == 153) {
					goto case 367;
				} else {
					goto case 370;
				}
			}
			case 369: {
				Expect(166, t); // "Overrides"
				currentState = stateStack.Pop();
				break;
			}
			case 370: {
				if (t.kind == 166) {
					goto case 369;
				} else {
					goto case 372;
				}
			}
			case 371: {
				Expect(164, t); // "Overloads"
				currentState = stateStack.Pop();
				break;
			}
			case 372: {
				if (t.kind == 164) {
					goto case 371;
				} else {
					goto case 374;
				}
			}
			case 373: {
				Expect(168, t); // "Partial"
				currentState = stateStack.Pop();
				break;
			}
			case 374: {
				if (t.kind == 168) {
					goto case 373;
				} else {
					goto case 376;
				}
			}
			case 375: {
				Expect(219, t); // "WithEvents"
				currentState = stateStack.Pop();
				break;
			}
			case 376: {
				if (t.kind == 219) {
					goto case 375;
				} else {
					goto case 378;
				}
			}
			case 377: {
				Expect(92, t); // "Dim"
				currentState = stateStack.Pop();
				break;
			}
			case 378: {
				if (t.kind == 92) {
					goto case 377;
				} else {
					Error(t);
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 379: {
				Expect(59, t); // "ByVal"
				currentState = stateStack.Pop();
				break;
			}
			case 380: {
				Expect(56, t); // "ByRef"
				currentState = stateStack.Pop();
				break;
			}
			case 381: { // start of ParameterModifier
				if (t.kind == 59) {
					goto case 379;
				} else {
					goto case 382;
				}
			}
			case 382: {
				if (t.kind == 56) {
					goto case 380;
				} else {
					goto case 384;
				}
			}
			case 383: {
				Expect(160, t); // "Optional"
				currentState = stateStack.Pop();
				break;
			}
			case 384: {
				if (t.kind == 160) {
					goto case 383;
				} else {
					goto case 386;
				}
			}
			case 385: {
				Expect(167, t); // "ParamArray"
				currentState = stateStack.Pop();
				break;
			}
			case 386: {
				if (t.kind == 167) {
					goto case 385;
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
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,T,T,x, T,T,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,T, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,T,T,x, T,x,T,x, T,T,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
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