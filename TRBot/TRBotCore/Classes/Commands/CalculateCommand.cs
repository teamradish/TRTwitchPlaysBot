using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Numerics;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using NCalc;

namespace TRBot
{
    public sealed class CalculateCommand : BaseCommand
    {
        public CalculateCommand()
        {

        }

        public override void ExecuteCommand(object sender, OnChatCommandReceivedArgs e)
        {
            Expression exp = null;

            try
            {
                string expression = e.Command.ChatMessage.Message.Replace($"{Globals.CommandIdentifier}calculate", string.Empty);

                exp = new Expression(expression);
                exp.Options = EvaluateOptions.IgnoreCase;
                exp.EvaluateParameter += Exp_EvaluateParameter;
                exp.EvaluateFunction += Exp_EvaluateFunction;

                string finalExpr = exp.Evaluate().ToString();

                //You can use text in calculate to make the bot do things, such as Twitch chat commands
                //Ignore any output with a "/" in it to avoid exploiting this
                if (finalExpr.Contains('/') == true)
                {
                    BotProgram.QueueMessage("Very clever, but I'm one step ahead of you.");
                    return;
                }

                BotProgram.QueueMessage(finalExpr);
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);

                BotProgram.QueueMessage(exc.Message);//"Sorry, I can't calculate that!");
            }
            finally
            {
                if (exp != null)
                {
                    exp.EvaluateParameter -= Exp_EvaluateParameter;
                    exp.EvaluateFunction -= Exp_EvaluateFunction;
                }
            }
        }

        private void Exp_EvaluateFunction(string name, FunctionArgs args)
        {
            string funcname = name.ToLower();

            if (funcname == "tanh")
            {
                double d = 0d;
                double.TryParse(args.EvaluateParameters()[0].ToString(), out d);

                args.Result = Math.Tanh(d);
            }

            if (funcname == "sinh")
            {
                double.TryParse(args.EvaluateParameters()[0].ToString(), out double d);

                args.Result = Math.Sinh(d);
            }

            if (funcname == "cosh")
            {
                double.TryParse(args.EvaluateParameters()[0].ToString(), out double d);

                args.Result = Math.Cosh(d);
            }

            //if (funcname == "fib")
            //{
            //    long.TryParse(args.EvaluateParameters()[0].ToString(), out long a);
            //
            //    args.Result = Fibonacci(a);
            //}

            if (funcname == "fac")
            {
                long.TryParse(args.EvaluateParameters()[0].ToString(), out long a);

                args.Result = Factorial(a);
            }

            if (funcname == "log" && args.EvaluateParameters().Length == 1)
            {
                double.TryParse(args.EvaluateParameters()[0].ToString(), out double d);

                args.Result = Math.Log(d);
            }

            if (funcname == "logfac")
            {
                long.TryParse(args.EvaluateParameters()[0].ToString(), out long a);

                args.Result = LogFactorial(a);
            }

            if (funcname == "hermite")
            {
                object[] arr = args.EvaluateParameters();

                if (arr.Length == 5)
                {
                    float.TryParse(arr[0].ToString(), out float val1);
                    float.TryParse(arr[1].ToString(), out float tang1);
                    float.TryParse(arr[2].ToString(), out float val2);
                    float.TryParse(arr[3].ToString(), out float tang2);
                    float.TryParse(arr[4].ToString(), out float amt);

                    args.Result = Hermite(val1, tang1, val2, tang2, amt);
                }
            }
        }

        private void Exp_EvaluateParameter(string name, ParameterArgs args)
        {
            if (name.ToLower() == "pi")
            {
                args.Result = Math.PI;
            }

            if (name.ToLower() == "e")
            {
                args.Result = Math.E;
            }
        }

        private long Fibonacci(long i)
        {
            if (i <= 0) return 0;
            if (i == 1) return 1;

            return (Fibonacci(i - 1) + Fibonacci(i - 2));
        }

        private long Factorial(long i)
        {
            if (i <= 1)
                return 1;
            return i * Factorial(i - 1);
        }

        private double LogFactorial(long i)
        {
            if (i <= 1) return 0;

            return Math.Log(i, 10) + LogFactorial(i - 1);
        }

        private double Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
        {
            /*Hermite basis functions:
             * s = amount (or time)
             * h1 = 2s^3 - 3s^2 + 1
             * h2 = -2s^3 + 3s^2
             * h3 = s^3 - 2s^2 + s
             * h4 = s^3 - s^2
             * 
             * The values are multiplied by the basis functions and added together like so:
             * result = (h1 * val1) + (h2 * val2) + (h3 * tan1) + (h4 * tan2);
            */

            double val1 = value1;
            double val2 = value2;
            double tan1 = tangent1;
            double tan2 = tangent2;
            double amt = amount;
            double result = 0d;

            //Define cube and squares
            double amtCubed = amt * amt * amt;
            double amtSquared = amt * amt;

            //If 0, return the initial value
            if (amount == 0f)
            {
                result = value1;
            }
            //If 1, return the 
            else if (amount == 1f)
            {
                result = value2;
            }
            else
            {
                //Define hermite functions
                //double h1 = (2 * amtCubed) - (3 * amtSquared) + 1;
                //double h2 = (-2 * amtCubed) + (3 * amtSquared);
                //double h3 = amtCubed - (2 * amtSquared) + amt;
                //double h4 = amtCubed - amtSquared;

                //Multiply the results
                //result = (h1 * val1) + (h2 * val2) + (h3 * tan1) + (h4 * tan2);

                //Condensed
                result =
                    (((2 * val1) - (2 * val2) + tan2 + tan1) * amtCubed) +
                    (((3 * val2) - (3 * val1) - (2 * tan1) - tan2) * amtSquared) +
                    (tan1 * amt) + val1;
            }

            return result;
        }
    }
}
