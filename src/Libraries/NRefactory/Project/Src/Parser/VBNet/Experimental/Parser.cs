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

int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool nextTokenIsPotentialStartOfXmlMode = false;
	bool readXmlIdentifier = false;
	
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
		ApplyToken(t);
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, t);
				goto case 1;
			}
			case 1: {
				if (t == null) { currentState = 1; break; }
				if (t.kind == 172) {
					stateStack.Push(1);
					goto case 215;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (t == null) { currentState = 2; break; }
				if (t.kind == 136) {
					stateStack.Push(2);
					goto case 212;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (t == null) { currentState = 3; break; }
				if (t.kind == 39) {
					stateStack.Push(3);
					goto case 144;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (t == null) { currentState = 4; break; }
				if (set[0, t.kind]) {
					stateStack.Push(4);
					goto case 5;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 5: {
				if (t == null) { currentState = 5; break; }
				if (t.kind == 159) {
					goto case 208;
				} else {
					if (set[1, t.kind]) {
						goto case 7;
					} else {
						goto case 6;
					}
				}
			}
			case 6: {
				Error(t);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 7: {
				if (t == null) { currentState = 7; break; }
				if (t.kind == 39) {
					stateStack.Push(7);
					goto case 144;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (t == null) { currentState = 8; break; }
				if (set[2, t.kind]) {
					currentState = 8;
					break;
				} else {
					if (t.kind == 83 || t.kind == 154) {
						goto case 207;
					} else {
						Error(t);
						goto case 9;
					}
				}
			}
			case 9: {
				if (t == null) { currentState = 9; break; }
				if (set[3, t.kind]) {
					goto case 207;
				} else {
					stateStack.Push(10);
					goto case 15;
				}
			}
			case 10: {
				PushContext(Context.Type, t);
				goto case 11;
			}
			case 11: {
				if (t == null) { currentState = 11; break; }
				if (set[4, t.kind]) {
					stateStack.Push(11);
					goto case 17;
				} else {
					Expect(112, t); // "End"
					currentState = 12;
					break;
				}
			}
			case 12: {
				if (t == null) { currentState = 12; break; }
				if (t.kind == 83 || t.kind == 154) {
					currentState = 13;
					break;
				} else {
					Error(t);
					goto case 13;
				}
			}
			case 13: {
				stateStack.Push(14);
				goto case 15;
			}
			case 14: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 15: {
				if (t == null) { currentState = 15; break; }
				if (t.kind == 1 || t.kind == 22) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 16: {
				if (t == null) { currentState = 16; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 17: {
				PushContext(Context.Member, t);
				goto case 18;
			}
			case 18: {
				if (t == null) { currentState = 18; break; }
				if (t.kind == 39) {
					stateStack.Push(18);
					goto case 144;
				} else {
					goto case 19;
				}
			}
			case 19: {
				if (t == null) { currentState = 19; break; }
				if (set[5, t.kind]) {
					currentState = 19;
					break;
				} else {
					if (set[6, t.kind]) {
						stateStack.Push(20);
						goto case 200;
					} else {
						if (t.kind == 126 || t.kind == 208) {
							stateStack.Push(20);
							goto case 188;
						} else {
							if (t.kind == 100) {
								stateStack.Push(20);
								goto case 177;
							} else {
								if (t.kind == 118) {
									stateStack.Push(20);
									goto case 167;
								} else {
									if (t.kind == 97) {
										stateStack.Push(20);
										goto case 154;
									} else {
										if (t.kind == 171) {
											stateStack.Push(20);
											goto case 21;
										} else {
											Error(t);
											goto case 20;
										}
									}
								}
							}
						}
					}
				}
			}
			case 20: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 21: {
				if (t == null) { currentState = 21; break; }
				Expect(171, t); // "Operator"
				currentState = 22;
				break;
			}
			case 22: {
				if (t == null) { currentState = 22; break; }
				currentState = 23;
				break;
			}
			case 23: {
				if (t == null) { currentState = 23; break; }
				Expect(36, t); // "("
				currentState = 24;
				break;
			}
			case 24: {
				stateStack.Push(25);
				goto case 149;
			}
			case 25: {
				if (t == null) { currentState = 25; break; }
				Expect(37, t); // ")"
				currentState = 26;
				break;
			}
			case 26: {
				if (t == null) { currentState = 26; break; }
				if (t.kind == 62) {
					currentState = 143;
					break;
				} else {
					goto case 27;
				}
			}
			case 27: {
				if (t == null) { currentState = 27; break; }
				Expect(1, t); // EOL
				currentState = 28;
				break;
			}
			case 28: {
				stateStack.Push(29);
				goto case 32;
			}
			case 29: {
				if (t == null) { currentState = 29; break; }
				Expect(112, t); // "End"
				currentState = 30;
				break;
			}
			case 30: {
				if (t == null) { currentState = 30; break; }
				Expect(171, t); // "Operator"
				currentState = 31;
				break;
			}
			case 31: {
				goto case 15;
			}
			case 32: {
				PushContext(Context.Body, t);
				goto case 33;
			}
			case 33: {
				stateStack.Push(34);
				goto case 15;
			}
			case 34: {
				if (t == null) { currentState = 34; break; }
				if (set[7, t.kind]) {
					if (set[8, t.kind]) {
						stateStack.Push(33);
						goto case 35;
					} else {
						goto case 33;
					}
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				if (t == null) { currentState = 35; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					goto case 126;
				} else {
					if (t.kind == 209 || t.kind == 231) {
						goto case 120;
					} else {
						if (t.kind == 55 || t.kind == 191) {
							goto case 117;
						} else {
							if (t.kind == 187) {
								goto case 114;
							} else {
								if (set[9, t.kind]) {
									goto case 36;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 36: {
				if (t == null) { currentState = 36; break; }
				if (t.kind == 72) {
					goto case 113;
				} else {
					goto case 37;
				}
			}
			case 37: {
				goto case 38;
			}
			case 38: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 39;
			}
			case 39: {
				if (t == null) { currentState = 39; break; }
				if (set[10, t.kind]) {
					goto case 76;
				} else {
					if (t.kind == 161) {
						goto case 40;
					} else {
						goto case 6;
					}
				}
			}
			case 40: {
				if (t == null) { currentState = 40; break; }
				Expect(161, t); // "New"
				currentState = 41;
				break;
			}
			case 41: {
				if (t == null) { currentState = 41; break; }
				if (set[11, t.kind]) {
					stateStack.Push(52);
					goto case 57;
				} else {
					goto case 42;
				}
			}
			case 42: {
				if (t == null) { currentState = 42; break; }
				if (t.kind == 231) {
					currentState = 43;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 43: {
				goto case 44;
			}
			case 44: {
				if (t == null) { currentState = 44; break; }
				Expect(34, t); // "{"
				currentState = 45;
				break;
			}
			case 45: {
				if (t == null) { currentState = 45; break; }
				if (t.kind == 146) {
					currentState = 46;
					break;
				} else {
					goto case 46;
				}
			}
			case 46: {
				if (t == null) { currentState = 46; break; }
				Expect(27, t); // "."
				currentState = 47;
				break;
			}
			case 47: {
				stateStack.Push(48);
				goto case 16;
			}
			case 48: {
				if (t == null) { currentState = 48; break; }
				Expect(21, t); // "="
				currentState = 49;
				break;
			}
			case 49: {
				stateStack.Push(50);
				goto case 38;
			}
			case 50: {
				if (t == null) { currentState = 50; break; }
				if (t.kind == 23) {
					currentState = 45;
					break;
				} else {
					goto case 51;
				}
			}
			case 51: {
				if (t == null) { currentState = 51; break; }
				Expect(35, t); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 52: {
				if (t == null) { currentState = 52; break; }
				if (t.kind == 125) {
					currentState = 53;
					break;
				} else {
					goto case 42;
				}
			}
			case 53: {
				stateStack.Push(42);
				goto case 54;
			}
			case 54: {
				if (t == null) { currentState = 54; break; }
				Expect(34, t); // "{"
				currentState = 55;
				break;
			}
			case 55: {
				if (t == null) { currentState = 55; break; }
				if (set[12, t.kind]) {
					stateStack.Push(56);
					goto case 38;
				} else {
					if (t.kind == 34) {
						stateStack.Push(56);
						goto case 54;
					} else {
						Error(t);
						goto case 56;
					}
				}
			}
			case 56: {
				if (t == null) { currentState = 56; break; }
				if (t.kind == 23) {
					currentState = 55;
					break;
				} else {
					goto case 51;
				}
			}
			case 57: {
				if (t == null) { currentState = 57; break; }
				if (t.kind == 129) {
					goto case 72;
				} else {
					if (set[13, t.kind]) {
						stateStack.Push(58);
						goto case 73;
					} else {
						if (set[14, t.kind]) {
							goto case 72;
						} else {
							Error(t);
							goto case 58;
						}
					}
				}
			}
			case 58: {
				if (t == null) { currentState = 58; break; }
				if (t.kind == 36) {
					stateStack.Push(58);
					goto case 62;
				} else {
					goto case 59;
				}
			}
			case 59: {
				if (t == null) { currentState = 59; break; }
				if (t.kind == 27) {
					currentState = 60;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 60: {
				stateStack.Push(61);
				goto case 16;
			}
			case 61: {
				if (t == null) { currentState = 61; break; }
				if (t.kind == 36) {
					stateStack.Push(61);
					goto case 62;
				} else {
					goto case 59;
				}
			}
			case 62: {
				if (t == null) { currentState = 62; break; }
				Expect(36, t); // "("
				currentState = 63;
				break;
			}
			case 63: {
				if (t == null) { currentState = 63; break; }
				if (t.kind == 168) {
					goto case 69;
				} else {
					if (set[15, t.kind]) {
						goto case 65;
					} else {
						Error(t);
						goto case 64;
					}
				}
			}
			case 64: {
				if (t == null) { currentState = 64; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 65: {
				if (t == null) { currentState = 65; break; }
				if (set[12, t.kind]) {
					goto case 66;
				} else {
					goto case 64;
				}
			}
			case 66: {
				stateStack.Push(64);
				goto case 67;
			}
			case 67: {
				stateStack.Push(68);
				goto case 38;
			}
			case 68: {
				if (t == null) { currentState = 68; break; }
				if (t.kind == 23) {
					currentState = 67;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 69: {
				if (t == null) { currentState = 69; break; }
				currentState = 70;
				break;
			}
			case 70: {
				if (t == null) { currentState = 70; break; }
				if (set[11, t.kind]) {
					stateStack.Push(71);
					goto case 57;
				} else {
					goto case 71;
				}
			}
			case 71: {
				if (t == null) { currentState = 71; break; }
				if (t.kind == 23) {
					goto case 69;
				} else {
					goto case 64;
				}
			}
			case 72: {
				if (t == null) { currentState = 72; break; }
				currentState = 58;
				break;
			}
			case 73: {
				PushContext(Context.IdentifierExpected, t);
				goto case 74;
			}
			case 74: {
				if (t == null) { currentState = 74; break; }
				if (set[13, t.kind]) {
					currentState = 75;
					break;
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
				stateStack.Push(77);
				goto case 78;
			}
			case 77: {
				if (t == null) { currentState = 77; break; }
				if (set[16, t.kind]) {
					currentState = 76;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 78: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 79;
			}
			case 79: {
				if (t == null) { currentState = 79; break; }
				if (set[17, t.kind]) {
					currentState = 79;
					break;
				} else {
					if (set[18, t.kind]) {
						stateStack.Push(91);
						goto case 100;
					} else {
						if (t.kind == 218) {
							currentState = 88;
							break;
						} else {
							if (set[19, t.kind]) {
								goto case 80;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 80: {
				if (t == null) { currentState = 80; break; }
				if (set[20, t.kind]) {
					goto case 85;
				} else {
					if (t.kind == 93 || t.kind == 105 || t.kind == 217) {
						currentState = 81;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 81: {
				if (t == null) { currentState = 81; break; }
				Expect(36, t); // "("
				currentState = 82;
				break;
			}
			case 82: {
				stateStack.Push(83);
				goto case 38;
			}
			case 83: {
				if (t == null) { currentState = 83; break; }
				Expect(23, t); // ","
				currentState = 84;
				break;
			}
			case 84: {
				stateStack.Push(64);
				goto case 57;
			}
			case 85: {
				if (t == null) { currentState = 85; break; }
				if (set[20, t.kind]) {
					currentState = 86;
					break;
				} else {
					Error(t);
					goto case 86;
				}
			}
			case 86: {
				if (t == null) { currentState = 86; break; }
				Expect(36, t); // "("
				currentState = 87;
				break;
			}
			case 87: {
				stateStack.Push(64);
				goto case 38;
			}
			case 88: {
				stateStack.Push(89);
				goto case 78;
			}
			case 89: {
				if (t == null) { currentState = 89; break; }
				Expect(143, t); // "Is"
				currentState = 90;
				break;
			}
			case 90: {
				goto case 57;
			}
			case 91: {
				if (t == null) { currentState = 91; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(91);
					goto case 92;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 92: {
				if (t == null) { currentState = 92; break; }
				if (t.kind == 36) {
					currentState = 95;
					break;
				} else {
					if (t.kind == 27 || t.kind == 28) {
						goto case 93;
					} else {
						goto case 6;
					}
				}
			}
			case 93: {
				if (t == null) { currentState = 93; break; }
				currentState = 94;
				break;
			}
			case 94: {
				goto case 16;
			}
			case 95: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 96;
			}
			case 96: {
				if (t == null) { currentState = 96; break; }
				if (t.kind == 168) {
					goto case 97;
				} else {
					if (set[12, t.kind]) {
						goto case 66;
					} else {
						goto case 6;
					}
				}
			}
			case 97: {
				if (t == null) { currentState = 97; break; }
				currentState = 98;
				break;
			}
			case 98: {
				stateStack.Push(99);
				goto case 57;
			}
			case 99: {
				if (t == null) { currentState = 99; break; }
				if (t.kind == 23) {
					goto case 97;
				} else {
					goto case 64;
				}
			}
			case 100: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 101;
			}
			case 101: {
				if (t == null) { currentState = 101; break; }
				if (set[21, t.kind]) {
					goto case 16;
				} else {
					if (t.kind == 36) {
						currentState = 87;
						break;
					} else {
						if (set[13, t.kind]) {
							goto case 73;
						} else {
							if (t.kind == 27 || t.kind == 28) {
								goto case 93;
							} else {
								if (t.kind == 128) {
									currentState = 112;
									break;
								} else {
									if (t.kind == 235) {
										currentState = 110;
										break;
									} else {
										if (t.kind == 10 || t.kind == 17) {
											goto case 102;
										} else {
											goto case 6;
										}
									}
								}
							}
						}
					}
				}
			}
			case 102: {
				PushContext(Context.Xml, t);
				goto case 103;
			}
			case 103: {
				if (t == null) { currentState = 103; break; }
				if (t.kind == 17) {
					currentState = 103;
					break;
				} else {
					stateStack.Push(104);
					goto case 105;
				}
			}
			case 104: {
				if (t == null) { currentState = 104; break; }
				if (t.kind == 17) {
					currentState = 104;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 105: {
				if (t == null) { currentState = 105; break; }
				Expect(10, t); // XmlOpenTag
				currentState = 106;
				break;
			}
			case 106: {
				if (t == null) { currentState = 106; break; }
				if (set[22, t.kind]) {
					currentState = 106;
					break;
				} else {
					if (t.kind == 14) {
						goto case 16;
					} else {
						if (t.kind == 11) {
							goto case 107;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 107: {
				if (t == null) { currentState = 107; break; }
				currentState = 108;
				break;
			}
			case 108: {
				if (t == null) { currentState = 108; break; }
				if (set[23, t.kind]) {
					if (set[24, t.kind]) {
						goto case 107;
					} else {
						if (t.kind == 10) {
							stateStack.Push(108);
							goto case 105;
						} else {
							Error(t);
							goto case 108;
						}
					}
				} else {
					Expect(15, t); // XmlOpenEndTag
					currentState = 109;
					break;
				}
			}
			case 109: {
				if (t == null) { currentState = 109; break; }
				if (set[25, t.kind]) {
					currentState = 109;
					break;
				} else {
					Expect(11, t); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 110: {
				if (t == null) { currentState = 110; break; }
				Expect(36, t); // "("
				currentState = 111;
				break;
			}
			case 111: {
				readXmlIdentifier = true;
				stateStack.Push(64);
				goto case 73;
			}
			case 112: {
				if (t == null) { currentState = 112; break; }
				Expect(36, t); // "("
				currentState = 84;
				break;
			}
			case 113: {
				if (t == null) { currentState = 113; break; }
				currentState = 37;
				break;
			}
			case 114: {
				if (t == null) { currentState = 114; break; }
				Expect(187, t); // "RaiseEvent"
				currentState = 115;
				break;
			}
			case 115: {
				stateStack.Push(116);
				goto case 16;
			}
			case 116: {
				if (t == null) { currentState = 116; break; }
				if (t.kind == 36) {
					currentState = 65;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 117: {
				if (t == null) { currentState = 117; break; }
				if (t.kind == 55 || t.kind == 191) {
					currentState = 118;
					break;
				} else {
					Error(t);
					goto case 118;
				}
			}
			case 118: {
				stateStack.Push(119);
				goto case 38;
			}
			case 119: {
				if (t == null) { currentState = 119; break; }
				Expect(23, t); // ","
				currentState = 37;
				break;
			}
			case 120: {
				if (t == null) { currentState = 120; break; }
				if (t.kind == 209 || t.kind == 231) {
					currentState = 121;
					break;
				} else {
					Error(t);
					goto case 121;
				}
			}
			case 121: {
				stateStack.Push(122);
				goto case 38;
			}
			case 122: {
				stateStack.Push(123);
				goto case 15;
			}
			case 123: {
				stateStack.Push(124);
				goto case 32;
			}
			case 124: {
				if (t == null) { currentState = 124; break; }
				Expect(112, t); // "End"
				currentState = 125;
				break;
			}
			case 125: {
				if (t == null) { currentState = 125; break; }
				if (t.kind == 209 || t.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 126: {
				if (t == null) { currentState = 126; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					currentState = 127;
					break;
				} else {
					Error(t);
					goto case 127;
				}
			}
			case 127: {
				stateStack.Push(128);
				goto case 73;
			}
			case 128: {
				if (t == null) { currentState = 128; break; }
				if (t.kind == 32) {
					currentState = 129;
					break;
				} else {
					goto case 129;
				}
			}
			case 129: {
				if (t == null) { currentState = 129; break; }
				if (t.kind == 36) {
					goto case 141;
				} else {
					goto case 130;
				}
			}
			case 130: {
				if (t == null) { currentState = 130; break; }
				if (t.kind == 23) {
					currentState = 135;
					break;
				} else {
					if (t.kind == 62) {
						currentState = 132;
						break;
					} else {
						goto case 131;
					}
				}
			}
			case 131: {
				if (t == null) { currentState = 131; break; }
				if (t.kind == 21) {
					goto case 113;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 132: {
				if (t == null) { currentState = 132; break; }
				if (t.kind == 161) {
					goto case 134;
				} else {
					goto case 133;
				}
			}
			case 133: {
				stateStack.Push(131);
				goto case 57;
			}
			case 134: {
				if (t == null) { currentState = 134; break; }
				currentState = 133;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 73;
			}
			case 136: {
				if (t == null) { currentState = 136; break; }
				if (t.kind == 32) {
					currentState = 137;
					break;
				} else {
					goto case 137;
				}
			}
			case 137: {
				if (t == null) { currentState = 137; break; }
				if (t.kind == 36) {
					goto case 138;
				} else {
					goto case 130;
				}
			}
			case 138: {
				if (t == null) { currentState = 138; break; }
				currentState = 139;
				break;
			}
			case 139: {
				if (t == null) { currentState = 139; break; }
				if (t.kind == 23) {
					goto case 138;
				} else {
					goto case 140;
				}
			}
			case 140: {
				if (t == null) { currentState = 140; break; }
				Expect(37, t); // ")"
				currentState = 130;
				break;
			}
			case 141: {
				if (t == null) { currentState = 141; break; }
				currentState = 142;
				break;
			}
			case 142: {
				if (t == null) { currentState = 142; break; }
				if (t.kind == 23) {
					goto case 141;
				} else {
					goto case 140;
				}
			}
			case 143: {
				if (t == null) { currentState = 143; break; }
				if (t.kind == 39) {
					stateStack.Push(143);
					goto case 144;
				} else {
					stateStack.Push(27);
					goto case 57;
				}
			}
			case 144: {
				if (t == null) { currentState = 144; break; }
				Expect(39, t); // "<"
				currentState = 145;
				break;
			}
			case 145: {
				PushContext(Context.Attribute, t);
				goto case 146;
			}
			case 146: {
				if (t == null) { currentState = 146; break; }
				if (set[26, t.kind]) {
					currentState = 146;
					break;
				} else {
					Expect(38, t); // ">"
					currentState = 147;
					break;
				}
			}
			case 147: {
				PopContext();
				goto case 148;
			}
			case 148: {
				if (t == null) { currentState = 148; break; }
				if (t.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 149: {
				stateStack.Push(150);
				goto case 151;
			}
			case 150: {
				if (t == null) { currentState = 150; break; }
				if (t.kind == 23) {
					currentState = 149;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 151: {
				if (t == null) { currentState = 151; break; }
				if (t.kind == 39) {
					stateStack.Push(151);
					goto case 144;
				} else {
					goto case 152;
				}
			}
			case 152: {
				if (t == null) { currentState = 152; break; }
				if (set[27, t.kind]) {
					currentState = 152;
					break;
				} else {
					stateStack.Push(153);
					goto case 73;
				}
			}
			case 153: {
				if (t == null) { currentState = 153; break; }
				if (t.kind == 62) {
					goto case 134;
				} else {
					goto case 131;
				}
			}
			case 154: {
				if (t == null) { currentState = 154; break; }
				Expect(97, t); // "Custom"
				currentState = 155;
				break;
			}
			case 155: {
				stateStack.Push(156);
				goto case 167;
			}
			case 156: {
				if (t == null) { currentState = 156; break; }
				if (set[28, t.kind]) {
					goto case 158;
				} else {
					Expect(112, t); // "End"
					currentState = 157;
					break;
				}
			}
			case 157: {
				if (t == null) { currentState = 157; break; }
				Expect(118, t); // "Event"
				currentState = 31;
				break;
			}
			case 158: {
				if (t == null) { currentState = 158; break; }
				if (t.kind == 39) {
					stateStack.Push(158);
					goto case 144;
				} else {
					if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
						currentState = 159;
						break;
					} else {
						Error(t);
						goto case 159;
					}
				}
			}
			case 159: {
				if (t == null) { currentState = 159; break; }
				Expect(36, t); // "("
				currentState = 160;
				break;
			}
			case 160: {
				stateStack.Push(161);
				goto case 149;
			}
			case 161: {
				if (t == null) { currentState = 161; break; }
				Expect(37, t); // ")"
				currentState = 162;
				break;
			}
			case 162: {
				if (t == null) { currentState = 162; break; }
				Expect(1, t); // EOL
				currentState = 163;
				break;
			}
			case 163: {
				stateStack.Push(164);
				goto case 32;
			}
			case 164: {
				if (t == null) { currentState = 164; break; }
				Expect(112, t); // "End"
				currentState = 165;
				break;
			}
			case 165: {
				if (t == null) { currentState = 165; break; }
				if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
					currentState = 166;
					break;
				} else {
					Error(t);
					goto case 166;
				}
			}
			case 166: {
				stateStack.Push(156);
				goto case 15;
			}
			case 167: {
				if (t == null) { currentState = 167; break; }
				Expect(118, t); // "Event"
				currentState = 168;
				break;
			}
			case 168: {
				stateStack.Push(169);
				goto case 73;
			}
			case 169: {
				if (t == null) { currentState = 169; break; }
				if (t.kind == 62) {
					currentState = 176;
					break;
				} else {
					if (set[29, t.kind]) {
						if (t.kind == 36) {
							currentState = 174;
							break;
						} else {
							goto case 170;
						}
					} else {
						Error(t);
						goto case 170;
					}
				}
			}
			case 170: {
				if (t == null) { currentState = 170; break; }
				if (t.kind == 135) {
					goto case 171;
				} else {
					goto case 31;
				}
			}
			case 171: {
				if (t == null) { currentState = 171; break; }
				currentState = 172;
				break;
			}
			case 172: {
				stateStack.Push(173);
				goto case 57;
			}
			case 173: {
				if (t == null) { currentState = 173; break; }
				if (t.kind == 23) {
					goto case 171;
				} else {
					goto case 31;
				}
			}
			case 174: {
				if (t == null) { currentState = 174; break; }
				if (set[30, t.kind]) {
					stateStack.Push(175);
					goto case 149;
				} else {
					goto case 175;
				}
			}
			case 175: {
				if (t == null) { currentState = 175; break; }
				Expect(37, t); // ")"
				currentState = 170;
				break;
			}
			case 176: {
				stateStack.Push(170);
				goto case 57;
			}
			case 177: {
				if (t == null) { currentState = 177; break; }
				Expect(100, t); // "Declare"
				currentState = 178;
				break;
			}
			case 178: {
				if (t == null) { currentState = 178; break; }
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					currentState = 179;
					break;
				} else {
					goto case 179;
				}
			}
			case 179: {
				if (t == null) { currentState = 179; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 180;
					break;
				} else {
					Error(t);
					goto case 180;
				}
			}
			case 180: {
				stateStack.Push(181);
				goto case 73;
			}
			case 181: {
				if (t == null) { currentState = 181; break; }
				Expect(148, t); // "Lib"
				currentState = 182;
				break;
			}
			case 182: {
				if (t == null) { currentState = 182; break; }
				Expect(3, t); // LiteralString
				currentState = 183;
				break;
			}
			case 183: {
				if (t == null) { currentState = 183; break; }
				if (t.kind == 58) {
					currentState = 187;
					break;
				} else {
					goto case 184;
				}
			}
			case 184: {
				if (t == null) { currentState = 184; break; }
				if (t.kind == 36) {
					currentState = 185;
					break;
				} else {
					goto case 31;
				}
			}
			case 185: {
				if (t == null) { currentState = 185; break; }
				if (set[30, t.kind]) {
					stateStack.Push(186);
					goto case 149;
				} else {
					goto case 186;
				}
			}
			case 186: {
				if (t == null) { currentState = 186; break; }
				Expect(37, t); // ")"
				currentState = 31;
				break;
			}
			case 187: {
				if (t == null) { currentState = 187; break; }
				Expect(3, t); // LiteralString
				currentState = 184;
				break;
			}
			case 188: {
				if (t == null) { currentState = 188; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 189;
					break;
				} else {
					Error(t);
					goto case 189;
				}
			}
			case 189: {
				PushContext(Context.IdentifierExpected, t);
				goto case 190;
			}
			case 190: {
				if (t == null) { currentState = 190; break; }
				currentState = 191;
				break;
			}
			case 191: {
				PopContext();
				goto case 192;
			}
			case 192: {
				if (t == null) { currentState = 192; break; }
				if (t.kind == 36) {
					currentState = 198;
					break;
				} else {
					goto case 193;
				}
			}
			case 193: {
				if (t == null) { currentState = 193; break; }
				if (t.kind == 62) {
					currentState = 197;
					break;
				} else {
					goto case 194;
				}
			}
			case 194: {
				stateStack.Push(195);
				goto case 32;
			}
			case 195: {
				if (t == null) { currentState = 195; break; }
				Expect(112, t); // "End"
				currentState = 196;
				break;
			}
			case 196: {
				if (t == null) { currentState = 196; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 31;
					break;
				} else {
					Error(t);
					goto case 31;
				}
			}
			case 197: {
				stateStack.Push(194);
				goto case 57;
			}
			case 198: {
				if (t == null) { currentState = 198; break; }
				if (set[30, t.kind]) {
					stateStack.Push(199);
					goto case 149;
				} else {
					goto case 199;
				}
			}
			case 199: {
				if (t == null) { currentState = 199; break; }
				Expect(37, t); // ")"
				currentState = 193;
				break;
			}
			case 200: {
				if (t == null) { currentState = 200; break; }
				if (t.kind == 87) {
					currentState = 201;
					break;
				} else {
					goto case 201;
				}
			}
			case 201: {
				stateStack.Push(202);
				goto case 206;
			}
			case 202: {
				if (t == null) { currentState = 202; break; }
				if (t.kind == 62) {
					currentState = 205;
					break;
				} else {
					goto case 203;
				}
			}
			case 203: {
				if (t == null) { currentState = 203; break; }
				if (t.kind == 21) {
					currentState = 204;
					break;
				} else {
					goto case 31;
				}
			}
			case 204: {
				stateStack.Push(31);
				goto case 38;
			}
			case 205: {
				stateStack.Push(203);
				goto case 57;
			}
			case 206: {
				if (t == null) { currentState = 206; break; }
				if (set[31, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 207: {
				if (t == null) { currentState = 207; break; }
				currentState = 9;
				break;
			}
			case 208: {
				if (t == null) { currentState = 208; break; }
				Expect(159, t); // "Namespace"
				currentState = 209;
				break;
			}
			case 209: {
				if (t == null) { currentState = 209; break; }
				if (set[3, t.kind]) {
					currentState = 209;
					break;
				} else {
					stateStack.Push(210);
					goto case 15;
				}
			}
			case 210: {
				if (t == null) { currentState = 210; break; }
				if (set[32, t.kind]) {
					stateStack.Push(210);
					goto case 5;
				} else {
					Expect(112, t); // "End"
					currentState = 211;
					break;
				}
			}
			case 211: {
				if (t == null) { currentState = 211; break; }
				Expect(159, t); // "Namespace"
				currentState = 31;
				break;
			}
			case 212: {
				if (t == null) { currentState = 212; break; }
				Expect(136, t); // "Imports"
				currentState = 213;
				break;
			}
			case 213: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 214;
			}
			case 214: {
				if (t == null) { currentState = 214; break; }
				if (set[3, t.kind]) {
					currentState = 214;
					break;
				} else {
					goto case 31;
				}
			}
			case 215: {
				if (t == null) { currentState = 215; break; }
				Expect(172, t); // "Option"
				currentState = 216;
				break;
			}
			case 216: {
				if (t == null) { currentState = 216; break; }
				if (set[3, t.kind]) {
					currentState = 216;
					break;
				} else {
					goto case 31;
				}
			}
		}

	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, x,T,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};

} // end Parser


}