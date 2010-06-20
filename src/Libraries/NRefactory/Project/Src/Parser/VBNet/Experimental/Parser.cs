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
					goto case 317;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (t == null) { currentState = 2; break; }
				if (t.kind == 136) {
					stateStack.Push(2);
					goto case 314;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (t == null) { currentState = 3; break; }
				if (t.kind == 39) {
					stateStack.Push(3);
					goto case 247;
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
					goto case 310;
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
					goto case 247;
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
						goto case 309;
					} else {
						Error(t);
						goto case 9;
					}
				}
			}
			case 9: {
				if (t == null) { currentState = 9; break; }
				if (set[3, t.kind]) {
					goto case 309;
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
					PushContext(Context.Member, t);
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
				if (t == null) { currentState = 17; break; }
				if (t.kind == 39) {
					stateStack.Push(17);
					goto case 247;
				} else {
					goto case 18;
				}
			}
			case 18: {
				if (t == null) { currentState = 18; break; }
				if (set[5, t.kind]) {
					currentState = 18;
					break;
				} else {
					if (set[6, t.kind]) {
						stateStack.Push(19);
						goto case 302;
					} else {
						if (t.kind == 126 || t.kind == 208) {
							stateStack.Push(19);
							goto case 290;
						} else {
							if (t.kind == 100) {
								stateStack.Push(19);
								goto case 279;
							} else {
								if (t.kind == 118) {
									stateStack.Push(19);
									goto case 269;
								} else {
									if (t.kind == 97) {
										stateStack.Push(19);
										goto case 257;
									} else {
										if (t.kind == 171) {
											stateStack.Push(19);
											goto case 20;
										} else {
											Error(t);
											goto case 19;
										}
									}
								}
							}
						}
					}
				}
			}
			case 19: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 20: {
				if (t == null) { currentState = 20; break; }
				Expect(171, t); // "Operator"
				currentState = 21;
				break;
			}
			case 21: {
				if (t == null) { currentState = 21; break; }
				currentState = 22;
				break;
			}
			case 22: {
				if (t == null) { currentState = 22; break; }
				Expect(36, t); // "("
				currentState = 23;
				break;
			}
			case 23: {
				stateStack.Push(24);
				goto case 252;
			}
			case 24: {
				if (t == null) { currentState = 24; break; }
				Expect(37, t); // ")"
				currentState = 25;
				break;
			}
			case 25: {
				if (t == null) { currentState = 25; break; }
				if (t.kind == 62) {
					currentState = 246;
					break;
				} else {
					goto case 26;
				}
			}
			case 26: {
				stateStack.Push(27);
				goto case 29;
			}
			case 27: {
				if (t == null) { currentState = 27; break; }
				Expect(112, t); // "End"
				currentState = 28;
				break;
			}
			case 28: {
				if (t == null) { currentState = 28; break; }
				Expect(171, t); // "Operator"
				currentState = 15;
				break;
			}
			case 29: {
				PushContext(Context.Body, t);
				goto case 30;
			}
			case 30: {
				stateStack.Push(31);
				goto case 15;
			}
			case 31: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 32;
			}
			case 32: {
				if (t == null) { currentState = 32; break; }
				if (set[7, t.kind]) {
					if (set[8, t.kind]) {
						stateStack.Push(30);
						goto case 33;
					} else {
						goto case 30;
					}
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 33: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 34;
			}
			case 34: {
				if (t == null) { currentState = 34; break; }
				if (t.kind == 87 || t.kind == 104 || t.kind == 202) {
					currentState = 230;
					break;
				} else {
					if (t.kind == 209 || t.kind == 231) {
						currentState = 226;
						break;
					} else {
						if (t.kind == 55 || t.kind == 191) {
							currentState = 224;
							break;
						} else {
							if (t.kind == 187) {
								currentState = 222;
								break;
							} else {
								if (t.kind == 134) {
									currentState = 201;
									break;
								} else {
									if (t.kind == 195) {
										currentState = 186;
										break;
									} else {
										if (t.kind == 229) {
											currentState = 182;
											break;
										} else {
											if (t.kind == 107) {
												currentState = 176;
												break;
											} else {
												if (t.kind == 123) {
													currentState = 152;
													break;
												} else {
													if (t.kind == 117 || t.kind == 170 || t.kind == 192) {
														if (t.kind == 117 || t.kind == 170) {
															if (t.kind == 170) {
																currentState = 147;
																break;
															} else {
																goto case 147;
															}
														} else {
															if (t.kind == 192) {
																currentState = 145;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (t.kind == 213) {
															goto case 129;
														} else {
															if (t.kind == 216) {
																currentState = 135;
																break;
															} else {
																if (set[9, t.kind]) {
																	if (t.kind == 131) {
																		currentState = 134;
																		break;
																	} else {
																		if (t.kind == 119) {
																			currentState = 133;
																			break;
																		} else {
																			if (t.kind == 88) {
																				currentState = 132;
																				break;
																			} else {
																				if (t.kind == 204) {
																					goto case 16;
																				} else {
																					if (t.kind == 193) {
																						goto case 129;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (t.kind == 189) {
																		currentState = 127;
																		break;
																	} else {
																		if (t.kind == 116) {
																			goto case 124;
																		} else {
																			if (t.kind == 224) {
																				currentState = 120;
																				break;
																			} else {
																				if (set[10, t.kind]) {
																					if (t.kind == 72) {
																						goto case 119;
																					} else {
																						goto case 35;
																					}
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
			}
			case 35: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 36;
			}
			case 36: {
				if (t == null) { currentState = 36; break; }
				if (set[11, t.kind]) {
					goto case 45;
				} else {
					if (t.kind == 134) {
						currentState = 37;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 37: {
				if (t == null) { currentState = 37; break; }
				Expect(36, t); // "("
				currentState = 38;
				break;
			}
			case 38: {
				stateStack.Push(39);
				goto case 35;
			}
			case 39: {
				if (t == null) { currentState = 39; break; }
				Expect(23, t); // ","
				currentState = 40;
				break;
			}
			case 40: {
				stateStack.Push(41);
				goto case 35;
			}
			case 41: {
				if (t == null) { currentState = 41; break; }
				if (t.kind == 23) {
					goto case 43;
				} else {
					goto case 42;
				}
			}
			case 42: {
				if (t == null) { currentState = 42; break; }
				Expect(37, t); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 43: {
				if (t == null) { currentState = 43; break; }
				currentState = 44;
				break;
			}
			case 44: {
				stateStack.Push(42);
				goto case 35;
			}
			case 45: {
				stateStack.Push(46);
				goto case 47;
			}
			case 46: {
				if (t == null) { currentState = 46; break; }
				if (set[12, t.kind]) {
					currentState = 45;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 47: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 48;
			}
			case 48: {
				if (t == null) { currentState = 48; break; }
				if (set[13, t.kind]) {
					currentState = 47;
					break;
				} else {
					if (set[14, t.kind]) {
						stateStack.Push(94);
						goto case 102;
					} else {
						if (t.kind == 218) {
							currentState = 92;
							break;
						} else {
							if (t.kind == 161) {
								currentState = 49;
								break;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 49: {
				if (t == null) { currentState = 49; break; }
				if (set[15, t.kind]) {
					stateStack.Push(59);
					goto case 65;
				} else {
					goto case 50;
				}
			}
			case 50: {
				if (t == null) { currentState = 50; break; }
				if (t.kind == 231) {
					currentState = 51;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 51: {
				if (t == null) { currentState = 51; break; }
				Expect(34, t); // "{"
				currentState = 52;
				break;
			}
			case 52: {
				if (t == null) { currentState = 52; break; }
				if (t.kind == 146) {
					currentState = 53;
					break;
				} else {
					goto case 53;
				}
			}
			case 53: {
				if (t == null) { currentState = 53; break; }
				Expect(27, t); // "."
				currentState = 54;
				break;
			}
			case 54: {
				stateStack.Push(55);
				goto case 16;
			}
			case 55: {
				if (t == null) { currentState = 55; break; }
				Expect(21, t); // "="
				currentState = 56;
				break;
			}
			case 56: {
				stateStack.Push(57);
				goto case 35;
			}
			case 57: {
				if (t == null) { currentState = 57; break; }
				if (t.kind == 23) {
					currentState = 52;
					break;
				} else {
					goto case 58;
				}
			}
			case 58: {
				if (t == null) { currentState = 58; break; }
				Expect(35, t); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 59: {
				if (t == null) { currentState = 59; break; }
				if (t.kind == 125) {
					currentState = 60;
					break;
				} else {
					goto case 50;
				}
			}
			case 60: {
				stateStack.Push(50);
				goto case 61;
			}
			case 61: {
				if (t == null) { currentState = 61; break; }
				Expect(34, t); // "{"
				currentState = 62;
				break;
			}
			case 62: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 63;
			}
			case 63: {
				if (t == null) { currentState = 63; break; }
				if (set[16, t.kind]) {
					stateStack.Push(64);
					goto case 35;
				} else {
					if (t.kind == 34) {
						stateStack.Push(64);
						goto case 61;
					} else {
						Error(t);
						goto case 64;
					}
				}
			}
			case 64: {
				if (t == null) { currentState = 64; break; }
				if (t.kind == 23) {
					currentState = 62;
					break;
				} else {
					goto case 58;
				}
			}
			case 65: {
				if (t == null) { currentState = 65; break; }
				if (t.kind == 129) {
					goto case 88;
				} else {
					if (set[17, t.kind]) {
						stateStack.Push(66);
						goto case 89;
					} else {
						if (set[18, t.kind]) {
							goto case 88;
						} else {
							Error(t);
							goto case 66;
						}
					}
				}
			}
			case 66: {
				if (t == null) { currentState = 66; break; }
				if (t.kind == 36) {
					stateStack.Push(66);
					goto case 70;
				} else {
					goto case 67;
				}
			}
			case 67: {
				if (t == null) { currentState = 67; break; }
				if (t.kind == 27) {
					currentState = 68;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 68: {
				stateStack.Push(69);
				goto case 16;
			}
			case 69: {
				if (t == null) { currentState = 69; break; }
				if (t.kind == 36) {
					stateStack.Push(69);
					goto case 70;
				} else {
					goto case 67;
				}
			}
			case 70: {
				if (t == null) { currentState = 70; break; }
				Expect(36, t); // "("
				currentState = 71;
				break;
			}
			case 71: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 72;
			}
			case 72: {
				if (t == null) { currentState = 72; break; }
				if (t.kind == 168) {
					goto case 85;
				} else {
					if (set[19, t.kind]) {
						goto case 73;
					} else {
						Error(t);
						goto case 42;
					}
				}
			}
			case 73: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 74;
			}
			case 74: {
				if (t == null) { currentState = 74; break; }
				if (set[20, t.kind]) {
					goto case 75;
				} else {
					goto case 42;
				}
			}
			case 75: {
				stateStack.Push(42);
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 76;
			}
			case 76: {
				if (t == null) { currentState = 76; break; }
				if (set[16, t.kind]) {
					goto case 81;
				} else {
					if (t.kind == 23) {
						goto case 77;
					} else {
						goto case 6;
					}
				}
			}
			case 77: {
				if (t == null) { currentState = 77; break; }
				currentState = 78;
				break;
			}
			case 78: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 79;
			}
			case 79: {
				if (t == null) { currentState = 79; break; }
				if (set[16, t.kind]) {
					stateStack.Push(80);
					goto case 35;
				} else {
					goto case 80;
				}
			}
			case 80: {
				if (t == null) { currentState = 80; break; }
				if (t.kind == 23) {
					goto case 77;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 81: {
				stateStack.Push(82);
				goto case 35;
			}
			case 82: {
				if (t == null) { currentState = 82; break; }
				if (t.kind == 23) {
					currentState = 83;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 83: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 84;
			}
			case 84: {
				if (t == null) { currentState = 84; break; }
				if (set[16, t.kind]) {
					goto case 81;
				} else {
					goto case 82;
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
					goto case 65;
				} else {
					goto case 87;
				}
			}
			case 87: {
				if (t == null) { currentState = 87; break; }
				if (t.kind == 23) {
					goto case 85;
				} else {
					goto case 42;
				}
			}
			case 88: {
				if (t == null) { currentState = 88; break; }
				currentState = 66;
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
				goto case 47;
			}
			case 93: {
				if (t == null) { currentState = 93; break; }
				Expect(143, t); // "Is"
				currentState = 65;
				break;
			}
			case 94: {
				if (t == null) { currentState = 94; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(94);
					goto case 95;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 95: {
				if (t == null) { currentState = 95; break; }
				if (t.kind == 36) {
					currentState = 97;
					break;
				} else {
					if (t.kind == 27 || t.kind == 28) {
						goto case 96;
					} else {
						goto case 6;
					}
				}
			}
			case 96: {
				if (t == null) { currentState = 96; break; }
				currentState = 16;
				break;
			}
			case 97: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 98;
			}
			case 98: {
				if (t == null) { currentState = 98; break; }
				if (t.kind == 168) {
					goto case 99;
				} else {
					if (set[20, t.kind]) {
						goto case 75;
					} else {
						goto case 6;
					}
				}
			}
			case 99: {
				if (t == null) { currentState = 99; break; }
				currentState = 100;
				break;
			}
			case 100: {
				stateStack.Push(101);
				goto case 65;
			}
			case 101: {
				if (t == null) { currentState = 101; break; }
				if (t.kind == 23) {
					goto case 99;
				} else {
					goto case 42;
				}
			}
			case 102: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 103;
			}
			case 103: {
				if (t == null) { currentState = 103; break; }
				if (set[21, t.kind]) {
					goto case 16;
				} else {
					if (t.kind == 36) {
						goto case 43;
					} else {
						if (set[17, t.kind]) {
							goto case 89;
						} else {
							if (t.kind == 27 || t.kind == 28) {
								goto case 96;
							} else {
								if (t.kind == 128) {
									currentState = 118;
									break;
								} else {
									if (t.kind == 235) {
										currentState = 116;
										break;
									} else {
										if (t.kind == 10 || t.kind == 17) {
											nextTokenIsPotentialStartOfXmlMode = true;
											PushContext(Context.Xml, t);
											goto case 109;
										} else {
											if (set[22, t.kind]) {
												if (set[23, t.kind]) {
													currentState = 108;
													break;
												} else {
													if (t.kind == 93 || t.kind == 105 || t.kind == 217) {
														currentState = 104;
														break;
													} else {
														goto case 6;
													}
												}
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
			case 104: {
				if (t == null) { currentState = 104; break; }
				Expect(36, t); // "("
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(106);
				goto case 35;
			}
			case 106: {
				if (t == null) { currentState = 106; break; }
				Expect(23, t); // ","
				currentState = 107;
				break;
			}
			case 107: {
				stateStack.Push(42);
				goto case 65;
			}
			case 108: {
				if (t == null) { currentState = 108; break; }
				Expect(36, t); // "("
				currentState = 44;
				break;
			}
			case 109: {
				if (t == null) { currentState = 109; break; }
				if (t.kind == 17) {
					currentState = 109;
					break;
				} else {
					stateStack.Push(110);
					goto case 111;
				}
			}
			case 110: {
				if (t == null) { currentState = 110; break; }
				if (t.kind == 17) {
					currentState = 110;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 111: {
				if (t == null) { currentState = 111; break; }
				Expect(10, t); // XmlOpenTag
				currentState = 112;
				break;
			}
			case 112: {
				if (t == null) { currentState = 112; break; }
				if (set[24, t.kind]) {
					currentState = 112;
					break;
				} else {
					if (t.kind == 14) {
						goto case 16;
					} else {
						if (t.kind == 11) {
							goto case 113;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 113: {
				if (t == null) { currentState = 113; break; }
				currentState = 114;
				break;
			}
			case 114: {
				if (t == null) { currentState = 114; break; }
				if (set[25, t.kind]) {
					if (set[26, t.kind]) {
						goto case 113;
					} else {
						if (t.kind == 10) {
							stateStack.Push(114);
							goto case 111;
						} else {
							Error(t);
							goto case 114;
						}
					}
				} else {
					Expect(15, t); // XmlOpenEndTag
					currentState = 115;
					break;
				}
			}
			case 115: {
				if (t == null) { currentState = 115; break; }
				if (set[27, t.kind]) {
					currentState = 115;
					break;
				} else {
					Expect(11, t); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 116: {
				if (t == null) { currentState = 116; break; }
				Expect(36, t); // "("
				currentState = 117;
				break;
			}
			case 117: {
				readXmlIdentifier = true;
				stateStack.Push(42);
				goto case 89;
			}
			case 118: {
				if (t == null) { currentState = 118; break; }
				Expect(36, t); // "("
				currentState = 107;
				break;
			}
			case 119: {
				if (t == null) { currentState = 119; break; }
				currentState = 35;
				break;
			}
			case 120: {
				stateStack.Push(121);
				goto case 35;
			}
			case 121: {
				stateStack.Push(122);
				goto case 29;
			}
			case 122: {
				if (t == null) { currentState = 122; break; }
				Expect(112, t); // "End"
				currentState = 123;
				break;
			}
			case 123: {
				if (t == null) { currentState = 123; break; }
				Expect(224, t); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 124: {
				if (t == null) { currentState = 124; break; }
				currentState = 125;
				break;
			}
			case 125: {
				stateStack.Push(126);
				goto case 35;
			}
			case 126: {
				if (t == null) { currentState = 126; break; }
				if (t.kind == 23) {
					goto case 124;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 127: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 128;
			}
			case 128: {
				if (t == null) { currentState = 128; break; }
				if (t.kind == 182) {
					goto case 119;
				} else {
					goto case 35;
				}
			}
			case 129: {
				if (t == null) { currentState = 129; break; }
				currentState = 130;
				break;
			}
			case 130: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 131;
			}
			case 131: {
				if (t == null) { currentState = 131; break; }
				if (set[16, t.kind]) {
					goto case 35;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 132: {
				if (t == null) { currentState = 132; break; }
				if (t.kind == 107 || t.kind == 123 || t.kind == 229) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 133: {
				if (t == null) { currentState = 133; break; }
				if (set[28, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 134: {
				if (t == null) { currentState = 134; break; }
				if (set[17, t.kind]) {
					goto case 89;
				} else {
					if (t.kind == 5) {
						goto case 16;
					} else {
						goto case 6;
					}
				}
			}
			case 135: {
				stateStack.Push(136);
				goto case 29;
			}
			case 136: {
				if (t == null) { currentState = 136; break; }
				if (t.kind == 74) {
					currentState = 140;
					break;
				} else {
					if (t.kind == 122) {
						currentState = 139;
						break;
					} else {
						goto case 137;
					}
				}
			}
			case 137: {
				if (t == null) { currentState = 137; break; }
				Expect(112, t); // "End"
				currentState = 138;
				break;
			}
			case 138: {
				if (t == null) { currentState = 138; break; }
				Expect(216, t); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 139: {
				stateStack.Push(137);
				goto case 29;
			}
			case 140: {
				if (t == null) { currentState = 140; break; }
				if (set[17, t.kind]) {
					stateStack.Push(143);
					goto case 89;
				} else {
					goto case 141;
				}
			}
			case 141: {
				if (t == null) { currentState = 141; break; }
				if (t.kind == 227) {
					currentState = 142;
					break;
				} else {
					goto case 135;
				}
			}
			case 142: {
				stateStack.Push(135);
				goto case 35;
			}
			case 143: {
				if (t == null) { currentState = 143; break; }
				if (t.kind == 62) {
					currentState = 144;
					break;
				} else {
					goto case 141;
				}
			}
			case 144: {
				stateStack.Push(141);
				goto case 65;
			}
			case 145: {
				if (t == null) { currentState = 145; break; }
				if (t.kind == 5 || t.kind == 162) {
					goto case 16;
				} else {
					goto case 146;
				}
			}
			case 146: {
				if (t == null) { currentState = 146; break; }
				if (set[17, t.kind]) {
					goto case 89;
				} else {
					goto case 6;
				}
			}
			case 147: {
				if (t == null) { currentState = 147; break; }
				Expect(117, t); // "Error"
				currentState = 148;
				break;
			}
			case 148: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 149;
			}
			case 149: {
				if (t == null) { currentState = 149; break; }
				if (set[16, t.kind]) {
					goto case 35;
				} else {
					if (t.kind == 131) {
						currentState = 151;
						break;
					} else {
						if (t.kind == 192) {
							currentState = 150;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 150: {
				if (t == null) { currentState = 150; break; }
				Expect(162, t); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 151: {
				if (t == null) { currentState = 151; break; }
				if (t.kind == 5) {
					goto case 16;
				} else {
					goto case 146;
				}
			}
			case 152: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 153;
			}
			case 153: {
				if (t == null) { currentState = 153; break; }
				if (set[14, t.kind]) {
					stateStack.Push(166);
					goto case 163;
				} else {
					if (t.kind == 109) {
						currentState = 154;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 154: {
				stateStack.Push(155);
				goto case 163;
			}
			case 155: {
				if (t == null) { currentState = 155; break; }
				Expect(137, t); // "In"
				currentState = 156;
				break;
			}
			case 156: {
				stateStack.Push(157);
				goto case 35;
			}
			case 157: {
				stateStack.Push(158);
				goto case 29;
			}
			case 158: {
				if (t == null) { currentState = 158; break; }
				Expect(162, t); // "Next"
				currentState = 159;
				break;
			}
			case 159: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 160;
			}
			case 160: {
				if (t == null) { currentState = 160; break; }
				if (set[16, t.kind]) {
					goto case 161;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 161: {
				stateStack.Push(162);
				goto case 35;
			}
			case 162: {
				if (t == null) { currentState = 162; break; }
				if (t.kind == 23) {
					currentState = 161;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 163: {
				stateStack.Push(164);
				goto case 102;
			}
			case 164: {
				if (t == null) { currentState = 164; break; }
				if (t.kind == 32) {
					currentState = 165;
					break;
				} else {
					goto case 165;
				}
			}
			case 165: {
				if (t == null) { currentState = 165; break; }
				if (t.kind == 27 || t.kind == 28 || t.kind == 36) {
					stateStack.Push(165);
					goto case 95;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 166: {
				if (t == null) { currentState = 166; break; }
				Expect(21, t); // "="
				currentState = 167;
				break;
			}
			case 167: {
				stateStack.Push(168);
				goto case 35;
			}
			case 168: {
				if (t == null) { currentState = 168; break; }
				if (t.kind == 203) {
					currentState = 175;
					break;
				} else {
					goto case 169;
				}
			}
			case 169: {
				stateStack.Push(170);
				goto case 29;
			}
			case 170: {
				if (t == null) { currentState = 170; break; }
				Expect(162, t); // "Next"
				currentState = 171;
				break;
			}
			case 171: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 172;
			}
			case 172: {
				if (t == null) { currentState = 172; break; }
				if (set[16, t.kind]) {
					goto case 173;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 173: {
				stateStack.Push(174);
				goto case 35;
			}
			case 174: {
				if (t == null) { currentState = 174; break; }
				if (t.kind == 23) {
					currentState = 173;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 175: {
				stateStack.Push(169);
				goto case 35;
			}
			case 176: {
				if (t == null) { currentState = 176; break; }
				if (t.kind == 222 || t.kind == 229) {
					currentState = 179;
					break;
				} else {
					if (t.kind == 1 || t.kind == 22) {
						stateStack.Push(177);
						goto case 29;
					} else {
						goto case 6;
					}
				}
			}
			case 177: {
				if (t == null) { currentState = 177; break; }
				Expect(151, t); // "Loop"
				currentState = 178;
				break;
			}
			case 178: {
				if (t == null) { currentState = 178; break; }
				if (t.kind == 222 || t.kind == 229) {
					goto case 119;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 179: {
				stateStack.Push(180);
				goto case 35;
			}
			case 180: {
				stateStack.Push(181);
				goto case 29;
			}
			case 181: {
				if (t == null) { currentState = 181; break; }
				Expect(151, t); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 182: {
				stateStack.Push(183);
				goto case 35;
			}
			case 183: {
				stateStack.Push(184);
				goto case 29;
			}
			case 184: {
				if (t == null) { currentState = 184; break; }
				Expect(112, t); // "End"
				currentState = 185;
				break;
			}
			case 185: {
				if (t == null) { currentState = 185; break; }
				Expect(229, t); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 186: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 187;
			}
			case 187: {
				if (t == null) { currentState = 187; break; }
				if (t.kind == 73) {
					currentState = 188;
					break;
				} else {
					goto case 188;
				}
			}
			case 188: {
				stateStack.Push(189);
				goto case 35;
			}
			case 189: {
				stateStack.Push(190);
				goto case 15;
			}
			case 190: {
				if (t == null) { currentState = 190; break; }
				if (t.kind == 73) {
					currentState = 192;
					break;
				} else {
					Expect(112, t); // "End"
					currentState = 191;
					break;
				}
			}
			case 191: {
				if (t == null) { currentState = 191; break; }
				Expect(195, t); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 192: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 193;
			}
			case 193: {
				if (t == null) { currentState = 193; break; }
				if (t.kind == 110) {
					currentState = 194;
					break;
				} else {
					if (set[29, t.kind]) {
						goto case 195;
					} else {
						Error(t);
						goto case 194;
					}
				}
			}
			case 194: {
				stateStack.Push(190);
				goto case 29;
			}
			case 195: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 196;
			}
			case 196: {
				if (t == null) { currentState = 196; break; }
				if (set[30, t.kind]) {
					if (t.kind == 143) {
						currentState = 198;
						break;
					} else {
						goto case 198;
					}
				} else {
					if (set[16, t.kind]) {
						stateStack.Push(197);
						goto case 35;
					} else {
						Error(t);
						goto case 197;
					}
				}
			}
			case 197: {
				if (t == null) { currentState = 197; break; }
				if (t.kind == 23) {
					currentState = 195;
					break;
				} else {
					goto case 194;
				}
			}
			case 198: {
				stateStack.Push(199);
				goto case 200;
			}
			case 199: {
				stateStack.Push(197);
				goto case 47;
			}
			case 200: {
				if (t == null) { currentState = 200; break; }
				if (set[31, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 201: {
				stateStack.Push(202);
				goto case 35;
			}
			case 202: {
				if (t == null) { currentState = 202; break; }
				if (t.kind == 212) {
					currentState = 211;
					break;
				} else {
					goto case 203;
				}
			}
			case 203: {
				if (t == null) { currentState = 203; break; }
				if (t.kind == 1 || t.kind == 22) {
					goto case 204;
				} else {
					goto case 6;
				}
			}
			case 204: {
				stateStack.Push(205);
				goto case 29;
			}
			case 205: {
				if (t == null) { currentState = 205; break; }
				if (t.kind == 110 || t.kind == 111) {
					if (t.kind == 110) {
						currentState = 210;
						break;
					} else {
						if (t.kind == 111) {
							goto case 207;
						} else {
							Error(t);
							goto case 204;
						}
					}
				} else {
					Expect(112, t); // "End"
					currentState = 206;
					break;
				}
			}
			case 206: {
				if (t == null) { currentState = 206; break; }
				Expect(134, t); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 207: {
				if (t == null) { currentState = 207; break; }
				currentState = 208;
				break;
			}
			case 208: {
				stateStack.Push(209);
				goto case 35;
			}
			case 209: {
				if (t == null) { currentState = 209; break; }
				if (t.kind == 212) {
					currentState = 204;
					break;
				} else {
					goto case 204;
				}
			}
			case 210: {
				if (t == null) { currentState = 210; break; }
				if (t.kind == 134) {
					goto case 207;
				} else {
					goto case 204;
				}
			}
			case 211: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 212;
			}
			case 212: {
				if (t == null) { currentState = 212; break; }
				if (set[8, t.kind]) {
					goto case 213;
				} else {
					goto case 203;
				}
			}
			case 213: {
				stateStack.Push(214);
				goto case 33;
			}
			case 214: {
				if (t == null) { currentState = 214; break; }
				if (t.kind == 22) {
					currentState = 220;
					break;
				} else {
					if (t.kind == 110) {
						goto case 216;
					} else {
						goto case 215;
					}
				}
			}
			case 215: {
				if (t == null) { currentState = 215; break; }
				Expect(1, t); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 216: {
				if (t == null) { currentState = 216; break; }
				currentState = 217;
				break;
			}
			case 217: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 218;
			}
			case 218: {
				if (t == null) { currentState = 218; break; }
				if (set[8, t.kind]) {
					stateStack.Push(219);
					goto case 33;
				} else {
					goto case 219;
				}
			}
			case 219: {
				if (t == null) { currentState = 219; break; }
				if (t.kind == 22) {
					goto case 216;
				} else {
					goto case 215;
				}
			}
			case 220: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 221;
			}
			case 221: {
				if (t == null) { currentState = 221; break; }
				if (set[8, t.kind]) {
					goto case 213;
				} else {
					goto case 214;
				}
			}
			case 222: {
				stateStack.Push(223);
				goto case 16;
			}
			case 223: {
				if (t == null) { currentState = 223; break; }
				if (t.kind == 36) {
					currentState = 73;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 224: {
				stateStack.Push(225);
				goto case 35;
			}
			case 225: {
				if (t == null) { currentState = 225; break; }
				Expect(23, t); // ","
				currentState = 35;
				break;
			}
			case 226: {
				stateStack.Push(227);
				goto case 35;
			}
			case 227: {
				stateStack.Push(228);
				goto case 29;
			}
			case 228: {
				if (t == null) { currentState = 228; break; }
				Expect(112, t); // "End"
				currentState = 229;
				break;
			}
			case 229: {
				if (t == null) { currentState = 229; break; }
				if (t.kind == 209 || t.kind == 231) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 230: {
				stateStack.Push(231);
				goto case 89;
			}
			case 231: {
				if (t == null) { currentState = 231; break; }
				if (t.kind == 32) {
					currentState = 232;
					break;
				} else {
					goto case 232;
				}
			}
			case 232: {
				if (t == null) { currentState = 232; break; }
				if (t.kind == 36) {
					goto case 244;
				} else {
					goto case 233;
				}
			}
			case 233: {
				if (t == null) { currentState = 233; break; }
				if (t.kind == 23) {
					currentState = 238;
					break;
				} else {
					if (t.kind == 62) {
						currentState = 235;
						break;
					} else {
						goto case 234;
					}
				}
			}
			case 234: {
				if (t == null) { currentState = 234; break; }
				if (t.kind == 21) {
					goto case 119;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 235: {
				if (t == null) { currentState = 235; break; }
				if (t.kind == 161) {
					goto case 237;
				} else {
					goto case 236;
				}
			}
			case 236: {
				stateStack.Push(234);
				goto case 65;
			}
			case 237: {
				if (t == null) { currentState = 237; break; }
				currentState = 236;
				break;
			}
			case 238: {
				stateStack.Push(239);
				goto case 89;
			}
			case 239: {
				if (t == null) { currentState = 239; break; }
				if (t.kind == 32) {
					currentState = 240;
					break;
				} else {
					goto case 240;
				}
			}
			case 240: {
				if (t == null) { currentState = 240; break; }
				if (t.kind == 36) {
					goto case 241;
				} else {
					goto case 233;
				}
			}
			case 241: {
				if (t == null) { currentState = 241; break; }
				currentState = 242;
				break;
			}
			case 242: {
				if (t == null) { currentState = 242; break; }
				if (t.kind == 23) {
					goto case 241;
				} else {
					goto case 243;
				}
			}
			case 243: {
				if (t == null) { currentState = 243; break; }
				Expect(37, t); // ")"
				currentState = 233;
				break;
			}
			case 244: {
				if (t == null) { currentState = 244; break; }
				currentState = 245;
				break;
			}
			case 245: {
				if (t == null) { currentState = 245; break; }
				if (t.kind == 23) {
					goto case 244;
				} else {
					goto case 243;
				}
			}
			case 246: {
				if (t == null) { currentState = 246; break; }
				if (t.kind == 39) {
					stateStack.Push(246);
					goto case 247;
				} else {
					stateStack.Push(26);
					goto case 65;
				}
			}
			case 247: {
				if (t == null) { currentState = 247; break; }
				Expect(39, t); // "<"
				currentState = 248;
				break;
			}
			case 248: {
				PushContext(Context.Attribute, t);
				goto case 249;
			}
			case 249: {
				if (t == null) { currentState = 249; break; }
				if (set[32, t.kind]) {
					currentState = 249;
					break;
				} else {
					Expect(38, t); // ">"
					currentState = 250;
					break;
				}
			}
			case 250: {
				PopContext();
				goto case 251;
			}
			case 251: {
				if (t == null) { currentState = 251; break; }
				if (t.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 252: {
				stateStack.Push(253);
				goto case 254;
			}
			case 253: {
				if (t == null) { currentState = 253; break; }
				if (t.kind == 23) {
					currentState = 252;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 254: {
				if (t == null) { currentState = 254; break; }
				if (t.kind == 39) {
					stateStack.Push(254);
					goto case 247;
				} else {
					goto case 255;
				}
			}
			case 255: {
				if (t == null) { currentState = 255; break; }
				if (set[33, t.kind]) {
					currentState = 255;
					break;
				} else {
					stateStack.Push(256);
					goto case 89;
				}
			}
			case 256: {
				if (t == null) { currentState = 256; break; }
				if (t.kind == 62) {
					goto case 237;
				} else {
					goto case 234;
				}
			}
			case 257: {
				if (t == null) { currentState = 257; break; }
				Expect(97, t); // "Custom"
				currentState = 258;
				break;
			}
			case 258: {
				stateStack.Push(259);
				goto case 269;
			}
			case 259: {
				if (t == null) { currentState = 259; break; }
				if (set[34, t.kind]) {
					goto case 261;
				} else {
					Expect(112, t); // "End"
					currentState = 260;
					break;
				}
			}
			case 260: {
				if (t == null) { currentState = 260; break; }
				Expect(118, t); // "Event"
				currentState = 15;
				break;
			}
			case 261: {
				if (t == null) { currentState = 261; break; }
				if (t.kind == 39) {
					stateStack.Push(261);
					goto case 247;
				} else {
					if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
						currentState = 262;
						break;
					} else {
						Error(t);
						goto case 262;
					}
				}
			}
			case 262: {
				if (t == null) { currentState = 262; break; }
				Expect(36, t); // "("
				currentState = 263;
				break;
			}
			case 263: {
				stateStack.Push(264);
				goto case 252;
			}
			case 264: {
				if (t == null) { currentState = 264; break; }
				Expect(37, t); // ")"
				currentState = 265;
				break;
			}
			case 265: {
				stateStack.Push(266);
				goto case 29;
			}
			case 266: {
				if (t == null) { currentState = 266; break; }
				Expect(112, t); // "End"
				currentState = 267;
				break;
			}
			case 267: {
				if (t == null) { currentState = 267; break; }
				if (t.kind == 55 || t.kind == 187 || t.kind == 191) {
					currentState = 268;
					break;
				} else {
					Error(t);
					goto case 268;
				}
			}
			case 268: {
				stateStack.Push(259);
				goto case 15;
			}
			case 269: {
				if (t == null) { currentState = 269; break; }
				Expect(118, t); // "Event"
				currentState = 270;
				break;
			}
			case 270: {
				stateStack.Push(271);
				goto case 89;
			}
			case 271: {
				if (t == null) { currentState = 271; break; }
				if (t.kind == 62) {
					currentState = 278;
					break;
				} else {
					if (set[35, t.kind]) {
						if (t.kind == 36) {
							currentState = 276;
							break;
						} else {
							goto case 272;
						}
					} else {
						Error(t);
						goto case 272;
					}
				}
			}
			case 272: {
				if (t == null) { currentState = 272; break; }
				if (t.kind == 135) {
					goto case 273;
				} else {
					goto case 15;
				}
			}
			case 273: {
				if (t == null) { currentState = 273; break; }
				currentState = 274;
				break;
			}
			case 274: {
				stateStack.Push(275);
				goto case 65;
			}
			case 275: {
				if (t == null) { currentState = 275; break; }
				if (t.kind == 23) {
					goto case 273;
				} else {
					goto case 15;
				}
			}
			case 276: {
				if (t == null) { currentState = 276; break; }
				if (set[36, t.kind]) {
					stateStack.Push(277);
					goto case 252;
				} else {
					goto case 277;
				}
			}
			case 277: {
				if (t == null) { currentState = 277; break; }
				Expect(37, t); // ")"
				currentState = 272;
				break;
			}
			case 278: {
				stateStack.Push(272);
				goto case 65;
			}
			case 279: {
				if (t == null) { currentState = 279; break; }
				Expect(100, t); // "Declare"
				currentState = 280;
				break;
			}
			case 280: {
				if (t == null) { currentState = 280; break; }
				if (t.kind == 61 || t.kind == 65 || t.kind == 221) {
					currentState = 281;
					break;
				} else {
					goto case 281;
				}
			}
			case 281: {
				if (t == null) { currentState = 281; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 282;
					break;
				} else {
					Error(t);
					goto case 282;
				}
			}
			case 282: {
				stateStack.Push(283);
				goto case 89;
			}
			case 283: {
				if (t == null) { currentState = 283; break; }
				Expect(148, t); // "Lib"
				currentState = 284;
				break;
			}
			case 284: {
				if (t == null) { currentState = 284; break; }
				Expect(3, t); // LiteralString
				currentState = 285;
				break;
			}
			case 285: {
				if (t == null) { currentState = 285; break; }
				if (t.kind == 58) {
					currentState = 289;
					break;
				} else {
					goto case 286;
				}
			}
			case 286: {
				if (t == null) { currentState = 286; break; }
				if (t.kind == 36) {
					currentState = 287;
					break;
				} else {
					goto case 15;
				}
			}
			case 287: {
				if (t == null) { currentState = 287; break; }
				if (set[36, t.kind]) {
					stateStack.Push(288);
					goto case 252;
				} else {
					goto case 288;
				}
			}
			case 288: {
				if (t == null) { currentState = 288; break; }
				Expect(37, t); // ")"
				currentState = 15;
				break;
			}
			case 289: {
				if (t == null) { currentState = 289; break; }
				Expect(3, t); // LiteralString
				currentState = 286;
				break;
			}
			case 290: {
				if (t == null) { currentState = 290; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 291;
					break;
				} else {
					Error(t);
					goto case 291;
				}
			}
			case 291: {
				PushContext(Context.IdentifierExpected, t);
				goto case 292;
			}
			case 292: {
				if (t == null) { currentState = 292; break; }
				currentState = 293;
				break;
			}
			case 293: {
				PopContext();
				goto case 294;
			}
			case 294: {
				if (t == null) { currentState = 294; break; }
				if (t.kind == 36) {
					currentState = 300;
					break;
				} else {
					goto case 295;
				}
			}
			case 295: {
				if (t == null) { currentState = 295; break; }
				if (t.kind == 62) {
					currentState = 299;
					break;
				} else {
					goto case 296;
				}
			}
			case 296: {
				stateStack.Push(297);
				goto case 29;
			}
			case 297: {
				if (t == null) { currentState = 297; break; }
				Expect(112, t); // "End"
				currentState = 298;
				break;
			}
			case 298: {
				if (t == null) { currentState = 298; break; }
				if (t.kind == 126 || t.kind == 208) {
					currentState = 15;
					break;
				} else {
					Error(t);
					goto case 15;
				}
			}
			case 299: {
				stateStack.Push(296);
				goto case 65;
			}
			case 300: {
				if (t == null) { currentState = 300; break; }
				if (set[36, t.kind]) {
					stateStack.Push(301);
					goto case 252;
				} else {
					goto case 301;
				}
			}
			case 301: {
				if (t == null) { currentState = 301; break; }
				Expect(37, t); // ")"
				currentState = 295;
				break;
			}
			case 302: {
				if (t == null) { currentState = 302; break; }
				if (t.kind == 87) {
					currentState = 303;
					break;
				} else {
					goto case 303;
				}
			}
			case 303: {
				stateStack.Push(304);
				goto case 308;
			}
			case 304: {
				if (t == null) { currentState = 304; break; }
				if (t.kind == 62) {
					currentState = 307;
					break;
				} else {
					goto case 305;
				}
			}
			case 305: {
				if (t == null) { currentState = 305; break; }
				if (t.kind == 21) {
					currentState = 306;
					break;
				} else {
					goto case 15;
				}
			}
			case 306: {
				stateStack.Push(15);
				goto case 35;
			}
			case 307: {
				stateStack.Push(305);
				goto case 65;
			}
			case 308: {
				if (t == null) { currentState = 308; break; }
				if (set[37, t.kind]) {
					goto case 16;
				} else {
					goto case 6;
				}
			}
			case 309: {
				if (t == null) { currentState = 309; break; }
				currentState = 9;
				break;
			}
			case 310: {
				if (t == null) { currentState = 310; break; }
				currentState = 311;
				break;
			}
			case 311: {
				if (t == null) { currentState = 311; break; }
				if (set[3, t.kind]) {
					goto case 310;
				} else {
					stateStack.Push(312);
					goto case 15;
				}
			}
			case 312: {
				if (t == null) { currentState = 312; break; }
				if (set[38, t.kind]) {
					stateStack.Push(312);
					goto case 5;
				} else {
					Expect(112, t); // "End"
					currentState = 313;
					break;
				}
			}
			case 313: {
				if (t == null) { currentState = 313; break; }
				Expect(159, t); // "Namespace"
				currentState = 15;
				break;
			}
			case 314: {
				if (t == null) { currentState = 314; break; }
				Expect(136, t); // "Imports"
				currentState = 315;
				break;
			}
			case 315: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 316;
			}
			case 316: {
				if (t == null) { currentState = 316; break; }
				if (set[3, t.kind]) {
					currentState = 316;
					break;
				} else {
					goto case 15;
				}
			}
			case 317: {
				if (t == null) { currentState = 317; break; }
				Expect(172, t); // "Option"
				currentState = 318;
				break;
			}
			case 318: {
				if (t == null) { currentState = 318; break; }
				if (set[3, t.kind]) {
					currentState = 318;
					break;
				} else {
					goto case 15;
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
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,x,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, x,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
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