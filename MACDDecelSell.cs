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
    public class MACDDecelSell : Strategy {
        private MACD macd;
        private bool singlePosition = true;
        private int barsCountSinceBuy;
        protected override void OnStateChange () {
            if (State == State.SetDefaults) {
                Description = @"Project Athila";
                Name = "MACDDecelSell";
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
            if (CurrentBar > 15) {
                double diff = macd.Default[0] - macd.Default[1];
                double percentageOfIncreaseToday = (diff / macd.Default[1]) * 100;

                double diff1 = macd.Default[1] - macd.Default[2];
                double percentageOfIncreaseToday1 = (diff1 / macd.Default[2]) * 100;

                double diff2 = macd.Default[2] - macd.Default[3];
                double percentageOfIncreaseToday2 = (diff2 / macd.Default[3]) * 100;

                double diff3 = macd.Default[3] - macd.Default[4];
                double percentageOfIncreaseToday3 = (diff3 / macd.Default[4]) * 100;

                double diff4 = macd.Default[4] - macd.Default[5];
                double percentageOfIncreaseToday4 = (diff4 / macd.Default[5]) * 100;

                double diff5 = macd.Default[5] - macd.Default[6];
                double percentageOfIncreaseToday5 = (diff5 / macd.Default[6]) * 100;

                double diff6 = macd.Default[6] - macd.Default[7];
                double percentageOfIncreaseToday6 = (diff6 / macd.Default[7]) * 100;
                // Print (percentageOfIncreaseToday);

                if (percentageOfIncreaseToday6 > 0 &&
                    percentageOfIncreaseToday5 > 0 &&
                    percentageOfIncreaseToday4 > 0 &&
                    percentageOfIncreaseToday3 > 0 &&
                    percentageOfIncreaseToday2 > 0 &&
                    percentageOfIncreaseToday1 > 0 &&
                    percentageOfIncreaseToday < 0
                )

                {
                    Print ("here");
                    if (percentageOfIncreaseToday6 > percentageOfIncreaseToday5 &&
                        percentageOfIncreaseToday5 > percentageOfIncreaseToday4 &&
                        percentageOfIncreaseToday4 > percentageOfIncreaseToday3 &&
                        percentageOfIncreaseToday3 > percentageOfIncreaseToday2 &&
                        percentageOfIncreaseToday2 > percentageOfIncreaseToday1 &&
                        percentageOfIncreaseToday1 > percentageOfIncreaseToday) {
                        Print ("here2");
                        Print ("Entering short" + Time[0]);
                        EnterShort (10);
                    }

                }

                // if (CrossBelow (macd.Default, macd.Avg, 1)) {
                //     Print ("Entering short" + Time[0]);
                //     EnterShort (10);
                // }
            }
            if (Position.MarketPosition == MarketPosition.Short) {
                if (CrossAbove (macd.Default, macd.Avg, 1)) {
                    Print ("Exiting short" + Time[0]);
                    ExitShort ();
                }
            }
        }
    }
}