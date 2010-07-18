using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 49;
	const int endOfStatementTerminatorAndBlock = 227;
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
			case 64:
			case 228:
			case 460:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 154:
			case 160:
			case 167:
			case 205:
			case 209:
			case 247:
			case 348:
			case 360:
			case 409:
			case 450:
			case 458:
			case 466:
			case 490:
			case 537:
			case 552:
			case 622:
				return set[6];
			case 10:
			case 491:
			case 492:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 11:
			case 18:
			case 19:
			case 32:
			case 220:
			case 223:
			case 224:
			case 234:
			case 248:
			case 252:
			case 273:
			case 289:
			case 300:
			case 303:
			case 309:
			case 314:
			case 323:
			case 324:
			case 337:
			case 345:
			case 369:
			case 468:
			case 484:
			case 493:
			case 502:
			case 519:
			case 523:
			case 532:
			case 535:
			case 561:
			case 571:
			case 576:
			case 600:
			case 621:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 12:
			case 13:
				return set[7];
			case 14:
			case 15:
				return set[8];
			case 16:
			case 221:
			case 235:
			case 250:
			case 304:
			case 346:
			case 395:
			case 500:
			case 520:
			case 533:
			case 572:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 17:
			case 464:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 20:
			case 334:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 21:
			case 22:
				return set[9];
			case 23:
			case 604:
				return set[10];
			case 24:
				return set[11];
			case 25:
				return set[12];
			case 26:
			case 27:
			case 120:
			case 177:
			case 178:
			case 229:
			case 385:
			case 386:
			case 401:
			case 402:
			case 403:
			case 477:
			case 478:
			case 512:
			case 513:
			case 567:
			case 568:
			case 614:
			case 615:
				return set[13];
			case 28:
			case 29:
			case 451:
			case 459:
			case 479:
			case 480:
			case 557:
			case 569:
			case 570:
				return set[14];
			case 30:
			case 164:
			case 181:
			case 259:
			case 283:
			case 354:
			case 367:
			case 381:
			case 439:
			case 447:
			case 482:
			case 546:
			case 559:
			case 574:
			case 587:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					return a;
				}
			case 31:
			case 33:
			case 124:
			case 133:
			case 149:
			case 165:
			case 182:
			case 260:
			case 284:
			case 355:
			case 357:
			case 359:
			case 368:
			case 382:
			case 412:
			case 454:
			case 474:
			case 483:
			case 541:
			case 547:
			case 560:
			case 575:
			case 588:
			case 593:
			case 596:
			case 599:
			case 606:
			case 609:
			case 627:
				return set[15];
			case 34:
			case 37:
				return set[16];
			case 35:
				return set[17];
			case 36:
			case 70:
			case 74:
			case 129:
			case 340:
			case 415:
				return set[18];
			case 38:
			case 139:
			case 146:
			case 150:
			case 214:
			case 389:
			case 408:
			case 411:
			case 514:
			case 515:
			case 529:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 39:
			case 40:
			case 131:
			case 132:
				return set[19];
			case 41:
			case 217:
			case 365:
			case 392:
			case 410:
			case 426:
			case 457:
			case 463:
			case 487:
			case 517:
			case 531:
			case 545:
			case 564:
			case 579:
			case 603:
			case 613:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 42:
			case 43:
			case 46:
			case 47:
			case 420:
			case 421:
				return set[20];
			case 44:
				return set[21];
			case 45:
			case 141:
			case 148:
			case 343:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 48:
			case 134:
			case 143:
			case 364:
			case 366:
			case 371:
			case 379:
			case 419:
			case 423:
			case 433:
			case 441:
			case 449:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 49:
			case 50:
			case 52:
			case 53:
			case 54:
			case 57:
			case 72:
			case 122:
			case 140:
			case 142:
			case 144:
			case 147:
			case 156:
			case 158:
			case 200:
			case 233:
			case 237:
			case 239:
			case 240:
			case 256:
			case 272:
			case 277:
			case 287:
			case 293:
			case 295:
			case 299:
			case 302:
			case 308:
			case 319:
			case 321:
			case 327:
			case 342:
			case 344:
			case 380:
			case 405:
			case 417:
			case 418:
			case 473:
			case 586:
				return set[22];
			case 51:
			case 55:
			case 65:
				return set[23];
			case 56:
			case 66:
			case 67:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 58:
			case 73:
			case 444:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 59:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 60:
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 61:
			case 62:
				return set[24];
			case 63:
			case 75:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 71:
			case 180:
			case 183:
			case 184:
			case 286:
			case 623:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 76:
			case 305:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 77:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 78:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 80:
			case 251:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 81:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 83:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 84:
			case 396:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 87:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 88:
			case 311:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 91:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 93:
			case 267:
			case 274:
			case 290:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 95:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 96:
			case 187:
			case 192:
			case 194:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 97:
			case 189:
			case 193:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 100:
			case 222:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 101:
			case 212:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 103:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 104:
			case 157:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 107:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 108:
			case 524:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 111:
			case 170:
			case 199:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 112:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 115:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 116:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 117:
			case 211:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 118:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 119:
				return set[25];
			case 121:
				return set[26];
			case 123:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 125:
				return set[27];
			case 126:
				return set[28];
			case 127:
			case 128:
			case 413:
			case 414:
				return set[29];
			case 130:
				return set[30];
			case 135:
			case 136:
			case 270:
			case 279:
				return set[31];
			case 137:
				return set[32];
			case 138:
			case 326:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 145:
				return set[33];
			case 151:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 152:
			case 153:
				return set[34];
			case 155:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 159:
			case 174:
			case 191:
			case 196:
			case 202:
			case 204:
			case 208:
			case 210:
				return set[35];
			case 161:
			case 162:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 163:
			case 166:
			case 271:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 168:
			case 169:
			case 171:
			case 173:
			case 175:
			case 176:
			case 185:
			case 190:
			case 195:
			case 203:
			case 207:
				return set[36];
			case 172:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 179:
				return set[37];
			case 186:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 188:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 197:
			case 198:
				return set[38];
			case 201:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 206:
				return set[39];
			case 213:
			case 476:
			case 551:
			case 566:
			case 573:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 215:
			case 216:
			case 390:
			case 391:
			case 455:
			case 456:
			case 461:
			case 462:
			case 485:
			case 486:
			case 543:
			case 544:
			case 562:
			case 563:
				return set[40];
			case 218:
			case 219:
				return set[41];
			case 225:
			case 226:
				return set[42];
			case 227:
				return set[43];
			case 230:
				return set[44];
			case 231:
			case 232:
			case 332:
				return set[45];
			case 236:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 238:
			case 278:
			case 294:
				return set[46];
			case 241:
			case 242:
			case 275:
			case 276:
			case 291:
			case 292:
				return set[47];
			case 243:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 244:
				return set[48];
			case 245:
			case 263:
				return set[49];
			case 246:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 249:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 253:
			case 254:
				return set[50];
			case 255:
			case 261:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 257:
			case 258:
				return set[51];
			case 262:
				return set[52];
			case 264:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 265:
			case 266:
				return set[53];
			case 268:
			case 269:
				return set[54];
			case 280:
			case 281:
				return set[55];
			case 282:
				return set[56];
			case 285:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 288:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 296:
				return set[57];
			case 297:
			case 301:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 298:
				return set[58];
			case 306:
			case 307:
				return set[59];
			case 310:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 312:
			case 313:
				return set[60];
			case 315:
			case 316:
				return set[61];
			case 317:
			case 542:
			case 594:
			case 595:
			case 597:
			case 607:
			case 608:
			case 610:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 318:
			case 320:
				return set[62];
			case 322:
			case 328:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 325:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 329:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 330:
			case 331:
			case 335:
			case 336:
			case 393:
			case 394:
				return set[63];
			case 333:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 338:
			case 339:
				return set[64];
			case 341:
				return set[65];
			case 347:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 349:
			case 350:
			case 361:
			case 362:
				return set[66];
			case 351:
			case 363:
				return set[67];
			case 352:
				return set[68];
			case 353:
			case 358:
				return set[69];
			case 356:
				return set[70];
			case 370:
			case 372:
			case 373:
			case 516:
			case 530:
				return set[71];
			case 374:
			case 375:
				return set[72];
			case 376:
			case 377:
				return set[73];
			case 378:
			case 383:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 384:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 387:
			case 388:
				return set[74];
			case 397:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 398:
				return set[75];
			case 399:
				return set[76];
			case 400:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 404:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 406:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 407:
				return set[77];
			case 416:
				return set[78];
			case 422:
				return set[79];
			case 424:
			case 425:
			case 577:
			case 578:
				return set[80];
			case 427:
			case 428:
			case 429:
			case 434:
			case 435:
			case 580:
			case 602:
			case 612:
				return set[81];
			case 430:
			case 436:
			case 446:
				return set[82];
			case 431:
			case 432:
			case 437:
			case 438:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 440:
			case 442:
			case 448:
				return set[83];
			case 443:
			case 445:
				return set[84];
			case 452:
			case 467:
			case 481:
			case 518:
			case 558:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 453:
			case 522:
				return set[85];
			case 465:
			case 470:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 469:
				return set[86];
			case 471:
				return set[87];
			case 472:
			case 585:
			case 589:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 475:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 488:
			case 489:
			case 501:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 494:
			case 495:
				return set[88];
			case 496:
			case 497:
				return set[89];
			case 498:
			case 499:
			case 510:
				return set[90];
			case 503:
				return set[91];
			case 504:
			case 505:
				return set[92];
			case 506:
			case 507:
			case 591:
				return set[93];
			case 508:
				return set[94];
			case 509:
				return set[95];
			case 511:
			case 521:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 525:
			case 527:
			case 536:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 526:
				return set[96];
			case 528:
				return set[97];
			case 534:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 538:
			case 539:
				return set[98];
			case 540:
			case 548:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 549:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 550:
				return set[99];
			case 553:
			case 554:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 555:
			case 565:
			case 624:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 556:
				return set[100];
			case 581:
				return set[101];
			case 582:
			case 590:
				return set[102];
			case 583:
			case 584:
				return set[103];
			case 592:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 598:
			case 605:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 601:
			case 611:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 616:
				return set[104];
			case 617:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 618:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 619:
			case 620:
				return set[105];
			case 625:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 626:
				return set[106];
			case 628:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 629:
				return set[107];
			case 630:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 631:
				return set[108];
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
		switchlbl: switch (currentState) {
			case 0: {
				PushContext(Context.Global, la, t);
				goto case 1;
			}
			case 1: {
				if (la == null) { currentState = 1; break; }
				if (la.kind == 173) {
					stateStack.Push(1);
					goto case 628;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					PushContext(Context.Importable, la, t);
					goto case 618;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 384;
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
					currentState = 614;
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
					goto case 384;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[109].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 488;
					} else {
						if (la.kind == 103) {
							currentState = 476;
							break;
						} else {
							if (la.kind == 115) {
								currentState = 466;
								break;
							} else {
								if (la.kind == 142) {
									currentState = 9;
									break;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 9: {
				stateStack.Push(10);
				goto case 167;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				if (la.kind == 37) {
					currentState = 611;
					break;
				} else {
					goto case 11;
				}
			}
			case 11: {
				stateStack.Push(12);
				goto case 18;
			}
			case 12: {
				isMissingModifier = true;
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 140) {
					PushContext(Context.Type, la, t);
					goto case 605;
				} else {
					goto case 14;
				}
			}
			case 14: {
				isMissingModifier = true;
				goto case 15;
			}
			case 15: {
				if (la == null) { currentState = 15; break; }
				if (set[9].Get(la.kind)) {
					goto case 21;
				} else {
					isMissingModifier = false;
					goto case 16;
				}
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				Expect(113, la); // "End"
				currentState = 17;
				break;
			}
			case 17: {
				if (la == null) { currentState = 17; break; }
				Expect(142, la); // "Interface"
				currentState = 18;
				break;
			}
			case 18: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 19;
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 21: {
				isMissingModifier = true;
				goto case 22;
			}
			case 22: {
				if (la == null) { currentState = 22; break; }
				if (la.kind == 40) {
					stateStack.Push(21);
					goto case 384;
				} else {
					isMissingModifier = true;
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (set[110].Get(la.kind)) {
					currentState = 604;
					break;
				} else {
					isMissingModifier = false;
					goto case 24;
				}
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(14);
					goto case 488;
				} else {
					if (la.kind == 103) {
						stateStack.Push(14);
						goto case 475;
					} else {
						if (la.kind == 115) {
							stateStack.Push(14);
							goto case 465;
						} else {
							if (la.kind == 142) {
								stateStack.Push(14);
								goto case 464;
							} else {
								if (set[12].Get(la.kind)) {
									stateStack.Push(14);
									goto case 25;
								} else {
									Error(la);
									goto case 14;
								}
							}
						}
					}
				}
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				if (la.kind == 119) {
					currentState = 458;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 450;
						break;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							currentState = 26;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 26: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 27;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				currentState = 28;
				break;
			}
			case 28: {
				PopContext();
				goto case 29;
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 37) {
					currentState = 424;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 30;
					} else {
						goto case 18;
					}
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				Expect(63, la); // "As"
				currentState = 31;
				break;
			}
			case 31: {
				stateStack.Push(32);
				goto case 33;
			}
			case 32: {
				PopContext();
				goto case 18;
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (la.kind == 130) {
					currentState = 34;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 34;
						break;
					} else {
						if (set[111].Get(la.kind)) {
							currentState = 34;
							break;
						} else {
							Error(la);
							goto case 34;
						}
					}
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 37) {
					stateStack.Push(34);
					goto case 38;
				} else {
					goto case 35;
				}
			}
			case 35: {
				if (la == null) { currentState = 35; break; }
				if (la.kind == 26) {
					currentState = 36;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 36: {
				stateStack.Push(37);
				goto case 74;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (la.kind == 37) {
					stateStack.Push(37);
					goto case 38;
				} else {
					goto case 35;
				}
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				Expect(37, la); // "("
				currentState = 39;
				break;
			}
			case 39: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 40;
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				if (la.kind == 169) {
					currentState = 422;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 42;
					} else {
						Error(la);
						goto case 41;
					}
				}
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 42: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 43;
			}
			case 43: {
				if (la == null) { currentState = 43; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(41);
					nextTokenIsPotentialStartOfExpression = true;
					goto case 44;
				} else {
					goto case 41;
				}
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (set[22].Get(la.kind)) {
					goto case 418;
				} else {
					if (la.kind == 22) {
						goto case 45;
					} else {
						goto case 6;
					}
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				currentState = 46;
				break;
			}
			case 46: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 47;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(48);
					goto case 49;
				} else {
					goto case 48;
				}
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (la.kind == 22) {
					goto case 45;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 49: {
				PushContext(Context.Expression, la, t);
				goto case 50;
			}
			case 50: {
				stateStack.Push(51);
				goto case 52;
			}
			case 51: {
				if (la == null) { currentState = 51; break; }
				if (set[112].Get(la.kind)) {
					currentState = 50;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 52: {
				PushContext(Context.Expression, la, t);
				goto case 53;
			}
			case 53: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 54;
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				if (set[113].Get(la.kind)) {
					currentState = 53;
					break;
				} else {
					if (set[31].Get(la.kind)) {
						stateStack.Push(125);
						goto case 135;
					} else {
						if (la.kind == 220) {
							currentState = 122;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(55);
								goto case 60;
							} else {
								if (la.kind == 35) {
									stateStack.Push(55);
									goto case 56;
								} else {
									Error(la);
									goto case 55;
								}
							}
						}
					}
				}
			}
			case 55: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 56: {
				if (la == null) { currentState = 56; break; }
				Expect(35, la); // "{"
				currentState = 57;
				break;
			}
			case 57: {
				stateStack.Push(58);
				goto case 49;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (la.kind == 22) {
					currentState = 57;
					break;
				} else {
					goto case 59;
				}
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				Expect(162, la); // "New"
				currentState = 61;
				break;
			}
			case 61: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 62;
			}
			case 62: {
				if (la == null) { currentState = 62; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(119);
					goto case 33;
				} else {
					goto case 63;
				}
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (la.kind == 233) {
					currentState = 66;
					break;
				} else {
					goto case 64;
				}
			}
			case 64: {
				Error(la);
				goto case 65;
			}
			case 65: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 66: {
				stateStack.Push(65);
				goto case 67;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				Expect(35, la); // "{"
				currentState = 68;
				break;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				if (la.kind == 147) {
					currentState = 69;
					break;
				} else {
					goto case 69;
				}
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				Expect(26, la); // "."
				currentState = 70;
				break;
			}
			case 70: {
				stateStack.Push(71);
				goto case 74;
			}
			case 71: {
				if (la == null) { currentState = 71; break; }
				Expect(20, la); // "="
				currentState = 72;
				break;
			}
			case 72: {
				stateStack.Push(73);
				goto case 49;
			}
			case 73: {
				if (la == null) { currentState = 73; break; }
				if (la.kind == 22) {
					currentState = 68;
					break;
				} else {
					goto case 59;
				}
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				if (la.kind == 2) {
					goto case 118;
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
								goto case 117;
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
												goto case 116;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 115;
													} else {
														if (la.kind == 65) {
															goto case 114;
														} else {
															if (la.kind == 66) {
																goto case 113;
															} else {
																if (la.kind == 67) {
																	goto case 112;
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
																				goto case 111;
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
																																		goto case 110;
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
																																					goto case 109;
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
																																																goto case 108;
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
																																																						goto case 107;
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
																																																									goto case 106;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 105;
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
																																																																		goto case 104;
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
																																																																							goto case 103;
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
																																																																										goto case 102;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 101;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 100;
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
																																																																																			goto case 99;
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
																																																																																									goto case 98;
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
																																																																																													goto case 97;
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
																																																																																																goto case 96;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 95;
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
																																																																																																																goto case 94;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 93;
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
																																																																																																																								goto case 92;
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
																																																																																																																														goto case 91;
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
																																																																																																																																						goto case 90;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 89;
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
																																																																																																																																																			goto case 88;
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
																																																																																																																																																									goto case 87;
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
																																																																																																																																																												goto case 86;
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
																																																																																																																																																															goto case 85;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 84;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 83;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 82;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 81;
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
																																																																																																																																																																								goto case 80;
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
																																																																																																																																																																													goto case 79;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 78;
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
																																																																																																																																																																																				goto case 77;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 76;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 75;
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
				currentState = stateStack.Pop();
				break;
			}
			case 107: {
				if (la == null) { currentState = 107; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 108: {
				if (la == null) { currentState = 108; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 109: {
				if (la == null) { currentState = 109; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 110: {
				if (la == null) { currentState = 110; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 111: {
				if (la == null) { currentState = 111; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 112: {
				if (la == null) { currentState = 112; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 113: {
				if (la == null) { currentState = 113; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 114: {
				if (la == null) { currentState = 114; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 115: {
				if (la == null) { currentState = 115; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 116: {
				if (la == null) { currentState = 116; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 117: {
				if (la == null) { currentState = 117; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 118: {
				if (la == null) { currentState = 118; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 119: {
				if (la == null) { currentState = 119; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 120;
						break;
					} else {
						goto case 63;
					}
				} else {
					goto case 65;
				}
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				if (la.kind == 35) {
					stateStack.Push(65);
					goto case 56;
				} else {
					if (set[26].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 64;
					}
				}
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				currentState = 65;
				break;
			}
			case 122: {
				stateStack.Push(123);
				goto case 52;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				Expect(144, la); // "Is"
				currentState = 124;
				break;
			}
			case 124: {
				stateStack.Push(55);
				goto case 33;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				if (set[28].Get(la.kind)) {
					stateStack.Push(125);
					goto case 126;
				} else {
					goto case 55;
				}
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.kind == 37) {
					currentState = 131;
					break;
				} else {
					if (set[114].Get(la.kind)) {
						currentState = 127;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 127: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 128;
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				if (la.kind == 10) {
					currentState = 129;
					break;
				} else {
					goto case 129;
				}
			}
			case 129: {
				stateStack.Push(130);
				goto case 74;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 131: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 132;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 169) {
					currentState = 133;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 42;
					} else {
						goto case 6;
					}
				}
			}
			case 133: {
				stateStack.Push(134);
				goto case 33;
			}
			case 134: {
				if (la == null) { currentState = 134; break; }
				if (la.kind == 22) {
					currentState = 133;
					break;
				} else {
					goto case 41;
				}
			}
			case 135: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 136;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (set[115].Get(la.kind)) {
					currentState = 137;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 417;
						break;
					} else {
						if (set[116].Get(la.kind)) {
							currentState = 137;
							break;
						} else {
							if (set[114].Get(la.kind)) {
								currentState = 413;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 411;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 408;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(137);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 397;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(137);
												goto case 213;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(137);
													PushContext(Context.Query, la, t);
													goto case 151;
												} else {
													if (set[33].Get(la.kind)) {
														stateStack.Push(137);
														goto case 145;
													} else {
														if (la.kind == 135) {
															stateStack.Push(137);
															goto case 138;
														} else {
															Error(la);
															goto case 137;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 137: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				Expect(135, la); // "If"
				currentState = 139;
				break;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				Expect(37, la); // "("
				currentState = 140;
				break;
			}
			case 140: {
				stateStack.Push(141);
				goto case 49;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				Expect(22, la); // ","
				currentState = 142;
				break;
			}
			case 142: {
				stateStack.Push(143);
				goto case 49;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				if (la.kind == 22) {
					currentState = 144;
					break;
				} else {
					goto case 41;
				}
			}
			case 144: {
				stateStack.Push(41);
				goto case 49;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				if (set[117].Get(la.kind)) {
					currentState = 150;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 146;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				Expect(37, la); // "("
				currentState = 147;
				break;
			}
			case 147: {
				stateStack.Push(148);
				goto case 49;
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				Expect(22, la); // ","
				currentState = 149;
				break;
			}
			case 149: {
				stateStack.Push(41);
				goto case 33;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				Expect(37, la); // "("
				currentState = 144;
				break;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				if (la.kind == 126) {
					stateStack.Push(152);
					goto case 212;
				} else {
					if (la.kind == 58) {
						stateStack.Push(152);
						goto case 211;
					} else {
						Error(la);
						goto case 152;
					}
				}
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(152);
					goto case 153;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (la.kind == 126) {
					currentState = 209;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 205;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 203;
							break;
						} else {
							if (la.kind == 107) {
								goto case 106;
							} else {
								if (la.kind == 230) {
									currentState = 49;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 199;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 197;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 195;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 168;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 154;
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
			case 154: {
				stateStack.Push(155);
				goto case 160;
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				Expect(171, la); // "On"
				currentState = 156;
				break;
			}
			case 156: {
				stateStack.Push(157);
				goto case 49;
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				Expect(116, la); // "Equals"
				currentState = 158;
				break;
			}
			case 158: {
				stateStack.Push(159);
				goto case 49;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (la.kind == 22) {
					currentState = 156;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 160: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(161);
				goto case 167;
			}
			case 161: {
				PopContext();
				goto case 162;
			}
			case 162: {
				if (la == null) { currentState = 162; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 164;
				} else {
					goto case 163;
				}
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				Expect(138, la); // "In"
				currentState = 49;
				break;
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				Expect(63, la); // "As"
				currentState = 165;
				break;
			}
			case 165: {
				stateStack.Push(166);
				goto case 33;
			}
			case 166: {
				PopContext();
				goto case 163;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (set[102].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 108;
					} else {
						goto case 6;
					}
				}
			}
			case 168: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 169;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (la.kind == 146) {
					goto case 187;
				} else {
					if (set[36].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 171;
							break;
						} else {
							if (set[36].Get(la.kind)) {
								goto case 185;
							} else {
								Error(la);
								goto case 170;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				Expect(70, la); // "By"
				currentState = 171;
				break;
			}
			case 171: {
				stateStack.Push(172);
				goto case 175;
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				if (la.kind == 22) {
					currentState = 171;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 173;
					break;
				}
			}
			case 173: {
				stateStack.Push(174);
				goto case 175;
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
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 176;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(177);
					goto case 167;
				} else {
					goto case 49;
				}
			}
			case 177: {
				PopContext();
				goto case 178;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 181;
				} else {
					if (la.kind == 20) {
						goto case 180;
					} else {
						if (set[37].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 49;
						}
					}
				}
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				currentState = 49;
				break;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				currentState = 49;
				break;
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				Expect(63, la); // "As"
				currentState = 182;
				break;
			}
			case 182: {
				stateStack.Push(183);
				goto case 33;
			}
			case 183: {
				PopContext();
				goto case 184;
			}
			case 184: {
				if (la == null) { currentState = 184; break; }
				Expect(20, la); // "="
				currentState = 49;
				break;
			}
			case 185: {
				stateStack.Push(186);
				goto case 175;
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				if (la.kind == 22) {
					currentState = 185;
					break;
				} else {
					goto case 170;
				}
			}
			case 187: {
				stateStack.Push(188);
				goto case 194;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 192;
						break;
					} else {
						if (la.kind == 146) {
							goto case 187;
						} else {
							Error(la);
							goto case 188;
						}
					}
				} else {
					goto case 189;
				}
			}
			case 189: {
				if (la == null) { currentState = 189; break; }
				Expect(143, la); // "Into"
				currentState = 190;
				break;
			}
			case 190: {
				stateStack.Push(191);
				goto case 175;
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
				goto case 194;
			}
			case 193: {
				stateStack.Push(188);
				goto case 189;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				Expect(146, la); // "Join"
				currentState = 154;
				break;
			}
			case 195: {
				stateStack.Push(196);
				goto case 175;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				if (la.kind == 22) {
					currentState = 195;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 197: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 198;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (la.kind == 231) {
					currentState = 49;
					break;
				} else {
					goto case 49;
				}
			}
			case 199: {
				if (la == null) { currentState = 199; break; }
				Expect(70, la); // "By"
				currentState = 200;
				break;
			}
			case 200: {
				stateStack.Push(201);
				goto case 49;
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				if (la.kind == 64) {
					currentState = 202;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 202;
						break;
					} else {
						Error(la);
						goto case 202;
					}
				}
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (la.kind == 22) {
					currentState = 200;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 203: {
				stateStack.Push(204);
				goto case 175;
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				if (la.kind == 22) {
					currentState = 203;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 205: {
				stateStack.Push(206);
				goto case 160;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(206);
					goto case 153;
				} else {
					Expect(143, la); // "Into"
					currentState = 207;
					break;
				}
			}
			case 207: {
				stateStack.Push(208);
				goto case 175;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 22) {
					currentState = 207;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 209: {
				stateStack.Push(210);
				goto case 160;
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				if (la.kind == 22) {
					currentState = 209;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(58, la); // "Aggregate"
				currentState = 205;
				break;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				Expect(126, la); // "From"
				currentState = 209;
				break;
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				if (la.kind == 210) {
					currentState = 389;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 214;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				Expect(37, la); // "("
				currentState = 215;
				break;
			}
			case 215: {
				SetIdentifierExpected(la);
				goto case 216;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(217);
					goto case 370;
				} else {
					goto case 217;
				}
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				Expect(38, la); // ")"
				currentState = 218;
				break;
			}
			case 218: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 219;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							PushContext(Context.Type, la, t);
							goto case 367;
						} else {
							goto case 220;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 220: {
				stateStack.Push(221);
				goto case 223;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(113, la); // "End"
				currentState = 222;
				break;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 223: {
				PushContext(Context.Body, la, t);
				goto case 224;
			}
			case 224: {
				stateStack.Push(225);
				goto case 18;
			}
			case 225: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 226;
			}
			case 226: {
				if (la == null) { currentState = 226; break; }
				if (set[118].Get(la.kind)) {
					if (set[63].Get(la.kind)) {
						if (set[45].Get(la.kind)) {
							stateStack.Push(224);
							goto case 231;
						} else {
							goto case 224;
						}
					} else {
						if (la.kind == 113) {
							currentState = 229;
							break;
						} else {
							goto case 228;
						}
					}
				} else {
					goto case 227;
				}
			}
			case 227: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 228: {
				Error(la);
				goto case 225;
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 224;
				} else {
					if (set[44].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 228;
					}
				}
			}
			case 230: {
				if (la == null) { currentState = 230; break; }
				currentState = 225;
				break;
			}
			case 231: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 232;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 348;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 344;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 342;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 340;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 321;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 306;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 302;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 296;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 268;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 264;
																break;
															} else {
																goto case 264;
															}
														} else {
															if (la.kind == 194) {
																currentState = 262;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 241;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 248;
																break;
															} else {
																if (set[119].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 245;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 244;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 243;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 86;
																				} else {
																					if (la.kind == 195) {
																						currentState = 241;
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
																		currentState = 239;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 237;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 233;
																				break;
																			} else {
																				if (set[120].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 49;
																						break;
																					} else {
																						goto case 49;
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
			case 233: {
				stateStack.Push(234);
				goto case 49;
			}
			case 234: {
				stateStack.Push(235);
				goto case 223;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(113, la); // "End"
				currentState = 236;
				break;
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 237: {
				stateStack.Push(238);
				goto case 49;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				if (la.kind == 22) {
					currentState = 237;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 239: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 240;
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.kind == 184) {
					currentState = 49;
					break;
				} else {
					goto case 49;
				}
			}
			case 241: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 242;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 243: {
				if (la == null) { currentState = 243; break; }
				if (la.kind == 108) {
					goto case 105;
				} else {
					if (la.kind == 124) {
						goto case 102;
					} else {
						if (la.kind == 231) {
							goto case 76;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (la.kind == 108) {
					goto case 105;
				} else {
					if (la.kind == 124) {
						goto case 102;
					} else {
						if (la.kind == 231) {
							goto case 76;
						} else {
							if (la.kind == 197) {
								goto case 88;
							} else {
								if (la.kind == 210) {
									goto case 84;
								} else {
									if (la.kind == 127) {
										goto case 100;
									} else {
										if (la.kind == 186) {
											goto case 89;
										} else {
											if (la.kind == 218) {
												goto case 80;
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
			case 245: {
				if (la == null) { currentState = 245; break; }
				if (set[6].Get(la.kind)) {
					goto case 247;
				} else {
					if (la.kind == 5) {
						goto case 246;
					} else {
						goto case 6;
					}
				}
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 248: {
				stateStack.Push(249);
				goto case 223;
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				if (la.kind == 75) {
					currentState = 253;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 252;
						break;
					} else {
						goto case 250;
					}
				}
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				Expect(113, la); // "End"
				currentState = 251;
				break;
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 252: {
				stateStack.Push(250);
				goto case 223;
			}
			case 253: {
				SetIdentifierExpected(la);
				goto case 254;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(257);
					goto case 167;
				} else {
					goto case 255;
				}
			}
			case 255: {
				if (la == null) { currentState = 255; break; }
				if (la.kind == 229) {
					currentState = 256;
					break;
				} else {
					goto case 248;
				}
			}
			case 256: {
				stateStack.Push(248);
				goto case 49;
			}
			case 257: {
				PopContext();
				goto case 258;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 259;
				} else {
					goto case 255;
				}
			}
			case 259: {
				if (la == null) { currentState = 259; break; }
				Expect(63, la); // "As"
				currentState = 260;
				break;
			}
			case 260: {
				stateStack.Push(261);
				goto case 33;
			}
			case 261: {
				PopContext();
				goto case 255;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (la.kind == 163) {
					goto case 93;
				} else {
					goto case 263;
				}
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (la.kind == 5) {
					goto case 246;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 247;
					} else {
						goto case 6;
					}
				}
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
				Expect(118, la); // "Error"
				currentState = 265;
				break;
			}
			case 265: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 266;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					if (la.kind == 132) {
						currentState = 263;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 267;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 268: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 269;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(286);
					goto case 279;
				} else {
					if (la.kind == 110) {
						currentState = 270;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 270: {
				stateStack.Push(271);
				goto case 279;
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				Expect(138, la); // "In"
				currentState = 272;
				break;
			}
			case 272: {
				stateStack.Push(273);
				goto case 49;
			}
			case 273: {
				stateStack.Push(274);
				goto case 223;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				Expect(163, la); // "Next"
				currentState = 275;
				break;
			}
			case 275: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 276;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				if (set[22].Get(la.kind)) {
					goto case 277;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 277: {
				stateStack.Push(278);
				goto case 49;
			}
			case 278: {
				if (la == null) { currentState = 278; break; }
				if (la.kind == 22) {
					currentState = 277;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 279: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(280);
				goto case 135;
			}
			case 280: {
				PopContext();
				goto case 281;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (la.kind == 33) {
					currentState = 282;
					break;
				} else {
					goto case 282;
				}
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				if (set[28].Get(la.kind)) {
					stateStack.Push(282);
					goto case 126;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 283;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				Expect(63, la); // "As"
				currentState = 284;
				break;
			}
			case 284: {
				stateStack.Push(285);
				goto case 33;
			}
			case 285: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				Expect(20, la); // "="
				currentState = 287;
				break;
			}
			case 287: {
				stateStack.Push(288);
				goto case 49;
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				if (la.kind == 205) {
					currentState = 295;
					break;
				} else {
					goto case 289;
				}
			}
			case 289: {
				stateStack.Push(290);
				goto case 223;
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				Expect(163, la); // "Next"
				currentState = 291;
				break;
			}
			case 291: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 292;
			}
			case 292: {
				if (la == null) { currentState = 292; break; }
				if (set[22].Get(la.kind)) {
					goto case 293;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 293: {
				stateStack.Push(294);
				goto case 49;
			}
			case 294: {
				if (la == null) { currentState = 294; break; }
				if (la.kind == 22) {
					currentState = 293;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 295: {
				stateStack.Push(289);
				goto case 49;
			}
			case 296: {
				if (la == null) { currentState = 296; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 299;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(297);
						goto case 223;
					} else {
						goto case 6;
					}
				}
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(152, la); // "Loop"
				currentState = 298;
				break;
			}
			case 298: {
				if (la == null) { currentState = 298; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 49;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 299: {
				stateStack.Push(300);
				goto case 49;
			}
			case 300: {
				stateStack.Push(301);
				goto case 223;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 302: {
				stateStack.Push(303);
				goto case 49;
			}
			case 303: {
				stateStack.Push(304);
				goto case 223;
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(113, la); // "End"
				currentState = 305;
				break;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 306: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 307;
			}
			case 307: {
				if (la == null) { currentState = 307; break; }
				if (la.kind == 74) {
					currentState = 308;
					break;
				} else {
					goto case 308;
				}
			}
			case 308: {
				stateStack.Push(309);
				goto case 49;
			}
			case 309: {
				stateStack.Push(310);
				goto case 18;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				if (la.kind == 74) {
					currentState = 312;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 311;
					break;
				}
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 312: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 313;
			}
			case 313: {
				if (la == null) { currentState = 313; break; }
				if (la.kind == 111) {
					currentState = 314;
					break;
				} else {
					if (set[61].Get(la.kind)) {
						goto case 315;
					} else {
						Error(la);
						goto case 314;
					}
				}
			}
			case 314: {
				stateStack.Push(310);
				goto case 223;
			}
			case 315: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 316;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (set[121].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 318;
						break;
					} else {
						goto case 318;
					}
				} else {
					if (set[22].Get(la.kind)) {
						stateStack.Push(317);
						goto case 49;
					} else {
						Error(la);
						goto case 317;
					}
				}
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 22) {
					currentState = 315;
					break;
				} else {
					goto case 314;
				}
			}
			case 318: {
				stateStack.Push(319);
				goto case 320;
			}
			case 319: {
				stateStack.Push(317);
				goto case 52;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
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
			case 321: {
				stateStack.Push(322);
				goto case 49;
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 214) {
					currentState = 330;
					break;
				} else {
					goto case 323;
				}
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 324;
				} else {
					goto case 6;
				}
			}
			case 324: {
				stateStack.Push(325);
				goto case 223;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 329;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 327;
							break;
						} else {
							Error(la);
							goto case 324;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 326;
					break;
				}
			}
			case 326: {
				if (la == null) { currentState = 326; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 327: {
				stateStack.Push(328);
				goto case 49;
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 214) {
					currentState = 324;
					break;
				} else {
					goto case 324;
				}
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 135) {
					currentState = 327;
					break;
				} else {
					goto case 324;
				}
			}
			case 330: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 331;
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				if (set[45].Get(la.kind)) {
					goto case 332;
				} else {
					goto case 323;
				}
			}
			case 332: {
				stateStack.Push(333);
				goto case 231;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 21) {
					currentState = 338;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 335;
						break;
					} else {
						goto case 334;
					}
				}
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 335: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 336;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (set[45].Get(la.kind)) {
					stateStack.Push(337);
					goto case 231;
				} else {
					goto case 337;
				}
			}
			case 337: {
				if (la == null) { currentState = 337; break; }
				if (la.kind == 21) {
					currentState = 335;
					break;
				} else {
					goto case 334;
				}
			}
			case 338: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 339;
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				if (set[45].Get(la.kind)) {
					goto case 332;
				} else {
					goto case 333;
				}
			}
			case 340: {
				stateStack.Push(341);
				goto case 74;
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (la.kind == 37) {
					currentState = 42;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 342: {
				stateStack.Push(343);
				goto case 49;
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				Expect(22, la); // ","
				currentState = 49;
				break;
			}
			case 344: {
				stateStack.Push(345);
				goto case 49;
			}
			case 345: {
				stateStack.Push(346);
				goto case 223;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				Expect(113, la); // "End"
				currentState = 347;
				break;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 233) {
					goto case 75;
				} else {
					if (la.kind == 211) {
						goto case 83;
					} else {
						goto case 6;
					}
				}
			}
			case 348: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(349);
				goto case 167;
			}
			case 349: {
				PopContext();
				goto case 350;
			}
			case 350: {
				if (la == null) { currentState = 350; break; }
				if (la.kind == 33) {
					currentState = 351;
					break;
				} else {
					goto case 351;
				}
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 37) {
					currentState = 366;
					break;
				} else {
					goto case 352;
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 22) {
					currentState = 360;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 354;
					} else {
						goto case 353;
					}
				}
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (la.kind == 20) {
					goto case 180;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
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
				goto case 33;
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 162) {
					currentState = 359;
					break;
				} else {
					goto case 357;
				}
			}
			case 357: {
				stateStack.Push(358);
				goto case 33;
			}
			case 358: {
				if (CurrentBlock.context == Context.ObjectCreation)
					PopContext();
				PopContext();

				goto case 353;
			}
			case 359: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 357;
			}
			case 360: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(361);
				goto case 167;
			}
			case 361: {
				PopContext();
				goto case 362;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 33) {
					currentState = 363;
					break;
				} else {
					goto case 363;
				}
			}
			case 363: {
				if (la == null) { currentState = 363; break; }
				if (la.kind == 37) {
					currentState = 364;
					break;
				} else {
					goto case 352;
				}
			}
			case 364: {
				if (la == null) { currentState = 364; break; }
				if (la.kind == 22) {
					currentState = 364;
					break;
				} else {
					goto case 365;
				}
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				Expect(38, la); // ")"
				currentState = 352;
				break;
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (la.kind == 22) {
					currentState = 366;
					break;
				} else {
					goto case 365;
				}
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				Expect(63, la); // "As"
				currentState = 368;
				break;
			}
			case 368: {
				stateStack.Push(369);
				goto case 33;
			}
			case 369: {
				PopContext();
				goto case 220;
			}
			case 370: {
				stateStack.Push(371);
				PushContext(Context.Parameter, la, t);
				goto case 372;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (la.kind == 22) {
					currentState = 370;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 372: {
				SetIdentifierExpected(la);
				goto case 373;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 40) {
					stateStack.Push(372);
					goto case 384;
				} else {
					goto case 374;
				}
			}
			case 374: {
				SetIdentifierExpected(la);
				goto case 375;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (set[122].Get(la.kind)) {
					currentState = 374;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(376);
					goto case 167;
				}
			}
			case 376: {
				PopContext();
				goto case 377;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 381;
				} else {
					goto case 378;
				}
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (la.kind == 20) {
					currentState = 380;
					break;
				} else {
					goto case 379;
				}
			}
			case 379: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 380: {
				stateStack.Push(379);
				goto case 49;
			}
			case 381: {
				if (la == null) { currentState = 381; break; }
				Expect(63, la); // "As"
				currentState = 382;
				break;
			}
			case 382: {
				stateStack.Push(383);
				goto case 33;
			}
			case 383: {
				PopContext();
				goto case 378;
			}
			case 384: {
				if (la == null) { currentState = 384; break; }
				Expect(40, la); // "<"
				currentState = 385;
				break;
			}
			case 385: {
				PushContext(Context.Attribute, la, t);
				goto case 386;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				if (set[123].Get(la.kind)) {
					currentState = 386;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 387;
					break;
				}
			}
			case 387: {
				PopContext();
				goto case 388;
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				Expect(37, la); // "("
				currentState = 390;
				break;
			}
			case 390: {
				SetIdentifierExpected(la);
				goto case 391;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(392);
					goto case 370;
				} else {
					goto case 392;
				}
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				Expect(38, la); // ")"
				currentState = 393;
				break;
			}
			case 393: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 394;
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (set[45].Get(la.kind)) {
					goto case 231;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(395);
						goto case 223;
					} else {
						goto case 6;
					}
				}
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				Expect(113, la); // "End"
				currentState = 396;
				break;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 407;
					break;
				} else {
					stateStack.Push(398);
					goto case 400;
				}
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (la.kind == 17) {
					currentState = 399;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				if (la.kind == 16) {
					currentState = 398;
					break;
				} else {
					goto case 398;
				}
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 401;
				break;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (set[124].Get(la.kind)) {
					if (set[125].Get(la.kind)) {
						currentState = 401;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(401);
							goto case 404;
						} else {
							Error(la);
							goto case 401;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 402;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (set[126].Get(la.kind)) {
					if (set[127].Get(la.kind)) {
						currentState = 402;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(402);
							goto case 404;
						} else {
							if (la.kind == 10) {
								stateStack.Push(402);
								goto case 400;
							} else {
								Error(la);
								goto case 402;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 403;
					break;
				}
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (set[128].Get(la.kind)) {
					if (set[129].Get(la.kind)) {
						currentState = 403;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(403);
							goto case 404;
						} else {
							Error(la);
							goto case 403;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 405;
				break;
			}
			case 405: {
				stateStack.Push(406);
				goto case 49;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (la.kind == 16) {
					currentState = 397;
					break;
				} else {
					goto case 397;
				}
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(37, la); // "("
				currentState = 409;
				break;
			}
			case 409: {
				readXmlIdentifier = true;
				stateStack.Push(410);
				goto case 167;
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				Expect(38, la); // ")"
				currentState = 137;
				break;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				Expect(37, la); // "("
				currentState = 412;
				break;
			}
			case 412: {
				stateStack.Push(410);
				goto case 33;
			}
			case 413: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 414;
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 10) {
					currentState = 415;
					break;
				} else {
					goto case 415;
				}
			}
			case 415: {
				stateStack.Push(416);
				goto case 74;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				if (la.kind == 11) {
					currentState = 137;
					break;
				} else {
					goto case 137;
				}
			}
			case 417: {
				stateStack.Push(410);
				goto case 49;
			}
			case 418: {
				stateStack.Push(419);
				goto case 49;
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				if (la.kind == 22) {
					currentState = 420;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 420: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 421;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (set[22].Get(la.kind)) {
					goto case 418;
				} else {
					goto case 419;
				}
			}
			case 422: {
				if (la == null) { currentState = 422; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(423);
					goto case 33;
				} else {
					goto case 423;
				}
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 22) {
					currentState = 422;
					break;
				} else {
					goto case 41;
				}
			}
			case 424: {
				SetIdentifierExpected(la);
				goto case 425;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (set[130].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 427;
						break;
					} else {
						if (set[71].Get(la.kind)) {
							stateStack.Push(426);
							goto case 370;
						} else {
							Error(la);
							goto case 426;
						}
					}
				} else {
					goto case 426;
				}
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				Expect(38, la); // ")"
				currentState = 29;
				break;
			}
			case 427: {
				stateStack.Push(426);
				goto case 428;
			}
			case 428: {
				SetIdentifierExpected(la);
				goto case 429;
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 430;
					break;
				} else {
					goto case 430;
				}
			}
			case 430: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(431);
				goto case 446;
			}
			case 431: {
				PopContext();
				goto case 432;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 447;
				} else {
					goto case 433;
				}
			}
			case 433: {
				if (la == null) { currentState = 433; break; }
				if (la.kind == 22) {
					currentState = 434;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 434: {
				SetIdentifierExpected(la);
				goto case 435;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 436;
					break;
				} else {
					goto case 436;
				}
			}
			case 436: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(437);
				goto case 446;
			}
			case 437: {
				PopContext();
				goto case 438;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 439;
				} else {
					goto case 433;
				}
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				Expect(63, la); // "As"
				currentState = 440;
				break;
			}
			case 440: {
				stateStack.Push(441);
				goto case 442;
			}
			case 441: {
				PopContext();
				goto case 433;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (set[84].Get(la.kind)) {
					goto case 445;
				} else {
					if (la.kind == 35) {
						currentState = 443;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 443: {
				stateStack.Push(444);
				goto case 445;
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (la.kind == 22) {
					currentState = 443;
					break;
				} else {
					goto case 59;
				}
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (set[15].Get(la.kind)) {
					currentState = 34;
					break;
				} else {
					if (la.kind == 162) {
						goto case 94;
					} else {
						if (la.kind == 84) {
							goto case 110;
						} else {
							if (la.kind == 209) {
								goto case 85;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 446: {
				if (la == null) { currentState = 446; break; }
				if (la.kind == 2) {
					goto case 118;
				} else {
					if (la.kind == 62) {
						goto case 116;
					} else {
						if (la.kind == 64) {
							goto case 115;
						} else {
							if (la.kind == 65) {
								goto case 114;
							} else {
								if (la.kind == 66) {
									goto case 113;
								} else {
									if (la.kind == 67) {
										goto case 112;
									} else {
										if (la.kind == 70) {
											goto case 111;
										} else {
											if (la.kind == 87) {
												goto case 109;
											} else {
												if (la.kind == 104) {
													goto case 107;
												} else {
													if (la.kind == 107) {
														goto case 106;
													} else {
														if (la.kind == 116) {
															goto case 104;
														} else {
															if (la.kind == 121) {
																goto case 103;
															} else {
																if (la.kind == 133) {
																	goto case 99;
																} else {
																	if (la.kind == 139) {
																		goto case 98;
																	} else {
																		if (la.kind == 143) {
																			goto case 97;
																		} else {
																			if (la.kind == 146) {
																				goto case 96;
																			} else {
																				if (la.kind == 147) {
																					goto case 95;
																				} else {
																					if (la.kind == 170) {
																						goto case 92;
																					} else {
																						if (la.kind == 176) {
																							goto case 91;
																						} else {
																							if (la.kind == 184) {
																								goto case 90;
																							} else {
																								if (la.kind == 203) {
																									goto case 87;
																								} else {
																									if (la.kind == 212) {
																										goto case 82;
																									} else {
																										if (la.kind == 213) {
																											goto case 81;
																										} else {
																											if (la.kind == 223) {
																												goto case 79;
																											} else {
																												if (la.kind == 224) {
																													goto case 78;
																												} else {
																													if (la.kind == 230) {
																														goto case 77;
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
			case 447: {
				if (la == null) { currentState = 447; break; }
				Expect(63, la); // "As"
				currentState = 448;
				break;
			}
			case 448: {
				stateStack.Push(449);
				goto case 442;
			}
			case 449: {
				PopContext();
				goto case 433;
			}
			case 450: {
				stateStack.Push(451);
				goto case 167;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 37) {
					currentState = 455;
					break;
				} else {
					goto case 452;
				}
			}
			case 452: {
				if (la == null) { currentState = 452; break; }
				if (la.kind == 63) {
					currentState = 453;
					break;
				} else {
					goto case 18;
				}
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (la.kind == 40) {
					stateStack.Push(453);
					goto case 384;
				} else {
					goto case 454;
				}
			}
			case 454: {
				stateStack.Push(18);
				goto case 33;
			}
			case 455: {
				SetIdentifierExpected(la);
				goto case 456;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(457);
					goto case 370;
				} else {
					goto case 457;
				}
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				Expect(38, la); // ")"
				currentState = 452;
				break;
			}
			case 458: {
				stateStack.Push(459);
				goto case 167;
			}
			case 459: {
				if (la == null) { currentState = 459; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 454;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 461;
							break;
						} else {
							goto case 460;
						}
					}
				} else {
					goto case 18;
				}
			}
			case 460: {
				Error(la);
				goto case 18;
			}
			case 461: {
				SetIdentifierExpected(la);
				goto case 462;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(463);
					goto case 370;
				} else {
					goto case 463;
				}
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(38, la); // ")"
				currentState = 18;
				break;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				Expect(142, la); // "Interface"
				currentState = 9;
				break;
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				Expect(115, la); // "Enum"
				currentState = 466;
				break;
			}
			case 466: {
				stateStack.Push(467);
				goto case 167;
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				if (la.kind == 63) {
					currentState = 474;
					break;
				} else {
					goto case 468;
				}
			}
			case 468: {
				stateStack.Push(469);
				goto case 18;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (set[87].Get(la.kind)) {
					goto case 471;
				} else {
					Expect(113, la); // "End"
					currentState = 470;
					break;
				}
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				Expect(115, la); // "Enum"
				currentState = 18;
				break;
			}
			case 471: {
				if (la == null) { currentState = 471; break; }
				if (la.kind == 40) {
					stateStack.Push(471);
					goto case 384;
				} else {
					stateStack.Push(472);
					goto case 167;
				}
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 20) {
					currentState = 473;
					break;
				} else {
					goto case 468;
				}
			}
			case 473: {
				stateStack.Push(468);
				goto case 49;
			}
			case 474: {
				stateStack.Push(468);
				goto case 33;
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				Expect(103, la); // "Delegate"
				currentState = 476;
				break;
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (la.kind == 210) {
					currentState = 477;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 477;
						break;
					} else {
						Error(la);
						goto case 477;
					}
				}
			}
			case 477: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				currentState = 479;
				break;
			}
			case 479: {
				PopContext();
				goto case 480;
			}
			case 480: {
				if (la == null) { currentState = 480; break; }
				if (la.kind == 37) {
					currentState = 485;
					break;
				} else {
					goto case 481;
				}
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 482;
				} else {
					goto case 18;
				}
			}
			case 482: {
				if (la == null) { currentState = 482; break; }
				Expect(63, la); // "As"
				currentState = 483;
				break;
			}
			case 483: {
				stateStack.Push(484);
				goto case 33;
			}
			case 484: {
				PopContext();
				goto case 18;
			}
			case 485: {
				SetIdentifierExpected(la);
				goto case 486;
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(487);
					goto case 370;
				} else {
					goto case 487;
				}
			}
			case 487: {
				if (la == null) { currentState = 487; break; }
				Expect(38, la); // ")"
				currentState = 481;
				break;
			}
			case 488: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 489;
			}
			case 489: {
				if (la == null) { currentState = 489; break; }
				if (la.kind == 155) {
					currentState = 490;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 490;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 490;
							break;
						} else {
							Error(la);
							goto case 490;
						}
					}
				}
			}
			case 490: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(491);
				goto case 167;
			}
			case 491: {
				PopContext();
				goto case 492;
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				if (la.kind == 37) {
					currentState = 601;
					break;
				} else {
					goto case 493;
				}
			}
			case 493: {
				stateStack.Push(494);
				goto case 18;
			}
			case 494: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 495;
			}
			case 495: {
				if (la == null) { currentState = 495; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					PushContext(Context.Type, la, t);
					goto case 598;
				} else {
					goto case 496;
				}
			}
			case 496: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 497;
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					PushContext(Context.Type, la, t);
					goto case 592;
				} else {
					goto case 498;
				}
			}
			case 498: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 499;
			}
			case 499: {
				if (la == null) { currentState = 499; break; }
				if (set[92].Get(la.kind)) {
					goto case 504;
				} else {
					isMissingModifier = false;
					goto case 500;
				}
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				Expect(113, la); // "End"
				currentState = 501;
				break;
			}
			case 501: {
				if (la == null) { currentState = 501; break; }
				if (la.kind == 155) {
					currentState = 502;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 502;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 502;
							break;
						} else {
							Error(la);
							goto case 502;
						}
					}
				}
			}
			case 502: {
				stateStack.Push(503);
				goto case 18;
			}
			case 503: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 504: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 505;
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				if (la.kind == 40) {
					stateStack.Push(504);
					goto case 384;
				} else {
					isMissingModifier = true;
					goto case 506;
				}
			}
			case 506: {
				SetIdentifierExpected(la);
				goto case 507;
			}
			case 507: {
				if (la == null) { currentState = 507; break; }
				if (set[110].Get(la.kind)) {
					currentState = 591;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 508;
				}
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(498);
					goto case 488;
				} else {
					if (la.kind == 103) {
						stateStack.Push(498);
						goto case 475;
					} else {
						if (la.kind == 115) {
							stateStack.Push(498);
							goto case 465;
						} else {
							if (la.kind == 142) {
								stateStack.Push(498);
								goto case 464;
							} else {
								if (set[95].Get(la.kind)) {
									stateStack.Push(498);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 509;
								} else {
									Error(la);
									goto case 498;
								}
							}
						}
					}
				}
			}
			case 509: {
				if (la == null) { currentState = 509; break; }
				if (set[101].Get(la.kind)) {
					stateStack.Push(510);
					SetIdentifierExpected(la);
					goto case 581;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(510);
						goto case 566;
					} else {
						if (la.kind == 101) {
							stateStack.Push(510);
							goto case 549;
						} else {
							if (la.kind == 119) {
								stateStack.Push(510);
								goto case 536;
							} else {
								if (la.kind == 98) {
									stateStack.Push(510);
									goto case 524;
								} else {
									if (la.kind == 172) {
										stateStack.Push(510);
										goto case 511;
									} else {
										Error(la);
										goto case 510;
									}
								}
							}
						}
					}
				}
			}
			case 510: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 511: {
				if (la == null) { currentState = 511; break; }
				Expect(172, la); // "Operator"
				currentState = 512;
				break;
			}
			case 512: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 513;
			}
			case 513: {
				if (la == null) { currentState = 513; break; }
				currentState = 514;
				break;
			}
			case 514: {
				PopContext();
				goto case 515;
			}
			case 515: {
				if (la == null) { currentState = 515; break; }
				Expect(37, la); // "("
				currentState = 516;
				break;
			}
			case 516: {
				stateStack.Push(517);
				goto case 370;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				Expect(38, la); // ")"
				currentState = 518;
				break;
			}
			case 518: {
				if (la == null) { currentState = 518; break; }
				if (la.kind == 63) {
					currentState = 522;
					break;
				} else {
					goto case 519;
				}
			}
			case 519: {
				stateStack.Push(520);
				goto case 223;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				Expect(113, la); // "End"
				currentState = 521;
				break;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				Expect(172, la); // "Operator"
				currentState = 18;
				break;
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				if (la.kind == 40) {
					stateStack.Push(522);
					goto case 384;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(523);
					goto case 33;
				}
			}
			case 523: {
				PopContext();
				goto case 519;
			}
			case 524: {
				if (la == null) { currentState = 524; break; }
				Expect(98, la); // "Custom"
				currentState = 525;
				break;
			}
			case 525: {
				stateStack.Push(526);
				goto case 536;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (set[97].Get(la.kind)) {
					goto case 528;
				} else {
					Expect(113, la); // "End"
					currentState = 527;
					break;
				}
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				Expect(119, la); // "Event"
				currentState = 18;
				break;
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				if (la.kind == 40) {
					stateStack.Push(528);
					goto case 384;
				} else {
					if (la.kind == 56) {
						currentState = 529;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 529;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 529;
								break;
							} else {
								Error(la);
								goto case 529;
							}
						}
					}
				}
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				Expect(37, la); // "("
				currentState = 530;
				break;
			}
			case 530: {
				stateStack.Push(531);
				goto case 370;
			}
			case 531: {
				if (la == null) { currentState = 531; break; }
				Expect(38, la); // ")"
				currentState = 532;
				break;
			}
			case 532: {
				stateStack.Push(533);
				goto case 223;
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				Expect(113, la); // "End"
				currentState = 534;
				break;
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				if (la.kind == 56) {
					currentState = 535;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 535;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 535;
							break;
						} else {
							Error(la);
							goto case 535;
						}
					}
				}
			}
			case 535: {
				stateStack.Push(526);
				goto case 18;
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				Expect(119, la); // "Event"
				currentState = 537;
				break;
			}
			case 537: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(538);
				goto case 167;
			}
			case 538: {
				PopContext();
				goto case 539;
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 546;
				} else {
					if (set[131].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 543;
							break;
						} else {
							goto case 540;
						}
					} else {
						Error(la);
						goto case 540;
					}
				}
			}
			case 540: {
				if (la == null) { currentState = 540; break; }
				if (la.kind == 136) {
					currentState = 541;
					break;
				} else {
					goto case 18;
				}
			}
			case 541: {
				stateStack.Push(542);
				goto case 33;
			}
			case 542: {
				if (la == null) { currentState = 542; break; }
				if (la.kind == 22) {
					currentState = 541;
					break;
				} else {
					goto case 18;
				}
			}
			case 543: {
				SetIdentifierExpected(la);
				goto case 544;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(545);
					goto case 370;
				} else {
					goto case 545;
				}
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				Expect(38, la); // ")"
				currentState = 540;
				break;
			}
			case 546: {
				if (la == null) { currentState = 546; break; }
				Expect(63, la); // "As"
				currentState = 547;
				break;
			}
			case 547: {
				stateStack.Push(548);
				goto case 33;
			}
			case 548: {
				PopContext();
				goto case 540;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				Expect(101, la); // "Declare"
				currentState = 550;
				break;
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 551;
					break;
				} else {
					goto case 551;
				}
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				if (la.kind == 210) {
					currentState = 552;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 552;
						break;
					} else {
						Error(la);
						goto case 552;
					}
				}
			}
			case 552: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(553);
				goto case 167;
			}
			case 553: {
				PopContext();
				goto case 554;
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				Expect(149, la); // "Lib"
				currentState = 555;
				break;
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				Expect(3, la); // LiteralString
				currentState = 556;
				break;
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (la.kind == 59) {
					currentState = 565;
					break;
				} else {
					goto case 557;
				}
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (la.kind == 37) {
					currentState = 562;
					break;
				} else {
					goto case 558;
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 559;
				} else {
					goto case 18;
				}
			}
			case 559: {
				if (la == null) { currentState = 559; break; }
				Expect(63, la); // "As"
				currentState = 560;
				break;
			}
			case 560: {
				stateStack.Push(561);
				goto case 33;
			}
			case 561: {
				PopContext();
				goto case 18;
			}
			case 562: {
				SetIdentifierExpected(la);
				goto case 563;
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(564);
					goto case 370;
				} else {
					goto case 564;
				}
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				Expect(38, la); // ")"
				currentState = 558;
				break;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				Expect(3, la); // LiteralString
				currentState = 557;
				break;
			}
			case 566: {
				if (la == null) { currentState = 566; break; }
				if (la.kind == 210) {
					currentState = 567;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 567;
						break;
					} else {
						Error(la);
						goto case 567;
					}
				}
			}
			case 567: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 568;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				currentState = 569;
				break;
			}
			case 569: {
				PopContext();
				goto case 570;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				if (la.kind == 37) {
					currentState = 577;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 574;
					} else {
						goto case 571;
					}
				}
			}
			case 571: {
				stateStack.Push(572);
				goto case 223;
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				Expect(113, la); // "End"
				currentState = 573;
				break;
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				if (la.kind == 210) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 18;
						break;
					} else {
						goto case 460;
					}
				}
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				Expect(63, la); // "As"
				currentState = 575;
				break;
			}
			case 575: {
				stateStack.Push(576);
				goto case 33;
			}
			case 576: {
				PopContext();
				goto case 571;
			}
			case 577: {
				SetIdentifierExpected(la);
				goto case 578;
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (set[130].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 580;
						break;
					} else {
						if (set[71].Get(la.kind)) {
							stateStack.Push(579);
							goto case 370;
						} else {
							Error(la);
							goto case 579;
						}
					}
				} else {
					goto case 579;
				}
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				Expect(38, la); // ")"
				currentState = 570;
				break;
			}
			case 580: {
				stateStack.Push(579);
				goto case 428;
			}
			case 581: {
				if (la == null) { currentState = 581; break; }
				if (la.kind == 88) {
					currentState = 582;
					break;
				} else {
					goto case 582;
				}
			}
			case 582: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(583);
				goto case 590;
			}
			case 583: {
				PopContext();
				goto case 584;
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 587;
				} else {
					goto case 585;
				}
			}
			case 585: {
				if (la == null) { currentState = 585; break; }
				if (la.kind == 20) {
					currentState = 586;
					break;
				} else {
					goto case 18;
				}
			}
			case 586: {
				stateStack.Push(18);
				goto case 49;
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				Expect(63, la); // "As"
				currentState = 588;
				break;
			}
			case 588: {
				stateStack.Push(589);
				goto case 33;
			}
			case 589: {
				PopContext();
				goto case 585;
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (set[116].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 117;
					} else {
						if (la.kind == 126) {
							goto case 101;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 591: {
				isMissingModifier = false;
				goto case 506;
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				Expect(136, la); // "Implements"
				currentState = 593;
				break;
			}
			case 593: {
				stateStack.Push(594);
				goto case 33;
			}
			case 594: {
				PopContext();
				goto case 595;
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				if (la.kind == 22) {
					currentState = 596;
					break;
				} else {
					stateStack.Push(498);
					goto case 18;
				}
			}
			case 596: {
				PushContext(Context.Type, la, t);
				stateStack.Push(597);
				goto case 33;
			}
			case 597: {
				PopContext();
				goto case 595;
			}
			case 598: {
				if (la == null) { currentState = 598; break; }
				Expect(140, la); // "Inherits"
				currentState = 599;
				break;
			}
			case 599: {
				stateStack.Push(600);
				goto case 33;
			}
			case 600: {
				PopContext();
				stateStack.Push(496);
				goto case 18;
			}
			case 601: {
				if (la == null) { currentState = 601; break; }
				Expect(169, la); // "Of"
				currentState = 602;
				break;
			}
			case 602: {
				stateStack.Push(603);
				goto case 428;
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				Expect(38, la); // ")"
				currentState = 493;
				break;
			}
			case 604: {
				isMissingModifier = false;
				goto case 23;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				Expect(140, la); // "Inherits"
				currentState = 606;
				break;
			}
			case 606: {
				stateStack.Push(607);
				goto case 33;
			}
			case 607: {
				PopContext();
				goto case 608;
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				if (la.kind == 22) {
					currentState = 609;
					break;
				} else {
					stateStack.Push(14);
					goto case 18;
				}
			}
			case 609: {
				PushContext(Context.Type, la, t);
				stateStack.Push(610);
				goto case 33;
			}
			case 610: {
				PopContext();
				goto case 608;
			}
			case 611: {
				if (la == null) { currentState = 611; break; }
				Expect(169, la); // "Of"
				currentState = 612;
				break;
			}
			case 612: {
				stateStack.Push(613);
				goto case 428;
			}
			case 613: {
				if (la == null) { currentState = 613; break; }
				Expect(38, la); // ")"
				currentState = 11;
				break;
			}
			case 614: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 615;
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				if (set[44].Get(la.kind)) {
					currentState = 615;
					break;
				} else {
					PopContext();
					stateStack.Push(616);
					goto case 18;
				}
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(616);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 617;
					break;
				}
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				Expect(160, la); // "Namespace"
				currentState = 18;
				break;
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				Expect(137, la); // "Imports"
				currentState = 619;
				break;
			}
			case 619: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
					
					if (la != null)
						CurrentBlock.lastExpressionStart = la.Location;

				goto case 620;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				if (set[15].Get(la.kind)) {
					currentState = 626;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 622;
						break;
					} else {
						Error(la);
						goto case 621;
					}
				}
			}
			case 621: {
				PopContext();
				goto case 18;
			}
			case 622: {
				stateStack.Push(623);
				goto case 167;
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				Expect(20, la); // "="
				currentState = 624;
				break;
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				Expect(3, la); // LiteralString
				currentState = 625;
				break;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 621;
				break;
			}
			case 626: {
				if (la == null) { currentState = 626; break; }
				if (la.kind == 37) {
					stateStack.Push(626);
					goto case 38;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 627;
						break;
					} else {
						goto case 621;
					}
				}
			}
			case 627: {
				stateStack.Push(621);
				goto case 33;
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				Expect(173, la); // "Option"
				currentState = 629;
				break;
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 631;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 630;
						break;
					} else {
						goto case 460;
					}
				}
			}
			case 630: {
				if (la == null) { currentState = 630; break; }
				if (la.kind == 213) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 18;
						break;
					} else {
						goto case 460;
					}
				}
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 18;
					break;
				} else {
					goto case 18;
				}
			}
		}

		if (la != null) {
			t = la;
			nextTokenIsPotentialStartOfExpression = false;
			readXmlIdentifier = false;
			nextTokenIsStartOfImportsOrAccessExpression = false;
			wasQualifierTokenAtStart = false;
			identifierExpected = false;
		}
	}
	
	public void Advance()
	{
		//Console.WriteLine("Advance");
		InformToken(null);
	}
	
	public BitArray GetExpectedSet() { return GetExpectedSet(currentState); }
	
	static readonly BitArray[] set = {
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 436215809, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 436207617, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234112, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 436207617, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 436207616, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 537395328, 134234112, 436207616, 131200, 0}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 256, 1048576, -1601568128, 671109120, 515375234, 393600, 1280}),
		new BitArray(new int[] {0, 256, 1048576, -1601568128, 671105024, 515375234, 393600, 1280}),
		new BitArray(new int[] {0, 256, 1048576, -1601699200, 671105024, 515375234, 393600, 1280}),
		new BitArray(new int[] {0, 0, 1048576, -1601699200, 671105024, 515375234, 393600, 1280}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
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
		new BitArray(new int[] {7, 1157628160, 26477055, -493343812, 680323108, 1073567107, -533262446, 1347}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {721920, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {4194308, 1140850752, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850696, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850688, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850944, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108478212, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220956, 671930656, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220956, 671926560, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220956, 671926304, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {5, 1140850944, 26214479, -493220956, 671926304, 532485251, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493352028, 671926304, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850688, 26214479, -493352028, 671926304, 465376386, -2143942272, 1345}),
		new BitArray(new int[] {4, 1140850688, 26214479, -1030223452, 135055392, 17110016, -2143942656, 65}),
		new BitArray(new int[] {4, 1140850688, 25165903, -1030747868, 821280, 17110016, -2144073728, 65}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537526400, 134234112, 436207617, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 128, 0}),
		new BitArray(new int[] {0, 0, 0, 536871424, 536870912, 448266370, 384, 1280}),
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
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0})

	};

} // end Parser


}