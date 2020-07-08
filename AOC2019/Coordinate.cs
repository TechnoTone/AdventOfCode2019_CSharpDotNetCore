using System;

namespace AOC2019
{
    public struct Coordinate
    {
        private int x;
        private int y;

        public Coordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int distance => Math.Abs(x) + Math.Abs(y);

        public void Move(Direction direction)
        {
            switch (direction)
            {
                case Direction.Up:
                    x--;
                    break;
                case Direction.Down:
                    x++;
                    break;
                case Direction.Left:
                    y--;
                    break;
                case Direction.Right:
                    y++;
                    break;
            }
        }
    }
}
