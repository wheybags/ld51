using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ld51
{
    public enum Input
    {
        Up,
        Down,
        Left,
        Right,

        MAX_INPUT
    }

    public class InputHandler
    {
        private InputState lastState = new InputState();
        public InputState currentState { get; private set; } = new InputState();

        class RepeatState { public long startMs; public long lastRepeatMs; }
        private RepeatState[] inputRepeatBuffer = new RepeatState[(int)Input.MAX_INPUT];
        private long nowMs = 0;

        public void update(long nowMs)
        {
            lastState = currentState;
            currentState = new InputState();
            currentState.gamepad = GamePad.GetState(0, GamePadDeadZone.Circular);
            currentState.keyboard = Keyboard.GetState();

            for (Input i = 0; i < Input.MAX_INPUT; i++)
            {
                bool current = currentState.getInput(i);

                if (!current)
                    inputRepeatBuffer[(int)i] = null;
                if (current && inputRepeatBuffer[(int) i] == null)
                    inputRepeatBuffer[(int)i] = new RepeatState{ startMs = nowMs, lastRepeatMs = 0 };
            }
        }

        public bool downThisFrame(Input input)
        {
            return currentState.getInput(input) && !lastState.getInput(input);
        }

        public bool downThisFrame(Keys key)
        {
            return currentState.getInput(key) && !lastState.getInput(key);
        }

        public bool activeWithRepeat(Input input, long repeatIntervalMs = (long)(1000 * 0.125))
        {
            RepeatState state = inputRepeatBuffer[(int)input];
            if (state == null)
                return false;

            long lastRepeatIndex = (state.lastRepeatMs - state.startMs) / repeatIntervalMs;
            long nowIndex = (nowMs - state.startMs) / repeatIntervalMs;

            if (nowIndex > lastRepeatIndex)
            {
                state.lastRepeatMs = nowMs;
                return true;
            }

            return false;
        }

        public class InputState
        {
            public GamePadState gamepad;
            public KeyboardState keyboard;

            private enum Direction
            {
                Up,
                Right,
                Down,
                Left
            }

            public bool getInput(Keys key)
            {
                return keyboard.IsKeyDown(key);
            }

            public bool getInput(Input input)
            {
                var self = this;

                bool gamepadDirection(Direction direction)
                {
                    float threshold = 0.2f;

                    switch (direction)
                    {
                        case Direction.Up:
                            return self.gamepad.ThumbSticks.Left.Y > threshold || self.gamepad.DPad.Up == ButtonState.Pressed;
                        case Direction.Right:
                            return self.gamepad.ThumbSticks.Left.X > threshold || self.gamepad.DPad.Right == ButtonState.Pressed;
                        case Direction.Down:
                            return self.gamepad.ThumbSticks.Left.Y < -threshold || self.gamepad.DPad.Down == ButtonState.Pressed;
                        case Direction.Left:
                            return self.gamepad.ThumbSticks.Left.X < -threshold || self.gamepad.DPad.Left == ButtonState.Pressed;
                    }

                    Util.ReleaseAssert(false);
                    return false;
                }

                switch (input)
                {
                    case Input.Up:
                        return keyboard.IsKeyDown(Keys.W) || keyboard.IsKeyDown(Keys.Up) || gamepadDirection(Direction.Up);
                    case Input.Down:
                        return keyboard.IsKeyDown(Keys.S) || keyboard.IsKeyDown(Keys.Down) || gamepadDirection(Direction.Down);
                    case Input.Left:
                        return keyboard.IsKeyDown(Keys.A) || keyboard.IsKeyDown(Keys.Left) ||gamepadDirection(Direction.Left);
                    case Input.Right:
                        return keyboard.IsKeyDown(Keys.D) || keyboard.IsKeyDown(Keys.Right) ||gamepadDirection(Direction.Right);
                }

                Util.ReleaseAssert(false);
                return false;
            }
        }
    }
}