namespace OptimizerInterface
{
    public interface IOptimizer
    {
        public static abstract IOptimizer GetInstance();

        public void Init(int countGenset, int countBattery = 0, int countSolarPanelGroup = 0, int coutWindTurbine = 0);
        
        public void AddGenset(IGenset genset);
        public void AddtBattery(IBattery battery);
        public void AddSolarPanel(ISolarPanels solarPanels);
        public void AddWindTurbine(IWindTurbine windTurbine);

        /// <summary>
        /// Optimize according to the ressources added
        /// </summary>
        /// <returns>error message or null</returns>
        public string Optimize();
    }
}
