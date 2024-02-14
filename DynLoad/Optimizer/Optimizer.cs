using OptimizerInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optimizer
{
    public class Optimizer : IOptimizer
    {
        public int CountGenset { get; set; }
        public int CountBattery { get; set; }
        public int CountSolarPanelGroup { get; set; }
        public int CountWindTurbine { get; set; }

        public void Init(int countGenset, int countBattery = 0, int countSolarPanelGroup = 0, int coutWindTurbine = 0)
        {
            CountGenset = countGenset;
            CountBattery = countBattery;
            CountSolarPanelGroup = countSolarPanelGroup;
            CountWindTurbine = coutWindTurbine;
        }

        public string Optimize()
        {
            return "Not yet implemented";
        }

        public void AddtBattery(IBattery battery)
        {
            throw new NotImplementedException();
        }

        public void AddGenset(IGenset genset)
        {
            throw new NotImplementedException();
        }

        public void AddSolarPanel(ISolarPanels solarPanels)
        {
            throw new NotImplementedException();
        }

        public void AddWindTurbine(IWindTurbine windTurbine)
        {
            throw new NotImplementedException();
        }

        public static IOptimizer GetInstance()
        {
            return new Optimizer();
        }
    }
}
