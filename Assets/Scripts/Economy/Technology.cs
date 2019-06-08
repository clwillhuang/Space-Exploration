namespace Economy
{
    /// <summary>
    /// Class for a researchable technology.
    /// </summary>
    public class Technology 
    {
        public delegate void UnlockEffect(PlayerSession player, float value); 
    
        /// <summary>
        /// The name of this technology / research. 
        /// </summary>
        public readonly string technologyName;
        
        /// <summary>
        /// A brief description of this technology. 
        /// </summary>
        public readonly string description;

        /// <summary>
        /// The prerequisite tech required to begin research of this project. "" if none.
        /// </summary>
        public readonly string prerequisiteTech; 
        
        /// <summary>
        /// The research pt cost of this tech. 
        /// </summary>
        public readonly float cost;

        /// <summary>
        /// The value of improvement in the appropriate statistic when this tech is researched. 
        /// </summary>
        public readonly float value; 

        /// <summary>
        /// The type of unlock effect of this technology. 
        /// </summary>
        public readonly UnlockEffect unlockEffect; 
        
        /// <summary>
        /// Constructor for a new technology. 
        /// </summary>
        /// <param name="_technologyName">Name.</param>
        /// <param name="_description">Description/</param>
        /// <param name="_cost">Research pt cost.</param>
        /// <param name="_prerequisiteTech">Prereq tech, "" if none</param>
        /// <param name="_unlockEffect">Type of unlock effect</param>
        /// <param name="_value">Value of effect</param>
        public Technology(string _technologyName, string _description, float _cost, string _prerequisiteTech, UnlockEffect _unlockEffect, float _value)
        {
            technologyName = _technologyName;
            description = _description; 
            cost = _cost; 
            unlockEffect = _unlockEffect;
            prerequisiteTech = _prerequisiteTech;
            value = _value;

            description += "\n";

            if (unlockEffect == MoneyGrant)
                description += "Money : " + value.ToString("+0;-#"); 
            else if (unlockEffect == ShipSpeed)
                description += "Speed of ships : " + value.ToString("+0;-#");
            else if (unlockEffect == MiningSpeed)
                description += "Mining speed per mine per day : " + value.ToString("+0;-#");
            else if (unlockEffect == ResearchSpeed)
                description += "Research speed per mine per day : " + value.ToString("+0;-#");
            else if (unlockEffect == RefinerySpeed)
                description += "Fuel refined per refinery per day : " + value.ToString("+0;-#");
            else if (unlockEffect == Economy)
                description += "Money per financial center per day : " + value.ToString("+0;-#");
            else if (unlockEffect == SurveySpeed)
                description += "Survey speed : " + value.ToString("+0;-#");
            else if (unlockEffect == FactoryProduction)
                description += "Factory Production Pts per factory per day : " + value.ToString("+0;-#");
            else if (unlockEffect == ShipyardProduction)
                description += "Shipyard Production Pts per factory per day : " + value.ToString("+0;-#");

        }

        /// <summary>
        /// Unlock this technology.
        /// </summary>
        public void Unlock()
        {
            unlockEffect.Invoke(StateManager.currentSM.currentSession, value); 
        }
        
        /// <summary>
        /// Add money.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void MoneyGrant(PlayerSession player, float value)
        {
            player.money += (int)value; 
        }

        /// <summary>
        /// Improve ship speed.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void ShipSpeed(PlayerSession player, float value)
        {
            player.SpaceshipSpeed += value;
        }

        /// <summary>
        /// Improve mining speed.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void MiningSpeed(PlayerSession player, float value)
        {
            player.MiningPerMine += value; 
        }

        /// <summary>
        /// Improve research speed.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void ResearchSpeed(PlayerSession player, float value)
        {
            player.ResearchSpeedPerResearchLab += value; 
        }

        /// <summary>
        /// Improve refinery efficiency.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void RefinerySpeed(PlayerSession player, float value)
        {
            player.RefiningEfficiency += value;
        }

        /// <summary>
        /// Improve financial centre profit.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void Economy(PlayerSession player, float value)
        {
            player.MoneyPerFinancialCenter += (int)value;
        }

        /// <summary>
        /// Improve survey speed by ships.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void SurveySpeed(PlayerSession player, float value)
        {
            player.SurveySpeed += value;
        }

        /// <summary>
        /// Improve production rate.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void FactoryProduction(PlayerSession player, float value)
        {
            player.ProductionSpeedPerFactory += value;
        }

        /// <summary>
        /// Improve shipyard production rate.
        /// </summary>
        /// <param name="player">Player which researched this tech.</param>
        /// <param name="value">Value of improvement.</param>
        public static void ShipyardProduction(PlayerSession player, float value)
        {
            player.ProductionSpeedPerShipyard += value;
        }

    }
}
