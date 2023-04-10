using Rosie.Entities;
using Rosie.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Rosie.Code
{
    /// <summary>
    /// This class accepts keypresses and when they match a registerd pattern it raises
    /// an event which indicates that a command has been recognised which is submitted
    /// to the game.
    /// </summary>
    public class InputHandler
    {
        /// TODO Add input masking

        Player player => RosieGame.player;

        InputState State { get; set; }

        enum InputState
        {
            Waiting, Handling
        }


        /// <summary>
        /// A list of all game commands
        /// </summary>
        public List<Command> commands = new List<Command>();

        /// <summary>
        /// This is set when a key press is recognised
        /// </summary>
        Command CurrentCommand { get; set; }

        /// <summary>
        /// This is data inputed by the player
        /// </summary>
        List<int> CurrentData = new List<int>();

        /// <summary>
        /// The message to be displayed
        /// </summary>
        private List<string> OutputMessage { get; set; }

        public String DisplayMessage => string.Join("\r\n", OutputMessage.ToArray());

        /// <summary>
        /// Constructor
        /// </summary>
        public InputHandler()
        {
            State = InputState.Waiting;

            //
            var com = new Command(CommandType.Move, false,
                 keys.keypad1, keys.keypad2, keys.keypad3
                , keys.keypad4, keys.keypad6
                , keys.keypad7, keys.keypad8, keys.keypad9
            );
            commands.Add(com);

            //
            com = new Command(CommandType.Take, false, keys.keyT);
            commands.Add(com);

            //inventory
            com = new Command(CommandType.Drop, true, keys.keyI);
            com.AddStep(new Step() { dataType = DisplayInformation.Inventory });
            commands.Add(com);

            //drop
            com = new Command(CommandType.Drop, true, keys.keyD);
            com.AddStep(new Step() { dataType = DisplayInformation.Drop });
            commands.Add(com);

            //equip
            com = new Command(CommandType.Equip, true, keys.keyE);
            com.AddStep(new Step() { dataType = DisplayInformation.Equip });
            commands.Add(com);

            //open
            com = new Command(CommandType.Open, true, keys.keyO);
            com.AddStep(new Step() { dataType = DisplayInformation.ChooseDirection });
            commands.Add(com);

            //close
            com = new Command(CommandType.Close, true, keys.keyC);
            com.AddStep(new Step() { dataType = DisplayInformation.ChooseDirection });
            commands.Add(com);

        }

        /// <summary>
        /// Check if provided key press if known the input manager
        /// </summary>
        /// <param name="pKey"></param>
        public void ProcessCommand(int pKey)
        {

            if (pKey == (int)keys.escape)
            {
                RosieGame.ViewMode = GameViewMode.Game;
                State = InputState.Waiting;
            }

            if (State == InputState.Waiting)
            {
                if ((CurrentCommand = commands.FirstOrDefault(c => c.Trigger.Any(k => (int)k == pKey))) == null)
                    return;

                CurrentCommand.ResetEnumerator();

                if (CurrentCommand.UseIO)
                {
                    RosieGame.ViewMode = GameViewMode.IO;
                }

                // we have a trigger

                CurrentData = new List<int> { pKey };

                State = InputState.Handling;

            }
            else if (State == InputState.Handling)
            {
                CurrentData.Add(pKey);
            }

            if (CurrentCommand.GetNextStep() == false)
            {
                //reached the end of the enumerator
                //so raise an event
                State = InputState.Waiting;
                RosieGame.ViewMode = GameViewMode.Game;
                GameCommandIssued?.Invoke(this, new GameCommandEventArgs(CurrentCommand.commandType, CurrentData.ToArray()));

            }
            else
            {
                SetOutputMessage(CurrentCommand.CurrentStep.dataType);
            }
        }

        /// <summary>
        /// Got a step, so call back to the game to request a message to display
        /// </summary>
        /// <param name="pDisplay"></param>
        private void SetOutputMessage(DisplayInformation pDisplay)
        {
            OutputMessage = new();

            switch (CurrentCommand.CurrentStep.dataType)
            {
                case DisplayInformation.Inventory:
                    OutputMessage.Add(MessageStrings.Inventory_Carrying);
                    OutputMessage.Add("");
                    OutputMessage.AddRange(player.Inventory.Select((i, n) => Convert.ToChar(n + 97).ToString() + ") " + i.Name));
                    OutputMessage.Add("");
                    OutputMessage.Add(MessageStrings.Inventory_ItemActions);
                    break;

                case DisplayInformation.Drop:
                    OutputMessage.Add(string.Format(MessageStrings.Inventory_Drop, Convert.ToChar(player.Inventory.Count + 65 - 1).ToString()));
                    OutputMessage.Add("");
                    OutputMessage.AddRange(player.Inventory.Select((i, n) => Convert.ToChar(n + 97).ToString() + ") " + i.Name));
                    OutputMessage.Add("");
                    OutputMessage.Add(MessageStrings.Inventory_ItemActions);
                    break;

                case DisplayInformation.Equip:
                    OutputMessage.Add(string.Format(MessageStrings.Inventory_Drop, Convert.ToChar(player.Inventory.Count + 65 - 1).ToString()));
                    OutputMessage.Add("");
                    OutputMessage.AddRange(player.Inventory.Select((i, n) => Convert.ToChar(n + 97).ToString() + ") " + i.Name));
                    OutputMessage.Add("");
                    OutputMessage.Add(MessageStrings.Inventory_ItemActions);
                    break;

                case DisplayInformation.ChooseDirection:
                    OutputMessage.Add(MessageStrings.Direction_Chose);
                    break;

                default:
                    throw new NotImplementedException("SetOutputMessage: " + pDisplay.ToString());

            }
        }


        public event EventHandler<RequestDataEventArgs> RequestData;

        public class RequestDataEventArgs : EventArgs
        {
            string Data { get; set; }
        }




        public event EventHandler<GameCommandEventArgs> GameCommandIssued;
        public class GameCommandEventArgs : EventArgs
        {
            public GameCommandEventArgs(CommandType command, params int[] data)
            {
                Command = command;
                Data = data;
            }

            public CommandType Command { private set; get; }
            public int[] Data { private set; get; }
        }



    }

    /// <summary>
    /// Defines a keypress or presses which define a game command
    /// </summary>
    public class Command
    {
        public Command(CommandType pCommandType, bool pUseIO, params keys[] pKeys)
        {
            commandType = pCommandType;
            UseIO = pUseIO;
            Trigger = pKeys;
        }

        public Command(CommandType pCommandType, keys[] trigger, List<Step> pSteps)
        {
            commandType = pCommandType;
            Trigger = trigger;
            Steps = pSteps;
        }

        public CommandType commandType { get; private set; }

        /// <summary>
        /// Triggers the command
        /// </summary>
        public keys[] Trigger { get; set; }

        /// <summary>
        /// The steps which comprise the command
        /// </summary>
        private List<Step> Steps { get; set; } = new List<Step>();

        public void AddStep(Step pStep)
        {
            Steps.Add(pStep);
        }

        /// <summary>
        /// Enumerator for the steps list
        /// </summary>
        public IEnumerator StepsEnum { get; private set; }

        public Step CurrentStep => (Step)StepsEnum.Current;

        public Boolean UseIO { get; private set; }

        /// <summary>
        /// Reset the Enumerator
        /// </summary>
        public void ResetEnumerator()
        {
            StepsEnum = Steps.GetEnumerator();
        }

        /// <summary>
        /// Get the next step
        /// </summary>
        /// <returns></returns>
        public bool GetNextStep()
        {
            return StepsEnum.MoveNext();
        }

    }



    /// <summary>
    /// A screen which displays additional information to prompt to the player for another keypress
    /// </summary>
    public class Step
    {
        /// <summary>
        /// The data for the step to display
        /// </summary>
        public DisplayInformation dataType { get; set; }
    }


}
