﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Funbit.Ets.Telemetry.Server.Data.Reader;
using Funbit.Ets.Telemetry.Server.Helpers;

namespace Funbit.Ets.Telemetry.Server.Data
{
    class Ets2TelemetryData : IEts2TelemetryData
    {
        Box<Ets2TelemetryStructure> _rawData;
        
        public void Update(Ets2TelemetryStructure rawData)
        {
            _rawData = new Box<Ets2TelemetryStructure>(rawData);
        }

        internal static DateTime SecondsToDate(int seconds)
        {
            if (seconds < 0) seconds = 0;
            return new DateTime((long)seconds * 10000000, DateTimeKind.Utc);
        }

        internal static DateTime MinutesToDate(int minutes)
        {
            return SecondsToDate(minutes * 60);
        }

        internal static string BytesToString(byte[] bytes)
        {
            if (bytes == null)
                return string.Empty;
            var length = Array.FindIndex(bytes, b => b == 0);
            if (length == -1) length = bytes.Length;
            return Encoding.UTF8.GetString(bytes, 0, length);
        }

        public IEts2Game Game => new Ets2Game(_rawData);
        public IEts2Truck Truck => new Ets2Truck(_rawData);

        public IEts2Trailer Trailer1 => new Ets2Trailer(_rawData, 0);
        public IEts2Trailer Trailer2 => new Ets2Trailer(_rawData, 1);
        public IEts2Trailer Trailer3 => new Ets2Trailer(_rawData, 2);
        public IEts2Trailer Trailer4 => new Ets2Trailer(_rawData, 3);
        public IEts2Trailer Trailer5 => new Ets2Trailer(_rawData, 4);
        public IEts2Trailer Trailer6 => new Ets2Trailer(_rawData, 5);
        public IEts2Trailer Trailer7 => new Ets2Trailer(_rawData, 6);
        public IEts2Trailer Trailer8 => new Ets2Trailer(_rawData, 7);
        public IEts2Trailer Trailer9 => new Ets2Trailer(_rawData, 8);
        public IEts2Trailer Trailer10 => new Ets2Trailer(_rawData, 9);
        public IEts2Job Job => new Ets2Job(_rawData);
        public IEts2Cargo Cargo => new Ets2Cargo(_rawData);
        public IEts2Navigation Navigation => new Ets2Navigation(_rawData);
        public IEts2FinedGameplayEvent FinedEvent => new Ets2FinedGameplayEvent(_rawData);
        public IEts2JobGameplayEvent JobEvent => new Ets2JobGameplayEvent(_rawData);
        public IEts2TollgateGameplayEvent TollgateEvent => new Ets2TollgateGameplayEvent(_rawData);
        public IEts2FerryGameplayEvent FerryEvent => new Ets2FerryGameplayEvent(_rawData);
        public IEts2TrainGameplayEvent TrainEvent => new Ets2TrainGameplayEvent(_rawData);
    }

    class Ets2Game : IEts2Game
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2Game(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public bool Connected => _rawData.Struct.ets2_telemetry_plugin_revision != 0 &&
                                 Ets2ProcessHelper.IsEts2Running &&
                                 _rawData.Struct.timeAbsolute != 0;

        public string GameName => Ets2ProcessHelper.LastRunningGameName;
        public bool Paused => _rawData.Struct.paused != 0;
        public DateTime Time => Ets2TelemetryData.MinutesToDate(_rawData.Struct.timeAbsolute);
        public float TimeScale => _rawData.Struct.localScale;
        public DateTime NextRestStopTime => Ets2TelemetryData.MinutesToDate(_rawData.Struct.nextRestStop);
        public string Version => $"{_rawData.Struct.ets2_version_major}.{_rawData.Struct.ets2_version_minor}";
        public string TelemetryPluginVersion => _rawData.Struct.ets2_telemetry_plugin_revision.ToString();

        public int MaxTrailerCount => _rawData.Struct.maxTrailerCount;
    }

    class Ets2Vector : IEts2Vector
    {
        public float X { get; }
        public float Y { get; }
        public float Z { get; }

        public Ets2Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }

    class Ets2Placement : IEts2Placement
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }
        public double Heading { get; }
        public double Pitch { get; }
        public double Roll { get; }

        public Ets2Placement(double x, double y, double z,
            double heading, double pitch, double roll)
        {
            X = x;
            Y = y;
            Z = z;
            Heading = heading;
            Pitch = pitch;
            Roll = roll;
        }
    }

    class Ets2Truck : IEts2Truck
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2Truck(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public string Id => Ets2TelemetryData.BytesToString(_rawData.Struct.truckMakeId);
        public string Make => Ets2TelemetryData.BytesToString(_rawData.Struct.truckMake);
        public string Model => Ets2TelemetryData.BytesToString(_rawData.Struct.truckModel);

        /// <summary>
        /// Truck speed in km/h.
        /// </summary>
        public float Speed => _rawData.Struct.speed * 3.6f;

        /// <summary>
        /// Cruise control speed in km/h.
        /// </summary>
        public float CruiseControlSpeed => _rawData.Struct.cruiseControlSpeed * 3.6f;

        public bool CruiseControlOn => _rawData.Struct.cruiseControl != 0;
        public float Odometer => _rawData.Struct.truckOdometer;
        public int Gear => _rawData.Struct.gear;
        public int DisplayedGear => _rawData.Struct.displayedGear;
        public int ForwardGears => (int)_rawData.Struct.gearsForward;
        public int ReverseGears => (int)_rawData.Struct.gearsReverse;
        public string ShifterType => Ets2TelemetryData.BytesToString(_rawData.Struct.shifterType);
        public float EngineRpm => _rawData.Struct.engineRpm;
        public float EngineRpmMax => _rawData.Struct.engineRpmMax;
        public float Fuel => _rawData.Struct.fuel;
        public float FuelCapacity => _rawData.Struct.fuelCapacity;
        public float FuelAverageConsumption => _rawData.Struct.fuelAvgConsumption;
        public float FuelWarningFactor => _rawData.Struct.fuelWarningFactor;
        public bool FuelWarningOn => _rawData.Struct.fuelWarning != 0;
        public float WearEngine => _rawData.Struct.wearEngine;
        public float WearTransmission => _rawData.Struct.wearTransmission;
        public float WearCabin => _rawData.Struct.wearCabin;
        public float WearChassis => _rawData.Struct.wearChassis;
        public float WearWheels => _rawData.Struct.wearWheels;
        public float UserSteer => _rawData.Struct.userSteer;
        public float UserThrottle => _rawData.Struct.userThrottle;
        public float UserBrake => _rawData.Struct.userBrake;
        public float UserClutch => _rawData.Struct.userClutch;
        public float GameSteer => _rawData.Struct.gameSteer;
        public float GameThrottle => _rawData.Struct.gameThrottle;
        public float GameBrake => _rawData.Struct.gameBrake;
        public float GameClutch => _rawData.Struct.gameClutch;
        public int ShifterSlot => (int)_rawData.Struct.shifterSlot;

        //public int ShifterToggle
        //{
        //    get { return _rawData.Struct.shifterToggle; }
        //}

        public bool EngineOn => _rawData.Struct.engineEnabled != 0;
        public bool ElectricOn => _rawData.Struct.electricEnabled != 0;
        public bool WipersOn => _rawData.Struct.wipers != 0;
        public int RetarderBrake => (int)_rawData.Struct.retarderBrake;
        public int RetarderStepCount => (int)_rawData.Struct.retarderStepCount;
        public bool ParkBrakeOn => _rawData.Struct.parkBrake != 0;
        public bool MotorBrakeOn => _rawData.Struct.motorBrake != 0;
        public float BrakeTemperature => _rawData.Struct.brakeTemperature;
        public float Adblue => _rawData.Struct.adblue;
        public float AdblueCapacity => _rawData.Struct.adblueCapacity;
        public float AdblueAverageConsumption => 0.0F; // Removed in SDK 1.9
        public bool AdblueWarningOn => _rawData.Struct.adblueWarning != 0;
        public float AirPressure => _rawData.Struct.airPressure;
        public bool AirPressureWarningOn => _rawData.Struct.airPressureWarning != 0;
        public float AirPressureWarningValue => _rawData.Struct.airPressureWarningValue;
        public bool AirPressureEmergencyOn => _rawData.Struct.airPressureEmergency != 0;
        public float AirPressureEmergencyValue => _rawData.Struct.airPressureEmergencyValue;
        public float OilTemperature => _rawData.Struct.oilTemperature;
        public float OilPressure => _rawData.Struct.oilPressure;
        public bool OilPressureWarningOn => _rawData.Struct.oilPressureWarning != 0;
        public float OilPressureWarningValue => _rawData.Struct.oilPressureWarningValue;
        public float WaterTemperature => _rawData.Struct.waterTemperature;
        public bool WaterTemperatureWarningOn => _rawData.Struct.waterTemperatureWarning != 0;
        public float WaterTemperatureWarningValue => _rawData.Struct.waterTemperatureWarningValue;
        public float BatteryVoltage => _rawData.Struct.batteryVoltage;
        public bool BatteryVoltageWarningOn => _rawData.Struct.batteryVoltageWarning != 0;
        public float BatteryVoltageWarningValue => _rawData.Struct.batteryVoltageWarningValue;
        public float LightsDashboardValue => _rawData.Struct.lightsDashboard;
        public bool LightsDashboardOn => _rawData.Struct.lightsDashboard > 0;
        public bool BlinkerLeftActive => _rawData.Struct.blinkerLeftActive != 0;
        public bool BlinkerRightActive => _rawData.Struct.blinkerRightActive != 0;
        public bool BlinkerLeftOn => _rawData.Struct.blinkerLeftOn != 0;
        public bool BlinkerRightOn => _rawData.Struct.blinkerRightOn != 0;
        public bool LightsParkingOn => _rawData.Struct.lightsParking != 0;
        public bool LightsBeamLowOn => _rawData.Struct.lightsBeamLow != 0;
        public bool LightsBeamHighOn => _rawData.Struct.lightsBeamHigh != 0;
        public bool LightsAuxFrontOn => _rawData.Struct.lightsAuxFront != 0;
        public bool LightsAuxRoofOn => _rawData.Struct.lightsAuxRoof != 0;
        public bool LightsBeaconOn => _rawData.Struct.lightsBeacon != 0;
        public bool LightsBrakeOn => _rawData.Struct.lightsBrake != 0;
        public bool LightsReverseOn => _rawData.Struct.lightsReverse != 0;

        public IEts2Placement Placement => new Ets2Placement(
            _rawData.Struct.coordinateX,
            _rawData.Struct.coordinateY,
            _rawData.Struct.coordinateZ,
            _rawData.Struct.rotationX,
            _rawData.Struct.rotationY,
            _rawData.Struct.rotationZ);

        public IEts2Vector Acceleration => new Ets2Vector(
            _rawData.Struct.accelerationX,
            _rawData.Struct.accelerationY,
            _rawData.Struct.accelerationZ);

        public IEts2Vector Head => new Ets2Vector(
            _rawData.Struct.headPositionX,
            _rawData.Struct.headPositionY,
            _rawData.Struct.headPositionZ);

        public IEts2Vector Cabin => new Ets2Vector(
            _rawData.Struct.cabinPositionX,
            _rawData.Struct.cabinPositionY,
            _rawData.Struct.cabinPositionZ);

        public IEts2Vector Hook => new Ets2Vector(
            _rawData.Struct.hookPositionX,
            _rawData.Struct.hookPositionY,
            _rawData.Struct.hookPositionZ);

        public string LicensePlate => Ets2TelemetryData.BytesToString(_rawData.Struct.truckLicensePlate);

        public string LicensePlateCountryId =>
            Ets2TelemetryData.BytesToString(_rawData.Struct.truckLicensePlateCountryId);

        public string LicensePlateCountry => Ets2TelemetryData.BytesToString(_rawData.Struct.truckLicensePlateCountry);

        /*
        public IEts2GearSlot[] GearSlots
        {
            get
            {
                var array = new IEts2GearSlot[_rawData.Struct.selectorCount];
                for (int i = 0; i < array.Length; i++)
                    array[i] = new Ets2GearSlot(_rawData, i);
                return array;
            }
        }
                
        public IEts2Wheel[] Wheels
        {
            get
            {
                var array = new IEts2Wheel[_rawData.Struct.wheelCount];
                for (int i = 0; i < array.Length; i++)
                    array[i] = new Ets2Wheel(_rawData, i);
                return array;
            }
        }
        */
    }

    class Ets2Trailer : IEts2Trailer
    {
        readonly Box<Ets2TelemetryStructure> _rawData;
        readonly int _trailerNumber;

        public Ets2Trailer(Box<Ets2TelemetryStructure> rawData, int trailerNumber)
        {
            if (trailerNumber < 0 && trailerNumber > 9)
            {
                throw new ArgumentException($"trailerNumber must be between 0-9. Found: {trailerNumber}");
            }

            _rawData = rawData;
            _trailerNumber = trailerNumber;
        }

        public int TrailerNumber => _trailerNumber + 1;

        public bool Attached => _rawData.Struct.trailer0attached != 0 && 
                                (byte)_rawData.Struct.GetType().GetField($"trailer{_trailerNumber}attached").GetValue(_rawData.Struct) != 0;

        public bool Present => !string.IsNullOrEmpty(Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType().GetField($"trailer{_trailerNumber}id").GetValue(_rawData.Struct)));

        // ReSharper disable once PossibleNullReferenceException
        public string Id => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType().GetField($"trailer{_trailerNumber}id").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string Name => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType().GetField($"trailer{_trailerNumber}name").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public float WearWheels => (float) _rawData.Struct.GetType().GetField($"trailer{_trailerNumber}wearWheels").GetValue(_rawData.Struct);

        // ReSharper disable once PossibleNullReferenceException
        public float WearChassis => (float)_rawData.Struct.GetType().GetField($"trailer{_trailerNumber}wearChassis").GetValue(_rawData.Struct);

        // ReSharper disable once PossibleNullReferenceException
        public float CargoDamage => (float)_rawData.Struct.GetType().GetField($"trailer{_trailerNumber}cargoDamage").GetValue(_rawData.Struct);

        // ReSharper disable once PossibleNullReferenceException
        public string CargoAccessoryId => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}CargoAccessoryId").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string BrandId => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}brandId").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string Brand => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}brand").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string BodyType => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}bodyType").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string Cargo => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}bodyType").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string LicensePlate => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}licensePlate").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string LicensePlateCountry => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}licensePlateCountry").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string LicensePlateCountryId => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}licensePlateCountryId").GetValue(_rawData.Struct));

        // ReSharper disable once PossibleNullReferenceException
        public string ChainType => Ets2TelemetryData.BytesToString((byte[])_rawData.Struct.GetType()
            .GetField($"trailer{_trailerNumber}chainType").GetValue(_rawData.Struct));

        public IEts2Placement Placement => new Ets2Placement(
            _rawData.Struct.trailer0worldX,
            _rawData.Struct.trailer0worldY,
            _rawData.Struct.trailer0worldZ,
            _rawData.Struct.trailer0rotationX,
            _rawData.Struct.trailer0rotationY,
            _rawData.Struct.trailer0rotationZ);
    }

    class Ets2Navigation : IEts2Navigation
    {
        readonly Box<Ets2TelemetryStructure> _rawData;
        
        public Ets2Navigation(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }
        
        public DateTime EstimatedTime => Ets2TelemetryData.SecondsToDate((int)_rawData.Struct.navigationTime);
        public int EstimatedDistance => (int)_rawData.Struct.navigationDistance;
        public int SpeedLimit => _rawData.Struct.navigationSpeedLimit > 0 ? (int)Math.Round(_rawData.Struct.navigationSpeedLimit * 3.6f) : 0;
    }

    class Ets2Job : IEts2Job
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2Job(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public int Income => (int)_rawData.Struct.jobIncome;
        public DateTime DeadlineTime => Ets2TelemetryData.MinutesToDate((int)_rawData.Struct.jobDeadline);
        public DateTime RemainingTime 
        {
            get
            {
                if (_rawData.Struct.jobDeadline > 0)
                    return Ets2TelemetryData.MinutesToDate((int)(_rawData.Struct.jobDeadline - _rawData.Struct.timeAbsolute));
                return Ets2TelemetryData.MinutesToDate(0);
            }
        }

        public string SourceCity => Ets2TelemetryData.BytesToString(_rawData.Struct.jobCitySource);
        public string SourceCompany => Ets2TelemetryData.BytesToString(_rawData.Struct.jobCompanySource);
        public string DestinationCity => Ets2TelemetryData.BytesToString(_rawData.Struct.jobCityDestination);
        public string DestinationCompany => Ets2TelemetryData.BytesToString(_rawData.Struct.jobCompanyDestination);

        public bool SpecialTransport => _rawData.Struct.specialJob != 0;

        public string JobMarket => Ets2TelemetryData.BytesToString(_rawData.Struct.jobMarket);
    }

    class Ets2Cargo : IEts2Cargo
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2Cargo(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public bool CargoLoaded => _rawData.Struct.isCargoLoaded != 0;
        public string CargoId => Ets2TelemetryData.BytesToString(_rawData.Struct.cargoId);
        public string Cargo => Ets2TelemetryData.BytesToString(_rawData.Struct.cargo);
        public float Mass => _rawData.Struct.cargoMass;
        public float UnitMass => _rawData.Struct.unitMass;
        public int UnitCount => _rawData.Struct.unitCount;
        public float Damage => _rawData.Struct.cargoDamage;
    }

    class Ets2FinedGameplayEvent : IEts2FinedGameplayEvent
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2FinedGameplayEvent(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public string FineOffense => Ets2TelemetryData.BytesToString(_rawData.Struct.fineOffence);
        public int FineAmount => (int) _rawData.Struct.fineAmount;
        public bool Fined => _rawData.Struct.fined != 0;
    }

    class Ets2JobGameplayEvent : IEts2JobGameplayEvent
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2JobGameplayEvent(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public bool JobFinished => _rawData.Struct.jobFinished != 0;
        public bool JobCancelled => _rawData.Struct.jobCancelled != 0;
        public bool JobDelivered => _rawData.Struct.jobDelivered != 0;
        public int CancelPenalty => (int)_rawData.Struct.jobCancelledPenalty;
        public int Revenue => (int)_rawData.Struct.jobDeliveredRevenue;
        public int EarnedXp => _rawData.Struct.jobDeliveredEarnedXp;
        public float CargoDamage => _rawData.Struct.cargoDamage;
        public int Distance => (int)_rawData.Struct.navigationDistance;
        public DateTime DeliveryTime => Ets2TelemetryData.MinutesToDate((int)_rawData.Struct.jobDeliveredDeliveryTime);
        public bool AutoparkUsed => _rawData.Struct.jobDelieveredAutoparkUsed != 0;
        public bool AutoloadUsed => _rawData.Struct.jobDeliveredAutoloadUsed != 0;
    }

    class Ets2TollgateGameplayEvent : IEts2TollgateGameplayEvent
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2TollgateGameplayEvent(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public bool TollgateUsed => _rawData.Struct.tollgate != 0;
        public int PayAmount => (int)_rawData.Struct.tollgatePayAmount;
    }

    class Ets2FerryGameplayEvent : IEts2FerryGameplayEvent
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2FerryGameplayEvent(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public bool FerryUsed => _rawData.Struct.ferry != 0;
        public string SourceName => Ets2TelemetryData.BytesToString(_rawData.Struct.ferrySourceName);
        public string TargetName => Ets2TelemetryData.BytesToString(_rawData.Struct.ferryTargetName);
        public string SourceId => Ets2TelemetryData.BytesToString(_rawData.Struct.ferrySourceId);
        public string TargetId => Ets2TelemetryData.BytesToString(_rawData.Struct.ferryTargetId);
        public int PayAmount => (int) _rawData.Struct.ferryPayAmount;
    }

    class Ets2TrainGameplayEvent : IEts2TrainGameplayEvent
    {
        readonly Box<Ets2TelemetryStructure> _rawData;

        public Ets2TrainGameplayEvent(Box<Ets2TelemetryStructure> rawData)
        {
            _rawData = rawData;
        }

        public bool TrainUsed => _rawData.Struct.train != 0;
        public string SourceName => Ets2TelemetryData.BytesToString(_rawData.Struct.trainSourceName);
        public string TargetName => Ets2TelemetryData.BytesToString(_rawData.Struct.trainTargetName);
        public string SourceId => Ets2TelemetryData.BytesToString(_rawData.Struct.trainSourceId);
        public string TargetId => Ets2TelemetryData.BytesToString(_rawData.Struct.trainTargetId);
        public int PayAmount => (int)_rawData.Struct.trainPayAmount;
    }

    /*
    class Ets2Wheel : IEts2Wheel
    {
        public Ets2Wheel(Box<Ets2TelemetryStructure> rawData, int wheelIndex)
        {
            Simulated = rawData.Struct.wheelSimulated[wheelIndex] != 0;
            Steerable = rawData.Struct.wheelSteerable[wheelIndex] != 0;
            Radius = rawData.Struct.wheelRadius[wheelIndex];
            Position = new Ets2Vector(
                rawData.Struct.wheelPositionX[wheelIndex],
                rawData.Struct.wheelPositionY[wheelIndex],
                rawData.Struct.wheelPositionZ[wheelIndex]);
            Powered = rawData.Struct.wheelPowered[wheelIndex] != 0;
            Liftable = rawData.Struct.wheelLiftable[wheelIndex] != 0;
        }

        public bool Simulated { get; private set; }
        public bool Steerable { get; private set; }
        public bool Powered { get; private set; }
        public bool Liftable { get; private set; }
        public float Radius { get; private set; }
        public IEts2Vector Position { get; private set; }
    }
    
    class Ets2GearSlot : IEts2GearSlot
    {
        public Ets2GearSlot(Box<Ets2TelemetryStructure> rawData, int slotIndex)
        {
            Gear = rawData.Struct.slotGear[slotIndex];
            HandlePosition = (int)rawData.Struct.slotHandlePosition[slotIndex];
            SlotSelectors = (int)rawData.Struct.slotSelectors[slotIndex];
        }

        public int Gear { get; private set; }
        public int HandlePosition { get; private set; }
        public int SlotSelectors { get; private set; }
    }
    */

    class Box<T> where T : struct 
    {
        public T Struct { get; set; }

        public Box(T @struct)
        {
            Struct = @struct;
        }
    }
}