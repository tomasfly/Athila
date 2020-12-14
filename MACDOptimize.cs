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
    public class MACDOptimize : Strategy {
        // -1.5 default value
        // works fine with TSLA from 1 september to 30 september and -10 value
        // tested with october and september
        private double belowSignalThreshold = -10;
        private double diffThreshold = 0.04;

        private bool redCandleSequenceExit = true;
        private int candlesSinceBuy;
        private MACD macd;
        private bool singlePosition = true;
        private int barsCountSinceBuy;
        ArrayList myAL = new ArrayList ();
        protected override void OnStateChange () {
            if (State == State.SetDefaults) {
                BelowSignalThreshold = -10;
                DiffThreshold = 0.04;
                RedCandleSequenceExit = true;
                Description = @"Project Athila";
                Name = "MACDOptimize";
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
            if (CrossAbove (macd.Default, macd.Avg, 1) &&
                CurrentBar > 15
                // &&
                // // macd.Default[13] < macd.Avg[13] &&
                // // macd.Default[10] < macd.Avg[10] &&
                // // macd.Default[7] < macd.Avg[7] &&
                // macd.Default[5] < macd.Avg[5] &&
                // macd.Default[3] < macd.Avg[3] &&
                // macd.Default[1] < macd.Avg[1]
            ) {
                EnterLong (10);
            }
            if (Position.MarketPosition == MarketPosition.Long) {
                candlesSinceBuy++;
                if (candlesSinceBuy == 2 && (macd.Diff[0] < DiffThreshold && macd.Diff[1] < DiffThreshold)) {
                    ExitLong ("Low force", "Buy");
                    candlesSinceBuy = 0;
                }
                double diff = macd.Default[0] - macd.Default[1];
                double percentageOfIncrease = (diff / macd.Default[1]) * 100;
                myAL.Add (percentageOfIncrease);
                if (RedCandleSequenceExit) {
                    if (Close[0] < Open[0] && Close[1] < Open[1] && Close[2] < Open[2]) {
                        ExitLong ("Three red candles", "Buy");
                        candlesSinceBuy = 0;
                    }
                }
                if (myAL.Count > 2) {
                    double parsedValueTwoCandles = Double.Parse (myAL[myAL.Count - 2].ToString ());
                    double parsedValueThreeCandles = Double.Parse (myAL[myAL.Count - 3].ToString ());
                    if ((percentageOfIncrease < BelowSignalThreshold && parsedValueTwoCandles < BelowSignalThreshold && parsedValueThreeCandles < BelowSignalThreshold) || CrossBelow (macd.Default, macd.Avg, 1)) {
                        ExitLong ("Decreasing % or crossbelow signal", "Buy");
                        candlesSinceBuy = 0;
                        myAL.Clear ();
                    }
                }
            }
        }

        #region Properties
        [Range (-50, int.MaxValue), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "BelowSignalThreshold", GroupName = "NinjaScriptStrategyParameters", Order = 0)]
        public int BelowSignalThreshold { get; set; }

        [Range (-10, double.MaxValue), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "DiffThreshold", GroupName = "NinjaScriptStrategyParameters", Order = 1)]
        public double DiffThreshold { get; set; }

        [Display (ResourceType = typeof (Custom.Resource), Name = "RedCandleSequenceExit", GroupName = "NinjaScriptStrategyParameters", Order = 1)]
        public bool RedCandleSequenceExit { get; set; }
        #endregion
    }
}