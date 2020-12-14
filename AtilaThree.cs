// Buy when MACD crosses Signal only when MACD and Signal are above baseline
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
    public class AtilaThree : Strategy {
        private MACD macd;
        private RSI rsi;
        private ChoppinessIndex choppiness;
        protected override void OnStateChange () {
            if (State == State.SetDefaults) {
                Description = @"Project Zeus";
                Name = "AtilaThree";
                Threshold = -5;
                ThresholdSell = -5;
                BuyOnlyAboveSignal = true;
                Fast = 12;
                Slow = 26;
                Smooth = 9;
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
                IsInstantiatedOnEachOptimizationIteration = false;
            } else if (State == State.DataLoaded) {
                macd = MACD (Fast, Slow, Smooth);
                AddChartIndicator (macd);
            }
        }
        protected override void OnBarUpdate () {
            if (BuyOnlyAboveSignal) {
                if (CrossAbove (macd.Default, macd.Avg, 1) && macd.Default[0] > 0 && macd.Avg[0] > 0) {
                    EnterLong (10);
                }
            } else {
                if (CrossAbove (macd.Default, macd.Avg, 1)) {
                    if (!(Position.MarketPosition == MarketPosition.Short)) {
                        EnterLong (10);
                    }

                }
                if (CrossBelow (macd.Default, macd.Avg, 1)) {
                    if (!(Position.MarketPosition == MarketPosition.Long)) {
                        EnterShort (10);
                    }
                }
            }

            if (Position.MarketPosition == MarketPosition.Long) {
                double diff = macd.Default[0] - macd.Default[1];
                double percentageOfIncrease = (diff / macd.Default[1]) * 100;
                if (percentageOfIncrease < Threshold || CrossBelow (macd.Default, macd.Avg, 1)) {
                    ExitLong ();
                }
            }
            if (Position.MarketPosition == MarketPosition.Short) {
                double diff = macd.Default[1] - macd.Default[0];
                double percentageOfIncrease = (diff / macd.Default[0]) * 100;
                if (percentageOfIncrease > ThresholdSell || CrossAbove (macd.Default, macd.Avg, 1)) {
                    ExitShort ();
                }

            }
        }

        [Range (-50, 20), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "Threshold", GroupName = "NinjaScriptStrategyParameters", Order = 1)]
        public int Threshold { get; set; }

        [Display (ResourceType = typeof (Custom.Resource), Name = "BuyOnlyAboveSignal", GroupName = "NinjaScriptStrategyParameters", Order = 2)]
        public bool BuyOnlyAboveSignal { get; set; }

        [Range (-10, 20), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "Fast", GroupName = "NinjaScriptStrategyParameters", Order = 3)]
        public int Fast { get; set; }

        [Range (-10, 40), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "Slow", GroupName = "NinjaScriptStrategyParameters", Order = 4)]
        public int Slow { get; set; }

        [Range (-10, 20), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "Smooth", GroupName = "NinjaScriptStrategyParameters", Order = 5)]
        public int Smooth { get; set; }

        [Range (-50, 20), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "ThresholdSell", GroupName = "NinjaScriptStrategyParameters", Order = 6)]
        public int ThresholdSell { get; set; }
    }
}