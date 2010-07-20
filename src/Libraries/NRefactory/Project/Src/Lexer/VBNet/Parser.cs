using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 50;
	const int endOfStatementTerminatorAndBlock = 229;
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
			case 65:
			case 230:
			case 465:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 156:
			case 162:
			case 169:
			case 207:
			case 211:
			case 250:
			case 353:
			case 365:
			case 414:
			case 455:
			case 463:
			case 471:
			case 495:
			case 542:
			case 557:
			case 627:
				return set[6];
			case 10:
			case 496:
			case 497:
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
			case 222:
			case 225:
			case 226:
			case 236:
			case 251:
			case 255:
			case 278:
			case 294:
			case 305:
			case 308:
			case 314:
			case 319:
			case 328:
			case 329:
			case 342:
			case 350:
			case 374:
			case 473:
			case 489:
			case 498:
			case 507:
			case 524:
			case 528:
			case 537:
			case 540:
			case 566:
			case 576:
			case 581:
			case 605:
			case 626:
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
			case 223:
			case 237:
			case 253:
			case 309:
			case 351:
			case 400:
			case 505:
			case 525:
			case 538:
			case 577:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 17:
			case 469:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 20:
			case 339:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 21:
			case 22:
				return set[9];
			case 23:
			case 609:
				return set[10];
			case 24:
				return set[11];
			case 25:
				return set[12];
			case 26:
			case 27:
			case 121:
			case 179:
			case 180:
			case 231:
			case 390:
			case 391:
			case 406:
			case 407:
			case 408:
			case 482:
			case 483:
			case 517:
			case 518:
			case 572:
			case 573:
			case 619:
			case 620:
				return set[13];
			case 28:
			case 29:
			case 456:
			case 464:
			case 484:
			case 485:
			case 562:
			case 574:
			case 575:
				return set[14];
			case 30:
			case 166:
			case 183:
			case 262:
			case 288:
			case 359:
			case 372:
			case 386:
			case 444:
			case 452:
			case 487:
			case 551:
			case 564:
			case 579:
			case 592:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					return a;
				}
			case 31:
			case 33:
			case 125:
			case 135:
			case 151:
			case 167:
			case 184:
			case 263:
			case 289:
			case 360:
			case 362:
			case 364:
			case 373:
			case 387:
			case 417:
			case 459:
			case 479:
			case 488:
			case 546:
			case 552:
			case 565:
			case 580:
			case 593:
			case 598:
			case 601:
			case 604:
			case 611:
			case 614:
			case 632:
				return set[15];
			case 34:
			case 37:
				return set[16];
			case 35:
				return set[17];
			case 36:
			case 71:
			case 75:
			case 130:
			case 345:
			case 420:
				return set[18];
			case 38:
			case 141:
			case 148:
			case 152:
			case 216:
			case 394:
			case 413:
			case 416:
			case 519:
			case 520:
			case 534:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 39:
			case 40:
			case 132:
			case 133:
				return set[19];
			case 41:
			case 134:
			case 219:
			case 370:
			case 397:
			case 415:
			case 431:
			case 462:
			case 468:
			case 492:
			case 522:
			case 536:
			case 550:
			case 569:
			case 584:
			case 608:
			case 618:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 42:
			case 43:
			case 47:
			case 48:
			case 425:
			case 426:
				return set[20];
			case 44:
			case 45:
				return set[21];
			case 46:
			case 143:
			case 150:
			case 348:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 49:
			case 136:
			case 145:
			case 369:
			case 371:
			case 376:
			case 384:
			case 424:
			case 428:
			case 438:
			case 446:
			case 454:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 50:
			case 51:
			case 53:
			case 54:
			case 55:
			case 58:
			case 73:
			case 123:
			case 142:
			case 144:
			case 146:
			case 149:
			case 158:
			case 160:
			case 202:
			case 235:
			case 239:
			case 241:
			case 242:
			case 259:
			case 277:
			case 282:
			case 292:
			case 298:
			case 300:
			case 304:
			case 307:
			case 313:
			case 324:
			case 326:
			case 332:
			case 347:
			case 349:
			case 385:
			case 410:
			case 422:
			case 423:
			case 478:
			case 591:
				return set[22];
			case 52:
			case 56:
			case 66:
				return set[23];
			case 57:
			case 67:
			case 68:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 59:
			case 74:
			case 449:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 60:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 61:
			case 95:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 62:
			case 63:
				return set[24];
			case 64:
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 69:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 70:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 72:
			case 182:
			case 185:
			case 186:
			case 291:
			case 628:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 77:
			case 310:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 78:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 79:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 80:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 81:
			case 254:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 83:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 84:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 85:
			case 401:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 87:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 89:
			case 316:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 91:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 94:
			case 272:
			case 279:
			case 295:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 96:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 97:
			case 189:
			case 194:
			case 196:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 98:
			case 191:
			case 195:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 100:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 101:
			case 224:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 102:
			case 214:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 103:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 104:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 105:
			case 159:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 107:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 108:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 109:
			case 529:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 111:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 112:
			case 172:
			case 201:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 115:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 116:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 118:
			case 213:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 119:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 120:
				return set[25];
			case 122:
				return set[26];
			case 124:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 126:
				return set[27];
			case 127:
				return set[28];
			case 128:
			case 129:
			case 418:
			case 419:
				return set[29];
			case 131:
				return set[30];
			case 137:
			case 138:
			case 275:
			case 284:
				return set[31];
			case 139:
				return set[32];
			case 140:
			case 331:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 147:
				return set[33];
			case 153:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 154:
			case 155:
				return set[34];
			case 157:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 161:
			case 176:
			case 193:
			case 198:
			case 204:
			case 206:
			case 210:
			case 212:
				return set[35];
			case 163:
			case 164:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 165:
			case 168:
			case 276:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 170:
			case 171:
			case 173:
			case 175:
			case 177:
			case 178:
			case 187:
			case 192:
			case 197:
			case 205:
			case 209:
				return set[36];
			case 174:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 181:
				return set[37];
			case 188:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 190:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 199:
			case 200:
				return set[38];
			case 203:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 208:
				return set[39];
			case 215:
			case 481:
			case 556:
			case 571:
			case 578:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 217:
			case 218:
			case 395:
			case 396:
			case 460:
			case 461:
			case 466:
			case 467:
			case 490:
			case 491:
			case 548:
			case 549:
			case 567:
			case 568:
				return set[40];
			case 220:
			case 221:
				return set[41];
			case 227:
			case 228:
				return set[42];
			case 229:
				return set[43];
			case 232:
				return set[44];
			case 233:
			case 234:
			case 337:
				return set[45];
			case 238:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 240:
			case 283:
			case 299:
				return set[46];
			case 243:
			case 244:
			case 265:
			case 266:
			case 280:
			case 281:
			case 296:
			case 297:
				return set[47];
			case 245:
			case 338:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 246:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 247:
				return set[48];
			case 248:
			case 268:
				return set[49];
			case 249:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 252:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 256:
			case 257:
				return set[50];
			case 258:
			case 264:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 260:
			case 261:
				return set[51];
			case 267:
				return set[52];
			case 269:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 270:
			case 271:
				return set[53];
			case 273:
			case 274:
				return set[54];
			case 285:
			case 286:
				return set[55];
			case 287:
				return set[56];
			case 290:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 293:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 301:
				return set[57];
			case 302:
			case 306:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 303:
				return set[58];
			case 311:
			case 312:
				return set[59];
			case 315:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 317:
			case 318:
				return set[60];
			case 320:
			case 321:
				return set[61];
			case 322:
			case 547:
			case 599:
			case 600:
			case 602:
			case 612:
			case 613:
			case 615:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 323:
			case 325:
				return set[62];
			case 327:
			case 333:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 330:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 334:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 335:
			case 336:
			case 340:
			case 341:
			case 398:
			case 399:
				return set[63];
			case 343:
			case 344:
				return set[64];
			case 346:
				return set[65];
			case 352:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 354:
			case 355:
			case 366:
			case 367:
				return set[66];
			case 356:
			case 368:
				return set[67];
			case 357:
				return set[68];
			case 358:
			case 363:
				return set[69];
			case 361:
				return set[70];
			case 375:
			case 377:
			case 378:
			case 521:
			case 535:
				return set[71];
			case 379:
			case 380:
				return set[72];
			case 381:
			case 382:
				return set[73];
			case 383:
			case 388:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 389:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 392:
			case 393:
				return set[74];
			case 402:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					a.Set(17, true);
					a.Set(19, true);
					return a;
				}
			case 403:
				return set[75];
			case 404:
				return set[76];
			case 405:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 409:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 411:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 412:
				return set[77];
			case 421:
				return set[78];
			case 427:
				return set[79];
			case 429:
			case 430:
			case 582:
			case 583:
				return set[80];
			case 432:
			case 433:
			case 434:
			case 439:
			case 440:
			case 585:
			case 607:
			case 617:
				return set[81];
			case 435:
			case 441:
			case 451:
				return set[82];
			case 436:
			case 437:
			case 442:
			case 443:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 445:
			case 447:
			case 453:
				return set[83];
			case 448:
			case 450:
				return set[84];
			case 457:
			case 472:
			case 486:
			case 523:
			case 563:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 458:
			case 527:
				return set[85];
			case 470:
			case 475:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 474:
				return set[86];
			case 476:
				return set[87];
			case 477:
			case 590:
			case 594:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 480:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 493:
			case 494:
			case 506:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 499:
			case 500:
				return set[88];
			case 501:
			case 502:
				return set[89];
			case 503:
			case 504:
			case 515:
				return set[90];
			case 508:
				return set[91];
			case 509:
			case 510:
				return set[92];
			case 511:
			case 512:
			case 596:
				return set[93];
			case 513:
				return set[94];
			case 514:
				return set[95];
			case 516:
			case 526:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 530:
			case 532:
			case 541:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 531:
				return set[96];
			case 533:
				return set[97];
			case 539:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 543:
			case 544:
				return set[98];
			case 545:
			case 553:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 554:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 555:
				return set[99];
			case 558:
			case 559:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 560:
			case 570:
			case 629:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 561:
				return set[100];
			case 586:
				return set[101];
			case 587:
			case 595:
				return set[102];
			case 588:
			case 589:
				return set[103];
			case 597:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 603:
			case 610:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 606:
			case 616:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 621:
				return set[104];
			case 622:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 623:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 624:
			case 625:
				return set[105];
			case 630:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 631:
				return set[106];
			case 633:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 634:
				return set[107];
			case 635:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 636:
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
					goto case 633;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 623;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 389;
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
					currentState = 619;
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
					goto case 389;
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
						goto case 493;
					} else {
						if (la.kind == 103) {
							currentState = 481;
							break;
						} else {
							if (la.kind == 115) {
								currentState = 471;
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
				goto case 169;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				if (la.kind == 37) {
					currentState = 616;
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
					goto case 610;
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
					goto case 389;
				} else {
					isMissingModifier = true;
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (set[110].Get(la.kind)) {
					currentState = 609;
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
					goto case 493;
				} else {
					if (la.kind == 103) {
						stateStack.Push(14);
						goto case 480;
					} else {
						if (la.kind == 115) {
							stateStack.Push(14);
							goto case 470;
						} else {
							if (la.kind == 142) {
								stateStack.Push(14);
								goto case 469;
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
					currentState = 463;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 455;
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
					currentState = 429;
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
				goto case 75;
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
					currentState = 427;
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
					goto case 44;
				} else {
					goto case 41;
				}
			}
			case 44: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 45;
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				if (set[22].Get(la.kind)) {
					goto case 423;
				} else {
					if (la.kind == 22) {
						goto case 46;
					} else {
						goto case 6;
					}
				}
			}
			case 46: {
				if (la == null) { currentState = 46; break; }
				currentState = 47;
				break;
			}
			case 47: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 48;
			}
			case 48: {
				if (la == null) { currentState = 48; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(49);
					goto case 50;
				} else {
					goto case 49;
				}
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (la.kind == 22) {
					goto case 46;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 50: {
				PushContext(Context.Expression, la, t);
				goto case 51;
			}
			case 51: {
				stateStack.Push(52);
				goto case 53;
			}
			case 52: {
				if (la == null) { currentState = 52; break; }
				if (set[112].Get(la.kind)) {
					currentState = 51;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 53: {
				PushContext(Context.Expression, la, t);
				goto case 54;
			}
			case 54: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 55;
			}
			case 55: {
				if (la == null) { currentState = 55; break; }
				if (set[113].Get(la.kind)) {
					currentState = 54;
					break;
				} else {
					if (set[31].Get(la.kind)) {
						stateStack.Push(126);
						goto case 137;
					} else {
						if (la.kind == 220) {
							currentState = 123;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(56);
								goto case 61;
							} else {
								if (la.kind == 35) {
									stateStack.Push(56);
									goto case 57;
								} else {
									Error(la);
									goto case 56;
								}
							}
						}
					}
				}
			}
			case 56: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 57: {
				if (la == null) { currentState = 57; break; }
				Expect(35, la); // "{"
				currentState = 58;
				break;
			}
			case 58: {
				stateStack.Push(59);
				goto case 50;
			}
			case 59: {
				if (la == null) { currentState = 59; break; }
				if (la.kind == 22) {
					currentState = 58;
					break;
				} else {
					goto case 60;
				}
			}
			case 60: {
				if (la == null) { currentState = 60; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				Expect(162, la); // "New"
				currentState = 62;
				break;
			}
			case 62: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 63;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(120);
					goto case 33;
				} else {
					goto case 64;
				}
			}
			case 64: {
				if (la == null) { currentState = 64; break; }
				if (la.kind == 233) {
					currentState = 67;
					break;
				} else {
					goto case 65;
				}
			}
			case 65: {
				Error(la);
				goto case 66;
			}
			case 66: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 67: {
				stateStack.Push(66);
				goto case 68;
			}
			case 68: {
				if (la == null) { currentState = 68; break; }
				Expect(35, la); // "{"
				currentState = 69;
				break;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				if (la.kind == 147) {
					currentState = 70;
					break;
				} else {
					goto case 70;
				}
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				Expect(26, la); // "."
				currentState = 71;
				break;
			}
			case 71: {
				stateStack.Push(72);
				goto case 75;
			}
			case 72: {
				if (la == null) { currentState = 72; break; }
				Expect(20, la); // "="
				currentState = 73;
				break;
			}
			case 73: {
				stateStack.Push(74);
				goto case 50;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				if (la.kind == 22) {
					currentState = 69;
					break;
				} else {
					goto case 60;
				}
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				if (la.kind == 2) {
					goto case 119;
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
								goto case 118;
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
												goto case 117;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 116;
													} else {
														if (la.kind == 65) {
															goto case 115;
														} else {
															if (la.kind == 66) {
																goto case 114;
															} else {
																if (la.kind == 67) {
																	goto case 113;
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
																				goto case 112;
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
																																		goto case 111;
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
																																					goto case 110;
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
																																																goto case 109;
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
																																																						goto case 108;
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
																																																									goto case 107;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 106;
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
																																																																		goto case 105;
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
																																																																							goto case 104;
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
																																																																										goto case 103;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 102;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 101;
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
																																																																																			goto case 100;
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
																																																																																									goto case 99;
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
																																																																																													goto case 98;
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
																																																																																																goto case 97;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 96;
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
																																																																																																																goto case 95;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 94;
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
																																																																																																																								goto case 93;
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
																																																																																																																														goto case 92;
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
																																																																																																																																						goto case 91;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 90;
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
																																																																																																																																																			goto case 89;
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
																																																																																																																																																									goto case 88;
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
																																																																																																																																																												goto case 87;
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
																																																																																																																																																															goto case 86;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 85;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 84;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 83;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 82;
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
																																																																																																																																																																								goto case 81;
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
																																																																																																																																																																													goto case 80;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 79;
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
																																																																																																																																																																																				goto case 78;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 77;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 76;
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
				currentState = stateStack.Pop();
				break;
			}
			case 120: {
				if (la == null) { currentState = 120; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 121;
						break;
					} else {
						goto case 64;
					}
				} else {
					goto case 66;
				}
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				if (la.kind == 35) {
					stateStack.Push(66);
					goto case 57;
				} else {
					if (set[26].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 65;
					}
				}
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				currentState = 66;
				break;
			}
			case 123: {
				stateStack.Push(124);
				goto case 53;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				Expect(144, la); // "Is"
				currentState = 125;
				break;
			}
			case 125: {
				stateStack.Push(56);
				goto case 33;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (set[28].Get(la.kind)) {
					stateStack.Push(126);
					goto case 127;
				} else {
					goto case 56;
				}
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (la.kind == 37) {
					currentState = 132;
					break;
				} else {
					if (set[114].Get(la.kind)) {
						currentState = 128;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 128: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 129;
			}
			case 129: {
				if (la == null) { currentState = 129; break; }
				if (la.kind == 10) {
					currentState = 130;
					break;
				} else {
					goto case 130;
				}
			}
			case 130: {
				stateStack.Push(131);
				goto case 75;
			}
			case 131: {
				if (la == null) { currentState = 131; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 132: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 133;
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (la.kind == 169) {
					currentState = 135;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						if (set[21].Get(la.kind)) {
							stateStack.Push(134);
							goto case 44;
						} else {
							goto case 134;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 134: {
				PopContext();
				goto case 41;
			}
			case 135: {
				stateStack.Push(136);
				goto case 33;
			}
			case 136: {
				if (la == null) { currentState = 136; break; }
				if (la.kind == 22) {
					currentState = 135;
					break;
				} else {
					goto case 41;
				}
			}
			case 137: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 138;
			}
			case 138: {
				if (la == null) { currentState = 138; break; }
				if (set[115].Get(la.kind)) {
					currentState = 139;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 422;
						break;
					} else {
						if (set[116].Get(la.kind)) {
							currentState = 139;
							break;
						} else {
							if (set[114].Get(la.kind)) {
								currentState = 418;
								break;
							} else {
								if (la.kind == 129) {
									currentState = 416;
									break;
								} else {
									if (la.kind == 237) {
										currentState = 413;
										break;
									} else {
										if (la.kind == 10 || la.kind == 17 || la.kind == 19) {
											stateStack.Push(139);
											nextTokenIsPotentialStartOfExpression = true;
											PushContext(Context.Xml, la, t);
											goto case 402;
										} else {
											if (la.kind == 127 || la.kind == 210) {
												stateStack.Push(139);
												goto case 215;
											} else {
												if (la.kind == 58 || la.kind == 126) {
													stateStack.Push(139);
													PushContext(Context.Query, la, t);
													goto case 153;
												} else {
													if (set[33].Get(la.kind)) {
														stateStack.Push(139);
														goto case 147;
													} else {
														if (la.kind == 135) {
															stateStack.Push(139);
															goto case 140;
														} else {
															Error(la);
															goto case 139;
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 139: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				Expect(135, la); // "If"
				currentState = 141;
				break;
			}
			case 141: {
				if (la == null) { currentState = 141; break; }
				Expect(37, la); // "("
				currentState = 142;
				break;
			}
			case 142: {
				stateStack.Push(143);
				goto case 50;
			}
			case 143: {
				if (la == null) { currentState = 143; break; }
				Expect(22, la); // ","
				currentState = 144;
				break;
			}
			case 144: {
				stateStack.Push(145);
				goto case 50;
			}
			case 145: {
				if (la == null) { currentState = 145; break; }
				if (la.kind == 22) {
					currentState = 146;
					break;
				} else {
					goto case 41;
				}
			}
			case 146: {
				stateStack.Push(41);
				goto case 50;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				if (set[117].Get(la.kind)) {
					currentState = 152;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 148;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 148: {
				if (la == null) { currentState = 148; break; }
				Expect(37, la); // "("
				currentState = 149;
				break;
			}
			case 149: {
				stateStack.Push(150);
				goto case 50;
			}
			case 150: {
				if (la == null) { currentState = 150; break; }
				Expect(22, la); // ","
				currentState = 151;
				break;
			}
			case 151: {
				stateStack.Push(41);
				goto case 33;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				Expect(37, la); // "("
				currentState = 146;
				break;
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (la.kind == 126) {
					stateStack.Push(154);
					goto case 214;
				} else {
					if (la.kind == 58) {
						stateStack.Push(154);
						goto case 213;
					} else {
						Error(la);
						goto case 154;
					}
				}
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(154);
					goto case 155;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 155: {
				if (la == null) { currentState = 155; break; }
				if (la.kind == 126) {
					currentState = 211;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 207;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 205;
							break;
						} else {
							if (la.kind == 107) {
								goto case 107;
							} else {
								if (la.kind == 230) {
									currentState = 50;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 201;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 199;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 197;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 170;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 156;
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
			case 156: {
				stateStack.Push(157);
				goto case 162;
			}
			case 157: {
				if (la == null) { currentState = 157; break; }
				Expect(171, la); // "On"
				currentState = 158;
				break;
			}
			case 158: {
				stateStack.Push(159);
				goto case 50;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				Expect(116, la); // "Equals"
				currentState = 160;
				break;
			}
			case 160: {
				stateStack.Push(161);
				goto case 50;
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.kind == 22) {
					currentState = 158;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 162: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(163);
				goto case 169;
			}
			case 163: {
				PopContext();
				goto case 164;
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 166;
				} else {
					goto case 165;
				}
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				Expect(138, la); // "In"
				currentState = 50;
				break;
			}
			case 166: {
				if (la == null) { currentState = 166; break; }
				Expect(63, la); // "As"
				currentState = 167;
				break;
			}
			case 167: {
				stateStack.Push(168);
				goto case 33;
			}
			case 168: {
				PopContext();
				goto case 165;
			}
			case 169: {
				if (la == null) { currentState = 169; break; }
				if (set[102].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 109;
					} else {
						goto case 6;
					}
				}
			}
			case 170: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 171;
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				if (la.kind == 146) {
					goto case 189;
				} else {
					if (set[36].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 173;
							break;
						} else {
							if (set[36].Get(la.kind)) {
								goto case 187;
							} else {
								Error(la);
								goto case 172;
							}
						}
					} else {
						goto case 6;
					}
				}
			}
			case 172: {
				if (la == null) { currentState = 172; break; }
				Expect(70, la); // "By"
				currentState = 173;
				break;
			}
			case 173: {
				stateStack.Push(174);
				goto case 177;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (la.kind == 22) {
					currentState = 173;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 175;
					break;
				}
			}
			case 175: {
				stateStack.Push(176);
				goto case 177;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 22) {
					currentState = 175;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 177: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 178;
			}
			case 178: {
				if (la == null) { currentState = 178; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(179);
					goto case 169;
				} else {
					goto case 50;
				}
			}
			case 179: {
				PopContext();
				goto case 180;
			}
			case 180: {
				if (la == null) { currentState = 180; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 183;
				} else {
					if (la.kind == 20) {
						goto case 182;
					} else {
						if (set[37].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 50;
						}
					}
				}
			}
			case 181: {
				if (la == null) { currentState = 181; break; }
				currentState = 50;
				break;
			}
			case 182: {
				if (la == null) { currentState = 182; break; }
				currentState = 50;
				break;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				Expect(63, la); // "As"
				currentState = 184;
				break;
			}
			case 184: {
				stateStack.Push(185);
				goto case 33;
			}
			case 185: {
				PopContext();
				goto case 186;
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				Expect(20, la); // "="
				currentState = 50;
				break;
			}
			case 187: {
				stateStack.Push(188);
				goto case 177;
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				if (la.kind == 22) {
					currentState = 187;
					break;
				} else {
					goto case 172;
				}
			}
			case 189: {
				stateStack.Push(190);
				goto case 196;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 194;
						break;
					} else {
						if (la.kind == 146) {
							goto case 189;
						} else {
							Error(la);
							goto case 190;
						}
					}
				} else {
					goto case 191;
				}
			}
			case 191: {
				if (la == null) { currentState = 191; break; }
				Expect(143, la); // "Into"
				currentState = 192;
				break;
			}
			case 192: {
				stateStack.Push(193);
				goto case 177;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				if (la.kind == 22) {
					currentState = 192;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 194: {
				stateStack.Push(195);
				goto case 196;
			}
			case 195: {
				stateStack.Push(190);
				goto case 191;
			}
			case 196: {
				if (la == null) { currentState = 196; break; }
				Expect(146, la); // "Join"
				currentState = 156;
				break;
			}
			case 197: {
				stateStack.Push(198);
				goto case 177;
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				if (la.kind == 22) {
					currentState = 197;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 199: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 200;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (la.kind == 231) {
					currentState = 50;
					break;
				} else {
					goto case 50;
				}
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				Expect(70, la); // "By"
				currentState = 202;
				break;
			}
			case 202: {
				stateStack.Push(203);
				goto case 50;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.kind == 64) {
					currentState = 204;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 204;
						break;
					} else {
						Error(la);
						goto case 204;
					}
				}
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				if (la.kind == 22) {
					currentState = 202;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 205: {
				stateStack.Push(206);
				goto case 177;
			}
			case 206: {
				if (la == null) { currentState = 206; break; }
				if (la.kind == 22) {
					currentState = 205;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 207: {
				stateStack.Push(208);
				goto case 162;
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (set[34].Get(la.kind)) {
					stateStack.Push(208);
					goto case 155;
				} else {
					Expect(143, la); // "Into"
					currentState = 209;
					break;
				}
			}
			case 209: {
				stateStack.Push(210);
				goto case 177;
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
				stateStack.Push(212);
				goto case 162;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.kind == 22) {
					currentState = 211;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				Expect(58, la); // "Aggregate"
				currentState = 207;
				break;
			}
			case 214: {
				if (la == null) { currentState = 214; break; }
				Expect(126, la); // "From"
				currentState = 211;
				break;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (la.kind == 210) {
					currentState = 394;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 216;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(37, la); // "("
				currentState = 217;
				break;
			}
			case 217: {
				SetIdentifierExpected(la);
				goto case 218;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(219);
					goto case 375;
				} else {
					goto case 219;
				}
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				Expect(38, la); // ")"
				currentState = 220;
				break;
			}
			case 220: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 221;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				if (set[22].Get(la.kind)) {
					goto case 50;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							PushContext(Context.Type, la, t);
							goto case 372;
						} else {
							goto case 222;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 222: {
				stateStack.Push(223);
				goto case 225;
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				Expect(113, la); // "End"
				currentState = 224;
				break;
			}
			case 224: {
				if (la == null) { currentState = 224; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 225: {
				PushContext(Context.Body, la, t);
				goto case 226;
			}
			case 226: {
				stateStack.Push(227);
				goto case 18;
			}
			case 227: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 228;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (set[118].Get(la.kind)) {
					if (set[63].Get(la.kind)) {
						if (set[45].Get(la.kind)) {
							stateStack.Push(226);
							goto case 233;
						} else {
							goto case 226;
						}
					} else {
						if (la.kind == 113) {
							currentState = 231;
							break;
						} else {
							goto case 230;
						}
					}
				} else {
					goto case 229;
				}
			}
			case 229: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 230: {
				Error(la);
				goto case 227;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 226;
				} else {
					if (set[44].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 230;
					}
				}
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				currentState = 227;
				break;
			}
			case 233: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 234;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 353;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 349;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 347;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 345;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 326;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 311;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 307;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 301;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 273;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 269;
																break;
															} else {
																goto case 269;
															}
														} else {
															if (la.kind == 194) {
																currentState = 267;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 265;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 251;
																break;
															} else {
																if (set[119].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 248;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 247;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 246;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 87;
																				} else {
																					if (la.kind == 195) {
																						currentState = 243;
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
																		currentState = 241;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 239;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 235;
																				break;
																			} else {
																				if (set[120].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 50;
																						break;
																					} else {
																						goto case 50;
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
			case 235: {
				stateStack.Push(236);
				goto case 50;
			}
			case 236: {
				stateStack.Push(237);
				goto case 225;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				Expect(113, la); // "End"
				currentState = 238;
				break;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 239: {
				stateStack.Push(240);
				goto case 50;
			}
			case 240: {
				if (la == null) { currentState = 240; break; }
				if (la.kind == 22) {
					currentState = 239;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 241: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 242;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				if (la.kind == 184) {
					currentState = 50;
					break;
				} else {
					goto case 50;
				}
			}
			case 243: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 244;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(245);
					goto case 50;
				} else {
					goto case 245;
				}
			}
			case 245: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (la.kind == 108) {
					goto case 106;
				} else {
					if (la.kind == 124) {
						goto case 103;
					} else {
						if (la.kind == 231) {
							goto case 77;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 247: {
				if (la == null) { currentState = 247; break; }
				if (la.kind == 108) {
					goto case 106;
				} else {
					if (la.kind == 124) {
						goto case 103;
					} else {
						if (la.kind == 231) {
							goto case 77;
						} else {
							if (la.kind == 197) {
								goto case 89;
							} else {
								if (la.kind == 210) {
									goto case 85;
								} else {
									if (la.kind == 127) {
										goto case 101;
									} else {
										if (la.kind == 186) {
											goto case 90;
										} else {
											if (la.kind == 218) {
												goto case 81;
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
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (set[6].Get(la.kind)) {
					goto case 250;
				} else {
					if (la.kind == 5) {
						goto case 249;
					} else {
						goto case 6;
					}
				}
			}
			case 249: {
				if (la == null) { currentState = 249; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 251: {
				stateStack.Push(252);
				goto case 225;
			}
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (la.kind == 75) {
					currentState = 256;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 255;
						break;
					} else {
						goto case 253;
					}
				}
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				Expect(113, la); // "End"
				currentState = 254;
				break;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 255: {
				stateStack.Push(253);
				goto case 225;
			}
			case 256: {
				SetIdentifierExpected(la);
				goto case 257;
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(260);
					goto case 169;
				} else {
					goto case 258;
				}
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				if (la.kind == 229) {
					currentState = 259;
					break;
				} else {
					goto case 251;
				}
			}
			case 259: {
				stateStack.Push(251);
				goto case 50;
			}
			case 260: {
				PopContext();
				goto case 261;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 262;
				} else {
					goto case 258;
				}
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				Expect(63, la); // "As"
				currentState = 263;
				break;
			}
			case 263: {
				stateStack.Push(264);
				goto case 33;
			}
			case 264: {
				PopContext();
				goto case 258;
			}
			case 265: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 266;
			}
			case 266: {
				if (la == null) { currentState = 266; break; }
				if (set[22].Get(la.kind)) {
					goto case 50;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (la.kind == 163) {
					goto case 94;
				} else {
					goto case 268;
				}
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				if (la.kind == 5) {
					goto case 249;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 250;
					} else {
						goto case 6;
					}
				}
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				Expect(118, la); // "Error"
				currentState = 270;
				break;
			}
			case 270: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 271;
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				if (set[22].Get(la.kind)) {
					goto case 50;
				} else {
					if (la.kind == 132) {
						currentState = 268;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 272;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 273: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 274;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[31].Get(la.kind)) {
					stateStack.Push(291);
					goto case 284;
				} else {
					if (la.kind == 110) {
						currentState = 275;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 275: {
				stateStack.Push(276);
				goto case 284;
			}
			case 276: {
				if (la == null) { currentState = 276; break; }
				Expect(138, la); // "In"
				currentState = 277;
				break;
			}
			case 277: {
				stateStack.Push(278);
				goto case 50;
			}
			case 278: {
				stateStack.Push(279);
				goto case 225;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				Expect(163, la); // "Next"
				currentState = 280;
				break;
			}
			case 280: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 281;
			}
			case 281: {
				if (la == null) { currentState = 281; break; }
				if (set[22].Get(la.kind)) {
					goto case 282;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 282: {
				stateStack.Push(283);
				goto case 50;
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (la.kind == 22) {
					currentState = 282;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 284: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(285);
				goto case 137;
			}
			case 285: {
				PopContext();
				goto case 286;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (la.kind == 33) {
					currentState = 287;
					break;
				} else {
					goto case 287;
				}
			}
			case 287: {
				if (la == null) { currentState = 287; break; }
				if (set[28].Get(la.kind)) {
					stateStack.Push(287);
					goto case 127;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 288;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 288: {
				if (la == null) { currentState = 288; break; }
				Expect(63, la); // "As"
				currentState = 289;
				break;
			}
			case 289: {
				stateStack.Push(290);
				goto case 33;
			}
			case 290: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 291: {
				if (la == null) { currentState = 291; break; }
				Expect(20, la); // "="
				currentState = 292;
				break;
			}
			case 292: {
				stateStack.Push(293);
				goto case 50;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				if (la.kind == 205) {
					currentState = 300;
					break;
				} else {
					goto case 294;
				}
			}
			case 294: {
				stateStack.Push(295);
				goto case 225;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				Expect(163, la); // "Next"
				currentState = 296;
				break;
			}
			case 296: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 297;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				if (set[22].Get(la.kind)) {
					goto case 298;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 298: {
				stateStack.Push(299);
				goto case 50;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (la.kind == 22) {
					currentState = 298;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 300: {
				stateStack.Push(294);
				goto case 50;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 304;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(302);
						goto case 225;
					} else {
						goto case 6;
					}
				}
			}
			case 302: {
				if (la == null) { currentState = 302; break; }
				Expect(152, la); // "Loop"
				currentState = 303;
				break;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 50;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 304: {
				stateStack.Push(305);
				goto case 50;
			}
			case 305: {
				stateStack.Push(306);
				goto case 225;
			}
			case 306: {
				if (la == null) { currentState = 306; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 307: {
				stateStack.Push(308);
				goto case 50;
			}
			case 308: {
				stateStack.Push(309);
				goto case 225;
			}
			case 309: {
				if (la == null) { currentState = 309; break; }
				Expect(113, la); // "End"
				currentState = 310;
				break;
			}
			case 310: {
				if (la == null) { currentState = 310; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 311: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 312;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				if (la.kind == 74) {
					currentState = 313;
					break;
				} else {
					goto case 313;
				}
			}
			case 313: {
				stateStack.Push(314);
				goto case 50;
			}
			case 314: {
				stateStack.Push(315);
				goto case 18;
			}
			case 315: {
				if (la == null) { currentState = 315; break; }
				if (la.kind == 74) {
					currentState = 317;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 316;
					break;
				}
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 317: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 318;
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				if (la.kind == 111) {
					currentState = 319;
					break;
				} else {
					if (set[61].Get(la.kind)) {
						goto case 320;
					} else {
						Error(la);
						goto case 319;
					}
				}
			}
			case 319: {
				stateStack.Push(315);
				goto case 225;
			}
			case 320: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 321;
			}
			case 321: {
				if (la == null) { currentState = 321; break; }
				if (set[121].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 323;
						break;
					} else {
						goto case 323;
					}
				} else {
					if (set[22].Get(la.kind)) {
						stateStack.Push(322);
						goto case 50;
					} else {
						Error(la);
						goto case 322;
					}
				}
			}
			case 322: {
				if (la == null) { currentState = 322; break; }
				if (la.kind == 22) {
					currentState = 320;
					break;
				} else {
					goto case 319;
				}
			}
			case 323: {
				stateStack.Push(324);
				goto case 325;
			}
			case 324: {
				stateStack.Push(322);
				goto case 53;
			}
			case 325: {
				if (la == null) { currentState = 325; break; }
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
			case 326: {
				stateStack.Push(327);
				goto case 50;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
				if (la.kind == 214) {
					currentState = 335;
					break;
				} else {
					goto case 328;
				}
			}
			case 328: {
				if (la == null) { currentState = 328; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 329;
				} else {
					goto case 6;
				}
			}
			case 329: {
				stateStack.Push(330);
				goto case 225;
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 334;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 332;
							break;
						} else {
							Error(la);
							goto case 329;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 331;
					break;
				}
			}
			case 331: {
				if (la == null) { currentState = 331; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 332: {
				stateStack.Push(333);
				goto case 50;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 214) {
					currentState = 329;
					break;
				} else {
					goto case 329;
				}
			}
			case 334: {
				if (la == null) { currentState = 334; break; }
				if (la.kind == 135) {
					currentState = 332;
					break;
				} else {
					goto case 329;
				}
			}
			case 335: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 336;
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (set[45].Get(la.kind)) {
					goto case 337;
				} else {
					goto case 328;
				}
			}
			case 337: {
				stateStack.Push(338);
				goto case 233;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (la.kind == 21) {
					currentState = 343;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 340;
						break;
					} else {
						goto case 339;
					}
				}
			}
			case 339: {
				if (la == null) { currentState = 339; break; }
				Expect(1, la); // EOL
				currentState = stateStack.Pop();
				break;
			}
			case 340: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 341;
			}
			case 341: {
				if (la == null) { currentState = 341; break; }
				if (set[45].Get(la.kind)) {
					stateStack.Push(342);
					goto case 233;
				} else {
					goto case 342;
				}
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (la.kind == 21) {
					currentState = 340;
					break;
				} else {
					goto case 339;
				}
			}
			case 343: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 344;
			}
			case 344: {
				if (la == null) { currentState = 344; break; }
				if (set[45].Get(la.kind)) {
					goto case 337;
				} else {
					goto case 338;
				}
			}
			case 345: {
				stateStack.Push(346);
				goto case 75;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
				if (la.kind == 37) {
					currentState = 42;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 347: {
				stateStack.Push(348);
				goto case 50;
			}
			case 348: {
				if (la == null) { currentState = 348; break; }
				Expect(22, la); // ","
				currentState = 50;
				break;
			}
			case 349: {
				stateStack.Push(350);
				goto case 50;
			}
			case 350: {
				stateStack.Push(351);
				goto case 225;
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				Expect(113, la); // "End"
				currentState = 352;
				break;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 233) {
					goto case 76;
				} else {
					if (la.kind == 211) {
						goto case 84;
					} else {
						goto case 6;
					}
				}
			}
			case 353: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(354);
				goto case 169;
			}
			case 354: {
				PopContext();
				goto case 355;
			}
			case 355: {
				if (la == null) { currentState = 355; break; }
				if (la.kind == 33) {
					currentState = 356;
					break;
				} else {
					goto case 356;
				}
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 37) {
					currentState = 371;
					break;
				} else {
					goto case 357;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 22) {
					currentState = 365;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 359;
					} else {
						goto case 358;
					}
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 20) {
					goto case 182;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				Expect(63, la); // "As"
				currentState = 360;
				break;
			}
			case 360: {
				stateStack.Push(361);
				goto case 33;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 162) {
					currentState = 364;
					break;
				} else {
					goto case 362;
				}
			}
			case 362: {
				stateStack.Push(363);
				goto case 33;
			}
			case 363: {
				if (CurrentBlock.context == Context.ObjectCreation)
					PopContext();
				PopContext();

				goto case 358;
			}
			case 364: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 362;
			}
			case 365: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(366);
				goto case 169;
			}
			case 366: {
				PopContext();
				goto case 367;
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (la.kind == 33) {
					currentState = 368;
					break;
				} else {
					goto case 368;
				}
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				if (la.kind == 37) {
					currentState = 369;
					break;
				} else {
					goto case 357;
				}
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 22) {
					currentState = 369;
					break;
				} else {
					goto case 370;
				}
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				Expect(38, la); // ")"
				currentState = 357;
				break;
			}
			case 371: {
				if (la == null) { currentState = 371; break; }
				if (la.kind == 22) {
					currentState = 371;
					break;
				} else {
					goto case 370;
				}
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				Expect(63, la); // "As"
				currentState = 373;
				break;
			}
			case 373: {
				stateStack.Push(374);
				goto case 33;
			}
			case 374: {
				PopContext();
				goto case 222;
			}
			case 375: {
				stateStack.Push(376);
				PushContext(Context.Parameter, la, t);
				goto case 377;
			}
			case 376: {
				if (la == null) { currentState = 376; break; }
				if (la.kind == 22) {
					currentState = 375;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 377: {
				SetIdentifierExpected(la);
				goto case 378;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				if (la.kind == 40) {
					stateStack.Push(377);
					goto case 389;
				} else {
					goto case 379;
				}
			}
			case 379: {
				SetIdentifierExpected(la);
				goto case 380;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (set[122].Get(la.kind)) {
					currentState = 379;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(381);
					goto case 169;
				}
			}
			case 381: {
				PopContext();
				goto case 382;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 386;
				} else {
					goto case 383;
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				if (la.kind == 20) {
					currentState = 385;
					break;
				} else {
					goto case 384;
				}
			}
			case 384: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 385: {
				stateStack.Push(384);
				goto case 50;
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				Expect(63, la); // "As"
				currentState = 387;
				break;
			}
			case 387: {
				stateStack.Push(388);
				goto case 33;
			}
			case 388: {
				PopContext();
				goto case 383;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				Expect(40, la); // "<"
				currentState = 390;
				break;
			}
			case 390: {
				PushContext(Context.Attribute, la, t);
				goto case 391;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (set[123].Get(la.kind)) {
					currentState = 391;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 392;
					break;
				}
			}
			case 392: {
				PopContext();
				goto case 393;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				Expect(37, la); // "("
				currentState = 395;
				break;
			}
			case 395: {
				SetIdentifierExpected(la);
				goto case 396;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(397);
					goto case 375;
				} else {
					goto case 397;
				}
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				Expect(38, la); // ")"
				currentState = 398;
				break;
			}
			case 398: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 399;
			}
			case 399: {
				if (la == null) { currentState = 399; break; }
				if (set[45].Get(la.kind)) {
					goto case 233;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(400);
						goto case 225;
					} else {
						goto case 6;
					}
				}
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				Expect(113, la); // "End"
				currentState = 401;
				break;
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 402: {
				if (la == null) { currentState = 402; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 412;
					break;
				} else {
					stateStack.Push(403);
					goto case 405;
				}
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				if (la.kind == 17) {
					currentState = 404;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				if (la.kind == 16) {
					currentState = 403;
					break;
				} else {
					goto case 403;
				}
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 406;
				break;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (set[124].Get(la.kind)) {
					if (set[125].Get(la.kind)) {
						currentState = 406;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(406);
							goto case 409;
						} else {
							Error(la);
							goto case 406;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = stateStack.Pop();
						break;
					} else {
						if (la.kind == 11) {
							currentState = 407;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (set[126].Get(la.kind)) {
					if (set[127].Get(la.kind)) {
						currentState = 407;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(407);
							goto case 409;
						} else {
							if (la.kind == 10) {
								stateStack.Push(407);
								goto case 405;
							} else {
								Error(la);
								goto case 407;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 408;
					break;
				}
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				if (set[128].Get(la.kind)) {
					if (set[129].Get(la.kind)) {
						currentState = 408;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(408);
							goto case 409;
						} else {
							Error(la);
							goto case 408;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = stateStack.Pop();
					break;
				}
			}
			case 409: {
				if (la == null) { currentState = 409; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 410;
				break;
			}
			case 410: {
				stateStack.Push(411);
				goto case 50;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (la.kind == 16) {
					currentState = 402;
					break;
				} else {
					goto case 402;
				}
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				Expect(37, la); // "("
				currentState = 414;
				break;
			}
			case 414: {
				readXmlIdentifier = true;
				stateStack.Push(415);
				goto case 169;
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				Expect(38, la); // ")"
				currentState = 139;
				break;
			}
			case 416: {
				if (la == null) { currentState = 416; break; }
				Expect(37, la); // "("
				currentState = 417;
				break;
			}
			case 417: {
				stateStack.Push(415);
				goto case 33;
			}
			case 418: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 419;
			}
			case 419: {
				if (la == null) { currentState = 419; break; }
				if (la.kind == 10) {
					currentState = 420;
					break;
				} else {
					goto case 420;
				}
			}
			case 420: {
				stateStack.Push(421);
				goto case 75;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (la.kind == 11) {
					currentState = 139;
					break;
				} else {
					goto case 139;
				}
			}
			case 422: {
				stateStack.Push(415);
				goto case 50;
			}
			case 423: {
				stateStack.Push(424);
				goto case 50;
			}
			case 424: {
				if (la == null) { currentState = 424; break; }
				if (la.kind == 22) {
					currentState = 425;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 425: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 426;
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (set[22].Get(la.kind)) {
					goto case 423;
				} else {
					goto case 424;
				}
			}
			case 427: {
				if (la == null) { currentState = 427; break; }
				if (set[15].Get(la.kind)) {
					stateStack.Push(428);
					goto case 33;
				} else {
					goto case 428;
				}
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (la.kind == 22) {
					currentState = 427;
					break;
				} else {
					goto case 41;
				}
			}
			case 429: {
				SetIdentifierExpected(la);
				goto case 430;
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (set[130].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 432;
						break;
					} else {
						if (set[71].Get(la.kind)) {
							stateStack.Push(431);
							goto case 375;
						} else {
							Error(la);
							goto case 431;
						}
					}
				} else {
					goto case 431;
				}
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				Expect(38, la); // ")"
				currentState = 29;
				break;
			}
			case 432: {
				stateStack.Push(431);
				goto case 433;
			}
			case 433: {
				SetIdentifierExpected(la);
				goto case 434;
			}
			case 434: {
				if (la == null) { currentState = 434; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 435;
					break;
				} else {
					goto case 435;
				}
			}
			case 435: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(436);
				goto case 451;
			}
			case 436: {
				PopContext();
				goto case 437;
			}
			case 437: {
				if (la == null) { currentState = 437; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 452;
				} else {
					goto case 438;
				}
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 22) {
					currentState = 439;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 439: {
				SetIdentifierExpected(la);
				goto case 440;
			}
			case 440: {
				if (la == null) { currentState = 440; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 441;
					break;
				} else {
					goto case 441;
				}
			}
			case 441: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(442);
				goto case 451;
			}
			case 442: {
				PopContext();
				goto case 443;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 444;
				} else {
					goto case 438;
				}
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				Expect(63, la); // "As"
				currentState = 445;
				break;
			}
			case 445: {
				stateStack.Push(446);
				goto case 447;
			}
			case 446: {
				PopContext();
				goto case 438;
			}
			case 447: {
				if (la == null) { currentState = 447; break; }
				if (set[84].Get(la.kind)) {
					goto case 450;
				} else {
					if (la.kind == 35) {
						currentState = 448;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 448: {
				stateStack.Push(449);
				goto case 450;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				if (la.kind == 22) {
					currentState = 448;
					break;
				} else {
					goto case 60;
				}
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				if (set[15].Get(la.kind)) {
					currentState = 34;
					break;
				} else {
					if (la.kind == 162) {
						goto case 95;
					} else {
						if (la.kind == 84) {
							goto case 111;
						} else {
							if (la.kind == 209) {
								goto case 86;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 2) {
					goto case 119;
				} else {
					if (la.kind == 62) {
						goto case 117;
					} else {
						if (la.kind == 64) {
							goto case 116;
						} else {
							if (la.kind == 65) {
								goto case 115;
							} else {
								if (la.kind == 66) {
									goto case 114;
								} else {
									if (la.kind == 67) {
										goto case 113;
									} else {
										if (la.kind == 70) {
											goto case 112;
										} else {
											if (la.kind == 87) {
												goto case 110;
											} else {
												if (la.kind == 104) {
													goto case 108;
												} else {
													if (la.kind == 107) {
														goto case 107;
													} else {
														if (la.kind == 116) {
															goto case 105;
														} else {
															if (la.kind == 121) {
																goto case 104;
															} else {
																if (la.kind == 133) {
																	goto case 100;
																} else {
																	if (la.kind == 139) {
																		goto case 99;
																	} else {
																		if (la.kind == 143) {
																			goto case 98;
																		} else {
																			if (la.kind == 146) {
																				goto case 97;
																			} else {
																				if (la.kind == 147) {
																					goto case 96;
																				} else {
																					if (la.kind == 170) {
																						goto case 93;
																					} else {
																						if (la.kind == 176) {
																							goto case 92;
																						} else {
																							if (la.kind == 184) {
																								goto case 91;
																							} else {
																								if (la.kind == 203) {
																									goto case 88;
																								} else {
																									if (la.kind == 212) {
																										goto case 83;
																									} else {
																										if (la.kind == 213) {
																											goto case 82;
																										} else {
																											if (la.kind == 223) {
																												goto case 80;
																											} else {
																												if (la.kind == 224) {
																													goto case 79;
																												} else {
																													if (la.kind == 230) {
																														goto case 78;
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
			case 452: {
				if (la == null) { currentState = 452; break; }
				Expect(63, la); // "As"
				currentState = 453;
				break;
			}
			case 453: {
				stateStack.Push(454);
				goto case 447;
			}
			case 454: {
				PopContext();
				goto case 438;
			}
			case 455: {
				stateStack.Push(456);
				goto case 169;
			}
			case 456: {
				if (la == null) { currentState = 456; break; }
				if (la.kind == 37) {
					currentState = 460;
					break;
				} else {
					goto case 457;
				}
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				if (la.kind == 63) {
					currentState = 458;
					break;
				} else {
					goto case 18;
				}
			}
			case 458: {
				if (la == null) { currentState = 458; break; }
				if (la.kind == 40) {
					stateStack.Push(458);
					goto case 389;
				} else {
					goto case 459;
				}
			}
			case 459: {
				stateStack.Push(18);
				goto case 33;
			}
			case 460: {
				SetIdentifierExpected(la);
				goto case 461;
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(462);
					goto case 375;
				} else {
					goto case 462;
				}
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				Expect(38, la); // ")"
				currentState = 457;
				break;
			}
			case 463: {
				stateStack.Push(464);
				goto case 169;
			}
			case 464: {
				if (la == null) { currentState = 464; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 459;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 466;
							break;
						} else {
							goto case 465;
						}
					}
				} else {
					goto case 18;
				}
			}
			case 465: {
				Error(la);
				goto case 18;
			}
			case 466: {
				SetIdentifierExpected(la);
				goto case 467;
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(468);
					goto case 375;
				} else {
					goto case 468;
				}
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				Expect(38, la); // ")"
				currentState = 18;
				break;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				Expect(142, la); // "Interface"
				currentState = 9;
				break;
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				Expect(115, la); // "Enum"
				currentState = 471;
				break;
			}
			case 471: {
				stateStack.Push(472);
				goto case 169;
			}
			case 472: {
				if (la == null) { currentState = 472; break; }
				if (la.kind == 63) {
					currentState = 479;
					break;
				} else {
					goto case 473;
				}
			}
			case 473: {
				stateStack.Push(474);
				goto case 18;
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				if (set[87].Get(la.kind)) {
					goto case 476;
				} else {
					Expect(113, la); // "End"
					currentState = 475;
					break;
				}
			}
			case 475: {
				if (la == null) { currentState = 475; break; }
				Expect(115, la); // "Enum"
				currentState = 18;
				break;
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				if (la.kind == 40) {
					stateStack.Push(476);
					goto case 389;
				} else {
					stateStack.Push(477);
					goto case 169;
				}
			}
			case 477: {
				if (la == null) { currentState = 477; break; }
				if (la.kind == 20) {
					currentState = 478;
					break;
				} else {
					goto case 473;
				}
			}
			case 478: {
				stateStack.Push(473);
				goto case 50;
			}
			case 479: {
				stateStack.Push(473);
				goto case 33;
			}
			case 480: {
				if (la == null) { currentState = 480; break; }
				Expect(103, la); // "Delegate"
				currentState = 481;
				break;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				if (la.kind == 210) {
					currentState = 482;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 482;
						break;
					} else {
						Error(la);
						goto case 482;
					}
				}
			}
			case 482: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 483;
			}
			case 483: {
				if (la == null) { currentState = 483; break; }
				currentState = 484;
				break;
			}
			case 484: {
				PopContext();
				goto case 485;
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
					goto case 18;
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
				goto case 33;
			}
			case 489: {
				PopContext();
				goto case 18;
			}
			case 490: {
				SetIdentifierExpected(la);
				goto case 491;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(492);
					goto case 375;
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
				PushContext(Context.TypeDeclaration, la, t);
				goto case 494;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (la.kind == 155) {
					currentState = 495;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 495;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 495;
							break;
						} else {
							Error(la);
							goto case 495;
						}
					}
				}
			}
			case 495: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(496);
				goto case 169;
			}
			case 496: {
				PopContext();
				goto case 497;
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				if (la.kind == 37) {
					currentState = 606;
					break;
				} else {
					goto case 498;
				}
			}
			case 498: {
				stateStack.Push(499);
				goto case 18;
			}
			case 499: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 500;
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					PushContext(Context.Type, la, t);
					goto case 603;
				} else {
					goto case 501;
				}
			}
			case 501: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 502;
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					PushContext(Context.Type, la, t);
					goto case 597;
				} else {
					goto case 503;
				}
			}
			case 503: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 504;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				if (set[92].Get(la.kind)) {
					goto case 509;
				} else {
					isMissingModifier = false;
					goto case 505;
				}
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				Expect(113, la); // "End"
				currentState = 506;
				break;
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				if (la.kind == 155) {
					currentState = 507;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 507;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 507;
							break;
						} else {
							Error(la);
							goto case 507;
						}
					}
				}
			}
			case 507: {
				stateStack.Push(508);
				goto case 18;
			}
			case 508: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 509: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 510;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				if (la.kind == 40) {
					stateStack.Push(509);
					goto case 389;
				} else {
					isMissingModifier = true;
					goto case 511;
				}
			}
			case 511: {
				SetIdentifierExpected(la);
				goto case 512;
			}
			case 512: {
				if (la == null) { currentState = 512; break; }
				if (set[110].Get(la.kind)) {
					currentState = 596;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 513;
				}
			}
			case 513: {
				if (la == null) { currentState = 513; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(503);
					goto case 493;
				} else {
					if (la.kind == 103) {
						stateStack.Push(503);
						goto case 480;
					} else {
						if (la.kind == 115) {
							stateStack.Push(503);
							goto case 470;
						} else {
							if (la.kind == 142) {
								stateStack.Push(503);
								goto case 469;
							} else {
								if (set[95].Get(la.kind)) {
									stateStack.Push(503);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 514;
								} else {
									Error(la);
									goto case 503;
								}
							}
						}
					}
				}
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (set[101].Get(la.kind)) {
					stateStack.Push(515);
					SetIdentifierExpected(la);
					goto case 586;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(515);
						goto case 571;
					} else {
						if (la.kind == 101) {
							stateStack.Push(515);
							goto case 554;
						} else {
							if (la.kind == 119) {
								stateStack.Push(515);
								goto case 541;
							} else {
								if (la.kind == 98) {
									stateStack.Push(515);
									goto case 529;
								} else {
									if (la.kind == 172) {
										stateStack.Push(515);
										goto case 516;
									} else {
										Error(la);
										goto case 515;
									}
								}
							}
						}
					}
				}
			}
			case 515: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				Expect(172, la); // "Operator"
				currentState = 517;
				break;
			}
			case 517: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 518;
			}
			case 518: {
				if (la == null) { currentState = 518; break; }
				currentState = 519;
				break;
			}
			case 519: {
				PopContext();
				goto case 520;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				Expect(37, la); // "("
				currentState = 521;
				break;
			}
			case 521: {
				stateStack.Push(522);
				goto case 375;
			}
			case 522: {
				if (la == null) { currentState = 522; break; }
				Expect(38, la); // ")"
				currentState = 523;
				break;
			}
			case 523: {
				if (la == null) { currentState = 523; break; }
				if (la.kind == 63) {
					currentState = 527;
					break;
				} else {
					goto case 524;
				}
			}
			case 524: {
				stateStack.Push(525);
				goto case 225;
			}
			case 525: {
				if (la == null) { currentState = 525; break; }
				Expect(113, la); // "End"
				currentState = 526;
				break;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				Expect(172, la); // "Operator"
				currentState = 18;
				break;
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 40) {
					stateStack.Push(527);
					goto case 389;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(528);
					goto case 33;
				}
			}
			case 528: {
				PopContext();
				goto case 524;
			}
			case 529: {
				if (la == null) { currentState = 529; break; }
				Expect(98, la); // "Custom"
				currentState = 530;
				break;
			}
			case 530: {
				stateStack.Push(531);
				goto case 541;
			}
			case 531: {
				if (la == null) { currentState = 531; break; }
				if (set[97].Get(la.kind)) {
					goto case 533;
				} else {
					Expect(113, la); // "End"
					currentState = 532;
					break;
				}
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				Expect(119, la); // "Event"
				currentState = 18;
				break;
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (la.kind == 40) {
					stateStack.Push(533);
					goto case 389;
				} else {
					if (la.kind == 56) {
						currentState = 534;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 534;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 534;
								break;
							} else {
								Error(la);
								goto case 534;
							}
						}
					}
				}
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				Expect(37, la); // "("
				currentState = 535;
				break;
			}
			case 535: {
				stateStack.Push(536);
				goto case 375;
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				Expect(38, la); // ")"
				currentState = 537;
				break;
			}
			case 537: {
				stateStack.Push(538);
				goto case 225;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				Expect(113, la); // "End"
				currentState = 539;
				break;
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				if (la.kind == 56) {
					currentState = 540;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 540;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 540;
							break;
						} else {
							Error(la);
							goto case 540;
						}
					}
				}
			}
			case 540: {
				stateStack.Push(531);
				goto case 18;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				Expect(119, la); // "Event"
				currentState = 542;
				break;
			}
			case 542: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(543);
				goto case 169;
			}
			case 543: {
				PopContext();
				goto case 544;
			}
			case 544: {
				if (la == null) { currentState = 544; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 551;
				} else {
					if (set[131].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 548;
							break;
						} else {
							goto case 545;
						}
					} else {
						Error(la);
						goto case 545;
					}
				}
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				if (la.kind == 136) {
					currentState = 546;
					break;
				} else {
					goto case 18;
				}
			}
			case 546: {
				stateStack.Push(547);
				goto case 33;
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				if (la.kind == 22) {
					currentState = 546;
					break;
				} else {
					goto case 18;
				}
			}
			case 548: {
				SetIdentifierExpected(la);
				goto case 549;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(550);
					goto case 375;
				} else {
					goto case 550;
				}
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				Expect(38, la); // ")"
				currentState = 545;
				break;
			}
			case 551: {
				if (la == null) { currentState = 551; break; }
				Expect(63, la); // "As"
				currentState = 552;
				break;
			}
			case 552: {
				stateStack.Push(553);
				goto case 33;
			}
			case 553: {
				PopContext();
				goto case 545;
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				Expect(101, la); // "Declare"
				currentState = 555;
				break;
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 556;
					break;
				} else {
					goto case 556;
				}
			}
			case 556: {
				if (la == null) { currentState = 556; break; }
				if (la.kind == 210) {
					currentState = 557;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 557;
						break;
					} else {
						Error(la);
						goto case 557;
					}
				}
			}
			case 557: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(558);
				goto case 169;
			}
			case 558: {
				PopContext();
				goto case 559;
			}
			case 559: {
				if (la == null) { currentState = 559; break; }
				Expect(149, la); // "Lib"
				currentState = 560;
				break;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				Expect(3, la); // LiteralString
				currentState = 561;
				break;
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				if (la.kind == 59) {
					currentState = 570;
					break;
				} else {
					goto case 562;
				}
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				if (la.kind == 37) {
					currentState = 567;
					break;
				} else {
					goto case 563;
				}
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 564;
				} else {
					goto case 18;
				}
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				Expect(63, la); // "As"
				currentState = 565;
				break;
			}
			case 565: {
				stateStack.Push(566);
				goto case 33;
			}
			case 566: {
				PopContext();
				goto case 18;
			}
			case 567: {
				SetIdentifierExpected(la);
				goto case 568;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (set[71].Get(la.kind)) {
					stateStack.Push(569);
					goto case 375;
				} else {
					goto case 569;
				}
			}
			case 569: {
				if (la == null) { currentState = 569; break; }
				Expect(38, la); // ")"
				currentState = 563;
				break;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				Expect(3, la); // LiteralString
				currentState = 562;
				break;
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				if (la.kind == 210) {
					currentState = 572;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 572;
						break;
					} else {
						Error(la);
						goto case 572;
					}
				}
			}
			case 572: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 573;
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				currentState = 574;
				break;
			}
			case 574: {
				PopContext();
				goto case 575;
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				if (la.kind == 37) {
					currentState = 582;
					break;
				} else {
					if (la.kind == 63) {
						PushContext(Context.Type, la, t);
						goto case 579;
					} else {
						goto case 576;
					}
				}
			}
			case 576: {
				stateStack.Push(577);
				goto case 225;
			}
			case 577: {
				if (la == null) { currentState = 577; break; }
				Expect(113, la); // "End"
				currentState = 578;
				break;
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (la.kind == 210) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 18;
						break;
					} else {
						goto case 465;
					}
				}
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				Expect(63, la); // "As"
				currentState = 580;
				break;
			}
			case 580: {
				stateStack.Push(581);
				goto case 33;
			}
			case 581: {
				PopContext();
				goto case 576;
			}
			case 582: {
				SetIdentifierExpected(la);
				goto case 583;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (set[130].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 585;
						break;
					} else {
						if (set[71].Get(la.kind)) {
							stateStack.Push(584);
							goto case 375;
						} else {
							Error(la);
							goto case 584;
						}
					}
				} else {
					goto case 584;
				}
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				Expect(38, la); // ")"
				currentState = 575;
				break;
			}
			case 585: {
				stateStack.Push(584);
				goto case 433;
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				if (la.kind == 88) {
					currentState = 587;
					break;
				} else {
					goto case 587;
				}
			}
			case 587: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(588);
				goto case 595;
			}
			case 588: {
				PopContext();
				goto case 589;
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (la.kind == 63) {
					PushContext(Context.Type, la, t);
					goto case 592;
				} else {
					goto case 590;
				}
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (la.kind == 20) {
					currentState = 591;
					break;
				} else {
					goto case 18;
				}
			}
			case 591: {
				stateStack.Push(18);
				goto case 50;
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				Expect(63, la); // "As"
				currentState = 593;
				break;
			}
			case 593: {
				stateStack.Push(594);
				goto case 33;
			}
			case 594: {
				PopContext();
				goto case 590;
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				if (set[116].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 118;
					} else {
						if (la.kind == 126) {
							goto case 102;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 596: {
				isMissingModifier = false;
				goto case 511;
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				Expect(136, la); // "Implements"
				currentState = 598;
				break;
			}
			case 598: {
				stateStack.Push(599);
				goto case 33;
			}
			case 599: {
				PopContext();
				goto case 600;
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				if (la.kind == 22) {
					currentState = 601;
					break;
				} else {
					stateStack.Push(503);
					goto case 18;
				}
			}
			case 601: {
				PushContext(Context.Type, la, t);
				stateStack.Push(602);
				goto case 33;
			}
			case 602: {
				PopContext();
				goto case 600;
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				Expect(140, la); // "Inherits"
				currentState = 604;
				break;
			}
			case 604: {
				stateStack.Push(605);
				goto case 33;
			}
			case 605: {
				PopContext();
				stateStack.Push(501);
				goto case 18;
			}
			case 606: {
				if (la == null) { currentState = 606; break; }
				Expect(169, la); // "Of"
				currentState = 607;
				break;
			}
			case 607: {
				stateStack.Push(608);
				goto case 433;
			}
			case 608: {
				if (la == null) { currentState = 608; break; }
				Expect(38, la); // ")"
				currentState = 498;
				break;
			}
			case 609: {
				isMissingModifier = false;
				goto case 23;
			}
			case 610: {
				if (la == null) { currentState = 610; break; }
				Expect(140, la); // "Inherits"
				currentState = 611;
				break;
			}
			case 611: {
				stateStack.Push(612);
				goto case 33;
			}
			case 612: {
				PopContext();
				goto case 613;
			}
			case 613: {
				if (la == null) { currentState = 613; break; }
				if (la.kind == 22) {
					currentState = 614;
					break;
				} else {
					stateStack.Push(14);
					goto case 18;
				}
			}
			case 614: {
				PushContext(Context.Type, la, t);
				stateStack.Push(615);
				goto case 33;
			}
			case 615: {
				PopContext();
				goto case 613;
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				Expect(169, la); // "Of"
				currentState = 617;
				break;
			}
			case 617: {
				stateStack.Push(618);
				goto case 433;
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				Expect(38, la); // ")"
				currentState = 11;
				break;
			}
			case 619: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 620;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				if (set[44].Get(la.kind)) {
					currentState = 620;
					break;
				} else {
					PopContext();
					stateStack.Push(621);
					goto case 18;
				}
			}
			case 621: {
				if (la == null) { currentState = 621; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(621);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 622;
					break;
				}
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				Expect(160, la); // "Namespace"
				currentState = 18;
				break;
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				Expect(137, la); // "Imports"
				currentState = 624;
				break;
			}
			case 624: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 625;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				if (set[15].Get(la.kind)) {
					currentState = 631;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 627;
						break;
					} else {
						Error(la);
						goto case 626;
					}
				}
			}
			case 626: {
				PopContext();
				goto case 18;
			}
			case 627: {
				stateStack.Push(628);
				goto case 169;
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				Expect(20, la); // "="
				currentState = 629;
				break;
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				Expect(3, la); // LiteralString
				currentState = 630;
				break;
			}
			case 630: {
				if (la == null) { currentState = 630; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 626;
				break;
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				if (la.kind == 37) {
					stateStack.Push(631);
					goto case 38;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 632;
						break;
					} else {
						goto case 626;
					}
				}
			}
			case 632: {
				stateStack.Push(626);
				goto case 33;
			}
			case 633: {
				if (la == null) { currentState = 633; break; }
				Expect(173, la); // "Option"
				currentState = 634;
				break;
			}
			case 634: {
				if (la == null) { currentState = 634; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 636;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 635;
						break;
					} else {
						goto case 465;
					}
				}
			}
			case 635: {
				if (la == null) { currentState = 635; break; }
				if (la.kind == 213) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 18;
						break;
					} else {
						goto case 465;
					}
				}
			}
			case 636: {
				if (la == null) { currentState = 636; break; }
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