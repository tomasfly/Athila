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
//makes profit with TSLA apparently. Maybe each ticker has its own EMA value
namespace NinjaTrader.NinjaScript.Strategies {
  public class PriceCrossAboveSMA9Optimize : Strategy {
    private EMA EMA1;
    private EMA EMA2;
    private EMA EMA3;
    private int crossedAbove = 0;
    private bool crossed = false;

    private int candlesToWaitUntilBuy = 5;

    protected override void OnStateChange () {
      if (State == State.SetDefaults) {
        Description = @"Enter the description for your new custom Strategy here.";
        Name = "PriceCrossAboveSMA9Optimize";
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
      } else if (State == State.Configure) { } else if (State == State.DataLoaded) {
        EMA1 = EMA (Close, Fast);
        AddChartIndicator (EMA1);
      }
    }

    protected override void OnBarUpdate () {

      if (BarsInProgress != 0)
        return;

      if (CurrentBars[0] < 1)
        return;

      if (!crossed) {
        if (CrossAbove (Close, EMA1, 1)) {
          crossed = true;
        }
      }

      // Set 1
      if (crossed) {
        if (Close[0] > EMA1[0]) {
          crossedAbove++;
        }

        if (crossedAbove >= candlesToWaitUntilBuy) {
          EnterLong (10);
        }
      }

      // Set 2
      if (CrossBelow (Close, EMA1, 1)) {
        ExitLong (10);
        crossedAbove = 0;
        crossed = false;
      }
    }

    // No upper bound, lower bound of 1
    [Range (-10, int.MaxValue), NinjaScriptProperty]
    [Display (ResourceType = typeof (Custom.Resource), Name = "Fast", GroupName = "NinjaScriptStrategyParameters", Order = 1)]
    public int Fast { get; set; }
  }
}