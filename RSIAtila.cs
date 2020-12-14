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

namespace NinjaTrader.NinjaScript.Strategies {
    public class RSIAtila : Strategy {
        private RSI rsi;
        protected override void OnStateChange () {
            if (State == State.SetDefaults) {
                Description = @"Project Zeus";
                Name = "RSIAtila";
                Calculate = Calculate.OnBarClose;
                EntriesPerDirection = 1;
                overSoldRSI = 15;
                overBoughtRSI = 70;
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
                rsi = RSI (14, 3);
                AddChartIndicator (rsi);
                AddDataSeries (Data.BarsPeriodType.Tick, 1);
            }
        }

        protected override void OnBarUpdate () {
			if(Bars.BarsSinceNewTradingDay>10){			
            if (CurrentBar < BarsRequiredToTrade)
                return;
            if (rsi[0] < overSoldRSI) {
                EnterLong (10);
            }

            if (Position.MarketPosition == MarketPosition.Long) {
                if (rsi[0] >= overBoughtRSI) {
                    ExitLong ();
                }

            }
			}
        }

        [Range (1, 100), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "overSoldRSI", GroupName = "NinjaScriptStrategyParameters", Order = 3)]
        public int overSoldRSI { get; set; }

        [Range (1, 100), NinjaScriptProperty]
        [Display (ResourceType = typeof (Custom.Resource), Name = "overBoughtRSI", GroupName = "NinjaScriptStrategyParameters", Order = 4)]
        public int overBoughtRSI { get; set; }
    }
}