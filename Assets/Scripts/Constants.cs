using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static string playerTag = "Player";
    public static string cameraWallTag = "CameraWall";
    public static int maxPlayerAmount = 4;
    public static string levelName = "BroSpeedLevel1";
    public static string powerupLabel = "Powerup:";

    // Dictionary of colors to associate with each player 
    public static Dictionary<string, Color> PlayerColors = new Dictionary<string, Color>
    {
        {"Nathan", new Color(.9f, .8f, .18f)},
        {"NathanTextTop", new Color(1f, 0.82f, 0.22f)},
        {"NathanTextBottom", new Color(0.94f, 0.6f, 0.06f)},
        {"NathanBatteryTop", new Color(1f, 0.91f, 0.63f)},
        {"NathanBatteryBottom", new Color(1f, 0.82f, 0.22f)},
        {"NathanBatteryBackground", new Color(0.94f, 0.6f, 0.06f)},

        {"OwOBot", new Color(.9f, .2f, .9f)},
        {"OwOBotTextTop", new Color(0.89f, 0.43f, 0.85f)},
        {"OwOBotTextBottom", new Color(0.79f, 0.23f, 0.6f)},
        {"OwOBotBatteryTop", new Color(0.99f, 0.69f, 1f)},
        {"OwOBotBatteryBottom", new Color(0.89f, 0.43f, 0.85f)},
        {"OwOBotBatteryBackground", new Color(0.79f, 0.23f, 0.6f)},

        {"LilBlu", new Color(.2f, .9f, .9f)},
        {"LilBluTextTop", new Color(0.09f, 0.63f, 0.62f)},
        {"LilBluTextBottom", new Color(0f, 0.32f, 0.3f)},
        {"LilBluBatteryTop", new Color(0.12f, 0.99f, 0.97f)},
        {"LilBluBatteryBottom", new Color(0.09f, 0.63f, 0.62f)},
        {"LilBluBatteryBackground", new Color(0f, 0.32f, 0.3f)},

        {"MechaHarpy", new Color(.53f, .17f, .7f)},
        {"MechaHarpyTextTop", new Color(0.75f, 0.15f, 1f)},
        {"MechaHarpyTextBottom", new Color(0.48f, 0f, 0.65f)},
        {"MechaHarpyBatteryTop", new Color(0.88f, 0.63f, 1f)},
        {"MechaHarpyBatteryBottom", new Color(0.75f, 0.15f, 1f)},
        {"MechaHarpyBatteryBackground", new Color(0.48f, 0f, 0.65f)}
    };

}
