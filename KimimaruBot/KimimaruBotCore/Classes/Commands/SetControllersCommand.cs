using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using TwitchLib.Client.Events;

namespace KimimaruBot
{
    /// <summary>
    /// Sets the number of controllers available.
    /// For simplicity, this will reset all inputs and recapture the vJoy devices.
    /// </summary>
    public sealed class SetControllersCommand : BaseCommand
    {
        public override void Initialize(CommandHandler commandHandler)
        {
            AccessLevel = (int)AccessLevels.Levels.Admin;
        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            List<string> args = e.Command.ArgumentsAsList;

            if (args.Count != 1)
            {
                BotProgram.QueueMessage($"Usage: \"# of controllers (min: {VJoyController.MIN_VJOY_DEVICE_ID}, max: {VJoyController.MAX_VJOY_DEVICE_ID})\"");
                return;
            }

            if (int.TryParse(args[0], out int newJoystickCount) == false)
            {
                BotProgram.QueueMessage("Invalid number of controllers!");
                return;
            }

            if (newJoystickCount < VJoyController.MIN_VJOY_DEVICE_ID)
            {
                BotProgram.QueueMessage($"Value is less than {VJoyController.MIN_VJOY_DEVICE_ID}!");
                return;
            }

            if (newJoystickCount > VJoyController.MAX_VJOY_DEVICE_ID)
            {
                BotProgram.QueueMessage($"Value is greater than {VJoyController.MAX_VJOY_DEVICE_ID}, which is the max number of supported controllers!");
                return;
            }

            if (newJoystickCount == BotProgram.BotData.JoystickCount)
            {
                BotProgram.QueueMessage("There are already that many controllers plugged in!");
                return;
            }

            //We changed count, so let's stop all inputs and reinitialize the vJoy devices
            BotProgram.QueueMessage($"Changing controller count from {BotProgram.BotData.JoystickCount} to {newJoystickCount}. Stopping all inputs and reinitializing.");

            InputHandler.CancelRunningInputs();

            //Wait until no inputs are running
            while (InputHandler.CurrentRunningInputs > 0)
            {

            }

            //Reinitialize the vJoy devices
            VJoyController.CleanUp();

            //Kimimaru: Time out so we don't softlock everything if all devices cannot be freed
            //While this is an issue if it happens, we'll let the streamer know without permanently suspending inputs
            const long timeOut = 60000;

            Stopwatch sw = Stopwatch.StartNew();

            //Wait until all vJoy devices are no longer owned
            while (true)
            {
                int freeCount = 0;

                for (int i = 0; i < VJoyController.Joysticks.Length; i++)
                {
                    VjdStat stat = VJoyController.VJoyInstance.GetVJDStatus(VJoyController.Joysticks[i].ControllerID);
                    if (stat != VjdStat.VJD_STAT_OWN)
                    {
                        freeCount++;
                    }
                }

                //We're done if all are no longer owned
                if (freeCount == VJoyController.Joysticks.Length)
                {
                    break;
                }

                if (sw.ElapsedMilliseconds >= timeOut)
                {
                    BotProgram.QueueMessage($"ERROR: Unable to free all vJoy controllers. {freeCount}/{VJoyController.Joysticks.Length} freed.");
                    break;
                }
            }

            int acquiredCount = VJoyController.InitControllers(newJoystickCount);
            Console.WriteLine($"Acquired {acquiredCount} controllers!");

            const long wait = 500L;
            sw.Stop();
            sw.Reset();
            sw.Start();

            //Wait again to reinitialize
            while (sw.ElapsedMilliseconds < wait)
            {

            }

            InputHandler.ResumeRunningInputs();

            BotProgram.BotData.JoystickCount = newJoystickCount;
            BotProgram.SaveBotData();

            BotProgram.QueueMessage("Controllers reinitialized and inputs resumed!");
        }
    }
}
