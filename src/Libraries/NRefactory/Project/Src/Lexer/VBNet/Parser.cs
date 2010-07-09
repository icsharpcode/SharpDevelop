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
	const int startOfExpression = 34;
	const int endOfStatementTerminatorAndBlock = 203;
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
			case 204:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 190:
			case 434:
			case 441:
			case 449:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 10:
			case 11:
			case 101:
			case 156:
			case 157:
			case 205:
			case 343:
			case 344:
			case 358:
			case 359:
			case 360:
			case 383:
			case 384:
			case 385:
			case 386:
			case 399:
			case 400:
			case 442:
			case 443:
			case 461:
			case 462:
			case 474:
			case 475:
			case 479:
			case 480:
			case 482:
				return set[6];
			case 12:
			case 13:
			case 444:
			case 445:
				return set[7];
			case 14:
			case 405:
			case 446:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 15:
			case 196:
			case 199:
			case 200:
			case 210:
			case 224:
			case 228:
			case 246:
			case 259:
			case 270:
			case 273:
			case 279:
			case 284:
			case 293:
			case 294:
			case 307:
			case 315:
			case 393:
			case 406:
			case 418:
			case 421:
			case 447:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 16:
			case 304:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 17:
			case 18:
			case 105:
			case 114:
			case 130:
			case 145:
			case 160:
			case 234:
			case 325:
			case 333:
			case 341:
			case 369:
			case 427:
			case 431:
			case 450:
			case 459:
				return set[8];
			case 19:
			case 22:
				return set[9];
			case 20:
				return set[10];
			case 21:
			case 51:
			case 55:
			case 110:
			case 310:
			case 372:
				return set[11];
			case 23:
			case 120:
			case 127:
			case 131:
			case 191:
			case 347:
			case 365:
			case 368:
			case 401:
			case 402:
			case 415:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 24:
			case 25:
			case 112:
			case 113:
				return set[12];
			case 26:
			case 193:
			case 331:
			case 349:
			case 367:
			case 382:
			case 404:
			case 417:
			case 430:
			case 452:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 27:
			case 28:
			case 31:
			case 32:
			case 377:
			case 378:
				return set[13];
			case 29:
				return set[14];
			case 30:
			case 122:
			case 129:
			case 313:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 33:
			case 115:
			case 124:
			case 330:
			case 332:
			case 335:
			case 376:
			case 380:
			case 467:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 34:
			case 35:
			case 37:
			case 38:
			case 39:
			case 42:
			case 53:
			case 103:
			case 121:
			case 123:
			case 125:
			case 128:
			case 137:
			case 139:
			case 177:
			case 209:
			case 213:
			case 215:
			case 216:
			case 231:
			case 245:
			case 250:
			case 257:
			case 263:
			case 265:
			case 269:
			case 272:
			case 278:
			case 289:
			case 291:
			case 297:
			case 312:
			case 314:
			case 362:
			case 374:
			case 375:
			case 458:
				return set[15];
			case 36:
			case 40:
				return set[16];
			case 41:
			case 48:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 43:
			case 54:
			case 471:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 44:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 45:
			case 75:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 46:
				return set[17];
			case 47:
			case 56:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 49:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 50:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 52:
			case 159:
			case 161:
			case 256:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 57:
			case 275:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 58:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 59:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 60:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 61:
			case 227:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 62:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 63:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 64:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 65:
			case 353:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 67:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 69:
			case 281:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 70:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 71:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 72:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 73:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 74:
			case 240:
			case 247:
			case 260:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 77:
			case 164:
			case 169:
			case 171:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 78:
			case 166:
			case 170:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 80:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 81:
			case 198:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 82:
			case 189:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 83:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 84:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 85:
			case 138:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 87:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 89:
			case 410:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 91:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 92:
			case 149:
			case 176:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 95:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 96:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 98:
			case 188:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 100:
				return set[18];
			case 102:
				return set[19];
			case 104:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 106:
				return set[20];
			case 107:
				return set[21];
			case 108:
			case 109:
			case 370:
			case 371:
				return set[22];
			case 111:
				return set[23];
			case 116:
			case 117:
			case 243:
			case 252:
				return set[24];
			case 118:
				return set[25];
			case 119:
			case 296:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 126:
				return set[26];
			case 132:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 133:
			case 134:
				return set[27];
			case 135:
			case 141:
			case 146:
			case 182:
			case 186:
			case 223:
			case 318:
			case 326:
			case 366:
			case 423:
			case 435:
				return set[28];
			case 136:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 140:
			case 153:
			case 168:
			case 173:
			case 179:
			case 181:
			case 185:
			case 187:
				return set[29];
			case 142:
			case 143:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 144:
			case 244:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 147:
			case 148:
			case 150:
			case 152:
			case 154:
			case 155:
			case 162:
			case 167:
			case 172:
			case 180:
			case 184:
				return set[30];
			case 151:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 158:
				return set[31];
			case 163:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 165:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 174:
			case 175:
				return set[32];
			case 178:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 183:
				return set[33];
			case 192:
			case 348:
			case 381:
			case 429:
			case 451:
				return set[34];
			case 194:
			case 195:
				return set[35];
			case 197:
			case 211:
			case 226:
			case 274:
			case 316:
			case 352:
			case 407:
			case 419:
			case 448:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 201:
			case 202:
				return set[36];
			case 203:
				return set[37];
			case 206:
				return set[38];
			case 207:
			case 208:
			case 302:
				return set[39];
			case 212:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 214:
			case 251:
			case 264:
				return set[40];
			case 217:
			case 218:
			case 248:
			case 249:
			case 261:
			case 262:
				return set[41];
			case 219:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 220:
				return set[42];
			case 221:
			case 236:
				return set[43];
			case 222:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 225:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 229:
				return set[44];
			case 230:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 232:
			case 233:
				return set[45];
			case 235:
				return set[46];
			case 237:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 238:
			case 239:
				return set[47];
			case 241:
			case 242:
				return set[48];
			case 253:
			case 254:
				return set[49];
			case 255:
				return set[50];
			case 258:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 266:
				return set[51];
			case 267:
			case 271:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 268:
				return set[52];
			case 276:
			case 277:
				return set[53];
			case 280:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 282:
			case 283:
				return set[54];
			case 285:
			case 286:
				return set[55];
			case 287:
			case 428:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 288:
			case 290:
				return set[56];
			case 292:
			case 298:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 295:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 299:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 300:
			case 301:
			case 305:
			case 306:
			case 350:
			case 351:
				return set[57];
			case 303:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 308:
			case 309:
				return set[58];
			case 311:
				return set[59];
			case 317:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 319:
			case 320:
			case 327:
			case 328:
				return set[60];
			case 321:
			case 329:
				return set[61];
			case 322:
				return set[62];
			case 323:
				return set[63];
			case 324:
				return set[64];
			case 334:
			case 336:
			case 403:
			case 416:
				return set[65];
			case 337:
				return set[66];
			case 338:
			case 339:
				return set[67];
			case 340:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 342:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 345:
			case 346:
				return set[68];
			case 354:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 355:
				return set[69];
			case 356:
				return set[70];
			case 357:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 361:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 363:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 364:
				return set[71];
			case 373:
				return set[72];
			case 379:
				return set[73];
			case 387:
				return set[74];
			case 388:
				return set[75];
			case 389:
				return set[76];
			case 390:
			case 391:
			case 397:
				return set[77];
			case 392:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 394:
				return set[78];
			case 395:
				return set[79];
			case 396:
				return set[80];
			case 398:
			case 408:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 409:
				return set[81];
			case 411:
			case 413:
			case 422:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 412:
				return set[82];
			case 414:
				return set[83];
			case 420:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 424:
			case 425:
				return set[84];
			case 426:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 432:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 433:
				return set[85];
			case 436:
			case 437:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 438:
			case 440:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 439:
				return set[86];
			case 453:
				return set[87];
			case 454:
			case 460:
				return set[88];
			case 455:
			case 456:
				return set[89];
			case 457:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 463:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 464:
				return set[90];
			case 465:
			case 473:
				return set[91];
			case 466:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 468:
			case 469:
				return set[92];
			case 470:
			case 472:
				return set[93];
			case 476:
				return set[94];
			case 477:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 478:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 481:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
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
					goto case 481;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 478;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 342;
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
					currentState = 474;
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
					goto case 342;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[95].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						currentState = 383;
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
					currentState = 381;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				if (la.kind == 63) {
					currentState = 17;
					break;
				} else {
					goto case 15;
				}
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 17: {
				stateStack.Push(15);
				goto case 18;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (la.kind == 130) {
					currentState = 19;
					break;
				} else {
					if (set[28].Get(la.kind)) {
						currentState = 19;
						break;
					} else {
						if (set[96].Get(la.kind)) {
							currentState = 19;
							break;
						} else {
							Error(la);
							goto case 19;
						}
					}
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.kind == 37) {
					stateStack.Push(19);
					goto case 23;
				} else {
					goto case 20;
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				if (la.kind == 26) {
					currentState = 21;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 21: {
				stateStack.Push(22);
				goto case 55;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (la.kind == 37) {
					stateStack.Push(22);
					goto case 23;
				} else {
					goto case 20;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				Expect(37, la); // "("
				currentState = 24;
				break;
			}
			case 24: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 25;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 169) {
					currentState = 379;
					break;
				} else {
					if (set[13].Get(la.kind)) {
						goto case 27;
					} else {
						Error(la);
						goto case 26;
					}
				}
			}
			case 26: {
				if (la == null) { currentState = 26; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 27: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 28;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[14].Get(la.kind)) {
					stateStack.Push(26);
					nextTokenIsPotentialStartOfExpression = true;
					goto case 29;
				} else {
					goto case 26;
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (set[15].Get(la.kind)) {
					goto case 375;
				} else {
					if (la.kind == 22) {
						goto case 30;
					} else {
						goto case 6;
					}
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				currentState = 31;
				break;
			}
			case 31: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 32;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(33);
					goto case 34;
				} else {
					goto case 33;
				}
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (la.kind == 22) {
					goto case 30;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 34: {
				PushContext(Context.Expression, la, t);
				goto case 35;
			}
			case 35: {
				stateStack.Push(36);
				goto case 37;
			}
			case 36: {
				if (la == null) { currentState = 36; break; }
				if (set[97].Get(la.kind)) {
					currentState = 35;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 37: {
				PushContext(Context.Expression, la, t);
				goto case 38;
			}
			case 38: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 39;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (set[98].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					if (set[24].Get(la.kind)) {
						stateStack.Push(106);
						goto case 116;
					} else {
						if (la.kind == 220) {
							currentState = 103;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(40);
								goto case 45;
							} else {
								if (la.kind == 35) {
									stateStack.Push(40);
									goto case 41;
								} else {
									Error(la);
									goto case 40;
								}
							}
						}
					}
				}
			}
			case 40: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				Expect(35, la); // "{"
				currentState = 42;
				break;
			}
			case 42: {
				stateStack.Push(43);
				goto case 34;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (la.kind == 22) {
					currentState = 42;
					break;
				} else {
					goto case 44;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(162, la); // "New"
				currentState = 46;
				break;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				if (set[8].Get(la.kind)) {
					stateStack.Push(100);
					goto case 18;
				} else {
					goto case 47;
				}
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (la.kind == 233) {
					currentState = 48;
					break;
				} else {
					goto case 6;
				}
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				Expect(35, la); // "{"
				currentState = 49;
				break;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (la.kind == 147) {
					currentState = 50;
					break;
				} else {
					goto case 50;
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				Expect(26, la); // "."
				currentState = 51;
				break;
			}
			case 51: {
				stateStack.Push(52);
				goto case 55;
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				Expect(20, la); // "="
				currentState = 53;
				break;
			}
			case 53: {
				stateStack.Push(54);
				goto case 34;
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				if (la.kind == 22) {
					currentState = 49;
					break;
				} else {
					goto case 44;
				}
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (la.kind == 2) {
					goto case 99;
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
								goto case 98;
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
												goto case 97;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 96;
													} else {
														if (la.kind == 65) {
															goto case 95;
														} else {
															if (la.kind == 66) {
																goto case 94;
															} else {
																if (la.kind == 67) {
																	goto case 93;
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
																				goto case 92;
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
																																		goto case 91;
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
																																					goto case 90;
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
																																																goto case 89;
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
																																																						goto case 88;
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
																																																									goto case 87;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 86;
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
																																																																		goto case 85;
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
																																																																							goto case 84;
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
																																																																										goto case 83;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 82;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 81;
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
																																																																																			goto case 80;
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
																																																																																									goto case 79;
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
																																																																																													goto case 78;
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
																																																																																																goto case 77;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 76;
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
																																																																																																																goto case 75;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 74;
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
																																																																																																																								goto case 73;
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
																																																																																																																														goto case 72;
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
																																																																																																																																						goto case 71;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 70;
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
																																																																																																																																																			goto case 69;
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
																																																																																																																																																									goto case 68;
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
																																																																																																																																																												goto case 67;
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
																																																																																																																																																															goto case 66;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 65;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 64;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 63;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 62;
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
																																																																																																																																																																								goto case 61;
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
																																																																																																																																																																													goto case 60;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 59;
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
																																																																																																																																																																																				goto case 58;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 57;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 56;
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
			case 56: {
				if (la == null) { currentState = 56; break; }
				currentState = stateStack.Pop();
				break;
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
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 101;
						break;
					} else {
						goto case 47;
					}
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 101: {
				if (la == null) { currentState = 101; break; }
				if (la.kind == 35) {
					currentState = 42;
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
			case 102: {
				if (la == null) { currentState = 102; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 103: {
				stateStack.Push(104);
				goto case 37;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				Expect(144, la); // "Is"
				currentState = 105;
				break;
			}
			case 105: {
				stateStack.Push(40);
				goto case 18;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(106);
					goto case 107;
				} else {
					goto case 40;
				}
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				if (la.kind == 37) {
					currentState = 112;
					break;
				} else {
					if (set[99].Get(la.kind)) {
						currentState = 108;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 108: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 109;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				if (la.kind == 10) {
					currentState = 110;
					break;
				} else {
					goto case 110;
				}
			}
			case 110: {
				stateStack.Push(111);
				goto case 55;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 112: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 113;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				if (la.kind == 169) {
					currentState = 114;
					break;
				} else {
					if (set[13].Get(la.kind)) {
						goto case 27;
					} else {
						goto case 6;
					}
				}
			}
			case 114: {
				stateStack.Push(115);
				goto case 18;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				if (la.kind == 22) {
					currentState = 114;
					break;
				} else {
					goto case 26;
				}
			}
			case 116: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 117;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				if (set[100].Get(la.kind)) {
					currentState = 118;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 374;
						break;
					} else {
						if (set[101].Get(la.kind)) {
							currentState = 118;
							break;
						} else {
							if (set[99].Get(la.kind)) {
								currentState = 370;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 368;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 365;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(118);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 354;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(118);
												goto case 190;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(118);
													PushContext(Context.Query, la, t);
													goto case 132;
												} else {
													if (set[26].Get(la.kind)) {
														stateStack.Push(118);
														goto case 126;
													} else {
														if (la.kind == 135) {
															stateStack.Push(118);
															goto case 119;
														} else {
															Error(la);
															goto case 118;
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
			case 118: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				Expect(135, la); // "If"
				currentState = 120;
				break;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				Expect(37, la); // "("
				currentState = 121;
				break;
			}
			case 121: {
				stateStack.Push(122);
				goto case 34;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				Expect(22, la); // ","
				currentState = 123;
				break;
			}
			case 123: {
				stateStack.Push(124);
				goto case 34;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				if (la.kind == 22) {
					currentState = 125;
					break;
				} else {
					goto case 26;
				}
			}
			case 125: {
				stateStack.Push(26);
				goto case 34;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (set[102].Get(la.kind)) {
					currentState = 131;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 127;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				Expect(37, la); // "("
				currentState = 128;
				break;
			}
			case 128: {
				stateStack.Push(129);
				goto case 34;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				Expect(22, la); // ","
				currentState = 130;
				break;
			}
			case 130: {
				stateStack.Push(26);
				goto case 18;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				Expect(37, la); // "("
				currentState = 125;
				break;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 126) {
					stateStack.Push(133);
					goto case 189;
				} else {
					if (la.kind == 58) {
						stateStack.Push(133);
						goto case 188;
					} else {
						Error(la);
						goto case 133;
					}
				}
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(133);
					goto case 134;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				if (la.kind == 126) {
					currentState = 186;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 182;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 180;
							break;
						} else {
							if (la.kind == 107) {
								goto case 87;
							} else {
								if (la.kind == 230) {
									currentState = 34;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 176;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 174;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 172;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 147;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 135;
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
			case 135: {
				stateStack.Push(136);
				goto case 141;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				Expect(171, la); // "On"
				currentState = 137;
				break;
			}
			case 137: {
				stateStack.Push(138);
				goto case 34;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				Expect(116, la); // "Equals"
				currentState = 139;
				break;
			}
			case 139: {
				stateStack.Push(140);
				goto case 34;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				if (la.kind == 22) {
					currentState = 137;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 141: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(142);
				goto case 146;
			}
			case 142: {
				PopContext();
				goto case 143;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				if (la.kind == 63) {
					currentState = 145;
					break;
				} else {
					goto case 144;
				}
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				Expect(138, la); // "In"
				currentState = 34;
				break;
			}
			case 145: {
				stateStack.Push(144);
				goto case 18;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (set[88].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 89;
					} else {
						goto case 6;
					}
				}
			}
			case 147: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 148;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				if (la.kind == 146) {
					goto case 164;
				} else {
					if (set[30].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 150;
							break;
						} else {
							if (set[30].Get(la.kind)) {
								goto case 162;
							} else {
								Error(la);
								goto case 149;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(70, la); // "By"
				currentState = 150;
				break;
			}
			case 150: {
				stateStack.Push(151);
				goto case 154;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				if (la.kind == 22) {
					currentState = 150;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 152;
					break;
				}
			}
			case 152: {
				stateStack.Push(153);
				goto case 154;
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (la.kind == 22) {
					currentState = 152;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 154: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 155;
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				if (set[28].Get(la.kind)) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(156);
					goto case 146;
				} else {
					goto case 34;
				}
			}
			case 156: {
				PopContext();
				goto case 157;
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				if (la.kind == 63) {
					currentState = 160;
					break;
				} else {
					if (la.kind == 20) {
						goto case 159;
					} else {
						if (set[31].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 34;
						}
					}
				}
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				currentState = 34;
				break;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				currentState = 34;
				break;
			}
			case 160: {
				stateStack.Push(161);
				goto case 18;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				Expect(20, la); // "="
				currentState = 34;
				break;
			}
			case 162: {
				stateStack.Push(163);
				goto case 154;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.kind == 22) {
					currentState = 162;
					break;
				} else {
					goto case 149;
				}
			}
			case 164: {
				stateStack.Push(165);
				goto case 171;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 169;
						break;
					} else {
						if (la.kind == 146) {
							goto case 164;
						} else {
							Error(la);
							goto case 165;
						}
					}
				} else {
					goto case 166;
				}
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				Expect(143, la); // "Into"
				currentState = 167;
				break;
			}
			case 167: {
				stateStack.Push(168);
				goto case 154;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				if (la.kind == 22) {
					currentState = 167;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 169: {
				stateStack.Push(170);
				goto case 171;
			}
			case 170: {
				stateStack.Push(165);
				goto case 166;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				Expect(146, la); // "Join"
				currentState = 135;
				break;
			}
			case 172: {
				stateStack.Push(173);
				goto case 154;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				if (la.kind == 22) {
					currentState = 172;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 174: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 175;
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (la.kind == 231) {
					currentState = 34;
					break;
				} else {
					goto case 34;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				Expect(70, la); // "By"
				currentState = 177;
				break;
			}
			case 177: {
				stateStack.Push(178);
				goto case 34;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (la.kind == 64) {
					currentState = 179;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 179;
						break;
					} else {
						Error(la);
						goto case 179;
					}
				}
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				if (la.kind == 22) {
					currentState = 177;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 180: {
				stateStack.Push(181);
				goto case 154;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				if (la.kind == 22) {
					currentState = 180;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 182: {
				stateStack.Push(183);
				goto case 141;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(183);
					goto case 134;
				} else {
					Expect(143, la); // "Into"
					currentState = 184;
					break;
				}
			}
			case 184: {
				stateStack.Push(185);
				goto case 154;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 22) {
					currentState = 184;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 186: {
				stateStack.Push(187);
				goto case 141;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.kind == 22) {
					currentState = 186;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				Expect(58, la); // "Aggregate"
				currentState = 182;
				break;
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				Expect(126, la); // "From"
				currentState = 186;
				break;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 210) {
					currentState = 347;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 191;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				Expect(37, la); // "("
				currentState = 192;
				break;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(193);
					goto case 334;
				} else {
					goto case 193;
				}
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				Expect(38, la); // ")"
				currentState = 194;
				break;
			}
			case 194: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 195;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (set[15].Get(la.kind)) {
					goto case 34;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 333;
							break;
						} else {
							goto case 196;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 196: {
				stateStack.Push(197);
				goto case 199;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				Expect(113, la); // "End"
				currentState = 198;
				break;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 199: {
				PushContext(Context.Body, la, t);
				goto case 200;
			}
			case 200: {
				stateStack.Push(201);
				goto case 15;
			}
			case 201: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 202;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (set[103].Get(la.kind)) {
					if (set[57].Get(la.kind)) {
						if (set[39].Get(la.kind)) {
							stateStack.Push(200);
							goto case 207;
						} else {
							goto case 200;
						}
					} else {
						if (la.kind == 113) {
							currentState = 205;
							break;
						} else {
							goto case 204;
						}
					}
				} else {
					goto case 203;
				}
			}
			case 203: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 204: {
				Error(la);
				goto case 201;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (la.kind == 1 || la.kind == 21) {
					currentState = 201;
					break;
				} else {
					if (set[38].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 204;
					}
				}
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				currentState = 201;
				break;
			}
			case 207: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 208;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 318;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 314;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 312;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 310;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 291;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 276;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 272;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 266;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 241;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 237;
																break;
															} else {
																goto case 237;
															}
														} else {
															if (la.kind == 194) {
																currentState = 235;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 217;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 224;
																break;
															} else {
																if (set[104].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 221;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 220;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 219;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 67;
																				} else {
																					if (la.kind == 195) {
																						currentState = 217;
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
																		currentState = 215;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 213;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 209;
																				break;
																			} else {
																				if (set[105].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 34;
																						break;
																					} else {
																						goto case 34;
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
			case 209: {
				stateStack.Push(210);
				goto case 34;
			}
			case 210: {
				stateStack.Push(211);
				goto case 199;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(113, la); // "End"
				currentState = 212;
				break;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 213: {
				stateStack.Push(214);
				goto case 34;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				if (la.kind == 22) {
					currentState = 213;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 215: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 216;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (la.kind == 184) {
					currentState = 34;
					break;
				} else {
					goto case 34;
				}
			}
			case 217: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 218;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[15].Get(la.kind)) {
					goto case 34;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 108) {
					goto case 86;
				} else {
					if (la.kind == 124) {
						goto case 83;
					} else {
						if (la.kind == 231) {
							goto case 57;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				if (la.kind == 108) {
					goto case 86;
				} else {
					if (la.kind == 124) {
						goto case 83;
					} else {
						if (la.kind == 231) {
							goto case 57;
						} else {
							if (la.kind == 197) {
								goto case 69;
							} else {
								if (la.kind == 210) {
									goto case 65;
								} else {
									if (la.kind == 127) {
										goto case 81;
									} else {
										if (la.kind == 186) {
											goto case 70;
										} else {
											if (la.kind == 218) {
												goto case 61;
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
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (set[28].Get(la.kind)) {
					goto case 223;
				} else {
					if (la.kind == 5) {
						goto case 222;
					} else {
						goto case 6;
					}
				}
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				stateStack.Push(225);
				goto case 199;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.kind == 75) {
					currentState = 229;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 228;
						break;
					} else {
						goto case 226;
					}
				}
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				Expect(113, la); // "End"
				currentState = 227;
				break;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 228: {
				stateStack.Push(226);
				goto case 199;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (set[28].Get(la.kind)) {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(232);
					goto case 146;
				} else {
					goto case 230;
				}
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (la.kind == 229) {
					currentState = 231;
					break;
				} else {
					goto case 224;
				}
			}
			case 231: {
				stateStack.Push(224);
				goto case 34;
			}
			case 232: {
				PopContext();
				goto case 233;
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				if (la.kind == 63) {
					currentState = 234;
					break;
				} else {
					goto case 230;
				}
			}
			case 234: {
				stateStack.Push(230);
				goto case 18;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (la.kind == 163) {
					goto case 74;
				} else {
					goto case 236;
				}
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (la.kind == 5) {
					goto case 222;
				} else {
					if (set[28].Get(la.kind)) {
						goto case 223;
					} else {
						goto case 6;
					}
				}
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				Expect(118, la); // "Error"
				currentState = 238;
				break;
			}
			case 238: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 239;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (set[15].Get(la.kind)) {
					goto case 34;
				} else {
					if (la.kind == 132) {
						currentState = 236;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 240;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 241: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 242;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (set[24].Get(la.kind)) {
					stateStack.Push(256);
					goto case 252;
				} else {
					if (la.kind == 110) {
						currentState = 243;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 243: {
				stateStack.Push(244);
				goto case 252;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				Expect(138, la); // "In"
				currentState = 245;
				break;
			}
			case 245: {
				stateStack.Push(246);
				goto case 34;
			}
			case 246: {
				stateStack.Push(247);
				goto case 199;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				Expect(163, la); // "Next"
				currentState = 248;
				break;
			}
			case 248: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 249;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (set[15].Get(la.kind)) {
					goto case 250;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 250: {
				stateStack.Push(251);
				goto case 34;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (la.kind == 22) {
					currentState = 250;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 252: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(253);
				goto case 116;
			}
			case 253: {
				PopContext();
				goto case 254;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (la.kind == 33) {
					currentState = 255;
					break;
				} else {
					goto case 255;
				}
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(255);
					goto case 107;
				} else {
					if (la.kind == 63) {
						currentState = 18;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				Expect(20, la); // "="
				currentState = 257;
				break;
			}
			case 257: {
				stateStack.Push(258);
				goto case 34;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (la.kind == 205) {
					currentState = 265;
					break;
				} else {
					goto case 259;
				}
			}
			case 259: {
				stateStack.Push(260);
				goto case 199;
			}
			case 260: {
				if (la == null) { currentState = 260; break; }
				Expect(163, la); // "Next"
				currentState = 261;
				break;
			}
			case 261: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 262;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (set[15].Get(la.kind)) {
					goto case 263;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 263: {
				stateStack.Push(264);
				goto case 34;
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				if (la.kind == 22) {
					currentState = 263;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 265: {
				stateStack.Push(259);
				goto case 34;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 269;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(267);
						goto case 199;
					} else {
						goto case 6;
					}
				}
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				Expect(152, la); // "Loop"
				currentState = 268;
				break;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 34;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 269: {
				stateStack.Push(270);
				goto case 34;
			}
			case 270: {
				stateStack.Push(271);
				goto case 199;
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 272: {
				stateStack.Push(273);
				goto case 34;
			}
			case 273: {
				stateStack.Push(274);
				goto case 199;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				Expect(113, la); // "End"
				currentState = 275;
				break;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 276: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 277;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (la.kind == 74) {
					currentState = 278;
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
				stateStack.Push(280);
				goto case 15;
			}
			case 280: {
				if (la == null) { currentState = 280; break; }
				if (la.kind == 74) {
					currentState = 282;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 281;
					break;
				}
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 282: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 283;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (la.kind == 111) {
					currentState = 284;
					break;
				} else {
					if (set[55].Get(la.kind)) {
						goto case 285;
					} else {
						Error(la);
						goto case 284;
					}
				}
			}
			case 284: {
				stateStack.Push(280);
				goto case 199;
			}
			case 285: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 286;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (set[106].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 288;
						break;
					} else {
						goto case 288;
					}
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(287);
						goto case 34;
					} else {
						Error(la);
						goto case 287;
					}
				}
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (la.kind == 22) {
					currentState = 285;
					break;
				} else {
					goto case 284;
				}
			}
			case 288: {
				stateStack.Push(289);
				goto case 290;
			}
			case 289: {
				stateStack.Push(287);
				goto case 37;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
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
			case 291: {
				stateStack.Push(292);
				goto case 34;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (la.kind == 214) {
					currentState = 300;
					break;
				} else {
					goto case 293;
				}
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 294;
				} else {
					goto case 6;
				}
			}
			case 294: {
				stateStack.Push(295);
				goto case 199;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 299;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 297;
							break;
						} else {
							Error(la);
							goto case 294;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 296;
					break;
				}
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 297: {
				stateStack.Push(298);
				goto case 34;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 214) {
					currentState = 294;
					break;
				} else {
					goto case 294;
				}
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 135) {
					currentState = 297;
					break;
				} else {
					goto case 294;
				}
			}
			case 300: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 301;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (set[39].Get(la.kind)) {
					goto case 302;
				} else {
					goto case 293;
				}
			}
			case 302: {
				stateStack.Push(303);
				goto case 207;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (la.kind == 21) {
					currentState = 308;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 305;
						break;
					} else {
						goto case 304;
					}
				}
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 305: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 306;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				if (set[39].Get(la.kind)) {
					stateStack.Push(307);
					goto case 207;
				} else {
					goto case 307;
				}
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				if (la.kind == 21) {
					currentState = 305;
					break;
				} else {
					goto case 304;
				}
			}
			case 308: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 309;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (set[39].Get(la.kind)) {
					goto case 302;
				} else {
					goto case 303;
				}
			}
			case 310: {
				stateStack.Push(311);
				goto case 55;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				if (la.kind == 37) {
					currentState = 27;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 312: {
				stateStack.Push(313);
				goto case 34;
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				Expect(22, la); // ","
				currentState = 34;
				break;
			}
			case 314: {
				stateStack.Push(315);
				goto case 34;
			}
			case 315: {
				stateStack.Push(316);
				goto case 199;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				Expect(113, la); // "End"
				currentState = 317;
				break;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 233) {
					goto case 56;
				} else {
					if (la.kind == 211) {
						goto case 64;
					} else {
						goto case 6;
					}
				}
			}
			case 318: {
				PushContext(Context.IdentifierExpected, la, t);	
				stateStack.Push(319);
				goto case 146;
			}
			case 319: {
				PopContext();
				goto case 320;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.kind == 33) {
					currentState = 321;
					break;
				} else {
					goto case 321;
				}
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (la.kind == 37) {
					currentState = 332;
					break;
				} else {
					goto case 322;
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 22) {
					currentState = 326;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 324;
						break;
					} else {
						goto case 323;
					}
				}
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 20) {
					goto case 159;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (la.kind == 162) {
					currentState = 325;
					break;
				} else {
					goto case 325;
				}
			}
			case 325: {
				stateStack.Push(323);
				goto case 18;
			}
			case 326: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(327);
				goto case 146;
			}
			case 327: {
				PopContext();
				goto case 328;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 33) {
					currentState = 329;
					break;
				} else {
					goto case 329;
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 37) {
					currentState = 330;
					break;
				} else {
					goto case 322;
				}
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				if (la.kind == 22) {
					currentState = 330;
					break;
				} else {
					goto case 331;
				}
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				Expect(38, la); // ")"
				currentState = 322;
				break;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 22) {
					currentState = 332;
					break;
				} else {
					goto case 331;
				}
			}
			case 333: {
				stateStack.Push(196);
				goto case 18;
			}
			case 334: {
				stateStack.Push(335);
				goto case 336;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 22) {
					currentState = 334;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 40) {
					stateStack.Push(336);
					goto case 342;
				} else {
					goto case 337;
				}
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (set[107].Get(la.kind)) {
					currentState = 337;
					break;
				} else {
					PushContext(Context.IdentifierExpected, la, t);
					stateStack.Push(338);
					goto case 146;
				}
			}
			case 338: {
				PopContext();
				goto case 339;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.kind == 63) {
					currentState = 341;
					break;
				} else {
					goto case 340;
				}
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 20) {
					goto case 159;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 341: {
				stateStack.Push(340);
				goto case 18;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				Expect(40, la); // "<"
				currentState = 343;
				break;
			}
			case 343: {
				PushContext(Context.Attribute, la, t);
				goto case 344;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (set[108].Get(la.kind)) {
					currentState = 344;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 345;
					break;
				}
			}
			case 345: {
				PopContext();
				goto case 346;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 1) {
					goto case 16;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				Expect(37, la); // "("
				currentState = 348;
				break;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(349);
					goto case 334;
				} else {
					goto case 349;
				}
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				Expect(38, la); // ")"
				currentState = 350;
				break;
			}
			case 350: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 351;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (set[39].Get(la.kind)) {
					goto case 207;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(352);
						goto case 199;
					} else {
						goto case 6;
					}
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				Expect(113, la); // "End"
				currentState = 353;
				break;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 364;
					break;
				} else {
					stateStack.Push(355);
					goto case 357;
				}
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 17) {
					currentState = 356;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 16) {
					currentState = 355;
					break;
				} else {
					goto case 355;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 358;
				break;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (set[109].Get(la.kind)) {
					if (set[110].Get(la.kind)) {
						currentState = 358;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(358);
							goto case 361;
						} else {
							Error(la);
							goto case 358;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 359;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (set[111].Get(la.kind)) {
					if (set[112].Get(la.kind)) {
						currentState = 359;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(359);
							goto case 361;
						} else {
							if (la.kind == 10) {
								stateStack.Push(359);
								goto case 357;
							} else {
								Error(la);
								goto case 359;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 360;
					break;
				}
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (set[113].Get(la.kind)) {
					if (set[114].Get(la.kind)) {
						currentState = 360;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(360);
							goto case 361;
						} else {
							Error(la);
							goto case 360;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 362;
				break;
			}
			case 362: {
				stateStack.Push(363);
				goto case 34;
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 16) {
					currentState = 354;
					break;
				} else {
					goto case 354;
				}
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				Expect(37, la); // "("
				currentState = 366;
				break;
			}
			case 366: {
				readXmlIdentifier = true;
				stateStack.Push(367);
				goto case 146;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				Expect(38, la); // ")"
				currentState = 118;
				break;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				Expect(37, la); // "("
				currentState = 369;
				break;
			}
			case 369: {
				stateStack.Push(367);
				goto case 18;
			}
			case 370: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 371;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (la.kind == 10) {
					currentState = 372;
					break;
				} else {
					goto case 372;
				}
			}
			case 372: {
				stateStack.Push(373);
				goto case 55;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 11) {
					currentState = 118;
					break;
				} else {
					goto case 118;
				}
			}
			case 374: {
				stateStack.Push(367);
				goto case 34;
			}
			case 375: {
				stateStack.Push(376);
				goto case 34;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				if (la.kind == 22) {
					currentState = 377;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 377: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 378;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (set[15].Get(la.kind)) {
					goto case 375;
				} else {
					goto case 376;
				}
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (set[8].Get(la.kind)) {
					stateStack.Push(380);
					goto case 18;
				} else {
					goto case 380;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 22) {
					currentState = 379;
					break;
				} else {
					goto case 26;
				}
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(382);
					goto case 334;
				} else {
					goto case 382;
				}
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 383: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 384;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				currentState = 385;
				break;
			}
			case 385: {
				PopContext();
				goto case 386;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (la.kind == 37) {
					currentState = 463;
					break;
				} else {
					goto case 387;
				}
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (set[115].Get(la.kind)) {
					currentState = 387;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						currentState = 388;
						break;
					} else {
						goto case 388;
					}
				}
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (la.kind == 140) {
					currentState = 462;
					break;
				} else {
					goto case 389;
				}
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (la.kind == 136) {
					currentState = 461;
					break;
				} else {
					goto case 390;
				}
			}
			case 390: {
				PushContext(Context.Type, la, t);
				goto case 391;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (set[79].Get(la.kind)) {
					stateStack.Push(391);
					PushContext(Context.Member, la, t);
					goto case 395;
				} else {
					Expect(113, la); // "End"
					currentState = 392;
					break;
				}
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				if (la.kind == 155) {
					currentState = 393;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 393;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 393;
							break;
						} else {
							Error(la);
							goto case 393;
						}
					}
				}
			}
			case 393: {
				stateStack.Push(394);
				goto case 15;
			}
			case 394: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (la.kind == 40) {
					stateStack.Push(395);
					goto case 342;
				} else {
					goto case 396;
				}
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (set[116].Get(la.kind)) {
					currentState = 396;
					break;
				} else {
					if (set[87].Get(la.kind)) {
						stateStack.Push(397);
						goto case 453;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							stateStack.Push(397);
							goto case 441;
						} else {
							if (la.kind == 101) {
								stateStack.Push(397);
								goto case 432;
							} else {
								if (la.kind == 119) {
									stateStack.Push(397);
									goto case 422;
								} else {
									if (la.kind == 98) {
										stateStack.Push(397);
										goto case 410;
									} else {
										if (la.kind == 172) {
											stateStack.Push(397);
											goto case 398;
										} else {
											Error(la);
											goto case 397;
										}
									}
								}
							}
						}
					}
				}
			}
			case 397: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				Expect(172, la); // "Operator"
				currentState = 399;
				break;
			}
			case 399: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 400;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				currentState = 401;
				break;
			}
			case 401: {
				PopContext();
				goto case 402;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				Expect(37, la); // "("
				currentState = 403;
				break;
			}
			case 403: {
				stateStack.Push(404);
				goto case 334;
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				Expect(38, la); // ")"
				currentState = 405;
				break;
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 63) {
					currentState = 409;
					break;
				} else {
					goto case 406;
				}
			}
			case 406: {
				stateStack.Push(407);
				goto case 199;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				Expect(113, la); // "End"
				currentState = 408;
				break;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(172, la); // "Operator"
				currentState = 15;
				break;
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (la.kind == 40) {
					stateStack.Push(409);
					goto case 342;
				} else {
					stateStack.Push(406);
					goto case 18;
				}
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				Expect(98, la); // "Custom"
				currentState = 411;
				break;
			}
			case 411: {
				stateStack.Push(412);
				goto case 422;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (set[83].Get(la.kind)) {
					goto case 414;
				} else {
					Expect(113, la); // "End"
					currentState = 413;
					break;
				}
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				Expect(119, la); // "Event"
				currentState = 15;
				break;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 40) {
					stateStack.Push(414);
					goto case 342;
				} else {
					if (la.kind == 56) {
						currentState = 415;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 415;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 415;
								break;
							} else {
								Error(la);
								goto case 415;
							}
						}
					}
				}
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				Expect(37, la); // "("
				currentState = 416;
				break;
			}
			case 416: {
				stateStack.Push(417);
				goto case 334;
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				Expect(38, la); // ")"
				currentState = 418;
				break;
			}
			case 418: {
				stateStack.Push(419);
				goto case 199;
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				Expect(113, la); // "End"
				currentState = 420;
				break;
			}
			case 420: {
				if (la == null) { currentState = 420; break; }
				if (la.kind == 56) {
					currentState = 421;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 421;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 421;
							break;
						} else {
							Error(la);
							goto case 421;
						}
					}
				}
			}
			case 421: {
				stateStack.Push(412);
				goto case 15;
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				Expect(119, la); // "Event"
				currentState = 423;
				break;
			}
			case 423: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(424);
				goto case 146;
			}
			case 424: {
				PopContext();
				goto case 425;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (la.kind == 63) {
					currentState = 431;
					break;
				} else {
					if (set[117].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 429;
							break;
						} else {
							goto case 426;
						}
					} else {
						Error(la);
						goto case 426;
					}
				}
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (la.kind == 136) {
					currentState = 427;
					break;
				} else {
					goto case 15;
				}
			}
			case 427: {
				stateStack.Push(428);
				goto case 18;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 22) {
					currentState = 427;
					break;
				} else {
					goto case 15;
				}
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(430);
					goto case 334;
				} else {
					goto case 430;
				}
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				Expect(38, la); // ")"
				currentState = 426;
				break;
			}
			case 431: {
				stateStack.Push(426);
				goto case 18;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				Expect(101, la); // "Declare"
				currentState = 433;
				break;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 434;
					break;
				} else {
					goto case 434;
				}
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (la.kind == 210) {
					currentState = 435;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 435;
						break;
					} else {
						Error(la);
						goto case 435;
					}
				}
			}
			case 435: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(436);
				goto case 146;
			}
			case 436: {
				PopContext();
				goto case 437;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				Expect(149, la); // "Lib"
				currentState = 438;
				break;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				Expect(3, la); // LiteralString
				currentState = 439;
				break;
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (la.kind == 59) {
					currentState = 440;
					break;
				} else {
					goto case 13;
				}
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				Expect(3, la); // LiteralString
				currentState = 13;
				break;
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				if (la.kind == 210) {
					currentState = 442;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 442;
						break;
					} else {
						Error(la);
						goto case 442;
					}
				}
			}
			case 442: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 443;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				currentState = 444;
				break;
			}
			case 444: {
				PopContext();
				goto case 445;
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 37) {
					currentState = 451;
					break;
				} else {
					goto case 446;
				}
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (la.kind == 63) {
					currentState = 450;
					break;
				} else {
					goto case 447;
				}
			}
			case 447: {
				stateStack.Push(448);
				goto case 199;
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				Expect(113, la); // "End"
				currentState = 449;
				break;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				if (la.kind == 210) {
					currentState = 15;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 15;
						break;
					} else {
						Error(la);
						goto case 15;
					}
				}
			}
			case 450: {
				stateStack.Push(447);
				goto case 18;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(452);
					goto case 334;
				} else {
					goto case 452;
				}
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				Expect(38, la); // ")"
				currentState = 446;
				break;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (la.kind == 88) {
					currentState = 454;
					break;
				} else {
					goto case 454;
				}
			}
			case 454: {
				PushContext(Context.IdentifierExpected, la, t);
				stateStack.Push(455);
				goto case 460;
			}
			case 455: {
				PopContext();
				goto case 456;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (la.kind == 63) {
					currentState = 459;
					break;
				} else {
					goto case 457;
				}
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				if (la.kind == 20) {
					currentState = 458;
					break;
				} else {
					goto case 15;
				}
			}
			case 458: {
				stateStack.Push(15);
				goto case 34;
			}
			case 459: {
				stateStack.Push(457);
				goto case 18;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (set[101].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 98;
					} else {
						if (la.kind == 126) {
							goto case 82;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				if (set[38].Get(la.kind)) {
					currentState = 461;
					break;
				} else {
					stateStack.Push(390);
					goto case 15;
				}
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (set[38].Get(la.kind)) {
					currentState = 462;
					break;
				} else {
					stateStack.Push(389);
					goto case 15;
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(169, la); // "Of"
				currentState = 464;
				break;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 465;
					break;
				} else {
					goto case 465;
				}
			}
			case 465: {
				stateStack.Push(466);
				goto case 473;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (la.kind == 63) {
					currentState = 468;
					break;
				} else {
					goto case 467;
				}
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				if (la.kind == 22) {
					currentState = 464;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 387;
					break;
				}
			}
			case 468: {
				stateStack.Push(467);
				goto case 469;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (set[93].Get(la.kind)) {
					goto case 472;
				} else {
					if (la.kind == 35) {
						currentState = 470;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 470: {
				stateStack.Push(471);
				goto case 472;
			}
			case 471: {
				if (la == null) { currentState = 471; break; }
				if (la.kind == 22) {
					currentState = 470;
					break;
				} else {
					goto case 44;
				}
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (set[8].Get(la.kind)) {
					currentState = 19;
					break;
				} else {
					if (la.kind == 162) {
						goto case 75;
					} else {
						if (la.kind == 84) {
							goto case 91;
						} else {
							if (la.kind == 209) {
								goto case 66;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				if (la.kind == 2) {
					goto case 99;
				} else {
					if (la.kind == 62) {
						goto case 97;
					} else {
						if (la.kind == 64) {
							goto case 96;
						} else {
							if (la.kind == 65) {
								goto case 95;
							} else {
								if (la.kind == 66) {
									goto case 94;
								} else {
									if (la.kind == 67) {
										goto case 93;
									} else {
										if (la.kind == 70) {
											goto case 92;
										} else {
											if (la.kind == 87) {
												goto case 90;
											} else {
												if (la.kind == 104) {
													goto case 88;
												} else {
													if (la.kind == 107) {
														goto case 87;
													} else {
														if (la.kind == 116) {
															goto case 85;
														} else {
															if (la.kind == 121) {
																goto case 84;
															} else {
																if (la.kind == 133) {
																	goto case 80;
																} else {
																	if (la.kind == 139) {
																		goto case 79;
																	} else {
																		if (la.kind == 143) {
																			goto case 78;
																		} else {
																			if (la.kind == 146) {
																				goto case 77;
																			} else {
																				if (la.kind == 147) {
																					goto case 76;
																				} else {
																					if (la.kind == 170) {
																						goto case 73;
																					} else {
																						if (la.kind == 176) {
																							goto case 72;
																						} else {
																							if (la.kind == 184) {
																								goto case 71;
																							} else {
																								if (la.kind == 203) {
																									goto case 68;
																								} else {
																									if (la.kind == 212) {
																										goto case 63;
																									} else {
																										if (la.kind == 213) {
																											goto case 62;
																										} else {
																											if (la.kind == 223) {
																												goto case 60;
																											} else {
																												if (la.kind == 224) {
																													goto case 59;
																												} else {
																													if (la.kind == 230) {
																														goto case 58;
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
			case 474: {
				PushContext(Context.IdentifierExpected, la, t);
				goto case 475;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				if (set[38].Get(la.kind)) {
					currentState = 475;
					break;
				} else {
					PopContext();
					stateStack.Push(476);
					goto case 15;
				}
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(476);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 477;
					break;
				}
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				Expect(160, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				Expect(137, la); // "Imports"
				currentState = 479;
				break;
			}
			case 479: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 480;
			}
			case 480: {
				if (la == null) { currentState = 480; break; }
				if (set[38].Get(la.kind)) {
					currentState = 480;
					break;
				} else {
					goto case 15;
				}
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				Expect(173, la); // "Option"
				currentState = 482;
				break;
			}
			case 482: {
				if (la == null) { currentState = 482; break; }
				if (set[38].Get(la.kind)) {
					currentState = 482;
					break;
				} else {
					goto case 15;
				}
			}
		}

		ApplyToken(la);
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