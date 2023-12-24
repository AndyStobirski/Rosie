using Rosie.Code.Misc;
using Rosie.Entities;
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
            WaitingForStartKey, AwaitingKeyPress, AwaitingMouseClick
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
            State = InputState.WaitingForStartKey;

            //move
            var com = new Command(CommandType.Move, true,
                 keys.keypad1, keys.keypad2, keys.keypad3
                , keys.keypad4, keys.keypad5, keys.keypad6
                , keys.keypad7, keys.keypad8, keys.keypad9
            );
            commands.Add(com);

            //take
            com = new Command(CommandType.Take, true, keys.keyT);
            commands.Add(com);

            //inventory
            com = new Command(CommandType.Drop, true, keys.keyI);
            com.AddStep(new Step() { InfoToDisplay = DisplayInformation.Inventory });
            commands.Add(com);

            //drop
            com = new Command(CommandType.Drop, true, keys.keyD);
            com.AddStep(new Step() { InfoToDisplay = DisplayInformation.Drop });
            commands.Add(com);

            //equip
            com = new Command(CommandType.Equip, true, keys.keyE);
            com.AddStep(new Step() { InfoToDisplay = DisplayInformation.Equip });
            commands.Add(com);

            //open
            com = new Command(CommandType.Open, true, keys.keyO);
            com.AddStep(new Step() { InfoToDisplay = DisplayInformation.ChooseDirection });
            commands.Add(com);

            //close
            com = new Command(CommandType.Close, true, keys.keyC);
            com.AddStep(new Step() { InfoToDisplay = DisplayInformation.ChooseDirection });
            commands.Add(com);

            //Use stairs
            com = new Command(CommandType.StairsMove, true, keys.keyS);
            commands.Add(com);

            //Minimap
            com = new Command(CommandType.MiniMap, true, keys.keyM);
            commands.Add(com);

            //Look
            com = new Command(CommandType.Look, false, keys.keyL);
            com.AddStep(new Step() { InfoToDisplay = DisplayInformation.SelectCell, Input = InputType.MouseClick });
            commands.Add(com);


        }

        public void MapCellClicked(int pX, int pY)
        {
            if (State == InputState.AwaitingMouseClick)
            {

                CurrentData.Add(pX);
                CurrentData.Add(pY);

                GetNextStep();
            }
        }

        /// <summary>
        /// Check if provided key press if known to the input manager
        /// </summary>
        /// <param name="pKey"></param>
        public void GetUserKeyPress(int pKey)
        {
            if (pKey == (int)keys.escape)
            {
                State = InputState.WaitingForStartKey;
                GameCommandIssued?.Invoke(this, new GameCommandEventArgs(CommandType.Escape, false, null));
            }


            if (State == InputState.AwaitingMouseClick)
                return;


            if (State == InputState.WaitingForStartKey)
            {
                if ((CurrentCommand = commands.FirstOrDefault(c => c.Trigger.Any(k => (int)k == pKey))) == null)
                    return;

                CurrentCommand.ResetEnumerator();


                RosieGame.ViewMode = GameViewMode.InputHandlerMode;

                // We have a trigger, so initiate the current data array
                CurrentData = new List<int> { pKey };
            }
            else if (State == InputState.AwaitingKeyPress)
            {
                CurrentData.Add(pKey);
            }

            GetNextStep();

        }


        /// <summary>
        /// Get the next step of the current command
        /// </summary>
        private void GetNextStep()
        {
            if (CurrentCommand.GetNextStep() == false)
            {
                //reached the end of the enumerator
                //so raise an event
                State = InputState.WaitingForStartKey;
                RosieGame.ViewMode = GameViewMode.Game;
                GameCommandIssued?.Invoke(this, new GameCommandEventArgs(CurrentCommand.commandType, CurrentCommand.ConsumeTurn, CurrentData.ToArray()));

            }
            else
            {
                switch (CurrentCommand.CurrentStep.Input)
                {
                    case InputType.MouseClick:
                        State = InputState.AwaitingMouseClick;
                        break;

                    case InputType.KeyPress:
                        State = InputState.AwaitingKeyPress;
                        break;

                }

                SetOutputMessage(CurrentCommand.CurrentStep.InfoToDisplay);
            }
        }

        /// <summary>
        /// Got a step, so call back to the game to request a message to display
        /// </summary>
        /// <param name="pDisplay"></param>
        private void SetOutputMessage(DisplayInformation pDisplay)
        {
            OutputMessage = new();

            switch (CurrentCommand.CurrentStep.InfoToDisplay)
            {
                case DisplayInformation.None:
                    break;

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



                case DisplayInformation.SelectCell:
                    OutputMessage.Add(MessageStrings.Select_Cell);
                    break;

                default:
                    throw new NotImplementedException("SetOutputMessage: " + pDisplay.ToString());

            }
        }

        public event EventHandler<GameCommandEventArgs> GameCommandIssued;
        public class GameCommandEventArgs : EventArgs
        {
            public GameCommandEventArgs(CommandType command, bool pConsumeTurn, params int[] data)
            {
                Command = command;
                Data = data;
                ConsumeTurn = pConsumeTurn;
            }

            public CommandType Command { private set; get; }
            public int[] Data { private set; get; }

            public bool ConsumeTurn { private set; get; }
        }
    }

    /// <summary>
    /// Defines a keypress or which initiates a game command
    /// The command can consist of multiple steps which gather more
    /// information
    /// </summary>
    public class Command
    {
        public Command(CommandType pCommandType, bool pConsumeTurn, params keys[] pKeys)
        {
            commandType = pCommandType;
            Trigger = pKeys;
            ConsumeTurn = pConsumeTurn;
        }

        public Command(CommandType pCommandType, keys[] trigger, List<Step> pSteps, bool pConsumeTurn)
        {
            commandType = pCommandType;
            Trigger = trigger;
            Steps = pSteps;
            ConsumeTurn = pConsumeTurn;
        }

        public CommandType commandType { get; private set; }

        /// <summary>
        /// Triggers the command
        /// </summary>
        public keys[] Trigger { get; set; }

        /// <summary>
        /// Does the action consume a game turn
        /// </summary>
        public bool ConsumeTurn { get; private set; }

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
    /// A screen which displays additional information to prompt to the player for more input
    /// </summary>
    public class Step
    {
        /// <summary>
        /// The data for the step to display
        /// </summary>
        public DisplayInformation InfoToDisplay { get; set; } = DisplayInformation.None;

        public InputType Input { get; set; } = InputType.KeyPress;
    }

    public enum InputType
    {
        KeyPress, MouseClick
    }

}
