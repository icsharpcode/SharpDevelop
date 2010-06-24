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

	void Expect(int expectedKind, Token la)
	{
		if (la.kind != expectedKind)
			Error(la);
	}
	
	void Error(Token la) 
	{
		output.AppendLine("not expected: " + la);
		Console.WriteLine("not expected: " + la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		nextTokenIsPotentialStartOfXmlMode = false;
		const int endOfStatementTerminatorAndBlock = 38;
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 172) {
					stateStack.Push(1);
					goto case 402;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 136) {
					stateStack.Push(2);
					goto case 399;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 39) {
					stateStack.Push(3);
					goto case 191;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (la == null) { currentState = 4; break; }
				if (set[0, la.kind]) {
					stateStack.Push(4);
					goto case 5;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 5: {
				if (la == null) { currentState = 5; break; }
				if (la.kind == 159) {
					goto case 395;
				} else {
					if (set[1, la.kind]) {
						goto case 7;
					} else {
						goto case 6;
					}
				}
			}
			case 6: {
				Error(la);
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 7: {
				if (la == null) { currentState = 7; break; }
				if (la.kind == 39) {
					stateStack.Push(7);
					goto case 191;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[2, la.kind]) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 83 || la.kind == 154) {
						currentState = 9;
						break;
					} else {
						Error(la);
						goto case 9;
					}
				}
			}
			case 9: {
				PushContext(Context.IdentifierExpected, t);
				goto case 10;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				currentState = 11;
				break;
			}
			case 11: {
				PopContext();
				goto case 12;
			}
			case 12: {
				if (la == null) { currentState = 12; break; }
				if (set[3, la.kind]) {
					currentState = 12;
					break;
				} else {
					stateStack.Push(13);
					goto case 18;
				}
			}
			case 13: {
				PushContext(Context.Type, t);
				goto case 14;
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				if (set[4, la.kind]) {
					stateStack.Push(14);
					PushContext(Context.Member, t);
					goto case 20;
				} else {
					Expect(112, la); // "End"
					currentState = 15;
					break;
				}
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (la.kind == 83 || la.kind == 154) {
					currentState = 16;
					break;
				} else {
					Error(la);
					goto case 16;
				}
			}
			case 16: {
				stateStack.Push(17);
				goto case 18;
			}
			case 17: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (la.kind == 1 || la.kind == 22) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				if (la.kind == 39) {
					stateStack.Push(20);
					goto case 191;
				} else {
					goto case 21;
				}
			}
			case 21: {
				if (la == null) { currentState = 21; break; }
				if (set[5, la.kind]) {
					currentState = 21;
					break;
				} else {
					if (set[6, la.kind]) {
						stateStack.Push(22);
						goto case 387;
					} else {
						if (la.kind == 126 || la.kind == 208) {
							stateStack.Push(22);
							goto case 375;
						} else {
							if (la.kind == 100) {
								stateStack.Push(22);
								goto case 363;
							} else {
								if (la.kind == 118) {
									stateStack.Push(22);
									goto case 352;
								} else {
									if (la.kind == 97) {
										stateStack.Push(22);
										goto case 340;
									} else {
										if (la.kind == 171) {
											stateStack.Push(22);
											goto case 23;
										} else {
											Error(la);
											goto case 22;
										}
									}
								}
							}
						}
					}
				}
			}
			case 22: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				Expect(171, la); // "Operator"
				currentState = 24;
				break;
			}
			case 24: {
				PushContext(Context.IdentifierExpected, t);
				goto case 25;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				currentState = 26;
				break;
			}
			case 26: {
				PopContext();
				goto case 27;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				Expect(36, la); // "("
				currentState = 28;
				break;
			}
			case 28: {
				stateStack.Push(29);
				goto case 182;
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				Expect(37, la); // ")"
				currentState = 30;
				break;
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (la.kind == 62) {
					currentState = 339;
					break;
				} else {
					goto case 31;
				}
			}
			case 31: {
				stateStack.Push(32);
				goto case 34;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				Expect(112, la); // "End"
				currentState = 33;
				break;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				Expect(171, la); // "Operator"
				currentState = 18;
				break;
			}
			case 34: {
				PushContext(Context.Body, t);
				goto case 35;
			}
			case 35: {
				stateStack.Push(36);
				goto case 18;
			}
			case 36: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 37;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (set[7, la.kind]) {
					if (set[8, la.kind]) {
						if (set[9, la.kind]) {
							stateStack.Push(35);
							goto case 42;
						} else {
							goto case 35;
						}
					} else {
						if (la.kind == 112) {
							currentState = 40;
							break;
						} else {
							goto case 39;
						}
					}
				} else {
					goto case 38;
				}
			}
			case 38: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 39: {
				Error(la);
				goto case 36;
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				if (la.kind == 1 || la.kind == 22) {
					goto case 41;
				} else {
					if (set[3, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 39;
					}
				}
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				currentState = 36;
				break;
			}
			case 42: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 43;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (la.kind == 87 || la.kind == 104 || la.kind == 202) {
					currentState = 324;
					break;
				} else {
					if (la.kind == 209 || la.kind == 231) {
						currentState = 320;
						break;
					} else {
						if (la.kind == 55 || la.kind == 191) {
							currentState = 318;
							break;
						} else {
							if (la.kind == 187) {
								currentState = 316;
								break;
							} else {
								if (la.kind == 134) {
									currentState = 295;
									break;
								} else {
									if (la.kind == 195) {
										currentState = 280;
										break;
									} else {
										if (la.kind == 229) {
											currentState = 276;
											break;
										} else {
											if (la.kind == 107) {
												currentState = 270;
												break;
											} else {
												if (la.kind == 123) {
													currentState = 245;
													break;
												} else {
													if (la.kind == 117 || la.kind == 170 || la.kind == 192) {
														if (la.kind == 117 || la.kind == 170) {
															if (la.kind == 170) {
																currentState = 240;
																break;
															} else {
																goto case 240;
															}
														} else {
															if (la.kind == 192) {
																currentState = 239;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 213) {
															goto case 222;
														} else {
															if (la.kind == 216) {
																currentState = 228;
																break;
															} else {
																if (set[10, la.kind]) {
																	if (la.kind == 131) {
																		currentState = 227;
																		break;
																	} else {
																		if (la.kind == 119) {
																			currentState = 226;
																			break;
																		} else {
																			if (la.kind == 88) {
																				currentState = 225;
																				break;
																			} else {
																				if (la.kind == 204) {
																					goto case 19;
																				} else {
																					if (la.kind == 193) {
																						goto case 222;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 189) {
																		currentState = 220;
																		break;
																	} else {
																		if (la.kind == 116) {
																			goto case 217;
																		} else {
																			if (la.kind == 224) {
																				currentState = 213;
																				break;
																			} else {
																				if (set[11, la.kind]) {
																					if (la.kind == 72) {
																						goto case 46;
																					} else {
																						goto case 44;
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
			case 44: {
				stateStack.Push(45);
				goto case 47;
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				if (set[12, la.kind]) {
					goto case 46;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				currentState = 44;
				break;
			}
			case 47: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 48;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (set[13, la.kind]) {
					currentState = 47;
					break;
				} else {
					if (set[14, la.kind]) {
						stateStack.Push(92);
						goto case 100;
					} else {
						if (la.kind == 218) {
							currentState = 90;
							break;
						} else {
							if (la.kind == 161) {
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
				if (la == null) { currentState = 49; break; }
				if (set[15, la.kind]) {
					stateStack.Push(59);
					goto case 66;
				} else {
					goto case 50;
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				if (la.kind == 231) {
					currentState = 51;
					break;
				} else {
					goto case 6;
				}
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				Expect(34, la); // "{"
				currentState = 52;
				break;
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (la.kind == 146) {
					currentState = 53;
					break;
				} else {
					goto case 53;
				}
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				Expect(27, la); // "."
				currentState = 54;
				break;
			}
			case 54: {
				stateStack.Push(55);
				goto case 19;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				Expect(21, la); // "="
				currentState = 56;
				break;
			}
			case 56: {
				stateStack.Push(57);
				goto case 44;
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				if (la.kind == 23) {
					currentState = 52;
					break;
				} else {
					goto case 58;
				}
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				Expect(35, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				if (la.kind == 125 || la.kind == 231) {
					if (la.kind == 125) {
						currentState = 60;
						break;
					} else {
						goto case 50;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (la.kind == 34) {
					goto case 61;
				} else {
					if (set[16, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 6;
					}
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				currentState = 62;
				break;
			}
			case 62: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 63;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (set[17, la.kind]) {
					stateStack.Push(64);
					goto case 44;
				} else {
					if (la.kind == 34) {
						stateStack.Push(64);
						goto case 65;
					} else {
						Error(la);
						goto case 64;
					}
				}
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				if (la.kind == 23) {
					goto case 61;
				} else {
					goto case 58;
				}
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				Expect(34, la); // "{"
				currentState = 62;
				break;
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				if (set[15, la.kind]) {
					currentState = 67;
					break;
				} else {
					Error(la);
					goto case 67;
				}
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				if (la.kind == 36) {
					stateStack.Push(67);
					goto case 71;
				} else {
					goto case 68;
				}
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				if (la.kind == 27) {
					currentState = 69;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 69: {
				stateStack.Push(70);
				goto case 19;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.kind == 36) {
					stateStack.Push(70);
					goto case 71;
				} else {
					goto case 68;
				}
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				Expect(36, la); // "("
				currentState = 72;
				break;
			}
			case 72: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 73;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				if (la.kind == 168) {
					goto case 87;
				} else {
					if (set[18, la.kind]) {
						goto case 75;
					} else {
						Error(la);
						goto case 74;
					}
				}
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				Expect(37, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 75: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 76;
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				if (set[19, la.kind]) {
					goto case 77;
				} else {
					goto case 74;
				}
			}
			case 77: {
				stateStack.Push(74);
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 78;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				if (set[17, la.kind]) {
					goto case 83;
				} else {
					if (la.kind == 23) {
						goto case 79;
					} else {
						goto case 6;
					}
				}
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				currentState = 80;
				break;
			}
			case 80: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 81;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (set[17, la.kind]) {
					stateStack.Push(82);
					goto case 44;
				} else {
					goto case 82;
				}
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				if (la.kind == 23) {
					goto case 79;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 83: {
				stateStack.Push(84);
				goto case 44;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				if (la.kind == 23) {
					currentState = 85;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 85: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 86;
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				if (set[17, la.kind]) {
					goto case 83;
				} else {
					goto case 84;
				}
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				currentState = 88;
				break;
			}
			case 88: {
				if (la == null) { currentState = 88; break; }
				if (set[15, la.kind]) {
					stateStack.Push(89);
					goto case 66;
				} else {
					goto case 89;
				}
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				if (la.kind == 23) {
					goto case 87;
				} else {
					goto case 74;
				}
			}
			case 90: {
				stateStack.Push(91);
				goto case 47;
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				Expect(143, la); // "Is"
				currentState = 66;
				break;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				if (la.kind == 27 || la.kind == 28 || la.kind == 36) {
					stateStack.Push(92);
					goto case 93;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				if (la.kind == 36) {
					currentState = 95;
					break;
				} else {
					if (la.kind == 27 || la.kind == 28) {
						goto case 94;
					} else {
						goto case 6;
					}
				}
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				currentState = 19;
				break;
			}
			case 95: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 96;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				if (la.kind == 168) {
					goto case 97;
				} else {
					if (set[19, la.kind]) {
						goto case 77;
					} else {
						goto case 6;
					}
				}
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				currentState = 98;
				break;
			}
			case 98: {
				stateStack.Push(99);
				goto case 66;
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				if (la.kind == 23) {
					goto case 97;
				} else {
					goto case 74;
				}
			}
			case 100: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 101;
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				if (set[20, la.kind]) {
					goto case 19;
				} else {
					if (la.kind == 36) {
						goto case 107;
					} else {
						if (set[21, la.kind]) {
							goto case 19;
						} else {
							if (la.kind == 27 || la.kind == 28) {
								goto case 94;
							} else {
								if (la.kind == 128) {
									currentState = 212;
									break;
								} else {
									if (la.kind == 235) {
										currentState = 210;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17) {
											nextTokenIsPotentialStartOfXmlMode = true;
											PushContext(Context.Xml, t);
											goto case 203;
										} else {
											if (la.kind == 126 || la.kind == 208) {
												if (la.kind == 208) {
													currentState = 196;
													break;
												} else {
													if (la.kind == 126) {
														currentState = 173;
														break;
													} else {
														goto case 6;
													}
												}
											} else {
												if (la.kind == 57 || la.kind == 125) {
													if (la.kind == 125) {
														stateStack.Push(114);
														goto case 172;
													} else {
														if (la.kind == 57) {
															stateStack.Push(114);
															goto case 171;
														} else {
															Error(la);
															goto case 114;
														}
													}
												} else {
													if (set[22, la.kind]) {
														if (set[23, la.kind]) {
															currentState = 113;
															break;
														} else {
															if (la.kind == 93 || la.kind == 105 || la.kind == 217) {
																currentState = 109;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 134) {
															currentState = 102;
															break;
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
			case 102: {
				if (la == null) { currentState = 102; break; }
				Expect(36, la); // "("
				currentState = 103;
				break;
			}
			case 103: {
				stateStack.Push(104);
				goto case 44;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				Expect(23, la); // ","
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(106);
				goto case 44;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (la.kind == 23) {
					goto case 107;
				} else {
					goto case 74;
				}
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				currentState = 108;
				break;
			}
			case 108: {
				stateStack.Push(74);
				goto case 44;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				Expect(36, la); // "("
				currentState = 110;
				break;
			}
			case 110: {
				stateStack.Push(111);
				goto case 44;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				Expect(23, la); // ","
				currentState = 112;
				break;
			}
			case 112: {
				stateStack.Push(74);
				goto case 66;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				Expect(36, la); // "("
				currentState = 108;
				break;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				if (set[24, la.kind]) {
					stateStack.Push(114);
					goto case 115;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				if (la.kind == 125) {
					goto case 168;
				} else {
					if (la.kind == 57) {
						currentState = 164;
						break;
					} else {
						if (la.kind == 195) {
							goto case 161;
						} else {
							if (la.kind == 106) {
								goto case 19;
							} else {
								if (la.kind == 228) {
									goto case 46;
								} else {
									if (la.kind == 175) {
										currentState = 157;
										break;
									} else {
										if (la.kind == 201 || la.kind == 210) {
											currentState = 155;
											break;
										} else {
											if (la.kind == 147) {
												goto case 152;
											} else {
												if (la.kind == 132) {
													currentState = 128;
													break;
												} else {
													if (la.kind == 145) {
														currentState = 116;
														break;
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
			case 116: {
				stateStack.Push(117);
				goto case 122;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				Expect(170, la); // "On"
				currentState = 118;
				break;
			}
			case 118: {
				stateStack.Push(119);
				goto case 44;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				Expect(115, la); // "Equals"
				currentState = 120;
				break;
			}
			case 120: {
				stateStack.Push(121);
				goto case 44;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				if (la.kind == 23) {
					currentState = 118;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 122: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(123);
				goto case 127;
			}
			case 123: {
				PopContext();
				goto case 124;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				if (la.kind == 62) {
					currentState = 126;
					break;
				} else {
					goto case 125;
				}
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				Expect(137, la); // "In"
				currentState = 44;
				break;
			}
			case 126: {
				stateStack.Push(125);
				goto case 66;
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (set[25, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 128: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 129;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				if (la.kind == 145) {
					goto case 144;
				} else {
					if (set[26, la.kind]) {
						if (la.kind == 69) {
							goto case 141;
						} else {
							if (set[26, la.kind]) {
								goto case 142;
							} else {
								Error(la);
								goto case 130;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				Expect(69, la); // "By"
				currentState = 131;
				break;
			}
			case 131: {
				stateStack.Push(132);
				goto case 135;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 23) {
					goto case 141;
				} else {
					Expect(142, la); // "Into"
					currentState = 133;
					break;
				}
			}
			case 133: {
				stateStack.Push(134);
				goto case 135;
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				if (la.kind == 23) {
					currentState = 133;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 135: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 136;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (set[25, la.kind]) {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(137);
					goto case 127;
				} else {
					goto case 44;
				}
			}
			case 137: {
				PopContext();
				goto case 138;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (la.kind == 62) {
					currentState = 139;
					break;
				} else {
					if (la.kind == 21) {
						goto case 46;
					} else {
						if (set[27, la.kind]) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 44;
						}
					}
				}
			}
			case 139: {
				stateStack.Push(140);
				goto case 66;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				Expect(21, la); // "="
				currentState = 44;
				break;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				currentState = 131;
				break;
			}
			case 142: {
				stateStack.Push(143);
				goto case 135;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				if (la.kind == 23) {
					currentState = 142;
					break;
				} else {
					goto case 130;
				}
			}
			case 144: {
				stateStack.Push(145);
				goto case 151;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				if (la.kind == 132 || la.kind == 145) {
					if (la.kind == 132) {
						currentState = 149;
						break;
					} else {
						if (la.kind == 145) {
							goto case 144;
						} else {
							Error(la);
							goto case 145;
						}
					}
				} else {
					goto case 146;
				}
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				Expect(142, la); // "Into"
				currentState = 147;
				break;
			}
			case 147: {
				stateStack.Push(148);
				goto case 135;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				if (la.kind == 23) {
					currentState = 147;
					break;
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
				stateStack.Push(145);
				goto case 146;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(145, la); // "Join"
				currentState = 116;
				break;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				currentState = 153;
				break;
			}
			case 153: {
				stateStack.Push(154);
				goto case 135;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (la.kind == 23) {
					goto case 152;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 155: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 156;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (la.kind == 229) {
					goto case 46;
				} else {
					goto case 44;
				}
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				Expect(69, la); // "By"
				currentState = 158;
				break;
			}
			case 158: {
				stateStack.Push(159);
				goto case 44;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (la.kind == 63 || la.kind == 103) {
					currentState = 160;
					break;
				} else {
					Error(la);
					goto case 160;
				}
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (la.kind == 23) {
					currentState = 158;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				currentState = 162;
				break;
			}
			case 162: {
				stateStack.Push(163);
				goto case 135;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.kind == 23) {
					goto case 161;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 164: {
				stateStack.Push(165);
				goto case 122;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (set[24, la.kind]) {
					stateStack.Push(165);
					goto case 115;
				} else {
					Expect(142, la); // "Into"
					currentState = 166;
					break;
				}
			}
			case 166: {
				stateStack.Push(167);
				goto case 135;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (la.kind == 23) {
					currentState = 166;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				currentState = 169;
				break;
			}
			case 169: {
				stateStack.Push(170);
				goto case 122;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (la.kind == 23) {
					goto case 168;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				Expect(57, la); // "Aggregate"
				currentState = 164;
				break;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				Expect(125, la); // "From"
				currentState = 169;
				break;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				Expect(36, la); // "("
				currentState = 174;
				break;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (set[28, la.kind]) {
					stateStack.Push(175);
					goto case 182;
				} else {
					goto case 175;
				}
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				Expect(37, la); // ")"
				currentState = 176;
				break;
			}
			case 176: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 177;
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				if (set[17, la.kind]) {
					goto case 44;
				} else {
					if (la.kind == 1 || la.kind == 22 || la.kind == 62) {
						if (la.kind == 62) {
							currentState = 181;
							break;
						} else {
							goto case 178;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 178: {
				stateStack.Push(179);
				goto case 34;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				Expect(112, la); // "End"
				currentState = 180;
				break;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				Expect(126, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 181: {
				stateStack.Push(178);
				goto case 66;
			}
			case 182: {
				stateStack.Push(183);
				goto case 184;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (la.kind == 23) {
					currentState = 182;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				if (la.kind == 39) {
					stateStack.Push(184);
					goto case 191;
				} else {
					goto case 185;
				}
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (set[29, la.kind]) {
					currentState = 185;
					break;
				} else {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(186);
					goto case 127;
				}
			}
			case 186: {
				PopContext();
				goto case 187;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.kind == 62) {
					goto case 189;
				} else {
					goto case 188;
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 21) {
					goto case 46;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				currentState = 190;
				break;
			}
			case 190: {
				stateStack.Push(188);
				goto case 66;
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				Expect(39, la); // "<"
				currentState = 192;
				break;
			}
			case 192: {
				PushContext(Context.Attribute, t);
				goto case 193;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (set[30, la.kind]) {
					currentState = 193;
					break;
				} else {
					Expect(38, la); // ">"
					currentState = 194;
					break;
				}
			}
			case 194: {
				PopContext();
				goto case 195;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (la.kind == 1) {
					goto case 19;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				Expect(36, la); // "("
				currentState = 197;
				break;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (set[28, la.kind]) {
					stateStack.Push(198);
					goto case 182;
				} else {
					goto case 198;
				}
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(37, la); // ")"
				currentState = 199;
				break;
			}
			case 199: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 200;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (set[9, la.kind]) {
					goto case 42;
				} else {
					if (la.kind == 1 || la.kind == 22) {
						stateStack.Push(201);
						goto case 34;
					} else {
						goto case 6;
					}
				}
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				Expect(112, la); // "End"
				currentState = 202;
				break;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				Expect(208, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.kind == 17) {
					currentState = 203;
					break;
				} else {
					stateStack.Push(204);
					goto case 205;
				}
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				if (la.kind == 17) {
					currentState = 204;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 206;
				break;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				if (set[31, la.kind]) {
					currentState = 206;
					break;
				} else {
					if (la.kind == 14) {
						goto case 19;
					} else {
						if (la.kind == 11) {
							goto case 207;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				currentState = 208;
				break;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (set[32, la.kind]) {
					if (set[33, la.kind]) {
						goto case 207;
					} else {
						if (la.kind == 10) {
							stateStack.Push(208);
							goto case 205;
						} else {
							Error(la);
							goto case 208;
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 209;
					break;
				}
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (set[34, la.kind]) {
					currentState = 209;
					break;
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				Expect(36, la); // "("
				currentState = 211;
				break;
			}
			case 211: {
				readXmlIdentifier = true;
				stateStack.Push(74);
				goto case 127;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				Expect(36, la); // "("
				currentState = 112;
				break;
			}
			case 213: {
				stateStack.Push(214);
				goto case 44;
			}
			case 214: {
				stateStack.Push(215);
				goto case 34;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				Expect(112, la); // "End"
				currentState = 216;
				break;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(224, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				currentState = 218;
				break;
			}
			case 218: {
				stateStack.Push(219);
				goto case 44;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 23) {
					goto case 217;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 220: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 221;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 182) {
					goto case 46;
				} else {
					goto case 44;
				}
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				currentState = 223;
				break;
			}
			case 223: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 224;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				if (set[17, la.kind]) {
					goto case 44;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.kind == 107 || la.kind == 123 || la.kind == 229) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (set[35, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (set[36, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 228: {
				stateStack.Push(229);
				goto case 34;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (la.kind == 74) {
					currentState = 233;
					break;
				} else {
					if (la.kind == 122) {
						currentState = 232;
						break;
					} else {
						goto case 230;
					}
				}
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				Expect(112, la); // "End"
				currentState = 231;
				break;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				Expect(216, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 232: {
				stateStack.Push(230);
				goto case 34;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				if (set[25, la.kind]) {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(236);
					goto case 127;
				} else {
					goto case 234;
				}
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				if (la.kind == 227) {
					currentState = 235;
					break;
				} else {
					goto case 228;
				}
			}
			case 235: {
				stateStack.Push(228);
				goto case 44;
			}
			case 236: {
				PopContext();
				goto case 237;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (la.kind == 62) {
					currentState = 238;
					break;
				} else {
					goto case 234;
				}
			}
			case 238: {
				stateStack.Push(234);
				goto case 66;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (set[37, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				Expect(117, la); // "Error"
				currentState = 241;
				break;
			}
			case 241: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 242;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (set[17, la.kind]) {
					goto case 44;
				} else {
					if (la.kind == 131) {
						currentState = 244;
						break;
					} else {
						if (la.kind == 192) {
							currentState = 243;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				Expect(162, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (set[36, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 245: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 246;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (set[14, la.kind]) {
					stateStack.Push(260);
					goto case 256;
				} else {
					if (la.kind == 109) {
						currentState = 247;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 247: {
				stateStack.Push(248);
				goto case 256;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				Expect(137, la); // "In"
				currentState = 249;
				break;
			}
			case 249: {
				stateStack.Push(250);
				goto case 44;
			}
			case 250: {
				stateStack.Push(251);
				goto case 34;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				Expect(162, la); // "Next"
				currentState = 252;
				break;
			}
			case 252: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 253;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (set[17, la.kind]) {
					goto case 254;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 254: {
				stateStack.Push(255);
				goto case 44;
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 23) {
					currentState = 254;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 256: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(257);
				goto case 100;
			}
			case 257: {
				PopContext();
				goto case 258;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (la.kind == 32) {
					currentState = 259;
					break;
				} else {
					goto case 259;
				}
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (la.kind == 27 || la.kind == 28 || la.kind == 36) {
					stateStack.Push(259);
					goto case 93;
				} else {
					if (la.kind == 62) {
						currentState = 66;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				Expect(21, la); // "="
				currentState = 261;
				break;
			}
			case 261: {
				stateStack.Push(262);
				goto case 44;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (la.kind == 203) {
					currentState = 269;
					break;
				} else {
					goto case 263;
				}
			}
			case 263: {
				stateStack.Push(264);
				goto case 34;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				Expect(162, la); // "Next"
				currentState = 265;
				break;
			}
			case 265: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 266;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (set[17, la.kind]) {
					goto case 267;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 267: {
				stateStack.Push(268);
				goto case 44;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (la.kind == 23) {
					currentState = 267;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 269: {
				stateStack.Push(263);
				goto case 44;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.kind == 222 || la.kind == 229) {
					currentState = 273;
					break;
				} else {
					if (la.kind == 1 || la.kind == 22) {
						stateStack.Push(271);
						goto case 34;
					} else {
						goto case 6;
					}
				}
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				Expect(151, la); // "Loop"
				currentState = 272;
				break;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				if (la.kind == 222 || la.kind == 229) {
					goto case 46;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 273: {
				stateStack.Push(274);
				goto case 44;
			}
			case 274: {
				stateStack.Push(275);
				goto case 34;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(151, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 276: {
				stateStack.Push(277);
				goto case 44;
			}
			case 277: {
				stateStack.Push(278);
				goto case 34;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				Expect(112, la); // "End"
				currentState = 279;
				break;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				Expect(229, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 280: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 281;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 73) {
					currentState = 282;
					break;
				} else {
					goto case 282;
				}
			}
			case 282: {
				stateStack.Push(283);
				goto case 44;
			}
			case 283: {
				stateStack.Push(284);
				goto case 18;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 73) {
					currentState = 286;
					break;
				} else {
					Expect(112, la); // "End"
					currentState = 285;
					break;
				}
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				Expect(195, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 286: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 287;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 110) {
					currentState = 288;
					break;
				} else {
					if (set[38, la.kind]) {
						goto case 289;
					} else {
						Error(la);
						goto case 288;
					}
				}
			}
			case 288: {
				stateStack.Push(284);
				goto case 34;
			}
			case 289: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 290;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				if (set[39, la.kind]) {
					if (la.kind == 143) {
						currentState = 292;
						break;
					} else {
						goto case 292;
					}
				} else {
					if (set[17, la.kind]) {
						stateStack.Push(291);
						goto case 44;
					} else {
						Error(la);
						goto case 291;
					}
				}
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (la.kind == 23) {
					currentState = 289;
					break;
				} else {
					goto case 288;
				}
			}
			case 292: {
				stateStack.Push(293);
				goto case 294;
			}
			case 293: {
				stateStack.Push(291);
				goto case 47;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (set[40, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 295: {
				stateStack.Push(296);
				goto case 44;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (la.kind == 212) {
					currentState = 305;
					break;
				} else {
					goto case 297;
				}
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (la.kind == 1 || la.kind == 22) {
					goto case 298;
				} else {
					goto case 6;
				}
			}
			case 298: {
				stateStack.Push(299);
				goto case 34;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 110 || la.kind == 111) {
					if (la.kind == 110) {
						currentState = 304;
						break;
					} else {
						if (la.kind == 111) {
							goto case 301;
						} else {
							Error(la);
							goto case 298;
						}
					}
				} else {
					Expect(112, la); // "End"
					currentState = 300;
					break;
				}
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				Expect(134, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				currentState = 302;
				break;
			}
			case 302: {
				stateStack.Push(303);
				goto case 44;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (la.kind == 212) {
					currentState = 298;
					break;
				} else {
					goto case 298;
				}
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				if (la.kind == 134) {
					goto case 301;
				} else {
					goto case 298;
				}
			}
			case 305: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 306;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (set[9, la.kind]) {
					goto case 307;
				} else {
					goto case 297;
				}
			}
			case 307: {
				stateStack.Push(308);
				goto case 42;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (la.kind == 22) {
					currentState = 314;
					break;
				} else {
					if (la.kind == 110) {
						goto case 310;
					} else {
						goto case 309;
					}
				}
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				currentState = 311;
				break;
			}
			case 311: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 312;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (set[9, la.kind]) {
					stateStack.Push(313);
					goto case 42;
				} else {
					goto case 313;
				}
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				if (la.kind == 22) {
					goto case 310;
				} else {
					goto case 309;
				}
			}
			case 314: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 315;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (set[9, la.kind]) {
					goto case 307;
				} else {
					goto case 308;
				}
			}
			case 316: {
				stateStack.Push(317);
				goto case 19;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 36) {
					currentState = 75;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 318: {
				stateStack.Push(319);
				goto case 44;
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				Expect(23, la); // ","
				currentState = 44;
				break;
			}
			case 320: {
				stateStack.Push(321);
				goto case 44;
			}
			case 321: {
				stateStack.Push(322);
				goto case 34;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				Expect(112, la); // "End"
				currentState = 323;
				break;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 209 || la.kind == 231) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 324: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(325);
				goto case 127;
			}
			case 325: {
				PopContext();
				goto case 326;
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				if (la.kind == 32) {
					currentState = 327;
					break;
				} else {
					goto case 327;
				}
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 36) {
					goto case 337;
				} else {
					goto case 328;
				}
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 23) {
					currentState = 330;
					break;
				} else {
					if (la.kind == 62) {
						currentState = 329;
						break;
					} else {
						goto case 188;
					}
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 161) {
					goto case 189;
				} else {
					goto case 190;
				}
			}
			case 330: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(331);
				goto case 127;
			}
			case 331: {
				PopContext();
				goto case 332;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 32) {
					currentState = 333;
					break;
				} else {
					goto case 333;
				}
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 36) {
					goto case 334;
				} else {
					goto case 328;
				}
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				currentState = 335;
				break;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 23) {
					goto case 334;
				} else {
					goto case 336;
				}
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				Expect(37, la); // ")"
				currentState = 328;
				break;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				currentState = 338;
				break;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 23) {
					goto case 337;
				} else {
					goto case 336;
				}
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.kind == 39) {
					stateStack.Push(339);
					goto case 191;
				} else {
					stateStack.Push(31);
					goto case 66;
				}
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				Expect(97, la); // "Custom"
				currentState = 341;
				break;
			}
			case 341: {
				stateStack.Push(342);
				goto case 352;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (set[41, la.kind]) {
					goto case 344;
				} else {
					Expect(112, la); // "End"
					currentState = 343;
					break;
				}
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				Expect(118, la); // "Event"
				currentState = 18;
				break;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (la.kind == 39) {
					stateStack.Push(344);
					goto case 191;
				} else {
					if (la.kind == 55 || la.kind == 187 || la.kind == 191) {
						currentState = 345;
						break;
					} else {
						Error(la);
						goto case 345;
					}
				}
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				Expect(36, la); // "("
				currentState = 346;
				break;
			}
			case 346: {
				stateStack.Push(347);
				goto case 182;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				Expect(37, la); // ")"
				currentState = 348;
				break;
			}
			case 348: {
				stateStack.Push(349);
				goto case 34;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				Expect(112, la); // "End"
				currentState = 350;
				break;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 55 || la.kind == 187 || la.kind == 191) {
					currentState = 351;
					break;
				} else {
					Error(la);
					goto case 351;
				}
			}
			case 351: {
				stateStack.Push(342);
				goto case 18;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				Expect(118, la); // "Event"
				currentState = 353;
				break;
			}
			case 353: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(354);
				goto case 127;
			}
			case 354: {
				PopContext();
				goto case 355;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 62) {
					currentState = 362;
					break;
				} else {
					if (set[42, la.kind]) {
						if (la.kind == 36) {
							currentState = 360;
							break;
						} else {
							goto case 356;
						}
					} else {
						Error(la);
						goto case 356;
					}
				}
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 135) {
					goto case 357;
				} else {
					goto case 18;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				currentState = 358;
				break;
			}
			case 358: {
				stateStack.Push(359);
				goto case 66;
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (la.kind == 23) {
					goto case 357;
				} else {
					goto case 18;
				}
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (set[28, la.kind]) {
					stateStack.Push(361);
					goto case 182;
				} else {
					goto case 361;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				Expect(37, la); // ")"
				currentState = 356;
				break;
			}
			case 362: {
				stateStack.Push(356);
				goto case 66;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				Expect(100, la); // "Declare"
				currentState = 364;
				break;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 61 || la.kind == 65 || la.kind == 221) {
					currentState = 365;
					break;
				} else {
					goto case 365;
				}
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 126 || la.kind == 208) {
					currentState = 366;
					break;
				} else {
					Error(la);
					goto case 366;
				}
			}
			case 366: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(367);
				goto case 127;
			}
			case 367: {
				PopContext();
				goto case 368;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				Expect(148, la); // "Lib"
				currentState = 369;
				break;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				Expect(3, la); // LiteralString
				currentState = 370;
				break;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 58) {
					currentState = 374;
					break;
				} else {
					goto case 371;
				}
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (la.kind == 36) {
					currentState = 372;
					break;
				} else {
					goto case 18;
				}
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (set[28, la.kind]) {
					stateStack.Push(373);
					goto case 182;
				} else {
					goto case 373;
				}
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				Expect(37, la); // ")"
				currentState = 18;
				break;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				Expect(3, la); // LiteralString
				currentState = 371;
				break;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 126 || la.kind == 208) {
					currentState = 376;
					break;
				} else {
					Error(la);
					goto case 376;
				}
			}
			case 376: {
				PushContext(Context.IdentifierExpected, t);
				goto case 377;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				currentState = 378;
				break;
			}
			case 378: {
				PopContext();
				goto case 379;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 36) {
					currentState = 385;
					break;
				} else {
					goto case 380;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 62) {
					currentState = 384;
					break;
				} else {
					goto case 381;
				}
			}
			case 381: {
				stateStack.Push(382);
				goto case 34;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				Expect(112, la); // "End"
				currentState = 383;
				break;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (la.kind == 126 || la.kind == 208) {
					currentState = 18;
					break;
				} else {
					Error(la);
					goto case 18;
				}
			}
			case 384: {
				stateStack.Push(381);
				goto case 66;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (set[28, la.kind]) {
					stateStack.Push(386);
					goto case 182;
				} else {
					goto case 386;
				}
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				Expect(37, la); // ")"
				currentState = 380;
				break;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 87) {
					currentState = 388;
					break;
				} else {
					goto case 388;
				}
			}
			case 388: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(389);
				goto case 394;
			}
			case 389: {
				PopContext();
				goto case 390;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (la.kind == 62) {
					currentState = 393;
					break;
				} else {
					goto case 391;
				}
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (la.kind == 21) {
					currentState = 392;
					break;
				} else {
					goto case 18;
				}
			}
			case 392: {
				stateStack.Push(18);
				goto case 44;
			}
			case 393: {
				stateStack.Push(391);
				goto case 66;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (set[43, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				currentState = 396;
				break;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (set[3, la.kind]) {
					goto case 395;
				} else {
					stateStack.Push(397);
					goto case 18;
				}
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (set[44, la.kind]) {
					stateStack.Push(397);
					goto case 5;
				} else {
					Expect(112, la); // "End"
					currentState = 398;
					break;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				Expect(159, la); // "Namespace"
				currentState = 18;
				break;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				Expect(136, la); // "Imports"
				currentState = 400;
				break;
			}
			case 400: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 401;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (set[3, la.kind]) {
					currentState = 401;
					break;
				} else {
					goto case 18;
				}
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				Expect(172, la); // "Option"
				currentState = 403;
				break;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (set[3, la.kind]) {
					currentState = 403;
					break;
				} else {
					goto case 18;
				}
			}
		}

		ApplyToken(la);
		if (la != null) t = la;
	}
	
	public void Advance()
	{
		Console.WriteLine("Advance");
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
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, T,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, T,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, x,x,x,T, T,T,x,T, T,T,x,T, x,T,T,x, T,x,x,T, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,T, x,T,x,T, T,T,x,T, x,x,x,x, x,T,T,x, T,x,x,x, T,T,T,T, x,T,x,T, T,T,T,x, x,T,T,x, T,x,x,x, T,T,x,T, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, T,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,T,T,T, x,T,x,x, x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,x,T,x, T,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,T,x,x, T,x,x,x, x,x,T,x, T,x,T,x, x,T,T,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,T,x,x, x,x,T,x, x,x,T,T, x,x,x,x, x,x,x,T, T,T,T,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,x,x, x,T,T,T, T,x,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,T,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, T,T,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,T, T,T,T,x, x,x,x,x, T,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,T, T,T,T,T, T,x,T,x, T,T,T,x, x,T,T,T, T,T,T,T, T,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,T,x,x, x,T,T,x, T,x,x,x, T,x,T,x, x,x,T,x, x,x,T,T, x,T,T,x, x,x,x,x, T,x,x,x, x,T,T,x, x,T,x,T, T,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, T,x,T,T, x,x,x,T, x,T,T,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,T, T,T,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,T,x, x,x,T,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x}

	};

} // end Parser


}