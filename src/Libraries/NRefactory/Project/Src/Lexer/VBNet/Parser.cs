using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 49;
	const int endOfStatementTerminatorAndBlock = 226;
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
			case 227:
			case 458:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 155:
			case 161:
			case 167:
			case 204:
			case 208:
			case 247:
			case 347:
			case 356:
			case 409:
			case 448:
			case 456:
			case 464:
			case 487:
			case 522:
			case 576:
			case 590:
			case 659:
				return set[6];
			case 10:
			case 488:
			case 489:
			case 533:
			case 543:
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
			case 31:
			case 219:
			case 222:
			case 223:
			case 233:
			case 248:
			case 252:
			case 274:
			case 289:
			case 300:
			case 303:
			case 309:
			case 314:
			case 323:
			case 324:
			case 344:
			case 364:
			case 466:
			case 481:
			case 490:
			case 499:
			case 516:
			case 520:
			case 528:
			case 534:
			case 537:
			case 544:
			case 547:
			case 571:
			case 574:
			case 598:
			case 608:
			case 612:
			case 637:
			case 658:
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
			case 220:
			case 234:
			case 250:
			case 304:
			case 345:
			case 389:
			case 497:
			case 517:
			case 535:
			case 539:
			case 545:
			case 572:
			case 609:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 17:
			case 462:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 20:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 21:
			case 22:
				return set[9];
			case 23:
			case 641:
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
			case 228:
			case 379:
			case 380:
			case 397:
			case 398:
			case 399:
			case 400:
			case 475:
			case 476:
			case 509:
			case 510:
			case 604:
			case 605:
			case 651:
			case 652:
				return set[13];
			case 28:
			case 29:
			case 449:
			case 457:
			case 477:
			case 478:
			case 595:
			case 606:
			case 607:
				return set[14];
			case 30:
			case 32:
			case 124:
			case 134:
			case 150:
			case 165:
			case 181:
			case 259:
			case 284:
			case 363:
			case 376:
			case 412:
			case 452:
			case 472:
			case 480:
			case 556:
			case 580:
			case 585:
			case 597:
			case 611:
			case 630:
			case 633:
			case 636:
			case 643:
			case 646:
			case 664:
				return set[15];
			case 33:
			case 36:
				return set[16];
			case 34:
				return set[17];
			case 35:
			case 70:
			case 74:
			case 129:
			case 339:
			case 415:
				return set[18];
			case 37:
			case 140:
			case 147:
			case 151:
			case 213:
			case 383:
			case 408:
			case 411:
			case 511:
			case 512:
			case 568:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 38:
			case 39:
			case 131:
			case 132:
				return set[19];
			case 40:
			case 133:
			case 216:
			case 361:
			case 386:
			case 410:
			case 426:
			case 455:
			case 461:
			case 484:
			case 514:
			case 550:
			case 553:
			case 562:
			case 570:
			case 584:
			case 601:
			case 615:
			case 640:
			case 650:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 41:
			case 42:
			case 46:
			case 47:
			case 420:
			case 421:
				return set[20];
			case 43:
			case 44:
				return set[21];
			case 45:
			case 142:
			case 149:
			case 342:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 48:
			case 135:
			case 144:
			case 360:
			case 362:
			case 366:
			case 374:
			case 419:
			case 423:
			case 433:
			case 440:
			case 447:
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
			case 141:
			case 143:
			case 145:
			case 148:
			case 157:
			case 159:
			case 199:
			case 232:
			case 236:
			case 238:
			case 239:
			case 256:
			case 273:
			case 278:
			case 287:
			case 293:
			case 295:
			case 299:
			case 302:
			case 308:
			case 319:
			case 321:
			case 327:
			case 341:
			case 343:
			case 375:
			case 402:
			case 417:
			case 418:
			case 471:
			case 555:
				return set[22];
			case 51:
			case 55:
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
			case 443:
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
			case 65:
				return set[25];
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
			case 182:
			case 183:
			case 286:
			case 660:
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
			case 390:
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
			case 521:
			case 540:
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
			case 268:
			case 275:
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
			case 186:
			case 191:
			case 193:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 97:
			case 188:
			case 192:
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
			case 221:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 101:
			case 211:
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
			case 158:
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
			case 563:
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
			case 198:
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
			case 210:
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
				return set[26];
			case 121:
				return set[27];
			case 123:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 125:
				return set[28];
			case 126:
				return set[29];
			case 127:
			case 128:
			case 413:
			case 414:
				return set[30];
			case 130:
				return set[31];
			case 136:
			case 137:
			case 271:
			case 280:
				return set[32];
			case 138:
			case 392:
				return set[33];
			case 139:
			case 326:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 146:
				return set[34];
			case 152:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 153:
			case 154:
				return set[35];
			case 156:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 160:
			case 174:
			case 190:
			case 195:
			case 201:
			case 203:
			case 207:
			case 209:
				return set[36];
			case 162:
			case 163:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 164:
			case 166:
			case 272:
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
			case 184:
			case 189:
			case 194:
			case 202:
			case 206:
				return set[37];
			case 172:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 179:
				return set[38];
			case 185:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 187:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 196:
			case 197:
				return set[39];
			case 200:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 205:
				return set[40];
			case 212:
			case 474:
			case 589:
			case 603:
			case 610:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 214:
			case 215:
			case 384:
			case 385:
			case 453:
			case 454:
			case 459:
			case 460:
			case 482:
			case 483:
			case 548:
			case 549:
			case 551:
			case 552:
			case 560:
			case 561:
			case 582:
			case 583:
			case 599:
			case 600:
				return set[41];
			case 217:
			case 218:
				return set[42];
			case 224:
			case 225:
				return set[43];
			case 226:
				return set[44];
			case 229:
				return set[45];
			case 230:
			case 231:
			case 332:
				return set[46];
			case 235:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 237:
			case 279:
			case 294:
				return set[47];
			case 240:
			case 241:
			case 261:
			case 262:
			case 276:
			case 277:
			case 291:
			case 292:
				return set[48];
			case 242:
			case 333:
			case 336:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 243:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 244:
				return set[49];
			case 245:
			case 264:
				return set[50];
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
				return set[51];
			case 255:
			case 260:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 257:
			case 258:
				return set[52];
			case 263:
				return set[53];
			case 265:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 266:
			case 267:
				return set[54];
			case 269:
			case 270:
				return set[55];
			case 281:
			case 282:
				return set[56];
			case 283:
				return set[57];
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
				return set[58];
			case 297:
			case 301:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 298:
				return set[59];
			case 306:
			case 307:
				return set[60];
			case 310:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 312:
			case 313:
				return set[61];
			case 315:
			case 316:
				return set[62];
			case 317:
			case 581:
			case 618:
			case 631:
			case 632:
			case 634:
			case 644:
			case 645:
			case 647:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 318:
			case 320:
				return set[63];
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
			case 387:
			case 388:
				return set[64];
			case 334:
			case 335:
			case 337:
			case 338:
				return set[65];
			case 340:
				return set[66];
			case 346:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 348:
			case 349:
			case 357:
			case 358:
				return set[67];
			case 350:
			case 359:
				return set[68];
			case 351:
				return set[69];
			case 352:
			case 355:
				return set[70];
			case 353:
			case 354:
			case 624:
			case 625:
				return set[71];
			case 365:
			case 367:
			case 368:
			case 513:
			case 569:
				return set[72];
			case 369:
			case 370:
				return set[73];
			case 371:
			case 372:
				return set[74];
			case 373:
			case 377:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 378:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 381:
			case 382:
				return set[75];
			case 391:
				return set[76];
			case 393:
			case 406:
				return set[77];
			case 394:
			case 407:
				return set[78];
			case 395:
			case 396:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 401:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 403:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 404:
				return set[79];
			case 405:
				return set[80];
			case 416:
				return set[81];
			case 422:
				return set[82];
			case 424:
			case 425:
			case 613:
			case 614:
				return set[83];
			case 427:
			case 428:
			case 429:
			case 434:
			case 435:
			case 616:
			case 639:
			case 649:
				return set[84];
			case 430:
			case 436:
			case 445:
				return set[85];
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
			case 439:
			case 441:
			case 446:
				return set[86];
			case 442:
			case 444:
				return set[87];
			case 450:
			case 465:
			case 479:
			case 515:
			case 596:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 451:
			case 519:
				return set[88];
			case 463:
			case 468:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 467:
				return set[89];
			case 469:
				return set[90];
			case 470:
			case 527:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 473:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 485:
			case 486:
			case 498:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 491:
			case 492:
				return set[91];
			case 493:
			case 494:
				return set[92];
			case 495:
			case 496:
			case 507:
				return set[93];
			case 500:
				return set[94];
			case 501:
			case 502:
				return set[95];
			case 503:
			case 504:
			case 628:
				return set[96];
			case 505:
				return set[97];
			case 506:
				return set[98];
			case 508:
			case 518:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 523:
			case 524:
				return set[99];
			case 525:
				return set[100];
			case 526:
			case 559:
				return set[101];
			case 529:
			case 530:
			case 531:
			case 554:
				return set[102];
			case 532:
			case 536:
			case 546:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 538:
				return set[103];
			case 541:
				return set[104];
			case 542:
				return set[105];
			case 557:
			case 623:
			case 626:
				return set[106];
			case 558:
				return set[107];
			case 564:
			case 566:
			case 575:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 565:
				return set[108];
			case 567:
				return set[109];
			case 573:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 577:
			case 578:
				return set[110];
			case 579:
			case 586:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 587:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 588:
				return set[111];
			case 591:
			case 592:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 593:
			case 602:
			case 661:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 594:
				return set[112];
			case 617:
			case 619:
				return set[113];
			case 620:
			case 627:
				return set[114];
			case 621:
			case 622:
				return set[115];
			case 629:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 635:
			case 642:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 638:
			case 648:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 653:
				return set[116];
			case 654:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 655:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 656:
			case 657:
				return set[117];
			case 662:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 663:
				return set[118];
			case 665:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 666:
				return set[119];
			case 667:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 668:
				return set[120];
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
					goto case 665;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 655;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 378;
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
					currentState = 651;
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
					goto case 378;
				} else {
					goto case 8;
				}
			}
			case 8: {
				if (la == null) { currentState = 8; break; }
				if (set[121].Get(la.kind)) {
					currentState = 8;
					break;
				} else {
					if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
						goto case 485;
					} else {
						if (la.kind == 103) {
							currentState = 474;
							break;
						} else {
							if (la.kind == 115) {
								currentState = 464;
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
					currentState = 648;
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
					goto case 642;
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
					goto case 378;
				} else {
					isMissingModifier = true;
					goto case 23;
				}
			}
			case 23: {
				if (la == null) { currentState = 23; break; }
				if (set[122].Get(la.kind)) {
					currentState = 641;
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
					goto case 485;
				} else {
					if (la.kind == 103) {
						stateStack.Push(14);
						goto case 473;
					} else {
						if (la.kind == 115) {
							stateStack.Push(14);
							goto case 463;
						} else {
							if (la.kind == 142) {
								stateStack.Push(14);
								goto case 462;
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
					currentState = 456;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 448;
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
						currentState = 30;
						break;
					} else {
						goto case 18;
					}
				}
			}
			case 30: {
				PushContext(Context.Type, la, t);
				stateStack.Push(31);
				goto case 32;
			}
			case 31: {
				PopContext();
				goto case 18;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				if (la.kind == 130) {
					currentState = 33;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 33;
						break;
					} else {
						if (set[123].Get(la.kind)) {
							currentState = 33;
							break;
						} else {
							if (la.kind == 33) {
								currentState = 33;
								break;
							} else {
								Error(la);
								goto case 33;
							}
						}
					}
				}
			}
			case 33: {
				if (la == null) { currentState = 33; break; }
				if (la.kind == 37) {
					stateStack.Push(33);
					goto case 37;
				} else {
					goto case 34;
				}
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 26) {
					currentState = 35;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 35: {
				stateStack.Push(36);
				goto case 74;
			}
			case 36: {
				if (la == null) { currentState = 36; break; }
				if (la.kind == 37) {
					stateStack.Push(36);
					goto case 37;
				} else {
					goto case 34;
				}
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				Expect(37, la); // "("
				currentState = 38;
				break;
			}
			case 38: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 39;
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (la.kind == 169) {
					currentState = 422;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						goto case 41;
					} else {
						Error(la);
						goto case 40;
					}
				}
			}
			case 40: {
				if (la == null) { currentState = 40; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 41: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 42;
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				if (set[21].Get(la.kind)) {
					stateStack.Push(40);
					goto case 43;
				} else {
					goto case 40;
				}
			}
			case 43: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 44;
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
				if (set[124].Get(la.kind)) {
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
				if (set[125].Get(la.kind)) {
					currentState = 53;
					break;
				} else {
					if (set[32].Get(la.kind)) {
						stateStack.Push(125);
						goto case 136;
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
					goto case 32;
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
					if (set[27].Get(la.kind)) {
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
				goto case 32;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				if (set[29].Get(la.kind)) {
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
					if (set[126].Get(la.kind)) {
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
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 132;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (la.kind == 169) {
					currentState = 134;
					break;
				} else {
					if (set[20].Get(la.kind)) {
						if (set[21].Get(la.kind)) {
							stateStack.Push(133);
							goto case 43;
						} else {
							goto case 133;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 133: {
				PopContext();
				goto case 40;
			}
			case 134: {
				stateStack.Push(135);
				goto case 32;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (la.kind == 22) {
					currentState = 134;
					break;
				} else {
					goto case 40;
				}
			}
			case 136: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 137;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (set[127].Get(la.kind)) {
					currentState = 138;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 417;
						break;
					} else {
						if (set[128].Get(la.kind)) {
							currentState = 138;
							break;
						} else {
							if (set[123].Get(la.kind)) {
								currentState = 138;
								break;
							} else {
								if (set[126].Get(la.kind)) {
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
											if (set[76].Get(la.kind)) {
												stateStack.Push(138);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 391;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(138);
													goto case 212;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(138);
														PushContext(Context.Query, la, t);
														goto case 152;
													} else {
														if (set[34].Get(la.kind)) {
															stateStack.Push(138);
															goto case 146;
														} else {
															if (la.kind == 135) {
																stateStack.Push(138);
																goto case 139;
															} else {
																Error(la);
																goto case 138;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 138: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				Expect(135, la); // "If"
				currentState = 140;
				break;
			}
			case 140: {
				if (la == null) { currentState = 140; break; }
				Expect(37, la); // "("
				currentState = 141;
				break;
			}
			case 141: {
				stateStack.Push(142);
				goto case 49;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				Expect(22, la); // ","
				currentState = 143;
				break;
			}
			case 143: {
				stateStack.Push(144);
				goto case 49;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (la.kind == 22) {
					currentState = 145;
					break;
				} else {
					goto case 40;
				}
			}
			case 145: {
				stateStack.Push(40);
				goto case 49;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				if (set[129].Get(la.kind)) {
					currentState = 151;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 147;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				Expect(37, la); // "("
				currentState = 148;
				break;
			}
			case 148: {
				stateStack.Push(149);
				goto case 49;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(22, la); // ","
				currentState = 150;
				break;
			}
			case 150: {
				stateStack.Push(40);
				goto case 32;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				Expect(37, la); // "("
				currentState = 145;
				break;
			}
			case 152: {
				if (la == null) { currentState = 152; break; }
				if (la.kind == 126) {
					stateStack.Push(153);
					goto case 211;
				} else {
					if (la.kind == 58) {
						stateStack.Push(153);
						goto case 210;
					} else {
						Error(la);
						goto case 153;
					}
				}
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (set[35].Get(la.kind)) {
					stateStack.Push(153);
					goto case 154;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				if (la.kind == 126) {
					currentState = 208;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 204;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 202;
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
										currentState = 198;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 196;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 194;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 168;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 155;
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
			case 155: {
				stateStack.Push(156);
				goto case 161;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				Expect(171, la); // "On"
				currentState = 157;
				break;
			}
			case 157: {
				stateStack.Push(158);
				goto case 49;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				Expect(116, la); // "Equals"
				currentState = 159;
				break;
			}
			case 159: {
				stateStack.Push(160);
				goto case 49;
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (la.kind == 22) {
					currentState = 157;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 161: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(162);
				goto case 167;
			}
			case 162: {
				PopContext();
				goto case 163;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				if (la.kind == 63) {
					currentState = 165;
					break;
				} else {
					goto case 164;
				}
			}
			case 164: {
				if (la == null) { currentState = 164; break; }
				Expect(138, la); // "In"
				currentState = 49;
				break;
			}
			case 165: {
				PushContext(Context.Type, la, t);
				stateStack.Push(166);
				goto case 32;
			}
			case 166: {
				PopContext();
				goto case 164;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (set[114].Get(la.kind)) {
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
					goto case 186;
				} else {
					if (set[37].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 171;
							break;
						} else {
							if (set[37].Get(la.kind)) {
								goto case 184;
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
					currentState = 181;
					break;
				} else {
					if (la.kind == 20) {
						goto case 180;
					} else {
						if (set[38].Get(la.kind)) {
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
				PushContext(Context.Type, la, t);
				stateStack.Push(182);
				goto case 32;
			}
			case 182: {
				PopContext();
				goto case 183;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				Expect(20, la); // "="
				currentState = 49;
				break;
			}
			case 184: {
				stateStack.Push(185);
				goto case 175;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 22) {
					currentState = 184;
					break;
				} else {
					goto case 170;
				}
			}
			case 186: {
				stateStack.Push(187);
				goto case 193;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 191;
						break;
					} else {
						if (la.kind == 146) {
							goto case 186;
						} else {
							Error(la);
							goto case 187;
						}
					}
				} else {
					goto case 188;
				}
			}
			case 188: {
				if (la == null) { currentState = 188; break; }
				Expect(143, la); // "Into"
				currentState = 189;
				break;
			}
			case 189: {
				stateStack.Push(190);
				goto case 175;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				if (la.kind == 22) {
					currentState = 189;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 191: {
				stateStack.Push(192);
				goto case 193;
			}
			case 192: {
				stateStack.Push(187);
				goto case 188;
			}
			case 193: {
				if (la == null) { currentState = 193; break; }
				Expect(146, la); // "Join"
				currentState = 155;
				break;
			}
			case 194: {
				stateStack.Push(195);
				goto case 175;
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
				nextTokenIsPotentialStartOfExpression = true;
				goto case 197;
			}
			case 197: {
				if (la == null) { currentState = 197; break; }
				if (la.kind == 231) {
					currentState = 49;
					break;
				} else {
					goto case 49;
				}
			}
			case 198: {
				if (la == null) { currentState = 198; break; }
				Expect(70, la); // "By"
				currentState = 199;
				break;
			}
			case 199: {
				stateStack.Push(200);
				goto case 49;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				if (la.kind == 64) {
					currentState = 201;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 201;
						break;
					} else {
						Error(la);
						goto case 201;
					}
				}
			}
			case 201: {
				if (la == null) { currentState = 201; break; }
				if (la.kind == 22) {
					currentState = 199;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 202: {
				stateStack.Push(203);
				goto case 175;
			}
			case 203: {
				if (la == null) { currentState = 203; break; }
				if (la.kind == 22) {
					currentState = 202;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 204: {
				stateStack.Push(205);
				goto case 161;
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				if (set[35].Get(la.kind)) {
					stateStack.Push(205);
					goto case 154;
				} else {
					Expect(143, la); // "Into"
					currentState = 206;
					break;
				}
			}
			case 206: {
				stateStack.Push(207);
				goto case 175;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				if (la.kind == 22) {
					currentState = 206;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 208: {
				stateStack.Push(209);
				goto case 161;
			}
			case 209: {
				if (la == null) { currentState = 209; break; }
				if (la.kind == 22) {
					currentState = 208;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 210: {
				if (la == null) { currentState = 210; break; }
				Expect(58, la); // "Aggregate"
				currentState = 204;
				break;
			}
			case 211: {
				if (la == null) { currentState = 211; break; }
				Expect(126, la); // "From"
				currentState = 208;
				break;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (la.kind == 210) {
					currentState = 383;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 213;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 213: {
				if (la == null) { currentState = 213; break; }
				Expect(37, la); // "("
				currentState = 214;
				break;
			}
			case 214: {
				SetIdentifierExpected(la);
				goto case 215;
			}
			case 215: {
				if (la == null) { currentState = 215; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(216);
					goto case 365;
				} else {
					goto case 216;
				}
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				Expect(38, la); // ")"
				currentState = 217;
				break;
			}
			case 217: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 218;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 363;
							break;
						} else {
							goto case 219;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 219: {
				stateStack.Push(220);
				goto case 222;
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				Expect(113, la); // "End"
				currentState = 221;
				break;
			}
			case 221: {
				if (la == null) { currentState = 221; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 222: {
				PushContext(Context.Body, la, t);
				goto case 223;
			}
			case 223: {
				stateStack.Push(224);
				goto case 18;
			}
			case 224: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 225;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (set[130].Get(la.kind)) {
					if (set[64].Get(la.kind)) {
						if (set[46].Get(la.kind)) {
							stateStack.Push(223);
							goto case 230;
						} else {
							goto case 223;
						}
					} else {
						if (la.kind == 113) {
							currentState = 228;
							break;
						} else {
							goto case 227;
						}
					}
				} else {
					goto case 226;
				}
			}
			case 226: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 227: {
				Error(la);
				goto case 224;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 223;
				} else {
					if (set[45].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 227;
					}
				}
			}
			case 229: {
				if (la == null) { currentState = 229; break; }
				currentState = 224;
				break;
			}
			case 230: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 231;
			}
			case 231: {
				if (la == null) { currentState = 231; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 347;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 343;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 341;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 339;
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
													currentState = 269;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 265;
																break;
															} else {
																goto case 265;
															}
														} else {
															if (la.kind == 194) {
																currentState = 263;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 261;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 248;
																break;
															} else {
																if (set[131].Get(la.kind)) {
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
																						currentState = 240;
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
																		currentState = 238;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 236;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 232;
																				break;
																			} else {
																				if (set[132].Get(la.kind)) {
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
			case 232: {
				stateStack.Push(233);
				goto case 49;
			}
			case 233: {
				stateStack.Push(234);
				goto case 222;
			}
			case 234: {
				if (la == null) { currentState = 234; break; }
				Expect(113, la); // "End"
				currentState = 235;
				break;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 236: {
				stateStack.Push(237);
				goto case 49;
			}
			case 237: {
				if (la == null) { currentState = 237; break; }
				if (la.kind == 22) {
					currentState = 236;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 238: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 239;
			}
			case 239: {
				if (la == null) { currentState = 239; break; }
				if (la.kind == 184) {
					currentState = 49;
					break;
				} else {
					goto case 49;
				}
			}
			case 240: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 241;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(242);
					goto case 49;
				} else {
					goto case 242;
				}
			}
			case 242: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
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
				goto case 222;
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
				goto case 222;
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
					currentState = 259;
					break;
				} else {
					goto case 255;
				}
			}
			case 259: {
				PushContext(Context.Type, la, t);
				stateStack.Push(260);
				goto case 32;
			}
			case 260: {
				PopContext();
				goto case 255;
			}
			case 261: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 262;
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 263: {
				if (la == null) { currentState = 263; break; }
				if (la.kind == 163) {
					goto case 93;
				} else {
					goto case 264;
				}
			}
			case 264: {
				if (la == null) { currentState = 264; break; }
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
			case 265: {
				if (la == null) { currentState = 265; break; }
				Expect(118, la); // "Error"
				currentState = 266;
				break;
			}
			case 266: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 267;
			}
			case 267: {
				if (la == null) { currentState = 267; break; }
				if (set[22].Get(la.kind)) {
					goto case 49;
				} else {
					if (la.kind == 132) {
						currentState = 264;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 268;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 268: {
				if (la == null) { currentState = 268; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 269: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 270;
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (set[32].Get(la.kind)) {
					stateStack.Push(286);
					goto case 280;
				} else {
					if (la.kind == 110) {
						currentState = 271;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 271: {
				stateStack.Push(272);
				goto case 280;
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				Expect(138, la); // "In"
				currentState = 273;
				break;
			}
			case 273: {
				stateStack.Push(274);
				goto case 49;
			}
			case 274: {
				stateStack.Push(275);
				goto case 222;
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(163, la); // "Next"
				currentState = 276;
				break;
			}
			case 276: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 277;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (set[22].Get(la.kind)) {
					goto case 278;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 278: {
				stateStack.Push(279);
				goto case 49;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				if (la.kind == 22) {
					currentState = 278;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 280: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(281);
				goto case 136;
			}
			case 281: {
				PopContext();
				goto case 282;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				if (la.kind == 33) {
					currentState = 283;
					break;
				} else {
					goto case 283;
				}
			}
			case 283: {
				if (la == null) { currentState = 283; break; }
				if (set[29].Get(la.kind)) {
					stateStack.Push(283);
					goto case 126;
				} else {
					if (la.kind == 63) {
						currentState = 284;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 284: {
				PushContext(Context.Type, la, t);
				stateStack.Push(285);
				goto case 32;
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
				goto case 222;
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
						goto case 222;
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
				goto case 222;
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
				goto case 222;
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
					if (set[62].Get(la.kind)) {
						goto case 315;
					} else {
						Error(la);
						goto case 314;
					}
				}
			}
			case 314: {
				stateStack.Push(310);
				goto case 222;
			}
			case 315: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 316;
			}
			case 316: {
				if (la == null) { currentState = 316; break; }
				if (set[133].Get(la.kind)) {
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
				goto case 222;
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
				if (set[46].Get(la.kind)) {
					goto case 332;
				} else {
					goto case 323;
				}
			}
			case 332: {
				stateStack.Push(333);
				goto case 230;
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				if (la.kind == 21) {
					currentState = 337;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 334;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 334: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 335;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (set[46].Get(la.kind)) {
					stateStack.Push(336);
					goto case 230;
				} else {
					goto case 336;
				}
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 21) {
					currentState = 334;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 337: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 338;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (set[46].Get(la.kind)) {
					goto case 332;
				} else {
					goto case 333;
				}
			}
			case 339: {
				stateStack.Push(340);
				goto case 74;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 37) {
					currentState = 41;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 341: {
				stateStack.Push(342);
				goto case 49;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				Expect(22, la); // ","
				currentState = 49;
				break;
			}
			case 343: {
				stateStack.Push(344);
				goto case 49;
			}
			case 344: {
				stateStack.Push(345);
				goto case 222;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				Expect(113, la); // "End"
				currentState = 346;
				break;
			}
			case 346: {
				if (la == null) { currentState = 346; break; }
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
			case 347: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(348);
				goto case 167;
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
					currentState = 362;
					break;
				} else {
					goto case 351;
				}
			}
			case 351: {
				if (la == null) { currentState = 351; break; }
				if (la.kind == 22) {
					currentState = 356;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 353;
						break;
					} else {
						goto case 352;
					}
				}
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				if (la.kind == 20) {
					goto case 180;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 353: {
				PushContext(Context.Type, la, t);
				goto case 354;
			}
			case 354: {
				if (la == null) { currentState = 354; break; }
				if (la.kind == 162) {
					stateStack.Push(355);
					goto case 60;
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(355);
						goto case 32;
					} else {
						Error(la);
						goto case 355;
					}
				}
			}
			case 355: {
				PopContext();
				goto case 352;
			}
			case 356: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(357);
				goto case 167;
			}
			case 357: {
				PopContext();
				goto case 358;
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 33) {
					currentState = 359;
					break;
				} else {
					goto case 359;
				}
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (la.kind == 37) {
					currentState = 360;
					break;
				} else {
					goto case 351;
				}
			}
			case 360: {
				if (la == null) { currentState = 360; break; }
				if (la.kind == 22) {
					currentState = 360;
					break;
				} else {
					goto case 361;
				}
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				Expect(38, la); // ")"
				currentState = 351;
				break;
			}
			case 362: {
				if (la == null) { currentState = 362; break; }
				if (la.kind == 22) {
					currentState = 362;
					break;
				} else {
					goto case 361;
				}
			}
			case 363: {
				PushContext(Context.Type, la, t);
				stateStack.Push(364);
				goto case 32;
			}
			case 364: {
				PopContext();
				goto case 219;
			}
			case 365: {
				stateStack.Push(366);
				PushContext(Context.Parameter, la, t);
				goto case 367;
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (la.kind == 22) {
					currentState = 365;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 367: {
				SetIdentifierExpected(la);
				goto case 368;
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				if (la.kind == 40) {
					stateStack.Push(367);
					goto case 378;
				} else {
					goto case 369;
				}
			}
			case 369: {
				SetIdentifierExpected(la);
				goto case 370;
			}
			case 370: {
				if (la == null) { currentState = 370; break; }
				if (set[134].Get(la.kind)) {
					currentState = 369;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(371);
					goto case 167;
				}
			}
			case 371: {
				PopContext();
				goto case 372;
			}
			case 372: {
				if (la == null) { currentState = 372; break; }
				if (la.kind == 63) {
					currentState = 376;
					break;
				} else {
					goto case 373;
				}
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 20) {
					currentState = 375;
					break;
				} else {
					goto case 374;
				}
			}
			case 374: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 375: {
				stateStack.Push(374);
				goto case 49;
			}
			case 376: {
				PushContext(Context.Type, la, t);
				stateStack.Push(377);
				goto case 32;
			}
			case 377: {
				PopContext();
				goto case 373;
			}
			case 378: {
				if (la == null) { currentState = 378; break; }
				Expect(40, la); // "<"
				currentState = 379;
				break;
			}
			case 379: {
				PushContext(Context.Attribute, la, t);
				goto case 380;
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (set[135].Get(la.kind)) {
					currentState = 380;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 381;
					break;
				}
			}
			case 381: {
				PopContext();
				goto case 382;
			}
			case 382: {
				if (la == null) { currentState = 382; break; }
				if (la.kind == 1) {
					goto case 20;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 383: {
				if (la == null) { currentState = 383; break; }
				Expect(37, la); // "("
				currentState = 384;
				break;
			}
			case 384: {
				SetIdentifierExpected(la);
				goto case 385;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(386);
					goto case 365;
				} else {
					goto case 386;
				}
			}
			case 386: {
				if (la == null) { currentState = 386; break; }
				Expect(38, la); // ")"
				currentState = 387;
				break;
			}
			case 387: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 388;
			}
			case 388: {
				if (la == null) { currentState = 388; break; }
				if (set[46].Get(la.kind)) {
					goto case 230;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(389);
						goto case 222;
					} else {
						goto case 6;
					}
				}
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				Expect(113, la); // "End"
				currentState = 390;
				break;
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 391: {
				if (la == null) { currentState = 391; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 404;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(393);
						goto case 395;
					} else {
						Error(la);
						goto case 392;
					}
				}
			}
			case 392: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				if (la.kind == 17) {
					currentState = 394;
					break;
				} else {
					goto case 392;
				}
			}
			case 394: {
				if (la == null) { currentState = 394; break; }
				if (la.kind == 16) {
					currentState = 393;
					break;
				} else {
					goto case 393;
				}
			}
			case 395: {
				PushContext(Context.Xml, la, t);
				goto case 396;
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 397;
				break;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				if (set[136].Get(la.kind)) {
					if (set[137].Get(la.kind)) {
						currentState = 397;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(397);
							goto case 401;
						} else {
							Error(la);
							goto case 397;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 398;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 399;
							break;
						} else {
							Error(la);
							goto case 398;
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
				if (set[138].Get(la.kind)) {
					if (set[139].Get(la.kind)) {
						currentState = 399;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(399);
							goto case 401;
						} else {
							if (la.kind == 10) {
								stateStack.Push(399);
								goto case 395;
							} else {
								Error(la);
								goto case 399;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 400;
					break;
				}
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (set[140].Get(la.kind)) {
					if (set[141].Get(la.kind)) {
						currentState = 400;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(400);
							goto case 401;
						} else {
							Error(la);
							goto case 400;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 398;
					break;
				}
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 402;
				break;
			}
			case 402: {
				stateStack.Push(403);
				goto case 49;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				if (la.kind == 16) {
					currentState = 405;
					break;
				} else {
					goto case 405;
				}
			}
			case 405: {
				if (la == null) { currentState = 405; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 404;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(406);
						goto case 395;
					} else {
						goto case 392;
					}
				}
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (la.kind == 17) {
					currentState = 407;
					break;
				} else {
					goto case 392;
				}
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (la.kind == 16) {
					currentState = 406;
					break;
				} else {
					goto case 406;
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
				currentState = 138;
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
				goto case 32;
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
					currentState = 138;
					break;
				} else {
					goto case 138;
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
					goto case 32;
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
					goto case 40;
				}
			}
			case 424: {
				SetIdentifierExpected(la);
				goto case 425;
			}
			case 425: {
				if (la == null) { currentState = 425; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 427;
						break;
					} else {
						if (set[72].Get(la.kind)) {
							stateStack.Push(426);
							goto case 365;
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
				goto case 445;
			}
			case 431: {
				PopContext();
				goto case 432;
			}
			case 432: {
				if (la == null) { currentState = 432; break; }
				if (la.kind == 63) {
					currentState = 446;
					break;
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
				goto case 445;
			}
			case 437: {
				PopContext();
				goto case 438;
			}
			case 438: {
				if (la == null) { currentState = 438; break; }
				if (la.kind == 63) {
					currentState = 439;
					break;
				} else {
					goto case 433;
				}
			}
			case 439: {
				PushContext(Context.Type, la, t);
				stateStack.Push(440);
				goto case 441;
			}
			case 440: {
				PopContext();
				goto case 433;
			}
			case 441: {
				if (la == null) { currentState = 441; break; }
				if (set[87].Get(la.kind)) {
					goto case 444;
				} else {
					if (la.kind == 35) {
						currentState = 442;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 442: {
				stateStack.Push(443);
				goto case 444;
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (la.kind == 22) {
					currentState = 442;
					break;
				} else {
					goto case 59;
				}
			}
			case 444: {
				if (la == null) { currentState = 444; break; }
				if (set[15].Get(la.kind)) {
					currentState = 33;
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
			case 445: {
				if (la == null) { currentState = 445; break; }
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
			case 446: {
				PushContext(Context.Type, la, t);
				stateStack.Push(447);
				goto case 441;
			}
			case 447: {
				PopContext();
				goto case 433;
			}
			case 448: {
				stateStack.Push(449);
				goto case 167;
			}
			case 449: {
				if (la == null) { currentState = 449; break; }
				if (la.kind == 37) {
					currentState = 453;
					break;
				} else {
					goto case 450;
				}
			}
			case 450: {
				if (la == null) { currentState = 450; break; }
				if (la.kind == 63) {
					currentState = 451;
					break;
				} else {
					goto case 18;
				}
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (la.kind == 40) {
					stateStack.Push(451);
					goto case 378;
				} else {
					goto case 452;
				}
			}
			case 452: {
				stateStack.Push(18);
				goto case 32;
			}
			case 453: {
				SetIdentifierExpected(la);
				goto case 454;
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(455);
					goto case 365;
				} else {
					goto case 455;
				}
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				Expect(38, la); // ")"
				currentState = 450;
				break;
			}
			case 456: {
				stateStack.Push(457);
				goto case 167;
			}
			case 457: {
				if (la == null) { currentState = 457; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 452;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 459;
							break;
						} else {
							goto case 458;
						}
					}
				} else {
					goto case 18;
				}
			}
			case 458: {
				Error(la);
				goto case 18;
			}
			case 459: {
				SetIdentifierExpected(la);
				goto case 460;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(461);
					goto case 365;
				} else {
					goto case 461;
				}
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				Expect(38, la); // ")"
				currentState = 18;
				break;
			}
			case 462: {
				if (la == null) { currentState = 462; break; }
				Expect(142, la); // "Interface"
				currentState = 9;
				break;
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				Expect(115, la); // "Enum"
				currentState = 464;
				break;
			}
			case 464: {
				stateStack.Push(465);
				goto case 167;
			}
			case 465: {
				if (la == null) { currentState = 465; break; }
				if (la.kind == 63) {
					currentState = 472;
					break;
				} else {
					goto case 466;
				}
			}
			case 466: {
				stateStack.Push(467);
				goto case 18;
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				if (set[90].Get(la.kind)) {
					goto case 469;
				} else {
					Expect(113, la); // "End"
					currentState = 468;
					break;
				}
			}
			case 468: {
				if (la == null) { currentState = 468; break; }
				Expect(115, la); // "Enum"
				currentState = 18;
				break;
			}
			case 469: {
				if (la == null) { currentState = 469; break; }
				if (la.kind == 40) {
					stateStack.Push(469);
					goto case 378;
				} else {
					stateStack.Push(470);
					goto case 167;
				}
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				if (la.kind == 20) {
					currentState = 471;
					break;
				} else {
					goto case 466;
				}
			}
			case 471: {
				stateStack.Push(466);
				goto case 49;
			}
			case 472: {
				stateStack.Push(466);
				goto case 32;
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				Expect(103, la); // "Delegate"
				currentState = 474;
				break;
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				if (la.kind == 210) {
					currentState = 475;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 475;
						break;
					} else {
						Error(la);
						goto case 475;
					}
				}
			}
			case 475: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 476;
			}
			case 476: {
				if (la == null) { currentState = 476; break; }
				currentState = 477;
				break;
			}
			case 477: {
				PopContext();
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				if (la.kind == 37) {
					currentState = 482;
					break;
				} else {
					goto case 479;
				}
			}
			case 479: {
				if (la == null) { currentState = 479; break; }
				if (la.kind == 63) {
					currentState = 480;
					break;
				} else {
					goto case 18;
				}
			}
			case 480: {
				PushContext(Context.Type, la, t);
				stateStack.Push(481);
				goto case 32;
			}
			case 481: {
				PopContext();
				goto case 18;
			}
			case 482: {
				SetIdentifierExpected(la);
				goto case 483;
			}
			case 483: {
				if (la == null) { currentState = 483; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(484);
					goto case 365;
				} else {
					goto case 484;
				}
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				Expect(38, la); // ")"
				currentState = 479;
				break;
			}
			case 485: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 486;
			}
			case 486: {
				if (la == null) { currentState = 486; break; }
				if (la.kind == 155) {
					currentState = 487;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 487;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 487;
							break;
						} else {
							Error(la);
							goto case 487;
						}
					}
				}
			}
			case 487: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(488);
				goto case 167;
			}
			case 488: {
				PopContext();
				goto case 489;
			}
			case 489: {
				if (la == null) { currentState = 489; break; }
				if (la.kind == 37) {
					currentState = 638;
					break;
				} else {
					goto case 490;
				}
			}
			case 490: {
				stateStack.Push(491);
				goto case 18;
			}
			case 491: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 492;
			}
			case 492: {
				if (la == null) { currentState = 492; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 635;
				} else {
					goto case 493;
				}
			}
			case 493: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 494;
			}
			case 494: {
				if (la == null) { currentState = 494; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 629;
				} else {
					goto case 495;
				}
			}
			case 495: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 496;
			}
			case 496: {
				if (la == null) { currentState = 496; break; }
				if (set[95].Get(la.kind)) {
					goto case 501;
				} else {
					isMissingModifier = false;
					goto case 497;
				}
			}
			case 497: {
				if (la == null) { currentState = 497; break; }
				Expect(113, la); // "End"
				currentState = 498;
				break;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				if (la.kind == 155) {
					currentState = 499;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 499;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 499;
							break;
						} else {
							Error(la);
							goto case 499;
						}
					}
				}
			}
			case 499: {
				stateStack.Push(500);
				goto case 18;
			}
			case 500: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 501: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 502;
			}
			case 502: {
				if (la == null) { currentState = 502; break; }
				if (la.kind == 40) {
					stateStack.Push(501);
					goto case 378;
				} else {
					isMissingModifier = true;
					goto case 503;
				}
			}
			case 503: {
				SetIdentifierExpected(la);
				goto case 504;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				if (set[122].Get(la.kind)) {
					currentState = 628;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 505;
				}
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(495);
					goto case 485;
				} else {
					if (la.kind == 103) {
						stateStack.Push(495);
						goto case 473;
					} else {
						if (la.kind == 115) {
							stateStack.Push(495);
							goto case 463;
						} else {
							if (la.kind == 142) {
								stateStack.Push(495);
								goto case 462;
							} else {
								if (set[98].Get(la.kind)) {
									stateStack.Push(495);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 506;
								} else {
									Error(la);
									goto case 495;
								}
							}
						}
					}
				}
			}
			case 506: {
				if (la == null) { currentState = 506; break; }
				if (set[113].Get(la.kind)) {
					stateStack.Push(507);
					goto case 617;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(507);
						goto case 603;
					} else {
						if (la.kind == 101) {
							stateStack.Push(507);
							goto case 587;
						} else {
							if (la.kind == 119) {
								stateStack.Push(507);
								goto case 575;
							} else {
								if (la.kind == 98) {
									stateStack.Push(507);
									goto case 563;
								} else {
									if (la.kind == 186) {
										stateStack.Push(507);
										goto case 521;
									} else {
										if (la.kind == 172) {
											stateStack.Push(507);
											goto case 508;
										} else {
											Error(la);
											goto case 507;
										}
									}
								}
							}
						}
					}
				}
			}
			case 507: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				Expect(172, la); // "Operator"
				currentState = 509;
				break;
			}
			case 509: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 510;
			}
			case 510: {
				if (la == null) { currentState = 510; break; }
				currentState = 511;
				break;
			}
			case 511: {
				PopContext();
				goto case 512;
			}
			case 512: {
				if (la == null) { currentState = 512; break; }
				Expect(37, la); // "("
				currentState = 513;
				break;
			}
			case 513: {
				stateStack.Push(514);
				goto case 365;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				Expect(38, la); // ")"
				currentState = 515;
				break;
			}
			case 515: {
				if (la == null) { currentState = 515; break; }
				if (la.kind == 63) {
					currentState = 519;
					break;
				} else {
					goto case 516;
				}
			}
			case 516: {
				stateStack.Push(517);
				goto case 222;
			}
			case 517: {
				if (la == null) { currentState = 517; break; }
				Expect(113, la); // "End"
				currentState = 518;
				break;
			}
			case 518: {
				if (la == null) { currentState = 518; break; }
				Expect(172, la); // "Operator"
				currentState = 18;
				break;
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				if (la.kind == 40) {
					stateStack.Push(519);
					goto case 378;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(520);
					goto case 32;
				}
			}
			case 520: {
				PopContext();
				goto case 516;
			}
			case 521: {
				if (la == null) { currentState = 521; break; }
				Expect(186, la); // "Property"
				currentState = 522;
				break;
			}
			case 522: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(523);
				goto case 167;
			}
			case 523: {
				PopContext();
				goto case 524;
			}
			case 524: {
				if (la == null) { currentState = 524; break; }
				if (la.kind == 37) {
					currentState = 560;
					break;
				} else {
					goto case 525;
				}
			}
			case 525: {
				if (la == null) { currentState = 525; break; }
				if (la.kind == 63) {
					currentState = 558;
					break;
				} else {
					goto case 526;
				}
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (la.kind == 136) {
					currentState = 556;
					break;
				} else {
					goto case 527;
				}
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 20) {
					currentState = 555;
					break;
				} else {
					goto case 528;
				}
			}
			case 528: {
				stateStack.Push(529);
				goto case 18;
			}
			case 529: {
				PopContext();
				goto case 530;
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				if (la.kind == 40) {
					stateStack.Push(530);
					goto case 378;
				} else {
					goto case 531;
				}
			}
			case 531: {
				if (la == null) { currentState = 531; break; }
				if (set[143].Get(la.kind)) {
					currentState = 554;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 532;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				if (la.kind == 128) {
					currentState = 533;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 533;
						break;
					} else {
						Error(la);
						goto case 533;
					}
				}
			}
			case 533: {
				if (la == null) { currentState = 533; break; }
				if (la.kind == 37) {
					currentState = 551;
					break;
				} else {
					goto case 534;
				}
			}
			case 534: {
				stateStack.Push(535);
				goto case 222;
			}
			case 535: {
				if (la == null) { currentState = 535; break; }
				Expect(113, la); // "End"
				currentState = 536;
				break;
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				if (la.kind == 128) {
					currentState = 537;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 537;
						break;
					} else {
						Error(la);
						goto case 537;
					}
				}
			}
			case 537: {
				stateStack.Push(538);
				goto case 18;
			}
			case 538: {
				if (la == null) { currentState = 538; break; }
				if (set[104].Get(la.kind)) {
					goto case 541;
				} else {
					goto case 539;
				}
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				Expect(113, la); // "End"
				currentState = 540;
				break;
			}
			case 540: {
				if (la == null) { currentState = 540; break; }
				Expect(186, la); // "Property"
				currentState = 18;
				break;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 40) {
					stateStack.Push(541);
					goto case 378;
				} else {
					goto case 542;
				}
			}
			case 542: {
				if (la == null) { currentState = 542; break; }
				if (set[143].Get(la.kind)) {
					currentState = 542;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 543;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 543;
							break;
						} else {
							Error(la);
							goto case 543;
						}
					}
				}
			}
			case 543: {
				if (la == null) { currentState = 543; break; }
				if (la.kind == 37) {
					currentState = 548;
					break;
				} else {
					goto case 544;
				}
			}
			case 544: {
				stateStack.Push(545);
				goto case 222;
			}
			case 545: {
				if (la == null) { currentState = 545; break; }
				Expect(113, la); // "End"
				currentState = 546;
				break;
			}
			case 546: {
				if (la == null) { currentState = 546; break; }
				if (la.kind == 128) {
					currentState = 547;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 547;
						break;
					} else {
						Error(la);
						goto case 547;
					}
				}
			}
			case 547: {
				stateStack.Push(539);
				goto case 18;
			}
			case 548: {
				SetIdentifierExpected(la);
				goto case 549;
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(550);
					goto case 365;
				} else {
					goto case 550;
				}
			}
			case 550: {
				if (la == null) { currentState = 550; break; }
				Expect(38, la); // ")"
				currentState = 544;
				break;
			}
			case 551: {
				SetIdentifierExpected(la);
				goto case 552;
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(553);
					goto case 365;
				} else {
					goto case 553;
				}
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				Expect(38, la); // ")"
				currentState = 534;
				break;
			}
			case 554: {
				SetIdentifierExpected(la);
				goto case 531;
			}
			case 555: {
				stateStack.Push(528);
				goto case 49;
			}
			case 556: {
				stateStack.Push(557);
				goto case 32;
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				if (la.kind == 22) {
					currentState = 556;
					break;
				} else {
					goto case 527;
				}
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 40) {
					stateStack.Push(558);
					goto case 378;
				} else {
					if (la.kind == 162) {
						stateStack.Push(526);
						goto case 60;
					} else {
						if (set[15].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(559);
							goto case 32;
						} else {
							Error(la);
							goto case 526;
						}
					}
				}
			}
			case 559: {
				PopContext();
				goto case 526;
			}
			case 560: {
				SetIdentifierExpected(la);
				goto case 561;
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(562);
					goto case 365;
				} else {
					goto case 562;
				}
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				Expect(38, la); // ")"
				currentState = 525;
				break;
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				Expect(98, la); // "Custom"
				currentState = 564;
				break;
			}
			case 564: {
				stateStack.Push(565);
				goto case 575;
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				if (set[109].Get(la.kind)) {
					goto case 567;
				} else {
					Expect(113, la); // "End"
					currentState = 566;
					break;
				}
			}
			case 566: {
				if (la == null) { currentState = 566; break; }
				Expect(119, la); // "Event"
				currentState = 18;
				break;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				if (la.kind == 40) {
					stateStack.Push(567);
					goto case 378;
				} else {
					if (la.kind == 56) {
						currentState = 568;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 568;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 568;
								break;
							} else {
								Error(la);
								goto case 568;
							}
						}
					}
				}
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				Expect(37, la); // "("
				currentState = 569;
				break;
			}
			case 569: {
				stateStack.Push(570);
				goto case 365;
			}
			case 570: {
				if (la == null) { currentState = 570; break; }
				Expect(38, la); // ")"
				currentState = 571;
				break;
			}
			case 571: {
				stateStack.Push(572);
				goto case 222;
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				Expect(113, la); // "End"
				currentState = 573;
				break;
			}
			case 573: {
				if (la == null) { currentState = 573; break; }
				if (la.kind == 56) {
					currentState = 574;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 574;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 574;
							break;
						} else {
							Error(la);
							goto case 574;
						}
					}
				}
			}
			case 574: {
				stateStack.Push(565);
				goto case 18;
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				Expect(119, la); // "Event"
				currentState = 576;
				break;
			}
			case 576: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(577);
				goto case 167;
			}
			case 577: {
				PopContext();
				goto case 578;
			}
			case 578: {
				if (la == null) { currentState = 578; break; }
				if (la.kind == 63) {
					currentState = 585;
					break;
				} else {
					if (set[144].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 582;
							break;
						} else {
							goto case 579;
						}
					} else {
						Error(la);
						goto case 579;
					}
				}
			}
			case 579: {
				if (la == null) { currentState = 579; break; }
				if (la.kind == 136) {
					currentState = 580;
					break;
				} else {
					goto case 18;
				}
			}
			case 580: {
				stateStack.Push(581);
				goto case 32;
			}
			case 581: {
				if (la == null) { currentState = 581; break; }
				if (la.kind == 22) {
					currentState = 580;
					break;
				} else {
					goto case 18;
				}
			}
			case 582: {
				SetIdentifierExpected(la);
				goto case 583;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(584);
					goto case 365;
				} else {
					goto case 584;
				}
			}
			case 584: {
				if (la == null) { currentState = 584; break; }
				Expect(38, la); // ")"
				currentState = 579;
				break;
			}
			case 585: {
				PushContext(Context.Type, la, t);
				stateStack.Push(586);
				goto case 32;
			}
			case 586: {
				PopContext();
				goto case 579;
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				Expect(101, la); // "Declare"
				currentState = 588;
				break;
			}
			case 588: {
				if (la == null) { currentState = 588; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 589;
					break;
				} else {
					goto case 589;
				}
			}
			case 589: {
				if (la == null) { currentState = 589; break; }
				if (la.kind == 210) {
					currentState = 590;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 590;
						break;
					} else {
						Error(la);
						goto case 590;
					}
				}
			}
			case 590: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(591);
				goto case 167;
			}
			case 591: {
				PopContext();
				goto case 592;
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				Expect(149, la); // "Lib"
				currentState = 593;
				break;
			}
			case 593: {
				if (la == null) { currentState = 593; break; }
				Expect(3, la); // LiteralString
				currentState = 594;
				break;
			}
			case 594: {
				if (la == null) { currentState = 594; break; }
				if (la.kind == 59) {
					currentState = 602;
					break;
				} else {
					goto case 595;
				}
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				if (la.kind == 37) {
					currentState = 599;
					break;
				} else {
					goto case 596;
				}
			}
			case 596: {
				if (la == null) { currentState = 596; break; }
				if (la.kind == 63) {
					currentState = 597;
					break;
				} else {
					goto case 18;
				}
			}
			case 597: {
				PushContext(Context.Type, la, t);
				stateStack.Push(598);
				goto case 32;
			}
			case 598: {
				PopContext();
				goto case 18;
			}
			case 599: {
				SetIdentifierExpected(la);
				goto case 600;
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				if (set[72].Get(la.kind)) {
					stateStack.Push(601);
					goto case 365;
				} else {
					goto case 601;
				}
			}
			case 601: {
				if (la == null) { currentState = 601; break; }
				Expect(38, la); // ")"
				currentState = 596;
				break;
			}
			case 602: {
				if (la == null) { currentState = 602; break; }
				Expect(3, la); // LiteralString
				currentState = 595;
				break;
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				if (la.kind == 210) {
					currentState = 604;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 604;
						break;
					} else {
						Error(la);
						goto case 604;
					}
				}
			}
			case 604: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 605;
			}
			case 605: {
				if (la == null) { currentState = 605; break; }
				currentState = 606;
				break;
			}
			case 606: {
				PopContext();
				goto case 607;
			}
			case 607: {
				if (la == null) { currentState = 607; break; }
				if (la.kind == 37) {
					currentState = 613;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 611;
						break;
					} else {
						goto case 608;
					}
				}
			}
			case 608: {
				stateStack.Push(609);
				goto case 222;
			}
			case 609: {
				if (la == null) { currentState = 609; break; }
				Expect(113, la); // "End"
				currentState = 610;
				break;
			}
			case 610: {
				if (la == null) { currentState = 610; break; }
				if (la.kind == 210) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 18;
						break;
					} else {
						goto case 458;
					}
				}
			}
			case 611: {
				PushContext(Context.Type, la, t);
				stateStack.Push(612);
				goto case 32;
			}
			case 612: {
				PopContext();
				goto case 608;
			}
			case 613: {
				SetIdentifierExpected(la);
				goto case 614;
			}
			case 614: {
				if (la == null) { currentState = 614; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 616;
						break;
					} else {
						if (set[72].Get(la.kind)) {
							stateStack.Push(615);
							goto case 365;
						} else {
							Error(la);
							goto case 615;
						}
					}
				} else {
					goto case 615;
				}
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				Expect(38, la); // ")"
				currentState = 607;
				break;
			}
			case 616: {
				stateStack.Push(615);
				goto case 428;
			}
			case 617: {
				stateStack.Push(618);
				SetIdentifierExpected(la);
				goto case 619;
			}
			case 618: {
				if (la == null) { currentState = 618; break; }
				if (la.kind == 22) {
					currentState = 617;
					break;
				} else {
					goto case 18;
				}
			}
			case 619: {
				if (la == null) { currentState = 619; break; }
				if (la.kind == 88) {
					currentState = 620;
					break;
				} else {
					goto case 620;
				}
			}
			case 620: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(621);
				goto case 627;
			}
			case 621: {
				PopContext();
				goto case 622;
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				if (la.kind == 63) {
					currentState = 624;
					break;
				} else {
					goto case 623;
				}
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				if (la.kind == 20) {
					goto case 180;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 624: {
				PushContext(Context.Type, la, t);
				goto case 625;
			}
			case 625: {
				if (la == null) { currentState = 625; break; }
				if (la.kind == 162) {
					stateStack.Push(626);
					goto case 60;
				} else {
					if (set[15].Get(la.kind)) {
						stateStack.Push(626);
						goto case 32;
					} else {
						Error(la);
						goto case 626;
					}
				}
			}
			case 626: {
				PopContext();
				goto case 623;
			}
			case 627: {
				if (la == null) { currentState = 627; break; }
				if (set[128].Get(la.kind)) {
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
			case 628: {
				isMissingModifier = false;
				goto case 503;
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				Expect(136, la); // "Implements"
				currentState = 630;
				break;
			}
			case 630: {
				PushContext(Context.Type, la, t);
				stateStack.Push(631);
				goto case 32;
			}
			case 631: {
				PopContext();
				goto case 632;
			}
			case 632: {
				if (la == null) { currentState = 632; break; }
				if (la.kind == 22) {
					currentState = 633;
					break;
				} else {
					stateStack.Push(495);
					goto case 18;
				}
			}
			case 633: {
				PushContext(Context.Type, la, t);
				stateStack.Push(634);
				goto case 32;
			}
			case 634: {
				PopContext();
				goto case 632;
			}
			case 635: {
				if (la == null) { currentState = 635; break; }
				Expect(140, la); // "Inherits"
				currentState = 636;
				break;
			}
			case 636: {
				PushContext(Context.Type, la, t);
				stateStack.Push(637);
				goto case 32;
			}
			case 637: {
				PopContext();
				stateStack.Push(493);
				goto case 18;
			}
			case 638: {
				if (la == null) { currentState = 638; break; }
				Expect(169, la); // "Of"
				currentState = 639;
				break;
			}
			case 639: {
				stateStack.Push(640);
				goto case 428;
			}
			case 640: {
				if (la == null) { currentState = 640; break; }
				Expect(38, la); // ")"
				currentState = 490;
				break;
			}
			case 641: {
				isMissingModifier = false;
				goto case 23;
			}
			case 642: {
				if (la == null) { currentState = 642; break; }
				Expect(140, la); // "Inherits"
				currentState = 643;
				break;
			}
			case 643: {
				stateStack.Push(644);
				goto case 32;
			}
			case 644: {
				PopContext();
				goto case 645;
			}
			case 645: {
				if (la == null) { currentState = 645; break; }
				if (la.kind == 22) {
					currentState = 646;
					break;
				} else {
					stateStack.Push(14);
					goto case 18;
				}
			}
			case 646: {
				PushContext(Context.Type, la, t);
				stateStack.Push(647);
				goto case 32;
			}
			case 647: {
				PopContext();
				goto case 645;
			}
			case 648: {
				if (la == null) { currentState = 648; break; }
				Expect(169, la); // "Of"
				currentState = 649;
				break;
			}
			case 649: {
				stateStack.Push(650);
				goto case 428;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				Expect(38, la); // ")"
				currentState = 11;
				break;
			}
			case 651: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 652;
			}
			case 652: {
				if (la == null) { currentState = 652; break; }
				if (set[45].Get(la.kind)) {
					currentState = 652;
					break;
				} else {
					PopContext();
					stateStack.Push(653);
					goto case 18;
				}
			}
			case 653: {
				if (la == null) { currentState = 653; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(653);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 654;
					break;
				}
			}
			case 654: {
				if (la == null) { currentState = 654; break; }
				Expect(160, la); // "Namespace"
				currentState = 18;
				break;
			}
			case 655: {
				if (la == null) { currentState = 655; break; }
				Expect(137, la); // "Imports"
				currentState = 656;
				break;
			}
			case 656: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 657;
			}
			case 657: {
				if (la == null) { currentState = 657; break; }
				if (set[145].Get(la.kind)) {
					currentState = 663;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 659;
						break;
					} else {
						Error(la);
						goto case 658;
					}
				}
			}
			case 658: {
				PopContext();
				goto case 18;
			}
			case 659: {
				stateStack.Push(660);
				goto case 167;
			}
			case 660: {
				if (la == null) { currentState = 660; break; }
				Expect(20, la); // "="
				currentState = 661;
				break;
			}
			case 661: {
				if (la == null) { currentState = 661; break; }
				Expect(3, la); // LiteralString
				currentState = 662;
				break;
			}
			case 662: {
				if (la == null) { currentState = 662; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 658;
				break;
			}
			case 663: {
				if (la == null) { currentState = 663; break; }
				if (la.kind == 37) {
					stateStack.Push(663);
					goto case 37;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 664;
						break;
					} else {
						goto case 658;
					}
				}
			}
			case 664: {
				stateStack.Push(658);
				goto case 32;
			}
			case 665: {
				if (la == null) { currentState = 665; break; }
				Expect(173, la); // "Option"
				currentState = 666;
				break;
			}
			case 666: {
				if (la == null) { currentState = 666; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 668;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 667;
						break;
					} else {
						goto case 458;
					}
				}
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				if (la.kind == 213) {
					currentState = 18;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 18;
						break;
					} else {
						goto case 458;
					}
				}
			}
			case 668: {
				if (la == null) { currentState = 668; break; }
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
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 444604417, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234624, 444596225, 131200, 0}),
		new BitArray(new int[] {1, 256, 1048576, 537395328, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537395328, 134234112, 444596224, 131200, 0}),
		new BitArray(new int[] {0, 0, 1048576, 537395328, 134234112, 444596224, 131200, 0}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 256, 1048576, -1601568064, 671109120, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 256, 1048576, -1601568064, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 256, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -1601699136, 671105024, 1589117058, 393600, 3328}),
		new BitArray(new int[] {0, 0, 1048576, -2138570624, 134234112, 67108864, 393216, 0}),
		new BitArray(new int[] {0, 0, 0, -2139095040, 0, 67108864, 262144, 0}),
		new BitArray(new int[] {-2, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {-940564478, 889192437, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {-940564478, 889192405, 65, 1074825472, 72844576, 231424, 22030368, 4704}),
		new BitArray(new int[] {4, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106740, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405224, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-61995012, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106176, -533656048, 579}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1007673342, 889192405, 65, 1074825472, 72843552, 231424, 22030368, 4672}),
		new BitArray(new int[] {-2, -9, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-1040382, 889192437, 65, 1074825472, 72843296, 231424, 22030368, 4160}),
		new BitArray(new int[] {1006632960, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1028, -16777216, -1, -1, -1, -1, -1, 16383}),
		new BitArray(new int[] {-1038334, -1258291211, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {1007552508, 1140850720, -51384097, -972018405, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {-1040382, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {0, 0, -60035072, 1027, 0, 0, 134217728, 0}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1310752, 65536, 1050656, 64}),
		new BitArray(new int[] {4194304, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018401, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-1048578, 2147483647, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969182, 17106228, -97186288, 8387}),
		new BitArray(new int[] {0, 67108864, 0, 1073743872, 1343520, 65536, 1050656, 64}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {-64092162, -973078488, -51384097, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-64092162, 1191182376, -1048865, -546062565, -1014191950, -1593504452, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 3072, 134447104, 16777216, 8, 0, 0}),
		new BitArray(new int[] {-2097156, -1, -1, -1, -1, -1, -1, -1}),
		new BitArray(new int[] {-66189316, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {6291458, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1174405160, -51384097, -971985637, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {0, 0, 0, -1879044096, 0, 67108864, 67371040, 128}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {2097158, 1140850688, 8388687, 1108347140, 821280, 17105920, -2144335872, 97}),
		new BitArray(new int[] {2097154, -2147483648, 0, 0, 0, 0, 0, 32}),
		new BitArray(new int[] {36, 1140850688, 8388687, 1108347140, 821280, 17105928, -2144335872, 65}),
		new BitArray(new int[] {-66189316, 1174405160, -51384097, -972018405, -1030969166, 17106228, -97186284, 8259}),
		new BitArray(new int[] {1007552508, 1140850720, -51384097, -972002021, -1030969182, 17106208, -365621744, 8259}),
		new BitArray(new int[] {1007681536, -2147483614, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {1007681536, -2147483616, 0, 0, 1024, 0, 0, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 0, 0, 129}),
		new BitArray(new int[] {2097154, 0, 0, 32768, 0, 0, 0, 129}),
		new BitArray(new int[] {-66189316, 1174405160, -51383073, -972018405, -1030969182, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65140740, 1174409128, -51384097, -971985637, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {-65140740, 1174409128, -51384097, -972018405, -1030903646, 17106228, -97186288, 8259}),
		new BitArray(new int[] {1048576, 3968, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680509669, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680476901, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {2097154, 32, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483614, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483616, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {7340034, -2147483648, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 32768, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850690, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388975, 1108347140, 821280, 21316608, -2144335872, 65}),
		new BitArray(new int[] {5242880, -2147483584, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {7, 1157628162, 26477055, -493212676, 680323109, 2147308935, -533262382, 3395}),
		new BitArray(new int[] {918528, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {-909310, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-843774, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-318462, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-383998, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {-1038334, -1258291209, 65, 1074825472, 72844320, 231424, 22030368, 4160}),
		new BitArray(new int[] {4194308, 1140850754, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140851008, 8388975, 1108347140, 821280, 21317120, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 822304, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 16843776, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850698, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850690, 9699551, 1108355356, 9218084, 17106180, -533524976, 67}),
		new BitArray(new int[] {4, 1140850946, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108478212, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 8388687, 1108347140, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671930656, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926560, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {5, 1140850944, 26214479, -493220892, 671926304, 1606227075, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493351964, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850688, 26214479, -493351964, 671926304, 1606227074, -2143942272, 3393}),
		new BitArray(new int[] {4, 1140850688, 26214479, -1030223452, 135055392, 84218880, -2143942656, 65}),
		new BitArray(new int[] {4, 1140850688, 25165903, -1030747868, 821280, 84218880, -2144073728, 65}),
		new BitArray(new int[] {3145730, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {3145730, -2147483648, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {3145730, 0, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850944, 26214479, -493220892, 671926305, 1606227074, -2143942208, 3393}),
		new BitArray(new int[] {0, 256, 0, 537001984, 1, 436207616, 64, 0}),
		new BitArray(new int[] {0, 256, 0, 536870912, 1, 436207616, 64, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 1, 436207616, 64, 0}),
		new BitArray(new int[] {7340034, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850946, 8650975, 1108355356, 9218084, 17106180, -533656048, 67}),
		new BitArray(new int[] {0, 16777472, 0, 131072, 0, 536870912, 2, 0}),
		new BitArray(new int[] {0, 16777472, 0, 0, 0, 536870912, 2, 0}),
		new BitArray(new int[] {2097154, -2147483616, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {0, 1073741824, 4, -2147483648, 0, 0, -2147221504, 0}),
		new BitArray(new int[] {2097154, -2013265888, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 25165903, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {4, 1140850688, 8388687, 1108347136, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {7340034, -2147483648, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 256, 1048576, 537526400, 134234112, 444596225, 131200, 0}),
		new BitArray(new int[] {1028, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67}),
		new BitArray(new int[] {70254594, 32, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {0, 0, 8388608, 33554432, 2048, 0, 32768, 0}),
		new BitArray(new int[] {2097154, 0, 0, 0, 0, 3072, 0, 0}),
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 444596224, 128, 0}),
		new BitArray(new int[] {0, 0, 0, 536871488, 536870912, 1522008194, 384, 3328}),
		new BitArray(new int[] {0, 0, 262288, 8216, 8396800, 256, 1610679824, 2}),
		new BitArray(new int[] {-1013972992, 822083461, 0, 0, 71499776, 163840, 16777216, 4096}),
		new BitArray(new int[] {-1073741824, 33554432, 0, 0, 0, 16, 0, 0}),
		new BitArray(new int[] {1006632960, 0, 0, 0, 0, 0, 0, 0}),
		new BitArray(new int[] {1016, 0, 0, 67108864, -1040187392, 32, 33554432, 0}),
		new BitArray(new int[] {4, 1073741824, 8388687, 34605312, 821280, 17105920, -2144335872, 65}),
		new BitArray(new int[] {0, 0, -1133776896, 3, 0, 0, 0, 0}),
		new BitArray(new int[] {-64092162, 1191182376, -1051937, -680378597, -1030969166, -1593504460, -21144002, 8903}),
		new BitArray(new int[] {0, 0, 33554432, 16777216, 16, 0, 16392, 0}),
		new BitArray(new int[] {-66189316, 1174405160, -51383585, -972018405, -1030969182, 17106228, -97186288, 8259}),
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
		new BitArray(new int[] {0, 0, 0, 536870912, 0, 436207616, 0, 0}),
		new BitArray(new int[] {2097154, 32, 0, 0, 256, 0, 0, 0}),
		new BitArray(new int[] {4, 1140850688, 8650975, 1108355356, 9218084, 17106176, -533656048, 67})

	};

} // end Parser


}