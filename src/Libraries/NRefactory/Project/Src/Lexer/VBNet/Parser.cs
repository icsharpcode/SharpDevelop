using System;
using System.Collections;
using System.Collections.Generic;



namespace ICSharpCode.NRefactory.Parser.VB {



partial class ExpressionFinder {
	const int startOfExpression = 56;
	const int endOfStatementTerminatorAndBlock = 233;
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
			case 71:
			case 234:
			case 471:
				{
					BitArray a = new BitArray(239);
					return a;
				}
			case 7:
				return set[4];
			case 8:
				return set[5];
			case 9:
			case 10:
			case 20:
				{
					BitArray a = new BitArray(239);
					a.Set(142, true);
					return a;
				}
			case 11:
			case 162:
			case 168:
			case 174:
			case 211:
			case 215:
			case 254:
			case 354:
			case 363:
			case 416:
			case 458:
			case 468:
			case 479:
			case 509:
			case 544:
			case 601:
			case 618:
			case 687:
				return set[6];
			case 12:
			case 13:
			case 510:
			case 511:
			case 555:
			case 565:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(37, true);
					return a;
				}
			case 14:
			case 21:
			case 23:
			case 24:
			case 36:
			case 226:
			case 229:
			case 230:
			case 240:
			case 255:
			case 259:
			case 281:
			case 296:
			case 307:
			case 310:
			case 316:
			case 321:
			case 330:
			case 331:
			case 351:
			case 371:
			case 464:
			case 476:
			case 482:
			case 486:
			case 494:
			case 502:
			case 512:
			case 521:
			case 538:
			case 542:
			case 550:
			case 556:
			case 559:
			case 566:
			case 569:
			case 596:
			case 599:
			case 626:
			case 636:
			case 640:
			case 666:
			case 686:
			case 693:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					return a;
				}
			case 15:
			case 16:
				return set[7];
			case 17:
			case 18:
				return set[8];
			case 19:
			case 227:
			case 241:
			case 257:
			case 311:
			case 352:
			case 396:
			case 519:
			case 539:
			case 557:
			case 561:
			case 567:
			case 597:
			case 637:
				{
					BitArray a = new BitArray(239);
					a.Set(113, true);
					return a;
				}
			case 22:
			case 487:
			case 522:
				return set[9];
			case 25:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					return a;
				}
			case 26:
			case 27:
				return set[10];
			case 28:
			case 670:
				return set[11];
			case 29:
				return set[12];
			case 30:
				return set[13];
			case 31:
			case 32:
			case 127:
			case 184:
			case 185:
			case 235:
			case 386:
			case 387:
			case 404:
			case 405:
			case 406:
			case 407:
			case 497:
			case 498:
			case 531:
			case 532:
			case 632:
			case 633:
			case 679:
			case 680:
				return set[14];
			case 33:
			case 34:
			case 459:
			case 460:
			case 469:
			case 470:
			case 499:
			case 500:
			case 623:
			case 634:
			case 635:
				return set[15];
			case 35:
			case 37:
			case 131:
			case 141:
			case 157:
			case 172:
			case 188:
			case 266:
			case 291:
			case 370:
			case 383:
			case 419:
			case 475:
			case 493:
			case 501:
			case 578:
			case 581:
			case 605:
			case 608:
			case 613:
			case 625:
			case 639:
			case 659:
			case 662:
			case 665:
			case 671:
			case 674:
			case 692:
				return set[16];
			case 38:
			case 41:
				return set[17];
			case 39:
				return set[18];
			case 40:
			case 77:
			case 81:
			case 136:
			case 346:
			case 422:
				return set[19];
			case 42:
			case 147:
			case 154:
			case 158:
			case 220:
			case 390:
			case 415:
			case 418:
			case 533:
			case 534:
			case 593:
				{
					BitArray a = new BitArray(239);
					a.Set(37, true);
					return a;
				}
			case 43:
			case 44:
			case 138:
			case 139:
				return set[20];
			case 45:
			case 140:
			case 223:
			case 368:
			case 393:
			case 417:
			case 436:
			case 467:
			case 474:
			case 505:
			case 536:
			case 572:
			case 575:
			case 587:
			case 595:
			case 612:
			case 629:
			case 643:
			case 669:
			case 678:
				{
					BitArray a = new BitArray(239);
					a.Set(38, true);
					return a;
				}
			case 46:
			case 47:
			case 51:
			case 52:
			case 53:
			case 55:
			case 427:
			case 428:
				return set[21];
			case 48:
			case 49:
				return set[22];
			case 50:
			case 149:
			case 156:
			case 349:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					return a;
				}
			case 54:
			case 142:
			case 151:
			case 367:
			case 369:
			case 373:
			case 381:
			case 426:
			case 430:
			case 432:
			case 433:
			case 443:
			case 450:
			case 457:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 56:
			case 57:
			case 59:
			case 60:
			case 61:
			case 64:
			case 79:
			case 129:
			case 148:
			case 150:
			case 152:
			case 155:
			case 164:
			case 166:
			case 206:
			case 239:
			case 243:
			case 245:
			case 246:
			case 263:
			case 280:
			case 285:
			case 294:
			case 300:
			case 302:
			case 306:
			case 309:
			case 315:
			case 326:
			case 328:
			case 334:
			case 348:
			case 350:
			case 382:
			case 409:
			case 424:
			case 425:
			case 492:
			case 577:
				return set[23];
			case 58:
			case 62:
				return set[24];
			case 63:
			case 73:
			case 74:
				{
					BitArray a = new BitArray(239);
					a.Set(35, true);
					return a;
				}
			case 65:
			case 80:
			case 453:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(36, true);
					return a;
				}
			case 66:
				{
					BitArray a = new BitArray(239);
					a.Set(36, true);
					return a;
				}
			case 67:
			case 101:
				{
					BitArray a = new BitArray(239);
					a.Set(162, true);
					return a;
				}
			case 68:
			case 69:
				return set[25];
			case 70:
			case 82:
				{
					BitArray a = new BitArray(239);
					a.Set(233, true);
					return a;
				}
			case 72:
				return set[26];
			case 75:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					a.Set(147, true);
					return a;
				}
			case 76:
				{
					BitArray a = new BitArray(239);
					a.Set(26, true);
					return a;
				}
			case 78:
			case 187:
			case 189:
			case 190:
			case 293:
			case 688:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					return a;
				}
			case 83:
			case 312:
				{
					BitArray a = new BitArray(239);
					a.Set(231, true);
					return a;
				}
			case 84:
				{
					BitArray a = new BitArray(239);
					a.Set(230, true);
					return a;
				}
			case 85:
				{
					BitArray a = new BitArray(239);
					a.Set(224, true);
					return a;
				}
			case 86:
				{
					BitArray a = new BitArray(239);
					a.Set(223, true);
					return a;
				}
			case 87:
			case 258:
				{
					BitArray a = new BitArray(239);
					a.Set(218, true);
					return a;
				}
			case 88:
				{
					BitArray a = new BitArray(239);
					a.Set(213, true);
					return a;
				}
			case 89:
				{
					BitArray a = new BitArray(239);
					a.Set(212, true);
					return a;
				}
			case 90:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					return a;
				}
			case 91:
			case 397:
				{
					BitArray a = new BitArray(239);
					a.Set(210, true);
					return a;
				}
			case 92:
				{
					BitArray a = new BitArray(239);
					a.Set(209, true);
					return a;
				}
			case 93:
				{
					BitArray a = new BitArray(239);
					a.Set(206, true);
					return a;
				}
			case 94:
				{
					BitArray a = new BitArray(239);
					a.Set(203, true);
					return a;
				}
			case 95:
			case 318:
				{
					BitArray a = new BitArray(239);
					a.Set(197, true);
					return a;
				}
			case 96:
			case 543:
			case 562:
				{
					BitArray a = new BitArray(239);
					a.Set(186, true);
					return a;
				}
			case 97:
				{
					BitArray a = new BitArray(239);
					a.Set(184, true);
					return a;
				}
			case 98:
				{
					BitArray a = new BitArray(239);
					a.Set(176, true);
					return a;
				}
			case 99:
				{
					BitArray a = new BitArray(239);
					a.Set(170, true);
					return a;
				}
			case 100:
			case 275:
			case 282:
			case 297:
				{
					BitArray a = new BitArray(239);
					a.Set(163, true);
					return a;
				}
			case 102:
				{
					BitArray a = new BitArray(239);
					a.Set(147, true);
					return a;
				}
			case 103:
			case 193:
			case 198:
			case 200:
				{
					BitArray a = new BitArray(239);
					a.Set(146, true);
					return a;
				}
			case 104:
			case 195:
			case 199:
				{
					BitArray a = new BitArray(239);
					a.Set(143, true);
					return a;
				}
			case 105:
				{
					BitArray a = new BitArray(239);
					a.Set(139, true);
					return a;
				}
			case 106:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					return a;
				}
			case 107:
			case 228:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					return a;
				}
			case 108:
			case 218:
				{
					BitArray a = new BitArray(239);
					a.Set(126, true);
					return a;
				}
			case 109:
				{
					BitArray a = new BitArray(239);
					a.Set(124, true);
					return a;
				}
			case 110:
				{
					BitArray a = new BitArray(239);
					a.Set(121, true);
					return a;
				}
			case 111:
			case 165:
				{
					BitArray a = new BitArray(239);
					a.Set(116, true);
					return a;
				}
			case 112:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					return a;
				}
			case 113:
				{
					BitArray a = new BitArray(239);
					a.Set(107, true);
					return a;
				}
			case 114:
				{
					BitArray a = new BitArray(239);
					a.Set(104, true);
					return a;
				}
			case 115:
			case 588:
				{
					BitArray a = new BitArray(239);
					a.Set(98, true);
					return a;
				}
			case 116:
				{
					BitArray a = new BitArray(239);
					a.Set(87, true);
					return a;
				}
			case 117:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					return a;
				}
			case 118:
			case 177:
			case 205:
				{
					BitArray a = new BitArray(239);
					a.Set(70, true);
					return a;
				}
			case 119:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					return a;
				}
			case 120:
				{
					BitArray a = new BitArray(239);
					a.Set(66, true);
					return a;
				}
			case 121:
				{
					BitArray a = new BitArray(239);
					a.Set(65, true);
					return a;
				}
			case 122:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					return a;
				}
			case 123:
				{
					BitArray a = new BitArray(239);
					a.Set(62, true);
					return a;
				}
			case 124:
			case 217:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					return a;
				}
			case 125:
				{
					BitArray a = new BitArray(239);
					a.Set(2, true);
					return a;
				}
			case 126:
				return set[27];
			case 128:
				return set[28];
			case 130:
				{
					BitArray a = new BitArray(239);
					a.Set(144, true);
					return a;
				}
			case 132:
				return set[29];
			case 133:
				return set[30];
			case 134:
			case 135:
			case 420:
			case 421:
				return set[31];
			case 137:
				return set[32];
			case 143:
			case 144:
			case 278:
			case 287:
				return set[33];
			case 145:
			case 399:
				return set[34];
			case 146:
			case 333:
				{
					BitArray a = new BitArray(239);
					a.Set(135, true);
					return a;
				}
			case 153:
				return set[35];
			case 159:
				{
					BitArray a = new BitArray(239);
					a.Set(58, true);
					a.Set(126, true);
					return a;
				}
			case 160:
			case 161:
				return set[36];
			case 163:
				{
					BitArray a = new BitArray(239);
					a.Set(171, true);
					return a;
				}
			case 167:
			case 181:
			case 197:
			case 202:
			case 208:
			case 210:
			case 214:
			case 216:
				return set[37];
			case 169:
			case 170:
				{
					BitArray a = new BitArray(239);
					a.Set(63, true);
					a.Set(138, true);
					return a;
				}
			case 171:
			case 173:
			case 279:
				{
					BitArray a = new BitArray(239);
					a.Set(138, true);
					return a;
				}
			case 175:
			case 176:
			case 178:
			case 180:
			case 182:
			case 183:
			case 191:
			case 196:
			case 201:
			case 209:
			case 213:
				return set[38];
			case 179:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(143, true);
					return a;
				}
			case 186:
				return set[39];
			case 192:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(70, true);
					return a;
				}
			case 194:
				{
					BitArray a = new BitArray(239);
					a.Set(133, true);
					a.Set(143, true);
					a.Set(146, true);
					return a;
				}
			case 203:
			case 204:
				return set[40];
			case 207:
				{
					BitArray a = new BitArray(239);
					a.Set(64, true);
					a.Set(104, true);
					return a;
				}
			case 212:
				return set[41];
			case 219:
			case 496:
			case 617:
			case 631:
			case 638:
				{
					BitArray a = new BitArray(239);
					a.Set(127, true);
					a.Set(210, true);
					return a;
				}
			case 221:
			case 222:
			case 391:
			case 392:
			case 465:
			case 466:
			case 472:
			case 473:
			case 570:
			case 571:
			case 573:
			case 574:
			case 585:
			case 586:
			case 610:
			case 611:
			case 627:
			case 628:
				return set[42];
			case 224:
			case 225:
				return set[43];
			case 231:
			case 232:
				return set[44];
			case 233:
				return set[45];
			case 236:
				return set[46];
			case 237:
			case 238:
			case 339:
				return set[47];
			case 242:
				{
					BitArray a = new BitArray(239);
					a.Set(226, true);
					return a;
				}
			case 244:
			case 286:
			case 301:
				return set[48];
			case 247:
			case 248:
			case 268:
			case 269:
			case 283:
			case 284:
			case 298:
			case 299:
				return set[49];
			case 249:
			case 340:
			case 343:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(111, true);
					return a;
				}
			case 250:
				{
					BitArray a = new BitArray(239);
					a.Set(108, true);
					a.Set(124, true);
					a.Set(231, true);
					return a;
				}
			case 251:
				return set[50];
			case 252:
			case 271:
				return set[51];
			case 253:
				{
					BitArray a = new BitArray(239);
					a.Set(5, true);
					return a;
				}
			case 256:
				{
					BitArray a = new BitArray(239);
					a.Set(75, true);
					a.Set(113, true);
					a.Set(123, true);
					return a;
				}
			case 260:
			case 261:
				return set[52];
			case 262:
			case 267:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(229, true);
					return a;
				}
			case 264:
			case 265:
				return set[53];
			case 270:
				return set[54];
			case 272:
				{
					BitArray a = new BitArray(239);
					a.Set(118, true);
					return a;
				}
			case 273:
			case 274:
				return set[55];
			case 276:
			case 277:
				return set[56];
			case 288:
			case 289:
				return set[57];
			case 290:
				return set[58];
			case 292:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(138, true);
					return a;
				}
			case 295:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(205, true);
					return a;
				}
			case 303:
				return set[59];
			case 304:
			case 308:
				{
					BitArray a = new BitArray(239);
					a.Set(152, true);
					return a;
				}
			case 305:
				return set[60];
			case 313:
			case 314:
				return set[61];
			case 317:
				{
					BitArray a = new BitArray(239);
					a.Set(74, true);
					a.Set(113, true);
					return a;
				}
			case 319:
			case 320:
				return set[62];
			case 322:
			case 323:
				return set[63];
			case 324:
			case 606:
			case 607:
			case 609:
			case 646:
			case 660:
			case 661:
			case 663:
			case 672:
			case 673:
			case 675:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(22, true);
					return a;
				}
			case 325:
			case 327:
				return set[64];
			case 329:
			case 335:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(214, true);
					return a;
				}
			case 332:
				{
					BitArray a = new BitArray(239);
					a.Set(111, true);
					a.Set(112, true);
					a.Set(113, true);
					return a;
				}
			case 336:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(135, true);
					return a;
				}
			case 337:
			case 338:
			case 394:
			case 395:
				return set[65];
			case 341:
			case 342:
			case 344:
			case 345:
				return set[66];
			case 347:
				return set[67];
			case 353:
				{
					BitArray a = new BitArray(239);
					a.Set(211, true);
					a.Set(233, true);
					return a;
				}
			case 355:
			case 356:
			case 364:
			case 365:
				return set[68];
			case 357:
			case 366:
				return set[69];
			case 358:
				return set[70];
			case 359:
			case 362:
				return set[71];
			case 360:
			case 361:
			case 652:
			case 653:
				return set[72];
			case 372:
			case 374:
			case 375:
			case 535:
			case 594:
				return set[73];
			case 376:
			case 377:
				return set[74];
			case 378:
			case 379:
				return set[75];
			case 380:
			case 384:
				{
					BitArray a = new BitArray(239);
					a.Set(20, true);
					a.Set(22, true);
					a.Set(38, true);
					return a;
				}
			case 385:
				{
					BitArray a = new BitArray(239);
					a.Set(40, true);
					return a;
				}
			case 388:
			case 389:
				return set[76];
			case 398:
				return set[77];
			case 400:
			case 413:
				return set[78];
			case 401:
			case 414:
				return set[79];
			case 402:
			case 403:
				{
					BitArray a = new BitArray(239);
					a.Set(10, true);
					return a;
				}
			case 408:
				{
					BitArray a = new BitArray(239);
					a.Set(12, true);
					return a;
				}
			case 410:
				{
					BitArray a = new BitArray(239);
					a.Set(13, true);
					return a;
				}
			case 411:
				return set[80];
			case 412:
				return set[81];
			case 423:
				return set[82];
			case 429:
			case 431:
				return set[83];
			case 434:
			case 435:
			case 503:
			case 504:
			case 641:
			case 642:
				return set[84];
			case 437:
			case 438:
			case 439:
			case 444:
			case 445:
			case 506:
			case 644:
			case 668:
			case 677:
				return set[85];
			case 440:
			case 446:
			case 455:
				return set[86];
			case 441:
			case 442:
			case 447:
			case 448:
				{
					BitArray a = new BitArray(239);
					a.Set(22, true);
					a.Set(38, true);
					a.Set(63, true);
					return a;
				}
			case 449:
			case 451:
			case 456:
				return set[87];
			case 452:
			case 454:
				return set[88];
			case 461:
			case 480:
			case 481:
			case 537:
			case 624:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(63, true);
					return a;
				}
			case 462:
			case 463:
			case 541:
				return set[89];
			case 477:
			case 478:
			case 485:
				{
					BitArray a = new BitArray(239);
					a.Set(115, true);
					return a;
				}
			case 483:
			case 484:
				return set[90];
			case 488:
			case 489:
				return set[91];
			case 490:
			case 491:
			case 549:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(20, true);
					a.Set(21, true);
					return a;
				}
			case 495:
				{
					BitArray a = new BitArray(239);
					a.Set(103, true);
					return a;
				}
			case 507:
			case 508:
			case 520:
				{
					BitArray a = new BitArray(239);
					a.Set(84, true);
					a.Set(155, true);
					a.Set(209, true);
					return a;
				}
			case 513:
			case 514:
				return set[92];
			case 515:
			case 516:
				return set[93];
			case 517:
			case 518:
			case 529:
				return set[94];
			case 523:
			case 524:
				return set[95];
			case 525:
			case 526:
			case 657:
				return set[96];
			case 527:
				return set[97];
			case 528:
				return set[98];
			case 530:
			case 540:
				{
					BitArray a = new BitArray(239);
					a.Set(172, true);
					return a;
				}
			case 545:
			case 546:
				return set[99];
			case 547:
				return set[100];
			case 548:
			case 584:
				return set[101];
			case 551:
			case 552:
			case 553:
			case 576:
				return set[102];
			case 554:
			case 558:
			case 568:
				{
					BitArray a = new BitArray(239);
					a.Set(128, true);
					a.Set(198, true);
					return a;
				}
			case 560:
				return set[103];
			case 563:
				return set[104];
			case 564:
				return set[105];
			case 579:
			case 580:
			case 582:
			case 651:
			case 654:
			case 655:
				return set[106];
			case 583:
				return set[107];
			case 589:
			case 591:
			case 600:
				{
					BitArray a = new BitArray(239);
					a.Set(119, true);
					return a;
				}
			case 590:
				return set[108];
			case 592:
				return set[109];
			case 598:
				{
					BitArray a = new BitArray(239);
					a.Set(56, true);
					a.Set(189, true);
					a.Set(193, true);
					return a;
				}
			case 602:
			case 603:
				return set[110];
			case 604:
			case 614:
				{
					BitArray a = new BitArray(239);
					a.Set(1, true);
					a.Set(21, true);
					a.Set(136, true);
					return a;
				}
			case 615:
				{
					BitArray a = new BitArray(239);
					a.Set(101, true);
					return a;
				}
			case 616:
				return set[111];
			case 619:
			case 620:
				{
					BitArray a = new BitArray(239);
					a.Set(149, true);
					return a;
				}
			case 621:
			case 630:
			case 689:
				{
					BitArray a = new BitArray(239);
					a.Set(3, true);
					return a;
				}
			case 622:
				return set[112];
			case 645:
			case 647:
				return set[113];
			case 648:
			case 656:
				return set[114];
			case 649:
			case 650:
				return set[115];
			case 658:
				{
					BitArray a = new BitArray(239);
					a.Set(136, true);
					return a;
				}
			case 664:
				{
					BitArray a = new BitArray(239);
					a.Set(140, true);
					return a;
				}
			case 667:
			case 676:
				{
					BitArray a = new BitArray(239);
					a.Set(169, true);
					return a;
				}
			case 681:
				return set[116];
			case 682:
				{
					BitArray a = new BitArray(239);
					a.Set(160, true);
					return a;
				}
			case 683:
				{
					BitArray a = new BitArray(239);
					a.Set(137, true);
					return a;
				}
			case 684:
			case 685:
				return set[117];
			case 690:
				{
					BitArray a = new BitArray(239);
					a.Set(11, true);
					return a;
				}
			case 691:
				return set[118];
			case 694:
				{
					BitArray a = new BitArray(239);
					a.Set(173, true);
					return a;
				}
			case 695:
				return set[119];
			case 696:
				{
					BitArray a = new BitArray(239);
					a.Set(67, true);
					a.Set(213, true);
					return a;
				}
			case 697:
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
	int activeArgument = 0;
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
					goto case 694;
				} else {
					goto case 2;
				}
			}
			case 2: {
				if (la == null) { currentState = 2; break; }
				if (la.kind == 137) {
					stateStack.Push(2);
					goto case 683;
				} else {
					goto case 3;
				}
			}
			case 3: {
				if (la == null) { currentState = 3; break; }
				if (la.kind == 40) {
					stateStack.Push(3);
					goto case 385;
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
					currentState = 679;
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
					goto case 385;
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
						goto case 507;
					} else {
						if (la.kind == 103) {
							currentState = 496;
							break;
						} else {
							if (la.kind == 115) {
								goto case 477;
							} else {
								if (la.kind == 142) {
									goto case 9;
								} else {
									goto case 6;
								}
							}
						}
					}
				}
			}
			case 9: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 10;
			}
			case 10: {
				if (la == null) { currentState = 10; break; }
				Expect(142, la); // "Interface"
				currentState = 11;
				break;
			}
			case 11: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(12);
				goto case 174;
			}
			case 12: {
				PopContext();
				goto case 13;
			}
			case 13: {
				if (la == null) { currentState = 13; break; }
				if (la.kind == 37) {
					currentState = 676;
					break;
				} else {
					goto case 14;
				}
			}
			case 14: {
				stateStack.Push(15);
				goto case 23;
			}
			case 15: {
				isMissingModifier = true;
				goto case 16;
			}
			case 16: {
				if (la == null) { currentState = 16; break; }
				if (la.kind == 140) {
					currentState = 671;
					break;
				} else {
					goto case 17;
				}
			}
			case 17: {
				isMissingModifier = true;
				goto case 18;
			}
			case 18: {
				if (la == null) { currentState = 18; break; }
				if (set[10].Get(la.kind)) {
					goto case 26;
				} else {
					isMissingModifier = false;
					goto case 19;
				}
			}
			case 19: {
				if (la == null) { currentState = 19; break; }
				Expect(113, la); // "End"
				currentState = 20;
				break;
			}
			case 20: {
				if (la == null) { currentState = 20; break; }
				Expect(142, la); // "Interface"
				currentState = 21;
				break;
			}
			case 21: {
				stateStack.Push(22);
				goto case 23;
			}
			case 22: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 23: {
				if (la != null) CurrentBlock.lastExpressionStart = la.Location;
				goto case 24;
			}
			case 24: {
				if (la == null) { currentState = 24; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					if (la.kind == 21) {
						currentState = stateStack.Pop();
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 25: {
				if (la == null) { currentState = 25; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 26: {
				isMissingModifier = true;
				goto case 27;
			}
			case 27: {
				if (la == null) { currentState = 27; break; }
				if (la.kind == 40) {
					stateStack.Push(26);
					goto case 385;
				} else {
					isMissingModifier = true;
					goto case 28;
				}
			}
			case 28: {
				if (la == null) { currentState = 28; break; }
				if (set[122].Get(la.kind)) {
					currentState = 670;
					break;
				} else {
					isMissingModifier = false;
					goto case 29;
				}
			}
			case 29: {
				if (la == null) { currentState = 29; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(17);
					goto case 507;
				} else {
					if (la.kind == 103) {
						stateStack.Push(17);
						goto case 495;
					} else {
						if (la.kind == 115) {
							stateStack.Push(17);
							goto case 477;
						} else {
							if (la.kind == 142) {
								stateStack.Push(17);
								goto case 9;
							} else {
								if (set[13].Get(la.kind)) {
									stateStack.Push(17);
									goto case 30;
								} else {
									Error(la);
									goto case 17;
								}
							}
						}
					}
				}
			}
			case 30: {
				if (la == null) { currentState = 30; break; }
				if (la.kind == 119) {
					currentState = 468;
					break;
				} else {
					if (la.kind == 186) {
						currentState = 458;
						break;
					} else {
						if (la.kind == 127 || la.kind == 210) {
							currentState = 31;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 31: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 32;
			}
			case 32: {
				if (la == null) { currentState = 32; break; }
				currentState = 33;
				break;
			}
			case 33: {
				PopContext();
				goto case 34;
			}
			case 34: {
				if (la == null) { currentState = 34; break; }
				if (la.kind == 37) {
					currentState = 434;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 35;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 35: {
				PushContext(Context.Type, la, t);
				stateStack.Push(36);
				goto case 37;
			}
			case 36: {
				PopContext();
				goto case 23;
			}
			case 37: {
				if (la == null) { currentState = 37; break; }
				if (la.kind == 130) {
					currentState = 38;
					break;
				} else {
					if (set[6].Get(la.kind)) {
						currentState = 38;
						break;
					} else {
						if (set[123].Get(la.kind)) {
							currentState = 38;
							break;
						} else {
							if (la.kind == 33) {
								currentState = 38;
								break;
							} else {
								Error(la);
								goto case 38;
							}
						}
					}
				}
			}
			case 38: {
				if (la == null) { currentState = 38; break; }
				if (la.kind == 37) {
					stateStack.Push(38);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 39: {
				if (la == null) { currentState = 39; break; }
				if (la.kind == 26) {
					currentState = 40;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 40: {
				stateStack.Push(41);
				goto case 81;
			}
			case 41: {
				if (la == null) { currentState = 41; break; }
				if (la.kind == 37) {
					stateStack.Push(41);
					goto case 42;
				} else {
					goto case 39;
				}
			}
			case 42: {
				if (la == null) { currentState = 42; break; }
				Expect(37, la); // "("
				currentState = 43;
				break;
			}
			case 43: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 44;
			}
			case 44: {
				if (la == null) { currentState = 44; break; }
				if (la.kind == 169) {
					currentState = 429;
					break;
				} else {
					if (set[21].Get(la.kind)) {
						goto case 46;
					} else {
						Error(la);
						goto case 45;
					}
				}
			}
			case 45: {
				if (la == null) { currentState = 45; break; }
				Expect(38, la); // ")"
				currentState = stateStack.Pop();
				break;
			}
			case 46: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 47;
			}
			case 47: {
				if (la == null) { currentState = 47; break; }
				if (set[22].Get(la.kind)) {
					stateStack.Push(45);
					goto case 48;
				} else {
					goto case 45;
				}
			}
			case 48: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 49;
			}
			case 49: {
				if (la == null) { currentState = 49; break; }
				if (set[23].Get(la.kind)) {
					activeArgument = 0;
					goto case 425;
				} else {
					if (la.kind == 22) {
						activeArgument = 0;
						goto case 50;
					} else {
						goto case 6;
					}
				}
			}
			case 50: {
				if (la == null) { currentState = 50; break; }
				Expect(22, la); // ","
				currentState = 51;
				break;
			}
			case 51: {
				activeArgument++;
				goto case 52;
			}
			case 52: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 53;
			}
			case 53: {
				if (la == null) { currentState = 53; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(54);
					goto case 56;
				} else {
					goto case 54;
				}
			}
			case 54: {
				if (la == null) { currentState = 54; break; }
				if (la.kind == 22) {
					currentState = 55;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 55: {
				activeArgument++;
				goto case 52;
			}
			case 56: {
				PushContext(Context.Expression, la, t);
				goto case 57;
			}
			case 57: {
				stateStack.Push(58);
				goto case 59;
			}
			case 58: {
				if (la == null) { currentState = 58; break; }
				if (set[124].Get(la.kind)) {
					currentState = 57;
					break;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 59: {
				PushContext(Context.Expression, la, t);
				goto case 60;
			}
			case 60: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 61;
			}
			case 61: {
				if (la == null) { currentState = 61; break; }
				if (set[125].Get(la.kind)) {
					currentState = 60;
					break;
				} else {
					if (set[33].Get(la.kind)) {
						stateStack.Push(132);
						goto case 143;
					} else {
						if (la.kind == 220) {
							currentState = 129;
							break;
						} else {
							if (la.kind == 162) {
								stateStack.Push(62);
								goto case 67;
							} else {
								if (la.kind == 35) {
									stateStack.Push(62);
									goto case 63;
								} else {
									Error(la);
									goto case 62;
								}
							}
						}
					}
				}
			}
			case 62: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 63: {
				if (la == null) { currentState = 63; break; }
				Expect(35, la); // "{"
				currentState = 64;
				break;
			}
			case 64: {
				stateStack.Push(65);
				goto case 56;
			}
			case 65: {
				if (la == null) { currentState = 65; break; }
				if (la.kind == 22) {
					currentState = 64;
					break;
				} else {
					goto case 66;
				}
			}
			case 66: {
				if (la == null) { currentState = 66; break; }
				Expect(36, la); // "}"
				currentState = stateStack.Pop();
				break;
			}
			case 67: {
				if (la == null) { currentState = 67; break; }
				Expect(162, la); // "New"
				currentState = 68;
				break;
			}
			case 68: {
				PushContext(Context.ObjectCreation, la, t);
				goto case 69;
			}
			case 69: {
				if (la == null) { currentState = 69; break; }
				if (set[16].Get(la.kind)) {
					stateStack.Push(126);
					goto case 37;
				} else {
					goto case 70;
				}
			}
			case 70: {
				if (la == null) { currentState = 70; break; }
				if (la.kind == 233) {
					currentState = 73;
					break;
				} else {
					goto case 71;
				}
			}
			case 71: {
				Error(la);
				goto case 72;
			}
			case 72: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 73: {
				stateStack.Push(72);
				goto case 74;
			}
			case 74: {
				if (la == null) { currentState = 74; break; }
				Expect(35, la); // "{"
				currentState = 75;
				break;
			}
			case 75: {
				if (la == null) { currentState = 75; break; }
				if (la.kind == 147) {
					currentState = 76;
					break;
				} else {
					goto case 76;
				}
			}
			case 76: {
				if (la == null) { currentState = 76; break; }
				Expect(26, la); // "."
				currentState = 77;
				break;
			}
			case 77: {
				stateStack.Push(78);
				goto case 81;
			}
			case 78: {
				if (la == null) { currentState = 78; break; }
				Expect(20, la); // "="
				currentState = 79;
				break;
			}
			case 79: {
				stateStack.Push(80);
				goto case 56;
			}
			case 80: {
				if (la == null) { currentState = 80; break; }
				if (la.kind == 22) {
					currentState = 75;
					break;
				} else {
					goto case 66;
				}
			}
			case 81: {
				if (la == null) { currentState = 81; break; }
				if (la.kind == 2) {
					goto case 125;
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
								goto case 124;
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
												goto case 123;
											} else {
												if (la.kind == 63) {
													currentState = stateStack.Pop();
													break;
												} else {
													if (la.kind == 64) {
														goto case 122;
													} else {
														if (la.kind == 65) {
															goto case 121;
														} else {
															if (la.kind == 66) {
																goto case 120;
															} else {
																if (la.kind == 67) {
																	goto case 119;
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
																				goto case 118;
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
																																		goto case 117;
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
																																					goto case 116;
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
																																																goto case 115;
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
																																																						goto case 114;
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
																																																									goto case 113;
																																																								} else {
																																																									if (la.kind == 108) {
																																																										goto case 112;
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
																																																																		goto case 111;
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
																																																																							goto case 110;
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
																																																																										goto case 109;
																																																																									} else {
																																																																										if (la.kind == 125) {
																																																																											currentState = stateStack.Pop();
																																																																											break;
																																																																										} else {
																																																																											if (la.kind == 126) {
																																																																												goto case 108;
																																																																											} else {
																																																																												if (la.kind == 127) {
																																																																													goto case 107;
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
																																																																																			goto case 106;
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
																																																																																									goto case 105;
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
																																																																																													goto case 104;
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
																																																																																																goto case 103;
																																																																																															} else {
																																																																																																if (la.kind == 147) {
																																																																																																	goto case 102;
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
																																																																																																																goto case 101;
																																																																																																															} else {
																																																																																																																if (la.kind == 163) {
																																																																																																																	goto case 100;
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
																																																																																																																								goto case 99;
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
																																																																																																																														goto case 98;
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
																																																																																																																																						goto case 97;
																																																																																																																																					} else {
																																																																																																																																						if (la.kind == 185) {
																																																																																																																																							currentState = stateStack.Pop();
																																																																																																																																							break;
																																																																																																																																						} else {
																																																																																																																																							if (la.kind == 186) {
																																																																																																																																								goto case 96;
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
																																																																																																																																																			goto case 95;
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
																																																																																																																																																									goto case 94;
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
																																																																																																																																																												goto case 93;
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
																																																																																																																																																															goto case 92;
																																																																																																																																																														} else {
																																																																																																																																																															if (la.kind == 210) {
																																																																																																																																																																goto case 91;
																																																																																																																																																															} else {
																																																																																																																																																																if (la.kind == 211) {
																																																																																																																																																																	goto case 90;
																																																																																																																																																																} else {
																																																																																																																																																																	if (la.kind == 212) {
																																																																																																																																																																		goto case 89;
																																																																																																																																																																	} else {
																																																																																																																																																																		if (la.kind == 213) {
																																																																																																																																																																			goto case 88;
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
																																																																																																																																																																								goto case 87;
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
																																																																																																																																																																													goto case 86;
																																																																																																																																																																												} else {
																																																																																																																																																																													if (la.kind == 224) {
																																																																																																																																																																														goto case 85;
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
																																																																																																																																																																																				goto case 84;
																																																																																																																																																																																			} else {
																																																																																																																																																																																				if (la.kind == 231) {
																																																																																																																																																																																					goto case 83;
																																																																																																																																																																																				} else {
																																																																																																																																																																																					if (la.kind == 232) {
																																																																																																																																																																																						currentState = stateStack.Pop();
																																																																																																																																																																																						break;
																																																																																																																																																																																					} else {
																																																																																																																																																																																						if (la.kind == 233) {
																																																																																																																																																																																							goto case 82;
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
				currentState = stateStack.Pop();
				break;
			}
			case 121: {
				if (la == null) { currentState = 121; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 122: {
				if (la == null) { currentState = 122; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 123: {
				if (la == null) { currentState = 123; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 124: {
				if (la == null) { currentState = 124; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 125: {
				if (la == null) { currentState = 125; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 126: {
				if (la == null) { currentState = 126; break; }
				if (la.kind == 126 || la.kind == 233) {
					if (la.kind == 126) {
						currentState = 127;
						break;
					} else {
						goto case 70;
					}
				} else {
					goto case 72;
				}
			}
			case 127: {
				if (la == null) { currentState = 127; break; }
				if (la.kind == 35) {
					stateStack.Push(72);
					goto case 63;
				} else {
					if (set[28].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
							InformToken(t); /* process From again*/
							/* for processing current token (la): go to the position after processing End */
							goto switchlbl;

					} else {
						goto case 71;
					}
				}
			}
			case 128: {
				if (la == null) { currentState = 128; break; }
				currentState = 72;
				break;
			}
			case 129: {
				stateStack.Push(130);
				goto case 59;
			}
			case 130: {
				if (la == null) { currentState = 130; break; }
				Expect(144, la); // "Is"
				currentState = 131;
				break;
			}
			case 131: {
				stateStack.Push(62);
				goto case 37;
			}
			case 132: {
				if (la == null) { currentState = 132; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(132);
					goto case 133;
				} else {
					goto case 62;
				}
			}
			case 133: {
				if (la == null) { currentState = 133; break; }
				if (la.kind == 37) {
					currentState = 138;
					break;
				} else {
					if (set[126].Get(la.kind)) {
						currentState = 134;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 134: {
				nextTokenIsStartOfImportsOrAccessExpression = true;
				goto case 135;
			}
			case 135: {
				if (la == null) { currentState = 135; break; }
				if (la.kind == 10) {
					currentState = 136;
					break;
				} else {
					goto case 136;
				}
			}
			case 136: {
				stateStack.Push(137);
				goto case 81;
			}
			case 137: {
				if (la == null) { currentState = 137; break; }
				if (la.kind == 11) {
					currentState = stateStack.Pop();
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 138: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 139;
			}
			case 139: {
				if (la == null) { currentState = 139; break; }
				if (la.kind == 169) {
					currentState = 141;
					break;
				} else {
					if (set[21].Get(la.kind)) {
						if (set[22].Get(la.kind)) {
							stateStack.Push(140);
							goto case 48;
						} else {
							goto case 140;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 140: {
				PopContext();
				goto case 45;
			}
			case 141: {
				stateStack.Push(142);
				goto case 37;
			}
			case 142: {
				if (la == null) { currentState = 142; break; }
				if (la.kind == 22) {
					currentState = 141;
					break;
				} else {
					goto case 45;
				}
			}
			case 143: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 144;
			}
			case 144: {
				if (la == null) { currentState = 144; break; }
				if (set[127].Get(la.kind)) {
					currentState = 145;
					break;
				} else {
					if (la.kind == 37) {
						currentState = 424;
						break;
					} else {
						if (set[128].Get(la.kind)) {
							currentState = 145;
							break;
						} else {
							if (set[123].Get(la.kind)) {
								currentState = 145;
								break;
							} else {
								if (set[126].Get(la.kind)) {
									currentState = 420;
									break;
								} else {
									if (la.kind == 129) {
										currentState = 418;
										break;
									} else {
										if (la.kind == 237) {
											currentState = 415;
											break;
										} else {
											if (set[77].Get(la.kind)) {
												stateStack.Push(145);
												nextTokenIsPotentialStartOfExpression = true;
												PushContext(Context.Xml, la, t);
												goto case 398;
											} else {
												if (la.kind == 127 || la.kind == 210) {
													stateStack.Push(145);
													goto case 219;
												} else {
													if (la.kind == 58 || la.kind == 126) {
														stateStack.Push(145);
														PushContext(Context.Query, la, t);
														goto case 159;
													} else {
														if (set[35].Get(la.kind)) {
															stateStack.Push(145);
															goto case 153;
														} else {
															if (la.kind == 135) {
																stateStack.Push(145);
																goto case 146;
															} else {
																Error(la);
																goto case 145;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			case 145: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 146: {
				if (la == null) { currentState = 146; break; }
				Expect(135, la); // "If"
				currentState = 147;
				break;
			}
			case 147: {
				if (la == null) { currentState = 147; break; }
				Expect(37, la); // "("
				currentState = 148;
				break;
			}
			case 148: {
				stateStack.Push(149);
				goto case 56;
			}
			case 149: {
				if (la == null) { currentState = 149; break; }
				Expect(22, la); // ","
				currentState = 150;
				break;
			}
			case 150: {
				stateStack.Push(151);
				goto case 56;
			}
			case 151: {
				if (la == null) { currentState = 151; break; }
				if (la.kind == 22) {
					currentState = 152;
					break;
				} else {
					goto case 45;
				}
			}
			case 152: {
				stateStack.Push(45);
				goto case 56;
			}
			case 153: {
				if (la == null) { currentState = 153; break; }
				if (set[129].Get(la.kind)) {
					currentState = 158;
					break;
				} else {
					if (la.kind == 94 || la.kind == 106 || la.kind == 219) {
						currentState = 154;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 154: {
				if (la == null) { currentState = 154; break; }
				Expect(37, la); // "("
				currentState = 155;
				break;
			}
			case 155: {
				stateStack.Push(156);
				goto case 56;
			}
			case 156: {
				if (la == null) { currentState = 156; break; }
				Expect(22, la); // ","
				currentState = 157;
				break;
			}
			case 157: {
				stateStack.Push(45);
				goto case 37;
			}
			case 158: {
				if (la == null) { currentState = 158; break; }
				Expect(37, la); // "("
				currentState = 152;
				break;
			}
			case 159: {
				if (la == null) { currentState = 159; break; }
				if (la.kind == 126) {
					stateStack.Push(160);
					goto case 218;
				} else {
					if (la.kind == 58) {
						stateStack.Push(160);
						goto case 217;
					} else {
						Error(la);
						goto case 160;
					}
				}
			}
			case 160: {
				if (la == null) { currentState = 160; break; }
				if (set[36].Get(la.kind)) {
					stateStack.Push(160);
					goto case 161;
				} else {
					PopContext();
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 161: {
				if (la == null) { currentState = 161; break; }
				if (la.kind == 126) {
					currentState = 215;
					break;
				} else {
					if (la.kind == 58) {
						currentState = 211;
						break;
					} else {
						if (la.kind == 197) {
							currentState = 209;
							break;
						} else {
							if (la.kind == 107) {
								goto case 113;
							} else {
								if (la.kind == 230) {
									currentState = 56;
									break;
								} else {
									if (la.kind == 176) {
										currentState = 205;
										break;
									} else {
										if (la.kind == 203 || la.kind == 212) {
											currentState = 203;
											break;
										} else {
											if (la.kind == 148) {
												currentState = 201;
												break;
											} else {
												if (la.kind == 133) {
													currentState = 175;
													break;
												} else {
													if (la.kind == 146) {
														currentState = 162;
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
			case 162: {
				stateStack.Push(163);
				goto case 168;
			}
			case 163: {
				if (la == null) { currentState = 163; break; }
				Expect(171, la); // "On"
				currentState = 164;
				break;
			}
			case 164: {
				stateStack.Push(165);
				goto case 56;
			}
			case 165: {
				if (la == null) { currentState = 165; break; }
				Expect(116, la); // "Equals"
				currentState = 166;
				break;
			}
			case 166: {
				stateStack.Push(167);
				goto case 56;
			}
			case 167: {
				if (la == null) { currentState = 167; break; }
				if (la.kind == 22) {
					currentState = 164;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 168: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(169);
				goto case 174;
			}
			case 169: {
				PopContext();
				goto case 170;
			}
			case 170: {
				if (la == null) { currentState = 170; break; }
				if (la.kind == 63) {
					currentState = 172;
					break;
				} else {
					goto case 171;
				}
			}
			case 171: {
				if (la == null) { currentState = 171; break; }
				Expect(138, la); // "In"
				currentState = 56;
				break;
			}
			case 172: {
				PushContext(Context.Type, la, t);
				stateStack.Push(173);
				goto case 37;
			}
			case 173: {
				PopContext();
				goto case 171;
			}
			case 174: {
				if (la == null) { currentState = 174; break; }
				if (set[114].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 98) {
						goto case 115;
					} else {
						goto case 6;
					}
				}
			}
			case 175: {
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 176;
			}
			case 176: {
				if (la == null) { currentState = 176; break; }
				if (la.kind == 146) {
					goto case 193;
				} else {
					if (set[38].Get(la.kind)) {
						if (la.kind == 70) {
							currentState = 178;
							break;
						} else {
							if (set[38].Get(la.kind)) {
								goto case 191;
							} else {
								Error(la);
								goto case 177;
							}
						}
					} else {
						goto case 6;
					}
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
				goto case 182;
			}
			case 179: {
				if (la == null) { currentState = 179; break; }
				if (la.kind == 22) {
					currentState = 178;
					break;
				} else {
					Expect(143, la); // "Into"
					currentState = 180;
					break;
				}
			}
			case 180: {
				stateStack.Push(181);
				goto case 182;
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
				SetIdentifierExpected(la);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 183;
			}
			case 183: {
				if (la == null) { currentState = 183; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(184);
					goto case 174;
				} else {
					goto case 56;
				}
			}
			case 184: {
				PopContext();
				goto case 185;
			}
			case 185: {
				if (la == null) { currentState = 185; break; }
				if (la.kind == 63) {
					currentState = 188;
					break;
				} else {
					if (la.kind == 20) {
						goto case 187;
					} else {
						if (set[39].Get(la.kind)) {
							currentState = endOfStatementTerminatorAndBlock; /* leave this block */
								InformToken(t); /* process Identifier again*/
								/* for processing current token (la): go to the position after processing End */
								goto switchlbl;

						} else {
							Error(la);
							goto case 56;
						}
					}
				}
			}
			case 186: {
				if (la == null) { currentState = 186; break; }
				currentState = 56;
				break;
			}
			case 187: {
				if (la == null) { currentState = 187; break; }
				currentState = 56;
				break;
			}
			case 188: {
				PushContext(Context.Type, la, t);
				stateStack.Push(189);
				goto case 37;
			}
			case 189: {
				PopContext();
				goto case 190;
			}
			case 190: {
				if (la == null) { currentState = 190; break; }
				Expect(20, la); // "="
				currentState = 56;
				break;
			}
			case 191: {
				stateStack.Push(192);
				goto case 182;
			}
			case 192: {
				if (la == null) { currentState = 192; break; }
				if (la.kind == 22) {
					currentState = 191;
					break;
				} else {
					goto case 177;
				}
			}
			case 193: {
				stateStack.Push(194);
				goto case 200;
			}
			case 194: {
				if (la == null) { currentState = 194; break; }
				if (la.kind == 133 || la.kind == 146) {
					if (la.kind == 133) {
						currentState = 198;
						break;
					} else {
						if (la.kind == 146) {
							goto case 193;
						} else {
							Error(la);
							goto case 194;
						}
					}
				} else {
					goto case 195;
				}
			}
			case 195: {
				if (la == null) { currentState = 195; break; }
				Expect(143, la); // "Into"
				currentState = 196;
				break;
			}
			case 196: {
				stateStack.Push(197);
				goto case 182;
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
				stateStack.Push(199);
				goto case 200;
			}
			case 199: {
				stateStack.Push(194);
				goto case 195;
			}
			case 200: {
				if (la == null) { currentState = 200; break; }
				Expect(146, la); // "Join"
				currentState = 162;
				break;
			}
			case 201: {
				stateStack.Push(202);
				goto case 182;
			}
			case 202: {
				if (la == null) { currentState = 202; break; }
				if (la.kind == 22) {
					currentState = 201;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 203: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 204;
			}
			case 204: {
				if (la == null) { currentState = 204; break; }
				if (la.kind == 231) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 205: {
				if (la == null) { currentState = 205; break; }
				Expect(70, la); // "By"
				currentState = 206;
				break;
			}
			case 206: {
				stateStack.Push(207);
				goto case 56;
			}
			case 207: {
				if (la == null) { currentState = 207; break; }
				if (la.kind == 64) {
					currentState = 208;
					break;
				} else {
					if (la.kind == 104) {
						currentState = 208;
						break;
					} else {
						Error(la);
						goto case 208;
					}
				}
			}
			case 208: {
				if (la == null) { currentState = 208; break; }
				if (la.kind == 22) {
					currentState = 206;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 209: {
				stateStack.Push(210);
				goto case 182;
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
				goto case 168;
			}
			case 212: {
				if (la == null) { currentState = 212; break; }
				if (set[36].Get(la.kind)) {
					stateStack.Push(212);
					goto case 161;
				} else {
					Expect(143, la); // "Into"
					currentState = 213;
					break;
				}
			}
			case 213: {
				stateStack.Push(214);
				goto case 182;
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
				stateStack.Push(216);
				goto case 168;
			}
			case 216: {
				if (la == null) { currentState = 216; break; }
				if (la.kind == 22) {
					currentState = 215;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 217: {
				if (la == null) { currentState = 217; break; }
				Expect(58, la); // "Aggregate"
				currentState = 211;
				break;
			}
			case 218: {
				if (la == null) { currentState = 218; break; }
				Expect(126, la); // "From"
				currentState = 215;
				break;
			}
			case 219: {
				if (la == null) { currentState = 219; break; }
				if (la.kind == 210) {
					currentState = 390;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 220;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 220: {
				if (la == null) { currentState = 220; break; }
				Expect(37, la); // "("
				currentState = 221;
				break;
			}
			case 221: {
				SetIdentifierExpected(la);
				goto case 222;
			}
			case 222: {
				if (la == null) { currentState = 222; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(223);
					goto case 372;
				} else {
					goto case 223;
				}
			}
			case 223: {
				if (la == null) { currentState = 223; break; }
				Expect(38, la); // ")"
				currentState = 224;
				break;
			}
			case 224: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 225;
			}
			case 225: {
				if (la == null) { currentState = 225; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 1 || la.kind == 21 || la.kind == 63) {
						if (la.kind == 63) {
							currentState = 370;
							break;
						} else {
							goto case 226;
						}
					} else {
						goto case 6;
					}
				}
			}
			case 226: {
				stateStack.Push(227);
				goto case 229;
			}
			case 227: {
				if (la == null) { currentState = 227; break; }
				Expect(113, la); // "End"
				currentState = 228;
				break;
			}
			case 228: {
				if (la == null) { currentState = 228; break; }
				Expect(127, la); // "Function"
				currentState = stateStack.Pop();
				break;
			}
			case 229: {
				PushContext(Context.Body, la, t);
				goto case 230;
			}
			case 230: {
				stateStack.Push(231);
				goto case 23;
			}
			case 231: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 232;
			}
			case 232: {
				if (la == null) { currentState = 232; break; }
				if (set[130].Get(la.kind)) {
					if (set[65].Get(la.kind)) {
						if (set[47].Get(la.kind)) {
							stateStack.Push(230);
							goto case 237;
						} else {
							goto case 230;
						}
					} else {
						if (la.kind == 113) {
							currentState = 235;
							break;
						} else {
							goto case 234;
						}
					}
				} else {
					goto case 233;
				}
			}
			case 233: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 234: {
				Error(la);
				goto case 231;
			}
			case 235: {
				if (la == null) { currentState = 235; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 230;
				} else {
					if (set[46].Get(la.kind)) {
						currentState = endOfStatementTerminatorAndBlock; /* leave this block */
						InformToken(t); /* process End again*/
						/* for processing current token (la): go to the position after processing End */
						goto switchlbl;

					} else {
						goto case 234;
					}
				}
			}
			case 236: {
				if (la == null) { currentState = 236; break; }
				currentState = 231;
				break;
			}
			case 237: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 238;
			}
			case 238: {
				if (la == null) { currentState = 238; break; }
				if (la.kind == 88 || la.kind == 105 || la.kind == 204) {
					currentState = 354;
					break;
				} else {
					if (la.kind == 211 || la.kind == 233) {
						currentState = 350;
						break;
					} else {
						if (la.kind == 56 || la.kind == 193) {
							currentState = 348;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 346;
								break;
							} else {
								if (la.kind == 135) {
									currentState = 328;
									break;
								} else {
									if (la.kind == 197) {
										currentState = 313;
										break;
									} else {
										if (la.kind == 231) {
											currentState = 309;
											break;
										} else {
											if (la.kind == 108) {
												currentState = 303;
												break;
											} else {
												if (la.kind == 124) {
													currentState = 276;
													break;
												} else {
													if (la.kind == 118 || la.kind == 171 || la.kind == 194) {
														if (la.kind == 118 || la.kind == 171) {
															if (la.kind == 171) {
																currentState = 272;
																break;
															} else {
																goto case 272;
															}
														} else {
															if (la.kind == 194) {
																currentState = 270;
																break;
															} else {
																goto case 6;
															}
														}
													} else {
														if (la.kind == 215) {
															currentState = 268;
															break;
														} else {
															if (la.kind == 218) {
																currentState = 255;
																break;
															} else {
																if (set[131].Get(la.kind)) {
																	if (la.kind == 132) {
																		currentState = 252;
																		break;
																	} else {
																		if (la.kind == 120) {
																			currentState = 251;
																			break;
																		} else {
																			if (la.kind == 89) {
																				currentState = 250;
																				break;
																			} else {
																				if (la.kind == 206) {
																					goto case 93;
																				} else {
																					if (la.kind == 195) {
																						currentState = 247;
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
																		currentState = 245;
																		break;
																	} else {
																		if (la.kind == 117) {
																			currentState = 243;
																			break;
																		} else {
																			if (la.kind == 226) {
																				currentState = 239;
																				break;
																			} else {
																				if (set[132].Get(la.kind)) {
																					if (la.kind == 73) {
																						currentState = 56;
																						break;
																					} else {
																						goto case 56;
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
			case 239: {
				stateStack.Push(240);
				goto case 56;
			}
			case 240: {
				stateStack.Push(241);
				goto case 229;
			}
			case 241: {
				if (la == null) { currentState = 241; break; }
				Expect(113, la); // "End"
				currentState = 242;
				break;
			}
			case 242: {
				if (la == null) { currentState = 242; break; }
				Expect(226, la); // "Using"
				currentState = stateStack.Pop();
				break;
			}
			case 243: {
				stateStack.Push(244);
				goto case 56;
			}
			case 244: {
				if (la == null) { currentState = 244; break; }
				if (la.kind == 22) {
					currentState = 243;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 245: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 246;
			}
			case 246: {
				if (la == null) { currentState = 246; break; }
				if (la.kind == 184) {
					currentState = 56;
					break;
				} else {
					goto case 56;
				}
			}
			case 247: {
				PushContext(Context.Expression, la, t);
				nextTokenIsPotentialStartOfExpression = true;
				goto case 248;
			}
			case 248: {
				if (la == null) { currentState = 248; break; }
				if (set[23].Get(la.kind)) {
					stateStack.Push(249);
					goto case 56;
				} else {
					goto case 249;
				}
			}
			case 249: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 250: {
				if (la == null) { currentState = 250; break; }
				if (la.kind == 108) {
					goto case 112;
				} else {
					if (la.kind == 124) {
						goto case 109;
					} else {
						if (la.kind == 231) {
							goto case 83;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 251: {
				if (la == null) { currentState = 251; break; }
				if (la.kind == 108) {
					goto case 112;
				} else {
					if (la.kind == 124) {
						goto case 109;
					} else {
						if (la.kind == 231) {
							goto case 83;
						} else {
							if (la.kind == 197) {
								goto case 95;
							} else {
								if (la.kind == 210) {
									goto case 91;
								} else {
									if (la.kind == 127) {
										goto case 107;
									} else {
										if (la.kind == 186) {
											goto case 96;
										} else {
											if (la.kind == 218) {
												goto case 87;
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
			case 252: {
				if (la == null) { currentState = 252; break; }
				if (set[6].Get(la.kind)) {
					goto case 254;
				} else {
					if (la.kind == 5) {
						goto case 253;
					} else {
						goto case 6;
					}
				}
			}
			case 253: {
				if (la == null) { currentState = 253; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 254: {
				if (la == null) { currentState = 254; break; }
				currentState = stateStack.Pop();
				break;
			}
			case 255: {
				stateStack.Push(256);
				goto case 229;
			}
			case 256: {
				if (la == null) { currentState = 256; break; }
				if (la.kind == 75) {
					currentState = 260;
					break;
				} else {
					if (la.kind == 123) {
						currentState = 259;
						break;
					} else {
						goto case 257;
					}
				}
			}
			case 257: {
				if (la == null) { currentState = 257; break; }
				Expect(113, la); // "End"
				currentState = 258;
				break;
			}
			case 258: {
				if (la == null) { currentState = 258; break; }
				Expect(218, la); // "Try"
				currentState = stateStack.Pop();
				break;
			}
			case 259: {
				stateStack.Push(257);
				goto case 229;
			}
			case 260: {
				SetIdentifierExpected(la);
				goto case 261;
			}
			case 261: {
				if (la == null) { currentState = 261; break; }
				if (set[6].Get(la.kind)) {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(264);
					goto case 174;
				} else {
					goto case 262;
				}
			}
			case 262: {
				if (la == null) { currentState = 262; break; }
				if (la.kind == 229) {
					currentState = 263;
					break;
				} else {
					goto case 255;
				}
			}
			case 263: {
				stateStack.Push(255);
				goto case 56;
			}
			case 264: {
				PopContext();
				goto case 265;
			}
			case 265: {
				if (la == null) { currentState = 265; break; }
				if (la.kind == 63) {
					currentState = 266;
					break;
				} else {
					goto case 262;
				}
			}
			case 266: {
				PushContext(Context.Type, la, t);
				stateStack.Push(267);
				goto case 37;
			}
			case 267: {
				PopContext();
				goto case 262;
			}
			case 268: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 269;
			}
			case 269: {
				if (la == null) { currentState = 269; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 270: {
				if (la == null) { currentState = 270; break; }
				if (la.kind == 163) {
					goto case 100;
				} else {
					goto case 271;
				}
			}
			case 271: {
				if (la == null) { currentState = 271; break; }
				if (la.kind == 5) {
					goto case 253;
				} else {
					if (set[6].Get(la.kind)) {
						goto case 254;
					} else {
						goto case 6;
					}
				}
			}
			case 272: {
				if (la == null) { currentState = 272; break; }
				Expect(118, la); // "Error"
				currentState = 273;
				break;
			}
			case 273: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 274;
			}
			case 274: {
				if (la == null) { currentState = 274; break; }
				if (set[23].Get(la.kind)) {
					goto case 56;
				} else {
					if (la.kind == 132) {
						currentState = 271;
						break;
					} else {
						if (la.kind == 194) {
							currentState = 275;
							break;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 275: {
				if (la == null) { currentState = 275; break; }
				Expect(163, la); // "Next"
				currentState = stateStack.Pop();
				break;
			}
			case 276: {
				nextTokenIsPotentialStartOfExpression = true;
				SetIdentifierExpected(la);
				goto case 277;
			}
			case 277: {
				if (la == null) { currentState = 277; break; }
				if (set[33].Get(la.kind)) {
					stateStack.Push(293);
					goto case 287;
				} else {
					if (la.kind == 110) {
						currentState = 278;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 278: {
				stateStack.Push(279);
				goto case 287;
			}
			case 279: {
				if (la == null) { currentState = 279; break; }
				Expect(138, la); // "In"
				currentState = 280;
				break;
			}
			case 280: {
				stateStack.Push(281);
				goto case 56;
			}
			case 281: {
				stateStack.Push(282);
				goto case 229;
			}
			case 282: {
				if (la == null) { currentState = 282; break; }
				Expect(163, la); // "Next"
				currentState = 283;
				break;
			}
			case 283: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 284;
			}
			case 284: {
				if (la == null) { currentState = 284; break; }
				if (set[23].Get(la.kind)) {
					goto case 285;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 285: {
				stateStack.Push(286);
				goto case 56;
			}
			case 286: {
				if (la == null) { currentState = 286; break; }
				if (la.kind == 22) {
					currentState = 285;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 287: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(288);
				goto case 143;
			}
			case 288: {
				PopContext();
				goto case 289;
			}
			case 289: {
				if (la == null) { currentState = 289; break; }
				if (la.kind == 33) {
					currentState = 290;
					break;
				} else {
					goto case 290;
				}
			}
			case 290: {
				if (la == null) { currentState = 290; break; }
				if (set[30].Get(la.kind)) {
					stateStack.Push(290);
					goto case 133;
				} else {
					if (la.kind == 63) {
						currentState = 291;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 291: {
				PushContext(Context.Type, la, t);
				stateStack.Push(292);
				goto case 37;
			}
			case 292: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 293: {
				if (la == null) { currentState = 293; break; }
				Expect(20, la); // "="
				currentState = 294;
				break;
			}
			case 294: {
				stateStack.Push(295);
				goto case 56;
			}
			case 295: {
				if (la == null) { currentState = 295; break; }
				if (la.kind == 205) {
					currentState = 302;
					break;
				} else {
					goto case 296;
				}
			}
			case 296: {
				stateStack.Push(297);
				goto case 229;
			}
			case 297: {
				if (la == null) { currentState = 297; break; }
				Expect(163, la); // "Next"
				currentState = 298;
				break;
			}
			case 298: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 299;
			}
			case 299: {
				if (la == null) { currentState = 299; break; }
				if (set[23].Get(la.kind)) {
					goto case 300;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 300: {
				stateStack.Push(301);
				goto case 56;
			}
			case 301: {
				if (la == null) { currentState = 301; break; }
				if (la.kind == 22) {
					currentState = 300;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 302: {
				stateStack.Push(296);
				goto case 56;
			}
			case 303: {
				if (la == null) { currentState = 303; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 306;
					break;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(304);
						goto case 229;
					} else {
						goto case 6;
					}
				}
			}
			case 304: {
				if (la == null) { currentState = 304; break; }
				Expect(152, la); // "Loop"
				currentState = 305;
				break;
			}
			case 305: {
				if (la == null) { currentState = 305; break; }
				if (la.kind == 224 || la.kind == 231) {
					currentState = 56;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 306: {
				stateStack.Push(307);
				goto case 56;
			}
			case 307: {
				stateStack.Push(308);
				goto case 229;
			}
			case 308: {
				if (la == null) { currentState = 308; break; }
				Expect(152, la); // "Loop"
				currentState = stateStack.Pop();
				break;
			}
			case 309: {
				stateStack.Push(310);
				goto case 56;
			}
			case 310: {
				stateStack.Push(311);
				goto case 229;
			}
			case 311: {
				if (la == null) { currentState = 311; break; }
				Expect(113, la); // "End"
				currentState = 312;
				break;
			}
			case 312: {
				if (la == null) { currentState = 312; break; }
				Expect(231, la); // "While"
				currentState = stateStack.Pop();
				break;
			}
			case 313: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 314;
			}
			case 314: {
				if (la == null) { currentState = 314; break; }
				if (la.kind == 74) {
					currentState = 315;
					break;
				} else {
					goto case 315;
				}
			}
			case 315: {
				stateStack.Push(316);
				goto case 56;
			}
			case 316: {
				stateStack.Push(317);
				goto case 23;
			}
			case 317: {
				if (la == null) { currentState = 317; break; }
				if (la.kind == 74) {
					currentState = 319;
					break;
				} else {
					Expect(113, la); // "End"
					currentState = 318;
					break;
				}
			}
			case 318: {
				if (la == null) { currentState = 318; break; }
				Expect(197, la); // "Select"
				currentState = stateStack.Pop();
				break;
			}
			case 319: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 320;
			}
			case 320: {
				if (la == null) { currentState = 320; break; }
				if (la.kind == 111) {
					currentState = 321;
					break;
				} else {
					if (set[63].Get(la.kind)) {
						goto case 322;
					} else {
						Error(la);
						goto case 321;
					}
				}
			}
			case 321: {
				stateStack.Push(317);
				goto case 229;
			}
			case 322: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 323;
			}
			case 323: {
				if (la == null) { currentState = 323; break; }
				if (set[133].Get(la.kind)) {
					if (la.kind == 144) {
						currentState = 325;
						break;
					} else {
						goto case 325;
					}
				} else {
					if (set[23].Get(la.kind)) {
						stateStack.Push(324);
						goto case 56;
					} else {
						Error(la);
						goto case 324;
					}
				}
			}
			case 324: {
				if (la == null) { currentState = 324; break; }
				if (la.kind == 22) {
					currentState = 322;
					break;
				} else {
					goto case 321;
				}
			}
			case 325: {
				stateStack.Push(326);
				goto case 327;
			}
			case 326: {
				stateStack.Push(324);
				goto case 59;
			}
			case 327: {
				if (la == null) { currentState = 327; break; }
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
			case 328: {
				stateStack.Push(329);
				goto case 56;
			}
			case 329: {
				if (la == null) { currentState = 329; break; }
				if (la.kind == 214) {
					currentState = 337;
					break;
				} else {
					goto case 330;
				}
			}
			case 330: {
				if (la == null) { currentState = 330; break; }
				if (la.kind == 1 || la.kind == 21) {
					goto case 331;
				} else {
					goto case 6;
				}
			}
			case 331: {
				stateStack.Push(332);
				goto case 229;
			}
			case 332: {
				if (la == null) { currentState = 332; break; }
				if (la.kind == 111 || la.kind == 112) {
					if (la.kind == 111) {
						currentState = 336;
						break;
					} else {
						if (la.kind == 112) {
							currentState = 334;
							break;
						} else {
							Error(la);
							goto case 331;
						}
					}
				} else {
					Expect(113, la); // "End"
					currentState = 333;
					break;
				}
			}
			case 333: {
				if (la == null) { currentState = 333; break; }
				Expect(135, la); // "If"
				currentState = stateStack.Pop();
				break;
			}
			case 334: {
				stateStack.Push(335);
				goto case 56;
			}
			case 335: {
				if (la == null) { currentState = 335; break; }
				if (la.kind == 214) {
					currentState = 331;
					break;
				} else {
					goto case 331;
				}
			}
			case 336: {
				if (la == null) { currentState = 336; break; }
				if (la.kind == 135) {
					currentState = 334;
					break;
				} else {
					goto case 331;
				}
			}
			case 337: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 338;
			}
			case 338: {
				if (la == null) { currentState = 338; break; }
				if (set[47].Get(la.kind)) {
					goto case 339;
				} else {
					goto case 330;
				}
			}
			case 339: {
				stateStack.Push(340);
				goto case 237;
			}
			case 340: {
				if (la == null) { currentState = 340; break; }
				if (la.kind == 21) {
					currentState = 344;
					break;
				} else {
					if (la.kind == 111) {
						currentState = 341;
						break;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 341: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 342;
			}
			case 342: {
				if (la == null) { currentState = 342; break; }
				if (set[47].Get(la.kind)) {
					stateStack.Push(343);
					goto case 237;
				} else {
					goto case 343;
				}
			}
			case 343: {
				if (la == null) { currentState = 343; break; }
				if (la.kind == 21) {
					currentState = 341;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 344: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 345;
			}
			case 345: {
				if (la == null) { currentState = 345; break; }
				if (set[47].Get(la.kind)) {
					goto case 339;
				} else {
					goto case 340;
				}
			}
			case 346: {
				stateStack.Push(347);
				goto case 81;
			}
			case 347: {
				if (la == null) { currentState = 347; break; }
				if (la.kind == 37) {
					currentState = 46;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 348: {
				stateStack.Push(349);
				goto case 56;
			}
			case 349: {
				if (la == null) { currentState = 349; break; }
				Expect(22, la); // ","
				currentState = 56;
				break;
			}
			case 350: {
				stateStack.Push(351);
				goto case 56;
			}
			case 351: {
				stateStack.Push(352);
				goto case 229;
			}
			case 352: {
				if (la == null) { currentState = 352; break; }
				Expect(113, la); // "End"
				currentState = 353;
				break;
			}
			case 353: {
				if (la == null) { currentState = 353; break; }
				if (la.kind == 233) {
					goto case 82;
				} else {
					if (la.kind == 211) {
						goto case 90;
					} else {
						goto case 6;
					}
				}
			}
			case 354: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(355);
				goto case 174;
			}
			case 355: {
				PopContext();
				goto case 356;
			}
			case 356: {
				if (la == null) { currentState = 356; break; }
				if (la.kind == 33) {
					currentState = 357;
					break;
				} else {
					goto case 357;
				}
			}
			case 357: {
				if (la == null) { currentState = 357; break; }
				if (la.kind == 37) {
					currentState = 369;
					break;
				} else {
					goto case 358;
				}
			}
			case 358: {
				if (la == null) { currentState = 358; break; }
				if (la.kind == 22) {
					currentState = 363;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 360;
						break;
					} else {
						goto case 359;
					}
				}
			}
			case 359: {
				if (la == null) { currentState = 359; break; }
				if (la.kind == 20) {
					goto case 187;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 360: {
				PushContext(Context.Type, la, t);
				goto case 361;
			}
			case 361: {
				if (la == null) { currentState = 361; break; }
				if (la.kind == 162) {
					stateStack.Push(362);
					goto case 67;
				} else {
					if (set[16].Get(la.kind)) {
						stateStack.Push(362);
						goto case 37;
					} else {
						Error(la);
						goto case 362;
					}
				}
			}
			case 362: {
				PopContext();
				goto case 359;
			}
			case 363: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(364);
				goto case 174;
			}
			case 364: {
				PopContext();
				goto case 365;
			}
			case 365: {
				if (la == null) { currentState = 365; break; }
				if (la.kind == 33) {
					currentState = 366;
					break;
				} else {
					goto case 366;
				}
			}
			case 366: {
				if (la == null) { currentState = 366; break; }
				if (la.kind == 37) {
					currentState = 367;
					break;
				} else {
					goto case 358;
				}
			}
			case 367: {
				if (la == null) { currentState = 367; break; }
				if (la.kind == 22) {
					currentState = 367;
					break;
				} else {
					goto case 368;
				}
			}
			case 368: {
				if (la == null) { currentState = 368; break; }
				Expect(38, la); // ")"
				currentState = 358;
				break;
			}
			case 369: {
				if (la == null) { currentState = 369; break; }
				if (la.kind == 22) {
					currentState = 369;
					break;
				} else {
					goto case 368;
				}
			}
			case 370: {
				PushContext(Context.Type, la, t);
				stateStack.Push(371);
				goto case 37;
			}
			case 371: {
				PopContext();
				goto case 226;
			}
			case 372: {
				stateStack.Push(373);
				PushContext(Context.Parameter, la, t);
				goto case 374;
			}
			case 373: {
				if (la == null) { currentState = 373; break; }
				if (la.kind == 22) {
					currentState = 372;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 374: {
				SetIdentifierExpected(la);
				goto case 375;
			}
			case 375: {
				if (la == null) { currentState = 375; break; }
				if (la.kind == 40) {
					stateStack.Push(374);
					goto case 385;
				} else {
					goto case 376;
				}
			}
			case 376: {
				SetIdentifierExpected(la);
				goto case 377;
			}
			case 377: {
				if (la == null) { currentState = 377; break; }
				if (set[134].Get(la.kind)) {
					currentState = 376;
					break;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(378);
					goto case 174;
				}
			}
			case 378: {
				PopContext();
				goto case 379;
			}
			case 379: {
				if (la == null) { currentState = 379; break; }
				if (la.kind == 63) {
					currentState = 383;
					break;
				} else {
					goto case 380;
				}
			}
			case 380: {
				if (la == null) { currentState = 380; break; }
				if (la.kind == 20) {
					currentState = 382;
					break;
				} else {
					goto case 381;
				}
			}
			case 381: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 382: {
				stateStack.Push(381);
				goto case 56;
			}
			case 383: {
				PushContext(Context.Type, la, t);
				stateStack.Push(384);
				goto case 37;
			}
			case 384: {
				PopContext();
				goto case 380;
			}
			case 385: {
				if (la == null) { currentState = 385; break; }
				Expect(40, la); // "<"
				currentState = 386;
				break;
			}
			case 386: {
				PushContext(Context.Attribute, la, t);
				goto case 387;
			}
			case 387: {
				if (la == null) { currentState = 387; break; }
				if (set[135].Get(la.kind)) {
					currentState = 387;
					break;
				} else {
					Expect(39, la); // ">"
					currentState = 388;
					break;
				}
			}
			case 388: {
				PopContext();
				goto case 389;
			}
			case 389: {
				if (la == null) { currentState = 389; break; }
				if (la.kind == 1) {
					goto case 25;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 390: {
				if (la == null) { currentState = 390; break; }
				Expect(37, la); // "("
				currentState = 391;
				break;
			}
			case 391: {
				SetIdentifierExpected(la);
				goto case 392;
			}
			case 392: {
				if (la == null) { currentState = 392; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(393);
					goto case 372;
				} else {
					goto case 393;
				}
			}
			case 393: {
				if (la == null) { currentState = 393; break; }
				Expect(38, la); // ")"
				currentState = 394;
				break;
			}
			case 394: {
				nextTokenIsPotentialStartOfExpression = true;
				goto case 395;
			}
			case 395: {
				if (la == null) { currentState = 395; break; }
				if (set[47].Get(la.kind)) {
					goto case 237;
				} else {
					if (la.kind == 1 || la.kind == 21) {
						stateStack.Push(396);
						goto case 229;
					} else {
						goto case 6;
					}
				}
			}
			case 396: {
				if (la == null) { currentState = 396; break; }
				Expect(113, la); // "End"
				currentState = 397;
				break;
			}
			case 397: {
				if (la == null) { currentState = 397; break; }
				Expect(210, la); // "Sub"
				currentState = stateStack.Pop();
				break;
			}
			case 398: {
				if (la == null) { currentState = 398; break; }
				if (la.kind == 17 || la.kind == 18 || la.kind == 19) {
					currentState = 411;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(400);
						goto case 402;
					} else {
						Error(la);
						goto case 399;
					}
				}
			}
			case 399: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 400: {
				if (la == null) { currentState = 400; break; }
				if (la.kind == 17) {
					currentState = 401;
					break;
				} else {
					goto case 399;
				}
			}
			case 401: {
				if (la == null) { currentState = 401; break; }
				if (la.kind == 16) {
					currentState = 400;
					break;
				} else {
					goto case 400;
				}
			}
			case 402: {
				PushContext(Context.Xml, la, t);
				goto case 403;
			}
			case 403: {
				if (la == null) { currentState = 403; break; }
				Expect(10, la); // XmlOpenTag
				currentState = 404;
				break;
			}
			case 404: {
				if (la == null) { currentState = 404; break; }
				if (set[136].Get(la.kind)) {
					if (set[137].Get(la.kind)) {
						currentState = 404;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(404);
							goto case 408;
						} else {
							Error(la);
							goto case 404;
						}
					}
				} else {
					if (la.kind == 14) {
						currentState = 405;
						break;
					} else {
						if (la.kind == 11) {
							currentState = 406;
							break;
						} else {
							Error(la);
							goto case 405;
						}
					}
				}
			}
			case 405: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 406: {
				if (la == null) { currentState = 406; break; }
				if (set[138].Get(la.kind)) {
					if (set[139].Get(la.kind)) {
						currentState = 406;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(406);
							goto case 408;
						} else {
							if (la.kind == 10) {
								stateStack.Push(406);
								goto case 402;
							} else {
								Error(la);
								goto case 406;
							}
						}
					}
				} else {
					Expect(15, la); // XmlOpenEndTag
					currentState = 407;
					break;
				}
			}
			case 407: {
				if (la == null) { currentState = 407; break; }
				if (set[140].Get(la.kind)) {
					if (set[141].Get(la.kind)) {
						currentState = 407;
						break;
					} else {
						if (la.kind == 12) {
							stateStack.Push(407);
							goto case 408;
						} else {
							Error(la);
							goto case 407;
						}
					}
				} else {
					Expect(11, la); // XmlCloseTag
					currentState = 405;
					break;
				}
			}
			case 408: {
				if (la == null) { currentState = 408; break; }
				Expect(12, la); // XmlStartInlineVB
				currentState = 409;
				break;
			}
			case 409: {
				stateStack.Push(410);
				goto case 56;
			}
			case 410: {
				if (la == null) { currentState = 410; break; }
				Expect(13, la); // XmlEndInlineVB
				currentState = stateStack.Pop();
				break;
			}
			case 411: {
				if (la == null) { currentState = 411; break; }
				if (la.kind == 16) {
					currentState = 412;
					break;
				} else {
					goto case 412;
				}
			}
			case 412: {
				if (la == null) { currentState = 412; break; }
				if (la.kind == 17 || la.kind == 19) {
					currentState = 411;
					break;
				} else {
					if (la.kind == 10) {
						stateStack.Push(413);
						goto case 402;
					} else {
						goto case 399;
					}
				}
			}
			case 413: {
				if (la == null) { currentState = 413; break; }
				if (la.kind == 17) {
					currentState = 414;
					break;
				} else {
					goto case 399;
				}
			}
			case 414: {
				if (la == null) { currentState = 414; break; }
				if (la.kind == 16) {
					currentState = 413;
					break;
				} else {
					goto case 413;
				}
			}
			case 415: {
				if (la == null) { currentState = 415; break; }
				Expect(37, la); // "("
				currentState = 416;
				break;
			}
			case 416: {
				readXmlIdentifier = true;
				stateStack.Push(417);
				goto case 174;
			}
			case 417: {
				if (la == null) { currentState = 417; break; }
				Expect(38, la); // ")"
				currentState = 145;
				break;
			}
			case 418: {
				if (la == null) { currentState = 418; break; }
				Expect(37, la); // "("
				currentState = 419;
				break;
			}
			case 419: {
				stateStack.Push(417);
				goto case 37;
			}
			case 420: {
				nextTokenIsStartOfImportsOrAccessExpression = true; wasQualifierTokenAtStart = true;
				goto case 421;
			}
			case 421: {
				if (la == null) { currentState = 421; break; }
				if (la.kind == 10) {
					currentState = 422;
					break;
				} else {
					goto case 422;
				}
			}
			case 422: {
				stateStack.Push(423);
				goto case 81;
			}
			case 423: {
				if (la == null) { currentState = 423; break; }
				if (la.kind == 11) {
					currentState = 145;
					break;
				} else {
					goto case 145;
				}
			}
			case 424: {
				stateStack.Push(417);
				goto case 56;
			}
			case 425: {
				stateStack.Push(426);
				goto case 56;
			}
			case 426: {
				if (la == null) { currentState = 426; break; }
				if (la.kind == 22) {
					currentState = 427;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 427: {
				activeArgument++;
				nextTokenIsPotentialStartOfExpression = true;
				goto case 428;
			}
			case 428: {
				if (la == null) { currentState = 428; break; }
				if (set[23].Get(la.kind)) {
					goto case 425;
				} else {
					goto case 426;
				}
			}
			case 429: {
				if (la == null) { currentState = 429; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(433);
					goto case 37;
				} else {
					goto case 430;
				}
			}
			case 430: {
				if (la == null) { currentState = 430; break; }
				if (la.kind == 22) {
					currentState = 431;
					break;
				} else {
					goto case 45;
				}
			}
			case 431: {
				if (la == null) { currentState = 431; break; }
				if (set[16].Get(la.kind)) {
					PushContext(Context.Type, la, t);
					stateStack.Push(432);
					goto case 37;
				} else {
					goto case 430;
				}
			}
			case 432: {
				PopContext();
				goto case 430;
			}
			case 433: {
				PopContext();
				goto case 430;
			}
			case 434: {
				SetIdentifierExpected(la);
				goto case 435;
			}
			case 435: {
				if (la == null) { currentState = 435; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 437;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(436);
							goto case 372;
						} else {
							Error(la);
							goto case 436;
						}
					}
				} else {
					goto case 436;
				}
			}
			case 436: {
				if (la == null) { currentState = 436; break; }
				Expect(38, la); // ")"
				currentState = 34;
				break;
			}
			case 437: {
				stateStack.Push(436);
				goto case 438;
			}
			case 438: {
				SetIdentifierExpected(la);
				goto case 439;
			}
			case 439: {
				if (la == null) { currentState = 439; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 440;
					break;
				} else {
					goto case 440;
				}
			}
			case 440: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(441);
				goto case 455;
			}
			case 441: {
				PopContext();
				goto case 442;
			}
			case 442: {
				if (la == null) { currentState = 442; break; }
				if (la.kind == 63) {
					currentState = 456;
					break;
				} else {
					goto case 443;
				}
			}
			case 443: {
				if (la == null) { currentState = 443; break; }
				if (la.kind == 22) {
					currentState = 444;
					break;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 444: {
				SetIdentifierExpected(la);
				goto case 445;
			}
			case 445: {
				if (la == null) { currentState = 445; break; }
				if (la.kind == 138 || la.kind == 178) {
					currentState = 446;
					break;
				} else {
					goto case 446;
				}
			}
			case 446: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(447);
				goto case 455;
			}
			case 447: {
				PopContext();
				goto case 448;
			}
			case 448: {
				if (la == null) { currentState = 448; break; }
				if (la.kind == 63) {
					currentState = 449;
					break;
				} else {
					goto case 443;
				}
			}
			case 449: {
				PushContext(Context.Type, la, t);
				stateStack.Push(450);
				goto case 451;
			}
			case 450: {
				PopContext();
				goto case 443;
			}
			case 451: {
				if (la == null) { currentState = 451; break; }
				if (set[88].Get(la.kind)) {
					goto case 454;
				} else {
					if (la.kind == 35) {
						currentState = 452;
						break;
					} else {
						goto case 6;
					}
				}
			}
			case 452: {
				stateStack.Push(453);
				goto case 454;
			}
			case 453: {
				if (la == null) { currentState = 453; break; }
				if (la.kind == 22) {
					currentState = 452;
					break;
				} else {
					goto case 66;
				}
			}
			case 454: {
				if (la == null) { currentState = 454; break; }
				if (set[16].Get(la.kind)) {
					currentState = 38;
					break;
				} else {
					if (la.kind == 162) {
						goto case 101;
					} else {
						if (la.kind == 84) {
							goto case 117;
						} else {
							if (la.kind == 209) {
								goto case 92;
							} else {
								goto case 6;
							}
						}
					}
				}
			}
			case 455: {
				if (la == null) { currentState = 455; break; }
				if (la.kind == 2) {
					goto case 125;
				} else {
					if (la.kind == 62) {
						goto case 123;
					} else {
						if (la.kind == 64) {
							goto case 122;
						} else {
							if (la.kind == 65) {
								goto case 121;
							} else {
								if (la.kind == 66) {
									goto case 120;
								} else {
									if (la.kind == 67) {
										goto case 119;
									} else {
										if (la.kind == 70) {
											goto case 118;
										} else {
											if (la.kind == 87) {
												goto case 116;
											} else {
												if (la.kind == 104) {
													goto case 114;
												} else {
													if (la.kind == 107) {
														goto case 113;
													} else {
														if (la.kind == 116) {
															goto case 111;
														} else {
															if (la.kind == 121) {
																goto case 110;
															} else {
																if (la.kind == 133) {
																	goto case 106;
																} else {
																	if (la.kind == 139) {
																		goto case 105;
																	} else {
																		if (la.kind == 143) {
																			goto case 104;
																		} else {
																			if (la.kind == 146) {
																				goto case 103;
																			} else {
																				if (la.kind == 147) {
																					goto case 102;
																				} else {
																					if (la.kind == 170) {
																						goto case 99;
																					} else {
																						if (la.kind == 176) {
																							goto case 98;
																						} else {
																							if (la.kind == 184) {
																								goto case 97;
																							} else {
																								if (la.kind == 203) {
																									goto case 94;
																								} else {
																									if (la.kind == 212) {
																										goto case 89;
																									} else {
																										if (la.kind == 213) {
																											goto case 88;
																										} else {
																											if (la.kind == 223) {
																												goto case 86;
																											} else {
																												if (la.kind == 224) {
																													goto case 85;
																												} else {
																													if (la.kind == 230) {
																														goto case 84;
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
			case 456: {
				PushContext(Context.Type, la, t);
				stateStack.Push(457);
				goto case 451;
			}
			case 457: {
				PopContext();
				goto case 443;
			}
			case 458: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(459);
				goto case 174;
			}
			case 459: {
				PopContext();
				goto case 460;
			}
			case 460: {
				if (la == null) { currentState = 460; break; }
				if (la.kind == 37) {
					currentState = 465;
					break;
				} else {
					goto case 461;
				}
			}
			case 461: {
				if (la == null) { currentState = 461; break; }
				if (la.kind == 63) {
					currentState = 462;
					break;
				} else {
					goto case 23;
				}
			}
			case 462: {
				PushContext(Context.Type, la, t);
				goto case 463;
			}
			case 463: {
				if (la == null) { currentState = 463; break; }
				if (la.kind == 40) {
					stateStack.Push(463);
					goto case 385;
				} else {
					stateStack.Push(464);
					goto case 37;
				}
			}
			case 464: {
				PopContext();
				goto case 23;
			}
			case 465: {
				SetIdentifierExpected(la);
				goto case 466;
			}
			case 466: {
				if (la == null) { currentState = 466; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(467);
					goto case 372;
				} else {
					goto case 467;
				}
			}
			case 467: {
				if (la == null) { currentState = 467; break; }
				Expect(38, la); // ")"
				currentState = 461;
				break;
			}
			case 468: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(469);
				goto case 174;
			}
			case 469: {
				PopContext();
				goto case 470;
			}
			case 470: {
				if (la == null) { currentState = 470; break; }
				if (la.kind == 37 || la.kind == 63) {
					if (la.kind == 63) {
						currentState = 475;
						break;
					} else {
						if (la.kind == 37) {
							currentState = 472;
							break;
						} else {
							goto case 471;
						}
					}
				} else {
					goto case 23;
				}
			}
			case 471: {
				Error(la);
				goto case 23;
			}
			case 472: {
				SetIdentifierExpected(la);
				goto case 473;
			}
			case 473: {
				if (la == null) { currentState = 473; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(474);
					goto case 372;
				} else {
					goto case 474;
				}
			}
			case 474: {
				if (la == null) { currentState = 474; break; }
				Expect(38, la); // ")"
				currentState = 23;
				break;
			}
			case 475: {
				PushContext(Context.Type, la, t);
				stateStack.Push(476);
				goto case 37;
			}
			case 476: {
				PopContext();
				goto case 23;
			}
			case 477: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 478;
			}
			case 478: {
				if (la == null) { currentState = 478; break; }
				Expect(115, la); // "Enum"
				currentState = 479;
				break;
			}
			case 479: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(480);
				goto case 174;
			}
			case 480: {
				PopContext();
				goto case 481;
			}
			case 481: {
				if (la == null) { currentState = 481; break; }
				if (la.kind == 63) {
					currentState = 493;
					break;
				} else {
					goto case 482;
				}
			}
			case 482: {
				stateStack.Push(483);
				goto case 23;
			}
			case 483: {
				SetIdentifierExpected(la);
				goto case 484;
			}
			case 484: {
				if (la == null) { currentState = 484; break; }
				if (set[91].Get(la.kind)) {
					goto case 488;
				} else {
					Expect(113, la); // "End"
					currentState = 485;
					break;
				}
			}
			case 485: {
				if (la == null) { currentState = 485; break; }
				Expect(115, la); // "Enum"
				currentState = 486;
				break;
			}
			case 486: {
				stateStack.Push(487);
				goto case 23;
			}
			case 487: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 488: {
				SetIdentifierExpected(la);
				goto case 489;
			}
			case 489: {
				if (la == null) { currentState = 489; break; }
				if (la.kind == 40) {
					stateStack.Push(488);
					goto case 385;
				} else {
					PushContext(Context.Identifier, la, t);
					SetIdentifierExpected(la);
					stateStack.Push(490);
					goto case 174;
				}
			}
			case 490: {
				PopContext();
				goto case 491;
			}
			case 491: {
				if (la == null) { currentState = 491; break; }
				if (la.kind == 20) {
					currentState = 492;
					break;
				} else {
					goto case 482;
				}
			}
			case 492: {
				stateStack.Push(482);
				goto case 56;
			}
			case 493: {
				PushContext(Context.Type, la, t);
				stateStack.Push(494);
				goto case 37;
			}
			case 494: {
				PopContext();
				goto case 482;
			}
			case 495: {
				if (la == null) { currentState = 495; break; }
				Expect(103, la); // "Delegate"
				currentState = 496;
				break;
			}
			case 496: {
				if (la == null) { currentState = 496; break; }
				if (la.kind == 210) {
					currentState = 497;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 497;
						break;
					} else {
						Error(la);
						goto case 497;
					}
				}
			}
			case 497: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 498;
			}
			case 498: {
				if (la == null) { currentState = 498; break; }
				currentState = 499;
				break;
			}
			case 499: {
				PopContext();
				goto case 500;
			}
			case 500: {
				if (la == null) { currentState = 500; break; }
				if (la.kind == 37) {
					currentState = 503;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 501;
						break;
					} else {
						goto case 23;
					}
				}
			}
			case 501: {
				PushContext(Context.Type, la, t);
				stateStack.Push(502);
				goto case 37;
			}
			case 502: {
				PopContext();
				goto case 23;
			}
			case 503: {
				SetIdentifierExpected(la);
				goto case 504;
			}
			case 504: {
				if (la == null) { currentState = 504; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 506;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(505);
							goto case 372;
						} else {
							Error(la);
							goto case 505;
						}
					}
				} else {
					goto case 505;
				}
			}
			case 505: {
				if (la == null) { currentState = 505; break; }
				Expect(38, la); // ")"
				currentState = 500;
				break;
			}
			case 506: {
				stateStack.Push(505);
				goto case 438;
			}
			case 507: {
				PushContext(Context.TypeDeclaration, la, t);
				goto case 508;
			}
			case 508: {
				if (la == null) { currentState = 508; break; }
				if (la.kind == 155) {
					currentState = 509;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 509;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 509;
							break;
						} else {
							Error(la);
							goto case 509;
						}
					}
				}
			}
			case 509: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(510);
				goto case 174;
			}
			case 510: {
				PopContext();
				goto case 511;
			}
			case 511: {
				if (la == null) { currentState = 511; break; }
				if (la.kind == 37) {
					currentState = 667;
					break;
				} else {
					goto case 512;
				}
			}
			case 512: {
				stateStack.Push(513);
				goto case 23;
			}
			case 513: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 514;
			}
			case 514: {
				if (la == null) { currentState = 514; break; }
				if (la.kind == 140) {
					isMissingModifier = false;
					goto case 664;
				} else {
					goto case 515;
				}
			}
			case 515: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 516;
			}
			case 516: {
				if (la == null) { currentState = 516; break; }
				if (la.kind == 136) {
					isMissingModifier = false;
					goto case 658;
				} else {
					goto case 517;
				}
			}
			case 517: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 518;
			}
			case 518: {
				if (la == null) { currentState = 518; break; }
				if (set[95].Get(la.kind)) {
					goto case 523;
				} else {
					isMissingModifier = false;
					goto case 519;
				}
			}
			case 519: {
				if (la == null) { currentState = 519; break; }
				Expect(113, la); // "End"
				currentState = 520;
				break;
			}
			case 520: {
				if (la == null) { currentState = 520; break; }
				if (la.kind == 155) {
					currentState = 521;
					break;
				} else {
					if (la.kind == 84) {
						currentState = 521;
						break;
					} else {
						if (la.kind == 209) {
							currentState = 521;
							break;
						} else {
							Error(la);
							goto case 521;
						}
					}
				}
			}
			case 521: {
				stateStack.Push(522);
				goto case 23;
			}
			case 522: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 523: {
				SetIdentifierExpected(la);
				isMissingModifier = true;
				goto case 524;
			}
			case 524: {
				if (la == null) { currentState = 524; break; }
				if (la.kind == 40) {
					stateStack.Push(523);
					goto case 385;
				} else {
					isMissingModifier = true;
					goto case 525;
				}
			}
			case 525: {
				SetIdentifierExpected(la);
				goto case 526;
			}
			case 526: {
				if (la == null) { currentState = 526; break; }
				if (set[122].Get(la.kind)) {
					currentState = 657;
					break;
				} else {
					isMissingModifier = false;
					SetIdentifierExpected(la);
					goto case 527;
				}
			}
			case 527: {
				if (la == null) { currentState = 527; break; }
				if (la.kind == 84 || la.kind == 155 || la.kind == 209) {
					stateStack.Push(517);
					goto case 507;
				} else {
					if (la.kind == 103) {
						stateStack.Push(517);
						goto case 495;
					} else {
						if (la.kind == 115) {
							stateStack.Push(517);
							goto case 477;
						} else {
							if (la.kind == 142) {
								stateStack.Push(517);
								goto case 9;
							} else {
								if (set[98].Get(la.kind)) {
									stateStack.Push(517);
									PushContext(Context.Member, la, t);
									SetIdentifierExpected(la);
									goto case 528;
								} else {
									Error(la);
									goto case 517;
								}
							}
						}
					}
				}
			}
			case 528: {
				if (la == null) { currentState = 528; break; }
				if (set[113].Get(la.kind)) {
					stateStack.Push(529);
					goto case 645;
				} else {
					if (la.kind == 127 || la.kind == 210) {
						stateStack.Push(529);
						goto case 631;
					} else {
						if (la.kind == 101) {
							stateStack.Push(529);
							goto case 615;
						} else {
							if (la.kind == 119) {
								stateStack.Push(529);
								goto case 600;
							} else {
								if (la.kind == 98) {
									stateStack.Push(529);
									goto case 588;
								} else {
									if (la.kind == 186) {
										stateStack.Push(529);
										goto case 543;
									} else {
										if (la.kind == 172) {
											stateStack.Push(529);
											goto case 530;
										} else {
											Error(la);
											goto case 529;
										}
									}
								}
							}
						}
					}
				}
			}
			case 529: {
				PopContext();
				currentState = stateStack.Pop();
				goto switchlbl;
			}
			case 530: {
				if (la == null) { currentState = 530; break; }
				Expect(172, la); // "Operator"
				currentState = 531;
				break;
			}
			case 531: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 532;
			}
			case 532: {
				if (la == null) { currentState = 532; break; }
				currentState = 533;
				break;
			}
			case 533: {
				PopContext();
				goto case 534;
			}
			case 534: {
				if (la == null) { currentState = 534; break; }
				Expect(37, la); // "("
				currentState = 535;
				break;
			}
			case 535: {
				stateStack.Push(536);
				goto case 372;
			}
			case 536: {
				if (la == null) { currentState = 536; break; }
				Expect(38, la); // ")"
				currentState = 537;
				break;
			}
			case 537: {
				if (la == null) { currentState = 537; break; }
				if (la.kind == 63) {
					currentState = 541;
					break;
				} else {
					goto case 538;
				}
			}
			case 538: {
				stateStack.Push(539);
				goto case 229;
			}
			case 539: {
				if (la == null) { currentState = 539; break; }
				Expect(113, la); // "End"
				currentState = 540;
				break;
			}
			case 540: {
				if (la == null) { currentState = 540; break; }
				Expect(172, la); // "Operator"
				currentState = 23;
				break;
			}
			case 541: {
				if (la == null) { currentState = 541; break; }
				if (la.kind == 40) {
					stateStack.Push(541);
					goto case 385;
				} else {
					PushContext(Context.Type, la, t);
					stateStack.Push(542);
					goto case 37;
				}
			}
			case 542: {
				PopContext();
				goto case 538;
			}
			case 543: {
				if (la == null) { currentState = 543; break; }
				Expect(186, la); // "Property"
				currentState = 544;
				break;
			}
			case 544: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(545);
				goto case 174;
			}
			case 545: {
				PopContext();
				goto case 546;
			}
			case 546: {
				if (la == null) { currentState = 546; break; }
				if (la.kind == 37) {
					currentState = 585;
					break;
				} else {
					goto case 547;
				}
			}
			case 547: {
				if (la == null) { currentState = 547; break; }
				if (la.kind == 63) {
					currentState = 583;
					break;
				} else {
					goto case 548;
				}
			}
			case 548: {
				if (la == null) { currentState = 548; break; }
				if (la.kind == 136) {
					currentState = 578;
					break;
				} else {
					goto case 549;
				}
			}
			case 549: {
				if (la == null) { currentState = 549; break; }
				if (la.kind == 20) {
					currentState = 577;
					break;
				} else {
					goto case 550;
				}
			}
			case 550: {
				stateStack.Push(551);
				goto case 23;
			}
			case 551: {
				PopContext();
				goto case 552;
			}
			case 552: {
				if (la == null) { currentState = 552; break; }
				if (la.kind == 40) {
					stateStack.Push(552);
					goto case 385;
				} else {
					goto case 553;
				}
			}
			case 553: {
				if (la == null) { currentState = 553; break; }
				if (set[143].Get(la.kind)) {
					currentState = 576;
					break;
				} else {
					if (la.kind == 128 || la.kind == 198) {
						PushContext(Context.Member, la, t);
						goto case 554;
					} else {
						currentState = stateStack.Pop();
						goto switchlbl;
					}
				}
			}
			case 554: {
				if (la == null) { currentState = 554; break; }
				if (la.kind == 128) {
					currentState = 555;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 555;
						break;
					} else {
						Error(la);
						goto case 555;
					}
				}
			}
			case 555: {
				if (la == null) { currentState = 555; break; }
				if (la.kind == 37) {
					currentState = 573;
					break;
				} else {
					goto case 556;
				}
			}
			case 556: {
				stateStack.Push(557);
				goto case 229;
			}
			case 557: {
				if (la == null) { currentState = 557; break; }
				Expect(113, la); // "End"
				currentState = 558;
				break;
			}
			case 558: {
				if (la == null) { currentState = 558; break; }
				if (la.kind == 128) {
					currentState = 559;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 559;
						break;
					} else {
						Error(la);
						goto case 559;
					}
				}
			}
			case 559: {
				stateStack.Push(560);
				goto case 23;
			}
			case 560: {
				if (la == null) { currentState = 560; break; }
				if (set[104].Get(la.kind)) {
					goto case 563;
				} else {
					goto case 561;
				}
			}
			case 561: {
				if (la == null) { currentState = 561; break; }
				Expect(113, la); // "End"
				currentState = 562;
				break;
			}
			case 562: {
				if (la == null) { currentState = 562; break; }
				Expect(186, la); // "Property"
				currentState = 23;
				break;
			}
			case 563: {
				if (la == null) { currentState = 563; break; }
				if (la.kind == 40) {
					stateStack.Push(563);
					goto case 385;
				} else {
					goto case 564;
				}
			}
			case 564: {
				if (la == null) { currentState = 564; break; }
				if (set[143].Get(la.kind)) {
					currentState = 564;
					break;
				} else {
					if (la.kind == 128) {
						currentState = 565;
						break;
					} else {
						if (la.kind == 198) {
							currentState = 565;
							break;
						} else {
							Error(la);
							goto case 565;
						}
					}
				}
			}
			case 565: {
				if (la == null) { currentState = 565; break; }
				if (la.kind == 37) {
					currentState = 570;
					break;
				} else {
					goto case 566;
				}
			}
			case 566: {
				stateStack.Push(567);
				goto case 229;
			}
			case 567: {
				if (la == null) { currentState = 567; break; }
				Expect(113, la); // "End"
				currentState = 568;
				break;
			}
			case 568: {
				if (la == null) { currentState = 568; break; }
				if (la.kind == 128) {
					currentState = 569;
					break;
				} else {
					if (la.kind == 198) {
						currentState = 569;
						break;
					} else {
						Error(la);
						goto case 569;
					}
				}
			}
			case 569: {
				stateStack.Push(561);
				goto case 23;
			}
			case 570: {
				SetIdentifierExpected(la);
				goto case 571;
			}
			case 571: {
				if (la == null) { currentState = 571; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(572);
					goto case 372;
				} else {
					goto case 572;
				}
			}
			case 572: {
				if (la == null) { currentState = 572; break; }
				Expect(38, la); // ")"
				currentState = 566;
				break;
			}
			case 573: {
				SetIdentifierExpected(la);
				goto case 574;
			}
			case 574: {
				if (la == null) { currentState = 574; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(575);
					goto case 372;
				} else {
					goto case 575;
				}
			}
			case 575: {
				if (la == null) { currentState = 575; break; }
				Expect(38, la); // ")"
				currentState = 556;
				break;
			}
			case 576: {
				SetIdentifierExpected(la);
				goto case 553;
			}
			case 577: {
				stateStack.Push(550);
				goto case 56;
			}
			case 578: {
				PushContext(Context.Type, la, t);
				stateStack.Push(579);
				goto case 37;
			}
			case 579: {
				PopContext();
				goto case 580;
			}
			case 580: {
				if (la == null) { currentState = 580; break; }
				if (la.kind == 22) {
					currentState = 581;
					break;
				} else {
					goto case 549;
				}
			}
			case 581: {
				PushContext(Context.Type, la, t);
				stateStack.Push(582);
				goto case 37;
			}
			case 582: {
				PopContext();
				goto case 580;
			}
			case 583: {
				if (la == null) { currentState = 583; break; }
				if (la.kind == 40) {
					stateStack.Push(583);
					goto case 385;
				} else {
					if (la.kind == 162) {
						stateStack.Push(548);
						goto case 67;
					} else {
						if (set[16].Get(la.kind)) {
							PushContext(Context.Type, la, t);
							stateStack.Push(584);
							goto case 37;
						} else {
							Error(la);
							goto case 548;
						}
					}
				}
			}
			case 584: {
				PopContext();
				goto case 548;
			}
			case 585: {
				SetIdentifierExpected(la);
				goto case 586;
			}
			case 586: {
				if (la == null) { currentState = 586; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(587);
					goto case 372;
				} else {
					goto case 587;
				}
			}
			case 587: {
				if (la == null) { currentState = 587; break; }
				Expect(38, la); // ")"
				currentState = 547;
				break;
			}
			case 588: {
				if (la == null) { currentState = 588; break; }
				Expect(98, la); // "Custom"
				currentState = 589;
				break;
			}
			case 589: {
				stateStack.Push(590);
				goto case 600;
			}
			case 590: {
				if (la == null) { currentState = 590; break; }
				if (set[109].Get(la.kind)) {
					goto case 592;
				} else {
					Expect(113, la); // "End"
					currentState = 591;
					break;
				}
			}
			case 591: {
				if (la == null) { currentState = 591; break; }
				Expect(119, la); // "Event"
				currentState = 23;
				break;
			}
			case 592: {
				if (la == null) { currentState = 592; break; }
				if (la.kind == 40) {
					stateStack.Push(592);
					goto case 385;
				} else {
					if (la.kind == 56) {
						currentState = 593;
						break;
					} else {
						if (la.kind == 193) {
							currentState = 593;
							break;
						} else {
							if (la.kind == 189) {
								currentState = 593;
								break;
							} else {
								Error(la);
								goto case 593;
							}
						}
					}
				}
			}
			case 593: {
				if (la == null) { currentState = 593; break; }
				Expect(37, la); // "("
				currentState = 594;
				break;
			}
			case 594: {
				stateStack.Push(595);
				goto case 372;
			}
			case 595: {
				if (la == null) { currentState = 595; break; }
				Expect(38, la); // ")"
				currentState = 596;
				break;
			}
			case 596: {
				stateStack.Push(597);
				goto case 229;
			}
			case 597: {
				if (la == null) { currentState = 597; break; }
				Expect(113, la); // "End"
				currentState = 598;
				break;
			}
			case 598: {
				if (la == null) { currentState = 598; break; }
				if (la.kind == 56) {
					currentState = 599;
					break;
				} else {
					if (la.kind == 193) {
						currentState = 599;
						break;
					} else {
						if (la.kind == 189) {
							currentState = 599;
							break;
						} else {
							Error(la);
							goto case 599;
						}
					}
				}
			}
			case 599: {
				stateStack.Push(590);
				goto case 23;
			}
			case 600: {
				if (la == null) { currentState = 600; break; }
				Expect(119, la); // "Event"
				currentState = 601;
				break;
			}
			case 601: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(602);
				goto case 174;
			}
			case 602: {
				PopContext();
				goto case 603;
			}
			case 603: {
				if (la == null) { currentState = 603; break; }
				if (la.kind == 63) {
					currentState = 613;
					break;
				} else {
					if (set[144].Get(la.kind)) {
						if (la.kind == 37) {
							currentState = 610;
							break;
						} else {
							goto case 604;
						}
					} else {
						Error(la);
						goto case 604;
					}
				}
			}
			case 604: {
				if (la == null) { currentState = 604; break; }
				if (la.kind == 136) {
					currentState = 605;
					break;
				} else {
					goto case 23;
				}
			}
			case 605: {
				PushContext(Context.Type, la, t);
				stateStack.Push(606);
				goto case 37;
			}
			case 606: {
				PopContext();
				goto case 607;
			}
			case 607: {
				if (la == null) { currentState = 607; break; }
				if (la.kind == 22) {
					currentState = 608;
					break;
				} else {
					goto case 23;
				}
			}
			case 608: {
				PushContext(Context.Type, la, t);
				stateStack.Push(609);
				goto case 37;
			}
			case 609: {
				PopContext();
				goto case 607;
			}
			case 610: {
				SetIdentifierExpected(la);
				goto case 611;
			}
			case 611: {
				if (la == null) { currentState = 611; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(612);
					goto case 372;
				} else {
					goto case 612;
				}
			}
			case 612: {
				if (la == null) { currentState = 612; break; }
				Expect(38, la); // ")"
				currentState = 604;
				break;
			}
			case 613: {
				PushContext(Context.Type, la, t);
				stateStack.Push(614);
				goto case 37;
			}
			case 614: {
				PopContext();
				goto case 604;
			}
			case 615: {
				if (la == null) { currentState = 615; break; }
				Expect(101, la); // "Declare"
				currentState = 616;
				break;
			}
			case 616: {
				if (la == null) { currentState = 616; break; }
				if (la.kind == 62 || la.kind == 66 || la.kind == 223) {
					currentState = 617;
					break;
				} else {
					goto case 617;
				}
			}
			case 617: {
				if (la == null) { currentState = 617; break; }
				if (la.kind == 210) {
					currentState = 618;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 618;
						break;
					} else {
						Error(la);
						goto case 618;
					}
				}
			}
			case 618: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(619);
				goto case 174;
			}
			case 619: {
				PopContext();
				goto case 620;
			}
			case 620: {
				if (la == null) { currentState = 620; break; }
				Expect(149, la); // "Lib"
				currentState = 621;
				break;
			}
			case 621: {
				if (la == null) { currentState = 621; break; }
				Expect(3, la); // LiteralString
				currentState = 622;
				break;
			}
			case 622: {
				if (la == null) { currentState = 622; break; }
				if (la.kind == 59) {
					currentState = 630;
					break;
				} else {
					goto case 623;
				}
			}
			case 623: {
				if (la == null) { currentState = 623; break; }
				if (la.kind == 37) {
					currentState = 627;
					break;
				} else {
					goto case 624;
				}
			}
			case 624: {
				if (la == null) { currentState = 624; break; }
				if (la.kind == 63) {
					currentState = 625;
					break;
				} else {
					goto case 23;
				}
			}
			case 625: {
				PushContext(Context.Type, la, t);
				stateStack.Push(626);
				goto case 37;
			}
			case 626: {
				PopContext();
				goto case 23;
			}
			case 627: {
				SetIdentifierExpected(la);
				goto case 628;
			}
			case 628: {
				if (la == null) { currentState = 628; break; }
				if (set[73].Get(la.kind)) {
					stateStack.Push(629);
					goto case 372;
				} else {
					goto case 629;
				}
			}
			case 629: {
				if (la == null) { currentState = 629; break; }
				Expect(38, la); // ")"
				currentState = 624;
				break;
			}
			case 630: {
				if (la == null) { currentState = 630; break; }
				Expect(3, la); // LiteralString
				currentState = 623;
				break;
			}
			case 631: {
				if (la == null) { currentState = 631; break; }
				if (la.kind == 210) {
					currentState = 632;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 632;
						break;
					} else {
						Error(la);
						goto case 632;
					}
				}
			}
			case 632: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 633;
			}
			case 633: {
				if (la == null) { currentState = 633; break; }
				currentState = 634;
				break;
			}
			case 634: {
				PopContext();
				goto case 635;
			}
			case 635: {
				if (la == null) { currentState = 635; break; }
				if (la.kind == 37) {
					currentState = 641;
					break;
				} else {
					if (la.kind == 63) {
						currentState = 639;
						break;
					} else {
						goto case 636;
					}
				}
			}
			case 636: {
				stateStack.Push(637);
				goto case 229;
			}
			case 637: {
				if (la == null) { currentState = 637; break; }
				Expect(113, la); // "End"
				currentState = 638;
				break;
			}
			case 638: {
				if (la == null) { currentState = 638; break; }
				if (la.kind == 210) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 127) {
						currentState = 23;
						break;
					} else {
						goto case 471;
					}
				}
			}
			case 639: {
				PushContext(Context.Type, la, t);
				stateStack.Push(640);
				goto case 37;
			}
			case 640: {
				PopContext();
				goto case 636;
			}
			case 641: {
				SetIdentifierExpected(la);
				goto case 642;
			}
			case 642: {
				if (la == null) { currentState = 642; break; }
				if (set[142].Get(la.kind)) {
					if (la.kind == 169) {
						currentState = 644;
						break;
					} else {
						if (set[73].Get(la.kind)) {
							stateStack.Push(643);
							goto case 372;
						} else {
							Error(la);
							goto case 643;
						}
					}
				} else {
					goto case 643;
				}
			}
			case 643: {
				if (la == null) { currentState = 643; break; }
				Expect(38, la); // ")"
				currentState = 635;
				break;
			}
			case 644: {
				stateStack.Push(643);
				goto case 438;
			}
			case 645: {
				stateStack.Push(646);
				SetIdentifierExpected(la);
				goto case 647;
			}
			case 646: {
				if (la == null) { currentState = 646; break; }
				if (la.kind == 22) {
					currentState = 645;
					break;
				} else {
					goto case 23;
				}
			}
			case 647: {
				if (la == null) { currentState = 647; break; }
				if (la.kind == 88) {
					currentState = 648;
					break;
				} else {
					goto case 648;
				}
			}
			case 648: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				stateStack.Push(649);
				goto case 656;
			}
			case 649: {
				PopContext();
				goto case 650;
			}
			case 650: {
				if (la == null) { currentState = 650; break; }
				if (la.kind == 63) {
					currentState = 652;
					break;
				} else {
					goto case 651;
				}
			}
			case 651: {
				if (la == null) { currentState = 651; break; }
				if (la.kind == 20) {
					goto case 187;
				} else {
					currentState = stateStack.Pop();
					goto switchlbl;
				}
			}
			case 652: {
				PushContext(Context.Type, la, t);
				goto case 653;
			}
			case 653: {
				if (la == null) { currentState = 653; break; }
				if (la.kind == 162) {
					stateStack.Push(654);
					goto case 67;
				} else {
					if (set[16].Get(la.kind)) {
						PushContext(Context.Type, la, t);
						stateStack.Push(655);
						goto case 37;
					} else {
						Error(la);
						goto case 654;
					}
				}
			}
			case 654: {
				PopContext();
				goto case 651;
			}
			case 655: {
				PopContext();
				goto case 654;
			}
			case 656: {
				if (la == null) { currentState = 656; break; }
				if (set[128].Get(la.kind)) {
					currentState = stateStack.Pop();
					break;
				} else {
					if (la.kind == 58) {
						goto case 124;
					} else {
						if (la.kind == 126) {
							goto case 108;
						} else {
							goto case 6;
						}
					}
				}
			}
			case 657: {
				isMissingModifier = false;
				goto case 525;
			}
			case 658: {
				if (la == null) { currentState = 658; break; }
				Expect(136, la); // "Implements"
				currentState = 659;
				break;
			}
			case 659: {
				PushContext(Context.Type, la, t);
				stateStack.Push(660);
				goto case 37;
			}
			case 660: {
				PopContext();
				goto case 661;
			}
			case 661: {
				if (la == null) { currentState = 661; break; }
				if (la.kind == 22) {
					currentState = 662;
					break;
				} else {
					stateStack.Push(517);
					goto case 23;
				}
			}
			case 662: {
				PushContext(Context.Type, la, t);
				stateStack.Push(663);
				goto case 37;
			}
			case 663: {
				PopContext();
				goto case 661;
			}
			case 664: {
				if (la == null) { currentState = 664; break; }
				Expect(140, la); // "Inherits"
				currentState = 665;
				break;
			}
			case 665: {
				PushContext(Context.Type, la, t);
				stateStack.Push(666);
				goto case 37;
			}
			case 666: {
				PopContext();
				stateStack.Push(515);
				goto case 23;
			}
			case 667: {
				if (la == null) { currentState = 667; break; }
				Expect(169, la); // "Of"
				currentState = 668;
				break;
			}
			case 668: {
				stateStack.Push(669);
				goto case 438;
			}
			case 669: {
				if (la == null) { currentState = 669; break; }
				Expect(38, la); // ")"
				currentState = 512;
				break;
			}
			case 670: {
				isMissingModifier = false;
				goto case 28;
			}
			case 671: {
				PushContext(Context.Type, la, t);
				stateStack.Push(672);
				goto case 37;
			}
			case 672: {
				PopContext();
				goto case 673;
			}
			case 673: {
				if (la == null) { currentState = 673; break; }
				if (la.kind == 22) {
					currentState = 674;
					break;
				} else {
					stateStack.Push(17);
					goto case 23;
				}
			}
			case 674: {
				PushContext(Context.Type, la, t);
				stateStack.Push(675);
				goto case 37;
			}
			case 675: {
				PopContext();
				goto case 673;
			}
			case 676: {
				if (la == null) { currentState = 676; break; }
				Expect(169, la); // "Of"
				currentState = 677;
				break;
			}
			case 677: {
				stateStack.Push(678);
				goto case 438;
			}
			case 678: {
				if (la == null) { currentState = 678; break; }
				Expect(38, la); // ")"
				currentState = 14;
				break;
			}
			case 679: {
				PushContext(Context.Identifier, la, t);
				SetIdentifierExpected(la);
				goto case 680;
			}
			case 680: {
				if (la == null) { currentState = 680; break; }
				if (set[46].Get(la.kind)) {
					currentState = 680;
					break;
				} else {
					PopContext();
					stateStack.Push(681);
					goto case 23;
				}
			}
			case 681: {
				if (la == null) { currentState = 681; break; }
				if (set[3].Get(la.kind)) {
					stateStack.Push(681);
					goto case 5;
				} else {
					Expect(113, la); // "End"
					currentState = 682;
					break;
				}
			}
			case 682: {
				if (la == null) { currentState = 682; break; }
				Expect(160, la); // "Namespace"
				currentState = 23;
				break;
			}
			case 683: {
				if (la == null) { currentState = 683; break; }
				Expect(137, la); // "Imports"
				currentState = 684;
				break;
			}
			case 684: {
				PushContext(Context.Importable, la, t);
				nextTokenIsStartOfImportsOrAccessExpression = true;	
				goto case 685;
			}
			case 685: {
				if (la == null) { currentState = 685; break; }
				if (set[145].Get(la.kind)) {
					currentState = 691;
					break;
				} else {
					if (la.kind == 10) {
						currentState = 687;
						break;
					} else {
						Error(la);
						goto case 686;
					}
				}
			}
			case 686: {
				PopContext();
				goto case 23;
			}
			case 687: {
				stateStack.Push(688);
				goto case 174;
			}
			case 688: {
				if (la == null) { currentState = 688; break; }
				Expect(20, la); // "="
				currentState = 689;
				break;
			}
			case 689: {
				if (la == null) { currentState = 689; break; }
				Expect(3, la); // LiteralString
				currentState = 690;
				break;
			}
			case 690: {
				if (la == null) { currentState = 690; break; }
				Expect(11, la); // XmlCloseTag
				currentState = 686;
				break;
			}
			case 691: {
				if (la == null) { currentState = 691; break; }
				if (la.kind == 37) {
					stateStack.Push(691);
					goto case 42;
				} else {
					if (la.kind == 20 || la.kind == 26) {
						currentState = 692;
						break;
					} else {
						goto case 686;
					}
				}
			}
			case 692: {
				PushContext(Context.Type, la, t);
				stateStack.Push(693);
				goto case 37;
			}
			case 693: {
				PopContext();
				goto case 686;
			}
			case 694: {
				if (la == null) { currentState = 694; break; }
				Expect(173, la); // "Option"
				currentState = 695;
				break;
			}
			case 695: {
				if (la == null) { currentState = 695; break; }
				if (la.kind == 121 || la.kind == 139 || la.kind == 207) {
					currentState = 697;
					break;
				} else {
					if (la.kind == 87) {
						currentState = 696;
						break;
					} else {
						goto case 471;
					}
				}
			}
			case 696: {
				if (la == null) { currentState = 696; break; }
				if (la.kind == 213) {
					currentState = 23;
					break;
				} else {
					if (la.kind == 67) {
						currentState = 23;
						break;
					} else {
						goto case 471;
					}
				}
			}
			case 697: {
				if (la == null) { currentState = 697; break; }
				if (la.kind == 170 || la.kind == 171) {
					currentState = 23;
					break;
				} else {
					goto case 23;
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
		new BitArray(new int[] {5, 1140850944, 26214479, -493220892, 671926304, 1606227075, -2143942272, 3393}),
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