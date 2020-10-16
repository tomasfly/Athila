// Works fine with TSLA july 1 2020 to july 30 2020
#region Using declarations
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.Data;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.NinjaScript;
using NinjaTrader.NinjaScript.DrawingTools;
using NinjaTrader.NinjaScript.Indicators;
#endregion

namespace NinjaTrader.NinjaScript.Strategies {
    public class MACDDecel : Strategy {
        private MACD macd;
        private bool singlePosition = true;
        private int barsCountSinceBuy;
        protected override void OnStateChange () {
            if (State == State.SetDefaults) {
                Description = @"Project Athila";
                Name = "MACDDecel";
                Calculate = Calculate.OnBarClose;
                EntriesPerDirection = 1;
                EntryHandling = EntryHandling.AllEntries;
                IsExitOnSessionCloseStrategy = true;
                ExitOnSessionCloseSeconds = 30;
                IsFillLimitOnTouch = false;
                MaximumBarsLookBack = MaximumBarsLookBack.TwoHundredFiftySix;
                OrderFillResolution = OrderFillResolution.Standard;
                Slippage = 0;
                StartBehavior = StartBehavior.WaitUntilFlat;
                TimeInForce = TimeInForce.Gtc;
                TraceOrders = false;
                RealtimeErrorHandling = RealtimeErrorHandling.StopCancelClose;
                StopTargetHandling = StopTargetHandling.PerEntryExecution;
                BarsRequiredToTrade = 20;
                IsInstantiatedOnEachOptimizationIteration = true;
            } else if (State == State.Configure) {
                macd = MACD (12, 26, 9);
                AddChartIndicator (macd);
                // Doesnt seem to be needed for the moment
                //AddDataSeries (Data.BarsPeriodType.Tick, 1);
            }
        }

        protected override void OnBarUpdate () {

            // // if macd is below signal
            // if (macd.Default[0] < macd.Avg[0]) {
            //     // if macd has been falling and there is a turn
            //     if ((macd.Default[0] > macd.Default[1]) &&
            //         (macd.Default[1] < macd.Default[2]) &&
            //         (macd.Default[2] < macd.Default[3]) &&
            //         (macd.Default[3] < macd.Default[4])) {
            //         // if macd is below baseline
            //         if (macd.Default[0] < 0) {
            //             // if macd minus 10 candles was still below baseline to avoid fake patterns
            //             if (macd.Default[10] < 0) {
            //                 Print ("Printing diff" + macd.Diff[5]);
            //                 EnterLong (10);
            //             }
            //         }
            //     }
            // }

            // Approach with less number of falling candles
            // if macd is below signal
			if(CurrentBar>15){
				if (macd.Default[0] < macd.Avg[0]) {
                // if macd has been falling and there is a turn
                	if ((macd.Default[0] > macd.Default[1]) && (macd.Default[1] < macd.Default[2])) {
                    // if macd is below baseline
                    	if (macd.Default[0] < 0) {
                        // if macd minus 10 candles was still below baseline to avoid fake patterns
                        	if (macd.Default[10] < 0) {
                            //                            Print ("Printing diff" + macd.Diff[5]);
                            EnterLong (10);
                        		}
                    		}
                		}
            		}
				}


            if (Position.MarketPosition == MarketPosition.Long) {
				barsCountSinceBuy++;
                // sell if two rend candles
                if (Close[0] < Open[0] && Close[1] < Open[1]) {
                    ExitLong ("Two consecutive red candles", "Buy");
					barsCountSinceBuy = 0;
                }
                // sell when macd crosses above signal
                // if (CrossAbove (macd.Default, macd.Avg, 1)) {
                //     ExitLong ("MACD crossed above signal", "Buy");
                // }

                // sell when macd starts to decel
                if (barsCountSinceBuy > 5) {
                    //calculate % of increase of MACD
                    double diff = macd.Default[0] - macd.Default[1];
                    double percentageOfIncrease = (diff / macd.Default[1]) * 100;
					Print(percentageOfIncrease);
                    if (percentageOfIncrease < 0) {
						Print("selling because percentageOfIncrease" + percentageOfIncrease);
                        Print("time"+Time[0]);
                        ExitLong ("MACD stopped rising", "Buy");
						barsCountSinceBuy = 0;
                    }
                }
            }
            if (Position.MarketPosition == MarketPosition.Short) {
                if (CrossBelow (macd.Default, macd.Avg, 1)) {
                    ExitShort ();
                } else if (Time[0].Hour == 15 && Time[0].Minute >= 50) {
                    ExitShort ();
                }
            }
        }
    }
}