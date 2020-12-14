// Buy when MACD crosses Signal no matter if it is above or below baseline
// Find a point where to sell
// Optimize parameter used for selling
#region Using declarations
using System;
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
    public class AtilaOne : Strategy {
        private MACD macd;
        private RSI rsi;
        protected override void OnStateChange () {
            if (State == State.SetDefaults) {
                Description = @"Project Zeus";
                Name = "AtilaOne";
                Threshold = -13;
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
                AddDataSeries (Data.BarsPeriodType.Tick, 1);
            }
        }

        protected override void OnBarUpdate () {
            if (CrossAbove (macd.Default, macd.Avg, 1)) {
                EnterLong (10);
            }
            if (Position.MarketPosition == MarketPosition.Long) {
                double diff = macd.Default[0] - macd.Default[1];
                double percentageOfIncrease = (diff / macd.Default[1]) * 100;
                if (percentageOfIncrease < Threshold || CrossBelow(macd.Default, macd.Avg, 1)) {
                    // Print ("Exiting");
                    ExitLong ("Decelering, hence selling.", "Buy");
                }
            }
            if (Position.MarketPosition == MarketPosition.Short) {
                if (CrossBelow (macd.Default, macd.Avg, 1)) {
                    ExitShort ();
                } else if (rsi[0] >= 85) {
                    ExitShort ();
                } else if (Time[0].Hour == 15 && Time[0].Minute >= 50) {
                    ExitShort ();
                }

            }
        }

        [Range (-50, 20), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "Threshold", GroupName = "NinjaScriptStrategyParameters", Order = 1)]
        public int Threshold { get; set; }
    }
}