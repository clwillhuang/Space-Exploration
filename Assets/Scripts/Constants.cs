/// <summary>
/// The values and parameters applicable for the game.
/// </summary>
public class Constants {

    /// <summary>
    /// Normal realw-world seconds per day in-game.
    /// </summary>
    public const float SECONDS_PER_HOUR = 0.1f;

    /// <summary>
    /// The maximum possible X coordinate for objects in a system
    /// </summary>
    public const float MAX_COORDINATE_X = 325f;

    /// <summary>
    /// The maximum possible Y coordinate for objects in a system 
    /// </summary>
    public const float MAX_COORDINATE_Y = 325f;

    /// <summary>
    /// The number of jump points per system
    /// </summary>
    public const int JUMP_POINTS_PER_SYSTEM = 8;

    /// <summary>
    /// % probability for a jump point to connect to a completely new system
    /// </summary>
    public const float NEW_SYSTEM_GENERATION_CHANCE = 0.2f;

    /// <summary>
    /// The chance for a newly generated planet to be fit for human habitation. 
    /// </summary>
    public const float HABITABLE_PLANET_GENERATION_CHANCE = 0.1f;

    /// <summary>
    /// The default time, in seconds, that a newly displayed message will be visible
    /// </summary>
    public const float MESSAGE_VISIBLE = 5f;

    /// <summary>
    /// The number of inhabitants to generate 1 point of unhappiness.
    /// </summary>
    public const float UNHAPPINESS = 5000000f;

    /// <summary>
    /// The unhappiness level where player suffers maximal productivity penalty.
    /// </summary>
    public const float UNHAPPINESS_PENALTY = -20f;

    /// <summary>
    /// The daily growth rate of a population.
    /// </summary>
    public const float DAILY_POP_GROWTH = 1.0004f;

    /// <summary>
    /// Amount of fuel consumed by spaceship in motion per realworld second.
    /// </summary>
    public const float FUEL_CONSUMPTION_PER_SECOND = 700f;

    /// <summary>
    /// Percentage speed penalty suffered by ships which do not have fuel.
    /// </summary>
    public const float EMPTY_FUEL_PENALTY = 5f;

    /// <summary>
    /// Default survey points required to survey a jump point.
    /// </summary>
    public const float JUMP_POINT_SURVEY_POINTS = 200f;

    /// <summary>
    /// The number of survey points required for a planetary mineral survey, per 10^23 kg of its mass
    /// </summary>
    public const float PLANET_SURVEY_POINTS_PER_1023 = 30f;

    /// <summary>
    /// Condition points that ship degrades each real-time second.
    /// </summary>
    public const float SHIP_DEGRADATION_PER_SECOND = 0.1f;

    /// <summary>
    /// The default number of research labs the player starts off with.
    /// </summary>
    public const int STARTING_RESEARCH_LABS = 30;

    /// <summary>
    /// The default starting population for a player's homeworld.
    /// </summary>
    public const long HOMEWORLD_STARTING_POPULATION = 50000000;

    /// <summary>
    /// The daily money cost of each building.
    /// </summary>
    public const float PER_BUILDING_COST = 1f; 
}
