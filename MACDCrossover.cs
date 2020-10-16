//Author : Jacob Amaral
//TODO : Maybe enter short if certain amount of profit
//TODO : Add more filters for entry like volume or 20 candles are below certain value
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
    public class MACDCrossover : Strategy
    {
        //Custom variables and indicators
        private MACD macd;
        private RSI rsi;
        protected override void OnStateChange()
        {
            if (State == State.SetDefaults)
            {
                Description = @"Project Zeus";
                Name = "MACDCrossover";
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
                // Disable this property for performance gains in Strategy Analyzer optimizations
                // See the Help Guide for additional information
                IsInstantiatedOnEachOptimizationIteration = true;
            }
            else if (State == State.Configure)
            {
                //Initial indicators and other account variables here
                macd = MACD(12, 26, 9);
                //rsi = RSI(14, 3);
				AddChartIndicator(macd);
				//AddChartIndicator(rsi);
				AddDataSeries(Data.BarsPeriodType.Tick, 1);
            }
        }

        protected override void OnBarUpdate()
        {
            //Entry
            #region
            //MACD Crossover and RSI confirmation
            if (CrossAbove(macd.Default, 0, 1)
				&& macd.Default[0] > macd.Avg[0])
            {
                EnterLong();
            }
            #endregion
            //Exit
            #region
            //Long
            if (Position.MarketPosition == MarketPosition.Long)
            {
                //Sell on MACD Cross Down
                if (CrossBelow(macd.Default, macd.Avg, 1))
                {
                    ExitLong("Cross below MACD Exit", "Buy");
                }
            }
            //Short
            if (Position.MarketPosition == MarketPosition.Short)
            {
                //Sell on MACD Cross Above
                if (CrossBelow(macd.Default, macd.Avg, 1))
                {
                    ExitShort();
                    //Or sell if RSI <= 10
                }
                else if (rsi[0] >= 85)
                {
                    ExitShort();
                    //Or sell if 3:50
                }
                else if (Time[0].Hour == 15 && Time[0].Minute >= 50)
                {
                    ExitShort();
                }

            }
            #endregion
        }
    }
}
