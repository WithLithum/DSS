using DSS.Utils;
using GTA;
using WithLithum.NativeWrapper;

namespace DSS.Threads
{
    class PlayerController
    {
        private static Vehicle _prevVehicle;
        private static ManagedVehicle _activeVehicle;
        private static bool _isManualActive;
        private static bool _isHornActive;

        internal static void Tick()
        {
            Ped playerPed = Game.Player.Character;
            if (!playerPed.IsSittingInVehicle()) return;
            
            Vehicle veh = playerPed.CurrentVehicle;
            if (veh == null || veh.GetPedOnSeat(VehicleSeat.Driver) != playerPed) return;
            // Below basic vehicle functionality will be available
            // eg. Indicators and Internal Lights
            
            Game.DisableControlThisFrame((Control)Settings.CON_HZRD);
            Game.DisableControlThisFrame((Control)Settings.CON_INDRIGHT);
            Game.DisableControlThisFrame((Control)Settings.CON_INDLEFT);

            // Registers new Vehicle
            if (_activeVehicle == null || _prevVehicle != veh)
            {
                _activeVehicle = veh.GetActiveVehicle();
                _prevVehicle = veh;
                veh.IsInteriorLightOn = false;
            }

            // Adds Brake Light Functionality
            if (!_activeVehicle.Blackout && veh.IsStopped)
                veh.AreBrakeLightsOn = true;
                    
            if (_activeVehicle.HasSiren)
            {
                Game.DisableControlThisFrame((Control)Settings.CON_TOGGLESIREN);
                Game.DisableControlThisFrame((Control)Settings.CON_NEXTSIREN);
                Game.DisableControlThisFrame((Control)Settings.CON_PREVSIREN);
                Game.DisableControlThisFrame((Control)Settings.CON_AUXSIREN);
                Game.DisableControlThisFrame((Control)Settings.CON_TOGGLELIGHTS);
                Game.DisableControlThisFrame((Control)Settings.CON_INTLT);
                Game.DisableControlThisFrame((Control)Settings.CON_HORN);

                if (!Game.IsPaused)
                {
                    // Toggle lighting
                    if (Game.IsControlJustReleased((Control)Settings.CON_TOGGLELIGHTS))
                    {
                        switch (veh.IsSirenActive)
                        {
                            case true:
                                Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                                    Settings.SET_AUDIOREF, true);
                                _activeVehicle.LightsOn = false;
                                veh.IsSirenActive = false;
                                _activeVehicle.SirenStage = SirenStage.Off;
                                Sirens.Update(_activeVehicle);
                                break;
                            case false:
                                Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                                    Settings.SET_AUDIOREF, true);
                                _activeVehicle.LightsOn = true;
                                veh.IsSirenActive = true;
                                veh.IsSirenSilent = true;
                                break;
                        }
                    }

                    // Toggle Aux Siren
                    if (Game.IsControlJustReleased((Control)Settings.CON_AUXSIREN))
                    {
                        if (_activeVehicle.AuxOn)
                        {
                            Sound.ClearTempSoundID(_activeVehicle.AuxID);
                            _activeVehicle.AuxOn = false;
                        }
                        else
                        {
                            _activeVehicle.AuxID = Sound.TempSoundID();
                            _activeVehicle.AuxOn = true;
                            
                            Natives.PlaySoundFromEntity(_activeVehicle.AuxID,
                                _activeVehicle.SoundSet == null
                                    ? "VEHICLES_HORNS_SIREN_1"
                                    : _activeVehicle.SoundSet.SirenTones[0], 
                                _activeVehicle.Vehicle.Handle, null, false,
                                0);
                        }
                    }

                    // Siren Switches
                    if (_activeVehicle.LightsOn)
                    {
                        if (Game.IsControlJustReleased((Control)Settings.CON_TOGGLESIREN))
                        {
                            Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                                Settings.SET_AUDIOREF, true);
                            _activeVehicle.SirenStage = _activeVehicle.SirenStage == SirenStage.Off 
                                ? SirenStage.One
                                : SirenStage.Off;
                            
                            Sirens.Update(_activeVehicle);
                        }
                    }

                    if (_activeVehicle.SirenStage > SirenStage.Off)
                    {
                        // Move Up Siren Stage
                        if (Game.IsControlJustReleased((Control)Settings.CON_NEXTSIREN))
                            Sirens.MoveUpStage(_activeVehicle);
                        // Move Down Siren Stage
                        if (Game.IsControlJustReleased((Control)Settings.CON_PREVSIREN))
                            Sirens.MoveDownStage(_activeVehicle);
                    }

                    // Interior Light
                    if (Game.IsControlJustReleased((Control)Settings.CON_INTLT))
                    {
                        _activeVehicle.InteriorLight = !_activeVehicle.InteriorLight;
                        veh.IsInteriorLightOn = _activeVehicle.InteriorLight;
                    }

                    // Manual                                                              
                    _isManualActive = _activeVehicle.SirenStage == SirenStage.Off
                                      && Game.IsControlPressed((Control)Settings.CON_NEXTSIREN);

                    // Horn
                    _isHornActive = Game.IsControlPressed((Control)Settings.CON_HORN);

                    // Manage Horn and Manual siren
                    var hornManualState = 0;
                    switch (_isHornActive)
                    {
                        case true when !_isManualActive:
                            hornManualState = 1;
                            break;
                        case false when _isManualActive:
                            hornManualState = 2;
                            break;
                        case true when _isManualActive:
                            hornManualState = 3;
                            break;
                    }

                    Sirens.SetAirManuState(_activeVehicle, hornManualState);
                }
            }

            // Indicators
            if (!Game.IsPaused)
            {
                // Left Indicator
                if (Game.IsControlJustReleased((Control)Settings.CON_INDLEFT) &&
                    Natives.IsUsingKeyboardAndMouse(0))
                {
                    if (_activeVehicle.IndStatus == IndStatus.Left)
                    {
                        _activeVehicle.IndStatus = IndStatus.Off;
                        Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                            Settings.SET_AUDIOREF, true);
                    }
                    else
                    {
                        _activeVehicle.IndStatus = IndStatus.Left;
                        Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                            Settings.SET_AUDIOREF, true);
                    }

                    Lights.UpdateIndicator(_activeVehicle);
                }

                // Right Indicator
                if (Game.IsControlJustReleased((Control)Settings.CON_INDRIGHT) &&
                    Natives.IsUsingKeyboardAndMouse(0))
                {
                    if (_activeVehicle.IndStatus == IndStatus.Right)
                    {
                        _activeVehicle.IndStatus = IndStatus.Off;
                        Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                            Settings.SET_AUDIOREF, true);
                    }
                    else
                    {
                        _activeVehicle.IndStatus = IndStatus.Right;
                        Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                            Settings.SET_AUDIOREF, true);
                    }

                    Lights.UpdateIndicator(_activeVehicle);
                }

                // Hazards
                if (Game.IsControlJustReleased((Control)Settings.CON_HZRD) &&
                    Natives.IsUsingKeyboardAndMouse(0))
                {
                    if (_activeVehicle.IndStatus == IndStatus.Both)
                    {
                        _activeVehicle.IndStatus = IndStatus.Off;
                        Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                            Settings.SET_AUDIOREF, true);
                    }
                    else
                    {
                        _activeVehicle.IndStatus = IndStatus.Both;
                        Natives.PlaySoundFrontend(-1, Settings.SET_AUDIONAME,
                            Settings.SET_AUDIOREF, true);
                    }

                    Lights.UpdateIndicator(_activeVehicle);
                }
            }
        }
    }
}