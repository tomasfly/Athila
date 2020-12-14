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

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies {
	public class EMACrossLongshort : Strategy {
		private EMA EMA1;
		private EMA EMA2;

		private RSI rsi;

		protected override void OnStateChange () {
			if (State == State.SetDefaults) {
				Description = @"Enter the description for your new custom Strategy here.";
				Name = "EMACrossLongshort";
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

			} else if (State == State.DataLoaded) {
				EMA1 = EMA (Close, 10);
				EMA2 = EMA (Close, 50);
				EMA1.Plots[0].Brush = Brushes.OrangeRed;
				EMA2.Plots[0].Brush = Brushes.Goldenrod;
				AddChartIndicator (EMA1);
				AddChartIndicator (EMA2);
				rsi = RSI(14, 3);
				AddChartIndicator(rsi);
			}
		}

		protected override void OnBarUpdate () {
			if (BarsInProgress != 0)
				return;

			if (CurrentBars[0] < 1)
				return;

			if (CrossAbove (EMA1, EMA2, 1)) {
				EnterLong ();
			}

			if (CrossBelow (EMA1, EMA2, 1)) {
				EnterShort ();
			}

			if (Position.MarketPosition == MarketPosition.Short) {

				// if (EMA1[0] > EMA1[1]) {
				// 	ExitShort ();
				// }
				if (CrossBelow (EMA1, EMA2, 1)) {
					ExitLong ();
				}

			}

			if (Position.MarketPosition == MarketPosition.Long) {

				// if (EMA1[0] < EMA1[1]) {
				// 	ExitLong ();
				// }
				if (CrossAbove (EMA1, EMA2, 1)) {
					ExitShort ();
				}

			}

		}
	}
}