using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using ICSharpCode.NRefactory.Ast;
using ICSharpCode.NRefactory.Parser.VB;
using ASTAttribute = ICSharpCode.NRefactory.Ast.Attribute;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 35;
	const int endOfStatementTerminatorAndBlock = 204;
	static BitArray GetExpectedSet(int state)
	{
		switch (state) {
			case 0:
			case 1:
				return set[0];
			case 2:
				return set[1];
			case 3:
			case 4:
				return set[2];
			case 5:
				return set[3];
			case 6:
			case 205:
			case 451:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 191:
			case 435:
			case 442:
			case 450:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 10:
			case 11:
			case 102:
			case 157:
			case 158:
			case 206:
			case 344:
			case 345:
			case 359:
			case 360:
			case 361:
			case 384:
			case 385:
			case 386:
			case 387:
			case 400:
			case 401:
			case 443:
			case 444:
			case 463:
			case 464:
			case 476:
			case 477:
				return set[6];
			case 12:
			case 13:
			case 445:
			case 446:
				return set[7];
			case 14:
			case 406:
			case 447:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 15:
			case 16:
			case 197:
			case 200:
			case 201:
			case 211:
			case 225:
			case 229:
			case 247:
			case 260:
			case 271:
			case 274:
			case 280:
			case 285:
			case 294:
			case 295:
			case 308:
			case 316:
			case 394:
			case 407:
			case 419:
			case 422:
			case 448:
			case 483:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 17:
			case 305:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 18:
			case 19:
			case 106:
			case 115:
			case 131:
			case 146:
			case 161:
			case 235:
			case 326:
			case 334:
			case 342:
			case 370:
			case 428:
			case 432:
			case 452:
			case 461:
			case 489:
				return set[8];
			case 20:
			case 23:
				return set[9];
			case 21:
				return set[10];
			case 22:
			case 52:
			case 56:
			case 111:
			case 311:
			case 373:
				return set[11];
			case 24:
			case 121:
			case 128:
			case 132:
			case 192:
			case 348:
			case 366:
			case 369:
			case 402:
			case 403:
			case 416:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 25:
			case 26:
			case 113:
			case 114:
				return set[12];
			case 27:
			case 194:
			case 332:
			case 350:
			case 368:
			case 383:
			case 405:
			case 418:
			case 431:
			case 454:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 28:
			case 29:
			case 32:
			case 33:
			case 378:
			case 379:
				return set[13];
			case 30:
				return set[14];
			case 31:
			case 123:
			case 130:
			case 314:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 34:
			case 116:
			case 125:
			case 331:
			case 333:
			case 336:
			case 377:
			case 381:
			case 469:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 35:
			case 36:
			case 38:
			case 39:
			case 40:
			case 43:
			case 54:
			case 104:
			case 122:
			case 124:
			case 126:
			case 129:
			case 138:
			case 140:
			case 178:
			case 210:
			case 214:
			case 216:
			case 217:
			case 232:
			case 246:
			case 251:
			case 258:
			case 264:
			case 266:
			case 270:
			case 273:
			case 279:
			case 290:
			case 292:
			case 298:
			case 313:
			case 315:
			case 363:
			case 375:
			case 376:
			case 460:
				return set[15];
			case 37:
			case 41:
				return set[16];
			case 42:
			case 49:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 44:
			case 55:
			case 473:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 45:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 46:
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 47:
				return set[17];
			case 48:
			case 57:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 50:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 51:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 53:
			case 160:
			case 162:
			case 257:
			case 485:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 58:
			case 276:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 59:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 60:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 61:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 62:
			case 228:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 63:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 64:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 65:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 66:
			case 354:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 67:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 70:
			case 282:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 71:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 72:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 73:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 74:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 75:
			case 241:
			case 248:
			case 261:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 77:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 78:
			case 165:
			case 170:
			case 172:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 79:
			case 167:
			case 171:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 80:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 81:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 82:
			case 199:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 83:
			case 190:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 84:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 86:
			case 139:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 87:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 90:
			case 411:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 91:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 93:
			case 150:
			case 177:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 95:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 96:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 99:
			case 189:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 100:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 101:
				return set[18];
			case 103:
				return set[19];
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 107:
				return set[20];
			case 108:
				return set[21];
			case 109:
			case 110:
			case 371:
			case 372:
				return set[22];
			case 112:
				return set[23];
			case 117:
			case 118:
			case 244:
			case 253:
				return set[24];
			case 119:
				return set[25];
			case 120:
			case 297:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 127:
				return set[26];
			case 133:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 134:
			case 135:
				return set[27];
			case 136:
			case 142:
			case 147:
			case 183:
			case 187:
			case 224:
			case 319:
			case 327:
			case 367:
			case 424:
			case 436:
			case 484:
				return set[28];
			case 137:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 141:
			case 154:
			case 169:
			case 174:
			case 180:
			case 182:
			case 186:
			case 188:
				return set[29];
			case 143:
			case 144:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 145:
			case 245:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 148:
			case 149:
			case 151:
			case 153:
			case 155:
			case 156:
			case 163:
			case 168:
			case 173:
			case 181:
			case 185:
				return set[30];
			case 152:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 159:
				return set[31];
			case 164:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 166:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 175:
			case 176:
				return set[32];
			case 179:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 184:
				return set[33];
			case 193:
			case 349:
			case 382:
			case 430:
			case 453:
				return set[34];
			case 195:
			case 196:
				return set[35];
			case 198:
			case 212:
			case 227:
			case 275:
			case 317:
			case 353:
			case 408:
			case 420:
			case 449:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 202:
			case 203:
				return set[36];
			case 204:
				return set[37];
			case 207:
				return set[38];
			case 208:
			case 209:
			case 303:
				return set[39];
			case 213:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 215:
			case 252:
			case 265:
				return set[40];
			case 218:
			case 219:
			case 249:
			case 250:
			case 262:
			case 263:
				return set[41];
			case 220:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 221:
				return set[42];
			case 222:
			case 237:
				return set[43];
			case 223:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 226:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 230:
				return set[44];
			case 231:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 233:
			case 234:
				return set[45];
			case 236:
				return set[46];
			case 238:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 239:
			case 240:
				return set[47];
			case 242:
			case 243:
				return set[48];
			case 254:
			case 255:
				return set[49];
			case 256:
				return set[50];
			case 259:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 267:
				return set[51];
			case 268:
			case 272:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 269:
				return set[52];
			case 277:
			case 278:
				return set[53];
			case 281:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 283:
			case 284:
				return set[54];
			case 286:
			case 287:
				return set[55];
			case 288:
			case 429:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 289:
			case 291:
				return set[56];
			case 293:
			case 299:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 296:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 300:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 301:
			case 302:
			case 306:
			case 307:
			case 351:
			case 352:
				return set[57];
			case 304:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 309:
			case 310:
				return set[58];
			case 312:
				return set[59];
			case 318:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 320:
			case 321:
			case 328:
			case 329:
				return set[60];
			case 322:
			case 330:
				return set[61];
			case 323:
				return set[62];
			case 324:
				return set[63];
			case 325:
				return set[64];
			case 335:
			case 337:
			case 404:
			case 417:
				return set[65];
			case 338:
				return set[66];
			case 339:
			case 340:
				return set[67];
			case 341:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 343:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 346:
			case 347:
				return set[68];
			case 355:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 356:
				return set[69];
			case 357:
				return set[70];
			case 358:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 362:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 364:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 365:
				return set[71];
			case 374:
				return set[72];
			case 380:
				return set[73];
			case 388:
				return set[74];
			case 389:
				return set[75];
			case 390:
				return set[76];
			case 391:
			case 392:
			case 398:
				return set[77];
			case 393:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 395:
				return set[78];
			case 396:
				return set[79];
			case 397:
				return set[80];
			case 399:
			case 409:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 410:
				return set[81];
			case 412:
			case 414:
			case 423:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 413:
				return set[82];
			case 415:
				return set[83];
			case 421:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 425:
			case 426:
				return set[84];
			case 427:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 433:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 434:
				return set[85];
			case 437:
			case 438:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 439:
			case 441:
			case 486:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 440:
				return set[86];
			case 455:
				return set[87];
			case 456:
			case 462:
				return set[88];
			case 457:
			case 458:
				return set[89];
			case 459:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 465:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 466:
				return set[90];
			case 467:
			case 475:
				return set[91];
			case 468:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 470:
			case 471:
				return set[92];
			case 472:
			case 474:
				return set[93];
			case 478:
				return set[94];
			case 479:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 480:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 481:
			case 482:
				return set[95];
			case 487:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 488:
				return set[96];
			case 490:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 491:
				return set[97];
			case 492:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 493:
				return set[98];
			default: throw new InvalidOperationException();
		}
	}

	const bool T = true;
	const bool x = false;

	int currentState = 0;

	readonly Stack<int> stateStack = new Stack<int>();
	bool wasQualifierTokenAtStart = false;
	bool nextTokenIsPotentialStartOfExpression = false;
	bool readXmlIdentifier = false;
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	List<Token> errors = new List<Token>();
	
	public ExpressionFinder()
	{
		stateStack.Push(-1); // required so that we don't crash when leaving the root production
	}

	void Expect(int expectedKind, Token la)
	{
		if (la.kind != expectedKind) {
			Error(la);
			output.AppendLine("expected: " + expectedKind);
			//Console.WriteLine("expected: " + expectedKind);
		}
	}
	
	void Error(Token la) 
	{
		output.AppendLine("not expected: " + la);
		//Console.WriteLine("not expected: " + la);
		errors.Add(la);
	}
	
	Token t;
	
	public void InformToken(Token la) 
	{
		nextTokenIsPotentialStartOfExpression = false;
		readXmlIdentifier = false;
		nextTokenIsStartOfImportsOrAccessExpression = false;
		wasQualifierTokenAtStart = false;
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, la, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 490;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					PushContext(Context.Importable, la, t);
					goto case 480;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 343;
				} else {
					goto case 4;
				}
			}
			case 4: {
				if (la == null) { currentState = 4; break; }
				if (set[3].Get(la.kind)) {
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
				if (la.kind == 160) {
					currentState = 476;
					break;
				} else {
					if (set[4].Get(la.kind)) {
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
				if (la.kind == 40) {
					stateStack.Push(7);
					goto case 343;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[99].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						currentState = 384;
						break;
					} else {
						if (la.kind == 103) {
							currentState = 9;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 9: {
				if (la == null) { currentState = 9; break; }
				if (la.kind == 210) {
					currentState = 10;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 10;
						break;
					} else {
						Error(la);
						goto case 10;
					}
				}
			}
			case 10: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 11;
			}
			case 11: {
				if (la == null) { currentState = 11; break; }
				currentState = 12;
				break;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 382;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				if (la.kind == 63) {
					currentState = 18;
					break;
				} else {
					goto case 15;
				}
			}
			case 15: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 16;
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				if (la.kind == 1) {
					goto case 17;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 17: {
				if (la == null) { currentState = 17; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 18: {
				stateStack.Push(15);
				goto case 19;
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.kind == 130) {
					currentState = 20;
					break;
				} else {
					if (set[28].Get(la.kind)) {
						currentState = 20;
						break;
					} else {
						if (set[100].Get(la.kind)) {
							currentState = 20;
							break;
						} else {
							Error(la);
							goto case 20;
						}
					}
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				if (la.kind == 37) {
					stateStack.Push(20);
					goto case 24;
				} else {
					goto case 21;
				}
			}
			case 21: {
				if (la == null) { currentState = 21; break; }
				if (la.kind == 26) {
					currentState = 22;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 22: {
				stateStack.Push(23);
				goto case 56;
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (la.kind == 37) {
					stateStack.Push(23);
					goto case 24;
				} else {
					goto case 21;
				}
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				Expect(37, la); // "("
				currentState = 25;
				break;
			}
			case 25: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 26;
			}
			case 26: {
				if (la == null) { currentState = 26; break; }
				if (la.kind == 169) {
					currentState = 380;
					break;
				} else {
					if (set[13].Get(la.kind)) {
						goto case 28;
					} else {
						Error(la);
						goto case 27;
					}
				}
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 28: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 29;
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (set[14].Get(la.kind)) {
					stateStack.Push(27);
					nextTokenIsPotentialStartOfExpression = true;
					goto case 30;
				} else {
					goto case 27;
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (set[15].Get(la.kind)) {
					goto case 376;
				} else {
					if (la.kind == 22) {
						goto case 31;
					} else {
						goto case 6;
					}
				}
			}
			case 31: {
				if (la == null) { currentState = 31; break; }
				currentState = 32;
				break;
			}
			case 32: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 33;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(34);
					goto case 35;
				} else {
					goto case 34;
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 22) {
					goto case 31;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				PushContext(Context.Expression, la, t);
				goto case 36;
			}
			case 36: {
				stateStack.Push(37);
				goto case 38;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (set[101].Get(la.kind)) {
					currentState = 36;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 38: {
				PushContext(Context.Expression, la, t);
				goto case 39;
			}
			case 39: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 40;
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				if (set[102].Get(la.kind)) {
					currentState = 39;
					break;
				} else {
					if (set[24].Get(la.kind)) {
						stateStack.Push(107);
						goto case 117;
					} else {
						if (la.kind == 220) {
							currentState = 104;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(41);
								goto case 46;
							} else {
								if (la.kind == 35) {
									stateStack.Push(41);
									goto case 42;
								} else {
									Error(la);
									goto case 41;
								}
							}
						}
					}
				}
			}
			case 41: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				Expect(35, la); // "{"
				currentState = 43;
				break;
			}
			case 43: {
				stateStack.Push(44);
				goto case 35;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.kind == 22) {
					currentState = 43;
					break;
				} else {
					goto case 45;
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				Expect(162, la); // "New"
				currentState = 47;
				break;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (set[8].Get(la.kind)) {
					stateStack.Push(101);
					goto case 19;
				} else {
					goto case 48;
				}
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (la.kind == 233) {
					currentState = 49;
					break;
				} else {
					goto case 6;
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				Expect(35, la); // "{"
				currentState = 50;
				break;
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				if (la.kind == 147) {
					currentState = 51;
					break;
				} else {
					goto case 51;
				}
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				Expect(26, la); // "."
				currentState = 52;
				break;
			}
			case 52: {
				stateStack.Push(53);
				goto case 56;
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				Expect(20, la); // "="
				currentState = 54;
				break;
			}
			case 54: {
				stateStack.Push(55);
				goto case 35;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (la.kind == 22) {
					currentState = 50;
					break;
				} else {
					goto case 45;
				}
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				if (la.kind == 2) {
					goto case 100;
				} else {
					if (la.kind == 56) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 57) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 58) {
								goto case 99;
							} else {
								if (la.kind == 59) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 60) {
										currentState = stateStack.Pop();
										break;
									} else {
										if (la.kind == 61) {
											currentState = stateStack.Pop();
											break;
										} else {
											if (la.kind == 62) {
												goto case 98;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 97;
													} else {
														if (la.kind == 65) {
															goto case 96;
														} else {
															if (la.kind == 66) {
																goto case 95;
															} else {
																if (la.kind == 67) {
																	goto case 94;
																} else {
																	if (la.kind == 68) {
																		currentState = stateStack.Pop();
																		break;
																	} else {
																		if (la.kind == 69) {
																			currentState = stateStack.Pop();
																			break;
																		} else {
																			if (la.kind == 70) {
																				goto case 93;
																			} else {
																				if (la.kind == 71) {
																					currentState = stateStack.Pop();
																					break;
																				} else {
																					if (la.kind == 72) {
																						currentState = stateStack.Pop();
																						break;
																					} else {
																						if (la.kind == 73) {
																							currentState = stateStack.Pop();
																							break;
																						} else {
																							if (la.kind == 74) {
																								currentState = stateStack.Pop();
																								break;
																							} else {
																								if (la.kind == 75) {
																									currentState = stateStack.Pop();
																									break;
																								} else {
																									if (la.kind == 76) {
																										currentState = stateStack.Pop();
																										break;
																									} else {
																										if (la.kind == 77) {
																											currentState = stateStack.Pop();
																											break;
																										} else {
																											if (la.kind == 78) {
																												currentState = stateStack.Pop();
																												break;
																											} else {
																												if (la.kind == 79) {
																													currentState = stateStack.Pop();
																													break;
																												} else {
																													if (la.kind == 80) {
																														currentState = stateStack.Pop();
																														break;
																													} else {
																														if (la.kind == 81) {
																															currentState = stateStack.Pop();
																															break;
																														} else {
																															if (la.kind == 82) {
																																currentState = stateStack.Pop();
																																break;
																															} else {
																																if (la.kind == 83) {
																																	currentState = stateStack.Pop();
																																	break;
																																} else {
																																	if (la.kind == 84) {
																																		goto case 92;
																																	} else {
																																		if (la.kind == 85) {
																																			currentState = stateStack.Pop();
																																			break;
																																		} else {
																																			if (la.kind == 86) {
																																				currentState = stateStack.Pop();
																																				break;
																																			} else {
																																				if (la.kind == 87) {
																																					goto case 91;
																																				} else {
																																					if (la.kind == 88) {
																																						currentState = stateStack.Pop();
																																						break;
																																					} else {
																																						if (la.kind == 89) {
																																							currentState = stateStack.Pop();
																																							break;
																																						} else {
																																							if (la.kind == 90) {
																																								currentState = stateStack.Pop();
																																								break;
																																							} else {
																																								if (la.kind == 91) {
																																									currentState = stateStack.Pop();
																																									break;
																																								} else {
																																									if (la.kind == 92) {
																																										currentState = stateStack.Pop();
																																										break;
																																									} else {
																																										if (la.kind == 93) {
																																											currentState = stateStack.Pop();
																																											break;
																																										} else {
																																											if (la.kind == 94) {
																																												currentState = stateStack.Pop();
																																												break;
																																											} else {
																																												if (la.kind == 95) {
																																													currentState = stateStack.Pop();
																																													break;
																																												} else {
																																													if (la.kind == 96) {
																																														currentState = stateStack.Pop();
																																														break;
																																													} else {
																																														if (la.kind == 97) {
																																															currentState = stateStack.Pop();
																																															break;
																																														} else {
																																															if (la.kind == 98) {
																																																goto case 90;
																																															} else {
																																																if (la.kind == 99) {
																																																	currentState = stateStack.Pop();
																																																	break;
																																																} else {
																																																	if (la.kind == 100) {
																																																		currentState = stateStack.Pop();
																																																		break;
																																																	} else {
																																																		if (la.kind == 101) {
																																																			currentState = stateStack.Pop();
																																																			break;
																																																		} else {
																																																			if (la.kind == 102) {
																																																				currentState = stateStack.Pop();
																																																				break;
																																																			} else {
																																																				if (la.kind == 103) {
																																																					currentState = stateStack.Pop();
																																																					break;
																																																				} else {
																																																					if (la.kind == 104) {
																																																						goto case 89;
																																																					} else {
																																																						if (la.kind == 105) {
																																																							currentState = stateStack.Pop();
																																																							break;
																																																						} else {
																																																							if (la.kind == 106) {
																																																								currentState = stateStack.Pop();
																																																								break;
																																																							} else {
																																																								if (la.kind == 107) {
																																																									goto case 88;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 87;
																																																									} else {
																																																										if (la.kind == 109) {
																																																											currentState = stateStack.Pop();
																																																											break;
																																																										} else {
																																																											if (la.kind == 110) {
																																																												currentState = stateStack.Pop();
																																																												break;
																																																											} else {
																																																												if (la.kind == 111) {
																																																													currentState = stateStack.Pop();
																																																													break;
																																																												} else {
																																																													if (la.kind == 112) {
																																																														currentState = stateStack.Pop();
																																																														break;
																																																													} else {
																																																														if (la.kind == 113) {
																																																															currentState = stateStack.Pop();
																																																															break;
																																																														} else {
																																																															if (la.kind == 114) {
																																																																currentState = stateStack.Pop();
																																																																break;
																																																															} else {
																																																																if (la.kind == 115) {
																																																																	currentState = stateStack.Pop();
																																																																	break;
																																																																} else {
																																																																	if (la.kind == 116) {
																																																																		goto case 86;
																																																																	} else {
																																																																		if (la.kind == 117) {
																																																																			currentState = stateStack.Pop();
																																																																			break;
																																																																		} else {
																																																																			if (la.kind == 118) {
																																																																				currentState = stateStack.Pop();
																																																																				break;
																																																																			} else {
																																																																				if (la.kind == 119) {
																																																																					currentState = stateStack.Pop();
																																																																					break;
																																																																				} else {
																																																																					if (la.kind == 120) {
																																																																						currentState = stateStack.Pop();
																																																																						break;
																																																																					} else {
																																																																						if (la.kind == 121) {
																																																																							goto case 85;
																																																																						} else {
																																																																							if (la.kind == 122) {
																																																																								currentState = stateStack.Pop();
																																																																								break;
																																																																							} else {
																																																																								if (la.kind == 123) {
																																																																									currentState = stateStack.Pop();
																																																																									break;
																																																																								} else {
																																																																									if (la.kind == 124) {
																																																																										goto case 84;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 83;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 82;
																																																																												} else {
																																																																													if (la.kind == 128) {
																																																																														currentState = stateStack.Pop();
																																																																														break;
																																																																													} else {
																																																																														if (la.kind == 129) {
																																																																															currentState = stateStack.Pop();
																																																																															break;
																																																																														} else {
																																																																															if (la.kind == 130) {
																																																																																currentState = stateStack.Pop();
																																																																																break;
																																																																															} else {
																																																																																if (la.kind == 131) {
																																																																																	currentState = stateStack.Pop();
																																																																																	break;
																																																																																} else {
																																																																																	if (la.kind == 132) {
																																																																																		currentState = stateStack.Pop();
																																																																																		break;
																																																																																	} else {
																																																																																		if (la.kind == 133) {
																																																																																			goto case 81;
																																																																																		} else {
																																																																																			if (la.kind == 134) {
																																																																																				currentState = stateStack.Pop();
																																																																																				break;
																																																																																			} else {
																																																																																				if (la.kind == 135) {
																																																																																					currentState = stateStack.Pop();
																																																																																					break;
																																																																																				} else {
																																																																																					if (la.kind == 136) {
																																																																																						currentState = stateStack.Pop();
																																																																																						break;
																																																																																					} else {
																																																																																						if (la.kind == 137) {
																																																																																							currentState = stateStack.Pop();
																																																																																							break;
																																																																																						} else {
																																																																																							if (la.kind == 138) {
																																																																																								currentState = stateStack.Pop();
																																																																																								break;
																																																																																							} else {
																																																																																								if (la.kind == 139) {
																																																																																									goto case 80;
																																																																																								} else {
																																																																																									if (la.kind == 140) {
																																																																																										currentState = stateStack.Pop();
																																																																																										break;
																																																																																									} else {
																																																																																										if (la.kind == 141) {
																																																																																											currentState = stateStack.Pop();
																																																																																											break;
																																																																																										} else {
																																																																																											if (la.kind == 142) {
																																																																																												currentState = stateStack.Pop();
																																																																																												break;
																																																																																											} else {
																																																																																												if (la.kind == 143) {
																																																																																													goto case 79;
																																																																																												} else {
																																																																																													if (la.kind == 144) {
																																																																																														currentState = stateStack.Pop();
																																																																																														break;
																																																																																													} else {
																																																																																														if (la.kind == 145) {
																																																																																															currentState = stateStack.Pop();
																																																																																															break;
																																																																																														} else {
																																																																																															if (la.kind == 146) {
																																																																																																goto case 78;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 77;
																																																																																																} else {
																																																																																																	if (la.kind == 148) {
																																																																																																		currentState = stateStack.Pop();
																																																																																																		break;
																																																																																																	} else {
																																																																																																		if (la.kind == 149) {
																																																																																																			currentState = stateStack.Pop();
																																																																																																			break;
																																																																																																		} else {
																																																																																																			if (la.kind == 150) {
																																																																																																				currentState = stateStack.Pop();
																																																																																																				break;
																																																																																																			} else {
																																																																																																				if (la.kind == 151) {
																																																																																																					currentState = stateStack.Pop();
																																																																																																					break;
																																																																																																				} else {
																																																																																																					if (la.kind == 152) {
																																																																																																						currentState = stateStack.Pop();
																																																																																																						break;
																																																																																																					} else {
																																																																																																						if (la.kind == 153) {
																																																																																																							currentState = stateStack.Pop();
																																																																																																							break;
																																																																																																						} else {
																																																																																																							if (la.kind == 154) {
																																																																																																								currentState = stateStack.Pop();
																																																																																																								break;
																																																																																																							} else {
																																																																																																								if (la.kind == 155) {
																																																																																																									currentState = stateStack.Pop();
																																																																																																									break;
																																																																																																								} else {
																																																																																																									if (la.kind == 156) {
																																																																																																										currentState = stateStack.Pop();
																																																																																																										break;
																																																																																																									} else {
																																																																																																										if (la.kind == 157) {
																																																																																																											currentState = stateStack.Pop();
																																																																																																											break;
																																																																																																										} else {
																																																																																																											if (la.kind == 158) {
																																																																																																												currentState = stateStack.Pop();
																																																																																																												break;
																																																																																																											} else {
																																																																																																												if (la.kind == 159) {
																																																																																																													currentState = stateStack.Pop();
																																																																																																													break;
																																																																																																												} else {
																																																																																																													if (la.kind == 160) {
																																																																																																														currentState = stateStack.Pop();
																																																																																																														break;
																																																																																																													} else {
																																																																																																														if (la.kind == 161) {
																																																																																																															currentState = stateStack.Pop();
																																																																																																															break;
																																																																																																														} else {
																																																																																																															if (la.kind == 162) {
																																																																																																																goto case 76;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 75;
																																																																																																																} else {
																																																																																																																	if (la.kind == 164) {
																																																																																																																		currentState = stateStack.Pop();
																																																																																																																		break;
																																																																																																																	} else {
																																																																																																																		if (la.kind == 165) {
																																																																																																																			currentState = stateStack.Pop();
																																																																																																																			break;
																																																																																																																		} else {
																																																																																																																			if (la.kind == 166) {
																																																																																																																				currentState = stateStack.Pop();
																																																																																																																				break;
																																																																																																																			} else {
																																																																																																																				if (la.kind == 167) {
																																																																																																																					currentState = stateStack.Pop();
																																																																																																																					break;
																																																																																																																				} else {
																																																																																																																					if (la.kind == 168) {
																																																																																																																						currentState = stateStack.Pop();
																																																																																																																						break;
																																																																																																																					} else {
																																																																																																																						if (la.kind == 169) {
																																																																																																																							currentState = stateStack.Pop();
																																																																																																																							break;
																																																																																																																						} else {
																																																																																																																							if (la.kind == 170) {
																																																																																																																								goto case 74;
																																																																																																																							} else {
																																																																																																																								if (la.kind == 171) {
																																																																																																																									currentState = stateStack.Pop();
																																																																																																																									break;
																																																																																																																								} else {
																																																																																																																									if (la.kind == 172) {
																																																																																																																										currentState = stateStack.Pop();
																																																																																																																										break;
																																																																																																																									} else {
																																																																																																																										if (la.kind == 173) {
																																																																																																																											currentState = stateStack.Pop();
																																																																																																																											break;
																																																																																																																										} else {
																																																																																																																											if (la.kind == 174) {
																																																																																																																												currentState = stateStack.Pop();
																																																																																																																												break;
																																																																																																																											} else {
																																																																																																																												if (la.kind == 175) {
																																																																																																																													currentState = stateStack.Pop();
																																																																																																																													break;
																																																																																																																												} else {
																																																																																																																													if (la.kind == 176) {
																																																																																																																														goto case 73;
																																																																																																																													} else {
																																																																																																																														if (la.kind == 177) {
																																																																																																																															currentState = stateStack.Pop();
																																																																																																																															break;
																																																																																																																														} else {
																																																																																																																															if (la.kind == 178) {
																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																break;
																																																																																																																															} else {
																																																																																																																																if (la.kind == 179) {
																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																	break;
																																																																																																																																} else {
																																																																																																																																	if (la.kind == 180) {
																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																		break;
																																																																																																																																	} else {
																																																																																																																																		if (la.kind == 181) {
																																																																																																																																			currentState = stateStack.Pop();
																																																																																																																																			break;
																																																																																																																																		} else {
																																																																																																																																			if (la.kind == 182) {
																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																				break;
																																																																																																																																			} else {
																																																																																																																																				if (la.kind == 183) {
																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																					break;
																																																																																																																																				} else {
																																																																																																																																					if (la.kind == 184) {
																																																																																																																																						goto case 72;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 71;
																																																																																																																																							} else {
																																																																																																																																								if (la.kind == 187) {
																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																									break;
																																																																																																																																								} else {
																																																																																																																																									if (la.kind == 188) {
																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																										break;
																																																																																																																																									} else {
																																																																																																																																										if (la.kind == 189) {
																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																											break;
																																																																																																																																										} else {
																																																																																																																																											if (la.kind == 190) {
																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																												break;
																																																																																																																																											} else {
																																																																																																																																												if (la.kind == 191) {
																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																													break;
																																																																																																																																												} else {
																																																																																																																																													if (la.kind == 192) {
																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																														break;
																																																																																																																																													} else {
																																																																																																																																														if (la.kind == 193) {
																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																															break;
																																																																																																																																														} else {
																																																																																																																																															if (la.kind == 194) {
																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																break;
																																																																																																																																															} else {
																																																																																																																																																if (la.kind == 195) {
																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																	break;
																																																																																																																																																} else {
																																																																																																																																																	if (la.kind == 196) {
																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																		break;
																																																																																																																																																	} else {
																																																																																																																																																		if (la.kind == 197) {
																																																																																																																																																			goto case 70;
																																																																																																																																																		} else {
																																																																																																																																																			if (la.kind == 198) {
																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																				break;
																																																																																																																																																			} else {
																																																																																																																																																				if (la.kind == 199) {
																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																					break;
																																																																																																																																																				} else {
																																																																																																																																																					if (la.kind == 200) {
																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																						break;
																																																																																																																																																					} else {
																																																																																																																																																						if (la.kind == 201) {
																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																							break;
																																																																																																																																																						} else {
																																																																																																																																																							if (la.kind == 202) {
																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																								break;
																																																																																																																																																							} else {
																																																																																																																																																								if (la.kind == 203) {
																																																																																																																																																									goto case 69;
																																																																																																																																																								} else {
																																																																																																																																																									if (la.kind == 204) {
																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																										break;
																																																																																																																																																									} else {
																																																																																																																																																										if (la.kind == 205) {
																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																											break;
																																																																																																																																																										} else {
																																																																																																																																																											if (la.kind == 206) {
																																																																																																																																																												goto case 68;
																																																																																																																																																											} else {
																																																																																																																																																												if (la.kind == 207) {
																																																																																																																																																													currentState = stateStack.Pop();
																																																																																																																																																													break;
																																																																																																																																																												} else {
																																																																																																																																																													if (la.kind == 208) {
																																																																																																																																																														currentState = stateStack.Pop();
																																																																																																																																																														break;
																																																																																																																																																													} else {
																																																																																																																																																														if (la.kind == 209) {
																																																																																																																																																															goto case 67;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 66;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 65;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 64;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 63;
																																																																																																																																																																		} else {
																																																																																																																																																																			if (la.kind == 214) {
																																																																																																																																																																				currentState = stateStack.Pop();
																																																																																																																																																																				break;
																																																																																																																																																																			} else {
																																																																																																																																																																				if (la.kind == 215) {
																																																																																																																																																																					currentState = stateStack.Pop();
																																																																																																																																																																					break;
																																																																																																																																																																				} else {
																																																																																																																																																																					if (la.kind == 216) {
																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																						break;
																																																																																																																																																																					} else {
																																																																																																																																																																						if (la.kind == 217) {
																																																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																																																							break;
																																																																																																																																																																						} else {
																																																																																																																																																																							if (la.kind == 218) {
																																																																																																																																																																								goto case 62;
																																																																																																																																																																							} else {
																																																																																																																																																																								if (la.kind == 219) {
																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																									break;
																																																																																																																																																																								} else {
																																																																																																																																																																									if (la.kind == 220) {
																																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																																										break;
																																																																																																																																																																									} else {
																																																																																																																																																																										if (la.kind == 221) {
																																																																																																																																																																											currentState = stateStack.Pop();
																																																																																																																																																																											break;
																																																																																																																																																																										} else {
																																																																																																																																																																											if (la.kind == 222) {
																																																																																																																																																																												currentState = stateStack.Pop();
																																																																																																																																																																												break;
																																																																																																																																																																											} else {
																																																																																																																																																																												if (la.kind == 223) {
																																																																																																																																																																													goto case 61;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 60;
																																																																																																																																																																													} else {
																																																																																																																																																																														if (la.kind == 225) {
																																																																																																																																																																															currentState = stateStack.Pop();
																																																																																																																																																																															break;
																																																																																																																																																																														} else {
																																																																																																																																																																															if (la.kind == 226) {
																																																																																																																																																																																currentState = stateStack.Pop();
																																																																																																																																																																																break;
																																																																																																																																																																															} else {
																																																																																																																																																																																if (la.kind == 227) {
																																																																																																																																																																																	currentState = stateStack.Pop();
																																																																																																																																																																																	break;
																																																																																																																																																																																} else {
																																																																																																																																																																																	if (la.kind == 228) {
																																																																																																																																																																																		currentState = stateStack.Pop();
																																																																																																																																																																																		break;
																																																																																																																																																																																	} else {
																																																																																																																																																																																		if (la.kind == 229) {
																																																																																																																																																																																			currentState = stateStack.Pop();
																																																																																																																																																																																			break;
																																																																																																																																																																																		} else {
																																																																																																																																																																																			if (la.kind == 230) {
																																																																																																																																																																																				goto case 59;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 58;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 57;
																																																																																																																																																																																						} else {
																																																																																																																																																																																							if (la.kind == 234) {
																																																																																																																																																																																								currentState = stateStack.Pop();
																																																																																																																																																																																								break;
																																																																																																																																																																																							} else {
																																																																																																																																																																																								if (la.kind == 235) {
																																																																																																																																																																																									currentState = stateStack.Pop();
																																																																																																																																																																																									break;
																																																																																																																																																																																								} else {
																																																																																																																																																																																									if (la.kind == 236) {
																																																																																																																																																																																										currentState = stateStack.Pop();
																																																																																																																																																																																										break;
																																																																																																																																																																																									} else {
																																																																																																																																																																																										if (la.kind == 237) {
																																																																																																																																																																																											currentState = stateStack.Pop();
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
			case 57: {
				if (la == null) { currentState = 57; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 77: {
				if (la == null) { currentState = 77; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 79: {
				if (la == null) { currentState = 79; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 82: {
				if (la == null) { currentState = 82; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 83: {
				if (la == null) { currentState = 83; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 84: {
				if (la == null) { currentState = 84; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 85: {
				if (la == null) { currentState = 85; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 86: {
				if (la == null) { currentState = 86; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 87: {
				if (la == null) { currentState = 87; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 88: {
				if (la == null) { currentState = 88; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 89: {
				if (la == null) { currentState = 89; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 90: {
				if (la == null) { currentState = 90; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 91: {
				if (la == null) { currentState = 91; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 92: {
				if (la == null) { currentState = 92; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 93: {
				if (la == null) { currentState = 93; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 94: {
				if (la == null) { currentState = 94; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 95: {
				if (la == null) { currentState = 95; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 96: {
				if (la == null) { currentState = 96; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 97: {
				if (la == null) { currentState = 97; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 98: {
				if (la == null) { currentState = 98; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 99: {
				if (la == null) { currentState = 99; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 100: {
				if (la == null) { currentState = 100; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 102;
						break;
					} else {
						goto case 48;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				if (la.kind == 35) {
					currentState = 43;
					break;
				} else {
					if (set[19].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 6;
					}
				}
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 104: {
				stateStack.Push(105);
				goto case 38;
			}
			case 105: {
				if (la == null) { currentState = 105; break; }
				Expect(144, la); // "Is"
				currentState = 106;
				break;
			}
			case 106: {
				stateStack.Push(41);
				goto case 19;
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(107);
					goto case 108;
				} else {
					goto case 41;
				}
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				if (la.kind == 37) {
					currentState = 113;
					break;
				} else {
					if (set[103].Get(la.kind)) {
						currentState = 109;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 109: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 110;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				if (la.kind == 10) {
					currentState = 111;
					break;
				} else {
					goto case 111;
				}
			}
			case 111: {
				stateStack.Push(112);
				goto case 56;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 113: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 114;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				if (la.kind == 169) {
					currentState = 115;
					break;
				} else {
					if (set[13].Get(la.kind)) {
						goto case 28;
					} else {
						goto case 6;
					}
				}
			}
			case 115: {
				stateStack.Push(116);
				goto case 19;
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				if (la.kind == 22) {
					currentState = 115;
					break;
				} else {
					goto case 27;
				}
			}
			case 117: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 118;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				if (set[104].Get(la.kind)) {
					currentState = 119;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 375;
						break;
					} else {
						if (set[105].Get(la.kind)) {
							currentState = 119;
							break;
						} else {
							if (set[103].Get(la.kind)) {
								currentState = 371;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 369;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 366;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(119);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 355;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(119);
												goto case 191;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(119);
													PushContext(Context.Query, la, t);
													goto case 133;
												} else {
													if (set[26].Get(la.kind)) {
														stateStack.Push(119);
														goto case 127;
													} else {
														if (la.kind == 135) {
															stateStack.Push(119);
															goto case 120;
														} else {
															Error(la);
															goto case 119;
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
			case 119: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				Expect(135, la); // "If"
				currentState = 121;
				break;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				Expect(37, la); // "("
				currentState = 122;
				break;
			}
			case 122: {
				stateStack.Push(123);
				goto case 35;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				Expect(22, la); // ","
				currentState = 124;
				break;
			}
			case 124: {
				stateStack.Push(125);
				goto case 35;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				if (la.kind == 22) {
					currentState = 126;
					break;
				} else {
					goto case 27;
				}
			}
			case 126: {
				stateStack.Push(27);
				goto case 35;
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (set[106].Get(la.kind)) {
					currentState = 132;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 128;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				Expect(37, la); // "("
				currentState = 129;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 35;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				Expect(22, la); // ","
				currentState = 131;
				break;
			}
			case 131: {
				stateStack.Push(27);
				goto case 19;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				Expect(37, la); // "("
				currentState = 126;
				break;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (la.kind == 126) {
					stateStack.Push(134);
					goto case 190;
				} else {
					if (la.kind == 58) {
						stateStack.Push(134);
						goto case 189;
					} else {
						Error(la);
						goto case 134;
					}
				}
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(134);
					goto case 135;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (la.kind == 126) {
					currentState = 187;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 183;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 181;
							break;
						} else {
							if (la.kind == 107) {
								goto case 88;
							} else {
								if (la.kind == 230) {
									currentState = 35;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 177;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 175;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 173;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 148;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 136;
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
			case 136: {
				stateStack.Push(137);
				goto case 142;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				Expect(171, la); // "On"
				currentState = 138;
				break;
			}
			case 138: {
				stateStack.Push(139);
				goto case 35;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				Expect(116, la); // "Equals"
				currentState = 140;
				break;
			}
			case 140: {
				stateStack.Push(141);
				goto case 35;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				if (la.kind == 22) {
					currentState = 138;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 142: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(143);
				goto case 147;
			}
			case 143: {
				PopContext();
				goto case 144;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (la.kind == 63) {
					currentState = 146;
					break;
				} else {
					goto case 145;
				}
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				Expect(138, la); // "In"
				currentState = 35;
				break;
			}
			case 146: {
				stateStack.Push(145);
				goto case 19;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				if (set[88].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 90;
					} else {
						goto case 6;
					}
				}
			}
			case 148: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 149;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				if (la.kind == 146) {
					goto case 165;
				} else {
					if (set[30].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 151;
							break;
						} else {
							if (set[30].Get(la.kind)) {
								goto case 163;
							} else {
								Error(la);
								goto case 150;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				Expect(70, la); // "By"
				currentState = 151;
				break;
			}
			case 151: {
				stateStack.Push(152);
				goto case 155;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (la.kind == 22) {
					currentState = 151;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 153;
					break;
				}
			}
			case 153: {
				stateStack.Push(154);
				goto case 155;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (la.kind == 22) {
					currentState = 153;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 155: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 156;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (set[28].Get(la.kind)) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(157);
					goto case 147;
				} else {
					goto case 35;
				}
			}
			case 157: {
				PopContext();
				goto case 158;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				if (la.kind == 63) {
					currentState = 161;
					break;
				} else {
					if (la.kind == 20) {
						goto case 160;
					} else {
						if (set[31].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 35;
						}
					}
				}
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				currentState = 35;
				break;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				currentState = 35;
				break;
			}
			case 161: {
				stateStack.Push(162);
				goto case 19;
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				Expect(20, la); // "="
				currentState = 35;
				break;
			}
			case 163: {
				stateStack.Push(164);
				goto case 155;
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				if (la.kind == 22) {
					currentState = 163;
					break;
				} else {
					goto case 150;
				}
			}
			case 165: {
				stateStack.Push(166);
				goto case 172;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 170;
						break;
					} else {
						if (la.kind == 146) {
							goto case 165;
						} else {
							Error(la);
							goto case 166;
						}
					}
				} else {
					goto case 167;
				}
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				Expect(143, la); // "Into"
				currentState = 168;
				break;
			}
			case 168: {
				stateStack.Push(169);
				goto case 155;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (la.kind == 22) {
					currentState = 168;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 170: {
				stateStack.Push(171);
				goto case 172;
			}
			case 171: {
				stateStack.Push(166);
				goto case 167;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				Expect(146, la); // "Join"
				currentState = 136;
				break;
			}
			case 173: {
				stateStack.Push(174);
				goto case 155;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.kind == 22) {
					currentState = 173;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 175: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 176;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 231) {
					currentState = 35;
					break;
				} else {
					goto case 35;
				}
			}
			case 177: {
				if (la == null) { currentState = 177; break; }
				Expect(70, la); // "By"
				currentState = 178;
				break;
			}
			case 178: {
				stateStack.Push(179);
				goto case 35;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				if (la.kind == 64) {
					currentState = 180;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 180;
						break;
					} else {
						Error(la);
						goto case 180;
					}
				}
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				if (la.kind == 22) {
					currentState = 178;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 181: {
				stateStack.Push(182);
				goto case 155;
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				if (la.kind == 22) {
					currentState = 181;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 183: {
				stateStack.Push(184);
				goto case 142;
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(184);
					goto case 135;
				} else {
					Expect(143, la); // "Into"
					currentState = 185;
					break;
				}
			}
			case 185: {
				stateStack.Push(186);
				goto case 155;
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				if (la.kind == 22) {
					currentState = 185;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 187: {
				stateStack.Push(188);
				goto case 142;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 22) {
					currentState = 187;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				Expect(58, la); // "Aggregate"
				currentState = 183;
				break;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				Expect(126, la); // "From"
				currentState = 187;
				break;
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				if (la.kind == 210) {
					currentState = 348;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 192;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				Expect(37, la); // "("
				currentState = 193;
				break;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(194);
					goto case 335;
				} else {
					goto case 194;
				}
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				Expect(38, la); // ")"
				currentState = 195;
				break;
			}
			case 195: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 196;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (set[15].Get(la.kind)) {
					goto case 35;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 334;
							break;
						} else {
							goto case 197;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 197: {
				stateStack.Push(198);
				goto case 200;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(113, la); // "End"
				currentState = 199;
				break;
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 200: {
				PushContext(Context.Body, la, t);
				goto case 201;
			}
			case 201: {
				stateStack.Push(202);
				goto case 15;
			}
			case 202: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 203;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (set[107].Get(la.kind)) {
					if (set[57].Get(la.kind)) {
						if (set[39].Get(la.kind)) {
							stateStack.Push(201);
							goto case 208;
						} else {
							goto case 201;
						}
					} else {
						if (la.kind == 113) {
							currentState = 206;
							break;
						} else {
							goto case 205;
						}
					}
				} else {
					goto case 204;
				}
			}
			case 204: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 205: {
				Error(la);
				goto case 202;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 201;
				} else {
					if (set[38].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 205;
					}
				}
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				currentState = 202;
				break;
			}
			case 208: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 209;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 319;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 315;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 313;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 311;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 292;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 277;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 273;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 267;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 242;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 238;
																break;
															} else {
																goto case 238;
															}
														} else {
															if (la.kind == 194) {
																currentState = 236;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 218;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 225;
																break;
															} else {
																if (set[108].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 222;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 221;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 220;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 68;
																				} else {
																					if (la.kind == 195) {
																						currentState = 218;
																						break;
																					} else {
																						goto case 6;
																					}
																				}
																			}
																		}
																	}
																} else {
																	if (la.kind == 191) {
																		currentState = 216;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 214;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 210;
																				break;
																			} else {
																				if (set[109].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 35;
																						break;
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
			case 210: {
				stateStack.Push(211);
				goto case 35;
			}
			case 211: {
				stateStack.Push(212);
				goto case 200;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				Expect(113, la); // "End"
				currentState = 213;
				break;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 214: {
				stateStack.Push(215);
				goto case 35;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (la.kind == 22) {
					currentState = 214;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 216: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 217;
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				if (la.kind == 184) {
					currentState = 35;
					break;
				} else {
					goto case 35;
				}
			}
			case 218: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 219;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (set[15].Get(la.kind)) {
					goto case 35;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				if (la.kind == 108) {
					goto case 87;
				} else {
					if (la.kind == 124) {
						goto case 84;
					} else {
						if (la.kind == 231) {
							goto case 58;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (la.kind == 108) {
					goto case 87;
				} else {
					if (la.kind == 124) {
						goto case 84;
					} else {
						if (la.kind == 231) {
							goto case 58;
						} else {
							if (la.kind == 197) {
								goto case 70;
							} else {
								if (la.kind == 210) {
									goto case 66;
								} else {
									if (la.kind == 127) {
										goto case 82;
									} else {
										if (la.kind == 186) {
											goto case 71;
										} else {
											if (la.kind == 218) {
												goto case 62;
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
			case 222: {
				if (la == null) { currentState = 222; break; }
				if (set[28].Get(la.kind)) {
					goto case 224;
				} else {
					if (la.kind == 5) {
						goto case 223;
					} else {
						goto case 6;
					}
				}
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 225: {
				stateStack.Push(226);
				goto case 200;
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (la.kind == 75) {
					currentState = 230;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 229;
						break;
					} else {
						goto case 227;
					}
				}
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				Expect(113, la); // "End"
				currentState = 228;
				break;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 229: {
				stateStack.Push(227);
				goto case 200;
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (set[28].Get(la.kind)) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(233);
					goto case 147;
				} else {
					goto case 231;
				}
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 229) {
					currentState = 232;
					break;
				} else {
					goto case 225;
				}
			}
			case 232: {
				stateStack.Push(225);
				goto case 35;
			}
			case 233: {
				PopContext();
				goto case 234;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				if (la.kind == 63) {
					currentState = 235;
					break;
				} else {
					goto case 231;
				}
			}
			case 235: {
				stateStack.Push(231);
				goto case 19;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (la.kind == 163) {
					goto case 75;
				} else {
					goto case 237;
				}
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (la.kind == 5) {
					goto case 223;
				} else {
					if (set[28].Get(la.kind)) {
						goto case 224;
					} else {
						goto case 6;
					}
				}
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				Expect(118, la); // "Error"
				currentState = 239;
				break;
			}
			case 239: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 240;
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (set[15].Get(la.kind)) {
					goto case 35;
				} else {
					if (la.kind == 132) {
						currentState = 237;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 241;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 242: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 243;
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (set[24].Get(la.kind)) {
					stateStack.Push(257);
					goto case 253;
				} else {
					if (la.kind == 110) {
						currentState = 244;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 244: {
				stateStack.Push(245);
				goto case 253;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				Expect(138, la); // "In"
				currentState = 246;
				break;
			}
			case 246: {
				stateStack.Push(247);
				goto case 35;
			}
			case 247: {
				stateStack.Push(248);
				goto case 200;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				Expect(163, la); // "Next"
				currentState = 249;
				break;
			}
			case 249: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 250;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				if (set[15].Get(la.kind)) {
					goto case 251;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 251: {
				stateStack.Push(252);
				goto case 35;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (la.kind == 22) {
					currentState = 251;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 253: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(254);
				goto case 117;
			}
			case 254: {
				PopContext();
				goto case 255;
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 33) {
					currentState = 256;
					break;
				} else {
					goto case 256;
				}
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(256);
					goto case 108;
				} else {
					if (la.kind == 63) {
						currentState = 19;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				Expect(20, la); // "="
				currentState = 258;
				break;
			}
			case 258: {
				stateStack.Push(259);
				goto case 35;
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				if (la.kind == 205) {
					currentState = 266;
					break;
				} else {
					goto case 260;
				}
			}
			case 260: {
				stateStack.Push(261);
				goto case 200;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				Expect(163, la); // "Next"
				currentState = 262;
				break;
			}
			case 262: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 263;
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (set[15].Get(la.kind)) {
					goto case 264;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 264: {
				stateStack.Push(265);
				goto case 35;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (la.kind == 22) {
					currentState = 264;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 266: {
				stateStack.Push(260);
				goto case 35;
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 270;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(268);
						goto case 200;
					} else {
						goto case 6;
					}
				}
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				Expect(152, la); // "Loop"
				currentState = 269;
				break;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 35;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 270: {
				stateStack.Push(271);
				goto case 35;
			}
			case 271: {
				stateStack.Push(272);
				goto case 200;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 273: {
				stateStack.Push(274);
				goto case 35;
			}
			case 274: {
				stateStack.Push(275);
				goto case 200;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(113, la); // "End"
				currentState = 276;
				break;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 277: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 278;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (la.kind == 74) {
					currentState = 279;
					break;
				} else {
					goto case 279;
				}
			}
			case 279: {
				stateStack.Push(280);
				goto case 35;
			}
			case 280: {
				stateStack.Push(281);
				goto case 15;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 74) {
					currentState = 283;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 282;
					break;
				}
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 283: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 284;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (la.kind == 111) {
					currentState = 285;
					break;
				} else {
					if (set[55].Get(la.kind)) {
						goto case 286;
					} else {
						Error(la);
						goto case 285;
					}
				}
			}
			case 285: {
				stateStack.Push(281);
				goto case 200;
			}
			case 286: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 287;
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (set[110].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 289;
						break;
					} else {
						goto case 289;
					}
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(288);
						goto case 35;
					} else {
						Error(la);
						goto case 288;
					}
				}
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (la.kind == 22) {
					currentState = 286;
					break;
				} else {
					goto case 285;
				}
			}
			case 289: {
				stateStack.Push(290);
				goto case 291;
			}
			case 290: {
				stateStack.Push(288);
				goto case 38;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				if (la.kind == 20) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 41) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 40) {
							currentState = stateStack.Pop();
							break;
						} else {
							if (la.kind == 39) {
								currentState = stateStack.Pop();
								break;
							} else {
								if (la.kind == 42) {
									currentState = stateStack.Pop();
									break;
								} else {
									if (la.kind == 43) {
										currentState = stateStack.Pop();
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
			case 292: {
				stateStack.Push(293);
				goto case 35;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 214) {
					currentState = 301;
					break;
				} else {
					goto case 294;
				}
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 295;
				} else {
					goto case 6;
				}
			}
			case 295: {
				stateStack.Push(296);
				goto case 200;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 300;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 298;
							break;
						} else {
							Error(la);
							goto case 295;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 297;
					break;
				}
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 298: {
				stateStack.Push(299);
				goto case 35;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 214) {
					currentState = 295;
					break;
				} else {
					goto case 295;
				}
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (la.kind == 135) {
					currentState = 298;
					break;
				} else {
					goto case 295;
				}
			}
			case 301: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 302;
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				if (set[39].Get(la.kind)) {
					goto case 303;
				} else {
					goto case 294;
				}
			}
			case 303: {
				stateStack.Push(304);
				goto case 208;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				if (la.kind == 21) {
					currentState = 309;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 306;
						break;
					} else {
						goto case 305;
					}
				}
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 306: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 307;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				if (set[39].Get(la.kind)) {
					stateStack.Push(308);
					goto case 208;
				} else {
					goto case 308;
				}
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				if (la.kind == 21) {
					currentState = 306;
					break;
				} else {
					goto case 305;
				}
			}
			case 309: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 310;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (set[39].Get(la.kind)) {
					goto case 303;
				} else {
					goto case 304;
				}
			}
			case 311: {
				stateStack.Push(312);
				goto case 56;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 37) {
					currentState = 28;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 313: {
				stateStack.Push(314);
				goto case 35;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				Expect(22, la); // ","
				currentState = 35;
				break;
			}
			case 315: {
				stateStack.Push(316);
				goto case 35;
			}
			case 316: {
				stateStack.Push(317);
				goto case 200;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				Expect(113, la); // "End"
				currentState = 318;
				break;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (la.kind == 233) {
					goto case 57;
				} else {
					if (la.kind == 211) {
						goto case 65;
					} else {
						goto case 6;
					}
				}
			}
			case 319: {
				PushContext(Context.IdentifierExpected, la, t);	
				stateStack.Push(320);
				goto case 147;
			}
			case 320: {
				PopContext();
				goto case 321;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (la.kind == 33) {
					currentState = 322;
					break;
				} else {
					goto case 322;
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 37) {
					currentState = 333;
					break;
				} else {
					goto case 323;
				}
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 22) {
					currentState = 327;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 325;
						break;
					} else {
						goto case 324;
					}
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (la.kind == 20) {
					goto case 160;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 162) {
					currentState = 326;
					break;
				} else {
					goto case 326;
				}
			}
			case 326: {
				stateStack.Push(324);
				goto case 19;
			}
			case 327: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(328);
				goto case 147;
			}
			case 328: {
				PopContext();
				goto case 329;
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 33) {
					currentState = 330;
					break;
				} else {
					goto case 330;
				}
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				if (la.kind == 37) {
					currentState = 331;
					break;
				} else {
					goto case 323;
				}
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (la.kind == 22) {
					currentState = 331;
					break;
				} else {
					goto case 332;
				}
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				Expect(38, la); // ")"
				currentState = 323;
				break;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 22) {
					currentState = 333;
					break;
				} else {
					goto case 332;
				}
			}
			case 334: {
				stateStack.Push(197);
				goto case 19;
			}
			case 335: {
				stateStack.Push(336);
				goto case 337;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 22) {
					currentState = 335;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (la.kind == 40) {
					stateStack.Push(337);
					goto case 343;
				} else {
					goto case 338;
				}
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (set[111].Get(la.kind)) {
					currentState = 338;
					break;
				} else {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(339);
					goto case 147;
				}
			}
			case 339: {
				PopContext();
				goto case 340;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 63) {
					currentState = 342;
					break;
				} else {
					goto case 341;
				}
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 20) {
					goto case 160;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 342: {
				stateStack.Push(341);
				goto case 19;
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				Expect(40, la); // "<"
				currentState = 344;
				break;
			}
			case 344: {
				PushContext(Context.Attribute, la, t);
				goto case 345;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (set[112].Get(la.kind)) {
					currentState = 345;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 346;
					break;
				}
			}
			case 346: {
				PopContext();
				goto case 347;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 1) {
					goto case 17;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				Expect(37, la); // "("
				currentState = 349;
				break;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(350);
					goto case 335;
				} else {
					goto case 350;
				}
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				Expect(38, la); // ")"
				currentState = 351;
				break;
			}
			case 351: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 352;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (set[39].Get(la.kind)) {
					goto case 208;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(353);
						goto case 200;
					} else {
						goto case 6;
					}
				}
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				Expect(113, la); // "End"
				currentState = 354;
				break;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 365;
					break;
				} else {
					stateStack.Push(356);
					goto case 358;
				}
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 17) {
					currentState = 357;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 16) {
					currentState = 356;
					break;
				} else {
					goto case 356;
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 359;
				break;
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (set[113].Get(la.kind)) {
					if (set[114].Get(la.kind)) {
						currentState = 359;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(359);
							goto case 362;
						} else {
							Error(la);
							goto case 359;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 360;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (set[115].Get(la.kind)) {
					if (set[116].Get(la.kind)) {
						currentState = 360;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(360);
							goto case 362;
						} else {
							if (la.kind == 10) {
								stateStack.Push(360);
								goto case 358;
							} else {
								Error(la);
								goto case 360;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 361;
					break;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (set[117].Get(la.kind)) {
					if (set[118].Get(la.kind)) {
						currentState = 361;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(361);
							goto case 362;
						} else {
							Error(la);
							goto case 361;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 363;
				break;
			}
			case 363: {
				stateStack.Push(364);
				goto case 35;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 16) {
					currentState = 355;
					break;
				} else {
					goto case 355;
				}
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				Expect(37, la); // "("
				currentState = 367;
				break;
			}
			case 367: {
				readXmlIdentifier = true;
				stateStack.Push(368);
				goto case 147;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				Expect(38, la); // ")"
				currentState = 119;
				break;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				Expect(37, la); // "("
				currentState = 370;
				break;
			}
			case 370: {
				stateStack.Push(368);
				goto case 19;
			}
			case 371: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 372;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (la.kind == 10) {
					currentState = 373;
					break;
				} else {
					goto case 373;
				}
			}
			case 373: {
				stateStack.Push(374);
				goto case 56;
			}
			case 374: {
				if (la == null) { currentState = 374; break; }
				if (la.kind == 11) {
					currentState = 119;
					break;
				} else {
					goto case 119;
				}
			}
			case 375: {
				stateStack.Push(368);
				goto case 35;
			}
			case 376: {
				stateStack.Push(377);
				goto case 35;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				if (la.kind == 22) {
					currentState = 378;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 378: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 379;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (set[15].Get(la.kind)) {
					goto case 376;
				} else {
					goto case 377;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (set[8].Get(la.kind)) {
					stateStack.Push(381);
					goto case 19;
				} else {
					goto case 381;
				}
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (la.kind == 22) {
					currentState = 380;
					break;
				} else {
					goto case 27;
				}
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(383);
					goto case 335;
				} else {
					goto case 383;
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 384: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 385;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				currentState = 386;
				break;
			}
			case 386: {
				PopContext();
				goto case 387;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (la.kind == 37) {
					currentState = 465;
					break;
				} else {
					goto case 388;
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (set[119].Get(la.kind)) {
					currentState = 388;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(389);
						goto case 15;
					} else {
						goto case 389;
					}
				}
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (la.kind == 140) {
					currentState = 464;
					break;
				} else {
					goto case 390;
				}
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (la.kind == 136) {
					currentState = 463;
					break;
				} else {
					goto case 391;
				}
			}
			case 391: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 392;
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				if (set[79].Get(la.kind)) {
					stateStack.Push(392);
					PushContext(Context.Member, la, t);
					goto case 396;
				} else {
					Expect(113, la); // "End"
					currentState = 393;
					break;
				}
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 155) {
					currentState = 394;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 394;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 394;
							break;
						} else {
							Error(la);
							goto case 394;
						}
					}
				}
			}
			case 394: {
				stateStack.Push(395);
				goto case 15;
			}
			case 395: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (la.kind == 40) {
					stateStack.Push(396);
					goto case 343;
				} else {
					goto case 397;
				}
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (set[120].Get(la.kind)) {
					currentState = 397;
					break;
				} else {
					if (set[87].Get(la.kind)) {
						stateStack.Push(398);
						goto case 455;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							stateStack.Push(398);
							goto case 442;
						} else {
							if (la.kind == 101) {
								stateStack.Push(398);
								goto case 433;
							} else {
								if (la.kind == 119) {
									stateStack.Push(398);
									goto case 423;
								} else {
									if (la.kind == 98) {
										stateStack.Push(398);
										goto case 411;
									} else {
										if (la.kind == 172) {
											stateStack.Push(398);
											goto case 399;
										} else {
											Error(la);
											goto case 398;
										}
									}
								}
							}
						}
					}
				}
			}
			case 398: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				Expect(172, la); // "Operator"
				currentState = 400;
				break;
			}
			case 400: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 401;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				currentState = 402;
				break;
			}
			case 402: {
				PopContext();
				goto case 403;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				Expect(37, la); // "("
				currentState = 404;
				break;
			}
			case 404: {
				stateStack.Push(405);
				goto case 335;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				Expect(38, la); // ")"
				currentState = 406;
				break;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 63) {
					currentState = 410;
					break;
				} else {
					goto case 407;
				}
			}
			case 407: {
				stateStack.Push(408);
				goto case 200;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(113, la); // "End"
				currentState = 409;
				break;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				Expect(172, la); // "Operator"
				currentState = 15;
				break;
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				if (la.kind == 40) {
					stateStack.Push(410);
					goto case 343;
				} else {
					stateStack.Push(407);
					goto case 19;
				}
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				Expect(98, la); // "Custom"
				currentState = 412;
				break;
			}
			case 412: {
				stateStack.Push(413);
				goto case 423;
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				if (set[83].Get(la.kind)) {
					goto case 415;
				} else {
					Expect(113, la); // "End"
					currentState = 414;
					break;
				}
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				Expect(119, la); // "Event"
				currentState = 15;
				break;
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				if (la.kind == 40) {
					stateStack.Push(415);
					goto case 343;
				} else {
					if (la.kind == 56) {
						currentState = 416;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 416;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 416;
								break;
							} else {
								Error(la);
								goto case 416;
							}
						}
					}
				}
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				Expect(37, la); // "("
				currentState = 417;
				break;
			}
			case 417: {
				stateStack.Push(418);
				goto case 335;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				Expect(38, la); // ")"
				currentState = 419;
				break;
			}
			case 419: {
				stateStack.Push(420);
				goto case 200;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				Expect(113, la); // "End"
				currentState = 421;
				break;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (la.kind == 56) {
					currentState = 422;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 422;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 422;
							break;
						} else {
							Error(la);
							goto case 422;
						}
					}
				}
			}
			case 422: {
				stateStack.Push(413);
				goto case 15;
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				Expect(119, la); // "Event"
				currentState = 424;
				break;
			}
			case 424: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(425);
				goto case 147;
			}
			case 425: {
				PopContext();
				goto case 426;
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (la.kind == 63) {
					currentState = 432;
					break;
				} else {
					if (set[121].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 430;
							break;
						} else {
							goto case 427;
						}
					} else {
						Error(la);
						goto case 427;
					}
				}
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (la.kind == 136) {
					currentState = 428;
					break;
				} else {
					goto case 15;
				}
			}
			case 428: {
				stateStack.Push(429);
				goto case 19;
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (la.kind == 22) {
					currentState = 428;
					break;
				} else {
					goto case 15;
				}
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(431);
					goto case 335;
				} else {
					goto case 431;
				}
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				Expect(38, la); // ")"
				currentState = 427;
				break;
			}
			case 432: {
				stateStack.Push(427);
				goto case 19;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				Expect(101, la); // "Declare"
				currentState = 434;
				break;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 435;
					break;
				} else {
					goto case 435;
				}
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (la.kind == 210) {
					currentState = 436;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 436;
						break;
					} else {
						Error(la);
						goto case 436;
					}
				}
			}
			case 436: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(437);
				goto case 147;
			}
			case 437: {
				PopContext();
				goto case 438;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				Expect(149, la); // "Lib"
				currentState = 439;
				break;
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				Expect(3, la); // LiteralString
				currentState = 440;
				break;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (la.kind == 59) {
					currentState = 441;
					break;
				} else {
					goto case 13;
				}
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				Expect(3, la); // LiteralString
				currentState = 13;
				break;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (la.kind == 210) {
					currentState = 443;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 443;
						break;
					} else {
						Error(la);
						goto case 443;
					}
				}
			}
			case 443: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 444;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				currentState = 445;
				break;
			}
			case 445: {
				PopContext();
				goto case 446;
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (la.kind == 37) {
					currentState = 453;
					break;
				} else {
					goto case 447;
				}
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				if (la.kind == 63) {
					currentState = 452;
					break;
				} else {
					goto case 448;
				}
			}
			case 448: {
				stateStack.Push(449);
				goto case 200;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				Expect(113, la); // "End"
				currentState = 450;
				break;
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				if (la.kind == 210) {
					currentState = 15;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 15;
						break;
					} else {
						goto case 451;
					}
				}
			}
			case 451: {
				Error(la);
				goto case 15;
			}
			case 452: {
				stateStack.Push(448);
				goto case 19;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(454);
					goto case 335;
				} else {
					goto case 454;
				}
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				Expect(38, la); // ")"
				currentState = 447;
				break;
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				if (la.kind == 88) {
					currentState = 456;
					break;
				} else {
					goto case 456;
				}
			}
			case 456: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(457);
				goto case 462;
			}
			case 457: {
				PopContext();
				goto case 458;
			}
			case 458: {
				if (la == null) { currentState = 458; break; }
				if (la.kind == 63) {
					currentState = 461;
					break;
				} else {
					goto case 459;
				}
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				if (la.kind == 20) {
					currentState = 460;
					break;
				} else {
					goto case 15;
				}
			}
			case 460: {
				stateStack.Push(15);
				goto case 35;
			}
			case 461: {
				stateStack.Push(459);
				goto case 19;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (set[105].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 99;
					} else {
						if (la.kind == 126) {
							goto case 83;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (set[38].Get(la.kind)) {
					currentState = 463;
					break;
				} else {
					stateStack.Push(391);
					goto case 15;
				}
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				if (set[38].Get(la.kind)) {
					currentState = 464;
					break;
				} else {
					stateStack.Push(390);
					goto case 15;
				}
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				Expect(169, la); // "Of"
				currentState = 466;
				break;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 467;
					break;
				} else {
					goto case 467;
				}
			}
			case 467: {
				stateStack.Push(468);
				goto case 475;
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				if (la.kind == 63) {
					currentState = 470;
					break;
				} else {
					goto case 469;
				}
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (la.kind == 22) {
					currentState = 466;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 388;
					break;
				}
			}
			case 470: {
				stateStack.Push(469);
				goto case 471;
			}
			case 471: {
				if (la == null) { currentState = 471; break; }
				if (set[93].Get(la.kind)) {
					goto case 474;
				} else {
					if (la.kind == 35) {
						currentState = 472;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 472: {
				stateStack.Push(473);
				goto case 474;
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				if (la.kind == 22) {
					currentState = 472;
					break;
				} else {
					goto case 45;
				}
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				if (set[8].Get(la.kind)) {
					currentState = 20;
					break;
				} else {
					if (la.kind == 162) {
						goto case 76;
					} else {
						if (la.kind == 84) {
							goto case 92;
						} else {
							if (la.kind == 209) {
								goto case 67;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (la.kind == 2) {
					goto case 100;
				} else {
					if (la.kind == 62) {
						goto case 98;
					} else {
						if (la.kind == 64) {
							goto case 97;
						} else {
							if (la.kind == 65) {
								goto case 96;
							} else {
								if (la.kind == 66) {
									goto case 95;
								} else {
									if (la.kind == 67) {
										goto case 94;
									} else {
										if (la.kind == 70) {
											goto case 93;
										} else {
											if (la.kind == 87) {
												goto case 91;
											} else {
												if (la.kind == 104) {
													goto case 89;
												} else {
													if (la.kind == 107) {
														goto case 88;
													} else {
														if (la.kind == 116) {
															goto case 86;
														} else {
															if (la.kind == 121) {
																goto case 85;
															} else {
																if (la.kind == 133) {
																	goto case 81;
																} else {
																	if (la.kind == 139) {
																		goto case 80;
																	} else {
																		if (la.kind == 143) {
																			goto case 79;
																		} else {
																			if (la.kind == 146) {
																				goto case 78;
																			} else {
																				if (la.kind == 147) {
																					goto case 77;
																				} else {
																					if (la.kind == 170) {
																						goto case 74;
																					} else {
																						if (la.kind == 176) {
																							goto case 73;
																						} else {
																							if (la.kind == 184) {
																								goto case 72;
																							} else {
																								if (la.kind == 203) {
																									goto case 69;
																								} else {
																									if (la.kind == 212) {
																										goto case 64;
																									} else {
																										if (la.kind == 213) {
																											goto case 63;
																										} else {
																											if (la.kind == 223) {
																												goto case 61;
																											} else {
																												if (la.kind == 224) {
																													goto case 60;
																												} else {
																													if (la.kind == 230) {
																														goto case 59;
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
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 476: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 477;
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				if (set[38].Get(la.kind)) {
					currentState = 477;
					break;
				} else {
					PopContext();
					stateStack.Push(478);
					goto case 15;
				}
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(478);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 479;
					break;
				}
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				Expect(160, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 480: {
				if (la == null) { currentState = 480; break; }
				Expect(137, la); // "Imports"
				currentState = 481;
				break;
			}
			case 481: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
					
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 482;
			}
			case 482: {
				if (la == null) { currentState = 482; break; }
				if (set[8].Get(la.kind)) {
					currentState = 488;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 484;
						break;
					} else {
						Error(la);
						goto case 483;
					}
				}
			}
			case 483: {
				PopContext();
				goto case 15;
			}
			case 484: {
				stateStack.Push(485);
				goto case 147;
			}
			case 485: {
				if (la == null) { currentState = 485; break; }
				Expect(20, la); // "="
				currentState = 486;
				break;
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				Expect(3, la); // LiteralString
				currentState = 487;
				break;
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 483;
				break;
			}
			case 488: {
				if (la == null) { currentState = 488; break; }
				if (la.kind == 37) {
					stateStack.Push(488);
					goto case 24;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 489;
						break;
					} else {
						goto case 483;
					}
				}
			}
			case 489: {
				stateStack.Push(483);
				goto case 19;
			}
			case 490: {
				if (la == null) { currentState = 490; break; }
				Expect(173, la); // "Option"
				currentState = 491;
				break;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 493;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 492;
						break;
					} else {
						goto case 451;
					}
				}
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				if (la.kind == 213) {
					currentState = 15;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 15;
						break;
					} else {
						goto case 451;
					}
				}
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 15;
					break;
				} else {
					goto case 15;
				}
			}
		}

		if (la != null) t = la;
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	public BitArray GetExpectedSet() { return GetExpectedSet(currentState); }
	
	static readonly BitArray[] set = {
		new BitArray(new int[] {1, 256, 1048576, 536871040, 134218240, 436215809, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 536871040, 134218240, 436207617, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 536871040, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 536871040, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 536871040, 134217728, 436207616, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 536871040, 134217728, 436207616, 131200, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564478, 889192437, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {-940564478, 889192405, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-62257156, 1174405224, -51646385, -972026621, -1039365982, 17106484, -1707866112, 8257}),
		new BitArray(new int[] {-62257156, 1174405224, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-62257156, 1174405160, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4672}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1040382, 889192437, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1028, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1038334, -1258291211, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {1007290364, 1140850720, -51646385, -972026621, -1039365982, 17105952, -1976301568, 8257}),
		new BitArray(new int[] {-1040382, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {0, 0, -60035072, 1027, 0, 0, 134217728, 0}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1310752, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4194304, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026617, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-1048578, 2147483647, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8385}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {-64354306, -973078488, -51646385, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-64354306, 1191182376, -1311153, -546070781, -1022588750, -1593504708, -1631823826, 8901}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66451460, 1191182376, -1314225, -680517885, -1039365966, -1593504716, -1631823826, 8901}),
		new BitArray(new int[] {6291458, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1174405160, -51646385, -971993853, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {2097158, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 97}),
		new BitArray(new int[] {2097154, -2147483648, 0, 0, 0, 0, 0, 32}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105928, -2144335872, 65}),
		new BitArray(new int[] {-66451460, 1174405160, -51646385, -972026621, -1039365966, 17105972, -1707866108, 8257}),
		new BitArray(new int[] {1007290364, 1140850720, -51646385, -972010237, -1039365982, 17105952, -1976301568, 8257}),
		new BitArray(new int[] {1007681536, -2147483614, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {1007681536, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 0, 0, 129}),
		new BitArray(new int[] {2097154, 0, 0, 32768, 0, 0, 0, 129}),
		new BitArray(new int[] {-66451460, 1174405160, -51645361, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-65402884, 1174409128, -51646385, -971993853, -1039300446, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {-65402884, 1174409128, -51646385, -972026621, -1039300446, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1191182376, -1314225, -680517885, -1039365966, -1593504716, -1631823826, 8901}),
		new BitArray(new int[] {-64354306, 1191182376, -1314225, -680485117, -1039365966, -1593504716, -1631823826, 8901}),
		new BitArray(new int[] {2097154, 32, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483614, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483648, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {5242880, -2147483584, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7, 1157628160, 26477055, -493868100, 680306724, 1006458243, -533262446, 1347}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {721920, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {4194308, 1140850752, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-2, -33, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537696544, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537692448, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {1, 256, 1048576, 537002112, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493876444, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850688, 25165903, -493876444, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850696, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850688, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {0, 256, 1048576, 537002112, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 128, 0}),
		new BitArray(new int[] {0, 0, 262288, 8216, 8396800, 256, 1610679824, 2}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1016, 0, 0, 67108864, -1040187392, 32, 33554432, 0}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 0, -1133776896, 3, 0, 0, 0, 0}),
		new BitArray(new int[] {-64354306, 1191182376, -1314225, -680386813, -1039365966, -1593504716, -1631823826, 8901}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {-66451460, 1174405160, -51645873, -972026621, -1039365982, 17105972, -1707866112, 8257}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 65536, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 288, 0, 0, 4210688, 0, 0}),
		new BitArray(new int[] {-2, -129, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-18434, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-22530, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-32770, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-37890, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2050, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-6146, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-2097160, -1140850977, -25165904, 493745371, -537696545, -465376387, 2144073343, -1346}),
		new BitArray(new int[] {0, 0, 0, 536871424, 536870912, 448266370, 384, 1280}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0})

	};

} // end Parser


}