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
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies
{
	public class Buy5 : Strategy
	{
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Buy 5 candles after open, sell next day.";
				Name										= "Buy5";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 0;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;

			}
			else if (State == State.Configure)
			{
				SetStopLoss(CalculationMode.Percent, 0.05);
				SetProfitTarget(CalculationMode.Percent, 0.05);
			}
		}
		
		bool firstBarFlag = true;		
		List<double> firstBars = new List<double>();

		protected override void OnBarUpdate()
		{
			bool rises = IsRising(SMA(10));
			
			if(firstBarFlag){				
				Log("Hello world",LogLevel.Information);
				Log(Bars.GetClose(1).ToString(),LogLevel.Information);
				Log(Bars.LastPrice.ToString(),LogLevel.Information);				
			}
			
			firstBarFlag = false;
		
			if (Bars.BarsSinceNewTradingDay == 5 && Position.MarketPosition == MarketPosition.Flat) {
				if (!goShort)
					if(rises)
						EnterLong();	
				else
					EnterShort();
			} else if (Bars.BarsSinceNewTradingDay == 5 && Position.MarketPosition != MarketPosition.Flat) {
							
				if (!goShort)
					ExitLong();	
				else
					ExitShort();	
			}
		}
		[NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Go Short", GroupName = "NinjaScriptStrategyParameters", Order = 0)]
        public bool goShort
        { get; set; }
	}
}
