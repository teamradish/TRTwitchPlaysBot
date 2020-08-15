/* This file is part of TRBot.
 *
 * TRBot is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * TRBot is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public License
 * along with TRBot.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitchLib.Client.Events;
using Commons.Music.Midi;

namespace TRBot
{
    public sealed class MusicCommand : BaseCommand
    {
        private IMidiOutput[] Outputs = null;

        public MusicCommand()
        {

        }

        public override void Initialize(CommandHandler commandHandler)
        {
            IMidiAccess access = MidiAccessManager.Default;

            Console.WriteLine($"Output count: {access.Outputs.Count()}");

            Outputs = new IMidiOutput[access.Outputs.Count() - 1];

            int i = -1;

            foreach (var thing in access.Outputs)
            {
                //Skip non-timidity output
                if (i < 0)
                {
                    i++;
                    continue;
                }
                string output = thing.Id;

                Console.WriteLine($"Output: {output}");

                Task<IMidiOutput> t = access.OpenOutputAsync(output);
            
                t.Wait();

                Outputs[i] = t.Result;
                IMidiPortDetails details = Outputs[i].Details;

                Console.WriteLine($"Finished waiting and opened output. Details: ({details.Id}, {details.Name}, {details.Manufacturer}, {details.Version}) | Connection: {Outputs[i].Connection}");

                i++;
            }
        }   

        public override void CleanUp()
        {
            for (int i = 0; i < Outputs.Length; i++)
            {
                if (Outputs[i] == null)
                    continue;

                Task task = Outputs[i].CloseAsync();
                task.Wait();

                Console.WriteLine($"Closed output {i}!");
            }
        }

        public override void ExecuteCommand(EvtChatCommandArgs e)
        {
            for (int i = 0; i < Outputs.Length; i++)
            {
                if (Outputs[i] == null)
                    continue;

                //Add 12 to the pitch for one octave higher
                //Ex. C# is 61, D is 62
                Outputs[i].Send(new byte [] {MidiEvent.Program, GeneralMidi.Instruments.AcousticGrandPiano}, 0, 2, 0); // There are constant fields for each GM instrument

                for (int j = 0; j < 10; j++)
                {
                    int modifier = j * 2;
                    int offsetMod = j * 2;

                    Outputs[i].Send(new byte [] {MidiEvent.NoteOn, (byte)(0x40 + modifier), (byte)(0x40 + modifier)}, 0, 3, offsetMod); // There are constant fields for each MIDI event
                    Outputs[i].Send(new byte [] {MidiEvent.NoteOff, (byte)(0x40 + modifier), (byte)(0x40 + modifier)}, 0, 3, offsetMod + 1);
                }
                //Outputs[i].Send(new byte [] {MidiEvent.Program, 0x30}, 0, 3, 0); // Strings Ensemble
            }
        }
    }
}
