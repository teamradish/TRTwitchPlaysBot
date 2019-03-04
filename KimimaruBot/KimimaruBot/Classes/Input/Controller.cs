using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace KimimaruBot
{
    public static class ControllerVars
    {
        public static Dictionary<string, int> INPUTS = new Dictionary<string, int>() {
            { "left", 0 }, { "c", 0 },
            { "right", 1 }, { "z", 1 },
            { "up", 2 }, { "tleft", 2 },
            { "down", 3 }, { "tright", 3 },
            { "a", 4 },
            { "b", 5 },
            { "l", 6 }, { "one", 6 },
            { "r", 7 }, { "two", 7 },
            { "select", 8 }, { "minus", 8 },
            { "start", 9 }, { "plus", 9 },
            { "cleft", 10 }, { "pleft", 10 },
            { "cright", 11 }, { "pright", 11 },
            { "cup", 12 }, { "pup", 12 },
            { "cdown", 13 }, { "pdown", 13 },
            { "dleft", 14 },
            { "dright", 15 },
            { "dup", 16 },
            { "ddown", 17 },
            { "SAVESTATE1", 18 }, { "tforward", 18 },
            { "SAVESTATE2", 19 }, { "tback", 19 },
            { "SAVESTATE3", 20 },
            { "SAVESTATE4", 21 },
            { "SAVESTATE5", 22 },
            { "SAVESTATE6", 23 },
            { "LOADSTATE1", 24 },
            { "LOADSTATE2", 25 },
            { "LOADSTATE3", 26 },
            { "LOADSTATE4", 27 },
            { "LOADSTATE5", 28 },
            { "LOADSTATE6", 29 },
            { "x", 30 }, { "shake", 30 },
            { "y", 31 }, { "point", 31 },
        };

        public static void reset()
        {
            //pyautogui.press("f1");
        }

        /*public static string savestate(int num, User user)
        {
            global Controllers

            if (user["level"] < 3 and num <= 3 and num > 0)
                return "Access denied";
            elif (num > 0 and num <= 6)
            {
                Controllers[1].execute_input("SAVESTATE" + num, 200);
                //pyautogui.press(str(num))
                //pyautogui.press("f5")

                File.WriteAllText("last_save.txt", num.ToString());

                return "/me Saving state " + num;
            }
            else return "Invalid number.";
        }*/

        /*public static void loadstate(int num, User user)
        {
            global Controllers

            if (num > 0 and num <= 6)
            {
                Controllers[1].execute_input("LOADSTATE" + str(num), 200);
                //pyautogui.press(str(num))
                //pyautogui.press("f7")

                File.WriteAllText("last_load.txt", num.ToString());

                return "/me Loading state " + num;
            }
            else return "Invalid number.";
        }*/

        public static readonly Controller[] Controllers = new Controller[] { new Controller(0), new Controller(1) };
    }

    public class Controller
    {
        public int id;
        public Dictionary<string, bool> buttons = null;
        private bool paused;
        private int thread_count;

        public Controller(int id = 0)
        {
            this.id = id;
            this.buttons = new Dictionary<string, bool>() {
            { "left",         false }, { "tleft",         false },
            { "right",        false }, { "tright",        false },
            { "up",           false },
            { "down",         false },
            { "a",            false },
            { "b",            false },
            { "l",            false }, {"one",            false },
            { "r",            false }, {"two",            false },
            { "select",       false }, {"minus",          false }, { "z", false },
            { "start",        false }, {"plus",           false },
            { "cleft",        false }, {"pleft",          false },
            { "cright",       false }, {"pright",         false },
            { "cup",          false }, {"pup",            false },
            { "cdown",        false }, {"pdown",          false },
            { "dleft",        false }, { "c",             false },
            { "dright",       false }, {"z",              false },
            { "dup",          false },
            { "ddown",        false },
            { "SAVESTATE1",   false }, { "tforward",      false },
            { "SAVESTATE2",   false }, { "tback",         false },
            { "SAVESTATE3",   false },
            { "SAVESTATE4",   false },
            { "SAVESTATE5",   false },
            { "SAVESTATE6",   false },
            { "LOADSTATE1",   false },
            { "LOADSTATE2",   false },
            { "LOADSTATE3",   false },
            { "LOADSTATE4",   false },
            { "LOADSTATE5",   false },
            { "LOADSTATE6",   false },
            { "x",            false }, { "shake",         false },
            { "y",            false }, { "point",         false },
            { "#",            false },
            { ".",            false }
            };

            this.paused = false;
            this.thread_count = 0;
        }

        public void execute_input_array(string[] input_array)
        {
            this.thread_count += 1;

            //The list of buttons this particular instance cares about
            List<string> instance_buttons = new List<string>();

            //long test1 = Stopwatch.GetTimestamp();

            //For each string of simultaneous buttons
            foreach (string simultaneous_buttons in input_array)
            {

                //Determine the delay time
                double max_delay = 0d;
                foreach (var button in simultaneous_buttons)
                {
                    //if button.duration > max_delay: max_delay = button.duration
                }

                //Press each button in a separate thread, as they are simultaneous
                foreach (var button in simultaneous_buttons)
                {
                    /*Thread b = Thread(target=this.execute_input, args=[button.name, button.duration, button.percent, button.hold, button.release])
                    b.Start();

                    instance_buttons.append(button.name)
                    */
                }

                //Wait the maximum time
                int delay_time = (int)max_delay / 1000;
                //Console.WriteLine(delay_time.ToString());
                if (delay_time > 0)
                    Thread.Sleep(delay_time);
                else
                    ;

                //long test2 = Stopwatch.GetTimestamp();
                //Console.WriteLine((test2-test1)*1000);
                //Release any buttons this instance started holding
                foreach (var button in instance_buttons)
                {
                    /*if this.buttons[button]:
                        # TEMP to add l
                        if button == "l" or button == "left" or button == "right" or button == "up" or button == "down" or button == "pleft" or button == "pright" or button == "pup" or button == "pdown":
                            this.release_analog(button);
                        elif button != "." and button != "#":
                            this.release_digital(button);
                     */
                }

                this.thread_count -= 1;
            }
        }

        public void execute_input(string val="", int duration=0, int percent=100, bool hold=false, bool release=false)
        {

            //TEMP to add 1
            if ((val == "l" && percent != 100) || val == "left" || val == "right" || val == "up" || val == "down" || val == "pleft" || val == "pright" || val == "pup" || val == "pdown")
            {
                if (hold)
                    //this.hold_analog_indefinite(val, percent);
                    ;
                else if (release)
                    //this.release_analog(val);
                    ;
                else
                    //this.hold_analog_duration(val, duration, percent);
                    ;
            }
            else if (val != "." && val != "#" && !(val.StartsWith("SAVE") || val.StartsWith("LOAD")))
            {
                if (hold)
                    //this.hold_digital_indefinite(val);
                    ;
                else if (release)
                    //this.release_digital(val);
                    ;
                else
                    //this.hold_digital_duration(val, duration);
                    ;
            }
            else
            {
                if (val == ".")
                {
                    //this.buttons[val] = true;
                    Thread.Sleep(TimeSpan.FromSeconds(0.2d));
                    //this.buttons[val] = false;
                }
                else if (val == "#")
                {
                    //this.buttons[val] = true;
                    Thread.Sleep(duration / 1000);
                    //this.buttons[val] = false;
                }
                else
                    //this.hold_digital_duration(val, duration);
                    ;
            }
        }
    }
}
