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
	
	Token consumedEndToken;
	
	public void InformToken(Token t) 
	{
		if (consumedEndToken != null) {
			if (t.kind == Tokens.EOL || t.kind == Tokens.Colon) {
				consumedEndToken = null;
				return; // ignore End statement
			}
			InformTokenInternal(consumedEndToken);
			consumedEndToken = null;
		}
		if (t.kind == Tokens.End) {
			consumedEndToken = t;
		} else {
			InformTokenInternal(t);
		}
	}

	void InformTokenInternal(Token t) 
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
					goto case 332;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (t == null) { currentState = 2; break; }
				if (t.kind == 136) {
					stateStack.Push(2);
					goto case 329;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (t == null) { currentState = 3; break; }
				if (t.kind == 39) {
					stateStack.Push(3);
					goto case 262;
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
					goto case 325;
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
					goto case 262;
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
						goto case 324;
					} else {
						Error(t);
						goto case 9;
					}
				}
			}
			case 9: {
				if (t == null) { currentState = 9; break; }
				if (set[3, t.kind]) {
					goto case 324;
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
					goto case 262;
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
						goto case 317;
					} else {
						if (t.kind == 126 || t.kind == 208) {
							stateStack.Push(20);
							goto case 305;
						} else {
							if (t.kind == 100) {
								stateStack.Push(20);
								goto case 294;
							} else {
								if (t.kind == 118) {
									stateStack.Push(20);
									goto case 284;
								} else {
									if (t.kind == 97) {
										stateStack.Push(20);
										goto case 272;
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
				goto case 267;
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
					currentState = 261;
					break;
				} else {
					goto case 27;
				}
			}
			case 27: {
				stateStack.Push(28);
				goto case 31;
			}
			case 28: {
				if (t == null) { currentState = 28; break; }
				Expect(112, t); // "End"
				currentState = 29;
				break;
			}
			case 29: {
				if (t == null) { currentState = 29; break; }
				Expect(171, t); // "Operator"
				currentState = 30;
				break;
			}
			case 30: {
				goto case 15;
			}
			case 31: {
				PushContext(Context.Body, t);
				goto case 32;
			}
			case 32: {
				stateStack.Push(33);
				goto case 15;
			}
			case 33: {
				if (t == null) { currentState = 33; break; }
				if (set[7, t.kind]) {
					if (set[8, t.kind]) {
						stateStack.Push(32);
						goto case 34;
					} else {
						goto case 32;
					}
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 34: {
				if (t == null) { currentState = 34; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					goto case 244;
				} else {
					if (t.kind == 209 || t.kind == 231) {
						goto case 239;
					} else {
						if (t.kind == 55 || t.kind == 191) {
							goto case 236;
						} else {
							if (t.kind == 187) {
								goto case 233;
							} else {
								if (t.kind == 134) {
									goto case 214;
								} else {
									if (t.kind == 195) {
										goto case 199;
									} else {
										if (t.kind == 229) {
											goto case 194;
										} else {
											if (t.kind == 107) {
												goto case 185;
											} else {
												if (t.kind == 123) {
													goto case 158;
												} else {
													if (t.kind == 117 || t.kind == 170 || t.kind == 192) {
														goto case 150;
													} else {
														if (t.kind == 213) {
															goto case 148;
														} else {
															if (t.kind == 216) {
																goto case 137;
															} else {
																if (set[9, t.kind]) {
																	goto case 131;
																} else {
																	if (t.kind == 189) {
																		goto case 129;
																	} else {
																		if (t.kind == 116) {
																			goto case 126;
																		} else {
																			if (set[10, t.kind]) {
																				goto case 35;
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
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 35: {
				if (t == null) { currentState = 35; break; }
				if (t.kind == 72) {
					goto case 125;
				} else {
					goto case 36;
				}
			}
			case 36: {
				goto case 37;
			}
			case 37: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 38;
			}
			case 38: {
				if (t == null) { currentState = 38; break; }
				if (set[11, t.kind]) {
					goto case 48;
				} else {
					if (t.kind == 134) {
						goto case 39;
					} else {
						goto case 6;
					}
				}
			}
			case 39: {
				if (t == null) { currentState = 39; break; }
				Expect(134, t); // "If"
				currentState = 40;
				break;
			}
			case 40: {
				if (t == null) { currentState = 40; break; }
				Expect(36, t); // "("
				currentState = 41;
				break;
			}
			case 41: {
				stateStack.Push(42);
				goto case 37;
			}
			case 42: {
				if (t == null) { currentState = 42; break; }
				Expect(23, t); // ","
				currentState = 43;
				break;
			}
			case 43: {
				stateStack.Push(44);
				goto case 37;
			}
			case 44: {
				if (t == null) { currentState = 44; break; }
				if (t.kind == 23) {
					goto case 46;
				} else {
					goto case 45;
				}
			}
			case 45: {
				if (t == null) { currentState = 45; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 46: {
				if (t == null) { currentState = 46; break; }
				currentState = 47;
				break;
			}
			case 47: {
				stateStack.Push(45);
				goto case 37;
			}
			case 48: {
				stateStack.Push(49);
				goto case 50;
			}
			case 49: {
				if (t == null) { currentState = 49; break; }
				if (set[12, t.kind]) {
					currentState = 48;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 50: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 51;
			}
			case 51: {
				if (t == null) { currentState = 51; break; }
				if (set[13, t.kind]) {
					currentState = 51;
					break;
				} else {
					if (set[14, t.kind]) {
						stateStack.Push(95);
						goto case 104;
					} else {
						if (t.kind == 218) {
							currentState = 92;
							break;
						} else {
							if (t.kind == 161) {
								goto case 52;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 52: {
				if (t == null) { currentState = 52; break; }
				Expect(161, t); // "New"
				currentState = 53;
				break;
			}
			case 53: {
				if (t == null) { currentState = 53; break; }
				if (set[15, t.kind]) {
					stateStack.Push(64);
					goto case 69;
				} else {
					goto case 54;
				}
			}
			case 54: {
				if (t == null) { currentState = 54; break; }
				if (t.kind == 231) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 55: {
				goto case 56;
			}
			case 56: {
				if (t == null) { currentState = 56; break; }
				Expect(34, t); // "{"
				currentState = 57;
				break;
			}
			case 57: {
				if (t == null) { currentState = 57; break; }
				if (t.kind == 146) {
					currentState = 58;
					break;
				} else {
					goto case 58;
				}
			}
			case 58: {
				if (t == null) { currentState = 58; break; }
				Expect(27, t); // "."
				currentState = 59;
				break;
			}
			case 59: {
				stateStack.Push(60);
				goto case 16;
			}
			case 60: {
				if (t == null) { currentState = 60; break; }
				Expect(21, t); // "="
				currentState = 61;
				break;
			}
			case 61: {
				stateStack.Push(62);
				goto case 37;
			}
			case 62: {
				if (t == null) { currentState = 62; break; }
				if (t.kind == 23) {
					currentState = 57;
					break;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (t == null) { currentState = 63; break; }
				Expect(35, t); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 64: {
				if (t == null) { currentState = 64; break; }
				if (t.kind == 125) {
					currentState = 65;
					break;
				} else {
					goto case 54;
				}
			}
			case 65: {
				stateStack.Push(54);
				goto case 66;
			}
			case 66: {
				if (t == null) { currentState = 66; break; }
				Expect(34, t); // "{"
				currentState = 67;
				break;
			}
			case 67: {
				if (t == null) { currentState = 67; break; }
				if (set[16, t.kind]) {
					stateStack.Push(68);
					goto case 37;
				} else {
					if (t.kind == 34) {
						stateStack.Push(68);
						goto case 66;
					} else {
						Error(t);
						goto case 68;
					}
				}
			}
			case 68: {
				if (t == null) { currentState = 68; break; }
				if (t.kind == 23) {
					currentState = 67;
					break;
				} else {
					goto case 63;
				}
			}
			case 69: {
				if (t == null) { currentState = 69; break; }
				if (t.kind == 129) {
					goto case 88;
				} else {
					if (set[17, t.kind]) {
						stateStack.Push(70);
						goto case 89;
					} else {
						if (set[18, t.kind]) {
							goto case 88;
						} else {
							Error(t);
							goto case 70;
						}
					}
				}
			}
			case 70: {
				if (t == null) { currentState = 70; break; }
				if (t.kind == 36) {
					stateStack.Push(70);
					goto case 74;
				} else {
					goto case 71;
				}
			}
			case 71: {
				if (t == null) { currentState = 71; break; }
				if (t.kind == 27) {
					currentState = 72;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 72: {
				stateStack.Push(73);
				goto case 16;
			}
			case 73: {
				if (t == null) { currentState = 73; break; }
				if (t.kind == 36) {
					stateStack.Push(73);
					goto case 74;
				} else {
					goto case 71;
				}
			}
			case 74: {
				if (t == null) { currentState = 74; break; }
				Expect(36, t); // "("
				currentState = 75;
				break;
			}
			case 75: {
				if (t == null) { currentState = 75; break; }
				if (t.kind == 168) {
					goto case 85;
				} else {
					if (set[19, t.kind]) {
						goto case 76;
					} else {
						Error(t);
						goto case 45;
					}
				}
			}
			case 76: {
				if (t == null) { currentState = 76; break; }
				if (set[20, t.kind]) {
					goto case 77;
				} else {
					goto case 45;
				}
			}
			case 77: {
				stateStack.Push(45);
				goto case 78;
			}
			case 78: {
				if (t == null) { currentState = 78; break; }
				if (set[16, t.kind]) {
					goto case 82;
				} else {
					if (t.kind == 23) {
						goto case 79;
					} else {
						goto case 6;
					}
				}
			}
			case 79: {
				if (t == null) { currentState = 79; break; }
				currentState = 80;
				break;
			}
			case 80: {
				if (t == null) { currentState = 80; break; }
				if (set[16, t.kind]) {
					stateStack.Push(81);
					goto case 37;
				} else {
					goto case 81;
				}
			}
			case 81: {
				if (t == null) { currentState = 81; break; }
				if (t.kind == 23) {
					goto case 79;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 82: {
				stateStack.Push(83);
				goto case 37;
			}
			case 83: {
				if (t == null) { currentState = 83; break; }
				if (t.kind == 23) {
					currentState = 84;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 84: {
				if (t == null) { currentState = 84; break; }
				if (set[16, t.kind]) {
					goto case 82;
				} else {
					goto case 83;
				}
			}
			case 85: {
				if (t == null) { currentState = 85; break; }
				currentState = 86;
				break;
			}
			case 86: {
				if (t == null) { currentState = 86; break; }
				if (set[15, t.kind]) {
					stateStack.Push(87);
					goto case 69;
				} else {
					goto case 87;
				}
			}
			case 87: {
				if (t == null) { currentState = 87; break; }
				if (t.kind == 23) {
					goto case 85;
				} else {
					goto case 45;
				}
			}
			case 88: {
				if (t == null) { currentState = 88; break; }
				currentState = 70;
				break;
			}
			case 89: {
				PushContext(Context.IdentifierExpected, t);
				goto case 90;
			}
			case 90: {
				if (t == null) { currentState = 90; break; }
				if (set[17, t.kind]) {
					currentState = 91;
					break;
				} else {
					Error(t);
					goto case 91;
				}
			}
			case 91: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 92: {
				stateStack.Push(93);
				goto case 50;
			}
			case 93: {
				if (t == null) { currentState = 93; break; }
				Expect(143, t); // "Is"
				currentState = 94;
				break;
			}
			case 94: {
				goto case 69;
			}
			case 95: {
				if (t == null) { currentState = 95; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(95);
					goto case 96;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 96: {
				if (t == null) { currentState = 96; break; }
				if (t.kind == 36) {
					currentState = 99;
					break;
				} else {
					if (t.kind == 27 || t.kind == 28) {
						goto case 97;
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
				goto case 16;
			}
			case 99: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 100;
			}
			case 100: {
				if (t == null) { currentState = 100; break; }
				if (t.kind == 168) {
					goto case 101;
				} else {
					if (set[20, t.kind]) {
						goto case 77;
					} else {
						goto case 6;
					}
				}
			}
			case 101: {
				if (t == null) { currentState = 101; break; }
				currentState = 102;
				break;
			}
			case 102: {
				stateStack.Push(103);
				goto case 69;
			}
			case 103: {
				if (t == null) { currentState = 103; break; }
				if (t.kind == 23) {
					goto case 101;
				} else {
					goto case 45;
				}
			}
			case 104: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 105;
			}
			case 105: {
				if (t == null) { currentState = 105; break; }
				if (set[21, t.kind]) {
					goto case 16;
				} else {
					if (t.kind == 36) {
						goto case 46;
					} else {
						if (set[17, t.kind]) {
							goto case 124;
						} else {
							if (t.kind == 27 || t.kind == 28) {
								goto case 97;
							} else {
								if (t.kind == 128) {
									currentState = 123;
									break;
								} else {
									if (t.kind == 235) {
										currentState = 121;
										break;
									} else {
										if (t.kind == 10 || t.kind == 17) {
											goto case 113;
										} else {
											if (set[22, t.kind]) {
												goto case 106;
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
			}
			case 106: {
				if (t == null) { currentState = 106; break; }
				if (set[23, t.kind]) {
					goto case 111;
				} else {
					if (t.kind == 93 || t.kind == 105 || t.kind == 217) {
						currentState = 107;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 107: {
				if (t == null) { currentState = 107; break; }
				Expect(36, t); // "("
				currentState = 108;
				break;
			}
			case 108: {
				stateStack.Push(109);
				goto case 37;
			}
			case 109: {
				if (t == null) { currentState = 109; break; }
				Expect(23, t); // ","
				currentState = 110;
				break;
			}
			case 110: {
				stateStack.Push(45);
				goto case 69;
			}
			case 111: {
				if (t == null) { currentState = 111; break; }
				if (set[23, t.kind]) {
					currentState = 112;
					break;
				} else {
					Error(t);
					goto case 112;
				}
			}
			case 112: {
				if (t == null) { currentState = 112; break; }
				Expect(36, t); // "("
				currentState = 47;
				break;
			}
			case 113: {
				PushContext(Context.Xml, t);
				goto case 114;
			}
			case 114: {
				if (t == null) { currentState = 114; break; }
				if (t.kind == 17) {
					currentState = 114;
					break;
				} else {
					stateStack.Push(115);
					goto case 116;
				}
			}
			case 115: {
				if (t == null) { currentState = 115; break; }
				if (t.kind == 17) {
					currentState = 115;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 116: {
				if (t == null) { currentState = 116; break; }
				Expect(10, t); // XmlOpenTag
				currentState = 117;
				break;
			}
			case 117: {
				if (t == null) { currentState = 117; break; }
				if (set[24, t.kind]) {
					currentState = 117;
					break;
				} else {
					if (t.kind == 14) {
						goto case 16;
					} else {
						if (t.kind == 11) {
							goto case 118;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 118: {
				if (t == null) { currentState = 118; break; }
				currentState = 119;
				break;
			}
			case 119: {
				if (t == null) { currentState = 119; break; }
				if (set[25, t.kind]) {
					if (set[26, t.kind]) {
						goto case 118;
					} else {
						if (t.kind == 10) {
							stateStack.Push(119);
							goto case 116;
						} else {
							Error(t);
							goto case 119;
						}
					}
				} else {
					Expect(15, t); // XmlOpenEndTag
					currentState = 120;
					break;
				}
			}
			case 120: {
				if (t == null) { currentState = 120; break; }
				if (set[27, t.kind]) {
					currentState = 120;
					break;
				} else {
					Expect(11, t); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 121: {
				if (t == null) { currentState = 121; break; }
				Expect(36, t); // "("
				currentState = 122;
				break;
			}
			case 122: {
				readXmlIdentifier = true;
				stateStack.Push(45);
				goto case 89;
			}
			case 123: {
				if (t == null) { currentState = 123; break; }
				Expect(36, t); // "("
				currentState = 110;
				break;
			}
			case 124: {
				goto case 89;
			}
			case 125: {
				if (t == null) { currentState = 125; break; }
				currentState = 36;
				break;
			}
			case 126: {
				if (t == null) { currentState = 126; break; }
				Expect(116, t); // "Erase"
				currentState = 127;
				break;
			}
			case 127: {
				stateStack.Push(128);
				goto case 37;
			}
			case 128: {
				if (t == null) { currentState = 128; break; }
				if (t.kind == 23) {
					currentState = 127;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 129: {
				if (t == null) { currentState = 129; break; }
				Expect(189, t); // "ReDim"
				currentState = 130;
				break;
			}
			case 130: {
				if (t == null) { currentState = 130; break; }
				if (t.kind == 182) {
					goto case 125;
				} else {
					goto case 36;
				}
			}
			case 131: {
				if (t == null) { currentState = 131; break; }
				if (t.kind == 131) {
					currentState = 136;
					break;
				} else {
					if (t.kind == 119) {
						currentState = 135;
						break;
					} else {
						if (t.kind == 88) {
							currentState = 134;
							break;
						} else {
							if (t.kind == 204) {
								goto case 16;
							} else {
								if (t.kind == 193) {
									currentState = 132;
									break;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 132: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 133;
			}
			case 133: {
				if (t == null) { currentState = 133; break; }
				if (set[16, t.kind]) {
					goto case 36;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 134: {
				if (t == null) { currentState = 134; break; }
				if (t.kind == 107 || t.kind == 123 || t.kind == 229) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 135: {
				if (t == null) { currentState = 135; break; }
				if (set[28, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 136: {
				if (t == null) { currentState = 136; break; }
				if (set[17, t.kind]) {
					goto case 124;
				} else {
					if (t.kind == 5) {
						goto case 16;
					} else {
						goto case 6;
					}
				}
			}
			case 137: {
				if (t == null) { currentState = 137; break; }
				Expect(216, t); // "Try"
				currentState = 138;
				break;
			}
			case 138: {
				stateStack.Push(139);
				goto case 31;
			}
			case 139: {
				if (t == null) { currentState = 139; break; }
				if (t.kind == 74) {
					currentState = 143;
					break;
				} else {
					if (t.kind == 122) {
						currentState = 142;
						break;
					} else {
						goto case 140;
					}
				}
			}
			case 140: {
				if (t == null) { currentState = 140; break; }
				Expect(112, t); // "End"
				currentState = 141;
				break;
			}
			case 141: {
				if (t == null) { currentState = 141; break; }
				Expect(216, t); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 142: {
				stateStack.Push(140);
				goto case 31;
			}
			case 143: {
				if (t == null) { currentState = 143; break; }
				if (set[17, t.kind]) {
					stateStack.Push(146);
					goto case 89;
				} else {
					goto case 144;
				}
			}
			case 144: {
				if (t == null) { currentState = 144; break; }
				if (t.kind == 227) {
					currentState = 145;
					break;
				} else {
					goto case 138;
				}
			}
			case 145: {
				stateStack.Push(138);
				goto case 37;
			}
			case 146: {
				if (t == null) { currentState = 146; break; }
				if (t.kind == 62) {
					currentState = 147;
					break;
				} else {
					goto case 144;
				}
			}
			case 147: {
				stateStack.Push(144);
				goto case 69;
			}
			case 148: {
				if (t == null) { currentState = 148; break; }
				Expect(213, t); // "Throw"
				currentState = 149;
				break;
			}
			case 149: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 133;
			}
			case 150: {
				if (t == null) { currentState = 150; break; }
				if (t.kind == 117 || t.kind == 170) {
					if (t.kind == 170) {
						currentState = 153;
						break;
					} else {
						goto case 153;
					}
				} else {
					if (t.kind == 192) {
						currentState = 151;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 151: {
				if (t == null) { currentState = 151; break; }
				if (t.kind == 5 || t.kind == 162) {
					goto case 16;
				} else {
					goto case 152;
				}
			}
			case 152: {
				if (t == null) { currentState = 152; break; }
				if (set[17, t.kind]) {
					goto case 124;
				} else {
					goto case 6;
				}
			}
			case 153: {
				if (t == null) { currentState = 153; break; }
				Expect(117, t); // "Error"
				currentState = 154;
				break;
			}
			case 154: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 155;
			}
			case 155: {
				if (t == null) { currentState = 155; break; }
				if (set[16, t.kind]) {
					goto case 36;
				} else {
					if (t.kind == 131) {
						currentState = 157;
						break;
					} else {
						if (t.kind == 192) {
							currentState = 156;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 156: {
				if (t == null) { currentState = 156; break; }
				Expect(162, t); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 157: {
				if (t == null) { currentState = 157; break; }
				if (t.kind == 5) {
					goto case 16;
				} else {
					goto case 152;
				}
			}
			case 158: {
				if (t == null) { currentState = 158; break; }
				Expect(123, t); // "For"
				currentState = 159;
				break;
			}
			case 159: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 160;
			}
			case 160: {
				if (t == null) { currentState = 160; break; }
				if (set[14, t.kind]) {
					goto case 174;
				} else {
					if (t.kind == 109) {
						goto case 161;
					} else {
						goto case 6;
					}
				}
			}
			case 161: {
				if (t == null) { currentState = 161; break; }
				Expect(109, t); // "Each"
				currentState = 162;
				break;
			}
			case 162: {
				stateStack.Push(163);
				goto case 171;
			}
			case 163: {
				if (t == null) { currentState = 163; break; }
				Expect(137, t); // "In"
				currentState = 164;
				break;
			}
			case 164: {
				stateStack.Push(165);
				goto case 37;
			}
			case 165: {
				stateStack.Push(166);
				goto case 31;
			}
			case 166: {
				if (t == null) { currentState = 166; break; }
				Expect(162, t); // "Next"
				currentState = 167;
				break;
			}
			case 167: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 168;
			}
			case 168: {
				if (t == null) { currentState = 168; break; }
				if (set[16, t.kind]) {
					goto case 169;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 169: {
				stateStack.Push(170);
				goto case 37;
			}
			case 170: {
				if (t == null) { currentState = 170; break; }
				if (t.kind == 23) {
					currentState = 169;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 171: {
				stateStack.Push(172);
				goto case 104;
			}
			case 172: {
				if (t == null) { currentState = 172; break; }
				if (t.kind == 32) {
					currentState = 173;
					break;
				} else {
					goto case 173;
				}
			}
			case 173: {
				if (t == null) { currentState = 173; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(173);
					goto case 96;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 174: {
				stateStack.Push(175);
				goto case 171;
			}
			case 175: {
				if (t == null) { currentState = 175; break; }
				Expect(21, t); // "="
				currentState = 176;
				break;
			}
			case 176: {
				stateStack.Push(177);
				goto case 37;
			}
			case 177: {
				if (t == null) { currentState = 177; break; }
				if (t.kind == 203) {
					currentState = 184;
					break;
				} else {
					goto case 178;
				}
			}
			case 178: {
				stateStack.Push(179);
				goto case 31;
			}
			case 179: {
				if (t == null) { currentState = 179; break; }
				Expect(162, t); // "Next"
				currentState = 180;
				break;
			}
			case 180: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 181;
			}
			case 181: {
				if (t == null) { currentState = 181; break; }
				if (set[16, t.kind]) {
					goto case 182;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 182: {
				stateStack.Push(183);
				goto case 37;
			}
			case 183: {
				if (t == null) { currentState = 183; break; }
				if (t.kind == 23) {
					currentState = 182;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 184: {
				stateStack.Push(178);
				goto case 37;
			}
			case 185: {
				if (t == null) { currentState = 185; break; }
				Expect(107, t); // "Do"
				currentState = 186;
				break;
			}
			case 186: {
				if (t == null) { currentState = 186; break; }
				if (t.kind == 222 || t.kind == 229) {
					goto case 190;
				} else {
					if (t.kind == 1 || t.kind == 22) {
						goto case 187;
					} else {
						goto case 6;
					}
				}
			}
			case 187: {
				stateStack.Push(188);
				goto case 31;
			}
			case 188: {
				if (t == null) { currentState = 188; break; }
				Expect(151, t); // "Loop"
				currentState = 189;
				break;
			}
			case 189: {
				if (t == null) { currentState = 189; break; }
				if (t.kind == 222 || t.kind == 229) {
					goto case 125;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 190: {
				if (t == null) { currentState = 190; break; }
				if (t.kind == 222 || t.kind == 229) {
					currentState = 191;
					break;
				} else {
					Error(t);
					goto case 191;
				}
			}
			case 191: {
				stateStack.Push(192);
				goto case 37;
			}
			case 192: {
				stateStack.Push(193);
				goto case 31;
			}
			case 193: {
				if (t == null) { currentState = 193; break; }
				Expect(151, t); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 194: {
				if (t == null) { currentState = 194; break; }
				Expect(229, t); // "While"
				currentState = 195;
				break;
			}
			case 195: {
				stateStack.Push(196);
				goto case 37;
			}
			case 196: {
				stateStack.Push(197);
				goto case 31;
			}
			case 197: {
				if (t == null) { currentState = 197; break; }
				Expect(112, t); // "End"
				currentState = 198;
				break;
			}
			case 198: {
				if (t == null) { currentState = 198; break; }
				Expect(229, t); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 199: {
				if (t == null) { currentState = 199; break; }
				Expect(195, t); // "Select"
				currentState = 200;
				break;
			}
			case 200: {
				if (t == null) { currentState = 200; break; }
				if (t.kind == 73) {
					currentState = 201;
					break;
				} else {
					goto case 201;
				}
			}
			case 201: {
				stateStack.Push(202);
				goto case 37;
			}
			case 202: {
				stateStack.Push(203);
				goto case 15;
			}
			case 203: {
				if (t == null) { currentState = 203; break; }
				if (t.kind == 73) {
					currentState = 205;
					break;
				} else {
					Expect(112, t); // "End"
					currentState = 204;
					break;
				}
			}
			case 204: {
				if (t == null) { currentState = 204; break; }
				Expect(195, t); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 205: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 206;
			}
			case 206: {
				if (t == null) { currentState = 206; break; }
				if (t.kind == 110) {
					currentState = 207;
					break;
				} else {
					if (set[29, t.kind]) {
						goto case 208;
					} else {
						Error(t);
						goto case 207;
					}
				}
			}
			case 207: {
				stateStack.Push(203);
				goto case 31;
			}
			case 208: {
				if (t == null) { currentState = 208; break; }
				if (set[30, t.kind]) {
					if (t.kind == 143) {
						currentState = 211;
						break;
					} else {
						goto case 211;
					}
				} else {
					if (set[16, t.kind]) {
						stateStack.Push(209);
						goto case 37;
					} else {
						Error(t);
						goto case 209;
					}
				}
			}
			case 209: {
				if (t == null) { currentState = 209; break; }
				if (t.kind == 23) {
					currentState = 210;
					break;
				} else {
					goto case 207;
				}
			}
			case 210: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 208;
			}
			case 211: {
				stateStack.Push(212);
				goto case 213;
			}
			case 212: {
				stateStack.Push(209);
				goto case 50;
			}
			case 213: {
				if (t == null) { currentState = 213; break; }
				if (set[31, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 214: {
				if (t == null) { currentState = 214; break; }
				Expect(134, t); // "If"
				currentState = 215;
				break;
			}
			case 215: {
				stateStack.Push(216);
				goto case 37;
			}
			case 216: {
				if (t == null) { currentState = 216; break; }
				if (t.kind == 212) {
					currentState = 225;
					break;
				} else {
					goto case 217;
				}
			}
			case 217: {
				if (t == null) { currentState = 217; break; }
				if (t.kind == 1 || t.kind == 22) {
					goto case 218;
				} else {
					goto case 6;
				}
			}
			case 218: {
				stateStack.Push(219);
				goto case 31;
			}
			case 219: {
				if (t == null) { currentState = 219; break; }
				if (t.kind == 110 || t.kind == 111) {
					if (t.kind == 110) {
						currentState = 224;
						break;
					} else {
						if (t.kind == 111) {
							goto case 221;
						} else {
							Error(t);
							goto case 218;
						}
					}
				} else {
					Expect(112, t); // "End"
					currentState = 220;
					break;
				}
			}
			case 220: {
				if (t == null) { currentState = 220; break; }
				Expect(134, t); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 221: {
				if (t == null) { currentState = 221; break; }
				currentState = 222;
				break;
			}
			case 222: {
				stateStack.Push(223);
				goto case 37;
			}
			case 223: {
				if (t == null) { currentState = 223; break; }
				if (t.kind == 212) {
					currentState = 218;
					break;
				} else {
					goto case 218;
				}
			}
			case 224: {
				if (t == null) { currentState = 224; break; }
				if (t.kind == 134) {
					goto case 221;
				} else {
					goto case 218;
				}
			}
			case 225: {
				if (t == null) { currentState = 225; break; }
				if (set[8, t.kind]) {
					goto case 226;
				} else {
					goto case 217;
				}
			}
			case 226: {
				stateStack.Push(227);
				goto case 34;
			}
			case 227: {
				if (t == null) { currentState = 227; break; }
				if (t.kind == 22) {
					currentState = 232;
					break;
				} else {
					if (t.kind == 110) {
						goto case 229;
					} else {
						goto case 228;
					}
				}
			}
			case 228: {
				if (t == null) { currentState = 228; break; }
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 229: {
				if (t == null) { currentState = 229; break; }
				currentState = 230;
				break;
			}
			case 230: {
				if (t == null) { currentState = 230; break; }
				if (set[8, t.kind]) {
					stateStack.Push(231);
					goto case 34;
				} else {
					goto case 231;
				}
			}
			case 231: {
				if (t == null) { currentState = 231; break; }
				if (t.kind == 22) {
					goto case 229;
				} else {
					goto case 228;
				}
			}
			case 232: {
				if (t == null) { currentState = 232; break; }
				if (set[8, t.kind]) {
					goto case 226;
				} else {
					goto case 227;
				}
			}
			case 233: {
				if (t == null) { currentState = 233; break; }
				Expect(187, t); // "RaiseEvent"
				currentState = 234;
				break;
			}
			case 234: {
				stateStack.Push(235);
				goto case 16;
			}
			case 235: {
				if (t == null) { currentState = 235; break; }
				if (t.kind == 36) {
					currentState = 76;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 236: {
				if (t == null) { currentState = 236; break; }
				if (t.kind == 55 || t.kind == 191) {
					currentState = 237;
					break;
				} else {
					Error(t);
					goto case 237;
				}
			}
			case 237: {
				stateStack.Push(238);
				goto case 37;
			}
			case 238: {
				if (t == null) { currentState = 238; break; }
				Expect(23, t); // ","
				currentState = 36;
				break;
			}
			case 239: {
				if (t == null) { currentState = 239; break; }
				if (t.kind == 209 || t.kind == 231) {
					currentState = 240;
					break;
				} else {
					Error(t);
					goto case 240;
				}
			}
			case 240: {
				stateStack.Push(241);
				goto case 37;
			}
			case 241: {
				stateStack.Push(242);
				goto case 31;
			}
			case 242: {
				if (t == null) { currentState = 242; break; }
				Expect(112, t); // "End"
				currentState = 243;
				break;
			}
			case 243: {
				if (t == null) { currentState = 243; break; }
				if (t.kind == 209 || t.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 244: {
				if (t == null) { currentState = 244; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					currentState = 245;
					break;
				} else {
					Error(t);
					goto case 245;
				}
			}
			case 245: {
				stateStack.Push(246);
				goto case 89;
			}
			case 246: {
				if (t == null) { currentState = 246; break; }
				if (t.kind == 32) {
					currentState = 247;
					break;
				} else {
					goto case 247;
				}
			}
			case 247: {
				if (t == null) { currentState = 247; break; }
				if (t.kind == 36) {
					goto case 259;
				} else {
					goto case 248;
				}
			}
			case 248: {
				if (t == null) { currentState = 248; break; }
				if (t.kind == 23) {
					currentState = 253;
					break;
				} else {
					if (t.kind == 62) {
						currentState = 250;
						break;
					} else {
						goto case 249;
					}
				}
			}
			case 249: {
				if (t == null) { currentState = 249; break; }
				if (t.kind == 21) {
					goto case 125;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 250: {
				if (t == null) { currentState = 250; break; }
				if (t.kind == 161) {
					goto case 252;
				} else {
					goto case 251;
				}
			}
			case 251: {
				stateStack.Push(249);
				goto case 69;
			}
			case 252: {
				if (t == null) { currentState = 252; break; }
				currentState = 251;
				break;
			}
			case 253: {
				stateStack.Push(254);
				goto case 89;
			}
			case 254: {
				if (t == null) { currentState = 254; break; }
				if (t.kind == 32) {
					currentState = 255;
					break;
				} else {
					goto case 255;
				}
			}
			case 255: {
				if (t == null) { currentState = 255; break; }
				if (t.kind == 36) {
					goto case 256;
				} else {
					goto case 248;
				}
			}
			case 256: {
				if (t == null) { currentState = 256; break; }
				currentState = 257;
				break;
			}
			case 257: {
				if (t == null) { currentState = 257; break; }
				if (t.kind == 23) {
					goto case 256;
				} else {
					goto case 258;
				}
			}
			case 258: {
				if (t == null) { currentState = 258; break; }
				Expect(37, t); // ")"
				currentState = 248;
				break;
			}
			case 259: {
				if (t == null) { currentState = 259; break; }
				currentState = 260;
				break;
			}
			case 260: {
				if (t == null) { currentState = 260; break; }
				if (t.kind == 23) {
					goto case 259;
				} else {
					goto case 258;
				}
			}
			case 261: {
				if (t == null) { currentState = 261; break; }
				if (t.kind == 39) {
					stateStack.Push(261);
					goto case 262;
				} else {
					stateStack.Push(27);
					goto case 69;
				}
			}
			case 262: {
				if (t == null) { currentState = 262; break; }
				Expect(39, t); // "<"
				currentState = 263;
				break;
			}
			case 263: {
				PushContext(Context.Attribute, t);
				goto case 264;
			}
			case 264: {
				if (t == null) { currentState = 264; break; }
				if (set[32, t.kind]) {
					currentState = 264;
					break;
				} else {
					Expect(38, t); // ">"
					currentState = 265;
					break;
				}
			}
			case 265: {
				PopContext();
				goto case 266;
			}
			case 266: {
				if (t == null) { currentState = 266; break; }
				if (t.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 267: {
				stateStack.Push(268);
				goto case 269;
			}
			case 268: {
				if (t == null) { currentState = 268; break; }
				if (t.kind == 23) {
					currentState = 267;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 269: {
				if (t == null) { currentState = 269; break; }
				if (t.kind == 39) {
					stateStack.Push(269);
					goto case 262;
				} else {
					goto case 270;
				}
			}
			case 270: {
				if (t == null) { currentState = 270; break; }
				if (set[33, t.kind]) {
					currentState = 270;
					break;
				} else {
					stateStack.Push(271);
					goto case 89;
				}
			}
			case 271: {
				if (t == null) { currentState = 271; break; }
				if (t.kind == 62) {
					goto case 252;
				} else {
					goto case 249;
				}
			}
			case 272: {
				if (t == null) { currentState = 272; break; }
				Expect(97, t); // "Custom"
				currentState = 273;
				break;
			}
			case 273: {
				stateStack.Push(274);
				goto case 284;
			}
			case 274: {
				if (t == null) { currentState = 274; break; }
				if (set[34, t.kind]) {
					goto case 276;
				} else {
					Expect(112, t); // "End"
					currentState = 275;
					break;
				}
			}
			case 275: {
				if (t == null) { currentState = 275; break; }
				Expect(118, t); // "Event"
				currentState = 30;
				break;
			}
			case 276: {
				if (t == null) { currentState = 276; break; }
				if (t.kind == 39) {
					stateStack.Push(276);
					goto case 262;
				} else {
					if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
						currentState = 277;
						break;
					} else {
						Error(t);
						goto case 277;
					}
				}
			}
			case 277: {
				if (t == null) { currentState = 277; break; }
				Expect(36, t); // "("
				currentState = 278;
				break;
			}
			case 278: {
				stateStack.Push(279);
				goto case 267;
			}
			case 279: {
				if (t == null) { currentState = 279; break; }
				Expect(37, t); // ")"
				currentState = 280;
				break;
			}
			case 280: {
				stateStack.Push(281);
				goto case 31;
			}
			case 281: {
				if (t == null) { currentState = 281; break; }
				Expect(112, t); // "End"
				currentState = 282;
				break;
			}
			case 282: {
				if (t == null) { currentState = 282; break; }
				if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
					currentState = 283;
					break;
				} else {
					Error(t);
					goto case 283;
				}
			}
			case 283: {
				stateStack.Push(274);
				goto case 15;
			}
			case 284: {
				if (t == null) { currentState = 284; break; }
				Expect(118, t); // "Event"
				currentState = 285;
				break;
			}
			case 285: {
				stateStack.Push(286);
				goto case 89;
			}
			case 286: {
				if (t == null) { currentState = 286; break; }
				if (t.kind == 62) {
					currentState = 293;
					break;
				} else {
					if (set[35, t.kind]) {
						if (t.kind == 36) {
							currentState = 291;
							break;
						} else {
							goto case 287;
						}
					} else {
						Error(t);
						goto case 287;
					}
				}
			}
			case 287: {
				if (t == null) { currentState = 287; break; }
				if (t.kind == 135) {
					goto case 288;
				} else {
					goto case 30;
				}
			}
			case 288: {
				if (t == null) { currentState = 288; break; }
				currentState = 289;
				break;
			}
			case 289: {
				stateStack.Push(290);
				goto case 69;
			}
			case 290: {
				if (t == null) { currentState = 290; break; }
				if (t.kind == 23) {
					goto case 288;
				} else {
					goto case 30;
				}
			}
			case 291: {
				if (t == null) { currentState = 291; break; }
				if (set[36, t.kind]) {
					stateStack.Push(292);
					goto case 267;
				} else {
					goto case 292;
				}
			}
			case 292: {
				if (t == null) { currentState = 292; break; }
				Expect(37, t); // ")"
				currentState = 287;
				break;
			}
			case 293: {
				stateStack.Push(287);
				goto case 69;
			}
			case 294: {
				if (t == null) { currentState = 294; break; }
				Expect(100, t); // "Declare"
				currentState = 295;
				break;
			}
			case 295: {
				if (t == null) { currentState = 295; break; }
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					currentState = 296;
					break;
				} else {
					goto case 296;
				}
			}
			case 296: {
				if (t == null) { currentState = 296; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 297;
					break;
				} else {
					Error(t);
					goto case 297;
				}
			}
			case 297: {
				stateStack.Push(298);
				goto case 89;
			}
			case 298: {
				if (t == null) { currentState = 298; break; }
				Expect(148, t); // "Lib"
				currentState = 299;
				break;
			}
			case 299: {
				if (t == null) { currentState = 299; break; }
				Expect(3, t); // LiteralString
				currentState = 300;
				break;
			}
			case 300: {
				if (t == null) { currentState = 300; break; }
				if (t.kind == 58) {
					currentState = 304;
					break;
				} else {
					goto case 301;
				}
			}
			case 301: {
				if (t == null) { currentState = 301; break; }
				if (t.kind == 36) {
					currentState = 302;
					break;
				} else {
					goto case 30;
				}
			}
			case 302: {
				if (t == null) { currentState = 302; break; }
				if (set[36, t.kind]) {
					stateStack.Push(303);
					goto case 267;
				} else {
					goto case 303;
				}
			}
			case 303: {
				if (t == null) { currentState = 303; break; }
				Expect(37, t); // ")"
				currentState = 30;
				break;
			}
			case 304: {
				if (t == null) { currentState = 304; break; }
				Expect(3, t); // LiteralString
				currentState = 301;
				break;
			}
			case 305: {
				if (t == null) { currentState = 305; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 306;
					break;
				} else {
					Error(t);
					goto case 306;
				}
			}
			case 306: {
				PushContext(Context.IdentifierExpected, t);
				goto case 307;
			}
			case 307: {
				if (t == null) { currentState = 307; break; }
				currentState = 308;
				break;
			}
			case 308: {
				PopContext();
				goto case 309;
			}
			case 309: {
				if (t == null) { currentState = 309; break; }
				if (t.kind == 36) {
					currentState = 315;
					break;
				} else {
					goto case 310;
				}
			}
			case 310: {
				if (t == null) { currentState = 310; break; }
				if (t.kind == 62) {
					currentState = 314;
					break;
				} else {
					goto case 311;
				}
			}
			case 311: {
				stateStack.Push(312);
				goto case 31;
			}
			case 312: {
				if (t == null) { currentState = 312; break; }
				Expect(112, t); // "End"
				currentState = 313;
				break;
			}
			case 313: {
				if (t == null) { currentState = 313; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 30;
					break;
				} else {
					Error(t);
					goto case 30;
				}
			}
			case 314: {
				stateStack.Push(311);
				goto case 69;
			}
			case 315: {
				if (t == null) { currentState = 315; break; }
				if (set[36, t.kind]) {
					stateStack.Push(316);
					goto case 267;
				} else {
					goto case 316;
				}
			}
			case 316: {
				if (t == null) { currentState = 316; break; }
				Expect(37, t); // ")"
				currentState = 310;
				break;
			}
			case 317: {
				if (t == null) { currentState = 317; break; }
				if (t.kind == 87) {
					currentState = 318;
					break;
				} else {
					goto case 318;
				}
			}
			case 318: {
				stateStack.Push(319);
				goto case 323;
			}
			case 319: {
				if (t == null) { currentState = 319; break; }
				if (t.kind == 62) {
					currentState = 322;
					break;
				} else {
					goto case 320;
				}
			}
			case 320: {
				if (t == null) { currentState = 320; break; }
				if (t.kind == 21) {
					currentState = 321;
					break;
				} else {
					goto case 30;
				}
			}
			case 321: {
				stateStack.Push(30);
				goto case 37;
			}
			case 322: {
				stateStack.Push(320);
				goto case 69;
			}
			case 323: {
				if (t == null) { currentState = 323; break; }
				if (set[37, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 324: {
				if (t == null) { currentState = 324; break; }
				currentState = 9;
				break;
			}
			case 325: {
				if (t == null) { currentState = 325; break; }
				Expect(159, t); // "Namespace"
				currentState = 326;
				break;
			}
			case 326: {
				if (t == null) { currentState = 326; break; }
				if (set[3, t.kind]) {
					currentState = 326;
					break;
				} else {
					stateStack.Push(327);
					goto case 15;
				}
			}
			case 327: {
				if (t == null) { currentState = 327; break; }
				if (set[38, t.kind]) {
					stateStack.Push(327);
					goto case 5;
				} else {
					Expect(112, t); // "End"
					currentState = 328;
					break;
				}
			}
			case 328: {
				if (t == null) { currentState = 328; break; }
				Expect(159, t); // "Namespace"
				currentState = 30;
				break;
			}
			case 329: {
				if (t == null) { currentState = 329; break; }
				Expect(136, t); // "Imports"
				currentState = 330;
				break;
			}
			case 330: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 331;
			}
			case 331: {
				if (t == null) { currentState = 331; break; }
				if (set[3, t.kind]) {
					currentState = 331;
					break;
				} else {
					goto case 30;
				}
			}
			case 332: {
				if (t == null) { currentState = 332; break; }
				Expect(172, t); // "Option"
				currentState = 333;
				break;
			}
			case 333: {
				if (t == null) { currentState = 333; break; }
				if (set[3, t.kind]) {
					currentState = 333;
					break;
				} else {
					goto case 30;
				}
			}
		}

	}
	
	public void Advance()
	{
		Console.WriteLine("Advance");
		InformTokenInternal(null);
	}
	
	static readonly bool[,] set = {
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,T, T,x,T,x, x,x,x,x, x,x,x,T, x,x,T,x, T,x,x,x, T,T,T,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,T,x,T, x,x,x,T, x,T,T,T, x,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,T,x, T,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,T,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, x,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, x,T,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,x,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,T, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
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