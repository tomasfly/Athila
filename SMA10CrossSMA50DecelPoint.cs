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
	public class SMA10CrossSMA50DecelPoint : Strategy {
		private EMA EMA1;
		private EMA EMA2;

		protected override void OnStateChange () {
			if (State == State.SetDefaults) {
				Description = @"Enter the description for your new custom Strategy here.";
				Name = "SMA10CrossSMA50DecelPoint";
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
				AddChartIndicator (EMA1);
				AddChartIndicator (EMA2);
			}
		}

		protected override void OnBarUpdate () {
			if (BarsInProgress != 0)
				return;

			if (CurrentBars[0] < 1)
				return;

			if (CrossAbove (EMA1, EMA2, 1)) {
				Print ("ENTERING LONG/////////");
				EnterLong (10);
			}

			if (Position.MarketPosition == MarketPosition.Long) {
				Print (EMA1[0]);

				if (EMA1[0] < EMA1[1] && EMA1[1] < EMA1[2]) {
					Print ("EXITING LONG/////////");
					ExitLong ();
					// Logic to check if it is decreasing a lot
					// double difference = EMA1[1] - EMA1[0];
					// double diffPercentage = (difference / EMA1[1])*100;
					// Print ("Warn" + EMA1[0]);
					// Print("Percentage Decrease is"+diffPercentage);
				}

				if (CrossBelow (EMA1, EMA2, 1)) {
					Print ("EXITING LONG/////////");
					ExitLong ();
				}
			}

		}
	}
}