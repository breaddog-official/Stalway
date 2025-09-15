#if UNITY_EDITOR
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Breaddog.Extensions
{
  public static class UnitsRegistrator
  {
    [InitializeOnLoadMethod]
    public static void AddCustomUnit()
    {
      // From CS2, where 1 unit = 2 cm
      UnitNumberUtility.AddCustomUnit(
        name: "UnitsPerSecond",
        symbols: new string[] { "u/s" },
        unitCategory: UnitCategory.Speed,
        multiplier: 1m * 50m);

      UnitNumberUtility.AddCustomUnit(
        name: "RoundsPerMinuteFreq",
        symbols: new string[] { "rpm" },
        unitCategory: UnitCategory.Frequency,
        multiplier: 1m * 60m);

      UnitNumberUtility.AddCustomUnit(
        name: "Dollars",
        symbols: new string[] { "$" },
        unitCategory: UnitCategory.Energy, // Most unused group
        multiplier: 0.000000028m); // Average price per joule

      UnitNumberUtility.AddCustomUnit(
        name: "RoundsPerMinute",
        symbols: new string[] { "rpm" },
        unitCategory: UnitCategory.Time,
        convertToBase: rpm => 60m / rpm,
        convertFromBase: delay => 60m / delay);
    }
  }
}
#endif