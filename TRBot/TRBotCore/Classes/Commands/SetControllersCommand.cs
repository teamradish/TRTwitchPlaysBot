using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using TwitchLib.Client.Events;

namespace TRBot
{
    /// <summary>
    /// Sets the number of controllers available.
    /// For simplicity, this will reset all inputs and recapture the virtual devices.
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
                BotProgram.QueueMessage($"Usage: \"# of controllers (min: {BotProgram.ControllerMngr.MinControllers}, max: {BotProgram.ControllerMngr.MaxControllers})\"");
                return;
            }

            if (int.TryParse(args[0], out int newJoystickCount) == false)
            {
                BotProgram.QueueMessage("Invalid number of controllers!");
                return;
            }

            if (newJoystickCount < BotProgram.ControllerMngr.MinControllers)
            {
                BotProgram.QueueMessage($"Value is less than {BotProgram.ControllerMngr.MinControllers}!");
                return;
            }

            if (newJoystickCount > BotProgram.ControllerMngr.MaxControllers)
            {
                BotProgram.QueueMessage($"Value is greater than {BotProgram.ControllerMngr.MaxControllers}, which is the max number of supported controllers!");
                return;
            }

            if (newJoystickCount == BotProgram.BotData.JoystickCount)
            {
                BotProgram.QueueMessage("There are already that many controllers plugged in!");
                return;
            }

            //We changed count, so let's stop all inputs and reinitialize the devices
            BotProgram.QueueMessage($"Changing controller count from {BotProgram.BotData.JoystickCount} to {newJoystickCount}. Stopping all inputs and reinitializing.");

            InputHandler.CancelRunningInputs();

            //Wait until no inputs are running
            while (InputHandler.CurrentRunningInputs > 0)
            {

            }

            //Reinitialize the virtual controllers
            BotProgram.ControllerMngr.CleanUp();

            //Kimimaru: Time out so we don't softlock everything if all devices cannot be freed
            //While this is an issue if it happens, we'll let the streamer know without permanently suspending inputs
            const long timeOut = 60000L;

            //Wait at least this much time before checking to give it some time
            const long minWait = 300L;

            Stopwatch sw = Stopwatch.StartNew();

            //Wait until all devices are no longer owned
            while (true)
            {
                if (sw.ElapsedMilliseconds < minWait)
                {
                    continue;
                }

                int freeCount = 0;

                for (int i = 0; i < BotProgram.ControllerMngr.ControllerCount; i++)
                {
                    if (BotProgram.ControllerMngr.GetController(i).IsAcquired == false)
                    {
                        freeCount++;
                    }
                }

                //We're done if all are no longer owned
                if (freeCount == BotProgram.ControllerMngr.ControllerCount)
                {
                    break;
                }

                if (sw.ElapsedMilliseconds >= timeOut)
                {
                    BotProgram.QueueMessage($"ERROR: Unable to free all virtual controllers. {freeCount}/{BotProgram.ControllerMngr.ControllerCount} freed.");
                    break;
                }
            }

            int acquiredCount = BotProgram.ControllerMngr.InitControllers(newJoystickCount);
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
