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
        // -1.5 default value
        // works fine with TSLA from 1 september to 30 september and -10 value
        private double belowSignalThreshold = -10;
        private int candlesSinceBuy;
        private MACD macd;
        private bool singlePosition = true;
        private int barsCountSinceBuy;
        ArrayList myAL = new ArrayList ();
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
            }
        }

        protected override void OnBarUpdate () {
            Print (macd.Diff[0]);
            if (CrossAbove (macd.Default, macd.Avg, 1) &&
                CurrentBar > 15 &&
                macd.Default[13] < macd.Avg[13] &&
                macd.Default[10] < macd.Avg[10] &&
                macd.Default[7] < macd.Avg[7] &&
                macd.Default[5] < macd.Avg[5] &&
                macd.Default[3] < macd.Avg[3] &&
                macd.Default[1] < macd.Avg[1]
            ) {
                Print ("Entering long, printing diff and prevs");
                Print (CurrentBar);
                Print (macd.Diff[4]);
                Print (macd.Diff[3]);
                Print (macd.Diff[2]);
                Print (macd.Diff[1]);
                Print (macd.Diff[0]);
                Print ("////");
                Print (Time[0]);
                EnterLong (10);
            }
            if (Position.MarketPosition == MarketPosition.Long) {
                candlesSinceBuy++;
                if (candlesSinceBuy == 1 && macd.Diff[0] < 0.07) {
                    ExitLong ("Low force", "Buy");
                    candlesSinceBuy = 0;
                }
                double diff = macd.Default[0] - macd.Default[1];
                double percentageOfIncrease = (diff / macd.Default[1]) * 100;
                myAL.Add (percentageOfIncrease);
                if (Close[0] < Open[0] && Close[1] < Open[1] && Close[2] < Open[2]) {
                    ExitLong ("Three red candles", "Buy");
                    candlesSinceBuy = 0;
                }
                if (myAL.Count > 2) {
                    double parsedValueTwoCandles = Double.Parse (myAL[myAL.Count - 2].ToString ());
                    double parsedValueThreeCandles = Double.Parse (myAL[myAL.Count - 3].ToString ());
                    if ((percentageOfIncrease < belowSignalThreshold && parsedValueTwoCandles < belowSignalThreshold && parsedValueThreeCandles < belowSignalThreshold) || CrossBelow (macd.Default, macd.Avg, 1)) {
                        ExitLong ("Decreasing % or crossbelow signal", "Buy");
                        candlesSinceBuy = 0;
                        myAL.Clear ();
                    }
                }
            }
        }
    }
}