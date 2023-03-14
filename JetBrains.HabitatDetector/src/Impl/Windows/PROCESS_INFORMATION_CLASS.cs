using System.Diagnostics.CodeAnalysis;

namespace JetBrains.HabitatDetector.Impl.Windows
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal enum PROCESS_INFORMATION_CLASS
  {
    ProcessMemoryPriority,          // MEMORY_PRIORITY_INFORMATION
    ProcessMemoryExhaustionInfo,    // PROCESS_MEMORY_EXHAUSTION_INFO
    ProcessAppMemoryInfo,           // APP_MEMORY_INFORMATION
    ProcessInPrivateInfo,           // BOOLEAN
    ProcessPowerThrottling,         // PROCESS_POWER_THROTTLING_STATE
    ProcessReservedValue1,          // Used to be for ProcessActivityThrottlePolicyInfo
    ProcessTelemetryCoverageInfo,   // TELEMETRY_COVERAGE_POINT
    ProcessProtectionLevelInfo,     // PROCESS_PROTECTION_LEVEL_INFORMATION
    ProcessLeapSecondInfo,          // PROCESS_LEAP_SECOND_INFO
    ProcessMachineTypeInfo,         // PROCESS_MACHINE_INFORMATION
    ProcessInformationClassMax
  }
}