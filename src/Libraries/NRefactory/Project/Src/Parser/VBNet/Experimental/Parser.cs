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
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	
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
		readXmlIdentifier = false;
		nextTokenIsStartOfImportsOrAccessExpression = false;
		const int endOfStatementTerminatorAndBlock = 38;
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 174) {
					stateStack.Push(1);
					goto case 417;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 138) {
					stateStack.Push(2);
					goto case 414;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 41) {
					stateStack.Push(3);
					goto case 196;
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
				if (la.kind == 161) {
					goto case 410;
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
				if (la.kind == 41) {
					stateStack.Push(7);
					goto case 196;
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
					if (la.kind == 85 || la.kind == 156) {
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
					Expect(114, la); // "End"
					currentState = 15;
					break;
				}
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (la.kind == 85 || la.kind == 156) {
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
				if (la.kind == 41) {
					stateStack.Push(20);
					goto case 196;
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
						goto case 402;
					} else {
						if (la.kind == 128 || la.kind == 210) {
							stateStack.Push(22);
							goto case 390;
						} else {
							if (la.kind == 102) {
								stateStack.Push(22);
								goto case 378;
							} else {
								if (la.kind == 120) {
									stateStack.Push(22);
									goto case 367;
								} else {
									if (la.kind == 99) {
										stateStack.Push(22);
										goto case 355;
									} else {
										if (la.kind == 173) {
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
				Expect(173, la); // "Operator"
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
				Expect(38, la); // "("
				currentState = 28;
				break;
			}
			case 28: {
				stateStack.Push(29);
				goto case 187;
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				Expect(39, la); // ")"
				currentState = 30;
				break;
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (la.kind == 64) {
					currentState = 354;
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
				Expect(114, la); // "End"
				currentState = 33;
				break;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				Expect(173, la); // "Operator"
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
						if (la.kind == 114) {
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
				if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 43;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (la.kind == 89 || la.kind == 106 || la.kind == 204) {
					currentState = 339;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 335;
						break;
					} else {
						if (la.kind == 57 || la.kind == 193) {
							currentState = 333;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 331;
								break;
							} else {
								if (la.kind == 136) {
									currentState = 310;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 295;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 291;
											break;
										} else {
											if (la.kind == 109) {
												currentState = 285;
												break;
											} else {
												if (la.kind == 125) {
													currentState = 260;
													break;
												} else {
													if (la.kind == 119 || la.kind == 172 || la.kind == 194) {
														if (la.kind == 119 || la.kind == 172) {
															if (la.kind == 172) {
																currentState = 255;
																break;
															} else {
																goto case 255;
															}
														} else {
															if (la.kind == 194) {
																currentState = 254;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															goto case 237;
														} else {
															if (la.kind == 218) {
																currentState = 243;
																break;
															} else {
																if (set[10, la.kind]) {
																	if (la.kind == 133) {
																		currentState = 242;
																		break;
																	} else {
																		if (la.kind == 121) {
																			currentState = 241;
																			break;
																		} else {
																			if (la.kind == 90) {
																				currentState = 240;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 19;
																				} else {
																					if (la.kind == 195) {
																						goto case 237;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 235;
																		break;
																	} else {
																		if (la.kind == 118) {
																			goto case 232;
																		} else {
																			if (la.kind == 226) {
																				currentState = 228;
																				break;
																			} else {
																				if (set[11, la.kind]) {
																					if (la.kind == 74) {
																						goto case 143;
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
				if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 45;
			}
			case 45: {
				stateStack.Push(46);
				goto case 47;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				if (set[12, la.kind]) {
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
				if (la == null) { currentState = 48; break; }
				if (set[13, la.kind]) {
					currentState = 47;
					break;
				} else {
					if (set[14, la.kind]) {
						stateStack.Push(93);
						goto case 104;
					} else {
						if (la.kind == 220) {
							currentState = 91;
							break;
						} else {
							if (la.kind == 163) {
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
					stateStack.Push(60);
					goto case 67;
				} else {
					goto case 50;
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				if (la.kind == 233) {
					currentState = 51;
					break;
				} else {
					goto case 6;
				}
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				Expect(36, la); // "{"
				currentState = 52;
				break;
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (la.kind == 148) {
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
				goto case 59;
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
				Expect(37, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				if (set[16, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (la.kind == 127 || la.kind == 233) {
					if (la.kind == 127) {
						currentState = 61;
						break;
					} else {
						goto case 50;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (la.kind == 36) {
					goto case 62;
				} else {
					if (set[17, la.kind]) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 6;
					}
				}
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				currentState = 63;
				break;
			}
			case 63: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 64;
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				if (set[18, la.kind]) {
					stateStack.Push(65);
					goto case 44;
				} else {
					if (la.kind == 36) {
						stateStack.Push(65);
						goto case 66;
					} else {
						Error(la);
						goto case 65;
					}
				}
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 23) {
					goto case 62;
				} else {
					goto case 58;
				}
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				Expect(36, la); // "{"
				currentState = 63;
				break;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				if (set[15, la.kind]) {
					currentState = 68;
					break;
				} else {
					Error(la);
					goto case 68;
				}
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				if (la.kind == 38) {
					stateStack.Push(68);
					goto case 72;
				} else {
					goto case 69;
				}
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				if (la.kind == 27) {
					currentState = 70;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 70: {
				stateStack.Push(71);
				goto case 59;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				if (la.kind == 38) {
					stateStack.Push(71);
					goto case 72;
				} else {
					goto case 69;
				}
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				Expect(38, la); // "("
				currentState = 73;
				break;
			}
			case 73: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 74;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				if (la.kind == 170) {
					goto case 88;
				} else {
					if (set[19, la.kind]) {
						goto case 76;
					} else {
						Error(la);
						goto case 75;
					}
				}
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				Expect(39, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 76: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 77;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				if (set[20, la.kind]) {
					goto case 78;
				} else {
					goto case 75;
				}
			}
			case 78: {
				stateStack.Push(75);
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 79;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				if (set[18, la.kind]) {
					goto case 84;
				} else {
					if (la.kind == 23) {
						goto case 80;
					} else {
						goto case 6;
					}
				}
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				currentState = 81;
				break;
			}
			case 81: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 82;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				if (set[18, la.kind]) {
					stateStack.Push(83);
					goto case 44;
				} else {
					goto case 83;
				}
			}
			case 83: {
				if (la == null) { currentState = 83; break; }
				if (la.kind == 23) {
					goto case 80;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 84: {
				stateStack.Push(85);
				goto case 44;
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				if (la.kind == 23) {
					currentState = 86;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 86: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 87;
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				if (set[18, la.kind]) {
					goto case 84;
				} else {
					goto case 85;
				}
			}
			case 88: {
				if (la == null) { currentState = 88; break; }
				currentState = 89;
				break;
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				if (set[15, la.kind]) {
					stateStack.Push(90);
					goto case 67;
				} else {
					goto case 90;
				}
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				if (la.kind == 23) {
					goto case 88;
				} else {
					goto case 75;
				}
			}
			case 91: {
				stateStack.Push(92);
				goto case 47;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				Expect(145, la); // "Is"
				currentState = 67;
				break;
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				if (set[21, la.kind]) {
					stateStack.Push(93);
					goto case 94;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				if (la.kind == 38) {
					currentState = 99;
					break;
				} else {
					if (set[22, la.kind]) {
						currentState = 95;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 95: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 96;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				if (la.kind == 10) {
					currentState = 97;
					break;
				} else {
					goto case 97;
				}
			}
			case 97: {
				stateStack.Push(98);
				goto case 59;
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				if (la.kind == 11) {
					goto case 19;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 99: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 100;
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				if (la.kind == 170) {
					goto case 101;
				} else {
					if (set[20, la.kind]) {
						goto case 78;
					} else {
						goto case 6;
					}
				}
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				currentState = 102;
				break;
			}
			case 102: {
				stateStack.Push(103);
				goto case 67;
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				if (la.kind == 23) {
					goto case 101;
				} else {
					goto case 75;
				}
			}
			case 104: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 105;
			}
			case 105: {
				if (la == null) { currentState = 105; break; }
				if (set[23, la.kind]) {
					goto case 19;
				} else {
					if (la.kind == 38) {
						goto case 111;
					} else {
						if (set[24, la.kind]) {
							goto case 19;
						} else {
							if (la.kind == 27 || la.kind == 30) {
								currentState = 59;
								break;
							} else {
								if (la.kind == 130) {
									currentState = 227;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 225;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											nextTokenIsPotentialStartOfXmlMode = true;
											PushContext(Context.Xml, t);
											goto case 208;
										} else {
											if (la.kind == 128 || la.kind == 210) {
												if (la.kind == 210) {
													currentState = 201;
													break;
												} else {
													if (la.kind == 128) {
														currentState = 178;
														break;
													} else {
														goto case 6;
													}
												}
											} else {
												if (la.kind == 59 || la.kind == 127) {
													if (la.kind == 127) {
														stateStack.Push(118);
														goto case 177;
													} else {
														if (la.kind == 59) {
															stateStack.Push(118);
															goto case 176;
														} else {
															Error(la);
															goto case 118;
														}
													}
												} else {
													if (set[25, la.kind]) {
														if (set[26, la.kind]) {
															currentState = 117;
															break;
														} else {
															if (la.kind == 95 || la.kind == 107 || la.kind == 219) {
																currentState = 113;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 136) {
															currentState = 106;
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
			case 106: {
				if (la == null) { currentState = 106; break; }
				Expect(38, la); // "("
				currentState = 107;
				break;
			}
			case 107: {
				stateStack.Push(108);
				goto case 44;
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				Expect(23, la); // ","
				currentState = 109;
				break;
			}
			case 109: {
				stateStack.Push(110);
				goto case 44;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				if (la.kind == 23) {
					goto case 111;
				} else {
					goto case 75;
				}
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				currentState = 112;
				break;
			}
			case 112: {
				stateStack.Push(75);
				goto case 44;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				Expect(38, la); // "("
				currentState = 114;
				break;
			}
			case 114: {
				stateStack.Push(115);
				goto case 44;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				Expect(23, la); // ","
				currentState = 116;
				break;
			}
			case 116: {
				stateStack.Push(75);
				goto case 67;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				Expect(38, la); // "("
				currentState = 112;
				break;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				if (set[27, la.kind]) {
					stateStack.Push(118);
					goto case 119;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				if (la.kind == 127) {
					goto case 173;
				} else {
					if (la.kind == 59) {
						currentState = 169;
						break;
					} else {
						if (la.kind == 197) {
							goto case 166;
						} else {
							if (la.kind == 108) {
								goto case 19;
							} else {
								if (la.kind == 230) {
									goto case 143;
								} else {
									if (la.kind == 177) {
										currentState = 162;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 160;
											break;
										} else {
											if (la.kind == 149) {
												goto case 157;
											} else {
												if (la.kind == 134) {
													currentState = 132;
													break;
												} else {
													if (la.kind == 147) {
														currentState = 120;
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
			case 120: {
				stateStack.Push(121);
				goto case 126;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				Expect(172, la); // "On"
				currentState = 122;
				break;
			}
			case 122: {
				stateStack.Push(123);
				goto case 44;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				Expect(117, la); // "Equals"
				currentState = 124;
				break;
			}
			case 124: {
				stateStack.Push(125);
				goto case 44;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				if (la.kind == 23) {
					currentState = 122;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 126: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(127);
				goto case 131;
			}
			case 127: {
				PopContext();
				goto case 128;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				if (la.kind == 64) {
					currentState = 130;
					break;
				} else {
					goto case 129;
				}
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				Expect(139, la); // "In"
				currentState = 44;
				break;
			}
			case 130: {
				stateStack.Push(129);
				goto case 67;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				if (set[28, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 132: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 133;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (la.kind == 147) {
					goto case 149;
				} else {
					if (set[29, la.kind]) {
						if (la.kind == 71) {
							goto case 146;
						} else {
							if (set[29, la.kind]) {
								goto case 147;
							} else {
								Error(la);
								goto case 134;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				Expect(71, la); // "By"
				currentState = 135;
				break;
			}
			case 135: {
				stateStack.Push(136);
				goto case 139;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (la.kind == 23) {
					goto case 146;
				} else {
					Expect(144, la); // "Into"
					currentState = 137;
					break;
				}
			}
			case 137: {
				stateStack.Push(138);
				goto case 139;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (la.kind == 23) {
					currentState = 137;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 139: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 140;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				if (set[28, la.kind]) {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(141);
					goto case 131;
				} else {
					goto case 44;
				}
			}
			case 141: {
				PopContext();
				goto case 142;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				if (la.kind == 64) {
					currentState = 144;
					break;
				} else {
					if (la.kind == 21) {
						goto case 143;
					} else {
						if (set[30, la.kind]) {
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
			case 143: {
				if (la == null) { currentState = 143; break; }
				currentState = 44;
				break;
			}
			case 144: {
				stateStack.Push(145);
				goto case 67;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				Expect(21, la); // "="
				currentState = 44;
				break;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				currentState = 135;
				break;
			}
			case 147: {
				stateStack.Push(148);
				goto case 139;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				if (la.kind == 23) {
					currentState = 147;
					break;
				} else {
					goto case 134;
				}
			}
			case 149: {
				stateStack.Push(150);
				goto case 156;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				if (la.kind == 134 || la.kind == 147) {
					if (la.kind == 134) {
						currentState = 154;
						break;
					} else {
						if (la.kind == 147) {
							goto case 149;
						} else {
							Error(la);
							goto case 150;
						}
					}
				} else {
					goto case 151;
				}
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(144, la); // "Into"
				currentState = 152;
				break;
			}
			case 152: {
				stateStack.Push(153);
				goto case 139;
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (la.kind == 23) {
					currentState = 152;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 154: {
				stateStack.Push(155);
				goto case 156;
			}
			case 155: {
				stateStack.Push(150);
				goto case 151;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				Expect(147, la); // "Join"
				currentState = 120;
				break;
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				currentState = 158;
				break;
			}
			case 158: {
				stateStack.Push(159);
				goto case 139;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (la.kind == 23) {
					goto case 157;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 160: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 161;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.kind == 231) {
					goto case 143;
				} else {
					goto case 44;
				}
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				Expect(71, la); // "By"
				currentState = 163;
				break;
			}
			case 163: {
				stateStack.Push(164);
				goto case 44;
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				if (la.kind == 65 || la.kind == 105) {
					currentState = 165;
					break;
				} else {
					Error(la);
					goto case 165;
				}
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 23) {
					currentState = 163;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				currentState = 167;
				break;
			}
			case 167: {
				stateStack.Push(168);
				goto case 139;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				if (la.kind == 23) {
					goto case 166;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 169: {
				stateStack.Push(170);
				goto case 126;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (set[27, la.kind]) {
					stateStack.Push(170);
					goto case 119;
				} else {
					Expect(144, la); // "Into"
					currentState = 171;
					break;
				}
			}
			case 171: {
				stateStack.Push(172);
				goto case 139;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				if (la.kind == 23) {
					currentState = 171;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				currentState = 174;
				break;
			}
			case 174: {
				stateStack.Push(175);
				goto case 126;
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (la.kind == 23) {
					goto case 173;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				Expect(59, la); // "Aggregate"
				currentState = 169;
				break;
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				Expect(127, la); // "From"
				currentState = 174;
				break;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				Expect(38, la); // "("
				currentState = 179;
				break;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				if (set[31, la.kind]) {
					stateStack.Push(180);
					goto case 187;
				} else {
					goto case 180;
				}
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				Expect(39, la); // ")"
				currentState = 181;
				break;
			}
			case 181: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 182;
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				if (set[18, la.kind]) {
					goto case 44;
				} else {
					if (la.kind == 1 || la.kind == 22 || la.kind == 64) {
						if (la.kind == 64) {
							currentState = 186;
							break;
						} else {
							goto case 183;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 183: {
				stateStack.Push(184);
				goto case 34;
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				Expect(114, la); // "End"
				currentState = 185;
				break;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				Expect(128, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 186: {
				stateStack.Push(183);
				goto case 67;
			}
			case 187: {
				stateStack.Push(188);
				goto case 189;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 23) {
					currentState = 187;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (la.kind == 41) {
					stateStack.Push(189);
					goto case 196;
				} else {
					goto case 190;
				}
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (set[32, la.kind]) {
					currentState = 190;
					break;
				} else {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(191);
					goto case 131;
				}
			}
			case 191: {
				PopContext();
				goto case 192;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				if (la.kind == 64) {
					goto case 194;
				} else {
					goto case 193;
				}
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (la.kind == 21) {
					goto case 143;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				currentState = 195;
				break;
			}
			case 195: {
				stateStack.Push(193);
				goto case 67;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				Expect(41, la); // "<"
				currentState = 197;
				break;
			}
			case 197: {
				PushContext(Context.Attribute, t);
				goto case 198;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (set[33, la.kind]) {
					currentState = 198;
					break;
				} else {
					Expect(40, la); // ">"
					currentState = 199;
					break;
				}
			}
			case 199: {
				PopContext();
				goto case 200;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (la.kind == 1) {
					goto case 19;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				Expect(38, la); // "("
				currentState = 202;
				break;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (set[31, la.kind]) {
					stateStack.Push(203);
					goto case 187;
				} else {
					goto case 203;
				}
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				Expect(39, la); // ")"
				currentState = 204;
				break;
			}
			case 204: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 205;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (set[9, la.kind]) {
					goto case 42;
				} else {
					if (la.kind == 1 || la.kind == 22) {
						stateStack.Push(206);
						goto case 34;
					} else {
						goto case 6;
					}
				}
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				Expect(114, la); // "End"
				currentState = 207;
				break;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 17 || la.kind == 19) {
					if (la.kind == 17) {
						currentState = 219;
						break;
					} else {
						if (la.kind == 19) {
							stateStack.Push(219);
							goto case 220;
						} else {
							Error(la);
							goto case 208;
						}
					}
				} else {
					stateStack.Push(209);
					goto case 211;
				}
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 17) {
					currentState = 210;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				if (la.kind == 16) {
					currentState = 209;
					break;
				} else {
					goto case 209;
				}
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 212;
				break;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (set[34, la.kind]) {
					if (set[35, la.kind]) {
						currentState = 212;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(212);
							goto case 216;
						} else {
							Error(la);
							goto case 212;
						}
					}
				} else {
					if (la.kind == 14) {
						goto case 19;
					} else {
						if (la.kind == 11) {
							goto case 213;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				currentState = 214;
				break;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (set[36, la.kind]) {
					if (set[37, la.kind]) {
						goto case 213;
					} else {
						if (la.kind == 12) {
							stateStack.Push(214);
							goto case 216;
						} else {
							if (la.kind == 10) {
								stateStack.Push(214);
								goto case 211;
							} else {
								Error(la);
								goto case 214;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 215;
					break;
				}
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (set[38, la.kind]) {
					if (set[39, la.kind]) {
						currentState = 215;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(215);
							goto case 216;
						} else {
							Error(la);
							goto case 215;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 217;
				break;
			}
			case 217: {
				stateStack.Push(218);
				goto case 44;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 16) {
					currentState = 208;
					break;
				} else {
					goto case 208;
				}
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				Expect(19, la); // XmlProcessingInstructionStart
				currentState = 221;
				break;
			}
			case 221: {
				stateStack.Push(222);
				goto case 131;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				if (set[28, la.kind]) {
					currentState = 223;
					break;
				} else {
					Expect(20, la); // XmlProcessingInstructionEnd
					currentState = stateStack.Pop();
					break;
				}
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				if (la.kind == 21) {
					currentState = 224;
					break;
				} else {
					goto case 222;
				}
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				Expect(3, la); // LiteralString
				currentState = 222;
				break;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				Expect(38, la); // "("
				currentState = 226;
				break;
			}
			case 226: {
				readXmlIdentifier = true;
				stateStack.Push(75);
				goto case 131;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				Expect(38, la); // "("
				currentState = 116;
				break;
			}
			case 228: {
				stateStack.Push(229);
				goto case 44;
			}
			case 229: {
				stateStack.Push(230);
				goto case 34;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				Expect(114, la); // "End"
				currentState = 231;
				break;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				currentState = 233;
				break;
			}
			case 233: {
				stateStack.Push(234);
				goto case 44;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				if (la.kind == 23) {
					goto case 232;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 235: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 236;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (la.kind == 184) {
					goto case 143;
				} else {
					goto case 44;
				}
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				currentState = 238;
				break;
			}
			case 238: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 239;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (set[18, la.kind]) {
					goto case 44;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.kind == 109 || la.kind == 125 || la.kind == 231) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (set[40, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (set[41, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 243: {
				stateStack.Push(244);
				goto case 34;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (la.kind == 76) {
					currentState = 248;
					break;
				} else {
					if (la.kind == 124) {
						currentState = 247;
						break;
					} else {
						goto case 245;
					}
				}
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				Expect(114, la); // "End"
				currentState = 246;
				break;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 247: {
				stateStack.Push(245);
				goto case 34;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (set[28, la.kind]) {
					PushContext(Context.IdentifierExpected, t);
					stateStack.Push(251);
					goto case 131;
				} else {
					goto case 249;
				}
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 229) {
					currentState = 250;
					break;
				} else {
					goto case 243;
				}
			}
			case 250: {
				stateStack.Push(243);
				goto case 44;
			}
			case 251: {
				PopContext();
				goto case 252;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (la.kind == 64) {
					currentState = 253;
					break;
				} else {
					goto case 249;
				}
			}
			case 253: {
				stateStack.Push(249);
				goto case 67;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (set[42, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				Expect(119, la); // "Error"
				currentState = 256;
				break;
			}
			case 256: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 257;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (set[18, la.kind]) {
					goto case 44;
				} else {
					if (la.kind == 133) {
						currentState = 259;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 258;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(164, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (set[41, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 260: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 261;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (set[14, la.kind]) {
					stateStack.Push(275);
					goto case 271;
				} else {
					if (la.kind == 111) {
						currentState = 262;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 262: {
				stateStack.Push(263);
				goto case 271;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				Expect(139, la); // "In"
				currentState = 264;
				break;
			}
			case 264: {
				stateStack.Push(265);
				goto case 44;
			}
			case 265: {
				stateStack.Push(266);
				goto case 34;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				Expect(164, la); // "Next"
				currentState = 267;
				break;
			}
			case 267: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (set[18, la.kind]) {
					goto case 269;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 269: {
				stateStack.Push(270);
				goto case 44;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.kind == 23) {
					currentState = 269;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 271: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(272);
				goto case 104;
			}
			case 272: {
				PopContext();
				goto case 273;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				if (la.kind == 34) {
					currentState = 274;
					break;
				} else {
					goto case 274;
				}
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[21, la.kind]) {
					stateStack.Push(274);
					goto case 94;
				} else {
					if (la.kind == 64) {
						currentState = 67;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(21, la); // "="
				currentState = 276;
				break;
			}
			case 276: {
				stateStack.Push(277);
				goto case 44;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (la.kind == 205) {
					currentState = 284;
					break;
				} else {
					goto case 278;
				}
			}
			case 278: {
				stateStack.Push(279);
				goto case 34;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				Expect(164, la); // "Next"
				currentState = 280;
				break;
			}
			case 280: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 281;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (set[18, la.kind]) {
					goto case 282;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 282: {
				stateStack.Push(283);
				goto case 44;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (la.kind == 23) {
					currentState = 282;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 284: {
				stateStack.Push(278);
				goto case 44;
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 288;
					break;
				} else {
					if (la.kind == 1 || la.kind == 22) {
						stateStack.Push(286);
						goto case 34;
					} else {
						goto case 6;
					}
				}
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				Expect(153, la); // "Loop"
				currentState = 287;
				break;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 224 || la.kind == 231) {
					goto case 143;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 288: {
				stateStack.Push(289);
				goto case 44;
			}
			case 289: {
				stateStack.Push(290);
				goto case 34;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				Expect(153, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 291: {
				stateStack.Push(292);
				goto case 44;
			}
			case 292: {
				stateStack.Push(293);
				goto case 34;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				Expect(114, la); // "End"
				currentState = 294;
				break;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 295: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 296;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (la.kind == 75) {
					currentState = 297;
					break;
				} else {
					goto case 297;
				}
			}
			case 297: {
				stateStack.Push(298);
				goto case 44;
			}
			case 298: {
				stateStack.Push(299);
				goto case 18;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 75) {
					currentState = 301;
					break;
				} else {
					Expect(114, la); // "End"
					currentState = 300;
					break;
				}
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 301: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 302;
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (la.kind == 112) {
					currentState = 303;
					break;
				} else {
					if (set[43, la.kind]) {
						goto case 304;
					} else {
						Error(la);
						goto case 303;
					}
				}
			}
			case 303: {
				stateStack.Push(299);
				goto case 34;
			}
			case 304: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 305;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (set[44, la.kind]) {
					if (la.kind == 145) {
						currentState = 307;
						break;
					} else {
						goto case 307;
					}
				} else {
					if (set[18, la.kind]) {
						stateStack.Push(306);
						goto case 44;
					} else {
						Error(la);
						goto case 306;
					}
				}
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (la.kind == 23) {
					currentState = 304;
					break;
				} else {
					goto case 303;
				}
			}
			case 307: {
				stateStack.Push(308);
				goto case 309;
			}
			case 308: {
				stateStack.Push(306);
				goto case 47;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (set[45, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 310: {
				stateStack.Push(311);
				goto case 44;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				if (la.kind == 214) {
					currentState = 320;
					break;
				} else {
					goto case 312;
				}
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 1 || la.kind == 22) {
					goto case 313;
				} else {
					goto case 6;
				}
			}
			case 313: {
				stateStack.Push(314);
				goto case 34;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 112 || la.kind == 113) {
					if (la.kind == 112) {
						currentState = 319;
						break;
					} else {
						if (la.kind == 113) {
							goto case 316;
						} else {
							Error(la);
							goto case 313;
						}
					}
				} else {
					Expect(114, la); // "End"
					currentState = 315;
					break;
				}
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				Expect(136, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				currentState = 317;
				break;
			}
			case 317: {
				stateStack.Push(318);
				goto case 44;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (la.kind == 214) {
					currentState = 313;
					break;
				} else {
					goto case 313;
				}
			}
			case 319: {
				if (la == null) { currentState = 319; break; }
				if (la.kind == 136) {
					goto case 316;
				} else {
					goto case 313;
				}
			}
			case 320: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 321;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (set[9, la.kind]) {
					goto case 322;
				} else {
					goto case 312;
				}
			}
			case 322: {
				stateStack.Push(323);
				goto case 42;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 22) {
					currentState = 329;
					break;
				} else {
					if (la.kind == 112) {
						goto case 325;
					} else {
						goto case 324;
					}
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				currentState = 326;
				break;
			}
			case 326: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 327;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (set[9, la.kind]) {
					stateStack.Push(328);
					goto case 42;
				} else {
					goto case 328;
				}
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 22) {
					goto case 325;
				} else {
					goto case 324;
				}
			}
			case 329: {
				nextTokenIsPotentialStartOfXmlMode = true;
				goto case 330;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				if (set[9, la.kind]) {
					goto case 322;
				} else {
					goto case 323;
				}
			}
			case 331: {
				stateStack.Push(332);
				goto case 59;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 38) {
					currentState = 76;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 333: {
				stateStack.Push(334);
				goto case 44;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				Expect(23, la); // ","
				currentState = 44;
				break;
			}
			case 335: {
				stateStack.Push(336);
				goto case 44;
			}
			case 336: {
				stateStack.Push(337);
				goto case 34;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				Expect(114, la); // "End"
				currentState = 338;
				break;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 211 || la.kind == 233) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 339: {
				PushContext(Context.IdentifierExpected, t);
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				stateStack.Push(340);
				goto case 131;
			}
			case 340: {
				PopContext();
				goto case 341;
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 34) {
					currentState = 342;
					break;
				} else {
					goto case 342;
				}
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 38) {
					goto case 352;
				} else {
					goto case 343;
				}
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (la.kind == 23) {
					currentState = 345;
					break;
				} else {
					if (la.kind == 64) {
						currentState = 344;
						break;
					} else {
						goto case 193;
					}
				}
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (la.kind == 163) {
					goto case 194;
				} else {
					goto case 195;
				}
			}
			case 345: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(346);
				goto case 131;
			}
			case 346: {
				PopContext();
				goto case 347;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 34) {
					currentState = 348;
					break;
				} else {
					goto case 348;
				}
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (la.kind == 38) {
					goto case 349;
				} else {
					goto case 343;
				}
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				currentState = 350;
				break;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 23) {
					goto case 349;
				} else {
					goto case 351;
				}
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				Expect(39, la); // ")"
				currentState = 343;
				break;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				currentState = 353;
				break;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (la.kind == 23) {
					goto case 352;
				} else {
					goto case 351;
				}
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 41) {
					stateStack.Push(354);
					goto case 196;
				} else {
					stateStack.Push(31);
					goto case 67;
				}
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				Expect(99, la); // "Custom"
				currentState = 356;
				break;
			}
			case 356: {
				stateStack.Push(357);
				goto case 367;
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (set[46, la.kind]) {
					goto case 359;
				} else {
					Expect(114, la); // "End"
					currentState = 358;
					break;
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				Expect(120, la); // "Event"
				currentState = 18;
				break;
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (la.kind == 41) {
					stateStack.Push(359);
					goto case 196;
				} else {
					if (la.kind == 57 || la.kind == 189 || la.kind == 193) {
						currentState = 360;
						break;
					} else {
						Error(la);
						goto case 360;
					}
				}
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				Expect(38, la); // "("
				currentState = 361;
				break;
			}
			case 361: {
				stateStack.Push(362);
				goto case 187;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				Expect(39, la); // ")"
				currentState = 363;
				break;
			}
			case 363: {
				stateStack.Push(364);
				goto case 34;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				Expect(114, la); // "End"
				currentState = 365;
				break;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 57 || la.kind == 189 || la.kind == 193) {
					currentState = 366;
					break;
				} else {
					Error(la);
					goto case 366;
				}
			}
			case 366: {
				stateStack.Push(357);
				goto case 18;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				Expect(120, la); // "Event"
				currentState = 368;
				break;
			}
			case 368: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(369);
				goto case 131;
			}
			case 369: {
				PopContext();
				goto case 370;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (la.kind == 64) {
					currentState = 377;
					break;
				} else {
					if (set[47, la.kind]) {
						if (la.kind == 38) {
							currentState = 375;
							break;
						} else {
							goto case 371;
						}
					} else {
						Error(la);
						goto case 371;
					}
				}
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (la.kind == 137) {
					goto case 372;
				} else {
					goto case 18;
				}
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				currentState = 373;
				break;
			}
			case 373: {
				stateStack.Push(374);
				goto case 67;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (la.kind == 23) {
					goto case 372;
				} else {
					goto case 18;
				}
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (set[31, la.kind]) {
					stateStack.Push(376);
					goto case 187;
				} else {
					goto case 376;
				}
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				Expect(39, la); // ")"
				currentState = 371;
				break;
			}
			case 377: {
				stateStack.Push(371);
				goto case 67;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				Expect(102, la); // "Declare"
				currentState = 379;
				break;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 63 || la.kind == 67 || la.kind == 223) {
					currentState = 380;
					break;
				} else {
					goto case 380;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 128 || la.kind == 210) {
					currentState = 381;
					break;
				} else {
					Error(la);
					goto case 381;
				}
			}
			case 381: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(382);
				goto case 131;
			}
			case 382: {
				PopContext();
				goto case 383;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				Expect(150, la); // "Lib"
				currentState = 384;
				break;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				Expect(3, la); // LiteralString
				currentState = 385;
				break;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (la.kind == 60) {
					currentState = 389;
					break;
				} else {
					goto case 386;
				}
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (la.kind == 38) {
					currentState = 387;
					break;
				} else {
					goto case 18;
				}
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (set[31, la.kind]) {
					stateStack.Push(388);
					goto case 187;
				} else {
					goto case 388;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				Expect(39, la); // ")"
				currentState = 18;
				break;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				Expect(3, la); // LiteralString
				currentState = 386;
				break;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (la.kind == 128 || la.kind == 210) {
					currentState = 391;
					break;
				} else {
					Error(la);
					goto case 391;
				}
			}
			case 391: {
				PushContext(Context.IdentifierExpected, t);
				goto case 392;
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				currentState = 393;
				break;
			}
			case 393: {
				PopContext();
				goto case 394;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 38) {
					currentState = 400;
					break;
				} else {
					goto case 395;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (la.kind == 64) {
					currentState = 399;
					break;
				} else {
					goto case 396;
				}
			}
			case 396: {
				stateStack.Push(397);
				goto case 34;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				Expect(114, la); // "End"
				currentState = 398;
				break;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (la.kind == 128 || la.kind == 210) {
					currentState = 18;
					break;
				} else {
					Error(la);
					goto case 18;
				}
			}
			case 399: {
				stateStack.Push(396);
				goto case 67;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (set[31, la.kind]) {
					stateStack.Push(401);
					goto case 187;
				} else {
					goto case 401;
				}
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				Expect(39, la); // ")"
				currentState = 395;
				break;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 89) {
					currentState = 403;
					break;
				} else {
					goto case 403;
				}
			}
			case 403: {
				PushContext(Context.IdentifierExpected, t);
				stateStack.Push(404);
				goto case 409;
			}
			case 404: {
				PopContext();
				goto case 405;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 64) {
					currentState = 408;
					break;
				} else {
					goto case 406;
				}
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 21) {
					currentState = 407;
					break;
				} else {
					goto case 18;
				}
			}
			case 407: {
				stateStack.Push(18);
				goto case 44;
			}
			case 408: {
				stateStack.Push(406);
				goto case 67;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (set[48, la.kind]) {
					goto case 19;
				} else {
					goto case 6;
				}
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				currentState = 411;
				break;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (set[3, la.kind]) {
					goto case 410;
				} else {
					stateStack.Push(412);
					goto case 18;
				}
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (set[49, la.kind]) {
					stateStack.Push(412);
					goto case 5;
				} else {
					Expect(114, la); // "End"
					currentState = 413;
					break;
				}
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				Expect(161, la); // "Namespace"
				currentState = 18;
				break;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				Expect(138, la); // "Imports"
				currentState = 415;
				break;
			}
			case 415: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 416;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (set[3, la.kind]) {
					currentState = 416;
					break;
				} else {
					goto case 18;
				}
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				Expect(174, la); // "Option"
				currentState = 418;
				break;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (set[3, la.kind]) {
					currentState = 418;
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
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, x,x,T,x, x,T,T,x, T,x,x,x, x,x,x,x, x,T,x,x, T,x,T,x, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, T,x,x,T, x,T,x,x, x,T,x,T, T,T,x,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, T,x,T,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,T,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,T, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,T,x, x,T,T,T, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,T,T,T, x,T,T,T, x,T,x,T, T,x,T,x, x,T,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,T,x, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,T,x, x,T,T,T, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, T,x,T,x, x,T,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,T,x, x,T,T,T, T,T,T,x, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,x,x, x,T,T,T, x,T,T,T, x,T,x,T, T,x,T,x, x,T,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, T,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,T,x,T, x,T,T,T, x,T,x,x, x,x,x,T, T,x,T,x, x,x,T,T, T,T,x,T, x,T,T,T, T,x,x,T, T,x,T,x, x,x,T,T, x,T,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,T,x, x,T,T,T, T,T,T,x, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,x, x,x,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,T,T,x, x,x,x,T, T,T,x,T, x,x,x,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,x,x, x,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,x, x,x,x,T, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,T, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,x, x,x,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, T,T,x,x, x,T,x,x, T,x,T,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,T, x,x,T,x, x,x,x,x, T,x,T,x, T,x,x,T, T,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,T,T, x,x,x,x, T,x,x,x, T,T,x,x, x,x,x,x, x,T,T,T, T,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,x, x,x,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,x, x,x,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,T, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,x, x,x,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,T, T,T,T,T, T,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, x,x,x,T, T,T,T,T, T,T,T,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, x,x,x,T, T,T,T,x, T,T,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,x,x,x, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,T, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,x, x,x,T,x, T,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,x,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,T,T, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,x,x, x,x,T,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,T,x,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,x,T, x,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,T,T,T, T,T,T,T, T,T,T,x, x,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,T, T,T,T,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,T,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, x,T,x,x, x,x,x,T, x,x,T,T, T,x,x,x, x,x,T,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,T,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,T,T,T, T,T,T,x, T,x,T,T, T,x,x,T, T,T,T,T, T,T,T,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,T, x,x,x,T, T,x,T,x, x,x,T,x, T,x,x,x, T,x,x,x, T,T,x,T, T,x,x,x, x,x,T,x, x,x,x,T, T,x,x,T, x,T,T,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,T,x, T,T,x,x, x,T,x,T, T,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,T,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,T,T,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x},
		{x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,T, x,T,T,T, T,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, T,x,x,x, x,x,x,x, x,T,x,x, x,x,T,x, x,x,x,T, x,x,x,x, x,x,T,x, x,x,x,x, T,x,x,x, T,x,x,T, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,T,x,x, x,x,x,x, T,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, T,T,x,x, x,x,x,x, x,x,x,T, T,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x},
		{x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,T,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, T,x,x,x, x,T,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,T,x,T, T,x,x,x, x,x,x,x, x,x,x,T, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x, x,x,x,x}

	};

} // end Parser


}