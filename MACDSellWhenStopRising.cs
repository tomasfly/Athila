//Author : Jacob Amaral
//TODO : Maybe enter short if certain amount of profit
//TODO : Add more filters for entry like volume or 20 candles are below certain value
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
    public class MACDSellWhenStopRising : Strategy {
        private MACD macd;
        private bool singlePosition = true;
        private int barsCountSinceBuy;
        protected override void OnStateChange () {
            if (State == State.SetDefaults) {
                Description = @"Project Athila";
                Name = "MACDSellWhenStopRising";
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
			
            if (CrossAbove (macd.Default, 0, 1) && macd.Default[0] > macd.Avg[0]) {
                EnterLong (10);
            }

            if (Position.MarketPosition == MarketPosition.Long) {
                barsCountSinceBuy++;
                if (barsCountSinceBuy > 5) {
                    //calculate % of increase of MACD
                    double diff = macd.Default[0] - macd.Default[1];
                    double percentageOfIncrease = (diff / macd.Default[1]) * 100;
					Print(percentageOfIncrease);
                    if (percentageOfIncrease < 0) {
						Print("selling because percentageOfIncrease" + percentageOfIncrease);
                        Print("time"+Time[0]);
                        ExitLong ("Cross below MACD Exit", "Buy");
                    }
                }

                if (CrossBelow (macd.Default, macd.Avg, 1)) {
                    barsCountSinceBuy = 0;
                    ExitLong ("Cross below MACD Exit", "Buy");
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