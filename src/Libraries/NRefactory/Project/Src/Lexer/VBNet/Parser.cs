using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 37;
	const int endOfStatementTerminatorAndBlock = 214;
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
			case 51:
			case 215:
			case 503:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 200:
			case 479:
			case 494:
			case 502:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 10:
			case 11:
			case 107:
			case 164:
			case 165:
			case 216:
			case 372:
			case 373:
			case 388:
			case 389:
			case 390:
			case 415:
			case 416:
			case 439:
			case 440:
			case 495:
			case 496:
			case 521:
			case 522:
			case 542:
			case 543:
				return set[6];
			case 12:
			case 13:
			case 485:
			case 497:
			case 498:
				return set[7];
			case 14:
			case 445:
			case 486:
			case 499:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 15:
			case 16:
			case 20:
			case 207:
			case 210:
			case 211:
			case 221:
			case 235:
			case 239:
			case 260:
			case 276:
			case 287:
			case 290:
			case 296:
			case 301:
			case 310:
			case 311:
			case 324:
			case 332:
			case 356:
			case 419:
			case 429:
			case 446:
			case 450:
			case 459:
			case 462:
			case 489:
			case 500:
			case 506:
			case 549:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 17:
			case 321:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 18:
			case 151:
			case 168:
			case 246:
			case 270:
			case 341:
			case 354:
			case 368:
			case 474:
			case 487:
			case 504:
			case 516:
			case 531:
			case 539:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					return a;
				}
			case 19:
			case 21:
			case 111:
			case 120:
			case 136:
			case 152:
			case 169:
			case 247:
			case 271:
			case 342:
			case 344:
			case 346:
			case 355:
			case 369:
			case 399:
			case 469:
			case 475:
			case 488:
			case 505:
			case 517:
			case 555:
				return set[8];
			case 22:
			case 25:
				return set[9];
			case 23:
				return set[10];
			case 24:
			case 57:
			case 61:
			case 116:
			case 327:
			case 402:
				return set[11];
			case 26:
			case 126:
			case 133:
			case 137:
			case 201:
			case 376:
			case 395:
			case 398:
			case 441:
			case 442:
			case 456:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 27:
			case 28:
			case 118:
			case 119:
				return set[12];
			case 29:
			case 204:
			case 352:
			case 379:
			case 397:
			case 413:
			case 444:
			case 458:
			case 473:
			case 492:
			case 509:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 30:
			case 31:
			case 34:
			case 35:
			case 407:
			case 408:
				return set[13];
			case 32:
				return set[14];
			case 33:
			case 128:
			case 135:
			case 330:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 36:
			case 121:
			case 130:
			case 351:
			case 353:
			case 358:
			case 366:
			case 406:
			case 410:
			case 527:
			case 533:
			case 541:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 37:
			case 38:
			case 40:
			case 41:
			case 42:
			case 45:
			case 59:
			case 109:
			case 127:
			case 129:
			case 131:
			case 134:
			case 143:
			case 145:
			case 187:
			case 220:
			case 224:
			case 226:
			case 227:
			case 243:
			case 259:
			case 264:
			case 274:
			case 280:
			case 282:
			case 286:
			case 289:
			case 295:
			case 306:
			case 308:
			case 314:
			case 329:
			case 331:
			case 367:
			case 392:
			case 404:
			case 405:
			case 515:
				return set[15];
			case 39:
			case 43:
			case 52:
				return set[16];
			case 44:
			case 53:
			case 54:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 46:
			case 60:
			case 536:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 47:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 48:
			case 81:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 49:
				return set[17];
			case 50:
			case 62:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 55:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 56:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 58:
			case 167:
			case 170:
			case 171:
			case 273:
			case 551:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 63:
			case 292:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 64:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 65:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 67:
			case 238:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 70:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 71:
			case 383:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 72:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 73:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 74:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 75:
			case 298:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 77:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 78:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 80:
			case 254:
			case 261:
			case 277:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 83:
			case 174:
			case 179:
			case 181:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 84:
			case 176:
			case 180:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 87:
			case 209:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 88:
			case 199:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 91:
			case 144:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 95:
			case 451:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 96:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 98:
			case 157:
			case 186:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 100:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 101:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 103:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 104:
			case 198:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 106:
				return set[18];
			case 108:
				return set[19];
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 112:
				return set[20];
			case 113:
				return set[21];
			case 114:
			case 115:
			case 400:
			case 401:
				return set[22];
			case 117:
				return set[23];
			case 122:
			case 123:
			case 257:
			case 266:
				return set[24];
			case 124:
				return set[25];
			case 125:
			case 313:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 132:
				return set[26];
			case 138:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 139:
			case 140:
				return set[27];
			case 141:
			case 147:
			case 154:
			case 192:
			case 196:
			case 234:
			case 335:
			case 347:
			case 396:
			case 465:
			case 480:
			case 550:
				return set[28];
			case 142:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 146:
			case 161:
			case 178:
			case 183:
			case 189:
			case 191:
			case 195:
			case 197:
				return set[29];
			case 148:
			case 149:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 150:
			case 153:
			case 258:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 155:
			case 156:
			case 158:
			case 160:
			case 162:
			case 163:
			case 172:
			case 177:
			case 182:
			case 190:
			case 194:
				return set[30];
			case 159:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 166:
				return set[31];
			case 173:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 175:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 184:
			case 185:
				return set[32];
			case 188:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 193:
				return set[33];
			case 202:
			case 203:
			case 377:
			case 378:
			case 411:
			case 412:
			case 471:
			case 472:
			case 490:
			case 491:
			case 507:
			case 508:
				return set[34];
			case 205:
			case 206:
				return set[35];
			case 208:
			case 222:
			case 237:
			case 291:
			case 333:
			case 382:
			case 427:
			case 447:
			case 460:
			case 501:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 212:
			case 213:
				return set[36];
			case 214:
				return set[37];
			case 217:
				return set[38];
			case 218:
			case 219:
			case 319:
				return set[39];
			case 223:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 225:
			case 265:
			case 281:
				return set[40];
			case 228:
			case 229:
			case 262:
			case 263:
			case 278:
			case 279:
				return set[41];
			case 230:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 231:
				return set[42];
			case 232:
			case 250:
				return set[43];
			case 233:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 236:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 240:
			case 241:
				return set[44];
			case 242:
			case 248:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 244:
			case 245:
				return set[45];
			case 249:
				return set[46];
			case 251:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 252:
			case 253:
				return set[47];
			case 255:
			case 256:
				return set[48];
			case 267:
			case 268:
				return set[49];
			case 269:
				return set[50];
			case 272:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 275:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 283:
				return set[51];
			case 284:
			case 288:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 285:
				return set[52];
			case 293:
			case 294:
				return set[53];
			case 297:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 299:
			case 300:
				return set[54];
			case 302:
			case 303:
				return set[55];
			case 304:
			case 470:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 305:
			case 307:
				return set[56];
			case 309:
			case 315:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 312:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 316:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 317:
			case 318:
			case 322:
			case 323:
			case 380:
			case 381:
				return set[57];
			case 320:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 325:
			case 326:
				return set[58];
			case 328:
				return set[59];
			case 334:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 336:
			case 337:
			case 348:
			case 349:
				return set[60];
			case 338:
			case 350:
				return set[61];
			case 339:
				return set[62];
			case 340:
			case 345:
				return set[63];
			case 343:
				return set[64];
			case 357:
			case 359:
			case 360:
			case 443:
			case 457:
				return set[65];
			case 361:
			case 362:
				return set[66];
			case 363:
			case 364:
				return set[67];
			case 365:
			case 370:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 371:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 374:
			case 375:
				return set[68];
			case 384:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 385:
				return set[69];
			case 386:
				return set[70];
			case 387:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 391:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 393:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 394:
				return set[71];
			case 403:
				return set[72];
			case 409:
				return set[73];
			case 414:
			case 428:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 417:
			case 418:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 420:
			case 421:
				return set[74];
			case 422:
			case 423:
				return set[75];
			case 424:
			case 425:
			case 426:
			case 431:
			case 437:
				return set[76];
			case 430:
				return set[77];
			case 432:
			case 433:
				return set[78];
			case 434:
			case 435:
			case 520:
				return set[79];
			case 436:
				return set[80];
			case 438:
			case 448:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 449:
				return set[81];
			case 452:
			case 454:
			case 463:
			case 464:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 453:
				return set[82];
			case 455:
				return set[83];
			case 461:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 466:
			case 467:
				return set[84];
			case 468:
			case 476:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 477:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 478:
				return set[85];
			case 481:
			case 482:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 483:
			case 493:
			case 552:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 484:
				return set[86];
			case 510:
				return set[87];
			case 511:
			case 519:
				return set[88];
			case 512:
			case 513:
				return set[89];
			case 514:
			case 518:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 523:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 524:
			case 528:
				return set[90];
			case 525:
			case 529:
			case 538:
				return set[91];
			case 526:
			case 530:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 532:
			case 534:
			case 540:
				return set[92];
			case 535:
			case 537:
				return set[93];
			case 544:
				return set[94];
			case 545:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 546:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 547:
			case 548:
				return set[95];
			case 553:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 554:
				return set[96];
			case 556:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 557:
				return set[97];
			case 558:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 559:
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
	bool identifierExpected = false;
	bool nextTokenIsStartOfImportsOrAccessExpression = false;
	bool isMissingModifier = false;
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
		identifierExpected = false;
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
					goto case 556;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					PushContext(Context.Importable, la, t);
					goto case 546;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 371;
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
					currentState = 542;
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
					goto case 371;
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
						PushContext(Context.TypeDeclaration, la, t);
						goto case 414;
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
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
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
					currentState = 411;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				if (la == null) { currentState = 14; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 18;
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
				if (la == null) { currentState = 18; break; }
				Expect(63, la); // "As"
				currentState = 19;
				break;
			}
			case 19: {
				stateStack.Push(20);
				goto case 21;
			}
			case 20: {
				PopContext();
				goto case 15;
			}
			case 21: {
				if (la == null) { currentState = 21; break; }
				if (la.kind == 130) {
					currentState = 22;
					break;
				} else {
					if (set[28].Get(la.kind)) {
						currentState = 22;
						break;
					} else {
						if (set[100].Get(la.kind)) {
							currentState = 22;
							break;
						} else {
							Error(la);
							goto case 22;
						}
					}
				}
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (la.kind == 37) {
					stateStack.Push(22);
					goto case 26;
				} else {
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (la.kind == 26) {
					currentState = 24;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 24: {
				stateStack.Push(25);
				goto case 61;
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 37) {
					stateStack.Push(25);
					goto case 26;
				} else {
					goto case 23;
				}
			}
			case 26: {
				if (la == null) { currentState = 26; break; }
				Expect(37, la); // "("
				currentState = 27;
				break;
			}
			case 27: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 28;
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (la.kind == 169) {
					currentState = 409;
					break;
				} else {
					if (set[13].Get(la.kind)) {
						goto case 30;
					} else {
						Error(la);
						goto case 29;
					}
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 30: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 31;
			}
			case 31: {
				if (la == null) { currentState = 31; break; }
				if (set[14].Get(la.kind)) {
					stateStack.Push(29);
					nextTokenIsPotentialStartOfExpression = true;
					goto case 32;
				} else {
					goto case 29;
				}
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (set[15].Get(la.kind)) {
					goto case 405;
				} else {
					if (la.kind == 22) {
						goto case 33;
					} else {
						goto case 6;
					}
				}
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				currentState = 34;
				break;
			}
			case 34: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 35;
			}
			case 35: {
				if (la == null) { currentState = 35; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(36);
					goto case 37;
				} else {
					goto case 36;
				}
			}
			case 36: {
				if (la == null) { currentState = 36; break; }
				if (la.kind == 22) {
					goto case 33;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 37: {
				PushContext(Context.Expression, la, t);
				goto case 38;
			}
			case 38: {
				stateStack.Push(39);
				goto case 40;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (set[101].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 40: {
				PushContext(Context.Expression, la, t);
				goto case 41;
			}
			case 41: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 42;
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				if (set[102].Get(la.kind)) {
					currentState = 41;
					break;
				} else {
					if (set[24].Get(la.kind)) {
						stateStack.Push(112);
						goto case 122;
					} else {
						if (la.kind == 220) {
							currentState = 109;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(43);
								PushContext(Context.ObjectCreation, la, t);
								goto case 48;
							} else {
								if (la.kind == 35) {
									stateStack.Push(43);
									goto case 44;
								} else {
									Error(la);
									goto case 43;
								}
							}
						}
					}
				}
			}
			case 43: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				Expect(35, la); // "{"
				currentState = 45;
				break;
			}
			case 45: {
				stateStack.Push(46);
				goto case 37;
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				if (la.kind == 22) {
					currentState = 45;
					break;
				} else {
					goto case 47;
				}
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				Expect(162, la); // "New"
				currentState = 49;
				break;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (set[8].Get(la.kind)) {
					stateStack.Push(106);
					goto case 21;
				} else {
					goto case 50;
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				if (la.kind == 233) {
					currentState = 53;
					break;
				} else {
					goto case 51;
				}
			}
			case 51: {
				Error(la);
				goto case 52;
			}
			case 52: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 53: {
				stateStack.Push(52);
				goto case 54;
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				Expect(35, la); // "{"
				currentState = 55;
				break;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (la.kind == 147) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				Expect(26, la); // "."
				currentState = 57;
				break;
			}
			case 57: {
				stateStack.Push(58);
				goto case 61;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				Expect(20, la); // "="
				currentState = 59;
				break;
			}
			case 59: {
				stateStack.Push(60);
				goto case 37;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				if (la.kind == 22) {
					currentState = 55;
					break;
				} else {
					goto case 47;
				}
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (la.kind == 2) {
					goto case 105;
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
								goto case 104;
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
												goto case 103;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 102;
													} else {
														if (la.kind == 65) {
															goto case 101;
														} else {
															if (la.kind == 66) {
																goto case 100;
															} else {
																if (la.kind == 67) {
																	goto case 99;
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
																				goto case 98;
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
																																		goto case 97;
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
																																					goto case 96;
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
																																																goto case 95;
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
																																																						goto case 94;
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
																																																									goto case 93;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 92;
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
																																																																		goto case 91;
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
																																																																							goto case 90;
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
																																																																										goto case 89;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 88;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 87;
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
																																																																																			goto case 86;
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
																																																																																									goto case 85;
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
																																																																																													goto case 84;
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
																																																																																																goto case 83;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 82;
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
																																																																																																																goto case 81;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 80;
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
																																																																																																																								goto case 79;
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
																																																																																																																														goto case 78;
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
																																																																																																																																						goto case 77;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 76;
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
																																																																																																																																																			goto case 75;
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
																																																																																																																																																									goto case 74;
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
																																																																																																																																																												goto case 73;
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
																																																																																																																																																															goto case 72;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 71;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 70;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 69;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 68;
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
																																																																																																																																																																								goto case 67;
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
																																																																																																																																																																													goto case 66;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 65;
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
																																																																																																																																																																																				goto case 64;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 63;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 62;
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
				currentState = stateStack.Pop();
				break;
			}
			case 102: {
				if (la == null) { currentState = 102; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 103: {
				if (la == null) { currentState = 103; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 104: {
				if (la == null) { currentState = 104; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 105: {
				if (la == null) { currentState = 105; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 106: {
				if (la == null) { currentState = 106; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 107;
						break;
					} else {
						goto case 50;
					}
				} else {
					goto case 52;
				}
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				if (la.kind == 35) {
					stateStack.Push(52);
					goto case 44;
				} else {
					if (set[19].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process From again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 51;
					}
				}
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				currentState = 52;
				break;
			}
			case 109: {
				stateStack.Push(110);
				goto case 40;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				Expect(144, la); // "Is"
				currentState = 111;
				break;
			}
			case 111: {
				stateStack.Push(43);
				goto case 21;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(112);
					goto case 113;
				} else {
					goto case 43;
				}
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				if (la.kind == 37) {
					currentState = 118;
					break;
				} else {
					if (set[103].Get(la.kind)) {
						currentState = 114;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 114: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 115;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				if (la.kind == 10) {
					currentState = 116;
					break;
				} else {
					goto case 116;
				}
			}
			case 116: {
				stateStack.Push(117);
				goto case 61;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 118: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 119;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				if (la.kind == 169) {
					currentState = 120;
					break;
				} else {
					if (set[13].Get(la.kind)) {
						goto case 30;
					} else {
						goto case 6;
					}
				}
			}
			case 120: {
				stateStack.Push(121);
				goto case 21;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				if (la.kind == 22) {
					currentState = 120;
					break;
				} else {
					goto case 29;
				}
			}
			case 122: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 123;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				if (set[104].Get(la.kind)) {
					currentState = 124;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 404;
						break;
					} else {
						if (set[105].Get(la.kind)) {
							currentState = 124;
							break;
						} else {
							if (set[103].Get(la.kind)) {
								currentState = 400;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 398;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 395;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(124);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 384;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(124);
												goto case 200;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(124);
													PushContext(Context.Query, la, t);
													goto case 138;
												} else {
													if (set[26].Get(la.kind)) {
														stateStack.Push(124);
														goto case 132;
													} else {
														if (la.kind == 135) {
															stateStack.Push(124);
															goto case 125;
														} else {
															Error(la);
															goto case 124;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 124: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				Expect(135, la); // "If"
				currentState = 126;
				break;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				Expect(37, la); // "("
				currentState = 127;
				break;
			}
			case 127: {
				stateStack.Push(128);
				goto case 37;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				Expect(22, la); // ","
				currentState = 129;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 37;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.kind == 22) {
					currentState = 131;
					break;
				} else {
					goto case 29;
				}
			}
			case 131: {
				stateStack.Push(29);
				goto case 37;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (set[106].Get(la.kind)) {
					currentState = 137;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 133;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				Expect(37, la); // "("
				currentState = 134;
				break;
			}
			case 134: {
				stateStack.Push(135);
				goto case 37;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				Expect(22, la); // ","
				currentState = 136;
				break;
			}
			case 136: {
				stateStack.Push(29);
				goto case 21;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				Expect(37, la); // "("
				currentState = 131;
				break;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (la.kind == 126) {
					stateStack.Push(139);
					goto case 199;
				} else {
					if (la.kind == 58) {
						stateStack.Push(139);
						goto case 198;
					} else {
						Error(la);
						goto case 139;
					}
				}
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(139);
					goto case 140;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				if (la.kind == 126) {
					currentState = 196;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 192;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 190;
							break;
						} else {
							if (la.kind == 107) {
								goto case 93;
							} else {
								if (la.kind == 230) {
									currentState = 37;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 186;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 184;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 182;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 155;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 141;
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
			case 141: {
				stateStack.Push(142);
				goto case 147;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				Expect(171, la); // "On"
				currentState = 143;
				break;
			}
			case 143: {
				stateStack.Push(144);
				goto case 37;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				Expect(116, la); // "Equals"
				currentState = 145;
				break;
			}
			case 145: {
				stateStack.Push(146);
				goto case 37;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (la.kind == 22) {
					currentState = 143;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 147: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(148);
				goto case 154;
			}
			case 148: {
				PopContext();
				goto case 149;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 151;
				} else {
					goto case 150;
				}
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				Expect(138, la); // "In"
				currentState = 37;
				break;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(63, la); // "As"
				currentState = 152;
				break;
			}
			case 152: {
				stateStack.Push(153);
				goto case 21;
			}
			case 153: {
				PopContext();
				goto case 150;
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (set[88].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 95;
					} else {
						goto case 6;
					}
				}
			}
			case 155: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 156;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				if (la.kind == 146) {
					goto case 174;
				} else {
					if (set[30].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 158;
							break;
						} else {
							if (set[30].Get(la.kind)) {
								goto case 172;
							} else {
								Error(la);
								goto case 157;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				Expect(70, la); // "By"
				currentState = 158;
				break;
			}
			case 158: {
				stateStack.Push(159);
				goto case 162;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (la.kind == 22) {
					currentState = 158;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 160;
					break;
				}
			}
			case 160: {
				stateStack.Push(161);
				goto case 162;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.kind == 22) {
					currentState = 160;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 162: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 163;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (set[28].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(164);
					goto case 154;
				} else {
					goto case 37;
				}
			}
			case 164: {
				PopContext();
				goto case 165;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 168;
				} else {
					if (la.kind == 20) {
						goto case 167;
					} else {
						if (set[31].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process Identifier again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

						} else {
							Error(la);
							goto case 37;
						}
					}
				}
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				currentState = 37;
				break;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				currentState = 37;
				break;
			}
			case 168: {
				if (la == null) { currentState = 168; break; }
				Expect(63, la); // "As"
				currentState = 169;
				break;
			}
			case 169: {
				stateStack.Push(170);
				goto case 21;
			}
			case 170: {
				PopContext();
				goto case 171;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				Expect(20, la); // "="
				currentState = 37;
				break;
			}
			case 172: {
				stateStack.Push(173);
				goto case 162;
			}
			case 173: {
				if (la == null) { currentState = 173; break; }
				if (la.kind == 22) {
					currentState = 172;
					break;
				} else {
					goto case 157;
				}
			}
			case 174: {
				stateStack.Push(175);
				goto case 181;
			}
			case 175: {
				if (la == null) { currentState = 175; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 179;
						break;
					} else {
						if (la.kind == 146) {
							goto case 174;
						} else {
							Error(la);
							goto case 175;
						}
					}
				} else {
					goto case 176;
				}
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				Expect(143, la); // "Into"
				currentState = 177;
				break;
			}
			case 177: {
				stateStack.Push(178);
				goto case 162;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (la.kind == 22) {
					currentState = 177;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 179: {
				stateStack.Push(180);
				goto case 181;
			}
			case 180: {
				stateStack.Push(175);
				goto case 176;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				Expect(146, la); // "Join"
				currentState = 141;
				break;
			}
			case 182: {
				stateStack.Push(183);
				goto case 162;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (la.kind == 22) {
					currentState = 182;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 184: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 185;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 231) {
					currentState = 37;
					break;
				} else {
					goto case 37;
				}
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				Expect(70, la); // "By"
				currentState = 187;
				break;
			}
			case 187: {
				stateStack.Push(188);
				goto case 37;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 64) {
					currentState = 189;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 189;
						break;
					} else {
						Error(la);
						goto case 189;
					}
				}
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				if (la.kind == 22) {
					currentState = 187;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 190: {
				stateStack.Push(191);
				goto case 162;
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				if (la.kind == 22) {
					currentState = 190;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 192: {
				stateStack.Push(193);
				goto case 147;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (set[27].Get(la.kind)) {
					stateStack.Push(193);
					goto case 140;
				} else {
					Expect(143, la); // "Into"
					currentState = 194;
					break;
				}
			}
			case 194: {
				stateStack.Push(195);
				goto case 162;
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				if (la.kind == 22) {
					currentState = 194;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 196: {
				stateStack.Push(197);
				goto case 147;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (la.kind == 22) {
					currentState = 196;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(58, la); // "Aggregate"
				currentState = 192;
				break;
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				Expect(126, la); // "From"
				currentState = 196;
				break;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (la.kind == 210) {
					currentState = 376;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 201;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				Expect(37, la); // "("
				currentState = 202;
				break;
			}
			case 202: {
				SetIdentifierExpected(la);
				goto case 203;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(204);
					goto case 357;
				} else {
					goto case 204;
				}
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				Expect(38, la); // ")"
				currentState = 205;
				break;
			}
			case 205: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 206;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				if (set[15].Get(la.kind)) {
					goto case 37;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							PushContext(Context.Type, la, t);
							goto case 354;
						} else {
							goto case 207;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 207: {
				stateStack.Push(208);
				goto case 210;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				Expect(113, la); // "End"
				currentState = 209;
				break;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 210: {
				PushContext(Context.Body, la, t);
				goto case 211;
			}
			case 211: {
				stateStack.Push(212);
				goto case 15;
			}
			case 212: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 213;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (set[107].Get(la.kind)) {
					if (set[57].Get(la.kind)) {
						if (set[39].Get(la.kind)) {
							stateStack.Push(211);
							goto case 218;
						} else {
							goto case 211;
						}
					} else {
						if (la.kind == 113) {
							currentState = 216;
							break;
						} else {
							goto case 215;
						}
					}
				} else {
					goto case 214;
				}
			}
			case 214: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 215: {
				Error(la);
				goto case 212;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 211;
				} else {
					if (set[38].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 215;
					}
				}
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				currentState = 212;
				break;
			}
			case 218: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 219;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 335;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 331;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 329;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 327;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 308;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 293;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 289;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 283;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 255;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 251;
																break;
															} else {
																goto case 251;
															}
														} else {
															if (la.kind == 194) {
																currentState = 249;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 228;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 235;
																break;
															} else {
																if (set[108].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 232;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 231;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 230;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 73;
																				} else {
																					if (la.kind == 195) {
																						currentState = 228;
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
																		currentState = 226;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 224;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 220;
																				break;
																			} else {
																				if (set[109].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 37;
																						break;
																					} else {
																						goto case 37;
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
			case 220: {
				stateStack.Push(221);
				goto case 37;
			}
			case 221: {
				stateStack.Push(222);
				goto case 210;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(113, la); // "End"
				currentState = 223;
				break;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 224: {
				stateStack.Push(225);
				goto case 37;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (la.kind == 22) {
					currentState = 224;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 226: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 227;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				if (la.kind == 184) {
					currentState = 37;
					break;
				} else {
					goto case 37;
				}
			}
			case 228: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 229;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (set[15].Get(la.kind)) {
					goto case 37;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				if (la.kind == 108) {
					goto case 92;
				} else {
					if (la.kind == 124) {
						goto case 89;
					} else {
						if (la.kind == 231) {
							goto case 63;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 108) {
					goto case 92;
				} else {
					if (la.kind == 124) {
						goto case 89;
					} else {
						if (la.kind == 231) {
							goto case 63;
						} else {
							if (la.kind == 197) {
								goto case 75;
							} else {
								if (la.kind == 210) {
									goto case 71;
								} else {
									if (la.kind == 127) {
										goto case 87;
									} else {
										if (la.kind == 186) {
											goto case 76;
										} else {
											if (la.kind == 218) {
												goto case 67;
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
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (set[28].Get(la.kind)) {
					goto case 234;
				} else {
					if (la.kind == 5) {
						goto case 233;
					} else {
						goto case 6;
					}
				}
			}
			case 233: {
				if (la == null) { currentState = 233; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 235: {
				stateStack.Push(236);
				goto case 210;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				if (la.kind == 75) {
					currentState = 240;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 239;
						break;
					} else {
						goto case 237;
					}
				}
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				Expect(113, la); // "End"
				currentState = 238;
				break;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 239: {
				stateStack.Push(237);
				goto case 210;
			}
			case 240: {
				SetIdentifierExpected(la);
				goto case 241;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (set[28].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(244);
					goto case 154;
				} else {
					goto case 242;
				}
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (la.kind == 229) {
					currentState = 243;
					break;
				} else {
					goto case 235;
				}
			}
			case 243: {
				stateStack.Push(235);
				goto case 37;
			}
			case 244: {
				PopContext();
				goto case 245;
			}
			case 245: {
				if (la == null) { currentState = 245; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 246;
				} else {
					goto case 242;
				}
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				Expect(63, la); // "As"
				currentState = 247;
				break;
			}
			case 247: {
				stateStack.Push(248);
				goto case 21;
			}
			case 248: {
				PopContext();
				goto case 242;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 163) {
					goto case 80;
				} else {
					goto case 250;
				}
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				if (la.kind == 5) {
					goto case 233;
				} else {
					if (set[28].Get(la.kind)) {
						goto case 234;
					} else {
						goto case 6;
					}
				}
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				Expect(118, la); // "Error"
				currentState = 252;
				break;
			}
			case 252: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 253;
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				if (set[15].Get(la.kind)) {
					goto case 37;
				} else {
					if (la.kind == 132) {
						currentState = 250;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 254;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 255: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 256;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (set[24].Get(la.kind)) {
					stateStack.Push(273);
					goto case 266;
				} else {
					if (la.kind == 110) {
						currentState = 257;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 257: {
				stateStack.Push(258);
				goto case 266;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(138, la); // "In"
				currentState = 259;
				break;
			}
			case 259: {
				stateStack.Push(260);
				goto case 37;
			}
			case 260: {
				stateStack.Push(261);
				goto case 210;
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
				goto case 37;
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
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(267);
				goto case 122;
			}
			case 267: {
				PopContext();
				goto case 268;
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (la.kind == 33) {
					currentState = 269;
					break;
				} else {
					goto case 269;
				}
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(269);
					goto case 113;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 270;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				Expect(63, la); // "As"
				currentState = 271;
				break;
			}
			case 271: {
				stateStack.Push(272);
				goto case 21;
			}
			case 272: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 273: {
				if (la == null) { currentState = 273; break; }
				Expect(20, la); // "="
				currentState = 274;
				break;
			}
			case 274: {
				stateStack.Push(275);
				goto case 37;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				if (la.kind == 205) {
					currentState = 282;
					break;
				} else {
					goto case 276;
				}
			}
			case 276: {
				stateStack.Push(277);
				goto case 210;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				Expect(163, la); // "Next"
				currentState = 278;
				break;
			}
			case 278: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 279;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (set[15].Get(la.kind)) {
					goto case 280;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 280: {
				stateStack.Push(281);
				goto case 37;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 22) {
					currentState = 280;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 282: {
				stateStack.Push(276);
				goto case 37;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 286;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(284);
						goto case 210;
					} else {
						goto case 6;
					}
				}
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				Expect(152, la); // "Loop"
				currentState = 285;
				break;
			}
			case 285: {
				if (la == null) { currentState = 285; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 37;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 286: {
				stateStack.Push(287);
				goto case 37;
			}
			case 287: {
				stateStack.Push(288);
				goto case 210;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 289: {
				stateStack.Push(290);
				goto case 37;
			}
			case 290: {
				stateStack.Push(291);
				goto case 210;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				Expect(113, la); // "End"
				currentState = 292;
				break;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 293: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 294;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.kind == 74) {
					currentState = 295;
					break;
				} else {
					goto case 295;
				}
			}
			case 295: {
				stateStack.Push(296);
				goto case 37;
			}
			case 296: {
				stateStack.Push(297);
				goto case 15;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (la.kind == 74) {
					currentState = 299;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 298;
					break;
				}
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 299: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 300;
			}
			case 300: {
				if (la == null) { currentState = 300; break; }
				if (la.kind == 111) {
					currentState = 301;
					break;
				} else {
					if (set[55].Get(la.kind)) {
						goto case 302;
					} else {
						Error(la);
						goto case 301;
					}
				}
			}
			case 301: {
				stateStack.Push(297);
				goto case 210;
			}
			case 302: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 303;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (set[110].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 305;
						break;
					} else {
						goto case 305;
					}
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(304);
						goto case 37;
					} else {
						Error(la);
						goto case 304;
					}
				}
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				if (la.kind == 22) {
					currentState = 302;
					break;
				} else {
					goto case 301;
				}
			}
			case 305: {
				stateStack.Push(306);
				goto case 307;
			}
			case 306: {
				stateStack.Push(304);
				goto case 40;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
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
			case 308: {
				stateStack.Push(309);
				goto case 37;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				if (la.kind == 214) {
					currentState = 317;
					break;
				} else {
					goto case 310;
				}
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 311;
				} else {
					goto case 6;
				}
			}
			case 311: {
				stateStack.Push(312);
				goto case 210;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 316;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 314;
							break;
						} else {
							Error(la);
							goto case 311;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 313;
					break;
				}
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 314: {
				stateStack.Push(315);
				goto case 37;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (la.kind == 214) {
					currentState = 311;
					break;
				} else {
					goto case 311;
				}
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (la.kind == 135) {
					currentState = 314;
					break;
				} else {
					goto case 311;
				}
			}
			case 317: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 318;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (set[39].Get(la.kind)) {
					goto case 319;
				} else {
					goto case 310;
				}
			}
			case 319: {
				stateStack.Push(320);
				goto case 218;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.kind == 21) {
					currentState = 325;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 322;
						break;
					} else {
						goto case 321;
					}
				}
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 322: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 323;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (set[39].Get(la.kind)) {
					stateStack.Push(324);
					goto case 218;
				} else {
					goto case 324;
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (la.kind == 21) {
					currentState = 322;
					break;
				} else {
					goto case 321;
				}
			}
			case 325: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 326;
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				if (set[39].Get(la.kind)) {
					goto case 319;
				} else {
					goto case 320;
				}
			}
			case 327: {
				stateStack.Push(328);
				goto case 61;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 37) {
					currentState = 30;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 329: {
				stateStack.Push(330);
				goto case 37;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				Expect(22, la); // ","
				currentState = 37;
				break;
			}
			case 331: {
				stateStack.Push(332);
				goto case 37;
			}
			case 332: {
				stateStack.Push(333);
				goto case 210;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				Expect(113, la); // "End"
				currentState = 334;
				break;
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				if (la.kind == 233) {
					goto case 62;
				} else {
					if (la.kind == 211) {
						goto case 70;
					} else {
						goto case 6;
					}
				}
			}
			case 335: {
				PushContext(Context.Identifier, la, t);	
				stateStack.Push(336);
				goto case 154;
			}
			case 336: {
				PopContext();
				goto case 337;
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (la.kind == 33) {
					currentState = 338;
					break;
				} else {
					goto case 338;
				}
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 37) {
					currentState = 353;
					break;
				} else {
					goto case 339;
				}
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (la.kind == 22) {
					currentState = 347;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 341;
					} else {
						goto case 340;
					}
				}
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 20) {
					goto case 167;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				Expect(63, la); // "As"
				currentState = 342;
				break;
			}
			case 342: {
				stateStack.Push(343);
				goto case 21;
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (la.kind == 162) {
					currentState = 346;
					break;
				} else {
					goto case 344;
				}
			}
			case 344: {
				stateStack.Push(345);
				goto case 21;
			}
			case 345: {
				if (CurrentBlock.context == Context.ObjectCreation)
				PopContext();
				PopContext();

				goto case 340;
			}
			case 346: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 344;
			}
			case 347: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(348);
				goto case 154;
			}
			case 348: {
				PopContext();
				goto case 349;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				if (la.kind == 33) {
					currentState = 350;
					break;
				} else {
					goto case 350;
				}
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 37) {
					currentState = 351;
					break;
				} else {
					goto case 339;
				}
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 22) {
					currentState = 351;
					break;
				} else {
					goto case 352;
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				Expect(38, la); // ")"
				currentState = 339;
				break;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (la.kind == 22) {
					currentState = 353;
					break;
				} else {
					goto case 352;
				}
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				Expect(63, la); // "As"
				currentState = 355;
				break;
			}
			case 355: {
				stateStack.Push(356);
				goto case 21;
			}
			case 356: {
				PopContext();
				goto case 207;
			}
			case 357: {
				stateStack.Push(358);
				PushContext(Context.Parameter, la, t);
				goto case 359;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 22) {
					currentState = 357;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 359: {
				SetIdentifierExpected(la);
				goto case 360;
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 40) {
					stateStack.Push(359);
					goto case 371;
				} else {
					goto case 361;
				}
			}
			case 361: {
				SetIdentifierExpected(la);
				goto case 362;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (set[111].Get(la.kind)) {
					currentState = 361;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(363);
					goto case 154;
				}
			}
			case 363: {
				PopContext();
				goto case 364;
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 368;
				} else {
					goto case 365;
				}
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 20) {
					currentState = 367;
					break;
				} else {
					goto case 366;
				}
			}
			case 366: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 367: {
				stateStack.Push(366);
				goto case 37;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				Expect(63, la); // "As"
				currentState = 369;
				break;
			}
			case 369: {
				stateStack.Push(370);
				goto case 21;
			}
			case 370: {
				PopContext();
				goto case 365;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				Expect(40, la); // "<"
				currentState = 372;
				break;
			}
			case 372: {
				PushContext(Context.Attribute, la, t);
				goto case 373;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (set[112].Get(la.kind)) {
					currentState = 373;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 374;
					break;
				}
			}
			case 374: {
				PopContext();
				goto case 375;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 1) {
					goto case 17;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				Expect(37, la); // "("
				currentState = 377;
				break;
			}
			case 377: {
				SetIdentifierExpected(la);
				goto case 378;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(379);
					goto case 357;
				} else {
					goto case 379;
				}
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				Expect(38, la); // ")"
				currentState = 380;
				break;
			}
			case 380: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 381;
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				if (set[39].Get(la.kind)) {
					goto case 218;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(382);
						goto case 210;
					} else {
						goto case 6;
					}
				}
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				Expect(113, la); // "End"
				currentState = 383;
				break;
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 394;
					break;
				} else {
					stateStack.Push(385);
					goto case 387;
				}
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (la.kind == 17) {
					currentState = 386;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (la.kind == 16) {
					currentState = 385;
					break;
				} else {
					goto case 385;
				}
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 388;
				break;
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (set[113].Get(la.kind)) {
					if (set[114].Get(la.kind)) {
						currentState = 388;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(388);
							goto case 391;
						} else {
							Error(la);
							goto case 388;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 389;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (set[115].Get(la.kind)) {
					if (set[116].Get(la.kind)) {
						currentState = 389;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(389);
							goto case 391;
						} else {
							if (la.kind == 10) {
								stateStack.Push(389);
								goto case 387;
							} else {
								Error(la);
								goto case 389;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 390;
					break;
				}
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				if (set[117].Get(la.kind)) {
					if (set[118].Get(la.kind)) {
						currentState = 390;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(390);
							goto case 391;
						} else {
							Error(la);
							goto case 390;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 392;
				break;
			}
			case 392: {
				stateStack.Push(393);
				goto case 37;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 16) {
					currentState = 384;
					break;
				} else {
					goto case 384;
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				Expect(37, la); // "("
				currentState = 396;
				break;
			}
			case 396: {
				readXmlIdentifier = true;
				stateStack.Push(397);
				goto case 154;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				Expect(38, la); // ")"
				currentState = 124;
				break;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				Expect(37, la); // "("
				currentState = 399;
				break;
			}
			case 399: {
				stateStack.Push(397);
				goto case 21;
			}
			case 400: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 401;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (la.kind == 10) {
					currentState = 402;
					break;
				} else {
					goto case 402;
				}
			}
			case 402: {
				stateStack.Push(403);
				goto case 61;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (la.kind == 11) {
					currentState = 124;
					break;
				} else {
					goto case 124;
				}
			}
			case 404: {
				stateStack.Push(397);
				goto case 37;
			}
			case 405: {
				stateStack.Push(406);
				goto case 37;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 22) {
					currentState = 407;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 407: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 408;
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				if (set[15].Get(la.kind)) {
					goto case 405;
				} else {
					goto case 406;
				}
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				if (set[8].Get(la.kind)) {
					stateStack.Push(410);
					goto case 21;
				} else {
					goto case 410;
				}
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				if (la.kind == 22) {
					currentState = 409;
					break;
				} else {
					goto case 29;
				}
			}
			case 411: {
				SetIdentifierExpected(la);
				goto case 412;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(413);
					goto case 357;
				} else {
					goto case 413;
				}
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 155) {
					currentState = 415;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 415;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 415;
							break;
						} else {
							Error(la);
							goto case 415;
						}
					}
				}
			}
			case 415: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 416;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				currentState = 417;
				break;
			}
			case 417: {
				PopContext();
				goto case 418;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				if (la.kind == 37) {
					currentState = 523;
					break;
				} else {
					goto case 419;
				}
			}
			case 419: {
				stateStack.Push(420);
				goto case 15;
			}
			case 420: {
				SetIdentifierExpected(la);
				goto case 421;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (la.kind == 140) {
					currentState = 522;
					break;
				} else {
					goto case 422;
				}
			}
			case 422: {
				SetIdentifierExpected(la);
				goto case 423;
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 136) {
					currentState = 521;
					break;
				} else {
					goto case 424;
				}
			}
			case 424: {
				isMissingModifier = true; Console.WriteLine("is true");
				goto case 425;
			}
			case 425: {
				SetIdentifierExpected(la);
				goto case 426;
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (set[78].Get(la.kind)) {
					stateStack.Push(431);
					PushContext(Context.Member, la, t); isMissingModifier = true;
					goto case 432;
				} else {
					isMissingModifier = false; Console.WriteLine("is false");
					goto case 427;
				}
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				Expect(113, la); // "End"
				currentState = 428;
				break;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 155) {
					currentState = 429;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 429;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 429;
							break;
						} else {
							Error(la);
							goto case 429;
						}
					}
				}
			}
			case 429: {
				stateStack.Push(430);
				goto case 15;
			}
			case 430: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 431: {
				isMissingModifier = true;
				goto case 425;
			}
			case 432: {
				SetIdentifierExpected(la);
				goto case 433;
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (la.kind == 40) {
					stateStack.Push(432);
					goto case 371;
				} else {
					goto case 434;
				}
			}
			case 434: {
				SetIdentifierExpected(la);
				goto case 435;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (set[119].Get(la.kind)) {
					currentState = 520;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 436;
				}
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				if (set[87].Get(la.kind)) {
					stateStack.Push(437);
					SetIdentifierExpected(la);
					goto case 510;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(437);
						goto case 494;
					} else {
						if (la.kind == 101) {
							stateStack.Push(437);
							isMissingModifier = false;
							goto case 477;
						} else {
							if (la.kind == 119) {
								stateStack.Push(437);
								goto case 463;
							} else {
								if (la.kind == 98) {
									stateStack.Push(437);
									isMissingModifier = false;
									goto case 451;
								} else {
									if (la.kind == 172) {
										stateStack.Push(437);
										goto case 438;
									} else {
										Error(la);
										goto case 437;
									}
								}
							}
						}
					}
				}
			}
			case 437: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				Expect(172, la); // "Operator"
				currentState = 439;
				break;
			}
			case 439: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 440;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				currentState = 441;
				break;
			}
			case 441: {
				PopContext();
				goto case 442;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				Expect(37, la); // "("
				currentState = 443;
				break;
			}
			case 443: {
				stateStack.Push(444);
				goto case 357;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				Expect(38, la); // ")"
				currentState = 445;
				break;
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 63) {
					currentState = 449;
					break;
				} else {
					goto case 446;
				}
			}
			case 446: {
				stateStack.Push(447);
				goto case 210;
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				Expect(113, la); // "End"
				currentState = 448;
				break;
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				Expect(172, la); // "Operator"
				currentState = 15;
				break;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				if (la.kind == 40) {
					stateStack.Push(449);
					goto case 371;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(450);
					goto case 21;
				}
			}
			case 450: {
				PopContext();
				goto case 446;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				Expect(98, la); // "Custom"
				currentState = 452;
				break;
			}
			case 452: {
				stateStack.Push(453);
				goto case 463;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (set[83].Get(la.kind)) {
					goto case 455;
				} else {
					Expect(113, la); // "End"
					currentState = 454;
					break;
				}
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				Expect(119, la); // "Event"
				currentState = 15;
				break;
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				if (la.kind == 40) {
					stateStack.Push(455);
					goto case 371;
				} else {
					if (la.kind == 56) {
						currentState = 456;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 456;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 456;
								break;
							} else {
								Error(la);
								goto case 456;
							}
						}
					}
				}
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				Expect(37, la); // "("
				currentState = 457;
				break;
			}
			case 457: {
				stateStack.Push(458);
				goto case 357;
			}
			case 458: {
				if (la == null) { currentState = 458; break; }
				Expect(38, la); // ")"
				currentState = 459;
				break;
			}
			case 459: {
				stateStack.Push(460);
				goto case 210;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				Expect(113, la); // "End"
				currentState = 461;
				break;
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				if (la.kind == 56) {
					currentState = 462;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 462;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 462;
							break;
						} else {
							Error(la);
							goto case 462;
						}
					}
				}
			}
			case 462: {
				stateStack.Push(453);
				goto case 15;
			}
			case 463: {
				isMissingModifier = false;
				goto case 464;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				Expect(119, la); // "Event"
				currentState = 465;
				break;
			}
			case 465: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(466);
				goto case 154;
			}
			case 466: {
				PopContext();
				goto case 467;
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 474;
				} else {
					if (set[120].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 471;
							break;
						} else {
							goto case 468;
						}
					} else {
						Error(la);
						goto case 468;
					}
				}
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				if (la.kind == 136) {
					currentState = 469;
					break;
				} else {
					goto case 15;
				}
			}
			case 469: {
				stateStack.Push(470);
				goto case 21;
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				if (la.kind == 22) {
					currentState = 469;
					break;
				} else {
					goto case 15;
				}
			}
			case 471: {
				SetIdentifierExpected(la);
				goto case 472;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(473);
					goto case 357;
				} else {
					goto case 473;
				}
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				Expect(38, la); // ")"
				currentState = 468;
				break;
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				Expect(63, la); // "As"
				currentState = 475;
				break;
			}
			case 475: {
				stateStack.Push(476);
				goto case 21;
			}
			case 476: {
				PopContext();
				goto case 468;
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				Expect(101, la); // "Declare"
				currentState = 478;
				break;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 479;
					break;
				} else {
					goto case 479;
				}
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				if (la.kind == 210) {
					currentState = 480;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 480;
						break;
					} else {
						Error(la);
						goto case 480;
					}
				}
			}
			case 480: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(481);
				goto case 154;
			}
			case 481: {
				PopContext();
				goto case 482;
			}
			case 482: {
				if (la == null) { currentState = 482; break; }
				Expect(149, la); // "Lib"
				currentState = 483;
				break;
			}
			case 483: {
				if (la == null) { currentState = 483; break; }
				Expect(3, la); // LiteralString
				currentState = 484;
				break;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (la.kind == 59) {
					currentState = 493;
					break;
				} else {
					goto case 485;
				}
			}
			case 485: {
				if (la == null) { currentState = 485; break; }
				if (la.kind == 37) {
					currentState = 490;
					break;
				} else {
					goto case 486;
				}
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 487;
				} else {
					goto case 15;
				}
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				Expect(63, la); // "As"
				currentState = 488;
				break;
			}
			case 488: {
				stateStack.Push(489);
				goto case 21;
			}
			case 489: {
				PopContext();
				goto case 15;
			}
			case 490: {
				SetIdentifierExpected(la);
				goto case 491;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(492);
					goto case 357;
				} else {
					goto case 492;
				}
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				Expect(38, la); // ")"
				currentState = 486;
				break;
			}
			case 493: {
				if (la == null) { currentState = 493; break; }
				Expect(3, la); // LiteralString
				currentState = 485;
				break;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (la.kind == 210) {
					currentState = 495;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 495;
						break;
					} else {
						Error(la);
						goto case 495;
					}
				}
			}
			case 495: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 496;
			}
			case 496: {
				if (la == null) { currentState = 496; break; }
				currentState = 497;
				break;
			}
			case 497: {
				PopContext();
				goto case 498;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				if (la.kind == 37) {
					currentState = 507;
					break;
				} else {
					goto case 499;
				}
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 504;
				} else {
					goto case 500;
				}
			}
			case 500: {
				stateStack.Push(501);
				goto case 210;
			}
			case 501: {
				if (la == null) { currentState = 501; break; }
				Expect(113, la); // "End"
				currentState = 502;
				break;
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				if (la.kind == 210) {
					currentState = 15;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 15;
						break;
					} else {
						goto case 503;
					}
				}
			}
			case 503: {
				Error(la);
				goto case 15;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				Expect(63, la); // "As"
				currentState = 505;
				break;
			}
			case 505: {
				stateStack.Push(506);
				goto case 21;
			}
			case 506: {
				PopContext();
				goto case 500;
			}
			case 507: {
				SetIdentifierExpected(la);
				goto case 508;
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				if (set[65].Get(la.kind)) {
					stateStack.Push(509);
					goto case 357;
				} else {
					goto case 509;
				}
			}
			case 509: {
				if (la == null) { currentState = 509; break; }
				Expect(38, la); // ")"
				currentState = 499;
				break;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				if (la.kind == 88) {
					currentState = 511;
					break;
				} else {
					goto case 511;
				}
			}
			case 511: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(512);
				goto case 519;
			}
			case 512: {
				PopContext();
				goto case 513;
			}
			case 513: {
				if (la == null) { currentState = 513; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 516;
				} else {
					goto case 514;
				}
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (la.kind == 20) {
					currentState = 515;
					break;
				} else {
					goto case 15;
				}
			}
			case 515: {
				stateStack.Push(15);
				goto case 37;
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				Expect(63, la); // "As"
				currentState = 517;
				break;
			}
			case 517: {
				stateStack.Push(518);
				goto case 21;
			}
			case 518: {
				PopContext();
				goto case 514;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				if (set[105].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 104;
					} else {
						if (la.kind == 126) {
							goto case 88;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 520: {
				isMissingModifier = false;
				goto case 434;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				if (set[38].Get(la.kind)) {
					currentState = 521;
					break;
				} else {
					stateStack.Push(424);
					goto case 15;
				}
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				if (set[38].Get(la.kind)) {
					currentState = 522;
					break;
				} else {
					stateStack.Push(422);
					goto case 15;
				}
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				Expect(169, la); // "Of"
				currentState = 524;
				break;
			}
			case 524: {
				if (la == null) { currentState = 524; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 525;
					break;
				} else {
					goto case 525;
				}
			}
			case 525: {
				stateStack.Push(526);
				goto case 538;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 539;
				} else {
					goto case 527;
				}
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 22) {
					currentState = 528;
					break;
				} else {
					Expect(38, la); // ")"
					currentState = 419;
					break;
				}
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 529;
					break;
				} else {
					goto case 529;
				}
			}
			case 529: {
				stateStack.Push(530);
				goto case 538;
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 531;
				} else {
					goto case 527;
				}
			}
			case 531: {
				if (la == null) { currentState = 531; break; }
				Expect(63, la); // "As"
				currentState = 532;
				break;
			}
			case 532: {
				stateStack.Push(533);
				goto case 534;
			}
			case 533: {
				PopContext();
				goto case 527;
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				if (set[93].Get(la.kind)) {
					goto case 537;
				} else {
					if (la.kind == 35) {
						currentState = 535;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 535: {
				stateStack.Push(536);
				goto case 537;
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				if (la.kind == 22) {
					currentState = 535;
					break;
				} else {
					goto case 47;
				}
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				if (set[8].Get(la.kind)) {
					currentState = 22;
					break;
				} else {
					if (la.kind == 162) {
						goto case 81;
					} else {
						if (la.kind == 84) {
							goto case 97;
						} else {
							if (la.kind == 209) {
								goto case 72;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (la.kind == 2) {
					goto case 105;
				} else {
					if (la.kind == 62) {
						goto case 103;
					} else {
						if (la.kind == 64) {
							goto case 102;
						} else {
							if (la.kind == 65) {
								goto case 101;
							} else {
								if (la.kind == 66) {
									goto case 100;
								} else {
									if (la.kind == 67) {
										goto case 99;
									} else {
										if (la.kind == 70) {
											goto case 98;
										} else {
											if (la.kind == 87) {
												goto case 96;
											} else {
												if (la.kind == 104) {
													goto case 94;
												} else {
													if (la.kind == 107) {
														goto case 93;
													} else {
														if (la.kind == 116) {
															goto case 91;
														} else {
															if (la.kind == 121) {
																goto case 90;
															} else {
																if (la.kind == 133) {
																	goto case 86;
																} else {
																	if (la.kind == 139) {
																		goto case 85;
																	} else {
																		if (la.kind == 143) {
																			goto case 84;
																		} else {
																			if (la.kind == 146) {
																				goto case 83;
																			} else {
																				if (la.kind == 147) {
																					goto case 82;
																				} else {
																					if (la.kind == 170) {
																						goto case 79;
																					} else {
																						if (la.kind == 176) {
																							goto case 78;
																						} else {
																							if (la.kind == 184) {
																								goto case 77;
																							} else {
																								if (la.kind == 203) {
																									goto case 74;
																								} else {
																									if (la.kind == 212) {
																										goto case 69;
																									} else {
																										if (la.kind == 213) {
																											goto case 68;
																										} else {
																											if (la.kind == 223) {
																												goto case 66;
																											} else {
																												if (la.kind == 224) {
																													goto case 65;
																												} else {
																													if (la.kind == 230) {
																														goto case 64;
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
			case 539: {
				if (la == null) { currentState = 539; break; }
				Expect(63, la); // "As"
				currentState = 540;
				break;
			}
			case 540: {
				stateStack.Push(541);
				goto case 534;
			}
			case 541: {
				PopContext();
				goto case 527;
			}
			case 542: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 543;
			}
			case 543: {
				if (la == null) { currentState = 543; break; }
				if (set[38].Get(la.kind)) {
					currentState = 543;
					break;
				} else {
					PopContext();
					stateStack.Push(544);
					goto case 15;
				}
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(544);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 545;
					break;
				}
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				Expect(160, la); // "Namespace"
				currentState = 15;
				break;
			}
			case 546: {
				if (la == null) { currentState = 546; break; }
				Expect(137, la); // "Imports"
				currentState = 547;
				break;
			}
			case 547: {
				nextTokenIsStartOfImportsOrAccessExpression = true;

				if (la != null)
				CurrentBlock.lastExpressionStart = la.Location;

				goto case 548;
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				if (set[8].Get(la.kind)) {
					currentState = 554;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 550;
						break;
					} else {
						Error(la);
						goto case 549;
					}
				}
			}
			case 549: {
				PopContext();
				goto case 15;
			}
			case 550: {
				stateStack.Push(551);
				goto case 154;
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				Expect(20, la); // "="
				currentState = 552;
				break;
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				Expect(3, la); // LiteralString
				currentState = 553;
				break;
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 549;
				break;
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				if (la.kind == 37) {
					stateStack.Push(554);
					goto case 26;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 555;
						break;
					} else {
						goto case 549;
					}
				}
			}
			case 555: {
				stateStack.Push(549);
				goto case 21;
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				Expect(173, la); // "Option"
				currentState = 557;
				break;
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 559;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 558;
						break;
					} else {
						goto case 503;
					}
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 213) {
					currentState = 15;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 15;
						break;
					} else {
						goto case 503;
					}
				}
			}
			case 559: {
				if (la == null) { currentState = 559; break; }
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
		new BitArray(new int[] {-940564474, 1962934261, 8650975, 1108388124, 81767716, 17272068, -512676304, 4707}),
		new BitArray(new int[] {-940564474, 1962934229, 8650975, 1108388124, 81767716, 17272068, -512676304, 4707}),
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
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537696544, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537692448, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493745372, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {1, 256, 1048576, 537002112, 134217728, 436207617, 131200, 0}),
		new BitArray(new int[] {4, 1140850944, 25165903, -493876444, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850688, 25165903, -493876444, 537692192, 465376386, -2144073344, 1345}),
		new BitArray(new int[] {4, 1140850688, 25165903, -1030747868, 821280, 17110016, -2144073728, 65}),
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
		new BitArray(new int[] {0, 0, 0, 536871424, 536870912, 448266370, 384, 1280}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0})

	};

} // end Parser


}